using HealthPolicyManagementSystem.Data;
using HealthPolicyManagementSystem.Helpers;
using HealthPolicyManagementSystem.Services;
using HealthPolicyManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Text.Json;

namespace HealthPolicyManagementSystem.Pages.Policies
{
    [Authorize(Roles = "Admin,SubAdmin,Employee")]
    public class PreviewPdfModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ActivityLogService _activityLog;

        public PreviewPdfModel(
            ApplicationDbContext context,
            ActivityLogService activityLog)
        {
            _context = context;
            _activityLog = activityLog;
        }

        public IActionResult OnGet(int id)
        {
            var policy = _context.Policies
                .FirstOrDefault(p => p.PolicyID == id);

            if (policy == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(policy.ContentJson))
                return NotFound("Policy content not found.");

            var model = new PolicyViewModel
            {
                PolicyID = policy.PolicyID,
                Title = policy.Title,
                Description = policy.Description,
                DepartmentID = policy.DepartmentID,
                BranchID = policy.BranchID,
                EffectiveDate = policy.EffectiveDate,
                Version = policy.Version
            };

            var templateEngine = new TemplateEngine(_context);

            var policyModel = templateEngine.BuildPolicy(model);

            var template = templateEngine.GetActiveTemplate();

            if (template == null)
                return NotFound("No active template found.");

            var documentElements =
                DeserializeDocumentElements(policy.ContentJson);

            int userId = int.Parse(
                User.FindFirst(
                    ClaimTypes.NameIdentifier)!.Value);

            _activityLog.Log(
                userId,
                $"Viewed policy: {policy.Title}");

            var pdf = PolicyPdfGenerator.Generate(
                policyModel,
                template,
                documentElements);

            return File(
                pdf,
                "application/pdf");
        }

        private static List<DocumentElement> DeserializeDocumentElements(
            string json)
        {
            var elements = new List<DocumentElement>();

            using var document =
                JsonDocument.Parse(json);

            foreach (var item in document.RootElement.EnumerateArray())
            {
                if (item.TryGetProperty("Text", out var textProperty))
                {
                    elements.Add(new ParagraphElement
                    {
                        Text = textProperty.GetString() ?? ""
                    });

                    continue;
                }

                if (item.TryGetProperty("Rows", out var rowsProperty))
                {
                    var table = new TableElement();

                    foreach (var row in rowsProperty.EnumerateArray())
                    {
                        var cells = new List<string>();

                        foreach (var cell in row.EnumerateArray())
                        {
                            cells.Add(
                                cell.GetString() ?? "");
                        }

                        table.Rows.Add(cells);
                    }

                    elements.Add(table);
                }
            }

            return elements;
        }
    }
}