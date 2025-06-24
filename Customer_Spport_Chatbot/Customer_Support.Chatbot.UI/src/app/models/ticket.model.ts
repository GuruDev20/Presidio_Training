export interface TicketHistoryFilter{
    userOrAgentId: string;
    role:string;
    searchKeyword?: string;
    timeRange?:string;
}