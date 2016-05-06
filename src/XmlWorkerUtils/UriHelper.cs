using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kuujinbo.StackOverflow.iTextSharp.MVC.XmlWorkerUtils
{
    public interface IUriHelper
    {
        Uri CreateBase(string baseUri);
        string Combine(string relativeUri);
        Uri BaseUri { get; }
    }

    // resolve URIs for LinkProvider & ImageProvider
    public class UriHelper : IUriHelper
    {
        /* IsLocal; when running in web context:
         * [1] give LinkProvider http[s] scheme; see CreateBase(string baseUri)
         * [2] give ImageProvider relative path starting with '/' - see:
         *     Combine(string relativeUri)
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

        /* get URI for IImageProvider to instantiate iTextSharp.text.Image for 
         * each <img> element in the HTML.
         */
        public string Combine(string relativeUri)
        {
            /* when running in a web context, the HTML is coming from a MVC view 
             * or web form, so convert the incoming URI to a **local** path
             */
            if (IsLocal && HttpContext != null && !BaseUri.IsAbsoluteUri)
            {
                return HttpContext.Server.MapPath(
                    // Combine() checks directory traversal exploits
                    VirtualPathUtility.Combine(BaseUri.ToString(), relativeUri)
                );
            }
            return BaseUri.Scheme == Uri.UriSchemeFile 
                ? Path.Combine(BaseUri.LocalPath, relativeUri)
                // for this example we're assuming URI.Scheme is http[s]
                : new Uri(BaseUri, relativeUri).AbsoluteUri;
        }

        public Uri CreateBase(string baseUri)
        {
            if (string.IsNullOrEmpty(baseUri) && HttpContext != null)
            {   // running on a web server; need to update original value  
                var req = HttpContext.Request;
                baseUri = IsLocal
                    // IImageProvider; absolute virtual path (starts with '/')
                    // used to convert to local file system path. see:
                    // Combine(string relativeUri)
                    ? req.ApplicationPath
                    // ILinkProvider; absolute http[s] URI scheme
                    : req.Url.GetLeftPart(UriPartial.Authority)
                        + HttpContext.Request.ApplicationPath;
            }

            Uri uri;
            if (Uri.TryCreate(baseUri, UriKind.RelativeOrAbsolute, out uri)) return uri;

            throw new InvalidOperationException("cannot create a valid BaseUri");
        }
    }
}