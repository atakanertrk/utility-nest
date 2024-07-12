import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  private notifySource = new Subject<string>();
  notifyObservable = this.notifySource.asObservable();

  constructor() {}

  notify(message: string) {
    this.notifySource.next(message);
  }
}