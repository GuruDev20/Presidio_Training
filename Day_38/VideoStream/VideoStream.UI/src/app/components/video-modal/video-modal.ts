import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
    selector: 'app-video-modal',
    imports: [CommonModule],
    templateUrl: './video-modal.html',
    styleUrl: './video-modal.css',
    standalone: true
})
export class VideoModal {
    @Input() video: any;
    @Output() close = new EventEmitter();

    handleClose() {
        console.log('Close clicked');
        this.close.emit();
    }
}
