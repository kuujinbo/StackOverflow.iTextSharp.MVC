using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace kuujinbo.StackOverflow.iTextSharp.MVC.Helpers
{
    public static class Pdf
    {
        public static PdfReader GetTestReader()
        {
            var stream = new MemoryStream();
            {
                using (Document document = new Document())
                {
                    PdfWriter.GetInstance(document, stream);
                    document.Open();
                    document.Add(new Phrase("A PDF used for testing"));
                }
                return new PdfReader(stream.ToArray());
            }
        }
    }
}