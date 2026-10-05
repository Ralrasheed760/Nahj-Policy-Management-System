using HealthPolicyManagementSystem.Data;
using HealthPolicyManagementSystem.Models;
using HealthPolicyManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace HealthPolicyManagementSystem.Pages.Departments
{
    [Authorize(Roles = "Admin,SubAdmin")]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ActivityLogService _activityLog;

        public DeleteModel(
            ApplicationDbContext context,
            ActivityLogService activityLog)
        {
            _context = context;
            _activityLog = activityLog;
        }

        [BindProperty]
        public Department Department { get; set; } = new();

        public IActionResult OnGet(int id)
        {
            var department = _context.Departments.Find(id);

            if (department == null)
                return NotFound();

            if (User.IsInRole("SubAdmin"))
            {
                int branchId = int.Parse(
                    User.FindFirst("BranchID")!.Value);

                if (department.BranchID != branchId)
                    return Forbid();
            }

            Department = department;

            return Page();
        }

        public IActionResult OnPost()
        {
            var department = _context.Departments.Find(Department.DepartmentId);

            if (department == null)
                return NotFound();

            if (User.IsInRole("SubAdmin"))
            {
                int branchId = int.Parse(
                    User.FindFirst("BranchID")!.Value);

                if (department.BranchID != branchId)
                    return Forbid();
            }

            department.IsDeleted = true;

            _context.SaveChanges();

            int userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            _activityLog.Log(
                userId,
                $"Deleted department: {department.DepartmentName}");

            return RedirectToPage("Index");
        }
    }
}
