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

  getStreamByUserId(userId: string): Observable<LiveStream | null> {
    return this.http.get<LiveStream>(`${this.apiUrl}/stream/user/${userId}`);
  }

  createStream(userId?: string): Observable<LiveStream> {
    // If userId is provided, send as query param, else send empty
    const url = userId ? `${this.apiUrl}/stream?userId=${userId}` : `${this.apiUrl}/stream`;
    return this.http.post<LiveStream>(url, {});
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

  joinViewer(streamId: string, viewerId: string): Observable<void> {
    // The backend expects viewerId as a query parameter, not in the body
    return this.http.post<void>(`${this.apiUrl}/stream/${streamId}/viewer/join?viewerId=${viewerId}`, {});
  }

  leaveViewer(streamId: string, viewerId: string): Observable<void> {
    // The backend expects viewerId as a query parameter, not in the body
    return this.http.post<void>(`${this.apiUrl}/stream/${streamId}/viewer/leave?viewerId=${viewerId}`, {});
  }

  getViewerCount(streamId: string): Observable<number> {
    return this.http.get<number>(`${this.apiUrl}/stream/${streamId}/viewers`);
  }
}
