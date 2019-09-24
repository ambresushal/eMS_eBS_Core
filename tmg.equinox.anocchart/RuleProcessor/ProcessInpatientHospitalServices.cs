using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.anocchart.GlobalUtilities;
using tmg.equinox.anocchart.Model;
using tmg.equinox.ruleinterpreter.executor;
using tmg.equinox.ruleinterpretercollateral;

namespace tmg.equinox.anocchart.RuleProcessor
{
    public class ProcessInpatientHospitalServices
    {
        ANOCChartSource _source = null;
        public ProcessInpatientHospitalServices(ANOCChartSource source)
        {
            _source = source;
        }

        public List<InpatientHospitalStays> InpatientServicesChange()
        {
            List<InpatientHospitalStays> summaryChange = new List<Model.InpatientHospitalStays>();
            
            List<JToken> services = new List<JToken>();
            JObject generalServices = (JObject)_source.MasterListANOCEOCJsonData.DeepClone();
            services = generalServices.SelectToken("BenefitChart.REPORTDATASCS") == null ? new List<JToken>() : generalServices.SelectToken("BenefitChart.REPORTDATASCS").ToList();
            
            List<string> networkTierList = _source.AnocHelper.GetNetworkTiers(true);

            var isPPOPOSPlan = (_source.AnocHelper.IsPOSPlan(true) || _source.AnocHelper.IsPPOPlan(true)) ? true : false;

            if (isPPOPOSPlan)
            {
                networkTierList = networkTierList.Where(whr => whr.ToString() != "OON-PPO" && whr.ToString() != "OON-POS").Select(sel => sel).ToList();

                if (!networkTierList.Contains("OON"))
                    networkTierList.Add("OON");
            }

            services = (from ser in services
                        where networkTierList.Contains(ser.SelectToken("Tier").ToString().Trim())
                        //where networkTierList.Any(s => s.Contains(ser.SelectToken("Tier").ToString().Trim()))
                        select ser).ToList();
            IList<JToken> AllServices = services;
            services = services.Where(whr => whr[RuleConstants.BenefitCategory1].ToString() == RuleConstants.IHSBenefitCategory1
                                                  && whr[RuleConstants.BenefitCategory2].ToString() == RuleConstants.IHSBenefitCategory2
                                                  && (whr[RuleConstants.BenefitCategory3].ToString() == RuleConstants.IHSBenefitCategory3
                                                  || whr[RuleConstants.BenefitCategory3].ToString() == RuleConstants.AdditionalDays
                                                  )
                                           ).Select(sel => sel).ToList();
            Dictionary<string, JToken> thisYearSources = new Dictionary<string, JToken>();
            thisYearSources.Add("PBPView", _source.PreviousPBPViewJsonData);
            thisYearSources.Add("Medicare", _source.PreviousMedicareJsonData);

            Dictionary<string, JToken> nextYearSources = new Dictionary<string, JToken>();
            nextYearSources.Add("PBPView", _source.NextPBPViewJsonData);
            nextYearSources.Add("Medicare", _source.NextMedicareJsonData);

            JToken serviceToken = JToken.FromObject(services);

            ReportDataResolver thisYearResolver = new ReportDataResolver(serviceToken, thisYearSources);
            JToken thisYearResolvedData = thisYearResolver.ResolveData();

            ReportDataResolver nextYearResolver = new ReportDataResolver(serviceToken, nextYearSources);
            JToken nextYearResolvedData = nextYearResolver.ResolveData();

            List<JToken> thisYearIHSServices = thisYearResolvedData.ToList();
            List<JToken> nextYearIHSServices = nextYearResolvedData.ToList();
            string costShareKey = string.Empty;

           List<JToken>  thisYearIHSServicesCol = (from ser in thisYearIHSServices
                                   //where networkTierList.Any(s => s.Contains(ser.SelectToken("Tier").ToString().Trim()))
                                   where networkTierList.Contains(ser.SelectToken("Tier").ToString().Trim())
                             && ser[RuleConstants.CostShareType].ToString()=="Copay"
                             && ser[RuleConstants.BenefitCategory3].ToString()!=RuleConstants.AdditionalDays
                        select ser).ToList();
            
            List<JToken> nextYearIHSServicescol = (from ser in nextYearIHSServices
                                   //where networkTierList.Any(s => s.Contains(ser.SelectToken("Tier").ToString().Trim()))
                                   where networkTierList.Contains(ser.SelectToken("Tier").ToString().Trim())
                                   && ser[RuleConstants.CostShareType].ToString() == "Copay"
                                   && ser[RuleConstants.BenefitCategory3].ToString() != RuleConstants.AdditionalDays
                                                   select ser).ToList();

            List<JToken> distinctCopayServices = services.Where(whr => whr[RuleConstants.CostShareType].ToString() == "Copay" && whr[RuleConstants.BenefitCategory3].ToString()!= RuleConstants.AdditionalDays)
                
                .Select(sel => sel).ToList();

            List<JToken> thisYearAdditionalDaysServicesCol = (from ser in thisYearIHSServices
                                                   //where networkTierList.Any(s => s.Contains(ser.SelectToken("Tier").ToString().Trim()))
                                                   where networkTierList.Contains(ser.SelectToken("Tier").ToString().Trim())
                                             && ser[RuleConstants.CostShareType].ToString() == "Copay"
                                             && ser[RuleConstants.BenefitCategory3].ToString() == RuleConstants.AdditionalDays
                                                   select ser).ToList();
            
            List<JToken> nextYearAdditionalDaysServicescol = (from ser in nextYearIHSServices
                                                   //where networkTierList.Any(s => s.Contains(ser.SelectToken("Tier").ToString().Trim()))
                                                   where networkTierList.Contains(ser.SelectToken("Tier").ToString().Trim())
                                                   && ser[RuleConstants.CostShareType].ToString() == "Copay"
                                                   && ser[RuleConstants.BenefitCategory3].ToString() == RuleConstants.AdditionalDays
                                                   select ser).ToList();
            
            Dictionary<string, string> costShareKeysDic = new Dictionary<string, string>();

            if (distinctCopayServices != null && distinctCopayServices.Count > 0)
            {
                for (int i = 0; i < distinctCopayServices.Count; i++)
                {
                    InpatientHospitalStays obj = new InpatientHospitalStays();
                    costShareKey = "IHS";

                    JToken thisYearRow = thisYearIHSServicesCol[i];
                    JToken nextYearRow = nextYearIHSServicescol[i];
                    obj.Tier = thisYearRow.SelectToken(RuleConstants.Tier).ToString();
                    obj.Interval = thisYearRow.SelectToken(RuleConstants.Interval).ToString();
                    costShareKey=costShareKey + obj.Tier + obj.Interval;
                    if (!costShareKeysDic.ContainsKey(costShareKey))
                    {
                        JToken thisYearCoinsToken = null;
                        JToken nextYearCoinsToken = null;

                        string thisYearCopayValue = thisYearRow.SelectToken(RuleConstants.Amount).ToString();
                        string thisYearCoinsValue = GetCoinsuranceValue(thisYearRow, thisYearIHSServices);
                        thisYearCoinsToken=!string.IsNullOrEmpty(thisYearCoinsValue)?GetCoinsToken(thisYearRow, thisYearIHSServices):null;
                        
                        string nextYearCopayValue = nextYearRow.SelectToken(RuleConstants.Amount).ToString();
                        string nextYearCoinsValue = GetCoinsuranceValue(nextYearRow, nextYearIHSServices);
                        nextYearCoinsToken = !string.IsNullOrEmpty(nextYearCoinsValue) ? GetCoinsToken(nextYearRow, nextYearIHSServices) : null;

                        obj.ThisYearBeginDay = !string.IsNullOrEmpty(thisYearCoinsValue) ? thisYearCoinsToken.SelectToken(RuleConstants.BeginDay).ToString() : thisYearRow.SelectToken(RuleConstants.BeginDay).ToString();
                        obj.ThisYearEndDay = !string.IsNullOrEmpty(thisYearCoinsValue) ? thisYearCoinsToken.SelectToken(RuleConstants.EndDay).ToString() :thisYearRow.SelectToken(RuleConstants.EndDay).ToString();
                        obj.NextYearBeginDay = !string.IsNullOrEmpty(nextYearCoinsValue) ? nextYearCoinsToken.SelectToken(RuleConstants.BeginDay).ToString() : nextYearRow.SelectToken(RuleConstants.BeginDay).ToString();
                        obj.NextYearEndDay = !string.IsNullOrEmpty(nextYearCoinsValue) ? nextYearCoinsToken.SelectToken(RuleConstants.EndDay).ToString() : nextYearRow.SelectToken(RuleConstants.EndDay).ToString();

                        obj.ThisYearIntervalCount = !string.IsNullOrEmpty(thisYearCoinsValue) ? thisYearCoinsToken.SelectToken(RuleConstants.IntervalCount).ToString() :thisYearRow.SelectToken(RuleConstants.IntervalCount).ToString();
                        obj.NextYearIntervalCount = !string.IsNullOrEmpty(nextYearCoinsValue) ? nextYearCoinsToken.SelectToken(RuleConstants.IntervalCount).ToString() : nextYearRow.SelectToken(RuleConstants.IntervalCount).ToString();
                        
                        obj.ThisYearAmount = GetCostShareValue(thisYearCopayValue, thisYearCoinsValue);
                        obj.NextYearAmount = GetCostShareValue(nextYearCopayValue, nextYearCoinsValue);
                        obj.CostShareKey = costShareKey;

                        costShareKeysDic.Add(obj.CostShareKey, obj.CostShareKey);
                        try
                        {
                            JToken thisAdditionalDaysthisYearRowYearRow = null;
                            JToken nextAdditionalDaysthisYearRowYearRow = null;
                            string thisYearAdditionalDaysCopayValue = string.Empty, thisYearAdditionalDaysCoinsValue = string.Empty;
                            string nextYearAdditionalDaysCopayValue = string.Empty, nextYearAdditionalDaysCoinsValue = string.Empty;
                            if (thisYearAdditionalDaysServicesCol.Count() > 0)
                            {
                                thisAdditionalDaysthisYearRowYearRow = thisYearAdditionalDaysServicesCol
                                                                       .Where(s => s["Interval"].ToString().Equals(obj.Interval.ToString())
                                                                       && s["Tier"].ToString().Equals(obj.Tier)
                                                                       )
                                                                       .FirstOrDefault();

                                thisYearAdditionalDaysCopayValue = thisAdditionalDaysthisYearRowYearRow != null? thisAdditionalDaysthisYearRowYearRow.SelectToken(RuleConstants.Amount).ToString():string.Empty;
                                thisYearAdditionalDaysCoinsValue = thisAdditionalDaysthisYearRowYearRow != null ? GetCoinsuranceValue(thisAdditionalDaysthisYearRowYearRow, thisYearAdditionalDaysServicesCol):string.Empty;
                                
                            }
                            if (nextYearAdditionalDaysServicescol.Count() > 0)
                            {
                                nextAdditionalDaysthisYearRowYearRow = nextYearAdditionalDaysServicescol
                                                                       .Where(s => s["Interval"].ToString().Equals(obj.Interval.ToString())
                                                                       && s["Tier"].ToString().Equals(obj.Tier)
                                                                       ).FirstOrDefault();
                               nextYearAdditionalDaysCopayValue = nextAdditionalDaysthisYearRowYearRow!=null?nextAdditionalDaysthisYearRowYearRow.SelectToken(RuleConstants.Amount).ToString():string.Empty;
                               nextYearAdditionalDaysCoinsValue = nextAdditionalDaysthisYearRowYearRow != null ? GetCoinsuranceValue(nextAdditionalDaysthisYearRowYearRow, nextYearAdditionalDaysServicescol):string.Empty;
                            }
                            
                            obj.AdditionalDaysThisYearBeginDay = thisAdditionalDaysthisYearRowYearRow != null ? thisAdditionalDaysthisYearRowYearRow.SelectToken(RuleConstants.BeginDay).ToString():string.Empty;
                            obj.AdditionalDaysThisYearEndDay = thisAdditionalDaysthisYearRowYearRow != null ? thisAdditionalDaysthisYearRowYearRow.SelectToken(RuleConstants.EndDay).ToString():string.Empty;
                            obj.AdditionalDaysThisYearAmount = GetCostShareValue(thisYearAdditionalDaysCopayValue, thisYearAdditionalDaysCoinsValue);
                            obj.AdditionalDaysThisIntervalCount = thisAdditionalDaysthisYearRowYearRow!=null?thisAdditionalDaysthisYearRowYearRow.SelectToken(RuleConstants.VAR1).ToString():string.Empty;

                            obj.AdditionalDaysNextYearBeginDay = nextAdditionalDaysthisYearRowYearRow!=null?nextAdditionalDaysthisYearRowYearRow.SelectToken(RuleConstants.BeginDay).ToString(): string.Empty;
                            obj.AdditionalDaysNextYearEndDay = nextAdditionalDaysthisYearRowYearRow != null ? nextAdditionalDaysthisYearRowYearRow.SelectToken(RuleConstants.EndDay).ToString(): string.Empty;
                            obj.AdditionalDaysNextYearAmount = GetCostShareValue(nextYearAdditionalDaysCopayValue, nextYearAdditionalDaysCoinsValue);
                            obj.AdditionalDaysNextIntervalCount = nextAdditionalDaysthisYearRowYearRow != null ? nextAdditionalDaysthisYearRowYearRow.SelectToken(RuleConstants.VAR1).ToString():string.Empty;
                        }
                        catch(Exception ex)
                        {

                        }

                        summaryChange.Add(obj);
                    }
                }
            }
            return summaryChange;
        }
        
