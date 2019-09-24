using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.QHP
{

    public class DrugDeductible : ICloneable
    {
        public string HIOSPlanIDStandardComponentVariant { get; set; }
        public string PlanMarketingName { get; set; }
        public string CSRVariationType { get; set; }
        public string LevelofCoverageMetalLevel { get; set; }
        public string DeductibleDrugType { get; set; }
        public List<DeductibleNetwork> PlanAndBenefitsTemplateNetworkList { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class DeductibleNetwork
    {
        public string NetworkName { get; set; }
        public string Individual { get; set; }
        public string Family { get; set; }
        public string DefaultCoinsurance { get; set; }

    }

    public class Deductible
    {
        public string NetworkTier { get; set; }
        public string DeductibleAmount { get; set; }
        public string CoverageName { get; set; }
    }

    public class Coinsurance
    {
        public string NetworkTier { get; set; }
        public string CoinsuranceAmount { get; set; }
    }

    public class RxSelection
    {
        public string Network { get; set; }
        public string RxDeductibleIndividual { get; set; }
        public string RxDeductibleFamily { get; set; }
        public string RxOOPMIndividual { get; set; }
        public string RxOOPMFamily { get; set; }
    }

    public static class DeductibleHelper
    {

        public static List<DrugDeductible> GetDrugDeductible(string integrated, string csrType)
        {
            bool isZeroCostSharing = string.Equals(csrType, "Zero Cost Sharing Plan Variation", StringComparison.OrdinalIgnoreCase);
            List<DrugDeductible> drugDeductibles = new List<DrugDeductible>();
            drugDeductibles.Add(GetIndividualDeductible("Medical EHB Deductible", integrated == "false", isZeroCostSharing && integrated == "false"));
            drugDeductibles.Add(GetIndividualDeductible("Drug EHB Deductible", integrated == "false", false));
            drugDeductibles.Add(GetIndividualDeductible("Combined Medical and Drug EHB Deductible", integrated == "true", isZeroCostSharing && integrated == "true"));
            return drugDeductibles;

        }

        public static DrugDeductible GetIndividualDeductible(string drugType, bool showDefault, bool zeroVariation)
        {
            string individual = zeroVariation ? "$0" : showDefault ? "Not Applicable" : string.Empty;
            string family = zeroVariation ? "$0 per person | $0 per group" : showDefault ? "per person not applicable | per group not applicable" : string.Empty;

            DrugDeductible defaultRow = new DrugDeductible();
            defaultRow.DeductibleDrugType = drugType;
            defaultRow.CSRVariationType = string.Empty;
            defaultRow.HIOSPlanIDStandardComponentVariant = string.Empty;
            defaultRow.LevelofCoverageMetalLevel = string.Empty;
            defaultRow.PlanAndBenefitsTemplateNetworkList = new List<DeductibleNetwork>();
            defaultRow.PlanAndBenefitsTemplateNetworkList.Add(new DeductibleNetwork() { NetworkName = "In Network (Tier 1)", Individual = string.Empty, Family = string.Empty, DefaultCoinsurance = string.Empty });
            defaultRow.PlanAndBenefitsTemplateNetworkList.Add(new DeductibleNetwork() { NetworkName = "In Network (Tier 2)", Individual = string.Empty, Family = string.Empty, DefaultCoinsurance = string.Empty });
            defaultRow.PlanAndBenefitsTemplateNetworkList.Add(new DeductibleNetwork() { NetworkName = "Out of Network", Individual = individual, Family = family, DefaultCoinsurance = string.Empty });
            defaultRow.PlanAndBenefitsTemplateNetworkList.Add(new DeductibleNetwork() { NetworkName = "Combined In/Out Network", Individual = individual, Family = family, DefaultCoinsurance = string.Empty });

            return defaultRow;
        }

        public static string GetCopayAmount(List<Deductible> deductibles, List<QHPNetwork> mlNetworks, string networkName, string coverageName)
        {
            string amount = string.Empty;
            var deductible = (from mln in mlNetworks
                              join dn in deductibles on mln.NetworkTier equals dn.NetworkTier
                              where mln.QHPNetworkName == networkName && dn.CoverageName == coverageName
                              select new
                              {
                                  Type = dn.CoverageName,
                                  Amount = dn.DeductibleAmount
                              }).FirstOrDefault();
            if (deductible != null)
            {
                amount = GetFormattedAmount(deductible.Amount);
            }
            return amount;
        }

        public static string GetFormattedAmount(string amount)
        {
            if (!string.Equals("Not Applicable", amount, StringComparison.OrdinalIgnoreCase))
            {
                decimal copayAmount;
                if (decimal.TryParse(amount, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, CultureInfo.GetCultureInfo("en-US"), out copayAmount))
                {
                    amount = copayAmount.ToString("C0");
                }
            }

            return amount;
        }

        public static string GetCoinsuranceAmount(List<Coinsurance> coinsurances, List<QHPNetwork> mlNetworks, string networkName)
        {
            string amount = string.Empty;
            var coins = (from mln in mlNetworks
                         join co in coinsurances on mln.NetworkTier equals co.NetworkTier
                         where mln.QHPNetworkName == networkName
                         select new
                         {
                             Amount = co.CoinsuranceAmount
                         }).FirstOrDefault();
            if (coins != null)
            {
                amount = GetFormattedPercentage(coins.Amount);
            }
            return amount;
        }

        public static string GetFormattedPercentage(string value)
        {
            if (!string.Equals("Not Applicable", value, StringComparison.OrdinalIgnoreCase))
            {
                string coinsAmt = value.Replace("%", "");
                decimal coinsAmount;
                if (decimal.TryParse(coinsAmt, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, CultureInfo.CurrentCulture, out coinsAmount))
                {
                    value = coinsAmount.ToString("n2") + "%";
                }
            }

            return value;
        }

        public static string GetDrugEHBDeductible(List<RxSelection> selection, List<QHPNetwork> mlNetworks, string networkName, string coverageName)
        {
            string amount = string.Empty;
            var deductible = (from mln in mlNetworks
                              join dn in selection on mln.NetworkTier equals dn.Network
                              where mln.QHPNetworkName == networkName
                              select new
                              {
                                  Network = dn.Network,
                                  Individual = dn.RxDeductibleIndividual,
                                  Family = dn.RxDeductibleFamily
                              }).FirstOrDefault();
            if (deductible != null)
            {
                amount = GetFormattedAmount(coverageName == "Individual" ? deductible.Individual : deductible.Family);
            }
            return amount;
        }

        public static DrugDeductible GetDrugEHBCombinedDeductible(DrugDeductible deductible, string rxCombined, List<DeductibleNetwork> deductibleAmounts, string deductibleType)
        {
            bool hasCombined = false;
            decimal individual = 0;
            decimal family = 0;

            if (!string.IsNullOrEmpty(rxCombined) && string.Equals(rxCombined, "Yes", StringComparison.OrdinalIgnoreCase))
            {
                DeductibleNetwork inn = deductibleAmounts.Where(s => s.NetworkName == "In Network (Tier 1)").FirstOrDefault();
                DeductibleNetwork oon = deductibleAmounts.Where(s => s.NetworkName == "Out of Network").FirstOrDefault();

                decimal innIndAmount, onnIndAmount;
                if (decimal.TryParse(inn.Individual, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, CultureInfo.CurrentCulture, out innIndAmount) && decimal.TryParse(oon.Individual, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, CultureInfo.CurrentCulture, out onnIndAmount))
                {
                    individual = (innIndAmount + onnIndAmount);
                }

                decimal innFamAmount, onnfamAmount;
                if (decimal.TryParse(inn.Family, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, CultureInfo.CurrentCulture, out innFamAmount) && decimal.TryParse(oon.Family, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, CultureInfo.CurrentCulture, out onnfamAmount))
                {
                    family = (innFamAmount + onnfamAmount);
                }
                hasCombined = true;
            }

            if (hasCombined)
            {
                deductible.PlanAndBenefitsTemplateNetworkList[3].Individual = individual.ToString("C0");
                if (string.Equals("Embedded", deductibleType, StringComparison.OrdinalIgnoreCase))
                {
                    deductible.PlanAndBenefitsTemplateNetworkList[3].Family = string.Format("{0} per person | {1} per group", individual.ToString("C0"), family.ToString("C0"));
                }
                else if (string.Equals("Non-Embedded", deductibleType, StringComparison.OrdinalIgnoreCase))
                {
                    deductible.PlanAndBenefitsTemplateNetworkList[3].Family = string.Format("{0} per person | {1} per group", family.ToString("C0"), family.ToString("C0"));
                }
            }
            return deductible;
        }

        public static DrugDeductible GetCombinedDeductible(DrugDeductible deductible, string sectionData, List<DeductibleNetwork> deductibleAmounts, string deductibleType)
        {
            JObject DedSectionObject = JObject.Parse(sectionData);
            string InnWithOon = Convert.ToString(DedSectionObject.SelectToken("IsTheDeductibleINNcombinedwithOON") ?? string.Empty);
            string Inn1WithOon = Convert.ToString(DedSectionObject.SelectToken("IsTheDeductibleINNTier1combinedwithOON") ?? string.Empty);
            string Inn2WithOon = Convert.ToString(DedSectionObject.SelectToken("IsTheDeductibleINNTier2combinedwithOON") ?? string.Empty);
            string Inn1WithInn2 = Convert.ToString(DedSectionObject.SelectToken("IsTheDeductibleINNTier1combinedwithINNTier2") ?? string.Empty);

            bool hasCombined = false;
            decimal individual = 0;
            decimal family = 0;

            if ((!string.IsNullOrEmpty(InnWithOon) && string.Equals(InnWithOon, "true", StringComparison.OrdinalIgnoreCase)) || (!string.IsNullOrEmpty(Inn1WithOon) && string.Equals(Inn1WithOon, "true", StringComparison.OrdinalIgnoreCase)))
            {
                DeductibleNetwork inn = deductibleAmounts.Where(s => s.NetworkName == "In Network (Tier 1)").FirstOrDefault();
                DeductibleNetwork oon = deductibleAmounts.Where(s => s.NetworkName == "Out of Network").FirstOrDefault();

                decimal innIndAmount, onnIndAmount;
                if (decimal.TryParse(inn.Individual, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, CultureInfo.CurrentCulture, out innIndAmount) && decimal.TryParse(oon.Individual, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, CultureInfo.CurrentCulture, out onnIndAmount))
                {
                    individual = (innIndAmount + onnIndAmount);
                }

                decimal innFamAmount, onnfamAmount;
                if (decimal.TryParse(inn.Family, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, CultureInfo.CurrentCulture, out innFamAmount) && decimal.TryParse(oon.Family, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, CultureInfo.CurrentCulture, out onnfamAmount))
                {
                    family = (innFamAmount + onnfamAmount);
                }
                hasCombined = true;
            }

            if (!string.IsNullOrEmpty(Inn2WithOon) && string.Equals(Inn2WithOon, "true", StringComparison.OrdinalIgnoreCase))
            {
                DeductibleNetwork inn2 = deductibleAmounts.Where(s => s.NetworkName == "In Network (Tier 2)").FirstOrDefault();

                decimal innIndAmount;
                if (decimal.TryParse(inn2.Individual, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, CultureInfo.CurrentCulture, out innIndAmount))
                {
                    individual = (innIndAmount + individual);
                }

                decimal innFamAmount;
                if (decimal.TryParse(inn2.Family, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, CultureInfo.CurrentCulture, out innFamAmount))
                {
                    family = (innFamAmount + family);
                }
                hasCombined = true;
            }

            if (!string.IsNullOrEmpty(Inn1WithInn2) && string.Equals(Inn1WithInn2, "true", StringComparison.OrdinalIgnoreCase))
            {
                if ((!string.IsNullOrEmpty(InnWithOon) && !string.Equals(InnWithOon, "true", StringComparison.OrdinalIgnoreCase)) && (!string.IsNullOrEmpty(Inn1WithOon) && !string.Equals(Inn1WithOon, "true", StringComparison.OrdinalIgnoreCase)))
                {
                    DeductibleNetwork inn1 = deductibleAmounts.Where(s => s.NetworkName == "In Network (Tier 1)").FirstOrDefault();
                    DeductibleNetwork inn2 = deductibleAmounts.Where(s => s.NetworkName == "In Network (Tier 2)").FirstOrDefault();

                    decimal innIndAmount, onnIndAmount;
                    if (decimal.TryParse(inn1.Individual, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, CultureInfo.CurrentCulture, out innIndAmount) && decimal.TryParse(inn2.Individual, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, CultureInfo.CurrentCulture, out onnIndAmount))
                    {
                        individual = (innIndAmount + onnIndAmount);
                    }

                    decimal innFamAmount, onnfamAmount;
                    if (decimal.TryParse(inn1.Family, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, CultureInfo.CurrentCulture, out innFamAmount) && decimal.TryParse(inn2.Family, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, CultureInfo.CurrentCulture, out onnfamAmount))
                    {
                        family = (innFamAmount + onnfamAmount);
                    }
                }
                hasCombined = true;
            }

            if (hasCombined)
            {
                deductible.PlanAndBenefitsTemplateNetworkList[3].Individual = individual.ToString("C0");
                if (string.Equals("Embedded", deductibleType, StringComparison.OrdinalIgnoreCase))
                {
                    deductible.PlanAndBenefitsTemplateNetworkList[3].Family = string.Format("{0} per person | {1} per group", individual.ToString("C0"), family.ToString("C0"));
                }
                else if (string.Equals("Non-Embedded", deductibleType, StringComparison.OrdinalIgnoreCase))
                {
                    deductible.PlanAndBenefitsTemplateNetworkList[3].Family = string.Format("{0} per person | {1} per group", family.ToString("C0"), family.ToString("C0"));
                }
            }
            return deductible;
        }

        public static bool IsDeductibleCombinedNetwork(string sectionData)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(sectionData))
            {
                List<string> deductibleCombinedNetworkList = new List<string>();
                JObject DedSectionObject = JObject.Parse(sectionData);
                deductibleCombinedNetworkList.Add(Convert.ToString(DedSectionObject.SelectToken("IsTheDeductibleINNcombinedwithOON") ?? string.Empty));
                deductibleCombinedNetworkList.Add(Convert.ToString(DedSectionObject.SelectToken("IsTheDeductibleINNTier1combinedwithOON") ?? string.Empty));
                deductibleCombinedNetworkList.Add(Convert.ToString(DedSectionObject.SelectToken("IsTheDeductibleINNTier2combinedwithOON") ?? string.Empty));
                deductibleCombinedNetworkList.Add(Convert.ToString(DedSectionObject.SelectToken("IsTheDeductibleINNTier3combinedwithOON") ?? string.Empty));
                deductibleCombinedNetworkList.Add(Convert.ToString(DedSectionObject.SelectToken("IsTheDeductibleINNTier4combinedwithOON") ?? string.Empty));
                deductibleCombinedNetworkList.Add(Convert.ToString(DedSectionObject.SelectToken("IsTheDeductibleINNTier1combinedwithINNTier2") ?? string.Empty));
                deductibleCombinedNetworkList.Add(Convert.ToString(DedSectionObject.SelectToken("IsTheDeductibleINNTier1combinedwithINNTier3") ?? string.Empty));
                deductibleCombinedNetworkList.Add(Convert.ToString(DedSectionObject.SelectToken("IsTheDeductibleINNTier1combinedwithINNTier4") ?? string.Empty));
                deductibleCombinedNetworkList.Add(Convert.ToString(DedSectionObject.SelectToken("IsTheDeductibleINNTier2combinedwithINNTier3") ?? string.Empty));
                deductibleCombinedNetworkList.Add(Convert.ToString(DedSectionObject.SelectToken("IsTheDeductibleINNTier2combinedwithINNTier4") ?? string.Empty));
                deductibleCombinedNetworkList.Add(Convert.ToString(DedSectionObject.SelectToken("IsTheDeductibleINNTier3combinedwithINNTier4") ?? string.Empty));
                if (deductibleCombinedNetworkList.Where(s => s.Equals("true")).Any())
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            return result;
        }

        public static string DeductibleTextGenrator(string indValue, string familyValue)
        {
            string text = string.Empty;
            if (indValue.Equals("Not Applicable") && familyValue.Equals("Not Applicable"))
            {
                text = "per person not applicable | per group not applicable";
            }
            else if (!indValue.Equals("Not Applicable") && !familyValue.Equals("Not Applicable"))
            {
                text = "{0} per person | {1} per group";
                text = string.Format(text, indValue, familyValue).ToLower();
            }
            else if (indValue.Equals("Not Applicable") && !familyValue.Equals("Not Applicable"))
            {
                text = "per person not applicable | {0} per group";
                text = string.Format(text, familyValue).ToLower();
            }
            else if (!indValue.Equals("Not Applicable") && familyValue.Equals("Not Applicable"))
            {
                text = "{0} per person | per group not applicable";
                text = string.Format(text, indValue).ToLower();
            }
            return text;
        }
    }
}
