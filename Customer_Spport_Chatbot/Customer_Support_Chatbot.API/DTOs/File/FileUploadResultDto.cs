namespace Customer_Support_Chatbot.DTOs.File
{
    public class FileUploadResultDto
    {
        public string FileName { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public Guid TicketId{ get; set; }
    }
}