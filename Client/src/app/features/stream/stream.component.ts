import { Component, ElementRef, OnInit, OnDestroy, ViewChild, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { TagModule } from 'primeng/tag';
import { AvatarModule } from 'primeng/avatar';
import { StreamService } from '../../services/stream.service';
import { LiveStream } from '../../models/stream/stream';
import { StreamCategory } from '../../models/enums/stream-categories';
import { UserService } from '../../services/user.service';
import { FollowRequest } from '../../models/user/follow-request';
import { User } from '../../models/user/user';
import { MessageService } from 'primeng/api';
import { LoginService } from '../../services/login.service';

declare let shaka: any;

@Component({
  selector: 'app-stream',
  standalone: true,
  imports: [
    CommonModule,
    CardModule,
    ButtonModule,
    ProgressSpinnerModule,
    TagModule,
    AvatarModule,
    RouterModule
  ],
  templateUrl: './stream.component.html',
  styleUrl: './stream.component.css'
})
export class StreamComponent implements OnInit, OnDestroy, AfterViewInit {
  @ViewChild('videoPlayer') videoElement!: ElementRef;

  streamId: string | null = null;
  streamData: LiveStream | null = null;

  loading: boolean = true;
  error: string | null = null;
  player: any = null;
  isFollowing: boolean = false; 
  currentViewers: number = 0;
  private viewerInterval: any;

  constructor(
    private route: ActivatedRoute,
    private streamService: StreamService,
    private userService: UserService,
    private messageService: MessageService,
  ) { }

  ngOnInit(): void {
    this.streamId = this.route.snapshot.paramMap.get('streamId');

    if (this.streamId) {
      this.streamService.getStreamById(this.streamId).subscribe({
        next: (data) => {
          this.streamData = data;
          this.loading = false;
          // Join viewer
          var user: User = JSON.parse(localStorage.getItem('user') || '{}');
          if (!user.id) {
            this.error = 'User not logged in';
            return;
          }
          this.streamService.joinViewer(this.streamId!, user.id ).subscribe();
          // Start polling viewer count
          this.pollViewerCount();
        },
        error: (err) => {
          this.error = 'Could not load stream';
          this.loading = false;
        }
      });
    } else {
      this.error = 'Stream ID not provided';
      this.loading = false;
    }

    // Check if the user is following the streamer
    let user: User = JSON.parse(localStorage.getItem('user') || '{}');
    this.userService.getFollowing(user.id).subscribe({
      next: (following) => {
        this.isFollowing = following.some((f: User) => f.id === this.streamData?.userId);
      }
    });
      
  }

  ngAfterViewInit(): void {
    // Initialize Shaka Player
    this.initPlayer();
  }

  ngOnDestroy(): void {
    // Clean up the player when component is destroyed
    if (this.player) {
      this.player.destroy();
      this.player = null;
    }
    if (this.streamId) {
      var user: User = JSON.parse(localStorage.getItem('user') || '{}');
      if (!user.id) {
        this.error = 'User not logged in';
        return;
      }
      this.streamService.leaveViewer(this.streamId, user.id).subscribe();
    }
    if (this.viewerInterval) {
      clearInterval(this.viewerInterval);
    }
  }

  // Method to assign PrimeNG severity based on stream category
  getCategorySeverity(category: StreamCategory): "success" | "info" | "warn" | "danger" | "secondary" | "contrast" | undefined {
    const categoryMap: { [key in StreamCategory]: "success" | "info" | "warn" | "danger" | "secondary" | "contrast" } = {
      Gaming: 'info',
      Music: 'success',
      Sports: 'warn',
      Art: 'info',
      Cooking: 'success',
      Education: 'info',
      Travel: 'warn',
      TalkShows: 'info',
      News: 'danger',
      Technology: 'secondary',
      Science: 'secondary',
      HealthAndFitness: 'success',
      FashionAndBeauty: 'info',
      FoodAndDrink: 'success',
      PetsAndAnimals: 'warn',
      DiyAndCrafts: 'info'
    };

    return categoryMap[category] || 'info';
  }

  private async initPlayer(): Promise<void> {
    // Install polyfills
    shaka.polyfill.installAll();

    // Check browser support
    if (!shaka.Player.isBrowserSupported()) {
      this.error = 'Browser not supported for video playback!';
      return;
    }

    // Wait for DOM to be ready
    setTimeout(async () => {
      try {
        const video = this.videoElement.nativeElement;
        this.player = new shaka.Player(video);

        // Listen for errors
        this.player.addEventListener('error', this.onPlayerError.bind(this));
     
        // Load the stream
        await this.player.load(this.streamData?.streamUrl);
        console.log('Stream loaded successfully');
      } catch (error) {
        console.error('Error loading the stream:', error);
        this.error = 'Error loading the stream';
      }
    }, 1500);
  }

  private onPlayerError(event: any): void {
    console.error('Error code', event.detail.code, 'object', event.detail);
    this.error = 'Error playing the stream: ' + event.detail.code;
  }

  public followUser(): void {

    let user: User = JSON.parse(localStorage.getItem('user') || '{}');
    let followRequest: FollowRequest = {
      followerId: user.id,
      followingId: this.streamData?.userId || ''
    }

    console.log ('Follow request:', followRequest);
    this.userService.followUser(followRequest).subscribe({
      next: () => {
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Successfully followed user!' });
        this.isFollowing = true; // Update the following status
      },
      error: (err) => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Error following user!' });
      }
    });
  }
  public unfollowUser(): void {
    let user : User = JSON.parse(localStorage.getItem('user') || '{}');

    let followRequest: FollowRequest = {
      followerId: user.id,
      followingId: this.streamData?.userId || ''
    }

    console.log ('Unfollow request:', followRequest);
    this.userService.unfollowUser(followRequest).subscribe({
      next: () => {
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Successfully unfollowed user!' });
        this.isFollowing = false; // Update the following status
      },
      error: (err) => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Error unfollowing user!' });
      }
    });

  }

  public copyLinkToClipboard(): void {
    const link = this.streamData?.streamUrl || '';
    navigator.clipboard.writeText(link).then(() => {
      this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Stream link copied to clipboard!' });
    }).catch((err) => {
      this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Error copying link to clipboard!' });
    });
  }

  pollViewerCount(): void {
    if (!this.streamId) return;
    this.viewerInterval = setInterval(() => {
      this.streamService.getViewerCount(this.streamId!).subscribe(count => {
        this.currentViewers = count;
      });
    }, 5000); // Poll every 5 seconds
    // Initial fetch
    this.streamService.getViewerCount(this.streamId!).subscribe(count => {
      this.currentViewers = count;
    });
  }
}