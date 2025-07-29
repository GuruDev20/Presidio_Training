import {
  Component,
  OnDestroy,
  OnInit,
  ViewChild,
  ElementRef,
} from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { UserService } from '../../services/user.service';
import { UserDevice, UserProfile } from '../../models/auth.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { SubscriptionModel } from '../../models/subscription.model';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  standalone: true,
  imports: [CommonModule, FormsModule],
})
export class Profile implements OnInit, OnDestroy {
  userProfile: any = null;
  showDeactivationForm = false;
  deactivationReason = '';
  devices: UserDevice[] = [];
  isMobileOrTablet = false;
  isEditingName = false;
  newFullName = '';
  showDeactivationSection = false;
  subscriptions: SubscriptionModel[] = [];
  showHistory = false;

  get activeSubscription(): SubscriptionModel | null {
    if (!this.subscriptions || this.subscriptions.length === 0) return null;
    // Find the first active subscription, or the most recent if none are active
    const active = this.subscriptions.find((s) => s.status === 'Active');
    if (active) return active;
    // If no active, return the most recent by endDate
    return this.subscriptions
      .slice()
      .sort(
        (a, b) => new Date(b.endDate).getTime() - new Date(a.endDate).getTime()
      )[0];
  }

  get previousSubscriptions(): SubscriptionModel[] {
    if (!this.subscriptions || this.subscriptions.length === 0) return [];
    const active = this.activeSubscription;
    return this.subscriptions
      .filter((s) => s !== active)
      .sort(
        (a, b) => new Date(b.endDate).getTime() - new Date(a.endDate).getTime()
      );
  }

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
  };

  onFocus = () => {
    this.checkDeviceSession();
  };

  ngOnDestroy(): void {
    window.removeEventListener('resize', this.onResize);
    window.removeEventListener('focus', this.onFocus);
  }

  checkDeviceType() {
    const width = window.innerWidth;
    this.isMobileOrTablet = width < 763;
  }

  get isAdminOrAgent(): boolean {
    return (
      this.userProfile?.role === 'Admin' || this.userProfile?.role === 'Agent'
    );
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
          console.error('Error checking device session:', err);
          this.authService.logout();
          this.router.navigate(['/auth/login']);
        },
      });
    } else {
      this.authService.logout();
      this.router.navigate(['/auth/login']);
    }
  }

  loadUserData() {
    this.authService.getCurrentUser().subscribe({
      next: (res) => {
        console;
        this.userProfile = res.data;
        this.newFullName = res.data.fullName;
        const userRole = res.data.role;
        this.showDeactivationSection =
          userRole !== 'Agent' && userRole !== 'Admin';
        this.subscriptions = (res.data.subscriptions?.$values || [])
          .slice()
          .sort(
            (a: SubscriptionModel, b: SubscriptionModel) =>
              new Date(b.startDate).getTime() - new Date(a.startDate).getTime()
          );

        console.log('User profile loaded:', this.userProfile);
      },
      error: (err) => {
        console.error('Error fetching user profile:', err);
        this.authService.logout();
        this.router.navigate(['/auth/login']);
      },
    });

    this.authService.getUserDevices().subscribe({
      next: (res: { data: { $values: UserDevice[] } }) => {
        this.devices = res?.data?.$values || [];
      },
      error: (err) => {
        console.error('Error fetching user devices:', err);
      },
    });
  }

  deactivateAccount() {}

  logout() {
    const currentDeviceId = localStorage.getItem('deviceId');
    if (currentDeviceId) {
      this.authService.logoutDevice(currentDeviceId).subscribe({
        next: () => {
          this.devices = this.devices.filter(
            (device) => device.deviceId !== currentDeviceId
          );
          this.authService.logout();
          this.router.navigate(['/auth/login']);
        },
        error: (err) => {
          console.error('Error logging out current device:', err);
          this.authService.logout();
          this.router.navigate(['/auth/login']);
        },
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
        this.devices = this.devices.filter(
          (device) => device.deviceId !== deviceId
        );
        if (deviceId === currentDeviceId) {
          this.authService.logout();
          this.router.navigate(['/auth/login']);
        }
      },
      error: (err) => {
        console.error('Error logging out device:', err);
      },
    });
  }

  triggerFileInput() {
    this.fileInput.nativeElement.click();
  }

  startEditingName() {
    this.isEditingName = true;
  }

  onProfilePictureChange(event: Event) {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (file) {
      const reader = new FileReader();
      reader.onload = () => {
        const profilePictureUrl = reader.result as string;
        this.userService.updateProfilePicture(profilePictureUrl).subscribe({
          next: () => {
            if (this.userProfile) {
              this.userProfile.profilePictureUrl = profilePictureUrl;
            }
          },
          error: (err) => {
            console.error('Error updating profile picture:', err);
          },
        });
      };
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
          console.error('Error updating name:', err);
          this.isEditingName = false;
        },
      });
    }
  }

  getSubscriptionProgress(startDate: Date, endDate: Date): number {
    const now = new Date();
    const start = new Date(startDate);
    const end = new Date(endDate);
    if (end <= start) return 100;
    if (now <= start) return 0;
    if (now >= end) return 100;
    const total = end.getTime() - start.getTime();
    const elapsed = now.getTime() - start.getTime();
    return Math.round((elapsed / total) * 100);
  }

  getSubscriptionDuration(startDate: Date, endDate: Date): string {
    const start = new Date(startDate);
    const end = new Date(endDate);
    const durationInDays = Math.ceil(
      (end.getTime() - start.getTime()) / (1000 * 60 * 60 * 24)
    );
    return `${durationInDays} days`;
  }
}
