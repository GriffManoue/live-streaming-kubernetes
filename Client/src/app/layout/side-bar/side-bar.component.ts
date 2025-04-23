import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PanelModule } from 'primeng/panel';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { AvatarModule } from 'primeng/avatar';
import { LoginService } from '../../services/login.service';
import { User } from '../../models/user/user';
import { Router } from '@angular/router';
import { FollowRequest } from '../../models/user/follow-request';
import { MessageService } from 'primeng/api';
import { forkJoin, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { LiveStream } from '../../models/stream/stream';
import { StreamDbHandlerService } from '../../services/stream-db-handler.service';
import { ViewerService } from '../../services/viewer.service';
import { UserDbHandlerService } from '../../services/user-db-handler.service';
import { FollowerService } from '../../services/follower.service';


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

  // Use array of { user, stream } objects
  streamers: { user: User, stream: LiveStream | null }[] = [];
  followedStreamerIds: Set<string> = new Set();
  private viewerInterval: any;

  constructor(
    private loginService: LoginService,
    private followerService: FollowerService,
    private router: Router,
    private messageService: MessageService,
    private streamDbService: StreamDbHandlerService,
    private viewerService: ViewerService
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
      this.followerService.getFollowing(user.id).subscribe((following: User[]) => {
        const streamRequests = following.map(f =>
          this.streamDbService.getStreamByUserId(f.id).pipe(
            catchError(() => of(null))
          )
        );
        forkJoin(streamRequests).subscribe((streams: (LiveStream | null)[]) => {
          const streamerPairs = following.map((f, idx) => ({
            user: f,
            stream: streams[idx]
          }));
          // For live streams, fetch viewer counts
          const viewerCountRequests = streamerPairs.map(pair =>
            pair.stream && pair.stream.streamUrl
              ? this.viewerService.getViewerCount(pair.stream.id).pipe(
                  map(count => ({ ...pair, stream: { ...pair.stream!, currentViewers: count } }))
                )
              : of(pair)
          );
          forkJoin(viewerCountRequests).subscribe((finalArr) => {
            this.streamers = finalArr.filter((s: any) => s.user && typeof s.user.username === 'string' && s.user.username.length > 0);
            this.followedStreamerIds = new Set(following.map(f => f.id));
          });
        });
      });
    }
  }

  followStreamer(streamer: User) {
    const user = JSON.parse(localStorage.getItem('user') || '{}');
    if (!user || !user.id) return;
    const request: FollowRequest = { followerId: user.id, followingId: streamer.id };
    this.followerService.followUser(request).subscribe({
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
    this.followerService.unfollowUser(request).subscribe({
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
    this.streamDbService.getStreamByUserId(streamer.id).subscribe({
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
