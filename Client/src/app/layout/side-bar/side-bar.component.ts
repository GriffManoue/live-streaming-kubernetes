import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PanelModule } from 'primeng/panel';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { AvatarModule } from 'primeng/avatar';

@Component({
  selector: 'app-side-bar',
  standalone: true,
  imports: [
    CommonModule,
    PanelModule,
    CardModule,
    ButtonModule,
    AvatarModule
  ],
  templateUrl: './side-bar.component.html',
  styleUrl: './side-bar.component.css'
})
export class SideBarComponent implements OnInit {
  streamers: any[] = [];

  ngOnInit() {
    // Mock data - replace with actual API call to your streamer service
    this.streamers = [
      { id: 1, name: 'gamer123', followers: 1250, category: 'Gaming', isLive: true },
      { id: 2, name: 'musician456', followers: 750, category: 'Music', isLive: false },
      { id: 3, name: 'coder789', followers: 500, category: 'Technology', isLive: true },
      { id: 4, name: 'artist101', followers: 320, category: 'Art', isLive: false },
      { id: 5, name: 'chef555', followers: 890, category: 'Food', isLive: true }
    ];
  }
}
