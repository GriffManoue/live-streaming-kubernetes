import { Routes } from '@angular/router';
import { LoginComponent } from './features/login/login.component';
import { AuthGuard } from './guards/auth.guard';
import { RegisterComponent } from './features/register/register.component';
// Remove direct component imports for lazy-loaded routes
// import { RecommendationComponent } from './features/recommendation/recommendation.component';
// import { StreamComponent } from './features/stream/stream.component';
// import { SettingsComponent } from './features/settings/settings.component';
// import { ProfileComponent } from './features/profile/profile.component';

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
    canActivate: [AuthGuard] // Apply AuthGuard here as well
  },

  // Wildcard route remains the same
  { path: '**', redirectTo: 'home'} // Redirect to home (or login if preferred) for unknown routes
  //todo : add 404 page
];
