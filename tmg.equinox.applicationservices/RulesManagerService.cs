using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.RulesManager;
using tmg.equinox.applicationservices.viewmodels.UIElement;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.repository.extensions;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.applicationservices
{
    public class RulesManagerService : IRulesManagerService
    {
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        public RulesManagerService(IUnitOfWorkAsync unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<FormDesignViewModel> GetFormDesigns(int tenandId)
        {
            List<FormDesignViewModel> formDesignList = null;
            try
            {
                formDesignList = (from design in this._unitOfWork.RepositoryAsync<FormDesign>().Get()
                                  join type in this._unitOfWork.RepositoryAsync<DocumentDesignType>().Get() on design.DocumentDesignTypeID equals type.DocumentDesignTypeID
                                  where design.IsActive == true
                                  select new FormDesignViewModel()
                                  {
                                      FormID = design.FormID,
                                      FormName = design.DisplayText,
                                      Group = type.DocumentDesignName
                                  }).OrderBy(s => s.Group).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return formDesignList;
        }

        public List<FormDesignViewModel> GetFormDesignVersions(int tenantId, int formDesignId)
        {
            List<FormDesignViewModel> formDesignVersionList = null;
            try
            {
                formDesignVersionList = (from design in this._unitOfWork.RepositoryAsync<FormDesignVersion>().Get()
                                         where design.FormDesignID == formDesignId
                                         select new FormDesignViewModel()
                                         {
                                             FormID = design.FormDesignVersionID,
                                             VersionNumber = design.VersionNumber,
                                             Status = design.StatusID
                                         }).OrderBy(s => s.FormID).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return formDesignVersionList;
        }

        public IEnumerable<ElementRowModel> GetElementList(int tenantId, int formDesignVersionId)
        {
            IList<ElementRowModel> uiElementRowModelList = null;
            try
            {
                SqlParameter paramFrmDesignVersionID = new SqlParameter("@FormDesignVersionID", formDesignVersionId);
                List<UIElementList> frmDesignVersionElementList = this._unitOfWork.Repository<UIElementList>()
                                                                                .ExecuteSql("exec [dbo].[uspGetFormDesignVersionElementList] @FormDesignVersionID", paramFrmDesignVersionID)
                                                                                .ToList();
                uiElementRowModelList = (from ele in frmDesignVersionElementList
                                         select new ElementRowModel()
                                         {
                                             Element = ele.Element,
                                             Parent = ele.Parent,
                                             Section = ele.Section.IndexOf(">") > 0 ? ele.Section.Substring(0, ele.Section.LastIndexOf(" >")) : ele.Section,
                                             UIElementID = ele.UIElementID,
                                             UIElementName = ele.UIElementName,
                                             IsContainer = ele.IsContainer
                                         }).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return uiElementRowModelList;
        }

        public List<RuleRowViewModel> GetRulesByFormDesignVersion(int tenantId, int formDesignVersionId)
        {
            List<RuleRowViewModel> ruleList = null;

            try
            {
                SqlParameter paramFrmDesignVersionID = new SqlParameter("@FormDesignVersionID", formDesignVersionId);
                List<RuleModel> ruleModelList = this._unitOfWork.Repository<RuleModel>()
                                                                                .ExecuteSql("exec [dbo].[uspGetFormDesignVersionRules] @FormDesignVersionID", paramFrmDesignVersionID)
                                                                                .ToList();
                var targets = this.GetRuleTargets(tenantId, formDesignVersionId);
                var sources = this.GetRuleSources(tenantId, formDesignVersionId);

                ruleList = (from rule in ruleModelList
                            join t in targets on rule.RuleID equals t.Key into ps
                            from p in ps.DefaultIfEmpty()
                            join s in sources on rule.RuleID equals s.Key into ss
                            from q in ss.DefaultIfEmpty()
                            select new RuleRowViewModel()
                            {
                                RuleID = rule.RuleID,
                                RuleName = rule.RuleName,
                                RuleCode = rule.RuleCode,
                                Description = rule.Description,
                                Type = rule.Type,
                                RuleTypeID = rule.RuleTargetTypeID,
                                SourceElement = q.Key == 0 ? "N/A" : q.Value,
                                TargetElement = p.Key == 0 ? "N/A" : p.Value
                            }).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return ruleList;
        }

        public GridPagingResponse<RuleRowViewModel> GetRulesByFormDesignVersion(int tenantId, int formDesignVersionId, GridPagingRequest gridPagingRequest)
        {
            List<RuleRowViewModel> ruleList = null;
            int count = 0;

            try
            {
                SearchCriteria criteria = new SearchCriteria();
                criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);

                SqlParameter paramFrmDesignVersionID = new SqlParameter("@FormDesignVersionID", formDesignVersionId);
                List<RuleModel> ruleModelList = this._unitOfWork.Repository<RuleModel>()
                                                                                .ExecuteSql("exec [dbo].[uspGetFormDesignVersionRules] @FormDesignVersionID", paramFrmDesignVersionID)
                                                                                .ToList();
                var targets = this.GetRuleTargets(tenantId, formDesignVersionId);
                var sources = this.GetRuleSources(tenantId, formDesignVersionId);

                ruleList = (from rule in ruleModelList
                            join t in targets on rule.RuleID equals t.Key into ps
                            from p in ps.DefaultIfEmpty()
                            join s in sources on rule.RuleID equals s.Key into ss
                            from q in ss.DefaultIfEmpty()
                            select new RuleRowViewModel()
                            {
                                RuleID = rule.RuleID,
                                RuleName = rule.RuleName,
                                RuleCode = rule.RuleCode,
                                Description = rule.Description,
                                Type = rule.Type,
                                RuleTypeID = rule.RuleTargetTypeID,
                                SourceElement = q.Key == 0 ? "N/A" : q.Value,
                                TargetElement = p.Key == 0 ? "N/A" : p.Value
                            }).ToList().ApplySearchCriteria(criteria).ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord)
                            .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return new GridPagingResponse<RuleRowViewModel>(gridPagingRequest.page, count, gridPagingRequest.rows, ruleList);
        }

        public List<RuleRowViewModel> GetRulesByFormDesignVersionByTarget(int tenantId, int formDesignVersionId)
        {
            List<RuleRowViewModel> ruleList = null;
            try
            {
                SqlParameter paramFrmDesignVersionID = new SqlParameter("@FormDesignVersionID", formDesignVersionId);
                List<RuleModel> ruleModelList = this._unitOfWork.Repository<RuleModel>()
                                                                                .ExecuteSql("exec [dbo].[uspGetFormDesignVersionRulesByTarget] @FormDesignVersionID", paramFrmDesignVersionID)
                                                                                .ToList();
                var sources = this.GetRuleSources(tenantId, formDesignVersionId);

                ruleList = (from rule in ruleModelList
                            join s in sources on rule.RuleID equals s.Key
                            select new RuleRowViewModel()
                            {
                                Section = rule.Section.IndexOf(">") > 0 ? rule.Section.Substring(0, rule.Section.LastIndexOf(" >")) : rule.Section,
                                TargetElement = rule.TargetElement,
                                SourceElement = s.Value,
                                KeyFilter = rule.KeyFilter,
                                RuleID = rule.RuleID,
                                RuleName = rule.RuleName,
                                RuleCode = rule.RuleCode,
                                Description = rule.Description,
                                Type = rule.Type
                            }).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return ruleList;
        }

        public GridPagingResponse<RuleRowViewModel> GetRulesByFormDesignVersionByTarget(int tenantId, int formDesignVersionId, GridPagingRequest gridPagingRequest)
        {
            List<RuleRowViewModel> ruleList = null;
            int count = 0;

            try
            {
                SearchCriteria criteria = new SearchCriteria();
                criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);

                SqlParameter paramFrmDesignVersionID = new SqlParameter("@FormDesignVersionID", formDesignVersionId);
                List<RuleModel> ruleModelList = this._unitOfWork.Repository<RuleModel>()
                                                                                .ExecuteSql("exec [dbo].[uspGetFormDesignVersionRulesByTarget] @FormDesignVersionID", paramFrmDesignVersionID)
                                                                                .ToList();
                var sources = this.GetRuleSources(tenantId, formDesignVersionId);

                ruleList = (from rule in ruleModelList
                            join s in sources on rule.RuleID equals s.Key
                            select new RuleRowViewModel()
                            {
                                Section = rule.Section.IndexOf(">") > 0 ? rule.Section.Substring(0, rule.Section.LastIndexOf(" >")) : rule.Section,
                                TargetElement = rule.TargetElement,
                                SourceElement = s.Value,
                                KeyFilter = rule.KeyFilter,
                                RuleID = rule.RuleID,
                                RuleName = rule.RuleName,
                                RuleCode = rule.RuleCode,
                                Description = rule.Description,
                                Type = rule.Type
                            }).ToList().ApplySearchCriteria(criteria).ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord)
                            .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return new GridPagingResponse<RuleRowViewModel>(gridPagingRequest.page, count, gridPagingRequest.rows, ruleList);
        }

        public List<RuleRowViewModel> GetRulesByFormDesignVersionBySource(int tenantId, int formDesignVersionId)
        {
            List<RuleRowViewModel> ruleList = null;
            try
            {
                SqlParameter paramFrmDesignVersionID = new SqlParameter("@FormDesignVersionID", formDesignVersionId);
                List<RuleModel> ruleModelList = this._unitOfWork.Repository<RuleModel>()
                                                                                .ExecuteSql("exec [dbo].[uspGetFormDesignVersionRulesBySource] @FormDesignVersionID", paramFrmDesignVersionID)
                                                                                .ToList();
                var targets = this.GetRuleTargets(tenantId, formDesignVersionId);

                ruleList = (from rule in ruleModelList
                            join t in targets on rule.RuleID equals t.Key
                            select new RuleRowViewModel()
                            {
                                Section = rule.Section.IndexOf(">") > 0 ? rule.Section.Substring(0, rule.Section.LastIndexOf(" >")) : rule.Section,
                                SourceElement = rule.SourceElement,
                                TargetElement = t.Value,
                                KeyFilter = rule.KeyFilter,
                                RuleID = rule.RuleID,
                                RuleName = rule.RuleName,
                                RuleCode = rule.RuleCode,
                                Description = rule.Description,
                                Type = rule.Type
                            }).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return ruleList;
        }

        public GridPagingResponse<RuleRowViewModel> GetRulesByFormDesignVersionBySource(int tenantId, int formDesignVersionId, GridPagingRequest gridPagingRequest)
        {
            List<RuleRowViewModel> ruleList = null;
            int count = 0;

            try
            {
                SearchCriteria criteria = new SearchCriteria();
                criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);

                SqlParameter paramFrmDesignVersionID = new SqlParameter("@FormDesignVersionID", formDesignVersionId);
                List<RuleModel> ruleModelList = this._unitOfWork.Repository<RuleModel>()
                                                                                .ExecuteSql("exec [dbo].[uspGetFormDesignVersionRulesBySource] @FormDesignVersionID", paramFrmDesignVersionID)
                                                                                .ToList();
                var targets = this.GetRuleTargets(tenantId, formDesignVersionId);

                ruleList = (from rule in ruleModelList
                            join t in targets on rule.RuleID equals t.Key
                            select new RuleRowViewModel()
                            {
                                Section = rule.Section.IndexOf(">") > 0 ? rule.Section.Substring(0, rule.Section.LastIndexOf(" >")) : rule.Section,
                                SourceElement = rule.SourceElement,
                                TargetElement = t.Value,
                                KeyFilter = rule.KeyFilter,
                                RuleID = rule.RuleID,
                                RuleName = rule.RuleName,
                                RuleCode = rule.RuleCode,
                                Description = rule.Description,
                                Type = rule.Type
                            }).ToList().ApplySearchCriteria(criteria).ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord)
                            .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return new GridPagingResponse<RuleRowViewModel>(gridPagingRequest.page, count, gridPagingRequest.rows, ruleList);
        }

        public RuleRowModel GetRuleByID(int tenantId, int ruleId, int formDesignVersionId)
        {
            RuleRowModel rowModelList = null;
            try
            {
                //Get all the rules along with expression for a uielement
                rowModelList = (from r in this._unitOfWork.RepositoryAsync<Rule>()
                                                           .Query()
                                                           .Include(c => c.Expressions)
                                                           .Get()
                                join map in this._unitOfWork.RepositoryAsync<PropertyRuleMap>().Get() on r.RuleID equals map.RuleID
                                join tgt in this._unitOfWork.RepositoryAsync<TargetProperty>().Get() on map.TargetPropertyID equals tgt.TargetPropertyID
                                where r.RuleID == ruleId
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
                                    ResultFailure = r.ResultFailure,
                                    ResultSuccess = r.ResultSuccess,
                                    IsResultFailureElement = r.IsResultFailureElement,
                                    IsResultSuccessElement = r.IsResultSuccessElement,
                                    Message = r.Message,
                                    RuleId = r.RuleID,
                                    TargetPropertyId = map.TargetPropertyID,
                                    RuleDescription = r.RuleDescription,
                                    TargetProperty = tgt.TargetPropertyName,
                                    TenantId = tenantId,
                                    RunOnLoad = r.RunOnLoad,
                                    IsStandard = r.IsStandard,
                                    RuleName = r.RuleName
                                }).FirstOrDefault();
                if (rowModelList != null)
                {
                    if (rowModelList.Expressions != null && rowModelList.Expressions.Count() > 0)
                    {
                        rowModelList.RootExpression = GenerateHierarchicalExpression(rowModelList.Expressions.ToList());
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

        public ServiceResult AddRule(int tenantId, string userName, RuleRowModel model)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                Rule rule = new Rule();
                rule.AddedBy = userName;
                rule.AddedDate = DateTime.Now;
                rule.RuleName = model.RuleName;
                rule.TargetTypeID = model.TargetPropertyId;
                rule.RuleTargetTypeID = 0;
                rule.ResultFailure = model.ResultFailure;
                rule.RuleDescription = model.RuleDescription;
                rule.ResultSuccess = model.ResultSuccess;
                rule.IsResultFailureElement = model.IsResultFailureElement;
                rule.IsResultSuccessElement = model.IsResultSuccessElement;
                rule.Message = model.Message;
                rule.RunOnLoad = model.RunOnLoad;
                rule.IsStandard = model.IsStandard;
                this._unitOfWork.RepositoryAsync<Rule>().Insert(rule);
                this._unitOfWork.Save();

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
                map.UIElementID = model.UIElementID;
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
                result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
                result.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }

            return result;
        }

        public ServiceResult UpdateRule(int tenantId, string userName, RuleRowModel rule)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                Rule ruleToUpdate = this._unitOfWork.RepositoryAsync<Rule>().FindById(rule.RuleId);
                ruleToUpdate.UpdatedBy = userName;
                ruleToUpdate.UpdatedDate = DateTime.Now;
                ruleToUpdate.RuleName = rule.RuleName;
                ruleToUpdate.TargetTypeID = rule.TargetPropertyId;
                ruleToUpdate.RuleDescription = rule.RuleDescription;
                ruleToUpdate.ResultFailure = rule.ResultFailure;
                ruleToUpdate.ResultSuccess = rule.ResultSuccess;
                ruleToUpdate.IsResultFailureElement = rule.IsResultFailureElement;
                ruleToUpdate.IsResultSuccessElement = rule.IsResultSuccessElement;
                ruleToUpdate.Message = rule.Message;
                ruleToUpdate.IsStandard = rule.IsStandard;
                ruleToUpdate.RunOnLoad = rule.RunOnLoad;
                ruleToUpdate.IsStandard = rule.IsStandard;
                this._unitOfWork.RepositoryAsync<Rule>().Update(ruleToUpdate);

                PropertyRuleMap updateRuleMap = this._unitOfWork.RepositoryAsync<PropertyRuleMap>().Get().Where(x => x.RuleID == rule.RuleId).FirstOrDefault();
                if (updateRuleMap != null)
                {
                    updateRuleMap.TargetPropertyID = rule.TargetPropertyId;
                    this._unitOfWork.RepositoryAsync<PropertyRuleMap>().Update(updateRuleMap);
                }

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
                result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
                result.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }

            return result;
        }

        public ServiceResult DeleteRule(int tenantId, int[] rules, int formDesignVersionId)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                foreach (int ruleId in rules)
                {
                    RuleRowModel rule = GetRuleByID(tenantId, ruleId, formDesignVersionId);

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

                    var propMap = this._unitOfWork.RepositoryAsync<PropertyRuleMap>().Get().Where(s => s.RuleID == rule.RuleId).ToList();
                    if (propMap.Count() > 0)
                    {
                        foreach (var item in propMap)
                        {
                            this._unitOfWork.RepositoryAsync<PropertyRuleMap>().Delete(item.PropertyRuleMapID);
                            this._unitOfWork.Save();
                        }
                    }

                    this._unitOfWork.RepositoryAsync<Rule>().Delete(rule.RuleId);
                    this._unitOfWork.Save();
                    result.Result = ServiceResultStatus.Success;
                }
            }
            catch (Exception ex)
            {
                result.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return result;
        }

        public List<RuleRowViewModel> GetSourceByRuleID(int tenantId, int formDesignVersionId, int ruleId)
        {
            List<RuleRowViewModel> sourceList = new List<RuleRowViewModel>();
            try
            {
                SqlParameter paramRuleId = new SqlParameter("@RuleID", ruleId);
                List<RuleModel> ruleModelList = this._unitOfWork.Repository<RuleModel>()
                                                                                .ExecuteSql("exec [dbo].[uspGetSourcesByRuleID] @RuleID", paramRuleId)
                                                                                .ToList();
                sourceList = (from src in ruleModelList
                              select new RuleRowViewModel()
                              {
                                  Section = src.Section.IndexOf(">") > 0 ? src.Section.Substring(0, src.Section.LastIndexOf(" >")) : "",
                                  Element = src.SourceElement,
                                  KeyFilter = src.KeyFilter
                              }).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return sourceList;
        }

        public List<RuleRowViewModel> GetTargetByRuleID(int tenantId, int formDesignVersionId, int ruleId)
        {
            List<RuleRowViewModel> targetList = new List<RuleRowViewModel>();
            try
            {
                SqlParameter paramRuleId = new SqlParameter("@RuleID", ruleId);
                List<RuleModel> ruleModelList = this._unitOfWork.Repository<RuleModel>()
                                                                                .ExecuteSql("exec [dbo].[uspGetTargetsByRuleID] @RuleID", paramRuleId)
                                                                                .ToList();
                targetList = (from src in ruleModelList
                              select new RuleRowViewModel()
                              {
                                  Section = src.Section.IndexOf(">") > 0 ? src.Section.Substring(0, src.Section.LastIndexOf(" >")) : "",
                                  Element = src.TargetElement,
                                  KeyFilter = src.KeyFilter
                              }).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return targetList;
        }

        public ServiceResult AssignTargets(int tenantId, int uiElementId, List<ElementRuleMap> elementMap, string userName)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                List<int> rules = elementMap.Select(s => s.RuleID).Distinct().ToList();
                foreach (var rule in rules)
                {
                    var objRule = (from r in this._unitOfWork.RepositoryAsync<Rule>().Get() where r.RuleID == rule select r).FirstOrDefault();
                    if (objRule != null)
                    {
                        List<int> elementIds = elementMap.Select(s => s.UIElementID).Distinct().ToList();
                        var unassigned = (from map in this._unitOfWork.RepositoryAsync<PropertyRuleMap>().Get()
                                          where map.RuleID == rule && !elementIds.Contains(map.UIElementID)
                                          select map).ToList();
                        if (unassigned.Count > 0)
                        {
                            foreach (var ruleMap in unassigned)
                            {
                                var filters = (from filter in this._unitOfWork.RepositoryAsync<TargetRepeaterKeyFilter>().Get()
                                               where filter.RuleID == rule && filter.PropertyRuleMapID == ruleMap.PropertyRuleMapID
                                               select filter).ToList();
                                foreach (var filter in filters)
                                {
                                    this._unitOfWork.RepositoryAsync<TargetRepeaterKeyFilter>().Delete(filter);
                                    this._unitOfWork.Save();
                                }

                                this._unitOfWork.RepositoryAsync<PropertyRuleMap>().Delete(ruleMap);
                                this._unitOfWork.Save();
                            }
                        }

                        foreach (var element in elementMap)
                        {
                            var propMap = (from map in this._unitOfWork.RepositoryAsync<PropertyRuleMap>().Get()
                                           where map.RuleID == element.RuleID && map.UIElementID == element.UIElementID
                                           select map)
                                           .FirstOrDefault();

                            if (propMap == null)
                            {
                                propMap = new PropertyRuleMap();
                                propMap.AddedBy = userName;
                                propMap.AddedDate = DateTime.Now;
                                propMap.IsCustomRule = false;
                                propMap.RuleID = element.RuleID;
                                propMap.TargetPropertyID = Convert.ToInt32(objRule.TargetTypeID);
                                propMap.UIElementID = element.UIElementID;
                                this._unitOfWork.RepositoryAsync<PropertyRuleMap>().Insert(propMap);
                                this._unitOfWork.Save();
                            }

                            if (element.TargetKeyFilter != null && element.TargetKeyFilter.Count > 0)
                            {
                                var filters = (from filter in this._unitOfWork.RepositoryAsync<TargetRepeaterKeyFilter>().Get()
                                               where filter.RuleID == element.RuleID && filter.PropertyRuleMapID == propMap.PropertyRuleMapID
                                               select filter).ToList();
                                foreach (var filter in filters)
                                {
                                    this._unitOfWork.RepositoryAsync<TargetRepeaterKeyFilter>().Delete(filter);
                                    this._unitOfWork.Save();
                                }

                                foreach (var filter in element.TargetKeyFilter)
                                {
                                    TargetRepeaterKeyFilter objFilter = new TargetRepeaterKeyFilter();
                                    objFilter.RuleID = element.RuleID;
                                    objFilter.RepeaterKey = filter.UIElementPath;
                                    objFilter.RepeaterKeyValue = filter.FilterValue;
                                    objFilter.PropertyRuleMapID = propMap.PropertyRuleMapID;
                                    this._unitOfWork.RepositoryAsync<TargetRepeaterKeyFilter>().Insert(objFilter);
                                    this._unitOfWork.Save();
                                }
                            }
                        }

                        if (objRule.RuleTargetTypeID == 0)
                        {
                            //var dummyMap = this._unitOfWork.RepositoryAsync<PropertyRuleMap>().Get().Where(s => s.UIElementID == uiElementId).ToList();
                            //foreach (var map in dummyMap)
                            //{
                            //    this._unitOfWork.RepositoryAsync<PropertyRuleMap>().Delete(map);
                            //    //this._unitOfWork.Save();
                            //}

                            objRule.RuleTargetTypeID = 1;
                            this._unitOfWork.RepositoryAsync<Rule>().Update(objRule);
                        }
                    }
                }
                this._unitOfWork.Save();
                result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
                result.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }

            return result;
        }

        public List<ElementRuleMap> GetTargetMapByRuleID(int tenantId, int ruleId)
        {
            List<ElementRuleMap> rulemap = new List<ElementRuleMap>();
            try
            {
                rulemap = (from map in this._unitOfWork.RepositoryAsync<PropertyRuleMap>().Get()
                           join r in this._unitOfWork.RepositoryAsync<Rule>().Get() on map.RuleID equals r.RuleID
                           join u in this._unitOfWork.RepositoryAsync<UIElement>().Get() on map.UIElementID equals u.UIElementID
                           where map.RuleID == ruleId && r.RuleTargetTypeID == 1
                           select new ElementRuleMap()
                           {
                               RuleMapID = map.PropertyRuleMapID,
                               RuleID = map.RuleID,
                               UIElementID = u.UIElementID
                           }).ToList();

                foreach (var rule in rulemap)
                {
                    rule.TargetKeyFilter = (from key in this._unitOfWork.RepositoryAsync<TargetRepeaterKeyFilter>().Get()
                                            join u in this._unitOfWork.RepositoryAsync<UIElement>().Get() on key.RepeaterKey equals u.UIElementName
                                            where key.RuleID == rule.RuleID && key.PropertyRuleMapID == rule.RuleMapID
                                            select new TargetKeyFilter()
                                            {
                                                FilterValue = key.RepeaterKeyValue,
                                                Label = u.Label,
                                                UIElementID = u.UIElementID,
                                                UIElementPath = key.RepeaterKey
                                            }).ToList();
                }

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }

            return rulemap;
        }

        public List<RepeaterKeyFilterModel> GetKeyFilter(int ruleId, bool isRightOperand)
        {
            List<RepeaterKeyFilterModel> keyFilterModel = null;
            try
            {
                keyFilterModel = (from keyfilter in _unitOfWork.RepositoryAsync<RepeaterKeyFilter>().Get()
                                  join e in _unitOfWork.RepositoryAsync<Expression>().Get()
                                    on keyfilter.ExpressionID equals e.ExpressionID
                                  join prm in _unitOfWork.RepositoryAsync<PropertyRuleMap>().Get()
                                  on e.RuleID equals prm.RuleID
                                  join ui in _unitOfWork.RepositoryAsync<UIElement>().Get()
                                  on prm.UIElementID equals ui.UIElementID
                                  where e.RuleID == ruleId && keyfilter.IsRightOperand == isRightOperand
                                  select new RepeaterKeyFilterModel
                                  {
                                      UIElementID = ui.UIElementID,
                                      Label = ui.Label,
                                      UIElementPath = keyfilter.RepeaterKey,
                                      FilterValue = keyfilter.RepeaterKeyValue,
                                      isChecked = false
                                  }).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return keyFilterModel;
        }

        public List<ProductTestDataModel> GetOperandNames(int tenantId, int formDesignVersionId, List<ProductTestDataModel> operands)
        {
            try
            {
                SqlParameter paramFrmDesignVersionID = new SqlParameter("@FormDesignVersionID", formDesignVersionId);
                List<FormDesignVersionUIElement> uiElementList = this._unitOfWork.Repository<FormDesignVersionUIElement>()
                                                                .ExecuteSql("exec [dbo].[uspGetFormDesignVersionUIElement] @FormDesignVersionID", paramFrmDesignVersionID)
                                                                .ToList();
                foreach (var element in operands)
                {
                    var elementFullPath = uiElementList.Where(s => s.UIElementName == element.UIElementName).Select(s => s.UIElementFullName).FirstOrDefault();
                    if (elementFullPath != null)
                    {
                        element.UIElementFullName = elementFullPath;
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return operands;
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

        private RuleRowModel GetComplexOperatorForRules(int tenantId, RuleRowModel rowModelList)
        {
            var complexOp = (from c in this._unitOfWork.RepositoryAsync<ComplexOperator>().Get()
                             join e in this._unitOfWork.RepositoryAsync<Expression>().Get() on c.ExpressionID equals e.ExpressionID
                             join r in this._unitOfWork.RepositoryAsync<Rule>().Get() on e.RuleID equals r.RuleID
                             where r.RuleID == rowModelList.RuleId
                             select c
                             ).ToList();
            if (rowModelList.RootExpression.Expressions != null)
            {
                foreach (var rExp in rowModelList.RootExpression.Expressions)
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
            return rowModelList;
        }

        private Dictionary<int, string> GetRuleSources(int tenantId, int formDesignVersionId)
        {
            Dictionary<int, string> ruleSources = new Dictionary<int, string>();

            var sources = (from ex in this._unitOfWork.RepositoryAsync<Expression>().Get()
                           join r in this._unitOfWork.RepositoryAsync<Rule>().Get() on ex.RuleID equals r.RuleID
                           join map in this._unitOfWork.RepositoryAsync<PropertyRuleMap>().Get() on r.RuleID equals map.RuleID
                           join ui in this._unitOfWork.RepositoryAsync<UIElement>().Get() on map.UIElementID equals ui.UIElementID
                           join umap in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Get() on ui.UIElementID equals umap.UIElementID
                           join ele in this._unitOfWork.RepositoryAsync<UIElement>().Get() on ex.LeftOperand equals ele.UIElementName
                           where umap.FormDesignVersionID == formDesignVersionId
                           select new
                           {
                               RuleID = ex.RuleID,
                               ExpressionID = ex.ExpressionID,
                               Target = ele.Label
                           }).ToList();

            var filters = this.GetRuleSourcesKey(tenantId, formDesignVersionId);
            var sourcesWithKeys = (from s in sources
                                   join f in filters on s.ExpressionID equals f.Key into ps
                                   from p in ps.DefaultIfEmpty()
                                   select new
                                   {
                                       RuleID = s.RuleID,
                                       ExpressionID = s.ExpressionID,
                                       Target = p.Key == 0 ? s.Target : s.Target + "[" + p.Value + "]"
                                   }).ToList();

            var results = from s in sourcesWithKeys group s by s.RuleID into g select new { RuleId = g.Key, Exps = g.ToList() };

            foreach (var item in results)
            {
                string[] exps = item.Exps.Select(s => s.Target).ToArray();
                ruleSources.Add(item.RuleId, string.Join(",", exps));
            }

            return ruleSources;
        }

        private Dictionary<int, string> GetRuleSourcesKey(int tenantId, int formDesignVersionId)
        {
            Dictionary<int, string> ruleSources = new Dictionary<int, string>();

            var sources = (from rpt in this._unitOfWork.RepositoryAsync<RepeaterKeyFilter>().Get()
                           join ui in this._unitOfWork.RepositoryAsync<UIElement>().Get() on rpt.RepeaterKey equals ui.UIElementName
                           join umap in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Get() on ui.UIElementID equals umap.UIElementID
                           where umap.FormDesignVersionID == formDesignVersionId
                           select new
                           {
                               KeyID = rpt.RepeaterKeyID,
                               ExpressionID = rpt.ExpressionID,
                               Target = ui.Label + "=" + rpt.RepeaterKeyValue
                           }).ToList();

            var results = from s in sources group s by s.ExpressionID into g select new { ExpId = g.Key, Filters = g.ToList() };

            foreach (var item in results)
            {
                string[] exps = item.Filters.Select(s => s.Target).ToArray();
                ruleSources.Add(item.ExpId, string.Join("&", exps));
            }

            return ruleSources;
        }

        private Dictionary<int, string> GetRuleTargets(int tenantId, int formDesignVersionId)
        {
            Dictionary<int, string> ruleTargets = new Dictionary<int, string>();

            var targets = (from map in this._unitOfWork.RepositoryAsync<PropertyRuleMap>().Get()
                           join ui in this._unitOfWork.RepositoryAsync<UIElement>().Get() on map.UIElementID equals ui.UIElementID
                           join umap in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Get() on ui.UIElementID equals umap.UIElementID
                           where umap.FormDesignVersionID == formDesignVersionId
                           select new
                           {
                               RuleID = map.RuleID,
                               Target = ui.Label
                           }).ToList();

            var filters = this.GetRuleTargetsKey(tenantId, formDesignVersionId);
            var sourcesWithKeys = (from t in targets
                                   join f in filters on t.RuleID equals f.Key into ps
                                   from p in ps.DefaultIfEmpty()
                                   select new
                                   {
                                       RuleID = t.RuleID,
                                       Target = p.Key == 0 ? t.Target : t.Target + "[" + p.Value + "]"
                                   }).ToList();

            var results = from t in targets group t by t.RuleID into g select new { RuleId = g.Key, Targets = g.ToList() };

            foreach (var item in results)
            {
                string[] exps = item.Targets.Select(s => s.Target).ToArray();
                ruleTargets.Add(item.RuleId, string.Join(",", exps));
            }

            return ruleTargets;
        }

        private Dictionary<int, string> GetRuleTargetsKey(int tenantId, int formDesignVersionId)
        {
            Dictionary<int, string> ruleTargets = new Dictionary<int, string>();

            var targets = (from rpt in this._unitOfWork.RepositoryAsync<TargetRepeaterKeyFilter>().Get()
                           join ui in this._unitOfWork.RepositoryAsync<UIElement>().Get() on rpt.RepeaterKey equals ui.UIElementName
                           join map in this._unitOfWork.RepositoryAsync<PropertyRuleMap>().Get() on rpt.PropertyRuleMapID equals map.PropertyRuleMapID
                           join umap in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Get() on ui.UIElementID equals umap.UIElementID
                           where umap.FormDesignVersionID == formDesignVersionId
                           select new
                           {
                               KeyID = rpt.TargetRepeaterKeyID,
                               RuleID = rpt.RuleID,
                               Target = ui.Label + "=" + rpt.RepeaterKeyValue
                           }).ToList();

            var results = from t in targets group t by t.RuleID into g select new { ExpId = g.Key, Filters = g.ToList() };

            foreach (var item in results)
            {
                string[] exps = item.Filters.Select(s => s.Target).ToArray();
                ruleTargets.Add(item.ExpId, string.Join("&", exps));
            }

            return ruleTargets;
        }

        public List<RuleHierarchyViewModel> GetRulesHierarchyByFormDesignVersion(int tenantId, int formDesignVersionId)
        {
            List<RuleHierarchyViewModel> ruleHierarchyViewModelList = null;
            try
            {
                SqlParameter paramFrmDesignVersionID = new SqlParameter("@FormDesignVersionID", formDesignVersionId);
                List<RuleHierarchyModel> ruleHierarchyModelList = this._unitOfWork.Repository<RuleHierarchyModel>()
                                                                                .ExecuteSql("exec [dbo].[uspGetRuleHierarchyForFormDesignVersion] @formDesignVersionID", paramFrmDesignVersionID)
                                                                                .ToList();
                ruleHierarchyViewModelList = (from rl in ruleHierarchyModelList
                                              select new RuleHierarchyViewModel()
                                              {
                                                  GroupID = rl.GroupID,
                                                  IsSourceATarget = rl.IsSourceATarget,
                                                  Level = rl.Level,
                                                  RuleDescription = rl.RuleDescription,
                                                  RuleID = rl.RuleID,
                                                  RuleName = rl.RuleName,
                                                  SourceElementID = rl.SourceElementID,
                                                  SourceElementName = rl.SourceElementName,
                                                  SourceElementPath = rl.SourceElementPath.Substring(rl.SourceElementPath.IndexOf(">") + 2),
                                                  SourceOperand = rl.SourceOperand,
                                                  SourceOperandType = rl.SourceOperandType,
                                                  TargetElementName = rl.TargetElementName,
                                                  TargetProperty = rl.TargetProperty,
                                                  TargetSectionPath = rl.TargetSectionPath.Substring(rl.TargetSectionPath.IndexOf(">") + 2),
                                                  TargetUIElementID = rl.TargetUIElementID
                                              }).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return ruleHierarchyViewModelList;
        }
    }
}
