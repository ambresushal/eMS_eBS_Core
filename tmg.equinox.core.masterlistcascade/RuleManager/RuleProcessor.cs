using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.infrastructure.util;
using tmg.equinox.ruleprocessor;

namespace tmg.equinox.ruleengine
{
    public abstract class RuleProcessor
    {
        protected JObject _formData;
        protected string _containerName;
        protected FormInstanceDataManager _formDataInstanceManager;
        protected int _formInstanceId;
        protected FormDesignVersionDetail _detail;
        protected string _sectionName;

        protected enum OperatorTypes
        {
            Equals = 1,
            GreaterThan = 2,
            LessThan = 3,
            Contains = 4,
            NotEquals = 5,
            GreaterThanOrEqualTo = 6,
            LessThanOrEqualTo = 7,
            IsNull = 8,
            NotContains = 10,
        }

        public enum LogicalOperatorTypes
        {
            AND = 1,
            OR = 2
        }

        public enum ExpressionTypes
        {
            NODE = 1,
            LEAF = 2
        }

        public enum TargetPropertyTypes
        {
            Enabled = 1,
            RunValidation = 2,
            Value = 3,
            Visible = 4,
            IsRequired = 5,
            Error = 6
        }

        //Process a rule object
        public abstract bool ProcessRule(RuleDesign rule);

        public bool ProcessNode(RuleDesign rule, ExpressionDesign rootExpression, JObject _rowData)
        {
            var exp = rootExpression.Expressions.Where(e => e.ExpressionTypeId != (int)ExpressionTypes.NODE).FirstOrDefault();
            string leftOperand = GetOperandValue(exp.LeftOperandName, exp.LeftOperand, _rowData);
            string rightOperand = exp.RightOperand;

            if (exp.IsRightOperandElement) { rightOperand = GetOperandValue(exp.RightOperandName, exp.RightOperand, _rowData); }

            if (rule.SuccessValue == "CheckInterval")
                return CheckInterval(leftOperand, rightOperand);
            if (rule.SuccessValue == "AdditionalDaysInterval")
                return AdditionalDaysInterval(leftOperand, rightOperand);

            return true;
        }

        private bool AdditionalDaysInterval(string leftOperand, string rightOperand)
        {
            decimal n1, n2;

            if (decimal.TryParse(leftOperand, out n1) && decimal.TryParse(rightOperand, out n2))
                return n2 == n1 + 90;

            return true;
        }
        private bool CheckInterval(string leftOperand, string rightOperand)
        {
            decimal n1, n2;

            if(decimal.TryParse(leftOperand, out n1) && decimal.TryParse(rightOperand, out n2))
                return  n2 == n1 + 1;

            return true;
        }   

        //Process list of expressions
        public bool ProcessNode(ExpressionDesign expression, JObject sourceData)
        {
            //loop through all the expression
            //Call ProcessLeaf to evaluate single expression
            bool isSuccess = expression.LogicalOperatorTypeId == (int)LogicalOperatorTypes.AND ? true : false;
            if (expression.Expressions != null && expression.Expressions.Count > 0)
            {
                for (var idx = 0; idx < expression.Expressions.Count; idx++)
                {
                    var exp = expression.Expressions[idx];
                    bool result = exp.ExpressionTypeId == (int)ExpressionTypes.NODE ? this.ProcessNode(exp, sourceData) : this.ProcessLeaf(exp, sourceData);

                    if (expression.LogicalOperatorTypeId == (int)LogicalOperatorTypes.AND)
                    {
                        if (result == false)
                        {
                            isSuccess = false;
                            break;
                        }
                    }
                    else
                    {
                        if (result == true)
                        {
                            isSuccess = true;
                            break;
                        }
                    }
                }
            }

            return isSuccess;
        }

        //Process an expressions
        public bool ProcessLeaf(ExpressionDesign expression, JObject sourceData)
        {
            string leftOperand = GetOperandValue(expression.LeftOperandName, expression.LeftOperand, sourceData);
            string rightOperand = expression.RightOperand;

            if (expression.IsRightOperandElement) { rightOperand = GetOperandValue(expression.RightOperandName, expression.RightOperand, sourceData); }
            return this.EvaluateExpression(leftOperand, expression.OperatorTypeId, rightOperand, expression.IsRightOperandElement, expression.LeftOperand);
        }

