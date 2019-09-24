using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.QHP
{

    public class OutOfPocket : ICloneable
    {
        public string HIOSPlanIDStandardComponentVariant { get; set; }
        public string PlanMarketingName { get; set; }
        public string CSRVariationType { get; set; }
        public string LevelofCoverageMetalLevel { get; set; }
        public string MaximumOutofPocketType { get; set; }
        public List<OOPMNetwork> PlanAndBenefitsTemplateNetworkList { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class OOPMNetwork
    {
        public string NetworkName { get; set; }
        public string Individual { get; set; }
        public string Family { get; set; }

    }

    public class OutOfPocketMaximum
    {
        public string NetworkTier { get; set; }
        public string OOPMAmount { get; set; }
        public string CoverageName { get; set; }
    }
    public static class OOPMHelper
    {
        public static List<OutOfPocket> GetOutOfPocket(string integrated, string csrType)
        {
            bool isZeroCostSharing = string.Equals(csrType, "Zero Cost Sharing Plan Variation", StringComparison.OrdinalIgnoreCase);
            List<OutOfPocket> oopm = new List<OutOfPocket>();
            oopm.Add(GetIndividualOOPM("Maximum Out of Pocket for Medical EHB Benefits", integrated == "false", isZeroCostSharing && integrated == "false"));
            oopm.Add(GetIndividualOOPM("Maximum Out of Pocket for Drug EHB Benefits", integrated == "false", false));
            oopm.Add(GetIndividualOOPM("Maximum Out of Pocket for Medical and Drug EHB Benefits (Total)", integrated == "true", isZeroCostSharing && integrated == "true"));
            return oopm;

        }

        public static OutOfPocket GetIndividualOOPM(string oopmType, bool showDefault, bool zeroVariation)
        {
            string individual = zeroVariation ? "$0" : showDefault ? "not applicable" : string.Empty;
            string family = zeroVariation ? "$0 per person | $0 per group" : showDefault ? "per person not applicable | per group not applicable" : string.Empty;

            OutOfPocket defaultRow = new OutOfPocket();
            defaultRow.MaximumOutofPocketType = oopmType;
            defaultRow.CSRVariationType = string.Empty;
            defaultRow.HIOSPlanIDStandardComponentVariant = string.Empty;
            defaultRow.LevelofCoverageMetalLevel = string.Empty;
            defaultRow.PlanAndBenefitsTemplateNetworkList = new List<OOPMNetwork>();
            defaultRow.PlanAndBenefitsTemplateNetworkList.Add(new OOPMNetwork() { NetworkName = "In Network (Tier 1)", Individual = string.Empty, Family = string.Empty });
            defaultRow.PlanAndBenefitsTemplateNetworkList.Add(new OOPMNetwork() { NetworkName = "In Network (Tier 2)", Individual = string.Empty, Family = string.Empty });
            defaultRow.PlanAndBenefitsTemplateNetworkList.Add(new OOPMNetwork() { NetworkName = "Out of Network", Individual = individual, Family = family });
            defaultRow.PlanAndBenefitsTemplateNetworkList.Add(new OOPMNetwork() { NetworkName = "Combined In/Out Network", Individual = individual, Family = family });

            return defaultRow;
        }

        public static string GetCopayAmount(List<OutOfPocketMaximum> oopms, List<QHPNetwork> mlNetworks, string networkName, string coverageName)
        {
            string amount = string.Empty;
            var deductible = (from mln in mlNetworks
                              join oo in oopms on mln.NetworkTier equals oo.NetworkTier
                              where mln.QHPNetworkName == networkName && oo.CoverageName == coverageName
                              select new
                              {
                                  Type = oo.CoverageName,
                                  Amount = oo.OOPMAmount
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

        public static string GetDrugEHBOOPM(List<RxSelection> selection, List<QHPNetwork> mlNetworks, string networkName, string coverageName)
        {
            string amount = string.Empty;
            var deductible = (from mln in mlNetworks
                              join dn in selection on mln.NetworkTier equals dn.Network
                              where mln.QHPNetworkName == networkName
                              select new
                              {
                                  Network = dn.Network,
                                  Individual = dn.RxOOPMIndividual,
                                  Family = dn.RxOOPMFamily
                              }).FirstOrDefault();
            if (deductible != null)
            {
                amount = GetFormattedAmount(coverageName == "Individual" ? deductible.Individual : deductible.Family);
            }
            return amount;
        }

        public static OutOfPocket GetDrugEHBCombinedOOPM(OutOfPocket oopm, string rxCombined, List<OOPMNetwork> oopmAmounts, string oopmType)
        {
            bool hasCombined = false;
            decimal individual = 0;
            decimal family = 0;

            if (!string.IsNullOrEmpty(rxCombined) && string.Equals(rxCombined, "Yes", StringComparison.OrdinalIgnoreCase))
            {
                OOPMNetwork inn = oopmAmounts.Where(s => s.NetworkName == "In Network (Tier 1)").FirstOrDefault();
                OOPMNetwork oon = oopmAmounts.Where(s => s.NetworkName == "Out of Network").FirstOrDefault();

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
                oopm.PlanAndBenefitsTemplateNetworkList[3].Individual = individual.ToString("C0");
                if (string.Equals("Embedded", oopmType, StringComparison.OrdinalIgnoreCase))
                {
                    oopm.PlanAndBenefitsTemplateNetworkList[3].Family= OOPMHelper.OOPMTextGenrator(individual.ToString("C0"), family.ToString("C0"));
                }
                else if (string.Equals("Non-Embedded", oopmType, StringComparison.OrdinalIgnoreCase))
                {
                    oopm.PlanAndBenefitsTemplateNetworkList[3].Family= OOPMHelper.OOPMTextGenrator(family.ToString("C0"), family.ToString("C0"));
                }
            }
            return oopm;
        }

        public static OutOfPocket GetCombinedOutOfPocket(OutOfPocket oopm, string sectionData, List<OOPMNetwork> oopmAmounts, string oopmType)
        {
            JObject DedSectionObject = JObject.Parse(sectionData);
            string InnWithOon = Convert.ToString(DedSectionObject.SelectToken("IsTheOutofPocketMaxINNcombinedwithOON") ?? string.Empty);
            string Inn1WithOon = Convert.ToString(DedSectionObject.SelectToken("IsTheOutofPocketMaxINNTier1combinedwithOON") ?? string.Empty);
            string Inn2WithOon = Convert.ToString(DedSectionObject.SelectToken("IsTheOutofPocketMaxINNTier2combinedwithOON") ?? string.Empty);
            string Inn1WithInn2 = Convert.ToString(DedSectionObject.SelectToken("IsTheOutofPocketMaxINNTier1combinedwithINNTier2") ?? string.Empty);

            bool hasCombined = false;
            decimal individual = 0;
            decimal family = 0;

            if ((!string.IsNullOrEmpty(InnWithOon) && string.Equals(InnWithOon, "true", StringComparison.OrdinalIgnoreCase)) || (!string.IsNullOrEmpty(Inn1WithOon) && string.Equals(Inn1WithOon, "true", StringComparison.OrdinalIgnoreCase)))
            {
                OOPMNetwork inn = oopmAmounts.Where(s => s.NetworkName == "In Network (Tier 1)").FirstOrDefault();
                OOPMNetwork oon = oopmAmounts.Where(s => s.NetworkName == "Out of Network").FirstOrDefault();

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
                OOPMNetwork inn2 = oopmAmounts.Where(s => s.NetworkName == "In Network (Tier 2)").FirstOrDefault();

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
                    OOPMNetwork inn1 = oopmAmounts.Where(s => s.NetworkName == "In Network (Tier 1)").FirstOrDefault();
                    OOPMNetwork inn2 = oopmAmounts.Where(s => s.NetworkName == "In Network (Tier 2)").FirstOrDefault();

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
                oopm.PlanAndBenefitsTemplateNetworkList[3].Individual = individual.ToString("C0");
                if (string.Equals("Embedded", oopmType, StringComparison.OrdinalIgnoreCase))
                {
                    oopm.PlanAndBenefitsTemplateNetworkList[3].Family = OOPMHelper.OOPMTextGenrator(individual.ToString("C0"), family.ToString("C0"));
                }
                else if (string.Equals("Non-Embedded", oopmType, StringComparison.OrdinalIgnoreCase))
                {
                    oopm.PlanAndBenefitsTemplateNetworkList[3].Family = OOPMHelper.OOPMTextGenrator(family.ToString("C0"), family.ToString("C0"));
                }
            }
            return oopm;
        }
        public static bool IsOOPMCombinedNetwork(string oopmSectionData)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(oopmSectionData))
            {
                List<string> oOPMCombinedNetworkList = new List<string>();
                JObject DedSectionObject = JObject.Parse(oopmSectionData);
                oOPMCombinedNetworkList.Add(Convert.ToString(DedSectionObject.SelectToken("IsTheOutofPocketMaxINNcombinedwithOON") ?? string.Empty));
                oOPMCombinedNetworkList.Add(Convert.ToString(DedSectionObject.SelectToken("IsTheOutofPocketMaxINNTier1combinedwithOON") ?? string.Empty));
                oOPMCombinedNetworkList.Add(Convert.ToString(DedSectionObject.SelectToken("IsTheOutofPocketMaxINNTier2combinedwithOON") ?? string.Empty));
                oOPMCombinedNetworkList.Add(Convert.ToString(DedSectionObject.SelectToken("IsTheOutofPocketMaxINNTier3combinedwithOON") ?? string.Empty));
                oOPMCombinedNetworkList.Add(Convert.ToString(DedSectionObject.SelectToken("IsTheOutofPocketMaxINNTier4combinedwithOON") ?? string.Empty));
                oOPMCombinedNetworkList.Add(Convert.ToString(DedSectionObject.SelectToken("IsTheOutofPocketMaxINNTier1combinedwithINNTier2") ?? string.Empty));
                oOPMCombinedNetworkList.Add(Convert.ToString(DedSectionObject.SelectToken("IsTheOutofPocketMaxINNTier1combinedwithINNTier3") ?? string.Empty));
                oOPMCombinedNetworkList.Add(Convert.ToString(DedSectionObject.SelectToken("IsTheOutofPocketMaxINNTier1combinedwithINNTier4") ?? string.Empty));
                oOPMCombinedNetworkList.Add(Convert.ToString(DedSectionObject.SelectToken("IsTheOutofPocketMaxINNTier2combinedwithINNTier3") ?? string.Empty));
                oOPMCombinedNetworkList.Add(Convert.ToString(DedSectionObject.SelectToken("IsTheOutofPocketMaxINNTier2combinedwithINNTier4") ?? string.Empty));
                oOPMCombinedNetworkList.Add(Convert.ToString(DedSectionObject.SelectToken("IsTheOutofPocketMaxINNTier3combinedwithINNTier4") ?? string.Empty));
                if (oOPMCombinedNetworkList.Where(s => s.Equals("true")).Any())
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

        public static string CSRVariationType(string[] variationTypeList, string variationType)
        {
            string result = string.Empty;
            foreach (var item in variationTypeList)
            {
                if (item.Contains(variationType))
                {
                    return item;
                }
            }

            return result;
        }

        public static string OOPMTextGenrator(string indValue, string familyValue)
        {
            string text = string.Empty;
            if (indValue.Equals("Not Applicable") && familyValue.Equals("Not Applicable"))
            {
                text = "per person not applicable | per group not applicable";
            }
            else if(!indValue.Equals("Not Applicable") && !familyValue.Equals("Not Applicable"))
            {
                text = "{0} per person | {1} per group";
                text= string.Format(text, indValue, familyValue).ToLower();
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
