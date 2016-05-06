using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using iTextSharp.text;
using iTextSharp.tool.xml.pipeline.html;

namespace kuujinbo.StackOverflow.iTextSharp.MVC.XmlWorkerUtils
{
    public class ImageProvider : IImageProvider
    {
        private Uri _uri;
        public List<string> SkippedImages = new List<string>();

        /// <summary>
        /// see Store(string src, Image img) 
        /// </summary>
        private Dictionary<string, Image> _imageCache = new Dictionary<string, Image>();

        public virtual float ScalePercent { get; set; }

        public ImageProvider(string baseUri) : this(baseUri, 67f) { }
        public ImageProvider(string baseUri, float scalePercent)
        {
            ScalePercent = scalePercent;
            if (!UriValidator.CreateBase(baseUri, false, out _uri))
                throw new InvalidOperationException(UriValidator.INVALID_BASEURI);
        }

        private string Combine(string relativeUri)
        {
            // convert URI to **local** path when running in web context
            HttpContext HttpContext = HttpContext.Current;
            if (HttpContext != null && !_uri.IsAbsoluteUri)
            {
                return HttpContext.Server.MapPath(
                    // Combine() checks directory traversal exploits
                    VirtualPathUtility.Combine(_uri.ToString(), relativeUri)
                );
            }
            else if (_uri.Scheme == Uri.UriSchemeFile)
            {
                return Path.Combine(_uri.LocalPath, relativeUri);
            }
            else if (UriValidator.IsWebUrl(_uri))
            {
                return new Uri(_uri, relativeUri).AbsoluteUri;
            }

            throw new InvalidOperationException(UriValidator.NOT_ABSOLUTE_URI);
        }

        private Image ScaleImage(Image image)
        {
            image.ScalePercent(ScalePercent);
            return image;
        }

        public virtual Image Retrieve(string src)
        {
            if (_imageCache.ContainsKey(src))
            {
                return _imageCache[src];
            }

            try
            {
                Uri uri;
                Match match;
                if (Regex.IsMatch(src, "^https?://", RegexOptions.IgnoreCase))
                {
                    if (UriValidator.CreateAbsolute(src, out uri)
                        && !UriValidator.PrivateTLDs.IsMatch(uri.Host))
                    {
                        return ScaleImage(Image.GetInstance(src));
                    }
                }
                else if ((match = UriValidator.Base64.Match(src)).Length > 0)
                {
                    return ScaleImage(Image.GetInstance(
                        Convert.FromBase64String(
                            match.Groups[UriValidator.BASE64_MATCH_GROUP].Value
                        )
                    ));
                }
                else
                {
                    var imgPath = Combine(src);
                    if (UriValidator.CreateAbsolute(imgPath, out uri)
                        && uri.Scheme == Uri.UriSchemeFile)
                    {
                        return ScaleImage(Image.GetInstance(imgPath));
                    }
                }

                SkippedImages.Add(src);
                return null;
            }
            catch (BadElementException ex) { return null; }
            catch (IOException ex) { return null; }
            catch (Exception ex) { return null; }
        }

        /*
         * always called after Retrieve(string src):
         * [1] cache any duplicate <img> in the HTML source so the image bytes
         *     are only written to the PDF **once**, which reduces the 
         *     resulting file size.
         * [2] the cache can also **potentially** save network IO if you're
         *     running the parser in a loop, since Image.GetInstance() creates
         *     a WebRequest when an image resides on a remote server. couldn't
         *     find a CachePolicy in the source code
         */
        public virtual void Store(string src, Image img)
        {
            if (!SkippedImages.Contains(src) && !_imageCache.ContainsKey(src))
            {
                _imageCache.Add(src, img);
            }
        }

        /* XMLWorker documentation for ImageProvider recommends implementing
         * GetImageRootPath():
         * 
         * http://demo.itextsupport.com/xmlworker/itextdoc/flatsite.html#itextdoc-menu-10
         * 
         * but a quick run through the debugger never hits the breakpoint, so 
         * not sure if I'm missing something, or something has changed internally 
         * with XMLWorker....
         */
        // public virtual string GetImageRootPath() { throw new Exception("WTF"); }
        public virtual string GetImageRootPath() { return null; }
        public virtual void Reset() { }
    }
}