        //Evaluate single expression
        public bool EvaluateExpression(string leftOperand, int op, string rightOperand, bool isRightOperandElement, string uiElementId)
        {
            var result = false;

            if (op == (int)OperatorTypes.Equals)
            {
                result = ExpressionHelper.Equal(leftOperand, rightOperand);
            }
            else if (op == (int)OperatorTypes.GreaterThan)
            {
                result = ExpressionHelper.GreaterThan(leftOperand, rightOperand, uiElementId.IndexOf("Calendar") > 0);
            }
            else if (op == (int)OperatorTypes.GreaterThanOrEqualTo)
            {
                result = ExpressionHelper.GreaterThanOrEqual(leftOperand, rightOperand, uiElementId.IndexOf("Calendar") > 0);
            }
            else if (op == (int)OperatorTypes.LessThan)
            {
                result = ExpressionHelper.LessThan(leftOperand, rightOperand, uiElementId.IndexOf("Calendar") > 0);
            }
            else if (op == (int)OperatorTypes.LessThanOrEqualTo)
            {
                result = ExpressionHelper.LessThanOrEqual(leftOperand, rightOperand, uiElementId.IndexOf("Calendar") > 0);
            }
            else if (op == (int)OperatorTypes.Contains)
            {
                if (uiElementId.IndexOf("DropDown") > -1)
                {
                    if (leftOperand == "Select One")
                    {
                        leftOperand = "";
                    }
                }
                result = ExpressionHelper.Contains(leftOperand, rightOperand);
            }
            else if (op == (int)OperatorTypes.NotContains)
            {
                if (uiElementId.IndexOf("DropDown") > -1)
                {
                    if (leftOperand == "Select One")
                    {
                        leftOperand = "";
                    }
                }
                result = ExpressionHelper.NotContains(leftOperand, rightOperand);
            }
            else if (op == (int)OperatorTypes.NotEquals)
            {
                result = ExpressionHelper.NotEqual(leftOperand, rightOperand);
            }
            else if (op == (int)OperatorTypes.IsNull)
            {
                result = ExpressionHelper.IsNull(leftOperand);
            }
            return result;
        }

        public string GetOperandValue(string operandElementFullName, string operandElementName, JObject sourceData)
        {
            if (_sectionName != null && operandElementFullName.Split('.')[0] != _sectionName)
            {                
                sourceData = JObject.Parse(_formDataInstanceManager.GetSectionData(_formInstanceId, operandElementFullName.Split('.')[0], false, _detail, false, false));
            }

            string value = null;
            if (!string.IsNullOrEmpty(_containerName) && operandElementFullName.Contains(_containerName) && sourceData != null)
            {
                string elementName = operandElementFullName.Replace(_containerName + ".", "");
                string[] nameParts = elementName.Split('.');
                JToken dataPart = null;
                for (var idx = 0; idx < nameParts.Length; idx++)
                {
                    if (idx == 0)
                    {
                        if (sourceData[nameParts[idx]] != null)
                        {
                            dataPart = sourceData[nameParts[idx]];
                        }
                    }
                    else if (idx == (nameParts.Length - 1))
                    {
                        if (sourceData[nameParts[idx]] != null)
                        {
                            dataPart = sourceData[nameParts[idx]];
                        }
                    }
                    else
                    {
                        dataPart = dataPart[nameParts[idx]];
                    }
                }
                value = dataPart == null ? "" : Convert.ToString(dataPart);
            }
            else
            {
                JToken dataPart = null;
                var nameParts = operandElementFullName.Split('.');
                for (var idx = 0; idx < nameParts.Length; idx++)
                {
                    if (idx == 0)
                    {
                        dataPart = sourceData[nameParts[idx]];
                    }
                    else
                    {
                        if (dataPart != null &&  dataPart.SelectToken(nameParts[idx]) != null)
                        {
                            dataPart = dataPart[nameParts[idx]];
                        }
                    }
                }
                value = dataPart == null ? "" : Convert.ToString(dataPart);
            }

            if (operandElementName != null)
            {
                if (operandElementName.IndexOf("CheckBox") > 0 || operandElementName.IndexOf("Radio") > 0)
                {
                    value = value == "true" || value == "True" || value == "Yes" || value == "yes" ? "Yes" : "No";
                }
            }

            if (!string.IsNullOrEmpty(value) && HtmlContentHelper.IsHTML(value))
                value = HtmlContentHelper.GetFreeFromHtmlText(value);

            return value;
        }
    }
}