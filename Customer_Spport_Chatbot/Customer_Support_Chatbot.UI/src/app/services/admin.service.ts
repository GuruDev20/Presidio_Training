import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { ApiResponse } from "../models/api.model";
import { Observable } from "rxjs";
import { HttpClient } from "@angular/common/http";

@Injectable({providedIn: 'root'})

export class AdminService{

    private baseUrl=environment.apiUrl;

    constructor(private http:HttpClient){}

    getOverview(): Observable<ApiResponse<any>> {
        return this.http.get<ApiResponse<any>>(`${this.baseUrl}/admin/dashboard/overview`);
    }
}