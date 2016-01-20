using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;

namespace kuujinbo.StackOverflow.iTextSharp.MVC.Controllers
{
    public class HtmlImageHandlerController : Controller
    {
        // GET: HtmlImageHandler
        public ActionResult Index()
        {
            return View();
        }
    }
}