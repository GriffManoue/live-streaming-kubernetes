import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardRoutingModule } from './dashboard-routing.module';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { ProfileComponent } from './components/profile/profile.component';
import { RecommendedStreamersComponent } from './components/recommended-streamers/recommended-streamers.component';
import { FollowingComponent } from './components/following/following.component';
import { ProfileEditComponent } from './components/profile-edit/profile-edit.component';
import { UserStatsComponent } from './components/user-stats/user-stats.component';
import { StreamerCardComponent } from './components/streamer-card/streamer-card.component';
import { CategoryFilterComponent } from './components/category-filter/category-filter.component';
import { FollowingListComponent } from './components/following-list/following-list.component';
import { SharedModule } from '../../shared/shared.module';

@NgModule({
  declarations: [
    DashboardComponent,
    ProfileComponent,
    RecommendedStreamersComponent,
    FollowingComponent,
    ProfileEditComponent,
    UserStatsComponent,
    StreamerCardComponent,
    CategoryFilterComponent,
    FollowingListComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    DashboardRoutingModule
  ]
})
export class DashboardModule { }
