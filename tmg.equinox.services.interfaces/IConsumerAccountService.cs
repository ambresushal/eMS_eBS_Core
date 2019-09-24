using System;
using System.Collections.Generic;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.ConsumerAccount;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.applicationservices.viewmodels.Portfolio;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IConsumerAccountService
    {
        #region Manage  Account

        /// <summary>
        /// Gets the account list.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns></returns>
        IEnumerable<ConsumerAccountViewModel> GetAccountList(int tenantId);

        List<AccountViewModel> GetAccountList(int skip, int pageSize, ref int count);
        AccountViewModel GetAccount(int accountId);
        AccountViewModel GetAccountByName(string accountName);

        /// <summary>
        /// Adds the account.
        /// </summary>
        /// <param name="tenantId"> tenant identifier.</param>
        /// <param name="accountName">Name of the account.</param>
        /// <param name="addedBy"> added by.</param>
        /// <returns></returns>
        ServiceResult AddAccount(int tenantId, string accountName, string addedBy);

        /// <summary>
        /// Updates the account.
        /// </summary>
        /// <param name="tenantId"> tenant identifier.</param>
        /// <param name="accountId"> account identifier.</param>
        /// <param name="accountName">Name of the account.</param>
        /// <param name="updatedBy"> updated by.</param>
        /// <returns></returns>
        ServiceResult UpdateAccount(int tenantId, int accountId, string accountName, string updatedBy);

        /// <summary>
        /// Deletes the account.
        /// </summary>
        /// <param name="tenantId"> tenant identifier.</param>
        /// <param name="accountId"> account identifier.</param>
        /// <param name="updatedBy"> updated by.</param>
        /// <returns></returns>
        ServiceResult DeleteAccount(int tenantId, int accountId, string updatedBy);

        /// <summary>
        /// Gets the name of the account.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="accountId">The account identifier.</param>
        /// <returns></returns>
        string GetAccountName(int tenantId, int accountId);

        /// <summary>
        /// check if the user has permission to create the account
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        bool GetUserAccountCreationPermission(int? userId);

        #endregion

        #region  Account Search
        /// <summary>
        /// Gets the account details list.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="IsPortfolio">is portfolio.</param>
        /// <returns></returns> 
        GridPagingResponse<ConsumerAccountViewModel> GetAccountDetailsList(int tenantID, bool? isPortfolio, GridPagingRequest gridPagingRequest, string documentName, int RoleID);


        //TO DO:Method can be transfered to MarketSegment service interface
        /// <summary>
        /// This method returns a collection of Data from 'MarketSegment' table
        /// The collection is filtered using 'tenantId' which is passing as parameter.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        IEnumerable<MarketSegmentViewModel> GetMarketSegmentsList(int tenantId);

        //TO DO:Method can be transfered to User service interface
        /// <summary>
        /// This method returns a collection of Data from 'User' table.
        /// The collection is filtered using 'tenantId' which is passing as parameter.
        /// </summary>
        ///<param name="tenantId"></param>
        /// <returns></returns>
        IEnumerable<UserViewModel> GetPrimaryContactsList(int tenantId);

        /// <summary>
        /// This method adds Data in 'Folder','FolderVersion','FolderVersionWorkFlowState' table.
        /// </summary>
        ///<param name="tenantId"></param>
        /// <returns></returns>
        ServiceResult AddFolder(int tenantId, int? accountId, string folderName, DateTime folderEffectiveDate, bool isPortfolio, int? userId, string primaryContact, int marketSegmentId, Nullable<int> consortiumID, Nullable<int> categoryID, string catID, string addedBy, bool isFoundation =false);

        /// <summary>
        /// Gets the folder version details list
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="folderId">The folder identifier.</param>
        /// <returns></returns>
        IEnumerable<AccountDetailViewModel> GetFolderVersionDetailsList(int tenantId, int folderId, string accountName, int accountID, string documentName);

        /// <summary>
        /// Gets the details of Portfolio Folders list.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns></returns>
        IEnumerable<ConsumerAccountViewModel> GetPortfolioFoldersList(int tenantID);

        /// <summary>
        /// Gets the details of Non-Portfolio Folders list.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns></returns>
        GridPagingResponse<ConsumerAccountViewModel> GetNonPortfolioFoldersList(int tenantID, GridPagingRequest gridPagingRequest, string documentName, int RoleID);


        ServiceResult CopyFolder(int tenantId, int? accountId, int originalFolderID, int originalFolderVersionID,
                                 string folderName, DateTime folderEffectiveDate, bool isPortfolio, int userId, string primaryContact,
                                 int marketSegmentId, Nullable<int> consortiumID, Nullable<int> categoryID, string catID, string addedBy, bool isFoundation= false);

        IEnumerable<FormDesignAccountViewModel> GetFormDesignAccountMapping(int formdesignversionid);

        ServiceResult UpdateAccountProductMap(int tenantId, int formInstanceId, int folderId, int FolderVersionID, string propertyType, string propertyName, string planCode, string enteredby, string serviceGroup, string productName, string anocChartPlanType, string rXBenefit, string sNPType);

        GridPagingResponse<DocumentViewModel> GetDocumentsList(int tenantID, GridPagingRequest gridPagingRequest);

        GridPagingResponse<DocumentViewModel> GetDocumentsList(int tenantID, int formDesignID, GridPagingRequest gridPagingRequest);
        GridPagingResponse<DocumentViewModel> GetDocumentsList(int tenantID, int formDesignID, int formInstanceId, GridPagingRequest gridPagingRequest, string appName);
        GridPagingResponse<DocumentViewModel> GetDocumentsList(int tenantID, int formDesignID, GridPagingRequest gridPagingRequest, string appName);
        GridPagingResponse<DocumentViewModel> GetDocumentsListForQHP(int tenantID, GridPagingRequest gridPagingRequest);

        /// <summary>
        /// Gets the details of Portfolio Folders list.
        /// </summary>
        /// <param name="tenantID">The tenant identifier.</param>
        /// <returns></returns>
        GridPagingResponse<PortfolioFoldersDocumentViewModel> GetPortfolioFoldersDocumentsList(int tenantID, GridPagingRequest gridPagingRequest, int year);
        GridPagingResponse<PortfolioViewModel> GetPortfolioDetailsList(int tenantId, GridPagingRequest gridPagingRequest, string documentName, int year);
        GridPagingResponse<DocumentViewModel> GetDocumentsListForMedicalANOCChart(int tenantID, int formDesignId, string planType, GridPagingRequest gridPagingRequest);
        GridPagingResponse<DocumentViewModel> GetDocumentsListForMedicareANOCChart(int tenantID, int formDesignId, string planType, GridPagingRequest gridPagingRequest);
        ServiceResult UpdatePBPImportDetailsMap(int tenantId, int formInstanceId, int folderId, int folderVersionId, string planname, string plannumber, int docId, string userName);
        GridPagingResponse<PortfolioViewModel> GetPortfolioFolderListForPBPImport(int tenantId, GridPagingRequest gridPagingRequest, string documentName);
        #endregion accountSearch
        int GetFolderCategoryId(string category);

    }
}
