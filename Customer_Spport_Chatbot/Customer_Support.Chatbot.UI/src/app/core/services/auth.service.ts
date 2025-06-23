import { Injectable } from "@angular/core";
import { environment } from "../../../environments/environment";
import { BehaviorSubject, Observable, tap } from "rxjs";
import { ApiResponse, AuthResponse, LoginRequest, RegisterRequest } from "../models/auth.model";
import { HttpClient } from "@angular/common/http";

@Injectable({providedIn: 'root'})
export class AuthService{

    private authUrl=`${environment.apiUrl}`;
    private _users$=new BehaviorSubject<AuthResponse|null>(null);
    users$=this._users$.asObservable();

    constructor(private http:HttpClient){}

    login(payload:LoginRequest):Observable<ApiResponse<AuthResponse>>{
        return this.http.post<ApiResponse<AuthResponse>>(`${this.authUrl}/auth/login`, payload).pipe(
            tap((res)=>{
                const user = res.data;
                this._users$.next(user);

                localStorage.setItem('accessToken', user.accessToken);
                localStorage.setItem('refreshToken', user.refreshToken);
                localStorage.setItem('userRole', user.role);
                localStorage.setItem('userId', user.userId);
                localStorage.setItem('accessTokenExpiry', new Date(user.expiresMinutes).toISOString());
            })
        );
    }

    register(payload:RegisterRequest):Observable<any>{
        return this.http.post(`${this.authUrl}/users/register`, payload);
    }

    logout(){
        this._users$.next(null);
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
    }

    getToken(){
        return localStorage.getItem('accessToken');
    }

    getRole(){
        return localStorage.getItem('userRole');
    }
}