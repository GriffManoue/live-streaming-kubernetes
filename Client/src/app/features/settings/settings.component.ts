import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { InputSwitchModule } from 'primeng/inputswitch';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { CommonModule } from '@angular/common';
import { LiveStream } from '../../models/stream/stream';
import { StreamCategories, StreamCategoryKey } from '../../models/enums/stream-categories';
import { ActivatedRoute } from '@angular/router';

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
    CardModule
  ],
  templateUrl: './settings.component.html',
  styleUrl: './settings.component.css'
})
export class SettingsComponent implements OnInit {
  streamForm!: FormGroup;
  stream: LiveStream = new LiveStream("1", "My Stream", "A description of my stream", "Gaming", "123");
  categories = Object.keys(StreamCategories);

  constructor(private fb: FormBuilder, private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.params.subscribe(params => {
      const id = params['id'];
      // Use the id as needed
      console.log(id);
    });
    this.initForm();
  }

  initForm() {
    this.streamForm = this.fb.group({
      streamName: [this.stream.streamName, Validators.required],
      streamDescription: [this.stream.streamDescription],
      streamCategory: [this.stream.streamCategory, Validators.required],
      isActive: [this.stream.isActive]
    });
  }

  onSubmit() {
    console.log(this.streamForm.value);
  }
}
