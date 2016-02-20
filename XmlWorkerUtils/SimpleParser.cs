using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
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
        public ILinkProvider LinkProvider { get; set; }
        public ICSSResolver CssResolver { get; set; }
        public ITagProcessorFactory TagProcessorFactory { get; set; }
        public HtmlPipelineContext HtmlPipelineContext { get; set; }

        Regex _imgPattern = new Regex(
            "(?<image><img[^>]+)(?<=[^/])>",
            RegexOptions.IgnorePatternWhitespace 
                | RegexOptions.IgnoreCase 
                | RegexOptions.Multiline
        );


        public Regex ImgPattern
        {
            get { return _imgPattern; }
            set { _imgPattern = value; }
        }

        public SimpleParser()
        {
            HtmlPipelineContext = new HtmlPipelineContext(null);
            LinkProvider = new IisLinkProvider();
            TagProcessorFactory = Tags.GetHtmlTagProcessorFactory();
            CssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(true);
        }

        /*
         * when sending a XHR via any of the popular JavaScript frameworks,
         * <img> tags are **NOT** always closed, which will result in the 
         * infamous iTextSharp.tool.xml.exceptions.RuntimeWorkerException:
         * 'Invalid nested tag a found, expected closing tag img.' a simple
         * workaround.
         */
        public virtual string SimpleAjaxImgFix(string xHtml)
        {
            return ImgPattern.Replace(
                xHtml,
                new MatchEvaluator(
                    match => match.Groups["image"].Value + " />")
            );
        }

        public virtual void Parse(Stream stream, string xHtml)
        {
            xHtml = SimpleAjaxImgFix(xHtml);

            using (var stringReader = new StringReader(xHtml))
            {
                using (Document document = new Document())
                {
                    PdfWriter writer = PdfWriter.GetInstance(
                        document, stream
                    );
                    document.Open();

                    var xmlWorkerHelper = XMLWorkerHelper.GetInstance();
                    var cssResolver = xmlWorkerHelper.GetDefaultCssResolver(true);
                    var htmlPipelineContext = new HtmlPipelineContext(null);
                    htmlPipelineContext
                        .SetTagFactory(Tags.GetHtmlTagProcessorFactory())
                        .SetLinkProvider(LinkProvider)
                        .SetImageProvider(new ImageProvider())
                    ;
                    var pdfWriterPipeline = new PdfWriterPipeline(document, writer);
                    var htmlPipeline = new HtmlPipeline(htmlPipelineContext, pdfWriterPipeline);
                    var cssResolverPipeline = new CssResolverPipeline(cssResolver, htmlPipeline);

                    XMLWorker worker = new XMLWorker(cssResolverPipeline, true);
                    XMLParser parser = new XMLParser(worker);
                    parser.Parse(stringReader);
                }
            }
        }


    }
}