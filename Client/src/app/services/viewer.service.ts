import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ServiceBase } from './service-base';

@Injectable({
  providedIn: 'root'
})
export class ViewerService extends ServiceBase {
  constructor(http: HttpClient) {
    super(http);
  }

  getViewerCount(streamId: string): Observable<number> {
    return this.http.get<number>(`${this.apiUrl}/viewer/${streamId}/count`);
  }

  joinStream(streamId: string, viewerId: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/viewer/${streamId}/join?viewerId=${viewerId}`, {});
  }

  leaveStream(streamId: string, viewerId: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/viewer/${streamId}/leave?viewerId=${viewerId}`, {});
  }
}
