import { Injectable } from '@angular/core';
import { User } from '../models/user.model';

@Injectable({
    providedIn: 'root'
})

export class AuthService {
    private users: User[] = [
        { email: 'admin@gmail.com', password: 'admin123' },
        { email: 'guru01803@gmail.com', password: 'GuruDev@2003' }
    ];

    login(user: User): boolean {
        return this.users.some(u => u.email === user.email && u.password === user.password);
    }

    storeUser(user: User): void {
        sessionStorage.setItem('currentUser', JSON.stringify(user));
    }

    getUser(): User | null {
        const data = sessionStorage.getItem('currentUser');
        return data ? JSON.parse(data) : null;
    }

    logout(): void {
        sessionStorage.removeItem('currentUser');
    }
}
