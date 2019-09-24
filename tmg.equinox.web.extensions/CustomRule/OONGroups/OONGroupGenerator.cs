using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.domain.viewmodels;

namespace tmg.equinox.rules.oongroups
{
    public class OONGroupGenerator
    {
        private string _sotData;
        private string _pbpSectionData;
        private JToken _sotDataJSON;
        private JToken _pbpSectionDataJSON;
        private string _pbpGroupNumberSectionData;
        private JToken _pbpGroupNumberSectionDataJSON;
        private int _oonGroupCount;

        private IFormInstanceService _fiService;
        int _formDesignVersionId;

        public OONGroupGenerator(string sotData, string pbpSectionData, string pbpGroupNumberSectionData, IFormInstanceService fiService, int formDesignVersionId)
        {
            _sotData = sotData;
            _sotDataJSON = JToken.Parse(sotData);
            _pbpSectionData = pbpSectionData;
            _pbpSectionDataJSON = JToken.Parse(pbpSectionData);
            _pbpGroupNumberSectionData = pbpGroupNumberSectionData;
            _pbpGroupNumberSectionDataJSON = JToken.Parse(pbpGroupNumberSectionData);
            _fiService = fiService;
            _formDesignVersionId = formDesignVersionId;
        }

        public string GetOONGroups()
        {
            //get OON Config Entries
            List<OONGroupEntryModel> oonGroupEntries = _fiService.GetOONGroupEntries(_formDesignVersionId);
            List<OONGroupTargetModel> results = new List<OONGroupTargetModel>();

            //populate from SOT
            OONGroupsHelper.GetOONData(_sotDataJSON, ref oonGroupEntries);

            //process other Benefits responses
            //get Medicare benefits for DVH

            var otherEntries = oonGroupEntries.Where(a => a.Package == "Other" || (a.Package != "Other" && a.BenefitType == "Medicare")).ToList();
            //for each benefit code , get min copay, max copay, min coins, max coins, cost share type
            var benefitCodeGroups = otherEntries.GroupBy(a => a.BenefitCode);
            List<OONGroupTargetModel> otherBenefits = new List<OONGroupTargetModel>();
            foreach (var group in benefitCodeGroups)
            {
                OONGroupTargetModel otherModel = OONGroupsHelper.GenerateTargetEntry(group.ToList(), "", false);
                otherBenefits.Add(otherModel);
            }

            List<OONGroupTargetModel> packageModels = new List<OONGroupTargetModel>();
            //process Dental Non-Medicare
            var dentalEntries = oonGroupEntries.Where(a => a.Package == "Dental" && a.BenefitType != "Medicare").ToList();
            OONGroupTargetModel dentalModel = OONGroupsHelper.GenerateTargetEntry(dentalEntries, "Dental", true);
            if (dentalModel.IsthereanOONCoinsuranceforthisGroup == "1" || dentalModel.IsthereanOONCopaymentforthisGroup == "1")
            {

                packageModels.Add(dentalModel);
            }

            //process Hearing Non-Medicare
            var hearingEntries = oonGroupEntries.Where(a => a.Package == "Hearing" && a.BenefitType != "Medicare").ToList();
            OONGroupTargetModel hearingModel = OONGroupsHelper.GenerateTargetEntry(hearingEntries, "Hearing", true);
            if (hearingModel.IsthereanOONCoinsuranceforthisGroup == "1" || hearingModel.IsthereanOONCopaymentforthisGroup == "1")
            {
                packageModels.Add(hearingModel);
            }
            //process Vision Non-Medicare
            var visionEntries = oonGroupEntries.Where(a => a.Package == "Vision" && a.BenefitType != "Medicare").ToList();
            OONGroupTargetModel visionModel = OONGroupsHelper.GenerateTargetEntry(visionEntries, "Vision", true);
            if (visionModel.IsthereanOONCoinsuranceforthisGroup == "1" || visionModel.IsthereanOONCopaymentforthisGroup == "1")
            {
                packageModels.Add(visionModel);
            }

            OONGroupEntryModel zdEntry = null;// GetZeroDollarEntry(oonGroupEntries);
            bool zdCondition = false;
            if (zdEntry != null && !String.IsNullOrEmpty(zdEntry.ConditionValue))
            {
                if (zdEntry.ConditionValue == "YES")
                {
                    zdCondition = true;
                }
            }

            //do final grouping 
            results = ProcessFinalGroupingForOtherBenefits(otherBenefits, packageModels.Count,zdCondition);
            results.AddRange(packageModels);
            _oonGroupCount = results.Count;
            return GeneratePBPGroupData(results);
        }




