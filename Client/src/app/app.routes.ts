import { Routes } from '@angular/router';
import { StreamPlayerComponent } from './features/stream-player/stream-player.component';

export const routes: Routes = [
    {
        path: '',
        component: StreamPlayerComponent,
        title: 'Stream Player',
    }
];
