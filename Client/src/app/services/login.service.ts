import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LoginService {
  private loggedIn = new BehaviorSubject<boolean>(this.hasToken());

  get isLoggedIn() {
    return this.loggedIn.asObservable();
  }

  hasToken(): boolean {
    return !!localStorage.getItem('auth_token') || !!sessionStorage.getItem('auth_token');
  }

  login(token: string, rememberMe: boolean, userId: string = '') {
    if (rememberMe) {
      localStorage.setItem('auth_token', token);
    } else {
      sessionStorage.setItem('auth_token', token);
    }
    
    // Store user information including ID
    const user = { id: userId || 'user-1' }; // Default ID if none provided
    localStorage.setItem('user', JSON.stringify(user));
    
    this.loggedIn.next(true);
  }

  logout() {
    localStorage.removeItem('auth_token');
    sessionStorage.removeItem('auth_token');
    localStorage.removeItem('user');
    this.loggedIn.next(false);
  }
}