        private string GeneratePBPGroupData(List<OONGroupTargetModel> results)
        {
            var tokenBase1 = _pbpSectionDataJSON.SelectToken("OONGroups.OONGroupsBase1");
            var tokenBase2 = _pbpSectionDataJSON.SelectToken("OONGroups.OONGroupsBase2");

            if (tokenBase1.Type != JTokenType.Array)
            {
                _pbpSectionDataJSON.SelectToken("OONGroups.OONGroupsBase1").Replace(new JArray());
                tokenBase1 = _pbpSectionDataJSON.SelectToken("OONGroups.OONGroupsBase1");
            }
            if (tokenBase2.Type != JTokenType.Array)
            {
                _pbpSectionDataJSON.SelectToken("OONGroups.OONGroupsBase2").Replace(new JArray());
                tokenBase2 = _pbpSectionDataJSON.SelectToken("OONGroups.OONGroupsBase2");
            }

            if (tokenBase1.Type == JTokenType.Array && results.Count > 0)
            {
                JArray tokenArray = (JArray)tokenBase1;
                JObject clone;
                if (tokenArray.Count > 0)
                {
                    clone = (JObject)tokenArray[0].DeepClone();
                }
                else
                {
                    clone = JObject.Parse("{'OONGroupID':'','EnterLabelforthisGroupOptional':'','SelectthebenefitsthatapplytotheOONGroups':[],'SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis':[],'SelecttheNonMedicarecoveredservicecategoriesincludedintheOONoptionfort':[],'Isthereamaximumplanbenefitcoverageamountforthisgroup':'','Indicatemaximumplanbenefitcoverageamount':''}");
                }
                tokenArray.Clear();
                int idx = 1;
                foreach (var result in results)
                {
                    JObject cloneCopy = (JObject)clone.DeepClone();
                    var newToken = cloneCopy["OONGroupID"];
                    var prop = newToken.Parent as JProperty;
                    prop.Value = idx.ToString();
                    idx++;
                    newToken = cloneCopy["EnterLabelforthisGroupOptional"];
                    prop = newToken.Parent as JProperty;
                    prop.Value = result.EnterLabelforthisGroupOptional;
                    newToken = cloneCopy["SelectthebenefitsthatapplytotheOONGroups"];
                    prop = newToken.Parent as JProperty;
                    prop.Value = GetBenefitsApplyValue(result.SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis, result.SelecttheNonMedicarecoveredservicecategoriesincludedintheOONoptionfort);
                    newToken = cloneCopy["SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis"];
                    prop = newToken.Parent as JProperty;
                    prop.Value = GetArray(result.SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis);
                    newToken = cloneCopy["SelecttheNonMedicarecoveredservicecategoriesincludedintheOONoptionfort"];
                    prop = newToken.Parent as JProperty;
                    prop.Value = GetArray(result.SelecttheNonMedicarecoveredservicecategoriesincludedintheOONoptionfort);
                    newToken = cloneCopy["Isthereamaximumplanbenefitcoverageamountforthisgroup"];
                    prop = newToken.Parent as JProperty;
                    prop.Value = result.Isthereamaximumplanbenefitcoverageamountforthisgroup;
                    newToken = cloneCopy["Indicatemaximumplanbenefitcoverageamount"];
                    prop = newToken.Parent as JProperty;
                    prop.Value = result.Indicatemaximumplanbenefitcoverageamount != null? result.Indicatemaximumplanbenefitcoverageamount:"";
                    tokenArray.Add(cloneCopy);
                }
            }

            if (tokenBase2.Type == JTokenType.Array && results.Count > 0)
            {
                JArray tokenArray = (JArray)tokenBase2;
                JObject clone;
                if (tokenArray.Count > 0)
                {
                    clone = (JObject)tokenArray[0].DeepClone();
                }
                else
                {
                    clone = JObject.Parse("{'OONGroupID':'','IsthereanOONCoinsuranceforthisGroup':'','EnterMinimumCoinsurancePercentageforthisGroup':'','EnterMaximumCoinsurancePercentageforthisGroup':'','IsthereanOONCopaymentforthisGroup':'','EnterMinimumCopaymentAmountforthisGroup':'','EnterMaximumCopaymentAmountforthisGroup':'','IsthereanOONDeductibleforthisgroup':'','EnterDeductibleAmountforthisgroup':''}");
                }
                tokenArray.Clear();
                int idx = 1;
                foreach (var result in results)
                {
                    JObject cloneCopy = (JObject)clone.DeepClone();
                    var newToken = cloneCopy["OONGroupID"];
                    var prop = newToken.Parent as JProperty;
                    prop.Value = idx.ToString();
                    idx++;
                    newToken = cloneCopy["IsthereanOONCoinsuranceforthisGroup"];
                    prop = newToken.Parent as JProperty;
                    prop.Value = result.IsthereanOONCoinsuranceforthisGroup;
                    newToken = cloneCopy["EnterMinimumCoinsurancePercentageforthisGroup"];
                    prop = newToken.Parent as JProperty;
                    prop.Value = result.EnterMinimumCoinsurancePercentageforthisGroup != null? result.EnterMinimumCoinsurancePercentageforthisGroup:"";
                    newToken = cloneCopy["EnterMaximumCoinsurancePercentageforthisGroup"];
                    prop = newToken.Parent as JProperty;
                    prop.Value = result.EnterMaximumCoinsurancePercentageforthisGroup != null ? result.EnterMaximumCoinsurancePercentageforthisGroup:"";
                    newToken = cloneCopy["IsthereanOONCopaymentforthisGroup"];
                    prop = newToken.Parent as JProperty;
                    prop.Value = result.IsthereanOONCopaymentforthisGroup;
                    newToken = cloneCopy["EnterMinimumCopaymentAmountforthisGroup"];
                    prop = newToken.Parent as JProperty;
                    prop.Value = result.EnterMinimumCopaymentAmountforthisGroup;
                    newToken = cloneCopy["EnterMaximumCopaymentAmountforthisGroup"];
                    prop = newToken.Parent as JProperty;
                    prop.Value = result.EnterMaximumCopaymentAmountforthisGroup;
                    newToken = cloneCopy["IsthereanOONDeductibleforthisgroup"];
                    prop = newToken.Parent as JProperty;
                    prop.Value = result.IsthereanOONDeductibleforthisgroup;
                    newToken = cloneCopy["EnterDeductibleAmountforthisgroup"];
                    prop = newToken.Parent as JProperty;
                    prop.Value = result.EnterDeductibleAmountforthisgroup != null? result.EnterDeductibleAmountforthisgroup:"";
                    tokenArray.Add(cloneCopy);
                }
            }
            return JsonConvert.SerializeObject(_pbpSectionDataJSON);
        }

