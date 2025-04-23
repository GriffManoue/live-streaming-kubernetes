import { HttpClient } from '@angular/common/http';
import { ServiceBase } from './service-base';
import { Observable } from 'rxjs';
import { User } from '../models/user/user';
import { Injectable } from '@angular/core';
import { FollowRequest } from '../models/user/follow-request';

@Injectable({
  providedIn: 'root'
})
export class FollowerService extends ServiceBase {

  constructor(http: HttpClient) {
    super(http);
  }

  getFollowers(userId: string): Observable<User[]> {
    return this.http.get<User[]>(`${this.apiUrl}/follower/${userId}/followers`);
  }

  getFollowing(userId: string): Observable<User[]> {
    return this.http.get<User[]>(`${this.apiUrl}/follower/${userId}/following`);
  }

  followUser(request: FollowRequest): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/follower/follow`, request);
  }

  unfollowUser(request: FollowRequest): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/follower/unfollow`, request);
  }
}

