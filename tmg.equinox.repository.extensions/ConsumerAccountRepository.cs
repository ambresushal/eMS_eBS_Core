using System.Linq;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.repository.extensions
{
    public static class ConsumerAccountRepository
    {
        #region Private Memebers

        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor
        #endregion Constructor

        #region Public Methods
        public static bool IsAccountNameExists(this IRepositoryAsync<Account> accountRepository, int tenantId, int accountId, string accountName)
        {
            if (accountId > 0)
            {
                return accountRepository
                        .Query()
                        .Filter(c => c.AccountName == accountName && c.TenantID == tenantId && c.AccountID != accountId )
                        .Get()
                        .Any();
            }
            else
            {
                return accountRepository
                       .Query()
                       .Filter(c => c.AccountName == accountName && c.TenantID == tenantId && c.IsActive == true)
                       .Get()
                       .Any();
            }
        }

        public static bool IsFolderNameExists(this IRepositoryAsync<Folder> folderRepository, int tenantId, int folderId, string folderName)
        {
            if (folderId > 0)
            {
                return folderRepository
                        .Query()
                        .Filter(c => c.Name == folderName && c.TenantID == tenantId && c.FolderID != folderId && c.IsPortfolio == true)
                        .Get()
                        .Any();
            }
            else
            {
                return folderRepository
                       .Query()
                       .Filter(c => c.Name == folderName && c.TenantID == tenantId && c.IsPortfolio == true)
                       .Get()
                       .Any();
            }
        }

        public static int GetFormDeisgnGroupID(this IRepositoryAsync<FormDesignGroup> formDeisgnGroupRepository, int tenantID, string folderType)
        {
            return formDeisgnGroupRepository
                .Query()
                .Filter(c => c.TenantID == tenantID && c.GroupName == folderType)
                .Get()
                .Select(sel => sel.FormDesignGroupID)
                .FirstOrDefault();
        }
        
        #endregion Public Methods

        #region Private Methods

        #endregion Private Methods
    }
}
