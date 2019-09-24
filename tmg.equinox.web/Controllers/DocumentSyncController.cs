using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tmg.equinox.applicationservices;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.comparesync;
using tmg.equinox.applicationservices.viewmodels.CompareSync;
using tmg.equinox.applicationservices.viewmodels.ConsumerAccount;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.documentcomparer;
using tmg.equinox.web.FormDesignManager;

namespace tmg.equinox.web.Controllers
{
    public class DocumentSyncController : AuthenticatedController
    {
        private int tenantID = 1;
        private ISyncDocumentService _syncDocumentService { get; set; }
        private IUIElementService _uiElementService { get; set; }
        private IFormDesignService _formDesign { get; set; }
        private IFolderVersionServices _folderVersion { get; set; }

        public DocumentSyncController(ISyncDocumentService syncDocumentService, IUIElementService uiElementService, IFormDesignService formDesign, IFolderVersionServices folderVersion)
        {
            this._syncDocumentService = syncDocumentService;
            this._uiElementService = uiElementService;
            this._formDesign = formDesign;
            this._folderVersion = folderVersion;
        }

        public ActionResult Index()
        {
            ViewBag.RoleId = RoleID;
            return View();
        }

        public JsonResult DocumentElements(int formDesignVersionID, int macroID, string template)
        {
            var elementList = this._uiElementService.GetDocumentElementList(tenantID, formDesignVersionID);
            string macroJSON = template ?? this._syncDocumentService.GetMacroJSONString(tenantID, macroID);
            if (string.IsNullOrEmpty(macroJSON))
            {
                FormDesignVersionDetail detail = FormDesignVersionDetailReadOnlyObjectCache.GetFormDesignVersionDetail(tenantID, formDesignVersionID, _formDesign);
                macroJSON = detail.GetSyncMacro();
            }

            DocumentMacroSync objMacroSync = new DocumentMacroSync(macroJSON);
            List<string> elements = objMacroSync.GetElementFromMacro();

            foreach (string item in elements)
            {
                var selectedElement = elementList.Where(s => s.UIElementPath == item).FirstOrDefault();
                if (selectedElement != null)
                {
                    selectedElement.isChecked = true;
                }
            }

            return Json(elementList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RepeaterKeys(int formDesignVersionID, int elementID, string elementPath, int macroID, string template)
        {
            var keys = this._uiElementService.GetRepeaterKeyList(tenantID, formDesignVersionID, elementID);

            string macroJSON = template ?? this._syncDocumentService.GetMacroJSONString(tenantID, macroID);
            if (string.IsNullOrEmpty(macroJSON))
            {
                FormDesignVersionDetail detail = FormDesignVersionDetailReadOnlyObjectCache.GetFormDesignVersionDetail(tenantID, formDesignVersionID, _formDesign);
                macroJSON = detail.GetSyncMacro();
            }

            DocumentMacroSync objMacroSync = new DocumentMacroSync(macroJSON);
            List<string> repeaterKeys = objMacroSync.GetRepeaterKey(elementPath);


            keys = (from r in keys
                    join k in repeaterKeys on r.UIElementPath equals k
                    select r).ToList();

            return Json(keys, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ElementChildren(int formDesignVersionID, int elementID, string elementPath, int macroID, string template)
        {
            IEnumerable<DocumentElementBaseModel> fields = null;
            try
            {
                fields = this._uiElementService.GetElementChildrenList(tenantID, formDesignVersionID, elementID);

                string macroJSON = template ?? this._syncDocumentService.GetMacroJSONString(tenantID, macroID);
                if (string.IsNullOrEmpty(macroJSON))
                {
                    FormDesignVersionDetail detail = FormDesignVersionDetailReadOnlyObjectCache.GetFormDesignVersionDetail(tenantID, formDesignVersionID, _formDesign);
                    macroJSON = detail.GetSyncMacro();
                }

                DocumentMacroSync objMacroSync = new DocumentMacroSync(macroJSON);
                List<string> elements = objMacroSync.GetChildElements(elementPath);

                foreach (string e in elements)
                {
                    var field = fields.Where(s => s.UIElementPath == e).FirstOrDefault();
                    if (field != null)
                    {
                        field.isChecked = true;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Json(fields, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SyncMacroList(int formInstanceID, GridPagingRequest request)
        {
            var macros = this._syncDocumentService.GetMacroList(tenantID, formInstanceID, request, CurrentUserName, RoleID);
            return Json(macros, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMacroDefinition(int macroID)
        {
            return Json(_syncDocumentService.GetMacroById(macroID), JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddSyncMacro(int formInstanceID, string MacroName, string Notes, bool IsPublic)
        {
            ServiceResult result = _syncDocumentService.isMacroExist(MacroName);

            if (result.Result == ServiceResultStatus.Failure)
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var formInstance = _folderVersion.GetFormInstance(tenantID, formInstanceID);

                FormDesignVersionDetail detail = FormDesignVersionDetailReadOnlyObjectCache.GetFormDesignVersionDetail(tenantID, formInstance.FormDesignVersionID, _formDesign);

                SyncDocumentMacroViewModel model = new SyncDocumentMacroViewModel()
                {
                    AddedBy = CurrentUserName,
                    AddedDate = DateTime.Now,
                    IsPublic = IsPublic,
                    FormDesignID = formInstance.FormDesignID,
                    MacroJSON = detail.GetSyncMacro(),
                    FormDesignVersionID = formInstance.FormDesignVersionID,
                    MacroName = MacroName,
                    Notes = Notes
                };

                result = _syncDocumentService.InsertMacro(tenantID, formInstanceID, model);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UpdateSyncMacro(int macroID, string macroJSON)
        {
            ServiceResult result = this._syncDocumentService.UpdateMacro(tenantID, macroID, macroJSON);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteMacro(int id)
        {
            ServiceResult result;
            result = _syncDocumentService.DeleteMacro(tenantID, id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CopyMacro(int macroID, string MacroName, string Notes, bool IsPublic)
        {
            ServiceResult result;
            result = _syncDocumentService.CopyMacro(tenantID, macroID, MacroName, Notes, IsPublic);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSelectedRepeater(int formDesignVersionID, int macroID, string template)
        {
            var repeaters = _uiElementService.GetRepeaterUIElement(tenantID, formDesignVersionID);

            string macroJSON = template ?? this._syncDocumentService.GetMacroJSONString(tenantID, macroID);
            if (string.IsNullOrEmpty(macroJSON))
            {
                FormDesignVersionDetail detail = FormDesignVersionDetailReadOnlyObjectCache.GetFormDesignVersionDetail(tenantID, formDesignVersionID,_formDesign);
                macroJSON = detail.GetSyncMacro();
            }

            DocumentMacroSync objMacroSync = new DocumentMacroSync(macroJSON);
            List<SelectedItem> selectedRepeaters = objMacroSync.GetRepeaterList();

            var items = (from r in repeaters
                         join s in selectedRepeaters on r.UIElementPath equals s.Value
                         select new SelectedItem
                         {
                             DisplayText = r.Label,
                             Value = (r.UIElementPath + "#" + r.UIElementID),
                             IsSet = s.IsSet
                         }).ToList();

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRepeaterCriteria(int formInstanceID, string repeaterName, string template)
        {
            DocumentMacroSync objMacroSync = new DocumentMacroSync(template);
            string keyList = JsonConvert.SerializeObject(objMacroSync.GetRepeaterKeywithCriteria(repeaterName));

            string sourceDocumentData = _syncDocumentService.GetSourceDocumentData(formInstanceID);
            DocumentDataHelper helper = new DocumentDataHelper(repeaterName, keyList, sourceDocumentData);
            dynamic dropdownData = helper.GetRepeaterListData();

            return Json(dropdownData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDropdownData(int formInstanceID, string repeaterName, string keyList)
        {
            string sourceDocumentData = _syncDocumentService.GetSourceDocumentData(formInstanceID);
            DocumentDataHelper helper = new DocumentDataHelper(repeaterName, keyList, sourceDocumentData);
            dynamic dropdownData = helper.GetRepeaterListData();
            return Json(dropdownData, JsonRequestBehavior.AllowGet);
        }

        public FileResult CompareReport(int tenantId, string source, string target, string macro)
        {
            //get source row
            string jsonStringSource = HttpUtility.UrlDecode(source);
            DocumentViewModel sourceRow = JsonConvert.DeserializeObject<DocumentViewModel>(jsonStringSource);
            string sourceFormInstance = _folderVersion.GetFormInstanceDataCompressed(tenantId, sourceRow.FormInstanceID);
            //get target row
            string jsonStringTarget = HttpUtility.UrlDecode(target);
            DocumentViewModel targetRow = JsonConvert.DeserializeObject<DocumentViewModel>(jsonStringTarget);
            string targetFormInstance = _folderVersion.GetFormInstanceDataCompressed(tenantId, targetRow.FormInstanceID);

            string matchType = targetRow.CompareFilter;

            //get macro JSON
            string macroJSON = HttpUtility.UrlDecode(macro);

            CompareDocument sourceDocument = new CompareDocument();
            sourceDocument.AccountName = sourceRow.AccountName;
            sourceDocument.DocumentName = sourceRow.FormInstanceName;
            sourceDocument.FolderName = sourceRow.FolderName;
            sourceDocument.FolderVerionNumber = sourceRow.VersionNumber;
            sourceDocument.EffectiveDate = sourceRow.EffectiveDate.ToString();

            CompareDocument targetDocument = new CompareDocument();
            targetDocument.AccountName = targetRow.AccountName;
            targetDocument.DocumentName = targetRow.FormInstanceName;
            targetDocument.FolderName = targetRow.FolderName;
            targetDocument.FolderVerionNumber = targetRow.VersionNumber;
            targetDocument.EffectiveDate = targetRow.EffectiveDate.ToString();

            DocumentComparer comparer = new DocumentComparer(sourceFormInstance, targetFormInstance, sourceDocument, targetDocument, macroJSON, matchType,CompareDocumentSource.GENERATEREPORT);
            DocumentCompareResult result = comparer.Compare();
            CompareExcelReportGenerator generator = new CompareExcelReportGenerator(result);
            byte[] reportBytes = generator.GenerateExcelReport();

            string fileName = "DocumentCompare";
            var fileDownloadName = fileName + ".xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(reportBytes, contentType, fileDownloadName);
        }

        public ActionResult SyncDocuments(int tenantId, string source, string targets, string macro, int macroId)
        {
            string jsonStringSource = HttpUtility.UrlDecode(source);
            DocumentViewModel sourceRow = JsonConvert.DeserializeObject<DocumentViewModel>(jsonStringSource);

            string jsonStringTargets = HttpUtility.UrlDecode(targets);
            List<DocumentViewModel> targetRows = JsonConvert.DeserializeObject<List<DocumentViewModel>>(jsonStringTargets);

            string macroJSON = HttpUtility.UrlDecode(macro);

            string sourceFormInstance = _folderVersion.GetFormInstanceDataCompressed(tenantId, sourceRow.FormInstanceID);
            CompareDocument sourceDocument = new CompareDocument();
            sourceDocument.AccountName = sourceRow.AccountName;
            sourceDocument.DocumentName = sourceRow.FormInstanceName;
            sourceDocument.FolderName = sourceRow.FolderName;
            sourceDocument.FolderVerionNumber = sourceRow.VersionNumber;
            sourceDocument.EffectiveDate = sourceRow.EffectiveDate.ToString();
            string matchType = "Mismatches only";
            sourceDocument.FormDesignID = sourceRow.FormDesignID;

            List<DocumentSyncResultViewModel> syncResults = new List<DocumentSyncResultViewModel>();

            //group by folder version id
            if (targetRows != null && targetRows.Count > 0)
            {
                var groups = targetRows.GroupBy(t => t.FolderVersionID);
                foreach (var group in groups)
                {
                    //SyncGroupLogViewModel groupModel = CreateSyncGroupLogEntry(sourceRow, macroId);
                    //List<SyncDocumentLogViewModel> documentLogModels = new List<SyncDocumentLogViewModel>();
                    DocumentSyncResultViewModel resultModel = new DocumentSyncResultViewModel();
                    List<DocumentCompareResult> results = new List<DocumentCompareResult>();
                    bool canSyncGroup = false;
                   // Dictionary<string, string> targetDocuments = new Dictionary<string, string>();
                    List<CompareDocument> targetDocuments = new List<CompareDocument>();
                    Dictionary<string, DocumentCompareResult> targetResults = new Dictionary<string, DocumentCompareResult>();
                    int? folderVersionId = 0;
                    int? folderId = 0;
                    string versionNumber = "";
                    DateTime? effectiveDate = null;
                    resultModel.FormInstances = new List<DocumentSyncResultRowViewModel>();
                    DocumentViewModel firstModel = group.First();
                    resultModel.AccountName = firstModel.AccountName;
                    resultModel.FolderName = firstModel.FolderName;
                    resultModel.FolderVersionID = firstModel.FolderVersionID.Value;
                    resultModel.VersionNumber = firstModel.VersionNumber;
                    ResourceLockHolder _resourceLockHolder = new ResourceLockHolder();
                    var objFolderLock = _resourceLockHolder.CheckIfDocumentLockedByOtherUser(firstModel.FormInstanceID, CurrentUserId);
                    //  var result = _resourceLockService.IsDocumentLocked(firstModel.FormInstanceID, CurrentUserId, sectionName, formDesignId);
                    // FolderVersionViewModel fvModel = _folderVersion.GetFolderLockStatusForSync(tenantId, firstModel.FolderID, CurrentUserId);
                    syncResults.Add(resultModel);
                    if (objFolderLock != null && objFolderLock.IsLocked == true)
                    {
                        resultModel.IsFolderLock = true;
                        continue;
                    }
                    foreach (DocumentViewModel model in group)
                    {
                        folderVersionId = model.FolderVersionID;
                        folderId = model.FolderID;
                        versionNumber = model.VersionNumber;
                        effectiveDate = model.EffectiveDate;
                        // - compare source and this form instance
                        string targetFormInstance = _folderVersion.GetFormInstanceDataCompressed(tenantId, model.FormInstanceID);
                        CompareDocument targetDocument = new CompareDocument();
                        targetDocument.AccountName = model.AccountName;
                        targetDocument.DocumentName = model.FormInstanceName;
                        targetDocument.FolderName = model.FolderName;
                        targetDocument.FolderVerionNumber = model.VersionNumber;
                        targetDocument.EffectiveDate = model.EffectiveDate.ToString();
                        targetDocument.FormDesignID = sourceDocument.FormDesignID;
                        DocumentComparer comparer = new DocumentComparer(sourceFormInstance, targetFormInstance, sourceDocument, targetDocument, macroJSON, matchType, CompareDocumentSource.SYNCDOCUMENTS);
                        DocumentCompareResult result = comparer.Compare();
                        result.CompareType = model.CompareType;
                        DocumentSyncResultRowViewModel resultRowModel = new DocumentSyncResultRowViewModel();
                        resultRowModel.FormInstanceID = model.FormInstanceID;
                        resultRowModel.FormInstanceName = model.FormInstanceName;
                        if (result.IsMatch == false)
                        {
                            if (result.CompareType == "Full Sync" || (result.CompareType == "Common Sync" && result.CanSync == true))
                            {
                                //targetDocuments.Add(targetDocument.FormDesignID, targetFormInstance);
                                CompareDocument syncDocument = new CompareDocument();
                                syncDocument.DocumentName = model.FormInstanceName;
                                syncDocument.FormDesignID = targetDocument.FormDesignID;
                                syncDocument.FormData = targetFormInstance;
                                targetDocuments.Add(syncDocument);
                                targetResults.Add(model.FormInstanceName, result);
                                canSyncGroup = true;
                            }
                        }
                        else
                        {
                            resultRowModel.SyncStatus = "NoSync";
                        }
                        resultModel.FormInstances.Add(resultRowModel);

                    }
                    if (canSyncGroup == true)
                    {
                        string comment = "Baselined for Sync - using Compare and Sync - ";
                        comment = comment + "Synced from Account : " + sourceDocument.AccountName + " , ";
                        comment = comment + "Folder : " + sourceDocument.FolderName + " , ";
                        comment = comment + "Folder Version Number : " + sourceDocument.FolderVerionNumber + " , ";
                        comment = comment + "Document " + sourceDocument.DocumentName;
                        if (comment.Length > 2000)
                        {
                            comment = comment.Substring(0, 2000);
                        }
                        //baseline folder version
                        ServiceResult result = _folderVersion.BaseLineFolderForCompareSync(tenantId, 0, folderId.Value, folderVersionId.Value, CurrentUserId.Value,
                            CurrentUserName, versionNumber, comment, 0, effectiveDate, isRelease: false, isNotApproved: false, isNewVersion: false);
                        if (result.Items != null && result.Items.Count() > 0)
                        {
                            ServiceResultItem item = result.Items.First();
                            if (item.Messages != null && item.Messages.Length > 0)
                            {
                                resultModel.SyncStatus = "Baselined";
                                int newFolderVersionId;
                                if (int.TryParse(item.Messages[0], out newFolderVersionId) == true)
                                {
                                    List<FormInstanceViewModel> formInstances = _folderVersion.GetFormInstanceList(tenantId, newFolderVersionId, folderId.Value);
                                    //sync form instances that can be synced and save
                                    foreach (var form in formInstances)
                                    {
                                        CompareDocument syncDocument = targetDocuments.Where(a => a.DocumentName == form.FormDesignName && a.FormDesignID == form.FormDesignID).FirstOrDefault();
                                        if (syncDocument != null)
                                        {
                                            //sync form instance
                                            string targetFormInstance = syncDocument.FormData;
                                            DocumentCompareResult compareResult = targetResults[form.FormDesignName];
                                            DocumentSynchronizer synchonizer = new DocumentSynchronizer(sourceFormInstance, targetFormInstance, compareResult);
                                            string synchronizedData = synchonizer.Synchronize();
                                            _folderVersion.SaveFormInstanceDataCompressed(form.FormInstanceID, synchronizedData);
                                            if (resultModel.FormInstances != null && resultModel.FormInstances.Count > 0)
                                            {
                                                var resultFormInstance = from res in resultModel.FormInstances where res.FormInstanceName == form.FormDesignName select res;
                                                if (resultFormInstance != null && resultFormInstance.Count() > 0)
                                                {
                                                    resultFormInstance.First().SyncStatus = "Sync";
                                                    //documentLogModels.Add(CreateSyncDocumentLogEntry(form, folderId.Value, sourceRow.FormInstanceID));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        resultModel.SyncStatus = "NoSync";
                    }
                    //if (documentLogModels != null && documentLogModels.Count > 0) 
                    //{
                    //    int syncGroupLogID = LogSyncGroup(groupModel);
                    //    LogSyncDocuments(documentLogModels, syncGroupLogID);
                    //}
                }
            }
            return Json(syncResults, JsonRequestBehavior.AllowGet);
        }

        private SyncGroupLogViewModel CreateSyncGroupLogEntry(DocumentViewModel source, int macroID) 
        {
            SyncGroupLogViewModel model = new SyncGroupLogViewModel();
            model.FolderID = source.FolderID.Value;
            model.FolderVersionID = source.FolderVersionID.Value;
            model.FolderVersionNumber = source.VersionNumber;
            model.LastUpdatedDate = DateTime.Now;
            model.MacroID = macroID;
            model.Notes = "";
            model.SourceDocumentID = source.FormInstanceID;
            model.SyncBy = CurrentUserName;
            model.SyncDate = DateTime.Now;
            model.UpdatedBy = CurrentUserName;
            model.UpdatedDate = DateTime.Now;
            return model;
        }

        private int LogSyncGroup(SyncGroupLogViewModel model) 
        {
            int syncGroupLogID = 0;
            ServiceResult result = _syncDocumentService.InsertSyncGroupLog(model, CurrentUserName);
            if (result.Items != null && result.Items.Count() > 0)
            {
                var resultItem = result.Items.First();
                if (resultItem.Messages != null && resultItem.Messages.Count() > 0)
                {
                    int.TryParse(resultItem.Messages[0], out syncGroupLogID);
                }
            }
            return syncGroupLogID;
        }


        private SyncDocumentLogViewModel CreateSyncDocumentLogEntry(FormInstanceViewModel formModel, int folderID, int sourceDocumentID) 
        {
            SyncDocumentLogViewModel syncModel = new SyncDocumentLogViewModel();
            syncModel.AddedBy = CurrentUserName;
            syncModel.AddedDate = DateTime.Now;
            syncModel.FolderID = folderID;
            syncModel.FolderVersionID = formModel.FolderVersionID;
            syncModel.LastUpdatedDate = DateTime.Now;
            syncModel.Notes = "";
            syncModel.SourceDocumentID = sourceDocumentID;
            syncModel.SyncCompleted = true;
            syncModel.TargetDocumentID = formModel.FormInstanceID;
            syncModel.UpdatedBy = CurrentUserName;
            syncModel.UpdatedDate = DateTime.Now;
            return syncModel;
        }
        private void LogSyncDocuments(List<SyncDocumentLogViewModel> models, int syncGroupLogID)
        {
            if (syncGroupLogID > 0 && models != null && models.Count > 0)
            {
                foreach (SyncDocumentLogViewModel model in models) 
                {
                    model.SyncGroupLogID = syncGroupLogID;
                }
                _syncDocumentService.InsertSyncDocumentLogs(models);
            }
        }
    }
}
