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

  constructor(private fb: FormBuilder, private route: ActivatedRoute, private streamService: StreamService) { }

  ngOnInit() {
    this.route.params.subscribe(params => {
      const id = params['id'];
      this.streamService.getStreamById(id).subscribe(stream => {
        this.stream = stream;
        this.initForm();
      }); // Assuming this method exists in your service
    });
  }

  initForm() {
    this.streamForm = this.fb.group({
      streamName: [this.stream.streamName, Validators.required],
      streamDescription: [this.stream.streamDescription],
      streamCategory: [this.stream.streamCategory, Validators.required],
      streamToken: [{ value: this.generateTokenIfEmpty(), disabled: true }]
    });
  }

  onSubmit() {
    console.log(this.streamForm.value);
    // TODO: Send to API
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
    // In a real app, this would call an API to generate a secure token
    const newToken = 'stream_' + Math.random().toString(36).substring(2, 15);
    this.streamForm.get('streamToken')?.setValue(newToken);
    // TODO: Save the new token to the backend
    console.log('Generated new token:', newToken);
  }

  private generateTokenIfEmpty(): string {
    // Check if the stream already has a token
    //todo
    
    // Generate a placeholder token for demo purposes
    // In production, this should come from the backend
    return 'stream_' + Math.random().toString(36).substring(2, 15);
  }
}
