using HealthPolicyManagementSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace HealthPolicyManagementSystem.Pages.SubAdmin
{
    [Authorize(Roles = "SubAdmin")]
    public class DashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DashboardModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public string FullName { get; set; } = string.Empty;

        public int TotalEmployees { get; set; }

        public int TotalDepartments { get; set; }

        public int TotalPolicies { get; set; }

        public int TotalDownloads { get; set; }

        public int TotalNotifications { get; set; }

        public List<string> MostEngagedPolicyNames { get; set; } = new();

        public List<int> MostEngagedPolicyViews { get; set; } = new();

        public void OnGet()
        {
            FullName = User.FindFirst("FullName")?.Value ?? "Sub Admin";

            int branchId = int.Parse(
                User.FindFirst("BranchID")!.Value);

            TotalEmployees = _context.Users.Count(u =>
                u.BranchID == branchId &&
                u.Role == "Employee" &&
                u.IsActive);

            TotalDepartments = _context.Departments.Count(d =>
                d.BranchID == branchId &&
                !d.IsDeleted);

            TotalPolicies = _context.Policies.Count(p =>
                p.BranchID == branchId);

            TotalDownloads = _context.Downloads.Count();

            TotalNotifications = _context.Notifications.Count();

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