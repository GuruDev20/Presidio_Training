import { Component, OnInit } from "@angular/core";
import { AuthService } from "../../../services/auth.service";
import { UserService } from "../../../services/user.service";
import { CommonModule } from "@angular/common";
import { LucideIconsModule } from "../../../utils/lucide-icons.module";
import { ChatSessionDto, ChatMessageDto, ChatAttachmentDto } from "../../../models/chat.model";
import { environment } from "../../../../environments/environment";

@Component({
    selector: 'app-agent-history',
    templateUrl: './history.component.html',
    standalone: true,
    imports: [CommonModule, LucideIconsModule]
})
export class AgentHistoryComponent implements OnInit {
    history: any[] = [];
    currentPage = 1;
    itemsPerPage = 10;
    openMenuIndex: number | null = null;
    selectedChatSession: ChatSessionDto | null = null;
    showChatPanel = false;
    curentUserRole: string | null = null;
    private baseApiUrl = environment.baseUrl;
    timeline: Array<{ type: 'message', data: ChatMessageDto } | { type: 'attachment', data: ChatAttachmentDto }> = [];

    constructor(private authService: AuthService, private userService: UserService) {}

    ngOnInit(): void {
        const userId = this.authService.getUserId();
        const role = this.authService.getRole();
        this.curentUserRole = role;
        this.userService.getTicketsByUser({ userOrAgentId: userId, role })
            .subscribe({
                next: (res) => {
                    this.history = Array.isArray((res.data as any)?.$values)
                        ? (res.data as any).$values
                        : [];
                },
                error: (err) => {
                    console.error('Error fetching ticket history:', err);
                }
            });
    }

    get totalPages(): number {
        return Math.ceil(this.history.length / this.itemsPerPage);
    }

    get paginatedHistory() {
        const start = (this.currentPage - 1) * this.itemsPerPage;
        return this.history.slice(start, start + this.itemsPerPage);
    }

    nextPage() {
        if (this.currentPage < this.totalPages) {
            this.currentPage++;
        }
    }

    prevPage() {
        if (this.currentPage > 1) {
            this.currentPage--;
        }
    }

    toggleMenu(index: number) {
        this.openMenuIndex = this.openMenuIndex === index ? null : index;
    }

    closeAllMenus() {
        this.openMenuIndex = null;
    }

    viewDetails(ticketId: string) {
        this.userService.getChatSession(ticketId).subscribe({
            next: (res) => {
                console.log('Chat session details:', res);
                this.selectedChatSession = res as unknown as ChatSessionDto;
                // Process messages
                if (this.selectedChatSession && (this.selectedChatSession.messages as any)?.$values) {
                    this.selectedChatSession.messages = (this.selectedChatSession.messages as any).$values;
                } else if (!Array.isArray(this.selectedChatSession?.messages)) {
                    this.selectedChatSession.messages = [];
                }
                // Process attachments
                if (this.selectedChatSession && (this.selectedChatSession.attachments as any)?.$values) {
                    this.selectedChatSession.attachments = (this.selectedChatSession.attachments as any).$values.map((attachment: any) => ({
                        ...attachment,
                        url: attachment.url.startsWith('http') ? attachment.url : `${this.baseApiUrl}${attachment.url}`,
                        sentAt: attachment.sendAt ? new Date(attachment.sendAt) : attachment.sentAt ? new Date(attachment.sentAt) : new Date()
                    }));
                } else if (!Array.isArray(this.selectedChatSession?.attachments)) {
                    this.selectedChatSession.attachments = [];
                }
                // Create timeline
                this.timeline = [];
                let lastSenderRole = 'Agent'; // Default fallback
                // Add messages to timeline
                this.selectedChatSession.messages.forEach(message => {
                    this.timeline.push({ type: 'message', data: message });
                    lastSenderRole = message.senderRole || lastSenderRole;
                });
                // Add attachments to timeline
                this.selectedChatSession.attachments.forEach(attachment => {
                    this.timeline.push({
                        type: 'attachment',
                        data: {
                            ...attachment,
                            senderRole: attachment.senderRole || lastSenderRole
                        }
                    });
                });
                // Sort timeline by sentAt
                this.timeline.sort((a, b) => {
                    const aTime = a.data.sentAt ? new Date(a.data.sentAt).getTime() : Number.MAX_SAFE_INTEGER;
                    const bTime = b.data.sentAt ? new Date(b.data.sentAt).getTime() : Number.MAX_SAFE_INTEGER;
                    return aTime - bTime;
                });
                this.showChatPanel = true;
                this.closeAllMenus();
            },
            error: (err) => {
                console.error('Error fetching chat session:', err);
                this.selectedChatSession = null;
                this.showChatPanel = false;
                this.timeline = [];
            }
        });
    }

    closeChatPanel() {
        this.showChatPanel = false;
        setTimeout(() => {
            this.selectedChatSession = null;
            this.timeline = [];
        }, 300);
    }

    deleteHistory(index: number) {
        
    }

    isImage(fileName: string): boolean {
        const imageExtensions = ['.jpg', '.jpeg', '.png', '.gif', '.bmp', '.webp'];
        return imageExtensions.some(ext => fileName.toLowerCase().endsWith(ext));
    }
}