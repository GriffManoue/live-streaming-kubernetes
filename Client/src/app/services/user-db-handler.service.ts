import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ServiceBase } from './service-base';
import { FollowRequest } from '../models/user/follow-request';
import { User } from '../models/user/user';

@Injectable({
  providedIn: 'root'
})
export class UserDbHandlerService extends ServiceBase {

  constructor(http: HttpClient) {
    super(http);
  }

  getUserById(id: string): Observable<User> {
    return this.http.get<User>(`${this.apiUrl}/user/${id}`);
  }

  getUserByUsername(username: string): Observable<User> {
    return this.http.get<User>(`${this.apiUrl}/user/username/${username}`);
  }

  updateUser(id: string, user: User): Observable<User> {
    return this.http.put<User>(`${this.apiUrl}/user/${id}`, user);
  }

  getUserByIdWithIncludes(id: string): Observable<User> {
    return this.http.get<User>(`${this.apiUrl}/user/includes/${id}`);
  }

  createUser(user: User): Observable<User> {
    return this.http.post<User>(`${this.apiUrl}/user`, user);
  }

  getUserByEmail(email: string): Observable<User> {
    return this.http.get<User>(`${this.apiUrl}/user/email/${email}`);
  }
}
