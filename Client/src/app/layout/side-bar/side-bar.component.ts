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

  constructor(
    private loginService: LoginService,
    private userService: UserService,
    private router: Router,
    private messageService: MessageService
  ) {}

  ngOnInit() {
    this.loginService.isLoggedIn.subscribe(loggedIn => {
      this.isLoggedIn = loggedIn;
      if (loggedIn) {
        const user = JSON.parse(localStorage.getItem('user') || '{}');
        if (user && user.id) {
          this.userService.getFollowing(user.id).subscribe((following: User[]) => {
            this.streamers = following;
          });
        }
      } else {
        this.streamers = [];
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

  watchStreamer(streamer: User) {
    this.router.navigate(['/stream', streamer.id]);
  }
}
