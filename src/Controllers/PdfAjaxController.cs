using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace kuujinbo.StackOverflow.iTextSharp.MVC.Controllers
{

public class PdfAjaxController : ApiController
{
    private byte[] CreatePdf()
    {
        using (MemoryStream ms = new MemoryStream())
        {
            using (Document document = new Document())
            {
                PdfWriter.GetInstance(document, ms);
                document.Open();
                document.Add(new Paragraph("Hello World"));
            }
            return ms.ToArray();
        }
    }

    public HttpResponseMessage Get()
    {
        var response = Request.CreateResponse(HttpStatusCode.OK);
        response.Content = new ByteArrayContent(CreatePdf());
        response.Content.Headers.ContentDisposition =
            new ContentDispositionHeaderValue("attachment")
        { 
            FileName = "test.pdf" 
        };
        response.Content.Headers .ContentType =
            new MediaTypeHeaderValue("application/pdf");

        return response;
    }
}


}