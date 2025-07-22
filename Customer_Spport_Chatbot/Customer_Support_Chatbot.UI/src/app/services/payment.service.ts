import { Inject, inject, Injectable, model } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class PaymentService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  createPayment(
    amount: number,
    currency: string,
    status: string,
    orderId: string,
    razorpayPaymentId: string
  ): Observable<any> {
    console.log('Creating payment with details:', {
      amount,
      currency,
      status,
      orderId,
      razorpayPaymentId,
    });
    return this.http.post<any>(`${this.baseUrl}/payments`, {
      amount,
      currency,
      status,
      orderId,
      razorpayPaymentId,
    });
  }

  getAllPayments(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/payments`);
  }

  getPaymentById(id: number): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/payments/${id}`);
  }
}
