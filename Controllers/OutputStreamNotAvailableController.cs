using System.IO;
using System.Collections.Generic;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;

// http://stackoverflow.com/questions/34810081
namespace kuujinbo.StackOverflow.iTextSharp.MVC.Controllers
{
    public class Plant
    {
        // stub for example
    }

    public class OutputStreamNotAvailableController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        // GET: ClosedStream
        public ActionResult IndexPdf()
        {
            var plants = new List<Plant>();
            // get your plants here
            byte[] byteInfo = GeneratePdf(plants);

            return File(byteInfo, "application/pdf");
        }

        private static byte[] GeneratePdf(List<Plant> plants)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (var doc = new Document())
                {
                    PdfWriter.GetInstance(doc, memoryStream);
                    doc.Open();
                    doc.SetMargins(120, 120, 270, 270);
                    Paragraph pgTitle = new Paragraph("TEST");
                    doc.Add(pgTitle);

                    // initialize title, etc, here
                    // and iterate over plants here
                }
                // return **AFTER** Document is disposed
                return memoryStream.ToArray();
            }
        }
    }


}