        private string GetCostShareValue(string copayValue, string coinsValue)
        {
            string tokenString = @"{""AmountCopay"":"""",""AmountCoinsurance"":""""}";
            JToken costShareToken = JToken.Parse(tokenString);
            costShareToken["AmountCopay"] = copayValue;
            costShareToken["AmountCoinsurance"] = coinsValue;
            string result = string.Empty;
            List<LanguageFormats> langFormats = new List<LanguageFormats>();
            string costSharePalceHolder = "CS:[AmountCopay,AmountCoinsurance]";
            CostSharePlaceHolderFunctionEvaluator evaluator = new CostSharePlaceHolderFunctionEvaluator(costSharePalceHolder, costShareToken, langFormats);
            result = evaluator.Evaluate();
            return result;
        }
        
        private string GetCoinsuranceValue(JToken dataSource, List<JToken> resolvedService)
        {
            string result = string.Empty;
            string costshareType = "Coinsurance";
            string currentBC1 = dataSource.SelectToken(RuleConstants.BenefitCategory1).ToString();
            string currentBC2 = dataSource.SelectToken(RuleConstants.BenefitCategory2).ToString();
            string currentBC3 = dataSource.SelectToken(RuleConstants.BenefitCategory3).ToString();
            string currentInterval = dataSource.SelectToken(RuleConstants.Interval).ToString();
            string currentTier = dataSource.SelectToken(RuleConstants.Tier).ToString();

            var otherCostShareIntervalRow = resolvedService.Where(whr => whr[RuleConstants.BenefitCategory1].ToString() == currentBC1
                                                                 && whr[RuleConstants.BenefitCategory2].ToString() == currentBC2
                                                                 && whr[RuleConstants.BenefitCategory3].ToString() == currentBC3
                                                                 && whr[RuleConstants.CostShareType].ToString() == costshareType
                                                                 && whr[RuleConstants.Interval].ToString() == currentInterval
                                                                 && whr[RuleConstants.Tier].ToString() == currentTier
                                                              ).Select(sel => sel);

            if (otherCostShareIntervalRow != null && otherCostShareIntervalRow.Count() > 0)
            {
                JToken otherCostShareRowValue = otherCostShareIntervalRow.FirstOrDefault();
                result = otherCostShareRowValue.SelectToken(RuleConstants.Amount).ToString();
            }
            return result;
        }

