using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.anocchart.GlobalUtilities;

namespace tmg.equinox.anocchart
{
    public class AnocchartHelper
    {
        JObject previousMedicareJsonData = null;
        JObject nextMedicareJsonData = null;
        JObject previousPBPViewJsonData = null;
        JObject nextPBPViewJsonData = null;

        public AnocchartHelper(JObject previousMedicareJson, JObject nextMedicareJson, JObject previousPBPViewJson, JObject nextPBPViewJson)
        {
            this.previousMedicareJsonData = previousMedicareJson;
            this.nextMedicareJsonData = nextMedicareJson;
            this.previousPBPViewJsonData = previousPBPViewJson;
            this.nextPBPViewJsonData = nextPBPViewJson;
        }

        public string GetPlanType(bool isNextYr)
        {
            string planType = String.Empty;
            try
            {
                if (isNextYr)
                {
                    planType = Convert.ToString(this.nextPBPViewJsonData.SelectToken(RuleConstants.PlanType) ?? String.Empty);
                }
                else
                {
                    planType = Convert.ToString(this.previousPBPViewJsonData.SelectToken(RuleConstants.PlanType) ?? String.Empty);
                }
            }
            catch (Exception ex)
            {
            }
            return planType;
        }

        public List<string> GetNetworkTiers(bool isNextYr)
        {
            JArray NetworkTierList = null;
            List<string> result = new List<string>();
            bool HasTier = false;
            try
            {
                if (isNextYr)
                {
                    if (this.nextMedicareJsonData.SelectToken(RuleConstants.NetworkTier).Count() > 0)
                    {
                        NetworkTierList = (JArray)this.nextMedicareJsonData.SelectToken(RuleConstants.NetworkTier) ?? new JArray();
                        HasTier = true;
                    }
                }
                else
                {
                    if (this.previousMedicareJsonData.SelectToken(RuleConstants.NetworkTier).Count() > 0)
                    {
                        NetworkTierList = (JArray)this.previousMedicareJsonData.SelectToken(RuleConstants.NetworkTier) ?? new JArray();
                        HasTier = true;
                    }
                }
                if (HasTier)
                {
                    result = NetworkTierList.ToObject<List<string>>();
                    if (result.Contains("OON"))
                    {
                        result.Remove("OON");
                        if (IsPPOPlan(isNextYr).Equals(true))
                        {
                            result.Add(RuleConstants.OONPPO);
                        }
                        else if (IsPOSPlan(isNextYr))
                        {
                            result.Add(RuleConstants.OONPOS);
                        }
                    }
                    else if (this.IsPOSPlan(isNextYr))
                    {
                        result.Add(RuleConstants.OONPOS);
                    }

                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        public bool IsPOSPlan(bool isNextYr)
        {
            bool IsExist = false;
            string planType = string.Empty;
            try
            {
                if (isNextYr)
                {
                    planType = GetPlanType(isNextYr);
                }
                else
                {
                    planType = GetPlanType(isNextYr);
                }
                IsExist = RuleConstants.GetPOSPlanList().Where(s => s.Key.Equals(planType)).Any();
            }
            catch (Exception ex)
            {

            }
            return IsExist;
        }

        public bool IsPPOPlan(bool isNextYr)
        {
            bool IsExist = false;
            string planType = string.Empty;
            try
            {
                if (isNextYr)
                {
                    planType = GetPlanType(isNextYr);
                }
                else
                {
                    planType = GetPlanType(isNextYr);
                }
                IsExist = RuleConstants.GetPPOPlanList().Where(s => s.Key.Equals(planType)).Any();
            }
            catch (Exception ex)
            {
            }
            return IsExist;
        }

        public bool IsContainOONNetworkTier(bool isNextYr)
        {
            bool IsOONNetworkExist = false;
            try
            {
                List<string> TierArray = GetNetworkTiers(isNextYr);
                IsOONNetworkExist = TierArray.Contains("OON-POS") || TierArray.Contains("OON-PPO");
            }
            catch (Exception ex)
            {
            }
            return IsOONNetworkExist;
        }

        public int GetNumberOfGroups(bool isNextYr)
        {
            int numberOfgroup = 0;
            string strNumberOfgoup = string.Empty;
            try
            {
                if (isNextYr)
                {
                    strNumberOfgoup = Convert.ToString(this.nextPBPViewJsonData.SelectToken(RuleConstants.NumberOfGrups) ?? String.Empty);
                }
                else
                {
                    strNumberOfgoup = Convert.ToString(this.previousPBPViewJsonData.SelectToken(RuleConstants.NumberOfGrups) ?? String.Empty);
                }
                if (!string.IsNullOrEmpty(strNumberOfgoup))
                {
                    numberOfgroup = Convert.ToInt32(strNumberOfgoup);
                }
            }
            catch (Exception ex)
            {
            }
            return numberOfgroup;
        }

        public JToken GetSectionData(bool isNextYr, string sectionPath)
        {
            if (isNextYr)
            {
                return this.nextPBPViewJsonData.SelectToken(sectionPath);
            }
            else
            {
                return this.previousPBPViewJsonData.SelectToken(sectionPath);
            }
        }
    }
}
