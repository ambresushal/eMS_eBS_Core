using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Security.Claims;
using System.Web.Mvc;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.applicationservices.viewmodels.ConsumerAccount;
using tmg.equinox.identitymanagement;
using tmg.equinox.web.Framework;
using System.Drawing.Printing;
using System.Web.Configuration;
using System.IO;
using TuesPechkin;
using System.Net;
using System.Collections.Specialized;
using tmg.equinox.applicationservices.viewmodels.MasterList;
using Newtonsoft.Json;
using System.Configuration;
using System.Net.Mail;
using tmg.equinox.web.Handler;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.web.FormInstance;
using Newtonsoft.Json.Converters;
using System.Dynamic;
using Newtonsoft.Json.Linq;
using tmg.equinox.applicationservices.viewmodels;
using System.Reflection;
using System.Text;
using Ionic.Zip;
using tmg.equinox.web.Framework.Caching;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.applicationservices.viewmodels.Settings;
using tmg.equinox.backgroundjob;
using tmg.equinox.queueprocess.masterlistcascade;
using tmg.equinox.domain.viewmodels;
using System.Web;
using tmg.equinox.applicationservices.viewmodels.DPF;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.ruleprocessor.formdesignmanager;
using tmg.equinox.ruleprocessor;
using tmg.equinox.web.FormInstanceProcessor;

namespace tmg.equinox.web.Controllers
{
    public class VBIDController : AuthenticatedController
    {
        #region Private Members
        private IFolderVersionServices folderVersionService;
        private IWorkFlowStateServices workflowStateService;
        private IConsumerAccountService _consumerAccountService;
        private IJournalReportService _journalReportService;
        private IFormDesignService _formDesignServices;
        private IFolderLockService _folderLockService;
        private IBackgroundJobManager _hangFireJobManager;
        private IMasterListCascadeEnqueueService _mlCascadeQueueService;
        private IMasterListService _masterListService;
        private IMasterListCascadeService _masterListCascadeService;
        private IResourceLockService _resourceLockService;
        private IPBPExportServices _pbpExportServices;
        private IPlanTaskUserMappingService _planTaskUserMappingService;
        private IUIElementService _uielementService;
        private IPBPImportService _pbpImportService;
        private IReportingDataService _reportingDataService;
        private IFormInstanceDataServices _formInstanceDataServices;
        private IFormInstanceService _formInstanceService;
        private IUIElementService _uiElementService;

        //TODO: Create a class for constants and move all constants string to that class.
        private const string ACCOUNT = "Account";
        private const string MASTERLIST = "MasterList";
        private const string PRODREF = "PRODREF_";
        private string sendGridUserName = string.Empty;
        private string sendGridPassword = string.Empty;
        private bool _inMemoryFolderLock = false;


        #endregion Private Members

        #region Constructor

        public VBIDController(IFolderVersionServices folderVersionService,
            IWorkFlowStateServices workflowStateService, IConsumerAccountService consumerAccountService,
            IJournalReportService journalReportService, IFormDesignService formDesignServices,
            IFolderLockService folderLockService, IBackgroundJobManager hangFireJobManager,
            IMasterListCascadeEnqueueService mlCascadeQueueService, IMasterListService masterListService, IMasterListCascadeService masterListCascadeService, IResourceLockService resourceLockService, IPBPExportServices pbpExportServices,
            IPlanTaskUserMappingService planTaskUserMappingService, IUIElementService uielementService, IPBPImportService pbpImportService, IReportingDataService reportingDataService,
            IFormInstanceDataServices formInstanceDataServices, IFormInstanceService formInstanceService,
            IUIElementService uiElementService)
        {
            this.folderVersionService = folderVersionService;
            this.workflowStateService = workflowStateService;
            this._consumerAccountService = consumerAccountService;
            this._journalReportService = journalReportService;
            this._formDesignServices = formDesignServices;
            this._folderLockService = folderLockService;
            this._hangFireJobManager = hangFireJobManager;
            this._mlCascadeQueueService = mlCascadeQueueService;
            this._masterListService = masterListService;
            this._masterListCascadeService = masterListCascadeService;
            _resourceLockService = resourceLockService;
            this._pbpExportServices = pbpExportServices;
            this._planTaskUserMappingService = planTaskUserMappingService;
            this._uielementService = uielementService;
            this._pbpImportService = pbpImportService;
            this._reportingDataService = reportingDataService;
            this._formInstanceDataServices = formInstanceDataServices;
            this._formInstanceService = formInstanceService;
            this._uiElementService = uiElementService;
            this._inMemoryFolderLock = Convert.ToString(ConfigurationManager.AppSettings["FolderLockToUse"]) == "InMemory" ? true : false;
        }
        #endregion Constructor

        #region Action Methods

