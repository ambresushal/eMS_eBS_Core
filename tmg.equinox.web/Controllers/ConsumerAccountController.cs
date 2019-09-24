using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web.Mvc;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.ConsumerAccount;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.applicationservices.viewmodels.Portfolio;
using tmg.equinox.web.Framework;
using System.Linq;
using tmg.equinox.identitymanagement;
using tmg.equinox.applicationservices.viewmodels;

namespace tmg.equinox.web.Controllers
{
    public partial class ConsumerAccountController : AuthenticatedController
    {
        #region Private Members
        private IConsumerAccountService _accountService { get; set; }
        private IPortfolioService _portfolioService { get; set; }
        #endregion Private Members

        #region Constructor

        public ConsumerAccountController(IConsumerAccountService accountService, IPortfolioService portfolioService, IFolderVersionServices folderVersionService)
        {
            this._portfolioService = portfolioService;
            this._accountService = accountService;
        }
        #endregion

        #region Public Methods

        #region Manage Account

        /// <summary>
        ///This shows the Manage Account screen.
        /// </summary>
        /// <returns></returns>

        public ActionResult ManageAccount()
        {
            ViewBag.Claims = ControllerHelper.SetClaimsInView(Claims);
            ViewBag.RoleId = RoleID;
            return View();
        }

