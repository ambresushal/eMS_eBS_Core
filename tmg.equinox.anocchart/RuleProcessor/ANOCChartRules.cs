using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
using tmg.equinox.anocchart.GlobalUtilities;
using tmg.equinox.anocchart.Model;
using tmg.equinox.ruleinterpreter.executor;
using tmg.equinox.infrastructure.exceptionhandling;
using System.Reflection;
using tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Extension;
using System.Text;

namespace tmg.equinox.anocchart.RuleProcessor
{
    public class ANOCChartRules
    {
        JObject previousMedicareJsonData = null;
        JObject nextMedicareJsonData = null;
        JObject previousPBPViewJsonData = null;
        JObject nextPBPViewJsonData = null;
        JObject masterListAnocJsonData = null;
        JObject anocViewJsonData = null;
        JObject masterListANOCEOCJsonData = null;
        AnocchartHelper AnocHelper = null;
        ChangeSummaryServices summaryChanges = null;
        public ANOCChartRules(string previousMedicareJSON, string nextMedicareJSON, string prevYearPBPViewJSONData, string nextYearPBPViewJSONData, string masterListAnocTemplateJSONData, string anocViewJSONData, string masterListANOCEOCJSONData)
        {
            if (!String.IsNullOrEmpty(previousMedicareJSON) && !String.IsNullOrEmpty(nextMedicareJSON) && !String.IsNullOrEmpty(prevYearPBPViewJSONData)
             && !String.IsNullOrEmpty(nextYearPBPViewJSONData) && !String.IsNullOrEmpty(masterListAnocTemplateJSONData))
            {
                this.previousMedicareJsonData = JObject.Parse(previousMedicareJSON);
                this.nextMedicareJsonData = JObject.Parse(nextMedicareJSON);
                this.previousPBPViewJsonData = JObject.Parse(prevYearPBPViewJSONData);
                this.nextPBPViewJsonData = JObject.Parse(nextYearPBPViewJSONData);
                this.masterListAnocJsonData = JObject.Parse(masterListAnocTemplateJSONData);
                    this.anocViewJsonData = JObject.Parse(anocViewJSONData);
                this.masterListANOCEOCJsonData = JObject.Parse(masterListANOCEOCJSONData);
                AnocHelper = new AnocchartHelper(this.previousMedicareJsonData, this.nextMedicareJsonData, this.previousPBPViewJsonData, this.nextPBPViewJsonData);
                summaryChanges = new RuleProcessor.ChangeSummaryServices(this.previousMedicareJsonData, this.nextMedicareJsonData, this.previousPBPViewJsonData, this.nextPBPViewJsonData, this.masterListAnocJsonData, this.anocViewJsonData, this.masterListANOCEOCJsonData, AnocHelper);
            }
        }
        public string GetANOCChartServices()
        {
            List<JToken> serviceDataList = new List<JToken>();
            JObject obj = JObject.Parse("{'ANOCChartPlanDetails.PlanInformation':'','ANOCChartPlanDetails.PlanPremiumInformation':'','ANOCChartPlanDetails.OutofPocketInformation':'','ANOCChartPlanDetails.ANOCBenefitsCompare':'','ANOCChartPlanDetails.PrescriptionInformation':'','ANOCChartPlanDetails.PreInitialCoverageLimitInformation':'','ANOCChartPlanDetails.DoctorOfficeVisitInfo':'','ANOCChartPlanDetails.InpatientHospitalStayInfo':'','ANOCChartPlanDetails.GapCoverageInformation':'','ANOCChartPlanDetails.ANOCCPCharts.DoctorOfficeVisits':'','ANOCChartPlanDetails.PreferredRetailInformation':''}");
            try
            {
                List<CoveredCost> chartServices = new List<CoveredCost>();
                List<ICLCompare> coverageLevelServices = new List<ICLCompare>();
                List<CostShareTier> costShareServices = new List<CostShareTier>();
                List<BenefitsCompare> benefitChartServices = new List<BenefitsCompare>();
                List<ICLCompare> preferredRetailInfo = new List<ICLCompare>();

                chartServices = ExecutePlanInformationRules();
                obj[RuleConstants.ANOCChartPlanInformation] = JsonConvert.SerializeObject(chartServices);

                chartServices = new List<CoveredCost>();
                chartServices = ExecutePlanPremiumInformationRules();
                obj[RuleConstants.ANOCChartPlanPremiumInformation] = JsonConvert.SerializeObject(chartServices);

                costShareServices = new List<CostShareTier>();
                costShareServices = ExecuteOutofPocketInformationRules();
                obj[RuleConstants.ANOCChartOutofPocketInformation] = JsonConvert.SerializeObject(costShareServices);

                benefitChartServices = new List<BenefitsCompare>();
                benefitChartServices = ExecuteANOCBenefitsCompareRules();
                obj[RuleConstants.ANOCChartANOCBenefitsCompare] = JsonConvert.SerializeObject(benefitChartServices);

                chartServices = new List<CoveredCost>();
                chartServices = ExecutePrescriptionInformationRules();
                obj[RuleConstants.ANOCChartPrescriptionInformation] = JsonConvert.SerializeObject(chartServices);

                coverageLevelServices = new List<ICLCompare>();
                coverageLevelServices = ExecuteInitialCoverageLimitInformationRules();
                obj[RuleConstants.ANOCChartInitialCoverageLimitInformation] = JsonConvert.SerializeObject(coverageLevelServices);

                coverageLevelServices = new List<ICLCompare>();
                coverageLevelServices = ExecuteGapCoverageLimitInformationRules();
                obj[RuleConstants.ANOCChartGapCoverageInformation] = JsonConvert.SerializeObject(coverageLevelServices);


                //DefaultServices
                List<DoctorOfficeVisit> officeVisits = new List<DoctorOfficeVisit>();
                List<InpatientHospitalStays> inpatientServices = new List<InpatientHospitalStays>();
                officeVisits = summaryChanges.GetDoctorOfficeVisitSummaryChange();
                inpatientServices = summaryChanges.GetInpatientHospitalStays();

                //DefaultServices

                obj[RuleConstants.ANOCChartDoctorOfficeVisitInfoPath] = JsonConvert.SerializeObject(officeVisits);
                obj[RuleConstants.ANOCChartIHSInfoPath] = JsonConvert.SerializeObject(inpatientServices);

                //Preffered Retail
                preferredRetailInfo = new List<ICLCompare>();
                preferredRetailInfo = ExecutePreferredRetailInformation();
                obj[RuleConstants.ANOCChartPreffedRetailInformation] = JsonConvert.SerializeObject(preferredRetailInfo);

                List<CostShareDetails> CostShareDetailsObj = new List<CostShareDetails>();
                CostShareDetailsObj = summaryChanges.CostShareDetailsChange();
                obj[RuleConstants.ANOCChartPlanDetailsCostShareDetails] = JsonConvert.SerializeObject(CostShareDetailsObj);

                serviceDataList.Add(obj);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
            }
            return JsonConvert.SerializeObject(serviceDataList);
        }
        public List<CoveredCost> ExecutePlanInformationRules()
        {
            List<CoveredCost> planInformationServices = new List<CoveredCost>();
            try
            {
                if (null != previousPBPViewJsonData && null != nextPBPViewJsonData && null != masterListAnocJsonData)
                {
                    CoveredCost coveredCostModel = new CoveredCost();
                    List<JToken> planInfoML = masterListAnocJsonData.SelectToken("ANOCChartBenefit.PlanInformation").ToList();
                    if (planInfoML != null && planInfoML.Count > 0)
                    {
                        JToken planInfo = planInfoML[0];
                        if (planInfo != null)
                        {
                            string planNamePath = Convert.ToString(planInfo["PlanName"] ?? String.Empty);
                            string planYearPath = Convert.ToString(planInfo["PlanYear"] ?? String.Empty);
                            string planNameParent = String.Empty; string planYearParent = String.Empty;

                            if (!String.IsNullOrEmpty(planNamePath))
                            {
                                planNameParent = planNamePath.Split('[')[0];
                                planNamePath = planNamePath.Split('[')[1].Split(']')[0];
                            }
                            if (!String.IsNullOrEmpty(planYearPath))
                            {
                                planYearParent = planYearPath.Split('[')[0];
                                planYearPath = planYearPath.Split('[')[1].Split(']')[0];
                            }

                            JToken previousPlanName = null; JToken nextPlanName = null;
                            if (!String.IsNullOrEmpty(planNameParent) && String.Equals(planNameParent, "PBPView"))
                            {
                                previousPlanName = previousPBPViewJsonData.SelectToken(planNamePath) ?? String.Empty;
                                nextPlanName = nextPBPViewJsonData.SelectToken(planNamePath) ?? String.Empty;
                            }
                            else if (!String.IsNullOrEmpty(planNameParent) && String.Equals(planNameParent, "Medicare"))
                            {
                                previousPlanName = previousMedicareJsonData.SelectToken(planNamePath) ?? String.Empty;
                                nextPlanName = nextMedicareJsonData.SelectToken(planNamePath) ?? String.Empty;
                            }

                            JToken previousPlanYear = null; JToken nextPlanYear = null;
                            if (!String.IsNullOrEmpty(planYearParent) && String.Equals(planYearParent, "PBPView"))
                            {
                                previousPlanYear = previousPBPViewJsonData.SelectToken(planYearPath) ?? String.Empty;
                                nextPlanYear = nextPBPViewJsonData.SelectToken(planYearPath) ?? String.Empty;
                            }
                            else if (!String.IsNullOrEmpty(planYearParent) && String.Equals(planYearParent, "Medicare"))
                            {
                                previousPlanYear = previousMedicareJsonData.SelectToken(planYearPath) ?? String.Empty;
                                nextPlanYear = nextMedicareJsonData.SelectToken(planYearPath) ?? String.Empty;
                            }
                            coveredCostModel = new CoveredCost();
                            //coveredCostModel.RowID = 1;
                            coveredCostModel.CoveredCosts = RuleConstants.PlanYear;
                            coveredCostModel.ThisYear = Convert.ToString(previousPlanYear);
                            coveredCostModel.NextYear = Convert.ToString(nextPlanYear);
                            coveredCostModel.DisplayinANOC = "Yes";
                            coveredCostModel.RowIDProperty = 0;
                            planInformationServices.Add(coveredCostModel);

                            coveredCostModel = new CoveredCost();
                            //coveredCostModel.RowID = 2;
                            coveredCostModel.CoveredCosts = RuleConstants.PlanName;
                            coveredCostModel.ThisYear = Convert.ToString(previousPlanName);
                            coveredCostModel.NextYear = Convert.ToString(nextPlanName);
                            coveredCostModel.DisplayinANOC = "Yes";
                            coveredCostModel.RowIDProperty = 1;
                            planInformationServices.Add(coveredCostModel);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
            }
            return planInformationServices;
        }
        public List<CoveredCost> ExecutePlanPremiumInformationRules()
        {
            List<CoveredCost> maxOOPCostShareInformationServices = new List<CoveredCost>();
            try
            {
                if (null != previousMedicareJsonData && null != nextMedicareJsonData && null != masterListAnocJsonData)
                {
                    List<JToken> planPremiumML = masterListAnocJsonData.SelectToken("ANOCChartBenefit.PlanPremiumInformation").ToList();
                    if (planPremiumML != null && planPremiumML.Count > 0)
                    {
                        JToken planPremiumInfo = planPremiumML[0];
                        if (planPremiumInfo != null)
                        {
                            string planPremiumPath = Convert.ToString(planPremiumInfo["MonthlyPlanPremium"] ?? String.Empty);
                            string planPremiumParent = String.Empty;
                            if (!String.IsNullOrEmpty(planPremiumPath))
                            {
                                planPremiumParent = planPremiumPath.Split('[')[0];
                                planPremiumPath = planPremiumPath.Split('[')[1].Split(']')[0];
                            }
                            CoveredCost coveredCostModel = new CoveredCost();
                            JToken previousMonthlyPremiumAmount = null; JToken nextMonthlyPremiumAmount = null;
                            if (!String.IsNullOrEmpty(planPremiumParent) && String.Equals(planPremiumParent, "Medicare"))
                            {
                                nextMonthlyPremiumAmount = nextMedicareJsonData.SelectToken(planPremiumPath) ?? String.Empty;
                                previousMonthlyPremiumAmount = previousMedicareJsonData.SelectToken(planPremiumPath) ?? String.Empty;
                            }
                            else if (!String.IsNullOrEmpty(planPremiumParent) && String.Equals(planPremiumParent, "PBPView"))
                            {
                                nextMonthlyPremiumAmount = nextPBPViewJsonData.SelectToken(planPremiumPath) ?? String.Empty;
                                previousMonthlyPremiumAmount = previousPBPViewJsonData.SelectToken(planPremiumPath) ?? String.Empty;
                            }
                            //coveredCostModel.RowID = 1;
                            coveredCostModel.CoveredCosts = RuleConstants.MonthlyPlanPremium;
                            coveredCostModel.ThisYear = Convert.ToString(previousMonthlyPremiumAmount) == String.Empty ? RuleConstants.NotApplicable : Convert.ToString(previousMonthlyPremiumAmount);
                            coveredCostModel.NextYear = Convert.ToString(nextMonthlyPremiumAmount) == String.Empty ? RuleConstants.NotApplicable : Convert.ToString(nextMonthlyPremiumAmount);
                            coveredCostModel.DisplayinANOC = "Yes";
                            coveredCostModel.RowIDProperty = 0;
                            maxOOPCostShareInformationServices.Add(coveredCostModel);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
            }
            return maxOOPCostShareInformationServices;
        }
        public List<CostShareTier> ExecuteOutofPocketInformationRules()
        {
            List<CostShareTier> CostShareInformationServices = new List<CostShareTier>();
            try
            {
                CostShareTier costShareTier = new CostShareTier();

                string inMOOPPath = String.Empty; string outMOOPPath = String.Empty; string combinedMOOPPath = String.Empty;
                string inMOOPParent = String.Empty; string outMOOPParent = String.Empty; string combinedMOOPParent = String.Empty;
                if (null != previousPBPViewJsonData && null != nextPBPViewJsonData && null != masterListAnocJsonData)
                {
                    List<JToken> oopInfoML = masterListAnocJsonData.SelectToken("ANOCChartBenefit.OutofPocketInformation").ToList();

                    //Plan Deductible info - Start
                    JToken prevYearPlanDedAmount = null; JToken nextYearRxPlanDedAmount = null;
                    List<JToken> planDeductibleML = masterListAnocJsonData.SelectToken("ANOCChartBenefit.DeductibleInformation").ToList();
                    if (planDeductibleML != null && planDeductibleML.Count > 0)
                    {
                        JToken planDeductibleMLInfo = planDeductibleML[0];
                        if (planDeductibleMLInfo != null)
                        {
                            string PlanDedParent = string.Empty; string PlanDedPath = string.Empty;
                            string dedAmtPath = planDeductibleMLInfo["DeductibleAmount"] != null ? planDeductibleMLInfo["DeductibleAmount"].ToString() : string.Empty;
                            if (!String.IsNullOrEmpty(dedAmtPath))
                            {
                                PlanDedParent = dedAmtPath.Split('[')[0];
                                PlanDedPath = dedAmtPath.Split('[')[1].Split(']')[0];
                            }

                            if (!String.IsNullOrEmpty(PlanDedParent) && String.Equals(PlanDedParent, "Medicare"))
                            {
                                prevYearPlanDedAmount = previousMedicareJsonData.SelectToken(PlanDedPath) ?? String.Empty;
                                nextYearRxPlanDedAmount = nextMedicareJsonData.SelectToken(PlanDedPath) ?? String.Empty;
                            }
                            else if (!String.IsNullOrEmpty(PlanDedParent) && String.Equals(PlanDedParent, "PBPView"))
                            {
                                prevYearPlanDedAmount = previousPBPViewJsonData.SelectToken(PlanDedPath) ?? String.Empty;
                                nextYearRxPlanDedAmount = nextPBPViewJsonData.SelectToken(PlanDedPath) ?? String.Empty;
                            }

                        }
                    }
                    //Plan Deductible info - End

                    if (oopInfoML != null && oopInfoML.Count > 0)
                    {
                        JToken oopInfo = oopInfoML[0];
                        if (oopInfo != null)
                        {
                            bool isNextPlanTypValid = CheckViewSectionDPlanLevelVisibility(true);
                            bool isPrevPlanTypValid = CheckViewSectionDPlanLevelVisibility(false);

                            inMOOPPath = Convert.ToString(oopInfo["InNetworkMaximumEnrolleeOutofPocket"] ?? String.Empty);
                            outMOOPPath = Convert.ToString(oopInfo["OutofNetworkMaximumEnrolleeOutofPocket"] ?? String.Empty);
                            combinedMOOPPath = Convert.ToString(oopInfo["CombinedMaximumEnrolleeOutofPocket"] ?? String.Empty);
                            if (!String.IsNullOrEmpty(inMOOPPath))
                            {
                                inMOOPParent = inMOOPPath.Split('[')[0];
                                inMOOPPath = inMOOPPath.Split('[')[1].Split(']')[0];
                            }
                            if (!String.IsNullOrEmpty(outMOOPPath))
                            {
                                outMOOPParent = outMOOPPath.Split('[')[0];
                                outMOOPPath = outMOOPPath.Split('[')[1].Split(']')[0];
                            }
                            if (!String.IsNullOrEmpty(combinedMOOPPath))
                            {
                                combinedMOOPParent = combinedMOOPPath.Split('[')[0];
                                combinedMOOPPath = combinedMOOPPath.Split('[')[1].Split(']')[0];
                            }
                            JToken previousInMOOPAmount = null; JToken nextInMOOPAmount = null;
                            if (!String.IsNullOrEmpty(inMOOPParent) && String.Equals(inMOOPParent, "PBPView"))
                            {
                                previousInMOOPAmount = previousPBPViewJsonData.SelectToken(inMOOPPath) ?? String.Empty;
                                nextInMOOPAmount = nextPBPViewJsonData.SelectToken(inMOOPPath) ?? String.Empty;
                            }
                            else if (!String.IsNullOrEmpty(inMOOPParent) && String.Equals(inMOOPParent, "Medicare"))
                            {
                                previousInMOOPAmount = previousMedicareJsonData.SelectToken(inMOOPPath) ?? String.Empty;
                                nextInMOOPAmount = nextMedicareJsonData.SelectToken(inMOOPPath) ?? String.Empty;
                            }

                            JToken previousOutMOOPAmount = null; JToken nextOutMOOPAmount = null;
                            if (!String.IsNullOrEmpty(outMOOPParent) && String.Equals(outMOOPParent, "PBPView"))
                            {
                                previousOutMOOPAmount = previousPBPViewJsonData.SelectToken(outMOOPPath) ?? String.Empty;
                                nextOutMOOPAmount = nextPBPViewJsonData.SelectToken(outMOOPPath) ?? String.Empty;
                            }
                            else if (!String.IsNullOrEmpty(outMOOPParent) && String.Equals(outMOOPParent, "Medicare"))
                            {
                                previousOutMOOPAmount = previousMedicareJsonData.SelectToken(outMOOPPath) ?? String.Empty;
                                nextOutMOOPAmount = nextMedicareJsonData.SelectToken(outMOOPPath) ?? String.Empty;
                            }
                            JToken previousCombinedMOOPAmount = null; JToken nextCombinedMOOPAmount = null;
                            if (!String.IsNullOrEmpty(combinedMOOPParent) && String.Equals(combinedMOOPParent, "PBPView"))
                            {
                                previousCombinedMOOPAmount = previousPBPViewJsonData.SelectToken(combinedMOOPPath) ?? String.Empty;
                                nextCombinedMOOPAmount = nextPBPViewJsonData.SelectToken(combinedMOOPPath) ?? String.Empty;
                            }
                            else if (!String.IsNullOrEmpty(combinedMOOPParent) && String.Equals(combinedMOOPParent, "Medicare"))
                            {
                                previousCombinedMOOPAmount = previousMedicareJsonData.SelectToken(combinedMOOPPath) ?? String.Empty;
                                nextCombinedMOOPAmount = nextMedicareJsonData.SelectToken(combinedMOOPPath) ?? String.Empty;
                            }

                            
                            //costShareTier.RowID = 1;
                            costShareTier.CoveredCosts = "In Network Maximum Enrollee Out of Pocket";
                            if (isPrevPlanTypValid)
                                costShareTier.ThisYear = Convert.ToString(previousInMOOPAmount) == String.Empty ? RuleConstants.NotApplicable : Convert.ToString(previousInMOOPAmount);
                            else
                                costShareTier.ThisYear = RuleConstants.NotCovered;

                            if (isNextPlanTypValid)
                                costShareTier.NextYear = Convert.ToString(nextInMOOPAmount) == String.Empty ? RuleConstants.NotApplicable : Convert.ToString(nextInMOOPAmount);
                            else
                                costShareTier.NextYear = RuleConstants.NotCovered;

                            costShareTier.DisplayinANOC = "Yes";
                            costShareTier.RowIDProperty = 0;
                            CostShareInformationServices.Add(costShareTier);

                            costShareTier = new CostShareTier();
                            //costShareTier.RowID = 2;
                            costShareTier.CoveredCosts = "Out of Network Maximum Enrollee Out of Pocket";
                            if (isPrevPlanTypValid)
                                costShareTier.ThisYear = Convert.ToString(previousOutMOOPAmount) == String.Empty ? RuleConstants.NotApplicable : Convert.ToString(previousOutMOOPAmount);
                            else
                                costShareTier.ThisYear = RuleConstants.NotCovered;

                            if (isNextPlanTypValid)
                                costShareTier.NextYear = Convert.ToString(nextOutMOOPAmount) == String.Empty ? RuleConstants.NotApplicable : Convert.ToString(nextOutMOOPAmount);
                            else
                                costShareTier.NextYear = RuleConstants.NotCovered;

                            costShareTier.DisplayinANOC = "Yes";
                            costShareTier.RowIDProperty = 1;
                            CostShareInformationServices.Add(costShareTier);

                            costShareTier = new CostShareTier();
                            //costShareTier.RowID = 3;
                            costShareTier.CoveredCosts = "In Combined Maximum Enrollee Out of Pocket";
                            if (isPrevPlanTypValid)
                                costShareTier.ThisYear = Convert.ToString(previousCombinedMOOPAmount) == String.Empty ? RuleConstants.NotApplicable : Convert.ToString(previousCombinedMOOPAmount);
                            else
                                costShareTier.ThisYear = RuleConstants.NotCovered;

                            if (isNextPlanTypValid)
                                costShareTier.NextYear = Convert.ToString(nextCombinedMOOPAmount) == String.Empty ? RuleConstants.NotApplicable : Convert.ToString(nextCombinedMOOPAmount);
                            else
                                costShareTier.NextYear = RuleConstants.NotCovered;

                            costShareTier.DisplayinANOC = "Yes";
                            costShareTier.RowIDProperty = 2;
                            CostShareInformationServices.Add(costShareTier);


                            //Plan Deductible Amount - Start
                            costShareTier = new CostShareTier();
                            costShareTier.RowIDProperty = 3;
                            costShareTier.CoveredCosts = RuleConstants.PlanDeductibleAmount;
                            costShareTier.NextYear = nextYearRxPlanDedAmount.ToString();
                            costShareTier.ThisYear = prevYearPlanDedAmount.ToString();
                            costShareTier.DisplayinANOC = "true";
                            CostShareInformationServices.Add(costShareTier);

                            //Plan Deductible Amount - End


                        }
                    }

                    JToken prevYearPlanInpatientAcuteMaxAmountOOP = null; JToken nextYearRxPlanInpatientAcuteMaxAmountOOP = null;
                    JToken prevYearPlanInpatientAcuteMaxPeriodicityOOP = null; JToken nextYearRxPlanInpatientAcuteMaxPeriodicityOOP = null;

                    prevYearPlanInpatientAcuteMaxAmountOOP = previousPBPViewJsonData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase2.IndicatetheMaximumEnrolleeOutofPocketCostamount") ?? String.Empty;
                    nextYearRxPlanInpatientAcuteMaxAmountOOP = nextPBPViewJsonData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase2.IndicatetheMaximumEnrolleeOutofPocketCostamount") ?? String.Empty;
                    prevYearPlanInpatientAcuteMaxPeriodicityOOP = previousPBPViewJsonData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase2.SelecttheMaximumEnrolleeOutofPocketCostperiodicity") ?? String.Empty;
                    nextYearRxPlanInpatientAcuteMaxPeriodicityOOP = nextPBPViewJsonData.SelectToken("aInpatientHospitalServicesAcute.InpatientAcuteBase2.SelecttheMaximumEnrolleeOutofPocketCostperiodicity") ?? String.Empty;
                    
                    costShareTier = new CostShareTier();
                    costShareTier.RowIDProperty = 4;
                    costShareTier.CoveredCosts = RuleConstants.InpatientAcuteMaxOOPAmount;
                    costShareTier.NextYear = Convert.ToString(nextYearRxPlanInpatientAcuteMaxAmountOOP) == String.Empty ? RuleConstants.NotApplicable : Convert.ToString(nextYearRxPlanInpatientAcuteMaxAmountOOP);
                    costShareTier.ThisYear = Convert.ToString(prevYearPlanInpatientAcuteMaxAmountOOP) == String.Empty ? RuleConstants.NotApplicable : Convert.ToString(prevYearPlanInpatientAcuteMaxAmountOOP);
                    costShareTier.DisplayinANOC = "true";
                    CostShareInformationServices.Add(costShareTier);

                    costShareTier = new CostShareTier();
                    costShareTier.RowIDProperty = 5;
                    costShareTier.CoveredCosts = RuleConstants.InaptientAcuteMaxOOPPeriodicity;
                    costShareTier.NextYear = GetOOPMPeriodicityAcute(Convert.ToString(nextYearRxPlanInpatientAcuteMaxPeriodicityOOP)) == String.Empty ? RuleConstants.NotApplicable : GetOOPMPeriodicityAcute(Convert.ToString(nextYearRxPlanInpatientAcuteMaxPeriodicityOOP));
                    costShareTier.ThisYear = GetOOPMPeriodicityAcute(Convert.ToString(prevYearPlanInpatientAcuteMaxPeriodicityOOP)) == String.Empty ? RuleConstants.NotApplicable : GetOOPMPeriodicityAcute(Convert.ToString(prevYearPlanInpatientAcuteMaxPeriodicityOOP));
                    costShareTier.DisplayinANOC = "true";
                    CostShareInformationServices.Add(costShareTier);

                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
            }
            return CostShareInformationServices;
        }
        public List<BenefitsCompare> ExecuteANOCBenefitsCompareRules()
        {
            List<BenefitsCompare> benefitsCompareServices = new List<BenefitsCompare>();
            List<BenefitsCompare> mergedServices = new List<BenefitsCompare>();

            BenefitsCompare benefitsCompare = new BenefitsCompare();
            try
            {
                List<JToken> previousYearBRGList = GetBRGServices(false, false, null);
                List<JToken> nextYearBRGList = GetBRGServices(true, false, null);
                //process OON network tier service


                if (AnocHelper.IsPPOPlan(false) && AnocHelper.IsContainOONNetworkTier(false))
                {
                    ProcessOONService(previousYearBRGList, false);
                }
                else if (AnocHelper.IsPOSPlan(false))
                {
                    ProcessOONService(previousYearBRGList, false);
                }

                if (AnocHelper.IsPPOPlan(true) && AnocHelper.IsContainOONNetworkTier(true))
                {
                    ProcessOONService(nextYearBRGList, true);
                }
                else if (AnocHelper.IsPOSPlan(true))
                {
                    ProcessOONService(nextYearBRGList, true);
                }




                //if (AnocHelper.IsContainOONNetworkTier(false))
                //{
                //    ProcessOONService(previousYearBRGList, false);
                //}
                //if (AnocHelper.IsContainOONNetworkTier(true))
                //{
                //    ProcessOONService(nextYearBRGList, true);
                //}



                List<JToken> resultForTierUnion = new List<JToken>();
                Dictionary<string, string> keys = new Dictionary<string, string>();
                keys.Add(RuleConstants.BenefitCategory1, RuleConstants.BenefitCategory1);
                keys.Add(RuleConstants.BenefitCategory2, RuleConstants.BenefitCategory2);
                keys.Add(RuleConstants.BenefitCategory3, RuleConstants.BenefitCategory3);
                keys.Add(RuleConstants.CostShareTiers, RuleConstants.CostShareTiers);

                CollectionExecutionComparer operatorProcessor = new CollectionExecutionComparer(previousYearBRGList, nextYearBRGList, keys);
                resultForTierUnion = operatorProcessor.Union();

                bool hasBlankRow = false;
                int rowID = 0;
                foreach (var token in resultForTierUnion)
                {
                    rowID = rowID + 1;
                    benefitsCompare = new BenefitsCompare();
                    hasBlankRow = HasBlankRow(token);
                    if (!hasBlankRow)
                    {
                        // benefitsCompare.RowID = rowID;
                        benefitsCompare.ANOCBenefitName = String.Empty;
                        benefitsCompare.BenefitCategory1 = Convert.ToString(token[RuleConstants.BenefitCategory1] ?? String.Empty);
                        benefitsCompare.BenefitCategory2 = Convert.ToString(token[RuleConstants.BenefitCategory2] ?? String.Empty);
                        benefitsCompare.BenefitCategory3 = Convert.ToString(token[RuleConstants.BenefitCategory3] ?? String.Empty);
                        benefitsCompare.CostShareTiers = Convert.ToString(token["Tier"] ?? String.Empty);
                        benefitsCompare.NextYear = GetCostShareValues(benefitsCompare.BenefitCategory1, benefitsCompare.BenefitCategory2, benefitsCompare.BenefitCategory3, benefitsCompare.CostShareTiers, previousYearBRGList, nextYearBRGList, token, true, false);
                        benefitsCompare.ThisYear = GetCostShareValues(benefitsCompare.BenefitCategory1, benefitsCompare.BenefitCategory2, benefitsCompare.BenefitCategory3, benefitsCompare.CostShareTiers, previousYearBRGList, nextYearBRGList, token, false, false);
                        benefitsCompare.DisplayinANOC = "Yes";
                        benefitsCompare.RowIDProperty = rowID - 1;
                        // Check if all network values are blank
                        if (!HasSameCostShare(benefitsCompare))
                            benefitsCompareServices.Add(benefitsCompare);
                    }
                }
                // Add default service in Comparision
                benefitsCompareServices = GetDefaultServiceCostShareValues(previousYearBRGList, nextYearBRGList, benefitsCompareServices);

                // Add sliding costshare services
                benefitsCompareServices = GetSlidingCostShareServices(benefitsCompareServices);

                // Set RowIDProperty for each Row
                benefitsCompareServices = AddRowIDProperty(benefitsCompareServices);

                // Merge All tiers for same services into single service with This And Next Year
                if (benefitsCompareServices != null && benefitsCompareServices.Count > 0)
                {
                    foreach (var ser in benefitsCompareServices)
                    {
                        bool exist = mergedServices.Exists(x => x.BenefitCategory1 == ser.BenefitCategory1
                                                && (x.BenefitCategory2 == ser.BenefitCategory2)
                                                && (x.BenefitCategory3 == ser.BenefitCategory3));
                        string pTagPrefix = "<p style=\"font-size:   12pt; font-family: 'Times New Roman', serif; margin-top: 6pt;\">";
                        string pTagPostFix = "</p>";
                        if (exist)
                        {
                            var service = mergedServices.Where(x => x.BenefitCategory1 == ser.BenefitCategory1
                                                && (x.BenefitCategory2 == ser.BenefitCategory2)
                                                && (x.BenefitCategory3 == ser.BenefitCategory3)).FirstOrDefault();

                            if (service != null)
                            {
                                if (ser.CostShareTiers.Equals(RuleConstants.OONPOS) || ser.CostShareTiers.Equals(RuleConstants.OONPPO))
                                {
                                    ser.CostShareTiers = "OON";
                                }
                                if (!String.IsNullOrEmpty(ser.NextYear) && !String.IsNullOrEmpty(ser.NextYear.Replace("<br/>", "").Trim()))
                                {
                                    service.NextYear += pTagPrefix + "<b>" + TierFormatter( ser.CostShareTiers) + "Network" + "</b>" + pTagPostFix + ser.NextYear;
                                }
                                if (!String.IsNullOrEmpty(ser.ThisYear) && !String.IsNullOrEmpty(ser.ThisYear.Replace("<br/>", "").Trim()))
                                {
                                    service.ThisYear += pTagPrefix + "<b>" + TierFormatter(ser.CostShareTiers) + "Network" + "</b>" + pTagPostFix + ser.ThisYear;
                                }
                                //service.CostShareTiers += "<br/>" + ser.CostShareTiers;
                                service.CostShareTiers += "," + ser.CostShareTiers;
                            }
                        }
                        else
                        {
                            if (ser.CostShareTiers.Equals(RuleConstants.OONPOS) || ser.CostShareTiers.Equals(RuleConstants.OONPPO))
                            {
                                ser.CostShareTiers = "OON";
                            }
                            string prefix = "<b>" + TierFormatter(ser.CostShareTiers) + "Network" + "</b>";
                            if (!String.IsNullOrEmpty(ser.NextYear) && !String.IsNullOrEmpty(ser.NextYear.Replace("<br/>", "").Trim()))
                            {
                                string nextYr = prefix + ser.NextYear;
                                ser.NextYear = nextYr;
                            }
                            if (!String.IsNullOrEmpty(ser.ThisYear) && !String.IsNullOrEmpty(ser.ThisYear.Replace("<br/>", "").Trim()))
                            {
                                string thisYr = prefix + ser.ThisYear;
                                ser.ThisYear = thisYr;
                            }
                            //string prefix = "<b>" + ser.CostShareTiers + "Network" + "</b>";
                            //string nextYr = prefix + ser.NextYear;
                            //string thisYr = prefix + ser.ThisYear;
                            mergedServices.Add(ser);
                        }
                    }
                    // Set RowIDProperty for each Row
                    benefitsCompareServices = AddRowIDProperty(mergedServices);
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
            }
            return benefitsCompareServices;
        }
        public List<CoveredCost> ExecutePrescriptionInformationRules()
        {
            List<CoveredCost> prescriptionInformationServices = new List<CoveredCost>();
            try
            {
                CoveredCost coveredCost = new CoveredCost();
                int rowId = 0;
                bool isPrevRxPlan = false; bool isNextRxPlan = false;
                if (null != previousMedicareJsonData && null != nextMedicareJsonData && null != masterListAnocJsonData)
                {
                    string isRx = Convert.ToString(previousMedicareJsonData.SelectToken("SECTIONASECTIONA1.DoesyourPlanofferaPrescriptionPartDbenefit") ?? String.Empty) == String.Empty ? "NO" : Convert.ToString(previousMedicareJsonData.SelectToken("SECTIONASECTIONA1.DoesyourPlanofferaPrescriptionPartDbenefit"));
                    if (!String.IsNullOrEmpty(isRx))
                    {
                        if (isRx.Equals("YES"))
                            isPrevRxPlan = true;
                        else
                            isPrevRxPlan = false;
                    }
                    isRx = Convert.ToString(nextMedicareJsonData.SelectToken("SECTIONASECTIONA1.DoesyourPlanofferaPrescriptionPartDbenefit") ?? String.Empty) == String.Empty ? "NO" : Convert.ToString(nextMedicareJsonData.SelectToken("SECTIONASECTIONA1.DoesyourPlanofferaPrescriptionPartDbenefit"));
                    if (!String.IsNullOrEmpty(isRx))
                    {
                        if (isRx.Equals("YES"))
                            isNextRxPlan = true;
                        else
                            isNextRxPlan = false;
                    }
                    List<JToken> prescriptionML = masterListAnocJsonData.SelectToken("ANOCChartBenefit.PrescriptionInformation").ToList();

                    if (prescriptionML != null && prescriptionML.Count > 0)
                    {
                        JToken prescriptionMLInfo = prescriptionML[0];
                        if (prescriptionMLInfo != null)
                        {
                            string dedAmtParent = String.Empty; string dedTierParent = String.Empty; string gapCoverageAmtParent = String.Empty; string catastrophicCoverageAmtParent = String.Empty;
                            string NoOfRxTiersParent = string.Empty; string NoOfTiersPath = string.Empty;

                            string dedAmtPath = Convert.ToString(prescriptionMLInfo["PrescriptionDeductible"] ?? String.Empty);
                            if (!String.IsNullOrEmpty(dedAmtPath))
                            {
                                dedAmtParent = dedAmtPath.Split('[')[0];
                                dedAmtPath = dedAmtPath.Split('[')[1].Split(']')[0];
                            }
                            string dedTierPath = Convert.ToString(prescriptionMLInfo["TierListforWhichDeductibleApplies"] ?? String.Empty);
                            if (!String.IsNullOrEmpty(dedTierPath))
                            {
                                dedTierParent = dedTierPath.Split('[')[0];
                                dedTierPath = dedTierPath.Split('[')[1].Split(']')[0];
                            }
                            string gapCoverageAmtPath = Convert.ToString(prescriptionMLInfo["GapCoverageAmount"] ?? String.Empty);
                            if (!String.IsNullOrEmpty(gapCoverageAmtPath))
                            {
                                gapCoverageAmtParent = gapCoverageAmtPath.Split('[')[0];
                                gapCoverageAmtPath = gapCoverageAmtPath.Split('[')[1].Split(']')[0];
                            }


                            NoOfTiersPath = Convert.ToString(prescriptionMLInfo["NumberofTiers"] ?? String.Empty);
                            if (!String.IsNullOrEmpty(NoOfTiersPath))
                            {
                                NoOfRxTiersParent = NoOfTiersPath.Split('[')[0];
                                NoOfTiersPath = NoOfTiersPath.Split('[')[1].Split(']')[0];
                            }

                            // Get Default value for CatastrophicCoverageAmount
                            //string catastrophicCoverageAmt = Convert.ToString(prescriptionMLInfo["CatastrophicCoverageAmount"] ?? String.Empty);

                            string catastrophicCoverageAmtPath = Convert.ToString(prescriptionMLInfo["CatastrophicCoverageAmount"] ?? String.Empty);


                            if (!String.IsNullOrEmpty(catastrophicCoverageAmtPath))
                            {
                                catastrophicCoverageAmtParent = catastrophicCoverageAmtPath.Split('[')[0];
                                catastrophicCoverageAmtPath = catastrophicCoverageAmtPath.Split('[')[1].Split(']')[0];
                            }

                            string previouscatastrophicCoverageAmt = null; string nextcatastrophicCoverageAmt = null;
                            if (!String.IsNullOrEmpty(catastrophicCoverageAmtParent) && String.Equals(catastrophicCoverageAmtParent, "Medicare"))
                            {
                                previouscatastrophicCoverageAmt = Convert.ToString(previousMedicareJsonData.SelectToken(catastrophicCoverageAmtPath) ?? String.Empty);
                                nextcatastrophicCoverageAmt = Convert.ToString(nextMedicareJsonData.SelectToken(catastrophicCoverageAmtPath) ?? String.Empty);
                            }
                            else if (!String.IsNullOrEmpty(catastrophicCoverageAmtParent) && String.Equals(catastrophicCoverageAmtParent, "PBPView"))
                            {
                                nextcatastrophicCoverageAmt = Convert.ToString(previousPBPViewJsonData.SelectToken(catastrophicCoverageAmtPath) ?? String.Empty);
                                nextcatastrophicCoverageAmt = Convert.ToString(nextPBPViewJsonData.SelectToken(catastrophicCoverageAmtPath) ?? String.Empty);
                            }


                            //Number of Tiers - Start
                            JToken previousRxTiersCount = null; JToken nextRxTiersCount = null;
                            if (!String.IsNullOrEmpty(NoOfRxTiersParent) && String.Equals(NoOfRxTiersParent, "Medicare"))
                            {
                                previousRxTiersCount = previousMedicareJsonData.SelectToken(NoOfTiersPath) ?? String.Empty;
                                nextRxTiersCount = nextMedicareJsonData.SelectToken(NoOfTiersPath) ?? String.Empty;
                            }
                            else if (!String.IsNullOrEmpty(dedAmtParent) && String.Equals(dedAmtParent, "PBPView"))
                            {
                                previousRxTiersCount = previousPBPViewJsonData.SelectToken(NoOfTiersPath) ?? String.Empty;
                                nextRxTiersCount = nextPBPViewJsonData.SelectToken(NoOfTiersPath) ?? String.Empty;
                            }

                            //Number of Tiers - End


                            JToken previousDeductibleAmt = null; JToken nextDeductibleAmt = null;
                            if (!String.IsNullOrEmpty(dedAmtParent) && String.Equals(dedAmtParent, "Medicare"))
                            {
                                previousDeductibleAmt = previousMedicareJsonData.SelectToken(dedAmtPath) ?? String.Empty;
                                nextDeductibleAmt = nextMedicareJsonData.SelectToken(dedAmtPath) ?? String.Empty;
                            }
                            else if (!String.IsNullOrEmpty(dedAmtParent) && String.Equals(dedAmtParent, "PBPView"))
                            {
                                previousDeductibleAmt = previousPBPViewJsonData.SelectToken(dedAmtPath) ?? String.Empty;
                                nextDeductibleAmt = nextPBPViewJsonData.SelectToken(dedAmtPath) ?? String.Empty;
                            }

                            rowId = rowId + 1;
                            //coveredCost.RowID = rowId;
                            coveredCost.RowIDProperty = rowId - 1;
                            coveredCost.CoveredCosts = RuleConstants.PrescriptionDeductible;
                            if (isNextRxPlan)
                                coveredCost.NextYear = Convert.ToString(nextDeductibleAmt) == String.Empty ? RuleConstants.NotApplicable : Convert.ToString(nextDeductibleAmt);
                            else
                                coveredCost.NextYear = RuleConstants.NotCovered;

                            if (isPrevRxPlan)
                                coveredCost.ThisYear = Convert.ToString(previousDeductibleAmt) == String.Empty ? RuleConstants.NotApplicable : Convert.ToString(previousDeductibleAmt);
                            else
                                coveredCost.ThisYear = RuleConstants.NotCovered;
                            coveredCost.DisplayinANOC = "Yes";
                            prescriptionInformationServices.Add(coveredCost);


                            List<JToken> previousDeductibleTierList = new List<JToken>();
                            List<JToken> nextDeductibleTierList = new List<JToken>();
                            if (!String.IsNullOrEmpty(dedTierParent) && String.Equals(dedTierParent, "Medicare"))
                            {
                                previousDeductibleTierList = previousMedicareJsonData.SelectToken(dedTierPath) == null ? new List<JToken>() : previousMedicareJsonData.SelectToken(dedTierPath).ToList();
                                nextDeductibleTierList = nextMedicareJsonData.SelectToken(dedTierPath) == null ? new List<JToken>() : nextMedicareJsonData.SelectToken(dedTierPath).ToList();
                            }
                            else if (!String.IsNullOrEmpty(dedTierParent) && String.Equals(dedTierParent, "PBPView"))
                            {
                                previousDeductibleTierList = previousPBPViewJsonData.SelectToken(dedTierPath) == null ? new List<JToken>() : previousPBPViewJsonData.SelectToken(dedTierPath).ToList();
                                nextDeductibleTierList = nextPBPViewJsonData.SelectToken(dedTierPath) == null ? new List<JToken>() : nextPBPViewJsonData.SelectToken(dedTierPath).ToList();
                            }

                            if (null != previousDeductibleTierList && null != nextDeductibleTierList)
                            {
                                coveredCost = new CoveredCost();
                                rowId = rowId + 1;
                                //coveredCost.RowID = rowId;
                                coveredCost.RowIDProperty = rowId - 1;
                                coveredCost.CoveredCosts = RuleConstants.DeductibleAppliedTier;
                                coveredCost.DisplayinANOC = "Yes";

                                string value = String.Empty;
                                int previousTierCount = previousDeductibleTierList.Count;
                                if (isPrevRxPlan)
                                {
                                    if (previousTierCount > 0)
                                    {
                                        value = String.Join(",", previousDeductibleTierList);
                                        coveredCost.ThisYear = value;
                                    }
                                    else
                                        coveredCost.ThisYear = RuleConstants.NotApplicable;
                                }
                                else
                                    coveredCost.ThisYear = RuleConstants.NotCovered;

                                value = String.Empty;
                                int nextTierCount = nextDeductibleTierList.Count;
                                if (isNextRxPlan)
                                {
                                    if (nextTierCount > 0)
                                    {
                                        value = String.Join(",", nextDeductibleTierList);
                                        coveredCost.NextYear = value;
                                    }
                                    else
                                        coveredCost.NextYear = RuleConstants.NotApplicable;
                                }
                                else
                                    coveredCost.NextYear = RuleConstants.NotCovered;

                                prescriptionInformationServices.Add(coveredCost);
                            }

                            // GapCoverageAmount
                            JToken previousGapCoverageAmt = null; JToken nextGapCoverageAmt = null;
                            if (!String.IsNullOrEmpty(gapCoverageAmtParent) && String.Equals(gapCoverageAmtParent, "Medicare"))
                            {
                                previousGapCoverageAmt = previousMedicareJsonData.SelectToken(gapCoverageAmtPath) ?? String.Empty;
                                nextGapCoverageAmt = nextMedicareJsonData.SelectToken(gapCoverageAmtPath) ?? String.Empty;
                            }
                            else if (!String.IsNullOrEmpty(gapCoverageAmtParent) && String.Equals(gapCoverageAmtParent, "PBPView"))
                            {
                                previousGapCoverageAmt = previousPBPViewJsonData.SelectToken(gapCoverageAmtPath) ?? String.Empty;
                                nextGapCoverageAmt = nextPBPViewJsonData.SelectToken(gapCoverageAmtPath) ?? String.Empty;
                            }

                            coveredCost = new CoveredCost();
                            rowId = rowId + 1;
                            //coveredCost.RowID = rowId;
                            coveredCost.RowIDProperty = rowId - 1;
                            coveredCost.CoveredCosts = RuleConstants.GapCoverageAmount;
                            if (isNextRxPlan)
                                coveredCost.NextYear = Convert.ToString(nextGapCoverageAmt) == String.Empty ? RuleConstants.NotApplicable : GetAmount(Convert.ToString(nextGapCoverageAmt));
                            else
                                coveredCost.NextYear = RuleConstants.NotCovered;

                            if (isPrevRxPlan)
                                coveredCost.ThisYear = Convert.ToString(previousGapCoverageAmt) == String.Empty ? RuleConstants.NotApplicable : GetAmount(Convert.ToString(previousGapCoverageAmt));
                            else
                                coveredCost.ThisYear = RuleConstants.NotCovered;

                            coveredCost.DisplayinANOC = "Yes";
                            prescriptionInformationServices.Add(coveredCost);

                            // CatastrophicCoverageAmt
                            coveredCost = new CoveredCost();
                            rowId = rowId + 1;
                            //coveredCost.RowID = rowId;
                            coveredCost.RowIDProperty = rowId - 1;
                            coveredCost.CoveredCosts = RuleConstants.CatastrophicAmount;

                            // Comment condition as of now as we are populating hardcoded value from ML
                            //if (isNextRxPlan)
                            //    coveredCost.NextYear = catastrophicCoverageAmt;
                            //else
                            //    coveredCost.NextYear = RuleConstants.NotCovered;
                            coveredCost.NextYear = nextcatastrophicCoverageAmt;

                            // Comment condition as of now as we are populating hardcoded value from ML
                            //if (isPrevRxPlan)
                            //    coveredCost.ThisYear = catastrophicCoverageAmt;
                            //else
                            //    coveredCost.ThisYear = RuleConstants.NotCovered;
                            coveredCost.ThisYear = previouscatastrophicCoverageAmt;

                            coveredCost.DisplayinANOC = "Yes";
                            prescriptionInformationServices.Add(coveredCost);

                            //No Of Tiers Row Insert - Start

                            coveredCost = new CoveredCost();
                            rowId = rowId + 1;
                            int nextYearTierCount = 0; int prevYearTiersCount = 0;
                            int nextYearNoOfTies = int.TryParse(Convert.ToString(nextRxTiersCount), out nextYearTierCount) ? nextYearTierCount : 0;
                            int prevYearNoOfTies = int.TryParse(Convert.ToString(previousRxTiersCount), out prevYearTiersCount) ? prevYearTiersCount : 0;

                            coveredCost.RowIDProperty = rowId - 1;
                            coveredCost.CoveredCosts = RuleConstants.RxNumbersOfTiers;
                            coveredCost.NextYear = nextYearNoOfTies.ToString();
                            coveredCost.ThisYear = prevYearNoOfTies.ToString();
                            coveredCost.DisplayinANOC = "true";
                            prescriptionInformationServices.Add(coveredCost);

                            //No Of Tiers Row Insert - End

                            //Preferred Retail Applicable - Start

                            coveredCost = new CoveredCost();
                            rowId = rowId + 1;
                            bool prefRetailPrevYear = IsPreferredRetailApplicable(false);
                            bool prefRetailNextYear = IsPreferredRetailApplicable(true);

                            coveredCost.RowIDProperty = rowId - 1;
                            coveredCost.CoveredCosts = RuleConstants.PreferredRetailApplicable;
                            coveredCost.NextYear = prefRetailNextYear ? "Yes" : "No";
                            coveredCost.ThisYear = prefRetailPrevYear ? "Yes" : "No";
                            coveredCost.DisplayinANOC = "true";
                            prescriptionInformationServices.Add(coveredCost);

                            //Preferred Retail Applicable - Start



                        }
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
            }
            return prescriptionInformationServices;
        }
        public List<ICLCompare> ExecuteInitialCoverageLimitInformationRulesOld()
        {
            List<ICLCompare> initialCoverageLimitInformationServices = new List<ICLCompare>();
            try
            {
                ICLCompare coveredCost = new ICLCompare();
                if (null != previousMedicareJsonData && null != nextMedicareJsonData)
                {
                    bool isPrevRxPlan = false; bool isNextRxPlan = false;
                    isPrevRxPlan = Convert.ToBoolean(Convert.ToString(previousMedicareJsonData.SelectToken(RuleConstants.PlanInformation + "." + RuleConstants.RxPlan) ?? String.Empty) == String.Empty ? "false" : Convert.ToString(previousMedicareJsonData.SelectToken(RuleConstants.PlanInformation + "." + RuleConstants.RxPlan)));
                    isNextRxPlan = Convert.ToBoolean(Convert.ToString(nextMedicareJsonData.SelectToken(RuleConstants.PlanInformation + "." + RuleConstants.RxPlan) ?? String.Empty) == String.Empty ? "false" : Convert.ToString(nextMedicareJsonData.SelectToken(RuleConstants.PlanInformation + "." + RuleConstants.RxPlan)));

                    List<JToken> previousICLTierList = previousMedicareJsonData.SelectToken(RuleConstants.StandardRetailCostSharingInformationGridFullpath) == null ? new List<JToken>() : previousMedicareJsonData.SelectToken(RuleConstants.StandardRetailCostSharingInformationGridFullpath).ToList();
                    List<JToken> nextICLTierList = nextMedicareJsonData.SelectToken(RuleConstants.StandardRetailCostSharingInformationGridFullpath) == null ? new List<JToken>() : nextMedicareJsonData.SelectToken(RuleConstants.StandardRetailCostSharingInformationGridFullpath).ToList();
                    if (null != previousICLTierList && null != nextICLTierList)
                    {
                        List<JToken> resultForTierUnion = new List<JToken>();
                        Dictionary<string, string> keys = new Dictionary<string, string>();
                        keys.Add(RuleConstants.Tiers, RuleConstants.Tiers);
                        keys.Add(RuleConstants.TierDescription, RuleConstants.TierDescription);
                        CollectionExecutionComparer operatorProcessor = new CollectionExecutionComparer(previousICLTierList, nextICLTierList, keys);
                        resultForTierUnion = operatorProcessor.Union();
                        bool hasBlankRow = false;
                        int rowId = 0;
                        foreach (var token in resultForTierUnion)
                        {
                            coveredCost = new ICLCompare();
                            hasBlankRow = HasBlankRow(token);
                            if (!hasBlankRow)
                            {
                                rowId = rowId + 1;
                                //coveredCost.RowID = rowId;
                                coveredCost.CoveredCosts = "Tier " + token[RuleConstants.Tiers];

                                string value = GetTokenValue(RuleConstants.Tiers, Convert.ToString(token[RuleConstants.Tiers]), RuleConstants.TierDescription, nextICLTierList);
                                if (isNextRxPlan)
                                {
                                    coveredCost.TierDescriptionNextYear = value;
                                    coveredCost.CostShareNextYear = GetAmount(GetTokenValue(RuleConstants.Tiers, Convert.ToString(token[RuleConstants.Tiers]), RuleConstants.Onemonthsupply, nextICLTierList));
                                }
                                else
                                {
                                    coveredCost.TierDescriptionNextYear = RuleConstants.NotCovered;
                                    coveredCost.CostShareNextYear = RuleConstants.NotCovered;
                                }
                                value = GetTokenValue(RuleConstants.Tiers, Convert.ToString(token[RuleConstants.Tiers]), RuleConstants.TierDescription, previousICLTierList);
                                if (isPrevRxPlan)
                                {
                                    coveredCost.TierDescriptionThisYear = value;
                                    coveredCost.CostShareThisYear = GetAmount(GetTokenValue(RuleConstants.Tiers, Convert.ToString(token[RuleConstants.Tiers]), RuleConstants.Onemonthsupply, previousICLTierList));
                                }
                                else
                                {
                                    coveredCost.TierDescriptionThisYear = RuleConstants.NotCovered;
                                    coveredCost.CostShareThisYear = RuleConstants.NotCovered;
                                }
                                coveredCost.DisplayinANOC = "Yes";
                                coveredCost.RowIDProperty = rowId - 1;
                                initialCoverageLimitInformationServices.Add(coveredCost);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
            }
            return initialCoverageLimitInformationServices;
        }
        public List<ICLCompare> ExecuteInitialCoverageLimitInformationRules()
        {
            bool isPrevRxPlan = false; bool isNextRxPlan = false; bool isMedicarePrevRxPlan = false; bool isMedicareNextRxPlan = false;
            List<ICLCompare> initialCoverageLimitInformationServices = new List<ICLCompare>();
            try
            {
                ICLCompare coveredCost = new ICLCompare();
                if (null != previousPBPViewJsonData && null != nextPBPViewJsonData && null != masterListAnocJsonData
                 && null != previousMedicareJsonData && null != nextMedicareJsonData)
                {
                    isNextRxPlan = CheckViewSectionRxPreICLVisibility(true);
                    isPrevRxPlan = CheckViewSectionRxPreICLVisibility(false);
                    isMedicareNextRxPlan = CheckMedicareSectionRxVisibility(true);
                    isMedicarePrevRxPlan = CheckMedicareSectionRxVisibility(false);

                    List<JToken> previousICLTierList = previousPBPViewJsonData.SelectToken("PreICL.PreICLGeneral") == null ? new List<JToken>() : previousPBPViewJsonData.SelectToken("PreICL.PreICLGeneral").ToList();
                    List<JToken> nextICLTierList = nextPBPViewJsonData.SelectToken("PreICL.PreICLGeneral") == null ? new List<JToken>() : nextPBPViewJsonData.SelectToken("PreICL.PreICLGeneral").ToList();
                    if (null != previousICLTierList && null != nextICLTierList)
                    {
                        List<JToken> ICLInfoML = masterListAnocJsonData.SelectToken("ANOCChartBenefit.InitialCoverageLimitInformation").ToList();
                        if (ICLInfoML != null && ICLInfoML.Count > 0)
                        {
                            JToken amount = null;
                            List<JToken> resultForTierUnion = new List<JToken>();
                            Dictionary<string, string> keys = new Dictionary<string, string>();
                            keys.Add("TierID", "TierID");
                            CollectionExecutionComparer operatorProcessor = new CollectionExecutionComparer(previousICLTierList, nextICLTierList, keys);
                            resultForTierUnion = operatorProcessor.Union();
                            bool hasBlankRow = false;
                            int rowId = 0;
                            if (resultForTierUnion.Count() > 0)
                            {
                                resultForTierUnion = resultForTierUnion.OrderBy(s => s.SelectToken("TierID")).ToList();
                            }
                            foreach (var token in resultForTierUnion)
                            {
                                coveredCost = new ICLCompare();
                                hasBlankRow = HasBlankRow(token);
                                if (!hasBlankRow)
                                {
                                    rowId = rowId + 1;
                                    //coveredCost.RowID = rowId;
                                    coveredCost.CoveredCosts = "Tier " + token["TierID"];
                                    string amountPath = String.Empty; string amountParent = String.Empty;
                                    var dt = ICLInfoML.Where(x => Convert.ToString(x.SelectToken("Name") ?? String.Empty) == Convert.ToString(token["TierID"])).FirstOrDefault();
                                    if (dt != null)
                                    {
                                        amountPath = Convert.ToString(dt["Amount"] ?? String.Empty);
                                        if (!String.IsNullOrEmpty(amountPath))
                                        {
                                            amountParent = amountPath.Split('[')[0];
                                            amountPath = amountPath.Split('[')[1].Split(']')[0];
                                        }
                                    }
                                    string value = GetTokenValue("TierID", Convert.ToString(token["TierID"]), "TierLabelDescription", nextICLTierList);
                                    if (isNextRxPlan)
                                        coveredCost.TierDescriptionNextYear = value;
                                    else
                                        coveredCost.TierDescriptionNextYear = RuleConstants.NotCovered;

                                    if (isMedicareNextRxPlan)
                                    {
                                        if (!String.IsNullOrEmpty(amountParent) && String.Equals(amountParent, "Medicare"))
                                            amount = nextMedicareJsonData.SelectToken(amountPath) ?? String.Empty;
                                        else if (!String.IsNullOrEmpty(amountParent) && String.Equals(amountParent, "PBPView"))
                                            amount = nextPBPViewJsonData.SelectToken(amountPath) ?? String.Empty;

                                        coveredCost.CostShareNextYear = Convert.ToString(amount) == String.Empty ? RuleConstants.NotApplicable : Convert.ToString(amount);
                                    }
                                    else
                                        coveredCost.CostShareNextYear = RuleConstants.NotCovered;

                                    value = GetTokenValue("TierID", Convert.ToString(token["TierID"]), "TierLabelDescription", previousICLTierList);
                                    if (isPrevRxPlan)
                                        coveredCost.TierDescriptionThisYear = value;
                                    else
                                        coveredCost.TierDescriptionThisYear = RuleConstants.NotCovered;

                                    if (isMedicarePrevRxPlan)
                                    {
                                        if (!String.IsNullOrEmpty(amountParent) && String.Equals(amountParent, "Medicare"))
                                            amount = previousMedicareJsonData.SelectToken(amountPath) ?? String.Empty;
                                        else if (!String.IsNullOrEmpty(amountParent) && String.Equals(amountParent, "PBPView"))
                                            amount = previousPBPViewJsonData.SelectToken(amountPath) ?? String.Empty;

                                        coveredCost.CostShareThisYear = Convert.ToString(amount) == String.Empty ? RuleConstants.NotApplicable : Convert.ToString(amount);
                                    }
                                    else
                                        coveredCost.CostShareThisYear = RuleConstants.NotCovered;

                                    coveredCost.DisplayinANOC = "Yes";
                                    coveredCost.RowIDProperty = rowId - 1;
                                    initialCoverageLimitInformationServices.Add(coveredCost);
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
            }
            return initialCoverageLimitInformationServices;
        }
        public List<ICLCompare> ExecuteGapCoverageLimitInformationRules()
        {
            bool isPrevRxPlan = false; bool isNextRxPlan = false; bool isMedicarePrevRxPlan = false; bool isMedicareNextRxPlan = false;
            List<ICLCompare> gapCoverageInformationServices = new List<ICLCompare>();
            try
            {
                ICLCompare coveredCost = new ICLCompare();
                if (null != previousPBPViewJsonData && null != nextPBPViewJsonData && null != masterListAnocJsonData
                 && null != previousMedicareJsonData && null != nextMedicareJsonData)
                {
                    isNextRxPlan = CheckViewSectionRxGapCoverageVisibility(true);
                    isPrevRxPlan = CheckViewSectionRxGapCoverageVisibility(false);
                    isMedicareNextRxPlan = CheckMedicareSectionRxVisibility(true);
                    isMedicarePrevRxPlan = CheckMedicareSectionRxVisibility(false);

                    List<JToken> previousGapTierList = previousPBPViewJsonData.SelectToken("GapCoverage.GapCoverageGeneral") == null ? new List<JToken>() : previousPBPViewJsonData.SelectToken("GapCoverage.GapCoverageGeneral").ToList();
                    List<JToken> nextGapTierList = nextPBPViewJsonData.SelectToken("GapCoverage.GapCoverageGeneral") == null ? new List<JToken>() : nextPBPViewJsonData.SelectToken("GapCoverage.GapCoverageGeneral").ToList();
                    if (null != previousGapTierList && null != nextGapTierList)
                    {
                        List<JToken> gapInfoML = masterListAnocJsonData.SelectToken("ANOCChartBenefit.GapCoverageInformation").ToList();
                        if (gapInfoML != null && gapInfoML.Count > 0)
                        {
                            JToken amount = null;
                            List<JToken> resultForTierUnion = new List<JToken>();
                            Dictionary<string, string> keys = new Dictionary<string, string>();
                            keys.Add("TierID", "TierID");
                            CollectionExecutionComparer operatorProcessor = new CollectionExecutionComparer(previousGapTierList, nextGapTierList, keys);
                            resultForTierUnion = operatorProcessor.Union();
                            bool hasBlankRow = false;
                            int rowId = 0;
                            foreach (var token in resultForTierUnion)
                            {
                                coveredCost = new ICLCompare();
                                hasBlankRow = HasBlankRow(token);
                                if (!hasBlankRow)
                                {
                                    rowId = rowId + 1;
                                    //coveredCost.RowID = rowId;
                                    coveredCost.CoveredCosts = "Tier " + token["TierID"];
                                    string amountPath = String.Empty; string amountParent = String.Empty;

                                    var dt = gapInfoML.Where(x => Convert.ToString(x.SelectToken("Name") ?? String.Empty) == Convert.ToString(token["TierID"])).FirstOrDefault();
                                    if (dt != null)
                                    {
                                        amountPath = Convert.ToString(dt["Path"] ?? String.Empty);
                                        if (!String.IsNullOrEmpty(amountPath))
                                        {
                                            amountParent = amountPath.Split('[')[0];
                                            amountPath = amountPath.Split('[')[1].Split(']')[0];
                                        }
                                    }
                                    string value = GetTokenValue("TierID", Convert.ToString(token["TierID"]), "TierLabelDescription", nextGapTierList);
                                    if (isNextRxPlan)
                                        coveredCost.TierDescriptionNextYear = value;
                                    else
                                        coveredCost.TierDescriptionNextYear = RuleConstants.NotCovered;

                                    if (isMedicareNextRxPlan)
                                    {
                                        if (!String.IsNullOrEmpty(amountParent) && String.Equals(amountParent, "Medicare"))
                                            amount = nextMedicareJsonData.SelectToken(amountPath) ?? String.Empty;
                                        else if (!String.IsNullOrEmpty(amountParent) && String.Equals(amountParent, "PBPView"))
                                            amount = nextPBPViewJsonData.SelectToken(amountPath) ?? String.Empty;

                                        coveredCost.CostShareNextYear = Convert.ToString(amount) == String.Empty ? RuleConstants.NotApplicable : Convert.ToString(amount);
                                    }
                                    else
                                        coveredCost.CostShareNextYear = RuleConstants.NotCovered;

                                    value = GetTokenValue("TierID", Convert.ToString(token["TierID"]), "TierLabelDescription", previousGapTierList);
                                    if (isPrevRxPlan)
                                        coveredCost.TierDescriptionThisYear = value;
                                    else
                                        coveredCost.TierDescriptionThisYear = RuleConstants.NotCovered;

                                    if (isMedicarePrevRxPlan)
                                    {
                                        if (!String.IsNullOrEmpty(amountParent) && String.Equals(amountParent, "Medicare"))
                                            amount = previousMedicareJsonData.SelectToken(amountPath) ?? String.Empty;
                                        else if (!String.IsNullOrEmpty(amountParent) && String.Equals(amountParent, "PBPView"))
                                            amount = previousPBPViewJsonData.SelectToken(amountPath) ?? String.Empty;

                                        coveredCost.CostShareThisYear = Convert.ToString(amount) == String.Empty ? RuleConstants.NotApplicable : Convert.ToString(amount);
                                    }
                                    else
                                        coveredCost.CostShareThisYear = RuleConstants.NotCovered;

                                    coveredCost.DisplayinANOC = "Yes";
                                    coveredCost.RowIDProperty = rowId - 1;
                                    gapCoverageInformationServices.Add(coveredCost);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
            }
            return gapCoverageInformationServices;
        }

        //Preferred Retail - Start
        public List<ICLCompare> ExecutePreferredRetailInformation()
        {
            bool isPrevRxPlan = false; bool isNextRxPlan = false; bool isMedicarePrevRxPlan = false; bool isMedicareNextRxPlan = false;
            bool isPrefRetailForPrevYear = false;
            bool isPrefRetailForNextYear = false;
            List<ICLCompare> pharmacyRetail = new List<ICLCompare>();
            try
            {
                ICLCompare coveredCost = new ICLCompare();
                if (null != previousPBPViewJsonData && null != nextPBPViewJsonData && null != masterListAnocJsonData
                 && null != previousMedicareJsonData && null != nextMedicareJsonData)
                {
                    isNextRxPlan = CheckViewSectionRxPreICLVisibility(true);
                    isPrevRxPlan = CheckViewSectionRxPreICLVisibility(false);
                    isMedicareNextRxPlan = CheckMedicareSectionRxVisibility(true);
                    isMedicarePrevRxPlan = CheckMedicareSectionRxVisibility(false);
                    isPrefRetailForNextYear = IsPreferredRetailApplicable(true);
                    isPrefRetailForPrevYear = IsPreferredRetailApplicable(false);

                    List<JToken> previousICLTierList = previousPBPViewJsonData.SelectToken("PreICL.PreICLGeneral") == null ? new List<JToken>() : previousPBPViewJsonData.SelectToken("PreICL.PreICLGeneral").ToList();
                    List<JToken> nextICLTierList = nextPBPViewJsonData.SelectToken("PreICL.PreICLGeneral") == null ? new List<JToken>() : nextPBPViewJsonData.SelectToken("PreICL.PreICLGeneral").ToList();
                    if (null != previousICLTierList && null != nextICLTierList)
                    {
                        List<JToken> ICLInfoML = masterListAnocJsonData.SelectToken("ANOCChartBenefit.InitialCoverageLimitInformation").ToList();
                        if (ICLInfoML != null && ICLInfoML.Count > 0)
                        {
                            JToken amount = null;
                            List<JToken> resultForTierUnion = new List<JToken>();
                            Dictionary<string, string> keys = new Dictionary<string, string>();
                            keys.Add("TierID", "TierID");
                            CollectionExecutionComparer operatorProcessor = new CollectionExecutionComparer(previousICLTierList, nextICLTierList, keys);
                            resultForTierUnion = operatorProcessor.Union();
                            bool hasBlankRow = false;
                            int rowId = 0;
                            foreach (var token in resultForTierUnion)
                            {
                                coveredCost = new ICLCompare();
                                hasBlankRow = HasBlankRow(token);
                                if (!hasBlankRow)
                                {
                                    rowId = rowId + 1;
                                    //coveredCost.RowID = rowId;
                                    coveredCost.CoveredCosts = "Tier " + token["TierID"];
                                    string amountPath = String.Empty; string amountParent = String.Empty;
                                    var dt = ICLInfoML.Where(x => Convert.ToString(x.SelectToken("Name") ?? String.Empty) == Convert.ToString(token["TierID"])).FirstOrDefault();
                                    if (dt != null)
                                    {
                                        amountPath = Convert.ToString(dt["PreferredRetailAmount"] ?? String.Empty);
                                        if (!String.IsNullOrEmpty(amountPath))
                                        {
                                            amountParent = amountPath.Split('[')[0];
                                            amountPath = amountPath.Split('[')[1].Split(']')[0];
                                        }
                                    }
                                    string value = GetTokenValue("TierID", Convert.ToString(token["TierID"]), "TierLabelDescription", nextICLTierList);
                                    if (isNextRxPlan)
                                        coveredCost.TierDescriptionNextYear = value;
                                    else
                                        coveredCost.TierDescriptionNextYear = RuleConstants.NotCovered;

                                    if (isMedicareNextRxPlan && isPrefRetailForNextYear)
                                    {
                                        if (!String.IsNullOrEmpty(amountParent) && String.Equals(amountParent, "Medicare"))
                                            amount = nextMedicareJsonData.SelectToken(amountPath) ?? String.Empty;
                                        else if (!String.IsNullOrEmpty(amountParent) && String.Equals(amountParent, "PBPView"))
                                            amount = nextPBPViewJsonData.SelectToken(amountPath) ?? String.Empty;

                                        coveredCost.CostShareNextYear = Convert.ToString(amount) == String.Empty ? RuleConstants.NotApplicable : Convert.ToString(amount);
                                    }
                                    else
                                        coveredCost.CostShareNextYear = RuleConstants.NotCovered;

                                    value = GetTokenValue("TierID", Convert.ToString(token["TierID"]), "TierLabelDescription", previousICLTierList);
                                    if (isPrevRxPlan)
                                        coveredCost.TierDescriptionThisYear = value;
                                    else
                                        coveredCost.TierDescriptionThisYear = RuleConstants.NotCovered;

                                    if (isMedicarePrevRxPlan && isPrefRetailForPrevYear)
                                    {
                                        if (!String.IsNullOrEmpty(amountParent) && String.Equals(amountParent, "Medicare"))
                                            amount = previousMedicareJsonData.SelectToken(amountPath) ?? String.Empty;
                                        else if (!String.IsNullOrEmpty(amountParent) && String.Equals(amountParent, "PBPView"))
                                            amount = previousPBPViewJsonData.SelectToken(amountPath) ?? String.Empty;

                                        coveredCost.CostShareThisYear = Convert.ToString(amount) == String.Empty ? RuleConstants.NotApplicable : Convert.ToString(amount);
                                    }
                                    else
                                        coveredCost.CostShareThisYear = RuleConstants.NotCovered;

                                    coveredCost.DisplayinANOC = "Yes";
                                    coveredCost.RowIDProperty = rowId - 1;
                                    pharmacyRetail.Add(coveredCost);
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
            }
            return pharmacyRetail;

        }

        //Preferred Retail -  End

        public string GetAdditionalANOCChartServices(string repeaterData)
        {
            List<JToken> resultForTierUnion = new List<JToken>();
            List<JToken> resultForTierExcept = new List<JToken>();
            List<JToken> resultForTierIntersect = new List<JToken>();
            BenefitsCompare benefitsCompare = new BenefitsCompare();
            try
            {
                List<JToken> existingServices = new List<JToken>();
                if (!String.IsNullOrEmpty(repeaterData))
                {
                    existingServices = JToken.Parse(repeaterData).ToList();
                }
                List<JToken> previousYearBRGList = GetBRGServices(false, true, existingServices);
                List<JToken> nextYearBRGList = GetBRGServices(true, true, existingServices);

                Dictionary<string, string> keys = new Dictionary<string, string>();
                keys.Add(RuleConstants.BenefitCategory1, RuleConstants.BenefitCategory1);
                keys.Add(RuleConstants.BenefitCategory2, RuleConstants.BenefitCategory2);
                keys.Add(RuleConstants.BenefitCategory3, RuleConstants.BenefitCategory3);
                //keys.Add(RuleConstants.CostShareTiers, RuleConstants.CostShareTiers);

                CollectionExecutionComparer operatorProcessor = new CollectionExecutionComparer(previousYearBRGList, nextYearBRGList, keys);
                resultForTierUnion = operatorProcessor.Union();

                if (null != repeaterData)
                {
                    List<JToken> comparedBenefitServicesData = JToken.Parse(repeaterData).ToList();
                    if (null != resultForTierUnion && null != comparedBenefitServicesData)
                    {
                        Dictionary<string, string> exceptKeys = new Dictionary<string, string>();
                        exceptKeys.Add(RuleConstants.BenefitCategory1, RuleConstants.BenefitCategory1);
                        exceptKeys.Add(RuleConstants.BenefitCategory2, RuleConstants.BenefitCategory2);
                        exceptKeys.Add(RuleConstants.BenefitCategory3, RuleConstants.BenefitCategory3);
                        //exceptKeys.Add(RuleConstants.CostShareTiers, "CostShareTiers:Tier");

                        operatorProcessor = new CollectionExecutionComparer(resultForTierUnion, comparedBenefitServicesData, exceptKeys);
                        resultForTierExcept = operatorProcessor.Except();
                        if (null != resultForTierExcept)
                        {
                            resultForTierIntersect = GetAdditionalSlidingCostShareServices(resultForTierExcept, comparedBenefitServicesData);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
            }
            return JsonConvert.SerializeObject(resultForTierIntersect);
        }
        public string GetAdditionalServicesData(string repeaterData, string repeaterSelectedData)
        {
            List<BenefitsCompare> benefitsCompareServices = new List<BenefitsCompare>();
            List<BenefitsCompare> additionalBenefitServices = new List<BenefitsCompare>();
            List<BenefitsCompare> repeaterSelectedServices = new List<BenefitsCompare>();
            List<BenefitsCompare> mergedServices = new List<BenefitsCompare>();

            BenefitsCompare benefitsCompare = new BenefitsCompare();
            try
            {
                List<JToken> existingServices = new List<JToken>();
                if (!String.IsNullOrEmpty(repeaterData))
                {
                    existingServices = JToken.Parse(repeaterData).ToList();
                }
                List<JToken> previousYearBRGList = GetBRGServices(false, true, existingServices);
                List<JToken> nextYearBRGList = GetBRGServices(true, true, existingServices);

                if (AnocHelper.IsPPOPlan(false) && AnocHelper.IsContainOONNetworkTier(false))
                {
                    ProcessOONService(previousYearBRGList, false);
                }
                else if (AnocHelper.IsPOSPlan(false))
                {
                    ProcessOONService(previousYearBRGList, false);
                }

                if (AnocHelper.IsPPOPlan(true) && AnocHelper.IsContainOONNetworkTier(true))
                {
                    ProcessOONService(nextYearBRGList, true);
                }
                else if (AnocHelper.IsPOSPlan(true))
                {
                    ProcessOONService(nextYearBRGList, true);
                }

                //if (AnocHelper.IsContainOONNetworkTier(false))
                //{
                //    ProcessOONService(previousYearBRGList, false);
                //}
                //
                //if (AnocHelper.IsContainOONNetworkTier(true))
                //{
                //    ProcessOONService(nextYearBRGList, true);
                //}


                List<JToken> resultForTierUnion = new List<JToken>();
                List<JToken> resultForTierExcept = new List<JToken>();
                List<JToken> resultForProcessing = new List<JToken>();

                //List<JToken> previousYearSlidingList = GetSlidingCostShareServices(false,true, existingServices);
                //List<JToken> nextYearSlidingList = GetSlidingCostShareServices(true,true, existingServices);

                Dictionary<string, string> keys = new Dictionary<string, string>();
                keys.Add(RuleConstants.BenefitCategory1, RuleConstants.BenefitCategory1);
                keys.Add(RuleConstants.BenefitCategory2, RuleConstants.BenefitCategory2);
                keys.Add(RuleConstants.BenefitCategory3, RuleConstants.BenefitCategory3);
                keys.Add(RuleConstants.CostShareTiers, RuleConstants.CostShareTiers);

                CollectionExecutionComparer operatorProcessor = new CollectionExecutionComparer(previousYearBRGList, nextYearBRGList, keys);
                resultForTierUnion = operatorProcessor.Union();
                resultForProcessing = resultForTierUnion;

                if (null != repeaterData && null != repeaterSelectedData)
                {
                    List<JToken> comparedBenefitServicesData = JToken.Parse(repeaterData).ToList();
                    additionalBenefitServices = JsonConvert.DeserializeObject<List<BenefitsCompare>>(repeaterData);
                    repeaterSelectedServices = JsonConvert.DeserializeObject<List<BenefitsCompare>>(repeaterSelectedData);
                    if (null != resultForTierUnion && null != comparedBenefitServicesData)
                    {
                        Dictionary<string, string> exceptKeys = new Dictionary<string, string>();
                        exceptKeys.Add(RuleConstants.BenefitCategory1, RuleConstants.BenefitCategory1);
                        exceptKeys.Add(RuleConstants.BenefitCategory2, RuleConstants.BenefitCategory2);
                        exceptKeys.Add(RuleConstants.BenefitCategory3, RuleConstants.BenefitCategory3);
                        //exceptKeys.Add(RuleConstants.CostShareTiers, "CostShareTiers:Tier");

                        operatorProcessor = new CollectionExecutionComparer(resultForTierUnion, comparedBenefitServicesData, exceptKeys);
                        resultForTierExcept = operatorProcessor.Except();
                        if (null != resultForTierExcept)
                        {
                            bool hasBlankRow = false;
                            int rowID = 0;
                            foreach (var entry in repeaterSelectedServices)
                            {
                                var service = resultForTierExcept.Where(x => Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty) == entry.BenefitCategory1
                                                      && Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty) == entry.BenefitCategory2
                                                      && Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty) == entry.BenefitCategory3
                                                      //&& Convert.ToString(x.SelectToken(RuleConstants.CostShareTiers) ?? String.Empty) == entry.CostShareTiers
                                                      ).FirstOrDefault();

                                if (null != service)
                                {
                                    var services = resultForProcessing.Where(x => Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty) == entry.BenefitCategory1
                                                              && Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty) == entry.BenefitCategory2
                                                              && Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty) == entry.BenefitCategory3
                                                              //&& Convert.ToString(x.SelectToken(RuleConstants.CostShareTiers) ?? String.Empty) == entry.CostShareTiers
                                                              ).ToList();

                                    foreach (var token in services)
                                    {
                                        rowID = rowID + 1;
                                        benefitsCompare = new BenefitsCompare();
                                        hasBlankRow = HasBlankRow(token);
                                        if (!hasBlankRow)
                                        {
                                            benefitsCompare.ANOCBenefitName = String.Empty;
                                            benefitsCompare.BenefitCategory1 = Convert.ToString(token[RuleConstants.BenefitCategory1] ?? String.Empty);
                                            benefitsCompare.BenefitCategory2 = Convert.ToString(token[RuleConstants.BenefitCategory2] ?? String.Empty);
                                            benefitsCompare.BenefitCategory3 = Convert.ToString(token[RuleConstants.BenefitCategory3] ?? String.Empty);
                                            benefitsCompare.CostShareTiers = Convert.ToString(token[RuleConstants.CostShareTiers] ?? String.Empty);
                                            benefitsCompare.NextYear = GetAdditionalCostShareValues(benefitsCompare.BenefitCategory1, benefitsCompare.BenefitCategory2, benefitsCompare.BenefitCategory3, benefitsCompare.CostShareTiers, previousYearBRGList, nextYearBRGList, token, true, false);
                                            benefitsCompare.ThisYear = GetAdditionalCostShareValues(benefitsCompare.BenefitCategory1, benefitsCompare.BenefitCategory2, benefitsCompare.BenefitCategory3, benefitsCompare.CostShareTiers, previousYearBRGList, nextYearBRGList, token, false, false);
                                            benefitsCompare.DisplayinANOC = "false";
                                            benefitsCompareServices.Add(benefitsCompare);
                                        }
                                    }
                                }
                            }
                            // Aditional sliding costshare services
                            benefitsCompareServices = GetAdditionalSlidingCostShareServicesData(benefitsCompareServices, comparedBenefitServicesData, repeaterSelectedServices);

                            if (null != benefitsCompareServices)
                            {
                                // Merge All tiers for same services into single service with This And Next Year
                                if (benefitsCompareServices != null && benefitsCompareServices.Count > 0)
                                {
                                    foreach (var ser in benefitsCompareServices)
                                    {
                                        string pTagPrefix = "<p style=\"font-size:   12pt; font-family: 'Times New Roman', serif; margin-top: 6pt;\">";
                                        string pTagPostFix = "</p>";
                                        bool exist = mergedServices.Exists(x => x.BenefitCategory1 == ser.BenefitCategory1
                                                                && (x.BenefitCategory2 == ser.BenefitCategory2)
                                                                && (x.BenefitCategory3 == ser.BenefitCategory3));

                                        if (exist)
                                        {
                                            var service = mergedServices.Where(x => x.BenefitCategory1 == ser.BenefitCategory1
                                                                && (x.BenefitCategory2 == ser.BenefitCategory2)
                                                                && (x.BenefitCategory3 == ser.BenefitCategory3)).FirstOrDefault();

                                            if (service != null)
                                            {
                                                if (!String.IsNullOrEmpty(ser.NextYear) && !String.IsNullOrEmpty(ser.NextYear.Replace("<br/>", "").Trim()))
                                                {
                                                    service.NextYear += pTagPrefix + "<b>" + TierFormatter(ser.CostShareTiers) + "Network" + "</b>" + ser.NextYear + pTagPostFix;
                                                }
                                                if (!String.IsNullOrEmpty(ser.ThisYear) && !String.IsNullOrEmpty(ser.ThisYear.Replace("<br/>", "").Trim()))
                                                {
                                                    service.ThisYear += pTagPrefix + "<b>" + TierFormatter(ser.CostShareTiers) + "Network" + "</b>" + ser.ThisYear + pTagPostFix;
                                                }
                                                //service.CostShareTiers += "<br/>" + ser.CostShareTiers;
                                                service.CostShareTiers += "," + ser.CostShareTiers;
                                            }
                                        }
                                        else
                                        {
                                            
                                            string prefix = pTagPrefix + "<b>" + TierFormatter(ser.CostShareTiers) + "Network" + "</b>";
                                            if (!String.IsNullOrEmpty(ser.NextYear) && !String.IsNullOrEmpty(ser.NextYear.Replace("<br/>", "").Trim()))
                                            {
                                                string nextYr = prefix + ser.NextYear + pTagPostFix;
                                                ser.NextYear = nextYr;
                                            }
                                            if (!String.IsNullOrEmpty(ser.ThisYear) && !String.IsNullOrEmpty(ser.ThisYear.Replace("<br/>", "").Trim()))
                                            {
                                                string thisYr = prefix + ser.ThisYear + pTagPostFix;
                                                ser.ThisYear = thisYr;
                                            }
                                            //string prefix = "<b>" + ser.CostShareTiers + "Network" + "</b>";
                                            //string nextYr = prefix + ser.NextYear;
                                            //string thisYr = prefix + ser.ThisYear;
                                            mergedServices.Add(ser);
                                        }
                                    }
                                    // Set RowIDProperty for each Row
                                    benefitsCompareServices = AddRowIDProperty(mergedServices);
                                }
                                foreach (var item in benefitsCompareServices)
                                {
                                    var isSelected = repeaterSelectedServices.Exists(x => x.BenefitCategory1 == item.BenefitCategory1
                                    && x.BenefitCategory2 == item.BenefitCategory2 && x.BenefitCategory3 == item.BenefitCategory3
                                    //&& x.CostShareTiers == item.CostShareTiers
                                    );
                                    if (isSelected)
                                    {
                                        additionalBenefitServices.Add(item);
                                    }
                                }
                            }
                            // Set RowIDProperty for each Row
                            additionalBenefitServices = AddRowIDProperty(additionalBenefitServices);


                        }
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
            }
            return JsonConvert.SerializeObject(additionalBenefitServices);
        }
        private bool HasBlankRow(JToken serviceToken)
        {
            bool hasBlankRow = false;
            try
            {
                if (null != serviceToken)
                {
                    var serviceTokenList = serviceToken.ToList();
                    int tokenCount = serviceTokenList.Count();
                    int cnt = 0;
                    foreach (JToken token in serviceTokenList)
                    {
                        JToken name = ((JProperty)token).Name;
                        if (Convert.ToString(name) != RuleConstants.RowIDProperty)
                        {
                            JToken value = ((JProperty)token).Value;
                            if (String.IsNullOrEmpty(Convert.ToString(value)))
                                cnt++;
                        }
                        else
                        {
                            tokenCount = tokenCount - 1;
                        }
                    }
                    if (tokenCount == cnt)
                    {
                        hasBlankRow = true;
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
            }

            return hasBlankRow;
        }
        private string GetTokenValue(string searchToken, string searchValue, string findValue, List<JToken> sourceTokenList)
        {
            string value = String.Empty;
            try
            {
                var token = sourceTokenList.Find(x => (x.SelectToken(searchToken) == null ? String.Empty : x.SelectToken(searchToken).ToString()) == searchValue);
                if (null != token)
                {
                    value = Convert.ToString(token[findValue]);
                    if (String.IsNullOrEmpty(value))
                    {
                        value = RuleConstants.NotApplicable;
                    }
                }
                else
                {
                    value = RuleConstants.NotCovered;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
            }

            return value;
        }
        private string GetTokenValue(string firstSearchToken, string firsteSearchValue, string secondSearchToken, string secondSearchValue, string findValue, List<JToken> sourceTokenList)
        {
            string value = String.Empty;
            try
            {
                var token = sourceTokenList.Find(x => (x.SelectToken(firstSearchToken) == null ? String.Empty : x.SelectToken(firstSearchToken).ToString()) == firsteSearchValue
                                              && (x.SelectToken(secondSearchToken) == null ? String.Empty : x.SelectToken(secondSearchToken).ToString()) == secondSearchValue);
                if (null != token)
                {
                    value = Convert.ToString(token[findValue]);
                    if (String.IsNullOrEmpty(value))
                    {
                        value = RuleConstants.NotApplicable;
                    }
                }
                else
                {
                    value = RuleConstants.NotCovered;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
            }
            return value;
        }
        private string GetCostShareValuesOld(string benefitCategory1, string benefitCategory2, string benefitCategory3, List<JToken> sourceTokenList)
        {
            bool isExist = false;
            string value = String.Empty;
            try
            {
                if (null != sourceTokenList && sourceTokenList.Count > 0)
                {
                    isExist = sourceTokenList.Exists(x => Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty) == benefitCategory1
                                                       && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty) == benefitCategory2)
                                                       && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty) == benefitCategory3));
                    if (isExist)
                    {
                        value = "CostShare values";
                    }
                    else
                    {
                        value = RuleConstants.NotCovered;
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
            }
            return value;
        }
        private string GetCostShareValues(string benefitCategory1, string benefitCategory2, string benefitCategory3, string tier, List<JToken> previousYearTokenList, List<JToken> nextYearTokenList, JToken networkTier, bool isNextYear, bool isDefaultService)
        {
            bool previousYearExist = false;
            bool nextYearExist = false;
            string value = String.Empty;
            List<Variance> detailedCompareList = new List<Variance>();
            try
            {
                if (null != previousYearTokenList && null != nextYearTokenList)
                {
                    #region Previous Year
                    if (!isNextYear)
                    {
                        previousYearExist = previousYearTokenList.Exists(x => Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty) == benefitCategory1
                                                       && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty) == benefitCategory2)
                                                       && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty) == benefitCategory3)
                                                       && (Convert.ToString(x.SelectToken(RuleConstants.CostShareTiers) ?? String.Empty) == tier));

                        if (previousYearExist)
                        {
                            nextYearExist = nextYearTokenList.Exists(x => Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty) == benefitCategory1
                                                      && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty) == benefitCategory2)
                                                      && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty) == benefitCategory3)
                                                      && (Convert.ToString(x.SelectToken(RuleConstants.CostShareTiers) ?? String.Empty) == tier));

                            if (nextYearExist)
                            {
                                var previousYrService = previousYearTokenList.Where(x => Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty) == benefitCategory1
                                                       && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty) == benefitCategory2)
                                                       && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty) == benefitCategory3)
                                                       && (Convert.ToString(x.SelectToken(RuleConstants.CostShareTiers) ?? String.Empty) == tier)).FirstOrDefault();

                                var nextYrService = nextYearTokenList.Where(x => Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty) == benefitCategory1
                                                     && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty) == benefitCategory2)
                                                     && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty) == benefitCategory3)
                                                     && (Convert.ToString(x.SelectToken(RuleConstants.CostShareTiers) ?? String.Empty) == tier)).FirstOrDefault();

                                detailedCompareList = GetNetworkServiceCostShare(networkTier, previousYrService, nextYrService, isDefaultService);
                                if (null != detailedCompareList)
                                {
                                    int count = detailedCompareList.Count;
                                    string val = string.Empty;
                                    for (int a = 0; a < count; a++)
                                    {
                                        if (isNextYear)
                                        {
                                            if (a == (count - 1))
                                            {
                                                val = detailedCompareList[a].nextYrVal;
                                                if (String.IsNullOrEmpty(val) && detailedCompareList[a].Prop != "PlanMaxAmountPeriodicity" && tier != RuleConstants.OON)
                                                    val = RuleConstants.NotApplicable;
                                                value += val + "-" + GetFormatedTokenName(detailedCompareList[a].Prop);
                                            }
                                            else
                                            {
                                                val = detailedCompareList[a].nextYrVal;
                                                if (String.IsNullOrEmpty(val) && detailedCompareList[a].Prop != "PlanMaxAmountPeriodicity" && tier != RuleConstants.OON)
                                                    val = RuleConstants.NotApplicable;
                                                value += val + "-" + GetFormatedTokenName(detailedCompareList[a].Prop) + ", ";
                                            }
                                        }
                                        else
                                        {
                                            if (a == (count - 1))
                                            {
                                                val = detailedCompareList[a].previousYrVal;
                                                if (String.IsNullOrEmpty(val) && detailedCompareList[a].Prop != "PlanMaxAmountPeriodicity" && tier != RuleConstants.OON)
                                                    val = RuleConstants.NotApplicable;
                                                value += val + "-" + GetFormatedTokenName(detailedCompareList[a].Prop);
                                            }
                                            else
                                            {
                                                val = detailedCompareList[a].previousYrVal;
                                                if (String.IsNullOrEmpty(val) && detailedCompareList[a].Prop != "PlanMaxAmountPeriodicity" && tier != RuleConstants.OON)
                                                    val = RuleConstants.NotApplicable;
                                                value += val + "-" + GetFormatedTokenName(detailedCompareList[a].Prop) + ", ";
                                            }
                                        }
                                    }
                                    // Check for Min / Max Copay - Coinsurance
                                    value = CheckDuplicateCostShare(value);
                                    // Get formated string for values
                                    value = GetBenefitServiceFormatedValue(value);
                                }
                            }
                            else
                            {
                                var service = previousYearTokenList.Where(x => Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty) == benefitCategory1
                                                        && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty) == benefitCategory2)
                                                        && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty) == benefitCategory3)
                                                        && (Convert.ToString(x.SelectToken(RuleConstants.CostShareTiers) ?? String.Empty) == tier)).FirstOrDefault();
                                value = GetNotCoveredServiceCostShare(networkTier, service);
                                // Check for Min / Max Copay.Coinsurance
                                value = CheckDuplicateCostShare(value);
                                // Get formated string for values
                                value = GetBenefitServiceFormatedValue(value);
                            }
                        }
                        else
                        {
                            value = RuleConstants.NotCovered;
                        }
                    }
                    #endregion Previous Year

                    #region Next Year
                    else
                    {
                        nextYearExist = nextYearTokenList.Exists(x => Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty) == benefitCategory1
                                                      && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty) == benefitCategory2)
                                                      && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty) == benefitCategory3)
                                                      && (Convert.ToString(x.SelectToken(RuleConstants.CostShareTiers) ?? String.Empty) == tier));

                        if (nextYearExist)
                        {
                            previousYearExist = previousYearTokenList.Exists(x => Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty) == benefitCategory1
                                                      && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty) == benefitCategory2)
                                                      && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty) == benefitCategory3)
                                                      && (Convert.ToString(x.SelectToken(RuleConstants.CostShareTiers) ?? String.Empty) == tier));

