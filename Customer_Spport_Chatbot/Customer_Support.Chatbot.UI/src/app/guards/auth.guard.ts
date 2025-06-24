import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, CanActivate, Router } from "@angular/router";
import { AuthService } from "../services/auth.service";

@Injectable({providedIn: 'root'})
export class AuthGurad implements CanActivate{

    constructor(private auth:AuthService,private router:Router){}

    canActivate(route:ActivatedRouteSnapshot):boolean{
        const token=this.auth.getToken();
        const role = this.auth.getRole();
        if(!token){
            this.router.navigate(['/login']);
            return false;
        }

        const url=route.url.map(s=>s.path).join('/');
        if(url.startsWith('admin') && role !=='Admin') return false;
        if(url.startsWith('agent') && role !=='Agent') return false;
        if(url.startsWith('user') && role !=='User') return false;
        return true;
    }
}