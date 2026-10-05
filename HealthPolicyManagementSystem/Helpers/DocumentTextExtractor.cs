using DocumentFormat.OpenXml.Packaging;
using WordParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using WordTable = DocumentFormat.OpenXml.Wordprocessing.Table;
using WordTableRow = DocumentFormat.OpenXml.Wordprocessing.TableRow;
using WordTableCell = DocumentFormat.OpenXml.Wordprocessing.TableCell;
using UglyToad.PdfPig;

namespace HealthPolicyManagementSystem.Helpers
{
    public static class DocumentTextExtractor
    {
        public static List<DocumentElement> Extract(string filePath)
        {
            string extension = System.IO.Path.GetExtension(filePath).ToLower();

            return extension switch
            {
                ".docx" => ExtractWord(filePath),
                ".pdf" => ExtractPdf(filePath),
                _ => new List<DocumentElement>()
            };
        }

        private static List<DocumentElement> ExtractWord(string filePath)
        {
            List<DocumentElement> elements = new();

            using var document = WordprocessingDocument.Open(
                filePath,
                false);

            var mainPart = document.MainDocumentPart;

            if (mainPart?.Document?.Body == null)
                return elements;

            var body = mainPart.Document.Body;

            foreach (var element in body.Elements())
            {
                if (element is WordParagraph paragraph)
                {
                    string text = paragraph.InnerText.Trim();

                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        elements.Add(new ParagraphElement
                        {
                            Text = text
                        });
                    }
                }
                else if (element is WordTable table)
                {
                    TableElement tableElement = new();

                    foreach (WordTableRow row in table.Elements<WordTableRow>())
                    {
                        List<string> cells = new();

                        foreach (WordTableCell cell in row.Elements<WordTableCell>())
                        {
                            cells.Add(cell.InnerText.Trim());
                        }

                        tableElement.Rows.Add(cells);
                    }

                    elements.Add(tableElement);
                }
            }

            return elements;
        }

        private static List<DocumentElement> ExtractPdf(string filePath)
        {
            List<DocumentElement> elements = new();

            using var document = PdfDocument.Open(filePath);

            foreach (var page in document.GetPages())
            {
                string text = page.Text.Trim();

                if (!string.IsNullOrWhiteSpace(text))
                {
                    elements.Add(new ParagraphElement
                    {
                        Text = text
                    });
                }
            }

            return elements;
        }
    }
}


