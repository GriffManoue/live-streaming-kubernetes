import { Component, ElementRef, OnInit, OnDestroy, ViewChild, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { TagModule } from 'primeng/tag';
import { AvatarModule } from 'primeng/avatar';

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
  streamData: any = {
    id: '123e4567-e89b-12d3-a456-426614174000',
    streamName: 'Live Coding Session',
    streamDescription: 'Building a live streaming platform with .NET and Angular',
    streamCategory: 'Technology',
    username: 'coder789',
    createdAt: new Date(),
    viewerCount: 512,
    isActive: true
  };
  
  loading: boolean = true;
  error: string | null = null;
  player:any = null;

  constructor(private route: ActivatedRoute) {}

  ngOnInit(): void {
    this.streamId = this.route.snapshot.paramMap.get('streamId');
    
    // In a real implementation, you would fetch the stream data from your API
    // Example:
    // this.streamService.getStream(this.streamId).subscribe({
    //   next: (data) => {
    //     this.streamData = data;
    //     this.loading = false;
    //   },
    //   error: (err) => {
    //     this.error = 'Could not load stream';
    //     this.loading = false;
    //   }
    // });
    
    // For demo purposes, simulate API call with timeout
    setTimeout(() => {
      this.loading = false;
    }, 1000);
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
  getCategorySeverity(category: string): "success" | "info" | "warn" | "danger" | "secondary" | "contrast" | undefined {
    const categoryMap: { [key: string]: "success" | "info" | "warn" | "danger" | "secondary" | "contrast" } = {
      'Gaming': 'info',
      'Music': 'success',
      'Sports': 'warn',
      'Art': 'info',
      'Cooking': 'success',
      'Education': 'info',
      'Travel': 'warn',
      'TalkShows': 'info',
      'News': 'danger',
      'Technology': 'secondary',
      'Science': 'secondary',
      'HealthAndFitness': 'success'
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
        const streamUrl = 'http://localhost:8080/hls/testkey.m3u8';
        //const streamUrl = 'https://storage.googleapis.com/shaka-demo-assets/angel-one-hls/hls.m3u8';
        //const streamUrl = 'https://demo.unified-streaming.com/k8s/features/stable/video/tears-of-steel/tears-of-steel.ism/.m3u8';
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
