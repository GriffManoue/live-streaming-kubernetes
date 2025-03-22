import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UserDto } from '../../models/user/user-dto';
import { ServiceBase } from '../service-base';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UserService extends ServiceBase {

  constructor(http: HttpClient) {
    // Use the API URL from environment config or default to relative path
    super(http, environment.apiUrl ? `${environment.apiUrl}/user` : '/api/user');
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
