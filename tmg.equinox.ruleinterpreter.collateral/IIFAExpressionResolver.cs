using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpretercollateral
{
    public class IIFAExpressionResolver : IReportExpressionResolver
    {
        public DocumentAliases GetAliases(List<string> expressions)
        {
            throw new NotImplementedException();
        }

        public ExpressionTemplate GetExpressionTemplate(string expression)
        {
            ExpressionTemplate template = new ExpressionTemplate();
            template.TemplateExp = expression;
            template.Template = "[";
            template.Variables = new List<DocumentAlias>();
            expression = expression.Remove(0, 1);

            if (!String.IsNullOrEmpty(expression))
            {
                char[] operators = new char[] { '=', '!', '>', '<', '!', '₳', 'Ɐ', '^' };
                char[] logicalOps = new char[] { '&', '|' };

                string[] exps = expression.Split(logicalOps);
                if (exps.Length > 0)
                {
                    template.LogicalOperator = exps.Length > 1 ? expression.Substring(exps[0].Length, 1) : "&";
                    int index = 0;
                    foreach (string condition in exps)
                    {
                        string[] cond = condition.Split(operators);
                        if (cond.Length >= 2)
                        {
                            string leftOp = cond[0];
                            string rigtOp = cond.Length == 2 ? cond[1] : cond[2];
                            DocumentAlias docAlias = new DocumentAlias();
                            docAlias.DocumentDesignName = leftOp.StartsWith("[")|| !leftOp.Contains("[") ? "" : leftOp.Substring(0, leftOp.IndexOf("["));
                            Match aliasMatch = Regex.Match(leftOp, @"\[(\w+)\]");
                            if (aliasMatch.Success)
                            {
                                docAlias.Alias = aliasMatch.Groups[1].Value;
                            }
                            docAlias.ValueType = GetValueType(rigtOp);
                            if (docAlias.ValueType == ValueType.Single)
                            {
                                docAlias.Value = rigtOp;
                            }
                            else
                            {
                                docAlias.Values = GetValues(rigtOp);
                            }
							if(string.IsNullOrEmpty(docAlias.DocumentDesignName) && string.IsNullOrEmpty(docAlias.Alias))
                            {
                                if (!string.IsNullOrEmpty(leftOp))
                                {
                                    docAlias.ActualValue = leftOp.Trim();
                                }
                            }
                            docAlias.Operator = condition.Substring(cond[0].Length, cond.Length == 2 ? 1 : 2);
                            template.Variables.Add(docAlias);
                            template.Template = template.Template + (leftOp + docAlias.Operator + "{" + index + "}");
                            if (index < exps.Length - 1)
                            {
                                template.Template += template.LogicalOperator;
                            }
                            index = index + 1;
                        }
                    }
                }

                template.IsValid = true;
                foreach (var varb in template.Variables)
                {
                    varb.IsValid = true;
                    if (String.IsNullOrEmpty(varb.DocumentDesignName) || String.IsNullOrEmpty(varb.Alias) || String.IsNullOrEmpty(varb.Operator))
                    {
                        varb.IsValid = false;
                        template.IsValid = false;
                    }

                }
            }
            return template;
        }

        private ValueType GetValueType(string value)
        {
            ValueType valueType = ValueType.Single;
            if (!String.IsNullOrEmpty(value))
            {
                if (value.StartsWith("[") && value.EndsWith("]"))
                {
                    valueType = ValueType.Array;
                }
            }
            return valueType;
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
    }
}
