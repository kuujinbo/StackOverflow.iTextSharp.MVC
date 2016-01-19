using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Net;

// http://stackoverflow.com/questions/30385192
namespace kuujinbo.StackOverflow.iTextSharp.MVC.Controllers
{
    public class UploadMultipleImagesController : Controller
    {
        int APPEND_NEW_PAGE_NUMBER = 2;
        // GET: UploadMultipleImages
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(IEnumerable<HttpPostedFileBase> files)
        {
            // image file extension check ignore for brevity
            var fileList = files.Where(h => h != null && h.ContentLength > 0);
            if (fileList.Count() < 1)
                throw new Exception("no files uploaded");

            var absoluteX = 20;
            var absoluteY = absoluteX;
            using (var stream = new MemoryStream())
            {
                using (var stamper = new PdfStamper(Helpers.Pdf.GetTestReader(), stream))
                {
                    var pageOneSize = stamper.Reader
                        .GetPageSize(APPEND_NEW_PAGE_NUMBER - 1);
                    foreach (var file in fileList)
                    {
                        using (var br = new BinaryReader(file.InputStream))
                        {
                            var imageBytes = br.ReadBytes(file.ContentLength);
                            var image = Image.GetInstance(imageBytes);

                            // still not sure if you want to add a new blank page, but 
                            // here's how
                            stamper.InsertPage(
                                APPEND_NEW_PAGE_NUMBER,
                                pageOneSize
                            );

                            // image absolute position
                            image.SetAbsolutePosition(absoluteX, absoluteY);

                            // scale image if needed
                            image.ScaleToFit(pageOneSize);
                            // image.ScaleAbsolute(...);

                            // PAGE_NUMBER => add image to specific page number
                            stamper.GetOverContent(APPEND_NEW_PAGE_NUMBER)
                                .AddImage(image);
                        }
                        ++APPEND_NEW_PAGE_NUMBER;
                    }
                }
                return File(stream.ToArray(), "application/pdf");
            }
        }

    }
}