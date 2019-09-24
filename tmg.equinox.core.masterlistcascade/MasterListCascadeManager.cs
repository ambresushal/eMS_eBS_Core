using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.applicationservices.viewmodels.masterListCascade;
using tmg.equinox.backgroundjob.Base;
using tmg.equinox.core.masterlistcascade.filter;
using tmg.equinox.ruleinterpreter.model;
using tmg.equinox.ruleinterpreter.rulecompiler;
using tmg.equinox.domain.viewmodels;
using tmg.equinox.expressionbuilder;
using tmg.equinox.forminstanceprocessor.expressionbuilder;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.ruleprocessor;
using tmg.equinox.ruleprocessor.formdesignmanager;
using tmg.equinox.savetoreportingdbmlcascade;

namespace tmg.equinox.core.masterlistcascade
{
    public class MasterListCascadeManager<T> : IMasterListCascadeManager<T> where T : BaseJobInfo
    {

        private IMasterListCascadeService _mlcService;
        private IMasterListService _mlService;
        private IFormInstanceDataServices _fidService;
        private IFormDesignService _formDesignService;
        private IFolderVersionServices _folderVersionService;
        private IFormInstanceService _formInstanceService;
        private IUIElementService _uiElementService;
        private IReportingDBEnqueueService _rptDBSaveService;
        private Dictionary<int,List<MasterListCascadeDocumentRuleViewModel>> _rules;
        private static readonly Object _lock = new Object();
        public MasterListCascadeManager(IMasterListCascadeService mlcService, IMasterListService mlService, IFormInstanceDataServices fidService, IFormDesignService formDesignService, IFolderVersionServices folderVersionService, IFormInstanceService formInstanceService, IUIElementService uiElementService, IReportingDBEnqueueService rptDBSaveService)
        {
            _mlcService = mlcService;
            _mlService = mlService;
            _fidService = fidService;
            _formDesignService = formDesignService;
            _folderVersionService = folderVersionService;
            _formInstanceService = formInstanceService;
            _uiElementService = uiElementService;
            _rptDBSaveService = rptDBSaveService;
            _rules = new Dictionary<int,List<MasterListCascadeDocumentRuleViewModel>> ();
        }
        public bool Execute(T queueItem, int formDesignID, int formDesignVersionID,int folderVersionID, int masterListCascadeBatchID, int userID, string userName)
        {
            int tenantId = 1;
            bool result = false;
            try
            {
                //get details of Master List Cascade to be executed
                List<MasterListCascadeViewModel> mlCascades = _mlcService.GetMasterListCascade(formDesignID, formDesignVersionID);
                if (mlCascades != null)
                {
                    //insert entry to MasterListCascadeBatch
                    _mlcService.UpdateMasterListCascadeBatch(masterListCascadeBatchID, 2, "Master List Cascade is being Processed.");
                    List<DocumentFilterResult> results = new List<DocumentFilterResult>();
                    foreach (var mlCascade in mlCascades)
                    {
                        MasterListVersions mlVers = _mlService.GetMasterListVersions(mlCascade.MasterListDesignVersionID,folderVersionID);
                        ProcessFilter(mlCascade, mlVers,0, ref results);
                    }
                    //group products by Folder Version
                    var folderVersionGroups = results.GroupBy(a => a.FolderVersionID);
                    //for each Folder Version 
                    List<JToken> fldrDetailList = new List<JToken>();
                    foreach (var fvGroup in folderVersionGroups)
                    {
                        List<DocumentFilterResult> filterResults = fvGroup.ToList();
                        try
                        {
                            //---- baseline minor version
                            DocumentFilterResult filterResult = fvGroup.First();
                            ServiceResult baselineResult = _folderVersionService.BaseLineFolderForCompareSync(tenantId, 0, filterResult.FolderID, filterResult.FolderVersionID,
                                userID, userName, filterResult.FolderVersionNumber, "Baselined for Master List Cascade", 0, filterResult.EffectiveDate, false, false, false, filterDocumentResults: filterResults);
                            _folderVersionService.UpdateFolderLockStatus(userID, 1, filterResult.FolderID);
                            if (baselineResult.Result == ServiceResultStatus.Success && baselineResult.Items.Count() > 0)
                            {
                                ServiceResultItem item = baselineResult.Items.First();
                                if (item.Messages != null && item.Messages.Count() > 0)
                                {
                                    int newFolderVersionID = 0;
                                    int.TryParse(item.Messages.First(), out newFolderVersionID);
                                    if (newFolderVersionID > 0)
                                    {
                                        results.Clear();
                                        foreach (var mlCascade in mlCascades)
                                        {
                                            MasterListVersions mlVersRep = _mlService.GetMasterListVersions(mlCascade.MasterListDesignVersionID, folderVersionID);
                                            ProcessFilter(mlCascade, mlVersRep, newFolderVersionID, ref results);
                                        }
                                        
                                        //---- for each Product
                                        foreach (var res in results)
                                        {
                                            try
                                            {
                                                //_folderVersionService.UpdateFolderLockStatus(userID, 1, res.FolderID);
                                                //-------- process each Rule
                                                DocumentFilterResult prevRes = filterResults.Find(a => a.DocumentID == res.DocumentID && a.FormDesignID == res.FormDesignID);
                                                var mlCas = mlCascades.Find(a => a.TargetDesignVersionID == res.FormDesignVersionID);
                                                ProcessMasterListCascadeRules(prevRes, res,mlCas,userID,userName);
                                                //-------- trigger Reporting database export
                                                //-------- update status of log
                                                MasterListCascadeBatchDetailViewModel model = GetCascadeBatchDetailModel(masterListCascadeBatchID, prevRes, res);
                                                model.Status = 3;
                                                model.Message = "Document Cascade Completed Successfully";
                                                _mlcService.AddMasterListCascadeBatchDetail(model);
                                                JObject fldrDetail = JObject.Parse("{'FolderId':'','FormInstanceId':'' }");
                                                fldrDetail["FolderId"] = res.FolderID;
                                                fldrDetail["FormInstanceId"] = res.FormInstanceID;
                                                fldrDetailList.Add(fldrDetail);
                                            }
                                            catch (Exception ex)
                                            {
                                                //Document Cascade failed
                                                string message = "";
                                                message = "Document Cascade failed : with Error " + ex.Message + " StackTrace : " + ex.StackTrace;
                                                DocumentFilterResult prevRes = filterResults.Find(a => a.DocumentID == res.DocumentID);
                                                MasterListCascadeBatchDetailViewModel model = GetCascadeBatchDetailModel(masterListCascadeBatchID, prevRes, res);
                                                model.Status = 4;
                                                model.Message = message;
                                                _mlcService.AddMasterListCascadeBatchDetail(model);
                                            }
                                            finally
                                            {
                                               // _folderVersionService.ReleaseFolderLock(userID, 1, res.FolderID);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //Baseline failed - failing the Folder Version Cascade
                                string message = "";
                                ServiceResultItem item = baselineResult.Items.First();
                                if (item.Messages != null && item.Messages.Count() > 0)
                                {
                                    message = item.Messages.First();
                                }
                                message = " Baseline Failed : " + message;
                                MasterListCascadeBatchDetailViewModel model = GetCascadeBatchDetailModel(masterListCascadeBatchID, filterResults.First(), null);
                                model.Status = 4;
                                model.Message = message;
                                _mlcService.AddMasterListCascadeBatchDetail(model);
                                throw new Exception("Baseline failed for Folder Version ID - " + model.PreviousFolderVersionID);
                            }
                        }
                        catch (Exception ex)
                        {
                            //Folder Version Cascade failed
                            string message = "";
                            message = " Baseline Failed : with Error " + ex.Message + " StackTrace : " + ex.StackTrace;
                            MasterListCascadeBatchDetailViewModel model = GetCascadeBatchDetailModel(masterListCascadeBatchID, filterResults.First(), null);
                            model.Status = 4;
                            model.Message = message;
                            _mlcService.AddMasterListCascadeBatchDetail(model);
                            message = " Exception Message : " + ex.Message + " StackTrace : " + ex.StackTrace;
                            int status = 4;
                            _mlcService.UpdateMasterListCascadeBatch(masterListCascadeBatchID, status, message);
                            result = false;
                            return result;
                        }
                        finally
                        {
                            _folderVersionService.ReleaseFolderLock(userID, 1, fvGroup.First().FolderID);
                        }
                    }
                    result = true;
                    _mlcService.UpdateMasterListCascadeBatch(masterListCascadeBatchID, 3, "Master List Cascade processed successfully.");

                    
                    
                }
            }
            catch (Exception ex)
            {
                if (masterListCascadeBatchID == 0)
                {
                    throw ex;
                }
                else
                {
                    string message = " Exception Message : " + ex.Message + " StackTrace : " + ex.StackTrace;
                    int status = 4;
                    _mlcService.UpdateMasterListCascadeBatch(masterListCascadeBatchID, status, message);
                }
            }
            
            return result;
        }


        private void ProcessFilter(MasterListCascadeViewModel mlCascade,MasterListVersions mlVers,int folderVersionID ,ref List<DocumentFilterResult> results)
        {
            //get current Master List data
            JObject mlData = _mlcService.GetMasterListSectionData(mlCascade, mlVers.CurrentFolderVersionID, mlVers.CurrentFormInstanceID);
            //---- filter the Products to be processed
            FilterExpression exp = GetFilterExpression(mlCascade);
            //get list of Products with Folder information
            DocumentFilter filter = new DocumentFilter(_mlcService,_folderVersionService, exp, mlCascade, mlVers, mlVers.CurrentEffectiveDate, folderVersionID);
            results.AddRange(filter.ProcessFilterExpression(mlData));

        }
        private FilterExpression GetFilterExpression(MasterListCascadeViewModel model)
        {
            FilterExpression exp = new FilterExpression();
            string rule = model.FilterExpressionRule;
            MLFilter filter = JsonConvert.DeserializeObject<MLFilter>(rule);
            if(filter != null && filter.Filters.Count > 0)
            {
                exp = filter.Filters[0];
            }
            return exp;
        }

        private bool ProcessMasterListCascadeRules(DocumentFilterResult previousDocument,DocumentFilterResult document,MasterListCascadeViewModel mlCas,int userID, string userName)
        {
            int tenantId = 1;
            int? userId = userID;
            string currentUserName = userName;
            //-------- save Product
            FormInstanceDataManager formInstanceDataManager = new FormInstanceDataManager(tenantId, userId, _fidService, currentUserName, _folderVersionService);
            SourceHandlerDBManager sourceDBManager = new SourceHandlerDBManager(tenantId, _folderVersionService, formInstanceDataManager, _formDesignService, _mlService);
            FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(tenantId,document.FormDesignVersionID, _formDesignService);
            FormDesignVersionDetail detail = formDesignVersionMgr.GetFormDesignVersion(true);
            

            //get rules for Master List Cascade
            List<MasterListCascadeDocumentRuleViewModel> rules = null;
            if (_rules.ContainsKey(mlCas.TargetDesignID))
            {
                rules = _rules[mlCas.TargetDesignID];
            }
            else
            {
                rules = _mlcService.GetRules(mlCas.TargetDesignID, mlCas.TargetDesignVersionID, mlCas.MasterListDesignID, mlCas.MasterListDesignVersionID);
                //TODO : Temporary :
                _rules.Add(mlCas.TargetDesignID, rules);
            }
            //for each rule
            rules = rules.OrderBy(a => a.SequenceNo).ToList();
            foreach (MasterListCascadeDocumentRuleViewModel rule in rules)
            {
                CurrentRequestContext requestContext = new CurrentRequestContext();

                ExpressionRuleTreeProcessor processor = new ExpressionRuleTreeProcessor(document.FormInstanceID, _uiElementService, document.FolderVersionID,
                sourceDBManager, _formDesignService, formInstanceDataManager, detail, _formInstanceService, userId, requestContext);

                int ruleSequenceNumber =  0;
                string targetPath = "";
                try
                {
                    //compile rule
                    var getJson = rule.RuleJSON;
                    Documentrule documentRule = DocumentRuleSerializer.Deserialize(getJson);
                    ruleSequenceNumber = rule.SequenceNo;
                    DocumentRuleCompiler ruleCompiler = new DocumentRuleCompiler(rule.DocumentRuleTypeID, documentRule);
                    CompiledDocumentRule compiledRule = ruleCompiler.CompileRule();
                    targetPath = compiledRule.Target.TargetPath;
                    string sectionName = processor.ProcessCompiledRule(compiledRule);
                }
                catch (Exception ex)
                {
                    string customMsg = "An error occurred while processing Rule Sequence '" + ruleSequenceNumber + "'" + "For Target Path '" + targetPath + "'";
                    Exception customException = new Exception(customMsg, ex);
                    ExceptionPolicyWrapper.HandleException(customException, ExceptionPolicies.ExceptionShielding);
                }
            }
            formInstanceDataManager.SaveSectionsData(document.FormInstanceID, true, _folderVersionService, _formDesignService, detail, "");
            ReportingDBQueueInfo rptSaveQueueInfo = new ReportingDBQueueInfo { };
            rptSaveQueueInfo.FormInstanceId = document.FormInstanceID;
            //rptSaveQueueInfo.FormInstanceId = fldr.for
            _rptDBSaveService.CreateJob(rptSaveQueueInfo);
            //-------- generate activity log
            try
            {
                ActivityLogger logger = new masterlistcascade.ActivityLogger(detail, previousDocument, document, mlCas, _folderVersionService, userID, userName);
                logger.LogChanges();
            }
            catch(Exception ex)
            {
                //TODO: add logging in case activity logging fails - not failing the cascade if activity log update fails
            }
            return true;
        }

        private MasterListCascadeBatchDetailViewModel GetCascadeBatchDetailModel(int masterListCascadeBatchID, DocumentFilterResult previousFormInstance, DocumentFilterResult currentFormInstance)
        {
            MasterListCascadeBatchDetailViewModel batchDetailModel = new MasterListCascadeBatchDetailViewModel();
            batchDetailModel.IsTargetMasterList = false;
            batchDetailModel.MasterListCascadeBatchID = masterListCascadeBatchID;
            if(previousFormInstance != null)
            {
                batchDetailModel.MasterListCascadeID = previousFormInstance.MasterListCascadeID;
                batchDetailModel.PreviousFolderVersionID = previousFormInstance.FolderVersionID;
                batchDetailModel.PreviousFormInstanceID = previousFormInstance.FormInstanceID;
            }
            batchDetailModel.TargetFolderID = previousFormInstance.FolderID;
            if(currentFormInstance != null)
            {
                batchDetailModel.NewFolderVersionID = currentFormInstance.FolderVersionID;
                batchDetailModel.NewFormInstanceID = currentFormInstance.FormInstanceID;
            }
            return batchDetailModel;
        }
    }
}
