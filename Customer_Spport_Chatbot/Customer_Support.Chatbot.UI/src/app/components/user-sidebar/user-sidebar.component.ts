import { CommonModule } from "@angular/common";
import { Component, EventEmitter, Output } from "@angular/core";
import { RouterLink, RouterLinkActive } from "@angular/router";

@Component({
    selector: 'app-user-sidebar',
    templateUrl: './user-sidebar.component.html',
    standalone: true,
    imports:[CommonModule, RouterLink,RouterLinkActive],
})
export class Sidebar{
    @Output() navClicked=new EventEmitter<void>();

    onNavClick(){
        this.navClicked.emit();
    }
}