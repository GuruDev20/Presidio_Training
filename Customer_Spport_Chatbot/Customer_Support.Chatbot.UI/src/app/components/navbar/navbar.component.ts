import { CommonModule } from "@angular/common";
import { Component, OnInit } from "@angular/core";

@Component({
    selector: 'app-navbar',
    templateUrl: './navbar.component.html',
    standalone: true,
    imports: [CommonModule],
})
export class Navbar implements OnInit{

    isDark=false;
    ngOnInit(): void {
        this.isDark= document.documentElement.classList.contains('dark');
    }

    toggleTheme(){
        this.isDark=!this.isDark;
        const html= document.documentElement;
        html.classList.toggle('dark', this.isDark);
        localStorage.setItem('theme', this.isDark ? 'dark' : 'light');
    }
    logout(){
        localStorage.clear();
        location.href = '/auth/login';
    }
}