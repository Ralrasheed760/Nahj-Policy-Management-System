using HealthPolicyManagementSystem.Data;
using HealthPolicyManagementSystem.Models;
using HealthPolicyManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace HealthPolicyManagementSystem.Pages.Users
{
    [Authorize(Roles = "Admin,SubAdmin")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ActivityLogService _activityLog;

        public CreateModel(
            ApplicationDbContext context,
            ActivityLogService activityLog)
        {
            _context = context;
            _activityLog = activityLog;
        }

        [BindProperty]
        public User AppUser { get; set; } = new();

        public List<Branch> Branches { get; set; } = new();

        public List<Department> Departments { get; set; } = new();

        public void OnGet()
        {
            LoadBranches();
            LoadDepartments();
        }

        public IActionResult OnPost()
        {
            LoadBranches();
            LoadDepartments();

            if (User.IsInRole("SubAdmin"))
            {
                int branchId = int.Parse(
                    User.FindFirst("BranchID")!.Value);

                int departmentId = int.Parse(
                    User.FindFirst("DepartmentID")!.Value);

                AppUser.Role = "Employee";
                AppUser.BranchID = branchId;
                AppUser.DepartmentID = departmentId;
            }
            else
            {
                if (AppUser.Role == "Admin")
                {
                    AppUser.BranchID = null;
                    AppUser.DepartmentID = null;
                }
                else
                {
                    if (AppUser.BranchID == null)
                    {
                        ModelState.AddModelError(
                            "AppUser.BranchID",
                            "Please select a branch.");
                    }

                    if (AppUser.DepartmentID == null)
                    {
                        ModelState.AddModelError(
                            "AppUser.DepartmentID",
                            "Please select a department.");
                    }

                    if (AppUser.BranchID != null &&
                        AppUser.DepartmentID != null)
                    {
                        var department = _context.Departments
                            .FirstOrDefault(d =>
                                d.DepartmentId == AppUser.DepartmentID &&
                                !d.IsDeleted);

                        if (department == null ||
                            department.BranchID != AppUser.BranchID)
                        {
                            ModelState.AddModelError(
                                "AppUser.DepartmentID",
                                "The selected department does not belong to the selected branch.");
                        }
                    }
                }
            }

            if (!ModelState.IsValid)
                return Page();

            _context.Users.Add(AppUser);
            _context.SaveChanges();

            int currentUser = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            _activityLog.Log(
                currentUser,
                $"Created user: {AppUser.FullName}");

            return RedirectToPage("Index");
        }

        private void LoadBranches()
        {
            if (User.IsInRole("Admin"))
            {
                Branches = _context.Branches
                    .Where(b => b.IsActive)
                    .OrderBy(b => b.BranchName)
                    .ToList();
            }
            else
            {
                int branchId = int.Parse(
                    User.FindFirst("BranchID")!.Value);

                Branches = _context.Branches
                    .Where(b =>
                        b.IsActive &&
                        b.BranchID == branchId)
                    .ToList();
            }
        }

        private void LoadDepartments()
        {
            if (User.IsInRole("Admin"))
            {
                Departments = _context.Departments
                    .Where(d => !d.IsDeleted)
                    .OrderBy(d => d.DepartmentName)
                    .ToList();
            }
            else
            {
                int departmentId = int.Parse(
                    User.FindFirst("DepartmentID")!.Value);

                Departments = _context.Departments
                    .Where(d =>
                        d.DepartmentId == departmentId &&
                        !d.IsDeleted)
                    .ToList();
            }
        }
    }
}
