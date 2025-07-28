import { PaymentModel } from './payment';

export interface SubscriptionModel {
  id: string;
  payment: PaymentModel;
  subscriptionPlan: SubscriptionPlanModel;
  startDate: Date;
  endDate: Date;
  status: 'Active' | 'Inactive' | 'Cancelled' | 'Expired' | 'Pending';
}

export interface SubscriptionPlanModel {
  id: string;
  name: string;
  price: number;
  description: string;
  features?: string[];
  durationInDays: number;
  highlight?: boolean;
}
