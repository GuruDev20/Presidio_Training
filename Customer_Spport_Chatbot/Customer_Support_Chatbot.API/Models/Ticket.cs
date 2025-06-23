namespace Customer_Support_Chatbot.Models
{
    public class Ticket
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "Open";
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public Guid? AgentId { get; set; }
        public Agent? Agent { get; set; }
        public ICollection<Message>? Messages { get; set; }
        public ICollection<FileAttachment>? Attachments { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ClosedAt { get; set; }
    }
}