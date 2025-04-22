import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { RecommendationRoutingModule } from './recommendation-routing.module';

@NgModule({
  imports: [CommonModule, RouterModule, RecommendationRoutingModule],
})
export class RecommendationModule {}
