namespace Customer_Support_Chatbot.DTOs.Admin
{
    public class PagedTicketResponseDto
    {
        public List<TicketDto> Tickets{ get; set; } = new List<TicketDto>();
        public int TotalCount { get; set; }
    }
}