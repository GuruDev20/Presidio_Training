import { Inject, inject, Injectable, model } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { ApiResponse } from '../models/api.model';
import { PaymentModel } from '../models/payment';

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
  ): Observable<ApiResponse<PaymentModel>> {
    console.log('Creating payment with details:', {
      amount,
      currency,
      status,
      orderId,
      razorpayPaymentId,
    });
    return this.http.post<ApiResponse<PaymentModel>>(
      `${this.baseUrl}/payments`,
      {
        amount,
        currency,
        status,
        orderId,
        razorpayPaymentId,
      }
    );
  }

  getAllPayments(): Observable<PaymentModel[]> {
    return this.http
      .get<ApiResponse<PaymentModel[]>>(`${this.baseUrl}/payments`)
      .pipe(map((response) => response.data));
  }

  getPaymentById(id: number): Observable<PaymentModel> {
    return this.http
      .get<ApiResponse<PaymentModel>>(`${this.baseUrl}/payments/${id}`)
      .pipe(map((response) => response.data));
  }
}
