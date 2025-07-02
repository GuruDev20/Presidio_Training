import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { HttpClient, HttpParams } from "@angular/common/http";
import { TicketHistoryFilter, TicketHistoryResponse } from "../models/ticket.model";
import { Observable } from "rxjs";
import { ApiResponse } from "../models/api.model";

@Injectable({providedIn: 'root'})

export class UserService{

    private baseurl=environment.apiUrl;
    constructor(private http:HttpClient){}

    getTicketsByUser(filter:TicketHistoryFilter):Observable<ApiResponse<TicketHistoryResponse[]>>{
        let params=new HttpParams()
            .set('userOrAgentId', filter.userOrAgentId)
            .set('role', filter.role);

        if(filter.searchKeyword){
            params=params.set('searchKeyword', filter.searchKeyword);        
        }
        if(filter.timeRange){
            params=params.set('timeRange', filter.timeRange);
        }
        return this.http.get<ApiResponse<TicketHistoryResponse[]>>(`${this.baseurl}/tickets/history`, {params});
    }

    getChatSession(ticketId: string): Observable<ApiResponse<any>> {
        return this.http.get<ApiResponse<any>>(`${this.baseurl}/tickets/${ticketId}/chat-session`);
    }
}