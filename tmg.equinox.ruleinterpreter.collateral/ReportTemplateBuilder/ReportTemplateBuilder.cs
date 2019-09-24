using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpretercollateral
{
    public class ReportTemplateBuilder
    {
        private JToken _templateContainer;
        private JArray _containerRows;
        private Dictionary<string, ReportTemplateRowSource> _resolvedSources;
        private Dictionary<string, JToken> _sources;
        public ReportTemplateBuilder(JToken templateContainer,Dictionary<string,JToken> sources)
        {
            _templateContainer = templateContainer;
            _sources = sources;
            _resolvedSources = new Dictionary<string, ReportTemplateRowSource>();
        }

        public ReportTemplate BuildReportTemplate(string templateContainerName)
        {
            ReportTemplate template = new ReportTemplate();

            _containerRows = (JArray)_templateContainer.SelectToken(templateContainerName);
            if(_containerRows != null && _containerRows.Count > 0)
            {
                bool hasKeyProperty = false;
                JObject keySource = (JObject)_containerRows.First();
                foreach (JProperty property in keySource.Properties())
                {
                    if (property.Name == "Key")
                    {
                        hasKeyProperty = true;
                        break;
                    }
                }
                if (hasKeyProperty)
                {
                    ExpressionParser parser = new ExpressionParser(_sources);
                    List<ExpressionTemplate> templates = parser.ParseExpressions(_containerRows, "");
                    templates = templates.Where(a => !a.TemplateExp.StartsWith("STATIC") && !a.TemplateExp.StartsWith("CUSTOM") && a.TemplateExp != "").ToList();
                    List<string> matches = parser.EvaluateExpressions(templates);
                    JArray arr = new JArray();
                    var filteredRows = _containerRows.Where(a => matches.Contains(a["Key"].ToString()) || a["Key"].ToString().StartsWith("CUSTOM") || a["Key"].ToString().StartsWith("STATIC") || a["Key"].ToString() == "").ToArray();
                    foreach(var row in filteredRows)
                    {
                        arr.Add(row);
                    }
                    _containerRows = arr;
                }
                if(_containerRows.Count > 0)
                {
                    var topLevelRows = _containerRows.Where(a => String.IsNullOrEmpty(a["Parent"].ToString()));
                    foreach (var row in topLevelRows)
                    {
                        ReportTemplateRow newRow = AddRow(row, null);
                        template.Rows.Add(newRow);
                    }
                }
            }
            return template;
        }

        private ReportTemplateRow AddRow(JToken row, ReportTemplateRow parentRow)
        {
            ReportTemplateRow newRow = new ReportTemplateRow();
            if (row["Parent"] != null)
            {
                string parent = row["Parent"].ToString();
                newRow.HasParent = String.IsNullOrEmpty(parent) ? false : true;
            }
            newRow.Parent = parentRow;
            if(row["RichText"] != null)
            {
                string richText = row["RichText"].ToString();
                newRow.RichText = String.IsNullOrEmpty(richText) ? "" : richText;
            }
            if(row["Type"] != null)
            {
                string type = row["Type"].ToString();
                type = String.IsNullOrEmpty(type) ? "" : type;
                switch (type)
                {
                    case "List":
                        newRow.RowType = ReportTemplateRowType.LIST;
                        break;
                    default:
                        newRow.RowType = ReportTemplateRowType.SCALAR;
                        break;
                }
            }
            if(row["TemplateName"] != null)
            {
                newRow.TemplateRowName = row["TemplateName"].ToString();
            }
            if (row["Title"] != null)
            {
                newRow.Title = row["Title"].ToString();
            }
            newRow.Sources = new List<ReportTemplateRowSource>();
            if(row["Sources"] != null)
            {
                string sources = row["Sources"].ToString();
                string[] sourceArr = sources.Split(',');
                string repeatFor = "";
                string sourceAction = string.Empty;
                if(row["RepeatFor"] != null)
                {
                    repeatFor = row["RepeatFor"].ToString();
                }

                if (row["SourceAction"] != null)
                {
                    sourceAction = row["SourceAction"].ToString();
                }

                foreach (var source in sourceArr)
                {
                    ReportTemplateRowSource sourceRow = null;
                    ReportTemplateRowSourceBuilder builder = new ReportTemplateRowSourceBuilder(source, _templateContainer, repeatFor, _sources,sourceAction);
                    sourceRow = builder.BuildSource();
                    newRow.Sources.Add(sourceRow);
                }                
            }
            if(row["FilterExpression"] != null)
            {
                string filterExpression = row["FilterExpression"].ToString();
                if (!String.IsNullOrEmpty(filterExpression))
                {
                    ReportTemplateRowFilter filter = new ReportTemplateRowFilter();
                    filter.Expression = filterExpression;
                    IReportExpressionResolver resolver = new ReportExpressionResolver();
                    filter.FilterTemplate = resolver.GetExpressionTemplate(filterExpression);
                    newRow.Filter = filter;
                }
            }
            var childRows = _containerRows.Where(a => a["Parent"].ToString() == newRow.TemplateRowName && !string.IsNullOrEmpty(newRow.TemplateRowName));
            if(childRows != null && childRows.Count() > 0)
            {
                newRow.ChildRows = new List<ReportTemplateRow>();
                foreach(var childRow in childRows)
                {
                    ReportTemplateRow newChildRow = AddRow(childRow, newRow);
                    newRow.ChildRows.Add(newChildRow);
                }
            }
            return newRow;
        }
    }
}
