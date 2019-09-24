using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.applicationservices.viewmodels.PBPImport;
using tmg.equinox.web.ExpresionBuilder;
using tmg.equinox.web.FormDesignManager;
using tmg.equinox.web.FormInstanceManager;

namespace tmg.equinox.pbpimportservices.PBPMappingsBuilder
{
    public class BenefitReviewBuilder
    {
        int _forminstanceid;
        string _pbpQId = string.Empty;
        int _pbpBatchId;
        List<PBPDataMapViewModel> _pbpDataModelList;
        string _userName;
        private IFolderVersionServices _folderVersionServices;
        private IPBPImportServices _pbpImportServices;
        private IFormDesignService _formDesignService;
        private IFormInstanceDataServices _formInstanceDataServices;
        MapPBPData _mapPBPData;
        int _qid;

        public BenefitReviewBuilder(int forminstanceId, int qid, string pbpQId, int pbpBatchId, string userName, List<PBPDataMapViewModel> pbpDataModelList, IPBPImportServices pbpServices, IFolderVersionServices folderVersionServices, IFormDesignService formDesignService, IFormInstanceDataServices formInstanceDataServices, MapPBPData mapPBPData)
        {
            this._forminstanceid = forminstanceId;
            this._pbpBatchId = pbpBatchId;
            this._pbpQId = pbpQId;
            this._pbpDataModelList = pbpDataModelList;
            this._userName = userName;
            this._pbpImportServices = pbpServices;
            this._mapPBPData = mapPBPData;
            this._qid = qid;
            this._folderVersionServices = folderVersionServices;
            this._formDesignService = formDesignService;
            this._formInstanceDataServices = formInstanceDataServices;
        }

        #region BenefitReview

