import { Component, HostListener, Input, OnDestroy, OnInit } from "@angular/core";
import { SidebarComponent } from "../../components/sidebar/sidebar.component";
import { NavbarComponent } from "../../components/navbar/navbar.component";
import { ActivatedRoute, RouterModule } from "@angular/router";
import { CommonModule } from "@angular/common";
import { SignalRService } from "../../services/signalr.service";

@Component({
    selector: 'app-user-layout',
    templateUrl: './user-layout.component.html',
    standalone: true,
    imports: [SidebarComponent, NavbarComponent, RouterModule, CommonModule]
})
export class UserLayoutComponent implements OnInit,OnDestroy {
    items: any[] = [];

    showSidebar = false;
    showProfileDrawer = false;

    isMobile = false;
    isTablet = false;
    isDesktop = false;

    constructor(private router: ActivatedRoute,private signalRService:SignalRService) {}

    ngOnInit(): void {
        this.signalRService.startConnection();
        this.checkScreenSize();
        this.items = this.router.snapshot.data['items'] || [];
    }

    ngOnDestroy(): void {
        this.signalRService.stopConnection();
    }

    @HostListener('window:resize')
    checkScreenSize(): void {
        const width = window.innerWidth;
        this.isMobile = width < 768;
        this.isTablet = width >= 768 && width < 1024;
        this.isDesktop = width >= 1024;
    }

    toggleSidebar(): void {
        this.showSidebar = !this.showSidebar;
    }

    toggleProfileDrawer(): void {
        this.showProfileDrawer = !this.showProfileDrawer;
    }

    closeDrawers(): void {
        this.showSidebar = false;
        this.showProfileDrawer = false;
    }
}
