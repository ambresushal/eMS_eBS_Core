using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using tmg.equinox.infrastructure.exceptionhandling;

namespace tmg.equinox.ruleinterpretercollateral
{
    public class RowTextGenerator
    {

        ReportTemplateRow _row;
        Dictionary<string, JToken> _sources;
        List<JToken> _iterationList;
        string _insertContainer;
        public RowTextGenerator(ReportTemplateRow row, Dictionary<string, JToken> sources, List<JToken> iterationList, string insertContainer)
        {
            _row = row;
            _sources = sources;
            _iterationList = iterationList;
            _insertContainer = insertContainer;
        }

        public string ProcessRow()
        {
            string resultRichText = "";
            string templatePattern = _row.TemplateRowName.Replace("[", @"\[").Replace("]", @"\]").Replace(@"\\", @"\");
            if (String.IsNullOrEmpty(_insertContainer) == true)
            {
                resultRichText = _row.RichText;
                _insertContainer = _row.RichText;
            }
            else
            {
                resultRichText = _insertContainer == null ? "" : _insertContainer;
            }
            string pattern = @"<!--" + templatePattern + @"-->.*<!--" + templatePattern + @"-->";
            MatchCollection coll = Regex.Matches(resultRichText, pattern, RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
            string templateStr = "";
            if (coll.Count > 0)
            {
                Match col = coll[0];
                templateStr = col.ToString();
                resultRichText = col.ToString().Replace("<!--" + _row.TemplateRowName + "-->", "");
            }

            RowIterator iterator = GetRowIterator();

            switch (iterator.IterationType)
            {
                case RowIterationType.GROUPBYSTRING:
                    ProcessGroupStringTemplate(iterator, ref resultRichText);
                    break;
                case RowIterationType.ROW:
                    ProcessRowTemplate(iterator, ref resultRichText);
                    break;

            }
            if ((iterator.IterationType == RowIterationType.GROUPBYSTRING && (iterator.GroupStringResult != null && iterator.GroupStringResult.Count == 0)))
            {
                resultRichText = "   ";
            }
            else
            {
                if (String.IsNullOrEmpty(templateStr) == false)
                {
                    resultRichText = _insertContainer.Replace(templateStr, resultRichText);
                }
            }
            return resultRichText;
        }

        private RowIterator GetRowIterator()
        {
            RowIterator iterator = new RowIterator();
            if (_row.Sources.Count > 0)
            {
                ReportTemplateRowSource first = _row.Sources.First();
                List<string> groupParams = null;
                if (first.Source!=null && first.Source.Count()>0 && first.RepeatFor.ExpressionType == RepeatForExpressionType.GROUP)
                {
                    groupParams = GetGroupParams(first.RepeatFor.Expression);
                    if (groupParams.Count == 1)
                    {
                        iterator.IterationType = RowIterationType.GROUPBYSTRING;
                    }
                    if (groupParams.Count > 1)
                    {
                        iterator.IterationType = RowIterationType.GROUPBYOBJECT;
                    }
                }
                else
                {
                    iterator.IterationType = RowIterationType.ROW;
                }
                SetIteration(first, groupParams, _row.Filter, ref iterator);
            }
            return iterator;
        }

        private List<string> GetGroupParams(string expression)
        {
            List<string> groupParams = new List<string>();
            string[] parts = expression.Split('[');
            if (parts.Length > 1)
            {
                string fields = parts[1].Replace("]", "");
                groupParams = fields.Split(',').ToList();
            }
            return groupParams;
        }

        private void SetIteration(ReportTemplateRowSource source, List<string> groupParams, ReportTemplateRowFilter filter, ref RowIterator iterator)
        {


            if (filter != null)
            {
                if (source.Source != null)
                {
                    JToken result = JToken.Parse("[]");
                    foreach (var row in source.Source)
                    {
                        int matchCount = 0;
                        DocumentAlias varb = new DocumentAlias();

                        foreach (DocumentAlias filterVar in filter.FilterTemplate.Variables)
                        {
                            varb = filterVar;
                            bool isAliasPath = false;
                            //TODO: Handled aliases for all operators in optimized way
                            if (!String.IsNullOrEmpty(filterVar.DocumentDesignName))
                            {
                                varb = GetActualValues(filterVar);
                                isAliasPath = true;
                            }

                            switch (varb.Operator)
                            {
                                case "=":

                                    if (isAliasPath && varb.ActualValue == varb.Value)
                                    {
                                        matchCount++;
                                    }

                                    else if (isAliasPath == false && row[varb.Alias].ToString() == varb.Value)
                                    {
                                        matchCount++;
                                    }

                                    break;
                                case "!=":

                                    if (isAliasPath && varb.ActualValue != varb.Value)
                                    {
                                        matchCount++;
                                    }

                                    else if (isAliasPath == false && row[varb.Alias].ToString() != varb.Value)
                                    {
                                        matchCount++;
                                    }

                                    break;
                                case "Ɐ":
                                    {
                                        try
                                        {
                                            //Curretly supported only One
                                            if (varb.ActualValues != null && varb.ActualValues.Contains(varb.Values[0]) == true)
                                            {
                                                matchCount++;
                                            }
                                        }
                                        catch (Exception ex)
                                        {

                                            throw;
                                        }


                                        break;
                                    }
                            }

                        }
                        if (matchCount == filter.FilterTemplate.Variables.Count)
                        {
                            ((JArray)result).Add(row);
                        }
                    }
                    if (result != null)
                    {
                        source.Source = result;
                        //   _iterationList = null;
                    }
                }
            }

            if (iterator.IterationType == RowIterationType.GROUPBYSTRING)
            {
                bool hasKeyProperty = false;
                JObject keySource = (JObject)source.Source.FirstOrDefault();
                if (keySource != null && keySource.Count > 0)
                {
                    foreach (JProperty property in keySource.Properties())
                    {
                        if (property.Name == "Key")
                        {
                            hasKeyProperty = true;
                            break;
                        }
                    }
                }
                if (hasKeyProperty)
                {
                    ExpressionParser parser = new ExpressionParser(_sources);
                    List<ExpressionTemplate> templates = parser.ParseExpressions(source.Source, "");
                    templates = templates.Where(a => !a.TemplateExp.StartsWith("STATIC") && !a.TemplateExp.StartsWith("CUSTOM")).ToList();
                    List<string> matches = parser.EvaluateExpressions(templates);

                    var rows = source.Source.Where(a => matches.Contains(a["Key"].ToString()) || a["Key"].ToString().StartsWith("CUSTOM") || a["Key"].ToString().StartsWith("STATIC")).ToList();
                    if (rows != null && rows.Count > 0)
                    {
                        if (rows[0]["SequenceNo"] != null)
                        {
                            iterator.GroupStringResult = rows.OrderBy(a => a, new SequenceComparer()).GroupBy(a => a[groupParams[0]]).ToList();
                        }
                        else
                        {
                            iterator.GroupStringResult = rows.OrderBy(a => GetNonHtmlText(a[groupParams[0]].ToString())).GroupBy(a => a[groupParams[0]]).ToList();
                        }
                    }
                }
                else
                {
                    if (source.Source != null && source.Source.Count() > 0)
                    {
                        if (source.Source[0]["SequenceNo"] != null)
                        {
                            iterator.GroupStringResult = source.Source.OrderBy(a => a, new SequenceComparer()).GroupBy(a => a[groupParams[0]]).ToList();
                        }
                        else
                        {
                            iterator.GroupStringResult = source.Source.OrderBy(a => GetNonHtmlText(a[groupParams[0]].ToString())).GroupBy(a => a[groupParams[0]]).ToList();
                        }
                    }
                }
            }
            if (iterator.IterationType == RowIterationType.ROW)
            {
                if (_iterationList == null)
                {
                    iterator.RowResult = source.Source == null ? null : source.Source.OrderBy(a => a, new SequenceComparer()).ToList();
                }
                else
                {
                    iterator.RowResult = _iterationList;
                }
            }
        }

        private void ProcessGroupStringTemplate(RowIterator iterator, ref string resultRichText)
        {
            List<IGrouping<JToken, JToken>> rows = iterator.GroupStringResult;
            string result = "";
            if (rows != null && rows.Count !=0)
            {
                foreach (var row in rows)
                {
                    PlaceHolderProcessor processor = new PlaceHolderProcessor(resultRichText, row.ToList(), _sources);
                    string richText = processor.Process();
                    if (_row.ChildRows != null)
                    {
                        foreach (ReportTemplateRow childRow in _row.ChildRows)

                        {
                            try
                            {
                                RowTextGenerator childGen = new RowTextGenerator(childRow, _sources, row.ToList(), richText);
                                richText = childGen.ProcessRow();
                            }
                            catch (Exception ex)
                            {
                                string customMsg = "Row :" + childRow.TemplateRowName + " -- for Parent Row : " + row.Key.ToString() + " could not be generated.";
                                Exception customException = new Exception(customMsg, ex);
                                ExceptionPolicyWrapper.HandleException(customException, ExceptionPolicies.ExceptionShielding);
                            }
                        }
                    }
                    result = result + richText;
                }
            }
            resultRichText = result;
        }

        private void ProcessRowTemplate(RowIterator iterator, ref string resultRichText)
        {
            List<JToken> rows = iterator.RowResult;
            if(rows != null && rows.Count > 0)
            {

                bool isLeaf = false;
                bool hasLanguage = false;
                if (_row.Sources.Where(a => a.SourceType == ReportTemplateRowSourceType.Language).Count() > 0 || (_row.ChildRows == null && _row.HasParent == true))
                {
                    isLeaf = true;
                    if (_row.Sources.Where(a => a.SourceType == ReportTemplateRowSourceType.Language).Count() > 0)
                    {
                        hasLanguage = true;
                    }
                }
                string phRichText = "";
                if (isLeaf == true)
                {
                    if (hasLanguage == true)
                    {
                        ReportTemplateRowSource languageSource = _row.Sources.Where(a => a.SourceType == ReportTemplateRowSourceType.Language).First();
                        LanguagePlaceHolderResolver langEval = new LanguagePlaceHolderResolver(languageSource, _sources, rows);
                        phRichText = langEval.Process();
                        resultRichText = resultRichText.Replace("<!--" + _row.TemplateRowName + "-->", _row.TemplateRowName);
                        resultRichText = resultRichText.Replace(_row.TemplateRowName, phRichText);
                    }
                    else
                    {
                        ReportTemplateRowSource dataSource = _row.Sources.First();
                        DataRowsResolver dataEval = new DataRowsResolver(_row, dataSource, _sources, rows);
                        phRichText = dataEval.Process();
                        resultRichText = resultRichText.Replace("<!--" + _row.TemplateRowName + "-->", _row.TemplateRowName);
                        resultRichText = resultRichText.Replace(_row.TemplateRowName, phRichText);
                    }
                }
                else
                {
                    string result = "";
                    foreach (var row in rows)
                    {
                        string richText = resultRichText;
                        List<JToken> rowList = new List<JToken>();
                        rowList.Add(row);
                        PlaceHolderProcessor processor = new PlaceHolderProcessor(resultRichText, rowList, _sources);
                        richText = processor.Process();
                        foreach (ReportTemplateRow childRow in _row.ChildRows)
                        {
                            if (row is JToken)
                            {
                                RowTextGenerator childGen = new RowTextGenerator(childRow, _sources, rowList, richText);
                                richText = childGen.ProcessRow();
                            }
                            else
                            {
                                RowTextGenerator childGen = new RowTextGenerator(childRow, _sources, row.ToList(), richText);
                                richText = childGen.ProcessRow();
                            }
                        }
                        result = result + richText;
                    }
                    resultRichText = result;
                }

            }
            else
            {
                resultRichText = " ";
            }
        }

        //Duplicate code , should use the Expression parser.
        private DocumentAlias GetActualValues(DocumentAlias filterVar)
        {
            var sourceAliases = _sources["DesignAliases_" + filterVar.DocumentDesignName];
            string path = "";
            var al = sourceAliases.Where(a => a["Alias"].ToString() == filterVar.Alias);
            if (al != null)
            {
                var firstAl = al.First();
                path = firstAl["ElementPath"].ToString();
            }

            if (!String.IsNullOrEmpty(path))
            {
                var tok = _sources[filterVar.DocumentDesignName].SelectToken(path);
                if (tok is JArray)
                {
                    filterVar.ActualValues = GetValues(tok.ToString());
                }
                else
                {
                    filterVar.ActualValue = tok != null ? tok.ToString() : string.Empty;
                }

            }
            return filterVar;
        }

        private string[] GetValues(string value)
        {
            string[] values = null;
            value = value.Trim(new char[] { '[', '\r', '\n', ']' });
            value = value.Replace("\"", "");
            values = value.Split(',');
            for (int idx = 0; idx < values.Length; idx++)
            {
                values[idx] = values[idx].Trim();
            }
            return values;
        }

        private string GetNonHtmlText(string groupText)
        {
            string resultText = string.Empty;
            HtmlNode groupHtmlNode = HtmlNode.CreateNode(groupText);
            resultText = (!string.IsNullOrEmpty(groupText) && groupHtmlNode != null) ? groupHtmlNode.InnerText : string.Empty;

            return resultText;
        }


    }
}

