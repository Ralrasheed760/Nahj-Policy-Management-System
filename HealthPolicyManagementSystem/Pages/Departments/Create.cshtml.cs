using HealthPolicyManagementSystem.Data;
using HealthPolicyManagementSystem.Models;
using HealthPolicyManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace HealthPolicyManagementSystem.Pages.Departments
{
    [Authorize(Roles = "Admin,SubAdmin")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ActivityLogService _activityLog;

        public CreateModel(ApplicationDbContext context, ActivityLogService activityLog)
        {
            _context = context;
            _activityLog = activityLog;
        }

        [BindProperty]
        public Department Department { get; set; } = new();

        public SelectList Branches { get; set; } = default!;

        public void OnGet()
        {
            if (User.IsInRole("Admin"))
            {
                Branches = new SelectList(
                    _context.Branches.Where(b => b.IsActive).ToList(),
                    "BranchID",
                    "BranchName");
            }
        }

        public IActionResult OnPost()
        {
            if (User.IsInRole("SubAdmin"))
            {
                Department.BranchID = int.Parse(
                    User.FindFirst("BranchID")!.Value);
            }

            if (!ModelState.IsValid)
            {
                if (User.IsInRole("Admin"))
                {
                    Branches = new SelectList(
                        _context.Branches.Where(b => b.IsActive).ToList(),
                        "BranchID",
                        "BranchName");
                }

                return Page();
            }

            _context.Departments.Add(Department);
            _context.SaveChanges();

            int adminId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            _activityLog.Log(
                adminId,
                $"Created department: {Department.DepartmentName}");

            return RedirectToPage("Index");
        }
    }
}
