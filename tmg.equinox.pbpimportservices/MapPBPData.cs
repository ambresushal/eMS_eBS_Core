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
using Newtonsoft.Json;
using tmg.equinox.applicationservices;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.PBPImport;
using tmg.equinox.dependencyresolution;
using Newtonsoft.Json.Linq;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.web.ExpresionBuilder;
using tmg.equinox.web.FormInstanceManager;
using tmg.equinox.web.FormDesignManager;
using System.Globalization;

namespace tmg.equinox.pbpimportservices
{
    public class MapPBPData
    {
        private IFolderVersionServices _folderVersionServices;
        private IPBPImportServices _pbpImportServices;
        private IFormDesignService _formDesignService;
        private IFormInstanceDataServices _formInstanceDataServices;
        int _forminstanceid;
        string _pbpQId = string.Empty;
        int _pbpBatchId;
        List<PBPDataMapViewModel> _pbpDataModelList;
        string _medicareJson = string.Empty;
        public bool _isOONApplicable = false;
        private string _userName;
        private int _queueId = 1;
        public List<CostShareDefaultServices> _defaultServices = new List<CostShareDefaultServices>();
        
        public MapPBPData(int forminstanceId, string pbpQId, int pbpBatchId, string userName)
        {
            UnityConfig.RegisterComponents();
            _folderVersionServices = UnityConfig.Resolve<IFolderVersionServices>();
            _pbpImportServices = UnityConfig.Resolve<IPBPImportServices>();
            _formDesignService = UnityConfig.Resolve<IFormDesignService>();
            _formInstanceDataServices = UnityConfig.Resolve<IFormInstanceDataServices>();
            _defaultServices = new List<CostShareDefaultServices>();

            this._forminstanceid = forminstanceId;
            this._pbpQId = pbpQId;
            this._pbpBatchId = pbpBatchId;
            this._userName = userName;

            _pbpDataModelList = _pbpImportServices.GetPBPDataMapList(_pbpBatchId).ToList();
            _medicareJson = _folderVersionServices.GetFormInstanceDataCompressed(1, _forminstanceid);
        }

        public void MapPBPDataToMedicare()
        {
            try
            {
                JObject json = JObject.Parse(_medicareJson);
                List<PBPImportActivityLogViewModel> mappinglog = new List<PBPImportActivityLogViewModel>();
                MapPlanInformationData(json);
                PopulateNetwork(json);
                PopulatePrescription(json, ref mappinglog);
                PopulateCostShare(json, ref mappinglog);
                PopulateBenefitReview(json, ref mappinglog);

                string json1 = JsonConvert.SerializeObject(json);
                _folderVersionServices.SaveFormInstanceDataCompressed(_forminstanceid, json1);
                _pbpImportServices.AddPBPImportActivityLog(mappinglog);
            }
            catch (Exception ex)
            {
                _pbpImportServices.AddPBPImportActivityLog(this._queueId, this._pbpBatchId, null, null, ex.InnerException == null ? ex.Message : ex.InnerException.InnerException.Message, this._userName);
            }
        }

        #region Prescription

        private void PopulatePrescription(JObject json, ref List<PBPImportActivityLogViewModel> prescriptionLog)
        {
            List<PrescriptionViewModel> presmapping = _pbpImportServices.GetPrescriptionMapping();
            List<PrescriptionRepeaterViewModel> repeatermaps = _pbpImportServices.GetPrescriptionRepeaterMapping();

            int tiercount = 0;
            Dictionary<int, string> tierdescription = new Dictionary<int, string>();

            foreach (PrescriptionViewModel viewmodel in presmapping)
            {
                prescriptionLog.Add(this.GetLogEntity("PrescriptionID = " + viewmodel.PrescriptionID + " IsRepeater = " + viewmodel.IsRepeater.ToString()));
                if (!viewmodel.IsRepeater)
                {
                    string pbpData = string.Empty;
                    if (viewmodel.IsCustomRule == false)
                        pbpData = GetDataFromPBP(viewmodel.PBPTableName, viewmodel.PBPFieldName, true);
                    else
                        pbpData = ApplyCustomRuleGetData(viewmodel.PBPTableName, viewmodel.PBPFieldName, viewmodel.CustomRuleTypeId, viewmodel.PositionForOne);

                    if (!string.IsNullOrEmpty(pbpData) && viewmodel.FieldName == "IndicatenumberofTiersinyourPartDbenefit")
                    {
                        if (!int.TryParse(pbpData, out tiercount))
                            tiercount = 0;
                    }
                    if (!string.IsNullOrEmpty(pbpData) && viewmodel.FieldName == ELEMENTNAME.Retailcostsharing && viewmodel.FieldPath == FIELDPATH.AdditionalPrescriptionCostShareInformation)
                    {
                        bool populate = false;
                        if (bool.TryParse(pbpData, out populate))
                        {
                            JArray j = PopulateRepeater(tierdescription, repeatermaps, 1, REPEATERNAME.StandardRetailCostSharingInformation, ref prescriptionLog);
                            json.SelectToken(viewmodel.FieldPath)[REPEATERNAME.StandardRetailCostSharingInformation] = j;
                        }
                    }
                    if (!string.IsNullOrEmpty(pbpData) && viewmodel.FieldName == ELEMENTNAME.Mailordercostsharing && viewmodel.FieldPath == FIELDPATH.AdditionalPrescriptionCostShareInformation)
                    {
                        bool populate = false;
                        if (bool.TryParse(pbpData, out populate))
                        {
                            JArray j = PopulateRepeater(tierdescription, repeatermaps, 1, REPEATERNAME.StandardMailOrderCostSharing, ref prescriptionLog);
                            json.SelectToken(viewmodel.FieldPath)[REPEATERNAME.StandardMailOrderCostSharing    ] = j;
                        }
                        else
                            json.SelectToken(viewmodel.FieldPath)[REPEATERNAME.StandardMailOrderCostSharing] = new JArray();

                    }
                    if (!string.IsNullOrEmpty(pbpData) && viewmodel.FieldName == ELEMENTNAME.Longtermcarecostsharing && viewmodel.FieldPath == FIELDPATH.AdditionalPrescriptionCostShareInformation)
                    {
                        bool populate = false;
                        if (bool.TryParse(pbpData, out populate))
                        {
                            JArray j = PopulateRepeater(tierdescription, repeatermaps, 1, REPEATERNAME.LongtermcareLTCcostsharingList, ref prescriptionLog);
                            json.SelectToken(viewmodel.FieldPath)[REPEATERNAME.LongtermcareLTCcostsharingList] = j;
                        }
                        else
                            json.SelectToken(viewmodel.FieldPath)[REPEATERNAME.LongtermcareLTCcostsharingList] = new JArray();
                    }
                    if (!string.IsNullOrEmpty(pbpData) && viewmodel.FieldName == ELEMENTNAME.OptionOutofnetworkcostsharing && viewmodel.FieldPath == FIELDPATH.AdditionalPrescriptionCostShareInformation)
                    {
                        bool populate = false;
                        if (bool.TryParse(pbpData, out populate))
                        {
                            JArray j = PopulateRepeater(tierdescription, repeatermaps, 1, REPEATERNAME.Outofnetworkcostsharing, ref prescriptionLog);
                            json.SelectToken(viewmodel.FieldPath)[REPEATERNAME.Outofnetworkcostsharing] = j;
                        }
                        else
                            json.SelectToken(viewmodel.FieldPath)[REPEATERNAME.Outofnetworkcostsharing] = new JArray();
                    }
                    if (!string.IsNullOrEmpty(pbpData) && viewmodel.FieldName == ELEMENTNAME.Retailcostsharing && viewmodel.FieldPath == FIELDPATH.GapCoverageInformation)
                    {
                        bool populate = false;
                        if (bool.TryParse(pbpData, out populate))
                        {
                            JArray j = PopulateRepeater(tierdescription, repeatermaps, 3, REPEATERNAME.StandardRetailCostSharingInformation, ref prescriptionLog);
                            json.SelectToken(viewmodel.FieldPath)[REPEATERNAME.StandardRetailCostSharingInformation] = j;
                        }
                        else
                            json.SelectToken(viewmodel.FieldPath)[REPEATERNAME.StandardRetailCostSharingInformation] = new JArray();

                    }
                    if (!string.IsNullOrEmpty(pbpData) && viewmodel.FieldName == ELEMENTNAME.Mailordercostsharing && viewmodel.FieldPath == FIELDPATH.GapCoverageInformation)
                    {
                        bool populate = false;
                        if (bool.TryParse(pbpData, out populate))
                        {
                            JArray j = PopulateRepeater(tierdescription, repeatermaps, 3, REPEATERNAME.StandardMailOrderCostSharing, ref prescriptionLog);
                            json.SelectToken(viewmodel.FieldPath)[REPEATERNAME.StandardMailOrderCostSharing] = j;
                        }
                    }

                    json.SelectToken(viewmodel.FieldPath)[viewmodel.FieldName] = pbpData;
                    if (!string.IsNullOrEmpty(pbpData))
                    {
                        bool isbooldata = false;
                        if(bool.TryParse(pbpData, out isbooldata))
                            json.SelectToken(viewmodel.FieldPath)[viewmodel.FieldName] = isbooldata;
                    }
                    
                }
                else
                {
                    if (viewmodel.FieldName == ELEMENTNAME.CostShareTiersInformation)
                    {
                        JArray jarr = new JArray();
                        string pbpData = GetDataFromPBP(viewmodel.PBPTableName, viewmodel.PBPFieldName);
                        if (!string.IsNullOrEmpty(pbpData))
                        {
                            string[] tierdes = pbpData.Split(',');
                            for (int i = 0; i < tiercount; i++)
                                tierdescription[i + 1] = tierdes[i].Trim();
                        }

                        if (tierdescription.Count > 0)
                        {
                            foreach (KeyValuePair<int, string> k in tierdescription)
                            {
                                JObject joj = new JObject();
                                joj[ELEMENTNAME.Tiers] = k.Key.ToString();
                                joj[ELEMENTNAME.TierDescription] = k.Value;

                                jarr.Add(joj);
                            }
                        }
                        json.SelectToken(viewmodel.FieldPath)[viewmodel.FieldName] = jarr;
                    }
                    else if (viewmodel.FieldName == ELEMENTNAME.Listeachtierforwhichthedeductibleapplies)
                    {
                        JArray jarr = new JArray();
                        string pbpData = GetDataFromPBP(viewmodel.PBPTableName, viewmodel.PBPFieldName);
                        if (!string.IsNullOrEmpty(pbpData))
                        {
                            if (tierdescription.Count > 0)
                            {
                                foreach (KeyValuePair<int, string> k in tierdescription)
                                {
                                    bool isapplicable = false;
                                    if (k.Key == 1)
                                        isapplicable = !Convert.ToBoolean(GetValueAgainstIndex(pbpData, 2));
                                    if (k.Key == 2)
                                        isapplicable = !Convert.ToBoolean(GetValueAgainstIndex(pbpData, 3));
                                    if (k.Key == 3)
                                        isapplicable = !Convert.ToBoolean(GetValueAgainstIndex(pbpData, 4));
                                    if (k.Key == 4)
                                        isapplicable = !Convert.ToBoolean(GetValueAgainstIndex(pbpData, 5));
                                    if (k.Key == 5)
                                        isapplicable = !Convert.ToBoolean(GetValueAgainstIndex(pbpData, 6));
                                    if (k.Key == 6)
                                        isapplicable = !Convert.ToBoolean(GetValueAgainstIndex(pbpData, 1));

                                    if (isapplicable)
                                    {
                                        JObject joj = new JObject();
                                        joj[ELEMENTNAME.Tiers] = k.Key.ToString();
                                        joj[ELEMENTNAME.TierDescription] = k.Value;

                                        jarr.Add(joj);
                                    }
                                }
                            }
                        }
                        json.SelectToken(viewmodel.FieldPath)[viewmodel.FieldName] = jarr;
                    }
                    prescriptionLog.Add(this.GetLogEntity("     " + viewmodel.FieldName + ": TierDescription Count= " + tierdescription.Count));
                }
            }
        }

