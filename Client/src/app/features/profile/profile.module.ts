import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ProfileRoutingModule } from './profile-routing.module';

@NgModule({
  imports: [CommonModule, RouterModule, ProfileRoutingModule],
})
export class ProfileModule {}
