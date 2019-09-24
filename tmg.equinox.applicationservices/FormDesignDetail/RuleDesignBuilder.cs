using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.domain.entities.Models;
using System.Reflection;
using tmg.equinox.repository.interfaces;
using tmg.equinox.repository.extensions;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.MasterList;

namespace tmg.equinox.applicationservices.FormDesignDetail
{
    internal class RuleDesignBuilder
    {
        int formDesignVersionId;
        int formDesignId;
        int tenantId;
        List<UIElement> formElementList;
        IUnitOfWorkAsync _unitOfWork;
        List<DataSourceDesign> dataSources;
        List<FormDesignVersionUIElement> _frmDesignVersionElementList;
        internal RuleDesignBuilder(int tenantId, int formDesignVersionId, List<UIElement> formElementList, List<DataSourceDesign> dataSources, List<FormDesignVersionUIElement> frmDesignVersionElementList, IUnitOfWorkAsync unitOfWork)
        {
            this.formDesignVersionId = formDesignVersionId;
            this.tenantId = tenantId;
            this.formElementList = formElementList;
            this.dataSources = dataSources;
            this._unitOfWork = unitOfWork;
            this._frmDesignVersionElementList = frmDesignVersionElementList;
        }

        internal List<RuleDesign> GetRules()
        {

            List<RuleDesign> designs = new List<RuleDesign>();

            if (this._frmDesignVersionElementList != null)
            {
                var rptKeyFilters = this._unitOfWork.RepositoryAsync<RepeaterKeyFilter>().Get().ToList();
                var compOps = this._unitOfWork.RepositoryAsync<ComplexOperator>().Get().ToList();
                var tgtKeyFilter = this._unitOfWork.RepositoryAsync<TargetRepeaterKeyFilter>().Get().ToList();

                var rules = (from r in this._unitOfWork.RepositoryAsync<PropertyRuleMap>()
                                                             .Query()
                                                             .Include(c => c.Rule)
                                                             .Include(d => d.Rule.Expressions)
                                                            .Get()
                             join rfd in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>()
                                                     .Query()
                                                     .Get()
                             on r.UIElementID equals rfd.UIElementID
                             where rfd.FormDesignVersionID == formDesignVersionId
                             select new RuleDesign
                             {
                                 RuleID = r.RuleID,
                                 SuccessValue = r.Rule.ResultSuccess,
                                 FailureValue = r.Rule.ResultFailure,
                                 UIELementID = r.UIElementID,
                                 TargetPropertyTypeId = r.TargetPropertyID,
                                 IsResultFailureElement = r.Rule.IsResultFailureElement,
                                 IsResultSuccessElement = r.Rule.IsResultSuccessElement,
                                 Message = r.Rule.Message,
                                 Expressions = (from e in r.Rule.Expressions
                                                select new ExpressionDesign
                                                {
                                                    ExpressionId = e.ExpressionID,
                                                    LeftOperand = e.LeftOperand,
                                                    RightOperand = e.RightOperand == null ? "" : e.RightOperand,
                                                    IsRightOperandElement = e.IsRightOperandElement,
                                                    OperatorTypeId = e.OperatorTypeID,
                                                    LogicalOperatorTypeId = e.LogicalOperatorTypeID,
                                                    ExpressionTypeId = e.ExpressionTypeID,
                                                    ParentExpressionId = e.ParentExpressionID
                                                }),
                                 RunOnLoad = r.Rule.RunOnLoad
                             });
                if (rules != null && rules.Count() > 0)
                {
                    FormDesignVersionUIElement element = null;
                    foreach (var rule in rules)
                    {
                        //clean up the null expression for now
                        if (rule.Expressions != null && rule.Expressions.Count() > 0)
                        {
                            rule.Expressions = (from exp in rule.Expressions where exp != null select exp).ToList();

                            foreach (var exp in rule.Expressions)
                            {
                                exp.LeftKeyFilters = (from k in rptKeyFilters
                                                      where k.ExpressionID == exp.ExpressionId && k.IsRightOperand == false
                                                      select new RepeaterKeyFilterDesign
                                                      {
                                                          RepeaterKey = k.RepeaterKey,
                                                          RepeaterKeyValue = k.RepeaterKeyValue
                                                      }).ToList();

                                exp.RightKeyFilters = (from k in rptKeyFilters
                                                       where k.ExpressionID == exp.ExpressionId && k.IsRightOperand == true
                                                       select new RepeaterKeyFilterDesign
                                                       {
                                                           RepeaterKey = k.RepeaterKey,
                                                           RepeaterKeyValue = k.RepeaterKeyValue
                                                       }).ToList();

                                exp.complexOp = (from op in compOps
                                                 where op.ExpressionID == exp.ExpressionId
                                                 select new ComplexOperatorDesign
                                                 {
                                                     Factor = op.Factor,
                                                     FactorValue = op.FactorValue,
                                                 }).FirstOrDefault();
                            }
                        }
                        element = this._frmDesignVersionElementList.Where(x => x.UIElementID == rule.UIELementID).FirstOrDefault();
                        if (element != null)
                        {
                            rule.UIElementName = element.GeneratedName;
                            rule.UIElementFullName = element.UIElementFullName;
                            rule.UIElementFormName = element.UIElementName;
                            rule.UIElementTypeID = element.UIElementTypeID;
                            rule.TargetKeyFilters = (from tgt in tgtKeyFilter
                                                     where tgt.RuleID == rule.RuleID
                                                     select new RepeaterKeyFilterDesign
                                                     {
                                                         RepeaterKey = tgt.RepeaterKey,
                                                         RepeaterKeyValue = tgt.RepeaterKeyValue,
                                                     }).ToList();
                        }
                        if (rule.IsResultSuccessElement == true)
                        {
                            element = this._frmDesignVersionElementList.Where(x => x.UIElementName == rule.SuccessValue).FirstOrDefault();
                            if (element != null)
                                rule.SuccessValueFullName = element.UIElementFullName;
                            else
                                rule.SuccessValueFullName = "";
                        }
                        if (rule.IsResultFailureElement == true)
                        {
                            element = this._frmDesignVersionElementList.Where(x => x.UIElementName == rule.FailureValue).FirstOrDefault();
                            if (element != null)
                                rule.FailureValueFullName = element.UIElementFullName;
                            else
                                rule.FailureValueFullName = "";
                        }

                        if (rule.Expressions != null && rule.Expressions.Count() > 0)
                        {
                            foreach (var filter in rule.TargetKeyFilters)
                            {
                                element = this._frmDesignVersionElementList.Where(x => x.UIElementName == filter.RepeaterKey).FirstOrDefault();
                                if (element != null)
                                    filter.RepeaterKeyName = element.UIElementFullName;
                            }

                            foreach (var exp in rule.Expressions)
                            {
                                if (exp != null)
                                {
                                    element = this._frmDesignVersionElementList.Where(x => x.UIElementName == exp.LeftOperand).FirstOrDefault();
                                    if (element != null)
                                        exp.LeftOperandName = element.UIElementFullName;
                                    else
                                        exp.LeftOperandName = "";
                                    if (exp.IsRightOperandElement == true)
                                    {
                                        element = this._frmDesignVersionElementList.Where(x => x.UIElementName == exp.RightOperand).FirstOrDefault();
                                        if (element != null)
                                            exp.RightOperandName = element.UIElementFullName;
                                        else
                                            exp.RightOperandName = "";
                                    }

                                    if (exp.LeftKeyFilters.Count > 0)
                                    {
                                        for (int i = 0; i < exp.LeftKeyFilters.Count; i++)
                                        {
                                            element = this._frmDesignVersionElementList.Where(x => x.UIElementName == exp.LeftKeyFilters[i].RepeaterKey).FirstOrDefault();
                                            exp.LeftKeyFilters[i].RepeaterKeyName = element.UIElementFullName;
                                        }
                                    }
                                    if (exp.RightKeyFilters.Count > 0)
                                    {
                                        for (int i = 0; i < exp.RightKeyFilters.Count; i++)
                                        {
                                            element = this._frmDesignVersionElementList.Where(x => x.UIElementName == exp.RightKeyFilters[i].RepeaterKey).FirstOrDefault();
                                            exp.RightKeyFilters[i].RepeaterKeyName = element.UIElementFullName;
                                        }
                                    }
                                }
                            }

                        }
                        rule.IsParentRepeater = IsParentRepeater(rule.UIELementID);
                        if (rule.IsParentRepeater)
                        {
                            SetRepeaterPropsIfChildDSElement(rule);
                        }
                        rule.RootExpression = GenerateHierarchicalExpression(rule.Expressions);
                        designs.Add(rule);
                    }
                }
            }
            return designs;
        }

