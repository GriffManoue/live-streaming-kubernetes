import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { StreamService } from '../../services/stream.service';
import { LiveStream } from '../../models/stream/stream';
import { RouterModule } from '@angular/router';

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
      this.streams = streams.slice(0, 10);
    });
  }
}