        public string GetOONGroupNumbers()
        {
            var token = _pbpGroupNumberSectionDataJSON.SelectToken("OONNumberofGroups.IndicatethenumberofOutofNetworkgroupingsofferedexcludingInpatientHospi");
            if(token != null)
            {
                var prop = token.Parent as JProperty;
                prop.Value = _oonGroupCount==0 ? string.Empty: _oonGroupCount.ToString();
            }
            return JsonConvert.SerializeObject(_pbpGroupNumberSectionDataJSON);
        }
        private List<OONGroupTargetModel> GetEqualCoinsuranceGroups(List<OONGroupTargetModel> coinsurances)
        {
            List<OONGroupTargetModel> result = new List<OONGroupTargetModel>();
            var groups = coinsurances.Where(a => (a.EnterMinimumCoinsurancePercentageforthisGroup == a.EnterMaximumCoinsurancePercentageforthisGroup) && (a.Isthereamaximumplanbenefitcoverageamountforthisgroup == "2") && (a.IsthereanOONDeductibleforthisgroup == "2")).GroupBy(b => b.EnterMinimumCoinsurancePercentageforthisGroup);
            foreach (var grp in groups)
            {
                var benefits = from g in grp where !String.IsNullOrEmpty(g.SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis) || !String.IsNullOrEmpty(g.SelecttheNonMedicarecoveredservicecategoriesincludedintheOONoptionfort) select g;
                string[] medicareCodes = benefits.Select(a => a.SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis.Replace("[", "").Replace("]", "")).ToArray();
                string[] nonMedicareCodes = benefits.Select(a => a.SelecttheNonMedicarecoveredservicecategoriesincludedintheOONoptionfort.Replace("[", "").Replace("]", "")).ToArray();
                string medicareCodeStr = OONGroupsHelper.GenerateJSONArrayString(medicareCodes, false);
                string nonMedicareCodeStr = OONGroupsHelper.GenerateJSONArrayString(nonMedicareCodes, false);

                OONGroupTargetModel res = (from ben in benefits
                                           select new OONGroupTargetModel
                                           {
                                               EnterDeductibleAmountforthisgroup = ben.EnterDeductibleAmountforthisgroup,
                                               EnterLabelforthisGroupOptional = ben.EnterLabelforthisGroupOptional,
                                               EnterMaximumCoinsurancePercentageforthisGroup = ben.EnterMaximumCoinsurancePercentageforthisGroup,
                                               EnterMaximumCopaymentAmountforthisGroup = ben.EnterMaximumCopaymentAmountforthisGroup,
                                               EnterMinimumCoinsurancePercentageforthisGroup = ben.EnterMinimumCoinsurancePercentageforthisGroup,
                                               EnterMinimumCopaymentAmountforthisGroup = ben.EnterMinimumCopaymentAmountforthisGroup,
                                               Indicatemaximumplanbenefitcoverageamount = ben.Indicatemaximumplanbenefitcoverageamount,
                                               Isthereamaximumplanbenefitcoverageamountforthisgroup = ben.Isthereamaximumplanbenefitcoverageamountforthisgroup,
                                               IsthereanOONCoinsuranceforthisGroup = ben.IsthereanOONCoinsuranceforthisGroup,
                                               IsthereanOONCopaymentforthisGroup = ben.IsthereanOONCopaymentforthisGroup,
                                               IsthereanOONDeductibleforthisgroup = ben.IsthereanOONDeductibleforthisgroup,
                                               SelectthebenefitsthatapplytotheOONGroups = ben.SelectthebenefitsthatapplytotheOONGroups,
                                               SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis = ben.SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis,
                                               SelecttheNonMedicarecoveredservicecategoriesincludedintheOONoptionfort = ben.SelecttheNonMedicarecoveredservicecategoriesincludedintheOONoptionfort,
                                               CoinsuranceMax = ben.CoinsuranceMax,
                                               CoinsuranceMin = ben.CoinsuranceMin,
                                               CopayMax = ben.CopayMax,
                                               CopayMin = ben.CopayMin
                                           }).First();
                res.SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis = medicareCodeStr;
                res.SelecttheNonMedicarecoveredservicecategoriesincludedintheOONoptionfort = nonMedicareCodeStr;
                if(grp.Count() > 1)
                {
                    res.EnterLabelforthisGroupOptional = grp.Key + "%";
                }
                result.Add(res);
            }
            return result;
        }

        private List<OONGroupTargetModel> GetRangeCoinsuranceGroups(List<OONGroupTargetModel> coinsurances)
        {
            List<OONGroupTargetModel> result = new List<OONGroupTargetModel>();
            var groups = coinsurances.Where(a => (a.EnterMinimumCoinsurancePercentageforthisGroup != a.EnterMaximumCoinsurancePercentageforthisGroup) && (a.Isthereamaximumplanbenefitcoverageamountforthisgroup == "2") && (a.IsthereanOONDeductibleforthisgroup == "2")).GroupBy(b => new { Min = b.CoinsuranceMin, Max = b.CoinsuranceMax });
            foreach (var grp in groups)
            {
                var benefits = from g in grp where !String.IsNullOrEmpty(g.SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis) || !String.IsNullOrEmpty(g.SelecttheNonMedicarecoveredservicecategoriesincludedintheOONoptionfort) select g;
                string[] medicareCodes = benefits.Select(a => a.SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis.Replace("[", "").Replace("]", "")).ToArray();
                string[] nonMedicareCodes = benefits.Select(a => a.SelecttheNonMedicarecoveredservicecategoriesincludedintheOONoptionfort.Replace("[", "").Replace("]", "")).ToArray();
                string medicareCodeStr = OONGroupsHelper.GenerateJSONArrayString(medicareCodes, false);
                string nonMedicareCodeStr = OONGroupsHelper.GenerateJSONArrayString(nonMedicareCodes, false);

                OONGroupTargetModel res = (from ben in benefits
                                           select new OONGroupTargetModel
                                           {
                                               EnterDeductibleAmountforthisgroup = ben.EnterDeductibleAmountforthisgroup,
                                               EnterLabelforthisGroupOptional = ben.EnterLabelforthisGroupOptional,
                                               EnterMaximumCoinsurancePercentageforthisGroup = grp.Key.Max.ToString(),
                                               EnterMaximumCopaymentAmountforthisGroup = ben.EnterMaximumCopaymentAmountforthisGroup,
                                               EnterMinimumCoinsurancePercentageforthisGroup = grp.Key.Min.ToString(),
                                               EnterMinimumCopaymentAmountforthisGroup = ben.EnterMinimumCopaymentAmountforthisGroup,
                                               Indicatemaximumplanbenefitcoverageamount = ben.Indicatemaximumplanbenefitcoverageamount,
                                               Isthereamaximumplanbenefitcoverageamountforthisgroup = ben.Isthereamaximumplanbenefitcoverageamountforthisgroup,
                                               IsthereanOONCoinsuranceforthisGroup = ben.IsthereanOONCoinsuranceforthisGroup,
                                               IsthereanOONCopaymentforthisGroup = ben.IsthereanOONCopaymentforthisGroup,
                                               IsthereanOONDeductibleforthisgroup = ben.IsthereanOONDeductibleforthisgroup,
                                               SelectthebenefitsthatapplytotheOONGroups = ben.SelectthebenefitsthatapplytotheOONGroups,
                                               SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis = ben.SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis,
                                               SelecttheNonMedicarecoveredservicecategoriesincludedintheOONoptionfort = ben.SelecttheNonMedicarecoveredservicecategoriesincludedintheOONoptionfort,
                                               CoinsuranceMax = grp.Key.Max,
                                               CoinsuranceMin = grp.Key.Min,
                                               CopayMax = ben.CopayMax,
                                               CopayMin = ben.CopayMin
                                           }).First();
                res.SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis = medicareCodeStr;
                res.SelecttheNonMedicarecoveredservicecategoriesincludedintheOONoptionfort = nonMedicareCodeStr;
                if (grp.Count() > 1)
                {
                    res.EnterLabelforthisGroupOptional = grp.Key.Min + "-" + grp.Key.Max + "%";
                }
                result.Add(res);
            }
            return result;
        }

