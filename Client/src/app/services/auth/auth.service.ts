import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ServiceBase } from '../service-base';
import { AuthResult } from '../../models/auth/auth-result.model';
import { LoginRequest } from '../../models/auth/login-request.model';
import { RegisterRequest } from '../../models/auth/register-request.model';
import { ValidateTokenRequest } from '../../models/auth/validate-token-request.model';
import { RevokeTokenRequest } from '../../models/auth/revoke-token-request.model';
import { StreamTokenRequest } from '../../models/auth/stream-token-request.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService extends ServiceBase {

  constructor(http: HttpClient) {
    super(http);
  }

  register(request: RegisterRequest): Observable<AuthResult> {
    return this.post<AuthResult>('auth/register', request);
  }

  login(request: LoginRequest): Observable<AuthResult> {
    return this.post<AuthResult>('auth/login', request);
  }

  validateToken(request: ValidateTokenRequest): Observable<AuthResult> {
    return this.post<AuthResult>('auth/validate', request);
  }

  revokeToken(request: RevokeTokenRequest): Observable<void> {
    return this.post<void>('auth/revoke', request);
  }

  generateStreamToken(request: StreamTokenRequest): Observable<AuthResult> {
    return this.post<AuthResult>('auth/stream-token', request);
  }
}
