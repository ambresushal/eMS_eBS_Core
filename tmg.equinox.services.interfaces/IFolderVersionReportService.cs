using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.Reporting;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IFolderVersionReportService
    {


        IEnumerable<ReportingViewModel> GetFolderList(int tenantId, bool isPortfolio);
        IEnumerable<ReportingViewModel> GetFolderVersionList(int tenantId, int folderId, bool isPortfolio);
        IEnumerable<ReportingViewModel> GetTargetFormInstanceList(int tenantId, int foldersVersionId);
        IEnumerable<ReportingViewModel> GetSourceFormInstanceList(int tenantId, int sourceFolderVersionId);

        string GetFolderAccountName(int tenantId, int sourceFolderId);
        IEnumerable<ReportingViewModel> GetUIElementList(int tenantId, int formInstanceId);
        IEnumerable<ReportingViewModel> GetDataSourceList(int tenantId, int formInstanceId);
    }
    
}
