import { Component } from '@angular/core';
import { PaymentHistoryService } from './services/paymentHistoryService.service';
import { PaymentForm } from './components/payment-form/payment-form';
import { CommonModule } from '@angular/common';
import { PaymentResponse } from './models/payment.model';

@Component({
    selector: 'app-root',
    templateUrl: './app.html',
    styleUrl: './app.css',
    imports:[PaymentForm,CommonModule]
})
export class App {

    paymentHistory: PaymentResponse[] = [];

    constructor(private paymentHistoryService: PaymentHistoryService) {
        this.paymentHistory = this.paymentHistoryService.getPaymentHistory();
    }

}
