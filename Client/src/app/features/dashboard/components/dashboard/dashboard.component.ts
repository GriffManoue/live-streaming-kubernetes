import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../../core/services/auth.service';
import { User } from '../../../../shared/models/user.model';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  currentUser: User | null = null;
  navLinks = [
    { path: '/dashboard/recommended', label: 'Recommended', icon: 'recommend' },
    { path: '/dashboard/following', label: 'Following', icon: 'people' },
    { path: '/dashboard/profile', label: 'Profile', icon: 'person' }
  ];
  
  constructor(
    private router: Router,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
    });
  }

  logout(): void {
    this.authService.logout();
  }

  startStream(): void {
    this.router.navigate(['/streaming/create']);
  }
}
