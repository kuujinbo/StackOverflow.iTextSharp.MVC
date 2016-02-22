using System;
using System.Web;
using System.Web.Mvc;
using kuujinbo.StackOverflow.iTextSharp.MVC.XmlWorkerUtils;

namespace kuujinbo.StackOverflow.iTextSharp.MVC.Controllers.XmlWorkers
{
    public class ImageHandlerController : Controller
    {
        // GET: ImageHandler
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
            var simpleParser = new SimpleParser();
            simpleParser.Parse(Response.OutputStream, xHtml);

            return new EmptyResult();
        }
    }
}