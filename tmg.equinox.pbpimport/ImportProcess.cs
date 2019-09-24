using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.pbpimport.Interfaces;
using tmg.equinox.repository.interfaces;


namespace tmg.equinox.pbpimport
{
    public class ImportProcess
    {
        private IFolderVersionServices _folderVersionService { get; set; }
        private ILoggingService _loggingService { get; set; }
        private IDomainModelService _domainModelService { get; set; }
        private IUnitOfWorkAsync _unitOfWorkAsync { get; set; }
        private IPBPImportService _pBPImportService { get; set; }
        private IFormDesignService _formDesignService { get; set; }
        private IAccessDbContext _accessDbContext { get; set; }
        private IPBPImportHelperServices _PBPImportHelperServices { get; set; }
        private ISQLImportOperations _SQLImportOperations { get; set; }
        private IPBPImportActivityLogServices _PBPImportActivityLogServices { get; set; }
        private IPBPMappingServices _PBPMappingServices { get; set; }

        public ImportProcess(IFolderVersionServices folderVersionService, ILoggingService loggingService, IDomainModelService domainModelService, IUnitOfWorkAsync unitOfWorkAsync, IPBPImportService pBPImportService, IFormDesignService formDesignService, IAccessDbContext accessDbContext, IPBPImportHelperServices PBPImportHelperServices, ISQLImportOperations SQLImportOperations, IPBPImportActivityLogServices PBPImportActivityLogServices, IPBPMappingServices PBPMappingServices)
        {
            this._folderVersionService = folderVersionService;
            this._loggingService = loggingService;
            this._domainModelService = domainModelService;
            this._unitOfWorkAsync = unitOfWorkAsync;
            this._pBPImportService = pBPImportService;
            this._formDesignService = formDesignService;
            _accessDbContext = accessDbContext;
            _PBPImportHelperServices = PBPImportHelperServices;
            _SQLImportOperations = SQLImportOperations;
            _PBPImportActivityLogServices = PBPImportActivityLogServices;
            _PBPMappingServices = PBPMappingServices;
        }

        public void Start(int pBPImportQueueID)
        {
            ImportSource ImportSourceObj = new ImportSource(_unitOfWorkAsync, _folderVersionService, _loggingService, _domainModelService, _pBPImportService, _formDesignService, _accessDbContext, _PBPImportHelperServices, _SQLImportOperations, _PBPImportActivityLogServices, _PBPMappingServices);
            ImportSourceObj.StartPBPImportOperation(pBPImportQueueID);
        }
    }
}
