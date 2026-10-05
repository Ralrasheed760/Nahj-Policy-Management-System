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
            if (!ModelState.IsValid)
                return Page();

            var branch = _context.Branches.Find(Branch.BranchID);

            if (branch == null)
                return NotFound();

            branch.BranchName = Branch.BranchName;
            branch.BranchType = Branch.BranchType;
            branch.IsActive = Branch.IsActive;

            _context.SaveChanges();

            int adminId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            _activityLog.Log(
                adminId,
                $"Edited branch: {branch.BranchName}");

            return RedirectToPage("Index");
        }
    }
}