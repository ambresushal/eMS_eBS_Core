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
using tmg.equinox.applicationservices.viewmodels.FolderVersion;

namespace tmg.equinox.applicationservices
{
    public partial class UIElementService : IUIElementService
    {
        #region Private Memebers

        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor

        #endregion Constructor

        #region Public Methods
        #region Expression Methods
        public ServiceResult AddExpressions(string userName, IEnumerable<ExpressionRowModel> expressions)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                //Add all the expressions in a rule
                foreach (ExpressionRowModel viewmodel in expressions)
                {
                    Expression expressionElement = new Expression
                    {
                        LeftOperand = viewmodel.LeftOperand,
                        LogicalOperatorTypeID = viewmodel.LogicalOperatorTypeId,
                        OperatorTypeID = viewmodel.OperatorTypeId,
                        RightOperand = viewmodel.RightOperand,
                        ExpressionTypeID = viewmodel.ExpressionTypeId,
                        IsRightOperandElement = viewmodel.IsRightOperandElement,
                        RuleID = viewmodel.RuleId,
                        AddedBy = userName,
                        AddedDate = DateTime.Now,
                    };

                    this._unitOfWork.RepositoryAsync<Expression>().Insert(expressionElement);
                }
                using (TransactionScope scope = new TransactionScope())
                {
                    this._unitOfWork.Save();
                    scope.Complete();
                }
                result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                result = ex.ExceptionMessages();
            }
            return result;
        }

        public ServiceResult UpdateExpressions(string userName, IEnumerable<ExpressionRowModel> expressions)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                if (expressions.Count() > 0)
                {
                    //Update all the expressions in a rule
                    foreach (ExpressionRowModel viewmodel in expressions)
                    {
                        Expression expressionElement = this._unitOfWork.RepositoryAsync<Expression>().FindById(viewmodel.ExpressionId);
                        if (expressionElement != null)
                        {
                            expressionElement.LeftOperand = viewmodel.LeftOperand;
                            expressionElement.LogicalOperatorTypeID = viewmodel.LogicalOperatorTypeId;
                            expressionElement.OperatorTypeID = viewmodel.OperatorTypeId;
                            expressionElement.RightOperand = viewmodel.RightOperand;
                            expressionElement.ExpressionTypeID = viewmodel.ExpressionTypeId;
                            expressionElement.IsRightOperandElement = viewmodel.IsRightOperandElement;
                            expressionElement.RuleID = viewmodel.RuleId;
                            expressionElement.UpdatedBy = userName;
                            expressionElement.UpdatedDate = DateTime.Now;

                            this._unitOfWork.RepositoryAsync<Expression>().Update(expressionElement);
                        };
                    }
                    using (TransactionScope scope = new TransactionScope())
                    {
                        this._unitOfWork.Save();
                        scope.Complete();
                    }
                    result.Result = ServiceResultStatus.Success;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                result = ex.ExceptionMessages();
            }
            return result;
        }

        public ServiceResult DeleteExpressions(string userName, int tenantId, int formDesignVersionId, int uiElementId, int ruleId, IEnumerable<int> expressionIds)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                //Delete all the expressions in a rule
                foreach (int expressionId in expressionIds)
                {
                    Expression expressionElement = this._unitOfWork.RepositoryAsync<Expression>().FindById(expressionId);
                    this._unitOfWork.RepositoryAsync<Expression>().Delete(expressionElement);
                }
                using (TransactionScope scope = new TransactionScope())
                {
                    this._unitOfWork.Save();
                    scope.Complete();
                }
                result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                result = ex.ExceptionMessages();
            }
            return result;
        }
        public DateTime GetFormDesignVersionEffectiveDate(int formDesignVersionId)
        {
            DateTime folderVersionEffectiveDate = DateTime.Now;
            try
            {
                var version = this._unitOfWork.RepositoryAsync<FormDesignVersion>()
                                                    .Query()
                                                    .Filter(fil => fil.FormDesignVersionID == formDesignVersionId)
                                                    .Get()
                                                    .Select(sel => sel.EffectiveDate)
                                                    .FirstOrDefault();
                if (version != null)
                {
                    folderVersionEffectiveDate = version.Value;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return folderVersionEffectiveDate;
        }

        public int GetFormDesignVersionID(int FormDesignID, DateTime effecttiveDate)
        {
            var formDesignVersionId = 0;
            FormDesignVersion formDesignVersion = this._unitOfWork.Repository<FormDesignVersion>().Get().Where(x => x.EffectiveDate <= effecttiveDate && x.FormDesignID == FormDesignID).OrderByDescending(i => i.FormDesignVersionID).FirstOrDefault();
            if (formDesignVersion != null)
            {
                formDesignVersionId = formDesignVersion.FormDesignVersionID;
            }
            return formDesignVersionId;
        }
        public List<DocumentViewListViewModel> GetDocumentViewListForExpressionRules(int formDesignId)
        {
            List<DocumentViewListViewModel> documentViewList = null;
            try
            {
                var DocumentDesignTypeID = (from fd in this._unitOfWork.RepositoryAsync<FormDesign>().Get()
                                            where fd.IsActive == true && fd.FormID == formDesignId
                                            select fd).FirstOrDefault().DocumentDesignTypeID;

                int anchorView = 1;
                int masterListView = 2;
                int collateralView = 11;
                int view = 14;

                int[] documentDesignTypes = new int[] { anchorView, masterListView, collateralView, view };
                if (DocumentDesignTypeID == anchorView)
                    documentDesignTypes = new int[] { anchorView, masterListView, view };
                else if (DocumentDesignTypeID == masterListView)
                    documentDesignTypes = new int[] { masterListView };
                else if (DocumentDesignTypeID == collateralView)
                    documentDesignTypes = new int[] { anchorView, masterListView, view };
                else if (DocumentDesignTypeID == view)
                    documentDesignTypes = new int[] { anchorView, masterListView };

                var documentViews = (from c in this._unitOfWork.RepositoryAsync<FormDesign>().Get()
                                     join ddt in this._unitOfWork.RepositoryAsync<DocumentDesignType>().Get() on c.DocumentDesignTypeID equals ddt.DocumentDesignTypeID
                                     join fdg in this._unitOfWork.RepositoryAsync<FormDesignGroupMapping>().Get() on c.FormID equals fdg.FormID
                                     join fdv in this._unitOfWork.RepositoryAsync<FormDesignVersion>().Get() on c.FormID equals fdv.FormDesignID
                                     where c.IsActive == true && (documentDesignTypes.Contains(c.DocumentDesignTypeID) || c.FormID == formDesignId)
                                     select new DocumentViewListViewModel
                                     {
                                         FormDesignTypeName = ddt.DocumentDesignName,
                                         FormDesignID = c.FormID,
                                         FormDesignName = c.FormName,
                                         FormDesignDisplayName = c.DisplayText,
                                     });


                if (documentViews != null)
                {
                    documentViewList = documentViews.Distinct().ToList();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }

            return documentViewList;
        }

        public string GetDocumentRuleJsonData(int dRulesID)
        {
            string documentRuleJson = string.Empty;
            try
            {
                documentRuleJson = (from docRule in this._unitOfWork.RepositoryAsync<DocumentRule>().Get()
                                    where docRule.DocumentRuleID == dRulesID
                                    select docRule).FirstOrDefault().RuleJSON;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return documentRuleJson;
        }
        #endregion Expression Methods
        #endregion Public Methods

        #region Private Methods

        #endregion Private Methods
    }
}
