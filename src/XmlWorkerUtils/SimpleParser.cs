﻿using System.Collections.Generic;
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
        public IEnumerable<Func<string, string>> XhtmlCleaner { get; set; }

        public SimpleParser() : this(null) { }
        public SimpleParser(string baseUri)
        {
            LinkProvider = new LinkProvider(new UriHelper(baseUri, false));
            ImageProvider = new ImageProvider(new UriHelper(baseUri, true));
        }

        public virtual void Parse(Stream stream, string xHtml)
        {
            xHtml = CleanXhtml(xHtml);

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

        public virtual string CloseSimpleTags(string xHtml)
        {
            return Regex.Replace(
                xHtml,
                "(?<selfClose><(?:img|br|hr)[^>]*)(?<=[^/])>",
                new MatchEvaluator(match => match.Groups["selfClose"].Value + " />"),
                RegexOptions.IgnoreCase | RegexOptions.Multiline
            );
        }

        private string CleanXhtml(string xHtml)
        {
            XhtmlCleaner = XhtmlCleaner ?? new List<Func<string, string>>() { CloseSimpleTags };
            foreach (var func in XhtmlCleaner) xHtml = func(xHtml);

            return xHtml;
        }

        private void InitParser()
        {
            HtmlPipelineContext = HtmlPipelineContext ?? new HtmlPipelineContext(null);
            LinkProvider = LinkProvider ?? new LinkProvider(new UriHelper(null, false));
            ImageProvider = ImageProvider ?? new ImageProvider(new UriHelper(null, true));
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