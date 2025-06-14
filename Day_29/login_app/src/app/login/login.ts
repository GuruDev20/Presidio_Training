import { Component } from '@angular/core';
import { User } from '../models/user.model';
import { AuthService } from '../service/auth';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-login',
    templateUrl: './login.html',
    imports: [FormsModule,CommonModule],
})
export class LoginComponent {
    user: User = { email: '', password: '' };
    errorMessage: string = '';

    constructor(private authService: AuthService, private router: Router) {}

    onSubmit(): void {
        if (this.authService.login(this.user)) {
        this.authService.storeUser(this.user);
        this.router.navigate(['/dashboard']);
        } else {
        this.errorMessage = 'Invalid credentials';
        }
    }
}
