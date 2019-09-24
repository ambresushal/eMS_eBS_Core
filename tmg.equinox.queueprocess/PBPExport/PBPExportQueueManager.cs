using System;
using System.IO;
using System.Reflection;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.PBPImport;
using tmg.equinox.backgroundjob.Base;
using tmg.equinox.domain.entities.Enums;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.pbpexport;
using tmg.equinox.pbpimport.Interfaces;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.queueprocess.PBPExport
{
    public class PBPExportQueueManager
    {
        private IUnitOfWorkAsync _unitOfWorkAsync { get; set; }
        private IPBPExportServices _PBPExportService { get; set; }
        private IAccessDbContext _accessDbContext { get; set; }
        private IFormDesignService _formDesignService { get; set; }
        private IFolderVersionServices _folderVersionService { get; set; }
        private IFormInstanceDataServices _formInstanceDataService { get; set; }
        private IUIElementService _uiElementService { get; set; }
        private IFormInstanceService _formInstanceService { get; set; }
        private IReportingDataService _reportingDataService { get; set; }
        private IMasterListService _masterListService { get; set; }
        private IExportPreQueueService _exportPreQueueService { get; set; }

        private string _userId { get; set; }
        private string _userName { get; set; }
        private int _pbpDatabase1Up { get; set; }
        private string _RunExportRulesInWindowsService { get; set; }
        public PBPExportQueueManager(IUnitOfWorkAsync unitOfWorkAsync, IPBPExportServices pBPExportService, IAccessDbContext accessDbContext,
                                      IFormDesignService formDesignService, IFolderVersionServices folderVersionService,
                                      IFormInstanceDataServices formInstanceDataService, IUIElementService uiElementService, IFormInstanceService formInstanceService,
                                      IReportingDataService reportingDataService, IMasterListService masterListService, IExportPreQueueService exportPreQueueService,int pbpDatabase1up,
                                      string userId,string userName,string runExportRulesInWindowsService)
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _accessDbContext = accessDbContext;
            _PBPExportService = pBPExportService;
            this._formDesignService = formDesignService;
            this._folderVersionService = folderVersionService;
            this._formInstanceDataService = formInstanceDataService;
            this._uiElementService = uiElementService;
            this._formInstanceService = formInstanceService;
            this._reportingDataService = reportingDataService;
            this._masterListService = masterListService;
            this._exportPreQueueService = exportPreQueueService;
            this._userId = userId;
            this._userName = userName;
            this._pbpDatabase1Up = pbpDatabase1up;
            this._RunExportRulesInWindowsService = runExportRulesInWindowsService;
        }

        public void Execute(BaseJobInfo queue)
        {
            PBPExportQueueViewModel queueModel = _PBPExportService.GetQueuedPBPExport(queue.QueueId);
            if(queueModel.Status == (int)domain.entities.Enums.ProcessStatusMasterCode.Queued)
            {
                PreProcessRules(queue.QueueId);
            }
            //PBPExportToMDB exportObj = new PBPExportToMDB(_unitOfWorkAsync, _PBPExportService,null);

           // exportObj.GenerateMDBFile(queue.QueueId, queue.UserId);
        }

        private void PreProcessRules(int queueId)
        {
            if(_RunExportRulesInWindowsService == "Yes")
            {
                var location = System.Reflection.Assembly.GetEntryAssembly().Location;
                var directoryPath = Path.GetDirectoryName(location);
                //run Preprocessor
                Assembly assembly = Assembly.LoadFrom(directoryPath + @"/tmg.equinox.web.extensions.dll");
                Type preProc = assembly.GetType("tmg.equinox.web.extensions.PBPExportPreProcessor");
                ExportPreQueueViewModel model = new ExportPreQueueViewModel();
                object[] args = new object[14];
                int userIDNum;
                int.TryParse(_userId, out userIDNum);
                args[0] = _pbpDatabase1Up;
                args[1] = queueId;
                args[2] = userIDNum;
                args[3] = _userName;
                args[4] = _PBPExportService;
                args[5] = _formDesignService;
                args[6] = _folderVersionService;
                args[7] = _formInstanceDataService;
                args[8] = _uiElementService;
                args[9] = _formInstanceService;
                args[10] = _reportingDataService;
                args[11] = _masterListService;
                args[12] = _exportPreQueueService;
                args[13] = model;
                object obj = Activator.CreateInstance(preProc, args);
                MethodInfo preProcMethod = preProc.GetMethod("ProcessRulesAndSaveSections");
                object[] methodParams = new object[1];
                methodParams[0] = true;
                preProcMethod.Invoke(obj, methodParams);

                _PBPExportService.UpdateExportQueueStatus(queueId, ProcessStatusMasterCode.InProgress);
            }
        }
    }
}

