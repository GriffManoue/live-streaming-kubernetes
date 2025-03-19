import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { MenubarModule } from 'primeng/menubar';
import { InputTextModule } from 'primeng/inputtext';
import { AvatarModule } from 'primeng/avatar';
import { MenuItem } from 'primeng/api';

@Component({
  selector: 'app-top-bar',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule, 
    ButtonModule,
    MenubarModule,
    InputTextModule,
    AvatarModule
  ],
  templateUrl: './top-bar.component.html',
  styleUrl: './top-bar.component.css'
})
export class TopBarComponent {
  items: MenuItem[];
  
  constructor() {
    this.items = [
      {
        label: 'Home',
        icon: 'pi pi-home',
        routerLink: '/'
      },
      {
        label: 'Live Streams',
        icon: 'pi pi-video',
        routerLink: '/streams'
      },
      {
        label: 'Categories',
        icon: 'pi pi-list',
        routerLink: '/categories'
      }
    ];
  }
}
