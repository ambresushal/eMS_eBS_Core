using System;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.backgroundjob;
using tmg.equinox.repository;

namespace tmg.equinox.applicationservices.PBPImport
{
    public class PBPImportCustomQueue : ICustomQueue
    {
        public backgroundjob.Base.BaseJobInfo rqInfo { get; set; }

        public void UpdateQueue(backgroundjob.Base.BaseJobInfo baseInfo)
        {
            IPBPImportService PBPImportServiceObj = new PBPImportService(new UnitOfWork());
            PBPImportServiceObj.UpdateImportQueueStatus(baseInfo.QueueId, domain.entities.Enums.ProcessStatusMasterCode.InProgress);
        }

        //Not needed for now
        public void InsertQueue(backgroundjob.Base.BaseJobInfo baseInfo)
        {
            // throw new NotImplementedException();
            //insert
        }

        public void UpdateFailQueue(backgroundjob.Base.BaseJobInfo baseInfo)
        {
            throw new NotImplementedException();
        }

        public void UpdateJobIdInQueue(backgroundjob.Base.BaseJobInfo baseInfo)
        {
            IPBPImportService PBPImportServiceObj = new PBPImportService(new UnitOfWork());
            PBPImportServiceObj.UpdateJobId(baseInfo.QueueId,Convert.ToInt32(baseInfo.JobId), baseInfo.ClassName);
        }
    }
}
