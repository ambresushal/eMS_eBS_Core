using System;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.backgroundjob.Base;

namespace tmg.equinox.savetoreportingdbmlcascade
{
    public class ReportDBSaveManager<T> : IReportDBSaveManager<T> where T : BaseJobInfo
    {
        private IFolderVersionServices _folderVersionService;
        private static readonly Object _lock = new Object();
        public ReportDBSaveManager(IFolderVersionServices folderVersionService)
        {
            _folderVersionService = folderVersionService;
        }
        public bool Execute(ReportingDBQueueInfo queueInfo)
        {
            int tenantId = 1;
            bool result = false;
            try
            {
                _folderVersionService.UpdateReportingCenterDatabase(queueInfo.FormInstanceId, null, false);
            }

            catch (Exception ex)
            {

            }
            return result;
        }
    }
}
