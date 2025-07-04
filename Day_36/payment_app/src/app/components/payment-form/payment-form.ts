import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { PaymentResponse } from '../../models/payment.model';
import { PaymentService } from '../../services/paymentService.service';
import { CommonModule } from '@angular/common';
import { PaymentResult } from '../payment-result/payment-result';

@Component({
    selector: 'app-payment-form',
    imports: [ReactiveFormsModule, FormsModule, CommonModule, PaymentResult],
    templateUrl: './payment-form.html',
    styleUrl: './payment-form.css',
    standalone: true
})
export class PaymentForm implements OnInit {
    paymentForm: FormGroup;
    isLoading = false;
    paymentResponse: PaymentResponse | null = null;

    constructor(
        private fb: FormBuilder,
        private paymentService: PaymentService,
        private cdr: ChangeDetectorRef 
    ) {
        this.paymentForm = this.fb.group({
            amount: ['', [Validators.required, Validators.min(1)]],
            customerName: ['', Validators.required],
            email: ['', [Validators.required, Validators.email]],
            contactNumber: ['', [Validators.required, Validators.pattern(/^\d{10}$/)]]
        });
    }

    ngOnInit(): void {}

    onSubmit() {
        if (this.paymentForm.valid) {
            this.isLoading = true;
            const formData = this.paymentForm.value;
            this.paymentService.initiatePayment(formData).subscribe({
                next: (response: PaymentResponse) => {
                    this.paymentResponse = response;
                    this.isLoading = false;
                    this.cdr.detectChanges();
                },
                error: () => {
                    this.isLoading = false;
                    this.paymentResponse = {
                        paymentId: '',
                        message: 'Payment initiation failed',
                        status: 'failure'
                    };
                    this.cdr.detectChanges(); 
                    console.error('Payment initiation failed');
                }
            });
        }
    }
}