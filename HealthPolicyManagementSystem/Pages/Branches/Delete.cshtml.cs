using HealthPolicyManagementSystem.Data;
using HealthPolicyManagementSystem.Models;
using HealthPolicyManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace HealthPolicyManagementSystem.Pages.Branches
{
    [Authorize(Roles = "Admin")]
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
        public Branch Branch { get; set; } = new();

        public IActionResult OnGet(int id)
        {
            var branch = _context.Branches.Find(id);

            if (branch == null)
                return NotFound();

            Branch = branch;

            return Page();
        }

        public IActionResult OnPost()
        {
            var branch = _context.Branches.Find(Branch.BranchID);

            if (branch == null)
                return NotFound();

            _context.Branches.Remove(branch);

            _context.SaveChanges();

            int adminId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            _activityLog.Log(
                adminId,
                $"Deleted branch: {branch.BranchName}");

            return RedirectToPage("Index");
        }
    }
}