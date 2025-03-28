import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ServiceBase } from './service-base';
import { HttpClient } from '@angular/common/http';
import { LiveStream } from '../models/stream/stream';

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
    return this.http.post<string>(`${this.apiUrl}/stream/${id}/generateStreamKey`, {});
  }
}
