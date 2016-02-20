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


//<div>Local URL: <img src='../content/kuujinbo_320-30.gif' /></div>
/*
xHtml = @"<div id='wanted'>
                
    <div>Remote URL: <img src='http://stackoverflow.com/users/flair/604196.png' /></div>
    <div>Base64: <img src='data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAYAAACNbyblAAAAHElEQVQI12P4//8/w38GIAXDIBKE0DHxgljNBAAO9TXL0Y4OHwAAAABJRU5ErkJggg==' /></div>
    <div>Remote URL: <img src='http://stackoverflow.com/users/flair/604196.png' /></div>
    <div>Base64: <img src='data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAYAAACNbyblAAAAHElEQVQI12P4//8/w38GIAXDIBKE0DHxgljNBAAO9TXL0Y4OHwAAAABJRU5ErkJggg==' /></div>
    <div>Remote URL: <img src='http://stackoverflow.com/users/flair/604196.png' /></div>
    <div>Base64: <img src='data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAYAAACNbyblAAAAHElEQVQI12P4//8/w38GIAXDIBKE0DHxgljNBAAO9TXL0Y4OHwAAAABJRU5ErkJggg==' /></div>
    <div>Remote URL: <img src='http://stackoverflow.com/users/flair/604196.png' /></div>
    <div>Base64: <img src='data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAYAAACNbyblAAAAHElEQVQI12P4//8/w38GIAXDIBKE0DHxgljNBAAO9TXL0Y4OHwAAAABJRU5ErkJggg==' /></div>
    <div>Remote URL: <img src='http://stackoverflow.com/users/flair/604196.png' /></div>
    <div>Base64: <img src='data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAYAAACNbyblAAAAHElEQVQI12P4//8/w38GIAXDIBKE0DHxgljNBAAO9TXL0Y4OHwAAAABJRU5ErkJggg==' /></div>
    <div>Remote URL: <img src='http://stackoverflow.com/users/flair/604196.png' /></div>
    <div>Base64: <img src='data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAYAAACNbyblAAAAHElEQVQI12P4//8/w38GIAXDIBKE0DHxgljNBAAO9TXL0Y4OHwAAAABJRU5ErkJggg==' /></div>
    </div>
";
 */