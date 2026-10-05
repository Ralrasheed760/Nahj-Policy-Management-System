using HealthPolicyManagementSystem.ViewModels;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;

namespace HealthPolicyManagementSystem.Helpers
{
    public static class PolicyPdfGenerator
    {
        public static byte[] Generate(
            PolicyViewModel policy,
            TemplateDesignerViewModel template,
            List<DocumentElement> documentElements)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(45);

                    page.DefaultTextStyle(x =>
                        x.FontSize(12));

                    page.Header()
                        .Row(row =>
                        {
                            if (!string.IsNullOrWhiteSpace(template.LogoPath))
                            {
                                string logo = Path.Combine(
                                    Directory.GetCurrentDirectory(),
                                    "wwwroot",
                                    template.LogoPath.TrimStart('/')
                                        .Replace(
                                            "/",
                                            Path.DirectorySeparatorChar.ToString()));

                                if (File.Exists(logo))
                                {
                                    row.ConstantItem(110)
                                        .Height(80)
                                        .AlignMiddle()
                                        .Image(logo)
                                        .FitArea();
                                }
                            }

                            row.RelativeItem();
                        });

                    page.Content()
                        .PaddingTop(80)
                        .Column(column =>
                        {
                            column.Spacing(20);

                            column.Item()
                                .AlignCenter()
                                .Text(policy.Title)
                                .Bold()
                                .FontSize(24)
                                .FontColor("#009FE3");

                            column.Item()
                                .Height(360);

                            column.Item()
                                .AlignLeft()
                                .Column(info =>
                                {
                                    info.Spacing(8);

                                    if (template.ShowVersion)
                                    {
                                        info.Item()
                                            .Text(text =>
                                            {
                                                text.Span("Version")
                                                    .Bold()
                                                    .FontColor("#009FE3");

                                                text.Span($"    {policy.Version}")
                                                    .FontColor("#000000");
                                            });
                                    }

                                    if (template.ShowDate)
                                    {
                                        info.Item()
                                            .Text(text =>
                                            {
                                                text.Span("Issue Date")
                                                    .Bold()
                                                    .FontColor("#009FE3");

                                                text.Span(
                                                    $"    {policy.EffectiveDate:yyyy-MM-dd}")
                                                    .FontColor("#000000");
                                            });
                                    }
                                });
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("Page ");
                            text.CurrentPageNumber();
                        });
                });


                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);

                    page.DefaultTextStyle(x =>
                        x.FontSize(12));

                    page.Header()
                        .Height(70)
                        .Row(row =>
                        {
                            if (!string.IsNullOrWhiteSpace(template.LogoPath))
                            {
                                string clusterLogo = Path.Combine(
                                    Directory.GetCurrentDirectory(),
                                    "wwwroot",
                                    template.LogoPath.TrimStart('/')
                                        .Replace(
                                            "/",
                                            Path.DirectorySeparatorChar.ToString()));

                                if (File.Exists(clusterLogo))
                                {
                                    row.ConstantItem(100)
                                        .Height(70)
                                        .AlignMiddle()
                                        .Image(clusterLogo)
                                        .FitArea();
                                }
                            }

                            row.RelativeItem();
                        });


                    page.Content()
                        .PaddingTop(15)
                        .Column(column =>
                        {
                            column.Spacing(10);

                            foreach (var element in documentElements)
                            {
                                if (element is ParagraphElement paragraph)
                                {
                                    if (!string.IsNullOrWhiteSpace(paragraph.Text))
                                    {
                                        column.Item()
                                            .PaddingBottom(8)
                                            .Text(paragraph.Text)
                                            .FontSize(12);
                                    }
                                }

                                else if (element is TableElement table)
                                {
                                    if (table.Rows == null ||
                                        table.Rows.Count == 0)
                                    {
                                        continue;
                                    }

                                    int columns = table.Rows
                                        .Max(row => row.Count);

                                    if (columns == 0)
                                    {
                                        continue;
                                    }

                                    column.Item()
                                        .PaddingVertical(10)
                                        .Table(t =>
                                        {
                                            t.ColumnsDefinition(c =>
                                            {
                                                for (int i = 0;
                                                     i < columns;
                                                     i++)
                                                {
                                                    c.RelativeColumn();
                                                }
                                            });

                                            foreach (var row in table.Rows)
                                            {
                                                for (int i = 0;
                                                     i < columns;
                                                     i++)
                                                {
                                                    string cellText =
                                                        i < row.Count
                                                            ? row[i]
                                                            : "";

                                                    t.Cell()
                                                        .Border(1)
                                                        .Padding(5)
                                                        .Text(cellText);
                                                }
                                            }
                                        });
                                }
                            }
                        });


                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("Page ");
                            text.CurrentPageNumber();
                        });
                });

            }).GeneratePdf();
        }
    }
}