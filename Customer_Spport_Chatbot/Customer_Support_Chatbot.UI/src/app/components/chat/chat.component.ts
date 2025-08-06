import { Component, OnInit, OnDestroy, ViewChild, ElementRef, ChangeDetectorRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { SignalRService } from '../../services/signalr.service';
import { AuthService } from '../../services/auth.service';
import { ChatService } from '../../services/chat.service';
import { TicketService } from '../../services/ticket.service';
import { ToastrService } from 'ngx-toastr';
import { environment } from '../../../environments/environment';
import { Subscription } from 'rxjs';

@Component({
    selector: 'app-chat',
    templateUrl: './chat.component.html',
    standalone: true,
    imports: [CommonModule, FormsModule],
})
export class ChatComponent implements OnInit, OnDestroy {
    messages: { sender: 'bot' | 'user' | 'agent'; text?: string; fileUrl?: string; isImage?: boolean; timestamp?: string; }[] = [];
    userInput: string = '';
    agentJoined = false;
    newSession = false;
    loading = true;
    isAgent = false;
    ticketId: string = '';
    agentId: string = '';
    botInterval: any;
    senderId: string = '';
    chatEnded = false;
    waitingForAgent = false;
    private subscriptions: Subscription[] = [];
    private agentJoinedProcessed = new Set<string>();
    private lastAgentJoinedTime: number = 0;

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

        this.authService.getCurrentUser().subscribe({
            next: (res) => {
                const user = res.data;
                let plan = 'Basic';
                if (user.subscriptions && user.subscriptions.$values.length > 0) {
                    const active = user.subscriptions.$values.find(
                        (s) => s.status === 'Active'
                    );
                    if (active) plan = active.plan.name;
                }
                this.cdr.detectChanges();
            },
            error: (err) => {
                console.error('[Chat] Error fetching user profile:', err);
            },
        });

        const currentNav = this.router.getCurrentNavigation();
        const state = currentNav?.extras?.state;

        if (state?.['ticketId']) {
            this.ticketId = state['ticketId'];
            this.agentId = state['agentId'];
            console.log(
                '[Chat] Ticket ID from state:', this.ticketId,
                'Agent ID:', this.agentId
            );
        }

        this.route.queryParams.subscribe((params) => {
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
                this.loading = false;
                return;
            }

            this.messages = [];
            this.fetchMessages();
            this.startSignalRConnection();
        });
    }

    private startSignalRConnection(): void {
        this.signalRService.ensureConnection()
            .then(() => {
                console.log('[Chat] SignalR connection established or already active');
                this.signalRService.joinChat(this.ticketId)
                    .then(() => {
                        console.log('[Chat] Joined chat for ticket:', this.ticketId);
                        this.loading = this.isAgent ? false : true;
                        if (this.isAgent) {
                            this.agentJoined = true;
                            this.killBot();
                        } else {
                            if (this.newSession) {
                                // For new tickets, agent is already assigned and notified in backend
                                this.waitingForAgent = true;
                                this.toastr.info('Notifying assigned agent to join the chat.', 'Info');
                                this.simulateLoadingAndBot();
                            } else {
                                // For existing tickets, check agent availability or assign new agent
                                this.notifyAgentOrAssignNew();
                            }
                        }
                        this.setupSignalRSubscriptions();
                        this.cdr.detectChanges();
                    })
                    .catch((err) => {
                        console.error('[Chat] Error joining chat:', err);
                        this.toastr.error('Failed to connect to chat. Retrying...', 'Error');
                        this.retrySignalRConnection();
                    });
            })
            .catch((err) => {
                console.error('[Chat] Error ensuring SignalR connection:', err);
                this.toastr.error('Failed to start chat connection. Retrying...', 'Error');
                this.retrySignalRConnection();
            });
    }

    private retrySignalRConnection(attempts = 3, delay = 3000): void {
        if (attempts <= 0) {
            console.error('[Chat] Max retry attempts reached.');
            this.toastr.error('Unable to connect to chat. Please try again later.', 'Error');
            this.loading = false;
            return;
        }
        setTimeout(() => {
            console.log(`[Chat] Retrying SignalR connection, attempts left: ${attempts}`);
            this.startSignalRConnection();
        }, delay);
    }

    private setupSignalRSubscriptions(): void {
        this.subscriptions.push(
            this.signalRService.message$.subscribe((data) => {
                if (data && data.ticketId === this.ticketId) {
                    const messageKey = `${data.ticketId}:${data.timestamp}`;
                    if (!this.agentJoinedProcessed.has(messageKey)) {
                        this.agentJoinedProcessed.add(messageKey);
                        if (Array.isArray(this.messages)) {
                            this.messages.push({
                                sender: data.sender,
                                text: data.text,
                                timestamp: data.timestamp,
                            });
                            console.log('[Chat] Messages array:', this.messages);
                            this.scrollToBottom();
                            this.cdr.detectChanges();
                        } else {
                            console.error('[Chat] this.messages is not an array:', this.messages);
                            this.messages = [];
                            this.toastr.error('Chat data corrupted. Resetting messages.', 'Error');
                        }
                    }
                }
            })
        );

        this.subscriptions.push(
            this.signalRService.file$.subscribe((data) => {
                if (data && data.ticketId === this.ticketId) {
                    const messageKey = `${data.ticketId}:${data.timestamp}`;
                    if (!this.agentJoinedProcessed.has(messageKey)) {
                        this.agentJoinedProcessed.add(messageKey);
                        if (Array.isArray(this.messages)) {
                            this.messages.push({
                                sender: data.sender,
                                fileUrl: data.fileUrl,
                                isImage: data.isImage,
                                timestamp: data.timestamp,
                            });
                            console.log('[Chat] Messages array:', this.messages);
                            this.scrollToBottom();
                            this.cdr.detectChanges();
                        } else {
                            console.error('[Chat] this.messages is not an array:', this.messages);
                            this.messages = [];
                            this.toastr.error('Chat data corrupted. Resetting messages.', 'Error');
                        }
                    }
                }
            })
        );

        this.subscriptions.push(
            this.signalRService.agentJoined$.subscribe((data) => {
                if (data && data.ticketId === this.ticketId) {
                    const currentTime = Date.now();
                    const messageKey = `${data.ticketId}:${data.timestamp}`;
                    if (this.agentJoinedProcessed.has(messageKey) || currentTime - this.lastAgentJoinedTime < 1000) {
                        console.warn('[Chat] Duplicate AgentJoined event ignored:', data);
                        return;
                    }
                    this.agentJoinedProcessed.add(messageKey);
                    this.lastAgentJoinedTime = currentTime;
                    this.agentJoined = true;
                    this.loading = false;
                    this.waitingForAgent = false;
                    this.killBot();
                    if (Array.isArray(this.messages)) {
                        this.messages.push({
                            sender: data.sender,
                            text: data.text,
                            timestamp: data.timestamp,
                        });
                        console.log('[Chat] Messages array:', this.messages);
                        this.toastr.success('An agent has joined the chat.', 'Agent Joined');
                        this.scrollToBottom();
                        this.cdr.detectChanges();
                    } else {
                        console.error('[Chat] this.messages is not an array:', this.messages);
                        this.messages = [];
                        this.toastr.error('Chat data corrupted. Resetting messages.', 'Error');
                    }
                }
            })
        );

        this.subscriptions.push(
            this.signalRService.chatEnded$.subscribe((data) => {
                if (data && data.ticketId === this.ticketId) {
                    const messageKey = `${data.ticketId}:${data.timestamp}`;
                    if (!this.agentJoinedProcessed.has(messageKey)) {
                        this.agentJoinedProcessed.add(messageKey);
                        this.chatEnded = true;
                        this.loading = false;
                        if (Array.isArray(this.messages)) {
                            this.messages.push({
                                sender: data.sender,
                                text: data.text,
                                timestamp: data.timestamp,
                            });
                            console.log('[Chat] Messages array:', this.messages);
                            this.toastr.info('The support ticket has been closed.', 'Chat Ended');
                            this.scrollToBottom();
                            this.cdr.detectChanges();
                            setTimeout(() => {
                                const path = this.isAgent ? '/agent/dashboard/workspace/active' : '/user/dashboard/tickets/active';
                                this.router.navigateByUrl(path).then(() => window.location.reload());
                            }, 2000);
                        } else {
                            console.error('[Chat] this.messages is not an array:', this.messages);
                            this.messages = [];
                            this.toastr.error('Chat data corrupted. Resetting messages.', 'Error');
                        }
                    }
                }
            })
        );

        this.subscriptions.push(
            this.signalRService.agentAvailability$.subscribe((available) => {
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

        this.subscriptions.push(
            this.signalRService.userLeftChat$.subscribe((data) => {
                if (data && data.ticketId === this.ticketId) {
                    const messageKey = `${data.ticketId}:${data.timestamp}`;
                    if (!this.agentJoinedProcessed.has(messageKey)) {
                        this.agentJoinedProcessed.add(messageKey);
                        if (Array.isArray(this.messages)) {
                            this.messages.push({
                                sender: data.sender,
                                text: data.text,
                                timestamp: data.timestamp,
                            });
                            console.log('[Chat] Messages array:', this.messages);
                            this.toastr.info(data.text, this.isAgent ? 'User Left' : 'Agent Left');
                            this.scrollToBottom();
                            this.cdr.detectChanges();
                            if (!this.isAgent) {
                                setTimeout(() => this.leaveChat(), 2000);
                            }
                        } else {
                            console.error('[Chat] this.messages is not an array:', this.messages);
                            this.messages = [];
                            this.toastr.error('Chat data corrupted. Resetting messages.', 'Error');
                        }
                    }
                }
            })
        );

        this.subscriptions.push(
            this.signalRService.leaveChat$.subscribe((data) => {
                if (data && data.ticketId === this.ticketId) {
                    const messageKey = `${data.ticketId}:${data.timestamp}`;
                    if (!this.agentJoinedProcessed.has(messageKey)) {
                        this.agentJoinedProcessed.add(messageKey);
                        console.log('[Chat] Received LeaveChat signal:', data);
                        this.chatEnded = true;
                        this.loading = false;
                        this.toastr.info(`${data.isAgent ? 'Agent' : 'User'} has left the chat.`, 'Info');
                        if (!data.isAgent && this.isAgent) {
                            console.log('[Chat] User left, agent auto-leaving chat.');
                            this.leaveChat();
                        } else {
                            this.cleanupAndNavigate();
                        }
                    }
                }
            })
        );
    }

    private fetchMessages(): void {
        console.log('[Chat] Fetching messages for ticket:', this.ticketId);
        this.chatService.getMessages(this.ticketId).subscribe({
            next: (response) => {
                const messages = Array.isArray(response) ? response : (response as any)?.$values || [];
                console.log('[Chat] Raw response from backend:', response);
                if (!Array.isArray(messages)) {
                    console.error('[Chat] Received non-array messages from backend:', messages);
                    this.messages = [];
                    this.toastr.error('Invalid chat data received from server.', 'Error');
                } else {
                    this.messages = messages.map((msg) => ({
                        sender: (['bot', 'user', 'agent'].includes(msg.sender) ? msg.sender : 'user') as 'bot' | 'user' | 'agent',
                        text: msg.text,
                        fileUrl: msg.fileUrl,
                        isImage: msg.isImage ?? false,
                        timestamp: msg.timestamp ?? new Date().toISOString(),
                    }));
                    console.log('[Chat] Fetched messages and attachments:', this.messages);
                }
                this.scrollToBottom();
                this.cdr.markForCheck();
                this.cdr.detectChanges();
            },
            error: (err) => {
                console.error('[Chat] Error fetching messages:', err);
                this.toastr.error('Failed to load chat history.', 'Error');
                this.loading = false;
                this.cdr.detectChanges();
            },
        });
    }

    private notifyAgentOrAssignNew(): void {
        console.log('[Chat] Checking agent for existing ticket:', this.ticketId);
        this.ticketService.getTicket(this.ticketId).subscribe({
            next: (response) => {
                if (response?.data) {
                    this.agentId = response.data.agentId;
                    if (this.agentId) {
                        this.ticketService.checkAgentAvailability(this.agentId).subscribe({
                            next: (availabilityResponse: any) => {
                                if (availabilityResponse.data?.isAvailable) {
                                    this.signalRService.notifySpecificAgent(this.ticketId, this.agentId!)
                                        .then(() => {
                                            console.log('[Chat] Notified assigned agent:', this.agentId);
                                            this.waitingForAgent = true;
                                            this.toastr.info('Notifying assigned agent to join the chat.', 'Info');
                                            this.simulateLoadingAndBot();
                                        })
                                        .catch((err) => {
                                            console.error('[Chat] Error notifying assigned agent:', err);
                                            this.toastr.error('Failed to notify assigned agent. Assigning new agent.', 'Error');
                                            this.assignNewAgent(this.ticketId);
                                        });
                                } else {
                                    console.log('[Chat] Assigned agent unavailable, assigning new agent.');
                                    this.assignNewAgent(this.ticketId);
                                }
                            },
                            error: (err) => {
                                console.error('[Chat] Error checking agent availability:', err);
                                this.toastr.error('Error checking agent availability. Assigning new agent.', 'Error');
                                this.assignNewAgent(this.ticketId);
                            },
                        });
                    } else {
                        console.log('[Chat] No agent assigned, assigning new agent.');
                        this.assignNewAgent(this.ticketId);
                    }
                } else {
                    this.assignNewAgent(this.ticketId);
                }
            },
            error: (err) => {
                console.error('[Chat] Error fetching ticket details:', err);
                this.toastr.error('Failed to fetch ticket details. Assigning new agent.', 'Error');
                this.assignNewAgent(this.ticketId);
            },
        });
    }

    private assignNewAgent(ticketId: string): void {
        console.log('[Chat] Assigning new agent for ticket:', ticketId);
        this.ticketService.assignNewAgent(ticketId).subscribe({
            next: (response: any) => {
                if (response.success && response.data?.agentId) {
                    this.agentId = response.data.agentId;
                    this.signalRService.notifySpecificAgent(this.ticketId, this.agentId!)
                        .then(() => {
                            console.log('[Chat] Notified new agent:', this.agentId);
                            this.waitingForAgent = true;
                            this.toastr.info('You are being prioritized for agent assignment based on your subscription plan.', 'Priority Support');
                            this.toastr.info('Notifying a new agent to join the chat.', 'Info');
                            this.simulateLoadingAndBot();
                        })
                        .catch((err) => {
                            console.error('[Chat] Error notifying new agent:', err);
                            this.toastr.error('Failed to notify new agent. Please try again.', 'Error');
                        });
                } else {
                    this.toastr.error('No available agents at the moment. Please try again later.', 'Error');
                    this.loading = false;
                }
            },
            error: (err) => {
                console.error('[Chat] Error assigning new agent:', err);
                this.toastr.error('Failed to assign new agent. Please try again.', 'Error');
                this.loading = false;
            },
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
        if (!this.chatEnded) {
            this.leaveChat();
        }
        this.subscriptions.forEach((sub) => sub.unsubscribe());
        this.agentJoinedProcessed.clear();
        this.lastAgentJoinedTime = 0;
        this.messages = [];
    }

    scrollToBottom(): void {
        try {
            if (this.chatContainer?.nativeElement) {
                this.chatContainer.nativeElement.scrollTop = this.chatContainer.nativeElement.scrollHeight;
            }
        } catch (err) {
            console.error('[Chat] Scroll error:', err);
        }
    }

    simulateLoadingAndBot() {
        if (!Array.isArray(this.messages)) {
            console.error('[Chat] this.messages is not an array in simulateLoadingAndBot:', this.messages);
            this.messages = [];
            this.toastr.error('Chat data corrupted. Resetting messages.', 'Error');
        }
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
            'Please stay with us while we connect you to a support agent.',
            'Still connecting you with an agent...',
            'Thanks for your patience!',
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
            if (Array.isArray(this.messages)) {
                this.messages.push({ sender: 'bot', text: botMsgs[i++] });
                console.log('[Chat] Messages array:', this.messages);
                this.scrollToBottom();
                this.cdr.detectChanges();
            } else {
                console.error('[Chat] this.messages is not an array in botMessageLoop:', this.messages);
                this.messages = [];
                this.toastr.error('Chat data corrupted. Resetting messages.', 'Error');
            }
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
            content: msg,
        };

        this.chatService.sendMessage(messageData).subscribe({
            next: () => {
                this.signalRService
                    .sendMessage(this.ticketId, this.senderId, msg)
                    .then(() => {
                        this.userInput = '';
                        this.cdr.detectChanges();
                    })
                    .catch((err) => {
                        console.error('[Chat] Error sending message via SignalR:', err);
                        this.toastr.error('Failed to send message.', 'Error');
                    });
            },
            error: (err) => {
                console.error('[Chat] Error saving message to DB:', err);
                this.toastr.error('Failed to save message.', 'Error');
            },
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
                this.signalRService
                    .sendFile(this.ticketId, this.senderId, fileUrl, isImage)
                    .then(() => {
                        this.toastr.success('File uploaded successfully.', 'Success');
                        this.cdr.detectChanges();
                    })
                    .catch((err) => {
                        console.error('[Chat] Error sending file via SignalR:', err);
                        this.toastr.error('Failed to send file.', 'Error');
                    });
            },
            error: (err) => {
                console.error('[Chat] Error uploading file:', err);
                this.toastr.error(err.error?.message || 'Failed to upload file.', 'Error');
            },
        });
    }

    leaveChat() {
        if (!this.ticketId || !this.senderId) {
            console.error('[Chat] No ticket ID or sender ID available.');
            this.toastr.error('Invalid session. Please try again.', 'Error');
            return;
        }

        this.ticketService.leaveChat(this.ticketId, this.senderId, this.isAgent).subscribe({
            next: () => {
                console.log('[Chat] Successfully initiated leave chat for ticket:', this.ticketId);
                this.killBot();
                this.signalRService.leaveChat(this.ticketId, this.senderId, this.isAgent)
                    .then(() => {
                        console.log('[Chat] SignalR leave chat invoked');
                        this.cleanupAndNavigate();
                    })
                    .catch((err) => {
                        console.error('[Chat] Error invoking SignalR leave chat:', err);
                        this.cleanupAndNavigate();
                    });
            },
            error: (err) => {
                console.error('[Chat] Error leaving chat:', err);
                this.toastr.error('Failed to leave chat. Please try again.', 'Error');
                this.cleanupAndNavigate();
            },
        });
    }

    private cleanupAndNavigate(): void {
        console.log('[Chat] Cleaning up and navigating to dashboard');
        this.messages = [];
        this.userInput = '';
        this.agentJoined = false;
        this.newSession = false;
        this.chatEnded = false;
        this.loading = false;
        this.waitingForAgent = false;
        this.agentJoinedProcessed.clear();
        this.lastAgentJoinedTime = 0;
        this.killBot();
        this.subscriptions.forEach((sub) => sub.unsubscribe());
        const path = this.isAgent ? '/agent/dashboard/workspace/active' : '/user/dashboard/tickets/active';
        this.router.navigateByUrl(path).then(() => window.location.reload());
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
                    if (this.signalRService.isConnected()) {
                        this.signalRService
                            .endChat(this.ticketId)
                            .then(() => {
                                this.toastr.success('Chat and ticket ended successfully.', 'Success');
                                this.cdr.detectChanges();
                            })
                            .catch((err) => {
                                console.error('[Chat] Error ending chat via SignalR:', err);
                                this.toastr.error('Failed to end chat. Please try again.', 'Error');
                            });
                    } else {
                        console.warn('[Chat] Cannot end chat: SignalR connection is not active');
                        this.toastr.error('Failed to end chat due to connection issues.', 'Error');
                    }
                },
                error: (err) => {
                    console.error('[Chat] Error ending ticket:', err);
                    this.toastr.error('Failed to end ticket. Please try again.', 'Error');
                },
            });
        } else {
            this.toastr.warning('Only agents can end the chat.', 'Warning');
        }
    }
}