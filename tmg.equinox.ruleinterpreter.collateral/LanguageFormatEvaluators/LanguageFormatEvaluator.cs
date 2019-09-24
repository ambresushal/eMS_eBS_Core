using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpretercollateral
{
   public class LanguageFormatEvaluator
    {

        public Dictionary<string,JToken> GetLanguageFormatSource(Dictionary<string, JToken> sources,JToken masterlistSource,string tabularChart)
        {
            Dictionary<string, JToken> languageFormatSource = new Dictionary<string, JToken>();
            string languageFormatPath = "FORMAT" + tabularChart;
            JToken source = masterlistSource.SelectToken(languageFormatPath);
            sources.Add(languageFormatPath, source);
            return sources;
        }


        public bool HasValueFormat(List<LanguageFormats> formats, string formatType)
        {
            bool result = false;
            var fmtTypes = formats.Where(a => a.FormatKeywordType == formatType);
            if (fmtTypes != null && fmtTypes.Count() > 0)
            {
                result = true;
            }
            return result;
        }

        public string GetValueFormat(List<LanguageFormats> formats, string formatType,string value)
        {
            string formatString = "";
            if(formats != null && formats.Count > 0 && value != null)
            {
                var fmtTypes = formats.Where(a => a.FormatKeywordType == formatType);
                if(fmtTypes != null && fmtTypes.Count() > 0)
                {
                    var fmtType = fmtTypes.First();
                    if (fmtType.FormatString.Contains('{'))
                    {
                        formatString = String.Format(fmtType.FormatString, value);
                    }
                    else
                    {
                        formatString = fmtType.FormatString;
                    }
                }
            }
            else
            {
                formatString = value;
            }
            return formatString;
        }
    }
}
