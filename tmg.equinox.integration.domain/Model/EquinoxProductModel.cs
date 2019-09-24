using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.domain.Models
{
    public class GeneralInformation
    {
        public string ProductName { get; set; }
        public string PlanMarketingName { get; set; }
        public string ProductType { get; set; }
        public string ApplicationType { get; set; }
        public string ProductSegment { get; set; }
        public string ProductContact { get; set; }
        public string Blank { get; set; }
        public string SalesEffectiveDate { get; set; }
        public string ClaimsEffectiveDate { get; set; }
        public string ClaimsCancelDate { get; set; }
        public string PlanExpirationDate { get; set; }
    }

    public class CDHInformation
    {
        public string ProductincludesCDH { get; set; }
        public string Blank { get; set; }
        public string CDHType { get; set; }
        public string CDHAdministrator { get; set; }
        public string CDHContribution { get; set; }
    }

    public class GlobalProductRules
    {
        public string Pleasecheckallthatapply { get; set; }
        public string Physicianofficeservicesmustbemedicallynecessary { get; set; }
        public string Applya9monthwaitingperiodforpreexistingconditionsadultonly { get; set; }
        public string Preexistingconditionwaitingperiodsdonotapplytomembersuptoage19 { get; set; }
        public string WaiveOutofNetworkcostshareformedicalemergencyandaccidentalinjuryservic { get; set; }
        public string WaiveOutofNetworkcostsharewhenamemberreceivesservicesfromanOutofNetwor { get; set; }
        public string WaiveOutofNetworkcostshareifamemberreceivesservicesfromaproviderforwhi { get; set; }
        public string WaiveOutofNetworkcostshareifthememberhasbeenreferredtoanOutofNetworkph { get; set; }
        public string DonotcoverservicesrenderedforWorkMansComp { get; set; }
        public string DonotcoverservicesrenderedforAutoAccidents { get; set; }
    }

    public class PDBCPrefixList
    {
        public string PDBCType { get; set; }
        public string PDBCPrefix { get; set; }
        public string SelectPrefix { get; set; }
        public string EffectiveDate { get; set; }
        public string CancelDate { get; set; }
        public int RowIDProperty { get; set; }
    }

    public class FacetProductComponentsPDBC
    {
        public List<PDBCPrefixList> PDBCPrefixList { get; set; }
    }

    public class FacetsProductInformation
    {
        public string ProductID { get; set; }
        public string LineofBusinessSwitchIndicator { get; set; }
        public string ProductLineofBusiness { get; set; }
        public string ProductAlternateLineofBusiness { get; set; }
        public string ProductAccumulatorSuffix { get; set; }
        public string CapitationPercentofPremiumLevel { get; set; }
        public string CapitationNewPayeeRetroactivityLimit { get; set; }
        public string CapitationCategory { get; set; }
        public string Blank { get; set; }
        public string ReferralsPreauthorizations { get; set; }
        public string PrePricing { get; set; }
        public string MedicalClaimsProcessing { get; set; }
        public string DentalPreDeterminations { get; set; }
        public string DentalClaimsProcessing { get; set; }
        public string ClinicalEdits { get; set; }
        public string CapitationRiskAllocation { get; set; }
        public string DOFR { get; set; }
        public string POSOptout { get; set; }
        public string PremiumIndicator { get; set; }
        public string StateDeterminationforClaimsInterest { get; set; }
        public string ProductBusinessCategory { get; set; }
        public string ProductValueCode1 { get; set; }
        public string ProductValueCode2 { get; set; }
        public FacetProductComponentsPDBC FacetProductComponentsPDBC { get; set; }
        public string Plugin { get; set; }
        public string Version { get; set; }
    }

    public class DiseaseManagementProgramsOffered
    {
        public string Asthma { get; set; }
        public string HeartDisease { get; set; }
        public string Depression { get; set; }
        public string Diabetes { get; set; }
        public string HighBloodPressureHighCholesterol { get; set; }
        public string LowBackPain { get; set; }
        public string PainManagement { get; set; }
        public string Pregnancy { get; set; }
    }

    public class GeneralDetails
    {
        public string PlanMarketingName { get; set; }
        public string HIOSPlanID { get; set; }
        public string HIOSProductID { get; set; }
        public string HPID { get; set; }
        public string NewExistingPlan { get; set; }
        public string QHPNonQHP { get; set; }
        public string FormularyID { get; set; }
        public string LevelofCoverage { get; set; }
        public string CSRVariationType { get; set; }
        public string UniquePlanDesign { get; set; }
        public string ChildOnlyOffering { get; set; }
        public string ChildOnlyPlanID { get; set; }
        public string TobaccoWellnessProgramOffered { get; set; }
        public DiseaseManagementProgramsOffered DiseaseManagementProgramsOffered { get; set; }
        public string IssuerActuarialValue { get; set; }
        public string NoticeRequiredforPregnancy { get; set; }
        public string Blank { get; set; }
        public string IsaReferralRequiredforSpecialist { get; set; }
        public string SpecialistsRequiringaReferral { get; set; }
        public string PlanLevelExclusions { get; set; }
        public string AVCalculatorOutputNumber { get; set; }
    }

    public class StandAloneDental
    {
        public string EHBApportionmentforPediatricDental { get; set; }
        public string GuaranteedvsEstimatedRate { get; set; }
    }

    public class Links
    {
        public string URLforSummaryofBenefitsCoverage { get; set; }
        public string Blank { get; set; }
        public string URLforEnrollmentPayment { get; set; }
        public string PlanBrochure { get; set; }
    }

    public class SBCScenarioCostSharing
    {
        public string Scenario { get; set; }
        public string CostSharingType { get; set; }
        public string CostSharingAmount { get; set; }
        public int RowIDProperty { get; set; }
    }

    public class SBCScenario
    {
        public List<SBCScenarioCostSharing> SBCScenarioCostSharing { get; set; }
    }

    public class QHPProductInformation
    {
        public GeneralDetails GeneralDetails { get; set; }
        public StandAloneDental StandAloneDental { get; set; }
        public Links Links { get; set; }
        public SBCScenario SBCScenario { get; set; }
    }

    public class ProductRules
    {
        public GeneralInformation GeneralInformation { get; set; }
        public CDHInformation CDHInformation { get; set; }
        public GlobalProductRules GlobalProductRules { get; set; }
        public FacetsProductInformation FacetsProductInformation { get; set; }
        public QHPProductInformation QHPProductInformation { get; set; }
    }

    public class NetworkList
    {
        public string NetworkName { get; set; }
        public string NetworkType { get; set; }
        public string CopayAmountDS { get; set; }
        public string CoinsuranceAmountDS { get; set; }
        public string DeductibleAmountDS { get; set; }
        public string FamilyDeductibleAmountDS { get; set; }
        public string OutofPocketMaxAmount { get; set; }
        public string FamilyOutofPocketMaxAmount { get; set; }
        public string Limit { get; set; }
        public string Message { get; set; }
        public string DrugOOPDeductible { get; set; }
        public string AllowedAmount { get; set; }
        public string AllowedCounter { get; set; }
        public string Tier { get; set; }
        public string AltRule { get; set; }
    }

    public class QHPNetworkInformation
    {
        public string NetworkID { get; set; }
        public string ServiceAreaID { get; set; }
        public string FirstTierUtilization { get; set; }
        public string SecondTierUtilization { get; set; }
        public string OutofCountryCoverage { get; set; }
        public string OutofCountryCoverageDescription { get; set; }
        public string OutofServiceAreaCoverage { get; set; }
        public string OutofServiceAreaCoverageDescription { get; set; }
        public string NationalNetwork { get; set; }
    }

    public class FacetProductVariableComponent
    {
        public string NetworkType { get; set; }
        public string ProductVariableComponentTier { get; set; }
        public string ProductVariableComponentType { get; set; }
        public string EffectiveDate { get; set; }
        public string SequenceNumber { get; set; }
        public string TerminationDate { get; set; }
        public string PrimaryCareProvider { get; set; }
        public string InNetworkProvider { get; set; }
        public string ParticipatingProvider { get; set; }
        public string NonParticipatingProvider { get; set; }
        public string PreauthorizationNotRequired { get; set; }
        public string PreauthorizationObtained { get; set; }
        public string PreauthorizationViolation { get; set; }
        public string ReferralNotRequired { get; set; }
        public string ReferralObtained { get; set; }
        public string ReferralViolation { get; set; }
        public string LineofBusinessIndicator { get; set; }
    }

    public class NetworkDS
    {
        public List<NetworkList> NetworkList { get; set; }
        public QHPNetworkInformation QHPNetworkInformation { get; set; }
        public List<FacetProductVariableComponent> FacetProductVariableComponent { get; set; }
    }

    public class QHPFACETSNetworkD
    {
        public string NetworkName { get; set; }
        public string NetworkType { get; set; }
        public string Amount { get; set; }
    }

    public class CopayAmountD
    {
        public string CopayType { get; set; }
        public List<QHPFACETSNetworkD> QHPFACETSNetworkDS { get; set; }
    }

    public class PCPPhysicianseligibleforPCPOfficeVisitCopay
    {
        public string SelectAllThatApply { get; set; }
        public string GeneralPractice { get; set; }
        public string FamilyPractice { get; set; }
        public string OBGYN { get; set; }
        public string InternalMedicine { get; set; }
        public string Pediatrician { get; set; }
        public string Geratrics { get; set; }
        public string IfOtherListHere { get; set; }
    }

    public class CopayRules
    {
        public string SelectAllThatApply { get; set; }
        public string Ifservicetakesacopaydeductibleandcoinsurancethenalwaysapplythecopayfir { get; set; }
        public string CopaydoesnotcontributetoOutofPocketMax { get; set; }
        public string CopaydoescontributetoOutofPocketMax { get; set; }
        public string Ifservicetakesacopaydeductibleandcoinsurancethenalwaysapplythedeductib { get; set; }
    }

    public class QHPCopayInformation
    {
        public string MaximumNumberofDaysforCharginganInpatientCopay { get; set; }
        public string BeginPrimaryCareCostSharingAfteraSetNumberofVisits { get; set; }
    }

    public class Copay
    {
        public string Isthereacopay { get; set; }
        public List<CopayAmountD> CopayAmountDS { get; set; }
        public string ArethereotherCopayTypes { get; set; }
        public string IfyesenterService { get; set; }
        public string IfyesenterCopayAmount { get; set; }
        public PCPPhysicianseligibleforPCPOfficeVisitCopay PCPPhysicianseligibleforPCPOfficeVisitCopay { get; set; }
        public CopayRules CopayRules { get; set; }
        public QHPCopayInformation QHPCopayInformation { get; set; }
    }

    public class QHPFACETSNetworkD2
    {
        public string NetworkName { get; set; }
        public string NetworkType { get; set; }
        public string Amount { get; set; }
    }

    public class DeductibleAmount
    {
        public string DeductibleType { get; set; }
        public List<QHPFACETSNetworkD2> QHPFACETSNetworkDS { get; set; }
    }

    public class QHPFACETSNetworkD3
    {
        public string NetworkName { get; set; }
        public string NetworkType { get; set; }
        public string Amount { get; set; }
    }

    public class DrugDeductibleAmount
    {
        public string DeductibleType { get; set; }
        public List<QHPFACETSNetworkD3> QHPFACETSNetworkDS { get; set; }
    }

    public class DeductibleAmountDS
    {
        public List<DeductibleAmount> DeductibleAmount { get; set; }
        public List<DrugDeductibleAmount> DrugDeductibleAmount { get; set; }
    }

    public class ServiceSpecificDeductible
    {
        public string Aretheredeductiblesforspecificservices { get; set; }
        public string Blank { get; set; }
        public string IfYesListService { get; set; }
        public string SingleAmount { get; set; }
        public string FamilyAmount { get; set; }
    }

    public class QHPFACETSNetworkD4
    {
        public string NetworkName { get; set; }
        public string NetworkType { get; set; }
        public string Amount { get; set; }
    }

    public class DeductibleCarryOverAmount
    {
        public string DeductibleType { get; set; }
        public List<QHPFACETSNetworkD4> QHPFACETSNetworkDS { get; set; }
    }

    public class DeductibleCarryOverAmountforFacets
    {
        public List<DeductibleCarryOverAmount> DeductibleCarryOverAmount { get; set; }
    }

    public class DeductibleRules
    {
        public string PlanYearorCalendarYear { get; set; }
        public string DeductibleNetworkAccumulation { get; set; }
        public string DeductibleFamilyAccumulation { get; set; }
        public string DeductibleStoplossAccumulation { get; set; }
        public string DeductibleRule { get; set; }
        public string COBMedicareOutofPocketIndicator { get; set; }
        public string DeductibleRulesAccumulatorNumber { get; set; }
        public string DeductibleRulesDescription { get; set; }
        public string SelectAllThatApply { get; set; }
        public string MemberDeductibleisSalaryBased { get; set; }
        public string FamilyDeductibleisSalaryBased { get; set; }
        public string RxandMedicaldeductibleisintegrated { get; set; }
        public string Alwaysapplydeductiblebeforecoinsurance { get; set; }
        public string Apply4thQuarterCarryOver { get; set; }
        public string AllowPriorDeductibleCreditPDCs { get; set; }
        public string IfOtherSpecify { get; set; }
    }

    public class QHPDeductibleInformation
    {
        public string BeginPrimaryCareDeductibleCoinsuranceAfteraSetNumberofCopays { get; set; }
    }

    public class FacetServiceSpecificDeductibleList
    {
        public string Include { get; set; }
        public string BenefitCategory1 { get; set; }
        public string BenefitCategory2 { get; set; }
        public string BenefitCategory3 { get; set; }
        public string PlaceofService { get; set; }
        public string Single { get; set; }
        public string Family { get; set; }
        public string SingleCarryOver { get; set; }
        public string FamilyCarryOver { get; set; }
        public string PlanYearorCalendarYear { get; set; }
        public string DeductibleNetworkAccumulation { get; set; }
        public string DeductibleFamilyAccumulation { get; set; }
        public string DeductibleStoplossAccumulation { get; set; }
        public string DeductibleRule { get; set; }
        public string COBMedicareOutofPocketIndicator { get; set; }
        public string DeductibleRulesAccumulatorNumber { get; set; }
        public string DeductibleRulesDescription { get; set; }
        public string MemberDeductibleisSalaryBased { get; set; }
        public string FamilyDeductibleisSalaryBased { get; set; }
        public string RxandMedicaldeductibleisintegrated { get; set; }
        public string Alwaysapplydeductiblebeforecoinsurance { get; set; }
        public string Apply4thQuarterCarryOver { get; set; }
        public string AllowPriorDeductibleCreditPDCs { get; set; }
    }

    public class FacetServiceSpecificDeductible
    {
        public List<FacetServiceSpecificDeductibleList> FacetServiceSpecificDeductibleList { get; set; }
    }

    public class Deductible
    {
        public DeductibleAmountDS DeductibleAmountDS { get; set; }
        public ServiceSpecificDeductible ServiceSpecificDeductible { get; set; }
        public DeductibleCarryOverAmountforFacets DeductibleCarryOverAmountforFacets { get; set; }
        public DeductibleRules DeductibleRules { get; set; }
        public QHPDeductibleInformation QHPDeductibleInformation { get; set; }
        public FacetServiceSpecificDeductible FacetServiceSpecificDeductible { get; set; }
    }

    public class CoinsuranceAmount
    {
        public string NetworkName { get; set; }
        public string NetworkType { get; set; }
        public string Amount { get; set; }
    }

    public class DrugCoinsuranceAmount
    {
        public string NetworkName { get; set; }
        public string NetworkType { get; set; }
        public string Amount { get; set; }
    }

    public class CoinsuranceAmountDS
    {
        public List<CoinsuranceAmount> CoinsuranceAmount { get; set; }
        public List<DrugCoinsuranceAmount> DrugCoinsuranceAmount { get; set; }
    }

    public class QHPCoinsuranceInformation
    {
        public string MaximumCoinsuranceforSpecialtyDrugs { get; set; }
    }

    public class Coinsurance
    {
        public CoinsuranceAmountDS CoinsuranceAmountDS { get; set; }
        public string SelectAllThatApply { get; set; }
        public string CoinsuranceisalwaystakenafterthedeductibleissatisfieduntiltheOutofPock { get; set; }
        public string Coinsurancewillnotapplytoservicessubjecttoaflatdollarcopay { get; set; }
        public QHPCoinsuranceInformation QHPCoinsuranceInformation { get; set; }
    }

    public class QHPFACETSNetworkD5
    {
        public string NetworkName { get; set; }
        public string NetworkType { get; set; }
        public string Amount { get; set; }
    }

    public class OutofPocketMaxAmountD
    {
        public string OutofPocketMaxType { get; set; }
        public List<QHPFACETSNetworkD5> QHPFACETSNetworkDS { get; set; }
    }

    public class QHPFACETSNetworkD6
    {
        public string NetworkName { get; set; }
        public string NetworkType { get; set; }
        public string Amount { get; set; }
    }

    public class DrugOutofPocketMaxAmountD
    {
        public string DrugOutofPocketMaxType { get; set; }
        public List<QHPFACETSNetworkD6> QHPFACETSNetworkDS { get; set; }
    }

    public class OutofPocketMaxAccumulationRules
    {
        public string SelectAllThatApply { get; set; }
        public string CopayscontributestoOOPM { get; set; }
        public string DeductiblecontributestoOOPM { get; set; }
        public string CoinsurancecontributestoOOPM { get; set; }
        public string RxdeductiblecontributestoOOPM { get; set; }
    }

    public class OutofPocketMaximum
    {
        public List<OutofPocketMaxAmountD> OutofPocketMaxAmountDS { get; set; }
        public List<DrugOutofPocketMaxAmountD> DrugOutofPocketMaxAmountDS { get; set; }
        public OutofPocketMaxAccumulationRules OutofPocketMaxAccumulationRules { get; set; }
    }

    public class CostShare
    {
        public Copay Copay { get; set; }
        public Deductible Deductible { get; set; }
        public Coinsurance Coinsurance { get; set; }
        public OutofPocketMaximum OutofPocketMaximum { get; set; }
    }

    public class LimitsList
    {
        public string SelectLimit { get; set; }
        public string LimitDescription { get; set; }
        public string Value { get; set; }
    }

    public class LimitRulesLTLT
    {
        public string LimitDescription { get; set; }
        public string AccumulatorNo { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Level { get; set; }
        public string Period { get; set; }
        public string Rule { get; set; }
        public string Relations { get; set; }
        public string Subsection { get; set; }
        public string ExplanationCode { get; set; }
        public string LimitAmountSalaryPct { get; set; }
        public string AlternateAmount { get; set; }
        public string SalaryBased { get; set; }
        public string Days { get; set; }
        public string UserMessage { get; set; }
    }

    public class LimitProcedureTableLTIP
    {
        public string LimitDescription { get; set; }
        public string RelatedProcedureCodeHigh { get; set; }
        public string RelatedProcedureCodeLow { get; set; }
    }

    public class LimitDiagnosisTableLTID
    {
        public string LimitDescription { get; set; }
        public string RelatedDiagnosisCode { get; set; }
    }

    public class LimitProviderTableLTPR
    {
        public string LimitDescription { get; set; }
        public string ProviderType { get; set; }
    }

    public class FacetsLimits
    {
        public List<LimitRulesLTLT> LimitRulesLTLT { get; set; }
        public List<LimitProcedureTableLTIP> LimitProcedureTableLTIP { get; set; }
        public List<LimitDiagnosisTableLTID> LimitDiagnosisTableLTID { get; set; }
        public List<LimitProviderTableLTPR> LimitProviderTableLTPR { get; set; }
    }

    public class Limits
    {
        public List<LimitsList> LimitsList { get; set; }
        public FacetsLimits FacetsLimits { get; set; }
    }

    public class QHPFACETSNetworkD7
    {
        public string NetworkName { get; set; }
        public string NetworkType { get; set; }
        public string MessageSERL { get; set; }
        public string DisallowedMessage { get; set; }
    }

    public class MessageServiceList
    {
        public string Include { get; set; }
        public string BenefitCategory1 { get; set; }
        public string BenefitCategory2 { get; set; }
        public string BenefitCategory3 { get; set; }
        public string PlaceofService { get; set; }
        public List<QHPFACETSNetworkD7> QHPFACETSNetworkDS { get; set; }
        public int RowIDProperty { get; set; }
    }

    public class Messages
    {
        public List<MessageServiceList> MessageServiceList { get; set; }
    }

    public class ProductMandatesMasterList
    {
        public string BenefitCategory1 { get; set; }
        public string BenefitCategory2 { get; set; }
        public string BenefitCategory3 { get; set; }
        public string PlaceofService { get; set; }
        public string SubjecttoDeductibleTier1 { get; set; }
        public string SubjecttoDeductibleTier2 { get; set; }
        public string ExcludedfromInNetworkMOOP { get; set; }
        public string ExcludedfromOutofNetworkMOOP { get; set; }
    }

    public class MandateList
    {
        public string Region { get; set; }
        public string Segment { get; set; }
        public string MandateName { get; set; }
        public string SelectMandate { get; set; }
        public List<ProductMandatesMasterList> ProductMandatesMasterList { get; set; }
    }

    public class MasterListService
    {
        public string BenefitCategory1 { get; set; }
        public string BenefitCategory2 { get; set; }
        public string BenefitCategory3 { get; set; }
        public string PlaceofService { get; set; }
        public string SubjecttoDeductibleTier1 { get; set; }
        public string SubjecttoDeductibleTier2 { get; set; }
        public string ExcludedfromInNetworkMOOP { get; set; }
        public string ExcludedfromOutofNetworkMOOP { get; set; }
    }

    public class Mandates
    {
        public List<MandateList> MandateList { get; set; }
        public List<MasterListService> MasterListServices { get; set; }
    }

    public class NonMandateService2
    {
        public string BenefitCategory1 { get; set; }
        public string BenefitCategory2 { get; set; }
        public string BenefitCategory3 { get; set; }
        public string PlaceofService { get; set; }
        public string SelectService { get; set; }
        public string SubjecttoDeductibleTier1 { get; set; }
        public string SubjecttoDeductibleTier2 { get; set; }
        public string ExcludedfromInNetworkMOOP { get; set; }
        public string ExcludedfromOutofNetworkMOOP { get; set; }
    }

    public class NonMandateService
    {
        public List<NonMandateService2> NonMandateServices { get; set; }
    }

    public class QHPFACETSNetworkD8
    {
        public string AltRule { get; set; }
        public string Copay { get; set; }
        public string Coinsurance { get; set; }
        public string SingleDeductible { get; set; }
        public string FamilyDeductible { get; set; }
        public string SingleOutofPocketMax { get; set; }
        public string FamilyOutofPocketMax { get; set; }
        public string Message { get; set; }
        public string NetworkName { get; set; }
        public string NetworkType { get; set; }
        public string AllowedAmount { get; set; }
        public string AllowedCounter { get; set; }
        public string Tier { get; set; }
    }

    public class BenefitReviewGrid
    {
        public string BenefitCat1 { get; set; }
        public string BenefitCat2 { get; set; }
        public string BenefitCat3 { get; set; }
        public string POS { get; set; }
        public string Treatas100 { get; set; }
        public string Limits { get; set; }
        public string ApplyCopayType { get; set; }
        public string ApplyCopay { get; set; }
        public string ApplyDedCoin { get; set; }
        public string QHPBenefitMapping { get; set; }
        public string EHBVarianceReason { get; set; }
        public string SubjecttoDeductibleTier1 { get; set; }
        public string SubjecttoDeductibleTier2 { get; set; }
        public string ExcludedfromInNetworkMOOP { get; set; }
        public string ExcludedfromOutofNetworkMOOP { get; set; }
        public List<QHPFACETSNetworkD8> QHPFACETSNetworkDS { get; set; }
    }

    public class BenefitReviewGridTierData
    {
        public string NetworkName { get; set; }
        public string NetworkType { get; set; }
        public string TierNo { get; set; }
        public string BenefitCategory1 { get; set; }
        public string BenefitCategory2 { get; set; }
        public string BenefitCategory3 { get; set; }
        public string PlaceofService { get; set; }
        public string Copay { get; set; }
        public string Coinsurance { get; set; }
        public string AllowedAmount { get; set; }
        public string AllowedCounter { get; set; }
    }

    public class BenefitReviewGridAltRulesData
    {
        public string BenefitCategory1 { get; set; }
        public string BenefitCategory2 { get; set; }
        public string BenefitCategory3 { get; set; }
        public string PlaceofService { get; set; }
        public string TierNo { get; set; }
        public string Copay { get; set; }
        public string Coinsurance { get; set; }
        public string AllowedAmount { get; set; }
        public string AllowedCounter { get; set; }
        public string NetworkName { get; set; }
        public string NetworkType { get; set; }
    }

    public class BenefitReview
    {
        public string ShowCopayinGrid { get; set; }
        public string ShowCoinsuranceinGrid { get; set; }
        public string ShowDeductiblesinGrid { get; set; }
        public string ShowOutOfPocketinGrid { get; set; }
        public string ShowLimitsandMessages { get; set; }
        public List<BenefitReviewGrid> BenefitReviewGrid { get; set; }
        public List<BenefitReviewGridTierData> BenefitReviewGridTierData { get; set; }
        public List<BenefitReviewGridAltRulesData> BenefitReviewGridAltRulesData { get; set; }
    }

    public class EquinioxProduct
    {
        public ProductRules ProductRules { get; set; }
        public NetworkDS NetworkDS { get; set; }
        public CostShare CostShare { get; set; }
        public Limits Limits { get; set; }
        public Messages Messages { get; set; }
        public Mandates Mandates { get; set; }
        public NonMandateService NonMandateService { get; set; }
        public BenefitReview BenefitReview { get; set; }
    }
}
