import { Component, OnDestroy, OnInit, ViewChild, ElementRef } from "@angular/core";
import { AuthService } from "../../services/auth.service";
import { UserService } from "../../services/user.service";
import { UserDevice, UserProfile } from "../../models/auth.model";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { Router } from "@angular/router";
import { SubscriptionModel } from "../../models/subscription.model";

@Component({
    selector: "app-profile",
    templateUrl: "./profile.component.html",
    standalone: true,
    imports: [CommonModule, FormsModule]
})
export class Profile implements OnInit, OnDestroy {
    userProfile: any = null;
    showDeactivationForm = false;
    deactivationReason = "";
    devices: UserDevice[] = [];
    isMobileOrTablet = false;
    isEditingName = false;
    newFullName = "";
    showDeactivationSection = false;

    subscription: SubscriptionModel | null = {
        id: "2321334",
        payment: {
            id: "123456",
            amount: 19,
            currency: "INR",
            status: "Success",
            razorpayPaymentId: "rzp_test_123456789",
        },
        tier: "Basic",
        startDate: new Date("2024-07-23"),
        endDate: new Date("2026-07-23"),
        status: 'Active'
    };

    @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;

    constructor(
        private authService: AuthService,
        private userService: UserService,
        private router: Router
    ) {}

    ngOnInit(): void {
        this.checkDeviceSession();
        this.loadUserData();
        this.checkDeviceType();
        window.addEventListener('resize', this.onResize);
        window.addEventListener('focus', this.onFocus);
    }

    onResize = () => {
        this.checkDeviceType();
    }

    onFocus = () => {
        this.checkDeviceSession();
    }

    ngOnDestroy(): void {
        window.removeEventListener('resize', this.onResize);
        window.removeEventListener('focus', this.onFocus);
    }

    checkDeviceType() {
        const width = window.innerWidth;
        this.isMobileOrTablet = width < 763;
    }

    get isAdminOrAgent(): boolean {
        return this.userProfile?.role === 'Admin' || this.userProfile?.role === 'Agent';
    }

    checkDeviceSession() {
        const deviceId = localStorage.getItem('deviceId');
        if (deviceId) {
            this.authService.isDeviceActive(deviceId).subscribe({
                next: (res) => {
                    if (!res.data) {
                        this.authService.logout();
                        this.router.navigate(['/auth/login']);
                    }
                },
                error: (err) => {
                    console.error("Error checking device session:", err);
                    this.authService.logout();
                    this.router.navigate(['/auth/login']);
                }
            });
        } else {
            this.authService.logout();
            this.router.navigate(['/auth/login']);
        }
    }

    loadUserData() {
        this.authService.getCurrentUser().subscribe({
            next: (res) => {
                this.userProfile = res.data;
                this.newFullName = res.data.fullName;
                const userRole = res.data.role;
                this.showDeactivationSection = userRole !== 'Agent' && userRole !== 'Admin';
            },
            error: (err) => {
                console.error("Error fetching user profile:", err);
                this.authService.logout();
                this.router.navigate(['/auth/login']);
            }
        });

        this.authService.getUserDevices().subscribe({
            next: (res: { data: { $values: UserDevice[] } }) => {
                this.devices = res?.data?.$values || [];
            },
            error: (err) => {
                console.error("Error fetching user devices:", err);
            }
        });
    }

    deactivateAccount() {
        
    }

    logout() {
        const currentDeviceId = localStorage.getItem('deviceId');
        if (currentDeviceId) {
            this.authService.logoutDevice(currentDeviceId).subscribe({
                next: () => {
                    this.devices = this.devices.filter(device => device.deviceId !== currentDeviceId);
                    this.authService.logout();
                    this.router.navigate(['/auth/login']);
                },
                error: (err) => {
                    console.error("Error logging out current device:", err);
                    this.authService.logout();
                    this.router.navigate(['/auth/login']);
                }
            });
        } else {
            this.authService.logout();
            this.router.navigate(['/auth/login']);
        }
    }

    logoutDevice(deviceId: string) {
        const currentDeviceId = localStorage.getItem('deviceId');
        this.authService.logoutDevice(deviceId).subscribe({
            next: () => {
                this.devices = this.devices.filter(device => device.deviceId !== deviceId);
                if (deviceId === currentDeviceId) {
                    this.authService.logout();
                    this.router.navigate(['/auth/login']);
                }
            },
            error: (err) => {
                console.error("Error logging out device:", err);
            }
        });
    }

    triggerFileInput() {
        this.fileInput.nativeElement.click();
    }

    startEditingName() {
        this.isEditingName = true;
    }

    onProfilePictureChange(event: Event) {
        const file=(event.target as HTMLInputElement).files?.[0];
        if(file){
            const reader=new FileReader();
            reader.onload=()=>{
                const profilePictureUrl = reader.result as string;
                this.userService.updateProfilePicture(profilePictureUrl).subscribe({
                    next: () => {
                        if (this.userProfile) {
                            this.userProfile.profilePictureUrl = profilePictureUrl;
                        }
                    },
                    error: (err) => {
                        console.error("Error updating profile picture:", err);
                    }
                })
            }
            reader.readAsDataURL(file);
        }
    }

    updateName() {
        if (this.newFullName && this.userProfile) {
            this.userService.updateProfileName(this.newFullName).subscribe({
                next: () => {
                    if (this.userProfile) {
                        this.userProfile.fullName = this.newFullName;
                    }
                    this.isEditingName = false;
                },
                error: (err) => {
                    console.error("Error updating name:", err);
                    this.isEditingName = false;
                }
            });
        }
    }

    get subscriptionProgress(): number {
        if (!this.subscription) return 0;
        const now = new Date();
        const start = new Date(this.subscription.startDate);
        const end = new Date(this.subscription.endDate);
        if (end <= start) return 100;
        if (now <= start) return 0;
        if (now >= end) return 100;
        const total = end.getTime() - start.getTime();
        const elapsed = now.getTime() - start.getTime();
        return Math.round((elapsed / total) * 100);
    }
}