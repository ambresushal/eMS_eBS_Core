using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using tmg.equinox.generatecollateral;
using tmg.equinox.applicationservices;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.dependencyresolution;
using tmg.equinox.applicationservices.viewmodels.CollateralWindowService;
using tmg.equinox.web.FormDesignManager;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.web.FormInstanceProcessor;
using tmg.equinox.web.FormInstanceManager;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.applicationservices.viewmodels.Account;
using System.IO;
using tmg.equinox.web.Framework.Caching;
using tmg.equinox.setting.Interface;
using tmg.equinox.web.extensions;
using tmg.equinox.domain.entities.Enums;
using tmg.equinox.applicationservices.viewmodels.PBPImport;
using System.Configuration;

namespace CollateralService
{
    public partial class CollateralService : ServiceBase
    {
        private ICollateralWindowServices _reportWindowsService;
        private ICollateralService _collateralService;
        private IUIElementService _uiElementService;
        private IFormDesignService _formDesignService;
        private IFolderVersionServices _folderVersionService;
        private IFormInstanceService _formInstanceService;
        private IFormInstanceDataServices _formInstanceDataService;
        private IMasterListService _masterListService;
        private IAccountService _accountService;
        ISettingManager _settingManager;
        private Timer timerForServcie = null;
        bool _isprocessing = false;
        bool _isPreExportProcessing = false;
        ScheduleEquinoxService scheduleService;
        string imagePath = string.Empty;
        private Timer timerForDeleteCollateralServcie = null;
        bool _isDeleteCollateraProcessing = false;
        int deleteCollaterNoOfDays = 7;
        DateTime _scheduleTimeForDeleteCollateral;
        private IPBPExportServices _pbpExportServices;
        private IReportingDataService _reportingDataService;
        private IExportPreQueueService _exportPreQueueService;
        private IMDMSyncDataService _mDMSyncDataService;
        //public static void Main(string[] args)
        //{
        //    ServiceBase.Run(new CollateralService());
        //}

        public CollateralService()
        {
            string[] d = new string[2];
            this.OnStart(d);


            UnityConfig.RegisterComponents();
            //InitializeComponent();
            ServiceName = "CollateralService_QueueOperation";

            if (ConfigurationManager.AppSettings["CollateralServiceName"]!=null)
            {
                ServiceName = ConfigurationManager.AppSettings["CollateralServiceName"].ToString();
            }
            _reportWindowsService = UnityConfig.Resolve<ICollateralWindowServices>();
            _uiElementService = UnityConfig.Resolve<IUIElementService>();
            _formInstanceService = UnityConfig.Resolve<IFormInstanceService>();
            _formInstanceDataService = UnityConfig.Resolve<IFormInstanceDataServices>();
            _masterListService = UnityConfig.Resolve<IMasterListService>();
            _collateralService = UnityConfig.Resolve<ICollateralService>();

            _formDesignService = UnityConfig.Resolve<IFormDesignService>();
            _folderVersionService = UnityConfig.Resolve<IFolderVersionServices>();
            _accountService = UnityConfig.Resolve<IAccountService>();
            _settingManager = UnityConfig.Resolve<ISettingManager>();
            _pbpExportServices = UnityConfig.Resolve<IPBPExportServices>();
            _mDMSyncDataService = UnityConfig.Resolve<IMDMSyncDataService>();
            _reportingDataService = UnityConfig.Resolve<IReportingDataService>();
            _exportPreQueueService = UnityConfig.Resolve<IExportPreQueueService>();
            
            this.OnStart(d);

        }

