using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.PBPImport;

namespace tmg.equinox.pbpimportservices.PBPMappingsBuilder
{
    public class PrescriptionBuilder
    {
        int _forminstanceid;
        string _pbpQId = string.Empty;
        int _pbpBatchId;
        List<PBPDataMapViewModel> _pbpDataModelList;
        string _userName;
        private IPBPImportServices _pbpImportServices;
        MapPBPData _mapPBPData;
        int _qid;

        public PrescriptionBuilder(int forminstanceId, int qid, string pbpQId, int pbpBatchId, string userName, List<PBPDataMapViewModel> pbpDataModelList, IPBPImportServices pbpServices, MapPBPData mapPBPData)
        {
            this._forminstanceid = forminstanceId;
            this._pbpBatchId = pbpBatchId;
            this._pbpQId = pbpQId;
            this._pbpDataModelList = pbpDataModelList;
            this._userName = userName;
            this._pbpImportServices = pbpServices;
            this._mapPBPData = mapPBPData;
            this._qid = qid;
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
                prescriptionLog.Add(HelperUtility.GetLogEntity("PrescriptionID = " + viewmodel.PrescriptionID + " IsRepeater = " + viewmodel.IsRepeater.ToString(), _userName, _qid, _pbpBatchId));
                if (!viewmodel.IsRepeater)
                {
                    string pbpData = string.Empty;
                    if (viewmodel.IsCustomRule == false)
                        pbpData = HelperUtility.GetDataFromPBP(viewmodel.PBPTableName, viewmodel.PBPFieldName, _pbpDataModelList, _pbpQId, _pbpBatchId);
                    else
                        pbpData = HelperUtility.ApplyCustomRuleGetData(viewmodel.PBPTableName, viewmodel.PBPFieldName, viewmodel.CustomRuleTypeId, viewmodel.PositionForOne, _pbpDataModelList, _pbpQId, _pbpBatchId, null);

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
                            json.SelectToken(viewmodel.FieldPath)[REPEATERNAME.StandardMailOrderCostSharing] = j;
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
                        if (bool.TryParse(pbpData, out isbooldata))
                            json.SelectToken(viewmodel.FieldPath)[viewmodel.FieldName] = isbooldata;
                    }

                }
                else
                {
                    if (viewmodel.FieldName == ELEMENTNAME.CostShareTiersInformation)
                    {
                        JArray jarr = new JArray();
                        string pbpData = HelperUtility.GetDataFromPBP(viewmodel.PBPTableName, viewmodel.PBPFieldName, _pbpDataModelList, _pbpQId, _pbpBatchId);
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
                        string pbpData = HelperUtility.GetDataFromPBP(viewmodel.PBPTableName, viewmodel.PBPFieldName, _pbpDataModelList, _pbpQId, _pbpBatchId);
                        if (!string.IsNullOrEmpty(pbpData))
                        {
                            if (tierdescription.Count > 0)
                            {
                                foreach (KeyValuePair<int, string> k in tierdescription)
                                {
                                    bool isapplicable = false;
                                    if (k.Key == 1)
                                        isapplicable = !Convert.ToBoolean(HelperUtility.GetValueAgainstIndex(pbpData, 2));
                                    if (k.Key == 2)
                                        isapplicable = !Convert.ToBoolean(HelperUtility.GetValueAgainstIndex(pbpData, 3));
                                    if (k.Key == 3)
                                        isapplicable = !Convert.ToBoolean(HelperUtility.GetValueAgainstIndex(pbpData, 4));
                                    if (k.Key == 4)
                                        isapplicable = !Convert.ToBoolean(HelperUtility.GetValueAgainstIndex(pbpData, 5));
                                    if (k.Key == 5)
                                        isapplicable = !Convert.ToBoolean(HelperUtility.GetValueAgainstIndex(pbpData, 6));
                                    if (k.Key == 6)
                                        isapplicable = !Convert.ToBoolean(HelperUtility.GetValueAgainstIndex(pbpData, 1));

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
                    prescriptionLog.Add(HelperUtility.GetLogEntity("     " + viewmodel.FieldName + ": TierDescription Count= " + tierdescription.Count, _userName, _qid, _pbpBatchId));
                }
            }
        }

        private JArray PopulateRepeater(Dictionary<int, string> tierdescription, List<PrescriptionRepeaterViewModel> repeatermaps, int typeid, string repeaterName, ref List<PBPImportActivityLogViewModel> populateRepeaterlog)
        {
            JArray jarr = new JArray();
            populateRepeaterlog.Add(HelperUtility.GetLogEntity("--> Populating " + repeaterName + " repeater", _userName, _qid, _pbpBatchId));
            PrescriptionRepeaterViewModel viewmodel = repeatermaps.Where(r => r.TypeID == 1 && r.RepeaterName == repeaterName).FirstOrDefault();
            if (viewmodel != null)
            {
                List<PBPDataMapViewModel> dataModelList = _pbpDataModelList.Where(d => d.QID == _pbpQId && d.TableName == viewmodel.TiersPBPTableName && d.PBPImportBatchID == _pbpBatchId).ToList();

                foreach (PBPDataMapViewModel model in dataModelList)
                {
                    string tid = HelperUtility.GetDataFromPBP(viewmodel.TiersPBPTableName, "MRX_TIER_TYPE_ID", model);
                    if (tid == typeid.ToString())
                    {
                        JObject j = new JObject();
                        string tier = HelperUtility.GetDataFromPBP(viewmodel.TiersPBPTableName, viewmodel.TiersPBPFieldName, model);

                        int tierid = 0;
                        if (!string.IsNullOrEmpty(tier) && int.TryParse(tier, out tierid))
                        {
                            string desc = tierdescription[tierid];

                            j[viewmodel.TiersFieldName] = tier;
                            j[viewmodel.DescriptionFieldName] = desc;

                            string onemonth = HelperUtility.GetDollerValue(HelperUtility.GetDataFromPBP(viewmodel.OnemonthsupplyPBPTableName, viewmodel.OnemonthsupplyPBPFieldName, model));
                            if (string.IsNullOrEmpty(onemonth))
                                onemonth = HelperUtility.GetPercentageValue(HelperUtility.GetDataFromPBP(viewmodel.OnemonthsupplyConiPBPTableName, viewmodel.OnemonthsupplyConiPBPFieldName, model));
                            j[viewmodel.OnemonthsupplyName] = string.IsNullOrEmpty(onemonth) ? STANDARDVALUE.NotApplicable : onemonth;

                            string twomonth = HelperUtility.GetDollerValue(HelperUtility.GetDataFromPBP(viewmodel.TwomonthsupplyPBPTableName, viewmodel.TwomonthsupplyPBPFieldName, model));
                            if (string.IsNullOrEmpty(onemonth))
                                twomonth = HelperUtility.GetPercentageValue(HelperUtility.GetDataFromPBP(viewmodel.TwomonthsupplyConiPBPTableName, viewmodel.TwomonthsupplyConiPBPFieldName, model));
                            j[viewmodel.TwomonthsupplyName] = string.IsNullOrEmpty(twomonth) ? STANDARDVALUE.NotApplicable : twomonth;

                            string threemonth = HelperUtility.GetDollerValue(HelperUtility.GetDataFromPBP(viewmodel.ThreemonthsupplyPBPTableName, viewmodel.ThreemonthsupplyPBPFieldName, model));
                            if (string.IsNullOrEmpty(threemonth))
                                threemonth = HelperUtility.GetPercentageValue(HelperUtility.GetDataFromPBP(viewmodel.ThreemonthsupplyConiPBPTableName, viewmodel.ThreemonthsupplyConiPBPFieldName, model));
                            j[viewmodel.ThreemonthsupplyName] = string.IsNullOrEmpty(threemonth) ? STANDARDVALUE.NotApplicable : threemonth;

                            string drugs = string.Empty;
                            if (!string.IsNullOrEmpty(viewmodel.DrugsCoveredName))
                            {
                                drugs = HelperUtility.GetDataFromPBP(viewmodel.DrugsCoveredPBPTableName, viewmodel.DrugsCoveredPBPFieldName, model);
                                if (string.IsNullOrEmpty(drugs))
                                    drugs = HelperUtility.GetDataFromPBP(viewmodel.DrugsCoveredPBPTableName, viewmodel.DrugsCoveredPBPFieldName, model);
                                j[viewmodel.DrugsCoveredName] = string.IsNullOrEmpty(drugs) ? STANDARDVALUE.NotApplicable : drugs;
                            }
                            populateRepeaterlog.Add(HelperUtility.GetLogEntity("     OneMonthSupply=" + onemonth + " TwoMonthSupply=" + twomonth + " ThreeMonthSupply=" + threemonth + " DrugsCovered=" + drugs, _userName, _qid, _pbpBatchId));
                            jarr.Add(j);
                        }
                    }
                }
            }
            populateRepeaterlog.Add(HelperUtility.GetLogEntity("<-- Populating " + repeaterName + " repeater", _userName, _qid, _pbpBatchId));
            return jarr;
        }
        #endregion Prescription



    }
}
