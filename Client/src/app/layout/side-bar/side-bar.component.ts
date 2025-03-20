import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PanelModule } from 'primeng/panel';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { ScrollPanelModule } from 'primeng/scrollpanel';

@Component({
  selector: 'app-side-bar',
  standalone: true,
  imports: [
    CommonModule,
    PanelModule,
    CardModule,
    ButtonModule,
    ScrollPanelModule
  ],
  templateUrl: './side-bar.component.html',
  styleUrl: './side-bar.component.css'
})
export class SideBarComponent implements OnInit {
  streams: any[] = [];

  ngOnInit() {
    // Mock data - replace with actual API call to your stream service
    this.streams = [
      { id: 1, title: 'Gaming Stream', user: 'gamer123', viewers: 1250, category: 'Gaming' },
      { id: 2, title: 'Music Session', user: 'musician456', viewers: 750, category: 'Music' },
      { id: 3, title: 'Coding Tutorial', user: 'coder789', viewers: 500, category: 'Technology' },
      { id: 4, title: 'Art Creation', user: 'artist101', viewers: 320, category: 'Art' },
      { id: 5, title: 'Cooking Show', user: 'chef555', viewers: 890, category: 'Food' }
    ];
  }
}
