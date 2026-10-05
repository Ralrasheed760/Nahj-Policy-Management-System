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
        public User AppUser { get; set; } = new();

        public IActionResult OnGet(int id)
        {
            var user = _context.Users.Find(id);

            if (user == null)
                return NotFound();

            if (User.IsInRole("SubAdmin"))
            {
                int branchId = int.Parse(
                    User.FindFirst("BranchID")!.Value);

                if (user.BranchID != branchId)
                    return Forbid();

                if (user.Role == "Admin")
                    return Forbid();
            }

            AppUser = user;

            return Page();
        }

        public IActionResult OnPost()
        {
            var user = _context.Users.Find(AppUser.UserID);

            if (user == null)
                return NotFound();

            if (User.IsInRole("SubAdmin"))
            {
                int branchId = int.Parse(
                    User.FindFirst("BranchID")!.Value);

                if (user.BranchID != branchId)
                    return Forbid();

                if (user.Role == "Admin")
                    return Forbid();
            }

            user.IsActive = false;

            _context.SaveChanges();

            int currentUser = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            _activityLog.Log(
                currentUser,
                $"Deleted user: {user.FullName}");

            return RedirectToPage("Index");
        }
    }
}
