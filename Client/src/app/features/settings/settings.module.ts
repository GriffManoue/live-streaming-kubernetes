import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { SettingsRoutingModule } from './settings-routing.module';

@NgModule({
  imports: [CommonModule, RouterModule, SettingsRoutingModule],
})
export class SettingsModule {}
