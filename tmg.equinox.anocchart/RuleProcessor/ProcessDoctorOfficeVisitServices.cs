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
    public class ProcessDoctorOfficeVisitServices
    {
        ANOCChartSource _source = null;
        public ProcessDoctorOfficeVisitServices(ANOCChartSource source)
        {
            _source = source;
        }

        public object AnocHelper { get; private set; }

        public List<DoctorOfficeVisit> DoctorOfficeVisitsChange()
        {
            List<DoctorOfficeVisit> summaryChnage = new List<Model.DoctorOfficeVisit>();
            List<JToken> services = new List<JToken>();
            JObject generalServices = (JObject)_source.MasterListANOCEOCJsonData.DeepClone();
            services = generalServices.SelectToken("BenefitChart.REPORTDATABRG") == null ? new List<JToken>() : generalServices.SelectToken("BenefitChart.REPORTDATABRG").ToList();


            List<string> networkTierList = _source.AnocHelper.GetNetworkTiers(true);
            var isPPOPOSPlan = (_source.AnocHelper.IsPOSPlan(true) || _source.AnocHelper.IsPPOPlan(true)) ? true : false;

            if (isPPOPOSPlan)
            {
                networkTierList = networkTierList.Where(whr => whr.ToString() != "OON-PPO" && whr.ToString() != "OON-POS").Select(sel => sel).ToList();

                if (!networkTierList.Contains("OON"))
                    networkTierList.Add("OON");
            }

            services = (from ser in services
                        //where networkTierList.Any(s => s.Contains(ser.SelectToken("Tier").ToString().Trim()))
                        where networkTierList.Contains(ser.SelectToken("Tier").ToString().Trim())
                        select ser).ToList();
            
            List<JToken> defaultServices = new List<JToken>();
            defaultServices = RuleConstants.GetDefaultServices();
            defaultServices = (from ser in defaultServices
                               //where networkTierList.Any(s => s.Contains(ser.SelectToken("Tier").ToString().Trim()))
                               where networkTierList.Contains(ser.SelectToken("Tier").ToString().Trim())
                               && Convert.ToBoolean(ser.SelectToken("IsBRGService") ?? false) == true
                               select ser).ToList();

            Dictionary<string, string> keys = new Dictionary<string, string>();
            keys.Add(RuleConstants.BenefitCategory1, RuleConstants.BenefitCategory1);
            keys.Add(RuleConstants.BenefitCategory2, RuleConstants.BenefitCategory2);
            keys.Add(RuleConstants.BenefitCategory3, RuleConstants.BenefitCategory3);
            keys.Add(RuleConstants.CostShareTiers, RuleConstants.CostShareTiers);

            CollectionExecutionComparer operatorProcessor = new CollectionExecutionComparer(services, defaultServices, keys);
            services = operatorProcessor.Intersection();

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

            List<JToken> thisYearDVServices = thisYearResolvedData.ToList();
            List<JToken> nextYearDVServices = nextYearResolvedData.ToList();
            string costShareKey = string.Empty;
            Dictionary<string, string> costShareKeysDic = new Dictionary<string, string>();

            if (services != null && services.Count > 0)
            {
                for (int i = 0; i < services.Count; i++)
                {
                    DoctorOfficeVisit obj = new DoctorOfficeVisit();
                    JToken thisYearRow = thisYearDVServices[i];
                    JToken nextYearRow = nextYearDVServices[i];
                    string tier = thisYearRow.SelectToken(RuleConstants.Tier).ToString();
                    string benefitCategory2 = thisYearRow.SelectToken(RuleConstants.BenefitCategory2).ToString();
                    string serviceType = benefitCategory2 == "Primary Care Physician Services" ? "PrimaryCare" :
                                                       (benefitCategory2 == "Physician Specialist Services" ? "SpecialistCare" : string.Empty);
                    costShareKey = serviceType + tier;
                    if (!costShareKeysDic.ContainsKey(costShareKey))
                    {
                        obj.CostShareKey = costShareKey;
                        obj.Tier = tier;
                        obj.ThisYear = GetCostSharevalue(thisYearRow);
                        obj.NextYear = GetCostSharevalue(nextYearRow);
                        obj.ServiceType = serviceType;
                        costShareKeysDic.Add(costShareKey,costShareKey);
                        summaryChnage.Add(obj);
                    }
                }
            }
            return summaryChnage;
        }


        private string GetCostSharevalue(JToken dataSource)
        {
            string result = string.Empty;
            List<LanguageFormats> langFormats = new List<LanguageFormats>();
            string costSharePalceHolder = "{{CS:[MinCopay,MaxCopay,MinCoinsurance,MaxCoinsurance]}}";
            CostSharePlaceHolderFunctionEvaluator evaluator = new CostSharePlaceHolderFunctionEvaluator(costSharePalceHolder, dataSource, langFormats);
            result = evaluator.Evaluate();
            return result;
        }
    }
}
