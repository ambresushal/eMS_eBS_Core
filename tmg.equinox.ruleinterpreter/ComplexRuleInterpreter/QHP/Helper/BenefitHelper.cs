using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.QHP
{
    public class BenefitHelper
    {
        public static string[] GetVariationTypeList()
        {
            string[] variationTypes = new string[] {
            "Off Exchange Plan",
            "On Exchange Plan",
            "Zero Cost Sharing Plan Variation",
            "Limited Cost Sharing Plan Variation",
            "73% AV Level Silver Plan",
            "87% AV Level Silver Plan",
            "94% AV Level Silver Plan" };
            return variationTypes;
        }
        public static PlanBenefit GetPlanBenefit()
        {
            PlanBenefit benefit = new PlanBenefit();
            benefit.BenefitsTemplateNetworkList = new List<CostShare>();
            benefit.BenefitsTemplateNetworkList.Add(new CostShare() { NetworkName = "In Network (Tier 1)", Coinsurance = "Not Applicable", Copay = "Not Applicable" });
            benefit.BenefitsTemplateNetworkList.Add(new CostShare() { NetworkName = "In Network (Tier 2)", Coinsurance = string.Empty, Copay = string.Empty });
            benefit.BenefitsTemplateNetworkList.Add(new CostShare() { NetworkName = "Out of Network", Coinsurance = "Not Applicable", Copay = "Not Applicable" });

            return benefit;
        }
        public static string GetCopay(string amount, string text, string indDeductible, string famDeductible, string network,string variationType)
        {
            bool hasDeductible = indDeductible == "Not Applicable" && famDeductible == "Not Applicable";
            string result = "Not Applicable";
            if (!string.IsNullOrEmpty(amount) && !string.Equals(amount, "Not Applicable", StringComparison.OrdinalIgnoreCase))
            {
                decimal copayAmount;
                if (decimal.TryParse(amount, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, CultureInfo.GetCultureInfo("en-US"), out copayAmount))
                {
                    if (copayAmount == 0 && network == "In Network (Tier 1)" && variationType!= "Zero Cost Sharing Plan Variation")
                    {
                        result = "No Charge" + (indDeductible == "Not Applicable" || indDeductible == string.Empty ? string.Empty : " after deductible");
                    }
                    else
                    {
                        result = copayAmount.ToString("C") + (hasDeductible ? string.Empty : text);
                    }
                }
            }
            return result;
        }

        public static string GetCoinsurance(string amount, string text, string indDeductible, string famDeductible, string network,string variationType)
        {
            bool hasDeductible = indDeductible == "Not Applicable" && famDeductible == "Not Applicable";
            string coins = amount.Replace("%", "");
            if (!string.IsNullOrEmpty(coins) && !string.Equals(coins, "Not Applicable", StringComparison.OrdinalIgnoreCase))
            {
                decimal coinsAmount;
                if (decimal.TryParse(coins, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, CultureInfo.CurrentCulture, out coinsAmount))
                {
                    if (coinsAmount == 0 && network == "In Network (Tier 1)" && variationType != "Zero Cost Sharing Plan Variation")
                    {
                        coins = "No Charge" + (indDeductible == "Not Applicable" || indDeductible == string.Empty ? string.Empty : " after deductible");
                    }
                    else
                    {
                        coins = coinsAmount.ToString("n2") + "%" + (hasDeductible ? string.Empty : text);
                    }
                }
            }
            return coins;
        }

        public static string CSRVariationTypeFormater(string variationType, string LevelofCoverageMetalLevel)
        {
            string resultText = string.Empty;
            if (variationType.Equals("On Exchange Plan") || variationType.Equals("Off Exchange Plan"))
            {
                resultText = "Standard " + LevelofCoverageMetalLevel + " " + variationType;
            }
            else if (variationType.Equals("Both (Display as On/Off Exchange)"))
            {
                resultText = "Standard " + LevelofCoverageMetalLevel + " Off Exchange Plan";
            }
            else
            {
                resultText = variationType;
            }
            return resultText;
        }

        public static string GetCSRVariationIndex(string variationType)
        {
            int index = 0;
            if (variationType.Equals("Both (Display as On/Off Exchange)"))
            {
                return "0";
            }
            else
            {
                index = Array.IndexOf(GetVariationTypeList(), variationType);
                return index.ToString();
            }
        }

        public static bool IsNoOONPlanType(string planType, List<NoOONPlanTypeList> noOONPlanTypeList)
        {
            bool result = false;
            if (noOONPlanTypeList != null)
            {
                if (noOONPlanTypeList.Count() > 0)
                {
                    result = noOONPlanTypeList.Where(s => s.PlanType.Equals(planType)).Any();
                }
            }
            return result;
        }

        public static OONBehaviorBenefit GetHMOEPOsOONBehavior(string benefit,string state,string metalTier,string marketType, List<OONBehaviorBenefit> OONBehaviorBenefitList,string cSRVariationType)
        {

            OONBehaviorBenefit oONBehaviorBenefit = OONBehaviorBenefitList.Where(s => s.QHPBenefitName.Equals(benefit)
                                 && s.State.Equals(state)
                                 && s.MetalTier.Equals(metalTier)
                                 && s.MarketCoverage.Equals(marketType)
                                 && s.CostShareVariationType.Equals(cSRVariationType)
                                 )
                                .FirstOrDefault();
            if (oONBehaviorBenefit == null)
            {
                oONBehaviorBenefit = new OONBehaviorBenefit();
            }
            return oONBehaviorBenefit;
        }

        public const string SameasInNetworkTier1 = "Same as In Network (Tier 1)";
        public const string SameasInNetworkTier2 = "Same as In Network (Tier 2)";

    }

    public class PlanBenefit : ICloneable
    {
        public string HIOSPlanIDStandardComponentVariant { get; set; }
        public string PlanMarketingName { get; set; }
        public string LevelofCoverageMetalLevel { get; set; }
        public string CSRVariationType { get; set; }
        public string Benefit { get; set; }
        public List<CostShare> BenefitsTemplateNetworkList { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class CostShare
    {
        public string NetworkName { get; set; }
        public string Copay { get; set; }
        public string Coinsurance { get; set; }
    }

    public class EssentialHealthBenefit
    {
        public string Benefits { get; set; }
        public string BenefitServiceCode { get; set; }
        public string BenefitCategory1 { get; set; }
        public string BenefitCategory2 { get; set; }
        public string BenefitCategory3 { get; set; }
        public string PlaceofService { get; set; }
        public string RxTierName { get; set; }
        public string RxTierType { get; set; }
        public string IsCovered { get; set; }
        public string Language { get; set; }
        public string State { get; set; }
        public string MarketCoverage { get; set; }
        public string CopayNoOONBenefitBehavior { get; set; }
        public string CoinsuranceNoOONBenefitBehavior { get; set; }
    }

    public class BenefitReview
    {
        public string BenefitServiceCode { get; set; }
        public string BenefitCategory1 { get; set; }
        public string BenefitCategory2 { get; set; }
        public string BenefitCategory3 { get; set; }
        public string PlaceofService { get; set; }
        public string NetworkTier { get; set; }
        public string IndDeductible { get; set; }
        public string FamDeductible { get; set; }
        public string Copay { get; set; }
        public string Coinsurance { get; set; }
    }

    public class RxTierDetail
    {
        public string Network { get; set; }
        public string RxService { get; set; }
        public string RxTierType { get; set; }
        public string Frequency { get; set; }
        public string Copay { get; set; }
        public string Coinsurance { get; set; }
        public string IndividualDeductible { get; set; }
        public string FamilyDeductible { get; set; }
    }

    public class QHPNetwork
    {
        public string NetworkTier { get; set; }
        public string QHPNetworkName { get; set; }
        public string DrugNetwork { get; set; }
    }
    public class NoOONPlanTypeList
    {
        public string PlanType { get; set; }
    }

    public class OONBehaviorBenefit
    {
        public string QHPBenefitName { get; set;}
        public string State { get; set; }
        public string MarketCoverage { get; set; }
        public string MetalTier { get; set; }
        public string CostShareVariationType { get; set; }
        public string OONCopay { get; set; }
        public string OONCoinsurance { get; set; }
        public string NoOONPlanTypes { get; set; }
    }
}
