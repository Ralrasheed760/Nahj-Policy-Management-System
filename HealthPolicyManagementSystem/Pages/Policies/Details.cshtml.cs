using HealthPolicyManagementSystem.Data;
using HealthPolicyManagementSystem.Helpers;
using HealthPolicyManagementSystem.Models;
using HealthPolicyManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HealthPolicyManagementSystem.Pages.Policies
{
    [Authorize(Roles = "Admin,SubAdmin,Employee")]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Policy Policy { get; set; } = new();

        public string DepartmentName { get; set; } = "";

        public string BranchName { get; set; } = "";

        public string PolicyJson { get; set; } = "";

        public IActionResult OnGet(int id)
        {
            var policy = _context.Policies
                .FirstOrDefault(p => p.PolicyID == id);

            if (policy == null)
                return NotFound();

            Policy = policy;

            var department = _context.Departments
                .FirstOrDefault(d =>
                    d.DepartmentId == Policy.DepartmentID);

            if (department != null)
                DepartmentName = department.DepartmentName;

            var branch = _context.Branches
                .FirstOrDefault(b =>
                    b.BranchID == Policy.BranchID);

            if (branch != null)
                BranchName = branch.BranchName;

            var model = new PolicyViewModel
            {
                PolicyID = Policy.PolicyID,
                Title = Policy.Title,
                Description = Policy.Description,
                DepartmentID = Policy.DepartmentID,
                BranchID = Policy.BranchID,
                EffectiveDate = Policy.EffectiveDate,
                Version = Policy.Version,
                ExistingFilePath = Policy.FilePath
            };

            PolicyJson = PolicyJsonHelper.ConvertToJson(model);

            return Page();
        }
    }
}