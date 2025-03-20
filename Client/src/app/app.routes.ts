import { Routes } from '@angular/router';
import { RecommendationComponent } from './features/recommendation/recommendation.component';
import { LoginComponent } from './features/login/login.component';
import { AuthGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', component: LoginComponent},
  { path: 'home', component: RecommendationComponent, canActivate: [AuthGuard] }
];
