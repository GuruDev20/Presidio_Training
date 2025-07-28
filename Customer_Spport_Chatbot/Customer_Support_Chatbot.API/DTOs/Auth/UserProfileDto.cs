using Customer_Support_Chatbot.Models;

namespace Customer_Support_Chatbot.DTOs.Auth
{
    public class UserProfileDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? ProfilePictureUrl { get; set; } = null;
        public DateTime CreatedAt { get; set; }
        public ICollection<UserSubscription>? Subscriptions { get; set; }
    }
}