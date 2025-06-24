import { Injectable } from '@angular/core';
import { ToastComponent } from '../components/toast/toast';

@Injectable({ providedIn: 'root' })
export class ToastService {
    private toastRef?: ToastComponent;

    register(toast: ToastComponent) {
        this.toastRef = toast;
    }

    show(message: string, type: 'success' | 'error' = 'success') {
        this.toastRef?.trigger(message, type);
    }
}