        private void PopulateBenefitReview(JObject json, ref List<PBPImportActivityLogViewModel> benefirReviewLog)
        {
            List<BenefitReviewViewModel> benefitmapping = _pbpImportServices.GetBenefitReviewMapping();
            List<BenefitReviewAmountViewModel> benefitamountmapping = _pbpImportServices.GetBenefitReviewAmountMapping();
            List<AuthMappingViewModel> authmapping = _pbpImportServices.GetAuthMappingMapping();
            List<BenefitReviewOONViewModel> oonmappings = _pbpImportServices.GetBenefitReviewOONMapping();

            string FieldPath = string.Empty;
            string FieldName = string.Empty;
            JArray jarry = new JArray();
            FieldPath = benefitmapping[0].FieldPath;
            FieldName = benefitmapping[0].FieldName;

            //GET Default services from masterlist
            if (json[FieldPath] is JObject)
            {
                FormInstanceViewModel forminstance = _folderVersionServices.GetFormInstance(1, _forminstanceid);
                FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(1, forminstance.FormDesignVersionID, _formDesignService);
                FormDesignVersionDetail formdetails = formDesignVersionMgr.GetFormDesignVersion();
                FormInstanceDataManager formDataInstanceManager = new FormInstanceDataManager(1, _formInstanceDataServices);
                BRGRules run = new BRGRules();
                JObject brgobj = (JObject)json[FieldPath];

                run.RunOnDocumentForPBP(formDataInstanceManager, _forminstanceid, formdetails, _folderVersionServices, _formDesignService, forminstance.FormDesignVersionID, json);
            }

            if (json.SelectToken(FieldPath)[FieldName] != null && json.SelectToken(FieldPath)[FieldName] is JArray && ((JArray)json.SelectToken(FieldPath)[FieldName]).Count > 0)
                jarry = (JArray)json.SelectToken(FieldPath)[FieldName];

            JToken job = jarry.Where(c => c[KEYNAME.BenefitCategory1].ToString() == string.Empty && c[KEYNAME.BenefitCategory2].ToString() == string.Empty && c[KEYNAME.BenefitCategory3].ToString() == string.Empty).FirstOrDefault();
            if (job != null)
                jarry.Remove(job);

            foreach (BenefitReviewViewModel viewmodel in benefitmapping)
            {
                string pbpData = string.Empty;

                bool isselectedincostshare = false;
                if (_mapPBPData._defaultServices != null)
                    foreach (CostShareDefaultServices c in _mapPBPData._defaultServices)
                        if (c.BenefitCategory1 == viewmodel.BenefitCategory1.Trim() && c.BenefitCategory2 == viewmodel.BenefitCategory2.Trim()
                            && c.BenefitCategory3 == viewmodel.BenefitCategory3.Trim())
                        {
                            isselectedincostshare = true;
                            break;
                        }

                if (viewmodel.ByDefaultSelection || isselectedincostshare)
                    pbpData = true.ToString();
                else if (viewmodel.SequenceNumberforOne != 0)
                    pbpData = HelperUtility.ApplyCustomRuleGetData(viewmodel.PBPTableName, viewmodel.PBPFieldName, viewmodel.CustomRuleTypeId, viewmodel.SequenceNumberforOne, _pbpDataModelList, _pbpQId, _pbpBatchId, null);
                else if (!string.IsNullOrEmpty(viewmodel.ValueToMatch))
                {
                    string code = HelperUtility.GetDataFromPBP(viewmodel.PBPTableName, viewmodel.PBPFieldName, _pbpDataModelList, _pbpQId, _pbpBatchId);

                    if (viewmodel.IsCustomRule && !string.IsNullOrEmpty(code))
                        pbpData = HelperUtility.CheckIfServiceCodeExists(code, viewmodel.ValueToMatch, ';').ToString();
                    else if (code == viewmodel.ValueToMatch)
                        pbpData = true.ToString();
                }
                else
                {
                    string code = HelperUtility.GetDataFromPBP(viewmodel.PBPTableName, viewmodel.PBPFieldName, _pbpDataModelList, _pbpQId, _pbpBatchId);
                    if (!string.IsNullOrEmpty(code))
                        pbpData = true.ToString();
                }

                JToken jobj = jarry.Where(c => c[KEYNAME.BenefitCategory1].ToString() == viewmodel.BenefitCategory1.Trim() && c[KEYNAME.BenefitCategory2].ToString() == viewmodel.BenefitCategory2.Trim() && c[KEYNAME.BenefitCategory3].ToString() == viewmodel.BenefitCategory3.Trim()).FirstOrDefault();
                bool isServiceSelected = false;
                string preAuthorization = string.Empty;
                if (bool.TryParse(pbpData, out isServiceSelected))
                {
                    if (isServiceSelected)
                    {
                        if (jobj == null)
                        {
                            jobj = new JObject();
                            jarry.Add(jobj);
                        }

                        jobj[KEYNAME.BenefitCategory1] = viewmodel.BenefitCategory1.Trim();
                        jobj[KEYNAME.BenefitCategory2] = viewmodel.BenefitCategory2.Trim();
                        jobj[KEYNAME.BenefitCategory3] = viewmodel.BenefitCategory3.Trim();
                        if (isselectedincostshare)
                            jobj[ELEMENTNAME.SlidingCostShare] = STANDARDVALUE.Yes;
                        else
                            jobj[ELEMENTNAME.SlidingCostShare] = STANDARDVALUE.No;

                        preAuthorization = GetPreAuthorization(authmapping, viewmodel.BenefitReviewID);
                        jobj[ELEMENTNAME.PreAuthorization] = preAuthorization;

                        benefirReviewLog.Add(HelperUtility.GetLogEntity("--> Service selected: BC1 = " + viewmodel.BenefitCategory1.Trim() + ", BC2 = " + viewmodel.BenefitCategory2.Trim() + ", BC3 = " + viewmodel.BenefitCategory3.Trim() + " ,PreAuthorization= " + preAuthorization, _userName, _qid, _pbpBatchId));

                        JArray jnetworklist = new JArray();
                        if (jobj[DATASOURCENAME.IQMedicareNetworkList] != null)
                            jnetworklist = (JArray)jobj[DATASOURCENAME.IQMedicareNetworkList];
                        jobj[DATASOURCENAME.IQMedicareNetworkList] = GetBenefitReviewAmount(jnetworklist, benefitamountmapping, viewmodel, oonmappings, ref benefirReviewLog);
                        benefirReviewLog.Add(HelperUtility.GetLogEntity("<-- Service selected: BC1 = " + viewmodel.BenefitCategory1.Trim() + ", BC2 = " + viewmodel.BenefitCategory2.Trim() + ", BC3 = " + viewmodel.BenefitCategory3.Trim() + " ,PreAuthorization= " + preAuthorization, _userName, _qid, _pbpBatchId));
                    }
                    else
                    {
                        if (jobj != null)
                            jarry.Remove(jobj);
                    }
                }
                else
                {
                    if (jobj != null)
                        jarry.Remove(jobj);
                }
            }

            json.SelectToken(FieldPath)[FieldName] = jarry;
        }

