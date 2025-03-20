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

  private hasToken(): boolean {
    return !!localStorage.getItem('auth_token') || !!sessionStorage.getItem('auth_token');
  }

  login(token: string, rememberMe: boolean) {
    if (rememberMe) {
      localStorage.setItem('auth_token', token);
    } else {
      sessionStorage.setItem('auth_token', token);
    }
    this.loggedIn.next(true);
  }

  logout() {
    localStorage.removeItem('auth_token');
    sessionStorage.removeItem('auth_token');
    this.loggedIn.next(false);
  }
}