        /// <summary>
        /// Gets the account list.
        /// Handled Null (formDesignMappedList) object
        /// by Snehal K on 20140901
        /// </summary>
        /// <param name="tenantId"> tenant identifier.</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetAccountList(int tenantId)
        {
            IEnumerable<ConsumerAccountViewModel> accountList = this._accountService.GetAccountList(tenantId);
            if (accountList == null)
            {
                accountList = new List<ConsumerAccountViewModel>();
            }
            ViewBag.Claims = ControllerHelper.FilterClaimsByMethod(Claims, GetActionName);
            return Json(accountList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Adds the specified Account.
        /// </summary>
        /// <param name="tenantId"> tenant identifier.</param>
        /// <param name="accountId"> account identifier.</param>
        /// <param name="accountName">Name of the account.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Add(int tenantId, int accountId, string accountName)
        {
            ServiceResult result = this._accountService.AddAccount(tenantId, accountName, CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Updates the specified Account.
        /// </summary>
        /// <param name="tenantId"> tenant identifier.</param>
        /// <param name="accountId"> account identifier.</param>
        /// <param name="accountName">Name of the account.</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Update(int tenantId, int accountId, string accountName)
        {
            ServiceResult result = this._accountService.UpdateAccount(tenantId, accountId, accountName, CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Deletes the specified Account.
        /// </summary>
        /// <param name="tenantId"> tenant identifier.</param>
        /// <param name="accountId"> account identifier.</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Delete(int tenantId, int accountId)
        {
            ServiceResult result = this._accountService.DeleteAccount(tenantId, accountId, CurrentUserName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Account Search

        /// <summary>
        ///This shows the Account search screen.
        /// </summary>
        /// <returns></returns>
        public ActionResult AccountSearch()
        {
            ViewBag.IsFolderLockEnable = IdentityManager.IsFolderLockEnable.ToString().ToLower();
            ViewBag.Claims = ControllerHelper.SetClaimsInView(Claims);
            ViewBag.RoleId = RoleID;
            return View();
        }

        /// <summary>
        /// Gets the portfolio based account list.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns></returns>
        public JsonResult GetPortfolioBasedAccountList(int tenantId, GridPagingRequest gridPagingRequest, string documentName)
        {
            GridPagingResponse<PortfolioViewModel> accountList = this._accountService.GetPortfolioDetailsList(1, gridPagingRequest, documentName, 0);

            ViewBag.Claims = ControllerHelper.FilterClaimsByMethod(Claims, GetActionName);
            return Json(accountList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the non portfolio based accountist.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns></returns>
        public JsonResult GetNonPortfolioBasedAccountist(int tenantId, GridPagingRequest gridPagingRequest, string documentName)
        {
            GridPagingResponse<ConsumerAccountViewModel> accountList = this._accountService.GetAccountDetailsList(tenantId, false, gridPagingRequest, documentName, RoleID);
            ViewBag.Claims = ControllerHelper.FilterClaimsByMethod(Claims, GetActionName);
            return Json(accountList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the non portfolio based accountist.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns></returns>
        public JsonResult GetDocumentsList(int tenantId, int formDesignID, int formInstanceId, GridPagingRequest gridPagingRequest)
        {
            GridPagingResponse<DocumentViewModel> documentsList = null;
            if (formDesignID > 0)
            {
                if (tmg.equinox.config.Config.GetApplicationName().ToLower() == "ebenefitsync")
                {
                    documentsList = this._accountService.GetDocumentsList(tenantId, formDesignID, formInstanceId, gridPagingRequest, "eBS");
                }
                else
                {

                    documentsList = this._accountService.GetDocumentsList(tenantId, formDesignID, gridPagingRequest);
                }
            }
            else
            {
                if (tmg.equinox.config.Config.GetApplicationName().ToLower() == "ebenefitsync")
                {
                    documentsList = this._accountService.GetDocumentsList(tenantId, formDesignID, formInstanceId, gridPagingRequest, "eBS");
                }
                else
                {
                    documentsList = this._accountService.GetDocumentsList(tenantId, gridPagingRequest);
                }

            }
            return Json(documentsList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDocumentsListNew(int tenantId, int formDesignID, GridPagingRequest gridPagingRequest)
        {
            GridPagingResponse<DocumentViewModel> documentsList = null;
            if (formDesignID > 0)
            {
                if (tmg.equinox.config.Config.GetApplicationName().ToLower() == "ebenefitsync")
                {
                    documentsList = this._accountService.GetDocumentsList(tenantId, formDesignID, gridPagingRequest, "eBS");
                }
                else
                {

                    documentsList = this._accountService.GetDocumentsList(tenantId, formDesignID, gridPagingRequest);
                }
            }
            else
            {
                if (tmg.equinox.config.Config.GetApplicationName().ToLower() == "ebenefitsync")
                {
                    documentsList = this._accountService.GetDocumentsList(tenantId, formDesignID, gridPagingRequest, "eBS");
                }
                else
                {
                    documentsList = this._accountService.GetDocumentsList(tenantId, gridPagingRequest);
                }

            }
            return Json(documentsList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPortfolioFoldersDocumentsList(int tenantId, GridPagingRequest gridPagingRequest, int year)
        {
            GridPagingResponse<PortfolioFoldersDocumentViewModel> documentsList = this._accountService.GetPortfolioFoldersDocumentsList(tenantId, gridPagingRequest, year);
            return Json(documentsList, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetDocumentsListForMedicalANOCChart(int tenantId, int formDesignId, string planType, GridPagingRequest gridPagingRequest)
        {
            GridPagingResponse<DocumentViewModel> documentsList = this._accountService.GetDocumentsListForMedicalANOCChart(tenantId, formDesignId, planType, gridPagingRequest);
            return Json(documentsList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDocumentsListForMedicareANOCChart(int tenantId, int formDesignId, string planType, GridPagingRequest gridPagingRequest)
        {
            GridPagingResponse<DocumentViewModel> documentsList = this._accountService.GetDocumentsListForMedicareANOCChart(tenantId, formDesignId, planType, gridPagingRequest);
            return Json(documentsList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFolderVersionList(int tenantId, int folderId, string accountName, int accountID, string documentName)
        {
            IEnumerable<AccountDetailViewModel> foldersList = this._accountService.GetFolderVersionDetailsList(tenantId, folderId, accountName, accountID, documentName);
            return Json(foldersList, JsonRequestBehavior.AllowGet);
        }


        //TO DO:Method can be transferred to MarketSegment service
        /// <summary>
        /// This method returns a collection of Data from 'MarketSegment' table
        /// The collection is filtered using 'tenantId' which is passing as parameter.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetMarketSegmentsList(int tenantId)
        {
            IEnumerable<MarketSegmentViewModel> marketSegmentList = this._accountService.GetMarketSegmentsList(tenantId);
            return Json(marketSegmentList, JsonRequestBehavior.AllowGet);
        }

        //TO DO:Method can be transferred to User service
        /// <summary>
        /// This method returns a collection of Data from 'User' table.
        /// The collection is filtered using 'tenantId' which is passing as parameter.
        /// </summary>
        ///<param name="tenantId"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetPrimaryContactsList(int tenantId)
        {
            IEnumerable<UserViewModel> primaryContactList = this._accountService.GetPrimaryContactsList(tenantId);
            return Json(primaryContactList, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// This method adds Data in 'Folder','FolderVersion','FolderVersionWorkFlowState' table.
        /// </summary>
        ///<param name="tenantId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddFolder(int tenantId, int? accountId, string folderName, DateTime folderEffectiveDate, bool isPortfolio, int userId, string userName, int marketSegmentId, Nullable<int> consortiumID, Nullable<int> categoryID, string catID,bool isFoundation)
        {
            ServiceResult result = this._accountService.AddFolder(tenantId, accountId, folderName, folderEffectiveDate, isPortfolio, CurrentUserId, CurrentUserName, marketSegmentId, consortiumID, categoryID, catID, CurrentUserName, isFoundation);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CopyFolder(int tenantId, int? accountID, int originalFolderID, int originalFolderVersionID, string folderName, DateTime folderEffectiveDate, bool isPortfolio, int userId, string userName, int marketSegmentId, Nullable<int> consortiumID, Nullable<int> categoryID, string catID, bool isFoundation)
        {
            ServiceResult result = this._accountService.CopyFolder(tenantId, accountID, originalFolderID, originalFolderVersionID, folderName, folderEffectiveDate, isPortfolio, (int)CurrentUserId, userName, marketSegmentId, consortiumID, categoryID, catID, CurrentUserName, isFoundation);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetUserAccountCreationPermission()
        {
            bool IsuserAbletoCreateAccount = this._accountService.GetUserAccountCreationPermission(base.CurrentUserId);
            return Json(IsuserAbletoCreateAccount, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPortfolioFolderListForPBPImport(int tenantId, GridPagingRequest gridPagingRequest, string documentName, int year)
        {
            GridPagingResponse<PortfolioViewModel> accountList = this._accountService.GetPortfolioDetailsList(1, gridPagingRequest, documentName, year);

            ViewBag.Claims = ControllerHelper.FilterClaimsByMethod(Claims, GetActionName);
            return Json(accountList, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #endregion

        #region Private Methods

        #endregion

    }


}