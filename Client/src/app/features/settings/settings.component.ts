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
import { MessageService } from 'primeng/api'; 

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

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private streamService: StreamService,
    private messageService: MessageService 
  ) {
    this.initializeForm();
  }

  ngOnInit() {
    this.route.params.subscribe(params => {
      const userId = params['id'];
      this.streamService.getStreamByUserId(userId).subscribe({
        next: (stream) => {
          this.stream = stream;
          this.updateFormWithStreamData();
        },
        error: (error) => {
          console.error('Error fetching stream:', error);
          if (error.status === 404) {
            this.createNewStream(userId);
          } else {
            this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to load stream settings.' });
          }
        }
      });
    });
  }

  private createNewStream(userId: string) {
    this.streamService.createStream().subscribe({
      next: (newStream) => {
        this.stream = newStream;
        this.updateFormWithStreamData();
        console.log('Created new stream:', newStream);
        this.messageService.add({ severity: 'info', summary: 'Info', detail: 'New stream settings created.' });
      },
      error: (error) => {
        console.error('Error creating new stream:', error);
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to create new stream settings.' });
      }
    });
  }

  private initializeForm() {
    this.streamForm = this.fb.group({
      streamName: ['', Validators.required],
      streamDescription: [''],
      streamCategory: ['', Validators.required],
      streamToken: [{ value: '', disabled: true }]
    });
  }

  private updateFormWithStreamData() {
    if (this.stream) {
      this.streamForm.patchValue({
        streamName: this.stream.streamName,
        streamDescription: this.stream.streamDescription,
        streamCategory: this.stream.streamCategory,
        streamToken: this.stream.streamKey || ''
      });
    }
  }

  onSubmit() {
    if (this.streamForm.valid && this.stream && this.stream.id) {
      const updatedStream: LiveStream = {
        ...this.stream,
        streamName: this.streamForm.get('streamName')?.value,
        streamDescription: this.streamForm.get('streamDescription')?.value,
        streamCategory: this.streamForm.get('streamCategory')?.value
      };

      this.streamService.updateStream(this.stream.id, updatedStream).subscribe({
        next: (result) => {
          console.log('Stream updated successfully', result);
          this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Stream settings saved successfully!' });
        },
        error: (error) => {
          console.error('Error updating stream', error);
          const detail = error?.error?.message || 'Failed to save stream settings.';
          this.messageService.add({ severity: 'error', summary: 'Error', detail: detail });
        }
      });
    } else {
      Object.keys(this.streamForm.controls).forEach(key => {
        const control = this.streamForm.get(key);
        control?.markAsTouched();
      });
      this.messageService.add({ severity: 'warn', summary: 'Validation Error', detail: 'Please check the form for errors.' });
    }
  }

  copyToken() {
    const tokenValue = this.streamForm.get('streamToken')?.value;
    if (tokenValue) {
      navigator.clipboard.writeText(tokenValue)
        .then(() => {
          console.log('Token copied to clipboard');
          this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Stream token copied to clipboard!' });
        })
        .catch(err => {
          console.error('Could not copy text: ', err);
          this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to copy stream token.' });
        });
    } else {
        this.messageService.add({ severity: 'info', summary: 'Info', detail: 'No stream token available to copy.' });
    }
  }

  generateNewToken() {
    if (this.stream && this.stream.id) {
      this.streamService.generateStreamKey(this.stream.id).subscribe({
        next: (newToken) => {
          this.streamForm.get('streamToken')?.setValue(newToken);
          this.stream.streamKey = newToken;
          this.stream.streamUrl = `http://localhost:8080/hls/${newToken}.m3u8`;
          console.log('Generated new token:', newToken);
          this.messageService.add({ severity: 'success', summary: 'Success', detail: 'New stream token generated successfully!' });
        },
        error: (err) => {
          console.error('Error generating new token:', err);
          const detail = err?.error?.message || 'Failed to generate new stream token.';
          this.messageService.add({ severity: 'error', summary: 'Error', detail: detail });
        }
      });
    } else {
      console.error('Cannot generate token: Stream ID is missing');
      this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Cannot generate token, stream data is missing.' });
    }
  }
}
