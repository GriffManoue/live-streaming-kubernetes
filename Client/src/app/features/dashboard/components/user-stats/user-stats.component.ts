import { Component, Input, OnInit } from '@angular/core';

interface UserStats {
  followersCount: number;
  followingCount: number;
  totalStreamTime: number;
  totalViews: number;
  averageViewers: number;
}

@Component({
  selector: 'app-user-stats',
  templateUrl: './user-stats.component.html',
  styleUrls: ['./user-stats.component.scss']
})
export class UserStatsComponent implements OnInit {
  @Input() userId: string | undefined;
  
  stats: UserStats = {
    followersCount: 0,
    followingCount: 0,
    totalStreamTime: 0,
    totalViews: 0,
    averageViewers: 0
  };
  
  loading = true;
  
  constructor() { }

  ngOnInit(): void {
    this.loadStats();
  }

  private loadStats(): void {
    // In a real application, this would call a service method to get the user stats
    // For now, we'll just simulate loading stats with mock data
    setTimeout(() => {
      this.stats = {
        followersCount: 125,
        followingCount: 45,
        totalStreamTime: 3600, // in seconds
        totalViews: 1250,
        averageViewers: 35
      };
      this.loading = false;
    }, 1000);
  }

  formatTime(seconds: number): string {
    const hours = Math.floor(seconds / 3600);
    const minutes = Math.floor((seconds % 3600) / 60);
    
    return `${hours}h ${minutes}m`;
  }
}
