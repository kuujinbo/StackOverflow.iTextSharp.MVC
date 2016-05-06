using System;
using System.Web;
using iTextSharp.tool.xml.pipeline.html;

namespace kuujinbo.StackOverflow.iTextSharp.MVC.XmlWorkerUtils
{
    // make hyperlinks with relative URLs absolute
    public class LinkProvider : ILinkProvider
    {
        // rfc1738 - file URI scheme section 3.10
        public const char SEPARATOR = '/';

        private Uri _uri;
        private string _baseUri;

        public LinkProvider() : this(null) { }
        public LinkProvider(string baseUri)
        {
            if (!UriValidator.CreateBase(baseUri, true, out _uri))
                throw new InvalidOperationException(UriValidator.NOT_ABSOLUTE_URI);

            // need trailing separator or file paths break
            if (_uri.Scheme == Uri.UriSchemeFile)
            {
                _baseUri = _uri.AbsoluteUri.TrimEnd(SEPARATOR) + SEPARATOR;
            }
            else if (UriValidator.IsWebUrl(_uri))
            {
                _baseUri = _uri.AbsoluteUri;
            }
            else
            {
                throw new InvalidOperationException(UriValidator.SUPPORTED_SCHEMES);
            }
        }

        public string GetLinkRoot()
        {
            return _baseUri;
        }
    }
}