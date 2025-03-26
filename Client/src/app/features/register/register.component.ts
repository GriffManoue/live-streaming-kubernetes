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
    private loginService: LoginService,
    private authService: AuthService,
    private fb: FormBuilder
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
      if (this.registerForm.errors?.['passwordMismatch']) {
        this.registerError = true;
        this.errorMessage = 'Passwords do not match';
      } else {
        this.registerError = true;
        this.errorMessage = 'Please correct the form errors';
      }
      return;
    }

    this.registerError = false;

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
        this.registerError = false;

        // Handle successful registration
        this.router.navigate(['/login']);
      },
      error: (error) => {
        this.registerError = true;
        this.errorMessage = error.error.message || 'Registration failed';
      }
    });

  }
}
