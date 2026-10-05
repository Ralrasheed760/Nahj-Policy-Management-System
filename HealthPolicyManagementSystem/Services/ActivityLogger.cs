using HealthPolicyManagementSystem.Data;
using HealthPolicyManagementSystem.Models;

namespace HealthPolicyManagementSystem.Services
{
    public class ActivityLogService
    {
        private readonly ApplicationDbContext _context;

        public ActivityLogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Log(int userId, string action)
        {
            var log = new ActivityLog
            {
                UserID = userId,
                Action = action,
                ActionDate = DateTime.Now
            };

            _context.ActivityLogs.Add(log);
            _context.SaveChanges();
        }
    }
}