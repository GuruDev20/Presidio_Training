import { Injectable } from "@angular/core";
import { PaymentResponse } from "../models/payment.model";
import { BehaviorSubject } from "rxjs";

@Injectable({providedIn: 'root'})

export class PaymentHistoryService{
    private readonly STORAGE_KEY = 'payment_history';
    private historySubject = new BehaviorSubject<PaymentResponse[]>(this.getPaymentHistory());

    addPayment(response: PaymentResponse): void {
        const history = this.getPaymentHistory();
        history.push(response);
        localStorage.setItem(this.STORAGE_KEY, JSON.stringify(history));
        this.historySubject.next(history);
    }

    getPaymentHistory(): PaymentResponse[] {
        const history = localStorage.getItem(this.STORAGE_KEY);
        return history ? JSON.parse(history) : [];
    }

    getHistoryObservable() {
        return this.historySubject.asObservable();
    }
}