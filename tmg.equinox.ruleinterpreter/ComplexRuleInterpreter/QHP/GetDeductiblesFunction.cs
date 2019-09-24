using Newtonsoft.Json.Linq;
using tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;

namespace tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.QHP
{
    class GetDeductiblesFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            Parser.Result result = new Parser.Result();

            var planId = SourceManager.Get(Thread.CurrentThread, "PlanID");
            var planMarketingName = SourceManager.Get(Thread.CurrentThread, "PlanMarketingName");
            var levelofCoverage = SourceManager.Get(Thread.CurrentThread, "LevelofCoverage");
            var variationType = SourceManager.Get(Thread.CurrentThread, "CSRVariationType");
            var integrated = SourceManager.Get(Thread.CurrentThread, "Integrated");
            var dedType = SourceManager.Get(Thread.CurrentThread, "DeductibleType");
            var rxIntType = SourceManager.Get(Thread.CurrentThread, "RxIntegrated");
            var marketType = SourceManager.Get(Thread.CurrentThread, "MarketType");
            var rxSelection = SourceManager.Get(Thread.CurrentThread, "RXSelection");
            var combinedNetwork = SourceManager.Get(Thread.CurrentThread, "CombinedNetwork");
            var deductibleSection = SourceManager.Get(Thread.CurrentThread, "DeductibleSection");
            var rxDeductibleCombined = SourceManager.Get(Thread.CurrentThread, "DeductibleCombined");
            string deductibleSectionData = Convert.ToString(deductibleSection ?? string.Empty);

            bool isCombinedNetwork = DeductibleHelper.IsDeductibleCombinedNetwork(deductibleSectionData);

            List<RxSelection> rxSelections = JsonConvert.DeserializeObject<List<RxSelection>>(rxSelection.ToString());
            marketType = Convert.ToString(marketType);

            var deductibles = SourceManager.Get(Thread.CurrentThread, "Deductibles");
            List<Deductible> deductibleList = JsonConvert.DeserializeObject<List<Deductible>>(deductibles.ToString());

            var coinsurances = SourceManager.Get(Thread.CurrentThread, "Coinsurance");
            List<Coinsurance> coinsuranceList = JsonConvert.DeserializeObject<List<Coinsurance>>(coinsurances.ToString());

            var networks = SourceManager.Get(Thread.CurrentThread, "Networks");
            List<QHPNetwork> networkList = JsonConvert.DeserializeObject<List<QHPNetwork>>(networks.ToString());
            integrated = integrated != null ? integrated : "";
            List<DrugDeductible> drugDeductibles = DeductibleHelper.GetDrugDeductible(integrated.ToString(), variationType.ToString());

