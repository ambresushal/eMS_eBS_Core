using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpretercollateral
{
    public class RegexConstants
    {
        //complex functions with 3 characters for function name - up to the next function
        public static string MultipleComplexFunctionRegex = @"{{(IIF|IIFA):[\w\W]*(?={{[a-zA-Z]{3})";
        //complex functions with 3 characters for function name
        public static string ComplexFunctionRegex = @"{{(IIF|IIFA):[\w\W]*]}}";
        //simple function
        public static string SimpleFunctionRegex = @"{{(CS|CSMIN|CSMAX|FMT):\[.+?(?=]}})]}}";
        //field placeholder
        public static string FieldRegex = @"{{[a-zA-Z0-9]*\[[\w.@><=!\s\&\-\;\:\(\)]*\]}}";
        //alias placeholder
        public static string AliasRegex = @"{{[a-zA-Z0-9]*\[[a-zA-Z0-9]*]}}";
        //internal field placeholder
        public static string InternalRegex = @"{{:[a-zA-Z0-9]*}}";

        public static string FMT_PN = @"\(([\w\d\-\s]+)\)";
        public static string FMT_PHF = @"\(([\w\d]+)\)";
        public static string FMT_PHXT = @"\(([\w\d]+)\)-";

        public static string WrappedComplexFunctionRegex = @"\<![ \r\n\t]*(?>--([^\-]|[\r\n]|-[^\-])*--[ \r\n\t]*)\>";
    }
}
