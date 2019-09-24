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
    class GetOOPMFunction : ParserFunction
    {
        protected override Parser.Result Evaluate(string data, ref int from)
        {
            Parser.Result result = new Parser.Result();

            //            JArray defaultRow = JArray.Parse("[{\"HIOSPlanIDStandardComponentVariant\":\"\",\"PlanMarketingName\":\"\",\"CSRVariationType\":\"\",\"LevelofCoverageMetalLevel\":\"\",\"MaximumOutofPocketType\":\"Maximum Out of Pocket for Medical EHB Benefits\",\"PlanAndBenefitsTemplateNetworkList\":[{\"NetworkName\":\"In Network (Tier 1)\",\"Individual\":\"\",\"Family\":\"\"},{\"NetworkName\":\"In Network (Tier 2)\",\"Individual\":\"\",\"Family\":\"\"},{\"NetworkName\":\"Out of Network\",\"Individual\":\"\",\"Family\":\"\"},{\"NetworkName\":\"Combined In/Out Network\",\"Individual\":\"\",\"Family\":\"\"}]},{\"HIOSPlanIDStandardComponentVariant\":\"\",\"PlanMarketingName\":\"\",\"CSRVariationType\":\"\",\"LevelofCoverageMetalLevel\":\"\",\"MaximumOutofPocketType\":\"Maximum Out of Pocket for Drug EHB Benefits\",\"PlanAndBenefitsTemplateNetworkList\":[{\"NetworkName\":\"In Network (Tier 1)\",\"Individual\":\"\",\"Family\":\"\"},{\"NetworkName\":\"In Network (Tier 2)\",\"Individual\":\"\",\"Family\":\"\"},{\"NetworkName\":\"Out of Network\",\"Individual\":\"\",\"Family\":\"\"},{\"NetworkName\":\"Combined In/Out Network\",\"Individual\":\"\",\"Family\":\"\"}]},{\"HIOSPlanIDStandardComponentVariant\":\"\",\"PlanMarketingName\":\"\",\"CSRVariationType\":\"\",\"LevelofCoverageMetalLevel\":\"\",\"MaximumOutofPocketType\":\"Maximum Out of Pocket for Medical and Drug EHB Benefits (Total)\",\"PlanAndBenefitsTemplateNetworkList\":[{\"NetworkName\":\"In Network (Tier 1)\",\"Individual\":\"\",\"Family\":\"\"},{\"NetworkName\":\"In Network (Tier 2)\",\"Individual\":\"\",\"Family\":\"\"},{\"NetworkName\":\"Out of Network\",\"Individual\":\"\",\"Family\":\"\"},{\"NetworkName\":\"Combined In/Out Network\",\"Individual\":\"\",\"Family\":\"\"}]}]");
            var planId = SourceManager.Get(Thread.CurrentThread, "PlanID");
            var planMarketingName = SourceManager.Get(Thread.CurrentThread, "PlanMarketingName");
            var levelofCoverage = SourceManager.Get(Thread.CurrentThread, "LevelofCoverage");
            var variationType = SourceManager.Get(Thread.CurrentThread, "CSRVariationType");
            var integrated = SourceManager.Get(Thread.CurrentThread, "Integrated");
            var oopmType = SourceManager.Get(Thread.CurrentThread, "OOPMType");
            var rxIntType = SourceManager.Get(Thread.CurrentThread, "RxIntegrated");
            var marketType = SourceManager.Get(Thread.CurrentThread, "MarketType");
            var rxSelection = SourceManager.Get(Thread.CurrentThread, "RXSelection");
            var combinedNetwork = SourceManager.Get(Thread.CurrentThread, "CombinedNetwork");
            var oOPMSection = SourceManager.Get(Thread.CurrentThread, "OOPMSection");
            var rxOOPCombined = SourceManager.Get(Thread.CurrentThread, "OOPMCombined");
            string oOPMSectionData = Convert.ToString(oOPMSection ?? string.Empty);

            bool isOOPMCombinedNetwork = OOPMHelper.IsOOPMCombinedNetwork(oOPMSectionData);

            List<RxSelection> rxSelections = JsonConvert.DeserializeObject<List<RxSelection>>(rxSelection.ToString());
            marketType = Convert.ToString(marketType);

            var oopms = SourceManager.Get(Thread.CurrentThread, "OOPM");
            List<OutOfPocketMaximum> oopmList = JsonConvert.DeserializeObject<List<OutOfPocketMaximum>>(oopms.ToString());

            var networks = SourceManager.Get(Thread.CurrentThread, "Networks");
            List<QHPNetwork> networkList = JsonConvert.DeserializeObject<List<QHPNetwork>>(networks.ToString());

            List<OutOfPocket> outOfPocket = OOPMHelper.GetOutOfPocket(integrated.ToString(), variationType.ToString());

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

                foreach (var oopm in outOfPocket)
                {
                    oopm.HIOSPlanIDStandardComponentVariant = planId + "-0" + BenefitHelper.GetCSRVariationIndex(variationType.ToString());
                    oopm.PlanMarketingName = planMarketingName.ToString();
                    oopm.LevelofCoverageMetalLevel = levelofCoverage.ToString();
                    oopm.CSRVariationType = BenefitHelper.CSRVariationTypeFormater(variationType.ToString(), levelofCoverage.ToString());
                }

                for (int i = index; i < length; i++)
                {
                    List<OOPMNetwork> opNetworks = new List<OOPMNetwork>();
                    var applNetworks = outOfPocket[i].PlanAndBenefitsTemplateNetworkList;
                    foreach (var network in applNetworks)
                    {
                        string indDeductbile = string.Empty;
                        string famDeductible = string.Empty;

                        if (string.Equals(network.NetworkName, "Combined In/Out Network", StringComparison.OrdinalIgnoreCase))
                        {
                            if (string.Equals("Maximum Out of Pocket for Drug EHB Benefits", outOfPocket[i].MaximumOutofPocketType, StringComparison.OrdinalIgnoreCase))
                            {
                                outOfPocket[i] = OOPMHelper.GetDrugEHBCombinedOOPM(outOfPocket[i], rxOOPCombined.ToString(), opNetworks, rxIntType.ToString());
                            }
                            else
                            {
                                outOfPocket[i] = OOPMHelper.GetCombinedOutOfPocket(outOfPocket[i], oOPMSectionData, opNetworks, oopmType.ToString());
                            }
                        }
                        else
                        {

                            if (string.Equals("Maximum Out of Pocket for Drug EHB Benefits", outOfPocket[i].MaximumOutofPocketType, StringComparison.OrdinalIgnoreCase))
                            {
                                indDeductbile = OOPMHelper.GetDrugEHBOOPM(rxSelections, networkList, network.NetworkName, "Individual");
                                famDeductible = OOPMHelper.GetDrugEHBOOPM(rxSelections, networkList, network.NetworkName, "Family");
                            }
                            else
                            {
                                indDeductbile = OOPMHelper.GetCopayAmount(oopmList, networkList, network.NetworkName, "Individual");
                                famDeductible = OOPMHelper.GetCopayAmount(oopmList, networkList, network.NetworkName, "Family");
                            }

                            opNetworks.Add(new OOPMNetwork() { NetworkName = network.NetworkName, Individual = indDeductbile, Family = famDeductible });
                            var intgType = outOfPocket[i].MaximumOutofPocketType == "Maximum Out of Pocket for Drug EHB Benefits" ? rxIntType : oopmType;

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
                                    //famDeductible = string.Format("per person {0} | per group  {1}", indDeductbile, famDeductible);
                                    famDeductible = OOPMHelper.OOPMTextGenrator(indDeductbile, famDeductible);
                                }
                                else if (intgType != null && string.Equals("Non-Embedded", intgType.ToString(), StringComparison.OrdinalIgnoreCase))
                                {
                                    //famDeductible = string.Format("per person {0} | per group  {1}", famDeductible, famDeductible);
                                    famDeductible = OOPMHelper.OOPMTextGenrator(famDeductible, famDeductible);
                                }
                                else
                                {
                                    famDeductible = OOPMHelper.OOPMTextGenrator(indDeductbile, famDeductible);
                                }
                                network.Individual = indDeductbile.ToLower();
                                network.Family = famDeductible;
                            }
                        }
                    }
                }
            }

            List<OutOfPocket> variations = new List<OutOfPocket>();
            if (string.Equals(variationType.ToString(), "Both (Display as On/Off Exchange)", StringComparison.OrdinalIgnoreCase))
            {

                foreach (var oopm in outOfPocket)
                {
                    OutOfPocket variantRow = (OutOfPocket)oopm.Clone();
                    variantRow.HIOSPlanIDStandardComponentVariant = variantRow.HIOSPlanIDStandardComponentVariant.Replace("-00", "-01");
                    variantRow.CSRVariationType = "Standard " + variantRow.LevelofCoverageMetalLevel + " On Exchange Plan";
                    variations.Add(variantRow);
                }
            }
            if (variations.Count > 0)
            {
                outOfPocket.AddRange(variations);
            }

            result.Token = JToken.FromObject(outOfPocket);
            return result;
        }
    }
}
