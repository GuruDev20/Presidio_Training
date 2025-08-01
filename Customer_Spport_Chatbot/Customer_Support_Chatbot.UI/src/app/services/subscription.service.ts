import { Inject, inject, Injectable, model } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class SubscriptionService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  createSubscription(
    userId: string,
    planId: string,
    paymentId: string,
    startDate: Date,
    endDate: Date
  ): Observable<any> {
    console.log('Creating subscription with details:', {
      userId,
      planId,
      paymentId,
      startDate,
      endDate,
    });
    return this.http.post<any>(`${this.baseUrl}/subscriptions`, {
      userId,
      planId,
      paymentId,
      startDate: new Date(startDate).toISOString(),
      endDate: new Date(endDate).toISOString(),
    });
  }

  getAllSubscriptions(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/subscriptions`);
  }

  getSubscriptionById(id: number): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/subscriptions/${id}`);
  }
}
