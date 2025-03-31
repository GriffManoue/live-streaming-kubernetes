import { Component, ElementRef, OnInit, OnDestroy, ViewChild, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { TagModule } from 'primeng/tag';
import { AvatarModule } from 'primeng/avatar';
import { StreamService } from '../../services/stream.service';
import { UserService } from '../../services/user.service';
import { LiveStream } from '../../models/stream/stream';
import { StreamCategoryKey } from '../../models/enums/stream-categories';

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
  username: string = 'Unknown user';
  userInitial: string = 'U';

  loading: boolean = true;
  error: string | null = null;
  player: any = null;

  constructor(
    private route: ActivatedRoute,
    private streamService: StreamService,
    private userService: UserService
  ) { }

  ngOnInit(): void {
    this.streamId = this.route.snapshot.paramMap.get('streamId');

    if (this.streamId) {
      this.streamService.getStreamById(this.streamId).subscribe({
        next: (data) => {
          this.streamData = data;
          this.loading = false;
          
          // Fetch username from UserService using the stream's userId
          if (this.streamData.userId) {
            this.userService.getUserById(this.streamData.userId).subscribe({
              next: (user) => {
                this.username = user.username || 'Unknown user';
                this.userInitial = this.username.charAt(0).toUpperCase();
              },
              error: (err) => {
                console.error('Error fetching user:', err);
                // Keep default values for username and userInitial
              }
            });
          }
          
          // Initialize player after data is loaded
          setTimeout(() => {
            if (this.videoElement) {
              this.initPlayer();
            }
          }, 0);
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
  }

  ngAfterViewInit(): void {
    // Only initialize the player when we have both the video element and stream data
    if (this.streamData?.streamUrl) {
      this.initPlayer();
    }
  }

  ngOnDestroy(): void {
    // Clean up the player when component is destroyed
    if (this.player) {
      this.player.destroy();
      this.player = null;
    }
  }

  // Method to assign PrimeNG severity based on stream category
  getCategorySeverity(category: StreamCategoryKey): "success" | "info" | "warn" | "danger" | "secondary" | "contrast" | undefined {
    const categoryMap: { [key in StreamCategoryKey]: "success" | "info" | "warn" | "danger" | "secondary" | "contrast" } = {
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
    // Check if shaka is available
    if (typeof shaka === 'undefined') {
      this.error = 'Video player library not loaded!';
      return;
    }

    // Install polyfills
    shaka.polyfill.installAll();

    // Check browser support
    if (!shaka.Player.isBrowserSupported()) {
      this.error = 'Browser not supported for video playback!';
      return;
    }

    // Wait for DOM to be ready and ensure video element exists
    setTimeout(async () => {
      try {
        if (!this.videoElement) {
          this.error = 'Video player element not found';
          return;
        }

        const video = this.videoElement.nativeElement;
        if (!video) {
          this.error = 'Video element reference is invalid';
          return;
        }

        this.player = new shaka.Player(video);

        // Listen for errors
        this.player.addEventListener('error', this.onPlayerError.bind(this));
     
        // Ensure we have a stream URL before attempting to load
        if (!this.streamData?.streamUrl) {
          this.error = 'Stream URL not available';
          return;
        }

        // Load the stream
        const streamUrl = "http://localhost:8080/hls/d935b49a-af65-4e1d-bb7a-00ecb0565371.m3u8";
        await this.player.load(streamUrl);
        console.log('Stream loaded successfully');
      } catch (error) {
        console.error('Error loading the stream:', error);
        this.error = 'Error loading the stream: ' + (error instanceof Error ? error.message : String(error));
      }
    }, 1000);
  }

  private onPlayerError(event: any): void {
    console.error('Error code', event.detail.code, 'object', event.detail);
    this.error = 'Error playing the stream: ' + event.detail.code;
  }
}