import { Injectable } from "@angular/core";
import { environment } from "../../../environments/environment";
import { BehaviorSubject, Observable, tap } from "rxjs";
import { AuthResponse, LoginRequest, RegisterRequest } from "../models/auth.mode";
import { HttpClient } from "@angular/common/http";

@Injectable({providedIn: 'root'})
export class AuthService{

    private authUrl=`${environment.apiUrl}/auth`;
    private _users$=new BehaviorSubject<AuthResponse|null>(null);
    users$=this._users$.asObservable();

    constructor(private http:HttpClient){}

    login(payload:LoginRequest):Observable<AuthResponse>{
        return this.http.post<AuthResponse>(`${this.authUrl}/login`, payload).pipe(
            tap((res)=>{
                this._users$.next(res);
                localStorage.setItem('accessToken',res.accessToken);
                localStorage.setItem('refreshToken',res.refreshToken);
                localStorage.setItem('userRole', res.role);
            })
        );
    }

    register(payload:RegisterRequest):Observable<any>{
        return this.http.post(`${this.authUrl}/register`, payload);
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