
namespace tmg.equinox.web.ExpresionBuilder
{
    public static class ExpressionBuilderConstants
    {
        public const int AnchorFormDesignID = 2359;
        public const string SectionA = "SECTIONASECTIONA1";
        public const string ContractNumber = "SECTIONASECTIONA1.ContractNumber";
        public const string IsFoundation = "ProductRules.FoundationTemplateInformation.IsFoundationtemplate";
        public const string PlanEffectiveDate = "ProductRules.PlanInformation.PlanEffectiveDate";
        public const string BasePlanTemplate = "ProductRules.PlanInformation.BasePlanTemplate";
        public const string PlanName = "ProductRules.PlanInformation.PlanName";
        public const string PlanRenewalDate = "ProductRules.PlanInformation.PlanRenewalDate";
        public const int COMMERCIALRIDEROPTION = 2432;
        public const int COMMERCIALMEDICAL = 2405;
        public const int ANOCCHARTML = 2400;
    }

    public static class COMMERCIALMEDICAL
    {
        #region SECTIONNAME
        public const string CostShare = "CostShare";
        public const string PreventCostShare = "PreventCostShare";
        public const string AdditionalServices = "AdditionalServices";
        public const string BenefitReview = "BenefitReview";
        public const string DeductibleSec = "Deductible";
        public const string OutofPocketMaximum = "OutofPocketMaximum";
        public const string CoinsuranceSec = "Coinsurance";
        public const string Network = "Network";
        public const string StandardServices = "StandardServices";
        public const string CopaySec = "Copay";
        public const string ReductionofBenefitsSec = "ReductionofBenefits";
        public const string LimitSec = "Limits";
        public const string ProductRules = "ProductRules";
        public const string PlanInformation = "PlanInformation";
        public const string RxSelection = "RxSelection";
        public const string RxRiderSelection = "RxRiderSelection";
        public const string ChiroRiderSelection = "ChiroRiderSelection";
        public const string ConfigurationSection = "Configuration";
        public const string AncillarySelection = "AncillarySelection";
        public const string CascadingCostShare = "CascadingCostShare";
        #endregion

        #region REPEATERNAME
        public const string PreventServicescovered = "PreventServicescovered";
        public const string BenefitReviewGrid = "BenefitReviewGrid";
        public const string DeductibleList = "DeductibleList";
        public const string OutofPocketMaximumList = "OutofPocketMaximumList";
        public const string CoinsuranceList = "CoinsuranceList";
        public const string AdditionalServiceList = "AdditionalServiceList";
        public const string NetworkTierList = "NetworkTierList";
        public const string StandardServiceList = "StandardServiceList";
        public const string CopayList = "CopayList";
        public const string WhatisthePlansReductionofBenefitAmount = "WhatisthePlansReductionofBenefitAmount";
        public const string LimitInformation = "LimitInformation";
        public const string LimitInformationDetail = "LimitInformationDetail";
        public const string RxRiderDetails = "RxRiderDetails";
        public const string ChiroRiderDetails = "ChiroRiderDetails";
        public const string LimitSummary = "LimitSummary";

        public const string NonHDHPDeductibleServicesList = "NonHDHPDeductibleServicesList";
        public const string ROBExceptionList = "ROBExceptionList";

        #endregion

