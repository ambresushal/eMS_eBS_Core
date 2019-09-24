using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpretercollateral
{
    public class LanguageFormatParser
    {
        JToken _source;
        public LanguageFormatParser(JToken source)
        {
            _source = source;
        }

        public List<LanguageFormats> GetLanguageFormats()
        {
            List<LanguageFormats> langFormats = new List<LanguageFormats>();
            foreach (var item in _source)
            {
                JToken formatType = item[LanguageFormatConstants.FormTypeTokenPath];
                JToken formatString = item[LanguageFormatConstants.FormStringTokenPath];

                if (formatType!=null)
                {
                    LanguageFormats format = new LanguageFormats();
                    format.FormatKeywordType = formatType.ToString();
                    format.FormatString = formatString != null ? formatString.ToString() : string.Empty;
                    langFormats.Add(format);
                }
            }
            return langFormats;
        }
    }
}
