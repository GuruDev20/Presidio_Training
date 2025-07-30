import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { LucideIconsModule } from "../../../utils/lucide-icons.module";
import { ActivatedRoute, Router, RouterModule } from "@angular/router";

@Component({
    selector: "app-admin-workspace",
    templateUrl: "./workspace.component.html",
    standalone: true,
    imports:[CommonModule,LucideIconsModule,RouterModule]
})

export class AdminWorkspaceComponent{

    constructor(private router:Router,private route:ActivatedRoute){}

    get isAtBaseTickets():boolean{
        const currentUrl = this.router.url;
        return currentUrl.endsWith('/workspace') || currentUrl.endsWith('/workspace/');
    }
    
}