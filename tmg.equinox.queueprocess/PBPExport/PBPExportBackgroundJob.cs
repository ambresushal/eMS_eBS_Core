using Hangfire;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.backgroundjob;
using tmg.equinox.hangfire.Jobfilters;
using tmg.equinox.pbpexport;
using tmg.equinox.pbpimport.Interfaces;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.queueprocess.PBPExport
{
    [Queue("export"), HangfireStateFilter()]
    public class PBPExportBackgroundJob : BackgroundJob<PBPExportQueueInfo>
    {
        private IUnitOfWorkAsync _unitOfWorkAsync { get; set; }
        private IPBPExportServices _PBPExportService { get; set; }
        private IFormDesignService _formDesignService { get; set; }
        private IFolderVersionServices _folderVersionService { get; set; }
        private IFormInstanceDataServices _formInstanceDataService { get; set; }
        private IUIElementService _uiElementService { get; set; }
        private IFormInstanceService _formInstanceService { get; set; }
        private IReportingDataService _reportingDataService { get; set; }
        private IMasterListService _masterListService { get; set; }
        private IExportPreQueueService _exportPreQueueService { get; set; }
        private IAccessDbContext _accessDbContext { get; set; }

        public PBPExportBackgroundJob(IUnitOfWorkAsync unitOfWorkAsync, IPBPExportServices pBPExportService, IAccessDbContext accessDbContext,
                                      IFormDesignService formDesignService, IFolderVersionServices folderVersionService,
                                      IFormInstanceDataServices formInstanceDataService,IUIElementService uiElementService, IFormInstanceService formInstanceService,
                                      IReportingDataService reportingDataService,IMasterListService masterListService, IExportPreQueueService exportPreQueueService)
        {
            this._unitOfWorkAsync = unitOfWorkAsync;
            this._PBPExportService = pBPExportService;
            this._accessDbContext = accessDbContext;
            this._formDesignService = formDesignService;
            this._folderVersionService = folderVersionService;
            this._formInstanceDataService = formInstanceDataService;
            this._uiElementService = uiElementService;
            this._formInstanceService = formInstanceService;
            this._reportingDataService = reportingDataService;
            this._masterListService = masterListService;
            this._exportPreQueueService = exportPreQueueService;
        }

        [Queue("export")]
        public override void Execute(PBPExportQueueInfo PBPExportInfo)
        {
            
            PBPExportQueueManager _pBPExportQueueManager = new PBPExportQueueManager(_unitOfWorkAsync, _PBPExportService, _accessDbContext,_formDesignService,
                                                            _folderVersionService,_formInstanceDataService,_uiElementService,_formInstanceService,_reportingDataService,
                                                            _masterListService,_exportPreQueueService, PBPExportInfo.pbpDatabase1Up, PBPExportInfo.UserId,
                                                            PBPExportInfo.UserName,PBPExportInfo.RunExportRulesInWindowsService);
            _pBPExportQueueManager.Execute(PBPExportInfo);
        }
    }
}
