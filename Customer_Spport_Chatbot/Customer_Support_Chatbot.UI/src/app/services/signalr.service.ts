import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { environment } from '../../environments/environment';
import { AuthService } from './auth.service';

@Injectable({ providedIn: 'root' })
export class SignalRService {
  private hubConnection: signalR.HubConnection | null = null;
  private baseUrl = environment.baseUrl;
  private ticketNotificationSource = new BehaviorSubject<any>(null);
  private messageSource = new BehaviorSubject<any>(null);
  private fileSource = new BehaviorSubject<any>(null);
  private agentJoinedSource = new BehaviorSubject<any>(null);
  private chatEndedSource = new BehaviorSubject<any>(null);
  private agentAvailabilitySource = new BehaviorSubject<boolean>(false);
  private userLeftChatSource = new BehaviorSubject<any>(null);
  private leaveChatSource = new BehaviorSubject<any>(null);
  private connectionEstablished = new BehaviorSubject<boolean>(false);

  ticketNotification$ = this.ticketNotificationSource.asObservable();
  message$ = this.messageSource.asObservable();
  file$ = this.fileSource.asObservable();
  agentJoined$ = this.agentJoinedSource.asObservable();
  chatEnded$ = this.chatEndedSource.asObservable();
  agentAvailability$ = this.agentAvailabilitySource.asObservable();
  userLeftChat$ = this.userLeftChatSource.asObservable();
  leaveChat$ = this.leaveChatSource.asObservable();

  private isListenersRegistered = false;

  constructor(private authService: AuthService) {}

