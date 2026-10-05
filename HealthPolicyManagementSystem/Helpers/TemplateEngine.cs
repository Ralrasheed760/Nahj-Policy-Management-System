using System.Text.Json;
using HealthPolicyManagementSystem.Data;
using HealthPolicyManagementSystem.ViewModels;

namespace HealthPolicyManagementSystem.Helpers
{
    public class TemplateEngine
    {
        private readonly ApplicationDbContext _context;

        public TemplateEngine(ApplicationDbContext context)
        {
            _context = context;
        }

        public TemplateDesignerViewModel? GetActiveTemplate()
        {
            var template = _context.Templates
                .OrderByDescending(t => t.TemplateID)
                .FirstOrDefault();

            if (template == null)
                return null;

            if (string.IsNullOrWhiteSpace(template.TemplateJson))
                return null;

            return JsonSerializer.Deserialize<TemplateDesignerViewModel>(
                template.TemplateJson);
        }

        public PolicyViewModel BuildPolicy(PolicyViewModel policy)
        {
            var template = GetActiveTemplate();

            if (template == null)
                throw new Exception("No active template.");

            return new PolicyViewModel
            {
                PolicyID = policy.PolicyID,
                Title = policy.Title,
                Description = policy.Description,
                DepartmentID = policy.DepartmentID,
                BranchID = policy.BranchID,
                Version = policy.Version,
                EffectiveDate = policy.EffectiveDate,
                ExistingFilePath = policy.ExistingFilePath
            };
        }
    }
}