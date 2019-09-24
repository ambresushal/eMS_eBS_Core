using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpretercollateral
{
    public class ReportExpressionResolver : IReportExpressionResolver
    {
        public DocumentAliases GetAliases(List<string> expressions)
        {
            throw new NotImplementedException();
        }

        public ExpressionTemplate GetExpressionTemplate(string expression)
        {
            ExpressionTemplate template = new ExpressionTemplate();
            template.TemplateExp = expression;
            template.Variables = new List<DocumentAlias>();
            if (!String.IsNullOrEmpty(expression))
            {
                char[] operators = new char[] { '=','!','>','<','!', '₳', 'Ɐ','^' };
                char[] logicalOps = new char[] { '&', '|' };
                char aliasStartChar = '[';
                char aliasEndChar = ']';
                

                string outputExp = "";
                int idx = 0;
                bool operatorStart = false;
                bool logicalOpStart = false;
                bool aliasRangeStart = false;

                int phIdx = 0;
                string designName = "";
                string alias = "";
                string value = "";
                string operatorType = "";
                for(idx=0;idx<expression.Length;idx++)
                {
                    char c = expression[idx];
                    if (aliasStartChar == c && operatorStart == false)
                    {
                        aliasRangeStart = true;
                    }
                    if (aliasEndChar == c && operatorStart == false)
                    {
                        aliasRangeStart = false;
                    }

                    if (operators.Contains(c) || logicalOps.Contains(c))
                    {
                        if (operators.Contains(c))
                        {
                            outputExp = outputExp + c;
                            operatorStart = true;
                            operatorType = operatorType + c;
                            logicalOpStart = false;
                        }
                        if (logicalOps.Contains(c))
                        {
                            if (String.IsNullOrEmpty(template.LogicalOperator))
                            {
                                template.LogicalOperator = c.ToString();
                            }
                            DocumentAlias docAlias = new DocumentAlias();
                            docAlias.DocumentDesignName = designName;
                            docAlias.Alias = alias;
                            docAlias.ValueType = GetValueType(value);
                            if(docAlias.ValueType == ValueType.Single)
                            {
                            docAlias.Value = value;
                            }
                            else
                            {
                                docAlias.Values = GetValues(value);
                            }
                            docAlias.Operator = operatorType;
                            operatorType = "";
                            template.Variables.Add(docAlias);
                            designName = "";
                            alias = "";
                            value = "";
                            operatorStart = false;
                            logicalOpStart = true;
                            outputExp = outputExp + "{" + phIdx + "}";
                            outputExp = outputExp + c;
                            phIdx++;
                        }
                    }
                    else
                    {
                        if(operatorStart == false)
                        {
                            outputExp = outputExp + c;
                        }
                        if(aliasRangeStart == false && operatorStart == false && c != aliasEndChar)
                        {
                            designName = designName + c;
                        }
                        if (aliasRangeStart == true && c != aliasStartChar)
                        {
                            alias = alias + c;
                        }
                        if (operatorStart == true && logicalOpStart == false)
                        {
                            value = value + c;
                        }

                    }
                }
                DocumentAlias docAliasLast = new DocumentAlias();
                docAliasLast.DocumentDesignName = designName;
                docAliasLast.Alias = alias;
                docAliasLast.ValueType = GetValueType(value);
                if (docAliasLast.ValueType == ValueType.Single)
                {
                docAliasLast.Value = value;
                }
                else
                {
                    docAliasLast.Values = GetValues(value);
                }
                docAliasLast.Operator = operatorType;
                template.Variables.Add(docAliasLast);
                outputExp = outputExp + "{" + phIdx + "}";
                template.Template = outputExp;
                template.IsValid = true;
                foreach(var varb in template.Variables)
                {
                    varb.IsValid = true;
                    if(String.IsNullOrEmpty(varb.DocumentDesignName) || String.IsNullOrEmpty(varb.Alias) || String.IsNullOrEmpty(varb.Operator)) 
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
                if(value.StartsWith("[") && value.EndsWith("]"))
                {
                    valueType = ValueType.Array;
                }
            }
            return valueType;
        }

        private string[] GetValues(string value)
        {
            string[] values = null;
            value = value.Trim(new char[] { '[', '\r', '\n',']'});
            value = value.Replace("\"", "");
            values = value.Split(',');
            for(int idx=0;idx<values.Length;idx++)
            {
                values[idx] = values[idx].Trim();
            }
            return values;
        }
    }
}
