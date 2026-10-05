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
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ActivityLogService _activityLog;

        public EditModel(
            ApplicationDbContext context,
            ActivityLogService activityLog)
        {
            _context = context;
            _activityLog = activityLog;
        }

        [BindProperty]
        public Department Department { get; set; } = new();

        public SelectList Branches { get; set; } = default!;

        public IActionResult OnGet(int id)
        {
            var department = _context.Departments.Find(id);

            if (department == null)
                return NotFound();

            if (User.IsInRole("SubAdmin"))
            {
                int branchId = int.Parse(User.FindFirst("BranchID")!.Value);

                if (department.BranchID != branchId)
                    return Forbid();
            }

            Department = department;

            if (User.IsInRole("Admin"))
            {
                Branches = new SelectList(
                    _context.Branches.Where(b => b.IsActive).ToList(),
                    "BranchID",
                    "BranchName",
                    Department.BranchID);
            }

            return Page();
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
                        "BranchName",
                        Department.BranchID);
                }

                return Page();
            }

            _context.Departments.Update(Department);
            _context.SaveChanges();

            int userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            _activityLog.Log(
                userId,
                $"Edited department: {Department.DepartmentName}");

            return RedirectToPage("Index");
        }
    }
}
