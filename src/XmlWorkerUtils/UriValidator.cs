using System;
using System.Text.RegularExpressions;
using System.Web;

namespace kuujinbo.StackOverflow.iTextSharp.MVC.XmlWorkerUtils
{
    public static class UriValidator
    {
        public const string SUPPORTED_SCHEMES = "Uri scheme must be: Uri.UriSchemeFile || Uri.UriSchemeHttp || Uri.UriSchemeHttps";
        public const string NOT_ABSOLUTE_URI = "not absolute Uri";
        public const string INVALID_BASEURI = "cannot create a valid BaseUri";

        /// <summary>
        /// rfc2606, section 2 
        /// </summary>
        public static Regex PrivateTLDs = new Regex(
            @"\.?(?:test|example|invalid)$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        public const string BASE64_MATCH_GROUP = "imageData";
        /// <summary>
        /// rfc2045, section 6.8 (alphabet/padding) 
        /// </summary>
        public static Regex Base64 = new Regex(
            string.Format(
                @"^data:image/[^;]+;base64,(?<{0}>[a-z0-9+/]+={{0,2}})$",
                BASE64_MATCH_GROUP
            ),
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        public static bool IsWebUrl(Uri uri)
        {
            return uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;
        }

        public static bool Create(string baseUri, out Uri uri)
        {
            return Uri.TryCreate(baseUri, UriKind.RelativeOrAbsolute, out uri);
        }

        public static bool CreateAbsolute(string baseUri, out Uri uri)
        {
            return Uri.TryCreate(baseUri, UriKind.Absolute, out uri);
        }

        public static bool CreateBase(string baseUri, bool absolute, out Uri uri)
        {
            var webContext = HttpContext.Current;
            if (string.IsNullOrEmpty(baseUri) && webContext != null)
            {   // running on a web server; need to update original value  
                var request = webContext.Request;
                baseUri = request.ApplicationPath;
                if (absolute)
                    baseUri = request.Url.GetLeftPart(UriPartial.Authority) + baseUri;
            }

            return absolute ? CreateAbsolute(baseUri, out uri) : Create(baseUri, out uri);
        }
    }
}