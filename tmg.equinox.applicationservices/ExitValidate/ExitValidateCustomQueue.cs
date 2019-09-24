using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.exitvalidate;
using tmg.equinox.backgroundjob;
using tmg.equinox.repository;

namespace tmg.equinox.applicationservices
{
    public class ExitValidateCustomQueue : ICustomQueue
    {
        public void UpdateQueue(backgroundjob.Base.BaseJobInfo evInfo)
        {
            var _ExitValidateService = new ExitValidateService(new UnitOfWork());
            _ExitValidateService.InitializeVariables(new UnitOfWork());
            _ExitValidateService.UpdateQueue(evInfo.QueueId, new ExitValidateViewModel { QueueID = evInfo.QueueId, JobId = Convert.ToInt32(evInfo.JobId), Status = evInfo.Status });
        }

        public void InsertQueue(backgroundjob.Base.BaseJobInfo baseInfo)
        {
            // throw new NotImplementedException();
            //insert
        }

        public void UpdateFailQueue(backgroundjob.Base.BaseJobInfo evInfo)
        {
            var _ExitValidateService = new ExitValidateService(new UnitOfWork());
            _ExitValidateService.InitializeVariables(new UnitOfWork());
            _ExitValidateService.UpdateQueue(evInfo.QueueId, new ExitValidateViewModel { QueueID = evInfo.QueueId, JobId = Convert.ToInt32(evInfo.JobId), Status = evInfo.Status, ErrorMessage = evInfo.Error });
        }
    }
}
