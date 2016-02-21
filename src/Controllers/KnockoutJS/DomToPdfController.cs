using System.IO;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;

// http://stackoverflow.com/questions/35342874
namespace kuujinbo.StackOverflow.iTextSharp.MVC.Controllers.KnockoutJS
{
    public class DomToPdfController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]  // some browsers have URL length limits
        [ValidateInput(false)] // or throws HttpRequestValidationException
        public ActionResult Index(string xHtml)
        {
            Response.ContentType = "application/pdf";
            Response.AppendHeader(
                "Content-Disposition", "attachment; filename=test.pdf"
            );
            using (var stringReader = new StringReader(xHtml))
            {
                using (Document document = new Document())
                {
                    PdfWriter writer = PdfWriter.GetInstance(
                        document, Response.OutputStream
                    );
                    document.Open();
                    XMLWorkerHelper.GetInstance().ParseXHtml(
                      writer, document, stringReader
                    );
                }
            }

            return new EmptyResult();
        }

    }
}