        private List<OONGroupTargetModel> GetRangeCopayGroups(List<OONGroupTargetModel> copays)
        {
            List<OONGroupTargetModel> result = new List<OONGroupTargetModel>();
            var groups = copays.Where(a => (a.EnterMinimumCopaymentAmountforthisGroup != a.EnterMaximumCopaymentAmountforthisGroup) && (a.Isthereamaximumplanbenefitcoverageamountforthisgroup == "2") && (a.IsthereanOONDeductibleforthisgroup == "2")).GroupBy(b => new { Min = b.CopayMin, Max = b.CopayMax });
            foreach (var grp in groups)
            {
                var benefits = from g in grp where !String.IsNullOrEmpty(g.SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis) || !String.IsNullOrEmpty(g.SelecttheNonMedicarecoveredservicecategoriesincludedintheOONoptionfort) select g;
                string[] medicareCodes = benefits.Select(a => a.SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis.Replace("[", "").Replace("]", "")).ToArray();
                string[] nonMedicareCodes = benefits.Select(a => a.SelecttheNonMedicarecoveredservicecategoriesincludedintheOONoptionfort.Replace("[", "").Replace("]", "")).ToArray();
                string medicareCodeStr = OONGroupsHelper.GenerateJSONArrayString(medicareCodes, false);
                string nonMedicareCodeStr = OONGroupsHelper.GenerateJSONArrayString(nonMedicareCodes, false);

                OONGroupTargetModel res = (from ben in benefits
                                           select new OONGroupTargetModel
                                           {
                                               EnterDeductibleAmountforthisgroup = ben.EnterDeductibleAmountforthisgroup,
                                               EnterLabelforthisGroupOptional = ben.EnterLabelforthisGroupOptional,
                                               EnterMaximumCoinsurancePercentageforthisGroup = ben.EnterMaximumCoinsurancePercentageforthisGroup,
                                               EnterMaximumCopaymentAmountforthisGroup = grp.Key.Max.Value.ToString("F2"),
                                               EnterMinimumCoinsurancePercentageforthisGroup = ben.EnterMinimumCoinsurancePercentageforthisGroup,
                                               EnterMinimumCopaymentAmountforthisGroup = grp.Key.Min.Value.ToString("F2"),
                                               Indicatemaximumplanbenefitcoverageamount = ben.Indicatemaximumplanbenefitcoverageamount,
                                               Isthereamaximumplanbenefitcoverageamountforthisgroup = ben.Isthereamaximumplanbenefitcoverageamountforthisgroup,
                                               IsthereanOONCoinsuranceforthisGroup = ben.IsthereanOONCoinsuranceforthisGroup,
                                               IsthereanOONCopaymentforthisGroup = ben.IsthereanOONCopaymentforthisGroup,
                                               IsthereanOONDeductibleforthisgroup = ben.IsthereanOONDeductibleforthisgroup,
                                               SelectthebenefitsthatapplytotheOONGroups = ben.SelectthebenefitsthatapplytotheOONGroups,
                                               SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis = ben.SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis,
                                               SelecttheNonMedicarecoveredservicecategoriesincludedintheOONoptionfort = ben.SelecttheNonMedicarecoveredservicecategoriesincludedintheOONoptionfort,
                                               CoinsuranceMax = ben.CoinsuranceMax,
                                               CoinsuranceMin = ben.CoinsuranceMin,
                                               CopayMax = grp.Key.Max,
                                               CopayMin = grp.Key.Min
                                           }).First();
                res.SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis = medicareCodeStr;
                res.SelecttheNonMedicarecoveredservicecategoriesincludedintheOONoptionfort = nonMedicareCodeStr;
                if (grp.Count() > 1)
                {
                    res.EnterLabelforthisGroupOptional = "$" + grp.Key.Min.Value.ToString("F2") + "-" + grp.Key.Max.Value.ToString("F2");
                }
                result.Add(res);
            }
            return result;
        }

