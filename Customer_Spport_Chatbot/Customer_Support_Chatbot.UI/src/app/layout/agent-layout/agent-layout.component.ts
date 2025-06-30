import { Component, HostListener, Input, OnInit } from "@angular/core";
import { SidebarComponent } from "../../components/sidebar/sidebar.component";
import { NavbarComponent } from "../../components/navbar/navbar.component";
import { ActivatedRoute, RouterModule, RouterOutlet } from "@angular/router";
import { CommonModule } from "@angular/common";

@Component({
    selector:'app-agent-layout',
    templateUrl:'./agent-layout.component.html',
    standalone:true,
    imports:[SidebarComponent,NavbarComponent,RouterModule,CommonModule]
})

export class AgentLayoutComponent implements OnInit{
    items: any[] = [];

    showSidebar = false;
    showProfileDrawer = false;

    isMobile = false;
    isTablet = false;
    isDesktop = false;

    constructor(private router: ActivatedRoute) {}

    ngOnInit(): void {
        this.checkScreenSize();
        this.items = this.router.snapshot.data['items'] || [];
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