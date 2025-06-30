using System.Security.Claims;
using Customer_Support_Chatbot.Contexts;
using Customer_Support_Chatbot.Services;
using Microsoft.AspNetCore.SignalR;

namespace Customer_Support_Chatbot.Hubs
{
    public class ChatHub : Hub
    {
        private readonly AppDbContext _context;
        public ChatHub(AppDbContext context)
        {
            _context = context;
        }

        public override Task OnConnectedAsync()
        {
            // var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return base.OnConnectedAsync();
        }

        public async Task JoinChat(Guid ticketId)
        {

        }
        public async Task SendMessage(Guid ticketId,Guid senderId,string content)
        {

        }

        public async Task EndChat(Guid ticketId)
        {

        }

        public async Task NotifyAgentTicketAssigned(Guid agentUserId, Guid ticketId, string title)
        {
            await Clients.User(agentUserId.ToString())
                .SendAsync("ReceiveTicketAssignedNotification", new {
                    TicketId = ticketId,
                    Title = title
                });
        }

        public async Task LeaveChat(Guid ticktId)
        {

        }

    }
}
