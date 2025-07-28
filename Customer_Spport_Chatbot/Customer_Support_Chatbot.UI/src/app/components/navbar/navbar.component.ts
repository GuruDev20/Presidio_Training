import { CommonModule } from "@angular/common";
import { Component, EventEmitter, OnInit, Output } from "@angular/core";
import { LucideIconsModule } from "../../utils/lucide-icons.module";
import { AuthService } from "../../services/auth.service";
import { Router } from "@angular/router";

@Component({
    selector:'app-navbar',
    standalone: true,
    templateUrl: './navbar.component.html',
    imports:[CommonModule,LucideIconsModule]
})

export class NavbarComponent implements OnInit{
    isDark= false;
    userProfile: any;
    @Output() openSidebar=new EventEmitter<void>();
    @Output() openProfileDrawer=new EventEmitter<void>();
    
    constructor(private authService:AuthService,private router:Router){}

    ngOnInit(): void {
        this.authService.getCurrentUser().subscribe({
            next: (res) => {
                this.userProfile = res.data;
            },
            error: (err) => {
                console.error('Error fetching user profile:', err);
            }
        });
        this.isDark = document.documentElement.classList.contains('dark');
    }
    toggleTheme() {
        this.isDark = !this.isDark;
        const html= document.documentElement;
        html.classList.toggle('dark', this.isDark);
    }

    logout(){
        this.authService.logout();
        this.router.navigate(['/auth/login']);
    }

    get profileInitial():string{
        if(!this.userProfile?.fullName) return 'U';
        return this.userProfile.fullName.charAt(0).toUpperCase();
    }
}