using HealthPolicyManagementSystem.Data;
using HealthPolicyManagementSystem.Models;
using HealthPolicyManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace HealthPolicyManagementSystem.Pages.Policies
{
    [Authorize(Roles = "Admin,SubAdmin")]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ActivityLogService _activityLog;

        public DeleteModel(
            ApplicationDbContext context,
            IWebHostEnvironment environment,
            ActivityLogService activityLog)
        {
            _context = context;
            _environment = environment;
            _activityLog = activityLog;
        }

        [BindProperty]
        public Policy Policy { get; set; } = new();

        public IActionResult OnGet(int id)
        {
            var policy = _context.Policies
                .FirstOrDefault(p => p.PolicyID == id);

            if (policy == null)
                return NotFound();

            if (User.IsInRole("SubAdmin"))
            {
                int departmentId = int.Parse(
                    User.FindFirst("DepartmentID")!.Value);

                if (policy.DepartmentID != departmentId)
                    return Forbid();
            }

            Policy = policy;

            return Page();
        }

        public IActionResult OnPost()
        {
            var policy = _context.Policies
                .FirstOrDefault(p => p.PolicyID == Policy.PolicyID);

            if (policy == null)
                return RedirectToPage("Index");

            if (User.IsInRole("SubAdmin"))
            {
                int departmentId = int.Parse(
                    User.FindFirst("DepartmentID")!.Value);

                if (policy.DepartmentID != departmentId)
                    return Forbid();
            }

            int userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            string policyTitle = policy.Title;

            if (!string.IsNullOrEmpty(policy.FilePath))
            {
                var file = Path.Combine(
                    _environment.WebRootPath,
                    policy.FilePath.TrimStart('/')
                        .Replace(
                            "/",
                            Path.DirectorySeparatorChar.ToString()));

                if (System.IO.File.Exists(file))
                    System.IO.File.Delete(file);
            }

            _context.Policies.Remove(policy);

            _context.SaveChanges();

            _activityLog.Log(
                userId,
                $"Deleted policy: {policyTitle}");

            return RedirectToPage("Index");
        }
    }
}