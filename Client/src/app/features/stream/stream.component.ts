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

  loading: boolean = true;
  error: string | null = null;
  player: any = null;

  constructor(
    private route: ActivatedRoute,
    private streamService: StreamService
  ) { }

  ngOnInit(): void {
    this.streamId = this.route.snapshot.paramMap.get('streamId');

    if (this.streamId) {
      this.streamService.getStreamById(this.streamId).subscribe({
        next: (data) => {
          this.streamData = data;
          this.loading = false;
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
    // Initialize Shaka Player
    this.initPlayer();
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

        // In a real implementation, you would use the actual stream URL
        // const streamUrl = `https://your-streaming-server.com/hls/${this.streamId}/index.m3u8`;

        // For demo, using a sample HLS stream
        const streamUrl = this.streamData?.streamUrl;
        //const streamUrl = 'http://localhost:8080/hls/testkey.m3u8';
        //const streamUrl = 'https://storage.googleapis.com/shaka-demo-assets/angel-one-hls/hls.m3u8';
        // Load the stream
        await this.player.load(streamUrl);
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
}