        private JToken GetCoinsToken(JToken dataSource, List<JToken> resolvedService)
        {
            string result = string.Empty;
            string costshareType = "Coinsurance";
            string currentBC1 = dataSource.SelectToken(RuleConstants.BenefitCategory1).ToString();
            string currentBC2 = dataSource.SelectToken(RuleConstants.BenefitCategory2).ToString();
            string currentBC3 = dataSource.SelectToken(RuleConstants.BenefitCategory3).ToString();
            string currentInterval = dataSource.SelectToken(RuleConstants.Interval).ToString();
            string currentTier = dataSource.SelectToken(RuleConstants.Tier).ToString();

            var otherCostShareIntervalRow = resolvedService.Where(whr => whr[RuleConstants.BenefitCategory1].ToString() == currentBC1
                                                                 && whr[RuleConstants.BenefitCategory2].ToString() == currentBC2
                                                                 && whr[RuleConstants.BenefitCategory3].ToString() == currentBC3
                                                                 && whr[RuleConstants.CostShareType].ToString() == costshareType
                                                                 && whr[RuleConstants.Interval].ToString() == currentInterval
                                                                 && whr[RuleConstants.Tier].ToString() == currentTier
                                                              ).Select(sel => sel);
                        
                JToken otherCostShareRowValue = otherCostShareIntervalRow.FirstOrDefault();
            
            return otherCostShareRowValue;
        }
    }
}
