import { Routes } from '@angular/router';
import { RecommendationComponent } from './features/recommendation/recommendation.component';
import { LoginComponent } from './features/login/login.component';
import { AuthGuard } from './guards/auth.guard';
import { RegisterComponent } from './features/register/register.component';
import { StreamComponent } from './features/stream/stream.component';

export const routes: Routes = [
  { path: '', component: LoginComponent},
  { path: 'home', component: RecommendationComponent, canActivate: [AuthGuard] },
  { path: 'register', component: RegisterComponent },
  { path: 'login', component: LoginComponent },
  {path: 'stream/:streamId', component: StreamComponent }, //canActivate: [AuthGuard]
];
