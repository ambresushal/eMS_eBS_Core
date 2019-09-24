using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace tmg.equinox.infrastructure.util
{
   public static class HtmlContentHelper
    {
       static Regex rgxIdentifyHtml = new Regex(@"<\s*([^ >]+)[^>]*>.*?<\s*/\s*\1\s*>");
       static Regex rgxReplaceHtml = new Regex(@"<.*?>");
        

        public static bool IsHTML(string contentString)
        {
            return rgxIdentifyHtml.IsMatch(contentString);
        }

        public static string GetFreeFromHtmlText(string htmlString)
        {
            return rgxReplaceHtml.Replace(htmlString, string.Empty);
        }
    }
}
