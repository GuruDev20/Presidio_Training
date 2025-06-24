import { CommonModule } from '@angular/common';
import { Component, Input, signal, computed, effect } from '@angular/core';

@Component({
    selector: 'app-toast',
    standalone: true,
    templateUrl: './toast.html',
    imports:[CommonModule]
})
export class ToastComponent {
    @Input() type: 'success' | 'error' = 'success';
    @Input() message: string = '';

    show = signal(false);

    trigger(message: string, type: 'success' | 'error') {
        this.message = message;
        this.type = type;
        this.show.set(true);

        setTimeout(() => this.show.set(false), 3000);
    }

    get colorClass() {
        return this.type === 'success'
        ? '#67AE6E'
        : '#CD5656';
    }
}
