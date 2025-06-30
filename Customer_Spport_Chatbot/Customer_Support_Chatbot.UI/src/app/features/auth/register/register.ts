import { Component, OnInit } from "@angular/core";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { AuthService } from "../../../services/auth.service";
import { Router, RouterLink } from "@angular/router";
import { CommonModule } from "@angular/common";
import { validUsername } from "../../../misc/username.validator";
import { passwordMatcher } from '../../../misc/passwordMatcher.validator';
import { LucideIconsModule } from "../../../utils/lucide-icons.module";

@Component({
    selector: 'app-register',
    templateUrl: './register.html',
    imports:[ReactiveFormsModule,CommonModule,RouterLink,LucideIconsModule],
    standalone: true
})
export class Register implements OnInit{
    
    form!:FormGroup;
    profilePreview:string | ArrayBuffer | null = null;

    constructor(private fb:FormBuilder,private auth:AuthService,private router:Router){}

    ngOnInit(): void {
        this.form = this.fb.group({
            username:['',[Validators.required, Validators.minLength(3),validUsername()]],
            email:['',[Validators.required, Validators.email]],
            password:['',[Validators.required, Validators.minLength(6)]],
            confirmPassword:['',Validators.required],
            profilePictureUrl:['']
        }, { validators: passwordMatcher() })
    }

    onImageSelected(event: Event): void{
        const file=(event.target as HTMLInputElement).files?.[0];
        if(file){
            const reader=new FileReader();
            reader.onload=()=>{
                this.profilePreview = reader.result;
                this.form.patchValue({ profilePictureUrl: reader.result });
            }
            reader.readAsDataURL(file);
        }
    }

    get formValue(){
        return{
            username: this.form.value.username || '',
            email: this.form.value.email || '',
            password: this.form.value.password || '',
            confirmPassword: this.form.value.confirmPassword || '',
            profilePictureUrl:this.form.value.profilePictureUrl || ''
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
                this.router.navigate(['/user/dashboard/overview']);
            },
            error:(err)=>{
                console.error('Login failed', err);
            }
        })
    }
}