        private string GetPreAuthorization(List<AuthMappingViewModel> authmapping, int benefitReviewID)
        {
            AuthMappingViewModel viewmodel = authmapping.Where(a => a.BenefitReviewID == benefitReviewID).FirstOrDefault();
            if (viewmodel.CustomRuleTypeId == 3)
            {
                string authmap = HelperUtility.ApplyCustomRuleGetData(viewmodel.PBPTableName, viewmodel.PBPFieldName, viewmodel.CustomRuleTypeId, viewmodel.SequenceNumberforOne, _pbpDataModelList, _pbpQId, _pbpBatchId, null);

                bool isServiceSelected = false;

                if (!string.IsNullOrEmpty(authmap) && bool.TryParse(authmap, out isServiceSelected))
                    return (!isServiceSelected).ToString();
                else
                    return false.ToString();
            }
            else if (viewmodel.CustomRuleTypeId == 2)
            {
                string authmap = HelperUtility.ApplyCustomRuleGetData(viewmodel.PBPTableName, viewmodel.PBPFieldName, viewmodel.CustomRuleTypeId, viewmodel.SequenceNumberforOne, _pbpDataModelList, _pbpQId, _pbpBatchId, null);
                return authmap;
            }
            else
                return false.ToString();
        }

        private JArray GetBenefitReviewAmount(JArray jnetworklist, List<BenefitReviewAmountViewModel> benefitamountmapping, BenefitReviewViewModel benviewmodel, List<BenefitReviewOONViewModel> oonmappings, ref List<PBPImportActivityLogViewModel> BRGAmountLog)
        {
            int benefitReviewID = benviewmodel.BenefitReviewID;
            List<BenefitReviewAmountViewModel> amountlist = new List<BenefitReviewAmountViewModel>();
            if (_mapPBPData._isOONApplicable)
                amountlist = benefitamountmapping.Where(b => b.BenefitReviewID == benefitReviewID).ToList();
            else
                amountlist = benefitamountmapping.Where(b => b.BenefitReviewID == benefitReviewID && b.NetworkType == DATA.InNetwork).ToList();

            foreach (BenefitReviewAmountViewModel viewmodel in amountlist)
            {
                //JObject jobj = new JObject();
                JToken jobj = jnetworklist.Where(j => j[ELEMENTNAME.CostShareTiers].ToString() == viewmodel.NetworkType).FirstOrDefault();

                if (jobj == null)
                {
                    jobj = new JObject();
                    jnetworklist.Add(jobj);
                }

                jobj[ELEMENTNAME.CostShareTiers] = viewmodel.NetworkType;
                BRGAmountLog.Add(HelperUtility.GetLogEntity("    NetworkType=" + viewmodel.NetworkType, _userName, _qid, _pbpBatchId));
                if (viewmodel.NetworkType == DATA.InNetwork)
                {
                    if (!string.IsNullOrEmpty(viewmodel.DeductiblePBPTableName) && !string.IsNullOrEmpty(viewmodel.DeductiblePBPFieldName))
                    {
                        string deductible = HelperUtility.GetDataFromPBP(viewmodel.DeductiblePBPTableName, viewmodel.DeductiblePBPFieldName, _pbpDataModelList, _pbpQId, _pbpBatchId);
                        jobj[viewmodel.DeductiblePath] = string.IsNullOrEmpty(deductible) ? STANDARDVALUE.NotApplicable : deductible;
                    }
                    else
                        jobj[viewmodel.DeductiblePath] = jobj[viewmodel.DeductiblePath] != null ? jobj[viewmodel.DeductiblePath].ToString() : STANDARDVALUE.NotApplicable;

                    if (!string.IsNullOrEmpty(viewmodel.MinimumCopayPBPTableName) && !string.IsNullOrEmpty(viewmodel.MinimumCopayPBPFieldName))
                    {
                        string minimumCopay = HelperUtility.GetDataFromPBP(viewmodel.MinimumCopayPBPTableName, viewmodel.MinimumCopayPBPFieldName, _pbpDataModelList, _pbpQId, _pbpBatchId);
                        jobj[viewmodel.MinimumCopayPath] = string.IsNullOrEmpty(minimumCopay) ? STANDARDVALUE.NotApplicable : minimumCopay;
                    }
                    else
                        jobj[viewmodel.MinimumCopayPath] = jobj[viewmodel.MinimumCopayPath] != null ? jobj[viewmodel.MinimumCopayPath].ToString() : STANDARDVALUE.NotApplicable;

                    if (!string.IsNullOrEmpty(viewmodel.MaximumCopayPBPTableName) && !string.IsNullOrEmpty(viewmodel.MaximumCopayPBPFieldName))
                    {
                        string maximumCopay = HelperUtility.GetDataFromPBP(viewmodel.MaximumCopayPBPTableName, viewmodel.MaximumCopayPBPFieldName, _pbpDataModelList, _pbpQId, _pbpBatchId);
                        jobj[viewmodel.MaximumCopayPath] = string.IsNullOrEmpty(maximumCopay) ? STANDARDVALUE.NotApplicable : maximumCopay;
                    }
                    else
                        jobj[viewmodel.MaximumCopayPath] = jobj[viewmodel.MaximumCopayPath] != null ? jobj[viewmodel.MaximumCopayPath] : STANDARDVALUE.NotApplicable;

                    if (!string.IsNullOrEmpty(viewmodel.MinimumCoissurancePBPTableName) && !string.IsNullOrEmpty(viewmodel.MinimumCoissurancePBPFieldName))
                    {
                        string minimumCoissurance = HelperUtility.GetDataFromPBP(viewmodel.MinimumCoissurancePBPTableName, viewmodel.MinimumCoissurancePBPFieldName, _pbpDataModelList, _pbpQId, _pbpBatchId);
                        jobj[viewmodel.MinimumCoissurancePath] = string.IsNullOrEmpty(minimumCoissurance) ? STANDARDVALUE.NotApplicable : minimumCoissurance;
                    }
                    else
                        jobj[viewmodel.MinimumCoissurancePath] = jobj[viewmodel.MinimumCoissurancePath] != null ? jobj[viewmodel.MinimumCoissurancePath] : STANDARDVALUE.NotApplicable;

                    if (!string.IsNullOrEmpty(viewmodel.MaximumCoissurancePBPTableName) && !string.IsNullOrEmpty(viewmodel.MaximumCoissurancePBPFieldName))
                    {
                        string maximumCoissurance = HelperUtility.GetDataFromPBP(viewmodel.MaximumCoissurancePBPTableName, viewmodel.MaximumCoissurancePBPFieldName, _pbpDataModelList, _pbpQId, _pbpBatchId);
                        jobj[viewmodel.MaximumCoissurancePath] = string.IsNullOrEmpty(maximumCoissurance) ? STANDARDVALUE.NotApplicable : maximumCoissurance;
                    }
                    else
                        jobj[viewmodel.MaximumCoissurancePath] = jobj[viewmodel.MaximumCoissurancePath] != null ? jobj[viewmodel.MaximumCoissurancePath] : STANDARDVALUE.NotApplicable;

                    if (!string.IsNullOrEmpty(viewmodel.OOPMValuePBPTableName) && !string.IsNullOrEmpty(viewmodel.OOPMValuePBPFieldName))
                    {
                        string oopm = HelperUtility.GetDataFromPBP(viewmodel.OOPMValuePBPTableName, viewmodel.OOPMValuePBPFieldName, _pbpDataModelList, _pbpQId, _pbpBatchId);
                        jobj[viewmodel.OOPMValuePath] = string.IsNullOrEmpty(oopm) ? STANDARDVALUE.NotApplicable : oopm;
                    }
                    else
                        jobj[viewmodel.OOPMValuePath] = jobj[viewmodel.OOPMValuePath] != null ? jobj[viewmodel.OOPMValuePath] : STANDARDVALUE.NotApplicable;
                    if (!string.IsNullOrEmpty(viewmodel.OOPMPeriodicityPBPTableName) && !string.IsNullOrEmpty(viewmodel.OOPMPeriodicityPBPFieldName))
                    {
                        string oopmperiod = HelperUtility.GetMaximumPlanBenefitCoveragePeriodicity(HelperUtility.GetDataFromPBP(viewmodel.OOPMPeriodicityPBPTableName, viewmodel.OOPMPeriodicityPBPFieldName, _pbpDataModelList, _pbpQId, _pbpBatchId));
                        jobj[viewmodel.OOPMPeriodicityPath] = string.IsNullOrEmpty(oopmperiod) ? STANDARDVALUE.NotApplicable : oopmperiod;
                    }
                    else
                        jobj[viewmodel.OOPMPeriodicityPath] = jobj[viewmodel.OOPMPeriodicityPath] != null ? jobj[viewmodel.OOPMPeriodicityPath].ToString() : STANDARDVALUE.NotApplicable;

                    if (!string.IsNullOrEmpty(viewmodel.MaximumPlanBenefitCoverageAmountPBPTableName) && !string.IsNullOrEmpty(viewmodel.MaximumPlanBenefitCoverageAmountPBPFieldName))
                    {
                        string maxcov = HelperUtility.GetDataFromPBP(viewmodel.MaximumPlanBenefitCoverageAmountPBPTableName, viewmodel.MaximumPlanBenefitCoverageAmountPBPFieldName, _pbpDataModelList, _pbpQId, _pbpBatchId);
                        jobj[viewmodel.MaximumPlanBenefitCoverageAmountPath] = string.IsNullOrEmpty(maxcov) ? STANDARDVALUE.NotApplicable : maxcov;
                    }
                    else
                        jobj[viewmodel.MaximumPlanBenefitCoverageAmountPath] = jobj[viewmodel.MaximumPlanBenefitCoverageAmountPath] != null ? jobj[viewmodel.MaximumPlanBenefitCoverageAmountPath].ToString() : STANDARDVALUE.NotApplicable;

                    if (!string.IsNullOrEmpty(viewmodel.MaximumPlanBenefitCoveragePeriodicityPBPTableName) && !string.IsNullOrEmpty(viewmodel.MaximumPlanBenefitCoveragePeriodicityPBPFieldName))
                    {
                        string maxcovperiod = HelperUtility.GetMaximumPlanBenefitCoveragePeriodicity(HelperUtility.GetDataFromPBP(viewmodel.MaximumPlanBenefitCoveragePeriodicityPBPTableName, viewmodel.MaximumPlanBenefitCoveragePeriodicityPBPFieldName, _pbpDataModelList, _pbpQId, _pbpBatchId));
                        jobj[viewmodel.MaximumPlanBenefitCoveragePeriodicityPath] = string.IsNullOrEmpty(maxcovperiod) ? STANDARDVALUE.NotApplicable : maxcovperiod;
                    }
                    else
                        jobj[viewmodel.MaximumPlanBenefitCoveragePeriodicityPath] = jobj[viewmodel.MaximumPlanBenefitCoveragePeriodicityPath] != null ? jobj[viewmodel.MaximumPlanBenefitCoveragePeriodicityPath].ToString() : STANDARDVALUE.NotApplicable;
                }
                else
                {
                    bool isOONApplicable = false;
                    if (viewmodel.NetworkType == DATA.OutOfNetwork)
                        isOONApplicable = CheckIfOONApplicable(benviewmodel, oonmappings);

                    string deductible = STANDARDVALUE.NotApplicable;
                    string minimumCopay = STANDARDVALUE.NotApplicable;
                    string maximumCopay = STANDARDVALUE.NotApplicable;
                    string minimumCoissurance = STANDARDVALUE.NotApplicable;
                    string maximumCoissurance = STANDARDVALUE.NotApplicable;
                    string oopm = STANDARDVALUE.NotApplicable;
                    string oopmperiod = STANDARDVALUE.NotApplicable;
                    string maxcov = STANDARDVALUE.NotApplicable;
                    string maxcovperiod = STANDARDVALUE.NotApplicable;

                    if (isOONApplicable)
                    {
                        PBPDataMapViewModel datamodel = GetPBPDataMapViewModelForCode(benviewmodel, oonmappings);
                        deductible = HelperUtility.GetDataFromPBP(viewmodel.DeductiblePBPTableName, viewmodel.DeductiblePBPFieldName, datamodel);
                        minimumCopay = HelperUtility.GetDataFromPBP(viewmodel.MinimumCopayPBPTableName, viewmodel.MinimumCopayPBPFieldName, datamodel);
                        maximumCopay = HelperUtility.GetDataFromPBP(viewmodel.MaximumCopayPBPTableName, viewmodel.MaximumCopayPBPFieldName, datamodel);
                        minimumCoissurance = HelperUtility.GetDataFromPBP(viewmodel.MinimumCoissurancePBPTableName, viewmodel.MinimumCoissurancePBPFieldName, datamodel);
                        maximumCoissurance = HelperUtility.GetDataFromPBP(viewmodel.MaximumCoissurancePBPTableName, viewmodel.MaximumCoissurancePBPFieldName, datamodel);
                        oopm = HelperUtility.GetDataFromPBP(viewmodel.OOPMValuePBPTableName, viewmodel.OOPMValuePBPFieldName, datamodel);
                        oopmperiod = HelperUtility.GetMaximumPlanBenefitCoveragePeriodicity(HelperUtility.GetDataFromPBP(viewmodel.OOPMPeriodicityPBPTableName, viewmodel.OOPMPeriodicityPBPFieldName, datamodel));
                        maxcov = HelperUtility.GetDataFromPBP(viewmodel.MaximumPlanBenefitCoverageAmountPBPTableName, viewmodel.MaximumPlanBenefitCoverageAmountPBPFieldName, datamodel);
                        maxcovperiod = HelperUtility.GetMaximumPlanBenefitCoveragePeriodicity(HelperUtility.GetDataFromPBP(viewmodel.MaximumPlanBenefitCoveragePeriodicityPBPTableName, viewmodel.MaximumPlanBenefitCoveragePeriodicityPBPFieldName, datamodel));
                        BRGAmountLog.Add(HelperUtility.GetLogEntity("   Deductible= " + deductible + " Min Copay= " + minimumCopay + " MaxCopay= " + maximumCopay + " Min Coissurance= " + minimumCoissurance + " Max Coissurance=" + maximumCoissurance + " OOPM= " + oopm + " OOPMPeriodicity= " + oopmperiod + " MaximumPlanBenefitCoverage= " + maxcov + " MaximumPlanBenefitCoveragePeriodicity= " + maxcovperiod, _userName, _qid, _pbpBatchId));
                    }

                    if (!string.IsNullOrEmpty(viewmodel.DeductiblePBPTableName) && !string.IsNullOrEmpty(viewmodel.DeductiblePBPFieldName))
                        jobj[viewmodel.DeductiblePath] = string.IsNullOrEmpty(deductible) ? STANDARDVALUE.NotApplicable : deductible;
                    else
                        jobj[viewmodel.DeductiblePath] = jobj[viewmodel.DeductiblePath] != null ? jobj[viewmodel.DeductiblePath].ToString() : STANDARDVALUE.NotApplicable;

                    if (!string.IsNullOrEmpty(viewmodel.MinimumCopayPBPTableName) && !string.IsNullOrEmpty(viewmodel.MinimumCopayPBPFieldName))
                        jobj[viewmodel.MinimumCopayPath] = string.IsNullOrEmpty(minimumCopay) ? STANDARDVALUE.NotApplicable : minimumCopay;
                    else
                        jobj[viewmodel.MinimumCopayPath] = jobj[viewmodel.MinimumCopayPath] != null ? jobj[viewmodel.MinimumCopayPath].ToString() : STANDARDVALUE.NotApplicable;

                    if (!string.IsNullOrEmpty(viewmodel.MaximumCopayPBPTableName) && !string.IsNullOrEmpty(viewmodel.MaximumCopayPBPFieldName))
                        jobj[viewmodel.MaximumCopayPath] = string.IsNullOrEmpty(maximumCopay) ? STANDARDVALUE.NotApplicable : maximumCopay;
                    else
                        jobj[viewmodel.MaximumCopayPath] = jobj[viewmodel.MaximumCopayPath] != null ? jobj[viewmodel.MaximumCopayPath] : STANDARDVALUE.NotApplicable;

                    if (!string.IsNullOrEmpty(viewmodel.MinimumCoissurancePBPTableName) && !string.IsNullOrEmpty(viewmodel.MinimumCoissurancePBPFieldName))
                        jobj[viewmodel.MinimumCoissurancePath] = string.IsNullOrEmpty(minimumCoissurance) ? STANDARDVALUE.NotApplicable : minimumCoissurance;
                    else
                        jobj[viewmodel.MinimumCoissurancePath] = jobj[viewmodel.MinimumCoissurancePath] != null ? jobj[viewmodel.MinimumCoissurancePath] : STANDARDVALUE.NotApplicable;

                    if (!string.IsNullOrEmpty(viewmodel.MaximumCoissurancePBPTableName) && !string.IsNullOrEmpty(viewmodel.MaximumCoissurancePBPFieldName))
                        jobj[viewmodel.MaximumCoissurancePath] = string.IsNullOrEmpty(maximumCoissurance) ? STANDARDVALUE.NotApplicable : maximumCoissurance;
                    else
                        jobj[viewmodel.MaximumCoissurancePath] = jobj[viewmodel.MaximumCoissurancePath] != null ? jobj[viewmodel.MaximumCoissurancePath] : STANDARDVALUE.NotApplicable;

                    if (!string.IsNullOrEmpty(viewmodel.OOPMValuePBPFieldName) && !string.IsNullOrEmpty(viewmodel.OOPMValuePBPFieldName))
                        jobj[viewmodel.OOPMValuePath] = string.IsNullOrEmpty(oopm) ? STANDARDVALUE.NotApplicable : oopm;
                    else
                        jobj[viewmodel.OOPMValuePath] = jobj[viewmodel.OOPMValuePath] != null ? jobj[viewmodel.OOPMValuePath] : STANDARDVALUE.NotApplicable;
                    if (!string.IsNullOrEmpty(viewmodel.OOPMPeriodicityPBPTableName) && !string.IsNullOrEmpty(viewmodel.OOPMPeriodicityPBPFieldName))
                        jobj[viewmodel.OOPMPeriodicityPath] = string.IsNullOrEmpty(oopmperiod) ? STANDARDVALUE.NotApplicable : oopmperiod;
                    else
                        jobj[viewmodel.OOPMPeriodicityPath] = jobj[viewmodel.OOPMPeriodicityPath] != null ? jobj[viewmodel.OOPMPeriodicityPath].ToString() : STANDARDVALUE.NotApplicable;

                    if (!string.IsNullOrEmpty(viewmodel.MaximumPlanBenefitCoverageAmountPBPTableName) && !string.IsNullOrEmpty(viewmodel.MaximumPlanBenefitCoverageAmountPBPFieldName))
                        jobj[viewmodel.MaximumPlanBenefitCoverageAmountPath] = string.IsNullOrEmpty(maxcov) ? STANDARDVALUE.NotApplicable : maxcov;
                    else
                        jobj[viewmodel.MaximumPlanBenefitCoverageAmountPath] = jobj[viewmodel.MaximumPlanBenefitCoverageAmountPath] != null ? jobj[viewmodel.MaximumPlanBenefitCoverageAmountPath].ToString() : STANDARDVALUE.NotApplicable;
                    if (!string.IsNullOrEmpty(viewmodel.MaximumPlanBenefitCoveragePeriodicityPBPTableName) && !string.IsNullOrEmpty(viewmodel.MaximumPlanBenefitCoveragePeriodicityPBPFieldName))
                        jobj[viewmodel.MaximumPlanBenefitCoveragePeriodicityPath] = string.IsNullOrEmpty(maxcovperiod) ? STANDARDVALUE.NotApplicable : maxcovperiod;
                    else
                        jobj[viewmodel.MaximumPlanBenefitCoveragePeriodicityPath] = jobj[viewmodel.MaximumPlanBenefitCoveragePeriodicityPath] != null ? jobj[viewmodel.MaximumPlanBenefitCoveragePeriodicityPath].ToString() : STANDARDVALUE.NotApplicable;
                }
            }

            return jnetworklist;
        }

