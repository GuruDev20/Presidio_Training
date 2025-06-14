import { Component, OnInit } from '@angular/core';
import { AuthService } from '../service/auth';
import { User } from '../models/user.model';
import { Router } from '@angular/router';

@Component({
    selector: 'app-dashboard',
    templateUrl: './dashboard.html'
})
export class DashboardComponent implements OnInit {
    currentUser: User | null = null;

    constructor(private authService: AuthService, private router: Router) {}

    ngOnInit(): void {
        this.currentUser = this.authService.getUser();
        if (!this.currentUser) {
        this.router.navigate(['/login']);
        }
    }

    logout(): void {
        this.authService.logout();
        this.router.navigate(['/login']);
    }
}
