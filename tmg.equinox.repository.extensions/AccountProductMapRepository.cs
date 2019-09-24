using System.Linq;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.repository.extensions
{
    public static class AccountProductMapRepository
    {
        #region Private Memebers

        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor

        #endregion Constructor

        #region Public Methods
        public static bool IsFolderNameExistsInAccount(this IRepositoryAsync<AccountFolderMap> accountFolderMapRepository, int tenantID, int? accountID, int folderID, string folderName)
        {
            
                if (folderID > 0)
                {

                    return accountFolderMapRepository
                            .Query()
                            .Include(includeFolder => includeFolder.Folder)
                            .Filter(c => c.Folder.Name == folderName && c.Folder.TenantID == tenantID && c.FolderID != folderID && c.AccountID == accountID)
                            .Get()
                            .Any();
                }
                else
                {
                    return accountFolderMapRepository
                          .Query()
                          .Include(includeFolder => includeFolder.Folder)
                          .Filter(c => c.Folder.Name == folderName && c.Folder.TenantID == tenantID && c.AccountID == accountID)
                          .Get()
                          .Any();

                }
            
        }
        #endregion Public Methods

        #region Private Methods

        #endregion Private Methods

    }
}
