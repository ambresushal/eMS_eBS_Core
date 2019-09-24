using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Transactions;
using tmg.equinox.applicationservices.ConsumerAccountDetail;
using tmg.equinox.applicationservices.FolderVersionDetail;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.ConsumerAccount;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.applicationservices.viewmodels.Portfolio;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.repository.extensions;
using tmg.equinox.repository.interfaces;
using FolderVersionState = tmg.equinox.domain.entities.Enums.FolderVersionState;
using tmg.equinox.applicationservices.viewmodels.MasterList;
using tmg.equinox.domain.entities.Enums;

namespace tmg.equinox.applicationservices
{
    public class ConsumerAccountService : IConsumerAccountService
    {
        #region Private Member

        private IUnitOfWorkAsync _unitOfWork { get; set; }
        private ILoggingService _loggingService { get; set; }
        private IFolderVersionServices _folderVersionService { get; set; }
        private IWorkFlowStateServices workflowStateService { get; set; }
        private IPlanTaskUserMappingService _planTaskUserMappingService;
        #endregion

        #region Constructor

        public ConsumerAccountService(IUnitOfWorkAsync unitOfWork, ILoggingService loggingService, IFolderVersionServices folderVersionService, IWorkFlowStateServices workflowStateService, IPlanTaskUserMappingService planTaskUserMappingService)
        {
            this._unitOfWork = unitOfWork;
            this._loggingService = loggingService;
            this._folderVersionService = folderVersionService;
            this.workflowStateService = workflowStateService;
            _planTaskUserMappingService = planTaskUserMappingService;
        }

        #endregion

        #region Public Methods

        #region Manage Account

        /// <summary>
        /// Gets the account list.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns></returns>
        public IEnumerable<ConsumerAccountViewModel> GetAccountList(int tenantId)
        {
            IList<ConsumerAccountViewModel> accountList = null;
            try
            {
                accountList = (from c in this._unitOfWork.RepositoryAsync<Account>()
                                              .Query()
                                              .Filter(c => c.TenantID == tenantId & c.IsActive == true)
                                              .Get()
                               select new ConsumerAccountViewModel
                               {
                                   TenantID = c.TenantID,
                                   AccountID = c.AccountID,
                                   AccountName = c.AccountName
                               }).ToList();

                if (accountList.Count() == 0)
                    accountList = null;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }

            return accountList;
        }

        /// <summary>
        /// Adds the account.
        /// </summary>
        /// <param name="tenantId">tenant identifier.</param>
        /// <param name="accountName">Name of the account.</param>
        /// <param name="addedBy">added by.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Account Name already exists</exception>
        public ServiceResult AddAccount(int tenantId, string accountName, string addedBy)
        {
            Contract.Requires(!string.IsNullOrEmpty(accountName), "Account Name cannot be empty");
            Contract.Requires(tenantId > 0, "Invalid tenantId");
            ServiceResult result = null;
            try
            {
                result = new ServiceResult();
                //since system is adding new account,accountId will be passed as 0 to check the accountName with other accounts.
                int accountId = 0;
                if (!this._unitOfWork.RepositoryAsync<Account>().IsAccountNameExists(tenantId, accountId, accountName))
                {

                    Account accountToAdd = new Account();
                    accountToAdd.TenantID = tenantId;
                    accountToAdd.AccountName = accountName;
                    accountToAdd.AddedBy = addedBy;
                    accountToAdd.AddedDate = DateTime.Now;
                    accountToAdd.IsActive = true;

                    this._unitOfWork.RepositoryAsync<Account>().Insert(accountToAdd);
                    this._unitOfWork.Save();

                    result.Result = ServiceResultStatus.Success;
                    ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem() { Messages = new string[] { accountToAdd.AccountID.ToString() } });
                }
                else
                {
                    result.Result = ServiceResultStatus.Failure;
                }
            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;

                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return result;
        }

