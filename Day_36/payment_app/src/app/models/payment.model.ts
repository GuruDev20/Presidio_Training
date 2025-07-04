export interface PaymentForm{
    amount:number;
    customerName:string;
    email:string;
    contactNumber:string;
}

export interface PaymentResponse{
    paymentId:string;
    message:string;
    status:'success' | 'failure' | 'cancelled';
}