        private List<OONGroupTargetModel> GetEqualCopayGroups(List<OONGroupTargetModel> copays)
        {
            List<OONGroupTargetModel> result = new List<OONGroupTargetModel>();
            var groups = copays.Where(a => (a.EnterMinimumCopaymentAmountforthisGroup == a.EnterMaximumCopaymentAmountforthisGroup) && (a.Isthereamaximumplanbenefitcoverageamountforthisgroup == "2") && (a.IsthereanOONDeductibleforthisgroup == "2")).GroupBy(b => b.EnterMinimumCopaymentAmountforthisGroup);
            foreach (var grp in groups)
            {
                var benefits = from g in grp where !String.IsNullOrEmpty(g.SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis) || !String.IsNullOrEmpty(g.SelecttheNonMedicarecoveredservicecategoriesincludedintheOONoptionfort) select g;
                string[] medicareCodes = benefits.Select(a => a.SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis.Replace("[", "").Replace("]", "")).ToArray();
                string[] nonMedicareCodes = benefits.Select(a => a.SelecttheNonMedicarecoveredservicecategoriesincludedintheOONoptionfort.Replace("[", "").Replace("]", "")).ToArray();
                string medicareCodeStr = OONGroupsHelper.GenerateJSONArrayString(medicareCodes, false);
                string nonMedicareCodeStr = OONGroupsHelper.GenerateJSONArrayString(nonMedicareCodes, false);

                OONGroupTargetModel res = (from ben in benefits
                                           select new OONGroupTargetModel
                                           {
                                               EnterDeductibleAmountforthisgroup = ben.EnterDeductibleAmountforthisgroup,
                                               EnterLabelforthisGroupOptional = ben.EnterLabelforthisGroupOptional,
                                               EnterMaximumCoinsurancePercentageforthisGroup = ben.EnterMaximumCoinsurancePercentageforthisGroup,
                                               EnterMaximumCopaymentAmountforthisGroup = ben.EnterMaximumCopaymentAmountforthisGroup,
                                               EnterMinimumCoinsurancePercentageforthisGroup = ben.EnterMinimumCoinsurancePercentageforthisGroup,
                                               EnterMinimumCopaymentAmountforthisGroup = ben.EnterMinimumCopaymentAmountforthisGroup,
                                               Indicatemaximumplanbenefitcoverageamount = ben.Indicatemaximumplanbenefitcoverageamount,
                                               Isthereamaximumplanbenefitcoverageamountforthisgroup = ben.Isthereamaximumplanbenefitcoverageamountforthisgroup,
                                               IsthereanOONCoinsuranceforthisGroup = ben.IsthereanOONCoinsuranceforthisGroup,
                                               IsthereanOONCopaymentforthisGroup = ben.IsthereanOONCopaymentforthisGroup,
                                               IsthereanOONDeductibleforthisgroup = ben.IsthereanOONDeductibleforthisgroup,
                                               SelectthebenefitsthatapplytotheOONGroups = ben.SelectthebenefitsthatapplytotheOONGroups,
                                               SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis = ben.SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis,
                                               SelecttheNonMedicarecoveredservicecategoriesincludedintheOONoptionfort = ben.SelecttheNonMedicarecoveredservicecategoriesincludedintheOONoptionfort,
                                               CoinsuranceMax = ben.CoinsuranceMax,
                                               CoinsuranceMin = ben.CoinsuranceMin,
                                               CopayMax = ben.CopayMax,
                                               CopayMin = ben.CopayMin
                                           }).First();
                res.SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis = medicareCodeStr;
                res.SelecttheNonMedicarecoveredservicecategoriesincludedintheOONoptionfort = nonMedicareCodeStr;
                if (grp.Count() > 1)
                {
                    res.EnterLabelforthisGroupOptional = "$" + grp.Key;
                }
                result.Add(res);
            }
            return result;
        }



        private List<OONGroupTargetModel> GetRangeCoinsurances(List<OONGroupTargetModel> coinsurances)
        {
            List<OONGroupTargetModel> result = new List<OONGroupTargetModel>();
            var coInsurances = coinsurances.Where(a => (a.EnterMinimumCoinsurancePercentageforthisGroup != a.EnterMaximumCoinsurancePercentageforthisGroup) && (a.Isthereamaximumplanbenefitcoverageamountforthisgroup == "2") && (a.IsthereanOONDeductibleforthisgroup == "2"));
            if(coInsurances.Count() > 0)
            {
                result.AddRange(coInsurances);
            }
            return result;
        }


        private List<OONGroupTargetModel> GetRangeCopays(List<OONGroupTargetModel> copays)
        {
            List<OONGroupTargetModel> result = new List<OONGroupTargetModel>();
            var rangeCopays = copays.Where(a => (a.EnterMinimumCopaymentAmountforthisGroup != a.EnterMaximumCopaymentAmountforthisGroup) && (a.Isthereamaximumplanbenefitcoverageamountforthisgroup == "2") && (a.IsthereanOONDeductibleforthisgroup == "2"));
            if (rangeCopays.Count() > 0)
            {
                result.AddRange(rangeCopays);
            }
            return result;
        }

        private OONGroupTargetModel GroupCoinsurances(List<OONGroupTargetModel> equalCoinsurances, List<OONGroupTargetModel> rangeCoinsurances)
        {
            OONGroupTargetModel result = null;
            List<OONGroupTargetModel> allCoinsurances = new List<OONGroupTargetModel>();
            allCoinsurances.AddRange(equalCoinsurances);
            allCoinsurances.AddRange(rangeCoinsurances);
            if(allCoinsurances.Count > 0)
            {
                double? minCoinsurance = allCoinsurances.Min(a => a.CoinsuranceMin);
                OONGroupTargetModel minModel = allCoinsurances.Where(a => a.CoinsuranceMin == minCoinsurance).First();

                double? maxCoinsurance = allCoinsurances.Max(a => a.CoinsuranceMax);
                OONGroupTargetModel maxModel = allCoinsurances.Where(a => a.CoinsuranceMax == maxCoinsurance).First();
                var medBenefits = from coins in allCoinsurances where !String.IsNullOrEmpty(coins.SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis) select coins;
                string[] medicareCodes = medBenefits.Select(a => a.SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis).ToArray();
                var nonMedBenefits = from coins in allCoinsurances where !String.IsNullOrEmpty(coins.SelecttheNonMedicarecoveredservicecategoriesincludedintheOONoptionfort) select coins;
                string[] nonMedicareCodes = nonMedBenefits.Select(a => a.SelecttheNonMedicarecoveredservicecategoriesincludedintheOONoptionfort).ToArray();
                string medicareCodeStr = OONGroupsHelper.GenerateJSONArrayString(medicareCodes, false);
                string nonMedicareCodeStr = OONGroupsHelper.GenerateJSONArrayString(nonMedicareCodes, false);

                string label = "";
                if(minModel.CoinsuranceMin == maxModel.CoinsuranceMax)
                {
                    label = minModel.EnterMinimumCoinsurancePercentageforthisGroup + "%";
                }
                else
                {
                    label = minModel.EnterMinimumCoinsurancePercentageforthisGroup + "%-" + maxModel.EnterMaximumCoinsurancePercentageforthisGroup + "%";
                }

                result = new OONGroupTargetModel
                {
                    CoinsuranceMax = maxCoinsurance,
                    CoinsuranceMin = minCoinsurance,
                    EnterDeductibleAmountforthisgroup = minModel.EnterDeductibleAmountforthisgroup,
                    CopayMax = minModel.CopayMax,
                    CopayMin = minModel.CopayMin,
                    EnterLabelforthisGroupOptional = label,
                    EnterMaximumCoinsurancePercentageforthisGroup = maxModel.EnterMaximumCoinsurancePercentageforthisGroup,
                    EnterMaximumCopaymentAmountforthisGroup = maxModel.EnterMaximumCopaymentAmountforthisGroup,
                    EnterMinimumCoinsurancePercentageforthisGroup = minModel.EnterMinimumCoinsurancePercentageforthisGroup,
                    EnterMinimumCopaymentAmountforthisGroup = minModel.EnterMinimumCopaymentAmountforthisGroup,
                    Indicatemaximumplanbenefitcoverageamount = minModel.Indicatemaximumplanbenefitcoverageamount,
                    Isthereamaximumplanbenefitcoverageamountforthisgroup = minModel.Isthereamaximumplanbenefitcoverageamountforthisgroup,
                    IsthereanOONCoinsuranceforthisGroup = minModel.IsthereanOONCoinsuranceforthisGroup,
                    IsthereanOONCopaymentforthisGroup = minModel.IsthereanOONCopaymentforthisGroup,
                    IsthereanOONDeductibleforthisgroup = minModel.IsthereanOONDeductibleforthisgroup,
                    SelectthebenefitsthatapplytotheOONGroups = "",
                    SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis = medicareCodeStr,
                    SelecttheNonMedicarecoveredservicecategoriesincludedintheOONoptionfort = nonMedicareCodeStr
                };
            }
            return result;
        }

