import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
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
    ReactiveFormsModule,
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
  loginForm!: FormGroup;
  loginError: boolean = false;
  errorMessage: string = '';
  loading: boolean = false;

  constructor(
    private router: Router, 
    private loginService: LoginService,
    private fb: FormBuilder
  ) {}

  ngOnInit() {
    const token = localStorage.getItem('auth_token');
    //todo: check if token is valid
    if (token) {
      this.router.navigate(['/home']);
    }
    
    this.initForm();
  }

  initForm(): void {
    this.loginForm = this.fb.group({
      username: ['', [Validators.required]],
      password: ['', [Validators.required]],
      rememberMe: [false]
    });
  }

  // Convenience getter for easy access to form fields
  get f() { return this.loginForm.controls; }

  onSubmit() {
    // Mark all fields as touched to trigger validation display
    Object.keys(this.loginForm.controls).forEach(key => {
      const control = this.loginForm.get(key);
      control?.markAsTouched();
    });

    if (this.loginForm.invalid) {
      this.loginError = true;
      this.errorMessage = 'Please correct the form errors';
      return;
    }

    this.loading = true;
    this.loginError = false;

    // Form values
    const formValues = this.loginForm.value;

    // Simulating API call with timeout
    setTimeout(() => {
      // For demo purposes - in real app you would validate with your AuthService
      if (formValues.username === 'demo' && formValues.password === 'password') {
        this.loginError = false;
        this.loginService.login('demo-token', formValues.rememberMe);
        this.router.navigate(['/home']);
      } else {
        this.loginError = true;
        this.errorMessage = 'Invalid username or password';
      }
      this.loading = false;
    }, 1000);
  }
}
