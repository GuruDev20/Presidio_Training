export interface TicketHistoryFilter{
    userOrAgentId: string;
    role:string;
    searchKeyword?: string;
    timeRange?:string;
}

export interface TicketHistoryResponse{
    id: string;
    name: string;
    description: string;
    status: string;
    userId: string;
    agentId: string | null;
    createdAt: string;
    closedAt?: string;
}

export interface CreateTicket{
    userId: string;
    title: string;
    description: string;
}

export interface TicketResponse{
    ticketId: string;
    assignedAgentId: string | null;
    title: string;
    description: string;
}

export interface Ticket {
    id: string;
    title: string;
    status: 'Active' | 'Pending' | 'Closed';
}