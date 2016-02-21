using iTextSharp.tool.xml.pipeline.html;

namespace kuujinbo.StackOverflow.iTextSharp.MVC.XmlWorkerUtils
{
    // make hyperlinks with relative URLs absolute: [1] URL; [2] file URI file:///
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