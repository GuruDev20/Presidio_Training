import { CommonModule } from "@angular/common";
import { Component, OnInit } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { Router } from "@angular/router";
import { TicketService } from "../../services/ticket.service";
import { AuthService } from "../../services/auth.service";
import { CreateTicket } from "../../models/ticket.model";


@Component({
    selector: 'app-new-ticket',
    templateUrl: './new-ticket.component.html',
    standalone: true,
    imports: [CommonModule,FormsModule],
})
export class NewTicketComponent implements OnInit{

    title: string = '';
    description: string = '';
    userId: string = '';

    constructor(private router:Router,private ticketService:TicketService,private authService:AuthService){}

    ngOnInit(): void {
        this.userId=this.authService.getUserId();
        if(!this.userId){
            this.router.navigate(['/login']);
        }
    }

    createTicket(){
        if(this.title && this.description){
            const payload:CreateTicket={
                userId: this.userId,
                title: this.title,
                description: this.description  
            };
            this.ticketService.createTicket(payload).subscribe({
                next:(res)=>{
                    console.log('Ticket created successfully:', res);
                    this.router.navigate(['/chat'],{
                        queryParams: { ticketId: res.data.ticketId, new: true }
                    });
                },
                error:(err)=>{
                    console.error('Error creating ticket:', err);
                },
            })
        }
    }
}