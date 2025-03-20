import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { MenubarModule } from 'primeng/menubar';
import { InputTextModule } from 'primeng/inputtext';
import { AvatarModule } from 'primeng/avatar';
import { MenuItem } from 'primeng/api';
import { StreamCategories } from '../../enums/stream-categories';

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
        icon: 'pi pi-video',
        routerLink: '/',
        routerLinkActiveOptions: { exact: true }
      },
      {
        label: 'Categories',
        icon: 'pi pi-list',
        routerLink: '/categories',
        routerLinkActiveOptions: { exact: true },
        items: Object.values(StreamCategories).map(category => ({
          label: category.name,
          icon: category.icon,
          routerLink: `/categories/${category.name.toLowerCase()}`,
          routerLinkActiveOptions: { exact: true }
        }))

      }
    ];
  }
}
