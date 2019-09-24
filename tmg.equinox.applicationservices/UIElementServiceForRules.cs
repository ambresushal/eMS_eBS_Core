using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.UIElement;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;
using tmg.equinox.repository.extensions;
using System.Transactions;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.applicationservices.viewmodels.FormContent;

namespace tmg.equinox.applicationservices
{
    public partial class UIElementService : IUIElementService
    {
        #region Private Members

        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor

        #endregion Constructor

        #region Public Methods
        #region Rule Methods

        public List<RuleRowModel> GetAllRulesForDesignVersion(int formDesignVersionId)
        {
            List<RuleRowModel> ruleList = null;
            try
            {
                ruleList = (from rule in this._unitOfWork.RepositoryAsync<Rule>().Get()
                            join prop in this._unitOfWork.RepositoryAsync<PropertyRuleMap>().Get() on rule.RuleID equals prop.RuleID
                            join map in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Get() on prop.UIElementID equals map.UIElementID
                            where map.FormDesignVersionID == formDesignVersionId
                            select new RuleRowModel
                            {
                                UIElementID = prop.UIElementID,
                                RuleId = rule.RuleID,
                                RuleDescription = rule.RuleDescription
                            }).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }

            return ruleList;
        }

        public IEnumerable<RuleRowModel> GetRulesForUIElement(int tenantId, int formDesignVersionId, int uiElementId)
        {

            IList<RuleRowModel> rowModelList = null;
            //call new function
            rowModelList = GetRulesForUIElementHierarchical(tenantId, formDesignVersionId, uiElementId);
            if (rowModelList == null)
            {
                try
                {
                    //Get all the rules along with expression for a uielement
                    rowModelList = (from r in this._unitOfWork.RepositoryAsync<Rule>()
                                                               .Query()
                                                               .Include(c => c.PropertyRuleMaps)
                                                               .Include(c => c.Expressions)
                                                               .Get()
                                    join prm in this._unitOfWork.RepositoryAsync<PropertyRuleMap>()
                                                               .Query()
                                                               .Include(c => c.UIElement)
                                                               .Include(c => c.TargetProperty)
                                                               .Get()
                                    on r.RuleID equals prm.RuleID
                                    where prm.UIElementID == uiElementId
                                    select new RuleRowModel
                                    {
                                        Expressions = (from exp in r.Expressions
                                                       select new ExpressionRowModel
                                                       {
                                                           ExpressionId = exp.ExpressionID,
                                                           LeftOperand = exp.LeftOperand,
                                                           LogicalOperatorTypeId = exp.LogicalOperatorTypeID,
                                                           OperatorTypeId = exp.OperatorTypeID,
                                                           RightOperand = exp.RightOperand,
                                                           RuleId = exp.RuleID,
                                                           ExpressionTypeId = exp.ExpressionTypeID,
                                                           IsRightOperandElement = exp.IsRightOperandElement,
                                                           TenantId = tenantId,
                                                       }).AsEnumerable(),
                                        IsCustomRule = prm.UIElement.HasCustomRule ?? false,
                                        PropertyRuleMapID = prm.PropertyRuleMapID,
                                        ResultFailure = r.ResultFailure,
                                        ResultSuccess = r.ResultSuccess,
                                        IsResultFailureElement = r.IsResultFailureElement,
                                        IsResultSuccessElement = r.IsResultSuccessElement,
                                        Message = r.Message,
                                        RuleId = r.RuleID,
                                        RuleDescription = r.RuleDescription,
                                        TargetPropertyId = prm.TargetPropertyID,
                                        TargetProperty = prm.TargetProperty.TargetPropertyName,
                                        TenantId = tenantId,
                                        UIElementID = prm.UIElementID,
                                        IsStandard = r.IsStandard,
                                        RunOnLoad = r.RunOnLoad
                                    }).ToList();
                    if (rowModelList.Count() == 0)
                        rowModelList = null;
                }
                catch (Exception ex)
                {
                    bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                    if (reThrow) throw ex;
                }
            }
            return rowModelList;
        }

