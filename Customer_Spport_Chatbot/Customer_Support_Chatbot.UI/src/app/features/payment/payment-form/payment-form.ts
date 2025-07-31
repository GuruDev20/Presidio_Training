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
  @Output() subscriptionCreated = new EventEmitter<any>();
  paymentForm: FormGroup;
  isLoading: boolean = false;
  userId: string | null = null;
  endMonth: string = '';
  calculatedAmount: number = 0;
  today: Date = new Date();
  minEndMonth: string = '';
  monthOptions: { value: string; label: string }[] = [];

  constructor(
    private fb: FormBuilder,
    private razorpayService: RazorpayService,
    private orderService: OrderService,
    private authService: AuthService
  ) {
    this.userId = this.authService.getUserId();

    console.log('Initializing PaymentFormComponent with userId:', this.userId);
    this.paymentForm = this.fb.group({
      customerName: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      contactNumber: [
        '',
        [Validators.required, Validators.pattern('^[0-9]{10,}$')],
      ],
      endMonth: ['', Validators.required],
    });
    this.generateMonthOptions();
    this.setMinEndMonth();
  }

  setMinEndMonth() {
    const year = this.today.getFullYear();
    const month = (this.today.getMonth() + 1).toString().padStart(2, '0');
    this.minEndMonth = `${year}-${month}`;
  }

  generateMonthOptions() {
    const options = [];
    const now = new Date();
    for (let i = 1; i <= 24; i++) {
      const date = new Date(now.getFullYear(), now.getMonth() + i, 1);
      const value = `${date.getFullYear()}-${(date.getMonth() + 1)
        .toString()
        .padStart(2, '0')}`;
      const label = date.toLocaleString('default', {
        month: 'long',
        year: 'numeric',
      });
      options.push({ value, label });
    }
    this.monthOptions = options;
  }

  onMonthChange() {
    this.endMonth = this.paymentForm.get('endMonth')?.value;
    this.updateCalculatedAmount();
  }

  getMonthsDiff(): number {
    if (!this.endMonth) return 0;
    const [startY, startM] = [this.today.getFullYear(), this.today.getMonth() + 1];
    const [endY, endM] = this.endMonth.split('-').map(Number);
    // If end month is current month or next month, always 1 month
    if (
      (endY === startY && endM === startM) ||
      (endY === startY && endM === startM + 1) ||
      (endY === startY + 1 && startM === 12 && endM === 1)
    ) {
      return 1;
    }
    return (endY - startY) * 12 + (endM - startM) + 1;
  }

  updateCalculatedAmount() {
    const months = this.getMonthsDiff();
    this.calculatedAmount = this.subscriptionPlan
      ? this.subscriptionPlan.price * months
      : 0;
  }

  getStartDate(): string {
    return this.today.toISOString().slice(0, 10);
  }

  getEndDate(): string | null {
    if (!this.endMonth) return null;
    const [year, month] = this.endMonth.split('-').map(Number);
    const lastDay = new Date(year, month, 0).getDate();
    return `${year}-${month.toString().padStart(2, '0')}-${lastDay}`;
  }

  async pay() {
    if (this.paymentForm.invalid) {
      this.paymentForm.markAllAsTouched();
      return;
    }
    this.isLoading = true;
    const { customerName, email, contactNumber, endMonth } = this.paymentForm.value;
    this.endMonth = endMonth;
    const startDate = this.getStartDate();
    const endDate = this.getEndDate();
    const amount = this.calculatedAmount;
    try {
      const order = await this.orderService
        .createOrder(amount, customerName, email, contactNumber)
        .subscribe({
          next: async (order) => {
            console.log('Order created successfully:', order);
            await this.razorpayService.initiateTransaction(
              this.userId,
              amount,
              customerName,
              email,
              contactNumber,
              order.data.razorpayOrderId!,
              order.data.id,
              this.subscriptionPlan?.id!,
              new Date(startDate!),
              new Date(endDate!)
            );
            this.subscriptionCreated.emit({
              success: true,
              plan: this.subscriptionPlan,
            });
          },
          error: (error) => {
            console.error('Error creating order:', error);
            alert('Failed to create order. Please try again.');
            this.subscriptionCreated.emit({ success: false });
          },
        });
    } catch (error) {
      alert('Payment failed or cancelled.');
      this.subscriptionCreated.emit({ success: false });
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