            if (integrated != null)
            {
                int index = 0; int length = 2;
                if (string.Equals("false", integrated.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    index = 0; length = 2;
                }
                else if (string.Equals("true", integrated.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    index = 2; length = 3;
                }

                foreach (var drug in drugDeductibles)
                {
                    drug.HIOSPlanIDStandardComponentVariant = planId + "-0" + BenefitHelper.GetCSRVariationIndex(variationType.ToString());
                    drug.PlanMarketingName = planMarketingName.ToString();
                    drug.LevelofCoverageMetalLevel = levelofCoverage.ToString();
                    drug.CSRVariationType = BenefitHelper.CSRVariationTypeFormater(variationType.ToString(), levelofCoverage.ToString());
                }

                for (int i = index; i < length; i++)
                {
                    List<DeductibleNetwork> dedNetworks = new List<DeductibleNetwork>();
                    var applDeductibles = drugDeductibles[i].PlanAndBenefitsTemplateNetworkList;
                    foreach (var network in applDeductibles)
                    {
                        string indDeductbile = string.Empty;
                        string famDeductible = string.Empty;
                        string defConinsurance = string.Empty;

                        if (string.Equals(network.NetworkName, "Combined In/Out Network", StringComparison.OrdinalIgnoreCase))
                        {
                            if (string.Equals("Drug EHB Deductible", drugDeductibles[i].DeductibleDrugType, StringComparison.OrdinalIgnoreCase))
                            {
                                drugDeductibles[i] = DeductibleHelper.GetDrugEHBCombinedDeductible(drugDeductibles[i], rxDeductibleCombined.ToString(), dedNetworks, rxIntType.ToString());
                            }
                            else
                            {
                                drugDeductibles[i] = DeductibleHelper.GetCombinedDeductible(drugDeductibles[i], deductibleSectionData, dedNetworks, dedType.ToString());
                            }
                        }
                        else
                        {

                            if (string.Equals("Drug EHB Deductible", drugDeductibles[i].DeductibleDrugType, StringComparison.OrdinalIgnoreCase))
                            {
                                indDeductbile = DeductibleHelper.GetDrugEHBDeductible(rxSelections, networkList, network.NetworkName, "Individual");
                                famDeductible = DeductibleHelper.GetDrugEHBDeductible(rxSelections, networkList, network.NetworkName, "Family");
                                defConinsurance = DeductibleHelper.GetFormattedPercentage("0");
                            }
                            else
                            {
                                indDeductbile = DeductibleHelper.GetCopayAmount(deductibleList, networkList, network.NetworkName, "Individual");
                                famDeductible = DeductibleHelper.GetCopayAmount(deductibleList, networkList, network.NetworkName, "Family");
                                defConinsurance = DeductibleHelper.GetCoinsuranceAmount(coinsuranceList, networkList, network.NetworkName);
                            }

                            dedNetworks.Add(new DeductibleNetwork() { NetworkName = network.NetworkName, DefaultCoinsurance = defConinsurance, Individual = indDeductbile, Family = famDeductible });
                            var intgType = drugDeductibles[i].DeductibleDrugType == "Drug EHB Deductible" ? rxIntType : dedType;

                            if (string.IsNullOrEmpty(indDeductbile))
                            {
                                indDeductbile = "Not Applicable";
                            }

                            if (string.IsNullOrEmpty(famDeductible))
                            {
                                famDeductible = "Not Applicable";
                            }

                            if (!string.IsNullOrEmpty(indDeductbile) && !string.IsNullOrEmpty(famDeductible))
                            {
                                if (intgType != null && string.Equals("Embedded", intgType.ToString(), StringComparison.OrdinalIgnoreCase))
                                {
                                    famDeductible = DeductibleHelper.DeductibleTextGenrator(indDeductbile, famDeductible);
                                }
                                else if (intgType != null && string.Equals("Non-Embedded", intgType.ToString(), StringComparison.OrdinalIgnoreCase))
                                {
                                    famDeductible = DeductibleHelper.DeductibleTextGenrator(famDeductible, famDeductible);
                                }
                                else
                                {
                                    famDeductible = DeductibleHelper.DeductibleTextGenrator(indDeductbile, famDeductible);
                                }
                                network.Individual = indDeductbile.ToLower();
                                network.Family = famDeductible;
                                network.DefaultCoinsurance = defConinsurance;
                            }
                        }
                    }
                }
            }

            List<DrugDeductible> variations = new List<DrugDeductible>();
            if (string.Equals(variationType.ToString(), "Both (Display as On/Off Exchange)", StringComparison.OrdinalIgnoreCase))
            {

                foreach (var drug in drugDeductibles)
                {
                    DrugDeductible variantRow = (DrugDeductible)drug.Clone();
                    variantRow.HIOSPlanIDStandardComponentVariant = variantRow.HIOSPlanIDStandardComponentVariant.Replace("-00", "-01");
                    variantRow.CSRVariationType = "Standard " + variantRow.LevelofCoverageMetalLevel + " On Exchange Plan";
                    variations.Add(variantRow);
                }
            }
            if (variations.Count > 0)
            {
                drugDeductibles.AddRange(variations);
            }

            result.Token = JToken.FromObject(drugDeductibles);
            return result;
        }
    }
}
