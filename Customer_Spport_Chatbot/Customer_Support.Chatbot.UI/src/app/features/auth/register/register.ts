import { Component, OnInit } from "@angular/core";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { AuthService } from "../../../services/auth.service";
import { Router, RouterLink } from "@angular/router";
import { CommonModule } from "@angular/common";
import { validUsername } from "../../../misc/username.validator";
import { passwordMatcher } from "../../../misc/passwordMatcher.validator";
import { ToastService } from "../../../services/toast.service";

@Component({
    selector: 'app-register',
    templateUrl: './register.html',
    imports:[ReactiveFormsModule,CommonModule,RouterLink],
    standalone: true
})
export class Register implements OnInit{
    
    form!:FormGroup;

    constructor(private fb:FormBuilder,private auth:AuthService,private router:Router,private toastService:ToastService){}

    ngOnInit(): void {
        this.form = this.fb.group({
            username:['',[Validators.required, Validators.minLength(3),validUsername()]],
            email:['',[Validators.required, Validators.email]],
            password:['',[Validators.required, Validators.minLength(6)]],
            confirmPassword:['',Validators.required]
        }, { validators: passwordMatcher() })
    }

    get formValue(){
        return{
            username: this.form.value.username || '',
            email: this.form.value.email || '',
            password: this.form.value.password || '',
            confirmPassword: this.form.value.confirmPassword || ''
        };
    }


    get passwordMismatch() {
        return this.form.errors?.['passwordMismatch'] &&
        this.form.get('confirmPassword')?.touched;
    }
    submit(){
        if(this.form.invalid){
            return;
        }
        this.auth.register(this.formValue).subscribe({
            next:()=>{
                this.toastService.show('Registration successful!', 'success');
                this.router.navigate(['/user/dashboard']);
            },
            error:(err)=>{
                console.error('Login failed', err);
            }
        })
    }
}