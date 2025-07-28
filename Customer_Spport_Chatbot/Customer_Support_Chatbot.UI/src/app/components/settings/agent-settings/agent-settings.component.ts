import { Component, OnInit, OnDestroy } from "@angular/core";
import { LucideIconsModule } from "../../../utils/lucide-icons.module";
import { CommonModule } from "@angular/common";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { UserService } from "../../../services/user.service";
import { Subscription } from 'rxjs';

@Component({
    selector: 'app-agent-settings',
    templateUrl: './agent-settings.component.html',
    standalone: true,
    imports: [LucideIconsModule, CommonModule, ReactiveFormsModule]
})
export class AgentSettingsComponent implements OnInit, OnDestroy {
    isDrawerOpen = { password: false, status: false };
    passwordForm: FormGroup;
    statusForm: FormGroup;
    errorMessage = { password: '', status: '' };
    successMessage = { password: '', status: '' };
    private statusSubscription: Subscription | null = null;

    constructor(private fb: FormBuilder, private userService: UserService) {
        this.passwordForm = this.fb.group({
            oldPassword: ['', [Validators.required]],
            newPassword: ['', [Validators.required, Validators.minLength(6)]],
            confirmPassword: ['', [Validators.required, Validators.minLength(6)]]
        }, { validators: this.passwordMatchValidator });

        this.statusForm = this.fb.group({
            status: ['', [Validators.required]]
        });
    }

    ngOnInit() {
        this.loadCurrentStatus();
        const statusControl = this.statusForm.get('status');
        if (statusControl) {
            this.statusSubscription = statusControl.valueChanges.subscribe(value => {
                console.log('Status form value changed to:', value);
            });
        } else {
            console.error('Status control not found in statusForm');
        }
    }

    ngOnDestroy() {
        if (this.statusSubscription) {
            this.statusSubscription.unsubscribe();
        }
    }

    passwordMatchValidator(form: FormGroup) {
        const newPassword = form.get('newPassword')?.value;
        const confirmPassword = form.get('confirmPassword')?.value;
        return newPassword === confirmPassword ? null : { passwordMismatch: true };
    }

    toggleDrawer(section: 'password' | 'status') {
        this.isDrawerOpen[section] = !this.isDrawerOpen[section];
        this.errorMessage[section] = '';
        this.successMessage[section] = '';
        if (!this.isDrawerOpen[section]) {
            if (section === 'password') {
                this.passwordForm.reset();
            } else if (section === 'status') {
                this.statusForm.reset();
                this.loadCurrentStatus();
            }
        }
    }

    loadCurrentStatus() {
        console.log('Loading current status...');
        this.userService.getCurrentStatus().subscribe({
            next: (response) => {
                console.log('Get status response:', response);
                if (response.success) {
                    const status = response.data.status || 'Available';
                    this.statusForm.patchValue({ status }, { emitEvent: false });
                    console.log('Status set to:', status);
                } else {
                    this.errorMessage.status = response.message || 'Failed to load current status.';
                    console.error('Status fetch failed:', response.message);
                }
            },
            error: (error) => {
                this.errorMessage.status = error.error?.message || 'Error loading current status.';
                console.error('Error fetching status:', error);
            }
        });
    }

    onSubmitPassword() {
        if (this.passwordForm.invalid) {
            this.errorMessage.password = 'Please fill in all fields correctly.';
            return;
        }

        const { oldPassword, newPassword } = this.passwordForm.value;

        this.userService.changePassword(oldPassword, newPassword).subscribe({
            next: (response) => {
                this.successMessage.password = 'Password changed successfully.';
                this.errorMessage.password = '';
                this.toggleDrawer('password');
            },
            error: (error) => {
                this.errorMessage.password = error.error?.message || 'An error occurred while changing the password.';
                this.successMessage.password = '';
            }
        });
    }

    onSubmitStatus() {
        if (this.statusForm.invalid) {
            this.errorMessage.status = 'Please select a status.';
            return;
        }

        const status = this.statusForm.value.status;
        console.log('Submitting status:', status);

        this.userService.updateStatus(status).subscribe({
            next: (response) => {
                console.log('Update status response:', response);
                this.successMessage.status = 'Status updated successfully.';
                this.errorMessage.status = '';
                this.toggleDrawer('status');
            },
            error: (error) => {
                this.errorMessage.status = error.error?.message || 'Error updating status.';
                this.successMessage.status = '';
                console.error('Error updating status:', error);
            }
        });
    }
}