import { HttpHeaders } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule, formatDate } from '@angular/common';
import { NotificationService } from '../services/notification.service';
import { HttpServiceService } from '../services/http-service.service';
import { ConfigService } from '../services/config.service';

@Component({
  selector: 'app-create-task-form',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './create-task-form.component.html',
  styleUrl: './create-task-form.component.css'
})
export class CreateTaskFormComponent {
  taskName: string = '';
  completedTasks: any[] = [];
  createdTasks: any[] = [];
  failedTasks: any[] = [];
  taskProcessTimeInSeconds: number = 5;

  constructor(private notificationService: NotificationService, private httpServiceService: HttpServiceService, private config:ConfigService) {

  }

  ngOnInit() {
    this.notificationService.notifyObservable.subscribe((message) => {
      this.completedTasks.push({message: message,datetime:formatDate(new Date(), 'HH:mm:ss', 'en')});
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

    this.httpServiceService.post(`tasks/createnew`, payload, headers).subscribe(
      response => {
        console.log(response);
        this.createdTasks.push({message: this.taskName,datetime:formatDate(new Date(), 'HH:mm:ss', 'en')});
      },
      error => {
        console.error('TaskError:', error);
        this.failedTasks.push({message: this.taskName,datetime:formatDate(new Date(), 'HH:mm:ss', 'en')});
      }
    );
  }

  clearHistory() {
    this.completedTasks = [];
    this.createdTasks = [];
    this.failedTasks = [];
  }

}
