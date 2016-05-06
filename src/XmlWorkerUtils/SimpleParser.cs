using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;

namespace kuujinbo.StackOverflow.iTextSharp.MVC.XmlWorkerUtils
{
    public class SimpleParser
    {
        public virtual HtmlPipelineContext HtmlPipelineContext { get; set; }
        public virtual ILinkProvider LinkProvider { get; set; }
        public virtual IImageProvider ImageProvider { get; set; }
        public virtual ITagProcessorFactory TagProcessorFactory { get; set; }
        public virtual IDictionary<AbstractTagProcessor, string[]> TagProcessors { get; set; }
        public virtual ICSSResolver CssResolver { get; set; }

        // for .NET 2.0
        public delegate TResult Func<T, TResult>(T arg);
        public Func<string, string> XhtmlCleaner { get; set; }

        public SimpleParser() : this(null) { }
        public SimpleParser(string baseUri)
            : this(
                new LinkProvider(baseUri),
                new ImageProvider(baseUri)
                ) { }
        public SimpleParser(
            ILinkProvider linkProvider, IImageProvider imageProvider)
        {
            LinkProvider = linkProvider;
            ImageProvider = imageProvider;
        }

        public virtual void Parse(Stream stream, string xHtml)
        {
            xHtml = XhtmlCleaner != null
                ? XhtmlCleaner(xHtml)
                : XhtmlHelper.CloseSimpleTags(xHtml);
            InitParser();

            using (var stringReader = new StringReader(xHtml))
            {
                using (Document document = new Document())
                {
                    PdfWriter writer = PdfWriter.GetInstance(document, stream);
                    document.Open();

                    HtmlPipelineContext
                        .SetTagFactory(TagProcessorFactory)
                        .SetLinkProvider(LinkProvider)
                        .SetImageProvider(ImageProvider)
                    ;
                    var pdfWriterPipeline = new PdfWriterPipeline(document, writer);
                    var htmlPipeline = new HtmlPipeline(HtmlPipelineContext, pdfWriterPipeline);
                    var cssResolverPipeline = new CssResolverPipeline(CssResolver, htmlPipeline);

                    XMLWorker worker = new XMLWorker(cssResolverPipeline, true);
                    XMLParser parser = new XMLParser(worker);
                    parser.Parse(stringReader);
                }
            }
        }

        /// <summary>
        /// initialize the parser
        /// </summary>
        private void InitParser()
        {
            HtmlPipelineContext = HtmlPipelineContext ?? new HtmlPipelineContext(null);
            LinkProvider = LinkProvider ?? new LinkProvider(null);
            ImageProvider = ImageProvider ?? new ImageProvider(null);
            CssResolver = CssResolver ?? XMLWorkerHelper.GetInstance().GetDefaultCssResolver(true);

            TagProcessorFactory = TagProcessorFactory ?? Tags.GetHtmlTagProcessorFactory();
            if (TagProcessors != null)
            {
                foreach (var processor in TagProcessors)
                {
                    TagProcessorFactory.AddProcessor(
                        processor.Key, processor.Value
                    );
                }
            }
        }
    }
}