        protected override void OnStart(string[] args)
        {
            timerForServcie = new System.Timers.Timer(10000);
            this.timerForServcie.Interval = 10000;
            this.timerForServcie.Elapsed += new ElapsedEventHandler(this.timerForServcie_Elapsed);
            this.timerForServcie.Enabled = true;
            this._isprocessing = false;
            imagePath = System.Configuration.ConfigurationManager.AppSettings["CollateralImagePath"];

            //Timer for Delete Collaterals generated seven days before
            string hoursString = System.Configuration.ConfigurationManager.AppSettings["DeleteCollateralHoursInterval"];
            int deleteCollateralHours = 16;
            if (!int.TryParse(hoursString, out deleteCollateralHours))
                deleteCollateralHours = 16;
            _scheduleTimeForDeleteCollateral = DateTime.Today.AddDays(1).AddHours(deleteCollateralHours);
            string deleteCollaterNoOfDaysString = System.Configuration.ConfigurationManager.AppSettings["DeleteCollaterNoOfDays"];
            if (!int.TryParse(deleteCollaterNoOfDaysString, out deleteCollaterNoOfDays))
                deleteCollaterNoOfDays = 7;
            double intervalValue = _scheduleTimeForDeleteCollateral.Subtract(DateTime.Now).TotalSeconds * 1000;
            timerForDeleteCollateralServcie = new System.Timers.Timer(intervalValue);
            this.timerForDeleteCollateralServcie.Interval = intervalValue;
            this.timerForDeleteCollateralServcie.Elapsed += new ElapsedEventHandler(this.TimerForDeleteCollateralServcie_Elapsed);
            this.timerForDeleteCollateralServcie.Enabled = true;
            this._isDeleteCollateraProcessing = false;

            //scheduleService = new ScheduleEquinoxService();
            //scheduleService.SchedulePBPImportService();

            //string RunExportRulesInWindowsService = ConfigurationManager.AppSettings["RunExportRulesInWindowsService"] ?? string.Empty;
            //if (RunExportRulesInWindowsService.Equals("Yes"))
            //{
            //    this.ExportPreProcessor();
            //}
        }