                            if (previousYearExist)
                            {
                                var previousYrService = previousYearTokenList.Where(x => Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty) == benefitCategory1
                                                  && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty) == benefitCategory2)
                                                  && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty) == benefitCategory3)
                                                  && (Convert.ToString(x.SelectToken(RuleConstants.CostShareTiers) ?? String.Empty) == tier)).FirstOrDefault();

                                var nextYrService = nextYearTokenList.Where(x => Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty) == benefitCategory1
                                                     && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty) == benefitCategory2)
                                                     && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty) == benefitCategory3)
                                                     && (Convert.ToString(x.SelectToken(RuleConstants.CostShareTiers) ?? String.Empty) == tier)).FirstOrDefault();

                                detailedCompareList = GetNetworkServiceCostShare(networkTier, previousYrService, nextYrService, isDefaultService);
                                if (null != detailedCompareList)
                                {
                                    int count = detailedCompareList.Count;
                                    string val = string.Empty;
                                    for (int a = 0; a < count; a++)
                                    {
                                        if (isNextYear)
                                        {
                                            if (a == (count - 1))
                                            {
                                                val = detailedCompareList[a].nextYrVal;
                                                if (String.IsNullOrEmpty(val) && detailedCompareList[a].Prop != "PlanMaxAmountPeriodicity" && tier != RuleConstants.OON)
                                                    val = RuleConstants.NotApplicable;
                                                value += val + "-" + GetFormatedTokenName(detailedCompareList[a].Prop);
                                            }
                                            else
                                            {
                                                val = detailedCompareList[a].nextYrVal;
                                                if (String.IsNullOrEmpty(val) && detailedCompareList[a].Prop != "PlanMaxAmountPeriodicity" && tier != RuleConstants.OON)
                                                    val = RuleConstants.NotApplicable;
                                                value += val + "-" + GetFormatedTokenName(detailedCompareList[a].Prop) + ", ";
                                            }
                                        }
                                        else
                                        {
                                            if (a == (count - 1))
                                            {
                                                val = detailedCompareList[a].previousYrVal;
                                                if (String.IsNullOrEmpty(val) && detailedCompareList[a].Prop != "PlanMaxAmountPeriodicity" && tier != RuleConstants.OON)
                                                    val = RuleConstants.NotApplicable;
                                                value += val + "-" + GetFormatedTokenName(detailedCompareList[a].Prop);
                                            }
                                            else
                                            {
                                                val = detailedCompareList[a].previousYrVal;
                                                if (String.IsNullOrEmpty(val) && detailedCompareList[a].Prop != "PlanMaxAmountPeriodicity" && tier != RuleConstants.OON)
                                                    val = RuleConstants.NotApplicable;
                                                value += val + "-" + GetFormatedTokenName(detailedCompareList[a].Prop) + ", ";
                                            }
                                        }
                                    }
                                    // Check for Min / Max Copay.Coinsurance
                                    value = CheckDuplicateCostShare(value);
                                    // Get formated string for values
                                    value = GetBenefitServiceFormatedValue(value);
                                }
                            }
                            else
                            {
                                var service = nextYearTokenList.Where(x => Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty) == benefitCategory1
                                                       && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty) == benefitCategory2)
                                                       && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty) == benefitCategory3)
                                                       && (Convert.ToString(x.SelectToken(RuleConstants.CostShareTiers) ?? String.Empty) == tier)).FirstOrDefault();

                                value = GetNotCoveredServiceCostShare(networkTier, service);
                                // Check for Min / Max Copay.Coinsurance
                                value = CheckDuplicateCostShare(value);
                                // Get formated string for values
                                value = GetBenefitServiceFormatedValue(value);
                            }
                        }
                        else
                        {
                            value = RuleConstants.NotCovered;
                        }
                    }
                    #endregion Next Year
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
            }
            return value;
        }
        private string GetNotCoveredServiceCostShare(JToken networkTier, JToken sourceService)
        {
            string value = String.Empty;
            try
            {
                if (null != networkTier && null != sourceService)
                {
                    string tierName = Convert.ToString(networkTier[RuleConstants.CostShareTiers] ?? String.Empty);
                    if (!String.IsNullOrEmpty(tierName))
                    {
                        var tierValueToken = sourceService;
                        if (null != tierValueToken)
                        {
                            List<JToken> tierTokens = tierValueToken.ToList();
                            if (null != tierTokens)
                            {
                                int tierTokensCount = tierTokens.Count;
                                string tokenName = string.Empty;
                                string tokenValue = string.Empty;
                                bool isNumeric = false;
                                string symbol = string.Empty;
                                string val = string.Empty;
                                for (int i = 0; i < tierTokensCount; i++)
                                {
                                    symbol = string.Empty;
                                    tokenName = ((JProperty)tierTokens[i]).Name;
                                    tokenValue = Convert.ToString(((JProperty)tierTokens[i]).Value ?? String.Empty);

                                    int n;
                                    val = tokenValue;
                                    if (tokenValue.Contains(",")) { val = tokenValue.Replace(",", ""); }
                                    if (tokenValue.Contains(".")) { val = tokenValue.Replace(".", ""); }

                                    isNumeric = int.TryParse(val, out n);
                                    if (tokenName != RuleConstants.CostShareTiers && tokenName != RuleConstants.BenefitCategory1
                                        && tokenName != RuleConstants.BenefitCategory2 && tokenName != RuleConstants.BenefitCategory3
                                        && tokenName != RuleConstants.RowIDProperty && tokenName != RuleConstants.ServiceSelectionMapping && tokenName != RuleConstants.PBPCode
                                        && tokenName != RuleConstants.Condition)
                                    {
                                        if (isNumeric && n >= 0)
                                        {
                                            if (tokenName == RuleConstants.MinimumCoinsurance || tokenName == RuleConstants.MaximumCoinsuarnce
                                                || tokenName == RuleConstants.MinCoinsurance || tokenName == RuleConstants.MaxCoinsurance)
                                                tokenValue = tokenValue + RuleConstants.Percent;
                                            else
                                            {
                                                if (tokenName != RuleConstants.OOPMPeriodicity && tokenName != RuleConstants.MaximumPlanBenefitCoveragePeriodicity
                                                    && tokenName != RuleConstants.RowIDProperty)
                                                {
                                                    tokenValue = RuleConstants.Dollar + tokenValue;
                                                }
                                            }
                                        }
                                        if (i == (tierTokensCount - 1))
                                            value += (tokenValue == String.Empty ? RuleConstants.NotApplicable : tokenValue) + "-" + GetFormatedTokenName(tokenName);
                                        else
                                        {
                                            value += (tokenValue == String.Empty ? RuleConstants.NotApplicable : tokenValue) + "-" + GetFormatedTokenName(tokenName) + ", ";
                                        }
                                    }
                                    else
                                    {
                                        // Remove last , from value if last element is BC1 or BC2 or BC3 or Tier or RowIDProperty
                                        if (!String.IsNullOrEmpty(value) && i == (tierTokensCount - 1))
                                        {
                                            value = value.Substring(0, value.Length - 2);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
            }
            return value;
        }

        private List<Variance> GetNetworkServiceCostShare(JToken networkTier, JToken previousYearService, JToken nextYearService, bool isDefaultService)
        {
            string value = String.Empty;
            List<Variance> detailedCompareList = new List<Variance>();
            try
            {
                if (null != networkTier && null != previousYearService && null != nextYearService)
                {
                    string tierName = Convert.ToString(networkTier["Tier"] ?? String.Empty);
                    if (!String.IsNullOrEmpty(tierName))
                    {
                        var previousYrValueToken = previousYearService;
                        var nextYrValueToken = nextYearService;
                        if (null != previousYrValueToken && null != nextYrValueToken)
                        {
                            detailedCompareList = DetailedCompare(previousYrValueToken, nextYrValueToken, isDefaultService);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
            }
            return detailedCompareList;
        }
        public List<Variance> DetailedCompare(JToken previousYrValueToken, JToken nextYrValueToken, bool isDefaultService)
        {
            List<Variance> variances = new List<Variance>();
            if (null != previousYrValueToken && null != nextYrValueToken)
            {
                List<JToken> nextYrTierTokens = nextYrValueToken.ToList();
                List<JToken> prevYrTierTokens = previousYrValueToken.ToList();

                if (null != nextYrTierTokens && null != prevYrTierTokens)
                {
                    int tierTokensCount = nextYrTierTokens.Count;
                    Variance v = new Variance();
                    string tokenName = string.Empty;
                    string previousVal = string.Empty;
                    string nextVal = string.Empty;
                    string symbol = string.Empty;
                    bool isNumeric = false;
                    for (int j = 0; j < tierTokensCount; j++)
                    {
                        symbol = string.Empty;
                        v = new Variance();
                        tokenName = ((JProperty)nextYrTierTokens[j]).Name;
                        nextVal = Convert.ToString(((JProperty)nextYrTierTokens[j]).Value ?? String.Empty);
                        previousVal = Convert.ToString(((JProperty)prevYrTierTokens[j]).Value ?? String.Empty);
                        //if(tokenName == RuleConstants.MaximumPlanBenefitCoverageAmount)
                        //{
                        if (!String.IsNullOrEmpty(nextVal) && nextVal.Equals("Not Applicable", StringComparison.CurrentCultureIgnoreCase))
                        {
                            nextVal = "";
                        }
                        if (!String.IsNullOrEmpty(previousVal) && previousVal.Equals("Not Applicable", StringComparison.CurrentCultureIgnoreCase))
                        {
                            previousVal = "";
                        }
                        //}
                        int n;
                        string val = previousVal;
                        if (previousVal.Contains(",")) { val = previousVal.Replace(",", ""); }
                        if (previousVal.Contains(".")) { val = previousVal.Replace(".", ""); }

                        isNumeric = int.TryParse(val, out n);
                        if (tokenName != RuleConstants.CostShareTiers && tokenName != RuleConstants.BenefitCategory1
                                    && tokenName != RuleConstants.BenefitCategory2 && tokenName != RuleConstants.BenefitCategory3
                                    && tokenName != RuleConstants.RowIDProperty && tokenName != RuleConstants.ServiceSelectionMapping && tokenName != RuleConstants.PBPCode
                                    && tokenName != RuleConstants.Condition)
                        {
                            if (isNumeric && n >= 0)
                            {
                                if (tokenName == RuleConstants.MinimumCoinsurance || tokenName == RuleConstants.MaximumCoinsuarnce
                                    || tokenName == RuleConstants.MinCoinsurance || tokenName == RuleConstants.MaxCoinsurance)
                                    previousVal = previousVal + RuleConstants.Percent;
                                else
                                {
                                    if (tokenName != RuleConstants.OOPMPeriodicity && tokenName != RuleConstants.MaximumPlanBenefitCoveragePeriodicity
                                        && tokenName != RuleConstants.RowIDProperty)
                                    {
                                        previousVal = RuleConstants.Dollar + previousVal;
                                    }
                                }
                            }
                            v.Prop = tokenName;
                            v.previousYrVal = symbol + previousVal;

                            symbol = string.Empty;

                            val = nextVal;
                            if (nextVal.Contains(",")) { val = nextVal.Replace(",", ""); }
                            if (nextVal.Contains(".")) { val = nextVal.Replace(".", ""); }
                            isNumeric = int.TryParse(val, out n);
                            if (isNumeric && n >= 0)
                            {
                                if (tokenName == RuleConstants.MinimumCoinsurance || tokenName == RuleConstants.MaximumCoinsuarnce
                                    || tokenName == RuleConstants.MinCoinsurance || tokenName == RuleConstants.MaxCoinsurance)
                                    nextVal = nextVal + RuleConstants.Percent;
                                else
                                {
                                    if (tokenName != RuleConstants.OOPMPeriodicity && tokenName != RuleConstants.MaximumPlanBenefitCoveragePeriodicity
                                        && tokenName != RuleConstants.RowIDProperty)
                                    {
                                        nextVal = RuleConstants.Dollar + nextVal;
                                    }
                                }
                            }
                            v.nextYrVal = symbol + nextVal;
                            if (isDefaultService)
                            {
                                if (tokenName != RuleConstants.CostShareTiers)
                                    variances.Add(v);
                            }
                            else
                            {
                                if (!v.nextYrVal.Equals(v.previousYrVal))
                                    variances.Add(v);
                            }
                        }
                    }
                    // Check for OOPMValue - Periodicity and MaximumPlanBenefitCoverageAmount - Periodicity
                    variances = FormatPeriodicityCostShare(variances, previousYrValueToken, nextYrValueToken);
                }
            }
            return variances;
        }
        private bool HasSameCostShare(BenefitsCompare benefitCompare)
        {
            bool result = false;
            try
            {
                if (null != benefitCompare)
                {
                    if (String.IsNullOrEmpty(benefitCompare.ThisYear) && String.IsNullOrEmpty(benefitCompare.NextYear))
                        result = true;
                }
            }
            catch (Exception)
            {
            }
            return result;
        }
        private string GetAmount(string sourceAmt)
        {
            string amount = sourceAmt;
            try
            {
                string val = sourceAmt;
                if (sourceAmt.Contains(",")) { val = sourceAmt.Replace(",", ""); }
                if (sourceAmt.Contains(".")) { val = sourceAmt.Replace(".", ""); }

                bool isNumeric = false; int n;
                isNumeric = int.TryParse(val, out n);
                if (isNumeric && n >= 0)
                    amount = RuleConstants.Dollar + sourceAmt;
            }
            catch (Exception)
            {
            }
            return amount;
        }
        private string GetFormatedTokenName(string key)
        {
            string tokenName = key;
            try
            {
                Dictionary<string, string> tokenDict = RuleConstants.GetTokenList();
                if (null != tokenDict && tokenDict.Count > 0)
                {
                    if (tokenDict.ContainsKey(key))
                        tokenName = tokenDict[key];
                }
            }
            catch (Exception)
            { }
            return tokenName;
        }
        private string GetOOPMPeriodicity(string key)
        {
            string tokenName = key;
            try
            {
                Dictionary<string, string> tokenDict = RuleConstants.GetPeriodicityList();
                if (null != tokenDict && tokenDict.Count > 0)
                {
                    key = key.Trim();
                    if (tokenDict.ContainsKey(key))
                        tokenName = tokenDict[key];
                }
            }
            catch (Exception)
            { }
            return tokenName;
        }

        private string GetOOPMPeriodicityAcute(string key)
        {
            string tokenName = key;
            try
            {
                Dictionary<string, string> tokenDict = RuleConstants.GetPeriodicityListForAcuteList();
                if (null != tokenDict && tokenDict.Count > 0)
                {
                    key = key.Trim();
                    if (tokenDict.ContainsKey(key))
                        tokenName = tokenDict[key];
                }
            }
            catch (Exception)
            { }
            return tokenName;
        }

        private string GetBenefitServiceFormatedValue(string value)
        {
            StringBuilder sb = new StringBuilder();
            Dictionary<string, string> costShareDic = new Dictionary<string, string>();
            string pTagPrefix = "<p style=\"font-size:   12pt; font-family: 'Times New Roman', serif; margin-top: 6pt;\">";
            string pTagPostFix = "</p>";
            try
            {
                if (!String.IsNullOrEmpty(value))
                {
                    string[] costShareDetails = value.Split(',');
                    foreach (string costShare in costShareDetails)
                    {
                        string[] costShareKeyValue = costShare.Split('-');
                        costShareDic.Add(costShareKeyValue[1], costShareKeyValue[0]);
                    }
                    foreach (var item in costShareDic)
                    {
                        switch (Convert.ToString(item.Key))
                        {
                            case "MinCopay":
                                if (!IsEmpty(item.Value))
                                    sb.Append("You pay " + SetValueFormat(item.Value) + " Minimum Copay for this Benefit.<br/>");
                                else
                                    sb.Append("You pay nothing for this Benefit.<br/>");
                                break;
                            case "MaxCopay":
                                if (!IsEmpty(item.Value))
                                    sb.Append("You pay " + SetValueFormat(item.Value) + " Maximum Copay for this Benefit.<br/>");
                                else
                                    sb.Append("You pay nothing for this Benefit.<br/>");
                                break;
                            case "MinCoinsurance":
                                if (!IsEmpty(item.Value))
                                    sb.Append("You pay " + item.Value + " Minimum Coinsurance for this Benefit.<br/>");
                                else
                                    sb.Append("You pay nothing for this Benefit.<br/>");
                                break;
                            case "MaxCoinsurance":
                                if (!IsEmpty(item.Value))
                                    sb.Append("You pay " + item.Value + " Maximum Coinsurance for this Benefit.<br/>");
                                else
                                    sb.Append("You pay nothing for this Benefit.");
                                break;
                            case "Deductible":
                                if (!IsEmpty(item.Value))
                                    sb.Append("There is a " + SetValueFormat(item.Value) + " Deductible.<br/>");
                                else
                                    sb.Append("Deductible is Not Applicable.<br/>");
                                break;
                            case "OOPM":
                                if (!IsEmpty(item.Value))
                                    sb.Append("There is a " + SetValueFormat(item.Value.Split('_')[0]) + " out-of - pocket limit " + GetOOPMPeriodicity(item.Value.Split('_')[1]) + ". <br/> ");
                                else
                                    sb.Append("There is no out-of-pocket limit.<br/>");
                                break;
                            case "Max Plan Benefit Amount":
                                if (!IsEmpty(item.Value))
                                    sb.Append("There is a " + SetValueFormat(item.Value.Split('_')[0]) + " allowance " + GetOOPMPeriodicity(item.Value.Split('_')[1]) + ". <br/>");
                                else
                                    sb.Append("There is no allowance.<br/>");
                                break;
                        }
                    }
                }
            }
            catch (Exception)
            { }
            return sb.ToString();
        }
        private string GetSlidingServiceFormatedValue(string value)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                if (!String.IsNullOrEmpty(value))
                {
                    string[] intervals = value.Split(';');
                    string interReportFormat = string.Empty;
                    foreach (string intervalData in intervals)
                    {
                        if (intervalData.Contains("Interval 0: "))
                        {
                            string[] IntervalZero = intervalData.Split(',');

                            if (IntervalZero.Length > 2)
                            {
                                if (!string.IsNullOrEmpty(IntervalZero[1]))
                                {
                                    if (IntervalZero[1].Trim() != "NA")
                                    {
                                        interReportFormat = SetValueFormat(IntervalZero[1]) + " per stay.<br/>";
                                    }
                                    else
                                    {
                                        interReportFormat = RuleConstants.NotApplicable + ".<br/>";
                                    }
                                }
                            }
                        }
                        else
                        {
                            string[] intervaleFormats = intervalData.Split(',');
                            string intervalLimit = intervaleFormats[0].Split(':')[1].Trim().ToString();
                            string benefitPeriod = !IsEmpty(intervaleFormats[2].Trim()) ? intervaleFormats[2] : "";
                            //if(intervalLimit == "Not Applicable - Not Applicable" && benefitPeriod == "" && SetValueFormat(intervaleFormats[1]).Contains("Not Applicable"))
                            //    interReportFormat = "You pay nothing for this Benefit. <br/>";
                            //else
                            interReportFormat = "You pay a " + SetValueFormat(intervaleFormats[1]) + " copay" + benefitPeriod + " for Days " + intervalLimit + ". <br/>";
                        }
                        sb.Append(interReportFormat);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return sb.ToString();
        }
        private bool IsEmpty(string value)
        {
            string[] exclusion = new string[] { "[]", "{}", "Not Applicable", "Not Applicable_ Not Applicable", "Not Covered", "NA", "Not Selected", "", "0", "0.0", "0.00","_" };
            return exclusion.Contains(value.Trim());
        }
        private string CheckDuplicateCostShare(string srcValue)
        {
            string value = srcValue;
            bool hasSameCopay = false;
            bool hasSameCoins = false;
            try
            {
                if (!String.IsNullOrEmpty(srcValue))
                {
                    String[] costShares = srcValue.Split(',');
                    if (null != costShares && costShares.Length > 0)
                    {
                        int count = costShares.Length;
                        Dictionary<string, string> costShareDict = new Dictionary<string, string>();
                        string dictKey = String.Empty; string dictVal = String.Empty;
                        for (int j = 0; j < count; j++)
                        {
                            dictVal = costShares[j].Split('-')[0];
                            dictKey = costShares[j].Split('-')[1];
                            costShareDict.Add(dictKey, dictVal);
                        }
                        if (null != costShareDict && costShareDict.Count > 0)
                        {
                            if (costShareDict.ContainsKey(RuleConstants.MinCopay) && costShareDict.ContainsKey(RuleConstants.MaxCopay))
                            {
                                string minCopayVal = costShareDict[RuleConstants.MinCopay];
                                string maxCopayVal = costShareDict[RuleConstants.MaxCopay];
                                if (minCopayVal.Trim().Equals(maxCopayVal.Trim()))
                                    hasSameCopay = true;
                            }
                            if (costShareDict.ContainsKey(RuleConstants.MinCoinsurance) && costShareDict.ContainsKey(RuleConstants.MaxCoinsurance))
                            {
                                string minCoinsVal = costShareDict[RuleConstants.MinCoinsurance];
                                string maxCoinsVal = costShareDict[RuleConstants.MaxCoinsurance];
                                if (minCoinsVal.Trim().Equals(maxCoinsVal.Trim()))
                                    hasSameCoins = true;
                            }
                            // Append OOPMValue & OOPMPeriodicity together                             
                            if (costShareDict.ContainsKey(RuleConstants.OOPM) && costShareDict.ContainsKey(RuleConstants.OOPM_Periodicity))
                            {
                                string OOPMValue = costShareDict[RuleConstants.OOPM];
                                string OOPMPeriodicity = costShareDict[RuleConstants.OOPM_Periodicity];
                                costShareDict.Remove(RuleConstants.OOPM_Periodicity);
                                costShareDict[RuleConstants.OOPM] = OOPMValue + "_" + OOPMPeriodicity;
                            }
                            // Append Max Benefit Amount & Max Benefit Amount Periodicity
                            if (costShareDict.ContainsKey(RuleConstants.MaxPlanBenefitAmount) && costShareDict.ContainsKey(RuleConstants.MaximunPlanBenefitPeriodicity))
                            {
                                string MaxPlanBenftAmt = costShareDict[RuleConstants.MaxPlanBenefitAmount];
                                string MaxPlanBenftPeriodicity = costShareDict[RuleConstants.MaximunPlanBenefitPeriodicity];
                                costShareDict.Remove(RuleConstants.MaximunPlanBenefitPeriodicity);
                                costShareDict[RuleConstants.MaxPlanBenefitAmount] = MaxPlanBenftAmt + "_" + MaxPlanBenftPeriodicity;
                            }
                            // Remove max copay from dictionary
                            if (hasSameCopay)
                                costShareDict.Remove(RuleConstants.MaxCopay);
                            // Remove max coinsurance from dictionary
                            if (hasSameCoins)
                                costShareDict.Remove(RuleConstants.MaxCoinsurance);
                            var dictCount = costShareDict.Count;
                            value = string.Empty;
                            int index = -1;
                            foreach (var item in costShareDict)
                            {
                                index = index + 1;
                                if (index == (dictCount - 1))
                                    value += item.Value + "-" + item.Key;
                                else
                                    value += item.Value + "-" + item.Key + ",";
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return value;
        }
        private List<BenefitsCompare> GetDefaultServiceCostShareValues(List<JToken> previousYearBRGList, List<JToken> nextYearBRGList, List<BenefitsCompare> comparisionBenefitServices)
        {
            string value = String.Empty;
            List<BenefitsCompare> benefitsCompareServices = new List<BenefitsCompare>();
            try
            {
                //List<JToken> defaultServices = new List<JToken>();
                //defaultServices = RuleConstants.GetDefaultServices();

                if (null != previousYearBRGList && null != nextYearBRGList)
                {
                    if (nextYearBRGList.Count > 0)
                    {
                        List<JToken> previousYearDefaultSer = GetDefaultBRGServices(false);
                        List<JToken> nextYearDefaultSer = GetDefaultBRGServices(true);
                        //create method for process OON Custom 

                        if (AnocHelper.IsPPOPlan(false) && AnocHelper.IsContainOONNetworkTier(false))
                        {
                            ProcessOONService(previousYearBRGList, false);
                        }
                        else if (AnocHelper.IsPOSPlan(false))
                        {
                            ProcessOONService(previousYearBRGList, false);
                        }

                        if (AnocHelper.IsPPOPlan(true) && AnocHelper.IsContainOONNetworkTier(true))
                        {
                            ProcessOONService(nextYearBRGList, true);
                        }
                        else if (AnocHelper.IsPOSPlan(true))
                        {
                            ProcessOONService(nextYearBRGList, true);
                        }


                        //if (AnocHelper.IsContainOONNetworkTier(false))
                        //{
                        //    ProcessOONService(previousYearDefaultSer, false);
                        //}
                        //if (AnocHelper.IsContainOONNetworkTier(true))
                        //{
                        //    ProcessOONService(nextYearDefaultSer, true);
                        //}


                        List<JToken> resultForTierUnion = new List<JToken>();
                        Dictionary<string, string> keys = new Dictionary<string, string>();
                        keys.Add(RuleConstants.BenefitCategory1, RuleConstants.BenefitCategory1);
                        keys.Add(RuleConstants.BenefitCategory2, RuleConstants.BenefitCategory2);
                        keys.Add(RuleConstants.BenefitCategory3, RuleConstants.BenefitCategory3);
                        keys.Add(RuleConstants.CostShareTiers, RuleConstants.CostShareTiers);

                        CollectionExecutionComparer operatorProcessor = new CollectionExecutionComparer(previousYearDefaultSer, nextYearDefaultSer, keys);
                        resultForTierUnion = operatorProcessor.Union();

                        //List<JToken> networkTierList = nextYearBRGList[0].SelectToken(RuleConstants.IQMedicareNetWorkList) == null ? new List<JToken>() : nextYearBRGList[0].SelectToken(RuleConstants.IQMedicareNetWorkList).ToList();
                        BenefitsCompare benefitsCompare = new BenefitsCompare();
                        int rowID = (comparisionBenefitServices != null && comparisionBenefitServices.Count > 0) ? comparisionBenefitServices.Last().RowIDProperty : 0;
                        foreach (var service in resultForTierUnion)
                        {
                            rowID = rowID + 1;
                            benefitsCompare = new BenefitsCompare();
                            // benefitsCompare.RowID = rowID;
                            benefitsCompare.ANOCBenefitName = String.Empty;
                            benefitsCompare.BenefitCategory1 = Convert.ToString(service[RuleConstants.BenefitCategory1] ?? String.Empty);
                            benefitsCompare.BenefitCategory2 = Convert.ToString(service[RuleConstants.BenefitCategory2] ?? String.Empty);
                            benefitsCompare.BenefitCategory3 = Convert.ToString(service[RuleConstants.BenefitCategory3] ?? String.Empty);
                            benefitsCompare.CostShareTiers = Convert.ToString(service["Tier"] ?? String.Empty);
                            benefitsCompare.NextYear = GetCostShareValues(benefitsCompare.BenefitCategory1, benefitsCompare.BenefitCategory2, benefitsCompare.BenefitCategory3, benefitsCompare.CostShareTiers, previousYearDefaultSer, nextYearDefaultSer, service, true, true);
                            benefitsCompare.ThisYear = GetCostShareValues(benefitsCompare.BenefitCategory1, benefitsCompare.BenefitCategory2, benefitsCompare.BenefitCategory3, benefitsCompare.CostShareTiers, previousYearDefaultSer, nextYearDefaultSer, service, false, true);
                            benefitsCompare.DisplayinANOC = "false";
                            //benefitsCompare.RowIDProperty = rowID - 1;
                            benefitsCompareServices.Add(benefitsCompare);
                        }
                        // Check if default services already 
                        if (null != benefitsCompareServices && null != comparisionBenefitServices)
                        {
                            foreach (var item in benefitsCompareServices)
                            {
                                var service = comparisionBenefitServices.Where(x => x.BenefitCategory1 == item.BenefitCategory1
                                                                           && x.BenefitCategory2 == item.BenefitCategory2
                                                                           && x.BenefitCategory3 == item.BenefitCategory3
                                                                           && x.CostShareTiers == item.CostShareTiers).FirstOrDefault();
                                if (null != service)
                                {
                                    comparisionBenefitServices.Remove(service);
                                    item.DisplayinANOC = "Yes";
                                    comparisionBenefitServices.Add(item);
                                }
                                else
                                    comparisionBenefitServices.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
            }
            return comparisionBenefitServices;
        }
        private List<Variance> FormatPeriodicityCostShare(List<Variance> varianceList, JToken previousYrValueToken, JToken nextYrValueToken)
        {
            List<Variance> diffrence = new List<Variance>();
            Variance v = new Variance();
            string tokenName = String.Empty;
            string nextVal; string previousVal;
            bool isNumeric = false;
            try
            {
                if (null != varianceList && varianceList.Count > 0)
                {
                    #region OOPM Value - Periodicity
                    var OOPMValue = varianceList.Where(x => x.Prop == RuleConstants.OOPMValue).FirstOrDefault();
                    if (null != OOPMValue)
                    {
                        // Check for OOPMPeriodicity if has OOPMValue
                        var OOPMPeriodicityVal = varianceList.Where(x => x.Prop == RuleConstants.OOPMPeriodicity).FirstOrDefault();
                        if (null == OOPMPeriodicityVal)
                        {
                            v = new Variance();
                            v.Prop = RuleConstants.OOPMPeriodicity;
                            nextVal = Convert.ToString(nextYrValueToken[RuleConstants.OOPMPeriodicity] ?? String.Empty);
                            previousVal = Convert.ToString(previousYrValueToken[RuleConstants.OOPMPeriodicity] ?? String.Empty);
                            v.previousYrVal = previousVal;
                            v.nextYrVal = nextVal;
                            diffrence.Add(v);
                        }
                    }
                    else
                    {
                        var OOPMPeriodicity = varianceList.Where(x => x.Prop == RuleConstants.OOPMPeriodicity).FirstOrDefault();
                        // Check for OOPMPeriodicity 
                        if (null != OOPMPeriodicity)
                        {
                            // Check for OOPMValue if has OOPMPeriodicity 
                            var OOPMVal = varianceList.Where(x => x.Prop == RuleConstants.OOPMValue).FirstOrDefault();
                            if (null == OOPMVal)
                            {
                                v = new Variance();
                                v.Prop = RuleConstants.OOPMValue;
                                nextVal = Convert.ToString(nextYrValueToken[RuleConstants.OOPMValue] ?? String.Empty);

                                int n;
                                string val = nextVal;
                                if (nextVal.Contains(",")) { val = nextVal.Replace(",", ""); }
                                if (nextVal.Contains(".")) { val = nextVal.Replace(".", ""); }

                                isNumeric = int.TryParse(val, out n);
                                if (isNumeric && n >= 0)
                                    nextVal = RuleConstants.Dollar + nextVal;

                                previousVal = Convert.ToString(previousYrValueToken[RuleConstants.OOPMValue] ?? String.Empty);

                                val = previousVal;
                                if (previousVal.Contains(",")) { val = previousVal.Replace(",", ""); }
                                if (previousVal.Contains(".")) { val = previousVal.Replace(".", ""); }

                                isNumeric = int.TryParse(val, out n);
                                if (isNumeric && n >= 0)
                                    previousVal = RuleConstants.Dollar + previousVal;

                                v.previousYrVal = previousVal;
                                v.nextYrVal = nextVal;
                                diffrence.Add(v);
                            }
                        }
                    }
                    #endregion OOPM Value - Periodicity

                    #region Max Benefit Coverage Amount - Periodicity
                    var MaxPlanBenftAmount = varianceList.Where(x => x.Prop == RuleConstants.MaximumPlanBenefitCoverageAmount).FirstOrDefault();
                    if (null != MaxPlanBenftAmount)
                    {
                        // Check for MaximumPlanBenefitCoveragePeriodicity if has MaxPlanBenftAmount 
                        var MaxPlanBenftPeriodicityVal = varianceList.Where(x => x.Prop == RuleConstants.MaximumPlanBenefitCoveragePeriodicity).FirstOrDefault();
                        if (null == MaxPlanBenftPeriodicityVal)
                        {
                            v = new Variance();
                            v.Prop = RuleConstants.MaximumPlanBenefitCoveragePeriodicity;
                            nextVal = Convert.ToString(nextYrValueToken[RuleConstants.MaximumPlanBenefitCoveragePeriodicity] ?? String.Empty);
                            previousVal = Convert.ToString(previousYrValueToken[RuleConstants.MaximumPlanBenefitCoveragePeriodicity] ?? String.Empty);
                            v.previousYrVal = previousVal;
                            v.nextYrVal = nextVal;
                            diffrence.Add(v);
                        }
                    }
                    else
                    {
                        var MaxPlanBenftPeriodicity = varianceList.Where(x => x.Prop == RuleConstants.MaximumPlanBenefitCoveragePeriodicity).FirstOrDefault();
                        // Check for MaxPlanBenftPeriodicity 
                        if (null != MaxPlanBenftPeriodicity)
                        {
                            // Check for MaxPlanBenftAmount if has MaxPlanBenftPeriodicity 
                            var MaxPlanBenftAmountVal = varianceList.Where(x => x.Prop == RuleConstants.MaximumPlanBenefitCoverageAmount).FirstOrDefault();
                            if (null == MaxPlanBenftAmountVal)
                            {
                                v = new Variance();
                                v.Prop = RuleConstants.MaximumPlanBenefitCoverageAmount;
                                int n;
                                nextVal = Convert.ToString(nextYrValueToken[RuleConstants.MaximumPlanBenefitCoverageAmount] ?? String.Empty);

                                string val = nextVal;
                                if (nextVal.Contains(",")) { val = nextVal.Replace(",", ""); }
                                if (nextVal.Contains(".")) { val = nextVal.Replace(".", ""); }

                                isNumeric = int.TryParse(val, out n);
                                if (isNumeric && n >= 0)
                                    nextVal = RuleConstants.Dollar + nextVal;

                                previousVal = Convert.ToString(previousYrValueToken[RuleConstants.MaximumPlanBenefitCoverageAmount] ?? String.Empty);
                                val = previousVal;
                                if (previousVal.Contains(",")) { val = previousVal.Replace(",", ""); }
                                if (previousVal.Contains(".")) { val = previousVal.Replace(".", ""); }

                                isNumeric = int.TryParse(val, out n);
                                if (isNumeric && n >= 0)
                                    previousVal = RuleConstants.Dollar + previousVal;
                                v.previousYrVal = previousVal;
                                v.nextYrVal = nextVal;
                                diffrence.Add(v);
                            }
                        }
                    }
                    #endregion Max Benefit Coverage Amount - Periodicity
                    if (null != diffrence && diffrence.Count > 0)
                    {
                        foreach (var item in diffrence)
                        {
                            varianceList.Add(item);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return varianceList;
        }

        private List<BenefitsCompare> GetSlidingCostShareServices(List<BenefitsCompare> comparisionBenefitServices)
        {
            string value = String.Empty;
            List<BenefitsCompare> slidingCostShareServices = new List<BenefitsCompare>();
            BenefitsCompare benefitsCompare = new BenefitsCompare();
            List<JToken> defaultServices = new List<JToken>();
            List<JToken> resultForTierUnion = new List<JToken>();
            try
            {
                //List<JToken> defaultServices = new List<JToken>();
                //defaultServices = RuleConstants.GetDefaultServices();

                List<JToken> previousYearDefaultSer = GetDefaultSlidingServices(false);
                List<JToken> nextYearDefaultSer = GetDefaultSlidingServices(true);

                Dictionary<string, string> keys = new Dictionary<string, string>();
                keys.Add(RuleConstants.BenefitCategory1, RuleConstants.BenefitCategory1);
                keys.Add(RuleConstants.BenefitCategory2, RuleConstants.BenefitCategory2);
                keys.Add(RuleConstants.BenefitCategory3, RuleConstants.BenefitCategory3);
                keys.Add(RuleConstants.CostShareTiers, RuleConstants.CostShareTiers);

                CollectionExecutionComparer operatorProcessor = new CollectionExecutionComparer(previousYearDefaultSer, nextYearDefaultSer, keys);
                defaultServices = operatorProcessor.Union();

                List<JToken> previousYearSlidingList = GetSlidingCostShareServices(false, false, null);
                List<JToken> nextYearSlidingList = GetSlidingCostShareServices(true, false, null);
                List<JToken> prevSlidingCostShareIntervalList = new List<JToken>();
                List<JToken> nextSlidingCostShareIntervalList = new List<JToken>();

                if (null != previousYearSlidingList && null != nextYearSlidingList)
                {
                    CollectionExecutionComparer distinctProcessor = new CollectionExecutionComparer(previousYearSlidingList, keys);
                    prevSlidingCostShareIntervalList = distinctProcessor.Distinct();

                    distinctProcessor = new CollectionExecutionComparer(nextYearSlidingList, keys);
                    nextSlidingCostShareIntervalList = distinctProcessor.Distinct();
                }

                List<JToken> prevSlidingCostShareInfoList = previousYearSlidingList;
                List<JToken> nextSlidingCostShareInfoList = nextYearSlidingList;

                operatorProcessor = new CollectionExecutionComparer(prevSlidingCostShareIntervalList, nextSlidingCostShareIntervalList, keys);
                resultForTierUnion = operatorProcessor.Union();

                // Check if Default Sliding cost share services exist in resultForTierUnion and if not add to resultForTierUnion as its default
                List<JToken> defaultSerToAdd = new List<JToken>();
                operatorProcessor = new CollectionExecutionComparer(defaultServices, resultForTierUnion, keys);
                defaultSerToAdd = operatorProcessor.Except();
                if (defaultSerToAdd != null && defaultSerToAdd.Count > 0)
                {
                    resultForTierUnion.AddRange(defaultSerToAdd);
                }

                bool hasBlankRow = false;
                int rowID = 0;
                List<JToken> prevIntervalInfo = new List<JToken>();
                List<JToken> nextIntervalInfo = new List<JToken>();
                bool isDefaultService = false; bool displayInANOC = false;
                foreach (var token in resultForTierUnion)
                {
                    rowID = rowID + 1;
                    benefitsCompare = new BenefitsCompare();
                    hasBlankRow = HasBlankRow(token);
                    if (!hasBlankRow)
                    {
                        isDefaultService = defaultServices.Exists(x => Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty) == Convert.ToString(token[RuleConstants.BenefitCategory1] ?? String.Empty)
                                                                    && Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty) == Convert.ToString(token[RuleConstants.BenefitCategory2] ?? String.Empty)
                                                                    && Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty) == Convert.ToString(token[RuleConstants.BenefitCategory3] ?? String.Empty)
                                                                    && Convert.ToString(x.SelectToken(RuleConstants.CostShareTiers) ?? String.Empty) == Convert.ToString(token[RuleConstants.CostShareTiers] ?? String.Empty));

                        displayInANOC = defaultSerToAdd.Exists(x => Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty) == Convert.ToString(token[RuleConstants.BenefitCategory1] ?? String.Empty)
                                                                    && Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty) == Convert.ToString(token[RuleConstants.BenefitCategory2] ?? String.Empty)
                                                                    && Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty) == Convert.ToString(token[RuleConstants.BenefitCategory3] ?? String.Empty)
                                                                    && Convert.ToString(x.SelectToken(RuleConstants.CostShareTiers) ?? String.Empty) == Convert.ToString(token[RuleConstants.CostShareTiers] ?? String.Empty));

                        prevIntervalInfo = prevSlidingCostShareInfoList.FindAll(x => Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty) == Convert.ToString(token[RuleConstants.BenefitCategory1] ?? String.Empty)
                                                   && Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty) == Convert.ToString(token[RuleConstants.BenefitCategory2] ?? String.Empty)
                                                   && Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty) == Convert.ToString(token[RuleConstants.BenefitCategory3] ?? String.Empty)
                                                   && Convert.ToString(x.SelectToken(RuleConstants.CostShareTiers) ?? String.Empty) == Convert.ToString(token[RuleConstants.CostShareTiers] ?? String.Empty));

                        nextIntervalInfo = nextSlidingCostShareInfoList.FindAll(x => Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty) == Convert.ToString(token[RuleConstants.BenefitCategory1] ?? String.Empty)
                                                     && Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty) == Convert.ToString(token[RuleConstants.BenefitCategory2] ?? String.Empty)
                                                     && Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty) == Convert.ToString(token[RuleConstants.BenefitCategory3] ?? String.Empty)
                                                     && Convert.ToString(x.SelectToken(RuleConstants.CostShareTiers) ?? String.Empty) == Convert.ToString(token[RuleConstants.CostShareTiers] ?? String.Empty));

                        //benefitsCompare.RowID = rowID;
                        benefitsCompare.ANOCBenefitName = String.Empty;
                        benefitsCompare.BenefitCategory1 = Convert.ToString(token[RuleConstants.BenefitCategory1] ?? String.Empty);
                        benefitsCompare.BenefitCategory2 = Convert.ToString(token[RuleConstants.BenefitCategory2] ?? String.Empty);
                        benefitsCompare.BenefitCategory3 = Convert.ToString(token[RuleConstants.BenefitCategory3] ?? String.Empty);
                        benefitsCompare.CostShareTiers = Convert.ToString(token[RuleConstants.CostShareTiers] ?? String.Empty);
                        benefitsCompare.NextYear = GetSlidingCostShareValues(benefitsCompare.BenefitCategory1, benefitsCompare.BenefitCategory2, benefitsCompare.BenefitCategory3, benefitsCompare.CostShareTiers, prevSlidingCostShareIntervalList, nextSlidingCostShareIntervalList, prevIntervalInfo, nextIntervalInfo, token, true, isDefaultService);
                        benefitsCompare.ThisYear = GetSlidingCostShareValues(benefitsCompare.BenefitCategory1, benefitsCompare.BenefitCategory2, benefitsCompare.BenefitCategory3, benefitsCompare.CostShareTiers, prevSlidingCostShareIntervalList, nextSlidingCostShareIntervalList, prevIntervalInfo, nextIntervalInfo, token, false, isDefaultService);
                        if (displayInANOC)
                            benefitsCompare.DisplayinANOC = "false";
                        else
                            benefitsCompare.DisplayinANOC = "Yes";
                        //benefitsCompare.RowIDProperty = rowID - 1;
                        // Check if all network values are blank
                        if (!HasSameCostShare(benefitsCompare))
                            slidingCostShareServices.Add(benefitsCompare);

                        // Need to check if service exist in BRG and not covered in sliding costshare comparision 
                        // If not covered in our sliding costshare comparison then we need to add it eventhough not in compaarision,
                        // as we are adding \r\n in between costshares of Slidingcostshare repeaters and BRG repeaters 
                        var slidingCostShareExist = slidingCostShareServices.Exists(x => x.BenefitCategory1 == Convert.ToString(token[RuleConstants.BenefitCategory1] ?? String.Empty)
                                                  && x.BenefitCategory2 == Convert.ToString(token[RuleConstants.BenefitCategory2] ?? String.Empty)
                                                  && x.BenefitCategory3 == Convert.ToString(token[RuleConstants.BenefitCategory3] ?? String.Empty)
                                                  && x.CostShareTiers == Convert.ToString(token[RuleConstants.CostShareTiers] ?? String.Empty));

                        if (!slidingCostShareExist)
                        {
                            var brgExist = comparisionBenefitServices.Exists(x => x.BenefitCategory1 == Convert.ToString(token[RuleConstants.BenefitCategory1] ?? String.Empty)
                                                  && x.BenefitCategory2 == Convert.ToString(token[RuleConstants.BenefitCategory2] ?? String.Empty)
                                                  && x.BenefitCategory3 == Convert.ToString(token[RuleConstants.BenefitCategory3] ?? String.Empty)
                                                  && x.CostShareTiers == Convert.ToString(token[RuleConstants.CostShareTiers] ?? String.Empty));
                            if (brgExist)
                                slidingCostShareServices.Add(benefitsCompare);
                        }
                    }
                }
                // Compare seervices of sliding costshare and BRG to merge into single repeater with both slidingcostshare and BRG data in required format
                comparisionBenefitServices = CompareServices(slidingCostShareServices, comparisionBenefitServices);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
            }
            return comparisionBenefitServices;
        }
        private string GetSlidingCostShareValues(string benefitCategory1, string benefitCategory2, string benefitCategory3, string tier, List<JToken> prevSlidingCostShareIntervalList, List<JToken> nextSlidingCostShareIntervalList, List<JToken> prevIntervalInfo, List<JToken> nextIntervalInfo, JToken networkTier, bool isNextYear, bool isDefaultService)
        {
            bool previousYearExist = false;
            bool nextYearExist = false;
            string value = String.Empty;
            Dictionary<string, List<Variance>> detailedCompareList = new Dictionary<string, List<Variance>>();
            try
            {
                if (null != prevSlidingCostShareIntervalList && null != nextSlidingCostShareIntervalList)
                {
                    #region Previous Year
                    if (!isNextYear)
                    {
                        previousYearExist = prevSlidingCostShareIntervalList.Exists(x => Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty) == benefitCategory1
                                                       && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty) == benefitCategory2)
                                                       && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty) == benefitCategory3)
                                                       && (Convert.ToString(x.SelectToken(RuleConstants.CostShareTiers) ?? String.Empty) == tier));

                        if (previousYearExist)
                        {
                            nextYearExist = nextSlidingCostShareIntervalList.Exists(x => Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty) == benefitCategory1
                                                      && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty) == benefitCategory2)
                                                      && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty) == benefitCategory3)
                                                      && (Convert.ToString(x.SelectToken(RuleConstants.CostShareTiers) ?? String.Empty) == tier));

                            if (nextYearExist)
                            {
                                var previousYrInterval = prevSlidingCostShareIntervalList.Where(x => Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty) == benefitCategory1
                                                       && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty) == benefitCategory2)
                                                       && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty) == benefitCategory3)
                                                       && (Convert.ToString(x.SelectToken(RuleConstants.CostShareTiers) ?? String.Empty) == tier)).FirstOrDefault();

                                var nextYrInterval = nextSlidingCostShareIntervalList.Where(x => Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty) == benefitCategory1
                                                      && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty) == benefitCategory2)
                                                      && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty) == benefitCategory3)
                                                      && (Convert.ToString(x.SelectToken(RuleConstants.CostShareTiers) ?? String.Empty) == tier)).FirstOrDefault();

                                detailedCompareList = GetNetworkServiceSlidingCostShare(networkTier, previousYrInterval, nextYrInterval, prevIntervalInfo, nextIntervalInfo, isDefaultService);
                                if (null != detailedCompareList && detailedCompareList.Count > 0)
                                {
                                    const string valFormat = "{0}: {1} - {2}, {3}, {4}";
                                    string first = ""; string second = ""; string third = ""; string fourth = ""; string fifth = "";
                                    int count = detailedCompareList.Count;
                                    List<Variance> difference = new List<Variance>();
                                    int indx = -1;
                                    string val = String.Empty;
                                    foreach (var item in detailedCompareList)
                                    {
                                        indx = indx + 1;
                                        if (isNextYear)
                                        {
                                            if (indx == (count - 1))
                                            {
                                                difference = detailedCompareList[item.Key];
                                                if (null != difference && difference.Count > 0)
                                                {
                                                    first = item.Key;
                                                    foreach (var entry in difference)
                                                    {
                                                        if (entry.Prop == "BenefitPeriod")
                                                        {
                                                            val = entry.nextYrVal;
                                                            if (String.IsNullOrEmpty(val))
                                                                val = RuleConstants.NotApplicable;
                                                            fifth = val;
                                                        }
                                                        if (entry.Prop == "Copay" || entry.Prop == "Coinsurance")
                                                        {
                                                            val = entry.nextYrVal;
                                                            if (String.IsNullOrEmpty(val))
                                                                val = RuleConstants.NotApplicable;
                                                            fourth = val;
                                                        }
                                                        else if (entry.Prop == "CopayBeginDay" || entry.Prop == "CoinsuranceBeginDay")
                                                        {
                                                            val = entry.nextYrVal;
                                                            if (String.IsNullOrEmpty(val))
                                                                val = RuleConstants.NotApplicable;
                                                            second = val;
                                                        }
                                                        else if (entry.Prop == "CopayEndDay" || entry.Prop == "CoinsuranceEndDay")
                                                        {
                                                            val = entry.nextYrVal;
                                                            if (String.IsNullOrEmpty(val))
                                                                val = RuleConstants.NotApplicable;
                                                            third = val;
                                                        }
                                                    }
                                                    value += string.Format(valFormat, first, second, third, fourth, fifth);
                                                }
                                            }
                                            else
                                            {
                                                difference = detailedCompareList[item.Key];
                                                if (null != difference && difference.Count > 0)
                                                {
                                                    first = item.Key;
                                                    foreach (var entry in difference)
                                                    {
                                                        if (entry.Prop == "BenefitPeriod")
                                                        {
                                                            val = entry.nextYrVal;
                                                            if (String.IsNullOrEmpty(val))
                                                                val = RuleConstants.NotApplicable;
                                                            fifth = val;
                                                        }
                                                        if (entry.Prop == "Copay" || entry.Prop == "Coinsurance")
                                                        {
                                                            val = entry.nextYrVal;
                                                            if (String.IsNullOrEmpty(val))
                                                                val = RuleConstants.NotApplicable;
                                                            fourth = val;
                                                        }
                                                        else if (entry.Prop == "CopayBeginDay" || entry.Prop == "CoinsuranceBeginDay")
                                                        {
                                                            val = entry.nextYrVal;
                                                            if (String.IsNullOrEmpty(val))
                                                                val = RuleConstants.NotApplicable;
                                                            second = val;
                                                        }
                                                        else if (entry.Prop == "CopayEndDay" || entry.Prop == "CoinsuranceEndDay")
                                                        {
                                                            val = entry.nextYrVal;
                                                            if (String.IsNullOrEmpty(val))
                                                                val = RuleConstants.NotApplicable;
                                                            third = val;
                                                        }
                                                    }
                                                    value += string.Format(valFormat, first, second, third, fourth, fifth) + "; ";
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (indx == (count - 1))
                                            {
                                                difference = detailedCompareList[item.Key];
                                                if (null != difference && difference.Count > 0)
                                                {
                                                    first = item.Key;
                                                    foreach (var entry in difference)
                                                    {
                                                        if (entry.Prop == "BenefitPeriod")
                                                        {
                                                            val = entry.previousYrVal;
                                                            if (String.IsNullOrEmpty(val))
                                                                val = RuleConstants.NotApplicable;
                                                            fifth = val;
                                                        }
                                                        if (entry.Prop == "Copay" || entry.Prop == "Coinsurance")
                                                        {
                                                            val = entry.previousYrVal;
                                                            if (String.IsNullOrEmpty(val))
                                                                val = RuleConstants.NotApplicable;
                                                            fourth = val;
                                                        }
                                                        else if (entry.Prop == "CopayBeginDay" || entry.Prop == "CoinsuranceBeginDay")
                                                        {
                                                            val = entry.previousYrVal;
                                                            if (String.IsNullOrEmpty(val))
                                                                val = RuleConstants.NotApplicable;
                                                            second = val;
                                                        }
                                                        else if (entry.Prop == "CopayEndDay" || entry.Prop == "CoinsuranceEndDay")
                                                        {
                                                            val = entry.previousYrVal;
                                                            if (String.IsNullOrEmpty(val))
                                                                val = RuleConstants.NotApplicable;
                                                            third = val;
                                                        }
                                                    }
                                                    value += string.Format(valFormat, first, second, third, fourth, fifth);
                                                }
                                            }
                                            else
                                            {
                                                difference = detailedCompareList[item.Key];
                                                if (null != difference && difference.Count > 0)
                                                {
                                                    first = item.Key;
                                                    foreach (var entry in difference)
                                                    {
                                                        if (entry.Prop == "BenefitPeriod")
                                                        {
                                                            val = entry.previousYrVal;
                                                            if (String.IsNullOrEmpty(val))
                                                                val = RuleConstants.NotApplicable;
                                                            fifth = val;
                                                        }
                                                        if (entry.Prop == "Copay" || entry.Prop == "Coinsurance")
                                                        {
                                                            val = entry.previousYrVal;
                                                            if (String.IsNullOrEmpty(val))
                                                                val = RuleConstants.NotApplicable;
                                                            fourth = val;
                                                        }
                                                        else if (entry.Prop == "CopayBeginDay" || entry.Prop == "CoinsuranceBeginDay")
                                                        {
                                                            val = entry.previousYrVal;
                                                            if (String.IsNullOrEmpty(val))
                                                                val = RuleConstants.NotApplicable;
                                                            second = val;
                                                        }
                                                        else if (entry.Prop == "CopayEndDay" || entry.Prop == "CoinsuranceEndDay")
                                                        {
                                                            val = entry.previousYrVal;
                                                            if (String.IsNullOrEmpty(val))
                                                                val = RuleConstants.NotApplicable;
                                                            third = val;
                                                        }
                                                    }
                                                    value += string.Format(valFormat, first, second, third, fourth, fifth) + "; ";
                                                }
                                            }
                                        }
                                    }
                                    // Get Slidiing CostShare foramatted text
                                    value = GetSlidingServiceFormatedValue(value);
                                }
                            }
                            else
                            {
                                var service = prevSlidingCostShareIntervalList.Where(x => Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty) == benefitCategory1
                                                        && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty) == benefitCategory2)
                                                        && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty) == benefitCategory3)
                                                        && (Convert.ToString(x.SelectToken(RuleConstants.CostShareTiers) ?? String.Empty) == tier)).FirstOrDefault();

                                detailedCompareList = GetNotCoveredServiceSlidingCostShare(networkTier, service, prevIntervalInfo, service, null);

                                #region previousSliding costshare
                                if (null != detailedCompareList && detailedCompareList.Count > 0)
                                {
                                    const string valFormat = "{0}: {1} - {2}, {3}, {4}";
                                    string first = ""; string second = ""; string third = ""; string fourth = ""; string fifth = "";
                                    int count = detailedCompareList.Count;
                                    List<Variance> difference = new List<Variance>();
                                    int indx = -1;
                                    string val = String.Empty;
                                    foreach (var item in detailedCompareList)
                                    {
                                        indx = indx + 1;
                                        if (indx == (count - 1))
                                        {
                                            difference = detailedCompareList[item.Key];
                                            if (null != difference && difference.Count > 0)
                                            {
                                                first = item.Key;
                                                foreach (var entry in difference)
                                                {
                                                    if (entry.Prop == "BenefitPeriod")
                                                    {
                                                        val = entry.previousYrVal;
                                                        if (String.IsNullOrEmpty(val))
                                                            val = RuleConstants.NotApplicable;
                                                        fifth = val;
                                                    }
                                                    if (entry.Prop == "Copay" || entry.Prop == "Coinsurance")
                                                    {
                                                        val = entry.previousYrVal;
                                                        if (String.IsNullOrEmpty(val))
                                                            val = RuleConstants.NotApplicable;
                                                        fourth = val;
                                                    }
                                                    else if (entry.Prop == "CopayBeginDay" || entry.Prop == "CoinsuranceBeginDay")
                                                    {
                                                        val = entry.previousYrVal;
                                                        if (String.IsNullOrEmpty(val))
                                                            val = RuleConstants.NotApplicable;
                                                        second = val;
                                                    }
                                                    else if (entry.Prop == "CopayEndDay" || entry.Prop == "CoinsuranceEndDay")
                                                    {
                                                        val = entry.previousYrVal;
                                                        if (String.IsNullOrEmpty(val))
                                                            val = RuleConstants.NotApplicable;
                                                        third = val;
                                                    }
                                                }
                                                value += string.Format(valFormat, first, second, third, fourth, fifth);
                                            }
                                        }
                                        else
                                        {
                                            difference = detailedCompareList[item.Key];
                                            if (null != difference && difference.Count > 0)
                                            {
                                                first = item.Key;
                                                foreach (var entry in difference)
                                                {
                                                    if (entry.Prop == "BenefitPeriod")
                                                    {
                                                        val = entry.previousYrVal;
                                                        if (String.IsNullOrEmpty(val))
                                                            val = RuleConstants.NotApplicable;
                                                        fifth = val;
                                                    }
                                                    if (entry.Prop == "Copay" || entry.Prop == "Coinsurance")
                                                    {
                                                        val = entry.previousYrVal;
                                                        if (String.IsNullOrEmpty(val))
                                                            val = RuleConstants.NotApplicable;
                                                        fourth = val;
                                                    }
                                                    else if (entry.Prop == "CopayBeginDay" || entry.Prop == "CoinsuranceBeginDay")
                                                    {
                                                        val = entry.previousYrVal;
                                                        if (String.IsNullOrEmpty(val))
                                                            val = RuleConstants.NotApplicable;
                                                        second = val;
                                                    }
                                                    else if (entry.Prop == "CopayEndDay" || entry.Prop == "CoinsuranceEndDay")
                                                    {
                                                        val = entry.previousYrVal;
                                                        if (String.IsNullOrEmpty(val))
                                                            val = RuleConstants.NotApplicable;
                                                        third = val;
                                                    }
                                                }
                                                value += string.Format(valFormat, first, second, third, fourth, fifth) + "; ";
                                            }
                                        }
                                    }
                                    value = GetSlidingServiceFormatedValue(value);
                                }
                                #endregion previousSliding costshare
                            }
                        }
                        else
                        {
                            value = RuleConstants.NotCovered;
                        }
                    }
                    #endregion Previous Year

                    #region Next Year
                    else
                    {
                        nextYearExist = nextSlidingCostShareIntervalList.Exists(x => Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty) == benefitCategory1
                                                      && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty) == benefitCategory2)
                                                      && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty) == benefitCategory3)
                                                      && (Convert.ToString(x.SelectToken(RuleConstants.CostShareTiers) ?? String.Empty) == tier));

                        if (nextYearExist)
                        {
                            previousYearExist = prevSlidingCostShareIntervalList.Exists(x => Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty) == benefitCategory1
                                                      && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty) == benefitCategory2)
                                                      && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty) == benefitCategory3)
                                                      && (Convert.ToString(x.SelectToken(RuleConstants.CostShareTiers) ?? String.Empty) == tier));

                            if (previousYearExist)
                            {
                                var previousYrInterval = prevSlidingCostShareIntervalList.Where(x => Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty) == benefitCategory1
                                                  && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty) == benefitCategory2)
                                                  && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty) == benefitCategory3)
                                                  && (Convert.ToString(x.SelectToken(RuleConstants.CostShareTiers) ?? String.Empty) == tier)).FirstOrDefault();

                                var nextYrInterval = nextSlidingCostShareIntervalList.Where(x => Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty) == benefitCategory1
                                                     && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty) == benefitCategory2)
                                                     && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty) == benefitCategory3)
                                                     && (Convert.ToString(x.SelectToken(RuleConstants.CostShareTiers) ?? String.Empty) == tier)).FirstOrDefault();

                                detailedCompareList = GetNetworkServiceSlidingCostShare(networkTier, previousYrInterval, nextYrInterval, prevIntervalInfo, nextIntervalInfo, isDefaultService);
                                if (null != detailedCompareList && detailedCompareList.Count > 0)
                                {
                                    const string valFormat = "{0}: {1} - {2}, {3}, {4}";
                                    string first = ""; string second = ""; string third = ""; string fourth = ""; string fifth = "";
                                    int count = detailedCompareList.Count;
                                    List<Variance> difference = new List<Variance>();
                                    int indx = -1;
                                    string val = String.Empty;
                                    foreach (var item in detailedCompareList)
                                    {
                                        indx = indx + 1;
                                        if (isNextYear)
                                        {
                                            if (indx == (count - 1))
                                            {
                                                difference = detailedCompareList[item.Key];
                                                if (null != difference && difference.Count > 0)
                                                {
                                                    first = item.Key;
                                                    foreach (var entry in difference)
                                                    {
                                                        if (entry.Prop == "BenefitPeriod")
                                                        {
                                                            val = entry.nextYrVal;
                                                            if (String.IsNullOrEmpty(val))
                                                                val = RuleConstants.NotApplicable;
                                                            fifth = val;
                                                        }

                                                        if (entry.Prop == "Copay" || entry.Prop == "Coinsurance")
                                                        {
                                                            val = entry.nextYrVal;
                                                            if (String.IsNullOrEmpty(val))
                                                                val = RuleConstants.NotApplicable;
                                                            fourth = val;
                                                        }
                                                        else if (entry.Prop == "CopayBeginDay" || entry.Prop == "CoinsuranceBeginDay")
                                                        {
                                                            val = entry.nextYrVal;
                                                            if (String.IsNullOrEmpty(val))
                                                                val = RuleConstants.NotApplicable;
                                                            second = val;
                                                        }
                                                        else if (entry.Prop == "CopayEndDay" || entry.Prop == "CoinsuranceEndDay")
                                                        {
                                                            val = entry.nextYrVal;
                                                            if (String.IsNullOrEmpty(val))
                                                                val = RuleConstants.NotApplicable;
                                                            third = val;
                                                        }
                                                    }
                                                    value += string.Format(valFormat, first, second, third, fourth, fifth);
                                                }
                                            }
                                            else
                                            {
                                                difference = detailedCompareList[item.Key];
                                                if (null != difference && difference.Count > 0)
                                                {
                                                    first = item.Key;
                                                    foreach (var entry in difference)
                                                    {
                                                        if (entry.Prop == "BenefitPeriod")
                                                        {
                                                            val = entry.nextYrVal;
                                                            if (String.IsNullOrEmpty(val))
                                                                val = RuleConstants.NotApplicable;
                                                            fifth = val;
                                                        }

                                                        if (entry.Prop == "Copay" || entry.Prop == "Coinsurance")
                                                        {
                                                            val = entry.nextYrVal;
                                                            if (String.IsNullOrEmpty(val))
                                                                val = RuleConstants.NotApplicable;
                                                            fourth = val;
                                                        }
                                                        else if (entry.Prop == "CopayBeginDay" || entry.Prop == "CoinsuranceBeginDay")
                                                        {
                                                            val = entry.nextYrVal;
                                                            if (String.IsNullOrEmpty(val))
                                                                val = RuleConstants.NotApplicable;
                                                            second = val;
                                                        }
                                                        else if (entry.Prop == "CopayEndDay" || entry.Prop == "CoinsuranceEndDay")
                                                        {
                                                            val = entry.nextYrVal;
                                                            if (String.IsNullOrEmpty(val))
                                                                val = RuleConstants.NotApplicable;
                                                            third = val;
                                                        }
                                                    }
                                                    value += string.Format(valFormat, first, second, third, fourth, fifth) + "; ";
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (indx == (count - 1))
                                            {
                                                difference = detailedCompareList[item.Key];
                                                if (null != difference && difference.Count > 0)
                                                {
                                                    first = item.Key;
                                                    foreach (var entry in difference)
                                                    {
                                                        if (entry.Prop == "BenefitPeriod")
                                                        {
                                                            val = entry.previousYrVal;
                                                            if (String.IsNullOrEmpty(val))
                                                                val = RuleConstants.NotApplicable;
                                                            fifth = val;
                                                        }

                                                        if (entry.Prop == "Copay" || entry.Prop == "Coinsurance")
                                                        {
                                                            val = entry.previousYrVal;
                                                            if (String.IsNullOrEmpty(val))
                                                                val = RuleConstants.NotApplicable;
                                                            fourth = val;
                                                        }
                                                        else if (entry.Prop == "CopayBeginDay" || entry.Prop == "CoinsuranceBeginDay")
                                                        {
                                                            val = entry.previousYrVal;
                                                            if (String.IsNullOrEmpty(val))
                                                                val = RuleConstants.NotApplicable;
                                                            second = val;
                                                        }
                                                        else if (entry.Prop == "CopayEndDay" || entry.Prop == "CoinsuranceEndDay")
                                                        {
                                                            val = entry.previousYrVal;
                                                            if (String.IsNullOrEmpty(val))
                                                                val = RuleConstants.NotApplicable;
                                                            third = val;
                                                        }
                                                    }
                                                    value += string.Format(valFormat, first, second, third, fourth, fifth);
                                                }
                                            }
                                            else
                                            {
                                                difference = detailedCompareList[item.Key];
                                                if (null != difference && difference.Count > 0)
                                                {
                                                    first = item.Key;
                                                    foreach (var entry in difference)
                                                    {
                                                        if (entry.Prop == "BenefitPeriod")
                                                        {
                                                            val = entry.previousYrVal;
                                                            if (String.IsNullOrEmpty(val))
                                                                val = RuleConstants.NotApplicable;
                                                            fifth = val;
                                                        }
                                                        if (entry.Prop == "Copay" || entry.Prop == "Coinsurance")
                                                        {
                                                            val = entry.previousYrVal;
                                                            if (String.IsNullOrEmpty(val))
                                                                val = RuleConstants.NotApplicable;
                                                            fourth = val;
                                                        }
                                                        else if (entry.Prop == "CopayBeginDay" || entry.Prop == "CoinsuranceBeginDay")
                                                        {
                                                            val = entry.previousYrVal;
                                                            if (String.IsNullOrEmpty(val))
                                                                val = RuleConstants.NotApplicable;
                                                            second = val;
                                                        }
                                                        else if (entry.Prop == "CopayEndDay" || entry.Prop == "CoinsuranceEndDay")
                                                        {
                                                            val = entry.previousYrVal;
                                                            if (String.IsNullOrEmpty(val))
                                                                val = RuleConstants.NotApplicable;
                                                            third = val;
                                                        }
                                                    }
                                                    value += string.Format(valFormat, first, second, third, fourth, fifth) + "; ";
                                                }
                                            }
                                        }
                                    }
                                    value = GetSlidingServiceFormatedValue(value);
                                }
                            }
                            else
                            {
                                var service = nextSlidingCostShareIntervalList.Where(x => Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty) == benefitCategory1
                                                       && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty) == benefitCategory2)
                                                       && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty) == benefitCategory3)
                                                       && (Convert.ToString(x.SelectToken(RuleConstants.CostShareTiers) ?? String.Empty) == tier)).FirstOrDefault();

                                detailedCompareList = GetNotCoveredServiceSlidingCostShare(networkTier, service, nextIntervalInfo, null, service);

                                #region nextSliding costshare
                                if (null != detailedCompareList && detailedCompareList.Count > 0)
                                {
                                    const string valFormat = "{0}: {1} - {2}, {3}, {4}";
                                    string first = ""; string second = ""; string third = ""; string fourth = ""; string fifth = "";
                                    int count = detailedCompareList.Count;
                                    List<Variance> difference = new List<Variance>();
                                    int indx = -1;
                                    string val = String.Empty;
                                    foreach (var item in detailedCompareList)
                                    {
                                        indx = indx + 1;
                                        if (indx == (count - 1))
                                        {
                                            difference = detailedCompareList[item.Key];
                                            if (null != difference && difference.Count > 0)
                                            {
                                                first = item.Key;
                                                foreach (var entry in difference)
                                                {
                                                    if (entry.Prop == "BenefitPeriod")
                                                    {
                                                        val = entry.nextYrVal;
                                                        if (String.IsNullOrEmpty(val))
                                                            val = RuleConstants.NotApplicable;
                                                        fifth = val;
                                                    }

                                                    if (entry.Prop == "Copay" || entry.Prop == "Coinsurance")
                                                    {
                                                        val = entry.nextYrVal;
                                                        if (String.IsNullOrEmpty(val))
                                                            val = RuleConstants.NotApplicable;
                                                        fourth = val;
                                                    }
                                                    else if (entry.Prop == "CopayBeginDay" || entry.Prop == "CoinsuranceBeginDay")
                                                    {
                                                        val = entry.nextYrVal;
                                                        if (String.IsNullOrEmpty(val))
                                                            val = RuleConstants.NotApplicable;
                                                        second = val;
                                                    }
                                                    else if (entry.Prop == "CopayEndDay" || entry.Prop == "CoinsuranceEndDay")
                                                    {
                                                        val = entry.nextYrVal;
                                                        if (String.IsNullOrEmpty(val))
                                                            val = RuleConstants.NotApplicable;
                                                        third = val;
                                                    }
                                                }
                                                value += string.Format(valFormat, first, second, third, fourth, fifth);
                                            }
                                        }
                                        else
                                        {
                                            difference = detailedCompareList[item.Key];
                                            if (null != difference && difference.Count > 0)
                                            {
                                                first = item.Key;
                                                foreach (var entry in difference)
                                                {
                                                    if (entry.Prop == "BenefitPeriod")
                                                    {
                                                        val = entry.nextYrVal;
                                                        if (String.IsNullOrEmpty(val))
                                                            val = RuleConstants.NotApplicable;
                                                        fifth = val;
                                                    }

                                                    if (entry.Prop == "Copay" || entry.Prop == "Coinsurance")
                                                    {
                                                        val = entry.nextYrVal;
                                                        if (String.IsNullOrEmpty(val))
                                                            val = RuleConstants.NotApplicable;
                                                        fourth = val;
                                                    }
                                                    else if (entry.Prop == "CopayBeginDay" || entry.Prop == "CoinsuranceBeginDay")
                                                    {
                                                        val = entry.nextYrVal;
                                                        if (String.IsNullOrEmpty(val))
                                                            val = RuleConstants.NotApplicable;
                                                        second = val;
                                                    }
                                                    else if (entry.Prop == "CopayEndDay" || entry.Prop == "CoinsuranceEndDay")
                                                    {
                                                        val = entry.nextYrVal;
                                                        if (String.IsNullOrEmpty(val))
                                                            val = RuleConstants.NotApplicable;
                                                        third = val;
                                                    }
                                                }
                                                value += string.Format(valFormat, first, second, third, fourth, fifth) + "; ";
                                            }
                                        }
                                    }
                                    value = GetSlidingServiceFormatedValue(value);
                                }
                                #endregion nextSliding costshare costshare
                            }
                        }
                        else
                        {
                            value = RuleConstants.NotCovered;
                        }
                    }
                    #endregion Next Year
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
            }
            return value;
        }
        private Dictionary<string, List<Variance>> GetNetworkServiceSlidingCostShare(JToken networkTier, JToken previousYrInterval, JToken nextYrInterval, List<JToken> prevIntervalInfo, List<JToken> nextIntervalInfo, bool isDefaultService)
        {
            string value = String.Empty;
            Dictionary<string, List<Variance>> detailedCompareList = new Dictionary<string, List<Variance>>();
            try
            {
                if (null != networkTier && null != previousYrInterval && null != nextYrInterval)
                {
                    string tierName = Convert.ToString(networkTier[RuleConstants.CostShareTiers] ?? String.Empty);
                    if (!String.IsNullOrEmpty(tierName))
                    {
                        detailedCompareList = DetailedCompareSlidingCostShare(previousYrInterval, nextYrInterval, prevIntervalInfo, nextIntervalInfo, networkTier, isDefaultService);
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
            }
            return detailedCompareList;
        }
        public Dictionary<string, List<Variance>> DetailedCompareSlidingCostShare(JToken previousYrInterval, JToken nextYrInterval, List<JToken> prevYrIntervalInfo, List<JToken> nextYrIntervalInfo, JToken networkTier, bool isDefaultService)
        {
            List<Variance> detailedCompareList = new List<Variance>();
            Dictionary<string, List<Variance>> intervalDiffDict = new Dictionary<string, List<Variance>>();
            if (null != prevYrIntervalInfo && null != nextYrIntervalInfo)
            {
                Dictionary<string, string> keys = new Dictionary<string, string>();
                keys.Add(RuleConstants.BenefitCategory1, RuleConstants.BenefitCategory1);
                keys.Add(RuleConstants.BenefitCategory2, RuleConstants.BenefitCategory2);
                keys.Add(RuleConstants.BenefitCategory3, RuleConstants.BenefitCategory3);
                keys.Add(RuleConstants.CostShareTiers, RuleConstants.CostShareTiers);
                keys.Add(RuleConstants.Interval, RuleConstants.Interval);

                List<JToken> prevYrDistictIntervalInfo = new List<JToken>();
                List<JToken> nextYrDistictIntervalInfo = new List<JToken>();

                CollectionExecutionComparer distinctProcessor = new CollectionExecutionComparer(prevYrIntervalInfo, keys);
                prevYrDistictIntervalInfo = distinctProcessor.Distinct();

                distinctProcessor = new CollectionExecutionComparer(nextYrIntervalInfo, keys);
                nextYrDistictIntervalInfo = distinctProcessor.Distinct();

                int prevCount = prevYrDistictIntervalInfo.Count();
                int nextCount = nextYrDistictIntervalInfo.Count();

                int maxInterval = 0;
                if (prevCount > nextCount)
                    maxInterval = prevCount;
                else
                    maxInterval = nextCount;
                if (null != networkTier)
                {
                    string tierName = Convert.ToString(networkTier[RuleConstants.CostShareTiers] ?? String.Empty);
                    for (int a = 0; a < maxInterval; a++)
                    {
                        if (!String.IsNullOrEmpty(tierName))
                        {
                            //var prevIQMedicareNetworkList = (a >= prevYrIntervalInfo.Count ? null : prevYrIntervalInfo[a].SelectToken("IQMedicareNetWorkList"));
                            //var nextIQMedicareNetworkList = (a >= nextYrIntervalInfo.Count ? null : nextYrIntervalInfo[a].SelectToken("IQMedicareNetWorkList"));
                            JObject previousYrValueToken = new JObject(); JObject nextYrValueToken = new JObject();

                            List<JToken> prevIntervalCopayCoinsList = prevYrIntervalInfo.FindAll(x => Convert.ToString(x.SelectToken(RuleConstants.Tier)) == tierName
                                                                                        && Convert.ToString(x.SelectToken(RuleConstants.Interval)) == Convert.ToString((a)));

                            List<JToken> nextIntervalCopayCoinsList = nextYrIntervalInfo.FindAll(x => Convert.ToString(x.SelectToken(RuleConstants.Tier)) == tierName
                                                                                    && Convert.ToString(x.SelectToken(RuleConstants.Interval)) == Convert.ToString((a)));

                            if (prevIntervalCopayCoinsList.Count == 0)
                                previousYrValueToken = null;

                            if (nextIntervalCopayCoinsList.Count == 0)
                                nextYrValueToken = null;

                            if (null != prevIntervalCopayCoinsList)
                            {
                                string tokenName = String.Empty; string costShareType = String.Empty;
                                foreach (var item in prevIntervalCopayCoinsList)
                                {
                                    costShareType = Convert.ToString(item.SelectToken("CostShareType") ?? String.Empty);
                                    List<JToken> tokenList = item.ToList();
                                    if (null != tokenList)
                                    {
                                        string prop = String.Empty;
                                        foreach (JToken element in tokenList)
                                        {
                                            tokenName = ((JProperty)element).Name;
                                            if (tokenName != RuleConstants.BenefitCategory1 && tokenName != RuleConstants.BenefitCategory2
                                             && tokenName != RuleConstants.BenefitCategory3 && tokenName != RuleConstants.Interval
                                             && tokenName != RuleConstants.Tier && tokenName != RuleConstants.CostShareType && tokenName != RuleConstants.RowIDProperty)
                                            {
                                                // If token is Amount i.e costShareType = "Copay/Coinsurance+Amount" but we have code as Copay/Coinsurance so set token name as blank
                                                if (tokenName == "Amount")
                                                    prop = String.Empty;
                                                else
                                                    prop = tokenName;

                                                previousYrValueToken[costShareType + prop] = Convert.ToString(item.SelectToken(tokenName) ?? String.Empty);
                                            }
                                        }
                                    }
                                }
                            }
                            if (null != nextIntervalCopayCoinsList)
                            {
                                string tokenName = String.Empty; string costShareType = String.Empty;
                                foreach (var item in nextIntervalCopayCoinsList)
                                {
                                    costShareType = Convert.ToString(item.SelectToken("CostShareType") ?? String.Empty);
                                    List<JToken> tokenList = item.ToList();
                                    if (null != tokenList)
                                    {
                                        string prop = String.Empty;
                                        foreach (JToken element in tokenList)
                                        {
                                            tokenName = ((JProperty)element).Name;
                                            if (tokenName != RuleConstants.BenefitCategory1 && tokenName != RuleConstants.BenefitCategory2
                                             && tokenName != RuleConstants.BenefitCategory3 && tokenName != RuleConstants.Interval
                                             && tokenName != RuleConstants.Tier && tokenName != RuleConstants.CostShareType && tokenName != RuleConstants.RowIDProperty)
                                            {
                                                // If token is Amount i.e costShareType = "Copay/Coinsurance+Amount" but we have code as Copay/Coinsurance so set token name as blank
                                                if (tokenName == "Amount")
                                                    prop = String.Empty;
                                                else
                                                    prop = tokenName;

                                                nextYrValueToken[costShareType + prop] = Convert.ToString(item.SelectToken(tokenName) ?? String.Empty);
                                            }
                                        }
                                    }
                                }
                            }
                            //var previousYrValueToken = (null != prevIQMedicareNetworkList ? prevIQMedicareNetworkList.Where(x => (x.SelectToken(RuleConstants.CostShareTiers) == null ? String.Empty : x.SelectToken(RuleConstants.CostShareTiers).ToString()) == tierName).FirstOrDefault() : null);
                            //var nextYrValueToken = (null != nextIQMedicareNetworkList ? nextIQMedicareNetworkList.Where(x => (x.SelectToken(RuleConstants.CostShareTiers) == null ? String.Empty : x.SelectToken(RuleConstants.CostShareTiers).ToString()) == tierName).FirstOrDefault() : null);
                            string key = "Interval " + (a);
                            if (null != previousYrValueToken && null != nextYrValueToken)
                            {
                                detailedCompareList = CompareSlidingCostShare(previousYrInterval, nextYrInterval, previousYrValueToken, nextYrValueToken, isDefaultService);
                                // If detailedCompareList has diffrence in any value Copay/Coins then add begin-end day
                                // Check if difference is in copay add copay bein-end day, if difference is in coinsurance add coinsurance bein-end day
                                if (null != detailedCompareList && detailedCompareList.Count > 0)
                                {
                                    Variance v = new Variance();
                                    string nextVal = string.Empty;
                                    string previousVal = string.Empty;
                                    if (detailedCompareList[0].Prop == "Copay")
                                    {
                                        v.Prop = "CopayBeginDay";
                                        previousVal = Convert.ToString(previousYrValueToken.SelectToken("CopayBeginDay") ?? String.Empty);
                                        nextVal = Convert.ToString(nextYrValueToken.SelectToken("CopayBeginDay") ?? String.Empty);
                                        v.nextYrVal = nextVal;
                                        v.previousYrVal = previousVal;
                                        detailedCompareList.Add(v);

                                        v = new Variance();
                                        v.Prop = "CopayEndDay";
                                        previousVal = Convert.ToString(previousYrValueToken.SelectToken("CopayEndDay") ?? String.Empty);
                                        nextVal = Convert.ToString(nextYrValueToken.SelectToken("CopayEndDay") ?? String.Empty);
                                        v.nextYrVal = nextVal;
                                        v.previousYrVal = previousVal;
                                        detailedCompareList.Add(v);
                                    }
                                    else if (detailedCompareList[0].Prop == "Coinsurance")
                                    {
                                        v.Prop = "CoinsuranceBeginDay";
                                        previousVal = Convert.ToString(previousYrValueToken.SelectToken("CoinsuranceBeginDay") ?? String.Empty);
                                        nextVal = Convert.ToString(nextYrValueToken.SelectToken("CoinsuranceBeginDay") ?? String.Empty);
                                        v.nextYrVal = nextVal;
                                        v.previousYrVal = previousVal;
                                        detailedCompareList.Add(v);

                                        v = new Variance();
                                        v.Prop = "CoinsuranceEndDay";
                                        previousVal = Convert.ToString(previousYrValueToken.SelectToken("CoinsuranceEndDay") ?? String.Empty);
                                        nextVal = Convert.ToString(nextYrValueToken.SelectToken("CoinsuranceEndDay") ?? String.Empty);
                                        v.nextYrVal = nextVal;
                                        v.previousYrVal = previousVal;
                                        detailedCompareList.Add(v);
                                    }
                                }
                                intervalDiffDict.Add(key, detailedCompareList);
                            }
                            else if (null == previousYrValueToken && null != nextYrValueToken)
                            {
                                // Add next year all values 
                                detailedCompareList = GetNonCoveredIntervalValues(previousYrInterval, nextYrInterval, nextYrValueToken, true);
                                if (null != detailedCompareList && detailedCompareList.Count > 0)
                                {
                                    Variance v = new Variance();
                                    string nextVal = string.Empty;
                                    string previousVal = string.Empty;
                                    if (detailedCompareList[0].Prop == "Copay")
                                    {
                                        v.Prop = "CopayBeginDay";
                                        previousVal = RuleConstants.NotApplicable;
                                        nextVal = Convert.ToString(nextYrValueToken.SelectToken("CopayBeginDay") ?? String.Empty);
                                        v.nextYrVal = nextVal;
                                        v.previousYrVal = previousVal;
                                        detailedCompareList.Add(v);

                                        v = new Variance();
                                        v.Prop = "CopayEndDay";
                                        previousVal = RuleConstants.NotApplicable;
                                        nextVal = Convert.ToString(nextYrValueToken.SelectToken("CopayEndDay") ?? String.Empty);
                                        v.nextYrVal = nextVal;
                                        v.previousYrVal = previousVal;
                                        detailedCompareList.Add(v);
                                    }
                                    else if (detailedCompareList[0].Prop == "Coinsurance")
                                    {
                                        v.Prop = "CoinsuranceBeginDay";
                                        previousVal = RuleConstants.NotApplicable;
                                        nextVal = Convert.ToString(nextYrValueToken.SelectToken("CoinsuranceBeginDay") ?? String.Empty);
                                        v.nextYrVal = nextVal;
                                        v.previousYrVal = previousVal;
                                        detailedCompareList.Add(v);

                                        v = new Variance();
                                        v.Prop = "CoinsuranceEndDay";
                                        previousVal = RuleConstants.NotApplicable;
                                        nextVal = Convert.ToString(nextYrValueToken.SelectToken("CoinsuranceEndDay") ?? String.Empty);
                                        v.nextYrVal = nextVal;
                                        v.previousYrVal = previousVal;
                                        detailedCompareList.Add(v);
                                    }
                                }
                                intervalDiffDict.Add(key, detailedCompareList);
                            }
                            else if (null == nextYrValueToken && null != previousYrValueToken)
                            {
                                // Add previous year all values 
                                detailedCompareList = GetNonCoveredIntervalValues(previousYrInterval, nextYrInterval, previousYrValueToken, false);
                                if (null != detailedCompareList && detailedCompareList.Count > 0)
                                {
                                    Variance v = new Variance();
                                    string nextVal = string.Empty;
                                    string previousVal = string.Empty;
                                    if (detailedCompareList[0].Prop == "Copay")
                                    {
                                        v.Prop = "CopayBeginDay";
                                        previousVal = Convert.ToString(previousYrValueToken.SelectToken("CopayBeginDay") ?? String.Empty);
                                        nextVal = RuleConstants.NotApplicable;
                                        v.nextYrVal = nextVal;
                                        v.previousYrVal = previousVal;
                                        detailedCompareList.Add(v);

                                        v = new Variance();
                                        v.Prop = "CopayEndDay";
                                        previousVal = Convert.ToString(previousYrValueToken.SelectToken("CopayEndDay") ?? String.Empty);
                                        nextVal = RuleConstants.NotApplicable;
                                        v.nextYrVal = nextVal;
                                        v.previousYrVal = previousVal;
                                        detailedCompareList.Add(v);
                                    }
                                    else if (detailedCompareList[0].Prop == "Coinsurance")
                                    {
                                        v.Prop = "CoinsuranceBeginDay";
                                        previousVal = Convert.ToString(previousYrValueToken.SelectToken("CoinsuranceBeginDay") ?? String.Empty);
                                        nextVal = RuleConstants.NotApplicable;
                                        v.nextYrVal = nextVal;
                                        v.previousYrVal = previousVal;
                                        detailedCompareList.Add(v);

                                        v = new Variance();
                                        v.Prop = "CoinsuranceEndDay";
                                        previousVal = Convert.ToString(previousYrValueToken.SelectToken("CoinsuranceEndDay") ?? String.Empty);
                                        nextVal = RuleConstants.NotApplicable;
                                        v.nextYrVal = nextVal;
                                        v.previousYrVal = previousVal;
                                        detailedCompareList.Add(v);
                                    }
                                }
                                intervalDiffDict.Add(key, detailedCompareList);
                            }
                        }
                    }
                }
            }
            return intervalDiffDict;
        }
        private List<Variance> CompareSlidingCostShare(JToken previousYrInterval, JToken nextYrInterval, JToken previousYrValueToken, JToken nextYrValueToken, bool isDefaultService)
        {
            List<Variance> variances = new List<Variance>();
            if (null != previousYrValueToken && null != nextYrValueToken)
            {
                List<JToken> nextYrTierTokens = nextYrValueToken.ToList();
                List<JToken> prevYrTierTokens = previousYrValueToken.ToList();

                if (null != nextYrTierTokens && null != prevYrTierTokens)
                {
                    int tierTokensCount = nextYrTierTokens.Count;
                    Variance v = new Variance();
                    string tokenName = string.Empty;
                    string previousVal = string.Empty;
                    string nextVal = string.Empty;
                    string symbol = string.Empty;
                    bool isNumeric = false;
                    for (int j = 0; j < tierTokensCount; j++)
                    {
                        // Check if anything is added in variance list, if yes no need to add other as we are showing only one value
                        // either copay or coinsurace, so if anyone costhare is addded skip adding another.
                        if (variances.Count == 0)
                        {
                            symbol = string.Empty;
                            v = new Variance();
                            tokenName = ((JProperty)nextYrTierTokens[j]).Name;
                            if (tokenName == "Copay" || tokenName == "Coinsurance")
                            {
                                nextVal = Convert.ToString(((JProperty)nextYrTierTokens[j]).Value ?? String.Empty);
                                previousVal = Convert.ToString(((JProperty)prevYrTierTokens[j]).Value ?? String.Empty);

                                if (String.Equals(nextVal, "NULL") || String.Equals(nextVal, "null")) nextVal = String.Empty;
                                if (String.Equals(previousVal, "NULL") || String.Equals(previousVal, "null")) previousVal = String.Empty;

                                int n;
                                string val = previousVal;
                                if (previousVal.Contains(",")) { val = previousVal.Replace(",", ""); }
                                if (previousVal.Contains(".")) { val = previousVal.Replace(".", ""); }

                                isNumeric = int.TryParse(val, out n);
                                if (isNumeric && n >= 0)
                                {
                                    if (tokenName == "Coinsurance")
                                        previousVal = previousVal + RuleConstants.Percent;
                                    else
                                        previousVal = RuleConstants.Dollar + previousVal;
                                }
                                v.Prop = tokenName;
                                v.previousYrVal = symbol + previousVal;

                                symbol = string.Empty;
                                val = nextVal;
                                if (nextVal.Contains(",")) { val = nextVal.Replace(",", ""); }
                                if (nextVal.Contains(".")) { val = nextVal.Replace(".", ""); }

                                isNumeric = int.TryParse(val, out n);
                                if (isNumeric && n >= 0)
                                {
                                    if (tokenName == "Coinsurance")
                                        nextVal = nextVal + RuleConstants.Percent;
                                    else
                                        nextVal = RuleConstants.Dollar + nextVal;
                                }
                                v.nextYrVal = symbol + nextVal;

                                if (isDefaultService)
                                {
                                    if (tokenName != RuleConstants.CostShareTiers)
                                        variances.Add(v);
                                }
                                else
                                {
                                    if (!v.nextYrVal.Equals(v.previousYrVal))
                                        variances.Add(v);
                                }
                            }
                        }
                    }
                    // Add benefit period if variances has some value
                    if (null != variances && variances.Count > 0)
                    {
                        string prevBenefitPeriod = String.Empty; string nextBenefitPeriod = String.Empty;
                        if (null != previousYrInterval)
                            prevBenefitPeriod = Convert.ToString(previousYrInterval.SelectToken("BenefitPeriod") ?? String.Empty);
                        if (null != nextYrInterval)
                            nextBenefitPeriod = Convert.ToString(nextYrInterval.SelectToken("BenefitPeriod") ?? String.Empty);
                        v = new Variance();
                        v.Prop = "BenefitPeriod";
                        v.nextYrVal = nextBenefitPeriod;
                        v.previousYrVal = prevBenefitPeriod;
                        variances.Add(v);
                    }
                }
            }
            return variances;
        }
        private List<Variance> GetNonCoveredIntervalValues(JToken previousYrInterval, JToken nextYrInterval, JToken valueToken, bool isNextYrToken)
        {
            List<Variance> variances = new List<Variance>();
            if (null != valueToken)
            {
                List<JToken> valueTokens = valueToken.ToList();

                if (null != valueTokens)
                {
                    int tierTokensCount = valueTokens.Count;
                    Variance v = new Variance();
                    string tokenName = string.Empty;
                    string previousVal = string.Empty;
                    string nextVal = string.Empty;
                    string symbol = string.Empty;
                    bool isNumeric = false;
                    for (int j = 0; j < tierTokensCount; j++)
                    {
                        // Check if anything is added in variance list, if yes no need to add other as we are showing only one value
                        // either copay or coinsurace, so if anyone costhare is addded skip adding another.
                        if (variances.Count == 0)
                        {
                            symbol = string.Empty;
                            v = new Variance();
                            tokenName = ((JProperty)valueTokens[j]).Name;
                            if (tokenName == "Copay" || tokenName == "Coinsurance")
                            {
                                if (isNextYrToken)
                                {
                                    previousVal = RuleConstants.NotApplicable;
                                    nextVal = Convert.ToString(((JProperty)valueTokens[j]).Value ?? String.Empty);
                                }
                                else
                                {
                                    previousVal = Convert.ToString(((JProperty)valueTokens[j]).Value ?? String.Empty);
                                    nextVal = RuleConstants.NotApplicable;
                                }
                                // Check if NULL / null in prev/next values
                                if (String.Equals(nextVal, "NULL") || String.Equals(nextVal, "null")) nextVal = String.Empty;
                                if (String.Equals(previousVal, "NULL") || String.Equals(previousVal, "null")) previousVal = String.Empty;

                                int n;
                                string val = previousVal;
                                if (previousVal.Contains(",")) { val = previousVal.Replace(",", ""); }
                                if (previousVal.Contains(".")) { val = previousVal.Replace(".", ""); }

                                isNumeric = int.TryParse(val, out n);
                                if (isNumeric && n >= 0)
                                {
                                    if (tokenName == "Coinsurance")
                                        previousVal = previousVal + RuleConstants.Percent;
                                    else
                                        previousVal = RuleConstants.Dollar + previousVal;
                                }
                                v.Prop = tokenName;
                                v.previousYrVal = symbol + previousVal;

                                symbol = string.Empty;
                                val = nextVal;
                                if (nextVal.Contains(",")) { val = nextVal.Replace(",", ""); }
                                if (nextVal.Contains(".")) { val = nextVal.Replace(".", ""); }

                                isNumeric = int.TryParse(val, out n);
                                if (isNumeric && n >= 0)
                                {
                                    if (tokenName == "Coinsurance")
                                        nextVal = nextVal + RuleConstants.Percent;
                                    else
                                        nextVal = RuleConstants.Dollar + nextVal;
                                }
                                v.nextYrVal = symbol + nextVal;
                                //if (!v.nextYrVal.Equals(v.previousYrVal))
                                if (isNextYrToken && !String.IsNullOrEmpty(v.nextYrVal))
                                    variances.Add(v);
                                else if (!isNextYrToken && !String.IsNullOrEmpty(v.previousYrVal))
                                    variances.Add(v);
                            }
                        }
                    }
                    // Add benefit period if variances has some value
                    if (null != variances && variances.Count > 0)
                    {
                        string prevBenefitPeriod = String.Empty; string nextBenefitPeriod = String.Empty;
                        if (null != previousYrInterval)
                            prevBenefitPeriod = Convert.ToString(previousYrInterval.SelectToken("BenefitPeriod") ?? String.Empty);
                        if (null != nextYrInterval)
                            nextBenefitPeriod = Convert.ToString(nextYrInterval.SelectToken("BenefitPeriod") ?? String.Empty);
                        v = new Variance();
                        v.Prop = "BenefitPeriod";
                        if (isNextYrToken)
                        {
                            v.nextYrVal = nextBenefitPeriod;
                            v.previousYrVal = RuleConstants.NotApplicable;
                        }
                        else
                        {
                            v.nextYrVal = RuleConstants.NotApplicable;
                            v.previousYrVal = prevBenefitPeriod;
                        }
                        variances.Add(v);
                    }
                }
            }
            return variances;
        }
        private Dictionary<string, List<Variance>> GetNotCoveredServiceSlidingCostShare(JToken networkTier, JToken sourceService, List<JToken> intervalInfo, JToken previousYrInterval, JToken nextYrInterval)
        {
            List<Variance> detailedCompareList = new List<Variance>();
            Dictionary<string, List<Variance>> intervalDiffDict = new Dictionary<string, List<Variance>>();
            if (null != sourceService && null != intervalInfo)
            {
                int intervalCount = intervalInfo.Count();
                if (null != networkTier)
                {
                    string tierName = Convert.ToString(networkTier[RuleConstants.CostShareTiers] ?? String.Empty);
                    for (int a = 0; a < intervalCount; a++)
                    {
                        if (!String.IsNullOrEmpty(tierName))
                        {
                            List<JToken> prevIntervalCopayCoinsList = intervalInfo.FindAll(x => Convert.ToString(x.SelectToken(RuleConstants.Tier)) == tierName
                                                                          && Convert.ToString(x.SelectToken(RuleConstants.Interval)) == Convert.ToString((a)));

                            JObject valueToken = new JObject();
                            if (null != prevIntervalCopayCoinsList)
                            {
                                string tokenName = String.Empty; string costShareType = String.Empty;
                                foreach (var item in prevIntervalCopayCoinsList)
                                {
                                    costShareType = Convert.ToString(item.SelectToken("CostShareType") ?? String.Empty);
                                    List<JToken> tokenList = item.ToList();
                                    if (null != tokenList)
                                    {
                                        string prop = String.Empty;
                                        foreach (JToken element in tokenList)
                                        {
                                            tokenName = ((JProperty)element).Name;
                                            if (tokenName != RuleConstants.BenefitCategory1 && tokenName != RuleConstants.BenefitCategory2
                                             && tokenName != RuleConstants.BenefitCategory3 && tokenName != RuleConstants.Interval
                                             && tokenName != RuleConstants.Tier && tokenName != RuleConstants.CostShareType && tokenName != RuleConstants.RowIDProperty)
                                            {
                                                // If token is Amount i.e costShareType = "Copay/Coinsurance+Amount" but we have code as Copay/Coinsurance so set token name as blank
                                                if (tokenName == "Amount")
                                                    prop = String.Empty;
                                                else
                                                    prop = tokenName;

                                                valueToken[costShareType + prop] = Convert.ToString(item.SelectToken(tokenName) ?? String.Empty);
                                            }
                                        }
                                    }
                                }
                            }
                            string key = "Interval " + (a);
                            if (null != valueToken)
                            {
                                // Add next year all values 
                                detailedCompareList = GetNonCoveredIntervalValues(previousYrInterval, nextYrInterval, valueToken, true);
                                if (null != detailedCompareList && detailedCompareList.Count > 0)
                                {
                                    Variance v = new Variance();
                                    string nextVal = string.Empty;
                                    string previousVal = string.Empty;
                                    if (detailedCompareList[0].Prop == "Copay")
                                    {
                                        v.Prop = "CopayBeginDay";
                                        previousVal = Convert.ToString(valueToken.SelectToken("CopayBeginDay") ?? String.Empty);
                                        nextVal = Convert.ToString(valueToken.SelectToken("CopayBeginDay") ?? String.Empty);
                                        v.nextYrVal = nextVal;
                                        v.previousYrVal = previousVal;
                                        detailedCompareList.Add(v);

                                        v = new Variance();
                                        v.Prop = "CopayEndDay";
                                        previousVal = Convert.ToString(valueToken.SelectToken("CopayEndDay") ?? String.Empty);
                                        nextVal = Convert.ToString(valueToken.SelectToken("CopayEndDay") ?? String.Empty);
                                        v.nextYrVal = nextVal;
                                        v.previousYrVal = previousVal;
                                        detailedCompareList.Add(v);
                                    }
                                    else if (detailedCompareList[0].Prop == "Coinsurance")
                                    {
                                        v.Prop = "CoinsuranceBeginDay";
                                        previousVal = Convert.ToString(valueToken.SelectToken("CoinsuranceBeginDay") ?? String.Empty);
                                        nextVal = Convert.ToString(valueToken.SelectToken("CoinsuranceBeginDay") ?? String.Empty);
                                        v.nextYrVal = nextVal;
                                        v.previousYrVal = previousVal;
                                        detailedCompareList.Add(v);

                                        v = new Variance();
                                        v.Prop = "CoinsuranceEndDay";
                                        previousVal = Convert.ToString(valueToken.SelectToken("CoinsuranceEndDay") ?? String.Empty);
                                        nextVal = Convert.ToString(valueToken.SelectToken("CoinsuranceEndDay") ?? String.Empty);
                                        v.nextYrVal = nextVal;
                                        v.previousYrVal = previousVal;
                                        detailedCompareList.Add(v);
                                    }
                                }
                                intervalDiffDict.Add(key, detailedCompareList);
                            }
                        }
                    }
                }
            }
            return intervalDiffDict;
        }
        private List<BenefitsCompare> CompareServices(List<BenefitsCompare> slidingCostShareServices, List<BenefitsCompare> comparisionBenefitServices)
        {
            try
            {
                if (null != slidingCostShareServices && null != comparisionBenefitServices)
                {
                    string pTagPrefix = "<p style=\"font-size:   12pt; font-family: 'Times New Roman', serif; margin-top: 6pt;\">";
                    string pTagPostFix = "</p>";

                    foreach (var item in slidingCostShareServices)
                    {
                        // Check if service exist in comparisonBenefitServices
                        var brgService = comparisionBenefitServices.Where(x => x.BenefitCategory1 == item.BenefitCategory1
                                                    && x.BenefitCategory2 == item.BenefitCategory2
                                                    && x.BenefitCategory3 == item.BenefitCategory3
                                                    && x.CostShareTiers == item.CostShareTiers).FirstOrDefault();
                        if (null != brgService)
                        {
                            // Service exist in both need to add \r\n in between each years Previous and NextYear values for each network.
                            var brgMedicareNetworkList = brgService;
                            var slidingMedicareNetworkList = item;
                            if (null != brgMedicareNetworkList)
                            {
                                brgService.NextYear = GetPlanYearValue(slidingMedicareNetworkList, brgMedicareNetworkList.CostShareTiers, true) + pTagPrefix + brgMedicareNetworkList.NextYear + pTagPostFix;
                                brgService.ThisYear = GetPlanYearValue(slidingMedicareNetworkList, brgMedicareNetworkList.CostShareTiers, false) + pTagPrefix + brgMedicareNetworkList.ThisYear + pTagPostFix;
                            }
                        }
                        else
                        {
                            // Add \r\n after sliding costshare previous-next year values for each network
                            var medicareNetworkList = item;
                            if (null != medicareNetworkList)
                            {
                                medicareNetworkList.NextYear = pTagPrefix + medicareNetworkList.NextYear + pTagPostFix;
                                medicareNetworkList.ThisYear = pTagPrefix + medicareNetworkList.ThisYear + pTagPostFix;
                                comparisionBenefitServices.Add(item);
                            }
                        }
                    }
                    // Append \r\n to all other services that are not in compaarision as we have format slidingCostshare \r\n BRG costshare
                    foreach (var item in comparisionBenefitServices)
                    {
                        // Check if service exist in slidingCostShareServices
                        var slidingService = slidingCostShareServices.Exists(x => x.BenefitCategory1 == item.BenefitCategory1
                                                    && x.BenefitCategory2 == item.BenefitCategory2
                                                    && x.BenefitCategory3 == item.BenefitCategory3
                                                    && x.CostShareTiers == item.CostShareTiers);
                        if (!slidingService)
                        {
                            // Service exist in both need to add \r\n in between each years Previous and NextYear values for each network.
                            var brgMedicareNetworkList = item;
                            if (null != brgMedicareNetworkList)
                            {
                                brgMedicareNetworkList.NextYear = pTagPrefix + brgMedicareNetworkList.NextYear + pTagPostFix;
                                brgMedicareNetworkList.ThisYear = pTagPrefix + brgMedicareNetworkList.ThisYear + pTagPostFix;
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
            }
            return comparisionBenefitServices;
        }

        private string GetPlanYearValue(BenefitsCompare plan, string tierName, bool isNextYr)
        {
            string val = String.Empty;
            if (!String.IsNullOrEmpty(tierName))
            {
                var token = plan;
                if (null != token)
                {
                    if (isNextYr)
                        val = token.NextYear;
                    else
                        val = token.ThisYear;
                }
            }
            return val;
        }
        private List<BenefitsCompare> AddRowIDProperty(List<BenefitsCompare> benefitsCompareServices)
        {
            if (null != benefitsCompareServices)
            {
                string pTagPrefix = "<p style=\"font-size:   12pt; font-family: 'Times New Roman', serif; margin-top: 6pt;\">";
                string pTagPrefixANOCBenefitName = "<p style=\"font-size:   12pt; font-family: 'Times New Roman', serif; margin-top: 6pt;\">";
                string pTagPostFix = "</p>";
                string checkPtag = "<p style=\"font-size:   12pt; ";
                int cnt = benefitsCompareServices.Count;
                for (int idx = 0; idx < cnt; idx++)
                {
                    benefitsCompareServices[idx].RowIDProperty = idx;
                    benefitsCompareServices[idx].ThisYear = benefitsCompareServices[idx].ThisYear.StartsWith(checkPtag) ? benefitsCompareServices[idx].ThisYear : pTagPrefix + benefitsCompareServices[idx].ThisYear + pTagPostFix;
                    benefitsCompareServices[idx].NextYear = benefitsCompareServices[idx].NextYear.StartsWith(checkPtag) ? benefitsCompareServices[idx].NextYear : pTagPrefix + benefitsCompareServices[idx].NextYear + pTagPostFix;
                    benefitsCompareServices[idx].ANOCBenefitName = benefitsCompareServices[idx].ANOCBenefitName.StartsWith(checkPtag) ? benefitsCompareServices[idx].ANOCBenefitName : pTagPrefixANOCBenefitName + benefitsCompareServices[idx].ANOCBenefitName + pTagPostFix;
                }
            }
            return benefitsCompareServices;
        }

        private string GetAdditionalCostShareValues(string benefitCategory1, string benefitCategory2, string benefitCategory3, string tier, List<JToken> previousYearTokenList, List<JToken> nextYearTokenList, JToken networkTier, bool isNextYear, bool isDefaultService)
        {
            bool previousYearExist = false;
            bool nextYearExist = false;
            string value = String.Empty;
            List<Variance> detailedCompareList = new List<Variance>();
            try
            {
                if (null != previousYearTokenList && null != nextYearTokenList)
                {
                    #region Previous Year
                    if (!isNextYear)
                    {
                        previousYearExist = previousYearTokenList.Exists(x => Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty) == benefitCategory1
                                                       && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty) == benefitCategory2)
                                                       && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty) == benefitCategory3)
                                                       && (Convert.ToString(x.SelectToken(RuleConstants.CostShareTiers) ?? String.Empty) == tier));
                        if (previousYearExist)
                        {
                            nextYearExist = nextYearTokenList.Exists(x => Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty) == benefitCategory1
                                                      && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty) == benefitCategory2)
                                                      && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty) == benefitCategory3)
                                                      && (Convert.ToString(x.SelectToken(RuleConstants.CostShareTiers) ?? String.Empty) == tier));

                            #region nextYearExist
                            if (nextYearExist)
                            {
                                var previousYrService = previousYearTokenList.Where(x => Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty) == benefitCategory1
                                                       && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty) == benefitCategory2)
                                                       && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty) == benefitCategory3)
                                                       && (Convert.ToString(x.SelectToken(RuleConstants.CostShareTiers) ?? String.Empty) == tier)).FirstOrDefault();

                                var nextYrService = nextYearTokenList.Where(x => Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty) == benefitCategory1
                                                     && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty) == benefitCategory2)
                                                     && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty) == benefitCategory3)
                                                     && (Convert.ToString(x.SelectToken(RuleConstants.CostShareTiers) ?? String.Empty) == tier)).FirstOrDefault();

                                detailedCompareList = GetAdditionalServiceCostShare(networkTier, previousYrService, nextYrService, isDefaultService);
                                if (null != detailedCompareList)
                                {
                                    int count = detailedCompareList.Count;
                                    string val = string.Empty;
                                    for (int a = 0; a < count; a++)
                                    {
                                        if (isNextYear)
                                        {
                                            if (a == (count - 1))
                                            {
                                                val = detailedCompareList[a].nextYrVal;
                                                if (String.IsNullOrEmpty(val))
                                                    val = RuleConstants.NotApplicable;
                                                value += val + "-" + GetFormatedTokenName(detailedCompareList[a].Prop);
                                            }
                                            else
                                            {
                                                val = detailedCompareList[a].nextYrVal;
                                                if (String.IsNullOrEmpty(val))
                                                    val = RuleConstants.NotApplicable;
                                                value += val + "-" + GetFormatedTokenName(detailedCompareList[a].Prop) + ", ";
                                            }
                                        }
                                        else
                                        {
                                            if (a == (count - 1))
                                            {
                                                val = detailedCompareList[a].previousYrVal;
                                                if (String.IsNullOrEmpty(val))
                                                    val = RuleConstants.NotApplicable;
                                                value += val + "-" + GetFormatedTokenName(detailedCompareList[a].Prop);
                                            }
                                            else
                                            {
                                                val = detailedCompareList[a].previousYrVal;
                                                if (String.IsNullOrEmpty(val))
                                                    val = RuleConstants.NotApplicable;
                                                value += val + "-" + GetFormatedTokenName(detailedCompareList[a].Prop) + ", ";
                                            }
                                        }
                                    }
                                    // Check for Min / Max Copay - Coinsurance
                                    value = CheckDuplicateCostShare(value);
                                    // Get formated string for values
                                    value = GetBenefitServiceFormatedValue(value);
                                }

                            }
                            #endregion nextYearExist
                            else
                            {
                                var service = previousYearTokenList.Where(x => Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty) == benefitCategory1
                                                        && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty) == benefitCategory2)
                                                        && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty) == benefitCategory3)
                                                        && (Convert.ToString(x.SelectToken(RuleConstants.CostShareTiers) ?? String.Empty) == tier)).FirstOrDefault();
                                value = GetNotCoveredServiceCostShare(networkTier, service);
                                // Check for Min / Max Copay.Coinsurance
                                value = CheckDuplicateCostShare(value);
                                // Get formated string for values
                                value = GetBenefitServiceFormatedValue(value);
                            }
                        }
                        else
                        {
                            // 
                            value = RuleConstants.NotCovered;
                        }
                    }
                    #endregion Previous Year

                    #region Next Year
                    else
                    {
                        nextYearExist = nextYearTokenList.Exists(x => Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty) == benefitCategory1
                                                      && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty) == benefitCategory2)
                                                      && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty) == benefitCategory3)
                                                      && (Convert.ToString(x.SelectToken(RuleConstants.CostShareTiers) ?? String.Empty) == tier));

                        if (nextYearExist)
                        {
                            previousYearExist = previousYearTokenList.Exists(x => Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty) == benefitCategory1
                                                      && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty) == benefitCategory2)
                                                      && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty) == benefitCategory3)
                                                      && (Convert.ToString(x.SelectToken(RuleConstants.CostShareTiers) ?? String.Empty) == tier));
                            #region previousYearExist
                            if (previousYearExist)
                            {
                                var previousYrService = previousYearTokenList.Where(x => Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty) == benefitCategory1
                                                  && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty) == benefitCategory2)
                                                  && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty) == benefitCategory3)
                                                  && (Convert.ToString(x.SelectToken(RuleConstants.CostShareTiers) ?? String.Empty) == tier)).FirstOrDefault();

                                var nextYrService = nextYearTokenList.Where(x => Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty) == benefitCategory1
                                                     && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty) == benefitCategory2)
                                                     && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty) == benefitCategory3)
                                                     && (Convert.ToString(x.SelectToken(RuleConstants.CostShareTiers) ?? String.Empty) == tier)).FirstOrDefault();

                                detailedCompareList = GetAdditionalServiceCostShare(networkTier, previousYrService, nextYrService, isDefaultService);
                                if (null != detailedCompareList)
                                {
                                    int count = detailedCompareList.Count;
                                    string val = string.Empty;
                                    for (int a = 0; a < count; a++)
                                    {
                                        if (isNextYear)
                                        {
                                            if (a == (count - 1))
                                            {
                                                val = detailedCompareList[a].nextYrVal;
                                                if (String.IsNullOrEmpty(val))
                                                    val = RuleConstants.NotApplicable;
                                                value += val + "-" + GetFormatedTokenName(detailedCompareList[a].Prop);
                                            }
                                            else
                                            {
                                                val = detailedCompareList[a].nextYrVal;
                                                if (String.IsNullOrEmpty(val))
                                                    val = RuleConstants.NotApplicable;
                                                value += val + "-" + GetFormatedTokenName(detailedCompareList[a].Prop) + ", ";
                                            }
                                        }
                                        else
                                        {
                                            if (a == (count - 1))
                                            {
                                                val = detailedCompareList[a].previousYrVal;
                                                if (String.IsNullOrEmpty(val))
                                                    val = RuleConstants.NotApplicable;
                                                value += val + "-" + GetFormatedTokenName(detailedCompareList[a].Prop);
                                            }
                                            else
                                            {
                                                val = detailedCompareList[a].previousYrVal;
                                                if (String.IsNullOrEmpty(val))
                                                    val = RuleConstants.NotApplicable;
                                                value += val + "-" + GetFormatedTokenName(detailedCompareList[a].Prop) + ", ";
                                            }
                                        }
                                    }
                                    // Check for Min / Max Copay.Coinsurance
                                    value = CheckDuplicateCostShare(value);
                                    // Get formated string for values
                                    value = GetBenefitServiceFormatedValue(value);
                                }
                            }
                            #endregion previousYearExist
                            else
                            {
                                var service = nextYearTokenList.Where(x => Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty) == benefitCategory1
                                                       && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty) == benefitCategory2)
                                                       && (Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty) == benefitCategory3)
                                                       && (Convert.ToString(x.SelectToken(RuleConstants.CostShareTiers) ?? String.Empty) == tier)).FirstOrDefault();

                                value = GetNotCoveredServiceCostShare(networkTier, service);
                                // Check for Min / Max Copay.Coinsurance
                                value = CheckDuplicateCostShare(value);
                                // Get formated string for values
                                value = GetBenefitServiceFormatedValue(value);
                            }
                        }
                        else
                        {
                            value = RuleConstants.NotCovered;
                        }
                    }
                    #endregion Next Year
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
            }
            return value;
        }

        private List<Variance> GetAdditionalServiceCostShare(JToken networkTier, JToken previousYearService, JToken nextYearService, bool isDefaultService)
        {
            string value = String.Empty;
            List<Variance> detailedCompareList = new List<Variance>();
            try
            {
                if (null != networkTier && null != previousYearService && null != nextYearService)
                {
                    string tierName = Convert.ToString(networkTier[RuleConstants.CostShareTiers] ?? String.Empty);
                    if (!String.IsNullOrEmpty(tierName))
                    {
                        //List<JToken> previousIQMedicareNetWorkList = previousYearService.SelectToken(RuleConstants.IQMedicareNetWorkList) == null ? new List<JToken>() : previousYearService.SelectToken(RuleConstants.IQMedicareNetWorkList).ToList();
                        //List<JToken> nextIQMedicareNetWorkList = nextYearService.SelectToken(RuleConstants.IQMedicareNetWorkList) == null ? new List<JToken>() : nextYearService.SelectToken(RuleConstants.IQMedicareNetWorkList).ToList();

                        //if (previousIQMedicareNetWorkList.Count > 0 && nextIQMedicareNetWorkList.Count > 0)
                        //{
                        //var previousYrValueToken = previousIQMedicareNetWorkList.Where(x => (x.SelectToken(RuleConstants.CostShareTiers) == null ? String.Empty : x.SelectToken(RuleConstants.CostShareTiers).ToString()) == tierName).FirstOrDefault();
                        //var nextYrValueToken = nextIQMedicareNetWorkList.Where(x => (x.SelectToken(RuleConstants.CostShareTiers) == null ? String.Empty : x.SelectToken(RuleConstants.CostShareTiers).ToString()) == tierName).FirstOrDefault();
                        var previousYrValueToken = previousYearService;
                        var nextYrValueToken = nextYearService;

                        if (null != previousYrValueToken && null != nextYrValueToken)
                        {
                            detailedCompareList = AdditionalServiceDetailedCompare(previousYrValueToken, nextYrValueToken, isDefaultService);
                        }
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
            }
            return detailedCompareList;
        }

        public List<Variance> AdditionalServiceDetailedCompare(JToken previousYrValueToken, JToken nextYrValueToken, bool isDefaultService)
        {
            List<Variance> variances = new List<Variance>();
            if (null != previousYrValueToken && null != nextYrValueToken)
            {
                List<JToken> nextYrTierTokens = nextYrValueToken.ToList();
                List<JToken> prevYrTierTokens = previousYrValueToken.ToList();

                if (null != nextYrTierTokens && null != prevYrTierTokens)
                {
                    int tierTokensCount = nextYrTierTokens.Count;
                    Variance v = new Variance();
                    string tokenName = string.Empty;
                    string previousVal = string.Empty;
                    string nextVal = string.Empty;
                    string symbol = string.Empty;
                    bool isNumeric = false;
                    for (int j = 0; j < tierTokensCount; j++)
                    {
                        symbol = string.Empty;
                        v = new Variance();
                        tokenName = ((JProperty)nextYrTierTokens[j]).Name;
                        nextVal = Convert.ToString(((JProperty)nextYrTierTokens[j]).Value ?? String.Empty);
                        previousVal = Convert.ToString(((JProperty)prevYrTierTokens[j]).Value ?? String.Empty);

                        int n;
                        string val = previousVal;
                        if (previousVal.Contains(",")) { val = previousVal.Replace(",", ""); }
                        if (previousVal.Contains(".")) { val = previousVal.Replace(".", ""); }
                        isNumeric = int.TryParse(val, out n);
                        if (tokenName != RuleConstants.CostShareTiers && tokenName != RuleConstants.BenefitCategory1
                               && tokenName != RuleConstants.BenefitCategory2 && tokenName != RuleConstants.BenefitCategory3
                               && tokenName != RuleConstants.RowIDProperty && tokenName != RuleConstants.ServiceSelectionMapping && tokenName != RuleConstants.PBPCode
                               && tokenName != RuleConstants.Condition)
                        {
                            if (isNumeric && n >= 0)
                            {
                                if (tokenName == RuleConstants.MinimumCoinsurance || tokenName == RuleConstants.MaximumCoinsuarnce
                                    || tokenName == RuleConstants.MinCoinsurance || tokenName == RuleConstants.MaxCoinsurance)
                                    previousVal = previousVal + RuleConstants.Percent;
                                else
                                {
                                    if (tokenName != RuleConstants.OOPMPeriodicity && tokenName != RuleConstants.MaximumPlanBenefitCoveragePeriodicity
                                        && tokenName != RuleConstants.RowIDProperty)
                                    {
                                        previousVal = RuleConstants.Dollar + previousVal;
                                    }
                                }
                            }
                            v.Prop = tokenName;
                            v.previousYrVal = symbol + previousVal;

                            symbol = string.Empty;
                            val = nextVal;
                            if (nextVal.Contains(",")) { val = nextVal.Replace(",", ""); }
                            if (nextVal.Contains(".")) { val = nextVal.Replace(".", ""); }

                            isNumeric = int.TryParse(val, out n);
                            if (isNumeric && n >= 0)
                            {
                                if (tokenName == RuleConstants.MinimumCoinsurance || tokenName == RuleConstants.MaximumCoinsuarnce
                                    || tokenName == RuleConstants.MinCoinsurance || tokenName == RuleConstants.MaxCoinsurance)
                                    nextVal = nextVal + RuleConstants.Percent;
                                else
                                {
                                    if (tokenName != RuleConstants.OOPMPeriodicity && tokenName != RuleConstants.MaximumPlanBenefitCoveragePeriodicity
                                        && tokenName != RuleConstants.RowIDProperty)
                                    {
                                        nextVal = RuleConstants.Dollar + nextVal;
                                    }
                                }
                            }
                            v.nextYrVal = symbol + nextVal;
                            if (isDefaultService)
                            {
                                if (tokenName != RuleConstants.CostShareTiers)
                                    variances.Add(v);
                            }
                            else
                            {
                                if (tokenName != RuleConstants.CostShareTiers)
                                    variances.Add(v);
                            }
                        }
                    }
                    // Check for OOPMValue - Periodicity and MaximumPlanBenefitCoverageAmount - Periodicity
                    variances = FormatPeriodicityCostShare(variances, previousYrValueToken, nextYrValueToken);
                }
            }

            return variances;
        }
        private List<BenefitsCompare> GetAdditionalSlidingCostShareServicesData(List<BenefitsCompare> comparisionBenefitServices, List<JToken> repeaterData, List<BenefitsCompare> repeaterSelectedServices)
        {
            string value = String.Empty;
            List<BenefitsCompare> slidingCostShareServices = new List<BenefitsCompare>();
            BenefitsCompare benefitsCompare = new BenefitsCompare();
            try
            {
                List<JToken> previousYearSlidingList = GetSlidingCostShareServices(false, true, repeaterData);
                List<JToken> nextYearSlidingList = GetSlidingCostShareServices(true, true, repeaterData);

                List<JToken> prevSlidingCostShareIntervalList = new List<JToken>();
                List<JToken> nextSlidingCostShareIntervalList = new List<JToken>();
                Dictionary<string, string> keys = new Dictionary<string, string>();
                if (null != previousYearSlidingList && null != nextYearSlidingList)
                {
                    keys.Add(RuleConstants.BenefitCategory1, RuleConstants.BenefitCategory1);
                    keys.Add(RuleConstants.BenefitCategory2, RuleConstants.BenefitCategory2);
                    keys.Add(RuleConstants.BenefitCategory3, RuleConstants.BenefitCategory3);
                    keys.Add(RuleConstants.CostShareTiers, RuleConstants.CostShareTiers);

                    CollectionExecutionComparer distinctProcessor = new CollectionExecutionComparer(previousYearSlidingList, keys);
                    prevSlidingCostShareIntervalList = distinctProcessor.Distinct();

                    distinctProcessor = new CollectionExecutionComparer(nextYearSlidingList, keys);
                    nextSlidingCostShareIntervalList = distinctProcessor.Distinct();
                }

                List<JToken> prevSlidingCostShareInfoList = previousYearSlidingList;
                List<JToken> nextSlidingCostShareInfoList = nextYearSlidingList;

                List<JToken> resultForTierUnion = new List<JToken>();
                List<JToken> resultForTierExcept = new List<JToken>();
                List<JToken> resultForProcessing = new List<JToken>();

                keys = new Dictionary<string, string>();
                keys.Add(RuleConstants.BenefitCategory1, RuleConstants.BenefitCategory1);
                keys.Add(RuleConstants.BenefitCategory2, RuleConstants.BenefitCategory2);
                keys.Add(RuleConstants.BenefitCategory3, RuleConstants.BenefitCategory3);
                keys.Add(RuleConstants.CostShareTiers, RuleConstants.CostShareTiers);

                CollectionExecutionComparer operatorProcessor = new CollectionExecutionComparer(prevSlidingCostShareIntervalList, nextSlidingCostShareIntervalList, keys);
                resultForTierUnion = operatorProcessor.Union();
                resultForProcessing = resultForTierUnion;

                Dictionary<string, string> exceptKeys = new Dictionary<string, string>();
                exceptKeys.Add(RuleConstants.BenefitCategory1, RuleConstants.BenefitCategory1);
                exceptKeys.Add(RuleConstants.BenefitCategory2, RuleConstants.BenefitCategory2);
                exceptKeys.Add(RuleConstants.BenefitCategory3, RuleConstants.BenefitCategory3);
                //exceptKeys.Add(RuleConstants.CostShareTiers, "CostShareTiers:Tier");

                operatorProcessor = new CollectionExecutionComparer(resultForTierUnion, repeaterData, exceptKeys);
                resultForTierExcept = operatorProcessor.Except();

                bool hasBlankRow = false;
                int rowID = 0;
                List<JToken> prevIntervalInfo = new List<JToken>();
                List<JToken> nextIntervalInfo = new List<JToken>();
                foreach (var entry in repeaterSelectedServices)
                {
                    var service = resultForTierExcept.Where(x => Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty) == entry.BenefitCategory1
                                                    && Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty) == entry.BenefitCategory2
                                                    && Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty) == entry.BenefitCategory3
                                                    //&& Convert.ToString(x.SelectToken(RuleConstants.CostShareTiers) ?? String.Empty) == entry.CostShareTiers
                                                    ).FirstOrDefault();
                    if (null != service)
                    {
                        var services = resultForProcessing.Where(x => Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty) == entry.BenefitCategory1
                                                  && Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty) == entry.BenefitCategory2
                                                  && Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty) == entry.BenefitCategory3
                                                  //&& Convert.ToString(x.SelectToken(RuleConstants.CostShareTiers) ?? String.Empty) == entry.CostShareTiers
                                                  ).ToList();

                        foreach (var token in services)
                        {
                            rowID = rowID + 1;
                            benefitsCompare = new BenefitsCompare();
                            hasBlankRow = HasBlankRow(token);
                            if (!hasBlankRow)
                            {
                                prevIntervalInfo = prevSlidingCostShareInfoList.FindAll(x => Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty) == Convert.ToString(token[RuleConstants.BenefitCategory1] ?? String.Empty)
                                                           && Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty) == Convert.ToString(token[RuleConstants.BenefitCategory2] ?? String.Empty)
                                                           && Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty) == Convert.ToString(token[RuleConstants.BenefitCategory3] ?? String.Empty)
                                                           && Convert.ToString(x.SelectToken(RuleConstants.CostShareTiers) ?? String.Empty) == Convert.ToString(token[RuleConstants.CostShareTiers] ?? String.Empty));

                                nextIntervalInfo = nextSlidingCostShareInfoList.FindAll(x => Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty) == Convert.ToString(token[RuleConstants.BenefitCategory1] ?? String.Empty)
                                                             && Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty) == Convert.ToString(token[RuleConstants.BenefitCategory2] ?? String.Empty)
                                                             && Convert.ToString(x.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty) == Convert.ToString(token[RuleConstants.BenefitCategory3] ?? String.Empty)
                                                             && Convert.ToString(x.SelectToken(RuleConstants.CostShareTiers) ?? String.Empty) == Convert.ToString(token[RuleConstants.CostShareTiers] ?? String.Empty));

                                //benefitsCompare.RowID = rowID;
                                benefitsCompare.ANOCBenefitName = String.Empty;
                                benefitsCompare.BenefitCategory1 = Convert.ToString(token[RuleConstants.BenefitCategory1] ?? String.Empty);
                                benefitsCompare.BenefitCategory2 = Convert.ToString(token[RuleConstants.BenefitCategory2] ?? String.Empty);
                                benefitsCompare.BenefitCategory3 = Convert.ToString(token[RuleConstants.BenefitCategory3] ?? String.Empty);
                                benefitsCompare.CostShareTiers = Convert.ToString(token[RuleConstants.CostShareTiers] ?? String.Empty);
                                benefitsCompare.NextYear = GetSlidingCostShareValues(benefitsCompare.BenefitCategory1, benefitsCompare.BenefitCategory2, benefitsCompare.BenefitCategory3, benefitsCompare.CostShareTiers, prevSlidingCostShareIntervalList, nextSlidingCostShareIntervalList, prevIntervalInfo, nextIntervalInfo, token, true, true);
                                benefitsCompare.ThisYear = GetSlidingCostShareValues(benefitsCompare.BenefitCategory1, benefitsCompare.BenefitCategory2, benefitsCompare.BenefitCategory3, benefitsCompare.CostShareTiers, prevSlidingCostShareIntervalList, nextSlidingCostShareIntervalList, prevIntervalInfo, nextIntervalInfo, token, false, true);
                                benefitsCompare.DisplayinANOC = "false";
                                //benefitsCompare.RowIDProperty = rowID - 1;
                                // Check if all network values are blank
                                //if (!HasSameCostShare(benefitsCompare.IQMedicareNetWorkList))
                                slidingCostShareServices.Add(benefitsCompare);

                                // Need to check if service exist in BRG and not covered in sliding costshare comparision 
                                // If not covered in our sliding costshare comparison then we need to add it eventhough not in compaarision,
                                // as we are adding \r\n in between costshares of Slidingcostshare repeaters and BRG repeaters 
                                //var slidingCostShareExist = slidingCostShareServices.Exists(x => x.BenefitCategory1 == Convert.ToString(token[RuleConstants.BenefitCategory1] ?? String.Empty)
                                //                          && x.BenefitCategory2 == Convert.ToString(token[RuleConstants.BenefitCategory2] ?? String.Empty)
                                //                          && x.BenefitCategory3 == Convert.ToString(token[RuleConstants.BenefitCategory3] ?? String.Empty));

                                //if (!slidingCostShareExist)
                                //{
                                //    var brgExist = comparisionBenefitServices.Exists(x => x.BenefitCategory1 == Convert.ToString(token[RuleConstants.BenefitCategory1] ?? String.Empty)
                                //                          && x.BenefitCategory2 == Convert.ToString(token[RuleConstants.BenefitCategory2] ?? String.Empty)
                                //                          && x.BenefitCategory3 == Convert.ToString(token[RuleConstants.BenefitCategory3] ?? String.Empty));
                                //    if (brgExist)
                                //        slidingCostShareServices.Add(benefitsCompare);
                                //}
                            }
                        }
                    }
                }
                // Compare seervices of sliding costshare and BRG to merge into single repeater with both slidingcostshare and BRG data in required format
                comparisionBenefitServices = CompareServices(slidingCostShareServices, comparisionBenefitServices);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
            }
            return comparisionBenefitServices;
        }
        private List<JToken> GetAdditionalSlidingCostShareServices(List<JToken> comparisionBenefitServices, List<JToken> repeaterData)
        {
            List<JToken> resultForTierUnion = new List<JToken>();
            List<JToken> resultForTierExcept = new List<JToken>();
            List<JToken> resultForServiceUnion = new List<JToken>();
            try
            {
                List<JToken> prevSlidingCostShareIntervalList = GetSlidingCostShareServices(false, true, repeaterData);
                List<JToken> nextSlidingCostShareIntervalList = GetSlidingCostShareServices(true, true, repeaterData);

                Dictionary<string, string> keys = new Dictionary<string, string>();
                keys.Add(RuleConstants.BenefitCategory1, RuleConstants.BenefitCategory1);
                keys.Add(RuleConstants.BenefitCategory2, RuleConstants.BenefitCategory2);
                keys.Add(RuleConstants.BenefitCategory3, RuleConstants.BenefitCategory3);
                //keys.Add(RuleConstants.CostShareTiers, RuleConstants.CostShareTiers);

                CollectionExecutionComparer operatorProcessor = new CollectionExecutionComparer(prevSlidingCostShareIntervalList, nextSlidingCostShareIntervalList, keys);
                resultForTierUnion = operatorProcessor.Union();

                Dictionary<string, string> exceptKeys = new Dictionary<string, string>();
                exceptKeys.Add(RuleConstants.BenefitCategory1, RuleConstants.BenefitCategory1);
                exceptKeys.Add(RuleConstants.BenefitCategory2, RuleConstants.BenefitCategory2);
                exceptKeys.Add(RuleConstants.BenefitCategory3, RuleConstants.BenefitCategory3);
                //exceptKeys.Add(RuleConstants.CostShareTiers, "CostShareTiers:Tier");

                operatorProcessor = new CollectionExecutionComparer(resultForTierUnion, repeaterData, exceptKeys);
                resultForTierExcept = operatorProcessor.Except();
                if (null != resultForTierExcept)
                {
                    operatorProcessor = new CollectionExecutionComparer(resultForTierExcept, comparisionBenefitServices, keys);
                    resultForServiceUnion = operatorProcessor.Union();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
            }
            return resultForServiceUnion;
        }
        private bool CheckViewSectionRxPreICLVisibility(bool isNextYear)
        {
            bool visible = false;
            try
            {
                string cond1Path = "SectionA.AdditionalFields.DoesyourplanofferaMedicarePrescriptiondrugPartDbenefit";
                string cond2Path = "SectionA.SectionA1.IsthisanEmployerOnlyplan";
                string doesRX = String.Empty; string isEmployerPlan = String.Empty;
                if (isNextYear)
                {
                    doesRX = Convert.ToString(nextPBPViewJsonData.SelectToken(cond1Path) ?? String.Empty);
                    isEmployerPlan = Convert.ToString(nextPBPViewJsonData.SelectToken(cond2Path) ?? String.Empty);
                    if (!String.IsNullOrEmpty(doesRX) && !String.IsNullOrEmpty(isEmployerPlan))
                    {
                        if (String.Equals(doesRX, "1") && String.Equals(isEmployerPlan, "2"))
                            visible = true;
                    }
                }
                else
                {
                    doesRX = Convert.ToString(previousPBPViewJsonData.SelectToken(cond1Path) ?? String.Empty);
                    isEmployerPlan = Convert.ToString(previousPBPViewJsonData.SelectToken(cond2Path) ?? String.Empty);
                    if (!String.IsNullOrEmpty(doesRX) && !String.IsNullOrEmpty(isEmployerPlan))
                    {
                        if (String.Equals(doesRX, "1") && String.Equals(isEmployerPlan, "2"))
                            visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                visible = false;
            }
            return visible;
        }
        private bool CheckViewSectionRxGapCoverageVisibility(bool isNextYear)
        {
            bool visible = false;
            try
            {
                string cond1Path = "BasicEnhancedAlternative.AlternativeGapCoverage.Selectthetiersthatincludegapcoverageselectallthatapply";
                string cond2Path = "SectionA.AdditionalFields.DoesyourplanofferaMedicarePrescriptiondrugPartDbenefit";
                string cond3Path = "MedicareRxGeneral.MedicareRxGeneral1.Selectthetypeofdrugbenefit";
                string cond4Path = "SectionA.SectionA1.IsthisanEmployerOnlyplan";
                string doesRX = String.Empty; string isEmployerPlan = String.Empty;
                List<JToken> gapCovTier = null; string drugBenft = String.Empty;
                if (isNextYear)
                {
                    gapCovTier = nextPBPViewJsonData.SelectToken(cond1Path) == null ? new List<JToken>() : nextPBPViewJsonData.SelectToken(cond1Path).ToList();
                    doesRX = Convert.ToString(nextPBPViewJsonData.SelectToken(cond2Path) ?? String.Empty);
                    drugBenft = Convert.ToString(nextPBPViewJsonData.SelectToken(cond3Path) ?? String.Empty);
                    isEmployerPlan = Convert.ToString(nextPBPViewJsonData.SelectToken(cond4Path) ?? String.Empty);
                    if (!String.IsNullOrEmpty(doesRX) && !String.IsNullOrEmpty(isEmployerPlan) && gapCovTier != null && !String.IsNullOrEmpty(drugBenft))
                    {
                        if (String.Equals(doesRX, "1") && String.Equals(isEmployerPlan, "2")
                            && gapCovTier.Count >= 1 && String.Equals(drugBenft, "4"))
                            visible = true;
                    }
                }
                else
                {
                    gapCovTier = previousPBPViewJsonData.SelectToken(cond1Path) == null ? new List<JToken>() : previousPBPViewJsonData.SelectToken(cond1Path).ToList();
                    doesRX = Convert.ToString(previousPBPViewJsonData.SelectToken(cond2Path) ?? String.Empty);
                    drugBenft = Convert.ToString(previousPBPViewJsonData.SelectToken(cond3Path) ?? String.Empty);
                    isEmployerPlan = Convert.ToString(previousPBPViewJsonData.SelectToken(cond4Path) ?? String.Empty);
                    if (!String.IsNullOrEmpty(doesRX) && !String.IsNullOrEmpty(isEmployerPlan) && gapCovTier != null && !String.IsNullOrEmpty(drugBenft))
                    {
                        if (String.Equals(doesRX, "1") && String.Equals(isEmployerPlan, "2")
                            && gapCovTier.Count >= 1 && String.Equals(drugBenft, "4"))
                            visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                visible = false;
            }
            return visible;
        }
        private bool CheckMedicareSectionRxVisibility(bool isNextYear)
        {
            bool visible = false;
            try
            {
                string isRx = String.Empty;
                if (isNextYear)
                {
                    isRx = Convert.ToString(nextMedicareJsonData.SelectToken("SECTIONASECTIONA1.DoesyourPlanofferaPrescriptionPartDbenefit") ?? String.Empty) == String.Empty ? "NO" : Convert.ToString(nextMedicareJsonData.SelectToken("SECTIONASECTIONA1.DoesyourPlanofferaPrescriptionPartDbenefit"));
                    if (!String.IsNullOrEmpty(isRx))
                    {
                        if (isRx.Equals("YES"))
                            visible = true;
                        else
                            visible = false;
                    }
                }
                else
                {
                    isRx = Convert.ToString(previousMedicareJsonData.SelectToken("SECTIONASECTIONA1.DoesyourPlanofferaPrescriptionPartDbenefit") ?? String.Empty) == String.Empty ? "NO" : Convert.ToString(previousMedicareJsonData.SelectToken("SECTIONASECTIONA1.DoesyourPlanofferaPrescriptionPartDbenefit"));
                    if (!String.IsNullOrEmpty(isRx))
                    {
                        if (isRx.Equals("YES"))
                            visible = true;
                        else
                            visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                visible = false;
            }
            return visible;
        }
        private bool CheckViewSectionDPlanLevelVisibility(bool isNextYear)
        {
            bool visible = false;
            try
            {
                string cond1Path = "SectionA.SectionA1.PlanType";
                string plaType = String.Empty;
                if (isNextYear)
                {
                    plaType = Convert.ToString(nextPBPViewJsonData.SelectToken(cond1Path) ?? String.Empty);
                    if (!String.IsNullOrEmpty(plaType))
                    {
                        if (!String.Equals(plaType, "29") && !String.Equals(plaType, "32")
                        && !String.Equals(plaType, "20") && !String.Equals(plaType, "30"))
                        {
                            visible = true;
                        }
                    }
                }
                else
                {
                    plaType = Convert.ToString(previousPBPViewJsonData.SelectToken(cond1Path) ?? String.Empty);
                    if (!String.IsNullOrEmpty(plaType))
                    {
                        if (!String.Equals(plaType, "29") && !String.Equals(plaType, "32")
                        && !String.Equals(plaType, "20") && !String.Equals(plaType, "30"))
                        {
                            visible = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                visible = false;
            }
            return visible;
        }
        private List<JToken> GetBRGServices(bool isNextYear, bool isManualService, List<JToken> repeaterData)
        {
            List<JToken> services = new List<JToken>();
            List<JToken> servicesToProcess = new List<JToken>();
            List<JToken> servicesNotCovered = new List<JToken>();
            try
            {
                if (null != masterListANOCEOCJsonData && null != previousMedicareJsonData && null != nextMedicareJsonData
                 && null != nextPBPViewJsonData && null != previousPBPViewJsonData)
                {
                    JObject o2 = (JObject)masterListANOCEOCJsonData.DeepClone();
                    services = o2.SelectToken("BenefitChart.ANOCCHARTDATABRG") == null ? new List<JToken>() : o2.SelectToken("BenefitChart.ANOCCHARTDATABRG").ToList();
                    // Filter only IN Tier
                    List<string> networkTierList = AnocHelper.GetNetworkTiers(isNextYear);
                    services = (from ser in services
                                //where networkTierList.Any(s => s.Contains(ser.SelectToken("Tier").ToString().Trim()))
                                where networkTierList.Contains(ser.SelectToken("Tier").ToString().Trim())
                                select ser).ToList();
                    //services = services.FindAll(x => Convert.ToString(x.SelectToken("Tier") ?? String.Empty) == "IN");


                    // Hold on all services
                    servicesToProcess = services;

                    //If manual service then take only those servies that are not covered in comparision 
                    // so we dont need to resolve path for already compared one that will reducce time. 
                    if (isManualService)
                    {
                        if (repeaterData != null && repeaterData.Count > 0)
                        {
                            Dictionary<string, string> keys = new Dictionary<string, string>();
                            keys.Add(RuleConstants.BenefitCategory1, RuleConstants.BenefitCategory1);
                            keys.Add(RuleConstants.BenefitCategory2, RuleConstants.BenefitCategory2);
                            keys.Add(RuleConstants.BenefitCategory3, RuleConstants.BenefitCategory3);
                            //keys.Add(RuleConstants.CostShareTiers, "CostShareTiers:Tier");
                            CollectionExecutionComparer operatorProcessor = new CollectionExecutionComparer(services, repeaterData, keys);
                            services = operatorProcessor.Except();
                            // Get services for selected tiers
                            services = (from val in servicesToProcess
                                        join ser in services on new { BC1 = val.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty, BC2 = val.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty, BC3 = val.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty }
                                        equals new { BC1 = ser.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty, BC2 = ser.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty, BC3 = ser.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty }
                                        select val).ToList();

                        }
                    }
                    if (null != services)
                    {
                        for (int i = 0; i < services.Count; i++)
                        {
                            try
                            {
                                JToken mainService = services[i];
                                if (mainService != null)
                                {
                                    List<JToken> columnList = mainService.ToList();
                                    if (columnList != null)
                                    {
                                        string tokenName = String.Empty; string parent = String.Empty;
                                        string tokenPath = String.Empty; JToken val = null;
                                        foreach (var column in columnList)
                                        {
                                            try
                                            {
                                                tokenName = String.Empty; tokenPath = String.Empty; parent = String.Empty; val = null;
                                                tokenName = ((JProperty)column).Name;
                                                tokenPath = Convert.ToString(((JProperty)column).Value ?? String.Empty);
                                                if (tokenName != RuleConstants.BenefitCategory1 && tokenName != RuleConstants.BenefitCategory2 && tokenName != RuleConstants.BenefitCategory3
                                                 && tokenName != RuleConstants.CostShareTiers && tokenName != RuleConstants.RowIDProperty
                                                 && tokenName != RuleConstants.ServiceGroup && tokenName != RuleConstants.SequenceNo
                                                 && tokenName != RuleConstants.PBPServiceCode && tokenName != RuleConstants.Auths
                                                 && tokenName != RuleConstants.Referrals)
                                                {
                                                    if (!String.IsNullOrEmpty(tokenPath))
                                                    {
                                                        int indx = tokenPath.IndexOf("[");
                                                        if (indx != -1)
                                                        {
                                                            parent = tokenPath.Split('[')[0];
                                                            tokenPath = tokenPath.Split('[')[1].Split(']')[0];
                                                        }
                                                        if (!String.IsNullOrEmpty(parent) && String.Equals(parent, "PBPView"))
                                                        {
                                                            if (isNextYear)
                                                                val = nextPBPViewJsonData.SelectToken(tokenPath) ?? String.Empty;
                                                            else
                                                                val = previousPBPViewJsonData.SelectToken(tokenPath) ?? String.Empty;

                                                            // replace path with path value
                                                            mainService[tokenName] = val;
                                                        }
                                                        else if (!String.IsNullOrEmpty(parent) && String.Equals(parent, "Medicare"))
                                                        {
                                                            if (isNextYear)
                                                                val = nextMedicareJsonData.SelectToken(tokenPath) ?? String.Empty;
                                                            else
                                                                val = previousMedicareJsonData.SelectToken(tokenPath) ?? String.Empty;
                                                            // replace path with path value
                                                            mainService[tokenName] = val;
                                                        }
                                                        else
                                                        {
                                                            if ((Convert.ToString(mainService[tokenName]) ?? string.Empty).Contains("[") || (Convert.ToString(mainService[tokenName]) ?? string.Empty).Contains("]"))
                                                            {
                                                                mainService[tokenName] = string.Empty;
                                                            }
                                                            //mainService[tokenName] = string.Empty;
                                                        }
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                            }
                                        }
                                        // Evaluate Expression
                                        string expression = Convert.ToString(mainService["Condition"] ?? String.Empty);
                                        if (!String.IsNullOrEmpty(expression))
                                        {
                                            bool isCovered = IsServiceCovered(expression, mainService);
                                            if (!isCovered)
                                            {
                                                servicesNotCovered.Add(mainService);
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        // Remove not covered services from main service list
                        if (servicesNotCovered != null && servicesNotCovered.Count > 0)
                        {
                            Dictionary<string, string> keys = new Dictionary<string, string>();
                            keys.Add(RuleConstants.BenefitCategory1, RuleConstants.BenefitCategory1);
                            keys.Add(RuleConstants.BenefitCategory2, RuleConstants.BenefitCategory2);
                            keys.Add(RuleConstants.BenefitCategory3, RuleConstants.BenefitCategory3);
                            keys.Add(RuleConstants.CostShareTiers, RuleConstants.CostShareTiers);
                            CollectionExecutionComparer operatorProcessor = new CollectionExecutionComparer(services, servicesNotCovered, keys);
                            services = operatorProcessor.Except();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return services;
        }
        private List<JToken> GetSlidingCostShareServices(bool isNextYear, bool isManualService, List<JToken> repeaterData)
        {
            List<JToken> services = new List<JToken>();
            List<JToken> servicesToProcess = new List<JToken>();
            List<JToken> servicesNotCovered = new List<JToken>();
            try
            {
                if (null != masterListANOCEOCJsonData && null != previousMedicareJsonData && null != nextMedicareJsonData
                 && null != nextPBPViewJsonData && null != previousPBPViewJsonData)
                {
                    JObject o2 = (JObject)masterListANOCEOCJsonData.DeepClone();

                    services = o2.SelectToken("BenefitChart.ANOCCHARTDATASCS") == null ? new List<JToken>() : o2.SelectToken("BenefitChart.ANOCCHARTDATASCS").ToList();
                    List<string> networkTierList = AnocHelper.GetNetworkTiers(isNextYear);
                    services = (from ser in services
                                //where networkTierList.Any(s => s.Contains(ser.SelectToken("Tier").ToString().Trim()))
                                where networkTierList.Contains(ser.SelectToken("Tier").ToString().Trim())
                                select ser).ToList();
                    var t = services.Where(s => s.SelectToken("Interval").ToString().Equals("0")).ToList();
                    //services = services.FindAll(x => Convert.ToString(x.SelectToken("Tier") ?? String.Empty) == "IN");

                    // Hold on all services
                    servicesToProcess = services;

                    //If manual service then take only those servies that are not covered in comparision 
                    // so we dont need to resolve path for already compared one that will reducce time. 
                    if (isManualService)
                    {
                        if (repeaterData != null && repeaterData.Count > 0)
                        {
                            Dictionary<string, string> keys = new Dictionary<string, string>();
                            keys.Add(RuleConstants.BenefitCategory1, RuleConstants.BenefitCategory1);
                            keys.Add(RuleConstants.BenefitCategory2, RuleConstants.BenefitCategory2);
                            keys.Add(RuleConstants.BenefitCategory3, RuleConstants.BenefitCategory3);
                            //keys.Add(RuleConstants.CostShareTiers, "CostShareTiers:Tier");
                            CollectionExecutionComparer operatorProcessor = new CollectionExecutionComparer(services, repeaterData, keys);
                            services = operatorProcessor.Except();
                            // Get services for selected tiers
                            services = (from val in servicesToProcess
                                        join ser in services on new { BC1 = val.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty, BC2 = val.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty, BC3 = val.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty }
                                        equals new { BC1 = ser.SelectToken(RuleConstants.BenefitCategory1) ?? String.Empty, BC2 = ser.SelectToken(RuleConstants.BenefitCategory2) ?? String.Empty, BC3 = ser.SelectToken(RuleConstants.BenefitCategory3) ?? String.Empty }
                                        select val).ToList();
                        }
                    }
                    if (null != services)
                    {
                        for (int i = 0; i < services.Count; i++)
                        {
                            JToken mainService = services[i];
                            if (mainService != null)
                            {
                                List<JToken> columnList = mainService.ToList();
                                if (columnList != null)
                                {
                                    string tokenName = String.Empty; string parent = String.Empty;
                                    string tokenPath = String.Empty; JToken val = null;
                                    foreach (var column in columnList)
                                    {
                                        try
                                        {
                                            tokenName = String.Empty; tokenPath = String.Empty; parent = String.Empty; val = null;
                                            tokenName = ((JProperty)column).Name;
                                            tokenPath = Convert.ToString(((JProperty)column).Value ?? String.Empty);
                                            if (tokenName != RuleConstants.BenefitCategory1 && tokenName != RuleConstants.BenefitCategory2 && tokenName != RuleConstants.BenefitCategory3
                                            && tokenName != RuleConstants.CostShareTiers && tokenName != RuleConstants.Interval && tokenName != RuleConstants.RowIDProperty
                                            && tokenName != RuleConstants.CostShareType && tokenName != RuleConstants.IntervalCount && tokenName != RuleConstants.Filter)
                                            {
                                                if (!String.IsNullOrEmpty(tokenPath))
                                                {
                                                    int indx = tokenPath.IndexOf("[");
                                                    if (indx != -1)
                                                    {
                                                        parent = tokenPath.Split('[')[0];
                                                        tokenPath = tokenPath.Split('[')[1].Split(']')[0];
                                                    }
                                                    if (!String.IsNullOrEmpty(parent) && String.Equals(parent, "PBPView"))
                                                    {
                                                        if (isNextYear)
                                                            val = nextPBPViewJsonData.SelectToken(tokenPath) ?? String.Empty;
                                                        else
                                                            val = previousPBPViewJsonData.SelectToken(tokenPath) ?? String.Empty;

                                                        // replace path with path value
                                                        mainService[tokenName] = val;
                                                    }
                                                    else if (!String.IsNullOrEmpty(parent) && String.Equals(parent, "Medicare"))
                                                    {
                                                        if (isNextYear)
                                                            val = nextMedicareJsonData.SelectToken(tokenPath) ?? String.Empty;
                                                        else
                                                            val = previousMedicareJsonData.SelectToken(tokenPath) ?? String.Empty;

                                                        // replace path with path value
                                                        mainService[tokenName] = val;
                                                    }
                                                    else
                                                    {
                                                        if ((Convert.ToString(mainService[tokenName]) ?? string.Empty).Contains("[") || (Convert.ToString(mainService[tokenName]) ?? string.Empty).Contains("]"))
                                                        {
                                                            mainService[tokenName] = string.Empty;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                        }
                                    }
                                    // Evaluate Expression
                                    string expression = Convert.ToString(mainService["Condition"] ?? String.Empty);
                                    if (!String.IsNullOrEmpty(expression))
                                    {
                                        bool isCovered = IsServiceCovered(expression, mainService);
                                        if (!isCovered)
                                        {
                                            servicesNotCovered.Add(mainService);
                                        }
                                    }
                                }
                            }
                        }
                        // Remove not covered services from main service list
                        if (servicesNotCovered != null && servicesNotCovered.Count > 0)
                        {
                            Dictionary<string, string> keys = new Dictionary<string, string>();
                            keys.Add(RuleConstants.BenefitCategory1, RuleConstants.BenefitCategory1);
                            keys.Add(RuleConstants.BenefitCategory2, RuleConstants.BenefitCategory2);
                            keys.Add(RuleConstants.BenefitCategory3, RuleConstants.BenefitCategory3);
                            keys.Add(RuleConstants.CostShareTiers, RuleConstants.CostShareTiers);
                            keys.Add(RuleConstants.Interval, RuleConstants.Interval);
                            keys.Add(RuleConstants.CostShareType, RuleConstants.CostShareType);

                            CollectionExecutionComparer operatorProcessor = new CollectionExecutionComparer(services, servicesNotCovered, keys);
                            services = operatorProcessor.Except();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return services;
        }

        private List<JToken> GetDefaultBRGServices(bool isNextYear)
        {
            List<JToken> services = new List<JToken>();
            try
            {
                if (null != masterListANOCEOCJsonData && null != previousMedicareJsonData && null != nextMedicareJsonData
                 && null != nextPBPViewJsonData && null != previousPBPViewJsonData)
                {
                    JObject o2 = (JObject)masterListANOCEOCJsonData.DeepClone();
                    services = o2.SelectToken("BenefitChart.ANOCCHARTDATABRG") == null ? new List<JToken>() : o2.SelectToken("BenefitChart.ANOCCHARTDATABRG").ToList();
                    // Filter only IN Tier
                    //services = services.FindAll(x => Convert.ToString(x.SelectToken("Tier") ?? String.Empty) == "IN");

                    List<JToken> defaultServices = new List<JToken>();
                    defaultServices = RuleConstants.GetDefaultServices();
                    // Filter BRG services acording tier
                    List<string> networkTierList = AnocHelper.GetNetworkTiers(isNextYear);
                    defaultServices = (from ser in defaultServices
                                       //where networkTierList.Any(s => s.Contains(ser.SelectToken("Tier").ToString().Trim()))
                                       where networkTierList.Contains(ser.SelectToken("Tier").ToString().Trim())
                                       && Convert.ToBoolean(ser.SelectToken("IsBRGService") ?? false) == true
                                       select ser).ToList();


                    //defaultServices = defaultServices.FindAll(x => Convert.ToString(x.SelectToken("Tier") ?? String.Empty) == "IN"
                    //                                       && Convert.ToBoolean(x.SelectToken("IsBRGService") ?? false) == true);

                    Dictionary<string, string> keys = new Dictionary<string, string>();
                    keys.Add(RuleConstants.BenefitCategory1, RuleConstants.BenefitCategory1);
                    keys.Add(RuleConstants.BenefitCategory2, RuleConstants.BenefitCategory2);
                    keys.Add(RuleConstants.BenefitCategory3, RuleConstants.BenefitCategory3);
                    keys.Add(RuleConstants.CostShareTiers, RuleConstants.CostShareTiers);

                    CollectionExecutionComparer operatorProcessor = new CollectionExecutionComparer(services, defaultServices, keys);
                    services = operatorProcessor.Intersection();

                    if (null != services)
                    {
                        for (int i = 0; i < services.Count; i++)
                        {
                            try
                            {
                                JToken mainService = services[i];
                                if (mainService != null)
                                {
                                    List<JToken> columnList = mainService.ToList();
                                    if (columnList != null)
                                    {
                                        string tokenName = String.Empty; string parent = String.Empty;
                                        string tokenPath = String.Empty; JToken val = null;
                                        foreach (var column in columnList)
                                        {
                                            try
                                            {
                                                tokenName = String.Empty; tokenPath = String.Empty; parent = String.Empty; val = null;
                                                tokenName = ((JProperty)column).Name;
                                                tokenPath = Convert.ToString(((JProperty)column).Value ?? String.Empty);
                                                if (tokenName != RuleConstants.BenefitCategory1 && tokenName != RuleConstants.BenefitCategory2 && tokenName != RuleConstants.BenefitCategory3
                                                 && tokenName != RuleConstants.CostShareTiers && tokenName != RuleConstants.RowIDProperty
                                                 && tokenName != RuleConstants.ServiceGroup && tokenName != RuleConstants.SequenceNo
                                                 && tokenName != RuleConstants.PBPServiceCode && tokenName != RuleConstants.Auths
                                                 && tokenName != RuleConstants.Referrals)
                                                {
                                                    if (!String.IsNullOrEmpty(tokenPath))
                                                    {
                                                        int indx = tokenPath.IndexOf("[");
                                                        if (indx != -1)
                                                        {
                                                            parent = tokenPath.Split('[')[0];
                                                            tokenPath = tokenPath.Split('[')[1].Split(']')[0];
                                                        }
                                                        if (!String.IsNullOrEmpty(parent) && String.Equals(parent, "PBPView"))
                                                        {
                                                            if (isNextYear)
                                                                val = nextPBPViewJsonData.SelectToken(tokenPath) ?? String.Empty;
                                                            else
                                                                val = previousPBPViewJsonData.SelectToken(tokenPath) ?? String.Empty;

                                                            // replace path with path value
                                                            mainService[tokenName] = val;
                                                        }
                                                        else if (!String.IsNullOrEmpty(parent) && String.Equals(parent, "Medicare"))
                                                        {
                                                            if (isNextYear)
                                                                val = nextMedicareJsonData.SelectToken(tokenPath) ?? String.Empty;
                                                            else
                                                                val = previousMedicareJsonData.SelectToken(tokenPath) ?? String.Empty;
                                                            // replace path with path value
                                                            mainService[tokenName] = val;
                                                        }
                                                        else
                                                        {
                                                            if ((Convert.ToString(mainService[tokenName]) ?? string.Empty).Contains("[") || (Convert.ToString(mainService[tokenName]) ?? string.Empty).Contains("]"))
                                                            {
                                                                mainService[tokenName] = string.Empty;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return services;
        }
        private List<JToken> GetDefaultSlidingServices(bool isNextYear)
        {
            List<JToken> services = new List<JToken>();
            try
            {
                JObject slidingSer = (JObject)masterListANOCEOCJsonData.DeepClone();
                services = slidingSer.SelectToken("BenefitChart.ANOCCHARTDATASCS") == null ? new List<JToken>() : slidingSer.SelectToken("BenefitChart.ANOCCHARTDATASCS").ToList();
                // Filter by tier

                List<string> networkTierList = AnocHelper.GetNetworkTiers(isNextYear);
                services = (from ser in services
                            //where networkTierList.Any(s => s.Contains(ser.SelectToken("Tier").ToString().Trim()))
                            where networkTierList.Contains(ser.SelectToken("Tier").ToString().Trim())
                            select ser).ToList();

                //services = services.FindAll(x => Convert.ToString(x.SelectToken("Tier") ?? String.Empty) == "IN");

                List<JToken> defaultServices = new List<JToken>();
                defaultServices = RuleConstants.GetDefaultServices();
                // Filter only IN Tier and BRG services
                defaultServices = (from ser in defaultServices
                                   //where networkTierList.Any(s => s.Contains(ser.SelectToken("Tier").ToString().Trim()))
                                   where networkTierList.Contains(ser.SelectToken("Tier").ToString().Trim())
                                   && Convert.ToBoolean(ser.SelectToken("IsBRGService") ?? false) == false
                                   select ser).ToList();

                Dictionary<string, string> keys = new Dictionary<string, string>();
                keys.Add(RuleConstants.BenefitCategory1, RuleConstants.BenefitCategory1);
                keys.Add(RuleConstants.BenefitCategory2, RuleConstants.BenefitCategory2);
                keys.Add(RuleConstants.BenefitCategory3, RuleConstants.BenefitCategory3);
                keys.Add(RuleConstants.CostShareTiers, RuleConstants.CostShareTiers);

                CollectionExecutionComparer operatorProcessor = new CollectionExecutionComparer(services, defaultServices, keys);
                services = operatorProcessor.Intersection();

                if (null != services)
                {
                    for (int i = 0; i < services.Count; i++)
                    {
                        JToken mainService = services[i];
                        if (mainService != null)
                        {
                            List<JToken> columnList = mainService.ToList();
                            if (columnList != null)
                            {
                                string tokenName = String.Empty; string parent = String.Empty;
                                string tokenPath = String.Empty; JToken val = null;
                                foreach (var column in columnList)
                                {
                                    try
                                    {
                                        tokenName = String.Empty; tokenPath = String.Empty; parent = String.Empty; val = null;
                                        tokenName = ((JProperty)column).Name;
                                        tokenPath = Convert.ToString(((JProperty)column).Value ?? String.Empty);
                                        if (tokenName != RuleConstants.BenefitCategory1 && tokenName != RuleConstants.BenefitCategory2 && tokenName != RuleConstants.BenefitCategory3
                                        && tokenName != RuleConstants.CostShareTiers && tokenName != RuleConstants.Interval && tokenName != RuleConstants.RowIDProperty
                                        && tokenName != RuleConstants.CostShareType && tokenName != RuleConstants.IntervalCount && tokenName != RuleConstants.Filter)
                                        {
                                            if (!String.IsNullOrEmpty(tokenPath))
                                            {
                                                int indx = tokenPath.IndexOf("[");
                                                if (indx != -1)
                                                {
                                                    parent = tokenPath.Split('[')[0];
                                                    tokenPath = tokenPath.Split('[')[1].Split(']')[0];
                                                }
                                                if (!String.IsNullOrEmpty(parent) && String.Equals(parent, "PBPView"))
                                                {
                                                    if (isNextYear)
                                                        val = nextPBPViewJsonData.SelectToken(tokenPath) ?? String.Empty;
                                                    else
                                                        val = previousPBPViewJsonData.SelectToken(tokenPath) ?? String.Empty;
                                                    // replace path with path value
                                                    mainService[tokenName] = val;
                                                }
                                                else if (!String.IsNullOrEmpty(parent) && String.Equals(parent, "Medicare"))
                                                {
                                                    if (isNextYear)
                                                        val = nextMedicareJsonData.SelectToken(tokenPath) ?? String.Empty;
                                                    else
                                                        val = previousMedicareJsonData.SelectToken(tokenPath) ?? String.Empty;
                                                    // replace path with path value
                                                    mainService[tokenName] = val;
                                                }
                                                else
                                                {
                                                    if ((Convert.ToString(mainService[tokenName]) ?? string.Empty).Contains("[") || (Convert.ToString(mainService[tokenName]) ?? string.Empty).Contains("]"))
                                                    {
                                                        mainService[tokenName] = string.Empty;
                                                    }
                                                    else
                                                    {
                                                        // replace path with path value
                                                        mainService[tokenName] = val;
                                                    }
                                                }
                                                // replace path with path value
                                                //mainService[tokenName] = val;
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception)
            {
            }
            return services;
        }

        private bool IsServiceCovered(string expression, JToken service)
        {
            bool covered = false;
            try
            {
                if (!String.IsNullOrEmpty(expression))
                {
                    string[] inclusion = new string[] { ">", "<", "<=", ">=", "Contains", "=", "!=" };
                    string[] evaluateConditions = expression.Split(inclusion, System.StringSplitOptions.RemoveEmptyEntries);
                    if (evaluateConditions != null)
                    {
                        int length = evaluateConditions.Length;
                        if (length > 0)
                        {
                            string op = expression;
                            foreach (var item in evaluateConditions)
                            {
                                op = op.Replace(item, "");
                            }
                            if (!String.IsNullOrEmpty(op) && service != null)
                            {
                                string leftOp = String.Empty; string rightOp = String.Empty;
                                leftOp = Convert.ToString(service[evaluateConditions[0].Replace("{{:", "").Replace("}", "").Trim()] ?? String.Empty);
                                rightOp = Convert.ToString(service[evaluateConditions[1].Replace("{{:", "").Replace("}", "").Trim()] ?? String.Empty);
                                covered = ProcessExpression(leftOp, rightOp, op.Trim());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return covered;
        }

        private bool ProcessExpression(string leftOperand, string rightOperand, string op)
        {
            bool isValid = false;
            try
            {
                if (!String.IsNullOrEmpty(op))
                {
                    switch (op)
                    {
                        case ">":
                            isValid = GreaterThan(leftOperand, rightOperand);
                            break;
                        case "<":
                            isValid = LessThan(leftOperand, rightOperand);
                            break;
                        case ">=":
                            isValid = GreaterThanOrEqual(leftOperand, rightOperand);
                            break;
                        case "<=":
                            isValid = LessThanOrEqual(leftOperand, rightOperand);
                            break;
                        case "Contains":
                            isValid = Contains(leftOperand, rightOperand);
                            break;
                        case "=":
                            isValid = Equal(leftOperand, rightOperand);
                            break;
                        case "!=":
                            isValid = NotEqual(leftOperand, rightOperand);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return isValid;
        }

        // Expression Parser
        private bool Equal(string val1, string val2)
        {
            if (!String.IsNullOrEmpty(val2) && String.Equals(val2.ToLower().Trim(), "value"))
            {
                bool result = false;
                if (!String.IsNullOrEmpty(val1))
                    result = true;
                return result;
            }
            else
            {
                decimal n1, n2;
                bool result = decimal.TryParse(val1, out n1) && decimal.TryParse(val2, out n2) ? n1 == n2 : string.Equals(val1, val2);
                return result;
            }
        }
        private bool NotEqual(string val1, string val2)
        {
            decimal n1, n2;
            bool result = decimal.TryParse(val1, out n1) && decimal.TryParse(val2, out n2) ? n1 != n2 : !string.Equals(val1, val2);
            return result;
        }
        private bool GreaterThan(string val1, string val2)
        {
            bool result = false;
            decimal n1, n2;
            bool isArray = IsArray(val1);
            if (isArray)
            {
                int length = val1.Split(',').Count();
                decimal.TryParse(val2, out n2);
                result = length > n2;
            }
            else if (IsCostShare(val1, val2))
            {
                if (decimal.TryParse(ReplaceSymbol(val1), out n1) && decimal.TryParse(ReplaceSymbol(val2), out n2))
                {
                    result = n1 > n2;
                }
            }
            else
            {
                if (decimal.TryParse(val1, out n1) && decimal.TryParse(val2, out n2))
                {
                    result = n1 > n2;
                }
            }
            return result;
        }
        private bool GreaterThanOrEqual(string val1, string val2)
        {
            bool result = false;
            decimal n1, n2;
            bool isArray = IsArray(val1);
            if (isArray)
            {
                int length = val1.Split(',').Count();
                decimal.TryParse(val2, out n2);
                result = length >= n2;
            }
            else if (IsCostShare(val1, val2))
            {
                if (decimal.TryParse(ReplaceSymbol(val1), out n1) && decimal.TryParse(ReplaceSymbol(val2), out n2))
                {
                    result = n1 >= n2;
                }
            }
            else
            {
                if (decimal.TryParse(val1, out n1) && decimal.TryParse(val2, out n2))
                {
                    result = n1 >= n2;
                }
            }
            return result;
        }
        private bool LessThan(string val1, string val2)
        {
            bool result = false;
            decimal n1, n2;
            bool isArray = IsArray(val1);
            if (isArray)
            {
                int length = val1.Split(',').Count();
                decimal.TryParse(val2, out n2);
                result = length < n2;
            }
            else if (IsCostShare(val1, val2))
            {
                if (decimal.TryParse(ReplaceSymbol(val1), out n1) && decimal.TryParse(ReplaceSymbol(val2), out n2))
                {
                    result = n1 < n2;
                }
            }
            else
            {
                if (decimal.TryParse(val1, out n1) && decimal.TryParse(val2, out n2))
                {
                    result = n1 < n2;
                }
            }

            return result;
        }
        private bool LessThanOrEqual(string val1, string val2)
        {
            bool result = false;
            decimal n1, n2;
            bool isArray = IsArray(val1);
            if (isArray)
            {
                int length = val1.Split(',').Count();
                decimal.TryParse(val2, out n2);
                result = length <= n2;
            }
            else if (IsCostShare(val1, val2))
            {
                if (decimal.TryParse(ReplaceSymbol(val1), out n1) && decimal.TryParse(ReplaceSymbol(val2), out n2))
                {
                    result = n1 <= n2;
                }
            }
            else
            {
                if (decimal.TryParse(val1, out n1) && decimal.TryParse(val2, out n2))
                {
                    result = n1 <= n2;
                }
            }
            return result;
        }
        private bool Contains(string val1, string val2)
        {
            bool result = false;
            try
            {
                // If val1 is multiselct dropdown
                if (val1.StartsWith("[") && val1.EndsWith("]"))
                {
                    List<string> values = JToken.Parse(val1).ToObject<List<string>>();
                    if (values != null && values.Count > 0)
                    {
                        result = values.IndexOf(val2) > -1;
                    }
                }
                else
                {
                    result = val1.IndexOf(val2) > -1;
                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }
        private bool NotContains(string val1, string val2)
        {
            bool result = val1.IndexOf(val2) < 0;
            return result;
        }
        private bool IsNull(string value)
        {
            bool result = string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value);
            return result;
        }
        private bool IsArray(string leftOperand)
        {
            bool isArray = false;
            try
            {
                if (leftOperand != null)
                {
                    if (leftOperand.StartsWith("[") && leftOperand.EndsWith("]"))
                    {
                        isArray = true;
                    }
                }
            }
            catch (Exception ex)
            {
                isArray = false;
            }
            return isArray;
        }
        private bool IsCostShare(string leftOperand, string rightOperand)
        {
            bool isCostShare = false;
            try
            {
                if (leftOperand != null && rightOperand != null)
                {
                    if (leftOperand.IndexOf('$') > -1 && rightOperand.IndexOf('$') > -1) isCostShare = true;
                    else if (leftOperand.IndexOf('%') > -1 && rightOperand.IndexOf('%') > -1) isCostShare = true;
                }
            }
            catch (Exception ex)
            {
                isCostShare = false;
            }
            return isCostShare;
        }
        private string ReplaceSymbol(string val)
        {
            if (!string.IsNullOrEmpty(val))
            {
                return val = val.Replace("$", "").Replace("%", "");
            }
            else
            {
                return val;
            }
        }
        private string SetValueFormat(string val)
        {
            try
            {
                if (!String.IsNullOrEmpty(val))
                {
                    if (val.IndexOf(".") > -1)
                    {
                        string[] valArray = val.Split('.');
                        if (valArray != null && valArray.Length > 0)
                        {
                            if (valArray[1] != null && valArray[1] == "00")
                                val = valArray[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return val;
        }

        private void ProcessOONService(List<JToken> bRGServiceList, bool isNxtYr)
        {
            JToken SectionData = null;
            List<JToken> OONService = null;
            try
            {
                if (bRGServiceList != null && bRGServiceList.Count > 0)
                {
                    if (AnocHelper.IsPPOPlan(isNxtYr).Equals(true))
                    {
                        OONService = bRGServiceList.Where(s => Convert.ToString(s.SelectToken(RuleConstants.CostShareTiers) ??
                           string.Empty) == RuleConstants.OONPPO
                            &&
                           Convert.ToString(s.SelectToken(RuleConstants.EnhancedBenefit) ??
                           string.Empty) != string.Empty
                           &&
                           Convert.ToString(s.SelectToken(RuleConstants.PBPOONCode) ??
                           string.Empty) != string.Empty
                           )
                           .ToList();
                        SectionData = AnocHelper.GetSectionData(isNxtYr, RuleConstants.PPOPlanOONSectionName);
                        if (SectionData != null)
                        {
                            foreach (var item in OONService)
                            {
                                ProcessPPOPlanOONService(item, SectionData);
                            }
                            bRGServiceList.Except(bRGServiceList.Where(s => Convert.ToString(s.SelectToken(RuleConstants.CostShareTiers) ??
                                     string.Empty) == RuleConstants.OONPOS)
                                     .ToList());
                        }
                    }
                    else
                    {
                        OONService = bRGServiceList.Where(s => Convert.ToString(s.SelectToken(RuleConstants.CostShareTiers) ??
                                     string.Empty) == RuleConstants.OONPOS
                                      &&
                                     Convert.ToString(s.SelectToken(RuleConstants.EnhancedBenefit) ??
                                     string.Empty) != string.Empty
                                     &&
                                     Convert.ToString(s.SelectToken(RuleConstants.PBPOONCode) ??
                                     string.Empty) != string.Empty
                                     )
                                     .ToList();
                        SectionData = AnocHelper.GetSectionData(isNxtYr, RuleConstants.POSPlanOONSectionName);
                        if (SectionData != null)
                        {

                            foreach (var item in OONService)
                            {
                                ProcessPOSPlanOONService(item, SectionData);
                            }
                            bRGServiceList.Except(bRGServiceList.Where(s => Convert.ToString(s.SelectToken(RuleConstants.CostShareTiers) ??
                                     string.Empty) == RuleConstants.OONPPO)
                                     .ToList());
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void ProcessPPOPlanOONService(JToken brgService, JToken sectionData)
        {
            try
            {
                List<JToken> Base1RepeaterData = sectionData.SelectToken(RuleConstants.OONGroupsBase1) == null ? new List<JToken>() : sectionData.SelectToken("OONGroupsBase1").ToList();
                List<JToken> Base2RepeaterData = sectionData.SelectToken(RuleConstants.OONGroupsBase2) == null ? new List<JToken>() : sectionData.SelectToken("OONGroupsBase2").ToList();
                if (Base1RepeaterData.Count > 0)
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(brgService.SelectToken(RuleConstants.EnhancedBenefit) ?? string.Empty).Trim()) && !string.IsNullOrEmpty(Convert.ToString(brgService.SelectToken("PBPOONCode") ?? string.Empty).Trim()))
                    {
                        JToken base1RowData = null;
                        string enhanceBenefitType = string.Empty;
                        enhanceBenefitType = Convert.ToString(brgService.SelectToken(RuleConstants.EnhancedBenefit) ?? string.Empty).Trim();

                        if (enhanceBenefitType.Equals(RuleConstants.MedicareCovered))
                        {
                            enhanceBenefitType = "SelecttheMedicarecoveredservicecategoriesincludedintheOONoptionforthis";
                        }
                        else if (enhanceBenefitType.Equals(RuleConstants.NonMedicareCovered))
                        {
                            enhanceBenefitType = "SelecttheNonMedicarecoveredservicecategoriesincludedintheOONoptionfort";
                        }
                        brgService["Tier"] = RuleConstants.OON;
                        base1RowData = Base1RepeaterData
                            .Where(s => (s.SelectToken(enhanceBenefitType) != null ? s.SelectToken(enhanceBenefitType).ToList() : new List<JToken>())
                            .Contains((Convert.ToString(brgService.SelectToken(RuleConstants.PBPOONCode) ?? string.Empty).Trim())))
                            .FirstOrDefault();

                        if (base1RowData == null)
                        {
                            foreach (JToken j in Base1RepeaterData)
                            {
                                if (j.SelectToken(enhanceBenefitType) != null)
                                {
                                    string oon = j.SelectToken(enhanceBenefitType).ToString().Replace("\"", "").Replace("[", "").Replace("]", "");
                                    string[] oongroups=oon.Split(new char[] { ';',','});
                                   
                                    foreach (string s in oongroups)
                                    {
                                        if (s.Replace("\r\n", string.Empty).Trim() == Convert.ToString(brgService.SelectToken(RuleConstants.PBPOONCode)))
                                        {
                                            base1RowData = j;
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (base1RowData != null)
                        {
                            brgService["PlanMaxAmount"] = Convert.ToString(base1RowData["Indicatemaximumplanbenefitcoverageamount"]) ?? string.Empty;

                            JToken base2RowData = (from base2row in Base2RepeaterData
                                                   where (Convert.ToString(base2row.SelectToken("OONGroupID")) ?? string.Empty).Trim()
                                                   .Equals((Convert.ToString(base1RowData.SelectToken("OONGroupID")) ?? string.Empty).Trim())
                                                   select base2row)
                                                   .FirstOrDefault();
                            brgService["Tier"] = RuleConstants.OON;
                            if (base2RowData != null)
                            {
                                brgService["MinCoinsurance"] = Convert.ToString(base2RowData["EnterMinimumCoinsurancePercentageforthisGroup"]) ?? string.Empty;
                                brgService["MaxCoinsurance"] = Convert.ToString(base2RowData["EnterMaximumCoinsurancePercentageforthisGroup"]) ?? string.Empty;
                                brgService["MinCopay"] = Convert.ToString(base2RowData["EnterMinimumCopaymentAmountforthisGroup"]) ?? string.Empty;
                                brgService["MaxCopay"] = Convert.ToString(base2RowData["EnterMaximumCopaymentAmountforthisGroup"]) ?? string.Empty;
                                brgService["Deductible"] = Convert.ToString(base2RowData["EnterDeductibleAmountforthisgroup"]) ?? string.Empty;
                            }
                            else
                            {
                                brgService["MinCoinsurance"] = string.Empty;
                                brgService["MaxCoinsurance"] = string.Empty;
                                brgService["MinCopay"] = string.Empty;
                                brgService["MaxCopay"] = string.Empty;
                                brgService["Deductible"] = string.Empty;
                                brgService["PlanMaxAmount"] = string.Empty;
                                
                            }
                            brgService["OOPM"] = string.Empty;
                            brgService["OOPMPeriodicity"] = string.Empty;
                            brgService["PlanMaxAmountPeriodicity"] = string.Empty;

                        }
                        else
                        {
                            brgService["MinCoinsurance"] = string.Empty;
                            brgService["MaxCoinsurance"] = string.Empty;
                            brgService["MinCopay"] = string.Empty;
                            brgService["MaxCopay"] = string.Empty;
                            brgService["PlanMaxAmount"] = string.Empty;
                            brgService["PlanMaxAmountPeriodicity"] = string.Empty;
                            brgService["Deductible"] = string.Empty;
                            brgService["OOPM"] = string.Empty;
                            brgService["OOPMPeriodicity"] = string.Empty;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void ProcessPOSPlanOONService(JToken brgService, JToken sectionData)
        {
            try
            {
                List<JToken> Base1RepeaterData = sectionData.SelectToken(RuleConstants.POSGroupsBase1) == null ? new List<JToken>() : sectionData.SelectToken("POSGroupsBase1").ToList();
                List<JToken> Base2RepeaterData = sectionData.SelectToken(RuleConstants.POSGroupsBase2) == null ? new List<JToken>() : sectionData.SelectToken("POSGroupsBase2").ToList();

                if (Base1RepeaterData.Count > 0)
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(brgService.SelectToken(RuleConstants.EnhancedBenefit) ?? string.Empty).Trim()) && !string.IsNullOrEmpty(Convert.ToString(brgService.SelectToken("PBPOONCode") ?? string.Empty).Trim()))
                    {
                        if (brgService.SelectToken("PBPOONCode").ToString().Equals("3-1"))
                        {

                        }

                        JToken base1RowData = null;
                        string enhanceBenefitType = string.Empty;

                        enhanceBenefitType = Convert.ToString(brgService.SelectToken(RuleConstants.EnhancedBenefit) ?? string.Empty).Trim();

                        if (enhanceBenefitType.Equals(RuleConstants.MedicareCovered))
                        {
                            enhanceBenefitType = "SelectalloftheMedicarecoveredServiceCategoriesthatapplytothePOS";
                        }
                        else if (enhanceBenefitType.Equals(RuleConstants.NonMedicareCovered))
                        {
                            enhanceBenefitType = "SelectalloftheNonMedicarecoveredServiceCategoriesthatapplytothePOS";
                        }

                        base1RowData = Base1RepeaterData.Where(s => Convert.ToString(s.SelectToken(enhanceBenefitType) ?? string.Empty)
                                    .Split(';').Contains(Convert.ToString(brgService.SelectToken(RuleConstants.PBPOONCode) ?? string.Empty).Trim()))
                                    .FirstOrDefault();
                        brgService["Tier"] = RuleConstants.OON;
                        if (base1RowData != null)
                        {
                            brgService["MinCoinsurance"] = Convert.ToString(base1RowData["EnterMinimumCoinsurancePercentageforthisGroup"]) ?? string.Empty;
                            brgService["MaxCoinsurance"] = Convert.ToString(base1RowData["EnterMaximumCoinsurancePercentageforthisGroup"]) ?? string.Empty;
                            brgService["MinCopay"] = Convert.ToString(base1RowData["EnterMinimumCopaymentAmountforthisGroup"]) ?? string.Empty;
                            brgService["MaxCopay"] = Convert.ToString(base1RowData["EnterMaximumCopaymentAmountforthisGroup"]) ?? string.Empty;

                            JToken base2RowData = (from base2row in Base2RepeaterData
                                                   where (Convert.ToString(base2row.SelectToken("POSGroupID")) ?? string.Empty).Trim()
                                                   .Equals((Convert.ToString(base1RowData.SelectToken("POSGroupID")) ?? string.Empty).Trim())
                                                   select base2row)
                                                   .FirstOrDefault();

                            if (base2RowData != null)
                            {
                                brgService["PlanMaxAmount"] = Convert.ToString(base2RowData["IndicateMaximumPlanBenefitCoverageamount"]) ?? string.Empty;
                                brgService["PlanMaxAmountPeriodicity"] = Convert.ToString(base2RowData["SelecttheMaximumPlanBenefitCoverageperiodicity"]) ?? string.Empty;
                                brgService["Deductible"] = Convert.ToString(base2RowData["IndicateDeductibleamountforPOSservices"]) ?? string.Empty;
                            }
                            else
                            {
                                brgService["PlanMaxAmount"] = string.Empty;
                                brgService["PlanMaxAmountPeriodicity"] = string.Empty;
                                brgService["Deductible"] = string.Empty;
                            }
                            brgService["OOPM"] = string.Empty;
                            brgService["OOPMPeriodicity"] = string.Empty;
                        }
                        else
                        {
                            brgService["MinCoinsurance"] = string.Empty;
                            brgService["MaxCoinsurance"] = string.Empty;
                            brgService["MinCopay"] = string.Empty;
                            brgService["MaxCopay"] = string.Empty;
                            brgService["PlanMaxAmount"] = string.Empty;
                            brgService["PlanMaxAmountPeriodicity"] = string.Empty;
                            brgService["Deductible"] = string.Empty;
                            brgService["OOPM"] = string.Empty;
                            brgService["OOPMPeriodicity"] = string.Empty;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private bool IsPreferredRetailApplicable(bool isNextYear)
        {
            string componenetPath = RuleConstants.MedicareRxTierComponent;
            bool isApplicable = false;

            if (!string.IsNullOrEmpty(componenetPath))
            {
                var applicableComponents = isNextYear ? nextPBPViewJsonData.SelectToken(componenetPath) : previousPBPViewJsonData.SelectToken(componenetPath);
                if (applicableComponents != null)
                {
                    var preferedComp = applicableComponents.Where(whr => whr.ToString() == RuleConstants.preferredRetailCode).Select(Sel => Sel);
                    isApplicable = (preferedComp != null && preferedComp.Count() > 0) ? true : false;
                }
            }
            return isApplicable;
        }

        private string TierFormatter(string tier)
        {

            string item = RuleConstants.DisplayTier.Where(s => s.Key.Equals(tier)).Select(s=>s.Value).FirstOrDefault();

            if (!string.IsNullOrEmpty(item))
            {
                return item;
            }
            else
            {
                return tier;
            }
        }
    }
    public class Variance
    {
        public string Prop { get; set; }
        public string previousYrVal { get; set; }
        public string nextYrVal { get; set; }
    }
}