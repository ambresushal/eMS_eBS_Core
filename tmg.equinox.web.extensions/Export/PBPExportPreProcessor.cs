using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.applicationservices.viewmodels.PBPImport;
using tmg.equinox.dependencyresolution;
using tmg.equinox.domain.entities.Enums;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.web.FormDesignManager;
using tmg.equinox.web.FormInstanceManager;
using tmg.equinox.web.FormInstanceProcessor;
using tmg.equinox.web.Framework.Caching;

namespace tmg.equinox.web.extensions
{
    public class PBPExportPreProcessor
    {
        private IPBPExportServices _pbpExportServices;
        private int _pbpDatabase1Up;
        private int _pbpExportQueueID;
        private int _evQueueID;
        private int _currentUserId;
        private string _currentUserName;
        private IFormDesignService _formDesignService;
        private IFolderVersionServices _folderVersionService;
        private IFormInstanceDataServices _formInstanceDataService;
        private IUIElementService _uiElementService;
        private IFormInstanceService _formInstanceService;
        private IReportingDataService _reportingDataService;
        private IMasterListService _masterListService;
        private IExportPreQueueService _exportPreQueueService;
        private int _forminstanceId;
        private ExportPreQueueViewModel _preQueueModel;

        public PBPExportPreProcessor(int pbpDatabase1Up, int pbpExportQueueID, int currentUserId, string currentUserName, IPBPExportServices pbpExportServices, IFormDesignService formDesignService, IFolderVersionServices folderVersionService, IFormInstanceDataServices formInstanceDataService, IUIElementService uiElementService, IFormInstanceService formInstanceService, IReportingDataService reportingDataService, IMasterListService masterListService, IExportPreQueueService exportPreQueueService, ExportPreQueueViewModel preQueuemodel)
        {
            _pbpExportServices = pbpExportServices;
            _pbpDatabase1Up = pbpDatabase1Up;
            _pbpExportQueueID = pbpExportQueueID;
            _currentUserId = currentUserId;
            _currentUserName = currentUserName;
            _formDesignService = formDesignService;
            _folderVersionService = folderVersionService;
            _formInstanceDataService = formInstanceDataService;
            _uiElementService = uiElementService;
            _formInstanceService = formInstanceService;
            _reportingDataService = reportingDataService;
            _masterListService = masterListService;
            _exportPreQueueService = exportPreQueueService;
            _preQueueModel = preQueuemodel;
        }

        public PBPExportPreProcessor(int pbpEVQueueID, int? currentUserId, string currentUserName, IFormDesignService formDesignService, IFolderVersionServices folderVersionService, IFormInstanceDataServices formInstanceDataService, IUIElementService uiElementService, IFormInstanceService formInstanceService, IReportingDataService reportingDataService, IMasterListService masterListService, int forminstanceId)
        {
            _evQueueID = pbpEVQueueID;
            _currentUserId = currentUserId ?? 0;
            _currentUserName = currentUserName;
            _formDesignService = formDesignService;
            _folderVersionService = folderVersionService;
            _formInstanceDataService = formInstanceDataService;
            _uiElementService = uiElementService;
            _formInstanceService = formInstanceService;
            _reportingDataService = reportingDataService;
            _masterListService = masterListService;
            _forminstanceId = forminstanceId;
        }

        public PBPExportPreProcessor(int? currentUserId, string currentUserName, IFormDesignService formDesignService, IFolderVersionServices folderVersionService, IFormInstanceDataServices formInstanceDataService, IUIElementService uiElementService, IFormInstanceService formInstanceService, IReportingDataService reportingDataService, IMasterListService masterListService, int forminstanceId)
        {
            _currentUserId = currentUserId ?? 0;
            _currentUserName = currentUserName;
            _formDesignService = formDesignService;
            _folderVersionService = folderVersionService;
            _formInstanceDataService = formInstanceDataService;
            _uiElementService = uiElementService;
            _formInstanceService = formInstanceService;
            _reportingDataService = reportingDataService;
            _masterListService = masterListService;
            _forminstanceId = forminstanceId;
        }

