import { Injectable } from "@angular/core";
import * as signalR from "@microsoft/signalr";
import { environment } from "../../environments/environment";
import { BehaviorSubject } from "rxjs";
import { AuthService } from "./auth.service";

@Injectable({ providedIn: 'root' })
export class SignalRService {
    private hubConnection: signalR.HubConnection | null = null;
    private baseUrl = environment.baseUrl;
    private ticketNotificationSource = new BehaviorSubject<any>(null);
    private messageSource = new BehaviorSubject<any>(null);
    private fileSource = new BehaviorSubject<any>(null);
    private agentJoinedSource = new BehaviorSubject<any>(null);
    private chatEndedSource = new BehaviorSubject<any>(null);
    private connectionEstablished = new BehaviorSubject<boolean>(false);
    private agentAvailabilitySource = new BehaviorSubject<boolean>(false);

    ticketNotification$ = this.ticketNotificationSource.asObservable();
    message$ = this.messageSource.asObservable();
    file$ = this.fileSource.asObservable();
    agentJoined$ = this.agentJoinedSource.asObservable();
    chatEnded$ = this.chatEndedSource.asObservable();
    agentAvailability$ = this.agentAvailabilitySource.asObservable();

    private isListenersRegistered = false;

    constructor(private authService: AuthService) {}

    public startConnection(): void {
        if (this.hubConnection && this.hubConnection.state === signalR.HubConnectionState.Connected) {
            console.log("SignalR already connected");
            this.connectionEstablished.next(true);
            return;
        }

        const token = this.authService.getToken();
        if (!token) {
            console.error("No auth token available for SignalR connection");
            return;
        }

        this.hubConnection = new signalR.HubConnectionBuilder()
            .withUrl(`${this.baseUrl}/chathub`, { accessTokenFactory: () => token })
            .withAutomaticReconnect({
                nextRetryDelayInMilliseconds: () => {
                    return Math.random() * 10000 + 5000; // Random delay between 5-15s
                }
            })
            .build();

        this.hubConnection.onreconnecting((err) => {
            console.warn("SignalR reconnecting:", err);
        });

        this.hubConnection.onreconnected(() => {
            console.log("SignalR reconnected");
            this.connectionEstablished.next(true);
            this.registerListeners();
        });

        this.hubConnection.onclose((err) => {
            console.error("SignalR connection closed:", err);
            this.connectionEstablished.next(false);
        });

        this.hubConnection.start()
            .then(() => {
                console.log("SignalR connected");
                this.connectionEstablished.next(true);
                this.registerListeners();
            })
            .catch(err => {
                console.error("SignalR connection error: ", err);
                this.connectionEstablished.next(false);
            });
    }

    private async waitForConnection(): Promise<void> {
        if (this.hubConnection && this.hubConnection.state === signalR.HubConnectionState.Connected) {
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
                if (!this.hubConnection || this.hubConnection.state !== signalR.HubConnectionState.Connected) {
                    subscription.unsubscribe();
                    reject(new Error("SignalR connection timeout"));
                }
            }, 15000); // Increased to 15s for stability
        });
    }

    private registerListeners(): void {
        if (this.isListenersRegistered || !this.hubConnection) return;

        this.hubConnection.off("ReceiveTicketAssignedNotification");
        this.hubConnection.off("ReceiveMessage");
        this.hubConnection.off("ReceiveFile");
        this.hubConnection.off("AgentJoined");
        this.hubConnection.off("ChatEnded");
        this.hubConnection.off("AgentAvailability");

        this.hubConnection.on("ReceiveTicketAssignedNotification", (data) => {
            console.log("Received ticket notification:", data);
            this.ticketNotificationSource.next(data);
        });

        this.hubConnection.on("ReceiveMessage", (data) => {
            console.log("Received message:", data);
            this.messageSource.next(data);
        });

        this.hubConnection.on("ReceiveFile", (data) => {
            console.log("Received file:", data);
            this.fileSource.next(data);
        });

        this.hubConnection.on("AgentJoined", (data) => {
            console.log("Received AgentJoined:", data); // Debug to check for multiple triggers
            this.agentJoinedSource.next(data);
        });

        this.hubConnection.on("ChatEnded", (data) => {
            console.log("Received ChatEnded:", data);
            this.chatEndedSource.next(data);
        });

        this.hubConnection.on("AgentAvailability", (data) => {
            console.log("Received AgentAvailability:", data);
            this.agentAvailabilitySource.next(data.available);
        });

        this.isListenersRegistered = true;
    }

    public async joinChat(ticketId: string): Promise<void> {
        if (!ticketId) {
            throw new Error("Ticket ID is required");
        }
        await this.waitForConnection();
        if (this.hubConnection) {
            await this.hubConnection.invoke("JoinChat", ticketId)
                .catch(err => {
                    console.error("Error joining chat:", err);
                    throw err;
                });
        }
    }

    public async notifyAgent(ticketId: string): Promise<void> {
        if (!ticketId) {
            throw new Error("Ticket ID is required");
        }
        await this.waitForConnection();
        if (this.hubConnection) {
            await this.hubConnection.invoke("NotifyAgent", ticketId)
                .catch(err => {
                    console.error("Error notifying agent:", err);
                    throw err;
                });
        }
    }
    
    public async sendMessage(ticketId: string, senderId: string, content: string): Promise<void> {
        await this.waitForConnection();
        if (this.hubConnection) {
            await this.hubConnection.invoke("SendMessage", ticketId, senderId, content)
                .catch(err => {
                    console.error("Error sending message:", err);
                    throw err;
                });
        }
    }

    public async sendFile(ticketId: string, senderId: string, fileUrl: string, isImage: boolean): Promise<void> {
        await this.waitForConnection();
        if (this.hubConnection) {
            await this.hubConnection.invoke("SendFile", ticketId, senderId, fileUrl, isImage)
                .catch(err => {
                    console.error("Error sending file:", err);
                    throw err;
                });
        }
    }

    public async endChat(ticketId: string): Promise<void> {
        await this.waitForConnection();
        if (this.hubConnection) {
            await this.hubConnection.invoke("EndChat", ticketId)
                .catch(err => {
                    console.error("Error ending chat:", err);
                    throw err;
                });
        }
    }

    public async leaveChat(ticketId: string): Promise<void> {
        await this.waitForConnection();
        if (this.hubConnection) {
            await this.hubConnection.invoke("LeaveChat", ticketId)
                .catch(err => {
                    console.error("Error leaving chat:", err);
                    throw err;
                });
        }
    }

    public stopConnection(): void {
        if (this.hubConnection) {
            this.hubConnection.stop()
                .then(() => {
                    console.log("SignalR disconnected");
                    this.connectionEstablished.next(false);
                    this.isListenersRegistered = false;
                    this.hubConnection = null;
                })
                .catch(err => console.error("SignalR error on disconnect:", err));
        }
    }
}