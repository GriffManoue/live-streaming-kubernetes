import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { StreamService } from '../../services/stream.service';
import { LiveStream } from '../../models/stream/stream';
import { RouterModule } from '@angular/router';
import { forkJoin } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { User } from '../../models/user/user';
import { UserService } from '../../services/user.service';
import { LoginService } from '../../services/login.service';

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
    // Parse user from localStorage and extract id
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
      const viewerCountObservables = streams.map(s =>
        this.streamService.getViewerCount(s.id).pipe(
          map(count => count as number),
          catchError(() => [0] as any)
        )
      );
      forkJoin(viewerCountObservables).subscribe(viewerCounts => {
        this.streams = streams.map((s, i) => ({ ...s, currentViewers: viewerCounts[i] as number }));
      });
    });
  }
}