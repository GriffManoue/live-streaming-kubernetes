import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ServiceBase } from '../service-base';
import { RegisterRequest } from '../../models/auth/register-request';
import { AuthResult } from '../../models/auth/auth-result';
import { LoginRequest } from '../../models/auth/login-request';

@Injectable({
  providedIn: 'root'
})
export class AuthService extends ServiceBase {

  constructor(http: HttpClient) {
    super(http);
  }

  register(request: RegisterRequest): Observable<AuthResult> {
    return this.http.post<AuthResult>(`${this.apiUrl}/api/auth/register`, request);
  }

  login(request: LoginRequest): Observable<AuthResult> {
    return this.http.post<AuthResult>(`${this.apiUrl}/api/auth/login`, request);
  }

  validateToken(token: string): Observable<AuthResult> {
    return this.http.post<AuthResult>(`${this.apiUrl}/api/auth/validate`, { token });
  }

  revokeToken(token: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/api/auth/revoke`, { token });
  }
}
