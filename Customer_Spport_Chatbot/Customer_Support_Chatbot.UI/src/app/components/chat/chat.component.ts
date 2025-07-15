import { CommonModule } from "@angular/common";
import { Component, ElementRef, OnDestroy, OnInit, ViewChild, ChangeDetectorRef } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { SignalRService } from "../../services/signalr.service";
import { AuthService } from "../../services/auth.service";
import { ChatService } from "../../services/chat.service";
import { TicketService } from "../../services/ticket.service";
import { ToastrService } from "ngx-toastr";
import { environment } from "../../../environments/environment";
import { Subscription } from "rxjs";

@Component({
    selector: 'app-chat',
    templateUrl: './chat.component.html',
    standalone: true,
    imports: [CommonModule, FormsModule],
})
export class ChatComponent implements OnInit, OnDestroy {
    messages: { sender: 'bot' | 'user' | 'agent'; text?: string; fileUrl?: string; isImage?: boolean; timestamp?: string }[] = [];
    userInput: string = '';
    agentJoined = false;
    newSession = false;
    loading = true;
    isAgent = false;
    ticketId: string = '';
    botInterval: any;
    senderId: string = '';
    chatEnded = false;
    waitingForAgent = false;
    private subscriptions: Subscription[] = [];
    private agentJoinedProcessed = new Set<string>(); // Track processed agent joined tickets
    private lastAgentJoinedTime: number = 0; // Track last agent joined event time

