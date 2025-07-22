import { PaymentModel } from './payment';

export class OrderModel {
  constructor(
    public id: string,
    public customerName?: string,
    public email?: string,
    public contactNumber?: string,
    public razorpayOrderId?: string,
    public payment?: PaymentModel
  ) {}
}
