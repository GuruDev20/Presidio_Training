import { Component, Input, OnChanges, SimpleChanges, ChangeDetectionStrategy } from '@angular/core';
import { PaymentResponse } from '../../models/payment.model';
import { CommonModule } from '@angular/common';
import { PaymentHistoryService } from '../../services/paymentHistoryService.service';

@Component({
    selector: 'app-payment-result',
    imports: [CommonModule],
    templateUrl: './payment-result.html',
    styleUrl: './payment-result.css',
    standalone: true,
    changeDetection: ChangeDetectionStrategy.OnPush 
})
export class PaymentResult implements OnChanges {
    @Input() response: PaymentResponse | null = null;
    paymentHistory: PaymentResponse[] = [];

    constructor(private paymentHistoryService: PaymentHistoryService) {}

    ngOnChanges(changes: SimpleChanges): void {
        if (changes['response'] && changes['response'].currentValue) {
            this.paymentHistory = this.paymentHistoryService.getPaymentHistory();
        }
    }
}