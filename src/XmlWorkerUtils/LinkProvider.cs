using System;
using System.IO;
using iTextSharp.tool.xml.pipeline.html;

namespace kuujinbo.StackOverflow.iTextSharp.MVC.XmlWorkerUtils
{
    // make hyperlinks with relative URLs absolute
    public class LinkProvider : ILinkProvider
    {
        // rfc1738 - file URI scheme section 3.10
        public const char SEPARATOR = '/';
        public string BaseUrl { get; private set; }

        public LinkProvider(string baseUrl)
        {
            var uri = new UriHelper(baseUrl, false).BaseUri;
            if (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
            {
                BaseUrl = new UriHelper(baseUrl, false).BaseUri.AbsoluteUri;
                // web context always http[s]    ^^^^^ Uri.Scheme 
            }
            else if (uri.Scheme == Uri.UriSchemeFile) 
            {
                // Uri class needs trailing separator or things break
                BaseUrl = uri.AbsoluteUri.TrimEnd(SEPARATOR) + SEPARATOR;
            }
            else { BaseUrl = baseUrl; }
        }

        public string GetLinkRoot()
        {
            return BaseUrl;
        }
    }
}