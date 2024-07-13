
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { LoginFormComponent } from "./login-form/login-form.component";
import { CreateTaskFormComponent } from "./create-task-form/create-task-form.component";
import { NotificationService } from './services/notification.service';
import { ConfigService } from './services/config.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule, FormsModule, LoginFormComponent, CreateTaskFormComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'task-notify-ui';

  private _username: string = '';
  get username(): string {
    if (!this._username || this._username === '') {
      this._username = localStorage.getItem('username') || '';
    }
    return this._username;
  }
  set username(value: string) {
    this._username = value;
    localStorage.setItem('username', value);
  }

  constructor(private notificationService: NotificationService, private configService: ConfigService) {

  }

  ngOnInit() {
    this.configService.loadConfigFile().subscribe({
      next: (config) => {
        const hubConnection: HubConnection = new HubConnectionBuilder()
          .withUrl(config.TaskNotifyHubAddress)
          .build();
  
        hubConnection.on('ReceiveTaskNotification', (userName, taskName) => {
          this.notificationService.notify(`${userName}: ${taskName}`);
        });
  
        hubConnection.start()
          .then(() => console.log('connection started'))
          .catch((err) => console.log('error while establishing signalr connection: ' + err));
      },
      error: (e) => {
        console.error(e);
      }
    });
  }

  setUsername(username: string) {
    this.username = username; // Set the username when the event is received
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('token');
  }

}