        private IList<RuleRowModel> GetComplexOperatorForRules(int tenantId, IList<RuleRowModel> rowModelList)
        {
            List<int> ruleIds = rowModelList.Select(s => s.RuleId).ToList();
            var complexOp = (from c in this._unitOfWork.RepositoryAsync<ComplexOperator>().Get()
                             join e in this._unitOfWork.RepositoryAsync<Expression>().Get() on c.ExpressionID equals e.ExpressionID
                             join r in this._unitOfWork.RepositoryAsync<Rule>().Get() on e.RuleID equals r.RuleID
                             where ruleIds.Contains(r.RuleID)
                             select c
                             ).ToList();

            foreach (var r in rowModelList)
            {
                if (r.RootExpression != null && r.RootExpression.Expressions != null)
                {
                    foreach (var rExp in r.RootExpression.Expressions)
                    {
                        if (rExp.Expressions != null)
                        {
                            foreach (var exp in rExp.Expressions)
                            {
                                exp.CompOp = (from co in complexOp
                                              where co.ExpressionID == exp.ExpressionId
                                              select new ComplextOperatorModel
                                              {
                                                  Factor = co.Factor,
                                                  FactorValue = co.FactorValue
                                              }).FirstOrDefault();
                            }
                        }
                        else
                        {
                            rExp.CompOp = (from co in complexOp
                                           where co.ExpressionID == rExp.ExpressionId
                                           select new ComplextOperatorModel
                                           {
                                               Factor = co.Factor,
                                               FactorValue = co.FactorValue
                                           }).FirstOrDefault();
                        }
                    }
                }
            }
            return rowModelList;
        }

