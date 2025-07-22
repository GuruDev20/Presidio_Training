import { Component, Input } from '@angular/core';
import {
  FormGroup,
  FormBuilder,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { RazorpayService } from '../../../services/razorpay.service';
import { CommonModule } from '@angular/common';
import { OrderService } from '../../../services/order.service';

@Component({
  selector: 'app-payment-form',
  templateUrl: './payment-form.html',
  styleUrls: ['./payment-form.css'],
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
})
export class PaymentFormComponent {
  @Input() amount: number = 0;
  paymentForm: FormGroup;
  isLoading: boolean = false;

  constructor(
    private fb: FormBuilder,
    private razorpayService: RazorpayService,
    private orderService: OrderService
  ) {
    this.paymentForm = this.fb.group({
      customerName: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      contactNumber: [
        '',
        [Validators.required, Validators.pattern('^[0-9]{10,}$')],
      ],
    });
  }

  async pay() {
    if (this.paymentForm.invalid) {
      this.paymentForm.markAllAsTouched();
      return;
    }
    this.isLoading = true;
    const { customerName, email, contactNumber } = this.paymentForm.value;
    try {
      const order = await this.orderService
        .createOrder(this.amount, customerName, email, contactNumber)
        .subscribe({
          next: (order) => {
            console.log('Order created successfully:', order);
            this.razorpayService.initiateTransaction(
              this.amount,
              customerName,
              email,
              contactNumber,
              order.razorpayOrderId!,
              order.id
            );
          },
          error: (error) => {
            console.error('Error creating order:', error);
            alert('Failed to create order. Please try again.');
          },
        });
    } catch (error) {
      alert('Payment failed or cancelled.');
    } finally {
      this.isLoading = false;
    }
  }

  get() {
    return this.paymentForm.get('');
  }
  get customerName() {
    return this.paymentForm.get('customerName');
  }
  get email() {
    return this.paymentForm.get('email');
  }
  get contactNumber() {
    return this.paymentForm.get('contactNumber');
  }
}