        private JArray PopulateRepeater(Dictionary<int, string> tierdescription, List<PrescriptionRepeaterViewModel> repeatermaps, int typeid, string repeaterName, ref List<PBPImportActivityLogViewModel> populateRepeaterlog)
        {
            JArray jarr = new JArray();
            populateRepeaterlog.Add(this.GetLogEntity("--> Populating " + repeaterName + " repeater"));
            PrescriptionRepeaterViewModel viewmodel = repeatermaps.Where(r => r.TypeID == 1 && r.RepeaterName == repeaterName).FirstOrDefault();
            if (viewmodel != null)
            {
                List<PBPDataMapViewModel> dataModelList = _pbpDataModelList.Where(d => d.QID == _pbpQId && d.TableName == viewmodel.TiersPBPTableName && d.PBPImportBatchID == _pbpBatchId).ToList();

                foreach (PBPDataMapViewModel model in dataModelList)
                {
                    string tid = GetDataFromPBP(viewmodel.TiersPBPTableName, "MRX_TIER_TYPE_ID", model);
                    if (tid == typeid.ToString())
                    {
                        JObject j = new JObject();
                        string tier = GetDataFromPBP(viewmodel.TiersPBPTableName, viewmodel.TiersPBPFieldName, model);

                        int tierid = 0;
                        if (!string.IsNullOrEmpty(tier) && int.TryParse(tier, out tierid))
                        {
                            string desc = tierdescription[tierid];

                            j[viewmodel.TiersFieldName] = tier;
                            j[viewmodel.DescriptionFieldName] = desc;

                            string onemonth = HelperUtility.GetDollerValue(GetDataFromPBP(viewmodel.OnemonthsupplyPBPTableName, viewmodel.OnemonthsupplyPBPFieldName, model, true));
                            if (string.IsNullOrEmpty(onemonth))
                                onemonth = HelperUtility.GetPercentageValue(GetDataFromPBP(viewmodel.OnemonthsupplyConiPBPTableName, viewmodel.OnemonthsupplyConiPBPFieldName, model));
                            j[viewmodel.OnemonthsupplyName] = string.IsNullOrEmpty(onemonth) ? STANDARDVALUE.NotApplicable : onemonth;

                            string twomonth = HelperUtility.GetDollerValue(GetDataFromPBP(viewmodel.TwomonthsupplyPBPTableName, viewmodel.TwomonthsupplyPBPFieldName, model, true));
                            if (string.IsNullOrEmpty(onemonth))
                                twomonth = HelperUtility.GetPercentageValue(GetDataFromPBP(viewmodel.TwomonthsupplyConiPBPTableName, viewmodel.TwomonthsupplyConiPBPFieldName, model));
                            j[viewmodel.TwomonthsupplyName] = string.IsNullOrEmpty(twomonth) ? STANDARDVALUE.NotApplicable : twomonth;

                            string threemonth = HelperUtility.GetDollerValue(GetDataFromPBP(viewmodel.ThreemonthsupplyPBPTableName, viewmodel.ThreemonthsupplyPBPFieldName, model, true));
                            if (string.IsNullOrEmpty(threemonth))
                                threemonth = HelperUtility.GetPercentageValue(GetDataFromPBP(viewmodel.ThreemonthsupplyConiPBPTableName, viewmodel.ThreemonthsupplyConiPBPFieldName, model));
                            j[viewmodel.ThreemonthsupplyName] = string.IsNullOrEmpty(threemonth) ? STANDARDVALUE.NotApplicable : threemonth;

                            string drugs = string.Empty;
                            if (!string.IsNullOrEmpty(viewmodel.DrugsCoveredName))
                            {
                                drugs = GetDataFromPBP(viewmodel.DrugsCoveredPBPTableName, viewmodel.DrugsCoveredPBPFieldName, model);
                                if (string.IsNullOrEmpty(drugs))
                                    drugs = GetDataFromPBP(viewmodel.DrugsCoveredPBPTableName, viewmodel.DrugsCoveredPBPFieldName, model);
                                j[viewmodel.DrugsCoveredName] = string.IsNullOrEmpty(drugs) ? STANDARDVALUE.NotApplicable : drugs;
                            }
                            populateRepeaterlog.Add(this.GetLogEntity("     OneMonthSupply=" + onemonth + " TwoMonthSupply=" + twomonth + " ThreeMonthSupply=" + threemonth + " DrugsCovered=" + drugs));
                            jarr.Add(j);
                        }
                    }
                }
            }
            populateRepeaterlog.Add(this.GetLogEntity("<-- Populating " + repeaterName + " repeater"));
            return jarr;
        }
        #endregion Prescription

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
                FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(1,  forminstance.FormDesignVersionID, _formDesignService);
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
                if(_defaultServices != null)
                    foreach (CostShareDefaultServices c in _defaultServices)
                        if (c.BenefitCategory1 == viewmodel.BenefitCategory1.Trim() && c.BenefitCategory2 == viewmodel.BenefitCategory2.Trim()
                            && c.BenefitCategory3 == viewmodel.BenefitCategory3.Trim())
                        {
                            isselectedincostshare = true;
                            break;
                        }