        public void ProcessRulesAndSaveSectionsValidation()
        {
            int tenantId = 1;
            applicationservices.viewmodels.FolderVersion.FormInstanceViewModel formInstance = _folderVersionService.GetFormInstance(tenantId, _forminstanceId);

            FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(tenantId, formInstance.FormDesignVersionID, _formDesignService);
            FormDesignVersionDetail detail = formDesignVersionMgr.GetFormDesignVersion(true);
            bool isReleased = _folderVersionService.IsFolderVersionReleased(formInstance.FolderVersionID);
            FormInstanceDataManager formDataInstanceManager = new FormInstanceDataManager(detail.TenantID, _currentUserId, _formInstanceDataService, _currentUserName, _folderVersionService, _reportingDataService, _masterListService);
            IDataProcessor dataProcessor = new FormInstanceDataProcessor(_folderVersionService, formInstance.FormInstanceID, formInstance.FolderVersionID, formInstance.FormDesignVersionID, isReleased, _currentUserId, formDataInstanceManager, _formInstanceDataService, _uiElementService, detail, _currentUserName, _formDesignService, _masterListService, _formInstanceService);
            string sectionName = "";
            foreach (var sec in detail.Sections)
            {
                sectionName = sec.FullName;
                dataProcessor.RunProcessorOnSectionLoad(sectionName, false);
                //dataProcessor.RunProcessorOnSectionLoad(sectionName, false);
                string sectionData = detail.JSONData;//formDataInstanceManager.GetSectionData(formInstance.FormInstanceID, sectionName, false, detail, false, false);//detail.JSONData;
                formDataInstanceManager.SetCacheData(formInstance.FormInstanceID, sectionName, sectionData);
                //dataProcessor.ExecuteProcessorOnPreExport(sectionName, true, sectionData, sectionData);
            }
            //ServiceResult result = formDataInstanceManager.SaveSectionsData(formInstance.FormInstanceID, true, _folderVersionService, _formDesignService, detail, sectionName);
            //ServiceResult result = null;
            //if (result == null || result.Result == ServiceResultStatus.Success)
            //    Task.Run(() => formDataInstanceManager.SaveFormInstanceDataIntoReportingCenterDB(tenantId, _currentUserId, _currentUserName, formInstance.FormInstanceID, formInstance.FolderID));
        }