        private OONGroupTargetModel GroupCopays(List<OONGroupTargetModel> equalCopays, List<OONGroupTargetModel> rangeCopays)
        {
            OONGroupTargetModel result = null;
            List<OONGroupTargetModel> allCopays = new List<OONGroupTargetModel>();
            allCopays.AddRange(equalCopays);
            allCopays.AddRange(rangeCopays);
            if(allCopays.Count > 0)
            {
                double? minCopay = allCopays.Min(a => a.CopayMin);
                OONGroupTargetModel minModel = allCopays.Where(a => a.CopayMin == minCopay).First();

                double? maxCopay = allCopays.Max(a => a.CopayMax);
                OONGroupTargetModel maxModel = allCopays.Where(a => a.CopayMax == maxCopay).First();
                var medBenefits = from copay in allCopays where !String.IsNullOrEmpty(copay.SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis) select copay;
                string[] medicareCodes = medBenefits.Select(a => a.SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis).ToArray();
                var nonMedBenefits = from copay in allCopays where !String.IsNullOrEmpty(copay.SelecttheNonMedicarecoveredservicecategoriesincludedintheOONoptionfort) select copay;
                string[] nonMedicareCodes = nonMedBenefits.Select(a => a.SelecttheNonMedicarecoveredservicecategoriesincludedintheOONoptionfort).ToArray();
                string medicareCodeStr = OONGroupsHelper.GenerateJSONArrayString(medicareCodes, false);
                string nonMedicareCodeStr = OONGroupsHelper.GenerateJSONArrayString(nonMedicareCodes, false);

                string label = "";
                if (minModel.CopayMin == maxModel.CopayMax)
                {
                    label = "$" + minModel.EnterMinimumCopaymentAmountforthisGroup;
                }
                else
                {
                    label = "$" + minModel.EnterMinimumCopaymentAmountforthisGroup + "-" + maxModel.EnterMaximumCopaymentAmountforthisGroup;
                }

                result = new OONGroupTargetModel
                {
                    CoinsuranceMax = minModel.CoinsuranceMax,
                    CoinsuranceMin = minModel.CoinsuranceMin,
                    EnterDeductibleAmountforthisgroup = minModel.EnterDeductibleAmountforthisgroup,
                    CopayMax = maxCopay,
                    CopayMin = minCopay,
                    EnterLabelforthisGroupOptional = label,
                    EnterMaximumCoinsurancePercentageforthisGroup = maxModel.EnterMaximumCoinsurancePercentageforthisGroup,
                    EnterMaximumCopaymentAmountforthisGroup = maxModel.EnterMaximumCopaymentAmountforthisGroup,
                    EnterMinimumCoinsurancePercentageforthisGroup = minModel.EnterMinimumCoinsurancePercentageforthisGroup,
                    EnterMinimumCopaymentAmountforthisGroup = minModel.EnterMinimumCopaymentAmountforthisGroup,
                    Indicatemaximumplanbenefitcoverageamount = minModel.Indicatemaximumplanbenefitcoverageamount,
                    Isthereamaximumplanbenefitcoverageamountforthisgroup = minModel.Isthereamaximumplanbenefitcoverageamountforthisgroup,
                    IsthereanOONCoinsuranceforthisGroup = minModel.IsthereanOONCoinsuranceforthisGroup,
                    IsthereanOONCopaymentforthisGroup = minModel.IsthereanOONCopaymentforthisGroup,
                    IsthereanOONDeductibleforthisgroup = minModel.IsthereanOONDeductibleforthisgroup,
                    SelectthebenefitsthatapplytotheOONGroups = "",
                    SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis = medicareCodeStr,
                    SelecttheNonMedicarecoveredservicecategoriesincludedintheOONoptionfort = nonMedicareCodeStr
                };
            }
            return result;
        }

        private OONGroupTargetModel GroupCombineds(List<OONGroupTargetModel> combineds)
        {
            OONGroupTargetModel result = null;
            if (combineds.Count > 0)
            {
                double? minCoinsurance = combineds.Min(a => a.CoinsuranceMin);
                double? maxCoinsurance = combineds.Max(a => a.CoinsuranceMax);
                double? minCopay = combineds.Min(a => a.CopayMin);
                double? maxCopay = combineds.Max(a => a.CopayMax);

                var medBenefits = from com in combineds where !String.IsNullOrEmpty(com.SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis) select com;
                string[] medicareCodes = medBenefits.Select(a => a.SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis).ToArray();
                var nonMedBenefits = from com in combineds where !String.IsNullOrEmpty(com.SelecttheNonMedicarecoveredservicecategoriesincludedintheOONoptionfort) select com;
                string[] nonMedicareCodes = nonMedBenefits.Select(a => a.SelecttheNonMedicarecoveredservicecategoriesincludedintheOONoptionfort).ToArray();
                string medicareCodeStr = OONGroupsHelper.GenerateJSONArrayString(medicareCodes, false);
                string nonMedicareCodeStr = OONGroupsHelper.GenerateJSONArrayString(nonMedicareCodes, false);

                string label = "";

                if (minCopay == maxCopay)
                {
                    label = "$" + minCopay;
                }
                else
                {
                    label = "$" + minCopay + "-" + maxCopay;
                }

                if (minCoinsurance == maxCoinsurance)
                {
                    label = label + " & " + minCoinsurance + "%";
                }
                else
                {
                    label = label + " & " + minCoinsurance + "-" + maxCoinsurance + "%";
                }

                result = new OONGroupTargetModel
                {
                    CoinsuranceMax = maxCoinsurance,
                    CoinsuranceMin = minCoinsurance,
                    EnterDeductibleAmountforthisgroup = "",
                    CopayMax = maxCopay,
                    CopayMin = minCopay,
                    EnterLabelforthisGroupOptional = label,
                    EnterMaximumCoinsurancePercentageforthisGroup = maxCoinsurance.ToString(),
                    EnterMaximumCopaymentAmountforthisGroup = maxCopay.Value.ToString("F2"),
                    EnterMinimumCoinsurancePercentageforthisGroup = minCoinsurance.ToString(),
                    EnterMinimumCopaymentAmountforthisGroup = minCopay.Value.ToString("F2"),
                    Indicatemaximumplanbenefitcoverageamount = "",
                    Isthereamaximumplanbenefitcoverageamountforthisgroup = "2",
                    IsthereanOONCoinsuranceforthisGroup = "1",
                    IsthereanOONCopaymentforthisGroup = "1",
                    IsthereanOONDeductibleforthisgroup = "2",
                    SelectthebenefitsthatapplytotheOONGroups = "",
                    SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis = medicareCodeStr,
                    SelecttheNonMedicarecoveredservicecategoriesincludedintheOONoptionfort = nonMedicareCodeStr
                };
            }
            return result;
        }