    @ViewChild('chatContainer') chatContainer!: ElementRef;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private signalRService: SignalRService,
        private authService: AuthService,
        private chatService: ChatService,
        private ticketService: TicketService,
        public toastr: ToastrService,
        private cdr: ChangeDetectorRef
    ) {}

    ngOnInit(): void {
        this.senderId = this.authService.getUserId();
        if (!this.senderId) {
            this.toastr.error('User not authenticated. Please log in.', 'Error');
            this.router.navigate(['/login']);
            return;
        }

        const currentNav = this.router.getCurrentNavigation();
        const state = currentNav?.extras?.state;

        if (state?.["ticketId"]) {
            this.ticketId = state["ticketId"];
            console.log('[Chat] Ticket ID from state:', this.ticketId);
        }

        this.route.queryParams.subscribe(params => {
            if (!this.ticketId && params['ticketId']) {
                this.ticketId = params['ticketId'];
                console.log('[Chat] Ticket ID from queryParams:', this.ticketId);
            }
            this.isAgent = params['agent'] === 'true';
            this.newSession = params['new'] === 'true';

            console.log('[Chat] Final resolved ticketId:', this.ticketId, 'isAgent:', this.isAgent);

            if (!this.ticketId) {
                console.error('[Chat] No ticketId found in either state or queryParams!');
                this.toastr.error('No ticket ID found. Please start a new session.', 'Error');
                this.router.navigate([this.isAgent ? '/agent/dashboard/workspace/active' : '/user/dashboard/tickets/active']);
                return;
            }

            // Fetch existing messages and files
            this.fetchMessages();

            this.signalRService.startConnection();
            this.signalRService.joinChat(this.ticketId)
                .then(() => {
                    console.log('[Chat] Joined chat for ticket:', this.ticketId);
                    this.loading = this.isAgent ? false : true;
                    if (this.isAgent) {
                        this.agentJoined = true;
                        this.killBot();
                    } else if (this.newSession) {
                        if (!this.newSession) {
                            this.signalRService.notifyAgent(this.ticketId)
                                .then(() => {
                                    console.log('[Chat] Agent notified for ticket:', this.ticketId);
                                    this.waitingForAgent = true; // Assume agent is unavailable until notified
                                    this.toastr.info('Waiting for an agent to join the chat.', 'Info');
                                })
                                .catch(err => {
                                    console.error('[Chat] Error notifying agent:', err);
                                    this.toastr.error('Failed to notify agent. Please try again.', 'Error');
                                    this.router.navigate([this.isAgent ? '/agent/dashboard/workspace/active' : '/user/dashboard/tickets/active']);
                                });
                        }
                        this.simulateLoadingAndBot();
                    }
                    this.cdr.detectChanges();
                })
                .catch(err => {
                    console.error('[Chat] Error joining chat:', err);
                    this.toastr.error('Failed to connect to chat. Please try again.', 'Error');
                    this.router.navigate([this.isAgent ? '/agent/dashboard/workspace/active' : '/user/dashboard/tickets/active']);
                });

            // Subscribe to SignalR events
            this.subscriptions.push(
                this.signalRService.message$.subscribe(data => {
                    if (data && data.ticketId === this.ticketId) {
                        const messageKey = `${data.ticketId}:${data.timestamp}`;
                        if (!this.agentJoinedProcessed.has(messageKey)) {
                            this.agentJoinedProcessed.add(messageKey);
                            this.messages.push({
                                sender: data.sender,
                                text: data.text,
                                timestamp: data.timestamp
                            });
                            console.log('[Chat] Messages array:', this.messages);
                            this.scrollToBottom();
                            this.cdr.detectChanges();
                        }
                    }
                })
            );

            this.subscriptions.push(
                this.signalRService.file$.subscribe(data => {
                    if (data && data.ticketId === this.ticketId) {
                        const messageKey = `${data.ticketId}:${data.timestamp}`;
                        if (!this.agentJoinedProcessed.has(messageKey)) {
                            this.agentJoinedProcessed.add(messageKey);
                            this.messages.push({
                                sender: data.sender,
                                fileUrl: data.fileUrl,
                                isImage: data.isImage,
                                timestamp: data.timestamp
                            });
                            console.log('[Chat] Messages array:', this.messages);
                            this.scrollToBottom();
                            this.cdr.detectChanges();
                        }
                    }
                })
            );

            this.subscriptions.push(
                this.signalRService.agentJoined$.subscribe(data => {
                    if (data && data.ticketId === this.ticketId) {
                        const currentTime = Date.now();
                        // Ignore events within 1 second of the last agent joined event
                        if (this.agentJoinedProcessed.has(data.ticketId) || currentTime - this.lastAgentJoinedTime < 1000) {
                            console.warn('[Chat] Duplicate AgentJoined event ignored:', data);
                            return;
                        }
                        this.agentJoinedProcessed.add(data.ticketId);
                        this.lastAgentJoinedTime = currentTime;
                        this.agentJoined = true;
                        this.loading = false;
                        this.waitingForAgent = false; 
                        this.killBot();
                        this.messages.push({
                            sender: data.sender,
                            text: data.text,
                            timestamp: data.timestamp
                        });
                        console.log('[Chat] Messages array:', this.messages);
                        this.toastr.success('An agent has joined the chat.', 'Agent Joined');
                        this.scrollToBottom();
                        this.cdr.detectChanges();
                    }
                })
            );

            this.subscriptions.push(
                this.signalRService.chatEnded$.subscribe(data => {
                    if (data && data.ticketId === this.ticketId) {
                        const messageKey = `${data.ticketId}:${data.timestamp}`;
                        if (!this.agentJoinedProcessed.has(messageKey)) {
                            this.agentJoinedProcessed.add(messageKey);
                            this.chatEnded = true;
                            this.loading = false;
                            this.messages.push({
                                sender: data.sender,
                                text: data.text,
                                timestamp: data.timestamp
                            });
                            console.log('[Chat] Messages array:', this.messages);
                            this.toastr.info('The support ticket has been closed.', 'Chat Ended');
                            this.scrollToBottom();
                            this.cdr.detectChanges();
                            setTimeout(() => this.leaveChat(), 2000);
                        }
                    }
                })
            );

            this.subscriptions.push(
                this.signalRService.agentAvailability$.subscribe(available => {
                    if (!this.isAgent && !this.newSession) {
                        this.waitingForAgent = !available;
                        if (!available) {
                            this.toastr.info('Agent is currently unavailable. Please wait.', 'Info');
                        } else {
                            this.toastr.info('Agent is now available.', 'Info');
                        }
                        this.cdr.detectChanges();
                    }
                })
            );
        });
    }

    private fetchMessages(): void {
        this.chatService.getMessages(this.ticketId).subscribe({
            next: (messages) => {
                this.messages = messages.map((msg: any) => ({
                    sender: msg.sender,
                    text: msg.text,
                    fileUrl: msg.fileUrl,
                    isImage: msg.isImage,
                    timestamp: msg.timestamp
                }));
                console.log('[Chat] Fetched messages:', this.messages);
                this.scrollToBottom();
                this.cdr.detectChanges();
            },
            error: (err) => {
                console.error('[Chat] Error fetching messages:', err);
                this.toastr.error('Failed to load chat history.', 'Error');
            }
        });
    }

    trackByMessage(index: number, msg: any): string {
        return msg.timestamp + (msg.text || '') + (msg.fileUrl || '');
    }

    killBot() {
        if (this.botInterval) {
            clearInterval(this.botInterval);
            this.botInterval = null;
            console.log('[Bot] Interval cleared');
        }
    }

    ngAfterViewChecked() {
        this.scrollToBottom();
    }

    ngOnDestroy(): void {
        this.killBot();
        this.signalRService.leaveChat(this.ticketId)
            .catch(err => console.error('[Chat] Error leaving chat:', err));
        this.signalRService.stopConnection();
        this.subscriptions.forEach(sub => sub.unsubscribe());
        this.agentJoinedProcessed.clear();
        this.lastAgentJoinedTime = 0;
    }

    scrollToBottom(): void {
        try {
            this.chatContainer.nativeElement.scrollTop = this.chatContainer.nativeElement.scrollHeight;
        } catch (err) {
            console.error('[Chat] Scroll error:', err);
        }
    }

    simulateLoadingAndBot() {
        this.messages.push({ sender: 'bot', text: 'Loading your support ticket chat...' });
        console.log('[Chat] Messages array:', this.messages);
        this.scrollToBottom();
        this.cdr.detectChanges();
        setTimeout(() => {
            if (this.agentJoined) {
                console.log('[Chat] Agent has already joined, skipping bot messages.');
                this.loading = false;
                return;
            }
            this.loading = false;
            this.botMessageLoop();
            this.cdr.detectChanges();
        }, 2000);
    }

    botMessageLoop() {
        const botMsgs = [
            "Hi! We're reviewing your ticket.",
            "Please stay with us while we connect you to a support agent.",
            "Still connecting you with an agent...",
            "Thanks for your patience!"
        ];
        let i = 0;
        this.botInterval = setInterval(() => {
            if (this.agentJoined || this.chatEnded) {
                console.log('[Chat] Agent has joined or chat ended, stopping bot messages.');
                this.killBot();
                return;
            }
            if (i >= botMsgs.length) {
                this.killBot();
                console.log('[Chat] Bot message loop completed.');
                return;
            }
            this.messages.push({ sender: 'bot', text: botMsgs[i++] });
            console.log('[Chat] Messages array:', this.messages);
            this.scrollToBottom();
            this.cdr.detectChanges();
        }, Math.floor(Math.random() * 4000) + 6000);
    }

    sendMessage(msg: string) {
        if (this.chatEnded) {
            this.toastr.info('Chat has ended. Please start a new session.', 'Info');
            return;
        }

        if (!msg.trim()) return;

        const messageData = {
            ticketId: this.ticketId,
            senderId: this.senderId,
            content: msg
        };

        this.chatService.sendMessage(messageData).subscribe({
            next: () => {
                this.signalRService.sendMessage(this.ticketId, this.senderId, msg)
                    .then(() => {
                        this.userInput = '';
                        this.cdr.detectChanges();
                    })
                    .catch(err => {
                        console.error('[Chat] Error sending message via SignalR:', err);
                        this.toastr.error('Failed to send message.', 'Error');
                    });
            },
            error: (err) => {
                console.error('[Chat] Error saving message to DB:', err);
                this.toastr.error('Failed to save message.', 'Error');
            }
        });
    }

    handleFileUpload(event: Event) {
        if (this.chatEnded) {
            this.toastr.info('Chat has ended. Please start a new session to upload files.', 'Info');
            return;
        }

        const input = event.target as HTMLInputElement;
        const file = input?.files?.[0];
        if (!file || !this.ticketId) {
            this.toastr.error('No file selected or invalid ticket ID.', 'Error');
            return;
        }

        this.chatService.uploadFile(file, this.ticketId).subscribe({
            next: (res) => {
                const fileUrl = `${environment.getFileUrl}${res.data.url}`;
                const isImage = file.type.startsWith('image/');
                this.signalRService.sendFile(this.ticketId, this.senderId, fileUrl, isImage)
                    .then(() => {
                        this.toastr.success('File uploaded successfully.', 'Success');
                        this.cdr.detectChanges();
                    })
                    .catch(err => {
                        console.error('[Chat] Error sending file via SignalR:', err);
                        this.toastr.error('Failed to send file.', 'Error');
                    });
            },
            error: (err) => {
                console.error('[Chat] Error uploading file:', err);
                this.toastr.error(err.error?.message || 'Failed to upload file.', 'Error');
            }
        });
    }

    leaveChat() {
        this.killBot();
        this.signalRService.leaveChat(this.ticketId)
            .catch(err => console.error('[Chat] Error leaving chat:', err));
        this.messages = [];
        this.userInput = '';
        this.agentJoined = false;
        this.newSession = false;
        this.chatEnded = false;
        this.loading = false;
        this.agentJoinedProcessed.clear();
        this.lastAgentJoinedTime = 0;
        this.waitingForAgent = false;
        this.router.navigate([this.isAgent ? '/agent/dashboard/workspace/active' : '/user/dashboard/tickets/active']);
        this.cdr.detectChanges();
    }

    endChat() {
        if (!this.ticketId) {
            console.error('[Chat] No ticket ID available to end chat.');
            this.toastr.error('No ticket ID available.', 'Error');
            return;
        }
        if (this.isAgent) {
            this.ticketService.endTicket(this.ticketId).subscribe({
                next: () => {
                    this.signalRService.endChat(this.ticketId)
                        .then(() => {
                            this.toastr.success('Chat and ticket ended successfully.', 'Success');
                            this.cdr.detectChanges();
                        })
                        .catch(err => {
                            console.error('[Chat] Error ending chat via SignalR:', err);
                            this.toastr.error('Failed to end chat. Please try again.', 'Error');
                        });
                },
                error: (err) => {
                    console.error('[Chat] Error ending ticket:', err);
                    this.toastr.error('Failed to end ticket. Please try again.', 'Error');
                }
            });
        } else {
            this.toastr.warning('Only agents can end the chat.', 'Warning');
        }
    }
}