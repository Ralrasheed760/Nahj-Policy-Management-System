using HealthPolicyManagementSystem.Data;
using HealthPolicyManagementSystem.Helpers;
using HealthPolicyManagementSystem.Models;
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
    public class DownloadModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ActivityLogService _activityLog;

        public DownloadModel(
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

            var pdf = PolicyPdfGenerator.Generate(
                policyModel,
                template,
                documentElements);

            int userId = int.Parse(
                User.FindFirst(
                    ClaimTypes.NameIdentifier)!.Value);

            var download = new Download
            {
                PolicyID = policy.PolicyID,
                UserID = userId,
                DownloadDate = DateTime.Now
            };

            _context.Downloads.Add(download);

            _context.SaveChanges();

            _activityLog.Log(
                userId,
                $"Downloaded policy: {policy.Title}");

            var fileName =
                $"{policy.Title}-v{policy.Version}.pdf";

            return File(
                pdf,
                "application/pdf",
                fileName);
        }

        private static List<DocumentElement> DeserializeDocumentElements(
            string json)
        {
            var elements = new List<DocumentElement>();

            using var document =
                JsonDocument.Parse(json);

            foreach (var item in document.RootElement.EnumerateArray())
            {
                if (item.TryGetProperty(
                    "Type",
                    out var typeProperty))
                {
                    var type = typeProperty.GetString();

                    if (type == "Paragraph")
                    {
                        var text =
                            item.TryGetProperty(
                                "Text",
                                out var textProperty)
                                ? textProperty.GetString() ?? ""
                                : "";

                        elements.Add(
                            new ParagraphElement
                            {
                                Text = text
                            });
                    }
                    else if (type == "Table")
                    {
                        var table =
                            new TableElement();

                        if (item.TryGetProperty(
                            "Rows",
                            out var rowsProperty))
                        {
                            foreach (
                                var row
                                in rowsProperty.EnumerateArray())
                            {
                                var cells =
                                    new List<string>();

                                foreach (
                                    var cell
                                    in row.EnumerateArray())
                                {
                                    cells.Add(
                                        cell.GetString() ?? "");
                                }

                                table.Rows.Add(cells);
                            }
                        }

                        elements.Add(table);
                    }
                }
            }

            return elements;
        }
    }
}