namespace Customer_Support_Chatbot.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public string? ProfilePictureUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeactivated { get; set; } = false;
        public DateTime? DeactivationRequestedAt { get; set; }
        public ICollection<Ticket>? Tickets { get; set; }
        public ICollection<RefreshToken>? RefreshTokens { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Subscription>? Subscriptions { get; set; }
    }
}