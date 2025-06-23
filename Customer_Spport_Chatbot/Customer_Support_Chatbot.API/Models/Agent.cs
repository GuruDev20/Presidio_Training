namespace Customer_Support_Chatbot.Models
{
    public class Agent
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public string Status { get; set; } = "Available";
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Ticket>? AssignedTickets { get; set; }
    }
}