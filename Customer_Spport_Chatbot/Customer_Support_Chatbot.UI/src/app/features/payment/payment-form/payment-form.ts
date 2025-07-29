import { Component, Input, Output, EventEmitter } from '@angular/core';
import {
  FormGroup,
  FormBuilder,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { RazorpayService } from '../../../services/razorpay.service';
import { CommonModule } from '@angular/common';
import { OrderService } from '../../../services/order.service';
import { AuthService } from '../../../services/auth.service';
import { SubscriptionPlanModel } from '../../../models/subscription.model';

@Component({
  selector: 'app-payment-form',
  templateUrl: './payment-form.html',
  styleUrls: ['./payment-form.css'],
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
})
export class PaymentFormComponent {
  @Input() subscriptionPlan: SubscriptionPlanModel | null = null;
  @Output() close = new EventEmitter<void>();
  paymentForm: FormGroup;
  isLoading: boolean = false;
  userId: string | null = null;

  constructor(
    private fb: FormBuilder,
    private razorpayService: RazorpayService,
    private orderService: OrderService,
    private authService: AuthService
  ) {
    this.paymentForm = this.fb.group({
      customerName: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      contactNumber: [
        '',
        [Validators.required, Validators.pattern('^[0-9]{10,}$')],
      ],
      startDate: ['', Validators.required],
      endDate: ['', Validators.required],
    });
  }

  ngOnInit(): void {
    const userId = this.authService.getUserId();
    if (userId) {
      this.userId = userId;
      console.log('User ID:', this.userId);
    } else {
      console.error('User ID not found');
    }
  }

  async pay() {
    if (this.paymentForm.invalid) {
      this.paymentForm.markAllAsTouched();
      return;
    }
    this.isLoading = true;
    const { customerName, email, contactNumber, startDate, endDate } =
      this.paymentForm.value;
    try {
      const order = await this.orderService
        .createOrder(
          this.subscriptionPlan?.price!,
          customerName,
          email,
          contactNumber
        )
        .subscribe({
          next: (order) => {
            console.log('Order created successfully:', order);
            this.razorpayService.initiateTransaction(
              this.userId,
              this.subscriptionPlan?.price!,
              customerName,
              email,
              contactNumber,
              order.data.razorpayOrderId!,
              order.data.id,
              this.subscriptionPlan?.id!,
              startDate,
              endDate
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
