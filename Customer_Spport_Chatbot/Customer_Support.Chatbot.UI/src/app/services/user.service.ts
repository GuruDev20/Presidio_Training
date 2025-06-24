import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { HttpClient, HttpParams } from "@angular/common/http";
import { Observable } from "rxjs";
import { TicketHistoryFilter } from "../models/ticket.model";

@Injectable({ providedIn: 'root' })
export class UserService{

    private baseUrl=environment.apiUrl;

    constructor(private http:HttpClient){}

    getTicketsByUser(filter:TicketHistoryFilter):Observable<{success: boolean, message: string, data: any[]}>{
        let params=new HttpParams()
            .set('userOrAgentId', filter.userOrAgentId)
            .set('role', filter.role);
        
        if(filter.searchKeyword){
            params=params.set('searchKeyword', filter.searchKeyword);
        }
        if(filter.timeRange){
            params=params.set('timeRange', filter.timeRange);
        }
        return this.http.get<{success: boolean, message: string, data: any[]}>(`${this.baseUrl}/tickets/history`, { params });
    }
}