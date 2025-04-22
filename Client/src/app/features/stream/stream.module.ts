import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { StreamRoutingModule } from './stream-routing.module';

@NgModule({
  imports: [CommonModule, RouterModule, StreamRoutingModule],
})
export class StreamModule {}
