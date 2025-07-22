import { Inject, inject, Injectable, model } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { OrderModel } from '../models/order';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class OrderService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  createOrder(
    amount: number,
    customerName: string,
    email: string,
    contactNumber: string
  ): Observable<OrderModel> {
    return this.http.post<any>(`${this.baseUrl}/orders`, {
      amount,
      customerName,
      email,
      contactNumber,
    });
  }

  getAllOrders(): Observable<OrderModel[]> {
    return this.http.get<OrderModel[]>(`${this.baseUrl}/orders`);
  }

  getOrderById(id: number): Observable<OrderModel> {
    return this.http.get<OrderModel>(`${this.baseUrl}/orders/${id}`);
  }
}
