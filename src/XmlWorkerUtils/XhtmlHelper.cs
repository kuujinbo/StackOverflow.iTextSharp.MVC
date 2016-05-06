using System.Text.RegularExpressions;

namespace kuujinbo.StackOverflow.iTextSharp.MVC.XmlWorkerUtils
{
    public static class XhtmlHelper
    {
        /// <summary>
        /// self-close simple tags: <img>; <hr>; <br>
        /// </summary>
        /// <param name="xHtml">XHTML string</param>
        /// <returns>'clean' XHTML</returns>
        public static string CloseSimpleTags(string xHtml)
        {
            return Regex.Replace(
                xHtml,
                @"(?<selfClose><(?:img|br|hr)[^>]*)(?<=[^/\s])>",
                new MatchEvaluator(match => match.Groups["selfClose"].Value + " />"),
                RegexOptions.IgnoreCase | RegexOptions.Multiline
            );
        }

    }
}