        #region ELEMENTNAME
        public const string NetworkTier = "NetworkTier";
        public const string HowPreventServicescovered = "HowPreventServicescovered";
        public const string Copay = "Copay";
        public const string Coinsurance = "Coinsurance";
        public const string Deductible = "Deductible";
        public const string ReductionofBenefits = "ReductionofBenefits";
        public const string OOPM = "OOPM";
        public const string DeductibleAmount = "DeductibleAmount";
        public const string OOPMAmount = "OOPMAmount";
        public const string CoinsuranceAmount = "CoinsuranceAmount";
        public const string Limits = "Limits";
        public const string IndDeductible = "IndDeductible";
        public const string FamDeductible = "FamDeductible";
        public const string IndOOPM = "IndOOPM";
        public const string FamilyOOPM = "FamilyOOPM";
        public const string PriorApproval = "PriorApproval";
        public const string CopayType = "CopayType";
        public const string CopayAmount = "CopayAmount";
        public const string CoverageName = "CoverageName";
        public const string ApplyCopayType = "ApplyCopayType";
        public const string ReductionofBenefitsAmount = "ReductionofBenefitsAmount";
        public const string DoesthisplanhaveaReductionofBenefits = "DoesthisplanhaveaReductionofBenefits";
        public const string StandardROB = "StandardROB";
        public const string LimitDescription = "LimitDescription";
        public const string LimitAmount = "LimitAmount";
        public const string LimitType = "LimitType";
        public const string LimitFrequency = "LimitFrequency";
        public const string BasePlanType = "BasePlanTemplate";
        public const string MarketType = "MarketType";
        public const string InNetworkIndOOPM = "InNetworkIndOOPM";
        public const string InNetworkFamOOPM = "InNetworkFamOOPM";
        public const string OutofNetworkIndOOPM = "OutofNetworkIndOOPM";
        public const string OutofNetworkFamOOPM = "OutofNetworkFamOOPM";
        public const string EmbeddedDeductible = "EmbeddedDeductible";
        public const string Comments = "Comments";
        public const string Limit = "Limit";
        public const string FamOOPM = "FamOOPM";
        public const string PlanIndDeductible = "PlanIndDeductible";
        public const string PlanFamDeductible = "PlanFamDeductible";
        public const string PlanDeductibleAppliestoRx = "PlanDeductibleAppliestoRx";
        public const string RxIndDeductible = "RxIndDeductible";
        public const string RxFamDeductible = "RxFamDeductible";
        public const string Generic = "Generic";
        public const string RetailGeneric = "RetailGeneric";
        public const string RetailFormulary = "RetailFormulary";
        public const string RetailNonFormulary = "RetailNonFormulary";
        public const string MailOrderGeneric = "MailOrderGeneric";
        public const string MailOrderFormulary = "MailOrderFormulary";
        public const string MailOrderNonFormulary = "MailOrderNonFormulary";
        public const string RxDeductibleIndividual = "RxDeductibleIndividual";
        public const string RxDeductibleFamily = "RxDeductibleFamily";
        public const string RxDeductibleEmbedded = "RxDeductibleEmbedded";
        public const string PlanOOPMInd = "PlanOOPMInd";
        public const string PlanOOPFam = "PlanOOPFam";
        public const string SelectRxRider = "SelectRxRider";
        public const string SelectChiroRider = "SelectChiroRider";
        public const string MarketSegment = "MarketSegment";
        public const string SelectService = "SelectService";

        public const string PlanType = "TypeofPlan";
        public const string HMOOptions = "HMOOptions";
        public const string PPOOptions = "PPOOptions";
        //public const string  
        #endregion

        #region Data
        public const string InPlan = "In Plan";
        public const string OutofPlan = "Out of Plan";
        public const string PreventCare = "Preventive Care";
        public const string HUNDAREDPERCENTAGE = "100%";
        public const string SubjecttoDeductible = "Subject to Deductible";
        public const string SubjecttoCoinsuranceANDDeductible = "Subject to Coinsurance & Deductible";
        public const string Coveredat100 = "Cover at 100%";
        public const string Individual = "Individual";
        public const string Family = "Family";
        public const string FullyFunded = "Fully Funded";
        public const string TeleHealthCopayType = "Telehealth";

        #endregion

        #region EELEMENTFULLPATH
        public const string SelectRxRiderPath = "RxSelection.SelectRxPackage";
        public const string SelectChiroRiderPath = "Riders.ChiroRiderSelection.SelectChiroRider";

        #endregion
    }
    public static class MASTERLISTRIDEROPTION
    {
        public const string ChiroRiderOption = "ChiroRiderOption";
        public const string RxRiderOption = "RxPackages";

        public const string ChiroRiderOptionList = "ChiroRiderDetails";
        public const string RxRiderOptionList = "RxPackageBasePlanAssociation";

        public const string BasePlan = "BasePlanName";
        public const string RXridername = "RxPackageName";
        public const string ChiroRider = "ChiroRider";
    }
}