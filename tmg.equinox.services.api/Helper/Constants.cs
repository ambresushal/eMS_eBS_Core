using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tmg.equinox.services.api.Validators
{
    public class Constants
    {
        #region Folder
        public const string FolderNotExist = "Folder does not exist.";
        public const string AccountIDEmpty = "Account Id cannot be blank";
        public const string FolderisPortfolio = "Please specify isPortfolio value";
        public const string FolderIDNotExist = "FolderId does not exist.";
        public const string ForlderNameEmpty = "The folder name cannot be blank.";
        public const string ForlderNameLength = "The folder name cannot be more than 200 characters.";
        public const string ForlderEffectiveDateEmpty = "The effective date cannot be blank.";
        public const string ForlderInvalidEffectiveDate = "Invalid folder effective date.";
        public const string ForlderMarketSegmentEmpty = "Market Segment cannot be blank.";
        public const string ForlderInvalidMarketSegment = "Invalid Market Segment, please select from - Largr ASC, iddle ASC, Underwritten Small, On Exchange.";
        public const string ForlderCategoryEmpty = "Category cannot be blank.";
        public const string FolderCategoryIdEmpty = "Category Id cannot be blank.";
        public const string ForlderNonPortfolioInvalidCategory = "Invalid Category, please select from - New Account, Renewal, Revision, Termination.";
        public const string ForlderPortfolioInvalidCategory = "Invalid Category, please select from - Templates.";
        public const string FolderVersionId = "FolderVersionId cannot be blank";
        public const string WFStateName = "WorkFlowState cannot be blank";
        public const string ForlderInvalidCategory = "Invalid Category, please select from - Default, Medicare, Commercial.";
        #endregion

        #region Folder Version
        public const string FolderVersionNotExist = "Folder versionId does not exist.";
        public const string FolderVersionIdEmpty = "FolderVersionId cannot be blank";
        public const string FolderVersionsDoNotExist = "There are no folderVersions for this folder.";
        public const string FolderVersionEffectiveDateEmpty = "The effective date cannot be blank.";
        public const string FolderVersionInvalidEffectiveDate = "Invalid folder effective date.";
        public const string FolderVersionCommentsEmpty = "Comments cannot be blank.";
        public const string FolderVersionCommentsLength = "Comments cannot be more than 200 characters.";
        public const string ProdctIDNotExist = "Product ID does not Exist.";
        public const string FolderIDEmpty = "The folder Id cannot be blank.";
        public const string EffectiveDateEmpty = "The effective date cannot be blank.";
        public const string QuoteIdEmpty = "The quote Id cannot be blank.";
        public const string DocumentEmpty = "Document does not exist in the folderVersion.";
        #endregion

        #region Account
        public const string AccountNotExist = "Account does not exist.";
        public const string AccountNameEmpty = "The account name cannot be blank.";
        public const string AccountNameLength = "The account name cannot be more than 200 characters.";
        #endregion

        #region Consortium
        public const string ConsortiumNotExist = "Consortium does not exist.";
        public const string ConsortiumNameEmpty = "The consortium name cannot be blank.";
        public const string ConsortiumNameLength = "The consortium name cannot be more than 200 characters.";
        #endregion

        #region Document
        public const string DocumentNotExist = "Document does not exist.";
        public const string DocumentNotActive = "There are no Active Documents for the folderVersion.";
        public const string DocumentSectionsNotExist = "The following sections {1} does not exist.";
        public const string DocumentRepeatersNotExist = "The following repeaters {1} does not exist.";
        public const string DocumentNameEmpty = "The document name cannot be blank.";
        public const string DocumentNameLength = "The document name cannot be more than 200 characters.";
        public const string DesignNameEmpty = "The design name cannot be blank.";
        public const string DesignNameLength = "The design name cannot be more than 200 characters.";
        public const string DocumentInvalidData = "The document data name cannot be blank.";
        #endregion

        public const string Failure = "Failure";

        public const string Success = "Success";
        public const string NotExist = "Resource does not exist.";
        public const string UpdateFail = "Unable to update";
        public const string UpdateSuccess = "Updated Successfully";
        public const string CreateSuccess = "Created Successfully";
    }
}