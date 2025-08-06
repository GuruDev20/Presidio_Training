import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { ApiResponse } from "../models/api.model";
import { Observable } from "rxjs";
import { HttpClient } from "@angular/common/http";

@Injectable({ providedIn: 'root' })
export class AdminService {
    private baseUrl = environment.apiUrl;

    constructor(private http: HttpClient) {}

    getOverview(): Observable<ApiResponse<any>> {
        return this.http.get<ApiResponse<any>>(`${this.baseUrl}/admin/dashboard/overview`);
    }

    getDeactivationRequests(): Observable<ApiResponse<any>> {
        return this.http.get<ApiResponse<any>>(`${this.baseUrl}/admin/dashboard/deactivation-requests`);
    }

    getTicketGrowth(filter: string): Observable<ApiResponse<any>> {
        return this.http.get<ApiResponse<any>>(`${this.baseUrl}/admin/dashboard/ticket-growth?filter=${filter}`);
    }

    getTicketDetails(page: number, pageSize: number): Observable<ApiResponse<any>> {
        return this.http.get<ApiResponse<any>>(`${this.baseUrl}/admin/dashboard/tickets?page=${page}&pageSize=${pageSize}`);
    }

    getAgentDetails(): Observable<ApiResponse<any>> {
        return this.http.get<ApiResponse<any>>(`${this.baseUrl}/admin/dashboard/agents`);
    }

    createAgent(agentData: any): Observable<any> {
        return this.http.post(`${this.baseUrl}/admin/dashboard/create-agent`, agentData);
    }

    updateAgent(agentData: any): Observable<any> {
        return this.http.put(`${this.baseUrl}/admin/dashboard/update-agent`, agentData);
    }

    deleteAgent(agentId: string): Observable<any> {
        return this.http.delete(`${this.baseUrl}/admin/dashboard/delete-agent/${agentId}`);
    }
    
    updateDeactivationRequestStatus(userId: string, status: string): Observable<ApiResponse<any>> {
        return this.http.put<ApiResponse<any>>(
            `${this.baseUrl}/admin/dashboard/deactivation-request/${userId}`,
            `"${status}"`,
            { headers: { 'Content-Type': 'application/json' } }
        );
    }
}