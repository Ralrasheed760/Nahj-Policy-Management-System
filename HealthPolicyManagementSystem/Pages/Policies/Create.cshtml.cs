using HealthPolicyManagementSystem.Data;
using HealthPolicyManagementSystem.Models;
using HealthPolicyManagementSystem.Services;
using HealthPolicyManagementSystem.ViewModels;
using HealthPolicyManagementSystem.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace HealthPolicyManagementSystem.Pages.Policies
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
        public PolicyViewModel PolicyVM { get; set; } = new();

        public void OnGet()
        {
            LoadLists();
        }

        public IActionResult OnPost()
        {
            LoadLists();

            if (User.IsInRole("SubAdmin"))
            {
                int departmentId = int.Parse(
                    User.FindFirst("DepartmentID")!.Value);

                if (PolicyVM.DepartmentID != departmentId)
                    return Forbid();
            }

            if (!ModelState.IsValid)
                return Page();

            if (PolicyVM.PolicyFile == null)
            {
                ModelState.AddModelError(
                    "",
                    "Please select a Word (.docx) file.");

                return Page();
            }

            var extension = Path.GetExtension(
                PolicyVM.PolicyFile.FileName)
                .ToLowerInvariant();

            if (extension != ".docx")
            {
                ModelState.AddModelError(
                    "",
                    "Only Word (.docx) files are allowed.");

                return Page();
            }

            var tempFilePath = Path.Combine(
                Path.GetTempPath(),
                Guid.NewGuid() + ".docx");

            try
            {
                using (var stream = new FileStream(
                    tempFilePath,
                    FileMode.Create))
                {
                    PolicyVM.PolicyFile.CopyTo(stream);
                }

                var documentElements =
                    DocumentTextExtractor.Extract(tempFilePath);

                var content = new List<object>();

                foreach (var element in documentElements)
                {
                    if (element is ParagraphElement paragraph)
                    {
                        content.Add(new
                        {
                            Type = "Paragraph",
                            Text = paragraph.Text
                        });
                    }
                    else if (element is TableElement table)
                    {
                        content.Add(new
                        {
                            Type = "Table",
                            Rows = table.Rows
                        });
                    }
                }

                var contentJson =
                    System.Text.Json.JsonSerializer.Serialize(content);

                int userId = int.Parse(
                    User.FindFirst(
                        ClaimTypes.NameIdentifier)!.Value);

                int branchId;

                if (User.IsInRole("Admin"))
                {
                    branchId = _context.Departments
                        .Where(d =>
                            d.DepartmentId ==
                            PolicyVM.DepartmentID)
                        .Select(d => d.BranchID)
                        .First();
                }
                else
                {
                    branchId = int.Parse(
                        User.FindFirst("BranchID")!.Value);
                }

                var policy = new Policy
                {
                    Title = PolicyVM.Title,
                    Description = PolicyVM.Description,
                    DepartmentID = PolicyVM.DepartmentID,
                    BranchID = branchId,
                    ContentJson = contentJson,
                    EffectiveDate = PolicyVM.EffectiveDate,
                    Version = PolicyVM.Version,
                    UploadedBy = userId,
                    UploadDate = DateTime.Now
                };

                _context.Policies.Add(policy);
                _context.SaveChanges();

                _activityLog.Log(
                    userId,
                    $"Created policy: {policy.Title}");

                return RedirectToPage("Index");
            }
            finally
            {
                if (System.IO.File.Exists(tempFilePath))
                {
                    System.IO.File.Delete(tempFilePath);
                }
            }
        }

        private void LoadLists()
        {
            var departments = _context.Departments
                .Where(d => !d.IsDeleted);

            if (User.IsInRole("SubAdmin"))
            {
                int departmentId = int.Parse(
                    User.FindFirst("DepartmentID")!.Value);

                departments = departments
                    .Where(d =>
                        d.DepartmentId == departmentId);
            }

            PolicyVM.Departments = departments
                .OrderBy(d => d.DepartmentName)
                .Select(d => new SelectListItem
                {
                    Value = d.DepartmentId.ToString(),
                    Text = d.DepartmentName
                })
                .ToList();
        }
    }
}
