namespace Customer_Support_Chatbot.DTOs.Ticket
{
    public class TicketResponseDto
    {
        public Guid TicketId { get; set; }
        public Guid AssignedAgentId { get; set; }
        public string Title { get; set; }=string.Empty;
        public string Description { get; set; }=string.Empty;
    }
}