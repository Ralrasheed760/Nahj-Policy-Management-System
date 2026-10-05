using HealthPolicyManagementSystem.Data;
using HealthPolicyManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HealthPolicyManagementSystem.Pages.PolicyVersions
{
    [Authorize(Roles = "Admin,SubAdmin")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<PolicyVersion> PolicyVersions { get; set; } =
            new List<PolicyVersion>();

        public Dictionary<int, string> PolicyTitles { get; set; } =
            new();

        public Dictionary<int, string> UserNames { get; set; } =
            new();

        [BindProperty(SupportsGet = true)]
        public int PolicyId { get; set; }

        public void OnGet()
        {
            PolicyVersions = _context.PolicyVersions
                .Where(v => v.PolicyID == PolicyId)
                .OrderByDescending(v => v.CreatedAt)
                .ToList();

            PolicyTitles = _context.Policies
                .ToDictionary(
                    p => p.PolicyID,
                    p => p.Title);

            UserNames = _context.Users
                .ToDictionary(
                    u => u.UserID,
                    u => u.FullName);
        }
    }
}