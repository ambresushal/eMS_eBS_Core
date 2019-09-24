using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.pbpimportservices
{
    public static class PBPMappingConstants
    {

    }

    public static class DATASOURCENAME
    {
        public const string IQMedicareNetworkList = "IQMedicareNetWorkList";
    }

    public static class KEYNAME
    {
        public const string BenefitCategory1 = "BenefitCategory1";
        public const string BenefitCategory2 = "BenefitCategory2";
        public const string BenefitCategory3 = "BenefitCategory3";
    }

    public static class STANDARDVALUE
    {
        public const string NotApplicable = "Not Applicable";
        public const string NotSelected = "Not Selected";
        public const string Blank = "";
        public const string ZeroDoller = "$0";
        public const string Zero = "0";
        public const string Yes = "Yes";
        public const string No = "No";
        public const string False = "false";
        public const string True = "true";
    }

    public static class SECTIONNAME
    {
        public const string SlidingCostShareSec = "SlidingCostShare";
        public const string MedicareBRG = "BenefitReview";
        public const string PlanInformation = "PlanInformation";
        public const string Tiers = "Tiers";
        public const string CostShare = "CostShare";
    }

    public static class REPEATERNAME
    {
        public const string BenefitReviewGrid = "BenefitReviewGrid";
        public const string SelectthePlansCostShareTiers = "SelectthePlansCostShareTiers";
        public const string SlidingCostShareIntervals = "SlidingCostShareIntervals";
        public const string SlidingCostShareInformation = "SlidingCostShareInformation";
        public const string GeneralCostShareInformation = "GeneralCostShareInformation";
        public const string MaximumOutofPocketList = "MaximumOutofPocketList";
        public const string StandardRetailCostSharingInformation = "StandardRetailCostSharingInformation";
        public const string StandardMailOrderCostSharing = "StandardMailOrderCostSharing";
        public const string LongtermcareLTCcostsharingList = "LongtermcareLTCcostsharingList";
        public const string Outofnetworkcostsharing = "Outofnetworkcostsharing";
    }

    public static class ELEMENTNAME
    {
        public const string PreAuthorization = "PreAuthorization";
        public const string SlidingCostShare = "SlidingCostShare";
        public const string PlanType = "PlanType";
        public const string CostShareTiers = "CostShareTiers";
        public const string Deductible = "Deductible";
        public const string MinimumCopay = "MinimumCopay";
        public const string MaximumCopay = "MaximumCopay";
        public const string MinimumCoinsurance = "MinimumCoinsurance";
        public const string MaximumCoinsuarnce = "MaximumCoinsuarnce";
        public const string OOPMValue = "OOPMValue";
        public const string OOPMPeriodicity = "OOPMPeriodicity";
        public const string MaximumPlanBenefitCoverageAmount = "MaximumPlanBenefitCoverageAmount";
        public const string MaximumPlanBenefitCoveragePeriodicity = "MaximumPlanBenefitCoveragePeriodicity";
        public const string BenefitPeriod = "BenefitPeriod";
        public const string IsthisBenefitUnlimited = "IsthisBenefitUnlimited";
        public const string IntervalNumber = "IntervalNumber";
        public const string MaximumOutofPocketAmount = "MaximumOutofPocketAmount";
        public const string MedicarecoveredZeroDollarPreventiveServicesAttestation = "MedicarecoveredZeroDollarPreventiveServicesAttestation";
        public const string Isthereadeductible = "Isthereadeductible";
        public const string Retailcostsharing = "Retailcostsharing";
        public const string Mailordercostsharing = "Mailordercostsharing";
        public const string Longtermcarecostsharing = "Longtermcarecostsharing";
        public const string OptionOutofnetworkcostsharing = "OptionOutofnetworkcostsharing";
        public const string CostShareTiersInformation = "CostShareTiersInformation";
        public const string Listeachtierforwhichthedeductibleapplies = "Listeachtierforwhichthedeductibleapplies";
        public const string Tiers = "Tiers";
        public const string TierDescription = "TierDescription";
    }

    public static class DATA
    {
        public const string InNetwork = "In Network";
        public const string OutOfNetwork = "Out of Network";
        public const string HMOPOS = "HMOPOS";
        public const string PreventiveandOtherDefinedSupplementalServices = "Preventive and Other Defined Supplemental Services";
        public const string MedicarecoveredZeroDollarPreventiveServices = "Medicare-covered Zero Dollar Preventive Services";
    }

    public static class FIELDPATH
    {
        public const string AdditionalPrescriptionCostShareInformation = "PrescriptionDrugPartD.AdditionalPrescriptionCostShareInformation";
        public const string GapCoverageInformation = "PrescriptionDrugPartD.GapCoverageInformation";
    }
}
