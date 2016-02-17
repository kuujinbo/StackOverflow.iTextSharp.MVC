using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using iTextSharp.text;
using iTextSharp.tool.xml.pipeline.html;
using iTextImage = iTextSharp.text.Image;

namespace kuujinbo.StackOverflow.iTextSharp.MVC.Helpers
{
    public class ImageProvider : IImageProvider
    {
        public static Regex Base64 = new Regex(
            @"^data:image/(?<mediaType>[^;]+);base64,(?<data>.*)",
            RegexOptions.Compiled
        );

        public string GetImageRootPath() { return null; }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public iTextImage Retrieve(string src)
        {
            try
            {
                Match match;
                if ((match = Base64.Match(src)).Length > 0)
                {
                    return iTextImage.GetInstance(
                        Convert.FromBase64String(match.Groups["data"].Value)
                    );
                }
                else if (Regex.IsMatch(src, "^http", RegexOptions.IgnoreCase))
                {
                    return iTextImage.GetInstance(src);
                }
                else
                {
                    return iTextImage.GetInstance(HttpContext.Current.Server.MapPath(src));
                }
            }
            catch (BadElementException ex) { return null; } 
            catch (IOException ex) { return null; }
        }

        public void Store(string src, iTextImage img)
        {
            // throw new NotImplementedException();
        }
    }
}