import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { NotificationService } from '../services/notification.service';

@Component({
  selector: 'app-create-task-form',
  standalone: true,
  imports: [FormsModule,CommonModule],
  templateUrl: './create-task-form.component.html',
  styleUrl: './create-task-form.component.css'
})
export class CreateTaskFormComponent {
  taskName: string = '';
  messages: string[] = []; 

  constructor(private httpClient: HttpClient,private notificationService: NotificationService) {
   
  }

  ngOnInit() {
    this.notificationService.notifyObservable.subscribe((message) => {
      this.messages.push(`TASK COMPLETED! -> ${message}`);
    });
  }

  onSubmitTaskForm() {
    let headers = new HttpHeaders({ 'Content-Type': 'application/json' });

    const payload = {
      taskName: this.taskName
    };
    const token = localStorage.getItem('token');
    headers = new HttpHeaders({ 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` });
    this.httpClient.post<any>('http://localhost:4998/api/tasks/createnew', payload, { headers }).subscribe(
      response => {
        console.log(response);
        this.messages.push(`Task created successfully! -> ${this.taskName}`);
      },
      error => {
        console.error('TaskError:', error);
        this.messages.push(`Failed to create task! -> ${this.taskName}`);      }
    );
  }

}
