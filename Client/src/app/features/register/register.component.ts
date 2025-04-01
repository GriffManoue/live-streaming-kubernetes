import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormControl } from '@angular/forms';
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
import { RegisterRequest } from '../../models/auth/register-request';
import { MessageService } from 'primeng/api'; // Import MessageService

@Component({
  selector: 'app-register',
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
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent implements OnInit {
  registerForm!: FormGroup;
  registerError: boolean = false;
  errorMessage: string = '';
  loading: boolean = false;

  constructor(
    private router: Router,
    private loginService: LoginService, // Keep if needed, otherwise remove
    private authService: AuthService,
    private fb: FormBuilder,
    private messageService: MessageService // Inject MessageService
  ) {}

  ngOnInit(): void {
    this.initForm();
  }

  initForm(): void {
    this.registerForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
      password: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', [Validators.required]],
      rememberMe: [false]
    }, { 
      validators: this.passwordMatchValidator 
    });
  }

  // Custom validator for password matching
  passwordMatchValidator(group: FormGroup): { [key: string]: boolean } | null {
    const password = group.get('password')?.value;
    const confirmPassword = group.get('confirmPassword')?.value;
    
    return password === confirmPassword ? null : { passwordMismatch: true };
  }

  // Convenience getters for easy access to form fields
  get f() { return this.registerForm.controls; }

  onSubmit() {
    // Mark all fields as touched to trigger validation display
    Object.keys(this.registerForm.controls).forEach(key => {
      const control = this.registerForm.get(key);
      control?.markAsTouched();
    });

    if (this.registerForm.invalid) {
      this.registerError = true; // Keep inline error if desired
      if (this.registerForm.errors?.['passwordMismatch']) {
        this.errorMessage = 'Passwords do not match';
        this.messageService.add({ severity: 'warn', summary: 'Validation Error', detail: 'Passwords do not match.' }); // Add toast
      } else {
        this.errorMessage = 'Please correct the form errors';
        this.messageService.add({ severity: 'warn', summary: 'Validation Error', detail: 'Please check the form for errors.' }); // Add toast
      }
      return;
    }

    this.registerError = false;
    this.loading = true; // Start loading indicator

    // Form values
    const formValues = this.registerForm.value;

    let registerRequest: RegisterRequest = {
      username: formValues.username,
      email: formValues.email,
      firstName: formValues.firstName,
      lastName: formValues.lastName,
      password: formValues.password,
    }

    this.authService.register(registerRequest).subscribe({
      next: (response) => {
        this.loading = false; // Stop loading indicator
        this.registerError = false;
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Registration successful! Please log in.' }); // Success toast
        // Navigate after a short delay
        setTimeout(() => this.router.navigate(['/login']), 1000);
      },
      error: (error) => {
        this.loading = false; // Stop loading indicator
        this.registerError = true;
        const detail = error?.error?.message || 'Registration failed';
        this.errorMessage = detail;
        this.messageService.add({ severity: 'error', summary: 'Registration Failed', detail: detail }); // Error toast
      }
    });

  }
}
