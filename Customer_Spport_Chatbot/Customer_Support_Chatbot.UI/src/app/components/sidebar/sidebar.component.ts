import { CommonModule } from "@angular/common";
import { Component, EventEmitter, Input, Output } from "@angular/core";
import { RouterModule } from "@angular/router";
import { LucideIconsModule } from "../../utils/lucide-icons.module";

@Component({
    selector:'app-sidebar',
    templateUrl:'./sidebar.component.html',
    standalone:true,
    imports:[CommonModule,RouterModule,LucideIconsModule]
})

export class SidebarComponent{
    @Input() items:any[]=[];
    @Output() navClicked=new EventEmitter<void>();

    onNavClick(){
        if (window.innerWidth < 768) {
            this.navClicked.emit();
        }
    }
}