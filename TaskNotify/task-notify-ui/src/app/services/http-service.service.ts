import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable, tap } from 'rxjs';
import { ConfigService } from './config.service';

@Injectable({
  providedIn: 'root'
})
export class HttpServiceService {

  constructor(private http: HttpClient, private config:ConfigService) {

  }

  post(address: string, payload: any, headers: HttpHeaders): Observable<any> {
    return this.http.post(this.config.getConfig().TaskAPIBaseAddress+address, payload, { headers }).pipe(
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
