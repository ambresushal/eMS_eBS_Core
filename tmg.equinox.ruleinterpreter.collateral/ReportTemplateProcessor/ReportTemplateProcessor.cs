using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpretercollateral
{
    public class ReportTemplateProcessor
    {
        JToken _masterListSource;
        Dictionary<string, JToken> _sources;
        string _repeaterString;
        public ReportTemplateProcessor(JToken masterListSource, Dictionary<string,JToken> sources,string repeaterString)
        {
            _masterListSource = masterListSource;
            _sources = sources;
            _repeaterString = repeaterString;
        }

        public string Process()
        {
            ReportTemplateBuilder builder = new ReportTemplateBuilder(_masterListSource, _sources);
            ReportTemplate template = builder.BuildReportTemplate(_repeaterString);
            string targetText = "";

            if (template.Rows.Count <= 0)
                targetText = " ";

            //iterate through top level rows
            foreach (var row in template.Rows)
            {
                RowTextGenerator gen = new RowTextGenerator(row, _sources, null,null);
                targetText = targetText + gen.ProcessRow();
            }
            return targetText;
        }
    }
}