  public async startConnection(): Promise<void> {
    if (this.hubConnection && this.hubConnection.state === signalR.HubConnectionState.Connected) {
      console.log('[SignalR] Already connected');
      this.connectionEstablished.next(true);
      return Promise.resolve();
    }

    const token = this.authService.getToken();
    if (!token) {
      console.error('[SignalR] No auth token available');
      return Promise.reject('No auth token available');
    }

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${this.baseUrl}/chathub`, { accessTokenFactory: () => token })
      .withAutomaticReconnect({
        nextRetryDelayInMilliseconds: retryContext => {
          const delay = Math.min(1000 * Math.pow(2, retryContext.previousRetryCount), 30000);
          console.log(`[SignalR] Reconnect attempt ${retryContext.previousRetryCount + 1}, delay: ${delay}ms`);
          return delay;
        }
      })
      .configureLogging(signalR.LogLevel.Information)
      .build();

    this.hubConnection.onreconnecting((err) => {
      console.warn('[SignalR] Reconnecting:', err);
      this.connectionEstablished.next(false);
    });

    this.hubConnection.onreconnected(() => {
      console.log('[SignalR] Reconnected');
      this.connectionEstablished.next(true);
      this.registerListeners();
    });

    this.hubConnection.onclose((err) => {
      console.error('[SignalR] Connection closed:', err);
      this.connectionEstablished.next(false);
      this.isListenersRegistered = false;
    });

    try {
      await this.hubConnection.start();
      console.log('[SignalR] Connected');
      this.connectionEstablished.next(true);
      this.registerListeners();
      return Promise.resolve();
    } catch (err) {
      console.error('[SignalR] Connection error:', err);
      this.connectionEstablished.next(false);
      return Promise.reject(err);
    }
  }

  public isConnected(): boolean {
    return this.hubConnection !== null && this.hubConnection.state === signalR.HubConnectionState.Connected;
  }

  private registerListeners(): void {
    if (this.isListenersRegistered || !this.hubConnection) return;

    this.hubConnection.off('ReceiveTicketAssignedNotification');
    this.hubConnection.off('ReceiveMessage');
    this.hubConnection.off('ReceiveFile');
    this.hubConnection.off('AgentJoined');
    this.hubConnection.off('ChatEnded');
    this.hubConnection.off('AgentAvailability');
    this.hubConnection.off('UserLeftChat');
    this.hubConnection.off('LeaveChat');

    this.hubConnection.on('ReceiveTicketAssignedNotification', (data) => {
      console.log('[SignalR] Received ticket notification:', data);
      this.ticketNotificationSource.next(data);
    });

    this.hubConnection.on('ReceiveMessage', (data) => {
      console.log('[SignalR] Received message:', data);
      this.messageSource.next(data);
    });

    this.hubConnection.on('ReceiveFile', (data) => {
      console.log('[SignalR] Received file:', data);
      this.fileSource.next(data);
    });

    this.hubConnection.on('AgentJoined', (data) => {
      console.log('[SignalR] Received AgentJoined:', data);
      this.agentJoinedSource.next(data);
    });

    this.hubConnection.on('ChatEnded', (data) => {
      console.log('[SignalR] Received ChatEnded:', data);
      this.chatEndedSource.next(data);
    });

    this.hubConnection.on('AgentAvailability', (available) => {
      console.log('[SignalR] Received AgentAvailability:', available);
      this.agentAvailabilitySource.next(available);
    });

    this.hubConnection.on('UserLeftChat', (data) => {
      console.log('[SignalR] Received UserLeftChat:', data);
      this.userLeftChatSource.next(data);
    });

    this.hubConnection.on('LeaveChat', (data) => {
      console.log('[SignalR] Received LeaveChat:', data);
      this.leaveChatSource.next(data);
    });

    this.isListenersRegistered = true;
  }

  public async joinChat(ticketId: string): Promise<void> {
    if (!ticketId) {
      throw new Error('[SignalR] Ticket ID is required');
    }
    await this.waitForConnection();
    if (this.hubConnection) {
      await this.hubConnection.invoke('JoinChat', ticketId)
        .catch(err => {
          console.error('[SignalR] Error joining chat:', err);
          throw err;
        });
    }
  }

  public async notifyAgent(ticketId: string): Promise<void> {
    if (!ticketId) {
      throw new Error('[SignalR] Ticket ID is required');
    }
    await this.waitForConnection();
    if (this.hubConnection) {
      await this.hubConnection.invoke('NotifyAgent', ticketId)
        .catch(err => {
          console.error('[SignalR] Error notifying agent:', err);
          throw err;
        });
    }
  }

  public async notifySpecificAgent(ticketId: string, agentId: string): Promise<void> {
    if (!ticketId || !agentId) {
      throw new Error('[SignalR] Ticket ID and Agent ID are required');
    }
    await this.waitForConnection();
    if (this.hubConnection) {
      await this.hubConnection.invoke('NotifySpecificAgent', ticketId, agentId)
        .catch(err => {
          console.error('[SignalR] Error notifying specific agent:', err);
          throw err;
        });
    }
  }

  public async sendMessage(ticketId: string, senderId: string, content: string): Promise<void> {
    await this.waitForConnection();
    if (this.hubConnection) {
      await this.hubConnection.invoke('SendMessage', ticketId, senderId, content)
        .catch(err => {
          console.error('[SignalR] Error sending message:', err);
          throw err;
        });
    }
  }

  public async sendFile(ticketId: string, senderId: string, fileUrl: string, isImage: boolean): Promise<void> {
    await this.waitForConnection();
    if (this.hubConnection) {
      await this.hubConnection.invoke('SendFile', ticketId, senderId, fileUrl, isImage)
        .catch(err => {
          console.error('[SignalR] Error sending file:', err);
          throw err;
        });
    }
  }

  public async endChat(ticketId: string): Promise<void> {
    await this.waitForConnection();
    if (this.hubConnection) {
      await this.hubConnection.invoke('EndChat', ticketId)
        .catch(err => {
          console.error('[SignalR] Error ending chat:', err);
          throw err;
        });
    }
  }

  public async leaveChat(ticketId: string): Promise<void> {
    if (!ticketId) {
      throw new Error('[SignalR] Ticket ID is required');
    }
    await this.waitForConnection();
    if (this.hubConnection) {
      await this.hubConnection.invoke('LeaveChat', ticketId)
        .catch(err => {
          console.error('[SignalR] Error leaving chat:', err);
          throw err;
        });
    }
  }

  public stopConnection(): void {
    if (this.hubConnection && this.hubConnection.state !== signalR.HubConnectionState.Disconnected) {
      this.hubConnection.stop()
        .then(() => {
          console.log('[SignalR] Disconnected');
          this.connectionEstablished.next(false);
          this.isListenersRegistered = false;
          this.hubConnection = null;
        })
        .catch(err => console.error('[SignalR] Error on disconnect:', err));
    } else {
      console.log('[SignalR] Already disconnected or no connection');
      this.connectionEstablished.next(false);
      this.isListenersRegistered = false;
      this.hubConnection = null;
    }
  }

  private async waitForConnection(): Promise<void> {
    if (this.isConnected()) {
      return Promise.resolve();
    }

    return new Promise((resolve, reject) => {
      const subscription = this.connectionEstablished.subscribe(connected => {
        if (connected) {
          subscription.unsubscribe();
          resolve();
        }
      });

      setTimeout(() => {
        if (!this.isConnected()) {
          subscription.unsubscribe();
          reject(new Error('[SignalR] Connection timeout'));
        }
      }, 15000);
    });
  }
}