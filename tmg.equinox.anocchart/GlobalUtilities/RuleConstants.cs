using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace tmg.equinox.anocchart.GlobalUtilities
{
    public static class RuleConstants
    {
        public const string PlanYear = "Plan Year";
        public const string PlanName = "Plan Name";
        public const string PrescriptionDeductible = "Prescription Deductible";
        public const string GapCoverageAmount = "Gap Coverage Amount";
        public const string RxNumbersOfTiers= "RxNumberOfTiers";
        public const string PlanDeductibleAmount = "PlanDeductible";
        public const string IHSBenefitCategory1 = "Inpatient Hospital Services";
        public const string IHSBenefitCategory2 = "Acute";
        public const string IHSBenefitCategory3 = "Medicare-covered stay";
        public const string PreferredRetailApplicable = "IsPreferredRetailApplicable";
        public const string AdditionalDays = "Additional Days";

        public const string MonthlyPlanPremium = "Monthly plan premium";
        public const string MaxOOPAmount = "Out of Pocket Maximum";
        public const string NotCovered = "Not Covered";
        public const string NotApplicable = "Not Applicable";
        public const string Dollar = "$";
        public const string Percent = "%";
        public const string DeductibleAppliedTier = "List each tier for which the deductible applies";
        public const string CatastrophicAmount = "Catastrophic Coverage Amount";
        public const string PlanType = "SectionA.SectionA1.PlanType";
        public const string NetworkTier = "SECTIONASECTIONA1.Tiers";
        public const string NumberOfGrups = "OONNumberofGroups.IndicatethenumberofOutofNetworkgroupingsofferedexcludingInpatientHospi";
        public const string OON = "OON";
        public const string InpatientAcuteMaxOOPAmount = "Inpatient Acute Max OOP Amount";
        public const string InaptientAcuteMaxOOPPeriodicity = "Inaptient Acute Max OOP Periodicity";


        #region section name
        public const string PlanInformation = "PlanInformation";
        public const string RxPlan = "DoesyourplanofferaMedicarePrescriptiondrugPartDbenefit";
        public const string Premiums = "Premiums";
        public const string CostShare = "CostShare";
        public const string BenefitReview = "BenefitReview";
        public const string GeneralCostShareInformation = "GeneralCostShareInformation";
        public const string PrescriptionDrugPartD = "PrescriptionDrugPartD";
        public const string PrescriptionDeductibleSection = "PrescriptionDeductible";
        public const string GapCoverageInformationSection = "GapCoverageInformation";
        public const string AdditionalPrescriptionCostShareInformation = "AdditionalPrescriptionCostShareInformation";
        public const string POSPlanOONSectionName = "POSGroups";
        public const string PPOPlanOONSectionName = "OONGroups";

        #endregion section name

        #region element name
        public const string Tier = "Tier";
        public const string Tiers = "Tiers";
        public const string TierDescription = "TierDescription";
        public const string PlanNameElement = "PlanName";
        public const string PlanYearElement = "PlanYear";
        public const string Coinsurance = "Coinsurance";
        public const string MinimumCoinsurance = "MinimumCoinsurance";
        public const string MaximumCoinsuarnce = "MaximumCoinsuarnce";
        public const string MonthlyPremiumAmount = "MonthlyPremiumAmount";
        public const string CostShareTiers = "Tier";
        public const string Interval = "Interval";
        public const string CostShareType = "CostShareType";
        public const string MaximumOutofPocketAmount = "MaximumOutofPocketAmount";
        public const string PrescriptionDeductibleAmount = "PrescriptionDeductibleAmount";
        public const string Onemonthsupply = "Onemonthsupply";
        public const string RowIDProperty = "RowIDProperty";
        public const string BenefitCategory1 = "BenefitCategory1";
        public const string BenefitCategory2 = "BenefitCategory2";
        public const string BenefitCategory3 = "BenefitCategory3";
        public const string IQMedicareNetWorkList = "IQMedicareNetWorkList";
        public const string CoverageGapAmount = "CoverageGapAmount";
        public const string OOPMValue = "OOPM";
        public const string OOPMPeriodicity = "OOPMPeriodicity";
        public const string MaximumPlanBenefitCoverageAmount = "PlanMaxAmount";
        public const string MaximumPlanBenefitCoveragePeriodicity = "PlanMaxAmountPeriodicity";
        public const string MinCopay = "MinCopay";
        public const string MaxCopay = "MaxCopay";
        public const string MinCoinsurance = "MinCoinsurance";
        public const string MaxCoinsurance = "MaxCoinsurance";
        public const string OOPM = "OOPM";
        public const string OOPM_Periodicity = "OOPM Periodicity";
        public const string MaxPlanBenefitAmount = "Max Plan Benefit Amount";
        public const string MaximunPlanBenefitPeriodicity = "Maximun Plan Benefit Periodicity";
        public const string ServiceGroup = "ServiceGroup";
        public const string SequenceNo = "SequenceNo";
        public const string PBPServiceCode = "PBPServiceCode";
        public const string Auths = "Auths";
        public const string Referrals = "Referrals";
        public const string Filter = "Filter";
        public const string IntervalCount = "IntervalCount";
        public const string ServiceSelectionMapping = "ServiceSelectionMapping";
        public const string PBPCode = "PBPCode";
        public const string Condition = "Condition";
        public const string PBPOONCode = "PBPOONCode";
        public const string MedicareCovered="Medicare Covered";
        public const string NonMedicareCovered = "Non-Medicare Covered";
        public const string EnhancedBenefit = "EnhancedBenefit";
        public const string OONPPO = "OON-PPO";
        public const string OONPOS = "OON-POS";
        public const string BeginDay = "BeginDay";
        public const string EndDay = "EndDay";
        public const string Amount = "Amount";
        public const string preferredRetailCode = "001000";
        public const string VAR1 = "VAR1";
        public const string VAR2 = "VAR2";

        #endregion element name


        #region repeater name

        public const string MaximumOutofPocketList = "MaximumOutofPocketList";
        public const string Listeachtierforwhichthedeductibleapplies = "Listeachtierforwhichthedeductibleapplies";
        public const string StandardRetailCostSharingInformation = "StandardRetailCostSharingInformation";
        public const string BenefitReviewGrid = "BenefitReviewGrid";
        public const string OONGroupsBase1 = "OONGroupsBase1";
        public const string OONGroupsBase2 = "OONGroupsBase2";
        public const string POSGroupsBase1 = "POSGroupsBase1";
        public const string POSGroupsBase2 = "POSGroupsBase2";
        #endregion repeater name

        #region repeater fullpath
        public const string ANOCChartPlanInformation = "ANOCChartPlanDetails.PlanInformation";
        public const string ANOCChartPlanPremiumInformation = "ANOCChartPlanDetails.PlanPremiumInformation";
        public const string ANOCChartOutofPocketInformation = "ANOCChartPlanDetails.OutofPocketInformation";
        public const string ANOCChartANOCBenefitsCompare = "ANOCChartPlanDetails.ANOCBenefitsCompare";
        public const string ANOCChartPrescriptionInformation = "ANOCChartPlanDetails.PrescriptionInformation";
        public const string ANOCChartInitialCoverageLimitInformation = "ANOCChartPlanDetails.PreInitialCoverageLimitInformation";
        public const string ANOCChartGapCoverageInformation = "ANOCChartPlanDetails.GapCoverageInformation";
        public const string MaximumOutofPocketListGridFullpath = "CostShare.GeneralCostShareInformation.MaximumOutofPocketList";
        public const string BenefitReviewGridFullpath = "BenefitReview.BenefitReviewGrid";
        public const string ListeachtierforwhichthedeductibleappliesGridFullpath = "PrescriptionDrugPartD.PrescriptionDeductible.Listeachtierforwhichthedeductibleapplies";
        public const string StandardRetailCostSharingInformationGridFullpath = "PrescriptionDrugPartD.AdditionalPrescriptionCostShareInformation.StandardRetailCostSharingInformation";
        public const string ANOCChartDoctorOfficeVisitInfoPath= "ANOCChartPlanDetails.DoctorOfficeVisitInfo";
        public const string ANOCChartIHSInfoPath = "ANOCChartPlanDetails.InpatientHospitalStayInfo";
        public const string ANOCChartPreffedRetailInformation = "ANOCChartPlanDetails.PreferredRetailInformation";
        public const string MedicareRxTierComponent = "MedicareRxGeneral.MedicareRxGeneral1.Describethecomponentsofyournetworkselectallthatapply";
        public const string ANOCChartPlanDetailsCostShareDetails="ANOCChartPlanDetails.CostShareDetails";

        public const string SlidingCostShareIntervalsGridFullpath = "CostShare.SlidingCostShare.SlidingCostShareIntervals";
        public const string SlidingCostShareInformationGridFullpath = "CostShare.SlidingCostShare.SlidingCostShareInformation";

        #endregion repeater fullpath

        #region Static methods 
        public static Dictionary<string, string> GetTokenList()
        {
            Dictionary<string, string> tokenDict = new Dictionary<string, string>();
            tokenDict.Add("Deductible", "Deductible");
            tokenDict.Add("MinimumCopay", "Min Copay");
            tokenDict.Add("MaximumCopay", "Max Copay");
            tokenDict.Add("MinimumCoinsurance", "Min Coinsurance");
            tokenDict.Add("MaximumCoinsuarnce", "Max Coinsurance");
            tokenDict.Add("OOPMValue", "OOPM");
            tokenDict.Add("OOPMPeriodicity", "OOPM Periodicity");
            tokenDict.Add("PlanMaxAmount", "Max Plan Benefit Amount");
            tokenDict.Add("PlanMaxAmountPeriodicity", "Maximun Plan Benefit Periodicity");

            tokenDict.Add("CopayBeginDay", "Copay Begin Day");
            tokenDict.Add("CopayEndDay", "Copay End Day");
            tokenDict.Add("CoinsuranceBeginDay", "Coinsurance Begin Day");
            tokenDict.Add("CoinsuranceEndDay", "Coinsurance End Day");
            tokenDict.Add("Copay", "Copay");
            tokenDict.Add("Coinsurance", "Coinsurance");

            return tokenDict;
        }

        public static List<JToken> GetDefaultServices()
        {
            List<JToken> defaultServices = new List<JToken>();

            JObject obj = JObject.Parse("{'BenefitCategory1':'','BenefitCategory2':'','BenefitCategory3':''}");
            obj[RuleConstants.BenefitCategory1] = "Health Care Professional Services";
            obj[RuleConstants.BenefitCategory2] = "Primary Care Physician Services";
            obj[RuleConstants.BenefitCategory3] = "Medicare-covered benefits";
            obj[RuleConstants.CostShareTiers] = "IN";
            obj["IsBRGService"] = true;
            defaultServices.Add(obj);

            obj = JObject.Parse("{'BenefitCategory1':'','BenefitCategory2':'','BenefitCategory3':''}");
            obj[RuleConstants.BenefitCategory1] = "Health Care Professional Services";
            obj[RuleConstants.BenefitCategory2] = "Primary Care Physician Services";
            obj[RuleConstants.BenefitCategory3] = "Medicare-covered benefits";
            obj[RuleConstants.CostShareTiers] = "IN-Tier 1";
            obj["IsBRGService"] = true;
            defaultServices.Add(obj);

            obj = JObject.Parse("{'BenefitCategory1':'','BenefitCategory2':'','BenefitCategory3':''}");
            obj[RuleConstants.BenefitCategory1] = "Health Care Professional Services";
            obj[RuleConstants.BenefitCategory2] = "Primary Care Physician Services";
            obj[RuleConstants.BenefitCategory3] = "Medicare-covered benefits";
            obj[RuleConstants.CostShareTiers] = "IN-Tier 2";
            obj["IsBRGService"] = true;
            defaultServices.Add(obj);

            obj = JObject.Parse("{'BenefitCategory1':'','BenefitCategory2':'','BenefitCategory3':''}");
            obj[RuleConstants.BenefitCategory1] = "Health Care Professional Services";
            obj[RuleConstants.BenefitCategory2] = "Primary Care Physician Services";
            obj[RuleConstants.BenefitCategory3] = "Medicare-covered benefits";
            obj[RuleConstants.CostShareTiers] = "IN-Tier 3";
            obj["IsBRGService"] = true;
            defaultServices.Add(obj);

            obj = JObject.Parse("{'BenefitCategory1':'','BenefitCategory2':'','BenefitCategory3':''}");
            obj[RuleConstants.BenefitCategory1] = "Health Care Professional Services";
            obj[RuleConstants.BenefitCategory2] = "Primary Care Physician Services";
            obj[RuleConstants.BenefitCategory3] = "Medicare-covered benefits";
            obj[RuleConstants.CostShareTiers] = "OON";
            obj["IsBRGService"] = true;
            defaultServices.Add(obj);


            obj = JObject.Parse("{'BenefitCategory1':'','BenefitCategory2':'','BenefitCategory3':''}");
            obj[RuleConstants.BenefitCategory1] = "Health Care Professional Services";
            obj[RuleConstants.BenefitCategory2] = "Physician Specialist Services";
            obj[RuleConstants.BenefitCategory3] = "Medicare-covered benefits";
            obj[RuleConstants.CostShareTiers] = "IN";
            obj["IsBRGService"] = true;
            defaultServices.Add(obj);

            obj = JObject.Parse("{'BenefitCategory1':'','BenefitCategory2':'','BenefitCategory3':''}");
            obj[RuleConstants.BenefitCategory1] = "Health Care Professional Services";
            obj[RuleConstants.BenefitCategory2] = "Physician Specialist Services";
            obj[RuleConstants.BenefitCategory3] = "Medicare-covered benefits";
            obj[RuleConstants.CostShareTiers] = "IN-Tier 1";
            obj["IsBRGService"] = true;
            defaultServices.Add(obj);

            obj = JObject.Parse("{'BenefitCategory1':'','BenefitCategory2':'','BenefitCategory3':''}");
            obj[RuleConstants.BenefitCategory1] = "Health Care Professional Services";
            obj[RuleConstants.BenefitCategory2] = "Physician Specialist Services";
            obj[RuleConstants.BenefitCategory3] = "Medicare-covered benefits";
            obj[RuleConstants.CostShareTiers] = "IN-Tier 2";
            obj["IsBRGService"] = true;
            defaultServices.Add(obj);

            obj = JObject.Parse("{'BenefitCategory1':'','BenefitCategory2':'','BenefitCategory3':''}");
            obj[RuleConstants.BenefitCategory1] = "Health Care Professional Services";
            obj[RuleConstants.BenefitCategory2] = "Physician Specialist Services";
            obj[RuleConstants.BenefitCategory3] = "Medicare-covered benefits";
            obj[RuleConstants.CostShareTiers] = "IN-Tier 3";
            obj["IsBRGService"] = true;
            defaultServices.Add(obj);

            obj = JObject.Parse("{'BenefitCategory1':'','BenefitCategory2':'','BenefitCategory3':''}");
            obj[RuleConstants.BenefitCategory1] = "Health Care Professional Services";
            obj[RuleConstants.BenefitCategory2] = "Physician Specialist Services";
            obj[RuleConstants.BenefitCategory3] = "Medicare-covered benefits";
            obj[RuleConstants.CostShareTiers] = "OON";
            obj["IsBRGService"] = true;
            defaultServices.Add(obj);

            obj = JObject.Parse("{'BenefitCategory1':'','BenefitCategory2':'','BenefitCategory3':''}");
            obj[RuleConstants.BenefitCategory1] = "Inpatient Hospital Services";
            obj[RuleConstants.BenefitCategory2] = "Acute";
            obj[RuleConstants.BenefitCategory3] = "Medicare-covered stay";
            obj[RuleConstants.CostShareTiers] = "IN";
            obj["IsBRGService"] = false;
            defaultServices.Add(obj);

            obj = JObject.Parse("{'BenefitCategory1':'','BenefitCategory2':'','BenefitCategory3':''}");
            obj[RuleConstants.BenefitCategory1] = "Inpatient Hospital Services";
            obj[RuleConstants.BenefitCategory2] = "Acute";
            obj[RuleConstants.BenefitCategory3] = "Medicare-covered stay";
            obj[RuleConstants.CostShareTiers] = "IN-Tier 1";
            obj["IsBRGService"] = false;
            defaultServices.Add(obj);

            obj = JObject.Parse("{'BenefitCategory1':'','BenefitCategory2':'','BenefitCategory3':''}");
            obj[RuleConstants.BenefitCategory1] = "Inpatient Hospital Services";
            obj[RuleConstants.BenefitCategory2] = "Acute";
            obj[RuleConstants.BenefitCategory3] = "Medicare-covered stay";
            obj[RuleConstants.CostShareTiers] = "IN-Tier 2";
            obj["IsBRGService"] = false;
            defaultServices.Add(obj);

            obj = JObject.Parse("{'BenefitCategory1':'','BenefitCategory2':'','BenefitCategory3':''}");
            obj[RuleConstants.BenefitCategory1] = "Inpatient Hospital Services";
            obj[RuleConstants.BenefitCategory2] = "Acute";
            obj[RuleConstants.BenefitCategory3] = "Medicare-covered stay";
            obj[RuleConstants.CostShareTiers] = "IN-Tier 3";
            obj["IsBRGService"] = false;
            defaultServices.Add(obj);

            obj = JObject.Parse("{'BenefitCategory1':'','BenefitCategory2':'','BenefitCategory3':''}");
            obj[RuleConstants.BenefitCategory1] = "Inpatient Hospital Services";
            obj[RuleConstants.BenefitCategory2] = "Acute";
            obj[RuleConstants.BenefitCategory3] = "Medicare-covered stay";
            obj[RuleConstants.CostShareTiers] = "OON";
            obj["IsBRGService"] = false;
            defaultServices.Add(obj);

            return defaultServices;
        }

        public static Dictionary<string, string> GetPeriodicityList()
        {
            Dictionary<string, string> tokenDict = new Dictionary<string, string>();
            tokenDict.Add("1", "Every Three Years");
            tokenDict.Add("2", "Every Two Years");
            tokenDict.Add("3", "Every Year");
            tokenDict.Add("4", "Every Six Months");
            tokenDict.Add("5", "Every Three Months");
            tokenDict.Add("6", "Other, Describe");
            return tokenDict;
        }

        public static Dictionary<string, string> GetPeriodicityListForAcuteList()
        {
            Dictionary<string, string> tokenDict = new Dictionary<string, string>();
            tokenDict.Add("1", "Every three years");
            tokenDict.Add("2", "Every two years");
            tokenDict.Add("3", "Every year");
            tokenDict.Add("4", "Every six months");
            tokenDict.Add("5", "Every three months");
            tokenDict.Add("6", "Every Benefit Period");
            tokenDict.Add("7", "Every Stay");
            tokenDict.Add("8", "Other, Describe");
            return tokenDict;
        }

        public static Dictionary<string, string> GetPPOPlanList()
        {
            Dictionary<string, string> tokenDict = new Dictionary<string, string>();
            tokenDict.Add("4", "PPO");
            tokenDict.Add("31", "Regional PPO");
            tokenDict.Add("47", "Employer Direct PPO");
            return tokenDict;
        }

        public static Dictionary<string, string> GetPOSPlanList()
        {
            Dictionary<string, string> tokenDict = new Dictionary<string, string>();
            tokenDict.Add("2", "HMOPOS");
            tokenDict.Add("43", "RFB HMOPOS");
            tokenDict.Add("49", "MMP HMOPOS");
            return tokenDict;
        }


        public static Dictionary<string, string> DisplayTier = new Dictionary<string, string> {
                                                    {"IN", " In-" },
                                                    {"OON", "Out-of-" },
                                                    {"IN-Tier 1", "IN-Tier 1-" },
                                                    {"IN-Tier 2", "IN-Tier 2-" },
                                                    {"IN-Tier 3", "IN-Tier 3-" }
                                                    };

        #endregion Static methods 
    }
}