        private List<OONGroupTargetModel> GetCoinsuranceBenefits(List<OONGroupTargetModel> otherBenefits)
        {
            return otherBenefits.Where(a => a.IsthereanOONCoinsuranceforthisGroup == "1" && a.IsthereanOONCopaymentforthisGroup == "2" && a.Isthereamaximumplanbenefitcoverageamountforthisgroup == "2").ToList();
        }

        private List<OONGroupTargetModel> GetCopayBenefits(List<OONGroupTargetModel> otherBenefits)
        {
            return otherBenefits.Where(a => a.IsthereanOONCoinsuranceforthisGroup == "2" && a.IsthereanOONCopaymentforthisGroup == "1" && a.Isthereamaximumplanbenefitcoverageamountforthisgroup == "2").ToList();
        }

        private List<OONGroupTargetModel> GetCombinedBenefits(List<OONGroupTargetModel> otherBenefits)
        {
            return otherBenefits.Where(a => a.IsthereanOONCoinsuranceforthisGroup == "1" && a.IsthereanOONCopaymentforthisGroup == "1" && a.Isthereamaximumplanbenefitcoverageamountforthisgroup == "2").ToList();
        }


        private List<OONGroupTargetModel> GetPlanMaxAndDeductibleBenefits(List<OONGroupTargetModel> otherBenefits)
        {
            return otherBenefits.Where(a => a.Isthereamaximumplanbenefitcoverageamountforthisgroup == "1" || a.IsthereanOONDeductibleforthisgroup == "1").ToList();
        }
        private List<OONGroupTargetModel> ProcessFinalGroupingForOtherBenefits(List<OONGroupTargetModel> otherBenefits,int packageCount,bool zdCondition)
        {
            List<OONGroupTargetModel> results = new List<OONGroupTargetModel>();
            List<OONGroupTargetModel> coInsurances = GetCoinsuranceBenefits(otherBenefits);
            List<OONGroupTargetModel> copays = GetCopayBenefits(otherBenefits);
            List<OONGroupTargetModel> combineds = GetCombinedBenefits(otherBenefits);
            List<OONGroupTargetModel> equalCoinsurances = GetEqualCoinsuranceGroups(coInsurances);
            List<OONGroupTargetModel> equalCopays = GetEqualCopayGroups(copays);
            List<OONGroupTargetModel> rangeCoinsurances = GetRangeCoinsurances(coInsurances);
            List<OONGroupTargetModel> rangeCopays = GetRangeCopays(copays);

            List<OONGroupTargetModel> planMaxes = GetPlanMaxAndDeductibleBenefits(otherBenefits);
            
            int planMaxesCount = planMaxes.Count;

            int zdCount = 0;
            OONGroupTargetModel zdModel = null;
            if (zdCondition == true)
            {
                var equalCopay = equalCopays.Where(a => a.CopayMin != null && a.CopayMin.Value == 0);
                if(equalCopay.Count() > 0)
                {
                    var eqCop = equalCopay.First();
                    eqCop.EnterLabelforthisGroupOptional = "$0.00";
                    if (!String.IsNullOrEmpty(eqCop.SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis))
                    {
                        if (!eqCop.SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis.Contains("14a"))
                        {
                            eqCop.SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis = eqCop.SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis + "," + "14a";
                        }
                    }
                    else
                    {
                        eqCop.SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis = "14a";
                    }
                }
                else
                {
                    zdCount = 1;
                    zdModel = new OONGroupTargetModel();
                    zdModel.CoinsuranceMax = null;
                    zdModel.CoinsuranceMin = null;
                    zdModel.CopayMax = 0;
                    zdModel.CopayMin = 0;
                    zdModel.EnterDeductibleAmountforthisgroup = "";
                    zdModel.EnterLabelforthisGroupOptional = "$0.00";
                    zdModel.EnterMaximumCoinsurancePercentageforthisGroup = "";
                    zdModel.EnterMaximumCopaymentAmountforthisGroup = "0";
                    zdModel.EnterMinimumCoinsurancePercentageforthisGroup = "";
                    zdModel.EnterMinimumCopaymentAmountforthisGroup = "0";
                    zdModel.Indicatemaximumplanbenefitcoverageamount = "";
                    zdModel.Isthereamaximumplanbenefitcoverageamountforthisgroup = "2";
                    zdModel.IsthereanOONCoinsuranceforthisGroup = "2";
                    zdModel.IsthereanOONCopaymentforthisGroup = "1";
                    zdModel.IsthereanOONDeductibleforthisgroup = "2";
                    zdModel.SelectthebenefitsthatapplytotheOONGroups = "01";
                    zdModel.SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis = "14a";
                    zdModel.SelecttheNonMedicarecoveredservicecategoriesincludedintheOONoptionfort = "";
                }
            }

            List<List<OONGroupTargetModel>> groups = new List<List<OONGroupTargetModel>>();
            //group copays except equal zero copays
            List<OONGroupTargetModel> zeroCopays = new List<OONGroupTargetModel>();
            var zeroCopay = equalCopays.Where(a => a.CopayMin == 0);
            int zeroCopayCount = 0;
            if (zeroCopay != null && zeroCopay.Count() > 0)
            {
                zeroCopays = zeroCopay.ToList();
                if(zdCount == 0)
                {
                    zeroCopayCount = 1;
                }
            }
            List<OONGroupTargetModel> nonZeroCopayList = new List<OONGroupTargetModel>();
            var nonZeroCopays = equalCopays.Where(a => a.CopayMin > 0);
            if (nonZeroCopays != null)
            {
                equalCopays = nonZeroCopays.ToList();
            }
            groups.Add(equalCoinsurances);
            groups.Add(rangeCoinsurances);
            groups.Add(equalCopays);
            groups.Add(rangeCopays);
            groups.Add(combineds);
            int otherCount = GetGroupCount(groups);
            int totalGroupCount = packageCount + planMaxesCount + zdCount + zeroCopayCount + otherCount;
            int coinsuranceCount = 0;
            //if(totalGroupCount > 15)
            //{
                rangeCoinsurances = GetRangeCoinsuranceGroups(rangeCoinsurances);
                rangeCopays = GetRangeCopayGroups(rangeCopays);
            //}
            groups = new List<List<OONGroupTargetModel>>();
            groups.Add(equalCoinsurances);
            groups.Add(rangeCoinsurances);
            groups.Add(equalCopays);
            groups.Add(rangeCopays);
            groups.Add(combineds);
            otherCount = GetGroupCount(groups);
            totalGroupCount = packageCount + planMaxesCount + zdCount + zeroCopayCount + otherCount;
            if (totalGroupCount > 15)
            {
                //get equal coinsurances up to 50%
                var equalCoinsurancesUpto50Percent = equalCoinsurances.Where(e => e.CoinsuranceMin <= 50).ToList();
                //get equal coinsurances more than 50%
                var equalCoinsurancesAbove50Percent = equalCoinsurances.Where(e => e.CoinsuranceMin > 50).ToList();
                //get range coninsurances up to 50%
                var rangeCoinsurancesUpto50Percent = rangeCoinsurances.Where(e => e.CoinsuranceMax <= 50).ToList();
                //get range coinsurances more than 50%
                var rangeCoinsurancesAbove50Percent = rangeCoinsurances.Where(e => e.CoinsuranceMax > 50).ToList();

                OONGroupTargetModel coInsuranceGroupUpto50Percent = GroupCoinsurances(equalCoinsurancesUpto50Percent, rangeCoinsurancesUpto50Percent);
                OONGroupTargetModel coInsuranceGroupAbove50Percent = GroupCoinsurances(equalCoinsurancesAbove50Percent, rangeCoinsurancesAbove50Percent);
                if (coInsuranceGroupUpto50Percent != null)
                {
                    results.Add(coInsuranceGroupUpto50Percent);
                    coinsuranceCount = coinsuranceCount + 1;
                }
                if (coInsuranceGroupAbove50Percent != null)
                {
                    results.Add(coInsuranceGroupAbove50Percent);
                    coinsuranceCount = coinsuranceCount + 1;
                }
            }
            else
            {
                if(equalCoinsurances.Count > 0)
                {
                    results.AddRange(equalCoinsurances);
                }
                if(rangeCoinsurances.Count > 0)
                {
                    results.AddRange(rangeCoinsurances);
                }
                coinsuranceCount = equalCoinsurances.Count + rangeCoinsurances.Count;
            }
            groups = new List<List<OONGroupTargetModel>>();
            groups.Add(equalCopays);
            groups.Add(rangeCopays);
            groups.Add(combineds);
            int copayCount = GetGroupCount(groups);
            totalGroupCount = packageCount + planMaxesCount + zdCount + zeroCopayCount + coinsuranceCount + copayCount;
            if(totalGroupCount > 15)
            {
                OONGroupTargetModel copayGroup = GroupCopays(equalCopays, rangeCopays);
                if(copayGroup != null)
                {
                    copayCount = 1;
                    results.Add(copayGroup);
                }
            }
            else
            {
                results.AddRange(equalCopays);
                results.AddRange(rangeCopays);
                copayCount = equalCopays.Count + rangeCopays.Count;
            }

            groups = new List<List<OONGroupTargetModel>>();
            groups.Add(combineds);
            int combinedCount = GetGroupCount(groups);
            totalGroupCount = packageCount + planMaxesCount + zdCount + zeroCopayCount + coinsuranceCount + copayCount + combinedCount;
            if (totalGroupCount > 15)
            {
                OONGroupTargetModel combinedGroup = GroupCombineds(combineds);
                if (combinedGroup != null)
                {
                    results.Add(combinedGroup);
                }
            }
            else
            {
                results.AddRange(combineds);
            }


            if (zdModel != null && zdCount > 0)
            {
                results.Add(zdModel);
            }
            else
            {
                if(zeroCopays != null && zeroCopays.Count > 0)
                {
                    results.Add(zeroCopays.First());
                }
            }
            results.AddRange(planMaxes);
            return results;
        }

        private OONGroupEntryModel GetZeroDollarEntry(List<OONGroupEntryModel> models)
        {
            OONGroupEntryModel result = null;
            var entry = from model in models where model.BenefitName == "Medicare Zero Dollar Service" select model;
            if(entry.Count() > 0)
            {
                result = entry.First();
            }
            return result;
        }

        private JArray GetArray(string codes)
        {
            JArray arr = new JArray();
            foreach (var code in codes.Split(','))
            {
                arr.Add(code);
            }
            return arr;
        }

        private JArray GetBenefitsApplyValue(string medicareCodesStr, string nonMedicareCodesStr)
        {
            JArray arr = new JArray();
            if (!String.IsNullOrEmpty(medicareCodesStr))
            {
                arr.Add("01");
            }
            if (!String.IsNullOrEmpty(nonMedicareCodesStr))
            {
                arr.Add("10");
            }
            return arr;
        }

        private int GetGroupCount(List<List<OONGroupTargetModel>> models)
        {
            int count = 0;
            foreach(var model in models)
            {
                count = count + model.Count;
            }
            return count;
        }
    }
}
