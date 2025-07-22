import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { PaymentFormComponent } from '../payment/payment-form/payment-form';

@Component({
  selector: 'app-pricing',
  imports: [CommonModule, PaymentFormComponent],
  templateUrl: './pricing.html',
  styleUrl: './pricing.css',
})
export class Pricing {
  tiers = [
    {
      name: 'Basic',
      price: 'Free',
      amount: 0,
      features: [
        'Standard support for text chat',
        'Standard agent assignment speed',
        'Standard agent relevance',
      ],
      highlight: false,
    },
    {
      name: 'Pro',
      price: '$19/month',
      amount: 19,
      features: [
        'Support for media types (video, audio, documents)',
        'Faster agent assignment',
        'Increased agent relevance',
        'Extended chat retention time',
      ],
      highlight: true,
    },
    {
      name: 'Premium',
      price: '$49/month',
      amount: 49,
      features: [
        'All Pro benefits',
        'Priority support (highest agent assignment speed)',
        'Longest chat retention time',
        'Dedicated account manager',
        'Early access to new features',
      ],
      highlight: false,
    },
  ];

  showPaymentDialog = false;
  selectedAmount: number | null = null;

  openPaymentDialog(amount: number) {
    this.selectedAmount = amount;
    this.showPaymentDialog = true;
  }

  closePaymentDialog() {
    this.showPaymentDialog = false;
    this.selectedAmount = null;
  }
}
