using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using iTextSharp.text;
using iTextSharp.tool.xml.pipeline.html;

namespace kuujinbo.StackOverflow.iTextSharp.MVC.XmlWorkerUtils
{
    // create iTextSharp.text.Image
    public class ImageProvider : IImageProvider
    {
        private UriHelper _uriHelper;
        // see Store(string src, Image img)
        private Dictionary<string, Image> _imageCache = 
            new Dictionary<string, Image>();

        public virtual float ScalePercent { get; set; }
        public virtual Regex Base64 { get; set; }

        public ImageProvider(string baseUri) : this(baseUri, 67f) { }
        public ImageProvider(string baseUri, float scalePercent)
        {
            _uriHelper = new UriHelper(baseUri);
            ScalePercent = scalePercent;
            Base64 = new Regex( // rfc2045, section 6.8 (alphabet/padding)
                @"^data:image/[^;]+;base64,(?<data>[a-z0-9+/]+={0,2})$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase
            );
        }
        
        public virtual Image ScaleImage(Image img)
        {
            img.ScalePercent(ScalePercent);
            return img;
        }

        public virtual Image Retrieve(string src)
        {
            if (_imageCache.ContainsKey(src)) return _imageCache[src];

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

                var imgPath = _uriHelper.Combine(src);
                return ScaleImage(Image.GetInstance(imgPath));
            }
            catch (BadElementException ex) { return null; }
            catch (IOException ex) { return null; }
            catch (Exception ex) { return null; }
        }

        /*
         * always called after Retrieve(string src): cache any duplicate <img>
         * in the HTML source so the iTextSharp.text.Image bytes are only 
         * written to the PDF **once**, which reduces the resulting file size.
         */
        public virtual void Store(string src, Image img)
        {
            if (!_imageCache.ContainsKey(src)) _imageCache.Add(src, img);
        }


        // no need to provide concrete implementations
        public virtual string GetImageRootPath() { return null; }
        public virtual void Reset() { }
    }
}