import { CommonModule } from "@angular/common";
import { Component, EventEmitter, OnInit, Output } from "@angular/core";

@Component({
    selector: 'app-navbar',
    templateUrl: './navbar.component.html',
    standalone: true,
    imports: [CommonModule],
})
export class Navbar{

    @Output() openSidebar=new EventEmitter<void>();
    @Output() openProfileDrawer=new EventEmitter<void>();

    isDark=false;

    toggleTheme(){
        this.isDark = !this.isDark;
        document.documentElement.classList.toggle('dark', this.isDark);
    }

    logout(){
        localStorage.clear();
        location.href='/auth/login';
    }
}