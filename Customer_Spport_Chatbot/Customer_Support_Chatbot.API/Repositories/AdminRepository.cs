using Customer_Support_Chatbot.Contexts;
using Customer_Support_Chatbot.Interfaces.Repositories;
using Customer_Support_Chatbot.Models;
using Microsoft.EntityFrameworkCore;

namespace Customer_Support_Chatbot.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AppDbContext _context;
        public AdminRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Agent> CreateAgentAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            var agent = new Agent
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Status = "Available",
                UpdatedAt = DateTime.UtcNow
            };
            _context.Agents.Add(agent);
            await _context.SaveChangesAsync();
            return agent;
        }

        public async Task<User> CreateAgentUserAsync(string username, string email, string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username cannot be null or empty.", nameof(username));
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));
            }
            if (string.IsNullOrWhiteSpace(hashedPassword))
            {
                throw new ArgumentException("Hashed password cannot be null or empty.", nameof(hashedPassword));
            }
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = username,
                Email = email,
                PasswordHash = hashedPassword,
                Role = "Agent",
                CreatedAt = DateTime.UtcNow,
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteAgentAsync(Guid agentId)
        {
            if (agentId == Guid.Empty)
            {
                throw new ArgumentException("Agent ID cannot be empty.", nameof(agentId));
            }
            var agent = await _context.Agents.Include(a => a.User).FirstOrDefaultAsync(a => a.UserId == agentId);
            if (agent == null)
            {
                Console.WriteLine($"Agent with ID {agentId} not found.");
                return false;
            }
            if (agent.User == null)
            {
                Console.WriteLine(" Agent found, but User is null.");
            }

            _context.Users.Remove(agent.User!);
            _context.Agents.Remove(agent);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<object> GetOverviewAsync()
        {
            var totalUsers = await _context.Users.CountAsync(u => u.Role == "User");
            var activeUsers = await _context.Users.CountAsync(u => u.Role == "User" && !u.IsDeactivated);
            var activeAgents = await _context.Agents.CountAsync(t => t.Status == "Available");
            var totalTickets = await _context.Tickets.CountAsync();
            return new
            {
                TotalUsers = totalUsers,
                ActiveUsers = activeUsers,
                ActiveAgents = activeAgents,
                TotalTickets = totalTickets
            };
        }
    }
}