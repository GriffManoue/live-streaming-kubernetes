import { Component } from '@angular/core';
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
  selector: 'app-register',
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
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  username: string = '';
  email: string = '';
  password: string = '';
  confirmPassword: string = '';
  firstName: string = '';
  lastName: string = '';
  phone: string = '';
  rememberMe: boolean = false;
  registerError: boolean = false;
  errorMessage: string = '';
  loading: boolean = false;

  constructor(private router: Router, private loginService: LoginService) {}

  onSubmit() {
    if (!this.username || !this.email || !this.password || !this.confirmPassword || !this.firstName || !this.lastName || !this.phone) {
      this.registerError = true;
      this.errorMessage = 'All fields are required';
      return;
    }

    if (this.password !== this.confirmPassword) {
      this.registerError = true;
      this.errorMessage = 'Passwords do not match';
      return;
    }

    this.loading = true;
    this.registerError = false;

    // Simulating API call with timeout
    setTimeout(() => {
      // For demo purposes - in real app you would validate with your AuthService
      if (this.username === 'demo' && this.password === 'password') {
        this.registerError = false;
        this.loginService.login('demo-token', this.rememberMe);
        this.router.navigate(['/home']);
      } else {
        this.registerError = true;
        this.errorMessage = 'Registration failed';
      }
      this.loading = false;
    }, 1000);
  }
}
