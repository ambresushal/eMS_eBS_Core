using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.ConsumerAccount;
using tmg.equinox.applicationservices.viewmodels.Qhp;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IQhpIntegrationService
    {
//Commented line - these lines are commented in Jamirs Copy.
//Discussion needed with Jamir on same
        //ServiceResult ImportQhpExcelTemplate(int tenantID, int formDesignVersionID, int folderVersionID, int folderID, string addedBy, string filePath);

        ServiceResult ExportQhpExcelTemplate(int tenantID, int folderVersionID, List<int> formInstanceList, string folderPath, out string filePath, bool offExchangeOnly);
        string GetSourceDocumentData(int formInstanceID);
        //IList<QHPValidationError> ValidateQhpDocument(int tenantID, int formDesignVersionID, int folderVersionID, int folderID, List<int> formInstanceList);

        //string GetQHPDataForFolder(int tenantID, string folderName, string folderVersion, string accountName);
        QHPReportingQueue UpdateQHPReportQueue(List<DocumentViewModel> documents, string CurrentUserName, bool offExchangeOnly);
        void UpdateQHPReportQueueStatus(QHPReportingQueue queue, string QueueStatus, string FilePath, string MessageText);
        GridPagingResponse<QHPReportQueueViewModel> GetQHPReportQueueList(int tenantID, GridPagingRequest gridPagingRequest);
        string GetQHPReportPath(int QHPQueueID);
        QHPReportQueueViewModel GetErrorDescription(int QueueID);
        List<QhpPackageGroup> BuildQhpPackageGroups(List<int> documents);
        List<QHPFormInstanceViewModel> GetFormInstances(List<int> formInstanceList);
    }
}