        /// <summary>
        /// Updates the account.
        /// </summary>
        /// <param name="tenantId">tenant identifier.</param>
        /// <param name="accountId">account identifier.</param>
        /// <param name="accountName">Name of the account.</param>
        /// <param name="updatedBy">updated by.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">
        /// Account Name Alraedy exists
        /// or
        /// Account Does Not exists
        /// </exception>
        public ServiceResult UpdateAccount(int tenantId, int accountId, string accountName, string updatedBy)
        {
            Contract.Requires(accountId > 0, "Invalid accountId");
            Contract.Requires(!string.IsNullOrEmpty(accountName), "Account Name cannot be empty");
            Contract.Requires(tenantId > 0, "Invalid tenantId");
            ServiceResult result = null;
            try
            {
                result = new ServiceResult();
                Account account = _unitOfWork.RepositoryAsync<Account>().FindById(accountId);

                if (account != null)
                {
                    if (!this._unitOfWork.RepositoryAsync<Account>().IsAccountNameExists(tenantId, accountId, accountName))
                    {
                        account.AccountName = accountName;
                        account.UpdatedBy = updatedBy;
                        account.UpdatedDate = DateTime.Now;

                        this._unitOfWork.RepositoryAsync<Account>().Update(account);
                        this._unitOfWork.Save();

                        result.Result = ServiceResultStatus.Success;
                    }
                    else
                    {
                        result.Result = ServiceResultStatus.Failure;
                    }
                }
                else
                {
                    result.Result = ServiceResultStatus.Failure;
                    throw new Exception("Account Does Not exists");
                }
            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;

                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return result;
        }

        /// <summary>
        /// Deletes the account.
        /// </summary>
        /// <param name="tenantId">tenant identifier.</param>
        /// <param name="accountId">account identifier.</param>
        /// <param name="updatedBy">updated by.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Account Does Not  exists</exception>
        public ServiceResult DeleteAccount(int tenantId, int accountId, string updatedBy)
        {
            Contract.Requires(accountId > 0, "Invalid accountId");
            Contract.Requires(tenantId > 0, "Invalid tenantId");
            ServiceResult result = null;
            try
            {
                result = new ServiceResult();
                Account account = _unitOfWork.RepositoryAsync<Account>().FindById(accountId);

                if (account != null)
                {
                    account.UpdatedBy = updatedBy;
                    account.UpdatedDate = DateTime.Now;
                    account.IsActive = false;

                    this._unitOfWork.RepositoryAsync<Account>().Update(account);
                    this._unitOfWork.Save();

                    result.Result = ServiceResultStatus.Success;
                }
                else
                {
                    result.Result = ServiceResultStatus.Failure;
                    throw new Exception("Account Does Not  exists");
                }
            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;

                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return result;
        }

        public string GetAccountName(int tenantId, int accountId)
        {
            string accountName = null;
            try
            {
                accountName = this._unitOfWork.RepositoryAsync<Account>().Query().Filter(a => a.AccountID == accountId).Get().FirstOrDefault().AccountName;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return accountName;
        }

        //Get AccountPropertyPath from FormDesignAccountPropertyMap
        public IEnumerable<FormDesignAccountViewModel> GetFormDesignAccountMapping(int formDesignVersionId)
        {
            List<FormDesignAccountViewModel> formDesignAccountMappingList = null;

            try
            {
                formDesignAccountMappingList = (from c in this._unitOfWork.RepositoryAsync<FormDesignAccountPropertyMap>()
                                                                 .Query()
                                                                 .Filter(c => c.FormDesignVersionID == formDesignVersionId)
                                                                 .Get()
                                                select new FormDesignAccountViewModel
                                                {
                                                    PropertyName = c.AccountPropertyName,
                                                    PropertyPath = c.AccountPropertyPath

                                                }).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return formDesignAccountMappingList;
        }


        //Add Or Update producttype,productname value to AccountProductMap table
        public ServiceResult UpdateAccountProductMap(int tenantId, int formInstanceId, int folderId, int folderVersionId, string propertyType, string propertyId, string planCode, string userName, string serviceGroup, string productName, string anocChartPlanType, string rXBenefit, string sNPType)
        {
            ServiceResult result = null;
            try
            {
                result = new ServiceResult();

                var accountProductMap = this._unitOfWork.RepositoryAsync<AccountProductMap>()
                                                                .Query()
                                                                .Filter(c => c.FormInstanceID == formInstanceId && c.TenantID == tenantId)
                                                                .Get().FirstOrDefault();

                if (accountProductMap != null)
                {
                    //update producttype,productname,updateddate and updateby in AccountProductMap                   
                    accountProductMap.ProductType = propertyType;
                    accountProductMap.ProductID = propertyId;
                    accountProductMap.ServiceGroup = serviceGroup;
                    accountProductMap.PlanCode = planCode;
                    accountProductMap.ANOCChartPlanType = anocChartPlanType;
                    accountProductMap.RXBenefit = rXBenefit;
                    accountProductMap.SNPType = sNPType;
                    accountProductMap.UpdatedDate = DateTime.Now;
                    accountProductMap.UpdatedBy = userName;
                    accountProductMap.ProductName = productName;

                    this._unitOfWork.RepositoryAsync<AccountProductMap>().Update(accountProductMap);
                    this._unitOfWork.Save();

                    result.Result = ServiceResultStatus.Success;
                    //update PBPMatchConfig if map or update  user action is selected 
                    result = this.UpdatePBPMatchConfig(folderId, folderVersionId, formInstanceId, propertyId, propertyType, userName);
                }
                else
                {
                    //add data to AccountProductMap Table
                    result = this.AddAccountProductMap(tenantId, formInstanceId, folderId, folderVersionId, propertyType, propertyId, planCode, userName, serviceGroup, anocChartPlanType, productName);
                }
            }
            catch (Exception ex)
            {

                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return result;
        }

        //Add Or Update producttype,productname value to AccountProductMap table
        public ServiceResult UpdatePBPImportDetailsMap(int tenantId, int formInstanceId, int folderId, int folderVersionId, string planname, string plannumber, int docId, string userName)
        {
            ServiceResult result = null;
            try
            {
                result = new ServiceResult();

                var pbpdetail = this._unitOfWork.RepositoryAsync<PBPImportDetails>()
                                                                .Query()
                                                                .Filter(c => c.FormInstanceID == formInstanceId && c.IsActive == true)
                                                                .Get().FirstOrDefault();

                if (pbpdetail != null)
                {
                    //update producttype,productname,updateddate and updateby in AccountProductMap                   
                    pbpdetail.ebsPlanName = planname;
                    pbpdetail.ebsPlanNumber = plannumber;
                    pbpdetail.FormInstanceID = formInstanceId;
                    pbpdetail.UpdatedDate = DateTime.Now;
                    pbpdetail.UpdatedBy = userName;

                    this._unitOfWork.RepositoryAsync<PBPImportDetails>().Update(pbpdetail);
                    this._unitOfWork.Save();

                    result.Result = ServiceResultStatus.Success;
                }
                else
                {
                    //add data to AccountProductMap Table
                    result = this.AddPBPImportDetailsMap(tenantId, formInstanceId, folderId, folderVersionId, planname, plannumber, docId, userName);
                }
            }
            catch (Exception ex)
            {

                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return result;
        }

        private ServiceResult AddPBPImportDetailsMap(int tenantId, int formInstanceId, int folderId, int folderVersionId, string planname, string plannumber, int docId, string userName)
        {
            ServiceResult result = null;
            try
            {
                result = new ServiceResult();

                PBPImportDetails pbpDetailsMap = new PBPImportDetails();
                pbpDetailsMap.FormInstanceID = formInstanceId;
                pbpDetailsMap.FolderId = folderId;
                pbpDetailsMap.FolderVersionId = folderVersionId;
                pbpDetailsMap.QID = plannumber;
                pbpDetailsMap.PlanName = planname;
                pbpDetailsMap.PlanNumber = plannumber;
                pbpDetailsMap.CreatedDate = DateTime.Now;
                pbpDetailsMap.CreatedBy = userName;
                pbpDetailsMap.IsActive = true;
                pbpDetailsMap.DocId = docId;

                this._unitOfWork.RepositoryAsync<PBPImportDetails>().Insert(pbpDetailsMap);
                this._unitOfWork.Save();

                result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;

            }
            return result;
        }

        private ServiceResult UpdatePBPMatchConfig(int folderId, int folderversionId, int forminstanceId, string productId, string productType, string userName)
        {
            ServiceResult Result = new ServiceResult();
            try
            {
                var pbpdetail = this._unitOfWork.RepositoryAsync<PBPMatchConfig>()
                                .Query()
                                .Filter(c => c.FolderId.Equals(folderId)
                                && c.FolderVersionId.Equals(folderversionId)
                                && c.FormInstanceID.Equals(forminstanceId)
                                && c.IsActive.Equals(true)
                                && (c.UserAction.Equals((int)PBPUserActionList.MapItWithAnothereBSPlan)
                                || c.UserAction.Equals((int)PBPUserActionList.UpdatePlan))
                                )
                                .Get().FirstOrDefault();

                if (pbpdetail != null)
                {
                    //update producttype,productname,updateddate and updateby in AccountProductMap                   
                    pbpdetail.ebsPlanName = productType;
                    pbpdetail.ebsPlanNumber = productId;
                    pbpdetail.UpdatedDate = DateTime.Now;
                    pbpdetail.UpdatedBy = userName;
                    this._unitOfWork.RepositoryAsync<PBPMatchConfig>().Update(pbpdetail);
                    this._unitOfWork.Save();
                    Result.Result = ServiceResultStatus.Success;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return Result;
        }

        #endregion

        #region Account Search

        /// <summary>
        /// Gets the details of Portfolio Folders list.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns></returns>
        public IEnumerable<ConsumerAccountViewModel> GetPortfolioFoldersList(int tenantID)
        {
            List<ConsumerAccountViewModel> accountDetailsList = null;
            return accountDetailsList;
        }

        /// <summary>
        /// Gets the details of Non-Portfolio Folders list.
        /// </summary>
        /// <param name="tenantID">The tenant identifier.</param>
        /// <returns></returns>
        public GridPagingResponse<ConsumerAccountViewModel> GetNonPortfolioFoldersList(int tenantID, GridPagingRequest gridPagingRequest, string documentName, int RoleID)
        {
            List<ConsumerAccountViewModel> accountDetailsList = null;
            int count = 0;

            try
            {
                SearchCriteria criteria = new SearchCriteria();
                criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);
                var currentFolders = from s in "st"
                                     select new
                                     {
                                         FolderID = 0,
                                         FolderVersionID = 0,
                                         VersionCount = 0
                                     };
                if (!String.IsNullOrEmpty(documentName))
                {
                    currentFolders = from s in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                     join f in this._unitOfWork.RepositoryAsync<FormInstance>().Get().Where(f => f.Name.Contains(documentName) && f.IsActive == true)
                                     on s.FolderVersionID equals f.FolderVersionID
                                     join fd in this._unitOfWork.RepositoryAsync<FormDesign>().Get().Where(fd => fd.IsMasterList != true && fd.DocumentDesignTypeID == 1)
                                     on f.FormDesignID equals fd.FormID
                                     join fl in this._unitOfWork.RepositoryAsync<Folder>().Get().Where(fld => fld.IsPortfolio != true)
                                     on s.FolderID equals fl.FolderID
                                     group s by s.FolderID into g
                                     select new { FolderID = g.Key, FolderVersionID = g.Max(s => s.FolderVersionID), VersionCount = g.Distinct().Count() };
                }
                else
                {
                    currentFolders = from s in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                     join fl in this._unitOfWork.RepositoryAsync<Folder>().Get().Where(fld => fld.IsPortfolio != true)
                                     on s.FolderID equals fl.FolderID
                                     group s by s.FolderID into g
                                     select new { FolderID = g.Key, FolderVersionID = g.Max(s => s.FolderVersionID), VersionCount = g.Where(x => x.FolderVersionStateID != (int)FolderVersionState.BASELINED).Distinct().Count() };

                }
                //var TPAANALYST = Convert.ToInt32(Role.WCSuperUser);
                accountDetailsList = (from fldv in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                      join fldr in this._unitOfWork.RepositoryAsync<Folder>().Get() on fldv.FolderID equals fldr.FolderID
                                      join c in currentFolders on new { fldv.FolderID, fldv.FolderVersionID } equals new { c.FolderID, c.FolderVersionID }
                                      join cat in this._unitOfWork.RepositoryAsync<FolderVersionCategory>().Get()   //.Where(ca => ca.FolderVersionCategoryID == ca.FolderVersionCategoryID)
                                          on fldv.CategoryID equals cat.FolderVersionCategoryID
                                      join map in this._unitOfWork.RepositoryAsync<AccountFolderMap>().Get() on fldr.FolderID equals map.FolderID
                                      join accn in this._unitOfWork.RepositoryAsync<Account>().Get() on map.AccountID equals accn.AccountID
                                      join st in this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().Get() on fldv.WFStateID equals st.WorkFlowVersionStateID
                                      join fldvws in this._unitOfWork.RepositoryAsync<FolderVersionWorkFlowState>().Get() on new { p1 = fldv.FolderVersionID, p2 = fldv.WFStateID }
                                      equals new { p1 = fldvws.FolderVersionID, p2 = (int?)fldvws.WFStateID }

                                      where (accn.TenantID == tenantID
                                            && accn.IsActive == true
                                            && (fldr.IsPortfolio != true)
                                            && (fldv.FolderVersionStateID != (int)FolderVersionState.BASELINED)
                                             )
                                      select new ConsumerAccountViewModel
                                      {
                                          RowID = accn.AccountID + "_" + fldv.FolderVersionID,
                                          TenantID = accn.TenantID,
                                          AccountID = accn.AccountID,
                                          AccountName = accn.AccountName,
                                          FolderID = fldr.FolderID,
                                          Portfolio = fldr.IsPortfolio,
                                          FolderName = fldr.Name,
                                          FolderVersionID = fldv.FolderVersionID,
                                          VersionNumber = fldv.FolderVersionNumber,
                                          EffectiveDate = fldv.EffectiveDate,
                                          Status = st.WorkFlowState.WFStateName,
                                          CategoryID = cat.FolderVersionCategoryID,
                                          CategoryName = cat.FolderVersionCategoryName,
                                          UpdatedBy = fldr.UpdatedBy,
                                          UpdatedDate = fldr.UpdatedDate,
                                          FolderVersionCount = c.VersionCount,
                                          ApprovalStatusID = fldvws.ApprovalStatusID,
                                          FolderVersionStateID = fldv.FolderVersionStateID,
                                      }).OrderByDescending(x => x.UpdatedDate).ToList().ApplySearchCriteria(criteria).ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord)
                                 .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return new GridPagingResponse<ConsumerAccountViewModel>(gridPagingRequest.page, count, gridPagingRequest.rows, accountDetailsList);
        }

        public GridPagingResponse<PortfolioViewModel> GetPortfolioDetailsList(int tenantId, GridPagingRequest gridPagingRequest, string documentName, int year)
        {
            //Contract.Requires(tenantId > 0, "Invalid tenantId");
            int count = 0;
            List<PortfolioViewModel> portfolioDetailsList = new List<PortfolioViewModel>();
            SearchCriteria criteria = new SearchCriteria();
            criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);
            var detailsWithCommaSeparatedList = new List<PortfolioViewModel>();
            try
            {
                var currentFolders = from s in "st"
                                     select new
                                     {
                                         FolderID = 0,
                                         FolderVersionID = 0,
                                         VersionCount = 0
                                     };

                List<AccountProductMap> ProductList = this._unitOfWork.RepositoryAsync<AccountProductMap>().Get().Where(t => t.IsActive == true).ToList();


                if (!String.IsNullOrEmpty(documentName))
                {
                    currentFolders = from s in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                     join f in this._unitOfWork.RepositoryAsync<FormInstance>().Get().Where(f => f.Name.Contains(documentName) && f.IsActive == true)
                                     on s.FolderVersionID equals f.FolderVersionID
                                     join fd in this._unitOfWork.RepositoryAsync<FormDesign>().Get().Where(fd => fd.IsMasterList != true && fd.DocumentDesignTypeID == 1)
                                     on f.FormDesignID equals fd.FormID
                                     join fl in this._unitOfWork.RepositoryAsync<Folder>().Get().Where(fld => fld.IsPortfolio == true)
                                     on s.FolderID equals fl.FolderID
                                     group s by s.FolderID into g
                                     select new { FolderID = g.Key, FolderVersionID = g.Max(s => s.FolderVersionID), VersionCount = g.Distinct().Count() };
                }
                else
                {
                    currentFolders = from s in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                     join fl in this._unitOfWork.RepositoryAsync<Folder>().Get().Where(fld => fld.IsPortfolio == true)
                                     on s.FolderID equals fl.FolderID
                                     group s by s.FolderID into g
                                     select new { FolderID = g.Key, FolderVersionID = g.Max(s => s.FolderVersionID), VersionCount = g.Where(x => x.FolderVersionStateID != (int)FolderVersionState.BASELINED).Distinct().Count() };
                }
                portfolioDetailsList = (from fldv in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                        join c in currentFolders on new { fldv.FolderID, fldv.FolderVersionID } equals new { c.FolderID, c.FolderVersionID }
                                        join fol in this._unitOfWork.RepositoryAsync<Folder>().Query().Get()
                                        on fldv.FolderID equals fol.FolderID
                                            into obj
                                        from p in obj.DefaultIfEmpty()
                                        join cat in this._unitOfWork.RepositoryAsync<FolderVersionCategory>().Get()
                                            on fldv.CategoryID equals cat.FolderVersionCategoryID
                                        where (p.TenantID == tenantId && p.IsPortfolio == true)
                                        select new PortfolioViewModel
                                        {
                                            FolderName = p.Name,
                                            FolderID = p.FolderID,
                                            MarketSegmentID = p.MarketSegmentID,
                                            PrimaryContactID = p.PrimaryContentID,
                                            FolderVersionID = fldv.FolderVersionID,
                                            VersionNumber = fldv.FolderVersionNumber,
                                            EffectiveDate = fldv.EffectiveDate,
                                            Status = fldv.WorkFlowVersionState.WorkFlowState.WFStateName,
                                            ApprovalStatusID = fldv.FolderVersionWorkFlowStates.Where(s => s.FolderVersionID == fldv.FolderVersionID && s.IsActive == true && s.WFStateID == fldv.WFStateID).Select(x => x.ApprovalStatusID).FirstOrDefault(),
                                            FolderVersionCount = p.FolderVersions.Where(row => row.FolderVersionStateID != (int)FolderVersionState.BASELINED).Count(),
                                            TenantID = tenantId,
                                            CategoryID = cat.FolderVersionCategoryID,
                                            CategoryName = cat.FolderVersionCategoryName,
                                            UpdatedBy = fldv.UpdatedBy,
                                            UpdatedDate = fldv.UpdatedDate,
                                            Mode = fldv.FolderVersionStateID == (int)FolderVersionState.INPROGRESS,
                                            IsFoundation = (p.IsFoundation) ? "Yes" : "No"
                                        }).OrderByDescending(x => x.UpdatedDate).ToList();


                if (year != 0)
                {
                    portfolioDetailsList = portfolioDetailsList
                                         .Where(s => s.EffectiveDate.Value.Year.Equals(year))
                                         .ToList();
                }
                portfolioDetailsList = portfolioDetailsList.ApplySearchCriteria(criteria).ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord)
                                        .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count);

                // code for showing pipe seprated hybrid workflow state 
                foreach (var model in portfolioDetailsList)
                {
                    model.Status = null;
                    IList<KeyValue> workFlowList = null;
                    if (model.FolderVersionID != null && model.TenantID != 0)
                    {
                        workFlowList = this.workflowStateService.GetCurrentWorkFlowState(model.TenantID, (int)model.FolderVersionID).ToList();
                    }
                    if (workFlowList != null && workFlowList.Count > 0)
                    {
                        if (workFlowList.Count() > 0)
                        {
                            foreach (var nm in workFlowList)
                            {
                                if (model.Status == null)
                                {
                                    model.Status = nm.Value;
                                }
                                else
                                {
                                    model.Status = model.Status + '|' + nm.Value;
                                }
                            }
                        }
                    }
                    model.ProductList = GetProductList(model.FolderID, ProductList);
                }


            }

            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }

            return new GridPagingResponse<PortfolioViewModel>(gridPagingRequest.page, count, gridPagingRequest.rows, portfolioDetailsList);
        }

        public GridPagingResponse<DocumentViewModel> GetDocumentsList(int tenantID, GridPagingRequest gridPagingRequest)
        {
            return GetDocumentsList(tenantID, 0, gridPagingRequest);
        }

        public GridPagingResponse<DocumentViewModel> GetDocumentsListForQHP(int tenantID, GridPagingRequest gridPagingRequest)
        {
            List<DocumentViewModel> documentList = null;
            int count = 0;

            try
            {
                SearchCriteria criteria = new SearchCriteria();
                criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);

                documentList = (from fi in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                                join fv in this._unitOfWork.RepositoryAsync<FolderVersion>().Get() on fi.FolderVersionID equals fv.FolderVersionID
                                join fldr in this._unitOfWork.RepositoryAsync<Folder>().Get() on fv.FolderID equals fldr.FolderID
                                join map in this._unitOfWork.RepositoryAsync<AccountProductMap>().Get() on fi.FormInstanceID equals map.FormInstanceID
                                join fvs in this._unitOfWork.RepositoryAsync<tmg.equinox.domain.entities.Models.FolderVersionState>().Get() on fv.FolderVersionStateID equals fvs.FolderVersionStateID
                                join frm in this._unitOfWork.RepositoryAsync<FormDesign>().Get() on fi.FormDesignID equals frm.FormID
                                join design in this._unitOfWork.RepositoryAsync<DocumentDesignType>().Get() on frm.DocumentDesignTypeID equals design.DocumentDesignTypeID
                                where fldr.Name != "Master List" && fi.IsActive == true && (map.ServiceGroup == "Yes")//ExchangeOffering
                                && (fvs.FolderVersionStateName == "In Progress" || fvs.FolderVersionStateName == "Released")
                                && design.DocumentDesignTypeID == 1
                                select new DocumentViewModel
                                {
                                    RowID = fv.FolderVersionID + "-" + fi.FormInstanceID,
                                    TenantID = fv.TenantID,
                                    AccountID = 0,
                                    AccountName = "",
                                    FolderID = fldr.FolderID,
                                    FolderName = fldr.Name,
                                    ConsortiumName = "",
                                    FolderVersionID = fv.FolderVersionID,
                                    FolderVersionStateName = fvs.FolderVersionStateName,
                                    VersionNumber = fv.FolderVersionNumber,
                                    EffectiveDate = fv.EffectiveDate,
                                    FormInstanceID = fi.FormInstanceID,
                                    FormInstanceName = fi.Name,
                                    DocumentId = fi.DocID,
                                    FormDesignID = fi.FormDesignID,
                                    DesignType = frm.DisplayText,
                                    MarketType = map.ProductName,
                                    State = map.ANOCChartPlanType,
                                    CSRVariationType = map.PlanCode
                                }).ToList()
                                .ApplySearchCriteria(criteria).ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord)
                                .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count)
                                .ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return new GridPagingResponse<DocumentViewModel>(gridPagingRequest.page, count, gridPagingRequest.rows, documentList);
        }

        /// <summary>
        /// Gets the details of Non-Portfolio Folders list.
        /// </summary>
        /// <param name="tenantID">The tenant identifier.</param>
        /// <returns></returns>
        public GridPagingResponse<DocumentViewModel> GetDocumentsList(int tenantID, int formDesignID, GridPagingRequest gridPagingRequest)
        {
            List<DocumentViewModel> documentList = null;
            int count = 0;

            try
            {
                SearchCriteria criteria = new SearchCriteria();
                criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);

                documentList = (from fi in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                                join fv in this._unitOfWork.RepositoryAsync<FolderVersion>().Get() on fi.FolderVersionID equals fv.FolderVersionID
                                join fldr in this._unitOfWork.RepositoryAsync<Folder>().Get() on fv.FolderID equals fldr.FolderID
                                join fvs in this._unitOfWork.RepositoryAsync<tmg.equinox.domain.entities.Models.FolderVersionState>().Get() on fv.FolderVersionStateID equals fvs.FolderVersionStateID
                                join frm in this._unitOfWork.RepositoryAsync<FormDesign>().Get()
                                on fi.FormDesignID equals frm.FormID
                                where fldr.Name != "Master List"
                                && (formDesignID == 0 || fi.FormDesignID == formDesignID)
                                select new DocumentViewModel
                                {
                                    RowID = fv.FolderVersionID + "-" + fi.FormInstanceID,
                                    TenantID = fv.TenantID,
                                    AccountID = 0,
                                    AccountName = "",
                                    FolderID = fldr.FolderID,
                                    FolderName = fldr.Name,
                                    ConsortiumName = "",
                                    FolderVersionID = fv.FolderVersionID,
                                    FolderVersionStateName = fvs.FolderVersionStateName,
                                    VersionNumber = fv.FolderVersionNumber,
                                    EffectiveDate = fv.EffectiveDate,
                                    FormInstanceID = fi.FormInstanceID,
                                    FormInstanceName = fi.Name,
                                    DocumentId = fi.DocID,
                                    FormDesignID = fi.FormDesignID,
                                    DesignType = frm.DisplayText ?? "NA"
                                }).ToList();


                if (gridPagingRequest.sidx != null)
                {
                    documentList = documentList.ApplySearchCriteria(criteria).ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord)
                                    .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count)
                                    .ToList();
                }
                else
                {
                    documentList = documentList.ToList();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return new GridPagingResponse<DocumentViewModel>(gridPagingRequest.page, count, gridPagingRequest.rows, documentList);
        }
        public GridPagingResponse<DocumentViewModel> GetDocumentsList(int tenantID, int formDesignID, GridPagingRequest gridPagingRequest, string appName)
        {
            List<DocumentViewModel> documentList = null;
            int count = 0;

            try
            {
                SearchCriteria criteria = new SearchCriteria();
                criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);

                documentList = (from fi in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                                join fv in this._unitOfWork.RepositoryAsync<FolderVersion>().Get() on fi.FolderVersionID equals fv.FolderVersionID
                                join fldr in this._unitOfWork.RepositoryAsync<Folder>().Get() on fv.FolderID equals fldr.FolderID
                                join map in this._unitOfWork.RepositoryAsync<AccountFolderMap>().Get() on fldr.FolderID equals map.FolderID into gj
                                from grp in gj.DefaultIfEmpty()
                                join accn in this._unitOfWork.RepositoryAsync<Account>().Get() on grp.AccountID equals accn.AccountID into ac
                                from acnt in ac.DefaultIfEmpty()
                                join fvs in this._unitOfWork.RepositoryAsync<tmg.equinox.domain.entities.Models.FolderVersionState>().Get() on fv.FolderVersionStateID equals fvs.FolderVersionStateID
                                join frm in this._unitOfWork.RepositoryAsync<FormDesign>().Get()
                                on fi.FormDesignID equals frm.FormID
                                where fldr.Name != "Master List"
                                      &&
                                      fi.IsActive == true

                                && (formDesignID == 0 || fi.FormDesignID == formDesignID)
                                select new DocumentViewModel
                                {
                                    RowID = fv.FolderVersionID + "-" + fi.FormInstanceID,
                                    TenantID = fv.TenantID,
                                    AccountID = acnt.AccountID,
                                    AccountName = acnt.AccountName,
                                    FolderID = fldr.FolderID,
                                    FolderName = fldr.Name,
                                    ConsortiumName = "",
                                    FolderVersionID = fv.FolderVersionID,
                                    FolderVersionStateName = fvs.FolderVersionStateName,
                                    VersionNumber = fv.FolderVersionNumber,
                                    EffectiveDate = fv.EffectiveDate,
                                    FormInstanceID = fi.FormInstanceID,
                                    FormInstanceName = fi.Name,
                                    DocumentId = fi.DocID,
                                    FormDesignID = fi.FormDesignID,
                                    DesignType = frm.DisplayText ?? "NA"
                                }).ToList();

                if (gridPagingRequest.sidx != null)
                {
                    documentList = documentList.ApplySearchCriteria(criteria).ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord)
                                .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count)
                                .ToList();
                }
                else
                {
                    documentList = documentList.ToList();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return new GridPagingResponse<DocumentViewModel>(gridPagingRequest.page, count, gridPagingRequest.rows, documentList);
        }

