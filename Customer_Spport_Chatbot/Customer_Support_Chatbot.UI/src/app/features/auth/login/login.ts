import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../../services/auth.service';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-login',
    imports: [CommonModule,ReactiveFormsModule,RouterModule],
    templateUrl: './login.html',
    styleUrl: './login.css'
})
export class Login implements OnInit{

    form!:FormGroup;

    constructor(private fb:FormBuilder,private authService:AuthService,private router:Router){}

    ngOnInit(): void {
        this.form=this.fb.group({
            email:['',[Validators.required, Validators.email]],
            password:['',[Validators.required, Validators.minLength(6)]]
        })
    }

    get formValue(){
        return{
            email:this.form.value.email || '',
            password:this.form.value.password || ''
        }
    }

    submit(){
        if(this.form.invalid){
            return;
        }
        this.authService.login(this.formValue).subscribe({
            next:()=>{
                const role=this.authService.getRole();
                switch(role){
                    case 'User':
                        this.router.navigate(['/user/dashboard/overview']);
                        break;
                    case 'Admin':
                        this.router.navigate(['/admin/dashboard/overview']);
                        break;
                    case 'Agent':
                        this.router.navigate(['/agent/dashboard/overview']);
                        break;
                    default:
                        console.error('Unknown role:', role);
                        break;
                }
            },
            error:(err)=>{
                console.error('Login failed:', err);
            }
        })
    }
}
