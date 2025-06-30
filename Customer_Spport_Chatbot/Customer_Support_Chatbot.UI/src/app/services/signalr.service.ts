import { Injectable } from "@angular/core";
import * as signalR from "@microsoft/signalr";
import { environment } from "../../environments/environment";
import { BehaviorSubject } from "rxjs";
import { AuthService } from "./auth.service";

@Injectable({providedIn:'root'})

export class SignalRService{

    private hubConnection!: signalR.HubConnection;
    private baseUrl=environment.baseUrl;
    private ticketNotificationSource=new BehaviorSubject<any>(null);
    ticketNotification$ = this.ticketNotificationSource.asObservable();
    private isListenersRegistered = false;

    constructor(private authServive:AuthService){}

    public startConnection():void{
        const token= this.authServive.getToken();
        this.hubConnection = new signalR.HubConnectionBuilder()
            .withUrl(`${this.baseUrl}/chathub`, { accessTokenFactory: () => token })
            // .configureLogging(signalR.LogLevel.Information)
            .withAutomaticReconnect()
            .build();

        this.hubConnection.start()
            .then(() => console.log("SignalR connected"))
            .catch(err => console.error("SignalR error: ", err));
        
        this.registerListeners();
    }

    private registerListeners(): void{

        if (this.isListenersRegistered) return;
        this.hubConnection.off("ReceiveTicketAssignedNotification");

        this.hubConnection.on("ReceiveTicketAssignedNotification",(data)=>{
            this.ticketNotificationSource.next(data);
        })

        this.isListenersRegistered = true;
    }

    public stopConnection():void{
        if(this.hubConnection){
            this.hubConnection.stop()
                .then(() => console.log("SignalR disconnected"))
                .catch(err => console.error("SignalR error on disconnect: ", err));
        }
    }
}