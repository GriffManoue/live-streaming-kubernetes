import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';

 import { UserService } from '../../services/user.service';
 import { User } from '../../models/user/user';
import { PasswordModule } from 'primeng/password';
import { MessagesModule } from 'primeng/messages';
import { MessageModule } from 'primeng/message';
import { MessageService } from 'primeng/api'; 

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    InputTextModule,
    ButtonModule,
    CardModule,
    PasswordModule,
    MessagesModule,
    MessageModule
  ],
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
  profileForm!: FormGroup;
  user: User | any = null; 
  userId!: string;
  loading = false;
  profileError = false;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private userService: UserService,
    private messageService: MessageService 
  ) {
    this.initializeForm();
  }

  ngOnInit() {
    this.route.params.subscribe(params => {
      this.userId = params['id'];
      this.fetchUserData(this.userId);
    });
  }

  private initializeForm() {
    this.profileForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      password: ['', [Validators.minLength(8)]],
      confirmPassword: ['']
    }, {
      validators: this.passwordMatchValidator
    }
  
  ); 
  }

  passwordMatchValidator(group: FormGroup): { [key: string]: boolean } | null {
    const passwordControl = group.get('password');
    const confirmPasswordControl = group.get('confirmPassword');
    if (!passwordControl || !confirmPasswordControl) {
      return null;
    }
    if (confirmPasswordControl.hasError('passwordMismatch')) {
        confirmPasswordControl.setErrors(null);
    }

    if (!passwordControl.value || !confirmPasswordControl.value) {
        return null;
    }

    if (passwordControl.value !== confirmPasswordControl.value) {
      confirmPasswordControl.setErrors({ ...confirmPasswordControl.errors, passwordMismatch: true });
      return { passwordMismatch: true };
    }
    return null; 
  }

  private fetchUserData(userId: string) {
    this.loading = true;
    this.profileError = false;
    console.log(`Fetching data for user ID: ${userId}`);
    this.userService.getUserById(userId).subscribe({
      next: (userData) => {
        this.user = userData;
        this.updateFormWithUserData();
        this.loading = false;
      },
      error: (err) => {
        console.error('Error fetching user data:', err);
        this.errorMessage = 'Failed to load user data.';
        this.profileError = true; 
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to load user data.' }); // Show toast
        this.loading = false;
      }
    });
  }

  private updateFormWithUserData() {
    if (this.user) {
      this.profileForm.patchValue({
        username: this.user.username,
        email: this.user.email,
        firstName: this.user.firstName,
        lastName: this.user.lastName
      });
    }
  }

  get f() { return this.profileForm.controls; }

  onSubmit() {
    this.profileError = false;
    if (this.profileForm.invalid) {
      Object.keys(this.profileForm.controls).forEach(key => {
        this.profileForm.get(key)?.markAsTouched();
      });
      this.messageService.add({ severity: 'warn', summary: 'Validation Error', detail: 'Please check the form for errors.' });
      return;
    }

    this.loading = true;
    const updatedUserData = {
      ...this.user,
      username: this.f['username'].value,
      email: this.f['email'].value,
      firstName: this.f['firstName'].value,
      lastName: this.f['lastName'].value,
      password: this.f['password'].value,
    };

    this.userService.updateUser(this.userId, updatedUserData).subscribe({
        next: (result) => {
          this.loading = false;
          this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Profile updated successfully!' }); 
          this.profileForm.get('password')?.reset('');
          this.profileForm.get('confirmPassword')?.reset('');
        },
        error: (error) => {
          const detail = error?.error?.message || 'Failed to update profile. Please try again.'; 
          this.errorMessage = detail;
          this.profileError = true;
          this.messageService.add({ severity: 'error', summary: 'Error', detail: detail });
          this.loading = false;
        }
      });
  }
}
