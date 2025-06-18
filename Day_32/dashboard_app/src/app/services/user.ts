import { Injectable, signal } from '@angular/core';
import { User } from '../models/user.model';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, map, Observable, tap } from 'rxjs';

@Injectable({providedIn: 'root'})
export class UserService {

    private apiurl='https://dummyjson.com/users';
    private users$=new BehaviorSubject<User[]>([]);

    filters=signal<{gender?:string,role?:string,country?:string}>({});

    constructor(private http:HttpClient){}

    addUser(user:User):Observable<User>{
        return this.http.post<User>(`${this.apiurl}/add`, user).pipe(
            map(_ => ({
                ...user,
                id:Date.now(),
                gender: user.gender ?? '',
                role:user.role ?? '',
                address: {
                    state: user.address?.state ?? ''
                }
            })),
            tap((newUser) => {
                const current = this.users$.value;
                this.users$.next([...current, newUser]);
            })
        );
    }

    getUsers(): Observable<User[]> {
        return this.http.get<{users: User[]}>(this.apiurl).pipe(
            map(res => res.users),
            tap(apiUsers => {
                const currentUsers = this.users$.value;
                const localUsers = currentUsers.filter(user => user.id === undefined || user.id >= 1000);
                const mergedUsers = [...apiUsers, ...localUsers];
                this.users$.next(mergedUsers);
            })
        );
    }

    get users():Observable<User[]>{
        return this.users$.asObservable();
    }

    applyFilters(filter:{gender?:string,role?:string,country?:string}){
        this.filters.set(filter);
    }
}
