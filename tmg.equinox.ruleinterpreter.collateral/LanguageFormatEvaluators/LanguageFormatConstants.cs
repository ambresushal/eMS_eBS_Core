using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpretercollateral
{
  public static class LanguageFormatConstants
    {
        public const char FormatKeyTypeSplitter ='/';
        public const string ValueRegex = @"\[(.*?)\]";
        public const char FormatValueSplitter = ',';
        public const string FormTypeTokenPath = "FormatType";
        public const string FormStringTokenPath = "FormatString";
    }
}
