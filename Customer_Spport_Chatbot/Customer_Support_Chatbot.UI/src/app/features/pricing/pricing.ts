
import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { PaymentFormComponent } from '../payment/payment-form/payment-form';
import { SubscriptionPlanModel } from '../../models/subscription.model';
import { SubscriptionPlanService } from '../../services/subscription-plan.service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-pricing',
  imports: [CommonModule, PaymentFormComponent],
  templateUrl: './pricing.html',
  styleUrl: './pricing.css',
})
export class Pricing {
  subscriptionPlans: SubscriptionPlanModel[] = [];

  showPaymentDialog = false;
  selectedPlan: SubscriptionPlanModel | null = null;

  constructor(
    private subscriptionPlanService: SubscriptionPlanService,
    private router: Router,
    private toastr: ToastrService
  ) {
    this.loadSubscriptionPlans();
  }

  loadSubscriptionPlans() {
    this.subscriptionPlanService.getAllSubscriptionPlans().subscribe({
      next: (plans) => {
        console.log('Subscription plans loaded:', plans);
        // @ts-expect-error
        this.subscriptionPlans = (plans.data.$values || []).map(
          (plan: any) => ({
            ...plan,
            features: plan.features?.$values || [],
          })
        );
      },
      error: (err) => {
        console.error('Error loading subscription plans', err);
      },
    });
  }

  openPaymentDialog(plan: SubscriptionPlanModel) {
    this.selectedPlan = plan;
    this.showPaymentDialog = true;
  }

  closePaymentDialog() {
    this.showPaymentDialog = false;
    this.selectedPlan = null;
  }

  getDurationInMonths(days: number): string {
    return `${Math.ceil(days / 30) > 1 ? Math.ceil(days / 30) : ''} month${
      days > 30 ? 's' : ''
    }`;
  }

  onSubscriptionCreated(event: any) {
    if (event && event.success) {
      this.toastr.success('Subscription created successfully!', 'Success');
      this.closePaymentDialog();
      this.router.navigate(['user/dashboard/profile']);
    } else {
      this.toastr.error('Failed to create subscription.', 'Error');
    }
  }
}

