import { Routes } from '@angular/router';
import { LoginComponent } from './features/login/login.component';
import { AuthGuard } from './guards/auth.guard';
import { RegisterComponent } from './features/register/register.component';

export const routes: Routes = [
  // Keep non-lazy routes as they are
  { path: '', component: LoginComponent},
  { path: 'register', component: RegisterComponent },
  { path: 'login', component: LoginComponent },

  // Lazy load feature components
  {
    path: 'home',
    loadComponent: () => import('./features/recommendation/recommendation.component').then(m => m.RecommendationComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'stream/:streamId',
    loadComponent: () => import('./features/stream/stream.component').then(m => m.StreamComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'settings/:id',
    loadComponent: () => import('./features/settings/settings.component').then(m => m.SettingsComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'profile/:id',
    loadComponent: () => import('./features/profile/profile.component').then(m => m.ProfileComponent),
    canActivate: [AuthGuard]
  },

  { path: '**', redirectTo: 'home'} // Redirect to home 
];
