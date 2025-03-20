import { Routes } from '@angular/router';
import { RecommendationComponent } from './features/recommendation/recommendation.component';
import { LoginComponent } from './features/login/login.component';

export const routes: Routes = [
  { path: '', component: LoginComponent},
  { path: 'home', component: RecommendationComponent }
];
