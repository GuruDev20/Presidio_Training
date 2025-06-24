import { Component, HostListener, OnInit } from '@angular/core';
import { RouterModule, RouterOutlet } from '@angular/router';
import { Sidebar } from '../../components/user-sidebar/user-sidebar.component';
import { Navbar } from '../../components/navbar/navbar.component';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-user-layout',
    standalone: true,
    imports: [CommonModule, Sidebar, Navbar, RouterModule],
    templateUrl:'./user-layout.component.html',
})
export class UserLayout implements OnInit {
    showSidebar=false;
    showProfileDrawer=false;
    isMobile = false;

    ngOnInit() {
        this.checkScreenSize();
    }

    @HostListener('window:resize')
    checkScreenSize() {
        this.isMobile = window.innerWidth < 768;
    }

    toggleSidebar() {
        this.showSidebar = !this.showSidebar;
    }

    toggleProfileDrawer() {
        this.showProfileDrawer = !this.showProfileDrawer;
    }

    closeDrawers() {
        this.showProfileDrawer = false;
        this.showSidebar = false;
    }
}
