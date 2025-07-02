import { Component, OnInit } from "@angular/core";
import { AuthService } from "../../../services/auth.service";
import { UserService } from "../../../services/user.service";
import { CommonModule } from "@angular/common";
import { LucideIconsModule } from "../../../utils/lucide-icons.module";
import { ChatSessionDto } from "../../../models/chat.model";

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

    constructor(private authService: AuthService, private userService: UserService) {}

    ngOnInit(): void {
        const userId = this.authService.getUserId();
        const role = this.authService.getRole();
        this.userService.getTicketsByUser({ userOrAgentId: userId, role })
            .subscribe({
                next: (res) => {
                    console.log('Ticket history response:', res.data);
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
                console.log('Chat session response:', res);
                if (res.success) {
                    this.selectedChatSession = res.data as ChatSessionDto;
                    console.log('Selected chat session:', this.selectedChatSession);
                    this.showChatPanel = true;
                    this.closeAllMenus();
                }
            },
            error: (err) => {
                console.error('Error fetching chat session:', err);
            }
        });
    }

    closeChatPanel() {
        this.showChatPanel = false;
        setTimeout(() => {
            this.selectedChatSession = null;
        }, 300);
    }

    deleteHistory(index: number) {
    }
}