        private bool CheckIfOONApplicable(BenefitReviewViewModel benviewmodel, List<BenefitReviewOONViewModel> oonmappings)
        {
            bool isapplicable = false;

            BenefitReviewOONViewModel oon = oonmappings.Where(b => b.BenefitReviewID == benviewmodel.BenefitReviewID).FirstOrDefault();
            if (oon == null)
                oon = oonmappings.Where(b => b.BC1.Trim() == benviewmodel.BenefitCategory1.Trim() && b.BC2.Trim() == benviewmodel.BenefitCategory2.Trim() && b.BC3.Trim() == benviewmodel.BenefitCategory3.Trim()).FirstOrDefault();

            string codelist = HelperUtility.GetDataFromPBP(benviewmodel.OONPBPTableName, benviewmodel.OONPBPFieldName, _pbpDataModelList, _pbpQId, _pbpBatchId);

            if (oon == null || string.IsNullOrEmpty(codelist))
                return isapplicable;

            isapplicable = HelperUtility.CheckIfServiceCodeExists(codelist, oon.Code, ';');

            return isapplicable;
        }

        private PBPDataMapViewModel GetPBPDataMapViewModelForCode(BenefitReviewViewModel benviewmodel, List<BenefitReviewOONViewModel> oonmappings)
        {
            BenefitReviewOONViewModel oon = oonmappings.Where(b => b.BenefitReviewID == benviewmodel.BenefitReviewID).FirstOrDefault();
            if (oon == null)
                oon = oonmappings.Where(b => b.BC1.Trim() == benviewmodel.BenefitCategory1.Trim() && b.BC2.Trim() == benviewmodel.BenefitCategory2.Trim() && b.BC3.Trim() == benviewmodel.BenefitCategory3.Trim()).FirstOrDefault();

            if (!string.IsNullOrEmpty(benviewmodel.OONPBPTableName) && !string.IsNullOrEmpty(benviewmodel.OONPBPTableName))
            {
                List<PBPDataMapViewModel> dataModelList = _pbpDataModelList.Where(d => d.QID == _pbpQId && d.TableName == benviewmodel.OONPBPTableName && d.PBPImportBatchID == _pbpBatchId).ToList();
                foreach (PBPDataMapViewModel datamodel in dataModelList)
                {
                    JObject jobj = JObject.Parse(datamodel.JsonData);
                    if (!string.IsNullOrEmpty(jobj[benviewmodel.OONPBPFieldName].ToString()))
                    {
                        string codelist = jobj[benviewmodel.OONPBPFieldName].ToString();
                        bool isapplicable = HelperUtility.CheckIfServiceCodeExists(codelist, oon.Code, ';');
                        if (isapplicable)
                            return datamodel;
                    }
                }
            }

            return null;
        }

        #endregion BenefitReview
    }
}
