using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;

namespace HealthPolicyManagementSystem.Helpers
{
    public static class PdfMergeHelper
    {
        public static byte[] Merge(
            byte[] coverPdf,
            string policyFilePath)
        {
            using var output = new MemoryStream();

            using (var resultDocument = new PdfDocument())
            {
                using (var coverStream = new MemoryStream(coverPdf))
                {
                    var coverDocument = PdfReader.Open(
                        coverStream,
                        PdfDocumentOpenMode.Import);

                    foreach (var page in coverDocument.Pages)
                    {
                        resultDocument.AddPage(page);
                    }
                }


                if (File.Exists(policyFilePath))
                {
                    using var policyDocument =
                        PdfReader.Open(
                            policyFilePath,
                            PdfDocumentOpenMode.Import);


                    foreach (var page in policyDocument.Pages)
                    {
                        resultDocument.AddPage(page);
                    }
                }


                resultDocument.Save(output);
            }

            return output.ToArray();
        }
    }
}