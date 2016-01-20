using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using iTextImage = iTextSharp.text.Image;

// http://stackoverflow.com/questions/34869412
namespace kuujinbo.StackOverflow.iTextSharp.MVC.WebForms
{
    public partial class HtmlImageHandler : System.Web.UI.Page
    {
        // ConvertControlToPdf is a HtmlTable, used for brevity.
        // - HTML conversion for a GridView is **EXACTLY** the same
        protected void ProcessHtml(object sender, CommandEventArgs e)
        {
            Response.ContentType = "application/pdf";
            Response.AppendHeader("Content-Disposition", "attachment; filename=table.pdf");
            using (Document document = new Document())
            {
                PdfWriter.GetInstance(document, Response.OutputStream);
                document.Open();

                var html = new StringBuilder();
                using (var stringWriter = new StringWriter(html))
                {
                    using (var htmlWriter = new HtmlTextWriter(stringWriter))
                    {
                        // replace with GridView control Id!
                        ConvertControlToPdf.RenderControl(htmlWriter);
                    }
                }

                var providers = new Dictionary<string, Object>();
                // HTMLWorker does **NOT** understand relative URLs, so
                // make existing ones in HTML source absolute, and handle 
                // base64 Data URI schemes
                var ih = new ImageHander() { BaseUri = Request.Url.ToString() };

                // dictionary key 'img_provider' is **HARD-CODED**, in 
                // iTextSharp 5.0.0 - 5.0.5, so you may need to use this
                // interfaceProps.Add("img_provider", ih);
                providers.Add(HTMLWorker.IMG_PROVIDER, ih);
                //            ^^^^^^^^^^^^^^^^^^^^^^^ - constant added in 5.0.6
                using (var sr = new StringReader(html.ToString()))
                {
                    foreach (IElement element in HTMLWorker.ParseToList(
                        sr, null, providers))
                    {
                        PdfPTable table = element as PdfPTable;
                        document.Add(element);
                    }
                }
            }
            Response.End();
        }

        // handle <img> tags in any System.Web.UI.Control (GridView) with:
        // 1. base64 Data URI scheme - https://en.wikipedia.org/wiki/Data_URI_scheme
        // 2. absolute URLs on a remote/local server 
        // 3. relative URLs on local server (DEFAULT)
        public class ImageHander : IImageProvider
        {
            public string BaseUri { get; set; }
            public static Regex Base64 = new Regex(
                @"^data:image/(?<mediaType>[^;]+);base64,(?<data>.*)",
                RegexOptions.Compiled
            );

            // alias: using iTextImage = iTextSharp.text.Image;
            public iTextImage GetImage(string src,
                IDictionary<string, string> attrs,
                ChainedProperties chain,
                IDocListener doc)
            {
                Match match;
                // [1]
                if ((match = Base64.Match(src)).Length > 0)
                {
                    return iTextImage.GetInstance(
                        Convert.FromBase64String(match.Groups["data"].Value)
                    );
                }

                // [2]
                if (!src.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    src = HttpContext.Current.Server.MapPath(
                        new Uri(new Uri(BaseUri), src).AbsolutePath
                    ); 
                }
                return iTextImage.GetInstance(src);
            }
        }

    }
}