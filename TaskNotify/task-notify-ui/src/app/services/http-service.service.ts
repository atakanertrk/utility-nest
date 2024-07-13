import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class HttpServiceService {

  constructor(private http: HttpClient) {

  }

  post(address: string, payload: any, headers: HttpHeaders): Observable<any> {
    return this.http.post(address, payload, { headers }).pipe(
      tap((data: any) => {
        return data;
      }),
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401) {
          localStorage.removeItem('token');
          localStorage.removeItem('username');
        }
        throw error;
      })
    );
  }
}