        public ActionResult GetVBIDSettings(int anchorId, int folderVersionId)
        {
            int tenantId = 1;
            List<FormInstanceViewModel> instances = folderVersionService.GetFormInstanceListForAnchor(tenantId, folderVersionId, anchorId);

            FormInstanceViewModel modelAnchor = null;
            FormInstanceViewModel modelPBPView = null;
            VBIDFolderVersionValuesViewModel model = new VBIDFolderVersionValuesViewModel();

            //get PBP View JSON
            var formInstPBP = from inst in instances where inst.FormDesignID == 2367 select inst;
            if (formInstPBP != null && formInstPBP.Count() > 0)
            {
                modelPBPView = formInstPBP.First();
            }
            //get Anchor JSON
            var formInstMed = from inst in instances where inst.FormDesignID == 2359 select inst;
            if (formInstMed != null && formInstPBP.Count() > 0)
            {
                modelAnchor = formInstMed.First();
            }
            if (modelAnchor != null)
            {
                FormDesignVersionManager formDesignMgr = new FormDesignVersionManager(tenantId, modelAnchor.FormDesignVersionID, _formDesignServices);
                FormDesignVersionDetail detail = formDesignMgr.GetFormDesignVersion(false);
                FormInstanceDataManager formInstanceDataManager = new FormInstanceDataManager(tenantId, base.CurrentUserId, _formInstanceDataServices, base.CurrentUserName, folderVersionService);
                string sectionData = formInstanceDataManager.GetSectionData(modelAnchor.FormInstanceID, "SECTIONASECTIONA1", detail, false);
                if (!string.IsNullOrEmpty(sectionData))
                {
                    JObject objSectionData = JObject.Parse(sectionData);
                    JToken tok = objSectionData.SelectToken("SECTIONASECTIONA1.DoesyourPlanofferaPrescriptionPartDbenefit");
                    model.DoesyourPlanofferaPrescriptionPartDbenefit = tok.ToString();
                    tok = objSectionData.SelectToken("SECTIONASECTIONA1.IsthisaVBIDPlan");
                    if (tok != null) model.IsthisaVBIDPlan = tok.ToString();
                }
            }

            if (modelPBPView != null)
            {
                FormDesignVersionManager formDesignMgr = new FormDesignVersionManager(tenantId, modelPBPView.FormDesignVersionID, _formDesignServices);
                FormDesignVersionDetail detail = formDesignMgr.GetFormDesignVersion(false);
                FormInstanceDataManager formInstanceDataManager = new FormInstanceDataManager(tenantId, base.CurrentUserId, _formInstanceDataServices, base.CurrentUserName, folderVersionService);
                string sectionData = formInstanceDataManager.GetSectionData(modelPBPView.FormInstanceID, "ValueBasedInsuranceDesignVBIDModelTest", detail, false);
                if (!string.IsNullOrEmpty(sectionData))
                {
                    JObject objSectionData = JObject.Parse(sectionData);
                    JToken tok = objSectionData.SelectToken("ValueBasedInsuranceDesignVBIDModelTest.ReducedCostSharingforVBIDS.DoesyourVBIDbenefitofferPartCreductionsincostoradditionalbenefits");
                    model.DoesyourVBIDbenefitofferPartCreductionsincostoradditionalbenefits = tok.ToString();
                    tok = objSectionData.SelectToken("ValueBasedInsuranceDesignVBIDModelTest.ReducedCostSharingforVBIDS.DoesyourplanincludeMAUniformityFlexibilitywithreductionsincostoradditi");
                    model.DoesyourplanincludeMAUniformityFlexibilitywithreductionsincostoradditi = tok.ToString();
                    tok = objSectionData.SelectToken("ValueBasedInsuranceDesignVBIDModelTest.ReducedCostSharingforVBIDS.ValueBasedInsuranceDesignAttestationIattestthat1thebenefitsenteredcomp");
                    model.ValueBasedInsuranceDesignAttestationIattestthat1thebenefitsenteredcomp = tok.ToString();
                    tok = objSectionData.SelectToken("ValueBasedInsuranceDesignVBIDModelTest.ReducedCostSharingforVBIDS.DoyouofferSpecialSupplementalBenefitsforChronicallyIII");
                    if (tok != null) model.DoyouofferSpecialSupplementalBenefitsforChronicallyIII = tok.ToString();
                    tok = objSectionData.SelectToken("ValueBasedInsuranceDesignVBIDModelTest.ReducedCostSharingforVBIDS.SelectwhattypeofbenefityourSSBCIincludes");
                    if (tok != null) model.SelectwhattypeofbenefityourSSBCIincludes = tok.ToArray().Contains("01") ? "true" : "false";
                    tok = objSectionData.SelectToken("ValueBasedInsuranceDesignVBIDModelTest.ReducedCostSharingforVBIDS.SelectwhattypeofbenefityourSSBCIincludes");
                    if (tok != null) model.SelectwhattypeofbenefityourSSBCIincludesAB = tok.ToArray().Contains("10") ? "true" : "false";
                }
                string sectionDataRx = formInstanceDataManager.GetSectionData(modelPBPView.FormInstanceID, "MedicareRxGeneral", detail, false);
                if (!string.IsNullOrEmpty(sectionDataRx))
                {
                    JObject objSectionData = JObject.Parse(sectionDataRx);
                    JToken tok = objSectionData.SelectToken("MedicareRxGeneral.MedicareRxGeneral3.IndicatenumberofTiersinyourPartDbenefit");
                    model.IndicatenumberofTiersinyourPartDbenefit = tok.ToString();
                }
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public ActionResult GetPackages(int anchorId, int folderVersionId)
        {
            int costSharingCount = 0;
            int additionalBenefitsCount = 0;
            int rxCount = 0;
            GetPackageCounts(anchorId, folderVersionId, ref costSharingCount, ref additionalBenefitsCount, ref rxCount);
            return Json(new { CostSharingCount = costSharingCount, AdditionalBenefitsCount = additionalBenefitsCount, RxCount = rxCount }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ManagePackages(AddVBIDPackageModel model, int anchorId, int folderVersionId)
        {
            int tenantId = 1;
            int anchorFormDesignId = 0;
            int anchorFormInstanceId = 0;
            DateTime? effectiveDate = DateTime.Now;

            //get instance list for Folder Version
            List<FormInstanceViewModel> instances = folderVersionService.GetFormInstanceListForAnchor(tenantId, folderVersionId, anchorId);
            var anchorInst = from inst in instances where inst.FormDesignID == 2359 select inst;
            if (anchorInst != null && anchorInst.Count() > 0)
            {
                anchorFormDesignId = anchorInst.First().FormDesignID;
                anchorFormInstanceId = anchorInst.First().FormInstanceID;
                effectiveDate = anchorInst.First().EffectiveDate;
            }
            //get vbid instance list
            var vbidInstances = from inst in instances where inst.FormDesignID == 2409 select inst;
            var nums = from vbid in vbidInstances select vbid.FormInstanceName.Replace("VBID View ", "");
            //order by name
            List<int> instanceNums = GetOrderedVBIDInstanceNums(vbidInstances.ToList());

            //start processing
            int vbidInstanceCount = instanceNums.Count;
            int requiredCount = GetRequiredPackageCount(vbidInstanceCount, model.ReducedCostSharePackageRequired, model.AdditionalBenefitPackageRequired, model.RxPackageRequired);
            //get form design version ID
            var childFormDesignList = _formDesignServices.GetMappedDesignDocumentList(tenantId, anchorFormDesignId, effectiveDate);
            var childDesigns = from childDes in childFormDesignList where childDes.FormDesignID == 2409 select childDes;
            int formDesignVersionId = 0;
            if (childDesigns != null && childDesigns.Count() > 0)
            {
                formDesignVersionId = childDesigns.First().FormDesignVersionID;
            }
            List<FormInstanceViewModel> models = vbidInstances.ToList();
            if (requiredCount == 0)
            {
                instanceNums = GetOrderedVBIDInstanceNums(models);
                UpdatePackages(tenantId, models, instanceNums, model);
            }
            else if (requiredCount > 0)
            {
                //add instances
                models = AddInstances(tenantId, folderVersionId, anchorFormInstanceId, formDesignVersionId, requiredCount, instanceNums);
                instanceNums = GetOrderedVBIDInstanceNums(models);
                UpdatePackages(tenantId, models, instanceNums, model);
            }
            else if (requiredCount < 0)
            {
                instanceNums = GetOrderedVBIDInstanceNums(models);
                UpdatePackages(tenantId, models, instanceNums, model);
                //delete instances
                models = DeleteInstances(tenantId, folderVersionId, anchorFormInstanceId, requiredCount, instanceNums);
            }
            //process updates
            UpdatePBPVBIDFieldsInternal(anchorId, folderVersionId);
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateForDeletedPackage(int anchorId, int folderVersionId)
        {
            //update the form instance names 
            int tenantId = 1;
            List<FormInstanceViewModel> formInstances = GetVBIDInstances(tenantId, folderVersionId, anchorId);
            List<int> instanceNums = GetOrderedVBIDInstanceNums(formInstances);
            int idx = 1;
            foreach (var ins in instanceNums)
            {
                if (ins > idx)
                {
                    var fIn = from fIns in formInstances where fIns.FormInstanceName == "VBID View " + ins select fIns;
                    if (fIn != null && fIn.Count() > 0)
                    {
                        int newIns = idx;
                        int fInstanceID = fIn.First().FormInstanceID;
                        //rename Form Instance here
                        folderVersionService.UpdateDocumentName(tenantId, fInstanceID, "VBID View " + newIns);
                        SetPackageIDOnDelete(newIns, fIn.First());
                    }
                }
                idx++;
            }
            UpdatePBPVBIDFieldsInternal(anchorId, folderVersionId);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdatePBPVBIDFields(int anchorId, int folderVersionId)
        {
            UpdatePBPVBIDFieldsInternal(anchorId, folderVersionId);
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        private void GetPackageCounts(int anchorId, int folderVersionId, ref int costSharingCount, ref int additionalBenefitsCount, ref int rxCount)
        {
            int tenantId = 1;
            //get existing packages
            List<FormInstanceViewModel> instances = folderVersionService.GetFormInstanceListForAnchor(tenantId, folderVersionId, anchorId);
            //for each VBID View - determine the sections enabled
            var vbidInstances = from inst in instances where inst.FormDesignID == 2409 select inst;

            if (vbidInstances != null && vbidInstances.Count() > 0)
            {
                foreach (var vbid in vbidInstances)
                {
                    //get values from JSON
                    VBIDPackageModel model = new VBIDPackageModel();
                    model.PackageNumber = 1;
                    model.IsCostSharing = false;
                    model.IsAdditionalBenefits = false;
                    model.IsRx = false;
                    FormDesignVersionManager formDesignMgr = new FormDesignVersionManager(tenantId, vbid.FormDesignVersionID, _formDesignServices);
                    FormDesignVersionDetail detail = formDesignMgr.GetFormDesignVersion(false);
                    FormInstanceDataManager formInstanceDataManager = new FormInstanceDataManager(tenantId, base.CurrentUserId, _formInstanceDataServices, base.CurrentUserName, folderVersionService);
                    string sectionData = formInstanceDataManager.GetSectionData(vbid.FormInstanceID, "PackageInformation", detail, false);
                    if (!string.IsNullOrEmpty(sectionData))
                    {
                        JObject objSectionData = JObject.Parse(sectionData);
                        JToken tok = objSectionData.SelectToken("PackageInformation.IsReducedCostSharingApplicable");
                        string tokValue = "";
                        if (tok != null)
                        {
                            tokValue = tok.ToString();
                            if (tokValue == "YES")
                            {
                                model.IsCostSharing = true;
                            }
                        }
                        tok = objSectionData.SelectToken("PackageInformation.IsAdditionalBenefitsApplicable");
                        if (tok != null)
                        {
                            tokValue = tok.ToString();
                            if (tokValue == "YES")
                            {
                                model.IsAdditionalBenefits = true;
                            }
                        }
                        tok = objSectionData.SelectToken("PackageInformation.IsRxApplicable");
                        if (tok != null)
                        {
                            tokValue = tok.ToString();
                            if (tokValue == "YES")
                            {
                                model.IsRx = true;
                            }
                        }
                    }
                    if (model.IsCostSharing == true)
                    {
                        costSharingCount = costSharingCount + 1;
                    }
                    if (model.IsAdditionalBenefits == true)
                    {
                        additionalBenefitsCount = additionalBenefitsCount + 1;
                    }
                    if (model.IsRx == true)
                    {
                        rxCount = rxCount + 1;
                    }
                }
            }
        }

        private int GetRequiredPackageCount(int currentCount, int reducedCostShareCount, int additionalBenefitCount, int rxCount)
        {
            List<int> requiredCounts = new List<int> { reducedCostShareCount, additionalBenefitCount, rxCount };
            int maxRequired = requiredCounts.Max();
            return maxRequired - currentCount;
        }

        private List<FormInstanceViewModel> GetVBIDInstances(int tenantId, int folderVersionId, int anchorId)
        {
            List<FormInstanceViewModel> vbids = new List<FormInstanceViewModel>();
            List<FormInstanceViewModel> instances = folderVersionService.GetFormInstanceListForAnchor(tenantId, folderVersionId, anchorId);
            var vbidInstances = from inst in instances where inst.FormDesignID == 2409 select inst;
            if (vbidInstances != null && vbidInstances.Count() > 0)
            {
                vbids = vbidInstances.ToList();
            }
            return vbids;
        }

        private List<int> GetOrderedVBIDInstanceNums(List<FormInstanceViewModel> vbidInstances)
        {
            var nums = from vbid in vbidInstances select vbid.FormInstanceName.Replace("VBID View ", "");
            List<int> instanceNums = new List<int>();
            int instanceVal;
            foreach (var num in nums)
            {
                int.TryParse(num, out instanceVal);
                if (instanceVal > 0)
                {
                    int res = instanceVal;
                    instanceNums.Add(res);
                }
            }
            instanceNums.Sort();
            return instanceNums;
        }

        private List<FormInstanceViewModel> AddInstances(int tenantId, int folderVersionId, int anchorFormInstanceId, int formDesignVersionId, int requiredCount, List<int> instanceNums)
        {
            int maxNum = 1;
            if(requiredCount < 15)
            {
                if (instanceNums.Count > 0)
                {
                    maxNum = instanceNums.Max() + 1;
                }
                while (requiredCount > 0)
                {
                    int formInstanceID = folderVersionService.AddChildFormInstance(tenantId, folderVersionId, formDesignVersionId, 2409, "VBID View " + maxNum++, CurrentUserName, anchorFormInstanceId);
                    requiredCount--;
                }

            }
            return GetVBIDInstances(tenantId, folderVersionId, anchorFormInstanceId);
        }

        private List<FormInstanceViewModel> DeleteInstances(int tenantId, int folderVersionId, int anchorFormInstanceId, int requiredCount, List<int> instanceNums)
        {
            List<FormInstanceViewModel> vbidinstances = GetVBIDInstances(tenantId, folderVersionId, anchorFormInstanceId);
            //TODO: update Rx Package Count

            int lastIdx = 1;
            while (requiredCount < 0)
            {
                int maxIdx = instanceNums[instanceNums.Count - lastIdx];
                //delete the corresponding package
                string formName = "VBID View " + maxIdx;
                var vbids = from v in vbidinstances where v.FormInstanceName == formName select v;
                var vbid = vbids.First();
                if (vbid != null)
                {
                    folderVersionService.DeleteFormInstance(vbid.FolderID, tenantId, vbid.FolderVersionID, vbid.FormInstanceID, CurrentUserName);
                }
                lastIdx++;
                requiredCount++;
            }
            return GetVBIDInstances(tenantId, folderVersionId, anchorFormInstanceId);
        }

        private void UpdatePackages(int tenantId, List<FormInstanceViewModel> vbidInstances, List<int> instanceNums, AddVBIDPackageModel model)
        {
            int vbidInstanceCount = vbidInstances.Count;
            int reducedCostSharing = model.ReducedCostSharePackageRequired - model.ReducedCostSharePackageCurrent;
            int additionalBenefits = model.AdditionalBenefitPackageRequired - model.AdditionalBenefitPackageCurrent;
            int rxPackage = model.RxPackageRequired - model.RxPackageCurrent;
            if (vbidInstanceCount > 0)
            {
                for (int loopIdx = 0; loopIdx < vbidInstanceCount; loopIdx++)
                {
                    var fiNameIdx = instanceNums[loopIdx];
                    string fName = "VBID View " + fiNameIdx;
                    var vbids = from v in vbidInstances where v.FormInstanceName == fName select v;
                    var vbid = vbids.First();
                    if (vbid != null)
                    {
                        bool isFormInstanceModified = false;
                        FormDesignVersionManager formDesignMgr = new FormDesignVersionManager(tenantId, vbid.FormDesignVersionID, _formDesignServices);
                        FormDesignVersionDetail detail = formDesignMgr.GetFormDesignVersion(false);
                        FormInstanceDataManager formInstanceDataManager = new FormInstanceDataManager(tenantId, base.CurrentUserId, _formInstanceDataServices, base.CurrentUserName, folderVersionService);
                        string sectionData = formInstanceDataManager.GetSectionData(vbid.FormInstanceID, "PackageInformation", false, detail, false, false);
                        if (!string.IsNullOrEmpty(sectionData))
                        {
                            JObject objSectionData = JObject.Parse(sectionData);
                            JToken tok = objSectionData.SelectToken("PackageInformation.IsReducedCostSharingApplicable");
                            string tokValue = "";
                            if (tok != null)
                            {
                                tokValue = tok.ToString();
                                if (tokValue != "YES" && reducedCostSharing > 0)
                                {
                                    var prop = tok.Parent as JProperty;
                                    prop.Value = "YES";
                                    reducedCostSharing = reducedCostSharing - 1;
                                    isFormInstanceModified = true;
                                }
                            }
                            tok = objSectionData.SelectToken("PackageInformation.IsAdditionalBenefitsApplicable");
                            if (tok != null)
                            {
                                tokValue = tok.ToString();
                                if (tokValue != "YES" && additionalBenefits > 0)
                                {
                                    var prop = tok.Parent as JProperty;
                                    prop.Value = "YES";
                                    additionalBenefits = additionalBenefits - 1;
                                    isFormInstanceModified = true;
                                }
                            }
                            tok = objSectionData.SelectToken("PackageInformation.IsRxApplicable");
                            if (tok != null)
                            {
                                tokValue = tok.ToString();
                                if (tokValue != "YES" && rxPackage > 0)
                                {
                                    var prop = tok.Parent as JProperty;
                                    prop.Value = "YES";
                                    rxPackage = rxPackage - 1;
                                    isFormInstanceModified = true;
                                }
                            }
                            if (isFormInstanceModified == true)
                            {
                                formInstanceDataManager.SetCacheData(vbid.FormInstanceID, "PackageInformation", objSectionData.ToString());
                                formInstanceDataManager.SaveSectionsData(vbid.FormInstanceID, false, folderVersionService, _formDesignServices, detail, "PackageInformation");
                            }
                        }
                        //add package ID
                        SetPackageID(fiNameIdx, vbid.FormInstanceID, formInstanceDataManager, detail);
                        if (model.RxPackageCurrent != model.RxPackageRequired)
                        {
                            string sectionDataVBRX = formInstanceDataManager.GetSectionData(vbid.FormInstanceID, "VBIDRX", false, detail, false, false);
                            if (!string.IsNullOrEmpty(sectionDataVBRX))
                            {
                                JObject objSectionData = JObject.Parse(sectionDataVBRX);
                                JToken tok = objSectionData.SelectToken("VBIDRX.VBIDRXGeneral.HowmanypackagesdoesyourPartDVBIDbenefitcontain");
                                if (tok != null)
                                {
                                    var prop = tok.Parent as JProperty;
                                    prop.Value = model.RxPackageRequired.ToString();
                                }
                                tok = objSectionData.SelectToken("VBIDRX.VBIDRXGeneral.DoesyourVBIDbenefitincludePartDreductionsinCost");
                                if (tok != null)
                                {
                                    var prop = tok.Parent as JProperty;
                                    if (model.RxPackageRequired > 0)
                                    {
                                        prop.Value = "YES";
                                    }
                                    else
                                    {
                                        prop.Value = "NO";
                                    }
                                }
                                formInstanceDataManager.SetCacheData(vbid.FormInstanceID, "VBIDRX", objSectionData.ToString());
                                formInstanceDataManager.SaveSectionsData(vbid.FormInstanceID, false, folderVersionService, _formDesignServices, detail, "VBIDRX");
                            }
                        }
                    }
                }
                for (int loopIdx = vbidInstanceCount - 1; loopIdx >= 0; loopIdx--)
                {
                    var fiNameIdx = instanceNums[loopIdx];
                    string fName = "VBID View " + fiNameIdx;
                    var vbids = from v in vbidInstances where v.FormInstanceName == fName select v;
                    var vbid = vbids.First();
                    if (vbid != null)
                    {
                        bool isFormInstanceModified = false;
                        FormDesignVersionManager formDesignMgr = new FormDesignVersionManager(tenantId, vbid.FormDesignVersionID, _formDesignServices);
                        FormDesignVersionDetail detail = formDesignMgr.GetFormDesignVersion(false);
                        FormInstanceDataManager formInstanceDataManager = new FormInstanceDataManager(tenantId, base.CurrentUserId, _formInstanceDataServices, base.CurrentUserName, folderVersionService);
                        string sectionData = formInstanceDataManager.GetSectionData(vbid.FormInstanceID, "PackageInformation", false, detail, false, false);
                        if (!string.IsNullOrEmpty(sectionData))
                        {
                            JObject objSectionData = JObject.Parse(sectionData);
                            JToken tok = objSectionData.SelectToken("PackageInformation.IsReducedCostSharingApplicable");
                            string tokValue = "";
                            if (tok != null)
                            {
                                tokValue = tok.ToString();
                                if (tokValue == "YES" && reducedCostSharing < 0)
                                {
                                    var prop = tok.Parent as JProperty;
                                    prop.Value = "NO";
                                    reducedCostSharing = reducedCostSharing + 1;
                                    isFormInstanceModified = true;
                                }
                            }
                            tok = objSectionData.SelectToken("PackageInformation.IsAdditionalBenefitsApplicable");
                            if (tok != null)
                            {
                                tokValue = tok.ToString();
                                if (tokValue == "YES" && additionalBenefits < 0)
                                {
                                    var prop = tok.Parent as JProperty;
                                    prop.Value = "NO";
                                    additionalBenefits = additionalBenefits + 1;
                                    isFormInstanceModified = true;
                                }
                            }
                            tok = objSectionData.SelectToken("PackageInformation.IsRxApplicable");
                            if (tok != null)
                            {
                                tokValue = tok.ToString();
                                if (tokValue == "YES" && rxPackage < 0)
                                {
                                    var prop = tok.Parent as JProperty;
                                    prop.Value = "NO";
                                    rxPackage = rxPackage + 1;
                                    isFormInstanceModified = true;
                                }
                            }
                            if (isFormInstanceModified == true)
                            {
                                formInstanceDataManager.SetCacheData(vbid.FormInstanceID, "PackageInformation", objSectionData.ToString());
                                formInstanceDataManager.SaveSectionsData(vbid.FormInstanceID, false, folderVersionService, _formDesignServices, detail, "PackageInformation");
                            }
                        }
                        //add package ID
                        SetPackageID(fiNameIdx, vbid.FormInstanceID, formInstanceDataManager, detail);
                        if (model.RxPackageCurrent != model.RxPackageRequired)
                        {
                            string sectionDataVBRX = formInstanceDataManager.GetSectionData(vbid.FormInstanceID, "VBIDRX", false, detail, false, false);
                            if (!string.IsNullOrEmpty(sectionDataVBRX))
                            {
                                JObject objSectionData = JObject.Parse(sectionDataVBRX);
                                JToken tok = objSectionData.SelectToken("VBIDRX.VBIDRXGeneral.HowmanypackagesdoesyourPartDVBIDbenefitcontain");
                                if (tok != null)
                                {
                                    var prop = tok.Parent as JProperty;
                                    prop.Value = model.RxPackageRequired.ToString();
                                }
                                tok = objSectionData.SelectToken("VBIDRX.VBIDRXGeneral.DoesyourVBIDbenefitincludePartDreductionsinCost");
                                if (tok != null)
                                {
                                    var prop = tok.Parent as JProperty;
                                    if (model.RxPackageRequired > 0)
                                    {
                                        prop.Value = "YES";
                                    }
                                    else
                                    {
                                        prop.Value = "NO";
                                    }
                                }
                                formInstanceDataManager.SetCacheData(vbid.FormInstanceID, "VBIDRX", objSectionData.ToString());
                                formInstanceDataManager.SaveSectionsData(vbid.FormInstanceID, false, folderVersionService, _formDesignServices, detail, "VBIDRX");
                            }
                        }
                    }
                }
            }
        }

        public void UpdatePBPVBIDFieldsInternal(int anchorId, int folderVersionId)
        {
            int costSharingCount = 0;
            int additionalBenefitsCount = 0;
            int rxCount = 0;
            GetPackageCounts(anchorId, folderVersionId, ref costSharingCount, ref additionalBenefitsCount, ref rxCount);
            int tenantId = 1;
            //get instances
            List<FormInstanceViewModel> instances = folderVersionService.GetFormInstanceListForAnchor(tenantId, folderVersionId, anchorId);
            //update PBP View
            int anchorFormDesignId = 0;
            int anchorFormInstanceId = 0;
            DateTime? effectiveDate = DateTime.Now;
            //get instances
            //for each VBID View - determine the sections enabled

            var anchorInst = from inst in instances where inst.FormDesignID == 2359 select inst;
            if (anchorInst != null && anchorInst.Count() > 0)
            {
                anchorFormDesignId = anchorInst.First().FormDesignID;
                anchorFormInstanceId = anchorInst.First().FormInstanceID;
                effectiveDate = anchorInst.First().EffectiveDate;
            }
            var childFormDesignList = _formDesignServices.GetMappedDesignDocumentList(tenantId, anchorFormDesignId, effectiveDate);
            var pbpInstances = from inst in instances where inst.FormDesignID == 2367 select inst;
            if (pbpInstances != null && pbpInstances.Count() > 0)
            {
                var pbpInstance = pbpInstances.First();
                var childDesigns = from childDes in childFormDesignList where childDes.FormDesignID == 2367 select childDes;
                int formDesignVersionId = 0;
                if (childDesigns != null && childDesigns.Count() > 0)
                {
                    formDesignVersionId = childDesigns.First().FormDesignVersionID;
                }
                FormDesignVersionManager formDesignMgr = new FormDesignVersionManager(tenantId, formDesignVersionId, _formDesignServices);
                FormDesignVersionDetail detail = formDesignMgr.GetFormDesignVersion(false);
                FormInstanceDataManager formInstanceDataManager = new FormInstanceDataManager(tenantId, base.CurrentUserId, _formInstanceDataServices, base.CurrentUserName, folderVersionService);
                string sectionData = formInstanceDataManager.GetSectionData(pbpInstance.FormInstanceID, "ValueBasedInsuranceDesignVBIDModelTest", false, detail, false, false);
                if (!string.IsNullOrEmpty(sectionData))
                {
                    JObject objSectionData = JObject.Parse(sectionData);
                    JToken tok = objSectionData.SelectToken("ValueBasedInsuranceDesignVBIDModelTest.ReducedCostSharingforVBIDS.DoesyourVBIDbenefitofferPartCreductionsincost");
                    if (tok != null)
                    {
                        var prop = tok.Parent as JProperty;
                        if (costSharingCount > 0)
                        {
                            prop.Value = "1";
                        }
                        else
                        {
                            prop.Value = "2";
                        }
                    }
                    tok = objSectionData.SelectToken("ValueBasedInsuranceDesignVBIDModelTest.ReducedCostSharingforVBIDS.Howmanypackagesdoesyour19aReductioninCostSharingVBIDbenefitcontain115");
                    if (tok != null)
                    {
                        var prop = tok.Parent as JProperty;
                        prop.Value = costSharingCount;
                    }
                    tok = objSectionData.SelectToken("ValueBasedInsuranceDesignVBIDModelTest.AdditionalBenefitsforVBIDs.DoesyourVBIDbenefitofferadditionalPartCbenefits");
                    if (tok != null)
                    {
                        var prop = tok.Parent as JProperty;
                        if (additionalBenefitsCount > 0)
                        {
                            prop.Value = "1";
                        }
                        else
                        {
                            prop.Value = "2";
                        }
                    }
                    tok = objSectionData.SelectToken("ValueBasedInsuranceDesignVBIDModelTest.AdditionalBenefitsforVBIDs.HowmanypackagesdoyourAdditionalBenefitscontain115");
                    if (tok != null)
                    {
                        var prop = tok.Parent as JProperty;
                        prop.Value = additionalBenefitsCount;
                    }
                    formInstanceDataManager.SetCacheData(pbpInstance.FormInstanceID, "ValueBasedInsuranceDesignVBIDModelTest", objSectionData.ToString());
                    formInstanceDataManager.SaveSectionsData(pbpInstance.FormInstanceID, false, folderVersionService, _formDesignServices, detail, "ValueBasedInsuranceDesignVBIDModelTest");
                }

                string sectionDataRx = formInstanceDataManager.GetSectionData(pbpInstance.FormInstanceID, "MedicareRxGeneral", false, detail, false, false);
                if (!string.IsNullOrEmpty(sectionDataRx))
                {
                    JObject objSectionDataRx = JObject.Parse(sectionDataRx);
                    JToken tok = objSectionDataRx.SelectToken("MedicareRxGeneral.VBIDRxGeneral.ValueBasedInsuranceDesignAttestation");
                    if (tok != null)
                    {
                        var prop = tok.Parent as JProperty;
                        if (rxCount > 0)
                        {
                            prop.Value = 1;
                        }
                        else
                        {
                            prop.Value = 2;
                        }
                    }
                    tok = objSectionDataRx.SelectToken("MedicareRxGeneral.VBIDRxGeneral.HowmanypackagesdoesyourPartDVBIDbenefitcontain");
                    if (tok != null)
                    {
                        var prop = tok.Parent as JProperty;
                        prop.Value = rxCount;
                    }
                    tok = objSectionDataRx.SelectToken("MedicareRxGeneral.VBIDRxGeneral.DoesyourVBIDbenefitincludePartDreductionsinCost");
                    if (tok != null)
                    {
                        var prop = tok.Parent as JProperty;
                        if (rxCount == 0)
                        {
                            prop.Value = 2;
                        }
                        else
                        {
                            prop.Value = 1;
                        }
                    }
                    formInstanceDataManager.SetCacheData(pbpInstance.FormInstanceID, "MedicareRxGeneral", objSectionDataRx.ToString());
                    formInstanceDataManager.SaveSectionsData(pbpInstance.FormInstanceID, false, folderVersionService, _formDesignServices, detail, "MedicareRxGeneral");
                }

                var vbidInstances = from inst in instances where inst.FormDesignID == 2409 select inst;
                if (vbidInstances != null && vbidInstances.Count() > 0)
                {
                    FormDesignVersionManager formDesignMgrVBID = new FormDesignVersionManager(tenantId, vbidInstances.First().FormDesignVersionID, _formDesignServices);
                    FormDesignVersionDetail detailVBID = formDesignMgrVBID.GetFormDesignVersion(false);
                    foreach (var vbid in vbidInstances)
                    {
                        string sectionVBIDRX = formInstanceDataManager.GetSectionData(vbid.FormInstanceID, "VBIDRX", false, detailVBID, false, false);
                        if (!string.IsNullOrEmpty(sectionVBIDRX))
                        {
                            JObject objSectionData = JObject.Parse(sectionVBIDRX);
                            JToken tok = objSectionData.SelectToken("VBIDRX.VBIDRXGeneral.HowmanypackagesdoesyourPartDVBIDbenefitcontain");
                            if (tok != null)
                            {
                                var prop = tok.Parent as JProperty;
                                prop.Value = rxCount.ToString();
                            }
                            formInstanceDataManager.SetCacheData(vbid.FormInstanceID, "VBIDRX", objSectionData.ToString());
                            formInstanceDataManager.SaveSectionsData(vbid.FormInstanceID, false, folderVersionService, _formDesignServices, detailVBID, "VBIDRX");
                        }
                    }
                }

            }
        }

        private void SetPackageID(int packageId, int formInstanceId, FormInstanceDataManager manager, FormDesignVersionDetail detail)
        {

            //NineteenAReducedCostSharingforVBIDUFGroup1.VBIDUFGeneralInformation.GroupID
            string sectionData = manager.GetSectionData(formInstanceId, "NineteenAReducedCostSharingforVBIDUFGroup1", false, detail, false, false);
            if (!string.IsNullOrEmpty(sectionData))
            {
                JObject objSectionData = JObject.Parse(sectionData);
                JToken tok = objSectionData.SelectToken("NineteenAReducedCostSharingforVBIDUFGroup1.VBIDUFGeneralInformation.GroupID");
                if (tok != null)
                {
                    var prop = tok.Parent as JProperty;
                    prop.Value = packageId;
                }
                manager.SetCacheData(formInstanceId, "NineteenAReducedCostSharingforVBIDUFGroup1", objSectionData.ToString());
                manager.SaveSectionsData(formInstanceId, false, folderVersionService, _formDesignServices, detail, "NineteenAReducedCostSharingforVBIDUFGroup1");
            }
        }

        private void SetPackageIDOnDelete(int packageId, FormInstanceViewModel fiModel)
        {
            int tenantId = 1;
            FormDesignVersionManager formDesignMgr = new FormDesignVersionManager(tenantId, fiModel.FormDesignVersionID, _formDesignServices);
            FormDesignVersionDetail detail = formDesignMgr.GetFormDesignVersion(false);
            FormInstanceDataManager formInstanceDataManager = new FormInstanceDataManager(tenantId, base.CurrentUserId, _formInstanceDataServices, base.CurrentUserName, folderVersionService);
            string sectionData = formInstanceDataManager.GetSectionData(fiModel.FormInstanceID, "NineteenAReducedCostSharingforVBIDUFGroup1", false, detail, false, false);

            //NineteenAReducedCostSharingforVBIDUFGroup1.VBIDUFGeneralInformation.GroupID
            if (!string.IsNullOrEmpty(sectionData))
            {
                JObject objSectionData = JObject.Parse(sectionData);
                JToken tok = objSectionData.SelectToken("NineteenAReducedCostSharingforVBIDUFGroup1.VBIDUFGeneralInformation.GroupID");
                if (tok != null)
                {
                    var prop = tok.Parent as JProperty;
                    prop.Value = packageId;
                }
                formInstanceDataManager.SetCacheData(fiModel.FormInstanceID, "NineteenAReducedCostSharingforVBIDUFGroup1", objSectionData.ToString());
                formInstanceDataManager.SaveSectionsData(fiModel.FormInstanceID, false, folderVersionService, _formDesignServices, detail, "NineteenAReducedCostSharingforVBIDUFGroup1");
            }
        }
    }
}