        void timerForServcie_Elapsed(object sender, ElapsedEventArgs e)
        {
            string clientName = ConfigurationManager.AppSettings["ClientName"] ?? string.Empty;
            if (clientName == "WellCare")
            {
                string RunExportRulesInWindowsService = ConfigurationManager.AppSettings["RunExportRulesInWindowsService"] ?? string.Empty;
                if (RunExportRulesInWindowsService.Equals("Yes"))
                {
                    if (!_isPreExportProcessing)
                    {
                        Library.WriteErrorLog("Test window service started");
                        Library.WriteErrorLog("ExportPreProcessor() service started");
                        this.ExportPreProcessor();
                        Library.WriteErrorLog("ExportPreProcessor() service end");
                    }
                }
            }
            else
            {

                Library.WriteErrorLog(_isprocessing.ToString());

                if (!_isprocessing)
                {
                    _isprocessing = true;
                    Library.WriteErrorLog("Test window service started");
                    ColleteralFilePath pdfFilePath = new ColleteralFilePath() ;
                    string wordFilePath = string.Empty;
                    try
                    {
                        List<CollateralProcessGovernanceViewModel> objList = _reportWindowsService.GetCollateralProcessGovernance().ToList();
                        //   _reportWindowsService.AddLogEntry(1, 1, 1, "Fetching collateral Process Governance data is started");

                        foreach (CollateralProcessGovernanceViewModel objCollProcGovM in objList)
                        {
                            try
                            {
                                _reportWindowsService.LoadActivity(1, objCollProcGovM.ProcessGovernance1up, "collateral Process for Governance = " + objCollProcGovM.ProcessGovernance1up.ToString());
                                _reportWindowsService.AddLogEntry(1, objCollProcGovM.ProcessGovernance1up, 1, "Fetching queued collateral for Governance = " + objCollProcGovM.ProcessGovernance1up.ToString());
                                List<CollateralProcessQueueViewModel> collateralProcessQueueData = _reportWindowsService.GetStatuswiseCollateralProcessQueue(objCollProcGovM.ProcessGovernance1up, 1).ToList();
                                _reportWindowsService.AddLogEntry(1, objCollProcGovM.ProcessGovernance1up, 1, "total count for queued collateral for Governance = " + objCollProcGovM.ProcessGovernance1up.ToString() + "are" + collateralProcessQueueData.Count.ToString());
                                _reportWindowsService.UpdateCollateralProcessGovData(objCollProcGovM.ProcessGovernance1up, 2);
                                foreach (CollateralProcessQueueViewModel queue in collateralProcessQueueData)
                                {
                                    int sbFormDesignVersionID;
                                    bool isSBDesign = _reportWindowsService.HasSBDesignTemplate(Convert.ToInt32(queue.TemplateReportID), Convert.ToInt32(queue.TemplateReportVersionID), out sbFormDesignVersionID);
                                    bool isEOCDesign = _reportWindowsService.HasEOCCombinedDesignTemplate(Convert.ToInt32(queue.TemplateReportID), Convert.ToInt32(queue.TemplateReportVersionID), out sbFormDesignVersionID);
                                    try
                                    {
                                        _reportWindowsService.UpdateCollateralStatus(queue.CollateralProcessQueue1Up, 2, string.Empty);
                                        _reportWindowsService.AddLogEntry(1, objCollProcGovM.ProcessGovernance1up, 1, "status updated to In progress for report=" + queue.TemplateReportID + " and product id=" + queue.ProductID);
                                        tmg.equinox.applicationservices.viewmodels.FolderVersion.FormInstanceViewModel anchorviewmodel = null;
                                        Library.WriteErrorLog(queue.FormDesignID.ToString() + "__" + queue.FormDesignVersionID.ToString() + "__" + queue.FormInstanceID.ToString());
                                        try
                                        {
                                            LoginViewModel userDetails = _accountService.FindUser(1, queue.CreatedBy);
                                            anchorviewmodel = _reportWindowsService.GetAnchorFromView(queue.FormInstanceID ?? 0);
                                            FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(1, anchorviewmodel.FormDesignVersionID, _formDesignService);
                                            FormDesignVersionDetail detail = formDesignVersionMgr.GetFormDesignVersion(true);
                                            FormInstanceDataManager formInstanceDataManager = new FormInstanceDataManager(1, userDetails.UserId, _formInstanceDataService, queue.CreatedBy, _folderVersionService);
                                            FormInstanceDataProcessor dataProcessor = new FormInstanceDataProcessor(_folderVersionService, anchorviewmodel.FormInstanceID,
                                                                                                                    queue.FolderVersionID ?? 0, queue.FormDesignVersionID ?? 0,
                                                                                                                    false,
                                                                                                                    userDetails.UserId, formInstanceDataManager, _formInstanceDataService,
                                                                                                                    _uiElementService, detail,
                                                                                                                    queue.CreatedBy, _formDesignService, _masterListService, _formInstanceService);

                                            int _sbFormDesignVersionID;
                                            isSBDesign = _reportWindowsService.HasSBDesignTemplate(Convert.ToInt32(queue.TemplateReportID), Convert.ToInt32(queue.TemplateReportVersionID), out _sbFormDesignVersionID);
                                            isEOCDesign = _reportWindowsService.HasEOCCombinedDesignTemplate(Convert.ToInt32(queue.TemplateReportID), Convert.ToInt32(queue.TemplateReportVersionID), out _sbFormDesignVersionID);
                                            var multipleAnchors = _reportWindowsService.HasMultilpeAnchorDocument(queue.CollateralProcessQueue1Up);
                                            //set number of documents for SB in thread
                                            int sbCount = multipleAnchors.Count + 1;
                                            System.Threading.Thread.SetData(System.Threading.Thread.GetNamedDataSlot("SBCOUNT"), sbCount);
                                            dataProcessor.RunViewProcessorsOnCollateralGeneration();
                                            formInstanceDataManager.SaveTargetSectionsData(anchorviewmodel.FormInstanceID, _folderVersionService, _formDesignService);

                                            FormInstanceSectionDataCacheHandler secCacheHandler = new FormInstanceSectionDataCacheHandler();
                                            secCacheHandler.RemoveAllEntries();

                                            if (isEOCDesign == true)
                                            {
                                                RunMultipleEOC(queue, detail, userDetails);
                                            }

                                            if (isSBDesign == true)
                                            {
                                                foreach (var anc in multipleAnchors)
                                                {
                                                    int formInstanceID = Convert.ToInt32(anc.FormInstanceID);
                                                    anchorviewmodel = _reportWindowsService.GetAnchorFromView(formInstanceID);
                                                    formDesignVersionMgr = new FormDesignVersionManager(1, anchorviewmodel.FormDesignVersionID, _formDesignService);
                                                    detail = formDesignVersionMgr.GetFormDesignVersion(true);
                                                    formInstanceDataManager = new FormInstanceDataManager(1, userDetails.UserId, _formInstanceDataService, queue.CreatedBy, _folderVersionService);
                                                    dataProcessor = new FormInstanceDataProcessor(_folderVersionService, anchorviewmodel.FormInstanceID,
                                                                                                                            anc.FolderVersionID, anc.FormDesignVersionID,
                                                                                                                            false,
                                                                                                                            userDetails.UserId, formInstanceDataManager, _formInstanceDataService,
                                                                                                                            _uiElementService, detail,
                                                                                                                            queue.CreatedBy, _formDesignService, _masterListService, _formInstanceService);


                                                    dataProcessor.RunViewProcessorsOnCollateralGeneration();
                                                    formInstanceDataManager.SaveTargetSectionsData(anchorviewmodel.FormInstanceID, _folderVersionService, _formDesignService);

                                                    secCacheHandler = new FormInstanceSectionDataCacheHandler();
                                                    secCacheHandler.RemoveAllEntries();
                                                }
                                            }

                                            //if (detail != null)
                                            //{
                                            //    foreach (SectionDesign secdesign in detail.Sections)
                                            //    {
                                            //        secCacheHandler.RemoveSectionData(anchorviewmodel.FormInstanceID, secdesign.FullName, userDetails.UserId);
                                            //    }
                                            //}
                                            //secCacheHandler.RevomeSectionListFromCache(anchorviewmodel.FormInstanceID, userDetails.UserId);
                                            //secCacheHandler.RemoveTargetFormInstanceFromCache(anchorviewmodel.FormInstanceID, userDetails.UserId);

                                        }
                                        catch (Exception ex)
                                        {
                                            string error = "Error Message =" + ex.Message + "/n StackFlow =" + ex.StackTrace;
                                            _reportWindowsService.UpdateCollateralStatus(queue.CollateralProcessQueue1Up, 3, error);
                                            _reportWindowsService.AddLogEntry(1, objCollProcGovM.ProcessGovernance1up, 1, "Error occured for report=" + queue.TemplateReportID + " and product id=" + queue.ProductID + "/n " + error);
                                        }
                                        List<string> jsonlist = null;

                                        if (isEOCDesign == true)
                                        {
                                            MergeEOCJSON(queue, ref jsonlist);
                                        }
                                        else if (isEOCDesign = false && isSBDesign == false)
                                        {
                                            jsonlist = _reportWindowsService.GetDataAndDesignJSON(queue.FormDesignID, queue.FormDesignVersionID, queue.FormInstanceID);
                                        }
                                        else
                                        {
                                            Dictionary<int, List<string>> anchorJSONList = new Dictionary<int, List<string>>();
                                            List<string> primaryJSONList = _reportWindowsService.GetDataAndDesignJSON(queue.FormDesignID, queue.FormDesignVersionID, queue.FormInstanceID);
                                            anchorJSONList.Add(queue.FormInstanceID.Value, primaryJSONList);
                                            var multipleAnchors = _reportWindowsService.HasMultilpeAnchorDocument(queue.CollateralProcessQueue1Up);
                                            List<string> anchorList = null;
                                            List<string> otherJSONs = new List<string>();
                                            foreach (var anc in multipleAnchors)
                                            {
                                                anchorList = _reportWindowsService.GetDataAndDesignJSON(anc.FormDesignID, anc.FormDesignVersionID, anc.FormInstanceID);
                                                anchorJSONList.Add(anc.FormInstanceID, anchorList);
                                                otherJSONs.Add(anchorList[0]);
                                            }
                                            //merge JSON
                                            SBJSONCombiner combiner = new SBJSONCombiner(primaryJSONList[0], otherJSONs);
                                            primaryJSONList[0] = combiner.Combine();
                                            jsonlist = primaryJSONList;
                                        }
                                        Library.WriteErrorLog(jsonlist[0].Length.ToString() + "__" + jsonlist[1].Length.ToString());
                                        _reportWindowsService.AddLogEntry(1, objCollProcGovM.ProcessGovernance1up, 1, "json data has been pulled for report=" + queue.TemplateReportID + " and product id=" + queue.ProductID);
                                        _reportWindowsService.AddLogEntry(1, objCollProcGovM.ProcessGovernance1up, 1, "SBDesign data processing started for report =" + queue.TemplateReportID + " and product id=" + queue.ProductID);

                                        GenerateCollateral gn;
                                        string outputpath = string.Empty;
                                        Library.WriteErrorLog(jsonlist.Count.ToString());
                                        if (jsonlist != null && jsonlist.Count == 2)
                                        {
                                            _reportWindowsService.AddLogEntry(1, objCollProcGovM.ProcessGovernance1up, 1, "report generation has been started for report=" + queue.TemplateReportID + " and product id=" + queue.ProductID);
                                            Library.WriteErrorLog(queue.CollateralStorageLocation.ToString() + queue.ProductID);
                                            Library.WriteErrorLog(queue.FilePath.ToString() + queue.ProductID);
                                            gn = new GenerateCollateral(queue.CollateralStorageLocation, jsonlist[0], jsonlist[1], queue.FilePath + queue.ProductID, imagePath, _settingManager, _formInstanceService, _collateralService, true);
                                            gn.FormInstanceDetails = _formInstanceService.GetFormInstanceDetails(queue.FormInstanceID.Value);
                                            outputpath = gn.Generate();
                                            wordFilePath = gn.GenerateWord(outputpath);
                                            pdfFilePath = gn.GeneratePDF(wordFilePath, queue.FormInstanceID.Value, queue.CreatedBy, queue.CollateralProcessQueue1Up, anchorviewmodel.Name);
                                            _reportWindowsService.AddLogEntry(1, objCollProcGovM.ProcessGovernance1up, 1, "report generation has been completed for report=" + queue.TemplateReportID + " and product id=" + queue.ProductID);
                                            Library.WriteErrorLog(outputpath);
                                        }

                                        if (string.IsNullOrEmpty(wordFilePath) && string.IsNullOrEmpty(pdfFilePath.PDF))
                                        {
                                            string error = "output path found empty so marked this as error for report=" + queue.TemplateReportID + " and product id=" + queue.ProductID;
                                            Library.WriteErrorLog(queue.ProcessGovernance1Up.ToString() + "__" + queue.CollateralProcessQueue1Up.ToString());
                                            _reportWindowsService.UpdateCollateralStatus(queue.CollateralProcessQueue1Up, 3, error);
                                            _reportWindowsService.AddLogEntry(1, objCollProcGovM.ProcessGovernance1up, 1, error);
                                        }
                                        else
                                        {
                                            Library.WriteErrorLog(queue.ProcessGovernance1Up.ToString() + "__" + queue.CollateralProcessQueue1Up.ToString() + "__" + outputpath);
                                            _reportWindowsService.UpdateCollateralFilePath(wordFilePath, pdfFilePath, queue.CollateralProcessQueue1Up, 4);
                                            _reportWindowsService.AddLogEntry(1, objCollProcGovM.ProcessGovernance1up, 1, "status updated to complete and output path updated for report=" + queue.TemplateReportID + " and product id=" + queue.ProductID);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        string error = "Error Message =" + ex.Message + "/n StackFlow =" + ex.StackTrace;
                                        _reportWindowsService.UpdateCollateralStatus(queue.CollateralProcessQueue1Up, 3, error);
                                        _reportWindowsService.AddLogEntry(1, objCollProcGovM.ProcessGovernance1up, 1, "Error occured for report=" + queue.TemplateReportID + " and product id=" + queue.ProductID + "/n " + error);
                                    }
                                }
                                _reportWindowsService.UpdateCollateralProcessGovData(objCollProcGovM.ProcessGovernance1up, 4);
                                _reportWindowsService.UnLoadActivity(1, objCollProcGovM.ProcessGovernance1up, "collateral Process completed for Governance = " + objCollProcGovM.ProcessGovernance1up.ToString());
                            }
                            catch (Exception ex)
                            {
                                _reportWindowsService.UpdateCollateralProcessGovData(objCollProcGovM.ProcessGovernance1up, 3);
                                _reportWindowsService.UnLoadActivity(1, objCollProcGovM.ProcessGovernance1up, "collateral Process completed with error for Governance = " + objCollProcGovM.ProcessGovernance1up.ToString()
                                                                                                                + "/n Error Message =" + ex.Message
                                                                                                                + "/n StackFlow =" + ex.StackTrace);

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Library.WriteErrorLog(ex.Message + "/n" + ex.StackTrace);
                    }

                    //_reportWindowsService.AddLogEntry(1, 1, 1, "Fetching collateral Process Governance data is done");
                    _isprocessing = false;
                }
            }
        }

        protected override void OnStop()
        {
            //scheduleService.PBPImportSchedular.Dispose();
            Library.WriteErrorLog("Test window service stopped");
        }

        private void TimerForDeleteCollateralServcie_Elapsed(object sender, ElapsedEventArgs e)
        {
            Library.WriteErrorLog(_isDeleteCollateraProcessing.ToString());
            if (!_isDeleteCollateraProcessing)
            {
                _isDeleteCollateraProcessing = true;
                Library.WriteErrorLog("Deleteing collateral operation is started");
                try
                {
                    _reportWindowsService.DeleteSevenDaysOldCollateral(deleteCollaterNoOfDays);
                }
                catch (Exception ex)
                {
                    _isDeleteCollateraProcessing = false;
                    Library.WriteErrorLog("Deleteing collateral operation Error : " + ex.Message + " / n" + ex.StackTrace);
                    string error = "Error Message =" + ex.Message + "/n StackFlow =" + ex.StackTrace;
                    _reportWindowsService.AddLogEntry(1, 0, 1, "Error occured for Deleteing collateral operation : " + error);
                }
                Library.WriteErrorLog("Deleteing collateral operation is completed");
                _isDeleteCollateraProcessing = false;
            }
            // 2. If tick for the first time, reset next run to every 24 hours
            if (this.timerForDeleteCollateralServcie.Interval != 24 * 60 * 60 * 1000)
            {
                this.timerForDeleteCollateralServcie.Interval = 24 * 60 * 60 * 1000;
            }
        }

        private void ExportPreProcessor()
        {
            ExportPreQueueViewModel item = _exportPreQueueService.GetExportPreQueueList();
            if (item != null)
            {
                _isPreExportProcessing = true;
                PBPExportPreProcessor exportPreProcessor = new PBPExportPreProcessor(item.PBPDatabase1Up, item.ExportId, Convert.ToInt32(item.UserId), item.AddedBy, _pbpExportServices, _formDesignService, _folderVersionService, _formInstanceDataService, _uiElementService, _formInstanceService, _reportingDataService, _masterListService, _exportPreQueueService, item);
                exportPreProcessor.ProcessRulesAndSaveSectionsInWindowsService();
                _exportPreQueueService.UpdateStatus(item.ExportPreQueue1Up, ExportPreStatus.PreQueueSuccess);
                _isPreExportProcessing = false;
            }
        }

        private void RunMultipleEOC(CollateralProcessQueueViewModel queue, FormDesignVersionDetail detail, LoginViewModel userDetails)
        {
            var multipleAnchors = _reportWindowsService.HasMultilpeAnchorDocument(queue.CollateralProcessQueue1Up);
            foreach (var anc in multipleAnchors)
            {
                int formInstanceID = Convert.ToInt32(anc.FormInstanceID);
                tmg.equinox.applicationservices.viewmodels.FolderVersion.FormInstanceViewModel anchorviewmodel = _reportWindowsService.GetAnchorFromView(formInstanceID);
                FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(1, anchorviewmodel.FormDesignVersionID, _formDesignService);
                detail = formDesignVersionMgr.GetFormDesignVersion(true);
                FormInstanceDataManager formInstanceDataManager = new FormInstanceDataManager(1, userDetails.UserId, _formInstanceDataService, queue.CreatedBy, _folderVersionService);
                FormInstanceDataProcessor dataProcessor = new FormInstanceDataProcessor(_folderVersionService, anchorviewmodel.FormInstanceID,
                                                                                        anc.FolderVersionID, anc.FormDesignVersionID,
                                                                                        false,
                                                                                        userDetails.UserId, formInstanceDataManager, _formInstanceDataService,
                                                                                        _uiElementService, detail,
                                                                                        queue.CreatedBy, _formDesignService, _masterListService, _formInstanceService);


                dataProcessor.RunViewProcessorsOnCollateralGeneration();
                formInstanceDataManager.SaveTargetSectionsData(anchorviewmodel.FormInstanceID, _folderVersionService, _formDesignService);

                FormInstanceSectionDataCacheHandler secCacheHandler = new FormInstanceSectionDataCacheHandler();
                secCacheHandler.RemoveAllEntries();
            }
        }

        private void MergeEOCJSON(CollateralProcessQueueViewModel queue, ref List<string> jsonlist)
        {
            Dictionary<int, List<string>> anchorJSONList = new Dictionary<int, List<string>>();
            List<string> primaryJSONList = _reportWindowsService.GetDataAndDesignJSON(queue.FormDesignID, queue.FormDesignVersionID, queue.FormInstanceID);
            anchorJSONList.Add(queue.FormInstanceID.Value, primaryJSONList);
            var multipleAnchors = _reportWindowsService.HasMultilpeAnchorDocument(queue.CollateralProcessQueue1Up);
            List<string> anchorList = null;
            List<string> otherJSONs = new List<string>();
            foreach (var anc in multipleAnchors)
            {
                anchorList = _reportWindowsService.GetDataAndDesignJSON(anc.FormDesignID, anc.FormDesignVersionID, anc.FormInstanceID);
                anchorJSONList.Add(anc.FormInstanceID, anchorList);
                otherJSONs.Add(anchorList[0]);
            }
            //merge JSON
            EOCJSONCombiner combiner = new EOCJSONCombiner(primaryJSONList[0], otherJSONs);
            primaryJSONList[0] = combiner.Combine();
            jsonlist = primaryJSONList;
        }
    }
}
