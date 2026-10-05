using HealthPolicyManagementSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace HealthPolicyManagementSystem.Pages.Employee
{
    [Authorize(Roles = "Employee")]
    public class DashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DashboardModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public string FullName { get; set; } = string.Empty;

        public int TotalPolicies { get; set; }

        public int UnreadNotifications { get; set; }

        public int TotalDownloads { get; set; }

        public List<string> MostEngagedPolicyNames { get; set; } = new();

        public List<int> MostEngagedPolicyViews { get; set; } = new();

        public void OnGet()
        {
            int userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            FullName = User.FindFirst("FullName")?.Value ?? "Employee";

            TotalPolicies = _context.Policies.Count();

            UnreadNotifications = _context.Notifications
                .Count(n => n.UserID == userId && !n.IsRead);

            TotalDownloads = _context.Downloads
                .Count(d => d.UserID == userId);

            const string prefix = "Viewed policy: ";

            var policyViews = _context.ActivityLogs
                .Where(a => a.Action.StartsWith(prefix))
                .AsEnumerable()
                .Select(a => a.Action.Substring(prefix.Length))
                .Where(title => !string.IsNullOrWhiteSpace(title))
                .GroupBy(title => title)
                .Select(g => new
                {
                    PolicyName = g.Key,
                    Views = g.Count()
                })
                .OrderByDescending(x => x.Views)
                .Take(5)
                .ToList();

            MostEngagedPolicyNames = policyViews
                .Select(x => x.PolicyName)
                .ToList();

            MostEngagedPolicyViews = policyViews
                .Select(x => x.Views)
                .ToList();
        }
    }
}
