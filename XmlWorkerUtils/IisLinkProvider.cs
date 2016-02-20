using System;
using System.Web;
using iTextSharp.tool.xml.pipeline.html;

namespace kuujinbo.StackOverflow.iTextSharp.MVC.XmlWorkerUtils
{
    public class IisLinkProvider : ILinkProvider
    {
        // resolve relative URLs
        public string GetLinkRoot()
        {
            return Iis.BaseUrl(HttpContext.Current);
        }
    }
}