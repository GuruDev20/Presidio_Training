namespace Customer_Support_Chatbot.DTOs.Auth
{
    public class UserDeviceDto
    {
        public string DeviceId { get; set; } = string.Empty;
        public string DeviceType { get; set; } = string.Empty;
        public string OS { get; set; } = string.Empty;
        public string Browser { get; set; } = string.Empty;
        public DateTime LastLogin { get; set; }
    }
}