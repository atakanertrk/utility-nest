import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-login-form',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './login-form.component.html',
  styleUrl: './login-form.component.css'
})
export class LoginFormComponent {

  username: string = '';
  password: string = '';

  constructor(private httpClient: HttpClient) {
   
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

}
