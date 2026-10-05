using HealthPolicyManagementSystem.Data;
using HealthPolicyManagementSystem.Services;
using HealthPolicyManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace HealthPolicyManagementSystem.Pages.Users
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
        public EditUserViewModel AppUser { get; set; } = new();

        public IActionResult OnGet(int id)
        {
            var user = _context.Users.Find(id);

            if (user == null)
                return NotFound();

            if (User.IsInRole("SubAdmin"))
            {
                int branchId = int.Parse(
                    User.FindFirst("BranchID")!.Value);

                int departmentId = int.Parse(
                    User.FindFirst("DepartmentID")!.Value);

                if (user.BranchID != branchId ||
                    user.DepartmentID != departmentId)
                {
                    return Forbid();
                }

                if (user.Role != "Employee")
                    return Forbid();
            }

            AppUser = new EditUserViewModel
            {
                UserID = user.UserID,
                FullName = user.FullName,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                BranchID = user.BranchID,
                DepartmentID = user.DepartmentID,
                IsActive = user.IsActive
            };

            LoadBranches();
            LoadDepartments();

            return Page();
        }

        public IActionResult OnPost()
        {
            LoadBranches();
            LoadDepartments();

            var user = _context.Users.Find(AppUser.UserID);

            if (user == null)
                return NotFound();

            if (User.IsInRole("SubAdmin"))
            {
                int branchId = int.Parse(
                    User.FindFirst("BranchID")!.Value);

                int departmentId = int.Parse(
                    User.FindFirst("DepartmentID")!.Value);

                if (user.BranchID != branchId ||
                    user.DepartmentID != departmentId)
                {
                    return Forbid();
                }

                if (user.Role != "Employee")
                    return Forbid();

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

            user.FullName = AppUser.FullName;
            user.Username = AppUser.Username;
            user.Email = AppUser.Email;
            user.Role = AppUser.Role;
            user.BranchID = AppUser.BranchID;
            user.DepartmentID = AppUser.DepartmentID;
            user.IsActive = AppUser.IsActive;

            if (!string.IsNullOrWhiteSpace(AppUser.Password))
            {
                user.Password = AppUser.Password;
            }

            _context.SaveChanges();

            int currentUser = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            _activityLog.Log(
                currentUser,
                $"Edited user: {user.FullName}");

            return RedirectToPage("Index");
        }

        private void LoadBranches()
        {
            if (User.IsInRole("Admin"))
            {
                AppUser.Branches = _context.Branches
                    .Where(b => b.IsActive)
                    .OrderBy(b => b.BranchName)
                    .Select(b => new SelectListItem
                    {
                        Value = b.BranchID.ToString(),
                        Text = b.BranchName
                    })
                    .ToList();
            }
            else
            {
                int branchId = int.Parse(
                    User.FindFirst("BranchID")!.Value);

                AppUser.Branches = _context.Branches
                    .Where(b =>
                        b.IsActive &&
                        b.BranchID == branchId)
                    .Select(b => new SelectListItem
                    {
                        Value = b.BranchID.ToString(),
                        Text = b.BranchName
                    })
                    .ToList();
            }
        }

        private void LoadDepartments()
        {
            if (User.IsInRole("Admin"))
            {
                AppUser.Departments = _context.Departments
                    .Where(d => !d.IsDeleted)
                    .OrderBy(d => d.DepartmentName)
                    .Select(d => new SelectListItem
                    {
                        Value = d.DepartmentId.ToString(),
                        Text = d.DepartmentName
                    })
                    .ToList();
            }
            else
            {
                int departmentId = int.Parse(
                    User.FindFirst("DepartmentID")!.Value);

                AppUser.Departments = _context.Departments
                    .Where(d =>
                        d.DepartmentId == departmentId &&
                        !d.IsDeleted)
                    .Select(d => new SelectListItem
                    {
                        Value = d.DepartmentId.ToString(),
                        Text = d.DepartmentName
                    })
                    .ToList();
            }
        }
    }
}