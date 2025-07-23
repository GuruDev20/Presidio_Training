import { PaymentModel } from "./payment";

export interface SubscriptionModel {
    id: string;
    payment: PaymentModel;
    tier: string;
    startDate: Date;
    endDate: Date;
    status: 'Active' | 'Inactive' | 'Cancelled' | 'Expired' | 'Pending';
}