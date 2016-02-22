using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kuujinbo.StackOverflow.iTextSharp.MVC.XmlWorkerUtils
{
    // resolve URIs for LinkProvider & ImageProvider
    public class UriHelper
    {
        /* when running in web context:
         * [1] give LinkProvider http[s] scheme; see CreateBase(string baseUrl)
         * [2] give ImageProvider relative path starting with '/' - see:
         *     Join(string relativeUri)
         */
        public bool IsLocal { get; set; }
        public HttpContext HttpContext { get; private set; }
        public Uri BaseUri { get; private set; }

        public UriHelper(string baseUri) : this(baseUri, true) {}
        public UriHelper(string baseUri, bool isLocal)
        {
            IsLocal = isLocal;
            HttpContext = HttpContext.Current;
            BaseUri = CreateBase(baseUri);
        }

        // IImageProvider; get URI 
        public string Combine(string relativeUri)
        {
            // web context; HTML from view or web form
            if (HttpContext != null && IsLocal && !BaseUri.IsAbsoluteUri)
            {
                return HttpContext.Server.MapPath(
                    // Combine() checks directory traversal exploits
                    VirtualPathUtility.Combine(BaseUri.ToString(), relativeUri)
                );
            }
            return Path.Combine(BaseUri.LocalPath, relativeUri);
        }

        private Uri CreateBase(string baseUrl)
        {
            if (HttpContext != null)
            {   // running on a web server; need to update original value  
                var req = HttpContext.Request;
                baseUrl = IsLocal
                    // IImageProvider; get local file system path
                    ? req.ApplicationPath
                    // ILinkProvider needs absolute http[s] URI; 
                    : req.Url.GetLeftPart(UriPartial.Authority)
                        + HttpContext.Request.ApplicationPath;
            }

            Uri uri;
            if (Uri.TryCreate(baseUrl, UriKind.RelativeOrAbsolute, out uri)) return uri;

            throw new InvalidOperationException("cannot create a valid BaseUri");
        }
    }
}