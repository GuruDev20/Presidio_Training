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
    agentId: string;
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
    assignedAgentId: string;
    title: string;
    description: string;
}