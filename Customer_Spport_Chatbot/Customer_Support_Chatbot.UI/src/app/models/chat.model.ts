export interface ChatMessageDto {
    content: string;
    senderRole: string;
    sentAt: Date;
}

export interface ChatAttachmentDto {
    fileName: string;
    url: string;
}

export interface ChatSessionDto {
    ticketId: string;
    title: string;
    status: string;
    userName: string;
    agentName: string;
    messages: ChatMessageDto[];
    attachments: ChatAttachmentDto[];
}