using HealthPolicyManagementSystem.Data;
using HealthPolicyManagementSystem.Models;
using HealthPolicyManagementSystem.Services;
using HealthPolicyManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace HealthPolicyManagementSystem.Pages.Policies
{
    [Authorize(Roles = "Admin,SubAdmin")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ActivityLogService _activityLog;

        public EditModel(
            ApplicationDbContext context,
            IWebHostEnvironment environment,
            ActivityLogService activityLog)
        {
            _context = context;
            _environment = environment;
            _activityLog = activityLog;
        }

        [BindProperty]
        public PolicyViewModel PolicyVM { get; set; } = new();

        public bool IsAdmin => User.IsInRole("Admin");

        public IActionResult OnGet(int id)
        {
            var policy = _context.Policies
                .FirstOrDefault(p => p.PolicyID == id);

            if (policy == null)
                return NotFound();

            if (User.IsInRole("SubAdmin"))
            {
                int departmentId = int.Parse(
                    User.FindFirst("DepartmentID")!.Value);

                if (policy.DepartmentID != departmentId)
                    return Forbid();
            }

            PolicyVM.PolicyID = policy.PolicyID;
            PolicyVM.Title = policy.Title;
            PolicyVM.Description = policy.Description;
            PolicyVM.DepartmentID = policy.DepartmentID;
            PolicyVM.BranchID = policy.BranchID;
            PolicyVM.EffectiveDate = policy.EffectiveDate;
            PolicyVM.Version = policy.Version;
            PolicyVM.ExistingFilePath = policy.FilePath;

            LoadLists();

            return Page();
        }

        public IActionResult OnPost()
        {
            LoadLists();

            var policy = _context.Policies
                .FirstOrDefault(p => p.PolicyID == PolicyVM.PolicyID);

            if (policy == null)
                return NotFound();

            if (User.IsInRole("SubAdmin"))
            {
                int departmentId = int.Parse(
                    User.FindFirst("DepartmentID")!.Value);

                if (policy.DepartmentID != departmentId)
                    return Forbid();

                PolicyVM.DepartmentID = departmentId;
                PolicyVM.BranchID = policy.BranchID;
            }

            if (!ModelState.IsValid)
                return Page();

            policy.Title = PolicyVM.Title;
            policy.Description = PolicyVM.Description;
            policy.DepartmentID = PolicyVM.DepartmentID;
            policy.BranchID = PolicyVM.BranchID;
            policy.EffectiveDate = PolicyVM.EffectiveDate;
            policy.Version = PolicyVM.Version;

            if (PolicyVM.PolicyFile != null)
            {
                var extension = Path.GetExtension(
                    PolicyVM.PolicyFile.FileName)
                    .ToLowerInvariant();

                if (extension != ".pdf")
                {
                    ModelState.AddModelError(
                        "PolicyVM.PolicyFile",
                        "Only PDF files are allowed.");

                    return Page();
                }

                if (!string.IsNullOrEmpty(policy.FilePath))
                {
                    var oldFile = Path.Combine(
                        _environment.WebRootPath,
                        policy.FilePath.TrimStart('/')
                            .Replace(
                                "/",
                                Path.DirectorySeparatorChar.ToString()));

                    if (System.IO.File.Exists(oldFile))
                        System.IO.File.Delete(oldFile);
                }

                var fileName = Guid.NewGuid().ToString() + extension;

                var folder = Path.Combine(
                    _environment.WebRootPath,
                    "Uploads",
                    "Policies");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                var filePath = Path.Combine(
                    folder,
                    fileName);

                using (var stream = new FileStream(
                    filePath,
                    FileMode.Create))
                {
                    PolicyVM.PolicyFile.CopyTo(stream);
                }

                policy.FilePath =
                    "/Uploads/Policies/" + fileName;
            }

            int userId = int.Parse(
                User.FindFirst(
                    ClaimTypes.NameIdentifier)!.Value);

            _context.PolicyVersions.Add(
                new PolicyVersion
                {
                    PolicyID = policy.PolicyID,
                    VersionNumber = policy.Version,
                    FilePath = policy.FilePath,
                    CreatedBy = userId,
                    CreatedAt = DateTime.Now
                });

            _context.SaveChanges();

            _activityLog.Log(
                userId,
                $"Updated policy: {policy.Title}");

            return RedirectToPage("Index");
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

            PolicyVM.Branches = _context.Branches
                .Where(b => b.IsActive)
                .OrderBy(b => b.BranchName)
                .Select(b => new SelectListItem
                {
                    Value = b.BranchID.ToString(),
                    Text = b.BranchName
                })
                .ToList();
        }
    }
}