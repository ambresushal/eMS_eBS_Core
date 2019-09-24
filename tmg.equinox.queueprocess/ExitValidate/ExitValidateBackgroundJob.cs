using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.backgroundjob.Base;
using tmg.equinox.hangfire;
using Hangfire;
using tmg.equinox.hangfire.Jobfilters;
using tmg.equinox.repository.interfaces;
using tmg.equinox.pbpimport.Interfaces;

namespace tmg.equinox.queueprocess.exitvalidate
{
    [Queue("exitvalidate"), HangfireStateFilter()]
    public class ExitValidateBackgroundJob : BaseBackgroundJob<ExitValidateQueueInfo>
    {
        private IUnitOfWorkAsync _unitOfWorkAsync { get; set; }
        private IPBPExportServices _PBPExportService { get; set; }
        private IAccessDbContext _accessDbContext { get; set; }

        private IExitValidateService _evService { get; set; }

        public ExitValidateBackgroundJob(IUnitOfWorkAsync unitOfWorkAsync, IPBPExportServices pBPExportService, IAccessDbContext accessDbContext,IExitValidateService evService)
        {
            this._unitOfWorkAsync = unitOfWorkAsync;
            this._PBPExportService = pBPExportService;
            this._accessDbContext = accessDbContext;
            this._evService = evService;
        }

        [Queue("exitvalidate")]
        public override void Execute(ExitValidateQueueInfo ExitValidateInfo)
        {
            ExitValidateQueueManager _exitValidateQueueManager = new ExitValidateQueueManager(_unitOfWorkAsync, _PBPExportService,_evService);
            _exitValidateQueueManager.Execute(ExitValidateInfo);
        }

    }

    [Queue("exitvalidatelow"), HangfireStateFilter()]
    public class ExitValidateBackgroundJobLowPriority : BaseBackgroundJob<ExitValidateQueueInfo>
    {
        private IUnitOfWorkAsync _unitOfWorkAsync { get; set; }
        private IPBPExportServices _PBPExportService { get; set; }
        private IAccessDbContext _accessDbContext { get; set; }

        private IExitValidateService _evService { get; set; }

        public ExitValidateBackgroundJobLowPriority(IUnitOfWorkAsync unitOfWorkAsync, IPBPExportServices pBPExportService, IAccessDbContext accessDbContext, IExitValidateService evService)
        {
            this._unitOfWorkAsync = unitOfWorkAsync;
            this._PBPExportService = pBPExportService;
            this._accessDbContext = accessDbContext;
            this._evService = evService;
        }

        
        [Queue("exitvalidatelow")] 
        public override void Execute(ExitValidateQueueInfo ExitValidateInfo)
        {
            ExitValidateQueueManager _exitValidateQueueManager = new ExitValidateQueueManager(_unitOfWorkAsync, _PBPExportService, _evService);
            _exitValidateQueueManager.Execute(ExitValidateInfo);
        }

    }
}
