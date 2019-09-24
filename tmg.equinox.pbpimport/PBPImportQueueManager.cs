using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.PBPImport;
using tmg.equinox.backgroundjob.Base;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.pbpimport
{

    //public class QueueItem : BaseJobInfo
    //{

    //}

    public class PBPImportQueueManager
    {
        public void Execute(BaseJobInfo queue)
        {
            ImportProcess Obj = new ImportProcess();
            Obj.Start(queue.QueueId);

            //write job id in queue table
            //UpdateJobId(queue);
            //UpdateQueueStatus(queue);
            //write status update code
        }

        //public void UpdateJobId(BaseJobInfo PBPImportQueue)
        //{
        //    PBPImportCustomQueue PBPImportCustomQueueObj = new PBPImportCustomQueue();
        //    PBPImportCustomQueueObj.UpdateJobIdInQueue(new BaseJobInfo
        //    {
        //        QueueId = PBPImportQueue.QueueId,
        //        JobId = PBPImportQueue.JobId,
        //        ClassName = PBPImportQueue.ClassName,
        //        Error = PBPImportQueue.Error
        //    });
        //}

        //public void UpdateQueueStatus(BaseJobInfo PBPImportQueue)
        //{
        //    PBPImportCustomQueue PBPImportCustomQueueObj = new PBPImportCustomQueue();
        //    PBPImportCustomQueueObj.UpdateQueue(new BaseJobInfo
        //    {
        //        QueueId = PBPImportQueue.QueueId,
        //        JobId = PBPImportQueue.JobId,
        //        ClassName = PBPImportQueue.ClassName
        //    });
        //}
    }
}

