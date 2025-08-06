using Customer_Support_Chatbot.Contexts;
using Customer_Support_Chatbot.Interfaces.Repositories;
using Customer_Support_Chatbot.Models;
using Microsoft.EntityFrameworkCore;

namespace Customer_Support_Chatbot.Repositories
{
    public class TicketRepository : Repository<Ticket>, ITicketRepository
    {
        private new readonly AppDbContext _context;
        public TicketRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task EndTicketAsync(Guid ticketId)
        {
            if (ticketId == Guid.Empty)
            {
                throw new ArgumentException("Ticket ID cannot be empty.", nameof(ticketId));
            }
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket != null)
            {
                ticket.Status = "Closed";
                ticket.ClosedAt = DateTime.UtcNow;
                _context.Tickets.Update(ticket);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Agent?> GetAvailableAgentAsync()
        {
            return await _context.Agents
                .Where(a => a.Status == "Available")
                .OrderBy(a => a.UpdatedAt)
                .FirstOrDefaultAsync();
        }
        // public async Task<Ticket?> GetNextPriorityTicketAsync()
        // {
        //     var now = DateTime.UtcNow;

        //     // Step 1: Get open, unassigned tickets
        //     var openTickets = await _context.Tickets
        //         .Where(t => t.Status == "Open" && t.AgentId == null)
        //         .Include(t => t.User)
        //             .ThenInclude(u => u.Subscriptions!)
        //                 .ThenInclude(s => s.Plan)
        //         .ToListAsync();

        //     Console.WriteLine("=== Open Tickets (Unassigned) ===");
        //     foreach (var t in openTickets)
        //     {
        //         Console.WriteLine($"Ticket ID: {t.Id}, CreatedAt: {t.CreatedAt}, User ID: {t.UserId}");
        //     }

        //     // Step 2: Compute priority for each ticket
        //     var ticketsWithPriority = openTickets
        //         .Select(t => new
        //         {
        //             Ticket = t,
        //             Priority = t.User?.Subscriptions?
        //                 .Where(s =>
        //                     s.Status == "Active" &&
        //                     s.StartDate <= now &&
        //                     s.EndDate >= now &&
        //                     s.Plan != null
        //                 )
        //                 .Select(s => s.Plan!.Priority)
        //                 .DefaultIfEmpty(0)
        //                 .Max() ?? 0
        //         })
        //         .ToList();

        //     Console.WriteLine("=== Tickets With Computed Priority ===");
        //     foreach (var tp in ticketsWithPriority)
        //     {
        //         Console.WriteLine($"Ticket ID: {tp.Ticket.Id}, Priority: {tp.Priority}, CreatedAt: {tp.Ticket.CreatedAt}");
        //     }

        //     // Step 3: Order tickets
        //     var orderedTickets = ticketsWithPriority
        //         .OrderByDescending(tp => tp.Priority)
        //         .ThenBy(tp => tp.Ticket.CreatedAt)
        //         .ToList();

        //     Console.WriteLine("=== Ordered Tickets ===");
        //     foreach (var ot in orderedTickets)
        //     {
        //         Console.WriteLine($"Ticket ID: {ot.Ticket.Id}, Priority: {ot.Priority}, CreatedAt: {ot.Ticket.CreatedAt}");
        //     }

        //     // Step 4: Get top ticket
        //     var topTicket = orderedTickets.FirstOrDefault()?.Ticket;

        //     if (topTicket != null)
        //     {
        //         Console.WriteLine($"=== Selected Ticket ===\nTicket ID: {topTicket.Id}, CreatedAt: {topTicket.CreatedAt}");
        //     }
        //     else
        //     {
        //         Console.WriteLine("=== No Ticket Found ===");
        //     }

        //     return topTicket;
        // }

        public async Task<Ticket?> GetFullTicketAsync(Guid ticketId)
        {
            return await _context.Tickets
                .Include(t => t.User)
                .Include(t => t.Agent).ThenInclude(a => a!.User)
                .Include(t => t.Messages)
                .Include(t => t.Attachments)
                .FirstOrDefaultAsync(t => t.Id == ticketId);
        }

        public async Task<IEnumerable<Ticket>> GetTicketsHistoryAsync(Guid id, string role, string? keyword, string? timeRange)
        {
            if (string.IsNullOrEmpty(role) || (role != "User" && role != "Agent"))
            {
                throw new ArgumentException("Role must be either 'User' or 'Agent'.", nameof(role));
            }
            var query = _context.Tickets
                .Include(t => t.Messages)
                .Include(t => t.Attachments)
                .AsQueryable();

            if (role == "User")
            {
                query = query.Where(t => t.UserId == id);
            }
            else if (role == "Agent")
            {
                query = query.Where(t => t.Agent != null && t.Agent.UserId == id);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(t => t.Name!.Contains(keyword) || t.Description.Contains(keyword));
            }
            if(timeRange=="Last 24 hours")
            {
                query = query.Where(t => t.CreatedAt >= DateTime.UtcNow.AddHours(-24));
            }
            else if(timeRange=="Last 7 days")
            {
                query = query.Where(t => t.CreatedAt >= DateTime.UtcNow.AddDays(-7));
            }
            else if(timeRange=="Last 30 days")
            {
                query = query.Where(t => t.CreatedAt >= DateTime.UtcNow.AddDays(-30));
            }
            return await query.ToListAsync();
        }
    }
}