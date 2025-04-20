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

  getStreamById(id: string): Observable<LiveStream> {
    return this.http.get<LiveStream>(`${this.apiUrl}/stream/${id}`);
  }

  getActiveStreams(): Observable<LiveStream[]> {
    return this.http.get<LiveStream[]>(`${this.apiUrl}/stream`);
  }

  getStreamByUserId(userId: string): Observable<LiveStream> {
    return this.http.get<LiveStream>(`${this.apiUrl}/stream/user/${userId}`);
  }

  createStream(): Observable<LiveStream> {
    return this.http.post<LiveStream>(`${this.apiUrl}/stream`, {});
  }

  updateStream(id: string, stream: LiveStream): Observable<LiveStream> {
    return this.http.put<LiveStream>(`${this.apiUrl}/stream/${id}`, stream);
  }

  generateStreamKey(id: string): Observable<string> {
    return this.http.post<{ streamKey: string }>(`${this.apiUrl}/stream/${id}/generateStreamKey`, {})
      .pipe(
        map(response => response.streamKey)
      );
  }

  joinViewer(streamId: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/stream/${streamId}/viewer/join`, {});
  }

  leaveViewer(streamId: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/stream/${streamId}/viewer/leave`, {});
  }

  getViewerCount(streamId: string): Observable<number> {
    return this.http.get<number>(`${this.apiUrl}/stream/${streamId}/viewers`);
  }
}
