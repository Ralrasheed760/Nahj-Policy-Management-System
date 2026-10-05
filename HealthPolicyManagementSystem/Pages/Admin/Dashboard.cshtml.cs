using HealthPolicyManagementSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HealthPolicyManagementSystem.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class DashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DashboardModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public string FullName { get; set; } = string.Empty;

        public int TotalUsers { get; set; }

        public int TotalBranches { get; set; }

        public int TotalDepartments { get; set; }

        public int TotalPolicies { get; set; }

        public int TotalDownloads { get; set; }

        public int TotalNotifications { get; set; }

        public List<string> MostReadPolicyNames { get; set; } = new();

        public List<int> MostReadPolicyCounts { get; set; } = new();

        public List<ActivityLogViewModel> LatestActivities { get; set; } = new();

        public void OnGet()
        {
            FullName = User.FindFirst("FullName")?.Value ?? "Administrator";

            TotalUsers = _context.Users
                .Count(u => u.IsActive);

            TotalBranches = _context.Branches
                .Count();

            TotalDepartments = _context.Departments
                .Count(d => !d.IsDeleted);

            TotalPolicies = _context.Policies
                .Count();

            TotalDownloads = _context.Downloads
                .Count();

            TotalNotifications = _context.Notifications
                .Count();

            var viewedPolicies = _context.ActivityLogs
                .Where(a => a.Action.StartsWith("Viewed policy:"))
                .Select(a => a.Action)
                .ToList();

            var mostRead = viewedPolicies
                .Select(a =>
                    a.Substring("Viewed policy:".Length).Trim())
                .GroupBy(title => title)
                .Select(g => new
                {
                    Title = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.Title)
                .Take(5)
                .ToList();

            MostReadPolicyNames = mostRead
                .Select(x => x.Title)
                .ToList();

            MostReadPolicyCounts = mostRead
                .Select(x => x.Count)
                .ToList();

            LatestActivities = _context.ActivityLogs
                .OrderByDescending(a => a.ActionDate)
                .Take(5)
                .Join(
                    _context.Users,
                    activity => activity.UserID,
                    user => user.UserID,
                    (activity, user) => new ActivityLogViewModel
                    {
                        UserName = user.FullName,
                        Action = activity.Action,
                        Date = activity.ActionDate
                    })
                .ToList();
        }
    }

    public class ActivityLogViewModel
    {
        public string UserName { get; set; } = string.Empty;

        public string Action { get; set; } = string.Empty;

        public DateTime Date { get; set; }
    }
}
