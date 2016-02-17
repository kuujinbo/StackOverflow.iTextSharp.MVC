using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;
using kuujinbo.StackOverflow.iTextSharp.MVC.Helpers;

namespace kuujinbo.StackOverflow.iTextSharp.MVC.Controllers.XmlWorkers
{

    public class IisLinkProvider : ILinkProvider
    {
        // default implementation to resolve relative URLs when using 
        // iTextSharp XMLWorker/XMLParser
        public string GetLinkRoot()
        {
            var context = HttpContext.Current;
            return context != null
            ? context.Request.Url.GetLeftPart(UriPartial.Authority)
            : "";
        }
    }

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

            //throw new Exception((xHtml == "<div id='wanted'><div>Remote URL: <img src='http://stackoverflow.com/users/flair/604196.png' /></div><div>Local URL: <img src='/content/kuujinbo_320-30.gif' /></div><div>Base64: <img src='data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAYAAACNbyblAAAAHElEQVQI12P4//8/w38GIAXDIBKE0DHxgljNBAAO9TXL0Y4OHwAAAABJRU5ErkJggg==' /></div></div>").ToString());

            //xHtml = "<div id='wanted'><div>Remote URL: <img src='http://stackoverflow.com/users/flair/604196.png' /></div><div>Local URL: <img src='/content/kuujinbo_320-30.gif' /></div><div>Base64: <img src='data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAYAAACNbyblAAAAHElEQVQI12P4//8/w38GIAXDIBKE0DHxgljNBAAO9TXL0Y4OHwAAAABJRU5ErkJggg==' /></div></div>";
            using (var stringReader = new StringReader(xHtml))
            {
                using (Document document = new Document())
                {
                    PdfWriter writer = PdfWriter.GetInstance(
                        document, Response.OutputStream
                    );
                    document.Open();

                    var xmlWorkerHelper = XMLWorkerHelper.GetInstance();
                    var cssResolver = xmlWorkerHelper.GetDefaultCssResolver(true);
                    var htmlPipelineContext = new HtmlPipelineContext(null);
                    htmlPipelineContext
                        .SetTagFactory(Tags.GetHtmlTagProcessorFactory())
                        // .SetLinkProvider(new IisLinkProvider())
                        .SetImageProvider(new ImageProvider())
                    ;
                    // tagProcessorFactory.RemoveProcessor(HTML.Tag.IMG);
                    // htmlPipelineContext.SetImageProvider(new ImageProvider());
                    var pdfWriterPipeline = new PdfWriterPipeline(document, writer);
                    var htmlPipeline = new HtmlPipeline(htmlPipelineContext, pdfWriterPipeline);
                    var cssResolverPipeline = new CssResolverPipeline(cssResolver, htmlPipeline);

                    XMLWorker worker = new XMLWorker(cssResolverPipeline, true);
                    XMLParser parser = new XMLParser(worker);
                    parser.Parse(stringReader);
                }
            }
            return new EmptyResult();
        }

    }
}