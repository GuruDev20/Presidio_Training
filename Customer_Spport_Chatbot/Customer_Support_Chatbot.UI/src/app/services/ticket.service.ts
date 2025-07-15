import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { HttpClient } from "@angular/common/http";
import { CreateTicket, TicketResponse } from "../models/ticket.model";
import { Observable } from "rxjs";
import { ApiResponse } from "../models/api.model";

@Injectable({providedIn: "root"})
export class TicketService{

    private baseUrl=environment.apiUrl;

    constructor(private http:HttpClient){}

    createTicket(payload:CreateTicket):Observable<ApiResponse<TicketResponse>>{
        return this.http.post<ApiResponse<TicketResponse>>(`${this.baseUrl}/tickets/create`, payload);
    }

    endTicket(ticketId:string):Observable<ApiResponse<string>>{
        return this.http.put<ApiResponse<string>>(`${this.baseUrl}/tickets/end/${ticketId}`, {});
    }
}