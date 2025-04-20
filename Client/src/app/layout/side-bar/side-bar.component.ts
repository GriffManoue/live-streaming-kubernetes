import { Component, Input, OnInit } from '@angular/core';
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
export class SideBarComponent implements OnInit {
  isLoggedIn: boolean = false;

  streamers: User[] = [];
  followedStreamerIds: Set<string> = new Set();

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
        const user = JSON.parse(localStorage.getItem('user') || '{}');
        if (user && user.id) {
          this.userService.getFollowing(user.id).subscribe((following: User[]) => {
            this.streamers = following;
            this.followedStreamerIds = new Set(following.map(f => f.id));
          });
        }
      } else {
        this.streamers = [];
        this.followedStreamerIds.clear();
      }
    });
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
