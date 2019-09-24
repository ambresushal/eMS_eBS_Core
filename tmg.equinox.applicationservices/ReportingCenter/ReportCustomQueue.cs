using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.WCReport;
using tmg.equinox.backgroundjob;
using tmg.equinox.backgroundjob.Base;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.reporting;
using tmg.equinox.queueprocess.reporting;
using tmg.equinox.repository;

namespace tmg.equinox.applicationservices
{
    public class ReportCustomQueue : ICustomQueue
    {

        public ReportQueueInfo rqInfo { get; set; }
        public void UpdateQueue(BaseJobInfo baseInfo)
        {
            var id = baseInfo.JobId; //gets JobId here

            this.rqInfo = (ReportQueueInfo)baseInfo;

            IReportQueueServices reportQueueServices = new ReportQueueServices(new RptUnitOfWork());
            //Needs to insert JobId and status=Processing in ReportQueue table.
            reportQueueServices.UpdateReportQueue(this.rqInfo.QueueId, new ReportQueueViewModel { ReportQueueId= this.rqInfo.QueueId, JobId = Convert.ToInt32(this.rqInfo.JobId), Status = this.rqInfo.Status, DestinationPath=this.rqInfo.FilePath, FileName=this.rqInfo.FileName  });

        }

        //Not needed for now
        public void InsertQueue(BaseJobInfo baseInfo)
        {
            // throw new NotImplementedException();
            //insert
        }

        public void UpdateFailQueue(BaseJobInfo baseInfo)
        {
            var id = baseInfo.JobId; //gets JobId here

            this.rqInfo = (ReportQueueInfo)baseInfo;

            IReportQueueServices reportQueueServices = new ReportQueueServices(new RptUnitOfWork());
    
            //Needs to insert JobId and status=Processing in ReportQueue table.
            reportQueueServices.UpdateReportQueue(this.rqInfo.QueueId, new ReportQueueViewModel { ReportQueueId = this.rqInfo.QueueId, JobId = Convert.ToInt32(this.rqInfo.JobId), Status = this.rqInfo.Status, DestinationPath = this.rqInfo.FilePath, FileName = this.rqInfo.FileName, ErrorMessage = this.rqInfo.Error });
        }
     
    }

}