                if (viewmodel.ByDefaultSelection || isselectedincostshare)
                    pbpData = true.ToString();
                else if (viewmodel.SequenceNumberforOne != 0)
                    pbpData = ApplyCustomRuleGetData(viewmodel.PBPTableName, viewmodel.PBPFieldName, viewmodel.CustomRuleTypeId, viewmodel.SequenceNumberforOne);
                else if (!string.IsNullOrEmpty(viewmodel.ValueToMatch))
                {
                    string code = GetDataFromPBP(viewmodel.PBPTableName, viewmodel.PBPFieldName);

                    if (viewmodel.IsCustomRule && !string.IsNullOrEmpty(code))
                        pbpData = HelperUtility.CheckIfServiceCodeExists(code, viewmodel.ValueToMatch, ';').ToString();
                    else if (code == viewmodel.ValueToMatch)
                        pbpData = true.ToString();
                }
                else
                {
                    string code = GetDataFromPBP(viewmodel.PBPTableName, viewmodel.PBPFieldName);
                    if(!string.IsNullOrEmpty(code))
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
                        if(isselectedincostshare)
                            jobj[ELEMENTNAME.SlidingCostShare] = STANDARDVALUE.Yes;
                        else
                            jobj[ELEMENTNAME.SlidingCostShare] = STANDARDVALUE.No;

                        preAuthorization = GetPreAuthorization(authmapping, viewmodel.BenefitReviewID);
                        jobj[ELEMENTNAME.PreAuthorization] = preAuthorization;

                        benefirReviewLog.Add(this.GetLogEntity("--> Service selected: BC1 = " + viewmodel.BenefitCategory1.Trim() + ", BC2 = " + viewmodel.BenefitCategory2.Trim() + ", BC3 = " + viewmodel.BenefitCategory3.Trim() + " ,PreAuthorization= " + preAuthorization));

                        JArray jnetworklist = new JArray();
                        if (jobj[DATASOURCENAME.IQMedicareNetworkList] != null)
                            jnetworklist = (JArray)jobj[DATASOURCENAME.IQMedicareNetworkList];
                        jobj[DATASOURCENAME.IQMedicareNetworkList] = GetBenefitReviewAmount(jnetworklist, benefitamountmapping, viewmodel, oonmappings, ref benefirReviewLog);
                        benefirReviewLog.Add(this.GetLogEntity("<-- Service selected: BC1 = " + viewmodel.BenefitCategory1.Trim() + ", BC2 = " + viewmodel.BenefitCategory2.Trim() + ", BC3 = " + viewmodel.BenefitCategory3.Trim() + " ,PreAuthorization= " + preAuthorization));
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
                string authmap = ApplyCustomRuleGetData(viewmodel.PBPTableName, viewmodel.PBPFieldName, viewmodel.CustomRuleTypeId, viewmodel.SequenceNumberforOne);

                bool isServiceSelected = false;

                if (!string.IsNullOrEmpty(authmap) && bool.TryParse(authmap, out isServiceSelected))
                    return (!isServiceSelected).ToString();
                else
                    return false.ToString();
            }
            else if (viewmodel.CustomRuleTypeId == 2)
            {
                string authmap = ApplyCustomRuleGetData(viewmodel.PBPTableName, viewmodel.PBPFieldName, viewmodel.CustomRuleTypeId, viewmodel.SequenceNumberforOne);
                return authmap;
            }
            else
                return false.ToString();
        }

