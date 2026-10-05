using HealthPolicyManagementSystem.Data;
using HealthPolicyManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HealthPolicyManagementSystem.Pages.ActivityLogs
{
    [Authorize(Roles = "Admin,SubAdmin")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();

        public Dictionary<int, string> Users { get; set; } = new();

        public void OnGet()
        {
            if (User.IsInRole("Admin"))
            {
                ActivityLogs = _context.ActivityLogs
                    .OrderByDescending(a => a.ActionDate)
                    .ToList();
            }
            else if (User.IsInRole("SubAdmin"))
            {
                var currentUserId = int.Parse(
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                var currentUser = _context.Users
                    .FirstOrDefault(u => u.UserID == currentUserId);

                if (currentUser != null && currentUser.BranchID.HasValue)
                {
                    var branchUserIds = _context.Users
                        .Where(u => u.BranchID == currentUser.BranchID)
                        .Select(u => u.UserID)
                        .ToList();

                    ActivityLogs = _context.ActivityLogs
                        .Where(a => branchUserIds.Contains(a.UserID))
                        .OrderByDescending(a => a.ActionDate)
                        .ToList();
                }
            }

            Users = _context.Users
                .ToDictionary(
                    u => u.UserID,
                    u => u.FullName);
        }
    }
}
