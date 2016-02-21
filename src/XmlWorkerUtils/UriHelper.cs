using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kuujinbo.StackOverflow.iTextSharp.MVC.XmlWorkerUtils
{
    public class UriHelper
    {
        private HttpContext _httpContext;
        private UriType _uriType;

        public enum UriType { Local, Remote }

        public Uri BaseUri { get; private set; }

        public UriHelper() : this(null, UriType.Local) { }
        public UriHelper(UriType uriType) : this(null, uriType) { }
        public UriHelper(string baseUri, UriType uriType)
        {
            _uriType = uriType;
            _httpContext = HttpContext.Current;
            BaseUri = CreateBase(baseUri);
        }

        // get a URI for IImageProvider
        public string Join(string relativeUri)
        {
            if (_uriType == UriType.Local)
            {
                return _httpContext != null
                    ? _httpContext.Server.MapPath(
                        new Uri(BaseUri, relativeUri).AbsolutePath
                    )
                    : new Uri(BaseUri, relativeUri).AbsolutePath;
            }
            // a file URI - file:///
            return new Uri(BaseUri, relativeUri).AbsoluteUri;
        }

        private Uri CreateBase(string baseUrl)
        {
            if (string.IsNullOrEmpty(baseUrl) && _httpContext != null)
            {   // running on a web server
                var req = _httpContext.Request;
                baseUrl = _uriType == UriType.Local
                    // get from file system for IImageProvider
                    ? req.ApplicationPath
                    // absolute URI for ILinkProvider
                    : req.Url.GetLeftPart(UriPartial.Authority)
                        + _httpContext.Request.ApplicationPath;
            }

            Uri uri;
            if (Uri.TryCreate(baseUrl, UriKind.RelativeOrAbsolute, out uri)) return uri;

            throw new InvalidOperationException("cannot create a valid BaseUri");
        }

    }
}