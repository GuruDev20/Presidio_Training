import { Component, OnInit } from "@angular/core";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { AuthService } from "../../../services/auth.service";
import { Router, RouterLink, RouterOutlet } from "@angular/router";
import { CommonModule } from "@angular/common";
import { ToastService } from "../../../services/toast.service";

@Component({
    selector: 'app-login',
    templateUrl: './login.html',
    imports:[ReactiveFormsModule,CommonModule,RouterLink],
    standalone: true
})
export class Login implements OnInit{
    
    form!:FormGroup;

    constructor(private fb:FormBuilder,private auth:AuthService,private router:Router,private toastService:ToastService){}

    ngOnInit(): void {
        this.form = this.fb.group({
            email:['',[Validators.required, Validators.email]],
            password:['',[Validators.required, Validators.minLength(6)]]
        })
    }

    get formValue(){
        return{
            email: this.form.value.email || '',
            password: this.form.value.password || ''
        };
    }
    
    submit(){
        if(this.form.invalid){
            return;
        }
        this.auth.login(this.formValue).subscribe({
            next:()=>{
                const role = this.auth.getRole();
                this.toastService.show('Login successful', 'success');
                switch(role){
                    case 'User':
                        this.router.navigate(['/user/dashboard/overview']);
                        break;
                    case 'Admin':
                        this.router.navigate(['/admin/dashboard']);
                        break;
                    case 'Agent':
                        this.router.navigate(['/agent/dashboard']);
                        break;
                    default:
                        this.router.navigate(['/login']);
                }
            },
            error:(err)=>{
                console.error('Login failed', err);
                this.toastService.show('Login failed. Please check your credentials.', 'error');
            }
        })
    }
}