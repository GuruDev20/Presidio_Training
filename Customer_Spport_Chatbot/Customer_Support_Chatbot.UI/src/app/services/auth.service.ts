import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { BehaviorSubject, Observable, tap } from "rxjs";
import { AuthResponse, LoginRequest, RegisterRequest, UserDevice, UserProfile } from "../models/auth.model";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { ApiResponse } from "../models/api.model";

@Injectable({ providedIn: 'root' })
export class AuthService {
    private baseUrl = environment.apiUrl;
    private _users$ = new BehaviorSubject<AuthResponse | null>(null);
    users$ = this._users$.asObservable();
    username: string | null = null;

    constructor(private http: HttpClient) {}

    login(payload: LoginRequest): Observable<ApiResponse<AuthResponse>> {
        const headers = new HttpHeaders({
            'User-Agent': navigator.userAgent
        });
        return this.http.post<ApiResponse<AuthResponse>>(`${this.baseUrl}/auth/login`, payload, { headers }).pipe(
            tap((res) => {
                const user = res.data;
                this._users$.next(user);
                
                localStorage.setItem('accessToken', user.accessToken);
                localStorage.setItem('refreshToken', user.refreshToken);
                localStorage.setItem('userId', user.userId);
                localStorage.setItem('userRole', user.role);
                localStorage.setItem('accessTokenExpiry', new Date(user.expiresMinutes).toISOString());
                localStorage.setItem('deviceId', user.deviceId);
            })
        );
    }

    register(payload: RegisterRequest): Observable<any> {
        return this.http.post(`${this.baseUrl}/users/register`, payload);
    }

    logout(): void {
        this._users$.next(null);
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
        localStorage.removeItem('userId');
        localStorage.removeItem('userRole');
        localStorage.removeItem('accessTokenExpiry');
        localStorage.removeItem('deviceId');
    }

    getToken() {
        const token = localStorage.getItem('accessToken');
        return token ? token : '';
    }

    getRole() {
        const role = localStorage.getItem('userRole');
        return role ? role : '';
    }

    getUserId() {
        const userId = localStorage.getItem('userId');
        return userId ? userId : '';
    }

    refreshToken(): Observable<any> {
        const refreshToken = localStorage.getItem('refreshToken');
        return this.http.post<ApiResponse<AuthResponse>>(`${this.baseUrl}/auth/refresh`, { refreshToken });
    }

    setAccessToken(token: string) {
        localStorage.setItem('accessToken', token);
    }

    getCurrentUser(): Observable<ApiResponse<UserProfile>> {
        return this.http.get<ApiResponse<UserProfile>>(`${this.baseUrl}/auth/profile`).pipe(
            tap((res) => {
                const userProfile = res.data;
                this.username = userProfile.fullName;
            })
        );
    }

    getUsername() {
        return this.username || localStorage.getItem('username') || null;
    }

    getUserDevices(): Observable<ApiResponse<{ $values: UserDevice[] }>> {
        return this.http.get<ApiResponse<{ $values: UserDevice[] }>>(`${this.baseUrl}/auth/devices`);
    }

    logoutDevice(deviceId: string): Observable<any> {
        return this.http.post(`${this.baseUrl}/auth/devices/logout`, { deviceId });
    }

    isDeviceActive(deviceId: string): Observable<ApiResponse<boolean>> {
        return this.http.get<ApiResponse<boolean>>(`${this.baseUrl}/auth/devices/${deviceId}/active`);
    }
}