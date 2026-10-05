using HealthPolicyManagementSystem.Data;
using HealthPolicyManagementSystem.Models;
using HealthPolicyManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace HealthPolicyManagementSystem.Pages.Notifications
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
        public Notification Notification { get; set; } = new();

        public IActionResult OnGet(int id)
        {
            var notification = _context.Notifications
                .FirstOrDefault(n => n.NotificationID == id);

            if (notification == null)
                return NotFound();

            if (User.IsInRole("SubAdmin"))
            {
                int branchId = int.Parse(
                    User.FindFirst("BranchID")!.Value);

                var receiver = _context.Users
                    .FirstOrDefault(u => u.UserID == notification.UserID);

                if (receiver == null || receiver.BranchID != branchId)
                    return Forbid();
            }

            Notification = notification;

            return Page();
        }

        public IActionResult OnPost()
        {
            var notification = _context.Notifications
                .FirstOrDefault(n => n.NotificationID == Notification.NotificationID);

            if (notification == null)
                return RedirectToPage("Index");

            if (User.IsInRole("SubAdmin"))
            {
                int branchId = int.Parse(
                    User.FindFirst("BranchID")!.Value);

                var receiver = _context.Users
                    .FirstOrDefault(u => u.UserID == notification.UserID);

                if (receiver == null || receiver.BranchID != branchId)
                    return Forbid();
            }

            int currentUser = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var user = _context.Users
                .FirstOrDefault(u => u.UserID == notification.UserID);

            string receiverName = user?.FullName ?? "Unknown";

            _context.Notifications.Remove(notification);
            _context.SaveChanges();

            _activityLog.Log(
                currentUser,
                $"Deleted notification sent to {receiverName}");

            return RedirectToPage("Index");
        }
    }
}