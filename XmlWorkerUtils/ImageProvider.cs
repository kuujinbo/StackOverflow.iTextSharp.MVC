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
        private HttpContext _httpContext;
        private bool _isWebContext;
        private Uri _baseUri;
        public float ScalePercent { get; set; }

        public ImageProvider() : this(null) { }
        public ImageProvider(string baseUri)
        {
            _httpContext = HttpContext.Current;
            if (!string.IsNullOrEmpty(baseUri)) 
            {
                _baseUri = new Uri(baseUri);
            }
            else if (_httpContext != null)
            {
                _isWebContext = true;
                _baseUri = new Uri(Iis.BaseUrl(_httpContext));
            }
            ScalePercent = 67f;
        }
        
        public Regex Base64 = new Regex(
            @"^data:image/(?<mediaType>[^;]+);base64,(?<data>.*)",
            RegexOptions.Compiled
        );

        public virtual Image ScaleImage(Image img)
        {
            img.ScalePercent(ScalePercent);
            return img;
        }

        private string _relativeToAbsolute(string src)
        {
            if (_isWebContext)
            {
                return _httpContext.Server.MapPath(
                    new Uri(_baseUri, src).AbsolutePath
                );
            }
            else if (_baseUri != null)
            {
                return new Uri(_baseUri, src).AbsolutePath;
            }

            return null;
        }

        public virtual Image Retrieve(string src)
        {
            if (_cachedImages.ContainsKey(src)) return _cachedImages[src];

            try
            {
                if (Regex.IsMatch(src, "^https?://", RegexOptions.IgnoreCase))
                {
                    return ScaleImage(Image.GetInstance(src));
                }

                Match match;
                if ((match = Base64.Match(src)).Length > 0)
                {
                    return ScaleImage(Image.GetInstance(
                        Convert.FromBase64String(match.Groups["data"].Value)
                    ));
                }

                var uri = _relativeToAbsolute(src);
                return !string.IsNullOrEmpty(uri)
                    ? ScaleImage(Image.GetInstance(uri)) : null;
            }
            catch (BadElementException ex) { return null; }
            catch (IOException ex) { return null; }
            catch (Exception ex) { return null; }
        }

        private Dictionary<string, Image> _cachedImages = new Dictionary<string,Image>();
        /*
         * always called after Retrieve(string src): cache any duplicate <img>
         * in the HTML source so the iTextSharp.text.Image bytes are only 
         * written to the PDF **once**, which reduces the resulting file size.
         */
        public virtual void Store(string src, Image img)
        {
            if (!_cachedImages.ContainsKey(src))
            {
                _cachedImages.Add(src, img);
            }
        }


        // no need to provide concrete implementations
        public virtual string GetImageRootPath() { return null; }
        public virtual void Reset() { }
    }
}