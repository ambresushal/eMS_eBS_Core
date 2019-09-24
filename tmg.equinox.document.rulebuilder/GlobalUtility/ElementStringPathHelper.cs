using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace tmg.equinox.document.rulebuilder.pathhelper
{
   public static class ElementStringPathHelper
    {

       public static string GetSectionName(this string path)
       {
           return path.Substring(path.IndexOf('[') + 1, (path.IndexOf('.') - path.IndexOf('[')) - 1); ;
       }

       public static string GetRuleAlise(this string path)
       {
           return path.Substring(0, path.IndexOf('[')); ;
       }

       public static string GetElementPath(this string path)
       {
           string regularExpressionPattern = @"(?<=\[)(.*?)(?=\])";
           Regex regex = new Regex(regularExpressionPattern);
           return regex.Match(path).Value.TrimEnd('.');
       }


       public static List<string> GetElementPaths(this string path)
       {
           List<string> sourceElements = new List<string>();
           string regularExpressionPattern = @"(?<=\[)(.*?)(?=\])";
           Regex regex = new Regex(regularExpressionPattern);
           foreach (Match m in regex.Matches(path))
           {
               sourceElements.Add(m.Value);
           }
           return sourceElements;
       }
    }
}
