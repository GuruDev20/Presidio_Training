import { Injectable } from '@angular/core';
import { HttpErrorResponse, HttpEvent,HttpHandler,HttpInterceptor,HttpRequest} from '@angular/common/http';
import { BehaviorSubject, catchError, filter, Observable, switchMap, take, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

    private isRefreshing=false;
    private refreshTokenSubject:BehaviorSubject<string|null>=new BehaviorSubject<string|null>(null);
    constructor(private authService: AuthService) {}

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        const token = this.authService.getToken();

        let authReq=req;
        if(token){
            authReq=this.addTokenHeader(req, token);
        }
        return next.handle(authReq).pipe(
            catchError(error=>{
                if(error instanceof HttpErrorResponse && error.status===401){
                    return this.handle401Error(authReq, next);
                }
                return throwError(()=>error);
            })
        );
    }

    private addTokenHeader(request:HttpRequest<any>,token:string){
        return request.clone({
            setHeaders: {
                Authorization: `Bearer ${token}`
            }
        });
    }

    private handle401Error(request:HttpRequest<any>,next:HttpHandler){
        if(!this.isRefreshing){
            this.isRefreshing=true;
            this.refreshTokenSubject.next(null);

            return this.authService.refreshToken().pipe(
                switchMap((newToken:any)=>{
                    this.isRefreshing=false;
                    this.authService.setAccessToken(newToken.data.accessToken);
                    this.refreshTokenSubject.next(newToken.data.accessToken);
                    return next.handle(this.addTokenHeader(request, newToken.data.accessToken));
                }),
                catchError((err:any)=>{
                    this.isRefreshing=false;
                    this.authService.logout();
                    return throwError(() => err);
                })
            );
        }
        else{
            return this.refreshTokenSubject.pipe(
                filter(token => token !== null),
                take(1),
                switchMap((token)=>next.handle(this.addTokenHeader(request, token!)))
            )
        }
    }
}
