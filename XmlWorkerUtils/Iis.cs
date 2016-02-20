using System;
using System.Web;

namespace kuujinbo.StackOverflow.iTextSharp.MVC.XmlWorkerUtils
{
    public class Iis
    {
        public static string BaseUrl(HttpContext context)
        {
            return context != null
                ? context.Request.Url.GetLeftPart(UriPartial.Authority)
                : ""
            ;
        }
    }
}