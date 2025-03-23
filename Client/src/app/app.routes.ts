import { Routes } from '@angular/router';
import { RecommendationComponent } from './features/recommendation/recommendation.component';
import { LoginComponent } from './features/login/login.component';
import { AuthGuard } from './guards/auth.guard';
import { RegisterComponent } from './features/register/register.component';
import { StreamComponent } from './features/stream/stream.component';
import { SettingsComponent } from './features/settings/settings.component';

export const routes: Routes = [
  { path: '', component: LoginComponent},
  { path: 'home', component: RecommendationComponent, canActivate: [AuthGuard] },
  { path: 'register', component: RegisterComponent },
  { path: 'login', component: LoginComponent },
  { path: 'stream/:streamId', component: StreamComponent }, //canActivate: [AuthGuard]
  { path: 'settings/:id', component: SettingsComponent}, // Lazy load settings component
  { path: '**', redirectTo: ''} // Redirect to home if no other route matches
  //todo : add 404 page
  //todo: lazy load other components
];
