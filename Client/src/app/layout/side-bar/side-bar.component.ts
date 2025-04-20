import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PanelModule } from 'primeng/panel';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { AvatarModule } from 'primeng/avatar';
import { LoginService } from '../../services/login.service';
import { UserService } from '../../services/user.service';
import { User } from '../../models/user/user';
import { Router } from '@angular/router';
import { FollowRequest } from '../../models/user/follow-request';
import { MessageService } from 'primeng/api';
import { StreamService } from '../../services/stream.service';
import { forkJoin } from 'rxjs';
import { map, catchError } from 'rxjs/operators';


type LiveSidebarStreamer = User & {
  isLive: boolean;
  streamId?: string;
  currentViewers?: number;
};


@Component({
  selector: 'app-side-bar',
  standalone: true,
  imports: [
    CommonModule,
    PanelModule,
    CardModule,
    ButtonModule,
    AvatarModule
  ],
  templateUrl: './side-bar.component.html',
  styleUrl: './side-bar.component.css'
})
export class SideBarComponent implements OnInit, OnDestroy {
  isLoggedIn: boolean = false;

  streamers: LiveSidebarStreamer[] = [];
  followedStreamerIds: Set<string> = new Set();
  private viewerInterval: any;

  constructor(
    private loginService: LoginService,
    private userService: UserService,
    private router: Router,
    private messageService: MessageService,
    private streamService: StreamService
  ) {}

  ngOnInit() {
    this.loginService.isLoggedIn.subscribe(loggedIn => {
      this.isLoggedIn = loggedIn;
      if (loggedIn) {
        this.fetchSidebarStreamers();
        this.viewerInterval = setInterval(() => this.fetchSidebarStreamers(), 10000); // Poll every 10s
      } else {
        this.streamers = [];
        this.followedStreamerIds.clear();
        if (this.viewerInterval) {
          clearInterval(this.viewerInterval);
        }
      }
    });
  }

  ngOnDestroy() {
    if (this.viewerInterval) {
      clearInterval(this.viewerInterval);
    }
  }

  private fetchSidebarStreamers() {
    const user = JSON.parse(localStorage.getItem('user') || '{}');
    if (user && user.id) {
      this.userService.getFollowing(user.id).subscribe((following: User[]) => {
        const liveRequests = following.map(f =>
          this.streamService.getStreamByUserId(f.id).pipe(
            map(stream => ({
              ...f,
              isLive: !!stream?.streamUrl,
              streamId: stream?.id,
              currentViewers: 0
            }) as LiveSidebarStreamer),
            catchError(() => [{ ...f, isLive: false }] as any)
          )
        );
        forkJoin(liveRequests).subscribe((streamersArr: any[]) => {
          const streamers: LiveSidebarStreamer[] = streamersArr.map((sArr: any) => Array.isArray(sArr) ? sArr[0] : sArr);
          const viewerCountRequests = streamers.map(s =>
            s.isLive && s.streamId ? this.streamService.getViewerCount(s.streamId).pipe(
              map(count => ({ ...s, currentViewers: count } as LiveSidebarStreamer)),
              catchError(() => [{ ...s, currentViewers: 0 }] as any)
            ) : [Promise.resolve(s)]
          );
          forkJoin(viewerCountRequests).subscribe((finalArr: any[]) => {
            this.streamers = finalArr.map((sArr: any) => Array.isArray(sArr) ? sArr[0] : sArr);
            this.followedStreamerIds = new Set(following.map(f => f.id));
          });
        });
      });
      console.log(this.streamers);
    }
  }

  followStreamer(streamer: User) {
    const user = JSON.parse(localStorage.getItem('user') || '{}');
    if (!user || !user.id) return;
    const request: FollowRequest = { followerId: user.id, followingId: streamer.id };
    this.userService.followUser(request).subscribe({
      next: () => {
        this.messageService.add({ severity: 'success', summary: 'Success', detail: `You are now following ${streamer.username}` });
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: `Failed to follow ${streamer.username}` });
      }
    });
  }

  unfollowStreamer(streamer: User) {
    const user = JSON.parse(localStorage.getItem('user') || '{}');
    if (!user || !user.id) return;
    const request: FollowRequest = { followerId: user.id, followingId: streamer.id };
    this.userService.unfollowUser(request).subscribe({
      next: () => {
        this.messageService.add({ severity: 'success', summary: 'Success', detail: `You have unfollowed ${streamer.username}` });
        this.followedStreamerIds.delete(streamer.id);
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: `Failed to unfollow ${streamer.username}` });
      }
    });
  }

  watchStreamer(streamer: User) {
    this.streamService.getStreamByUserId(streamer.id).subscribe({
      next: (stream) => {
        if (stream) {
          this.router.navigate(['/stream', stream.id]);
        } else {
          this.messageService.add({ severity: 'warn', summary: 'No Stream', detail: `${streamer.username} is not live` });
        }
      }
    });
  }
}
