using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using kuujinbo.StackOverflow.iTextSharp.MVC.XmlWorkerUtils;

namespace kuujinbo.StackOverflow.iTextSharp.MVC.WebForms.XmlWorkers
{
    public partial class ImageParser : System.Web.UI.Page
    {
        // ConvertControlToPdf is a HtmlTable, used for brevity.
        // - HTML conversion for a GridView is **EXACTLY** the same
        protected void ProcessHtml(object sender, CommandEventArgs e)
        {
            Response.ContentType = "application/pdf";
            Response.AppendHeader("Content-Disposition", "attachment; filename=test.pdf");
            using (var stringWriter = new StringWriter())
            {
                using (var htmlWriter = new HtmlTextWriter(stringWriter))
                {
                    ConvertControlToPdf.RenderControl(htmlWriter);
                }
                var simpleParser = new SimpleParser();
                simpleParser.Parse(Response.OutputStream, stringWriter.ToString());
            }
            Response.End();
        }
    }
}