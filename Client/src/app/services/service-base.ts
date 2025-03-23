import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

export abstract class ServiceBase {
  protected apiUrl: string = 'http://localhost/api'; // Base URL for the API

  constructor(protected http: HttpClient, apiUrl?: string) {
    if (apiUrl) {
      this.apiUrl = apiUrl;
    }
  }

  protected get<T>(url: string): Observable<T> {
    return this.http.get<T>(`${this.apiUrl}/${url}`);
  }

  protected post<T>(url: string, body: any): Observable<T> {
    return this.http.post<T>(`${this.apiUrl}/${url}`, body);
  }

  protected put<T>(url: string, body: any): Observable<T> {
    return this.http.put<T>(`${this.apiUrl}/${url}`, body);
  }

  protected delete<T>(url: string): Observable<T> {
    return this.http.delete<T>(`${this.apiUrl}/${url}`);
  }
}
