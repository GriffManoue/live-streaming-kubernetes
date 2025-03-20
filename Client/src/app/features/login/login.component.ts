import { Component, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { CardModule } from 'primeng/card';
import { PasswordModule } from 'primeng/password';
import { CheckboxModule } from 'primeng/checkbox';
import { DividerModule } from 'primeng/divider';
import { MessagesModule } from 'primeng/messages';
import { MessageModule } from 'primeng/message';
import { LoginService } from '../../services/login/login.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,
    ButtonModule,
    InputTextModule,
    CardModule,
    PasswordModule,
    CheckboxModule,
    DividerModule,
    MessagesModule,
    MessageModule
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit {
  @Output() loginSuccess: boolean = false;

  username: string = '';
  password: string = '';
  rememberMe: boolean = false;
  loginError: boolean = false;
  errorMessage: string = '';
  loading: boolean = false;

  constructor(private router: Router, private loginService: LoginService) {}

  ngOnInit() {
    const token = localStorage.getItem('auth_token');
    //todo: check if token is valid
    if (token) {
      this.router.navigate(['/home']);
    }
  }

  onSubmit() {
    if (!this.username || !this.password) {
      this.loginError = true;
      this.errorMessage = 'Username and password are required';
      return;
    }

    this.loading = true;
    this.loginError = false;

    // Here you would typically call your auth service
    // Simulating API call with timeout
    setTimeout(() => {
      // For demo purposes - in real app you would validate with your AuthService
      if (this.username === 'demo' && this.password === 'password') {
        this.loginSuccess = true;
        this.loginError = false;
        this.loginService.login('demo-token', this.rememberMe);
        this.router.navigate(['/home']);
      } else {
        this.loginError = true;
        this.errorMessage = 'Invalid username or password';
      }
      this.loading = false;
    }, 1000);
  }
}
