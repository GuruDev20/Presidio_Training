import { Inject, inject, Injectable, model } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { ApiResponse } from '../models/api.model';
import { SubscriptionPlanModel } from '../models/subscription.model';

@Injectable({ providedIn: 'root' })
export class SubscriptionPlanService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  createSubscriptionPlan(
    name: string,
    price: number,
    description: string,
    features: string[],
    durationInDays: number
  ): Observable<ApiResponse<SubscriptionPlanModel>> {
    console.log('Creating subscription plan with details:', {
      name,
      price,
      description,
      features,
      durationInDays,
    });
    return this.http.post<any>(`${this.baseUrl}/subscriptionPlans`, {
      name,
      price,
      description,
      features,
      durationInDays,
    });
  }

  getAllSubscriptionPlans(): Observable<ApiResponse<SubscriptionPlanModel[]>> {
    return this.http.get<ApiResponse<SubscriptionPlanModel[]>>(
      `${this.baseUrl}/subscriptionPlans`
    );
  }

  getSubscriptionPlanById(
    id: number
  ): Observable<ApiResponse<SubscriptionPlanModel>> {
    return this.http.get<ApiResponse<SubscriptionPlanModel>>(
      `${this.baseUrl}/subscriptionPlans/${id}`
    );
  }
}