        public void ProcessRulesAndSaveSections(bool inWindowsService)
        {
            List<applicationservices.viewmodels.PBPImport.FormInstanceViewModel> formInstances = _pbpExportServices.GetFormInstanceID_Name(_pbpDatabase1Up, (int)FormDesignID.PBPView);
            List<applicationservices.viewmodels.PBPImport.FormInstanceViewModel> formInstancesVBID = _pbpExportServices.GetFormInstanceID_Name(_pbpDatabase1Up, (int)FormDesignID.VBIDView);

            List<applicationservices.viewmodels.PBPImport.FormInstanceViewModel> anchorformInstances = _pbpExportServices.GetFormInstanceID_Name(_pbpDatabase1Up);
            _pbpExportServices.PreProccessingLog(_pbpExportQueueID, "Pre Proccesing started for plans : " + formInstances.Count.ToString(), _currentUserName, null);
            int tenantId = 1;
            int cnt = 0;
            FormDesignVersionDetail detail = null;
            FormDesignVersionDetail Anchordetail = null;
            FormDesignVersionDetail VBIDdetail = null;

            foreach (var anchorid in anchorformInstances)
            {
                try
                {
                    if (Anchordetail == null)
                    {
                        FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(tenantId, anchorid.FormDesignVersionID, _formDesignService);
                        Anchordetail = formDesignVersionMgr.GetFormDesignVersion(true);
                        _pbpExportServices.PreProccessingLog(_pbpExportQueueID, " Anchor Design retrived...", _currentUserName, null);
                    }
                    FormInstanceDataManager formDataInstanceManageranchor = new FormInstanceDataManager(Anchordetail.TenantID, _currentUserId, _formInstanceDataService, _currentUserName, _folderVersionService, _reportingDataService, _masterListService);
                    foreach (var sec in Anchordetail.Sections)
                    {
                        try
                        {
                            formDataInstanceManageranchor.RemoveSectionsData(anchorid.FormInstanceID, sec.FullName, _currentUserId);
                        }
                        catch (Exception ex)
                        {
                            _pbpExportServices.PreProccessingLog(_pbpExportQueueID, "Error in Remove cache !!!! " + sec.FullName + " " + anchorid.Name, _currentUserName, ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _pbpExportServices.PreProccessingLog(_pbpExportQueueID, "Error in Remove cache for Anchor!!!!", _currentUserName, ex);
                }
            }
                        
            foreach (var formInstance in formInstances)
            {
                try
                {
                    if (detail == null)
                    {
                        FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(tenantId, formInstance.FormDesignVersionID, _formDesignService);
                        detail = formDesignVersionMgr.GetFormDesignVersion(true);
                        _pbpExportServices.PreProccessingLog(_pbpExportQueueID, "Design retrived...", _currentUserName, null);
                    }
                    bool isReleased = _folderVersionService.IsFolderVersionReleased(formInstance.FolderVersionId);
                    if (!isReleased)
                    {
                        FormInstanceDataManager formDataInstanceManager = new FormInstanceDataManager(detail.TenantID, _currentUserId, _formInstanceDataService, _currentUserName, _folderVersionService, _reportingDataService, _masterListService);
                        IDataProcessor dataProcessor = new FormInstanceDataProcessor(_folderVersionService, formInstance.FormInstanceID, formInstance.FolderVersionId, formInstance.FormDesignVersionID, isReleased, _currentUserId, formDataInstanceManager, _formInstanceDataService, _uiElementService, detail, _currentUserName, _formDesignService, _masterListService, _formInstanceService);
                        string sectionName = "";
                        _pbpExportServices.PreProccessingLog(_pbpExportQueueID, "Proccesing started for : " + formInstance.Name + " ,FolderVersionId:  " + formInstance.FolderVersionId + " , FormInstanceID :" + formInstance.FormInstanceID, _currentUserName, null);
                        dataProcessor.RunProcessorOnSectionLoad(detail.Sections[0].FullName, true);

                        //get the first Section which contians the Status fields
                        Dictionary<string, bool> sectionVisibilities = new Dictionary<string, bool>();

                        foreach (var sec in detail.Sections)
                        {
                            sectionName = sec.FullName;

                            string sectionLabel = sec.Label;
                            sectionLabel = Regex.Replace(sectionLabel, "[^a-zA-Z0-9]", "");
                            sectionLabel = Regex.Match(sectionLabel, "SectionB[0-9]{1,2}|SectionC|SectionD|SectionRx").Value;

                            try
                            {
                                formDataInstanceManager.RemoveSectionsData(formInstance.FormInstanceID, sectionName, _currentUserId);
                            }
                            catch (Exception ex)
                            {
                                _pbpExportServices.PreProccessingLog(_pbpExportQueueID, "Error in Remove cache !!!! " + sectionName + " " + formInstance.Name, _currentUserName, ex);
                            }
                            dataProcessor.RunProcessorOnSectionLoad(sectionName, false);
                            //dataProcessor.RunSectionVisibleRule(sectionName);

                            string sectionData = detail.JSONData;//formDataInstanceManager.GetSectionData(formInstance.FormInstanceID, sectionName, false, detail, false, false);
                            formDataInstanceManager.SetCacheData(formInstance.FormInstanceID, sectionName, sectionData);
                            //dataProcessor.RunProcessorOnSectionSave(sectionName, false, sectionData, sectionData);

                            bool ruleResult = dataProcessor.RunSectionVisibleRule(sectionName);
                            if (!string.IsNullOrEmpty(sectionLabel))
                            {
                                if (ruleResult == true)
                                {
                                    if (sectionVisibilities.ContainsKey(sectionLabel) == true)
                                    {
                                        var sectionValue = sectionVisibilities.Where(s => s.Key == sectionLabel).Select(s => s.Value).FirstOrDefault();
                                        if (sectionValue == false)
                                        {
                                            sectionVisibilities.Remove(sectionLabel);
                                            sectionVisibilities.Add(sectionLabel, true);
                                        }
                                    }
                                    else
                                        sectionVisibilities.Add(sectionLabel, true);
                                }
                                else
                                {
                                    if (sectionVisibilities.ContainsKey(sectionLabel) == false)
                                        sectionVisibilities.Add(sectionLabel, false);
                                }
                            }
                        }

                        FormInstanceDataManager formInstanceDataManager = new FormInstanceDataManager(tenantId, _currentUserId, _formInstanceDataService, _currentUserName, _folderVersionService);
                        string sectionAData = formInstanceDataManager.GetSectionData(formInstance.FormInstanceID, "SectionA", false, detail, false, false);
                        JObject objSectionData = JObject.Parse(sectionAData);
                        string sectionPath = "SectionA.AdditionalFields.STATUSA";
                        JToken tok = objSectionData.SelectToken(sectionPath);
                        if (tok != null) { var prop = tok.Parent as JProperty; prop.Value = "2"; }

                        foreach (var sec in sectionVisibilities)
                        {
                            string currentSectionPath = sectionPath.Replace("STATUSA", sec.Key.ToUpper().Replace("SECTION", "STATUS"));
                            tok = objSectionData.SelectToken(currentSectionPath);
                            if (tok != null)
                            {
                                if (sec.Value == true) { var prop = tok.Parent as JProperty; prop.Value = "2"; }
                                else { var prop = tok.Parent as JProperty; prop.Value = string.Empty; }
                            }
                        }

                        formInstanceDataManager.SetCacheData(formInstance.FormInstanceID, "SectionA", objSectionData.ToString());

                        ServiceResult result = formDataInstanceManager.SaveSectionsData(formInstance.FormInstanceID, false, _folderVersionService, _formDesignService, detail, sectionName);
                        cnt++;
                        _pbpExportServices.PreProccessingLog(_pbpExportQueueID, "JSON Saved for " + formInstance.Name + ", Total completed plans : " + cnt.ToString(), _currentUserName, null);
                          
                    }
                }
                catch (Exception ex)
                {
                    _pbpExportServices.PreProccessingLog(_pbpExportQueueID, "Error !!!!", _currentUserName, ex);
                }
            };

            var hiddenList = _pbpExportServices.GetHiddenSectionList();

            foreach (var formInstance in formInstancesVBID)
            {
                try
                {
                    if (VBIDdetail == null)
                    {
                        FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(tenantId, formInstance.FormDesignVersionID, _formDesignService);
                        VBIDdetail = formDesignVersionMgr.GetFormDesignVersion(true);
                        _pbpExportServices.PreProccessingLog(_pbpExportQueueID, "VBID Design retrived...", _currentUserName, null);
                    }
                    bool isReleased = _folderVersionService.IsFolderVersionReleased(formInstance.FolderVersionId);
                    if (!isReleased)
                    {
                        FormInstanceDataManager formDataInstanceManager = new FormInstanceDataManager(VBIDdetail.TenantID, _currentUserId, _formInstanceDataService, _currentUserName, _folderVersionService, _reportingDataService, _masterListService);
                        IDataProcessor dataProcessor = new FormInstanceDataProcessor(_folderVersionService, formInstance.FormInstanceID, formInstance.FolderVersionId, formInstance.FormDesignVersionID, isReleased, _currentUserId, formDataInstanceManager, _formInstanceDataService, _uiElementService, VBIDdetail, _currentUserName, _formDesignService, _masterListService, _formInstanceService);
                        string sectionName = "";
                        _pbpExportServices.PreProccessingLog(_pbpExportQueueID, "VBID Proccesing started for : " + formInstance.Name + " ,FolderVersionId:  " + formInstance.FolderVersionId + " , FormInstanceID :" + formInstance.FormInstanceID, _currentUserName, null);
                        dataProcessor.RunProcessorOnSectionLoad(VBIDdetail.Sections[0].FullName, true);

                        //get the first Section which contians the Status fields
                        List<string> sectionVisibilities = new List<string>();

                        foreach (var sec in VBIDdetail.Sections)
                        {
                            sectionName = sec.FullName;

                            try
                            {
                                formDataInstanceManager.RemoveSectionsData(formInstance.FormInstanceID, sectionName, _currentUserId);
                            }
                            catch (Exception ex)
                            {
                                _pbpExportServices.PreProccessingLog(_pbpExportQueueID, "VBID Error in Remove cache !!!! " + sectionName + " " + formInstance.Name, _currentUserName, ex);
                            }

                            dataProcessor.RunProcessorOnSectionLoad(sectionName, false);
                            bool ruleResult = dataProcessor.RunSectionVisibleRule(sectionName);

                            if (ruleResult == false)
                                sectionVisibilities.Add(sectionName);

                            string sectionData = VBIDdetail.JSONData;
                            formDataInstanceManager.SetCacheData(formInstance.FormInstanceID, sectionName, sectionData);
                        }

                        FormInstanceDataManager formInstanceDataManager = new FormInstanceDataManager(tenantId, _currentUserId, _formInstanceDataService, _currentUserName, _folderVersionService);
                        string sectionAData = formInstanceDataManager.GetSectionData(formInstance.FormInstanceID, "NineteenAReducedCostSharingforVBIDUFGroup1", false, VBIDdetail, false, false);
                        JObject objSectionData = JObject.Parse(sectionAData);
                        string sectionPath = "NineteenAReducedCostSharingforVBIDUFGroup1";
                        string sectionnames = string.Empty;
                        foreach (string sec in sectionVisibilities)
                            sectionnames = sectionnames + ";" + sec;

                        foreach (string sec in hiddenList)
                            sectionnames = sectionnames + ";" + sec;

                        objSectionData["NineteenAReducedCostSharingforVBIDUFGroup1"]["VisibleSections"] = sectionnames + ";";
                        //objSectionData.Add(".", sectionnames);
                        formInstanceDataManager.SetCacheData(formInstance.FormInstanceID, "NineteenAReducedCostSharingforVBIDUFGroup1", objSectionData.ToString());

                        ServiceResult result = formDataInstanceManager.SaveSectionsData(formInstance.FormInstanceID, false, _folderVersionService, _formDesignService, VBIDdetail, sectionName);
                        cnt++;
                        _pbpExportServices.PreProccessingLog(_pbpExportQueueID, "VBID JSON Saved for " + formInstance.Name + ", Total completed plans : " + cnt.ToString(), _currentUserName, null);

                    }
                }
                catch (Exception ex)
                {
                    _pbpExportServices.PreProccessingLog(_pbpExportQueueID, "VBID Error !!!!", _currentUserName, ex);
                }
            };
            System.Diagnostics.Debug.WriteLine("Completed Preprecessing");
            if(inWindowsService == false)
            {
                _pbpExportServices.UpdateExportQueueStatus(_pbpExportQueueID, ProcessStatusMasterCode.InProgress);
                // _pbpExportServices.ScheduleForPBPExportQueue(_pbpExportQueueID, _currentUserName);
                _pbpExportServices.PreProccessingLog(_pbpExportQueueID, "Queued to Hangfire!", _currentUserName, null);
                
            }
        }

        public void ProcessRulesAndSaveSectionsParallel(bool inWindowsService)
        {
            List<applicationservices.viewmodels.PBPImport.FormInstanceViewModel> formInstances = _pbpExportServices.GetFormInstanceID_Name(_pbpDatabase1Up, (int)FormDesignID.PBPView);
            List<applicationservices.viewmodels.PBPImport.FormInstanceViewModel> anchorformInstances = _pbpExportServices.GetFormInstanceID_Name(_pbpDatabase1Up);
            _pbpExportServices.PreProccessingLog(_pbpExportQueueID, "Pre Processing started for plans : " + formInstances.Count.ToString(), _currentUserName, null);
            int tenantId = 1;
            int cnt = 0;

            ParallelOptions options = new ParallelOptions();
            options.MaxDegreeOfParallelism = 12;
            List<PBPExportLogViewModel>  preProcLogs = new List<PBPExportLogViewModel>();
            Parallel.ForEach(anchorformInstances, anchorid => 
            {
                try
                {
                    FormDesignVersionDetail Anchordetail = null;
                    IFormDesignService formDesignService = UnityConfig.Resolve<IFormDesignService>();
                    IFormInstanceDataServices formInstanceDataService = UnityConfig.Resolve<IFormInstanceDataServices>();
                    IFolderVersionServices folderVersionService = UnityConfig.Resolve<IFolderVersionServices>();
                    IReportingDataService reportingDataService = UnityConfig.Resolve<IReportingDataService>();
                    IMasterListService masterListService = UnityConfig.Resolve<IMasterListService>();
                    if (Anchordetail == null)
                    {
                        FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(tenantId, anchorid.FormDesignVersionID, formDesignService);
                        Anchordetail = formDesignVersionMgr.GetFormDesignVersion(true);

                        preProcLogs.Add(new PBPExportLogViewModel { Exception = null, ExportQueueID = _pbpExportQueueID, Message = " Anchor Design retrieved for " + anchorid.Name , UserName = _currentUserName, LogTime = DateTime.Now});
                        //_pbpExportServices.PreProccessingLog(_pbpExportQueueID, " Anchor Design retrived...", _currentUserName, null);
                    }
                    FormInstanceDataManager formDataInstanceManageranchor = new FormInstanceDataManager(Anchordetail.TenantID, _currentUserId, formInstanceDataService, _currentUserName, folderVersionService, reportingDataService, masterListService);
                    foreach (var sec in Anchordetail.Sections)
                    {
                        try
                        {
                            formDataInstanceManageranchor.RemoveSectionsData(anchorid.FormInstanceID, sec.FullName, _currentUserId);
                        }
                        catch (Exception ex)
                        {
                            preProcLogs.Add(new PBPExportLogViewModel { Exception = ex, ExportQueueID = _pbpExportQueueID, Message = "Error in Remove cache !!!! " + sec.FullName + " " + anchorid.Name, UserName = _currentUserName, LogTime = DateTime.Now });
                            //_pbpExportServices.PreProccessingLog(_pbpExportQueueID, "Error in Remove cache !!!! " + sec.FullName + " " + anchorid.Name, _currentUserName, ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    preProcLogs.Add(new PBPExportLogViewModel { Exception = ex, ExportQueueID = _pbpExportQueueID, Message = "Error in Remove cache for Anchor " + anchorid.Name + " !!!!", UserName = _currentUserName, LogTime = DateTime.Now });
                    //_pbpExportServices.PreProccessingLog(_pbpExportQueueID, "Error in Remove cache for Anchor!!!!", _currentUserName, ex);
                }
            });
            _pbpExportServices.PreProcessingLogs(preProcLogs);
            preProcLogs.Clear();
            Parallel.ForEach(formInstances, options, formInstance =>
            {
                try
                {
                    FormDesignVersionDetail detail = null;
                    FormDesignVersionDetail Anchordetail = null;
                    IFormDesignService formDesignService = UnityConfig.Resolve<IFormDesignService>();
                    IFormInstanceDataServices formInstanceDataService = UnityConfig.Resolve<IFormInstanceDataServices>();
                    IFolderVersionServices folderVersionService = UnityConfig.Resolve<IFolderVersionServices>();
                    IReportingDataService reportingDataService = UnityConfig.Resolve<IReportingDataService>();
                    IMasterListService masterListService = UnityConfig.Resolve<IMasterListService>();
                    IFormInstanceService formInstanceService = UnityConfig.Resolve<IFormInstanceService>();
                    IUIElementService uiElementService = UnityConfig.Resolve<IUIElementService>();

                    if (detail == null)
                    {
                        FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(tenantId, formInstance.FormDesignVersionID, formDesignService);
                        detail = formDesignVersionMgr.GetFormDesignVersion(true);
                        preProcLogs.Add(new PBPExportLogViewModel { Exception = null, ExportQueueID = _pbpExportQueueID, Message = "Design retrieved for " + formInstance.Name, UserName = _currentUserName, LogTime = DateTime.Now });
                        //_pbpExportServices.PreProccessingLog(_pbpExportQueueID, "Design retrieved for " + formInstance.FormInstanceID , _currentUserName, null);
                    }
                    bool isReleased = folderVersionService.IsFolderVersionReleased(formInstance.FolderVersionId);
                    if (!isReleased)
                    {
                        FormInstanceDataManager formDataInstanceManager = new FormInstanceDataManager(detail.TenantID, _currentUserId, formInstanceDataService, _currentUserName, folderVersionService, reportingDataService, masterListService);
                        IDataProcessor dataProcessor = new FormInstanceDataProcessor(folderVersionService, formInstance.FormInstanceID, formInstance.FolderVersionId, formInstance.FormDesignVersionID, isReleased, _currentUserId, formDataInstanceManager, formInstanceDataService, uiElementService, detail, _currentUserName, formDesignService, masterListService, formInstanceService);
                        string sectionName = "";
                        preProcLogs.Add(new PBPExportLogViewModel { Exception = null, ExportQueueID = _pbpExportQueueID, Message = "Processing started for : " + formInstance.Name + " ,FolderVersionId:  " + formInstance.FolderVersionId + " , FormInstanceID :" + formInstance.FormInstanceID, UserName = _currentUserName, LogTime = DateTime.Now });
                        //_pbpExportServices.PreProccessingLog(_pbpExportQueueID, "Proccesing started for : " + formInstance.Name + " ,FolderVersionId:  " + formInstance.FolderVersionId + " , FormInstanceID :" + formInstance.FormInstanceID, _currentUserName, null);
                        dataProcessor.RunProcessorOnSectionLoad(detail.Sections[0].FullName, true);

                        //get the first Section which contians the Status fields
                        Dictionary<string, bool> sectionVisibilities = new Dictionary<string, bool>();

                        foreach (var sec in detail.Sections)
                        {
                            sectionName = sec.FullName;

                            string sectionLabel = sec.Label;
                            sectionLabel = Regex.Replace(sectionLabel, "[^a-zA-Z0-9]", "");
                            sectionLabel = Regex.Match(sectionLabel, "SectionB[0-9]{1,2}|SectionC|SectionD|SectionRx").Value;

                            try
                            {
                                formDataInstanceManager.RemoveSectionsData(formInstance.FormInstanceID, sectionName, _currentUserId);
                            }
                            catch (Exception ex)
                            {
                                preProcLogs.Add(new PBPExportLogViewModel { Exception = ex, ExportQueueID = _pbpExportQueueID, Message = "Error in remove cache !!!! " + sectionName + formInstance.Name, UserName = _currentUserName, LogTime = DateTime.Now });
                                //_pbpExportServices.PreProccessingLog(_pbpExportQueueID, "Error in Remove cache !!!! " + sectionName + " " + formInstance.Name, _currentUserName, ex);
                            }
                            dataProcessor.RunProcessorOnSectionLoad(sectionName, false);
                            //dataProcessor.RunSectionVisibleRule(sectionName);

                            string sectionData = detail.JSONData;//formDataInstanceManager.GetSectionData(formInstance.FormInstanceID, sectionName, false, detail, false, false);
                            formDataInstanceManager.SetCacheData(formInstance.FormInstanceID, sectionName, sectionData);
                            //dataProcessor.RunProcessorOnSectionSave(sectionName, false, sectionData, sectionData);

                            bool ruleResult = dataProcessor.RunSectionVisibleRule(sectionName);
                            if (!string.IsNullOrEmpty(sectionLabel))
                            {
                                if (ruleResult == true)
                                {
                                    if (sectionVisibilities.ContainsKey(sectionLabel) == true)
                                    {
                                        var sectionValue = sectionVisibilities.Where(s => s.Key == sectionLabel).Select(s => s.Value).FirstOrDefault();
                                        if (sectionValue == false)
                                        {
                                            sectionVisibilities.Remove(sectionLabel);
                                            sectionVisibilities.Add(sectionLabel, true);
                                        }
                                    }
                                    else
                                        sectionVisibilities.Add(sectionLabel, true);
                                }
                                else
                                {
                                    if (sectionVisibilities.ContainsKey(sectionLabel) == false)
                                        sectionVisibilities.Add(sectionLabel, false);
                                }
                            }
                        }

                        FormInstanceDataManager formInstanceDataManager = new FormInstanceDataManager(tenantId, _currentUserId, formInstanceDataService, _currentUserName, folderVersionService);
                        string sectionAData = formInstanceDataManager.GetSectionData(formInstance.FormInstanceID, "SectionA", detail, false);
                        JObject objSectionData = JObject.Parse(sectionAData);
                        string sectionPath = "SectionA.AdditionalFields.STATUSA";
                        JToken tok = objSectionData.SelectToken(sectionPath);
                        if (tok != null) { var prop = tok.Parent as JProperty; prop.Value = "2"; }

                        foreach (var sec in sectionVisibilities)
                        {
                            string currentSectionPath = sectionPath.Replace("STATUSA", sec.Key.ToUpper().Replace("SECTION", "STATUS"));
                            tok = objSectionData.SelectToken(currentSectionPath);
                            if (tok != null)
                            {
                                if (sec.Value == true) { var prop = tok.Parent as JProperty; prop.Value = "2"; }
                                else { var prop = tok.Parent as JProperty; prop.Value = string.Empty; }
                            }
                        }

                        formInstanceDataManager.SetCacheData(formInstance.FormInstanceID, "SectionA", objSectionData.ToString());
                        ServiceResult result = formDataInstanceManager.SaveSectionsData(formInstance.FormInstanceID, false, folderVersionService, formDesignService, detail, sectionName);
                        cnt++;
                        preProcLogs.Add(new PBPExportLogViewModel { Exception = null, ExportQueueID = _pbpExportQueueID, Message = "JSON Saved for " + formInstance.Name, UserName = _currentUserName, LogTime = DateTime.Now });
                        //_pbpExportServices.PreProccessingLog(_pbpExportQueueID, "JSON Saved for " + formInstance.Name + ", Total completed plans : " + cnt.ToString(), _currentUserName, null);

                    }
                }
                catch (Exception ex)
                {
                    preProcLogs.Add(new PBPExportLogViewModel { Exception = ex, ExportQueueID = _pbpExportQueueID, Message = "Error for " + formInstance.Name + " !!!!", UserName = _currentUserName, LogTime = DateTime.Now });
                    //_pbpExportServices.PreProccessingLog(_pbpExportQueueID, "Error !!!!", _currentUserName, ex);
                }
            });
            System.Diagnostics.Debug.WriteLine("Completed Preprocessing");
            if (inWindowsService == false)
            {
                _pbpExportServices.UpdateExportQueueStatus(_pbpExportQueueID, ProcessStatusMasterCode.InProgress);
                // _pbpExportServices.ScheduleForPBPExportQueue(_pbpExportQueueID, _currentUserName);
                preProcLogs.Add(new PBPExportLogViewModel { Exception = null, ExportQueueID = _pbpExportQueueID, Message = "Queued to Hangfire!", UserName = _currentUserName, LogTime = DateTime.Now });
                //_pbpExportServices.PreProccessingLog(_pbpExportQueueID, "Queued to Hangfire!", _currentUserName, null);

            }
            _pbpExportServices.PreProcessingLogs(preProcLogs);
        }

        public void ProcessRulesAndSaveSectionsInWindowsService()
        {
            _exportPreQueueService.UpdateStatus(_preQueueModel.ExportPreQueue1Up, ExportPreStatus.PreQueueInProcess);
            List<applicationservices.viewmodels.PBPImport.FormInstanceViewModel> formInstances = _pbpExportServices.GetFormInstanceID_Name(_pbpDatabase1Up, (int)FormDesignID.PBPView);
            int tenantId = 1;
            int cnt = 0;
            FormDesignVersionDetail detail = null;

            List<ExportPreQueueLog> forinstanceIdList = (from inst in formInstances
                                                         select new ExportPreQueueLog
                                                         {
                                                             FromInstanceId = inst.FormInstanceID,
                                                             ExportPreQueueId = _preQueueModel.ExportPreQueue1Up,
                                                             Status = ExportPreStatus.InQueued,
                                                             ViewType = "PBPView"
                                                         }
                                                         ).ToList();
            _exportPreQueueService.SaveFormInstanceForPreQueue(forinstanceIdList);
            foreach (var formInstance in formInstances)
            {
                _exportPreQueueService.UpdatePreQueueLog(formInstance.FormInstanceID, _preQueueModel.ExportPreQueue1Up, ExportPreStatus.PreQueueInProcess, null);
                try
                {
                    if (detail == null)
                    {
                        FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(tenantId, formInstance.FormDesignVersionID, _formDesignService);
                        detail = formDesignVersionMgr.GetFormDesignVersion(true);
                    }
                    bool isReleased = _folderVersionService.IsFolderVersionReleased(formInstance.FolderVersionId);
                    if (!isReleased)
                    {
                        FormInstanceDataManager formDataInstanceManager = new FormInstanceDataManager(detail.TenantID, _currentUserId, _formInstanceDataService, _currentUserName, _folderVersionService, _reportingDataService, _masterListService);
                        IDataProcessor dataProcessor = new FormInstanceDataProcessor(_folderVersionService, formInstance.FormInstanceID, formInstance.FolderVersionId, formInstance.FormDesignVersionID, isReleased, _currentUserId, formDataInstanceManager, _formInstanceDataService, _uiElementService, detail, _currentUserName, _formDesignService, _masterListService, _formInstanceService);
                        string sectionName = "";
                        foreach (var sec in detail.Sections)
                        {
                            sectionName = sec.FullName;
                            try
                            {
                                dataProcessor.RunProcessorOnSectionLoad(sectionName, false);
                                string sectionData = detail.JSONData;//formDataInstanceManager.GetSectionData(formInstance.FormInstanceID, sectionName, false, detail, false, false);
                                formDataInstanceManager.SetCacheData(formInstance.FormInstanceID, sectionName, sectionData);                                

                            }
                            catch (Exception ex)
                            {
                                _exportPreQueueService.UpdatePreQueueLog(formInstance.FormInstanceID, _preQueueModel.ExportPreQueue1Up, ExportPreStatus.PreQueueFailed, ex);
                            }
                        }
                        ServiceResult result = formDataInstanceManager.SaveSectionsData(formInstance.FormInstanceID, false, _folderVersionService, _formDesignService, detail, sectionName);
                        try
                        {
                            foreach (var item in detail.Sections)
                            {
                                formDataInstanceManager.RemoveSectionsData(formInstance.FormInstanceID, item.FullName, _currentUserId);
                            }
                        }
                        catch (Exception ex)
                        {
                            _exportPreQueueService.UpdatePreQueueLog(formInstance.FormInstanceID, _preQueueModel.ExportPreQueue1Up, ExportPreStatus.PreQueueFailed, ex);
                        }
                        cnt++;
                    }
                }
                catch (Exception ex)
                {
                    _exportPreQueueService.UpdatePreQueueLog(formInstance.FormInstanceID, _preQueueModel.ExportPreQueue1Up, ExportPreStatus.PreQueueFailed, ex);
                }
                _exportPreQueueService.UpdatePreQueueLog(formInstance.FormInstanceID, _preQueueModel.ExportPreQueue1Up, ExportPreStatus.PreQueueSuccess, null);
            };
            _pbpExportServices.ScheduleForPBPExportQueue(_pbpExportQueueID, _currentUserName);
        }

        //private void Writer(string msg)
        //{
        //    using (StreamWriter w = File.AppendText("D:\\US-3638\\Release\\LogFile.txt"))
        //    {
        //        w.WriteLine(msg);
        //    }



        //}
        public void ExitValidateProcessRulesAndSaveSections(applicationservices.viewmodels.PBPImport.FormInstanceViewModel formInstance)
        {
            int tenantId = 1;
            int cnt = 0;
            FormDesignVersionDetail detail = null;
            try
            {
                if (detail == null)
                {
                    FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(tenantId, formInstance.FormDesignVersionID, _formDesignService);
                    detail = formDesignVersionMgr.GetFormDesignVersion(true);
                }
                bool isReleased = _folderVersionService.IsFolderVersionReleased(formInstance.FolderVersionId);
                if (!isReleased)
                {
                    FormInstanceDataManager formDataInstanceManager = new FormInstanceDataManager(detail.TenantID, _currentUserId, _formInstanceDataService, _currentUserName, _folderVersionService, _reportingDataService, _masterListService);
                    IDataProcessor dataProcessor = new FormInstanceDataProcessor(_folderVersionService, formInstance.FormInstanceID, formInstance.FolderVersionId, formInstance.FormDesignVersionID, isReleased, _currentUserId, formDataInstanceManager, _formInstanceDataService, _uiElementService, detail, _currentUserName, _formDesignService, _masterListService, _formInstanceService);
                    string sectionName = "";
                    //get the first Section which contians the Status fields
                    Dictionary<string, bool> sectionVisibilities = new Dictionary<string, bool>();
                    
                    foreach (var sec in detail.Sections)
                    {
                        sectionName = sec.FullName;
                        string sectionLabel = sec.Label;
                        sectionLabel = Regex.Replace(sectionLabel, "[^a-zA-Z0-9]", "");
                        sectionLabel = Regex.Match(sectionLabel, "SectionB[0-9]{1,2}|SectionC|SectionD|SectionRx").Value;
                        
                        try
                        {
                            dataProcessor.RunPreExitValidateRules(sectionName, false);
                            //run rule for visibility for each Section
                            bool ruleResult = dataProcessor.RunSectionVisibleRule(sectionName);
                            string sectionData = detail.JSONData;

                            if (!string.IsNullOrEmpty(sectionLabel))
                            {
                                if (ruleResult == true)
                                {
                                    if (sectionVisibilities.ContainsKey(sectionLabel) == true)
                                    {
                                        var sectionValue = sectionVisibilities.Where(s => s.Key == sectionLabel).Select(s => s.Value).FirstOrDefault();
                                        if (sectionValue == false)
                                        {
                                            sectionVisibilities.Remove(sectionLabel);
                                            sectionVisibilities.Add(sectionLabel, true);
                                        }
                                    }
                                    else
                                        sectionVisibilities.Add(sectionLabel, true);
                                }
                                else
                                {
                                    if (sectionVisibilities.ContainsKey(sectionLabel) == false)
                                        sectionVisibilities.Add(sectionLabel, false);
                                }
                            }

                            formDataInstanceManager.SetCacheData(formInstance.FormInstanceID, sectionName, sectionData);
                        }
                        catch (Exception ex)
                        {
                            _exportPreQueueService.UpdatePreQueueLog(formInstance.FormInstanceID, _preQueueModel.ExportPreQueue1Up, ExportPreStatus.PreQueueFailed, ex);
                        }
                    }

                    FormInstanceDataManager formInstanceDataManager = new FormInstanceDataManager(tenantId, _currentUserId, _formInstanceDataService, _currentUserName, _folderVersionService);
                    string sectionAData = formInstanceDataManager.GetSectionData(formInstance.FormInstanceID, "SectionA", detail, false);
                    JObject objSectionData = JObject.Parse(sectionAData);
                    string sectionPath = "SectionA.AdditionalFields.STATUSA";
                    JToken tok = objSectionData.SelectToken(sectionPath);
                    if (tok != null) { var prop = tok.Parent as JProperty; prop.Value = "2"; }

                    foreach (var sec in sectionVisibilities)
                    {
                        string currentSectionPath = sectionPath.Replace("STATUSA", sec.Key.ToUpper().Replace("SECTION", "STATUS"));
                        tok = objSectionData.SelectToken(currentSectionPath);
                        if (tok != null)
                        {
                            if (sec.Value == true) { var prop = tok.Parent as JProperty; prop.Value = "2"; }
                            else { var prop = tok.Parent as JProperty; prop.Value = string.Empty; }
                        }
                    }

                    formInstanceDataManager.SetCacheData(formInstance.FormInstanceID, "SectionA", objSectionData.ToString());

                    ServiceResult result = formDataInstanceManager.SaveSectionsData(formInstance.FormInstanceID, false, _folderVersionService, _formDesignService, detail, sectionName);
                    //try
                    //{
                    //    foreach (var item in detail.Sections)
                    //    {
                    //        formDataInstanceManager.RemoveSectionsData(formInstance.FormInstanceID, item.FullName, _currentUserId);
                    //    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    _exportPreQueueService.UpdatePreQueueLog(formInstance.FormInstanceID, _preQueueModel.ExportPreQueue1Up, ExportPreStatus.PreQueueFailed, ex);
                    //}
                    cnt++;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
