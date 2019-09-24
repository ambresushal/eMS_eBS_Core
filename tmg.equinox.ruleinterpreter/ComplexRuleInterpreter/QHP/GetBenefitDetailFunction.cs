using Newtonsoft.Json.Linq;
using tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Globalization;
using Newtonsoft.Json;

namespace tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.QHP
{
    class GetBenefitDetailFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            Parser.Result result = new Parser.Result();

            string planId = SourceManager.Get(Thread.CurrentThread, "PlanID").ToString();
            string planMarketingName = SourceManager.Get(Thread.CurrentThread, "PlanMarketingName").ToString();
            string levelofCoverage = SourceManager.Get(Thread.CurrentThread, "LevelofCoverage").ToString();
            string variationType = SourceManager.Get(Thread.CurrentThread, "CSRVariationType").ToString();
            string marketType = SourceManager.Get(Thread.CurrentThread, "MarketType").ToString();
            string state = SourceManager.Get(Thread.CurrentThread, "State").ToString();

            var benefitReviewGrid = SourceManager.Get(Thread.CurrentThread, "BRG");
            List<BenefitReview> brgServices = JsonConvert.DeserializeObject<List<BenefitReview>>(benefitReviewGrid.ToString());

            var qhpServices = SourceManager.Get(Thread.CurrentThread, "QHPServices");
            List<EssentialHealthBenefit> essentialBenefits = JsonConvert.DeserializeObject<List<EssentialHealthBenefit>>(qhpServices.ToString());
            List<EssentialHealthBenefit> reqServices = essentialBenefits.Where(s => s.State == state && s.MarketCoverage == marketType).ToList();

            var networks = SourceManager.Get(Thread.CurrentThread, "Networks");
            List<QHPNetwork> mlNetworks = JsonConvert.DeserializeObject<List<QHPNetwork>>(networks.ToString());

            var rxTierDetails = SourceManager.Get(Thread.CurrentThread, "RxTierDetails");
            List<RxTierDetail> tierList = JsonConvert.DeserializeObject<List<RxTierDetail>>(rxTierDetails.ToString());

