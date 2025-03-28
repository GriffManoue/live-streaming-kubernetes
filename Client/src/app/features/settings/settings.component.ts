import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { InputSwitchModule } from 'primeng/inputswitch';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { TooltipModule } from 'primeng/tooltip';
import { CommonModule } from '@angular/common';
import { LiveStream } from '../../models/stream/stream';
import { StreamCategories, StreamCategoryKey } from '../../models/enums/stream-categories';
import { ActivatedRoute } from '@angular/router';
import { StreamService } from '../../services/stream.service';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    InputTextModule,
    DropdownModule,
    InputSwitchModule,
    ButtonModule,
    CardModule,
    TooltipModule
  ],
  templateUrl: './settings.component.html',
  styleUrl: './settings.component.css'
})
export class SettingsComponent implements OnInit {
  streamForm!: FormGroup;
  stream!: LiveStream;
  categories = Object.keys(StreamCategories);

  constructor(private fb: FormBuilder, private route: ActivatedRoute, private streamService: StreamService) {
    // Initialize form with default values
    this.initializeForm();
  }

  ngOnInit() {
    this.route.params.subscribe(params => {
      const userId = params['id'];
      // Get stream by user ID instead of stream ID
      this.streamService.getStreamByUserId(userId).subscribe(stream => {
        this.stream = stream;
        this.updateFormWithStreamData();

      });
    });
  }


  private initializeForm() {
    // Initialize with empty values first
    this.streamForm = this.fb.group({
      streamName: ['', Validators.required],
      streamDescription: [''],
      streamCategory: ['', Validators.required],
      streamToken: [{ value: this.generateNewToken, disabled: true }]
    });
  }

  private updateFormWithStreamData() {
    if (this.stream) {
      this.streamForm.patchValue({
        streamName: this.stream.streamName,
        streamDescription: this.stream.streamDescription,
        streamCategory: this.stream.streamCategory,
        // Keep the existing token or generate a new one if needed
        streamToken: this.generateNewToken()
      });
    }
  }

  onSubmit() {
    if (this.streamForm.valid && this.stream && this.stream.id) {
      // Get form values
      const updatedStream: LiveStream = {
        ...this.stream,
        streamName: this.streamForm.get('streamName')?.value,
        streamDescription: this.streamForm.get('streamDescription')?.value,
        streamCategory: this.streamForm.get('streamCategory')?.value
      };

      // Call the API to update the stream
      this.streamService.updateStream(this.stream.id, updatedStream).subscribe({
        next: (result) => {
          console.log('Stream updated successfully', result);
          // TODO: Add success notification
        },
        error: (error) => {
          console.error('Error updating stream', error);
          // TODO: Add error notification
        }
      });
    } else {
      // Mark all fields as touched to trigger validation
      Object.keys(this.streamForm.controls).forEach(key => {
        const control = this.streamForm.get(key);
        control?.markAsTouched();
      });
    }
  }

  copyToken() {
    const tokenValue = this.streamForm.get('streamToken')?.value;
    if (tokenValue) {
      navigator.clipboard.writeText(tokenValue)
        .then(() => {
          // TODO: Add toast notification for success
          console.log('Token copied to clipboard');
        })
        .catch(err => {
          console.error('Could not copy text: ', err);
        });
    }
  }

  generateNewToken() {
    if (this.stream && this.stream.id) {
      this.streamService.generateStreamKey(this.stream.id).subscribe({
        next: (newToken) => {
          this.streamForm.get('streamToken')?.setValue(newToken);
          console.log('Generated new token:', newToken);
        },
        error: (err) => {
          console.error('Error generating new token:', err);
        }
      });
    } else {
      console.error('Cannot generate token: Stream ID is missing');
    }
  }
}
