import { CommonModule } from "@angular/common";
import { Component, OnDestroy, OnInit } from "@angular/core";
import { Subscription } from "rxjs";
import { SignalRService } from "../../services/signalr.service";
import { Router } from "@angular/router";
import { ToastrService } from "ngx-toastr";

@Component({
    selector: 'app-active-workspace',
    templateUrl: './active.component.html',
    standalone: true,
    imports: [CommonModule]
})
export class ActiveWorkspaceComponent implements OnInit, OnDestroy {
    tickets: { ticketId: string, title: string }[] = [];
    private signalrSub!: Subscription;

    constructor(
        private signalRService: SignalRService,
        private router: Router,
        private toastr: ToastrService
    ) {}

    ngOnInit(): void {
        this.signalRService.startConnection();

        this.signalrSub = this.signalRService.ticketNotification$.subscribe(data => {
            if (data && data.ticketId && data.title) {
                console.log('Received ticket notification:', data);

                const alreadyExists = this.tickets.some(ticket => ticket.ticketId === data.ticketId);
                if (!alreadyExists) {
                    const newTicket = {
                        ticketId: data.ticketId,
                        title: data.title
                    };
                    this.tickets.push(newTicket);
                    this.toastr.success(`New ticket assigned: ${data.title}`, 'Ticket Notification');
                } else {
                    console.warn('Duplicate ticket ignored:', data.ticketId);
                }
            }
        });
    }

    ngOnDestroy(): void {
        if (this.signalrSub) {
            this.signalrSub.unsubscribe();
        }
        this.signalRService.stopConnection();
    }

    joinChat(ticketId: string | null | undefined) {
        if (!ticketId) {
            console.error('No ticketId provided to joinChat');
            this.toastr.error('Invalid ticket ID. Please try again.', 'Error');
            return;
        }
        console.log('Joining chat for ticket:', ticketId);

        this.signalRService.joinChat(ticketId)
            .then(() => {
                this.router.navigate(['/agent/dashboard/workspace/chat'], {
                    state: { ticketId, agent: true },
                    queryParams: { ticketId, agent: true }
                });
            })
            .catch(err => {
                console.error('Error joining chat:', err);
                this.toastr.error('Failed to join chat. Please try again.', 'Error');
            });
    }
}
