import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Sidebar } from '../../components/user-sidebar/user-sidebar.component';
import { Navbar } from '../../components/navbar/navbar.component';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-user-layout',
    standalone: true,
    imports: [CommonModule, Sidebar, Navbar, RouterOutlet],
    templateUrl:'./user-layout.component.html',
})
export class UserLayout {}
