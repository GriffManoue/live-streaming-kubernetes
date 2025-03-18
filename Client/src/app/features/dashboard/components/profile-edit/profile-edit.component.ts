import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { User } from '../../../../shared/models/user.model';

@Component({
  selector: 'app-profile-edit',
  templateUrl: './profile-edit.component.html',
  styleUrls: ['./profile-edit.component.scss']
})
export class ProfileEditComponent implements OnInit {
  @Input() user: User | null = null;
  @Output() editComplete = new EventEmitter<void>();
  
  profileForm!: FormGroup;
  loading = false;
  error = '';
  
  constructor(private formBuilder: FormBuilder) { }

  ngOnInit(): void {
    this.initForm();
  }

  private initForm(): void {
    this.profileForm = this.formBuilder.group({
      username: [this.user?.username || '', [Validators.required, Validators.minLength(3), Validators.maxLength(20)]],
      email: [this.user?.email || '', [Validators.required, Validators.email]],
      bio: [this.user?.bio || ''],
      profilePictureUrl: [this.user?.profilePictureUrl || '']
    });
  }

  onSubmit(): void {
    if (this.profileForm.invalid) {
      return;
    }

    this.loading = true;
    
    // In a real application, this would call a service method to update the profile
    // For now, we'll just simulate a successful update
    setTimeout(() => {
      this.loading = false;
      this.editComplete.emit();
    }, 1000);
  }

  onCancel(): void {
    this.editComplete.emit();
  }
}
