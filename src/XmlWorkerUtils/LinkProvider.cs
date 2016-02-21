using iTextSharp.tool.xml.pipeline.html;

namespace kuujinbo.StackOverflow.iTextSharp.MVC.XmlWorkerUtils
{
    // resolve hyperlinks with relative URLs, and make them absolute
    public class LinkProvider : ILinkProvider
    {
        private string _baseUrl;

        public LinkProvider(string baseUrl) 
            : this(baseUrl, UriHelper.UriType.Remote) {}

        public LinkProvider(string baseUrl, UriHelper.UriType type)
        {
            _baseUrl = new UriHelper(baseUrl, type).BaseUri.AbsoluteUri;
        }

        public string GetLinkRoot()
        {
            return _baseUrl;
        }
    }
}