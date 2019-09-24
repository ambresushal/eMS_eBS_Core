using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.exitvalidate;
using tmg.equinox.backgroundjob.Base;
using tmg.equinox.pbpexport;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.queueprocess.exitvalidate
{
    public class ExitValidateQueueManager
    {
        private IUnitOfWorkAsync _unitOfWorkAsync;
        private IPBPExportServices _PBPExportService;
        private IExitValidateService _evService { get; set; }

        static readonly object _lockObject = new object();
        public ExitValidateQueueManager(IUnitOfWorkAsync unitOfWorkAsync, IPBPExportServices pBPExportService,IExitValidateService evService)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _PBPExportService = pBPExportService;
            _evService = evService;
        }

        public void Execute(BaseJobInfo jobInfo)
        {
            lock (_lockObject)
            {
                PBPExportToMDB exportObj = new PBPExportToMDB(_unitOfWorkAsync, _PBPExportService, _evService);
                string qid = exportObj.ExitValidateGenerateMDBFile(jobInfo.QueueId, jobInfo.UserId);
                //process exit validate
                ExitValidateViewModel evModel = _evService.GetExitValidateMappings(jobInfo.QueueId);
                //process exit validate automation
                evModel.QID = qid;
                bool isKilledExitValidate;
                string reportFileName = _evService.ProcessPBPExitValidate(evModel, out isKilledExitValidate);
                //parse and store exit validate results
                _evService.AddExitValidateResults(evModel, reportFileName);
                if (isKilledExitValidate)
                {
                    throw new Exception("Exit Validate process has been killed due to long processing time. Please resolve the error(s) and Re-ExitValidate the plan.");
                }
            }
        }
    }
}
