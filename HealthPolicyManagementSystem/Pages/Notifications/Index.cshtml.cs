using HealthPolicyManagementSystem.Data;
using HealthPolicyManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace HealthPolicyManagementSystem.Pages.Notifications
{
    [Authorize(Roles = "Admin,SubAdmin,Employee")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Notification> Notifications { get; set; } = new List<Notification>();

        public Dictionary<int, string> Users { get; set; } = new();

        public bool IsAdmin { get; set; }

        public bool IsSubAdmin { get; set; }

        public void OnGet()
        {
            IsAdmin = User.IsInRole("Admin");
            IsSubAdmin = User.IsInRole("SubAdmin");

            Users = _context.Users
                .ToDictionary(u => u.UserID, u => u.FullName);

            if (IsAdmin)
            {
                Notifications = _context.Notifications
                    .OrderByDescending(n => n.CreatedAt)
                    .ToList();

                return;
            }

            if (IsSubAdmin)
            {
                int branchId = int.Parse(
                    User.FindFirst("BranchID")!.Value);

                var employeeIds = _context.Users
                    .Where(u => u.BranchID == branchId)
                    .Select(u => u.UserID)
                    .ToList();

                Notifications = _context.Notifications
                    .Where(n => employeeIds.Contains(n.UserID))
                    .OrderByDescending(n => n.CreatedAt)
                    .ToList();

                return;
            }

            int userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var unread = _context.Notifications
                .Where(n => n.UserID == userId && !n.IsRead)
                .ToList();

            foreach (var item in unread)
            {
                item.IsRead = true;
            }

            _context.SaveChanges();

            Notifications = _context.Notifications
                .Where(n => n.UserID == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();
        }
    }
}
