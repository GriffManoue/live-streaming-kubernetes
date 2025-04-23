import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { StreamService } from '../../services/stream.service';
import { LiveStream } from '../../models/stream/stream';
import { RouterModule, ActivatedRoute } from '@angular/router';
import { forkJoin } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { StreamDbHandlerService } from '../../services/stream-db-handler.service';
import { ViewerService } from '../../services/viewer.service';

@Component({
  selector: 'app-recommendation',
  standalone: true,
  imports: [CommonModule, ButtonModule, CardModule, RouterModule],
  templateUrl: './recommendation.component.html',
  styleUrl: './recommendation.component.css'
})
export class RecommendationComponent implements OnInit {
  streams: LiveStream[] = [];

  constructor(
    private streamDbService: StreamDbHandlerService, 
    private streamService: StreamService,
    private viewerService: ViewerService,
    private route: ActivatedRoute
  ) { }

  ngOnInit(): void {
    this.route.queryParamMap.subscribe(params => {
      const showAll = params.get('all') === 'true';
      const selectedCategory = params.get('category');
      const filterByCategory = (streams: LiveStream[]) => {
        if (selectedCategory) {
          return streams.filter(s => s.streamCategory === selectedCategory);
        }
        return streams;
      };
      if (showAll) {
        // Show all streams
        this.streamDbService.getActiveStreams().subscribe(streams => {
          const filtered = filterByCategory(streams);
          const viewerCountObservables = filtered.map(s =>
            this.viewerService.getViewerCount(s.id).pipe(
              map(count => count as number),
              catchError(() => [0] as any)
            )
          );
          forkJoin(viewerCountObservables).subscribe(viewerCounts => {
            this.streams = filtered.map((s, i) => ({ ...s, currentViewers: viewerCounts[i] as number }));
          });
        });
      } else {
        // Show recommendations (default behavior)
        const userJson = localStorage.getItem('user');
        let userId: string | null = null;
        if (userJson) {
          try {
            const user = JSON.parse(userJson);
            userId = user.id;
          } catch {
            userId = null;
          }
        }
        if (!userId) {
          this.streams = [];
          return;
        }
        this.streamService.getRecommendations(userId, 10).subscribe(streams => {
          const filtered = filterByCategory(streams);
          const viewerCountObservables = filtered.map(s =>
            this.viewerService.getViewerCount(s.id).pipe(
              map(count => count as number),
              catchError(() => [0] as any)
            )
          );
          forkJoin(viewerCountObservables).subscribe(viewerCounts => {
            this.streams = filtered.map((s, i) => ({ ...s, currentViewers: viewerCounts[i] as number }));
          });
        });
      }
    });
  }
}