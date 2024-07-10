
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HttpClient, HttpHeaders } from '@angular/common/http';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule, FormsModule, HttpClientModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'task-notify-ui';
  username: string = '';
  password: string = '';
  taskName: string = '';
  hubConnection: HubConnection = new HubConnectionBuilder()
  .withUrl('http://localhost:60185/taskNotifyHub')
  .build();

  constructor(private httpClient: HttpClient) {
   
  }

  ngOnInit() {
  this.hubConnection.on('ReceiveTaskNotification', (userName,taskName) => {
    console.log(userName + ' ' + taskName);
  });

  this.hubConnection.start()
    .then(() => console.log('connection started'))
    .catch((err) => console.log('error while establishing signalr connection: ' + err));
  }

  onSubmitLogin() {
    console.log('Username:', this.username);
    console.log('Password:', this.password);

    let headers = new HttpHeaders({ 'Content-Type': 'application/json' });

    const payload = {
      username: this.username,
      password: this.password
    };

    this.httpClient.post<any>('http://localhost:4998/api/security/LoginUser', payload, { headers }).subscribe(
      response => {
        console.log('LoginResponse:', response);
        localStorage.setItem('token', response.token);
      },
      error => {
        console.error('LoginError:', error);
      }
    );
  }

  onSubmitForm() {
    console.log('Username:', this.username);
    console.log('Password:', this.password);

    let headers = new HttpHeaders({ 'Content-Type': 'application/json' });

    const payload = {
      userName: this.username,
      taskName: this.taskName
    };
    const token = localStorage.getItem('token');
    headers = new HttpHeaders({ 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` });
    this.httpClient.post<any>('http://localhost:4998/api/tasks/createnew', payload, { headers }).subscribe(
      response => {
        console.log(response);
      },
      error => {
        console.error('TaskError:', error);
      }
    );
  }

}
