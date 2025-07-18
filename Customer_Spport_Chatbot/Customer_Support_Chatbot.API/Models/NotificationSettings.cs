namespace Customer_Support_Chatbot.Models
{
    public class NotificationSettings
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public bool IsEnabled { get; set; } = true;
        public string Position { get; set; } = string.Empty; // e.g., "top-right", "bottom-left"
        public bool ChatNotifications { get; set; }
        public bool SystemNotifications { get; set; }
    }
}