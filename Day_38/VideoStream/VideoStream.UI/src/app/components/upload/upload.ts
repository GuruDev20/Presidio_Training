import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { VideoService } from '../../service/video.service';

@Component({
    selector: 'app-upload',
    imports: [CommonModule,FormsModule,ReactiveFormsModule],
    templateUrl: './upload.html',
    styleUrl: './upload.css',
    standalone: true
})
export class Upload {
    uploadForm: FormGroup;
    file: File | null = null;
    fileName = '';

    constructor(private fb: FormBuilder, private videoService: VideoService) {
        this.uploadForm = this.fb.group({
            title: ['', Validators.required],
            description: ['']
        });
    }

    get f() {
        return this.uploadForm.controls;
    }

    onFileChange(event: any) {
        const file = event.target.files[0];
        if (file) {
            this.file = file;
            this.fileName = file.name;
        }
    }

    submit() {
        if (!this.file) return;

        const formData = new FormData();
        formData.append('title', this.uploadForm.value.title);
        formData.append('description', this.uploadForm.value.description);
        formData.append('file', this.file);

        this.videoService.uploadVideo(formData).subscribe(() => {
            alert('Upload Successful!');
            this.uploadForm.reset();
            this.file = null;
            this.fileName = '';
        });
    }
}
