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
import { LoginService } from '../../services/login.service';
import { AuthService } from '../../services/auth.service';
import { LoginRequest } from '../../models/auth/login-request';
import { AuthResult } from '../../models/auth/auth-result';
import { MessageService } from 'primeng/api'; // Import MessageService

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
    private fb: FormBuilder,
    private authService: AuthService,
    private loginService: LoginService,
    private messageService: MessageService // Inject MessageService
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
      this.loginError = true; // Keep inline error if desired
      this.errorMessage = 'Please correct the form errors';
      this.messageService.add({ severity: 'warn', summary: 'Validation Error', detail: 'Please check the form for errors.' }); // Add toast
      return;
    }

    // Form values
    const formValues = this.loginForm.value;
    this.loginError = false;
    this.loading = true; // Start loading indicator

    let loginRequest: LoginRequest = {
      username: formValues.username,
      password: formValues.password,
    }

    this.authService.login(loginRequest).subscribe({
      next: (response: AuthResult) => {
        this.loading = false; // Stop loading indicator
        this.loginError = false;
        const userId = response.userId;
        if(response.token){
          this.loginService.login(response.token, formValues.rememberMe, userId);
          this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Login successful!' }); // Success toast
          // Navigate after a short delay to allow toast visibility
          setTimeout(() => this.router.navigate(['/home']), 500);
        }else{
          this.loginError = true;
          this.errorMessage = 'Invalid username or password';
          this.messageService.add({ severity: 'error', summary: 'Login Failed', detail: 'Invalid username or password.' }); // Error toast
        }
      },
      error: (error) => {
        this.loading = false; // Stop loading indicator
        this.loginError = true;
        const detail = error?.error?.message || 'Invalid username or password';
        this.errorMessage = detail;
        this.messageService.add({ severity: 'error', summary: 'Login Failed', detail: detail }); // Error toast
      }
    });
  }
}
