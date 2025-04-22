import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RecommendationComponent } from './recommendation.component';
import { AuthGuard } from '../../guards/auth.guard';

const routes: Routes = [
  {
    path: '',
    component: RecommendationComponent,
    canActivate: [AuthGuard]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RecommendationRoutingModule {}
