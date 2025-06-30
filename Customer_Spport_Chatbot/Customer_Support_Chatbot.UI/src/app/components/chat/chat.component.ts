import { CommonModule } from "@angular/common";
import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { TicketService } from "../../services/ticket.service";
import { ChatService } from "../../services/chat.service";
import { ApiResponse } from "../../models/api.model";

@Component({
    selector: 'app-chat',
    templateUrl: './chat.component.html',
    standalone: true,
    imports: [CommonModule, FormsModule],
})
export class ChatComponent implements OnInit,OnDestroy {
    
    messages: { sender: 'bot' | 'user' | 'agent'; text?: string; fileUrl?: string; isImage?: boolean }[] = [];
    userInput: string = '';
    agentJoined = false;
    newSession = false;
    loading = true;
    isAgent=false;
    ticketId: string = '';
    botInterval:any;

    @ViewChild('chatContainer') chatContainer!: ElementRef;

    constructor(private route: ActivatedRoute,private router:Router,private ticketService:TicketService,private chatService:ChatService){}
    ngOnInit(): void {
        const url=this.router.url;
        const isAgent=url.includes('/agent/');
        if(isAgent){
            this.isAgent = true;
        }
        this.route.queryParams.subscribe(params => {
            this.newSession = params['new'] === 'true';
            this.ticketId = params['ticketId'] || '';
            this.isAgent = params['agent'] === 'true';
            if (this.newSession) {
                this.simulateLoadingAndBot();
            }
            else if(this.isAgent){
                this.loading = false;
                this.agentJoined = true;
                if (this.botInterval) {
                    clearInterval(this.botInterval);
                }
                this.messages.push({ sender: 'agent', text: 'Hello! I’m here to help you now.' });
            }
        });
    }

    ngAfterViewChecked() {
        this.scrollToBottom();
    }

    ngOnDestroy(): void {
        if (this.botInterval) {
            clearInterval(this.botInterval);
        }
    }

    scrollToBottom(): void {
        try {
            this.chatContainer.nativeElement.scrollTop = this.chatContainer.nativeElement.scrollHeight;
        } catch (err) {
            console.error('Scroll error:', err);
        }
    }
    
    simulateLoadingAndBot() {
        this.messages.push({ sender: 'bot', text: 'Loading your support ticket chat...' });
        setTimeout(() => {
            this.loading = false;
            this.botMessageLoop();
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
        const interval = setInterval(() => {
        if (this.agentJoined || i >= botMsgs.length) {
            clearInterval(interval);
            if (!this.agentJoined && this.isAgent) {
                this.messages.push({ sender: 'agent', text: 'Hello! I’m here to help you now.' });
                return;
            }
            return;
        }
        this.messages.push({ sender: 'bot', text: botMsgs[i++] });
        }, Math.floor(Math.random() * 4000) + 6000); // random 6-10s
    }

    sendMessage(msg: string) {
        if (!msg.trim()) return;
        this.messages.push({ sender: this.isAgent ? 'agent' : 'user', text: msg });
        this.userInput = '';
    }

    handleFileUpload(event: Event) {
        const input = event.target as HTMLInputElement;
        const file = input?.files?.[0];
        if (file && file.type.startsWith('image/')) {
            const fileUrl = URL.createObjectURL(file);
            this.messages.push({
                sender: this.isAgent ? 'agent' : 'user',
                fileUrl,
                isImage: true,
            });
        }
    }

    leaveChat(){
        this.messages=[];
        this.userInput = '';
        this.agentJoined = false;
        this.newSession = false;
        this.router.navigate([this.isAgent?'/agent/dashboard/workspace/active':'/user/dashboard/tickets/active'])
    }

    endChat(){
        if(!this.ticketId) return;
        if(this.isAgent){
            this.ticketService.endTicket(this.ticketId).subscribe({
                next:(res:ApiResponse<string>)=>{
                    if(res.success){
                        this.messages.push({ sender: 'agent', text: 'The support ticket has been closed.' });
                        setTimeout(()=>this.leaveChat(),2000);
                    }
                    else{
                        console.error('Error ending ticket:', res.message);
                    }
                },
                error:(err:any)=>{
                    console.error('Error ending ticket:', err);
                }
            })
        }
    }
}
