import { Component } from "@angular/core";
import { LucideIconsModule } from "../../../utils/lucide-icons.module";
import { CommonModule } from "@angular/common";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { UserService } from "../../../services/user.service";

@Component({
    selector: 'app-admin-settings',
    templateUrl: './admin-settings.component.html',
    standalone: true,
    imports:[LucideIconsModule,CommonModule,ReactiveFormsModule]
})

export class AdminSettingsComponent{

    isDrawerOpen = false;
    passwordForm:FormGroup;
    errorMessage: string = '';
    successMessage: string = '';

    constructor(private fb:FormBuilder,private userService: UserService) {
        this.passwordForm = this.fb.group({
            oldPassword: ['',[Validators.required]],
            newPassword: ['', [Validators.required, Validators.minLength(6)]],
            confirmPassword: ['', [Validators.required, Validators.minLength(6)]]
        }, { validators: this.passwordMatchValidator });
    }

    passwordMatchValidator(form: FormGroup){
        const newPassword = form.get('newPassword')?.value;
        const confirmPassword = form.get('confirmPassword')?.value;
        return newPassword === confirmPassword ? null : { passwordMismatch: true };
    }

    toggleDrawer(){
        this.isDrawerOpen = !this.isDrawerOpen;
        this.errorMessage = '';
        this.successMessage = '';
        if(!this.isDrawerOpen) {
            this.passwordForm.reset();
        }
    }

    onSubmit(){
        if(this.passwordForm.invalid) {
            this.errorMessage = 'Please fill in all fields correctly.';
            return;
        }

        const { oldPassword, newPassword } = this.passwordForm.value;

        this.userService.changePassword(oldPassword, newPassword).subscribe({
            next: (response) => {
                this.successMessage = 'Password changed successfully.';
                this.errorMessage = '';
                this.toggleDrawer();
            },
            error: (error) => {
                this.errorMessage = error.error.message || 'An error occurred while changing the password.';
                this.successMessage = '';
            }
        });
    }
}