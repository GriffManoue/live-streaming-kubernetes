import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { StreamComponent } from './stream.component';
import { AuthGuard } from '../../guards/auth.guard';

const routes: Routes = [
  {
    path: ':streamId',
    component: StreamComponent,
    canActivate: [AuthGuard]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class StreamRoutingModule {}
