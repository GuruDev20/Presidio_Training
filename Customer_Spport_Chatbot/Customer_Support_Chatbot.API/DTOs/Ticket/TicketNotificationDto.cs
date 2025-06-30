namespace Customer_Support_Chatbot.DTOs.Ticket
{
    public class TicketNotificationDto
    {
        public Guid TicketId { get; set; }
        public string Title { get; set; }= string.Empty;
        public string Description { get; set; }= string.Empty;
        public Guid CustomerId { get; set; }
    }
}