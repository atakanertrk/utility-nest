import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { WebConfig } from '../common/WebConfig';

@Injectable({
  providedIn: 'root'
})
export class ConfigService {

  private config!: WebConfig;

  constructor(private http: HttpClient) {

  }

  loadConfigFile(): Observable<WebConfig> {
    return this.http.get('/assets/webconfig.json').pipe(
      tap((data:any) => {
        this.config = data as WebConfig;
        return data;
      })
    );
  }

  getConfig(): WebConfig {
    return this.config;
  }
}