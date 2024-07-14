import { HttpHeaders } from '@angular/common/http';
import { Component, EventEmitter, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpServiceService } from '../services/http-service.service';

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

  @Output() usernameSet = new EventEmitter<string>();

  constructor(private httpServiceService:HttpServiceService) {
   
  }

  onSubmitLogin() {
    let headers = new HttpHeaders({ 'Content-Type': 'application/json' });

    const payload = {
      username: this.username,
      password: this.password
    };

    this.httpServiceService.post('security/LoginUser', payload, headers).subscribe(
      response => {
        localStorage.setItem('token', response.token);
        this.usernameSet.emit(this.username);
      },
      error => {
        console.error('LoginError:', error);
      }
    );
  }

}
