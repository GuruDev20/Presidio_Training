import { Injectable } from "@angular/core";
import { PaymentForm, PaymentResponse } from "../models/payment.model";
import { Observable } from "rxjs";
import { PaymentHistoryService } from "./paymentHistoryService.service";

@Injectable({ providedIn: 'root' })
export class PaymentService {
    constructor(private paymentHistoryService: PaymentHistoryService) {}

    initiatePayment(formData: PaymentForm): Observable<PaymentResponse> {
        const mockOrder = {
            amount: formData.amount * 100,
            currency: 'INR',
            status: 'created'
        };

        return new Observable(observer => {
            const script = document.createElement('script');
            script.src = 'https://checkout.razorpay.com/v1/checkout.js';
            script.onload = () => {
                const options = {
                    key: 'rzp_test_P1LFJmoUkDFVnz',
                    amount: mockOrder.amount,
                    currency: mockOrder.currency,
                    name: 'UPI Payment Demo',
                    description: 'Test Payment',
                    prefill: {
                        name: formData.customerName,
                        email: formData.email,
                        contact: formData.contactNumber,
                        method: 'upi',
                        vpa: 'failure@razorpay'
                    },
                    handler: (response: any) => {
                        console.log('Payment success handler triggered:', response);
                        const paymentResponse: PaymentResponse = {
                            paymentId: response.razorpay_payment_id || 'mock_pay_' + Math.random().toString(36).substr(2, 10),
                            status: 'success',
                            message: 'Payment completed successfully!'
                        };
                        this.paymentHistoryService.addPayment(paymentResponse);
                        observer.next(paymentResponse);
                        observer.complete();
                    },
                    modal: {
                        ondismiss: () => {
                            console.log('Payment modal dismissed'); 
                            const paymentResponse: PaymentResponse = {
                                paymentId: '',
                                status: 'cancelled',
                                message: 'Payment was cancelled.'
                            };
                            this.paymentHistoryService.addPayment(paymentResponse);
                            observer.next(paymentResponse);
                            observer.complete();
                        }
                    }
                };

                const rzp = new (window as any).Razorpay(options);
                rzp.on('payment.failed', (error: any) => {
                    console.log('Payment failed:', error);
                    const paymentResponse: PaymentResponse = {
                        paymentId: error.error.metadata?.payment_id || '',
                        status: 'failure',
                        message: error.error.description || 'Payment failed. Please try again.'
                    };
                    this.paymentHistoryService.addPayment(paymentResponse);
                    observer.next(paymentResponse);
                    observer.complete();
                    rzp.close();
                });

                rzp.open();
            };
            script.onerror = () => {
                console.log('Failed to load Razorpay script'); 
                const paymentResponse: PaymentResponse = {
                    paymentId: '',
                    status: 'failure',
                    message: 'Failed to load Razorpay script.'
                };
                this.paymentHistoryService.addPayment(paymentResponse);
                observer.next(paymentResponse);
                observer.complete();
            };
            document.body.appendChild(script);
        });
    }
}