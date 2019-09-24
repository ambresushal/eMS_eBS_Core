using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.PBPImport;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IExportPreQueueService
    {
        ExportPreQueueViewModel GetExportPreQueueList();
        ServiceResult UpdateStatus(int exportPreQueueId, string status);
        ServiceResult PreQueueExport(int pBPExportId, int pbpDatabaseId, string addedBy, int? userId);
        ServiceResult UpdatePreQueueLog(int forminstanceId, int exportPreQueueId, string status, Exception errorLog);
        ServiceResult SaveFormInstanceForPreQueue(List<ExportPreQueueLog> forinstanceIdList);
    }
}
