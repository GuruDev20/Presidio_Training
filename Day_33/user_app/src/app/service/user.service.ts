import { BehaviorSubject, Observable } from "rxjs";
import { User } from "../models/user.model";
import { Injectable } from "@angular/core";

@Injectable({providedIn: 'root'})
export class UserService{
    private user$=new BehaviorSubject<User[]>([]);

    getUsers():Observable<User[]>{
        return this.user$.asObservable();
    }

    addUser(user:User){
        console.log('Adding user:', user);
        const current=this.user$.value;
        this.user$.next([...current,user]);
    }
}