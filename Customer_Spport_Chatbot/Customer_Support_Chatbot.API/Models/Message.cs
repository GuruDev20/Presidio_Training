namespace Customer_Support_Chatbot.Models
{
    public class Message
    {
        public Guid Id { get; set; }
        public Guid TicketId{ get; set; }
        public Ticket? Ticket { get; set; }
        public Guid SenderId { get; set; }
        public User? Sender { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}