using System;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.PBPImport;
using tmg.equinox.backgroundjob;
using tmg.equinox.repository;

namespace tmg.equinox.applicationservices
{
    public class PBPImportCustomQueue : ICustomQueue
    {
        public void UpdateQueue(backgroundjob.Base.BaseJobInfo pbpInfo)
        {
            var _PBPImportService = new PBPImportService(new UnitOfWork());
            _PBPImportService.InitializeVariables(new UnitOfWork());
            _PBPImportService.UpdateImportQueue(pbpInfo.QueueId, new PBPImportQueueViewModel { PBPImportQueueID = pbpInfo.QueueId, JobId = Convert.ToInt32(pbpInfo.JobId), ImportStatus = pbpInfo.Status });
        }

        public void InsertQueue(backgroundjob.Base.BaseJobInfo baseInfo)
        {
            // throw new NotImplementedException();
            //insert
        }

        public void UpdateFailQueue(backgroundjob.Base.BaseJobInfo pbpInfo)
        {
            var _PBPImportService = new PBPImportService(new UnitOfWork());
            _PBPImportService.InitializeVariables(new UnitOfWork());
            _PBPImportService.UpdateImportQueue(pbpInfo.QueueId, new PBPImportQueueViewModel { PBPImportQueueID = pbpInfo.QueueId, JobId = Convert.ToInt32(pbpInfo.JobId), ImportStatus = pbpInfo.Status, ErrorMessage = pbpInfo.Error });
        }
    }
}
