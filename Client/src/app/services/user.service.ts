import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ServiceBase } from './service-base';
import { FollowRequest } from '../models/user/follow-request';
import { User } from '../models/user/user';

@Injectable({
  providedIn: 'root'
})
export class UserService extends ServiceBase {

  constructor(http: HttpClient) {
    super(http );
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

  getFollowers(id: string): Observable<User[]> {
    return this.http.get<User[]>(`${this.apiUrl}/user/${id}/followers`);
  }

  getFollowing(id: string): Observable<User[]> {
    return this.http.get<User[]>(`${this.apiUrl}/user/${id}/following`);
  }

  followUser(request: FollowRequest): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/user/follow`, request);
  }

  unfollowUser(request: FollowRequest): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/user/unfollow`, request);
  }

}