        private ExpressionDesign GenerateHierarchicalExpression(IEnumerable<ExpressionDesign> expressionList)
        {
            var expression = expressionList.Where(n => n != null && n.ParentExpressionId == null).FirstOrDefault();
            if (expression != null)
            {
                GenerateParentExpression(expressionList, ref expression);
            }
            return expression;
        }

        private void GenerateParentExpression(IEnumerable<ExpressionDesign> expressionList, ref ExpressionDesign parentExpression)
        {
            var parent = parentExpression;
            var childExpressions = from exp in expressionList where exp.ParentExpressionId == parent.ExpressionId select exp;
            if (childExpressions != null && childExpressions.Count() > 0)
            {
                parentExpression.Expressions = new List<ExpressionDesign>();
                foreach (var childExpression in childExpressions)
                {
                    ExpressionDesign child = childExpression;
                    GenerateParentExpression(expressionList, ref child);
                    parentExpression.Expressions.Add(childExpression);
                }
            }
        }
        private string GetGeneratedNameFromID(int elementID)
        {
            string generatedName = "";
            UIElement element = (from elem in this.formElementList
                                 where elem.UIElementID == elementID
                                 select elem).FirstOrDefault();
            generatedName = element.GeneratedName;
            return generatedName;
        }

        private string GetFullNameFromID(int elementID)
        {
            string fullName = "";
            UIElement element = (from elem in this.formElementList
                                 where elem.UIElementID == elementID
                                 select elem).FirstOrDefault();
            fullName = GetFullName(element);
            return fullName;
        }

