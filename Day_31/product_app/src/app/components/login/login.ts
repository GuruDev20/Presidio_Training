import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
    selector: 'app-login',
    standalone: true,
    imports: [CommonModule,FormsModule],
    templateUrl: './login.html',
    styleUrl: './login.css'
})
export class Login {
    username='';
    password='';
    error='';

    constructor(private router:Router){}

    login(){
        if(this.username==="Dev" && this.password==="Dev123"){
            localStorage.setItem('authToken','sampleAuthToken');
            this.router.navigate(['/products']);
        }
        else {
            this.error='Invalid username or password';
        }
    }
}
