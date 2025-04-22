import { Routes } from '@angular/router';
import { LoginComponent } from './features/login/login.component';
import { AuthGuard } from './guards/auth.guard';
import { RegisterComponent } from './features/register/register.component';

export const routes: Routes = [
  // Keep non-lazy routes as they are
  { path: '', component: LoginComponent},
  { path: 'register', component: RegisterComponent },
  { path: 'login', component: LoginComponent },

  // Lazy load feature modules
  {
    path: 'home',
    loadChildren: () => import('./features/recommendation/recommendation.module').then(m => m.RecommendationModule)
  },
  {
    path: 'stream',
    loadChildren: () => import('./features/stream/stream.module').then(m => m.StreamModule)
  },
  {
    path: 'settings',
    loadChildren: () => import('./features/settings/settings.module').then(m => m.SettingsModule)
  },
  {
    path: 'profile',
    loadChildren: () => import('./features/profile/profile.module').then(m => m.ProfileModule)
  },

  { path: '**', redirectTo: 'home'} // Redirect to home 
];
