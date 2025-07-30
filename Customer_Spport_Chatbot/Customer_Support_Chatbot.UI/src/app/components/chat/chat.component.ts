import { CommonModule } from '@angular/common';
import {
  Component,
  ElementRef,
  OnDestroy,
  OnInit,
  ViewChild,
  ChangeDetectorRef,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
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
  messages: {
    sender: 'bot' | 'user' | 'agent';
    text?: string;
    fileUrl?: string;
    isImage?: boolean;
    timestamp?: string;
  }[] = [];
  userInput: string = '';
  agentJoined = false;
  newSession = false;
  loading = true;
  isAgent = false;
  ticketId: string = '';
  agentId: string | undefined = '';
  botInterval: any;
  senderId: string = '';
  chatEnded = false;
  waitingForAgent = false;
  private subscriptions: Subscription[] = [];
  private agentJoinedProcessed = new Set<string>();
  private lastAgentJoinedTime: number = 0;

  @ViewChild('chatContainer') chatContainer!: ElementRef;
  showFileTypeDialog = false;
  fileTypeOptions: { label: string; accept: string }[] = [];
  allowedFileTypes: string = 'image/*';
  userPlan: string = 'Basic';

  openFileTypeDialog() {
    this.showFileTypeDialog = true;
  }

  closeFileTypeDialog() {
    this.showFileTypeDialog = false;
  }

  selectFileType(option: { label: string; accept: string }) {
    const fileInput = document.getElementById('fileInput') as HTMLInputElement;
    if (fileInput) {
      fileInput.accept = option.accept;
      fileInput.value = '';
      fileInput.click();
    }
    this.closeFileTypeDialog();
  }

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
        this.userPlan = plan;

        console.log('[Chat] User plan:', this.userPlan);
        if (plan === 'Business') {
          this.fileTypeOptions = [
            { label: 'Image', accept: 'image/*' },
            { label: 'Audio', accept: 'audio/*' },
            { label: 'Video', accept: 'video/*' },
            { label: 'PDF', accept: '.pdf' },
            { label: 'Word Document', accept: '.doc,.docx' },
            { label: 'Excel Sheet', accept: '.xls,.xlsx' },
            { label: 'PowerPoint', accept: '.ppt,.pptx' },
            { label: 'Text File', accept: '.txt' },
            { label: 'CSV', accept: '.csv' },
          ];
          this.allowedFileTypes = 'image/*,audio/*,video/*,.pdf,.doc,.docx,.xls,.xlsx,.ppt,.pptx,.txt,.csv';
        } else if (plan === 'Pro') {
          this.fileTypeOptions = [
            { label: 'Image', accept: 'image/*' },
            { label: 'PDF', accept: '.pdf' },
            { label: 'Word Document', accept: '.doc,.docx' },
            { label: 'Excel Sheet', accept: '.xls,.xlsx' },
            { label: 'PowerPoint', accept: '.ppt,.pptx' },
            { label: 'Text File', accept: '.txt' },
            { label: 'CSV', accept: '.csv' },
          ];
          this.allowedFileTypes = 'image/*,.pdf,.doc,.docx,.xls,.xlsx,.ppt,.pptx,.txt,.csv';
        } else {
          this.fileTypeOptions = [{ label: 'Image', accept: 'image/*' }];
          this.allowedFileTypes = 'image/*';
        }
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('[Chat] Error fetching user profile:', err);
        this.userPlan = 'Basic';
        this.allowedFileTypes = 'image/*';
      },
    });

    const currentNav = this.router.getCurrentNavigation();
    const state = currentNav?.extras?.state;

    if (state?.['ticketId']) {
      this.ticketId = state['ticketId'];
      this.agentId = state['agentId'];
      console.log(
        '[Chat] Ticket ID from state:',
        this.ticketId,
        'Agent ID:',
        this.agentId
      );
    }

    this.route.queryParams.subscribe((params) => {
      if (!this.ticketId && params['ticketId']) {
        this.ticketId = params['ticketId'];
        console.log('[Chat] Ticket ID from queryParams:', this.ticketId);
      }
      this.isAgent = params['agent'] === 'true';
      this.newSession = params['new'] === 'true';

      console.log(
        '[Chat] Final resolved ticketId:',
        this.ticketId,
        'isAgent:',
        this.isAgent
      );

      if (!this.ticketId) {
        console.error(
          '[Chat] No ticketId found in either state or queryParams!'
        );
        this.toastr.error(
          'No ticket ID found. Please start a new session.',
          'Error'
        );
        this.router.navigate([
          this.isAgent
            ? '/agent/dashboard/workspace/active'
            : '/user/dashboard/tickets/active',
        ]);
        return;
      }

      this.messages = [];
      this.fetchMessages();
      this.startSignalRConnection();
    });
  }

  private startSignalRConnection(): void {
    this.signalRService
      .startConnection()
      .then(() => {
        console.log('[Chat] SignalR connection established');
        this.signalRService
          .joinChat(this.ticketId)
          .then(() => {
            console.log('[Chat] Joined chat for ticket:', this.ticketId);
            this.loading = this.isAgent ? false : true;
            if (this.isAgent) {
              this.agentJoined = true;
              this.killBot();
            } else {
              this.notifyAgentOrAssignNew();
            }
            this.setupSignalRSubscriptions();
            this.cdr.detectChanges();
          })
          .catch((err) => {
            console.error('[Chat] Error joining chat:', err);
            this.toastr.error(
              'Failed to connect to chat. Retrying...',
              'Error'
            );
            this.retrySignalRConnection();
          });
      })
      .catch((err) => {
        console.error('[Chat] Error starting SignalR connection:', err);
        this.toastr.error(
          'Failed to start chat connection. Retrying...',
          'Error'
        );
        this.retrySignalRConnection();
      });
  }

  private retrySignalRConnection(attempts = 3, delay = 3000): void {
    if (attempts <= 0) {
      console.error(
        '[Chat] Max retry attempts reached. Navigating to dashboard.'
      );
      this.toastr.error(
        'Unable to connect to chat. Please try again later.',
        'Error'
      );
      this.router.navigate([
        this.isAgent
          ? '/agent/dashboard/workspace/active'
          : '/user/dashboard/tickets/active',
      ]);
      return;
    }

    setTimeout(() => {
      console.log(
        `[Chat] Retrying SignalR connection, attempts left: ${attempts}`
      );
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
              console.error(
                '[Chat] this.messages is not an array:',
                this.messages
              );
              this.messages = [];
              this.toastr.error(
                'Chat data corrupted. Resetting messages.',
                'Error'
              );
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
              console.error(
                '[Chat] this.messages is not an array:',
                this.messages
              );
              this.messages = [];
              this.toastr.error(
                'Chat data corrupted. Resetting messages.',
                'Error'
              );
            }
          }
        }
      })
    );

    this.subscriptions.push(
      this.signalRService.agentJoined$.subscribe((data) => {
        if (data && data.ticketId === this.ticketId) {
          const currentTime = Date.now();
          if (
            this.agentJoinedProcessed.has(data.ticketId) ||
            currentTime - this.lastAgentJoinedTime < 1000
          ) {
            console.warn('[Chat] Duplicate AgentJoined event ignored:', data);
            return;
          }
          this.agentJoinedProcessed.add(data.ticketId);
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
            this.toastr.success(
              'An agent has joined the chat.',
              'Agent Joined'
            );
            this.scrollToBottom();
            this.cdr.detectChanges();
          } else {
            console.error(
              '[Chat] this.messages is not an array:',
              this.messages
            );
            this.messages = [];
            this.toastr.error(
              'Chat data corrupted. Resetting messages.',
              'Error'
            );
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
              this.toastr.info(
                'The support ticket has been closed.',
                'Chat Ended'
              );
              this.scrollToBottom();
              this.cdr.detectChanges();
              setTimeout(() => this.leaveChat(), 2000);
            } else {
              console.error(
                '[Chat] this.messages is not an array:',
                this.messages
              );
              this.messages = [];
              this.toastr.error(
                'Chat data corrupted. Resetting messages.',
                'Error'
              );
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
            this.toastr.info(
              'Agent is currently unavailable. Please wait.',
              'Info'
            );
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
              this.toastr.info(
                data.text,
                this.isAgent ? 'User Left' : 'Agent Left'
              );
              this.scrollToBottom();
              this.cdr.detectChanges();
              setTimeout(() => this.leaveChat(), 2000);
            } else {
              console.error(
                '[Chat] this.messages is not an array:',
                this.messages
              );
              this.messages = [];
              this.toastr.error(
                'Chat data corrupted. Resetting messages.',
                'Error'
              );
            }
          }
        }
      })
    );

    this.subscriptions.push(
      this.signalRService.leaveChat$.subscribe((data) => {
        if (data && data.ticketId === this.ticketId) {
          console.log('[Chat] Received LeaveChat signal:', data);
          this.chatEnded = true;
          this.loading = false;
          this.router.navigate([
            this.isAgent
              ? '/agent/dashboard/workspace/active'
              : '/user/dashboard/tickets/active',
          ]);
          this.cdr.detectChanges();
        }
      })
    );
  }

  private fetchMessages(): void {
    console.log('[Chat] Fetching messages for ticket:', this.ticketId);
    this.chatService.getMessages(this.ticketId).subscribe({
      next: (response) => {
        const messages = Array.isArray(response)
          ? response
          : (response as any)?.$values || [];
        console.log('[Chat] Raw response from backend:', response);
        if (!Array.isArray(messages)) {
          console.error(
            '[Chat] Received non-array messages from backend:',
            messages
          );
          this.messages = [];
          this.toastr.error('Invalid chat data received from server.', 'Error');
        } else {
          this.messages = messages.map((msg) => ({
            sender: (['bot', 'user', 'agent'].includes(msg.sender)
              ? msg.sender
              : 'user') as 'bot' | 'user' | 'agent',
            text: msg.text,
            fileUrl: msg.fileUrl,
            isImage: msg.isImage ?? false,
            timestamp: msg.timestamp ?? new Date().toISOString(),
          }));
          console.log(
            '[Chat] Fetched messages and attachments:',
            this.messages
          );
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
    if (this.newSession) {
      this.signalRService
        .notifyAgent(this.ticketId)
        .then(() => {
          console.log('[Chat] Agent notified for ticket:', this.ticketId);
          this.waitingForAgent = true;
          this.toastr.info('Waiting for an agent to join the chat.', 'Info');
          this.simulateLoadingAndBot();
        })
        .catch((err) => {
          console.error('[Chat] Error notifying agent:', err);
          this.toastr.error(
            'Failed to notify agent. Please try again.',
            'Error'
          );
          this.router.navigate([
            this.isAgent
              ? '/agent/dashboard/workspace/active'
              : '/user/dashboard/tickets/active',
          ]);
        });
    } else if (this.agentId) {
      this.ticketService.checkAgentAvailability(this.agentId).subscribe({
        next: (response: any) => {
          if (response.data?.isAvailable) {
            this.signalRService
              .notifySpecificAgent(this.ticketId, this.agentId!)
              .then(() => {
                console.log('[Chat] Notified assigned agent:', this.agentId);
                this.waitingForAgent = true;
                this.toastr.info(
                  'Notifying assigned agent to join the chat.',
                  'Info'
                );
                this.simulateLoadingAndBot();
              })
              .catch((err) => {
                console.error('[Chat] Error notifying assigned agent:', err);
                this.toastr.error(
                  'Failed to notify assigned agent. Assigning new agent.',
                  'Error'
                );
                this.assignNewAgent();
              });
          } else {
            console.log(
              '[Chat] Assigned agent unavailable, assigning new agent.'
            );
            this.assignNewAgent();
          }
        },
        error: (err) => {
          console.error('[Chat] Error checking agent availability:', err);
          this.toastr.error(
            'Error checking agent availability. Assigning new agent.',
            'Error'
          );
          this.assignNewAgent();
        },
      });
    } else {
      this.assignNewAgent();
    }
  }

  private assignNewAgent(): void {
    this.ticketService.assignNewAgent().subscribe({
      next: (response: any) => {
        if (response.success && response.data?.agentId) {
          this.agentId = response.data.agentId;
          this.signalRService
            .notifySpecificAgent(this.ticketId, this.agentId!)
            .then(() => {
              console.log('[Chat] Notified new agent:', this.agentId);
              this.waitingForAgent = true;
              this.toastr.info(
                'You are being prioritized for agent assignment based on your subscription plan.',
                'Priority Support'
              );
              this.toastr.info(
                'Notifying a new agent to join the chat.',
                'Info'
              );
              this.simulateLoadingAndBot();
            })
            .catch((err) => {
              console.error('[Chat] Error notifying new agent:', err);
              this.toastr.error(
                'Failed to notify new agent. Please try again.',
                'Error'
              );
              this.router.navigate([
                this.isAgent
                  ? '/agent/dashboard/workspace/active'
                  : '/user/dashboard/tickets/active',
              ]);
            });
        } else {
          this.toastr.error(
            'No available agents at the moment. Please try again later.',
            'Error'
          );
          this.router.navigate([
            this.isAgent
              ? '/agent/dashboard/workspace/active'
              : '/user/dashboard/tickets/active',
          ]);
        }
      },
      error: (err) => {
        console.error('[Chat] Error assigning new agent:', err);
        this.toastr.error(
          'Failed to assign new agent. Please try again.',
          'Error'
        );
        this.router.navigate([
          this.isAgent
            ? '/agent/dashboard/workspace/active'
            : '/user/dashboard/tickets/active',
        ]);
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
    this.signalRService.stopConnection();
    this.subscriptions.forEach((sub) => sub.unsubscribe());
    this.agentJoinedProcessed.clear();
    this.lastAgentJoinedTime = 0;
    this.messages = [];
  }

  scrollToBottom(): void {
    try {
      if (this.chatContainer?.nativeElement) {
        this.chatContainer.nativeElement.scrollTop =
          this.chatContainer.nativeElement.scrollHeight;
      }
    } catch (err) {
      console.error('[Chat] Scroll error:', err);
    }
  }

  simulateLoadingAndBot() {
    if (!Array.isArray(this.messages)) {
      console.error(
        '[Chat] this.messages is not an array in simulateLoadingAndBot:',
        this.messages
      );
      this.messages = [];
      this.toastr.error('Chat data corrupted. Resetting messages.', 'Error');
    }
    this.messages.push({
      sender: 'bot',
      text: 'Loading your support ticket chat...',
    });
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
        console.log(
          '[Chat] Agent has joined or chat ended, stopping bot messages.'
        );
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
        console.error(
          '[Chat] this.messages is not an array in botMessageLoop:',
          this.messages
        );
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
      this.toastr.info(
        'Chat has ended. Please start a new session to upload files.',
        'Info'
      );
      return;
    }

    const input = event.target as HTMLInputElement;
    const file = input?.files?.[0];
    if (!file || !this.ticketId) {
      this.toastr.error('No file selected or invalid ticket ID.', 'Error');
      return;
    }

    const allowedTypes = this.allowedFileTypes.split(',').map((t) => t.trim());
    let valid = false;
    for (const type of allowedTypes) {
      if (type.endsWith('/*')) {
        if (file.type.startsWith(type.replace('/*', '/'))) {
          valid = true;
          break;
        }
      } else if (type.startsWith('.')) {
        if (file.name.toLowerCase().endsWith(type)) {
          valid = true;
          break;
        }
      } else {
        if (file.type === type) {
          valid = true;
          break;
        }
      }
    }
    if (!valid) {
      this.toastr.error(
        `Your plan (${this.userPlan}) only allows uploading: ${
          this.userPlan === 'Basic'
            ? 'photos/images'
            : 'images, audio, video, and documents'
        }.`,
        'File type not allowed'
      );
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
        this.toastr.error(
          err.error?.message || 'Failed to upload file.',
          'Error'
        );
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
        this.signalRService.stopConnection();
        this.messages = [];
        this.userInput = '';
        this.agentJoined = false;
        this.newSession = false;
        this.chatEnded = false;
        this.loading = false;
        this.agentJoinedProcessed.clear();
        this.lastAgentJoinedTime = 0;
        this.waitingForAgent = false;
        this.router.navigate([
          this.isAgent
            ? '/agent/dashboard/workspace/active'
            : '/user/dashboard/tickets/active',
        ]);
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('[Chat] Error leaving chat:', err);
        this.toastr.error('Failed to leave chat. Please try again.', 'Error');
      },
    });
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
                this.toastr.success(
                  'Chat and ticket ended successfully.',
                  'Success'
                );
                this.cdr.detectChanges();
              })
              .catch((err) => {
                console.error('[Chat] Error ending chat via SignalR:', err);
                this.toastr.error(
                  'Failed to end chat. Please try again.',
                  'Error'
                );
              });
          } else {
            console.warn(
              '[Chat] Cannot end chat: SignalR connection is not active'
            );
            this.toastr.error(
              'Failed to end chat due to connection issues.',
              'Error'
            );
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