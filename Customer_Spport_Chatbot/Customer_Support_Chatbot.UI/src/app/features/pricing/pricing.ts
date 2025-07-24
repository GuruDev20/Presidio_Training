import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { PaymentFormComponent } from '../payment/payment-form/payment-form';
import { SubscriptionPlanModel } from '../../models/subscription.model';

@Component({
  selector: 'app-pricing',
  imports: [CommonModule, PaymentFormComponent],
  templateUrl: './pricing.html',
  styleUrl: './pricing.css',
})
export class Pricing {
  subscriptionPlans: SubscriptionPlanModel[] = [
    {
      id: '0',
      name: 'Basic',
      price: 0,
      description: 'Free plan with limited features',
      features: [
        'Standard support for text chat',
        'Standard agent assignment speed',
        'Standard agent relevance',
      ],
      durationInDays: 30,
      highlight: true,
    },
    {
      id: '1',
      name: 'Pro',
      price: 19,
      description: 'Pro plan with extended features',
      features: [
        'Support for media types (video, audio, documents)',
        'Faster agent assignment',
        'Increased agent relevance',
        'Extended chat retention time',
      ],
      durationInDays: 30,
      highlight: true,

    },
    {
      id: '2',
      name: 'Business',
      price: 49,
      description: 'Business plan with advanced features',
      features: [
        'Support for media types (video, audio, documents)',
        'Faster agent assignment',
        'Increased agent relevance',
        'Extended chat retention time',
      ],
      durationInDays: 30,
      highlight: true,

    },
  ];

  showPaymentDialog = false;
  selectedPlan: SubscriptionPlanModel | null = null;

  openPaymentDialog(plan: SubscriptionPlanModel) {
    this.selectedPlan = plan;
    this.showPaymentDialog = true;
  }

  closePaymentDialog() {
    this.showPaymentDialog = false;
    this.selectedPlan = null;
  }

  getDurationInMonths(days: number): string {
    return `${Math.ceil(days / 30) > 1 ? Math.ceil(days / 30) : ''} month${days > 30 ? 's' : ''}`;
  }
}
