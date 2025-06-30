import { Component } from '@angular/core';

@Component({
    selector: 'app-theme-toggle',
    imports: [],
    templateUrl: './theme-toggle.html',
    styleUrl: './theme-toggle.css'
})
export class ThemeToggle {
    isdark=false;

    toggleTheme(){
        this.isdark=!this.isdark;
        document.documentElement.classList.toggle('dark', this.isdark);
    }
}
