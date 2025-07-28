using Customer_Support_Chatbot.Interfaces.Repositories;
using Quartz;

namespace Customer_Support_Chatbot.Jobs
{
    public class DeleteDeactivatedUsersJob : IJob
    {
        private readonly IUserRepository _userRepository;

        public DeleteDeactivatedUsersJob(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                Console.WriteLine($"[{DateTime.UtcNow}] Running DeleteDeactivatedUsersJob...");
                var deactivationThreshold = DateTime.UtcNow.AddDays(-15); // Production
                var users = await _userRepository.GetDeactivatedUsersAsync(deactivationThreshold);
                Console.WriteLine($"[{DateTime.UtcNow}] Found {users.Count} users to delete.");

                foreach (var user in users)
                {
                    if (user.DeactivationRequestedAt.HasValue && user.DeactivationRequestedAt.Value <= deactivationThreshold)
                    {
                        Console.WriteLine($"[{DateTime.UtcNow}] Processing user {user.Id}...");
                        await _userRepository.UpdateDeactivationRequestStatusAsync(user.Id, "Deleted");
                        await _userRepository.DeleteAsync(user.Id);
                        Console.WriteLine($"[{DateTime.UtcNow}] User {user.Id} deleted successfully.");
                    }
                }
                await _userRepository.SaveChangesAsync();
                Console.WriteLine($"[{DateTime.UtcNow}] DeleteDeactivatedUsersJob completed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.UtcNow}] Error in DeleteDeactivatedUsersJob: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[{DateTime.UtcNow}] Inner Exception: {ex.InnerException.Message}");
                }
            }
        }
    }
}