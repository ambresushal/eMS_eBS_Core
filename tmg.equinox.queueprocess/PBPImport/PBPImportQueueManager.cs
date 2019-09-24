using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.backgroundjob.Base;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.pbpimport;
using tmg.equinox.pbpimport.Interfaces;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.queueprocess.PBPImport
{
    public class PBPImportQueueManager
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

        public PBPImportQueueManager(IFolderVersionServices folderVersionService, ILoggingService loggingService, IDomainModelService domainModelService, IUnitOfWorkAsync unitOfWorkAsync, IPBPImportService pBPImportService, IFormDesignService formDesignService, IAccessDbContext accessDbContext, IPBPImportHelperServices PBPImportHelperServices, ISQLImportOperations SQLImportOperations, IPBPImportActivityLogServices PBPImportActivityLogServices, IPBPMappingServices PBPMappingServices)
        {
            _folderVersionService = folderVersionService;
            _loggingService = loggingService;
            _domainModelService = domainModelService;
            _unitOfWorkAsync = unitOfWorkAsync;
            _pBPImportService = pBPImportService;
            _formDesignService = formDesignService;
            _accessDbContext = accessDbContext;
            _PBPImportHelperServices = PBPImportHelperServices;
            _SQLImportOperations = SQLImportOperations;
            _PBPImportActivityLogServices = PBPImportActivityLogServices;
            _PBPMappingServices = PBPMappingServices;
        }

        public void Execute(BaseJobInfo queue)
        {
            ImportProcess Obj = new ImportProcess(_folderVersionService, _loggingService, _domainModelService, _unitOfWorkAsync, _pBPImportService, _formDesignService, _accessDbContext, _PBPImportHelperServices, _SQLImportOperations, _PBPImportActivityLogServices, _PBPMappingServices);
            Obj.Start(queue.QueueId);
        }
    }
}

