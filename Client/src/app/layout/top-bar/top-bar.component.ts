import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { MenubarModule } from 'primeng/menubar';
import { InputTextModule } from 'primeng/inputtext';
import { AvatarModule } from 'primeng/avatar';
import { MenuItem } from 'primeng/api';
import { StreamCategories } from '../../models/enums/stream-categories';
import { ToggleSwitchModule } from 'primeng/toggleswitch';
import { FormsModule } from '@angular/forms';
import { LoginService } from '../../services/login.service';

@Component({
  selector: 'app-top-bar',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule, 
    ButtonModule,
    MenubarModule,
    InputTextModule,
    AvatarModule,
    ToggleSwitchModule,
    FormsModule
  ],
  templateUrl: './top-bar.component.html',
  styleUrl: './top-bar.component.css'
})
export class TopBarComponent {
  isLoggedIn: boolean = false;
  userId: string = '';
  
  items: MenuItem[] = []; // Menu items for the top bar
  isDarkMode: boolean; // Initialize dark mode state
  
  
  constructor(private loginService: LoginService) {
    this.loginService.isLoggedIn.subscribe(loggedIn => {
      this.isLoggedIn = loggedIn;
      if (this.isLoggedIn) {
        // Get the user ID from localStorage
        const user = JSON.parse(localStorage.getItem('user') || '{}');
        this.userId = user.id || '';
        
        this.items = [
          {
            label: 'Home',
            icon: 'pi pi-video',
            routerLink: '/home',
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
    });

    // Check local storage for dark mode preference
    const darkMode = localStorage.getItem('darkMode');
    const isDarkMode = darkMode === 'true';
    this.isDarkMode = isDarkMode === true; // Convert string to boolean
    const element = document.querySelector('html');
    if (element) {
      // Apply dark mode class if preference is set
      element.classList.toggle('my-app-dark', isDarkMode);
    }
  }

  
toggleDarkMode() {
  const element = document.querySelector('html');
  if (element) {
    // Toggle the dark mode class on the html

  element.classList.toggle('my-app-dark');
  }
  // Update the dark mode state in local storage
  const isDarkMode = element?.classList.contains('my-app-dark');
  localStorage.setItem('darkMode', JSON.stringify(isDarkMode));
}
}
