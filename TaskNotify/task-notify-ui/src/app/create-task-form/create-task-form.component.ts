import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { NotificationService } from '../services/notification.service';

@Component({
  selector: 'app-create-task-form',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './create-task-form.component.html',
  styleUrl: './create-task-form.component.css'
})
export class CreateTaskFormComponent {
  taskName: string = '';
  completedTasks: string[] = [];
  createdTasks: string[] = [];
  failedTasks: string[] = [];
  taskProcessTimeInSeconds: number = 5;

  constructor(private httpClient: HttpClient, private notificationService: NotificationService) {

  }

  ngOnInit() {
    this.notificationService.notifyObservable.subscribe((message) => {
      this.completedTasks.push(message);
    });
  }

  onSubmitTaskForm() {
    let headers = new HttpHeaders({ 'Content-Type': 'application/json' });

    const token = localStorage.getItem('token');
    const userName = localStorage.getItem('username');
    headers = new HttpHeaders({ 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` });
    const payload = {
      taskName: this.taskName,
      userName: userName,
      taskProcessTimeInSeconds: this.taskProcessTimeInSeconds
    };

    this.httpClient.post<any>('http://localhost:4998/api/tasks/createnew', payload, { headers }).subscribe(
      response => {
        console.log(response);
        this.createdTasks.push(this.taskName);
      },
      error => {
        console.error('TaskError:', error);
        if(error.status === 401){
          localStorage.removeItem('token');
          localStorage.removeItem('username');
        }
        this.failedTasks.push(this.taskName);
      }
    );
  }

  clearHistory() {
    this.completedTasks = [];
    this.createdTasks = [];
    this.failedTasks = [];
  }

}
