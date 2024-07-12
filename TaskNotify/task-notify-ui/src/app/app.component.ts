
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HttpClient, HttpHeaders } from '@angular/common/http';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { LoginFormComponent } from "./login-form/login-form.component";
import { CreateTaskFormComponent } from "./create-task-form/create-task-form.component";
import { NotificationService } from './services/notification.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule, FormsModule, HttpClientModule, LoginFormComponent, CreateTaskFormComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'task-notify-ui';
  hubConnection: HubConnection = new HubConnectionBuilder()
  .withUrl('http://localhost:4997/taskNotifyHub')
  .build();

  constructor(private httpClient: HttpClient,private notificationService: NotificationService) {
   
  }

  ngOnInit() {
  this.hubConnection.on('ReceiveTaskNotification', (userName,taskName) => {
    this.notificationService.notify(`${userName} ${taskName}`);
  });

  this.hubConnection.start()
    .then(() => console.log('connection started'))
    .catch((err) => console.log('error while establishing signalr connection: ' + err));
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('token');
  }

}