        public GridPagingResponse<DocumentViewModel> GetDocumentsList(int tenantID, int formDesignID,int formInstanceId, GridPagingRequest gridPagingRequest, string appName)
        {
            List<DocumentViewModel> documentList = null;
            int count = 0;

            try
            {
                SearchCriteria criteria = new SearchCriteria();
                criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);

                documentList = (from fi in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                                join fv in this._unitOfWork.RepositoryAsync<FolderVersion>().Get() on fi.FolderVersionID equals fv.FolderVersionID
                                join fldr in this._unitOfWork.RepositoryAsync<Folder>().Get() on fv.FolderID equals fldr.FolderID
                                join map in this._unitOfWork.RepositoryAsync<AccountFolderMap>().Get() on fldr.FolderID equals map.FolderID into gj
                                from grp in gj.DefaultIfEmpty()
                                join accn in this._unitOfWork.RepositoryAsync<Account>().Get() on grp.AccountID equals accn.AccountID into ac
                                from acnt in ac.DefaultIfEmpty()
                                join fvs in this._unitOfWork.RepositoryAsync<tmg.equinox.domain.entities.Models.FolderVersionState>().Get() on fv.FolderVersionStateID equals fvs.FolderVersionStateID
                                join frm in this._unitOfWork.RepositoryAsync<FormDesign>().Get()
                                on fi.FormDesignID equals frm.FormID
                                where fldr.Name != "Master List"
                                      && fi.FormInstanceID != formInstanceId
                                      &&
                                      fi.IsActive == true

                                && (formDesignID == 0 || fi.FormDesignID == formDesignID)
                                select new DocumentViewModel
                                {
                                    RowID = fv.FolderVersionID + "-" + fi.FormInstanceID,
                                    TenantID = fv.TenantID,
                                    AccountID = acnt.AccountID,
                                    AccountName = acnt.AccountName,
                                    FolderID = fldr.FolderID,
                                    FolderName = fldr.Name,
                                    ConsortiumName = "",
                                    FolderVersionID = fv.FolderVersionID,
                                    FolderVersionStateName = fvs.FolderVersionStateName,
                                    VersionNumber = fv.FolderVersionNumber,
                                    EffectiveDate = fv.EffectiveDate,
                                    FormInstanceID = fi.FormInstanceID,
                                    FormInstanceName = fi.Name,
                                    DocumentId = fi.DocID,
                                    FormDesignID = fi.FormDesignID,
                                    DesignType = frm.DisplayText ?? "NA"
                                }).ToList();


                if (gridPagingRequest.sidx != null)
                {
                    documentList = documentList.ApplySearchCriteria(criteria).ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord)
                                .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count)
                                .ToList();
                }
                else
                {
                    documentList = documentList.ToList();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return new GridPagingResponse<DocumentViewModel>(gridPagingRequest.page, count, gridPagingRequest.rows, documentList);
        }

        /// <summary>
        /// Gets the details of Portfolio Folders list.
        /// </summary>
        /// <param name="tenantID">The tenant identifier.</param>
        /// <returns></returns>
        public GridPagingResponse<PortfolioFoldersDocumentViewModel> GetPortfolioFoldersDocumentsList(int tenantID, GridPagingRequest gridPagingRequest, int year)
        {
            List<PortfolioFoldersDocumentViewModel> documentList = null;
            int count = 0;
            int FormID = this._unitOfWork.RepositoryAsync<FormDesign>().Get()
                                 .Where(s => s.FormName.Equals("Medicare")).Select(s => s.FormID).FirstOrDefault();
            try
            {
                SearchCriteria criteria = new SearchCriteria();
                criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);
                List<PBPImportDetails> PBPDetailsList = this._unitOfWork.RepositoryAsync<PBPImportDetails>()
                    .Get().Where(s => s.IsActive == true && s.PBPDatabase1Up.Equals(0))
                    .ToList();
                documentList = (from fi in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                                join fv in this._unitOfWork.RepositoryAsync<FolderVersion>().Get() on fi.FolderVersionID equals fv.FolderVersionID
                                join fvs in this._unitOfWork.RepositoryAsync<tmg.equinox.domain.entities.Models.FolderVersionState>().Get() on fv.FolderVersionStateID equals fvs.FolderVersionStateID
                                join fldr in this._unitOfWork.RepositoryAsync<Folder>().Get() on fv.FolderID equals fldr.FolderID
                                join accprodmap in this._unitOfWork.RepositoryAsync<AccountProductMap>().Get()
                               on fi.FormInstanceID equals accprodmap.FormInstanceID
                                join cons in this._unitOfWork.RepositoryAsync<Consortium>().Get() on fv.ConsortiumID equals cons.ConsortiumID
                                into wt1
                                from wt in wt1.DefaultIfEmpty()
                                where (
                                     (fldr.IsPortfolio == true || fldr.IsPortfolio == null)
                                      && fldr.Name != "Master List"
                                      // && fi.FormDesignID == 3
                                      && fvs.FolderVersionStateID == 1
                                      && fi.FormDesignID == FormID
                                      && fi.IsActive.Equals(true)
                                      && fv.EffectiveDate.Year.Equals(year)
                                      )
                                select new PortfolioFoldersDocumentViewModel
                                {
                                    FolderID = fldr.FolderID,
                                    FolderName = fldr.Name,
                                    ConsortiumName = wt.ConsortiumName,
                                    FolderVersionID = fv.FolderVersionID,
                                    FolderVersionStateName = fvs.FolderVersionStateName,
                                    VersionNumber = fv.FolderVersionNumber,
                                    EffectiveDate = fv.EffectiveDate,
                                    FormInstanceID = fi.FormInstanceID,
                                    FormInstanceName = fi.Name,
                                    DocumentId = fi.DocID,
                                    eBsPlanName = accprodmap.ProductType,
                                    eBsPlanNumber = accprodmap.ProductID
                                })
                                .ToList();


                List<PortfolioFoldersDocumentViewModel> documentList1 = documentList
                                                                        .Where(item => PBPDetailsList
                                                                        .Any(item2 => item2.FormInstanceID == item.FormInstanceID))
                                                                        .OrderBy(s => s.FolderName)
                                                                        .ToList();

                documentList = documentList1.ApplySearchCriteria(criteria).ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord)
                                .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count)
                                .ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return new GridPagingResponse<PortfolioFoldersDocumentViewModel>(gridPagingRequest.page, count, gridPagingRequest.rows, documentList);
        }
        /// <summary>
        /// Gets the details of Non-Portfolio Folders list.
        /// </summary>
        /// <param name="tenantID">The tenant identifier.</param>
        /// <returns></returns>
        public GridPagingResponse<DocumentViewModel> GetDocumentsListForMedicalANOCChart(int tenantID, int formDesignId, string planType, GridPagingRequest gridPagingRequest)
        {
            List<DocumentViewModel> documentList = null;
            int count = 0;

            try
            {
                SearchCriteria criteria = new SearchCriteria();
                criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);

                documentList = (from fi in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                                join fv in this._unitOfWork.RepositoryAsync<FolderVersion>().Get() on fi.FolderVersionID equals fv.FolderVersionID
                                join fvs in this._unitOfWork.RepositoryAsync<tmg.equinox.domain.entities.Models.FolderVersionState>().Get() on fv.FolderVersionStateID equals fvs.FolderVersionStateID
                                join fldr in this._unitOfWork.RepositoryAsync<Folder>().Get() on fv.FolderID equals fldr.FolderID
                                join accProdMap in this._unitOfWork.RepositoryAsync<AccountProductMap>().Get() on fi.FormInstanceID equals accProdMap.FormInstanceID
                                where ((fldr.IsPortfolio == true || fldr.IsPortfolio == null)
                                      && fi.IsActive == true
                                      && fi.FormDesignID == formDesignId
                                      && fv.FolderVersionStateID == (int)FolderVersionState.RELEASED)
                                      && accProdMap.ProductType == planType
                                select new DocumentViewModel
                                {
                                    FolderID = fldr.FolderID,
                                    FolderName = fldr.Name,
                                    FolderVersionID = fv.FolderVersionID,
                                    FolderVersionStateName = fvs.FolderVersionStateName,
                                    VersionNumber = fv.FolderVersionNumber,
                                    EffectiveDate = fv.EffectiveDate,
                                    FormInstanceID = fi.FormInstanceID,
                                    FormInstanceName = fi.Name
                                })
                                .ApplySearchCriteria(criteria).ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord)
                                .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count)
                                .ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return new GridPagingResponse<DocumentViewModel>(gridPagingRequest.page, count, gridPagingRequest.rows, documentList);
        }

        public GridPagingResponse<DocumentViewModel> GetDocumentsListForMedicareANOCChart(int tenantID, int formDesignId, string planType, GridPagingRequest gridPagingRequest)
        {
            List<DocumentViewModel> documentList = null;
            int count = 0;

            try
            {
                SearchCriteria criteria = new SearchCriteria();
                criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);

                documentList = (from fi in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                                join fv in this._unitOfWork.RepositoryAsync<FolderVersion>().Get() on fi.FolderVersionID equals fv.FolderVersionID
                                join fvs in this._unitOfWork.RepositoryAsync<tmg.equinox.domain.entities.Models.FolderVersionState>().Get() on fv.FolderVersionStateID equals fvs.FolderVersionStateID
                                join fldr in this._unitOfWork.RepositoryAsync<Folder>().Get() on fv.FolderID equals fldr.FolderID
                                join accProdMap in this._unitOfWork.RepositoryAsync<AccountProductMap>().Get() on fi.FormInstanceID equals accProdMap.FormInstanceID
                                where ((fldr.IsPortfolio == true || fldr.IsPortfolio == null)
                                      && fi.IsActive == true
                                      && fi.FormDesignID == formDesignId
                                      && fv.FolderVersionStateID == (int)FolderVersionState.RELEASED)
                                      && accProdMap.ANOCChartPlanType == planType
                                select new DocumentViewModel
                                {
                                    FolderID = fldr.FolderID,
                                    FolderName = fldr.Name,
                                    FolderVersionID = fv.FolderVersionID,
                                    FolderVersionStateName = fvs.FolderVersionStateName,
                                    VersionNumber = fv.FolderVersionNumber,
                                    EffectiveDate = fv.EffectiveDate,
                                    FormInstanceID = fi.FormInstanceID,
                                    FormInstanceName = fi.Name
                                })
                                .ApplySearchCriteria(criteria).ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord)
                                .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count)
                                .ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return new GridPagingResponse<DocumentViewModel>(gridPagingRequest.page, count, gridPagingRequest.rows, documentList);
        }
        /// <summary>
        /// Gets the account details list.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="isPortfolio">is portfolio.</param>
        /// <returns></returns>
        public GridPagingResponse<ConsumerAccountViewModel> GetAccountDetailsList(int tenantID, bool? isPortfolio, GridPagingRequest gridPagingRequest, string documentName, int RoleID)
        {
            GridPagingResponse<ConsumerAccountViewModel> accountDetailsList = null;
            try
            {
                if (isPortfolio != null && isPortfolio.HasValue && isPortfolio.Value)
                {
                    //accountDetailsList = GetPortfolioDetailsList(1, gridPagingRequest);
                }
                else
                {
                    accountDetailsList = GetNonPortfolioFoldersList(tenantID, gridPagingRequest, documentName, RoleID);
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return accountDetailsList;
        }

        /// <summary>
        /// Gets the folder version details list on the basis of searched documentName.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="folderId">The folder identifier.</param>
        /// <returns></returns>
        public IEnumerable<AccountDetailViewModel> GetFolderVersionDetailsList(int tenantId, int folderId, string accountName, int accountID, string documentName)
        {
            Contract.Requires(tenantId > 0, "Invalid tenantId");
            IList<AccountDetailViewModel> folderVersionList = null;
            try
            {
                bool isPortfolio = this._unitOfWork.RepositoryAsync<Folder>().Query().Filter(a => a.FolderID == folderId).Get().FirstOrDefault().IsPortfolio;

                if (isPortfolio)
                {
                    folderVersionList = (from fldv in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                         join fldr in this._unitOfWork.RepositoryAsync<Folder>().Get() on fldv.FolderID equals fldr.FolderID
                                         join cons in this._unitOfWork.RepositoryAsync<Consortium>().Get() on fldv.ConsortiumID equals cons.ConsortiumID
                                         into wt1
                                         from wt in wt1.DefaultIfEmpty()
                                         where (fldv.FolderID == folderId && fldv.FolderVersionStateID != (int)FolderVersionState.BASELINED)
                                         select new AccountDetailViewModel
                                         {
                                             TenantID = fldr.TenantID,
                                             FolderID = fldr.FolderID,
                                             FolderName = fldr.Name,
                                             FolderVersionID = fldv.FolderVersionID,
                                             VersionNumber = fldv.FolderVersionNumber,
                                             EffectiveDate = fldv.EffectiveDate,
                                             UpdatedBy = fldr.UpdatedBy,
                                             UpdatedDate = fldr.UpdatedDate,
                                             FolderVersionStateID = fldv.FolderVersionStateID
                                         }).OrderByDescending(O => O.FolderVersionID).ToList();
                    folderVersionList = folderVersionList.Skip(1).ToList();
                }
                else
                {
                    folderVersionList = (from fldv in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                         join fldr in this._unitOfWork.RepositoryAsync<Folder>().Get() on fldv.FolderID equals fldr.FolderID
                                         join cons in this._unitOfWork.RepositoryAsync<Consortium>().Get() on fldv.ConsortiumID equals cons.ConsortiumID
                                         into wt1
                                         from wt in wt1.DefaultIfEmpty()
                                         join map in this._unitOfWork.RepositoryAsync<AccountFolderMap>().Get() on fldr.FolderID equals map.FolderID
                                         join accn in this._unitOfWork.RepositoryAsync<Account>().Get() on map.AccountID equals accn.AccountID
                                         where (accn.TenantID == tenantId
                                               && accn.IsActive == true
                                               && fldr.FolderID == folderId
                                               && (fldv.FolderVersionStateID != (int)FolderVersionState.BASELINED)
                                               )
                                         select new AccountDetailViewModel
                                         {
                                             TenantID = accn.TenantID,
                                             AccountID = accn.AccountID,
                                             FolderID = fldr.FolderID,
                                             FolderName = fldr.Name,
                                             ConsortiumName = wt.ConsortiumName,
                                             FolderVersionID = fldv.FolderVersionID,
                                             VersionNumber = fldv.FolderVersionNumber,
                                             EffectiveDate = fldv.EffectiveDate,
                                             UpdatedBy = fldr.UpdatedBy,
                                             UpdatedDate = fldr.UpdatedDate,
                                             FolderVersionStateID = fldv.FolderVersionStateID
                                         }).OrderByDescending(O => O.FolderVersionID).ToList();
                    folderVersionList = folderVersionList.Skip(1).ToList();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return folderVersionList;

        }

        //TO DO:Method can be transferred to User service
        /// <summary>
        /// This method returns a collection of Data from 'User' table.
        /// The collection is filtered using 'tenantId' which is passing as parameter.
        /// </summary>
        ///<param name="tenantId"></param>
        /// <returns></returns>
        public IEnumerable<UserViewModel> GetPrimaryContactsList(int tenantId)
        {
            Contract.Requires(tenantId > 0, "Invalid tenantId");
            IEnumerable<UserViewModel> primaryContactsList = null;
            try
            {
                primaryContactsList = (from ms in this._unitOfWork.RepositoryAsync<User>().Query().Filter(ms => ms.TenantID == tenantId).Get()
                                       select new UserViewModel
                                       {
                                           UserName = ms.UserName,
                                           UserId = ms.UserID
                                       }).OrderBy(c => c.UserName).ToList();

                if (primaryContactsList.Count() == 0)
                    primaryContactsList = null;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return primaryContactsList;
        }

        //TO DO:Method can be transferred to MarketSegment service
        /// <summary>
        /// This method returns a collection of Data from 'MarketSegment' table
        /// The collection is filtered using 'tenantId' which is passing as parameter.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public IEnumerable<MarketSegmentViewModel> GetMarketSegmentsList(int tenantId)
        {
            Contract.Requires(tenantId > 0, "Invalid tenantId");
            IList<MarketSegmentViewModel> marketSegmentsList = null;
            try
            {
                marketSegmentsList = (from ms in this._unitOfWork.RepositoryAsync<MarketSegment>().Query().Filter(ms => ms.TenantID == tenantId).Get()
                                      select new MarketSegmentViewModel
                                      {
                                          MarketSegmentId = ms.MarketSegmentID,
                                          MarketSegment = ms.MarketSegmentName
                                      }).ToList();
                if (marketSegmentsList.Count() == 0)
                    marketSegmentsList = null;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }

            return marketSegmentsList;
        }

        /// <summary>
        /// This method adds Data in 'Folder','FolderVersion','FolderVersionWorkFlowState' table.
        /// </summary>
        ///<param name="tenantId"></param>
        /// <returns></returns>
        public ServiceResult AddFolder(int tenantId, int? accountId, string folderName, DateTime folderEffectiveDate, bool isPortfolio, int? userId, string primaryContact, int marketSegmentId, Nullable<int> consortiumID, Nullable<int> categoryID, string catID, string addedBy, bool isFoundation = false)
        {
            Contract.Requires(!string.IsNullOrEmpty(folderName), "Folder Name cannot be empty");
            Contract.Requires((folderEffectiveDate == null), "Folder effective date required");
            Contract.Requires(tenantId > 0, "Invalid tenantId");
            Contract.Requires(marketSegmentId > 0, "Invalid marketSegmentId");
            ServiceResult result = null;
            FolderVersionViewModel addedFolder = null;
            try
            {
                result = new ServiceResult();
                //using (var scope = new TransactionScope())
                {
                    if (accountId == null)
                    {
                        //for Portfolio Based Account
                        if (!this._unitOfWork.RepositoryAsync<Folder>().IsFolderNameExists(tenantId, 0, folderName))
                        {
                            addedFolder = CreateFolderWithVersion(tenantId, accountId, folderName, folderEffectiveDate, isPortfolio, userId, primaryContact, marketSegmentId, 0, categoryID, catID, addedBy, null, isFoundation);
                            //Return success result
                            result.Result = ServiceResultStatus.Success;
                        }
                        else
                        {
                            result.Result = ServiceResultStatus.Failure;
                            ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem() { Messages = new string[] { "This Folder Name already exists for this Account. Please enter a different Folder Name." } });

                        }
                    }
                    else
                    {
                        //for Non Portfolio Based Account
                        //Perform this only when Folder name is new.
                        if (!this._unitOfWork.RepositoryAsync<AccountFolderMap>().IsFolderNameExistsInAccount(tenantId, accountId, 0, folderName))
                        {
                            addedFolder = CreateFolderWithVersion(tenantId, accountId, folderName, folderEffectiveDate, isPortfolio, userId, primaryContact, marketSegmentId, consortiumID, categoryID, catID, addedBy, null, isFoundation);
                            //Return success result
                            result.Result = ServiceResultStatus.Success;
                        }
                        else
                        {
                            result.Result = ServiceResultStatus.Failure;
                            ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem() { Messages = new string[] { "This Folder Name already exists for this Account. Please enter a different Folder Name." } });

                        }
                    }

                    this._unitOfWork.Save();
                    if (addedFolder != null)
                        ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem() { Messages = new string[] { addedFolder.TenantID.ToString(), addedFolder.FolderVersionId.ToString(), addedFolder.FolderId.ToString() } });
                    //scope.Complete();
                }
            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;

            }
            return result;

        }

        /// <summary>
        /// Copies the folder.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="accountId">The account identifier.</param>
        /// <param name="originalFolderID">The original folder identifier.</param>
        /// <param name="originalFolderVersionId">The original folder version identifier.</param>
        /// <param name="folderName">Name of the folder.</param>
        /// <param name="folderEffectiveDate">The folder effective date.</param>
        /// <param name="isPortfolio">if set to <c>true</c> [is portfolio].</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="primaryContact">The primary contact.</param>
        /// <param name="marketSegmentId">The market segment identifier.</param>
        /// <param name="addedBy">The added by.</param>
        /// <returns></returns>
        public ServiceResult CopyFolder(int tenantId, int? accountId, int originalFolderID, int originalFolderVersionId,
                                 string folderName, DateTime folderEffectiveDate, bool isPortfolio, int userId, string primaryContact,
                                 int marketSegmentId, Nullable<int> consortiumID, Nullable<int> categoryID, string catID, string addedBy, bool isFoundation = false)
        {
            Contract.Requires(!string.IsNullOrEmpty(folderName), "Folder Name cannot be empty");
            Contract.Requires((folderEffectiveDate == null), "Folder effective date required");
            Contract.Requires(tenantId > 0, "Invalid tenantId");
            Contract.Requires(marketSegmentId > 0, "Invalid marketSegmentId");
            ServiceResult result = new ServiceResult();
            result.Result = ServiceResultStatus.Success;
            try
            {
                if (accountId == null)
                {
                    consortiumID = 0;
                    //for Portfolio Based Account
                    if (this._unitOfWork.RepositoryAsync<Folder>().IsFolderNameExists(tenantId, 0, folderName))
                    {
                        result.Result = ServiceResultStatus.Failure;
                        ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem() { Messages = new string[] { "Folder Name Already Exist." } });
                    }
                }
                else
                {
                    //For Non Portfolio Based Account
                    if (this._unitOfWork.RepositoryAsync<AccountFolderMap>().IsFolderNameExistsInAccount(tenantId, accountId, 0, folderName))
                    {
                        result.Result = ServiceResultStatus.Failure;
                        ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem() { Messages = new string[] { "Folder Name Already Exist in Account." } });
                    }
                }
                if (result.Result != ServiceResultStatus.Failure)
                {
                    var builder = new VersionNumberBuilder();
                    string newVersionNumber = builder.GetNextMinorVersionNumber(null, folderEffectiveDate);
                    var folderVersion = CopyFolder(tenantId, accountId, originalFolderID, originalFolderVersionId, folderName, folderEffectiveDate, isPortfolio, userId, primaryContact, marketSegmentId, consortiumID, categoryID, catID, addedBy, newVersionNumber, isFoundation);
                    if (null == folderVersion)
                    {
                        result.Result = ServiceResultStatus.Failure;
                        ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem() { Messages = new string[] { "Folder cannot be created" } });
                        return result;
                    }
                    else
                    {
                        List<FormInstanceViewModel> formList = _folderVersionService.GetFormInstanceList(tenantId, folderVersion.FolderVersionID, folderVersion.FolderID);

                        List<CopyFromAuditTrail> copyFromAuditTrails = _unitOfWork.RepositoryAsync<CopyFromAuditTrail>().Get().Where(x => x.FolderID == originalFolderID && x.SourceFolderVersionID == originalFolderVersionId && x.DestinationFolderVersionID == folderVersion.FolderVersionID).ToList();
                        //foreach (CopyFromAuditTrail copyFromAuditTrail in copyFromAuditTrails)
                        //    _folderVersionService.UpdateReportingCenterDatabase(copyFromAuditTrail.DestinationDocumentID, copyFromAuditTrail.SourceDocumentID);

                        _planTaskUserMappingService.SavetaskPlanNewFolderVersion(folderVersion.FolderVersionID, addedBy);

                        result.Result = ServiceResultStatus.Success;
                        ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem() { Messages = new string[] { folderVersion.TenantID.ToString(), folderVersion.FolderVersionID.ToString(), folderVersion.FolderID.ToString() } });
                    }
                }
            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }

            return result;
        } // CopyFolder

        /// <summary>
        /// </summary>
        /// <param name="tenantID">tenantID</param>
        /// <param name="accountID">accountID</param>
        /// <param name="originalFolderID">originalFolderID</param>
        /// <param name="originalFolderVersionID">originalFolderVersionID</param>
        /// <param name="folderName">folderName</param>
        /// <param name="folderEffectiveDate">folderEffectiveDate</param>
        /// <param name="isPortfolio">isPortfolio</param>
        /// <param name="userID">userID</param>
        /// <param name="userName">userName</param>
        /// <param name="marketSegmentId">marketSegmentId</param>
        /// <param name="consortiumID">consortiumID</param>
        /// <param name="currentUserName">currentUserName</param>
        /// <param name="newFolderVersionNumber">newFolderVersionNumber</param>
        /// <returns>List of Facet Mappings based on FormDesignID and FormDesignVersionID</returns>
        private FolderVersion CopyFolder(int tenantID, int? accountID, int originalFolderID, int originalFolderVersionID, string folderName, DateTime folderEffectiveDate,
            bool isPortfolio, int userID, string userName, int marketSegmentId, Nullable<int> consortiumID, Nullable<int> categoryID, string catID, string currentUserName, string newFolderVersionNumber, bool isFoundation = false)
        {
            FolderVersion folderData = null;
            SqlParameter paramConsortiumID = new SqlParameter();
            SqlParameter paramCategoryID = new SqlParameter();
            try
            {
                object accountId;
                if (accountID == null)
                    accountId = DBNull.Value;
                else
                    accountId = accountID;

                SqlParameter paramTenantID = new SqlParameter("@TenantID", tenantID);
                SqlParameter paramAccountID = new SqlParameter("@AccountID", accountId);
                SqlParameter paramOriginalFolderID = new SqlParameter("@OriginalFolderID", originalFolderID);
                SqlParameter paramOriginalFolderVersionID = new SqlParameter("@OriginalFolderVersionID", originalFolderVersionID);
                SqlParameter paramFolderName = new SqlParameter("@FolderName", folderName);
                SqlParameter paramFolderEffectiveDate = new SqlParameter("@FolderEffectiveDate", folderEffectiveDate);
                SqlParameter paramIsPortfolio = new SqlParameter("@IsPortfolio", isPortfolio);
                SqlParameter paramUserID = new SqlParameter("@UserID", userID);
                SqlParameter paramUserName = new SqlParameter("@UserName", userName);
                SqlParameter paramMarketSegmentId = new SqlParameter("@MarketSegmentId", marketSegmentId);
                if (null == consortiumID)
                    paramConsortiumID = new SqlParameter("@ConsortiumID", DBNull.Value);
                else
                    paramConsortiumID = new SqlParameter("@ConsortiumID", consortiumID);
                if (null == categoryID)
                    paramCategoryID = new SqlParameter("@CategoryID", DBNull.Value);
                else
                    paramCategoryID = new SqlParameter("@CategoryID", categoryID);

                SqlParameter paramCatID = new SqlParameter("@CatID", catID);
                SqlParameter paramCurrentUserName = new SqlParameter("@CurrentUserName", currentUserName);
                SqlParameter paramNewFolderVersionNumber = new SqlParameter("@NewFolderVersionNumber", newFolderVersionNumber);

                folderData = this._unitOfWork.Repository<FolderVersion>().ExecuteSql("exec [dbo].[USP_CopyFolder] @TenantID,@AccountID,@OriginalFolderID,@OriginalFolderVersionID ,@FolderName ,@FolderEffectiveDate ,@IsPortfolio ,@UserID ,@UserName ,@MarketSegmentId ,@ConsortiumID, @CategoryID, @CatID, @CurrentUserName ,@NewFolderVersionNumber ",
                    paramTenantID, paramAccountID, paramOriginalFolderID, paramOriginalFolderVersionID, paramFolderName, paramFolderEffectiveDate, paramIsPortfolio, paramUserID, paramUserName, paramMarketSegmentId, paramConsortiumID, paramCategoryID, paramCatID, paramCurrentUserName, paramNewFolderVersionNumber).ToList().FirstOrDefault();

                if (isFoundation)
                    UpdateFoundationFlag(folderData, isFoundation);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(new Exception("GetFolderDetail - " + ex.Message), ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return folderData;
        } // CopyFolder

        private void UpdateFoundationFlag(FolderVersion folderData, bool isFoundation = false)
        {
            Folder folder = this._unitOfWork.RepositoryAsync<Folder>().Get().Where(x => x.FolderID == folderData.FolderID).SingleOrDefault();
            if(folder != null)
            {
                folder.IsFoundation = isFoundation;
                this._unitOfWork.RepositoryAsync<Folder>().Update(folder);
                this._unitOfWork.Save();
            }
        }

        /// <summary>
        /// check if the user has permission to create the account
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool GetUserAccountCreationPermission(int? userId)
        {
            if (userId == null) throw new ArgumentNullException();
            bool isUserPermittedToCreateAccount = false;
            try
            {
                int roleId = (from userMap in this._unitOfWork.RepositoryAsync<User>().Get()
                              join userRoleMap in this._unitOfWork.RepositoryAsync<UserRoleAssoc>().Get()
                              on userMap.UserID equals userRoleMap.UserId
                              where userMap.UserID == userId
                              select new List<int>
                              {
                                  userRoleMap.RoleId
                              }).FirstOrDefault().First<int>();
                AccountFolderCreationPermission accountFolderCreationPermission = this._unitOfWork.RepositoryAsync<AccountFolderCreationPermission>()
                    .Query()
                    .Filter(row => row.UserRoleID == roleId && row.HasAccountCreationPermission == true).Get().FirstOrDefault();
                if (accountFolderCreationPermission != null)
                {
                    isUserPermittedToCreateAccount = true;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return isUserPermittedToCreateAccount;
        }


        public GridPagingResponse<PortfolioViewModel> GetPortfolioFolderListForPBPImport(int tenantId, GridPagingRequest gridPagingRequest, string documentName)
        {
            //Contract.Requires(tenantId > 0, "Invalid tenantId");
            int count = 0;
            List<PortfolioViewModel> portfolioDetailsList = new List<PortfolioViewModel>();
            SearchCriteria criteria = new SearchCriteria();
            criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);
            var detailsWithCommaSeparatedList = new List<PortfolioViewModel>();
            try
            {
                var currentFolders = from s in "st"
                                     select new
                                     {
                                         FolderID = 0,
                                         FolderVersionID = 0,
                                         VersionCount = 0
                                     };

                List<AccountProductMap> ProductList = this._unitOfWork.RepositoryAsync<AccountProductMap>().Get().ToList();


                if (!String.IsNullOrEmpty(documentName))
                {
                    currentFolders = from s in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                     join f in this._unitOfWork.RepositoryAsync<FormInstance>().Get().Where(f => f.Name.Contains(documentName) && f.IsActive == true)
                                     on s.FolderVersionID equals f.FolderVersionID
                                     join fd in this._unitOfWork.RepositoryAsync<FormDesign>().Get().Where(fd => fd.IsMasterList != true && fd.DocumentDesignTypeID == 1)
                                     on f.FormDesignID equals fd.FormID
                                     join fl in this._unitOfWork.RepositoryAsync<Folder>().Get().Where(fld => fld.IsPortfolio == true)
                                     on s.FolderID equals fl.FolderID
                                     group s by s.FolderID into g
                                     select new { FolderID = g.Key, FolderVersionID = g.Max(s => s.FolderVersionID), VersionCount = g.Distinct().Count() };
                }
                else
                {
                    currentFolders = from s in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                     join fl in this._unitOfWork.RepositoryAsync<Folder>().Get().Where(fld => fld.IsPortfolio == true)
                                     on s.FolderID equals fl.FolderID
                                     group s by s.FolderID into g
                                     select new { FolderID = g.Key, FolderVersionID = g.Max(s => s.FolderVersionID), VersionCount = g.Where(x => x.FolderVersionStateID == (int)FolderVersionState.INPROGRESS).Distinct().Count() };
                }
                portfolioDetailsList = (from fldv in this._unitOfWork.RepositoryAsync<FolderVersion>().Get()
                                        join c in currentFolders on new { fldv.FolderID, fldv.FolderVersionID } equals new { c.FolderID, c.FolderVersionID }
                                        join fol in this._unitOfWork.RepositoryAsync<Folder>().Query().Get()
                                        on fldv.FolderID equals fol.FolderID
                                            into obj
                                        from p in obj.DefaultIfEmpty()
                                        join cat in this._unitOfWork.RepositoryAsync<FolderVersionCategory>().Get()
                                            on fldv.CategoryID equals cat.FolderVersionCategoryID
                                        where (p.TenantID == tenantId && p.IsPortfolio == true)
                                        select new PortfolioViewModel
                                        {
                                            FolderName = p.Name,
                                            FolderID = p.FolderID,
                                            MarketSegmentID = p.MarketSegmentID,
                                            PrimaryContactID = p.PrimaryContentID,
                                            FolderVersionID = fldv.FolderVersionID,
                                            VersionNumber = fldv.FolderVersionNumber,
                                            EffectiveDate = fldv.EffectiveDate,
                                            Status = fldv.WorkFlowVersionState.WorkFlowState.WFStateName,
                                            ApprovalStatusID = fldv.FolderVersionWorkFlowStates.Where(s => s.FolderVersionID == fldv.FolderVersionID && s.IsActive == true && s.WFStateID == fldv.WFStateID).Select(x => x.ApprovalStatusID).FirstOrDefault(),
                                            FolderVersionCount = p.FolderVersions.Where(row => row.FolderVersionStateID != (int)FolderVersionState.BASELINED).Count(),
                                            TenantID = tenantId,
                                            CategoryID = cat.FolderVersionCategoryID,
                                            CategoryName = cat.FolderVersionCategoryName,
                                            UpdatedBy = fldv.UpdatedBy,
                                            UpdatedDate = fldv.UpdatedDate
                                        }).ToList()
                                                                .ApplySearchCriteria(criteria).ApplyOrderBy(gridPagingRequest.sidx, gridPagingRequest.sord)
                                                                .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count);

                // code for showing pipe seprated hybrid workflow state 
                foreach (var model in portfolioDetailsList)
                {
                    model.Status = null;
                    IList<KeyValue> workFlowList = null;
                    if (model.FolderVersionID != null && model.TenantID != 0)
                    {
                        workFlowList = this.workflowStateService.GetCurrentWorkFlowState(model.TenantID, (int)model.FolderVersionID).ToList();
                    }
                    if (workFlowList != null && workFlowList.Count > 0)
                    {
                        if (workFlowList.Count() > 0)
                        {
                            foreach (var nm in workFlowList)
                            {
                                if (model.Status == null)
                                {
                                    model.Status = nm.Value;
                                }
                                else
                                {
                                    model.Status = model.Status + '|' + nm.Value;
                                }
                            }
                        }
                    }
                    model.ProductList = GetProductList(model.FolderID, ProductList);
                }


            }

            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }

            return new GridPagingResponse<PortfolioViewModel>(gridPagingRequest.page, count, gridPagingRequest.rows, portfolioDetailsList);
        }

        #endregion Account Search

        public List<AccountViewModel> GetAccountList(int skip, int pageSize, ref int count)
        {
            List<AccountViewModel> model = null;
            try
            {
                count = this._unitOfWork.RepositoryAsync<Account>().Get().Count();

                model = (from entry in this._unitOfWork.RepositoryAsync<Account>().Get()
                         orderby entry.AccountID
                         select new AccountViewModel
                         {
                             AccountID = entry.AccountID,
                             AccountName = entry.AccountName,
                             IsActive = entry.IsActive
                         }).Skip(skip).Take(pageSize).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }

            return model;
        }
        public AccountViewModel GetAccount(int accountId)
        {
            AccountViewModel model = null;
            try
            {
                model = (from entry in this._unitOfWork.RepositoryAsync<Account>().Get()
                         where entry.AccountID == accountId
                         select new AccountViewModel
                         {
                             AccountID = entry.AccountID,
                             AccountName = entry.AccountName,
                             IsActive = entry.IsActive
                         }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }

            return model;
        }
        /// <summary>
        /// Gets the account Name. added on 8/18
        /// </summary>
        /// <param name="accountName"></param>
        /// <returns></returns>
        public AccountViewModel GetAccountByName(string accountName)
        {
            AccountViewModel model = null;
            try
            {
                model = (from entry in this._unitOfWork.RepositoryAsync<Account>().Get()
                         where entry.AccountName == accountName
                         select new AccountViewModel
                         {
                             AccountID = entry.AccountID,
                             AccountName = entry.AccountName,
                             IsActive = entry.IsActive
                         }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }

            return model;
        }

        public int GetFolderCategoryId(string category)
        {
            int categoryID = 0;
            categoryID = this._unitOfWork.RepositoryAsync<FolderVersionCategory>().Get().Where(s => s.FolderVersionCategoryName == category).Select(s => s.FolderVersionCategoryID).FirstOrDefault();
            return categoryID;
        }

        #endregion

        #region Private Methods

        #region Account Search

        /// <summary>
        /// Creates the folder with version.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="accountId">The account identifier.</param>
        /// <param name="folderName">Name of the folder.</param>
        /// <param name="folderEffectiveDate">The folder effective date.</param>
        /// <param name="isPortfolio">if set to <c>true</c> [is portfolio].</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="primaryContact">The primary contact.</param>
        /// <param name="marketSegmentId">The market segment identifier.</param>
        /// <param name="addedBy">The added by.</param>
        /// <param name="originalFolderID">The original folder identifier.</param>
        /// <returns></returns>
        private FolderVersionViewModel CreateFolderWithVersion(int tenantId, int? accountId, string folderName,
            DateTime folderEffectiveDate, bool isPortfolio, int? userId, string primaryContact,
            int marketSegmentId, Nullable<int> consortiumID, Nullable<int> categoryID, string catID, string addedBy, int? originalFolderID, bool isFoundation)
        {
            int? formDesignGroupID = null;

            if (!isPortfolio)
            {
                formDesignGroupID = this._unitOfWork.RepositoryAsync<FormDesignGroup>()
                                            .GetFormDeisgnGroupID(tenantId, folderType: "Account");
            }
            if (accountId == null)
            {
                formDesignGroupID = this._unitOfWork.RepositoryAsync<FormDesignGroup>()
                                            .GetFormDeisgnGroupID(tenantId, folderType: folderName);
                consortiumID = null;
                formDesignGroupID = null;
            }


            Folder folderToAdd = new Folder();
            folderToAdd.TenantID = tenantId;
            folderToAdd.Name = folderName;
            folderToAdd.IsPortfolio = isPortfolio;
            folderToAdd.MarketSegmentID = marketSegmentId;
            folderToAdd.PrimaryContent = primaryContact;
            folderToAdd.PrimaryContentID = userId;
            folderToAdd.AddedBy = addedBy;
            folderToAdd.AddedDate = DateTime.Now;
            folderToAdd.ParentFolderId = originalFolderID;
            folderToAdd.FormDesignGroupId = formDesignGroupID;
            folderToAdd.IsFoundation= isFoundation;
            //folderToAdd.MasterListFormDesignID = masterListFormDesignID;
            //Call to repository method to insert record.
            this._unitOfWork.RepositoryAsync<Folder>().Insert(folderToAdd);
            this._unitOfWork.Save();

            //Add entry to AccountFolderMap table
            if (accountId.HasValue)
            {
                AccountFolderMap accountFolderMap = new AccountFolderMap();
                accountFolderMap.AccountID = accountId.Value;
                accountFolderMap.FolderID = folderToAdd.FolderID;

                this._unitOfWork.RepositoryAsync<AccountFolderMap>().Insert(accountFolderMap);
            }
            WorkFlowVersionState workflowState = null;
            //Retrieve sequence of Setup State   
            if (categoryID != null)
            {
                workflowState = this._unitOfWork.RepositoryAsync<WorkFlowVersionState>().GetFirstWorkflowState(this._unitOfWork, (int)categoryID, isPortfolio);

            }
            //Adding Initial Minor Version and FolderVersionWorkFlowState for respective Folder   
            FolderVersionViewModel addVersion = new FolderVersionViewModel();
            if (workflowState != null)
            {
                addVersion = _folderVersionService.AddFolderVersion(tenantId, folderEffectiveDate, addedBy,
                                          folderToAdd.FolderID, workflowState.WorkFlowVersionStateID, consortiumID, categoryID, catID, userId); //Modification No.2
            }
            else
            {
                addVersion = _folderVersionService.AddFolderVersion(tenantId, folderEffectiveDate, addedBy,
                                              folderToAdd.FolderID, null, consortiumID, categoryID, catID, userId); //Modification No.2
            }


            return addVersion;
        }


        private string GetProductList(int? folderid, List<AccountProductMap> ProductList)
        {
            string Products = string.Empty;
            foreach (var product in ProductList.Where(s => s.FolderID.Equals(folderid)))
            {
                Products += product.ProductID + ",";
            }
            if (!string.IsNullOrEmpty(Products))
            {
                Products = Products.Remove(Products.Length - 1);
            }
            return Products;
        }

        #endregion

        #region Manage Account

        //Add Data To AccountProductMap Table
        private ServiceResult AddAccountProductMap(int tenantId, int formInstanceId, int folderId, int folderversionId, string propertyType, string propertyId, string planCode, string userName, string serviceGroup, string anocChartPlanType, string productName)
        {
            ServiceResult result = null;
            try
            {
                result = new ServiceResult();

                AccountProductMap accountProductMap = new AccountProductMap();
                accountProductMap.TenantID = tenantId;
                accountProductMap.FormInstanceID = formInstanceId;
                accountProductMap.FolderID = folderId;
                accountProductMap.FolderVersionID = folderversionId;
                accountProductMap.ProductType = propertyType;
                accountProductMap.ProductID = propertyId;
                accountProductMap.PlanCode = planCode;
                accountProductMap.ANOCChartPlanType = anocChartPlanType;
                accountProductMap.AddedDate = DateTime.Now;
                accountProductMap.AddedBy = userName;
                accountProductMap.IsActive = true;
                accountProductMap.ServiceGroup = serviceGroup;
                accountProductMap.ProductName = productName;
                this._unitOfWork.RepositoryAsync<AccountProductMap>().Insert(accountProductMap);
                this._unitOfWork.Save();

                result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;

            }
            return result;
        }

        #endregion

        #endregion
    }
}