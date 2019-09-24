using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IFolderService
    {
        List<FolderViewModel> GetFoldersByAccount(int accountId);

        List<FolderViewModel> GetPortfolioFolders(int skip, int pageSize, ref int total);

        List<FolderViewModel> GetFolderList(int skip, int pageSize, ref int total);

        FolderViewModel GetFolderById(int folderId);

        ServiceResult AddFolder(int tenantId, int? accountId, string folderName, DateTime effectiveDate, bool isPortfolio, int? userId, string primaryContact, string marketSegment, string category);

        ServiceResult DeleteFolder(int tenantId, int folderId);

        FolderViewModel GetFolderByName(string folderName);
    }
}
