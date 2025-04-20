import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { StreamService } from '../../services/stream.service';
import { LiveStream } from '../../models/stream/stream';
import { RouterModule } from '@angular/router';
import { forkJoin } from 'rxjs';
import { map, catchError } from 'rxjs/operators';

@Component({
  selector: 'app-recommendation',
  standalone: true,
  imports: [CommonModule, ButtonModule, CardModule, RouterModule],
  templateUrl: './recommendation.component.html',
  styleUrl: './recommendation.component.css'
})
export class RecommendationComponent implements OnInit {
  streams: LiveStream[] = [];

  constructor(private streamService: StreamService) { }

  ngOnInit(): void {
    this.streamService.getActiveStreams().subscribe(streams => {
      const topStreams = streams.slice(0, 10);
      const viewerCountObservables = topStreams.map(s =>
        this.streamService.getViewerCount(s.id).pipe(
          map(count => count as number),
          catchError(() => [0] as any)
        )
      );
      forkJoin(viewerCountObservables).subscribe(viewerCounts => {
        this.streams = topStreams.map((s, i) => ({ ...s, currentViewers: viewerCounts[i] as number }));
      });
    });
  }
}