        private string GetFormNameFromID(int elementID)
        {
            string formName = "";
            UIElement element = (from elem in this.formElementList
                                 where elem.UIElementID == elementID
                                 select elem).FirstOrDefault();
            formName = element.UIElementName;
            return formName;
        }

        private string GetFullNameFromName(string elementName)
        {
            string fullName = "";
            if (!String.IsNullOrEmpty(elementName))
            {
                UIElement element = (from elem in this.formElementList
                                     where elem.UIElementName == elementName
                                     select elem).FirstOrDefault();
                fullName = GetFullName(element);

            }
            return fullName;
        }

        private string GetFullName(UIElement element)
        {
            string fullName = "";
            if (element != null)
            {
                int currentElementID = element.UIElementID;
                int parentUIElementID = element.ParentUIElementID.HasValue ? element.ParentUIElementID.Value : 0;
                fullName = element.GeneratedName;
                while (parentUIElementID > 0)
                {
                    element = (from elem in formElementList
                               where elem.UIElementID == parentUIElementID
                               select elem).FirstOrDefault();
                    parentUIElementID = element.ParentUIElementID.HasValue ? element.ParentUIElementID.Value : 0;
                    if (parentUIElementID > 0)
                    {
                        fullName = element.GeneratedName + "." + fullName;
                    }
                }
            }
            return fullName;
        }

        private int GetUIElementType(int elementID)
        {
            int elementTypeId = 0;
            UIElement element = (from elem in this.formElementList
                                 where elem.UIElementID == elementID
                                 select elem).FirstOrDefault();
            if (element != null)
            {
                var props = element.GetType().GetProperties();
                var prop = from pro in props where pro.Name == "UIElementTypeID" select pro;
                if (prop != null && prop.Count() > 0)
                {
                    elementTypeId = (int)prop.First().GetValue(element);
                }
            }

            return elementTypeId;
        }

        private bool IsParentRepeater(int elementID)
        {
            bool isParentRepeater = false;
            UIElement element = (from elem in this.formElementList
                                 where elem.UIElementID == elementID
                                 select elem).FirstOrDefault();
            if (element != null)
            {
                if (element.UIElement2 is RepeaterUIElement)
                {
                    isParentRepeater = true;
                }
            }

            return isParentRepeater;
        }

        private void SetRepeaterPropsIfChildDSElement(RuleDesign rule)
        {
            //determine if element is inline or child
            var repeaterName = rule.UIElementFullName.Substring(0, rule.UIElementFullName.LastIndexOf('.'));
            string dsName = "";
            DataSourceDesign ds = null;
            if (this.dataSources != null && this.dataSources.Count > 0)
            {
                var dSrc = from dsr in this.dataSources where dsr.DisplayMode != "Primary" && dsr.Mappings.Any(s => s.TargetElement == rule.UIElementName) select dsr;
                if (dSrc != null && dSrc.Count() > 0)
                {
                    ds = dSrc.First();
                    dsName = ds.DataSourceName;
                    rule.UIElementFullName = rule.UIElementFullName.Insert(rule.UIElementFullName.LastIndexOf('.') + 1, dsName + ".");
                    rule.ParentRepeaterType = ds.DisplayMode;
                }
            }
            //determine if rule has to be run for row
            if (rule.ParentRepeaterType == null)
            {
                var expression = from exp in rule.Expressions where exp.LeftOperandName.Contains(repeaterName) select exp;
                if (expression != null && expression.Count() > 0)
                {
                    rule.RunForRow = true;
                }
                rule.RunForParentRow = false;
            }
            else
            {
                var childRepeaterName = repeaterName + "." + dsName;
                var elemNames = from dsr in ds.Mappings select dsr.TargetElement;
                bool runForRow = false;
                bool runForParentRow = false;
                foreach (var exp in rule.Expressions)
                {
                    string leftOperandName = exp.LeftOperandName;
                    var elemName = (from name in leftOperandName.Split('.') select name).Last();
                    if (leftOperandName.Contains(repeaterName))
                    {
                        if (elemNames.Contains(elemName))
                        {
                            exp.LeftOperandName = exp.LeftOperandName.Replace(repeaterName, childRepeaterName);
                            runForRow = true;
                        }
                        else
                        {
                            runForParentRow = true;
                        }
                    }
                    if (exp.IsRightOperandElement == true)
                    {
                        string rightOperandName = exp.RightOperandName;
                        var elemNameR = (from name in rightOperandName.Split('.') select name).Last();
                        if (rightOperandName.Contains(repeaterName))
                        {
                            if (elemNames.Contains(elemNameR))
                            {
                                exp.RightOperandName = exp.RightOperandName.Replace(repeaterName, childRepeaterName);
                                runForRow = true;
                            }
                            else
                            {
                                runForParentRow = true;
                            }
                        }
                    }
                }
                rule.RunForRow = runForRow;
                rule.RunForParentRow = runForParentRow;
            }
        }
    }
}
