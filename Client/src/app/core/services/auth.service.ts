import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, map } from 'rxjs';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';
import { User } from '../../shared/models/user.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject: BehaviorSubject<User | null>;
  public currentUser$: Observable<User | null>;
  private tokenKey = 'jwt_token';

  constructor(
    private http: HttpClient,
    private router: Router
  ) {
    const storedUser = localStorage.getItem('currentUser');
    this.currentUserSubject = new BehaviorSubject<User | null>(
      storedUser ? JSON.parse(storedUser) : null
    );
    this.currentUser$ = this.currentUserSubject.asObservable();
  }

  public get currentUserValue(): User | null {
    return this.currentUserSubject.value;
  }

  login(username: string, password: string): Observable<User> {
    return this.http.post<any>(`${environment.apiUrl}/auth/login`, { username, password })
      .pipe(map(response => {
        // Store user details and jwt token in local storage to keep user logged in
        const user: User = {
          id: response.id,
          username: response.username,
          email: response.email,
          token: response.token
        };
        
        localStorage.setItem('currentUser', JSON.stringify(user));
        localStorage.setItem(this.tokenKey, response.token);
        this.currentUserSubject.next(user);
        return user;
      }));
  }

  register(username: string, email: string, password: string): Observable<User> {
    return this.http.post<any>(`${environment.apiUrl}/auth/register`, { username, email, password })
      .pipe(map(response => {
        // Return the response from registration
        const user: User = {
          id: response.id,
          username: response.username,
          email: response.email
        };
        
        // After registration, we'll automatically log the user in
        this.login(username, password).subscribe();
        
        return user;
      }));
  }

  logout(): void {
    // Remove user from local storage and set current user to null
    localStorage.removeItem('currentUser');
    localStorage.removeItem(this.tokenKey);
    this.currentUserSubject.next(null);
    this.router.navigate(['/auth/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }
}
