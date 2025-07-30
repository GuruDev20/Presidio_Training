using BCrypt.Net;
using Customer_Support_Chatbot.Contexts;
using Customer_Support_Chatbot.DTOs.Admin;
using Customer_Support_Chatbot.DTOs.Ticket;
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

        public async Task<User> CreateAgentUserAsync(string username, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username cannot be null or empty.", nameof(username));
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Hashed password cannot be null or empty.", nameof(password));
            }
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = username,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
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

        public async Task<List<AgentDto>> GetAgentDetailsAsync()
        {
            return await _context.Agents
                .Join(_context.Users,
                    agent => agent.UserId,
                    user => user.Id,
                    (agent, user) => new { Agent = agent, User = user })
                .Select(au => new AgentDto
                {
                    Id = au.Agent.Id,
                    Username = au.User.Username,
                    Status = au.Agent.Status
                })
                .OrderBy(dto => dto.Username)
                .Take(10)
                .ToListAsync();
        }

        public async Task<List<DeactivationRequestDto>> GetDeactivationRequestsAsync()
        {
            return await _context.DeactivationRequests
                .Include(dr => dr.User)
                .Select(dr => new DeactivationRequestDto
                {
                    Id = dr.Id,
                    Username = dr.User.Username,
                    Reason = dr.Reason,
                    Status = dr.Status,
                    RequestedAt = dr.RequestedAt
                })
                .OrderByDescending(dr => dr.RequestedAt)
                .Take(10)
                .ToListAsync();
        }

        public async Task<object> GetOverviewAsync()
        {
            var totalUsers = await _context.Users.CountAsync(u => u.Role == "User");
            var activeUsers = await _context.Users.CountAsync(u => u.Role == "User" && !u.IsDeactivated);
            var totalAgents = await _context.Agents.CountAsync();
            var activeAgents = await _context.Agents.CountAsync(t => t.Status == "Available");
            var totalTickets = await _context.Tickets.CountAsync();
            return new
            {
                TotalUsers = totalUsers,
                ActiveUsers = activeUsers,
                TotalAgents = totalAgents,
                ActiveAgents = activeAgents,
                TotalTickets = totalTickets
            };
        }

        public async Task<(List<TicketDto> Tickets, int TotalCount)> GetTicketDetailsAsync(int page, int pageSize)
        {
            var query = _context.Tickets
                .Include(t => t.User)
                .Include(t => t.Agent)
                .ThenInclude(a => a!.User)
                .OrderByDescending(t => t.CreatedAt);

            var totalCount = await query.CountAsync();

            var tickets = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TicketDto
                {
                    Id = t.Id,
                    Title = t.Name,
                    Status = t.Status,
                    CreatedAt = t.CreatedAt,
                    AgentUsername = t.Agent != null ? t.Agent.User!.Username : null
                })
                .ToListAsync();

            return (tickets, totalCount);
        }

        public async Task<object> GetTicketGrowthAsync(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
                throw new ArgumentException("Filter cannot be null or empty.", nameof(filter));

            var now = DateTime.UtcNow;
            var normalizedFilter = filter.ToLowerInvariant();

            var tickets = await _context.Tickets
                .Where(t => t.CreatedAt >= now.AddYears(-1)) // fetch max needed range
                .ToListAsync();

            switch (normalizedFilter)
            {
                case "last24hours":
                    var hourlyData = Enumerable.Range(0, 24)
                        .Select(h => {
                            var hour = now.AddHours(-h);
                            var hourStart = new DateTime(hour.Year, hour.Month, hour.Day, hour.Hour, 0, 0);
                            return new TicketGrowthDto
                            {
                                Date = hourStart,
                                Count = tickets.Count(t =>
                                    t.CreatedAt >= hourStart &&
                                    t.CreatedAt < hourStart.AddHours(1))
                            };
                        })
                        .Reverse()
                        .ToList();
                    return hourlyData;

                case "last7days":
                    var dailyData = Enumerable.Range(0, 7)
                        .Select(d => {
                            var day = now.Date.AddDays(-d);
                            return new TicketGrowthDto
                            {
                                Date = day,
                                Count = tickets.Count(t => t.CreatedAt.Date == day)
                            };
                        })
                        .Reverse()
                        .ToList();
                    return dailyData;

                case "last30days":
                    var weeklyData = Enumerable.Range(0, 5)
                        .Select(w => {
                            var start = now.Date.AddDays(-(w + 1) * 7);
                            var end = start.AddDays(7);
                            return new TicketGrowthDto
                            {
                                Date = start,
                                Count = tickets.Count(t => t.CreatedAt >= start && t.CreatedAt < end)
                            };
                        })
                        .Reverse()
                        .ToList();
                    return weeklyData;

                case "last1year":
                    var monthlyData = Enumerable.Range(0, 12)
                        .Select(m => {
                            var month = new DateTime(now.Year, now.Month, 1).AddMonths(-m);
                            return new TicketGrowthDto
                            {
                                Date = month,
                                Count = tickets.Count(t => t.CreatedAt.Year == month.Year && t.CreatedAt.Month == month.Month)
                            };
                        })
                        .Reverse()
                        .ToList();
                    return monthlyData;

                default:
                    throw new ArgumentException("Invalid filter value.", nameof(filter));
            }
        }

        public async Task<Agent?> UpdateAgentAsync(Guid agentId, string? username)
        {
            if (agentId == Guid.Empty)
            {
                throw new ArgumentException("Agent ID cannot be empty.", nameof(agentId));
            }
            var agent = await _context.Agents.Include(a => a.User).FirstOrDefaultAsync(a => a.Id == agentId);
            if (agent == null)
            {
                Console.WriteLine($"Agent with ID {agentId} not found.");
                return null;
            }
            if (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("Username cannot be null or empty.");
                return null;
            }
            agent.User!.Username = username;
            agent.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return agent;
        }
    }
}