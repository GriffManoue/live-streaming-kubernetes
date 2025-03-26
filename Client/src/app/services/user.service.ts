import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ServiceBase } from './service-base';
import { FollowRequest } from '../models/user/follow-request';

@Injectable({
  providedIn: 'root'
})
export class UserService extends ServiceBase {

  constructor(http: HttpClient) {
    super(http );
  }

  getUserById(id: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/api/user/${id}`);
  }

  getUserByUsername(username: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/api/user/username/${username}`);
  }

  updateUser(id: string, user: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/api/user/${id}`, user);
  }

  getFollowers(id: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/api/user/${id}/followers`);
  }

  getFollowing(id: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/api/user/${id}/following`);
  }

  followUser(request: FollowRequest): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/api/user/follow`, request);
  }

  unfollowUser(request: FollowRequest): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/api/user/unfollow`, request);
  }

}
