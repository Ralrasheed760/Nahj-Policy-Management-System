using HealthPolicyManagementSystem.Data;
using HealthPolicyManagementSystem.Models;
using HealthPolicyManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace HealthPolicyManagementSystem.Pages.Notifications
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
        public Notification Notification { get; set; } = new();

        public List<SelectListItem> Users { get; set; } = new();

        public void OnGet()
        {
            LoadUsers();
        }

        public IActionResult OnPost()
        {
            LoadUsers();

            if (User.IsInRole("SubAdmin"))
            {
                int branchId = int.Parse(
                    User.FindFirst("BranchID")!.Value);

                bool allowed = _context.Users.Any(u =>
                    u.UserID == Notification.UserID &&
                    u.BranchID == branchId &&
                    u.Role == "Employee");

                if (!allowed)
                    return Forbid();
            }

            if (!ModelState.IsValid)
                return Page();

            Notification.IsRead = false;
            Notification.CreatedAt = DateTime.Now;

            _context.Notifications.Add(Notification);
            _context.SaveChanges();

            int currentUser = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var receiver = _context.Users
                .FirstOrDefault(u => u.UserID == Notification.UserID);

            _activityLog.Log(
                currentUser,
                $"Sent notification to {receiver?.FullName}");

            return RedirectToPage("Index");
        }

        private void LoadUsers()
        {
            if (User.IsInRole("Admin"))
            {
                Users = _context.Users
                    .Where(u => u.IsActive)
                    .OrderBy(u => u.FullName)
                    .Select(u => new SelectListItem
                    {
                        Value = u.UserID.ToString(),
                        Text = u.FullName
                    })
                    .ToList();
            }
            else
            {
                int branchId = int.Parse(
                    User.FindFirst("BranchID")!.Value);

                Users = _context.Users
                    .Where(u =>
                        u.IsActive &&
                        u.BranchID == branchId &&
                        u.Role == "Employee")
                    .OrderBy(u => u.FullName)
                    .Select(u => new SelectListItem
                    {
                        Value = u.UserID.ToString(),
                        Text = u.FullName
                    })
                    .ToList();
            }
        }
    }
}