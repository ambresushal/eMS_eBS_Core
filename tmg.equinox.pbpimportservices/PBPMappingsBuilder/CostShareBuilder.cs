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
    public class CostShareBuilder
    {
        int _forminstanceid;
        string _pbpQId = string.Empty;
        int _pbpBatchId;
        List<PBPDataMapViewModel> _pbpDataModelList;
        string _userName;
        private IPBPImportServices _pbpImportServices;
        MapPBPData _mapPBPData;
        int _qid;

        public CostShareBuilder(int forminstanceId, int qid, string pbpQId, int pbpBatchId, string userName, List<PBPDataMapViewModel> pbpDataModelList, IPBPImportServices pbpServices, MapPBPData mapPBPData)
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
                    pbpData = HelperUtility.ApplyCustomRuleGetData(viewmodel.PBPTableName, viewmodel.PBPFieldName, viewmodel.CustomRuleTypeId, viewmodel.SequenceNumberforOne, _pbpDataModelList, _pbpQId, _pbpBatchId, null);
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
                        _mapPBPData._defaultServices.Add(c);

                        jobj[KEYNAME.BenefitCategory1] = viewmodel.BenefitCategory1.Trim();
                        jobj[KEYNAME.BenefitCategory2] = viewmodel.BenefitCategory2.Trim();
                        jobj[KEYNAME.BenefitCategory3] = viewmodel.BenefitCategory3.Trim();

                        costSharelog.Add(HelperUtility.GetLogEntity("--> Service selected: BC1 = " + viewmodel.BenefitCategory1.Trim() + ", BC2 = " + viewmodel.BenefitCategory2.Trim() + ", BC3 = " + viewmodel.BenefitCategory3.Trim(), _userName, _qid, _pbpBatchId));

                        if (!string.IsNullOrEmpty(viewmodel.BenefitPeriodPBPTableName) && !string.IsNullOrEmpty(viewmodel.BenefitPeriodPBPFieldName))
                        {
                            string beperiod = HelperUtility.GetDataFromPBP(viewmodel.BenefitPeriodPBPTableName, viewmodel.BenefitPeriodPBPFieldName, _pbpDataModelList, _pbpQId, _pbpBatchId);
                            jobj[ELEMENTNAME.BenefitPeriod] = HelperUtility.GetBenefitperiod(beperiod);
                            costSharelog.Add(HelperUtility.GetLogEntity("    BenefitPeriod = " + beperiod, _userName, _qid, _pbpBatchId));
                        }

                        string isbenunlimited = HelperUtility.ApplyCustomRuleGetData(viewmodel.IsthisBenefitUnlimitedPBPTableName, viewmodel.IsthisBenefitUnlimitedPBPFieldName, 2, 0, _pbpDataModelList, _pbpQId, _pbpBatchId, null);
                        if (!string.IsNullOrEmpty(isbenunlimited))
                            jobj[ELEMENTNAME.IsthisBenefitUnlimited] = Convert.ToBoolean(isbenunlimited) == true ? STANDARDVALUE.Yes : STANDARDVALUE.No;
                        else
                            jobj[ELEMENTNAME.IsthisBenefitUnlimited] = string.Empty;

                        costSharelog.Add(HelperUtility.GetLogEntity("    IsthisBenefitUnlimited = " + isbenunlimited, _userName, _qid, _pbpBatchId));

                        int intervalnumber = -999;
                        int intervaloonnumber = -999;

                        if ((!string.IsNullOrEmpty(viewmodel.IntervalPBPTableName) && !string.IsNullOrEmpty(viewmodel.IntervalOONPBPTableName)) ||
                            (!string.IsNullOrEmpty(viewmodel.IntervalCopayPBPTableName) && !string.IsNullOrEmpty(viewmodel.IntervalCopayPBPFieldName)) ||
                            (!string.IsNullOrEmpty(viewmodel.IntervalOONPBPTableName) && !string.IsNullOrEmpty(viewmodel.IntervalOONPBPFieldName)))
                        {
                            string interval = HelperUtility.GetDataFromPBP(viewmodel.IntervalPBPTableName, viewmodel.IntervalPBPFieldName, _pbpDataModelList, _pbpQId, _pbpBatchId);
                            string intervalno = HelperUtility.GetIntervalNumber(interval);

                            costSharelog.Add(HelperUtility.GetLogEntity("    Interval Number In Network = " + intervalno, _userName, _qid, _pbpBatchId));

                            if (!int.TryParse(intervalno, out intervalnumber))
                                intervalnumber = -1;

                            if (intervalnumber < 0)
                            {
                                interval = HelperUtility.GetDataFromPBP(viewmodel.IntervalCopayPBPTableName, viewmodel.IntervalCopayPBPFieldName, _pbpDataModelList, _pbpQId, _pbpBatchId);
                                intervalno = HelperUtility.GetIntervalNumber(interval);

                                costSharelog.Add(HelperUtility.GetLogEntity("    Interval Number Copay In Network = " + intervalno, _userName, _qid, _pbpBatchId));

                                if (!int.TryParse(intervalno, out intervalnumber))
                                    intervalnumber = -1;
                            }

                            if (_mapPBPData._isOONApplicable)
                            {
                                string intervaloon = HelperUtility.GetDataFromPBP(viewmodel.IntervalOONPBPTableName, viewmodel.IntervalOONPBPFieldName, _pbpDataModelList, _pbpQId, _pbpBatchId);
                                string intervaloonno = HelperUtility.GetIntervalNumber(intervaloon);

                                costSharelog.Add(HelperUtility.GetLogEntity("    Interval Number Out Of Network =" + intervaloonno, _userName, _qid, _pbpBatchId));

                                if (!int.TryParse(intervaloonno, out intervaloonnumber))
                                    intervaloonnumber = -1;

                                if (intervaloonnumber < 0)
                                {
                                    intervaloon = HelperUtility.GetDataFromPBP(viewmodel.IntervalOONCopayPBPTableName, viewmodel.IntervalOONCopayPBPFieldName, _pbpDataModelList, _pbpQId, _pbpBatchId);
                                    intervaloonno = HelperUtility.GetIntervalNumber(intervaloon);

                                    costSharelog.Add(HelperUtility.GetLogEntity("    Interval Number Out Of Network =" + intervaloonno, _userName, _qid, _pbpBatchId));

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
                        costSharelog.Add(HelperUtility.GetLogEntity("<-- Service selected: BC1 = " + viewmodel.BenefitCategory1.Trim() + ", BC2 = " + viewmodel.BenefitCategory2.Trim() + ", BC3 = " + viewmodel.BenefitCategory3.Trim(), _userName, _qid, _pbpBatchId));
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
            if (_mapPBPData._isOONApplicable)
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
                            copaybeginday = HelperUtility.GetDataFromPBP(c.CopayBeginDayPBPTableName, c.CopayBeginDayPBPFieldName, _pbpDataModelList, _pbpQId, _pbpBatchId);
                            copayendday = HelperUtility.GetDataFromPBP(c.CopayEndDayPBPTableName, c.CopayEndDayPBPFieldName, _pbpDataModelList, _pbpQId, _pbpBatchId);
                            copay = HelperUtility.GetDataFromPBP(c.CopayPBPTableName, c.CopayPBPFieldName, _pbpDataModelList, _pbpQId, _pbpBatchId);
                            coissurancebeginday = HelperUtility.GetDataFromPBP(c.CoinsuranceBeginDayPBPTableName, c.CoinsuranceBeginDayPBPFieldName, _pbpDataModelList, _pbpQId, _pbpBatchId);
                            coissuranceendday = HelperUtility.GetDataFromPBP(c.CoinsuranceEndDayPBPTableName, c.CoinsuranceEndDayPBPFieldName, _pbpDataModelList, _pbpQId, _pbpBatchId);
                            coissurance = HelperUtility.GetDataFromPBP(c.CoinsurancePBPTableName, c.CoinsurancePBPFieldName, _pbpDataModelList, _pbpQId, _pbpBatchId);

                            costSharelog.Add(HelperUtility.GetLogEntity("    Cost Share Amount ID:" + c.CostShareAmountID.ToString() + "=> Copay Begin Day = " + copaybeginday + ", Copay End Day = " + copayendday + ", Copay = " + copay + ", Coinsurance Begin Day = " + coissurancebeginday + ", Coinsurance End Day = " + coissuranceendday + ", Coinsurance = " + coissurance, _userName, _qid, _pbpBatchId));
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
                    string value = HelperUtility.GetDataFromPBP(gen.PBPTableName, gen.PBPFieldName, _pbpDataModelList, _pbpQId, _pbpBatchId);
                    j[ELEMENTNAME.MaximumOutofPocketAmount] = string.IsNullOrEmpty(value) ? STANDARDVALUE.NotApplicable : value;
                    jarry.Add(j);
                }
                else if (_mapPBPData._isOONApplicable && gen.FieldName == ELEMENTNAME.MaximumOutofPocketAmount && gen.PBPFieldName == "PBP_D_OON_MAX_ENR_OOPC_AMT")
                {
                    JObject j = new JObject();
                    j[ELEMENTNAME.CostShareTiers] = DATA.OutOfNetwork;
                    string value = HelperUtility.GetDataFromPBP(gen.PBPTableName, gen.PBPFieldName, _pbpDataModelList, _pbpQId, _pbpBatchId);
                    j[ELEMENTNAME.MaximumOutofPocketAmount] = string.IsNullOrEmpty(value) ? STANDARDVALUE.NotApplicable : value;
                    jarry.Add(j);
                }
                else if (gen.FieldName == ELEMENTNAME.MedicarecoveredZeroDollarPreventiveServicesAttestation)
                    ja[gen.FieldName] = HelperUtility.ApplyCustomRuleGetData(gen.PBPTableName, gen.PBPFieldName, 3, 1, _pbpDataModelList, _pbpQId, _pbpBatchId, null);
                else if (gen.FieldName == ELEMENTNAME.Isthereadeductible)
                    ja[gen.FieldName] = HelperUtility.ApplyCustomRuleGetData(gen.PBPTableName, gen.PBPFieldName, 2, 0, _pbpDataModelList, _pbpQId, _pbpBatchId, null);
            }

            return ja;
        }

        #endregion CostShare
    }
}
