import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ServiceBase } from './service-base';
import { HttpClient } from '@angular/common/http';
import { LiveStream } from '../models/stream/stream';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class StreamService extends ServiceBase {

  constructor(http: HttpClient) {
    super(http);
  }

  generateStreamKey(id: string): Observable<string> {
    return this.http.post<{ streamKey: string }>(`${this.apiUrl}/stream/${id}/generateStreamKey`, {})
      .pipe(
        map(response => response.streamKey)
      );
  }

  getRecommendations(userId: string, count: number = 6): Observable<LiveStream[]> {
    return this.http.get<LiveStream[]>(`${this.apiUrl}/stream/${userId}/reccommendations?count=${count}`);
  }

  startStream(streamKey: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/stream/start/${streamKey}`, {});
  }

  endStream(streamKey: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/stream/end/${streamKey}`, {});
  }
}
