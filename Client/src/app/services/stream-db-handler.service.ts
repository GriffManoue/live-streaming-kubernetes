import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ServiceBase } from './service-base';
import { LiveStream } from '../models/stream/stream';

@Injectable({
  providedIn: 'root'
})
export class StreamDbHandlerService extends ServiceBase {
  constructor(http: HttpClient) {
    super(http);
  }

  getStreamById(id: string): Observable<LiveStream> {
    return this.http.get<LiveStream>(`${this.apiUrl}/streamdbhandler/${id}`);
  }

  getActiveStreams(): Observable<LiveStream[]> {
    return this.http.get<LiveStream[]>(`${this.apiUrl}/streamdbhandler`);
  }

  getStreamByUserId(userId: string): Observable<LiveStream> {
    return this.http.get<LiveStream>(`${this.apiUrl}/streamdbhandler/user/${userId}`);
  }

  createStream(userId?: string): Observable<LiveStream> {
    const url = userId 
      ? `${this.apiUrl}/streamdbhandler?userId=${userId}`
      : `${this.apiUrl}/streamdbhandler`;
    return this.http.post<LiveStream>(url, {});
  }

  updateStream(id: string, stream: LiveStream): Observable<LiveStream> {
    return this.http.put<LiveStream>(`${this.apiUrl}/streamdbhandler/${id}`, stream);
  }

  getAllStreams(): Observable<LiveStream[]> {
    return this.http.get<LiveStream[]>(`${this.apiUrl}/streamdbhandler/all`);
  }
}
