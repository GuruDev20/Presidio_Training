import { Component, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { AuthService } from "../../services/auth.service";
import { UserService } from "../../services/user.service";
import { Router } from "@angular/router";
import { TicketService } from "../../services/ticket.service";

@Component({
    selector: 'app-active-ticket',
    templateUrl: './active-ticket.component.html',
    standalone: true,
    imports: [CommonModule],
})
export class ActiveTicketComponent implements OnInit {
    activeTickets: any[] = [];
    pendingTickets: any[] = [];
    userId: string = '';
    role: string = '';
    errorMessage: string = '';

    constructor(private authService: AuthService, private userService: UserService,private router:Router,private ticketService:TicketService) {}

    ngOnInit(): void {
        this.userId = this.authService.getUserId();
        this.role = this.authService.getRole();
        this.loadTickets();
    }

    loadTickets(): void {
        this.userService.getTicketsByUser({ userOrAgentId: this.userId, role: this.role })
            .subscribe({
                next: (res) => {
                    const tickets = Array.isArray((res.data as any)?.$values) ? (res.data as any).$values : [];
                    this.activeTickets = tickets.filter((ticket: { status: string; }) => ticket.status === 'Open');
                    this.pendingTickets = tickets.filter((ticket: { status: string; }) => ticket.status === 'Pending');
                },
                error: (error) => {
                    this.errorMessage = 'Failed to load tickets. Please try again later.';
                    console.error('Error fetching tickets:', error);
                }
            });
    }

    loadChat(ticketId:string,agentId?: string){
        this.router.navigate(['/user/dashboard/tickets/chat'],{
            queryParams: { ticketId,new:false,agent:this.role === 'Agent' },
            state:{ticketId,agentId}
        });
    }
}