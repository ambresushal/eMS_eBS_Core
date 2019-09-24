using Hangfire;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.backgroundjob;
using tmg.equinox.hangfire.Jobfilters;
using tmg.equinox.pbpimport;
using tmg.equinox.pbpimport.Interfaces;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.queueprocess.PBPImport
{
    [Queue("import"), HangfireStateFilter()]
    public class PBPImportBackgroundJob : BackgroundJob<PBPImportQueueInfo>
    {
        IFolderVersionServices _folderVersionService;
        ILoggingService _loggingService;
        IDomainModelService _domainModelService;
        IUnitOfWorkAsync _unitOfWorkAsync;
        IPBPImportService _pBPImportService;
        IFormDesignService _formDesignService;
        IAccessDbContext _accessDbContext;
        IPBPImportHelperServices _PBPImportHelperServices;
        ISQLImportOperations _SQLImportOperations;
        IPBPImportActivityLogServices _PBPImportActivityLogServices;
        IPBPMappingServices _PBPMappingServices;

        public PBPImportBackgroundJob(IFolderVersionServices folderVersionService, ILoggingService loggingService, IDomainModelService domainModelService, IUnitOfWorkAsync unitOfWorkAsync, IPBPImportService pBPImportService, IFormDesignService formDesignService, IAccessDbContext accessDbContext, IPBPImportHelperServices PBPImportHelperServices, ISQLImportOperations SQLImportOperations, IPBPImportActivityLogServices PBPImportActivityLogServices, IPBPMappingServices PBPMappingServices)
        {
            this._folderVersionService = folderVersionService;
            this._loggingService = loggingService;
            this._domainModelService = domainModelService;
            this._unitOfWorkAsync = unitOfWorkAsync;
            this._pBPImportService = pBPImportService;
            this._formDesignService = formDesignService;
            this._accessDbContext = accessDbContext;
            this._PBPImportHelperServices = PBPImportHelperServices;
            this._SQLImportOperations = SQLImportOperations;
            this._PBPImportActivityLogServices = PBPImportActivityLogServices;
            this._PBPMappingServices = PBPMappingServices;
        }

        [Queue("import")]
        public override void Execute(PBPImportQueueInfo PBPImportinfo)
        {
            //Business Logic goes here
            PBPImportQueueManager _pBPImportQueueManager = new PBPImportQueueManager(_folderVersionService, _loggingService, _domainModelService, _unitOfWorkAsync, _pBPImportService, _formDesignService, _accessDbContext, _PBPImportHelperServices, _SQLImportOperations, _PBPImportActivityLogServices, _PBPMappingServices);
            _pBPImportQueueManager.Execute(PBPImportinfo);
        }
    }
}