        private JArray GetBenefitReviewAmount(JArray jnetworklist, List<BenefitReviewAmountViewModel> benefitamountmapping, BenefitReviewViewModel benviewmodel, List<BenefitReviewOONViewModel> oonmappings, ref List<PBPImportActivityLogViewModel> BRGAmountLog)
        {
            int benefitReviewID = benviewmodel.BenefitReviewID;
            List<BenefitReviewAmountViewModel> amountlist = new List<BenefitReviewAmountViewModel>();
            if (_isOONApplicable)
                amountlist = benefitamountmapping.Where(b => b.BenefitReviewID == benefitReviewID).OrderBy(n => n.NetworkType).ToList();
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
                BRGAmountLog.Add(this.GetLogEntity("    NetworkType=" + viewmodel.NetworkType));
                if (viewmodel.NetworkType == DATA.InNetwork)
                {
                    if (!string.IsNullOrEmpty(viewmodel.DeductiblePBPTableName) && !string.IsNullOrEmpty(viewmodel.DeductiblePBPFieldName))
                    {
                        string deductible = GetDataFromPBP(viewmodel.DeductiblePBPTableName, viewmodel.DeductiblePBPFieldName, true);
                        jobj[viewmodel.DeductiblePath] = string.IsNullOrEmpty(deductible) ? STANDARDVALUE.NotApplicable : deductible;
                    }
                    else
                        jobj[viewmodel.DeductiblePath] = jobj[viewmodel.DeductiblePath] != null ? jobj[viewmodel.DeductiblePath].ToString() : STANDARDVALUE.NotApplicable;

                    if (!string.IsNullOrEmpty(viewmodel.MinimumCopayPBPTableName) && !string.IsNullOrEmpty(viewmodel.MinimumCopayPBPFieldName))
                    {
                        string minimumCopay = GetDataFromPBP(viewmodel.MinimumCopayPBPTableName, viewmodel.MinimumCopayPBPFieldName, true);
                        jobj[viewmodel.MinimumCopayPath] = string.IsNullOrEmpty(minimumCopay) ? STANDARDVALUE.NotApplicable : minimumCopay;
                    }
                    else
                        jobj[viewmodel.MinimumCopayPath] = jobj[viewmodel.MinimumCopayPath] != null ? jobj[viewmodel.MinimumCopayPath].ToString() : STANDARDVALUE.NotApplicable;

                    if (!string.IsNullOrEmpty(viewmodel.MaximumCopayPBPTableName) && !string.IsNullOrEmpty(viewmodel.MaximumCopayPBPFieldName))
                    {
                        string maximumCopay = GetDataFromPBP(viewmodel.MaximumCopayPBPTableName, viewmodel.MaximumCopayPBPFieldName, true);
                        jobj[viewmodel.MaximumCopayPath] = string.IsNullOrEmpty(maximumCopay) ? STANDARDVALUE.NotApplicable : maximumCopay;
                    }
                    else
                        jobj[viewmodel.MaximumCopayPath] = jobj[viewmodel.MaximumCopayPath] != null ? jobj[viewmodel.MaximumCopayPath] : STANDARDVALUE.NotApplicable;

                    if (!string.IsNullOrEmpty(viewmodel.MinimumCoissurancePBPTableName) && !string.IsNullOrEmpty(viewmodel.MinimumCoissurancePBPFieldName))
                    {
                        string minimumCoissurance = GetDataFromPBP(viewmodel.MinimumCoissurancePBPTableName, viewmodel.MinimumCoissurancePBPFieldName, true);
                        jobj[viewmodel.MinimumCoissurancePath] = string.IsNullOrEmpty(minimumCoissurance) ? STANDARDVALUE.NotApplicable : minimumCoissurance;
                    }
                    else
                        jobj[viewmodel.MinimumCoissurancePath] = jobj[viewmodel.MinimumCoissurancePath] != null ? jobj[viewmodel.MinimumCoissurancePath] : STANDARDVALUE.NotApplicable;

                    if (!string.IsNullOrEmpty(viewmodel.MaximumCoissurancePBPTableName) && !string.IsNullOrEmpty(viewmodel.MaximumCoissurancePBPFieldName))
                    {
                        string maximumCoissurance = GetDataFromPBP(viewmodel.MaximumCoissurancePBPTableName, viewmodel.MaximumCoissurancePBPFieldName, true);
                        jobj[viewmodel.MaximumCoissurancePath] = string.IsNullOrEmpty(maximumCoissurance) ? STANDARDVALUE.NotApplicable : maximumCoissurance;
                    }
                    else
                        jobj[viewmodel.MaximumCoissurancePath] = jobj[viewmodel.MaximumCoissurancePath] != null ? jobj[viewmodel.MaximumCoissurancePath] : STANDARDVALUE.NotApplicable;

                    if (!string.IsNullOrEmpty(viewmodel.OOPMValuePBPTableName) && !string.IsNullOrEmpty(viewmodel.OOPMValuePBPFieldName))
                    {
                        string oopm = GetDataFromPBP(viewmodel.OOPMValuePBPTableName, viewmodel.OOPMValuePBPFieldName, true);
                        jobj[viewmodel.OOPMValuePath] = string.IsNullOrEmpty(oopm) ? STANDARDVALUE.NotApplicable : oopm;
                    }
                    else
                        jobj[viewmodel.OOPMValuePath] = jobj[viewmodel.OOPMValuePath] != null ? jobj[viewmodel.OOPMValuePath] : STANDARDVALUE.NotApplicable;
                    if (!string.IsNullOrEmpty(viewmodel.OOPMPeriodicityPBPTableName) && !string.IsNullOrEmpty(viewmodel.OOPMPeriodicityPBPFieldName))
                    {
                        string oopmperiod = HelperUtility.GetMaximumPlanBenefitCoveragePeriodicity(GetDataFromPBP(viewmodel.OOPMPeriodicityPBPTableName, viewmodel.OOPMPeriodicityPBPFieldName));
                        jobj[viewmodel.OOPMPeriodicityPath] = string.IsNullOrEmpty(oopmperiod) ? STANDARDVALUE.NotApplicable : oopmperiod;
                    }
                    else
                        jobj[viewmodel.OOPMPeriodicityPath] = jobj[viewmodel.OOPMPeriodicityPath] != null ? jobj[viewmodel.OOPMPeriodicityPath].ToString() : STANDARDVALUE.NotApplicable;

                    if (!string.IsNullOrEmpty(viewmodel.MaximumPlanBenefitCoverageAmountPBPTableName) && !string.IsNullOrEmpty(viewmodel.MaximumPlanBenefitCoverageAmountPBPFieldName))
                    {
                        string maxcov = GetDataFromPBP(viewmodel.MaximumPlanBenefitCoverageAmountPBPTableName, viewmodel.MaximumPlanBenefitCoverageAmountPBPFieldName, true);
                        jobj[viewmodel.MaximumPlanBenefitCoverageAmountPath] = string.IsNullOrEmpty(maxcov) ? STANDARDVALUE.NotApplicable : maxcov;
                    }
                    else
                        jobj[viewmodel.MaximumPlanBenefitCoverageAmountPath] = jobj[viewmodel.MaximumPlanBenefitCoverageAmountPath] != null ? jobj[viewmodel.MaximumPlanBenefitCoverageAmountPath].ToString() : STANDARDVALUE.NotApplicable;

                    if (!string.IsNullOrEmpty(viewmodel.MaximumPlanBenefitCoveragePeriodicityPBPTableName) && !string.IsNullOrEmpty(viewmodel.MaximumPlanBenefitCoveragePeriodicityPBPFieldName))
                    {
                        string maxcovperiod = HelperUtility.GetMaximumPlanBenefitCoveragePeriodicity(GetDataFromPBP(viewmodel.MaximumPlanBenefitCoveragePeriodicityPBPTableName, viewmodel.MaximumPlanBenefitCoveragePeriodicityPBPFieldName));
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
                        deductible = GetDataFromPBP(viewmodel.DeductiblePBPTableName, viewmodel.DeductiblePBPFieldName, datamodel, true);
                        minimumCopay = GetDataFromPBP(viewmodel.MinimumCopayPBPTableName, viewmodel.MinimumCopayPBPFieldName, datamodel, true);
                        maximumCopay = GetDataFromPBP(viewmodel.MaximumCopayPBPTableName, viewmodel.MaximumCopayPBPFieldName, datamodel, true);
                        minimumCoissurance = GetDataFromPBP(viewmodel.MinimumCoissurancePBPTableName, viewmodel.MinimumCoissurancePBPFieldName, datamodel, true);
                        maximumCoissurance = GetDataFromPBP(viewmodel.MaximumCoissurancePBPTableName, viewmodel.MaximumCoissurancePBPFieldName, datamodel, true);
                        oopm = GetDataFromPBP(viewmodel.OOPMValuePBPTableName, viewmodel.OOPMValuePBPFieldName, datamodel, true);
                        oopmperiod = HelperUtility.GetMaximumPlanBenefitCoveragePeriodicity(GetDataFromPBP(viewmodel.OOPMPeriodicityPBPTableName, viewmodel.OOPMPeriodicityPBPFieldName, datamodel));
                        maxcov = GetDataFromPBP(viewmodel.MaximumPlanBenefitCoverageAmountPBPTableName, viewmodel.MaximumPlanBenefitCoverageAmountPBPFieldName, datamodel, true);
                        maxcovperiod = HelperUtility.GetMaximumPlanBenefitCoveragePeriodicity(GetDataFromPBP(viewmodel.MaximumPlanBenefitCoveragePeriodicityPBPTableName, viewmodel.MaximumPlanBenefitCoveragePeriodicityPBPFieldName, datamodel));
                        BRGAmountLog.Add(this.GetLogEntity("   Deductible= " + deductible + " Min Copay= " + minimumCopay + " MaxCopay= " + maximumCopay + " Min Coissurance= " + minimumCoissurance + " Max Coissurance=" + maximumCoissurance + " OOPM= " + oopm + " OOPMPeriodicity= " + oopmperiod + " MaximumPlanBenefitCoverage= " + maxcov + " MaximumPlanBenefitCoveragePeriodicity= " + maxcovperiod));
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

            string codelist = GetDataFromPBP(benviewmodel.OONPBPTableName, benviewmodel.OONPBPFieldName);

            if (oon == null || string.IsNullOrEmpty(codelist))
                return isapplicable;

            isapplicable = HelperUtility.CheckIfServiceCodeExists(codelist, oon.Code, ';');

            if (!isapplicable)
            {
                string code = GetDataFromPBP(benviewmodel.OONPBPTableName, "PBP_C_POS_OUTPT_NMC_BENCATS");
                isapplicable = HelperUtility.CheckIfServiceCodeExists(code, oon.Code, ';');
            }

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
                bool isapplicable = false;
                foreach (PBPDataMapViewModel datamodel in dataModelList)
                {
                    JObject jobj = JObject.Parse(datamodel.JsonData);
                    if (!string.IsNullOrEmpty(jobj[benviewmodel.OONPBPFieldName].ToString()))
                    {
                        string codelist = jobj[benviewmodel.OONPBPFieldName].ToString();
                        isapplicable = HelperUtility.CheckIfServiceCodeExists(codelist, oon.Code, ';');
                        if (isapplicable)
                            return datamodel;
                    }
                }
                if (!isapplicable)
                {
                    foreach (PBPDataMapViewModel datamodel in dataModelList)
                    {
                        JObject jobj = JObject.Parse(datamodel.JsonData);
                        if (!string.IsNullOrEmpty(jobj["PBP_C_POS_OUTPT_NMC_BENCATS"].ToString()))
                        {
                            string codelist = jobj["PBP_C_POS_OUTPT_NMC_BENCATS"].ToString();
                            isapplicable = HelperUtility.CheckIfServiceCodeExists(codelist, oon.Code, ';');
                            if (isapplicable)
                                return datamodel;
                        }
                    }
                }
            }

            return null;
        }

        #endregion BenefitReview

        #region CostShare
        private void PopulateCostShare(JObject json, ref List<PBPImportActivityLogViewModel> costSharelog)
        {
            PBPImportActivityLogViewModel log = new PBPImportActivityLogViewModel();
            List<CostShareViewModel> costsharemapping = _pbpImportServices.GetCostShareMapping();
            List<CostShareAmountViewModel> costshareamountmapping = _pbpImportServices.GetCostShareAmountMapping();            
            string FieldPath = costsharemapping[0].FieldPath;
            string FieldName = costsharemapping[0].FieldName;
            JArray jarry = new JArray();
            if (json.SelectToken(FieldPath)[FieldName] != null && json.SelectToken(FieldPath)[FieldName] is JArray && ((JArray)json.SelectToken(FieldPath)[FieldName]).Count > 0)
                jarry = (JArray)json.SelectToken(FieldPath)[FieldName];

            JArray jInfoArray = new JArray();
            if (json.SelectToken(FieldPath)[REPEATERNAME.SlidingCostShareInformation] != null && json.SelectToken(FieldPath)[REPEATERNAME.SlidingCostShareInformation] is JArray && ((JArray)json.SelectToken(FieldPath)[REPEATERNAME.SlidingCostShareInformation]).Count > 0)
                jInfoArray = (JArray)json.SelectToken(FieldPath)[REPEATERNAME.SlidingCostShareInformation];

            JToken job = jarry.Where(c => c[KEYNAME.BenefitCategory1].ToString() == string.Empty && c[KEYNAME.BenefitCategory2].ToString() == string.Empty && c[KEYNAME.BenefitCategory3].ToString() == string.Empty).FirstOrDefault();
            if (job != null)
                jarry.Remove(job);

            foreach (CostShareViewModel viewmodel in costsharemapping)
            {
                JToken jobj = jarry.Where(c => c[KEYNAME.BenefitCategory1].ToString() == viewmodel.BenefitCategory1.Trim() && c[KEYNAME.BenefitCategory2].ToString() == viewmodel.BenefitCategory2.Trim() && c[KEYNAME.BenefitCategory3].ToString() == viewmodel.BenefitCategory3.Trim()).FirstOrDefault();
                string pbpData = string.Empty;

                FieldPath = viewmodel.FieldPath;
                FieldName = viewmodel.FieldName;

                if (viewmodel.IsDefaultSelected == true)
                    pbpData = true.ToString();
                else
                {
                    //Service Selection - All the service selections have the custom rule                
                    pbpData = ApplyCustomRuleGetData(viewmodel.PBPTableName, viewmodel.PBPFieldName, viewmodel.CustomRuleTypeId, viewmodel.SequenceNumberforOne);
                }

                bool isServiceSelected = false;
                if (bool.TryParse(pbpData, out isServiceSelected))
                {
                    if (isServiceSelected)
                    {   
                        if (jobj == null)
                        {
                            jobj = new JObject();
                            jarry.Add(jobj);
                        }

                        CostShareDefaultServices c = new CostShareDefaultServices();
                        c.BenefitCategory1 = viewmodel.BenefitCategory1.Trim();
                        c.BenefitCategory2 = viewmodel.BenefitCategory2.Trim();
                        c.BenefitCategory3 = viewmodel.BenefitCategory3.Trim();
                        _defaultServices.Add(c);

                        jobj[KEYNAME.BenefitCategory1] = viewmodel.BenefitCategory1.Trim();
                        jobj[KEYNAME.BenefitCategory2] = viewmodel.BenefitCategory2.Trim();
                        jobj[KEYNAME.BenefitCategory3] = viewmodel.BenefitCategory3.Trim();

                        costSharelog.Add(this.GetLogEntity("--> Service selected: BC1 = " + viewmodel.BenefitCategory1.Trim() + ", BC2 = " + viewmodel.BenefitCategory2.Trim() + ", BC3 = " + viewmodel.BenefitCategory3.Trim()));

                        if (!string.IsNullOrEmpty(viewmodel.BenefitPeriodPBPTableName) && !string.IsNullOrEmpty(viewmodel.BenefitPeriodPBPFieldName))
                        {
                            string beperiod = GetDataFromPBP(viewmodel.BenefitPeriodPBPTableName, viewmodel.BenefitPeriodPBPFieldName);
                            jobj[ELEMENTNAME.BenefitPeriod] = HelperUtility.GetBenefitperiod(beperiod);
                            costSharelog.Add(this.GetLogEntity("    BenefitPeriod = " + beperiod));
                        }

                        string isbenunlimited = ApplyCustomRuleGetData(viewmodel.IsthisBenefitUnlimitedPBPTableName, viewmodel.IsthisBenefitUnlimitedPBPFieldName, 2, 0);
                        if (!string.IsNullOrEmpty(isbenunlimited))
                            jobj[ELEMENTNAME.IsthisBenefitUnlimited] = Convert.ToBoolean(isbenunlimited) == true ? STANDARDVALUE.Yes : STANDARDVALUE.No;
                        else
                            jobj[ELEMENTNAME.IsthisBenefitUnlimited] = STANDARDVALUE.NotApplicable;

                        costSharelog.Add(this.GetLogEntity("    IsthisBenefitUnlimited = " + isbenunlimited));

                        int intervalnumber = -999;
                        int intervaloonnumber = -999;

                        if ((!string.IsNullOrEmpty(viewmodel.IntervalPBPTableName) && !string.IsNullOrEmpty(viewmodel.IntervalOONPBPTableName)) || 
                            (!string.IsNullOrEmpty(viewmodel.IntervalCopayPBPTableName) && !string.IsNullOrEmpty(viewmodel.IntervalCopayPBPFieldName)) ||
                            (!string.IsNullOrEmpty(viewmodel.IntervalOONPBPTableName) && !string.IsNullOrEmpty(viewmodel.IntervalOONPBPFieldName)))
                        {
                            string interval = GetDataFromPBP(viewmodel.IntervalPBPTableName, viewmodel.IntervalPBPFieldName);
                            string intervalno = HelperUtility.GetIntervalNumber(interval);

                            costSharelog.Add(this.GetLogEntity("    Interval Number In Network = " + intervalno));

                            if (!int.TryParse(intervalno, out intervalnumber))
                                intervalnumber = -1;

                            if (intervalnumber < 0)
                            {
                                interval = GetDataFromPBP(viewmodel.IntervalCopayPBPTableName, viewmodel.IntervalCopayPBPFieldName);
                                intervalno = HelperUtility.GetIntervalNumber(interval);

                                costSharelog.Add(this.GetLogEntity("    Interval Number Copay In Network = " + intervalno));

                                if (!int.TryParse(intervalno, out intervalnumber))
                                    intervalnumber = -1;
                            }

                            if (_isOONApplicable)
                            {
                                string intervaloon = GetDataFromPBP(viewmodel.IntervalOONPBPTableName, viewmodel.IntervalOONPBPFieldName);
                                string intervaloonno = HelperUtility.GetIntervalNumber(intervaloon);

                                costSharelog.Add(this.GetLogEntity("    Interval Number Out Of Network =" + intervaloonno));

                                if (!int.TryParse(intervaloonno, out intervaloonnumber))
                                    intervaloonnumber = -1;

                                if (intervaloonnumber < 0)
                                {
                                    intervaloon = GetDataFromPBP(viewmodel.IntervalOONCopayPBPTableName, viewmodel.IntervalOONCopayPBPFieldName);
                                    intervaloonno = HelperUtility.GetIntervalNumber(intervaloon);

                                    costSharelog.Add(this.GetLogEntity("    Interval Number Out Of Network =" + intervaloonno));

                                    if (!int.TryParse(intervalno, out intervaloonnumber))
                                        intervaloonnumber = -1;
                                }
                            }

                            if (intervaloonnumber < 0 && intervalnumber < 0)
                                jobj[viewmodel.IntervalFieldName] = STANDARDVALUE.NotApplicable;
                            else
                            {
                                int actualinterval = 0;
                                if (intervalnumber < intervaloonnumber)
                                    actualinterval = intervaloonnumber;
                                else if (intervalnumber >= intervaloonnumber)
                                    actualinterval = intervalnumber;

                                jobj[viewmodel.IntervalFieldName] = actualinterval.ToString();

                                PopulateCostShareInformation(jInfoArray, actualinterval, intervalnumber, intervaloonnumber, viewmodel, costshareamountmapping, ref costSharelog);
                            }
                        }
                        costSharelog.Add(this.GetLogEntity("<-- Service selected: BC1 = " + viewmodel.BenefitCategory1.Trim() + ", BC2 = " + viewmodel.BenefitCategory2.Trim() + ", BC3 = " + viewmodel.BenefitCategory3.Trim()));
                    }
                    else
                    {
                        if (jobj != null)
                        {
                            if (json.SelectToken(FieldPath)[REPEATERNAME.SlidingCostShareInformation] != null && json.SelectToken(FieldPath)[REPEATERNAME.SlidingCostShareInformation] is JArray && ((JArray)json.SelectToken(FieldPath)[REPEATERNAME.SlidingCostShareInformation]).Count > 0)
                            {
                                List<JToken> ja = jInfoArray.Where(j => j[KEYNAME.BenefitCategory1] == jobj[KEYNAME.BenefitCategory1] && j[KEYNAME.BenefitCategory2] == jobj[KEYNAME.BenefitCategory2]
                                    && j[KEYNAME.BenefitCategory3] == jobj[KEYNAME.BenefitCategory3]).ToList();

                                foreach (JObject j in ja)
                                    jInfoArray.Remove(j);
                            }
                            jarry.Remove(jobj);
                        }
                    }
                }
            }
            json.SelectToken(FieldPath)[FieldName] = jarry;

            if (json.SelectToken(FieldPath)[REPEATERNAME.SlidingCostShareInformation] != null && json.SelectToken(FieldPath)[REPEATERNAME.SlidingCostShareInformation] is JArray)
            {
                JArray j = (JArray)json.SelectToken(FieldPath)[REPEATERNAME.SlidingCostShareInformation];
                job = j.Where(c => c[KEYNAME.BenefitCategory1].ToString() == string.Empty && c[KEYNAME.BenefitCategory2].ToString() == string.Empty && c[KEYNAME.BenefitCategory3].ToString() == string.Empty).FirstOrDefault();
                if (job != null)
                    j.Remove(job);
            }

            json.SelectToken(FieldPath)[REPEATERNAME.SlidingCostShareInformation] = jInfoArray;
            json.SelectToken(SECTIONNAME.CostShare)[REPEATERNAME.GeneralCostShareInformation] = PopulateGeneralCostShare();
        }

        private void PopulateCostShareInformation(JArray jInfoArray, int intervalnumber, int intervalinnnumber, int intervaloonnumber, CostShareViewModel viewmodel, List<CostShareAmountViewModel> costshareamountmapping, ref List<PBPImportActivityLogViewModel> costSharelog)
        {
            List<CostShareAmountViewModel> mappings = new List<CostShareAmountViewModel>();
            bool iszerointerval = false;
            PBPImportActivityLogViewModel log = new PBPImportActivityLogViewModel();
            if (_isOONApplicable)
            {
                if (intervalnumber > 0)
                {
                    mappings = costshareamountmapping.Where(c => c.CostShareID == viewmodel.CostShareID && c.IntervalNumber <= intervalnumber && c.IntervalNumber != 0).ToList();
                    if (intervalinnnumber == 0 || intervaloonnumber == 0)
                    {
                        mappings.AddRange(costshareamountmapping.Where(c => c.CostShareID == viewmodel.CostShareID && c.IntervalNumber == 0).ToList());
                        iszerointerval = true;
                    }
                }
                else
                    mappings = costshareamountmapping.Where(c => c.CostShareID == viewmodel.CostShareID && c.IntervalNumber <= intervalnumber).ToList();
            }
            else
            {
                if (intervalnumber > 0)
                    mappings = costshareamountmapping.Where(c => c.CostShareID == viewmodel.CostShareID && c.NetworkType == DATA.InNetwork && c.IntervalNumber <= intervalnumber && c.IntervalNumber != 0).ToList();
                else
                    mappings = costshareamountmapping.Where(c => c.CostShareID == viewmodel.CostShareID && c.NetworkType == DATA.InNetwork && c.IntervalNumber <= intervalnumber).ToList();
            }

            for (int i = 0; i <= intervalnumber; i++)
            {
                List<CostShareAmountViewModel> networkmapping = mappings.Where(m => m.IntervalNumber == i).ToList();

                if (networkmapping != null && networkmapping.Count > 0)
                {
                    JToken job = null;
                    if (jInfoArray != null && jInfoArray.Count > 0)
                        job = jInfoArray.Where(j => j[KEYNAME.BenefitCategory1].ToString() == viewmodel.BenefitCategory1.Trim()
                                   && j[KEYNAME.BenefitCategory2].ToString() == viewmodel.BenefitCategory2.Trim()
                                   && j[KEYNAME.BenefitCategory3].ToString() == viewmodel.BenefitCategory3.Trim()
                                   && j[ELEMENTNAME.IntervalNumber].ToString() == i.ToString()).FirstOrDefault();

                    if (job == null)
                    {
                        job = new JObject();
                        jInfoArray.Add(job);
                    }

                    job[KEYNAME.BenefitCategory1] = viewmodel.BenefitCategory1.Trim();
                    job[KEYNAME.BenefitCategory2] = viewmodel.BenefitCategory2.Trim();
                    job[KEYNAME.BenefitCategory3] = viewmodel.BenefitCategory3.Trim();
                    job[ELEMENTNAME.IntervalNumber] = i.ToString();

                    JArray network = new JArray();
                    if (job[DATASOURCENAME.IQMedicareNetworkList] != null && job[DATASOURCENAME.IQMedicareNetworkList] is JArray)
                        network = (JArray)job[DATASOURCENAME.IQMedicareNetworkList];

                    job[DATASOURCENAME.IQMedicareNetworkList] = network;

                    foreach (CostShareAmountViewModel c in networkmapping)
                    {
                        JToken jobj = null;
                        if (network.Count > 0)
                            jobj = network.Where(n => n[ELEMENTNAME.CostShareTiers].ToString() == c.NetworkType).FirstOrDefault();

                        if (jobj == null)
                        {
                            jobj = new JObject();
                            network.Add(jobj);
                        }                     
   
                        jobj[ELEMENTNAME.CostShareTiers] = c.NetworkType;
                        bool checkforzero = false;

                        if (iszerointerval)
                        {
                            if (c.IntervalNumber == 0 && c.NetworkType == DATA.OutOfNetwork && intervalinnnumber == 0)
                                checkforzero = true;
                            else if (c.IntervalNumber == 0 && c.NetworkType == DATA.InNetwork && intervaloonnumber == 0)
                                checkforzero = true;
                        }

                        string copaybeginday = STANDARDVALUE.NotApplicable;
                        string copayendday = STANDARDVALUE.NotApplicable;
                        string copay = STANDARDVALUE.NotApplicable;
                        string coissurancebeginday = STANDARDVALUE.NotApplicable;
                        string coissuranceendday = STANDARDVALUE.NotApplicable;
                        string coissurance = STANDARDVALUE.NotApplicable;

                        if (!checkforzero)
                        {
                            copaybeginday = GetDataFromPBP(c.CopayBeginDayPBPTableName, c.CopayBeginDayPBPFieldName);
                            copayendday = GetDataFromPBP(c.CopayEndDayPBPTableName, c.CopayEndDayPBPFieldName);
                            copay = GetDataFromPBP(c.CopayPBPTableName, c.CopayPBPFieldName, true);
                            coissurancebeginday = GetDataFromPBP(c.CoinsuranceBeginDayPBPTableName, c.CoinsuranceBeginDayPBPFieldName);
                            coissuranceendday = GetDataFromPBP(c.CoinsuranceEndDayPBPTableName, c.CoinsuranceEndDayPBPFieldName);
                            coissurance = GetDataFromPBP(c.CoinsurancePBPTableName, c.CoinsurancePBPFieldName, true);

                            costSharelog.Add(this.GetLogEntity("    Cost Share Amount ID:" + c.CostShareAmountID.ToString() + "=> Copay Begin Day = " + copaybeginday + ", Copay End Day = " + copayendday + ", Copay = " + copay + ", Coinsurance Begin Day = " + coissurancebeginday + ", Coinsurance End Day = " + coissuranceendday + ", Coinsurance = " + coissurance));
                        }

                        if (!string.IsNullOrEmpty(c.CopayBeginDayPBPTableName) && !string.IsNullOrEmpty(c.CopayBeginDayPBPFieldName))
                            jobj[c.CopayBeginDayPath] = string.IsNullOrEmpty(copaybeginday) ? STANDARDVALUE.NotApplicable : copaybeginday;
                        else
                            jobj[c.CopayBeginDayPath] = jobj[c.CopayBeginDayPath] != null && !string.IsNullOrEmpty(jobj[c.CopayBeginDayPath].ToString()) ? jobj[c.CopayBeginDayPath].ToString() : STANDARDVALUE.NotApplicable;

                        if (!string.IsNullOrEmpty(c.CopayEndDayPBPTableName) && !string.IsNullOrEmpty(c.CopayEndDayPBPFieldName))
                            jobj[c.CopayEndDayPath] = string.IsNullOrEmpty(copayendday) ? STANDARDVALUE.NotApplicable : copayendday;
                        else
                            jobj[c.CopayEndDayPath] = jobj[c.CopayEndDayPath] != null && !string.IsNullOrEmpty(jobj[c.CopayEndDayPath].ToString()) ? jobj[c.CopayEndDayPath].ToString() : STANDARDVALUE.NotApplicable;

                        if (!string.IsNullOrEmpty(c.CopayPBPTableName) && !string.IsNullOrEmpty(c.CopayPBPFieldName))
                            jobj[c.CopayPath] = string.IsNullOrEmpty(copay) ? STANDARDVALUE.NotApplicable : copay;
                        else
                            jobj[c.CopayPath] = jobj[c.CopayPath] != null && !string.IsNullOrEmpty(jobj[c.CopayPath].ToString()) ? jobj[c.CopayPath].ToString() : STANDARDVALUE.NotApplicable;

                        if (!string.IsNullOrEmpty(c.CoinsuranceBeginDayPBPTableName) && !string.IsNullOrEmpty(c.CoinsuranceBeginDayPBPFieldName))
                            jobj[c.CoinsuranceBeginDayPath] = string.IsNullOrEmpty(coissurancebeginday) ? STANDARDVALUE.NotApplicable : coissurancebeginday;
                        else
                            jobj[c.CoinsuranceBeginDayPath] = jobj[c.CoinsuranceBeginDayPath] != null && !string.IsNullOrEmpty(jobj[c.CoinsuranceBeginDayPath].ToString()) ? jobj[c.CoinsuranceBeginDayPath].ToString() : STANDARDVALUE.NotApplicable;

                        if (!string.IsNullOrEmpty(c.CoinsuranceEndDayPBPTableName) && !string.IsNullOrEmpty(c.CoinsuranceEndDayPBPFieldName))
                            jobj[c.CoinsuranceEndDayPath] = string.IsNullOrEmpty(coissuranceendday) ? STANDARDVALUE.NotApplicable : coissuranceendday;
                        else
                            jobj[c.CoinsuranceEndDayPath] = jobj[c.CoinsuranceEndDayPath] != null && !string.IsNullOrEmpty(jobj[c.CoinsuranceEndDayPath].ToString()) ? jobj[c.CoinsuranceEndDayPath].ToString() : STANDARDVALUE.NotApplicable;

                        if (!string.IsNullOrEmpty(c.CoinsurancePBPTableName) && !string.IsNullOrEmpty(c.CoinsurancePBPFieldName))
                            jobj[c.CoinsurancePath] = string.IsNullOrEmpty(coissurance) ? STANDARDVALUE.NotApplicable : coissurance;
                        else
                            jobj[c.CoinsurancePath] = jobj[c.CoinsurancePath] != null && !string.IsNullOrEmpty(jobj[c.CoinsurancePath].ToString()) ? jobj[c.CoinsurancePath].ToString() : STANDARDVALUE.NotApplicable;
                    }
                }
            }            
        }

        private JObject PopulateGeneralCostShare()
        {
            JObject ja = new JObject();
            JArray jarry = new JArray();
            ja[REPEATERNAME.MaximumOutofPocketList] = jarry;
            List<GeneralCostShareViewModel> genmappings = _pbpImportServices.GetGeneralCostShareMapping();

            foreach (GeneralCostShareViewModel gen in genmappings)
            {
                if (gen.FieldName == ELEMENTNAME.MaximumOutofPocketAmount && gen.PBPFieldName == "PBP_D_OUT_POCKET_AMT")
                {
                    JObject j = new JObject();
                    j[ELEMENTNAME.CostShareTiers] = DATA.InNetwork;
                    string value = GetDataFromPBP(gen.PBPTableName, gen.PBPFieldName, true);
                    j[ELEMENTNAME.MaximumOutofPocketAmount] = string.IsNullOrEmpty(value) ? STANDARDVALUE.NotApplicable : value;
                    jarry.Add(j);
                }
                else if (_isOONApplicable && gen.FieldName == ELEMENTNAME.MaximumOutofPocketAmount && gen.PBPFieldName == "PBP_D_OON_MAX_ENR_OOPC_AMT")
                {
                    JObject j = new JObject();
                    j[ELEMENTNAME.CostShareTiers] = DATA.OutOfNetwork;
                    string value = GetDataFromPBP(gen.PBPTableName, gen.PBPFieldName, true);
                    j[ELEMENTNAME.MaximumOutofPocketAmount] = string.IsNullOrEmpty(value) ? STANDARDVALUE.NotApplicable : value;
                    jarry.Add(j);
                }
                else if (gen.FieldName == ELEMENTNAME.MedicarecoveredZeroDollarPreventiveServicesAttestation)
                    ja[gen.FieldName] = ApplyCustomRuleGetData(gen.PBPTableName, gen.PBPFieldName, 3, 1);
                else if (gen.FieldName == ELEMENTNAME.Isthereadeductible)
                    ja[gen.FieldName] = ApplyCustomRuleGetData(gen.PBPTableName, gen.PBPFieldName, 2, 0);
            }

            return ja;
        }

        #endregion CostShare

        #region Network

        private void PopulateNetwork(JObject json)
        {
            JArray jarry = new JArray();

            JObject j = new JObject();
            j[ELEMENTNAME.CostShareTiers] = DATA.InNetwork;

            jarry.Add(j);


            if (_isOONApplicable)
            {
                j = new JObject();
                j[ELEMENTNAME.CostShareTiers] = DATA.OutOfNetwork;
                jarry.Add(j);
            }

            json.SelectToken(SECTIONNAME.Tiers)[REPEATERNAME.SelectthePlansCostShareTiers] = jarry;
        }

        #endregion Network

        #region PlanInformation

        private void MapPlanInformationData(JObject json)
        {
            List<PlanInformationViewModel> planinfomapping = _pbpImportServices.GetPlanInformationMapping();

            foreach (PlanInformationViewModel viewmodel in planinfomapping)
            {
                string pbpData = string.Empty;
                if (viewmodel.IsCustomRule == false)
                    pbpData = GetDataFromPBP(viewmodel.PBPTableName, viewmodel.PBPFieldName);
                else
                    pbpData = ApplyCustomRuleGetData(viewmodel.PBPTableName, viewmodel.PBPFieldName, viewmodel.CustomRuleTypeId, 0);

                json.SelectToken(viewmodel.FieldPath)[viewmodel.FieldName] = pbpData;

                if (viewmodel.FieldName == "PlanType" && pbpData == "HMOPOS")
                    _isOONApplicable = true;
            }
        }

        #endregion PlanInformation

        #region private methods

        private string ApplyCustomRuleGetData(string pbpTableName, string pbpFieldName, int customRuleTypeId, int index)
        {
            string pbpActualData = string.Empty;
            string pbpData = string.Empty;

            if (string.IsNullOrEmpty(pbpTableName) || string.IsNullOrEmpty(pbpFieldName))
                return pbpData;

            pbpTableName = pbpTableName.Trim();
            pbpFieldName = pbpFieldName.Trim();

            PBPDataMapViewModel datamodel = _pbpDataModelList.Where(d => d.QID == _pbpQId && d.TableName == pbpTableName && d.PBPImportBatchID == _pbpBatchId).FirstOrDefault();

            if (datamodel != null)
            {
                JObject jobj = JObject.Parse(datamodel.JsonData);
                if (jobj[pbpFieldName] != null && !string.IsNullOrEmpty(jobj[pbpFieldName].ToString()))
                    pbpActualData = jobj[pbpFieldName].ToString();
            }

            if (!string.IsNullOrEmpty(pbpActualData))
            {
                switch (customRuleTypeId)
                {
                    //Get the values for Plan Type
                    case 1:
                        pbpData = GetValueFromCode(pbpActualData);
                        break;
                    //Get the values for check box/radio button 
                    case 2:
                        pbpData = GetValueForCheckBox(pbpActualData);
                        break;
                    //Get true/false depending on position of 1 in the string
                    case 3: pbpData = GetValueAgainstIndex(pbpActualData, index);
                        break;
                }
            }

            return pbpData;
        }

        private string GetValueAgainstIndex(string pbpActualData, int index)
        {
            char[] a = pbpActualData.ToCharArray();

            if (a.Length != 0 && index != 0)
            {
                if (a.Length < index - 1)
                    return false.ToString();

                if (a[index - 1] == '1')
                    return true.ToString();
                else
                    return false.ToString();
            }
            else
                return false.ToString();
        }

        private string GetValueForCheckBox(string pbpActualData)
        {
            string value = false.ToString();

            if (pbpActualData == "1")
                value = true.ToString();

            return value;
        }

        private string GetValueFromCode(string pbpActualData)
        {
            string value = string.Empty;

            int code = 0;
            if (int.TryParse(pbpActualData, out code))
            {
                List<PlanTypeViewModel> planTypeList = _pbpImportServices.GetPlanTypeCodeDescription();
                value = planTypeList.Where(p => p.Code == code).Select(p => p.Description).First();
            }
            return value;
        }

        private string GetDataFromPBP(string pbpTableName, string pbpFieldName, bool isamount)
        {
            string pbpData = GetDataFromPBP(pbpTableName, pbpFieldName);

            if (!string.IsNullOrEmpty(pbpData))
            {
                decimal amount;
                if (decimal.TryParse(pbpData, out amount))
                    return ((int)amount).ToString();
            }

            return pbpData;
        }

        private string GetDataFromPBP(string pbpTableName, string pbpFieldName)
        {
            string pbpData = string.Empty;

            if (string.IsNullOrEmpty(pbpTableName) || string.IsNullOrEmpty(pbpFieldName))
                return pbpData;
            pbpTableName = pbpTableName.Trim();
            pbpFieldName = pbpFieldName.Trim();

            List<PBPDataMapViewModel> dataModelList = _pbpDataModelList.Where(d => d.QID == _pbpQId && d.TableName == pbpTableName && d.PBPImportBatchID == _pbpBatchId).ToList();

            foreach (PBPDataMapViewModel datamodel in dataModelList)
            {
                JObject jobj = JObject.Parse(datamodel.JsonData);
                if (jobj[pbpFieldName] != null && !string.IsNullOrEmpty(jobj[pbpFieldName].ToString()))
                    pbpData += jobj[pbpFieldName].ToString();
            }

            return pbpData;
        }

        private string GetDataFromPBP(string pbpTableName, string pbpFieldName, PBPDataMapViewModel dataModel, bool isAmount)
        {
            string pbpData = GetDataFromPBP(pbpTableName, pbpFieldName, dataModel);

            if (!string.IsNullOrEmpty(pbpData))
            {
                decimal amount;
                if (decimal.TryParse(pbpData, out amount))
                    return ((int)amount).ToString();
            }

            return pbpData;
        }

        private string GetDataFromPBP(string pbpTableName, string pbpFieldName, PBPDataMapViewModel dataModel)
        {
            string pbpData = string.Empty;

            if (string.IsNullOrEmpty(pbpTableName) || string.IsNullOrEmpty(pbpFieldName))
                return pbpData;
            pbpTableName = pbpTableName.Trim();
            pbpFieldName = pbpFieldName.Trim();

            if (dataModel == null)
                return pbpData;
            else
            {
                JObject jobj = JObject.Parse(dataModel.JsonData);
                if (jobj[pbpFieldName] != null && !string.IsNullOrEmpty(jobj[pbpFieldName].ToString()))
                    pbpData = jobj[pbpFieldName].ToString();
            }

            return pbpData;
        }

        private PBPImportActivityLogViewModel GetLogEntity(string message)
        {
            PBPImportActivityLogViewModel log = new PBPImportActivityLogViewModel();
            log.CreatedBy = this._userName;
            log.CreatedDate = DateTime.Now;
            log.PBPImportQueueID = _queueId;
            log.PBPImportBatchID = this._pbpBatchId;
            log.Message = message;
            return log;
        }

        #endregion private methods
    }

    public class CostShareDefaultServices
    {
        public string BenefitCategory1 { get; set; }
        public string BenefitCategory2 { get; set; }
        public string BenefitCategory3 { get; set; }
    }
}