            List<PlanBenefit> planBenefitInfo = new List<PlanBenefit>();
            var planName = SourceManager.Get(Thread.CurrentThread, "PlanName").ToString();
            var noOONPlanTypes = SourceManager.Get(Thread.CurrentThread, "noOONPlanTypes").ToString();
            List<NoOONPlanTypeList> noOONPlanTypeList = JsonConvert.DeserializeObject<List<NoOONPlanTypeList>>(noOONPlanTypes.ToString());
            var oONBehaviorList = SourceManager.Get(Thread.CurrentThread, "HMOEPOsOONBehaviorList").ToString();
            List<OONBehaviorBenefit> OONBehaviorList = JsonConvert.DeserializeObject<List<OONBehaviorBenefit>>(oONBehaviorList.ToString());
            bool isNoOONPlan = BenefitHelper.IsNoOONPlanType(planName.ToString(), noOONPlanTypeList);
            foreach (var service in reqServices)
            {

                PlanBenefit benefit = BenefitHelper.GetPlanBenefit();
                benefit.Benefit = service.Benefits;
                benefit.HIOSPlanIDStandardComponentVariant = planId + "-0" + BenefitHelper.GetCSRVariationIndex(variationType.ToString());
                benefit.PlanMarketingName = planMarketingName.ToString();
                benefit.LevelofCoverageMetalLevel = levelofCoverage.ToString();
                benefit.CSRVariationType = BenefitHelper.CSRVariationTypeFormater(variationType.ToString(), levelofCoverage.ToString());
                
                if (string.Equals(service.IsCovered, "Yes", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var net in benefit.BenefitsTemplateNetworkList)
                    {
                        bool serviceFound = false;
                        var brgService = (from rs in reqServices
                                          join bs in brgServices on rs.BenefitServiceCode equals bs.BenefitServiceCode
                                          join nt in mlNetworks on bs.NetworkTier equals nt.NetworkTier
                                          where rs.Benefits == service.Benefits && nt.QHPNetworkName == net.NetworkName
                                          select bs).FirstOrDefault();

                        if (brgService != null)
                        {
                            serviceFound = true;
                            string indDeductible = brgService.IndDeductible;
                            string famDeductible = brgService.FamDeductible;
                            string language = service.Language;
                            string copay = brgService.Copay;
                            string coinsurance = brgService.Coinsurance;
                            string copayText = !string.IsNullOrWhiteSpace(language) ? string.Format(" Copay {0} after deductible", language) : " Copay after deductible";
                            bool hasNoDeductible = indDeductible == "Not Applicable" && famDeductible == "Not Applicable";

                            net.Copay = BenefitHelper.GetCopay(copay, copayText, indDeductible, famDeductible, net.NetworkName, variationType);
                            net.Coinsurance = BenefitHelper.GetCoinsurance(coinsurance, " Coinsurance after deductible", indDeductible, famDeductible, net.NetworkName, variationType);
                        }
                        else if (!string.IsNullOrEmpty(service.RxTierName))
                        {
                            var drugServices = (from rs in reqServices
                                                join bs in tierList on rs.RxTierName equals bs.RxService
                                                join nt in mlNetworks on bs.Network equals nt.NetworkTier
                                                where rs.Benefits == service.Benefits && nt.QHPNetworkName == net.NetworkName
                                                select bs).ToList();

                            var drugService = drugServices.Where(s => s.RxTierType == service.RxTierType).FirstOrDefault();

                            if (drugService != null)
                            {
                                serviceFound = true;
                                string indDeductible = drugService.IndividualDeductible;
                                string famDeductible = drugService.FamilyDeductible;
                                net.Copay = BenefitHelper.GetCopay(drugService.Copay, string.Empty, indDeductible, famDeductible, net.NetworkName, variationType);
                                net.Coinsurance = BenefitHelper.GetCoinsurance(drugService.Coinsurance, " Coinsurance after deductible", indDeductible, famDeductible, net.NetworkName, variationType);
                            }
                        }
                        
                        if (string.Equals(net.NetworkName, "Out of Network", StringComparison.OrdinalIgnoreCase) && serviceFound == false)
                        {
                            OONBehaviorBenefit oONBehaviorBenefit = BenefitHelper.GetHMOEPOsOONBehavior(benefit.Benefit, state, levelofCoverage,marketType, OONBehaviorList, variationType.ToString());
                            
                            if (isNoOONPlan && (oONBehaviorBenefit.OONCopay == BenefitHelper.SameasInNetworkTier1 || oONBehaviorBenefit.OONCopay == BenefitHelper.SameasInNetworkTier2))
                            {
                                net.Copay = benefit.BenefitsTemplateNetworkList[0].Copay;
                            }
                            else
                            {
                                net.Copay = oONBehaviorBenefit.OONCopay;
                            }
                            if (isNoOONPlan && (oONBehaviorBenefit.OONCoinsurance == BenefitHelper.SameasInNetworkTier1 || oONBehaviorBenefit.OONCoinsurance == BenefitHelper.SameasInNetworkTier2))
                            {
                                net.Coinsurance = benefit.BenefitsTemplateNetworkList[0].Coinsurance;
                            }
                            else
                            {
                                net.Coinsurance = oONBehaviorBenefit.OONCoinsurance;
                            }
                        }
                    }
                }
                planBenefitInfo.Add(benefit);
            }

            List<PlanBenefit> variantions = new List<PlanBenefit>();
            if (string.Equals(variationType, "Both (Display as On/Off Exchange)", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var row in planBenefitInfo)
                {
                    PlanBenefit variantRow = (PlanBenefit)row.Clone();
                    variantRow.HIOSPlanIDStandardComponentVariant = variantRow.HIOSPlanIDStandardComponentVariant.Replace("-00", "-01");
                    variantRow.CSRVariationType = "Standard " +
                                                  variantRow.LevelofCoverageMetalLevel +
                                                  " On Exchange Plan";
                    variantions.Add(variantRow);
                }
            }

            if (variantions.Count > 0)
            {
                planBenefitInfo.AddRange(variantions);
            }
            result.Token = JToken.FromObject(planBenefitInfo);
            return result;
        }
    }
}
