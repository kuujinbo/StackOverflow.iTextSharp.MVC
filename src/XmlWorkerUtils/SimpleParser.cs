﻿using System;
using System.IO;
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
        public virtual ILinkProvider LinkProvider { get; set; }
        public virtual IImageProvider ImageProvider { get; set; }
        
        public virtual HtmlPipelineContext HtmlPipelineContext { get; set; }
        public virtual ITagProcessorFactory TagProcessorFactory { get; set; }
        public virtual ICSSResolver CssResolver { get; set; }

        public SimpleParser() : this(null) { }
        public SimpleParser(string baseUrl)
        {
            LinkProvider = new LinkProvider(baseUrl);
            ImageProvider = new ImageProvider(baseUrl);

            HtmlPipelineContext = new HtmlPipelineContext(null);
            TagProcessorFactory = Tags.GetHtmlTagProcessorFactory();
            CssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(true);
        }

        /*
         * when sending XHR via any of the popular JavaScript frameworks,
         * <img> tags are **NOT** always closed, which results in the 
         * infamous iTextSharp.tool.xml.exceptions.RuntimeWorkerException:
         * 'Invalid nested tag a found, expected closing tag img.' a simple
         * workaround.
         */
        public virtual string SimpleAjaxImgFix(string xHtml)
        {
            return Regex.Replace(
                xHtml,
                "(?<image><img[^>]+)(?<=[^/])>",
                new MatchEvaluator(match => match.Groups["image"].Value + " />"),
                RegexOptions.IgnoreCase | RegexOptions.Multiline
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

                    HtmlPipelineContext
                        .SetTagFactory(Tags.GetHtmlTagProcessorFactory())
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
    }


}