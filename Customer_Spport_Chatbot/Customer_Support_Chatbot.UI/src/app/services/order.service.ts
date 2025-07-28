import { Inject, inject, Injectable, model } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { OrderModel } from '../models/order';
import { environment } from '../../environments/environment';
import { ApiResponse } from '../models/api.model';

@Injectable({ providedIn: 'root' })
export class OrderService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  createOrder(
    amount: number,
    customerName: string,
    email: string,
    contactNumber: string
  ): Observable<ApiResponse<OrderModel>> {
    return this.http.post<ApiResponse<OrderModel>>(`${this.baseUrl}/orders`, {
      amount,
      customerName,
      email,
      contactNumber,
    });
  }

  getAllOrders(): Observable<OrderModel[]> {
    return this.http
      .get<ApiResponse<OrderModel[]>>(`${this.baseUrl}/orders`)
      .pipe(map((response) => response.data));
  }

  getOrderById(id: number): Observable<OrderModel> {
    return this.http
      .get<ApiResponse<OrderModel>>(`${this.baseUrl}/orders/${id}`)
      .pipe(map((response) => response.data));
  }
}
