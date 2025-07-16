namespace Customer_Support_Chatbot.Models
{
    public class UserDevice
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string DeviceId { get; set; } = string.Empty;
        public string DeviceType { get; set; } = string.Empty;
        public string OS { get; set; } = string.Empty;
        public string Browser { get; set; } = string.Empty;
        public DateTime LastLogin { get; set; }
        public bool IsActive { get; set; } = true;
        public User User { get; set; } = null!;
    }
}