        private IList<RuleRowModel> GetRulesForUIElementHierarchical(int tenantId, int formDesignVersionId, int uiElementId)
        {
            IList<RuleRowModel> rowModelList = null;
            try
            {
                //Get all the rules along with expression for a uielement
                rowModelList = (from r in this._unitOfWork.RepositoryAsync<Rule>()
                                                           .Query()
                                                           .Include(c => c.PropertyRuleMaps)
                                                           .Include(c => c.Expressions)
                                                           .Get()
                                join prm in this._unitOfWork.RepositoryAsync<PropertyRuleMap>()
                                                           .Query()
                                                           .Include(c => c.UIElement)
                                                           .Include(c => c.TargetProperty)
                                                           .Get()
                                on r.RuleID equals prm.RuleID
                                where prm.UIElementID == uiElementId

                                select new RuleRowModel
                                {
                                    Expressions = (from exp in r.Expressions
                                                   select new ExpressionRowModel
                                                   {
                                                       ExpressionId = exp.ExpressionID,
                                                       LeftOperand = exp.LeftOperand,
                                                       LogicalOperatorTypeId = exp.LogicalOperatorTypeID,
                                                       OperatorTypeId = exp.OperatorTypeID,
                                                       RightOperand = exp.RightOperand,
                                                       RuleId = exp.RuleID,
                                                       TenantId = tenantId,
                                                       ParentExpressionId = exp.ParentExpressionID,
                                                       ExpressionTypeId = exp.ExpressionTypeID,
                                                       IsRightOperandElement = exp.IsRightOperandElement
                                                   }).AsEnumerable(),
                                    FormDesignVersionId = formDesignVersionId,
                                    IsCustomRule = prm.UIElement.HasCustomRule ?? false,
                                    PropertyRuleMapID = prm.PropertyRuleMapID,
                                    ResultFailure = r.ResultFailure,
                                    ResultSuccess = r.ResultSuccess,
                                    IsResultFailureElement = r.IsResultFailureElement,
                                    IsResultSuccessElement = r.IsResultSuccessElement,
                                    Message = r.Message,
                                    RuleId = r.RuleID,
                                    TargetPropertyId = prm.TargetPropertyID,
                                    RuleDescription = r.RuleDescription,
                                    TargetProperty = prm.TargetProperty.TargetPropertyName,
                                    TenantId = tenantId,
                                    UIElementID = prm.UIElementID,
                                    IsStandard = r.IsStandard,
                                    RunOnLoad = r.RunOnLoad
                                }).ToList();
                if (rowModelList.Count() == 0)
                {
                    rowModelList = null;
                }
                else
                {
                    foreach (var rowModel in rowModelList)
                    {
                        if (rowModel.Expressions != null && rowModel.Expressions.Count() > 0)
                        {
                            rowModel.RootExpression = GenerateHierarchicalExpression(rowModel.Expressions.ToList());
                        }
                    }
                    rowModelList = this.GetComplexOperatorForRules(tenantId, rowModelList);
                }

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return rowModelList;
        }

        private ExpressionRowModel GenerateHierarchicalExpression(List<ExpressionRowModel> expressionList)
        {
            var expression = expressionList.Where(n => n.ParentExpressionId == null).FirstOrDefault();
            if (expression != null)
            {
                GenerateParentExpression(expressionList, ref expression);
            }
            return expression;
        }

        private void GenerateParentExpression(List<ExpressionRowModel> expressionList, ref ExpressionRowModel parentExpression)
        {
            var parent = parentExpression;
            var childExpressions = from exp in expressionList where exp.ParentExpressionId == parent.ExpressionId select exp;
            if (childExpressions != null && childExpressions.Count() > 0)
            {
                parentExpression.Expressions = new List<ExpressionRowModel>();
                foreach (var childExpression in childExpressions)
                {
                    ExpressionRowModel child = childExpression;
                    GenerateParentExpression(expressionList, ref child);
                    parentExpression.Expressions.Add(childExpression);
                }
            }
        }

        public IEnumerable<LogicalOperatorTypeViewModel> GetDataSourceElementDisplayMode(int tenantId)
        {
            List<LogicalOperatorTypeViewModel> fields = new List<LogicalOperatorTypeViewModel>();
            try
            {
                fields = (from c in this._unitOfWork.RepositoryAsync<LogicalOperatorType>()
                                                           .Query()
                                                           .Filter(c => c.IsActive == true)
                                                           .Get()
                          select new LogicalOperatorTypeViewModel
                          {
                              LogicalOperatorTypeID = c.LogicalOperatorTypeID,
                              LogicalOperatorTypeCode = c.LogicalOperatorTypeCode
                          }).ToList();


                if (fields.Count() == 0)
                    fields = null;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return fields;
        }

        public IEnumerable<OperatorTypeViewModel> GetOperatorTypes(int tenantId)
        {
            List<OperatorTypeViewModel> operators = new List<OperatorTypeViewModel>();
            try
            {
                operators = (from c in this._unitOfWork.RepositoryAsync<OperatorType>()
                                                           .Query()
                                                           .Filter(c => c.IsActive == true)
                                                           .Get()
                             select new OperatorTypeViewModel
                             {
                                 OperatorTypeID = c.OperatorTypeID,
                                 DisplayText = c.DisplayText
                             }).ToList();


                if (operators.Count() == 0)
                    operators = null;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return operators;
        }


        public IEnumerable<TargetPropertyViewModel> GetTargetPropertyTypes(int tenantId)
        {
            List<TargetPropertyViewModel> targetProperties = new List<TargetPropertyViewModel>();
            try
            {
                targetProperties = (from c in this._unitOfWork.RepositoryAsync<TargetProperty>()
                                                           .Query()
                                                           .Get()
                                    select new TargetPropertyViewModel
                                    {
                                        TargetPropertyId = c.TargetPropertyID,
                                        TargetPropertyName = c.TargetPropertyName
                                    }).ToList();


                if (targetProperties.Count() == 0)
                    targetProperties = null;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return targetProperties;
        }

        public bool IsMDMNameUnique(int tenantId, int formDesignId, string mdmName, int uiElementId, string uiElementType)
        {
            //Contract.Requires(tenantId > 0, "Invalid tenantId");
            //Contract.Requires(uiElementId > 0, "Invalid uiElementId");
            //Contract.Requires(formDesignVersionId > 0, "Invalid formDesignVersionId");
            //Contract.Requires(uiElementType != null, "Invalid uiElementType");
            //Contract.Requires(dataSourceName != null, "Invalid dataSourceName");
            ServiceResult result = new ServiceResult();
            bool isMDMExist = false;

            try
            {
                if (uiElementType == "Section" || uiElementType == "Repeater")
                {
                    isMDMExist = this._unitOfWork.RepositoryAsync<UIElement>()
                             .Query()
                             .Filter(c => c.FormID == formDesignId
                              && ((c.MDMName == null || c.MDMName == "") ? (c.GeneratedName.ToLower().Replace(" ", "") == mdmName.ToLower().Replace(" ", ""))
                              : (c.MDMName.ToLower().Replace(" ", "") == mdmName.ToLower().Replace(" ", "")))
                             && c.UIElementID != uiElementId)
                             .Get()
                             .Any();
                }
                else
                {
                    int? parentUIElmentId = (from u in this._unitOfWork.RepositoryAsync<UIElement>().Get()
                                             where u.UIElementID == uiElementId
                                             select u.ParentUIElementID).SingleOrDefault();


                    isMDMExist = this._unitOfWork.RepositoryAsync<UIElement>()
                             .Query()
                             .Filter(c => c.ParentUIElementID == parentUIElmentId
                             && ((c.MDMName == null || c.MDMName == "") ? (c.GeneratedName.ToLower().Replace(" ", "") == mdmName.ToLower().Replace(" ", ""))
                             : (c.MDMName.ToLower().Replace(" ", "") == mdmName.ToLower().Replace(" ", "")))
                             && c.UIElementID != uiElementId)
                             .Get()
                             .Any();
                }
            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return isMDMExist;
        }
        #endregion Rule Methods
        #endregion Public Methods

        #region Private Methods
        private int GetMaxRuleID()
        {
            int maxID = 0;
            try
            {
                List<int> ruleIDList = this._unitOfWork.RepositoryAsync<Rule>().Query().Get().Select(c => c.RuleID).ToList();
                if (ruleIDList.Count() > 0)
                {
                    maxID = ruleIDList.Max();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return maxID;
        }

        private string GenerateRuleName(int uiElementID)
        {
            string ruleName = string.Empty;
            try
            {
                string uiElementName = this._unitOfWork.RepositoryAsync<UIElement>().FindById(uiElementID).UIElementName;
                ruleName = uiElementName + "_Rule_" + GetMaxRuleID();
            }
            catch (Exception ex)
            {
                throw;
            }
            return ruleName;
        }

        private IEnumerable<RuleRowModel> ChangeRules(string userName, int tenantId, int formDesignVersionId, int previousUIElementId, int uiElementId, IEnumerable<RuleRowModel> newRules, bool createNew)
        {
            //get the current rules

            IEnumerable<RuleRowModel> currentRules = GetRulesForUIElementHierarchical(tenantId, formDesignVersionId, previousUIElementId);
            if ((newRules == null || newRules.Count() == 0) && (currentRules == null || currentRules.Count() == 0))
            {
                return new List<RuleRowModel>();
            }
            if (currentRules == null)
            {
                currentRules = new List<RuleRowModel>();
            }
            if (createNew == false)
            {
                //delete rules that are not returned
                List<int> newRuleIDs = (from newRule in newRules select newRule.RuleId).ToList();
                var delRules = from del in currentRules where !newRuleIDs.Contains(del.RuleId) select del;
                if (delRules != null)
                {
                    foreach (var delRule in delRules)
                    {
                        DeleteRule(delRule);
                    }
                }
            }
            if ((newRules == null || newRules.Count() == 0) && createNew == true)
            {
                newRules = CopyRules(currentRules);
            }
            //for each rule, find matching rule - update if not new/ else create new
            foreach (RuleRowModel newRule in newRules)
            {
                if (createNew == false)
                {
                    var currentRule = (from c in currentRules where c.RuleId == newRule.RuleId select c).FirstOrDefault();
                    if (currentRule != null && currentRule.RuleId == newRule.RuleId)
                    {
                        //update rule
                        UpdateRule(userName, newRule);
                    }
                    else
                    {
                        //add new rule
                        AddRule(userName, uiElementId, newRule);
                    }
                }
                else
                {
                    //add new rule
                    AddRule(userName, uiElementId, newRule);
                }
            }
            return newRules;
        }

        private void AddRule(string userName, int uiElementID, RuleRowModel model)
        {
            Rule rule = new Rule();
            rule.AddedBy = userName;
            rule.AddedDate = DateTime.Now;
            rule.RuleName = "RULE"; //TODO: remove this column/attribute from DB
            rule.RuleTargetTypeID = 1; //TODO: remove this column/attribute from DB
            rule.ResultFailure = model.ResultFailure;
            rule.RuleDescription = model.RuleDescription;
            rule.ResultSuccess = model.ResultSuccess;
            rule.IsResultFailureElement = model.IsResultFailureElement;
            rule.IsResultSuccessElement = model.IsResultSuccessElement;
            rule.Message = model.Message;
            rule.IsStandard = model.IsStandard;
            rule.RunOnLoad = model.RunOnLoad;
            this._unitOfWork.RepositoryAsync<Rule>().Insert(rule);
            this._unitOfWork.Save();
            model.RuleId = rule.RuleID;

            if (model.TargetKeyFilter != null)
            {
                foreach (var filter in model.TargetKeyFilter)
                {
                    TargetRepeaterKeyFilter objFilter = new TargetRepeaterKeyFilter();
                    objFilter.RuleID = rule.RuleID;
                    objFilter.RepeaterKey = filter.UIElementPath;
                    objFilter.RepeaterKeyValue = filter.FilterValue;
                    this._unitOfWork.RepositoryAsync<TargetRepeaterKeyFilter>().Insert(objFilter);
                    this._unitOfWork.Save();
                }

            }

            PropertyRuleMap map = new PropertyRuleMap();
            map.AddedBy = userName;
            map.AddedDate = DateTime.Now;
            map.IsCustomRule = model.IsCustomRule;
            map.RuleID = rule.RuleID;
            map.TargetPropertyID = model.TargetPropertyId;
            map.UIElementID = uiElementID;
            this._unitOfWork.RepositoryAsync<PropertyRuleMap>().Insert(map);
            if (model.RootExpression != null)
            {
                List<ExpressionRowModel> expressions = new List<ExpressionRowModel>();
                GetExpressions(model.RootExpression, null, ref expressions);
                expressions.Reverse();
                Dictionary<int, int> expressionIds = new Dictionary<int, int>();
                foreach (ExpressionRowModel expModel in expressions)
                {
                    Expression exp = new Expression();
                    exp.AddedBy = userName;
                    exp.AddedDate = DateTime.Now;
                    exp.LeftOperand = expModel.LeftOperand;
                    exp.LogicalOperatorTypeID = expModel.LogicalOperatorTypeId > 0 ? expModel.LogicalOperatorTypeId : 1; //default to 1
                    exp.OperatorTypeID = expModel.OperatorTypeId > 0 ? expModel.OperatorTypeId : 1; //default to 1
                    exp.RightOperand = expModel.RightOperand;
                    exp.ExpressionTypeID = expModel.ExpressionTypeId;
                    exp.IsRightOperandElement = expModel.IsRightOperandElement;
                    exp.RuleID = rule.RuleID;
                    int previousExpressionId = expModel.ExpressionId;
                    if (expModel.ParentExpressionId.HasValue == true)
                    {
                        exp.ParentExpressionID = expressionIds[expModel.ParentExpressionId.Value];
                    }
                    this._unitOfWork.RepositoryAsync<Expression>().Insert(exp);
                    this._unitOfWork.Save();
                    expressionIds.Add(previousExpressionId, exp.ExpressionID);
                    expModel.ExpressionId = exp.ExpressionID;

                    this.UpdateRepeaterKeyFilter(expModel);
                    this.UpdateComplexOperator(expModel);
                }
            }
        }

        private void DeleteRule(RuleRowModel rule)
        {
            List<ExpressionRowModel> expressions = new List<ExpressionRowModel>();
            if (rule.RootExpression != null)
            {
                GetExpressions(rule.RootExpression, null, ref expressions);

                foreach (ExpressionRowModel expression in expressions)
                {
                    //Delete Repeater key filters
                    var rptFilter = this._unitOfWork.RepositoryAsync<RepeaterKeyFilter>().Get().Where(s => s.ExpressionID == expression.ExpressionId).ToList();
                    foreach (var filter in rptFilter)
                    {
                        this._unitOfWork.RepositoryAsync<RepeaterKeyFilter>().Delete(filter.RepeaterKeyID);
                        this._unitOfWork.Save();
                    }

                    //Delete Complex Operator
                    var compOp = this._unitOfWork.RepositoryAsync<ComplexOperator>().Get().Where(s => s.ExpressionID == expression.ExpressionId).ToList();
                    foreach (var ops in compOp)
                    {
                        this._unitOfWork.RepositoryAsync<ComplexOperator>().Delete(ops.ComplexOperatorID);
                        this._unitOfWork.Save();
                    }
                }

                foreach (ExpressionRowModel expression in expressions)
                {
                    this._unitOfWork.RepositoryAsync<Expression>().Delete(expression.ExpressionId);
                    this._unitOfWork.Save();
                }
            }

            var targetRptKey = this._unitOfWork.RepositoryAsync<TargetRepeaterKeyFilter>().Get().Where(s => s.RuleID == rule.RuleId).ToList();
            if (targetRptKey.Count() > 0)
            {
                foreach (var item in targetRptKey)
                {
                    this._unitOfWork.RepositoryAsync<TargetRepeaterKeyFilter>().Delete(item.TargetRepeaterKeyID);
                    this._unitOfWork.Save();
                }
            }

            this._unitOfWork.RepositoryAsync<Rule>().Delete(rule.RuleId);
            this._unitOfWork.RepositoryAsync<PropertyRuleMap>().Delete(rule.PropertyRuleMapID);
            this._unitOfWork.Save();
        }

        private void GetExpressions(ExpressionRowModel expression, Nullable<int> parentExpressionId, ref List<ExpressionRowModel> expressions)
        {
            if (expression != null)
            {
                if (expression.Expressions != null && expression.Expressions.Count() > 0)
                {
                    foreach (ExpressionRowModel model in expression.Expressions)
                    {
                        GetExpressions(model, expression.ExpressionId, ref expressions);
                    }
                }
                expression.ParentExpressionId = parentExpressionId;
                expressions.Add(expression);
            }
        }

        private void UpdateRule(string userName, RuleRowModel rule)
        {
            Rule ruleToUpdate = this._unitOfWork.RepositoryAsync<Rule>().FindById(rule.RuleId);
            ruleToUpdate.UpdatedBy = userName;
            ruleToUpdate.UpdatedDate = DateTime.Now;
            ruleToUpdate.RuleDescription = rule.RuleDescription;
            ruleToUpdate.ResultFailure = rule.ResultFailure;
            ruleToUpdate.ResultSuccess = rule.ResultSuccess;
            ruleToUpdate.IsResultFailureElement = rule.IsResultFailureElement;
            ruleToUpdate.IsResultSuccessElement = rule.IsResultSuccessElement;
            ruleToUpdate.Message = rule.Message;
            ruleToUpdate.IsStandard = rule.IsStandard;
            ruleToUpdate.RunOnLoad = rule.RunOnLoad;
            this._unitOfWork.RepositoryAsync<Rule>().Update(ruleToUpdate);

            if (rule.TargetKeyFilter != null)
            {
                var keyFilters = this._unitOfWork.RepositoryAsync<TargetRepeaterKeyFilter>().Get().Where(s => s.RuleID == rule.RuleId).ToList();
                if (keyFilters.Count > 0)
                {
                    foreach (var filter in keyFilters)
                    {
                        this._unitOfWork.RepositoryAsync<TargetRepeaterKeyFilter>().Delete(filter.TargetRepeaterKeyID);
                        this._unitOfWork.Save();
                    }

                }

                foreach (var filter in rule.TargetKeyFilter)
                {
                    TargetRepeaterKeyFilter objFilter = new TargetRepeaterKeyFilter();
                    objFilter.RuleID = rule.RuleId;
                    objFilter.RepeaterKey = filter.UIElementPath;
                    objFilter.RepeaterKeyValue = filter.FilterValue;
                    this._unitOfWork.RepositoryAsync<TargetRepeaterKeyFilter>().Insert(objFilter);
                    this._unitOfWork.Save();
                }

            }

            PropertyRuleMap map = this._unitOfWork.RepositoryAsync<PropertyRuleMap>().FindById(rule.PropertyRuleMapID);
            map.UpdatedBy = userName;
            map.UpdatedDate = DateTime.Now;
            map.TargetPropertyID = rule.TargetPropertyId;
            this._unitOfWork.RepositoryAsync<PropertyRuleMap>().Update(map);

            var expressions = this._unitOfWork.RepositoryAsync<Expression>().Query()
                                                                          .Filter(c => c.RuleID == rule.RuleId)
                                                                          .Get();

            if (rule.RootExpression != null)
            {
                List<ExpressionRowModel> newExpressions = new List<ExpressionRowModel>();
                GetExpressions(rule.RootExpression, null, ref newExpressions);
                if (expressions != null)
                {
                    List<int> newExpIDs = (from exp in newExpressions select exp.ExpressionId).ToList();
                    var delExpressions = from del in expressions where !newExpIDs.Contains(del.ExpressionID) orderby del.ExpressionID descending select del;
                    if (delExpressions != null)
                    {
                        if (delExpressions.Count() > 0)
                        {
                            foreach (var delExp in delExpressions)
                            {
                                var filters = (from flt in this._unitOfWork.RepositoryAsync<RepeaterKeyFilter>().Get() where flt.ExpressionID == delExp.ExpressionID select flt).ToList();
                                foreach (var rptFilter in filters)
                                {
                                    this._unitOfWork.RepositoryAsync<RepeaterKeyFilter>().Delete(rptFilter.RepeaterKeyID);
                                }
                            }
                            this._unitOfWork.Save();

                            foreach (var delExp in delExpressions)
                            {
                                var compOp = (from flt in this._unitOfWork.RepositoryAsync<ComplexOperator>().Get() where flt.ExpressionID == delExp.ExpressionID select flt).ToList();
                                foreach (var op in compOp)
                                {
                                    this._unitOfWork.RepositoryAsync<ComplexOperator>().Delete(op.ComplexOperatorID);
                                }
                            }
                            this._unitOfWork.Save();

                            foreach (var delExp in delExpressions)
                            {
                                this._unitOfWork.RepositoryAsync<Expression>().Delete(delExp.ExpressionID);
                            }
                            this._unitOfWork.Save();
                        }
                    }
                }
                newExpressions.Reverse();
                Dictionary<int, int> expressionIds = new Dictionary<int, int>();
                foreach (var exp in newExpressions)
                {
                    Expression updateExp = (from e in expressions where e.ExpressionID == exp.ExpressionId select e).FirstOrDefault();
                    if (updateExp != null && updateExp.ExpressionID == exp.ExpressionId)
                    {
                        updateExp.UpdatedBy = userName;
                        updateExp.UpdatedDate = DateTime.Now;
                        updateExp.LeftOperand = exp.LeftOperand;
                        updateExp.RightOperand = exp.RightOperand;
                        updateExp.LogicalOperatorTypeID = exp.LogicalOperatorTypeId > 0 ? exp.LogicalOperatorTypeId : 1; //default to 1
                        updateExp.OperatorTypeID = exp.OperatorTypeId > 0 ? exp.OperatorTypeId : 1; //default to 1
                        updateExp.ExpressionTypeID = exp.ExpressionTypeId;
                        updateExp.IsRightOperandElement = exp.IsRightOperandElement;
                        this._unitOfWork.RepositoryAsync<Expression>().Update(updateExp);
                    }
                    else
                    {
                        Expression newExp = new Expression();
                        newExp.AddedBy = userName;
                        newExp.AddedDate = DateTime.Now;
                        newExp.LeftOperand = exp.LeftOperand;
                        newExp.LogicalOperatorTypeID = exp.LogicalOperatorTypeId > 0 ? exp.LogicalOperatorTypeId : 1; //default to 1
                        newExp.OperatorTypeID = exp.OperatorTypeId > 0 ? exp.OperatorTypeId : 1; //default to 1
                        newExp.RightOperand = exp.RightOperand;
                        newExp.ExpressionTypeID = exp.ExpressionTypeId;
                        newExp.IsRightOperandElement = exp.IsRightOperandElement;
                        newExp.RuleID = rule.RuleId;
                        int previousExpressionId = exp.ExpressionId;
                        if (exp.ParentExpressionId.HasValue == true)
                        {
                            if (exp.ParentExpressionId.Value > -1)
                            {
                                newExp.ParentExpressionID = exp.ParentExpressionId;
                            }
                            else
                            {
                                newExp.ParentExpressionID = expressionIds[exp.ParentExpressionId.Value];
                            }
                        }

                        this._unitOfWork.RepositoryAsync<Expression>().Insert(newExp);
                        this._unitOfWork.Save();
                        expressionIds.Add(previousExpressionId, newExp.ExpressionID);
                        exp.ExpressionId = newExp.ExpressionID;
                    }
                    this.UpdateRepeaterKeyFilter(exp);
                    this.UpdateComplexOperator(exp);
                }
            }
            //if expression is null the delete the existing expressions for Rule if any.
            else
            {
                if (expressions != null)
                {
                    foreach (var delExp in expressions)
                    {
                        this._unitOfWork.RepositoryAsync<RepeaterKeyFilter>().Delete(delExp.ExpressionID);
                    }
                    this._unitOfWork.Save();

                    foreach (var delExp in expressions)
                    {
                        this._unitOfWork.RepositoryAsync<Expression>().Delete(delExp.ExpressionID);
                    }
                }
            }
        }

        private void UpdateRepeaterKeyFilter(ExpressionRowModel expModel)
        {
            UpdateRepeaterKeyFilter(expModel, false);
            UpdateRepeaterKeyFilter(expModel, true);
        }
        private void UpdateRepeaterKeyFilter(ExpressionRowModel expModel, bool isRightOnly)
        {
            List<RepeaterKeyFilterModel> keyFilterModel = isRightOnly ? expModel.RightKeyFilter : expModel.LeftKeyFilter;

            if (keyFilterModel != null)
            {
                var filters = this._unitOfWork.RepositoryAsync<RepeaterKeyFilter>().Query()
                                           .Filter(c => c.ExpressionID == expModel.ExpressionId && c.IsRightOperand == isRightOnly)
                                           .Get().ToList();
                if (filters.Count > 0)
                {
                    foreach (var item in filters)
                    {
                        this._unitOfWork.RepositoryAsync<RepeaterKeyFilter>().Delete(item.RepeaterKeyID);
                    }
                    this._unitOfWork.Save();
                }

                foreach (var fltr in keyFilterModel)
                {
                    RepeaterKeyFilter rptFilter = new RepeaterKeyFilter();
                    rptFilter.ExpressionID = expModel.ExpressionId;
                    rptFilter.RepeaterKey = fltr.UIElementPath;
                    rptFilter.RepeaterKeyValue = fltr.FilterValue;
                    rptFilter.IsRightOperand = isRightOnly;
                    this._unitOfWork.RepositoryAsync<RepeaterKeyFilter>().Insert(rptFilter);
                }
                this._unitOfWork.Save();
            }
        }
        private void UpdateComplexOperator(ExpressionRowModel expModel)
        {
            if (expModel.CompOp != null)
            {
                var objCompOp = (from e in this._unitOfWork.RepositoryAsync<ComplexOperator>().Get() where e.ExpressionID == expModel.ExpressionId select e).FirstOrDefault();
                if (objCompOp != null)
                {
                    objCompOp.Factor = expModel.CompOp.Factor;
                    objCompOp.FactorValue = expModel.CompOp.FactorValue;
                    this._unitOfWork.RepositoryAsync<ComplexOperator>().Update(objCompOp);
                    this._unitOfWork.Save();
                }
                else
                {
                    ComplexOperator objComlexOp = new ComplexOperator();
                    objComlexOp.ExpressionID = expModel.ExpressionId;
                    objComlexOp.OperatorID = expModel.OperatorTypeId;
                    objComlexOp.Factor = expModel.CompOp.Factor;
                    objComlexOp.FactorValue = expModel.CompOp.FactorValue;
                    this._unitOfWork.RepositoryAsync<ComplexOperator>().Insert(objComlexOp);
                    this._unitOfWork.Save();
                }
            }
        }
        private IEnumerable<RuleRowModel> CopyRules(IEnumerable<RuleRowModel> currentRules)
        {
            List<RuleRowModel> newRules = new List<RuleRowModel>();

            foreach (RuleRowModel currentRule in currentRules)
            {
                RuleRowModel newRule = new RuleRowModel();
                newRule.IsCustomRule = currentRule.IsCustomRule;
                newRule.ResultFailure = currentRule.ResultFailure;
                newRule.ResultSuccess = currentRule.ResultSuccess;
                newRule.IsResultSuccessElement = currentRule.IsResultSuccessElement;
                newRule.IsResultFailureElement = currentRule.IsResultFailureElement;
                newRule.Message = currentRule.Message;
                newRule.TargetPropertyId = currentRule.TargetPropertyId;
                newRule.RuleDescription = currentRule.RuleDescription;
                newRule.RootExpression = currentRule.RootExpression;
                newRule.TargetKeyFilter = currentRule.TargetKeyFilter;
                newRule.SourceRuleID = currentRule.RuleId;
                newRules.Add(newRule);
            }
            return newRules;
        }
        #endregion Private Methods
    }
}
