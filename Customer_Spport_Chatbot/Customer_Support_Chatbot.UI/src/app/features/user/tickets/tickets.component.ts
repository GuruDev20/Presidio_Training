import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { ActivatedRoute, Router, RouterModule } from "@angular/router";
import { LucideIconsModule } from "../../../utils/lucide-icons.module";

@Component({
    selector:'app-user-ticket',
    templateUrl:'./tickets.component.html',
    standalone:true,
    imports:[RouterModule,CommonModule,LucideIconsModule]
})

export class UserTicketsComponent{

    constructor(private router:Router,private route:ActivatedRoute){}

    get isAtBaseTickets():boolean{
        const currentUrl = this.router.url;
        return currentUrl.endsWith('/tickets') || currentUrl.endsWith('/tickets/');
    }
}