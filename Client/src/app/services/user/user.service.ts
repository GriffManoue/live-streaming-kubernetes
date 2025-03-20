import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UserDto } from '../../models/user/user-dto';
import { ServiceBase } from '../service-base';

@Injectable({
  providedIn: 'root'
})
export class UserService extends ServiceBase {

  constructor(http: HttpClient) {
    super(http, 'http://user-service.default.svc.cluster.local/api' );
  }

  getUserById(id: string): Observable<UserDto> {
    return this.get<UserDto>(`user/${id}`);
  }

  getUserByUsername(username: string): Observable<UserDto> {
    return this.get<UserDto>(`user/username/${username}`);
  }

  updateUser(id: string, user: UserDto): Observable<UserDto> {
    return this.put<UserDto>(`user/${id}`, user);
  }

  getFollowers(id: string): Observable<UserDto[]> {
    return this.get<UserDto[]>(`user/${id}/followers`);
  }

  getFollowing(id: string): Observable<UserDto[]> {
    return this.get<UserDto[]>(`user/${id}/following`);
  }

  followUser(followerId: string, followingId: string): Observable<void> {
    return this.post<void>('user/follow', { followerId, followingId });
  }

  unfollowUser(followerId: string, followingId: string): Observable<void> {
    return this.post<void>('user/unfollow', { followerId, followingId });
  }
}
