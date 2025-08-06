import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { SignalRService } from '../../services/signalr.service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
    selector: 'app-active-workspace',
    templateUrl: './active.component.html',
    standalone: true,
    imports: [CommonModule]
})
export class ActiveWorkspaceComponent implements OnInit, OnDestroy {
    tickets: { ticketId: string, title: string }[] = [];
    private subscriptions: Subscription[] = [];

    constructor(
        private signalRService: SignalRService,
        private router: Router,
        private toastr: ToastrService
    ) {}

    ngOnInit(): void {
        this.registerSignalRListeners();
    }

    private registerSignalRListeners(): void {
        this.subscriptions.push(
        this.signalRService.ticketNotification$.subscribe(data => {
            if (data && data.ticketId && data.title) {
                console.log('[ActiveWorkspace] Received ticket notification:', data);
                const alreadyExists = this.tickets.some(ticket => ticket.ticketId === data.ticketId);
                if (!alreadyExists) {
                    this.tickets.push({ ticketId: data.ticketId, title: data.title });
                    this.toastr.success(`New ticket assigned: ${data.title}`, 'Ticket Notification');
                } else {
                    console.warn('[ActiveWorkspace] Duplicate ticket ignored:', data.ticketId);
                }
            } else {
                console.error('[ActiveWorkspace] Invalid ticket notification data:', data);
            }
        })
    );

        this.subscriptions.push(
            this.signalRService.chatEnded$.subscribe(data => {
                if (data && data.ticketId) {
                    console.log('[ActiveWorkspace] Received chat ended notification for ticket:', data.ticketId);
                    this.tickets = this.tickets.filter(ticket => ticket.ticketId !== data.ticketId);
                    this.toastr.info(`Ticket ${data.ticketId} has been closed.`, 'Ticket Closed');
                }
            })
        );

        this.subscriptions.push(
            this.signalRService.userLeftChat$.subscribe(data => {
                if (data && data.ticketId) {
                    console.log('[ActiveWorkspace] Received user left chat notification for ticket:', data.ticketId);
                    this.tickets = this.tickets.filter(ticket => ticket.ticketId !== data.ticketId);
                    this.toastr.info(`Ticket ${data.ticketId} removed as user left the chat.`, 'User Left');
                }
            })
        );

        this.subscriptions.push(
            this.signalRService.leaveChat$.subscribe(data => {
                if (data && data.ticketId) {
                    console.log('[ActiveWorkspace] Received leave chat notification for ticket:', data.ticketId);
                    this.tickets = this.tickets.filter(ticket => ticket.ticketId !== data.ticketId);
                    this.toastr.info(`Ticket ${data.ticketId} removed as chat was left.`, 'Chat Left');
                }
            })
        );
    }

    ngOnDestroy(): void {
        this.subscriptions.forEach(sub => sub.unsubscribe());
    }

    joinChat(ticketId: string | null | undefined) {
        if (!ticketId) {
            console.error('[ActiveWorkspace] No ticketId provided to joinChat');
            this.toastr.error('Invalid ticket ID. Please try again.', 'Error');
            return;
        }
        console.log('[ActiveWorkspace] Joining chat for ticket:', ticketId);

        this.signalRService.joinChat(ticketId)
            .then(() => {
                this.router.navigate(['/agent/dashboard/workspace/chat'], {
                    state: { ticketId, agent: true },
                    queryParams: { ticketId, agent: true }
                });
            })
            .catch(err => {
                console.error('[ActiveWorkspace] Error joining chat:', err);
                this.toastr.error('Failed to join chat. Please try again.', 'Error');
            });
    }

    trackByTicket(index: number, ticket: { ticketId: string, title: string }): string {
        return ticket.ticketId;
    }
}