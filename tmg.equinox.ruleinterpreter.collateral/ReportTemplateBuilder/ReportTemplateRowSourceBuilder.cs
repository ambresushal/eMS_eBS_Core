using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpretercollateral
{
    public class ReportTemplateRowSourceBuilder
    {
        private string _sourceName;
        private JToken _templateSourceContainer;
        private string _repeatFor;
        private Dictionary<string, JToken> _sources;
        private string _sourceAction;

        public ReportTemplateRowSourceBuilder(string sourceName, JToken templateSourceContainer, string repeatFor, Dictionary<string, JToken> sources, string sourceAction)
        {
            _sourceName = sourceName;
            _templateSourceContainer = templateSourceContainer;
            _repeatFor = repeatFor;
            _sources = sources;
            _sourceAction = sourceAction;
        }

        public ReportTemplateRowSource BuildSource()
        {
            ReportTemplateRowSource source = new ReportTemplateRowSource();
            JToken reportData = null;
            bool isReportData = false;
            if (_sourceName.Contains("]"))
            {
                _sourceName = _sourceName.Replace("]", "");
                string sourceAlice = _sourceName.Split('[')[0];
                string sourcePath = _sourceName.Split('[')[1];
                reportData = _sources[sourceAlice].SelectToken(sourcePath);
                if (!string.IsNullOrEmpty(_sourceAction) && _sourceAction.Contains("["))
                {
                    SourceActionFormatter formatter = new SourceActionFormatter(_templateSourceContainer, reportData, _sourceAction);
                    reportData = formatter.FormatSource();
                }
                source.Source = reportData;
            }
            else
            {
                reportData = _templateSourceContainer.SelectToken(_sourceName.Replace("-", ""));
                isReportData = true;
            }


            if (reportData != null && reportData.Count() >0)
            {
                source.RepeatFor = new RepeatForExpression();
                source.RepeatFor.Expression = _repeatFor;
                if (_repeatFor.StartsWith("GROUP", StringComparison.OrdinalIgnoreCase) == true)
                {
                    source.RepeatFor.ExpressionType = RepeatForExpressionType.GROUP;
                }
                if (_repeatFor.StartsWith("ROW", StringComparison.OrdinalIgnoreCase) == true)
                {
                    source.RepeatFor.ExpressionType = RepeatForExpressionType.ROW;
                }
                if (isReportData)
                    source.Source = null;
                source.SourceName = _sourceName;
                if (_sourceName.StartsWith("REPORTDATA", StringComparison.OrdinalIgnoreCase) == true)
                {
                    source.SourceType = ReportTemplateRowSourceType.Data;
                    ReportDataResolver resolver = new ReportDataResolver(reportData, _sources);
                    source.Source = resolver.ResolveData();
                }
                if (_sourceName.StartsWith("Language", StringComparison.OrdinalIgnoreCase) == true)
                {
                    source.SourceType = ReportTemplateRowSourceType.Language;
                    source.Source = reportData;
                }
            }
            return source;
        }

    }

}
