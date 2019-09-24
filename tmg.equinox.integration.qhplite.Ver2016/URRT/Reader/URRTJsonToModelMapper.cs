using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.qhplite.Ver2016.QHPModel;
using tmg.equinox.integration.qhplite.Ver2016.DocumentExporter;
using Newtonsoft.Json;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    public class URRTJsonToModelMapper
    {
        #region Private Members

        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor
        public URRTJsonToModelMapper()
        {

        }
        #endregion Constructor

        #region Public Methods
        public PlanURRTemplate GetURRTTemplate(string urrtDocument, IList<PlanBenefitPackage> planBenefitPackageList)
        {
            PlanURRTemplate planURRTTemplate = new PlanURRTemplate();
            if (!string.IsNullOrEmpty(urrtDocument))
            {
                dynamic document = JsonConvert.DeserializeObject(urrtDocument);
                GenerateURRTTemplate(ref planURRTTemplate, document, planBenefitPackageList);
            }
            return planURRTTemplate;
        }
        #endregion Public Methods

        #region Private Methods
        private void GenerateURRTTemplate(ref PlanURRTemplate planURRTTemplate, dynamic document, IList<PlanBenefitPackage> planBenefitPackageList)
        {
            GenrateURRTMarketExperience(ref planURRTTemplate, document);
            GeneratePlanProductInfo(ref planURRTTemplate, planBenefitPackageList);
        }

        #region Market Experience Methods
        private void GenrateURRTMarketExperience(ref PlanURRTemplate planURRTTemplate, dynamic document)
        {
            GenerateMarketExperienceHeader(ref planURRTTemplate, document);
            GenerateMarketExperienceSectionI(ref planURRTTemplate, document);
            GenerateMarketExperienceSectionII(ref planURRTTemplate, document);
            GenerateMarketExperienceSectionIII(ref planURRTTemplate, document);
        }

        #region Market Experience Header
        private void GenerateMarketExperienceHeader(ref PlanURRTemplate planURRTTemplate, dynamic document)
        {
            if (planURRTTemplate == null)
            {
                planURRTTemplate = new PlanURRTemplate();
            }
            MarketExperienceHeader header = new MarketExperienceHeader();

            header.CompanyLegalName = document.MarketExperience.GeneralInformation.CompanyLegalName;
            header.State = document.MarketExperience.GeneralInformation.State;
            header.HIOSIssuerID = document.MarketExperience.GeneralInformation.HIOSIssuerID;
            header.Market = document.MarketExperience.GeneralInformation.Market;
            header.EffectiveDateofRateChanges = document.MarketExperience.GeneralInformation.EffectiveDateofRateChanges;

            planURRTTemplate.MarketExperience = new MarketExperience();
            planURRTTemplate.MarketExperience.Header = header;
        }
        #endregion Market Experience Header

        #region Market Experience Section I
        private void GenerateMarketExperienceSectionI(ref PlanURRTemplate planURRTTemplate, dynamic document)
        {
            SectionI sectionI = new SectionI();

            sectionI.ExperiencePeriodFrom = document.MarketExperience.SectionIExperiencePeriodData.ExperiencePeriod;
            sectionI.ExperiencePeriodTo = document.MarketExperience.SectionIExperiencePeriodData.To;

            sectionI.PremiumsinExperiencePeriod = new PremiumsInExperiencePeriod();
            sectionI.PremiumsinExperiencePeriod.PercentageOfPremium = document.MarketExperience.SectionIExperiencePeriodData.PremiumsnetofMLRRebateinExperiencePeriod.PercentageofPremium;
            sectionI.PremiumsinExperiencePeriod.PeriodAggregate = document.MarketExperience.SectionIExperiencePeriodData.PremiumsnetofMLRRebateinExperiencePeriod.ExperiencePeriodAggregateAmount;
            sectionI.PremiumsinExperiencePeriod.PMPM = document.MarketExperience.SectionIExperiencePeriodData.PremiumsnetofMLRRebateinExperiencePeriod.PMPM;

            sectionI.IncurredClaimsInExperiencePeriod = new IncurredClaimsInExperiencePeriod();
            sectionI.IncurredClaimsInExperiencePeriod.PercentageOfPremium = document.MarketExperience.SectionIExperiencePeriodData.IncurredClaimsinExperiencePeriod.PercentageofPremium;
            sectionI.IncurredClaimsInExperiencePeriod.PeriodAggregate = document.MarketExperience.SectionIExperiencePeriodData.IncurredClaimsinExperiencePeriod.ExperiencePeriodAggregateAmount;
            sectionI.IncurredClaimsInExperiencePeriod.PMPM = document.MarketExperience.SectionIExperiencePeriodData.IncurredClaimsinExperiencePeriod.PMPM;

            sectionI.AllowedClaim = new AllowedClaim();
            sectionI.AllowedClaim.PercentageOfPremium = document.MarketExperience.SectionIExperiencePeriodData.AllowedClaims.PercentageofPremium;
            sectionI.AllowedClaim.PeriodAggregate = document.MarketExperience.SectionIExperiencePeriodData.AllowedClaims.ExperiencePeriodAggregateAmount;
            sectionI.AllowedClaim.PMPM = document.MarketExperience.SectionIExperiencePeriodData.AllowedClaims.PMPM;

            sectionI.IndexRateofExperiencePeriod = new IndexRateofExperiencePeriod();
            sectionI.IndexRateofExperiencePeriod.PMPM = document.MarketExperience.SectionIExperiencePeriodData.IndexRateofExperiencePeriod.PMPM;

            sectionI.ExperiencePeriodMemberMonths = new ExperiencePeriodMemberMonths();
            sectionI.ExperiencePeriodMemberMonths.PeriodAggregate = document.MarketExperience.SectionIExperiencePeriodData.ExperiencePeriodMemberMonths.ExperiencePeriodAggregateAmount;

            planURRTTemplate.MarketExperience.SectionI = sectionI;
        }
        #endregion Market Experience Section I

        #region Market Experience Section II
        private void GenerateMarketExperienceSectionII(ref PlanURRTemplate planURRTTemplate, dynamic document)
        {
            SectionII sectionII = new SectionII();
            sectionII.ExperiencePeriod = new ExperiencePeriod();
            sectionII.ExperiencePeriod.ProjectPeriodFrom = planURRTTemplate.MarketExperience.SectionI.ExperiencePeriodFrom;
            sectionII.ExperiencePeriod.ProjectPeriodTo = planURRTTemplate.MarketExperience.SectionI.ExperiencePeriodTo;
            sectionII.ExperiencePeriod.MidPointtoMidPointExperiencetoProjection = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.MidpointtoMidpointExperiencetoProjection;

            GenerateMarketExperienceSectionIIOnActualExperienceAllowed(ref sectionII, document);
            GenerateMarketExperienceSectionIIAdjustmentFromExperienceToProjectPeriod(ref sectionII, document);
            GenerateMarketExperienceSectionIIAnnualizedTrendFactors(ref sectionII, document);
            GenerateMarketExperienceSectionIIProjectionsBeforeCredibilityAdjustment(ref sectionII, document);
            GenerateMarketExperienceSectionIICredibilityManual(ref sectionII, document);

            planURRTTemplate.MarketExperience.SectionII = sectionII;
        }

        #region On Actual Experience Allowed
        private void GenerateMarketExperienceSectionIIOnActualExperienceAllowed(ref SectionII sectionII, dynamic document)
        {
            OnActualExperienceAllowed actualExperienceAllowed = new OnActualExperienceAllowed();

            GenerateMarketExperienceInpatientHospitalActualExperienceAllowed(ref actualExperienceAllowed, document);
            GenerateMarketExperienceOutpatientHospitalActualExperienceAllowed(ref actualExperienceAllowed, document);
            GenerateMarketExperienceProfessionalActualExperienceAllowed(ref actualExperienceAllowed, document);
            GenerateMarketExperienceOtherMedicalActualExperienceAllowed(ref actualExperienceAllowed, document);
            GenerateMarketExperienceCapitationActualExperienceAllowed(ref actualExperienceAllowed, document);
            GenerateMarketExperiencePrescriptionDrugActualExperienceAllowed(ref actualExperienceAllowed, document);

            sectionII.OnActualExperienceAllowed = actualExperienceAllowed;
        }

        private void GenerateMarketExperienceInpatientHospitalActualExperienceAllowed(ref OnActualExperienceAllowed actualExperienceAllowed, dynamic document)
        {
            InpatientHospitalActualExperienceAllowed inpatient = new InpatientHospitalActualExperienceAllowed();
            inpatient.UtilizationDescription = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.OnActualExperienceAllowed.InpatientHospital.UtilizationDescription;
            inpatient.UtilizationPer1000 = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.OnActualExperienceAllowed.InpatientHospital.Utilizationper1000;
            inpatient.CostPerService = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.OnActualExperienceAllowed.InpatientHospital.AverageCostService;
            inpatient.PMPM = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.OnActualExperienceAllowed.InpatientHospital.PMPM;

            actualExperienceAllowed.InpatientHospitalActualExperienceAllowed = inpatient;
        }

        private void GenerateMarketExperienceOutpatientHospitalActualExperienceAllowed(ref OnActualExperienceAllowed actualExperienceAllowed, dynamic document)
        {
            OutpatientHospitalActualExperienceAllowed outpatient = new OutpatientHospitalActualExperienceAllowed();
            outpatient.UtilizationDescription = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.OnActualExperienceAllowed.OutpatientHospital.UtilizationDescription;
            outpatient.UtilizationPer1000 = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.OnActualExperienceAllowed.OutpatientHospital.Utilizationper1000;
            outpatient.CostPerService = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.OnActualExperienceAllowed.OutpatientHospital.AverageCostService;
            outpatient.PMPM = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.OnActualExperienceAllowed.OutpatientHospital.PMPM;

            actualExperienceAllowed.OutpatientHospitalActualExperienceAllowed = outpatient;
        }

        private void GenerateMarketExperienceProfessionalActualExperienceAllowed(ref OnActualExperienceAllowed actualExperienceAllowed, dynamic document)
        {
            ProfessionalActualExperienceAllowed professional = new ProfessionalActualExperienceAllowed();
            professional.UtilizationDescription = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.OnActualExperienceAllowed.Professional.UtilizationDescription;
            professional.UtilizationPer1000 = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.OnActualExperienceAllowed.Professional.Utilizationper1000;
            professional.CostPerService = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.OnActualExperienceAllowed.Professional.AverageCostService;
            professional.PMPM = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.OnActualExperienceAllowed.Professional.PMPM;

            actualExperienceAllowed.ProfessionalActualExperienceAllowed = professional;
        }

        private void GenerateMarketExperienceOtherMedicalActualExperienceAllowed(ref OnActualExperienceAllowed actualExperienceAllowed, dynamic document)
        {
            OtherMedicalActualExperienceAllowed otherMedical = new OtherMedicalActualExperienceAllowed();
            otherMedical.UtilizationDescription = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.OnActualExperienceAllowed.OtherMedical.UtilizationDescription;
            otherMedical.UtilizationPer1000 = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.OnActualExperienceAllowed.OtherMedical.Utilizationper1000;
            otherMedical.CostPerService = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.OnActualExperienceAllowed.OtherMedical.AverageCostService;
            otherMedical.PMPM = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.OnActualExperienceAllowed.OtherMedical.PMPM;

            actualExperienceAllowed.OtherMedicalActualExperienceAllowed = otherMedical;
        }

        private void GenerateMarketExperienceCapitationActualExperienceAllowed(ref OnActualExperienceAllowed actualExperienceAllowed, dynamic document)
        {
            CapitationActualExperienceAllowed capitation = new CapitationActualExperienceAllowed();
            capitation.UtilizationDescription = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.OnActualExperienceAllowed.Capitation.UtilizationDescription;
            capitation.UtilizationPer1000 = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.OnActualExperienceAllowed.Capitation.Utilizationper1000;
            capitation.CostPerService = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.OnActualExperienceAllowed.Capitation.AverageCostService;
            capitation.PMPM = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.OnActualExperienceAllowed.Capitation.PMPM;

            actualExperienceAllowed.CapitationActualExperienceAllowed = capitation;
        }

        private void GenerateMarketExperiencePrescriptionDrugActualExperienceAllowed(ref OnActualExperienceAllowed actualExperienceAllowed, dynamic document)
        {
            PrescriptionDrugActualExperienceAllowed prescriptionDrug = new PrescriptionDrugActualExperienceAllowed();
            prescriptionDrug.UtilizationDescription = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.OnActualExperienceAllowed.PrescriptionDrug.UtilizationDescription;
            prescriptionDrug.UtilizationPer1000 = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.OnActualExperienceAllowed.PrescriptionDrug.Utilizationper1000;
            prescriptionDrug.CostPerService = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.OnActualExperienceAllowed.PrescriptionDrug.AverageCostService;
            prescriptionDrug.PMPM = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.OnActualExperienceAllowed.PrescriptionDrug.PMPM;

            actualExperienceAllowed.PrescriptionDrugActualExperienceAllowed = prescriptionDrug;
        }
        #endregion region On Actual Experience Allowed

        #region Adjustment from Experience to Project Period
        private void GenerateMarketExperienceSectionIIAdjustmentFromExperienceToProjectPeriod(ref SectionII sectionII, dynamic document)
        {
            AdjustmentfromExperiencetoProjectionPeriod adjustment = new AdjustmentfromExperiencetoProjectionPeriod();

            GenerateInpatientHosipitalAdjustmentfromExperiencetoProjectionPeriod(ref adjustment, document);
            GenerateOutpatientHosipitalAdjustmentfromExperiencetoProjectionPeriod(ref adjustment, document);
            GenerateProfessionalAdjustmentfromExperiencetoProjectionPeriod(ref adjustment, document);
            GenerateOtherMedicalAdjustmentfromExperiencetoProjectionPeriod(ref adjustment, document);
            GenerateCapitationAdjustmentfromExperiencetoProjectionPeriod(ref adjustment, document);
            GeneratePrescriptionDrugAdjustmentfromExperiencetoProjectionPeriod(ref adjustment, document);

            sectionII.AdjustmentfromExperiencetoProjectionPeriod = adjustment;
        }

        private void GenerateInpatientHosipitalAdjustmentfromExperiencetoProjectionPeriod(ref AdjustmentfromExperiencetoProjectionPeriod adjustment, dynamic document)
        {
            InpatientHosipitalAdjustmentfromExperiencetoProjectionPeriod inpatient = new InpatientHosipitalAdjustmentfromExperiencetoProjectionPeriod();

            inpatient.Other = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.AdjtfromExperiencetoProjectionPeriod.InpatientHospital.Other;
            inpatient.PopulationRiskMorbidity = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.AdjtfromExperiencetoProjectionPeriod.InpatientHospital.PoplriskMorbidity;

            adjustment.InpatientHosipitalAdjustmentfromExperiencetoProjectionPeriod = inpatient;
        }

        private void GenerateOutpatientHosipitalAdjustmentfromExperiencetoProjectionPeriod(ref AdjustmentfromExperiencetoProjectionPeriod adjustment, dynamic document)
        {
            OutpatientHosipitalAdjustmentfromExperiencetoProjectionPeriod outpatient = new OutpatientHosipitalAdjustmentfromExperiencetoProjectionPeriod();

            outpatient.Other = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.AdjtfromExperiencetoProjectionPeriod.OutpatientHospital.Other;
            outpatient.PopulationRiskMorbidity = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.AdjtfromExperiencetoProjectionPeriod.OutpatientHospital.PoplriskMorbidity;

            adjustment.OutpatientHosipitalAdjustmentfromExperiencetoProjectionPeriod = outpatient;
        }

        private void GenerateProfessionalAdjustmentfromExperiencetoProjectionPeriod(ref AdjustmentfromExperiencetoProjectionPeriod adjustment, dynamic document)
        {
            ProfessionalAdjustmentfromExperiencetoProjectionPeriod professional = new ProfessionalAdjustmentfromExperiencetoProjectionPeriod();

            professional.Other = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.AdjtfromExperiencetoProjectionPeriod.Professional.Other;
            professional.PopulationRiskMorbidity = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.AdjtfromExperiencetoProjectionPeriod.Professional.PoplriskMorbidity;

            adjustment.ProfessionalAdjustmentfromExperiencetoProjectionPeriod = professional;
        }

        private void GenerateOtherMedicalAdjustmentfromExperiencetoProjectionPeriod(ref AdjustmentfromExperiencetoProjectionPeriod adjustment, dynamic document)
        {
            OtherMedicalAdjustmentfromExperiencetoProjectionPeriod otherMedical = new OtherMedicalAdjustmentfromExperiencetoProjectionPeriod();

            otherMedical.Other = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.AdjtfromExperiencetoProjectionPeriod.OtherMedical.Other;
            otherMedical.PopulationRiskMorbidity = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.AdjtfromExperiencetoProjectionPeriod.OtherMedical.PoplriskMorbidity;

            adjustment.OtherMedicalAdjustmentfromExperiencetoProjectionPeriod = otherMedical;
        }

        private void GenerateCapitationAdjustmentfromExperiencetoProjectionPeriod(ref AdjustmentfromExperiencetoProjectionPeriod adjustment, dynamic document)
        {
            CapitationAdjustmentfromExperiencetoProjectionPeriod capitation = new CapitationAdjustmentfromExperiencetoProjectionPeriod();

            capitation.Other = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.AdjtfromExperiencetoProjectionPeriod.Capitation.Other;
            capitation.PopulationRiskMorbidity = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.AdjtfromExperiencetoProjectionPeriod.Capitation.PoplriskMorbidity;

            adjustment.CapitationAdjustmentfromExperiencetoProjectionPeriod = capitation;
        }

        private void GeneratePrescriptionDrugAdjustmentfromExperiencetoProjectionPeriod(ref AdjustmentfromExperiencetoProjectionPeriod adjustment, dynamic document)
        {
            PrescriptionDrugAdjustmentfromExperiencetoProjectionPeriod capitation = new PrescriptionDrugAdjustmentfromExperiencetoProjectionPeriod();

            capitation.Other = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.AdjtfromExperiencetoProjectionPeriod.PrescriptionDrug.Other;
            capitation.PopulationRiskMorbidity = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.AdjtfromExperiencetoProjectionPeriod.PrescriptionDrug.PoplriskMorbidity;

            adjustment.PrescriptionDrugAdjustmentfromExperiencetoProjectionPeriod = capitation;
        }
        #endregion Adjustment from Experience to Project Period

        #region Annualized Trend Factors
        private void GenerateMarketExperienceSectionIIAnnualizedTrendFactors(ref SectionII sectionII, dynamic document)
        {
            AnnualizedTrendFactors adjustment = new AnnualizedTrendFactors();

            GenerateInpatientHosipitalAnnualizedTrendFactors(ref adjustment, document);
            GenerateOutpatientHosipitalAnnualizedTrendFactors(ref adjustment, document);
            GenerateProfessionalAnnualizedTrendFactors(ref adjustment, document);
            GenerateOtherMedicalAnnualizedTrendFactors(ref adjustment, document);
            GenerateCapitationAnnualizedTrendFactors(ref adjustment, document);
            GeneratePrescriptionDrugAnnualizedTrendFactors(ref adjustment, document);

            sectionII.AnnualizedTrendFactors = adjustment;
        }

        private void GenerateInpatientHosipitalAnnualizedTrendFactors(ref AnnualizedTrendFactors annualizedTrendFactors, dynamic document)
        {
            InpatientHosipitalAnnualizedTrendFactors inpatient = new InpatientHosipitalAnnualizedTrendFactors();

            inpatient.Cost = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.AnnualizedTrendFactors.InpatientHospital.Cost;
            inpatient.Utilization = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.AnnualizedTrendFactors.InpatientHospital.Util;

            annualizedTrendFactors.InpatientHosipitalAnnualizedTrendFactors = inpatient;
        }

        private void GenerateOutpatientHosipitalAnnualizedTrendFactors(ref AnnualizedTrendFactors annualizedTrendFactors, dynamic document)
        {
            OutpatientHosipitalAnnualizedTrendFactors outpatient = new OutpatientHosipitalAnnualizedTrendFactors();

            outpatient.Cost = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.AnnualizedTrendFactors.OutpatientHospital.Cost;
            outpatient.Utilization = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.AnnualizedTrendFactors.OutpatientHospital.Util;

            annualizedTrendFactors.OutpatientHosipitalAnnualizedTrendFactors = outpatient;
        }

        private void GenerateProfessionalAnnualizedTrendFactors(ref AnnualizedTrendFactors annualizedTrendFactors, dynamic document)
        {
            ProfessionalAnnualizedTrendFactors professional = new ProfessionalAnnualizedTrendFactors();

            professional.Cost = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.AnnualizedTrendFactors.Professional.Cost;
            professional.Utilization = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.AnnualizedTrendFactors.Professional.Util;

            annualizedTrendFactors.ProfessionalAnnualizedTrendFactors = professional;
        }

        private void GenerateOtherMedicalAnnualizedTrendFactors(ref AnnualizedTrendFactors annualizedTrendFactors, dynamic document)
        {
            OtherMedicalAnnualizedTrendFactors otherMedical = new OtherMedicalAnnualizedTrendFactors();

            otherMedical.Cost = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.AnnualizedTrendFactors.OtherMedical.Cost;
            otherMedical.Utilization = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.AnnualizedTrendFactors.OtherMedical.Util;

            annualizedTrendFactors.OtherMedicalAnnualizedTrendFactors = otherMedical;
        }

        private void GenerateCapitationAnnualizedTrendFactors(ref AnnualizedTrendFactors annualizedTrendFactors, dynamic document)
        {
            CapitationAnnualizedTrendFactors capitation = new CapitationAnnualizedTrendFactors();

            capitation.Cost = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.AnnualizedTrendFactors.Capitation.Cost;
            capitation.Utilization = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.AnnualizedTrendFactors.Capitation.Util;

            annualizedTrendFactors.CapitationAnnualizedTrendFactors = capitation;
        }

        private void GeneratePrescriptionDrugAnnualizedTrendFactors(ref AnnualizedTrendFactors annualizedTrendFactors, dynamic document)
        {
            PrescriptionDrugAnnualizedTrendFactors capitation = new PrescriptionDrugAnnualizedTrendFactors();

            capitation.Cost = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.AnnualizedTrendFactors.PrescriptionDrug.Cost;
            capitation.Utilization = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.AnnualizedTrendFactors.PrescriptionDrug.Util;

            annualizedTrendFactors.PrescriptionDrugAnnualizedTrendFactors = capitation;
        }
        #endregion Annualized Trend Factors

        #region Projections, before credibility Adjustment
        private void GenerateMarketExperienceSectionIIProjectionsBeforeCredibilityAdjustment(ref SectionII sectionII, dynamic document)
        {
            ProjectionsBeforeCredibilityAdjustment projectionBeforeCredibilityAdjustment = new ProjectionsBeforeCredibilityAdjustment();

            GenerateInpatientHospitalProjectionsBeforeCredibilityAdjustment(ref projectionBeforeCredibilityAdjustment, document);
            GenerateOutpatientHospitalProjectionsBeforeCredibilityAdjustment(ref projectionBeforeCredibilityAdjustment, document);
            GenerateProfessionalProjectionsBeforeCredibilityAdjustment(ref projectionBeforeCredibilityAdjustment, document);
            GenerateOtherMedicalProjectionsBeforeCredibilityAdjustment(ref projectionBeforeCredibilityAdjustment, document);
            GenerateCapitationProjectionsBeforeCredibilityAdjustment(ref projectionBeforeCredibilityAdjustment, document);
            GeneratePrescriptionDrugProjectionsBeforeCredibilityAdjustment(ref projectionBeforeCredibilityAdjustment, document);

            sectionII.ProjectionsBeforeCredibilityAdjustment = projectionBeforeCredibilityAdjustment;
        }

        private void GenerateInpatientHospitalProjectionsBeforeCredibilityAdjustment(ref ProjectionsBeforeCredibilityAdjustment projection, dynamic document)
        {
            InpatientHospitalProjectionsBeforeCredibilityAdjustment inpatient = new InpatientHospitalProjectionsBeforeCredibilityAdjustment();
            inpatient.UtilizationPer1000 = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.ProjectionsbeforecredibilityAdjustment.InpatientHospital.Utilizationper1000;
            inpatient.CostPerService = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.ProjectionsbeforecredibilityAdjustment.InpatientHospital.AverageCostService;
            inpatient.PMPM = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.ProjectionsbeforecredibilityAdjustment.InpatientHospital.PMPM;

            projection.InpatientHospitalProjectionsBeforeCredibilityAdjustment = inpatient;
        }

        private void GenerateOutpatientHospitalProjectionsBeforeCredibilityAdjustment(ref ProjectionsBeforeCredibilityAdjustment projection, dynamic document)
        {
            OutpatientHospitalProjectionsBeforeCredibilityAdjustment outpatient = new OutpatientHospitalProjectionsBeforeCredibilityAdjustment();
            outpatient.UtilizationPer1000 = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.ProjectionsbeforecredibilityAdjustment.OutpatientHospital.Utilizationper1000;
            outpatient.CostPerService = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.ProjectionsbeforecredibilityAdjustment.OutpatientHospital.AverageCostService;
            outpatient.PMPM = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.ProjectionsbeforecredibilityAdjustment.OutpatientHospital.PMPM;

            projection.OutpatientHospitalProjectionsBeforeCredibilityAdjustment = outpatient;
        }

        private void GenerateProfessionalProjectionsBeforeCredibilityAdjustment(ref ProjectionsBeforeCredibilityAdjustment projection, dynamic document)
        {
            ProfessionalProjectionsBeforeCredibilityAdjustment professional = new ProfessionalProjectionsBeforeCredibilityAdjustment();
            professional.UtilizationPer1000 = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.ProjectionsbeforecredibilityAdjustment.Professional.Utilizationper1000;
            professional.CostPerService = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.ProjectionsbeforecredibilityAdjustment.Professional.AverageCostService;
            professional.PMPM = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.ProjectionsbeforecredibilityAdjustment.Professional.PMPM;

            projection.ProfessionalProjectionsBeforeCredibilityAdjustment = professional;
        }

        private void GenerateOtherMedicalProjectionsBeforeCredibilityAdjustment(ref ProjectionsBeforeCredibilityAdjustment projection, dynamic document)
        {
            OtherMedicalProjectionsBeforeCredibilityAdjustment otherMedical = new OtherMedicalProjectionsBeforeCredibilityAdjustment();
            otherMedical.UtilizationPer1000 = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.ProjectionsbeforecredibilityAdjustment.OtherMedical.Utilizationper1000;
            otherMedical.CostPerService = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.ProjectionsbeforecredibilityAdjustment.OtherMedical.AverageCostService;
            otherMedical.PMPM = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.ProjectionsbeforecredibilityAdjustment.OtherMedical.PMPM;

            projection.OtherMedicalProjectionsBeforeCredibilityAdjustment = otherMedical;
        }

        private void GenerateCapitationProjectionsBeforeCredibilityAdjustment(ref ProjectionsBeforeCredibilityAdjustment projection, dynamic document)
        {
            CapitationProjectionsBeforeCredibilityAdjustment capitation = new CapitationProjectionsBeforeCredibilityAdjustment();
            capitation.UtilizationPer1000 = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.ProjectionsbeforecredibilityAdjustment.Capitation.Utilizationper1000;
            capitation.CostPerService = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.ProjectionsbeforecredibilityAdjustment.Capitation.AverageCostService;
            capitation.PMPM = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.ProjectionsbeforecredibilityAdjustment.Capitation.PMPM;

            projection.CapitationProjectionsBeforeCredibilityAdjustment = capitation;
        }

        private void GeneratePrescriptionDrugProjectionsBeforeCredibilityAdjustment(ref ProjectionsBeforeCredibilityAdjustment projection, dynamic document)
        {
            PrescriptionDrugProjectionsBeforeCredibilityAdjustment prescriptionDrug = new PrescriptionDrugProjectionsBeforeCredibilityAdjustment();
            prescriptionDrug.UtilizationPer1000 = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.ProjectionsbeforecredibilityAdjustment.PrescriptionDrug.Utilizationper1000;
            prescriptionDrug.CostPerService = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.ProjectionsbeforecredibilityAdjustment.PrescriptionDrug.AverageCostService;
            prescriptionDrug.PMPM = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.ProjectionsbeforecredibilityAdjustment.PrescriptionDrug.PMPM;

            projection.PrescriptionDrugProjectionsBeforeCredibilityAdjustment = prescriptionDrug;
        }
        #endregion Projections, before credibility Adjustment

        #region Credibility Mannual
        private void GenerateMarketExperienceSectionIICredibilityManual(ref SectionII sectionII, dynamic document)
        {
            CredibilityManual credibilityMannaul = new CredibilityManual();

            GenerateInpatientHospitalCredibilityManual(ref credibilityMannaul, document);
            GenerateOutpatientHospitalCredibilityManual(ref credibilityMannaul, document);
            GenerateProfessionalCredibilityManual(ref credibilityMannaul, document);
            GenerateOtherMedicalCredibilityManual(ref credibilityMannaul, document);
            GenerateCapitationCredibilityManual(ref credibilityMannaul, document);
            GeneratePrescriptionDrugCredibilityManual(ref credibilityMannaul, document);

            sectionII.CredibilityManual = credibilityMannaul;
        }

        private void GenerateInpatientHospitalCredibilityManual(ref CredibilityManual projection, dynamic document)
        {
            InpatientHospitalCredibilityManual inpatient = new InpatientHospitalCredibilityManual();
            inpatient.OnPer1000 = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.CredibilityManual.InpatientHospital.Utilizationper1000;
            inpatient.CostPerService = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.CredibilityManual.InpatientHospital.AverageCostService;
            inpatient.PMPM = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.CredibilityManual.InpatientHospital.PMPM;

            projection.InpatientHospitalCredibilityManual = inpatient;
        }

        private void GenerateOutpatientHospitalCredibilityManual(ref CredibilityManual projection, dynamic document)
        {
            OutpatientHospitalCredibilityManual outpatient = new OutpatientHospitalCredibilityManual();
            outpatient.OnPer1000 = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.CredibilityManual.OutpatientHospital.Utilizationper1000;
            outpatient.CostPerService = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.CredibilityManual.OutpatientHospital.AverageCostService;
            outpatient.PMPM = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.CredibilityManual.OutpatientHospital.PMPM;

            projection.OutpatientHospitalCredibilityManual = outpatient;
        }

        private void GenerateProfessionalCredibilityManual(ref CredibilityManual projection, dynamic document)
        {
            ProfessionalCredibilityManual professional = new ProfessionalCredibilityManual();
            professional.OnPer1000 = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.CredibilityManual.Professional.Utilizationper1000;
            professional.CostPerService = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.CredibilityManual.Professional.AverageCostService;
            professional.PMPM = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.CredibilityManual.Professional.PMPM;

            projection.ProfessionalCredibilityManual = professional;
        }

        private void GenerateOtherMedicalCredibilityManual(ref CredibilityManual projection, dynamic document)
        {
            OtherMedicalCredibilityManual otherMedical = new OtherMedicalCredibilityManual();
            otherMedical.OnPer1000 = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.CredibilityManual.OtherMedical.Utilizationper1000;
            otherMedical.CostPerService = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.CredibilityManual.OtherMedical.AverageCostService;
            otherMedical.PMPM = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.CredibilityManual.OtherMedical.PMPM;

            projection.OtherMedicalCredibilityManual = otherMedical;
        }

        private void GenerateCapitationCredibilityManual(ref CredibilityManual projection, dynamic document)
        {
            CapitationCredibilityManual capitation = new CapitationCredibilityManual();
            capitation.OnPer1000 = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.CredibilityManual.Capitation.Utilizationper1000;
            capitation.CostPerService = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.CredibilityManual.Capitation.AverageCostService;
            capitation.PMPM = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.CredibilityManual.Capitation.PMPM;

            projection.CapitationCredibilityManual = capitation;
        }

        private void GeneratePrescriptionDrugCredibilityManual(ref CredibilityManual projection, dynamic document)
        {
            PrescriptionDrugCredibilityManual prescriptionDrug = new PrescriptionDrugCredibilityManual();
            prescriptionDrug.OnPer1000 = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.CredibilityManual.PrescriptionDrug.Utilizationper1000;
            prescriptionDrug.CostPerService = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.CredibilityManual.PrescriptionDrug.AverageCostService;
            prescriptionDrug.PMPM = document.MarketExperience.SectionIIAllowedClaimsPMPMbasis.ExperiencePeriod.CredibilityManual.PrescriptionDrug.PMPM;

            projection.PrescriptionDrugCredibilityManual = prescriptionDrug;
        }
        #endregion Credibility Mannual
        #endregion Market Experience Section II

        #region Market Experience Section III
        private void GenerateMarketExperienceSectionIII(ref PlanURRTemplate planURRTTemplate, dynamic document)
        {
            SectionIII sectionIII = new SectionIII();

            GenerateSectionIIIProjectedAllowedExperienceClaimsPMPM(ref sectionIII, document);
            GenerateSectionIIIPaidtoAllowedAverageFactorinProjectionPeriod(ref sectionIII, document);
            GenerateSectionIIIProjectedIncurredClaimsBeforeACAAndRiskAdjustment(ref sectionIII, document);
            GenerateSectionIIIProjectedRiskAdjustmentsPMPM(ref sectionIII, document);
            GenerateSectionIIIProjectedIncurredClaimsBeforeReinsuranceRecoveries(ref sectionIII, document);
            GenerateSectionIIIProjectedACAReinsuranceRecoveries(ref sectionIII, document);
            GenerateSectionIIIProjectedIncurredClaims(ref sectionIII, document);
            GenerateSectionIIIAdministrativeExpenseLoad(ref sectionIII, document);
            GenerateSectionIIIProfitAndRiskLoad(ref sectionIII, document);
            GenerateSectionIIITaxesAndFees(ref sectionIII, document);
            GenerateSectionIIISingleRiskPoolGrossPremiumAverageRate(ref sectionIII, document);
            GenerateSectionIIIIndexRateforProjectionPeriod(ref sectionIII, document);

            sectionIII.ProjectedMemberMonths = document.MarketExperience.SectionIIIProjectedExperience.ProjectedMemberMonths.ProjectedPeriodTotals;
            planURRTTemplate.MarketExperience.SectionIII = sectionIII;
        }

        private void GenerateSectionIIIProjectedAllowedExperienceClaimsPMPM(ref SectionIII sectionIII, dynamic document)
        {
            ProjectedAllowedExperienceClaimsPMPM pmpm = new ProjectedAllowedExperienceClaimsPMPM();
            pmpm.BeforeCredibilityPMPM = document.MarketExperience.SectionIIIProjectedExperience.ProjectedAllowedExperienceClaimsPMPMwappliedcredibilityifapplicable.PMPM1;
            pmpm.AfterCredibilityPMPM = document.MarketExperience.SectionIIIProjectedExperience.ProjectedAllowedExperienceClaimsPMPMwappliedcredibilityifapplicable.PMPM2;
            pmpm.AfterCredibility = document.MarketExperience.SectionIIIProjectedExperience.ProjectedAllowedExperienceClaimsPMPMwappliedcredibilityifapplicable.AfterCredibility;
            pmpm.ProjectedPeriodTotals = document.MarketExperience.SectionIIIProjectedExperience.ProjectedAllowedExperienceClaimsPMPMwappliedcredibilityifapplicable.ProjectedPeriodTotals;

            sectionIII.ProjectedAllowedExperienceClaimsPMPM = pmpm;
        }

        private void GenerateSectionIIIPaidtoAllowedAverageFactorinProjectionPeriod(ref SectionIII sectionIII, dynamic document)
        {
            PaidtoAllowedAverageFactorinProjectionPeriod factor = new PaidtoAllowedAverageFactorinProjectionPeriod();
            factor.AfterCredibility = document.MarketExperience.SectionIIIProjectedExperience.PaidtoAllowedAverageFactorinProjectionPeriod.AfterCredibility;

            sectionIII.PaidtoAllowedAverageFactorinProjectionPeriod = factor;
        }

        private void GenerateSectionIIIProjectedIncurredClaimsBeforeACAAndRiskAdjustment(ref SectionIII sectionIII, dynamic document)
        {
            ProjectedIncurredClaimsBeforeACAAndRiskAdjustment adjustment = new ProjectedIncurredClaimsBeforeACAAndRiskAdjustment();
            adjustment.AfterCredibility = document.MarketExperience.SectionIIIProjectedExperience.ProjectedIncurredClaimsbeforeACAreinRiskAdjtPMPM.AfterCredibility;
            adjustment.ProjectedPeriodTotals = document.MarketExperience.SectionIIIProjectedExperience.ProjectedIncurredClaimsbeforeACAreinRiskAdjtPMPM.ProjectedPeriodTotals;

            sectionIII.ProjectedIncurredClaimsBeforeACAAndRiskAdjustment = adjustment;
        }

        private void GenerateSectionIIIProjectedRiskAdjustmentsPMPM(ref SectionIII sectionIII, dynamic document)
        {
            ProjectedRiskAdjustmentsPMPM adjustment = new ProjectedRiskAdjustmentsPMPM();
            adjustment.AfterCredibility = document.MarketExperience.SectionIIIProjectedExperience.ProjectedRiskAdjustmentsPMPM.AfterCredibility;
            adjustment.ProjectedPeriodTotals = document.MarketExperience.SectionIIIProjectedExperience.ProjectedRiskAdjustmentsPMPM.ProjectedPeriodTotals;

            sectionIII.ProjectedRiskAdjustmentsPMPM = adjustment;
        }

        private void GenerateSectionIIIProjectedIncurredClaimsBeforeReinsuranceRecoveries(ref SectionIII sectionIII, dynamic document)
        {
            ProjectedIncurredClaimsBeforeReinsuranceRecoveries adjustment = new ProjectedIncurredClaimsBeforeReinsuranceRecoveries();
            adjustment.AfterCredibility = document.MarketExperience.SectionIIIProjectedExperience.ProjectedIncurredClaimsbeforereinsurancerecoveriesnetofreinpremPMPM.AfterCredibility;
            adjustment.ProjectedPeriodTotals = document.MarketExperience.SectionIIIProjectedExperience.ProjectedIncurredClaimsbeforereinsurancerecoveriesnetofreinpremPMPM.ProjectedPeriodTotals;

            sectionIII.ProjectedIncurredClaimsBeforeReinsuranceRecoveries = adjustment;
        }

        private void GenerateSectionIIIProjectedACAReinsuranceRecoveries(ref SectionIII sectionIII, dynamic document)
        {
            ProjectedACAReinsuranceRecoveries adjustment = new ProjectedACAReinsuranceRecoveries();
            adjustment.AfterCredibility = document.MarketExperience.SectionIIIProjectedExperience.ProjectedACAreinsurancerecoveriesnetofreinpremPMPM.AfterCredibility;
            adjustment.ProjectedPeriodTotals = document.MarketExperience.SectionIIIProjectedExperience.ProjectedACAreinsurancerecoveriesnetofreinpremPMPM.ProjectedPeriodTotals;

            sectionIII.ProjectedACAReinsuranceRecoveries = adjustment;
        }

        private void GenerateSectionIIIProjectedIncurredClaims(ref SectionIII sectionIII, dynamic document)
        {
            ProjectedIncurredClaims adjustment = new ProjectedIncurredClaims();
            adjustment.AfterCredibility = document.MarketExperience.SectionIIIProjectedExperience.ProjectedIncurredClaims.AfterCredibility;
            adjustment.ProjectedPeriodTotals = document.MarketExperience.SectionIIIProjectedExperience.ProjectedIncurredClaims.ProjectedPeriodTotals;

            sectionIII.ProjectedIncurredClaims = adjustment;
        }

        private void GenerateSectionIIIAdministrativeExpenseLoad(ref SectionIII sectionIII, dynamic document)
        {
            AdministrativeExpenseLoad adjustment = new AdministrativeExpenseLoad();
            adjustment.PMPM = document.MarketExperience.SectionIIIProjectedExperience.AdministrativeExpenseLoad.PMPM2;
            adjustment.AfterCredibility = document.MarketExperience.SectionIIIProjectedExperience.AdministrativeExpenseLoad.AfterCredibility;
            adjustment.ProjectedPeriodTotals = document.MarketExperience.SectionIIIProjectedExperience.AdministrativeExpenseLoad.ProjectedPeriodTotals;

            sectionIII.AdministrativeExpenseLoad = adjustment;
        }

        private void GenerateSectionIIIProfitAndRiskLoad(ref SectionIII sectionIII, dynamic document)
        {
            ProfitAndRiskLoad adjustment = new ProfitAndRiskLoad();
            adjustment.PMPM = document.MarketExperience.SectionIIIProjectedExperience.ProfitRiskLoad.PMPM2;
            adjustment.AfterCredibility = document.MarketExperience.SectionIIIProjectedExperience.ProfitRiskLoad.AfterCredibility;
            adjustment.ProjectedPeriodTotals = document.MarketExperience.SectionIIIProjectedExperience.ProfitRiskLoad.ProjectedPeriodTotals;

            sectionIII.ProfitAndRiskLoad = adjustment;
        }

        private void GenerateSectionIIITaxesAndFees(ref SectionIII sectionIII, dynamic document)
        {
            TaxesAndFees adjustment = new TaxesAndFees();
            adjustment.PMPM = document.MarketExperience.SectionIIIProjectedExperience.TaxesFees.PMPM2;
            adjustment.AfterCredibility = document.MarketExperience.SectionIIIProjectedExperience.TaxesFees.AfterCredibility;
            adjustment.ProjectedPeriodTotals = document.MarketExperience.SectionIIIProjectedExperience.TaxesFees.ProjectedPeriodTotals;

            sectionIII.TaxesAndFees = adjustment;
        }

        private void GenerateSectionIIISingleRiskPoolGrossPremiumAverageRate(ref SectionIII sectionIII, dynamic document)
        {
            SingleRiskPoolGrossPremiumAverageRate adjustment = new SingleRiskPoolGrossPremiumAverageRate();
            adjustment.AfterCredibility = document.MarketExperience.SectionIIIProjectedExperience.SingleRiskPoolGrossPremiumAvgRatePMPM.AfterCredibility;
            adjustment.ProjectedPeriodTotals = document.MarketExperience.SectionIIIProjectedExperience.SingleRiskPoolGrossPremiumAvgRatePMPM.ProjectedPeriodTotals;

            sectionIII.SingleRiskPoolGrossPremiumAverageRate = adjustment;
        }

        private void GenerateSectionIIIIndexRateforProjectionPeriod(ref SectionIII sectionIII, dynamic document)
        {
            IndexRateforProjectionPeriod adjustment = new IndexRateforProjectionPeriod();
            adjustment.AfterCredibility = document.MarketExperience.SectionIIIProjectedExperience.IndexRateforProjectionPeriod.AfterCredibility;
            adjustment.PercentageIncreaseOverExperiencePeriodAfterCredibility = document.MarketExperience.SectionIIIProjectedExperience.increaseoverExperiencePeriod.AfterCredibility;
            adjustment.PercentageIncreaseAnnualized = document.MarketExperience.SectionIIIProjectedExperience.Increaseannualized.AfterCredibility;

            sectionIII.IndexRateforProjectionPeriod = adjustment;
        }
        #endregion Market Experience Section III
        #endregion Market Experience Methods

        #region Plan Product Info Methods
        private void GeneratePlanProductInfo(ref PlanURRTemplate planURRTTemplate, IList<PlanBenefitPackage> planBenefitPackageList)
        {

            GeneratePlanProductInfoHeader(ref planURRTTemplate);
            foreach (PlanBenefitPackage package in planBenefitPackageList)
            {
                GeneratePlanProductInfo(ref planURRTTemplate, package);
            }
        }

        #region Plan Product Info Header
        private void GeneratePlanProductInfoHeader(ref PlanURRTemplate planURRTTemplate)
        {
            PlanProductInfoHeader header = new PlanProductInfoHeader();

            header.CompanyLegalName = planURRTTemplate.MarketExperience.Header.CompanyLegalName;
            header.State = planURRTTemplate.MarketExperience.Header.State;
            header.HIOSIssuerID = planURRTTemplate.MarketExperience.Header.HIOSIssuerID;
            header.Market = planURRTTemplate.MarketExperience.Header.Market;
            header.EffectiveDateofRateChanges = planURRTTemplate.MarketExperience.Header.EffectiveDateofRateChanges;

            planURRTTemplate.PlanProductInfo = new PlanProductInfo();
            planURRTTemplate.PlanProductInfo.PlanProductInfoHeader = header;
        }
        #endregion Plan Product Info Header

        #region Plan Product Info
        private void GeneratePlanProductInfo(ref PlanURRTemplate planURRTTemplate, PlanBenefitPackage package)
        {
            planURRTTemplate.PlanProductInfo.ProductList = new List<ProductInfo>();

            foreach (var item in package.SectionIGeneralProductandPlanInformation)
            {
                ProductInfo product = new ProductInfo();

                GenerateProductGeneralInformation(ref product, item);
                GenerateComponentOfPremiumIncrease(ref product, package);
                GenerateExperiencePeriodInformation(ref product, package);
                GenerateProjectedExperiencePeriodInformation(ref product, package);
                planURRTTemplate.PlanProductInfo.ProductList.Add(product);
            }
        }

        private void GenerateProductGeneralInformation(ref ProductInfo product, SectionIGeneralProductandPlanInformation productGeneralInfo)
        {
            GeneralPlanAndProductInformation generalInfo = new GeneralPlanAndProductInformation();
            generalInfo.ProductName = productGeneralInfo.Product;
            generalInfo.ProductID = productGeneralInfo.ProductID;
            generalInfo.Metal = productGeneralInfo.Metal;
            generalInfo.AVMetalValue = productGeneralInfo.AVMetalValue;
            generalInfo.AVPricingValue = productGeneralInfo.AVPricingValue;
            generalInfo.PlanType = productGeneralInfo.PlanType;
            generalInfo.PlanName = productGeneralInfo.PlanName;
            generalInfo.PlanIDStandardComponentID = productGeneralInfo.PlanIDStandardComponentID;
            generalInfo.ExchangePlan = productGeneralInfo.ExchangePlan;
            generalInfo.HistoricalRateRelaeseCalendarYear2 = productGeneralInfo.HistoricalRateIncreaseCalendarYear2;
            generalInfo.HistoricalRateReleaseCalendarYear1 = productGeneralInfo.HistoricalRateIncreaseCalendarYear1;
            generalInfo.HistoricalRateReleaseCalendarYear0 = productGeneralInfo.HistoricalRateIncreaseCalendarYear0;
            generalInfo.EffectiveDateOfProposedRates = productGeneralInfo.EffectiveDateofProposedRates;
            generalInfo.RateChangePercentage = productGeneralInfo.RateChangePercentoverpreviousfiling;
            generalInfo.CummulativeRateChangePercentage = productGeneralInfo.CumulativeRateChangePercentover12mosprior;
            generalInfo.ProjectedPerRateChangePercentage = productGeneralInfo.ProjectedPerRateChangePercentageoverExperiencePeriod;
            generalInfo.ProductThresholdRateIncreasePercentage = productGeneralInfo.ProductThresholdRateIncreasePercentage;

            product.GeneralPlanAndProductInformation = generalInfo;
        }

        private void GenerateComponentOfPremiumIncrease(ref ProductInfo product, PlanBenefitPackage package)
        {
            var productStandardComponentID = product.GeneralPlanAndProductInformation.PlanIDStandardComponentID;
            foreach (var item in package.SectionIIComponentsofPremiumIncreasePMPMDollarAmountaboveCurrentAverag
                            .Where(c => c.PlanIDStandardComponentID == productStandardComponentID))
            {
                ComponentOfPremiumIncrease permiumIncress = new ComponentOfPremiumIncrease();

                permiumIncress.PlanIDStandardComponentID = item.PlanIDStandardComponentID;
                permiumIncress.Inpatient = item.Inpatient;
                permiumIncress.OutPatient = item.Outpatient;
                permiumIncress.Professional = item.Professional;
                permiumIncress.PrescriptionDrug = item.PrescriptionDrug;
                permiumIncress.Other = item.Other;
                permiumIncress.Capitation = item.Capitation;
                permiumIncress.Administration = item.Administration;
                permiumIncress.TaxesAndFees = item.TaxesFees;
                permiumIncress.RiskAndProfitCharge = item.RiskProfitCharge;
                permiumIncress.TotalRateIncrease = item.TotalRateIncrease;
                permiumIncress.MemberCostShareIncrease = item.MemberCostShareIncrease;
                permiumIncress.AverageCurrentRatePMPM = item.AverageCurrentRatePMPM;
                permiumIncress.ProjectedMemberMonths = item.ProjectedMemberMonths;
                product.ComponentOfPremiumIncrease = permiumIncress;
            }
        }

        private void GenerateExperiencePeriodInformation(ref ProductInfo product, PlanBenefitPackage package)
        {
            var productStandardComponentID = product.GeneralPlanAndProductInformation.PlanIDStandardComponentID;
            foreach (var item in package.SectionIIIExperiencePeriodInformation.Where(c => c.ClaimsInformation
                                                                                            .Where(d => d.PlanIDStandardComponentID == productStandardComponentID).Any()
                                                                            && c.PremiumInformation.Where(d => d.PlanIDStandardComponentID == productStandardComponentID).Any()))
            {
                ExperiencePeriodInformation experienceInfo = new ExperiencePeriodInformation();

                foreach (var premium in item.PremiumInformation)
                {
                    experienceInfo.PlanIDStandardComponentID = premium.PlanIDStandardComponentID;
                    experienceInfo.AverageRatePMPM = premium.AverageRatePMPM;
                    experienceInfo.MemberMonths = premium.MemberMonths;
                    experienceInfo.TotalPremium = premium.TotalPremiumTP;
                    experienceInfo.EHBPercentageOfTotalPremium = premium.EHBPercentofTP;
                    experienceInfo.StateMandatedBenefitPortionPercentageOfTotalPremium = premium.StatemandatedbenefitsportionofTPthatareotherthanEHB;
                    experienceInfo.OtherBenefitPortionOfTotalPremium = premium.OtherbenefitsportionofTP;
                }
                foreach (var claim in item.ClaimsInformation)
                {
                    experienceInfo.TotalAllowedClaims = claim.TotalAllowedClaimsTAC;
                    experienceInfo.EHBPercentageOfTotalAllowedClaims = claim.EHBPercentofTAC;
                    experienceInfo.StateMandatedBenefitPortionPercentageOfTotalAllowedClaims = claim.StatemandatedbenefitsportionofTACthatareotherthanEHB;
                    experienceInfo.OtherBenefitPortionTotalAllowedClaims = claim.OtherbenefitsportionofTAC;
                    experienceInfo.NonIssuerObligationAllowedClaims = claim.AllowedClaimswhicharenottheissuersobligation;
                    experienceInfo.HHSFundPortionDollors = claim.PortionoftheabovepayablebyHHSsfundsonbehalfofinsuredpersonsindollars;
                    experienceInfo.HHSFundPortionPercentage = claim.PortionofabovepayablebyHHSonbehalfofinsuredpersonasPercentage;
                    experienceInfo.TotalIncurredClaimsWithIssuerFunds = claim.TotalIncurredclaimspayablewithissuerfunds;
                    experienceInfo.NetAmountOfReinsurance = claim.NetAmtofRein;
                    experienceInfo.NetAmountOfRiskAdjustment = claim.NetAmtofRiskAdj;
                    experienceInfo.IncurredClaimsPMPM = claim.IncurredClaimsPMPM;
                    experienceInfo.AllowedClaimsPMPM = claim.AllowedClaimsPMPM;
                    experienceInfo.EHBPortionOfAllowedClaimsPMPM = claim.EHBportionofAllowedClaimsPMPM;
                }
                product.ExperiencePeriodInformation = experienceInfo;
            }
        }

        private void GenerateProjectedExperiencePeriodInformation(ref ProductInfo product, PlanBenefitPackage package)
        {
            var productStandardComponentID = product.GeneralPlanAndProductInformation.PlanIDStandardComponentID;
            foreach (var item in package.SectionIVProjected12monthsfollowingeffectivedate.Where(c => c.ClaimsInformation
                                                                                            .Where(d => d.PlanIDStandardComponentID == productStandardComponentID).Any()
                                                                            && c.PremiumInformation.Where(d => d.PlanIDStandardComponentID == productStandardComponentID).Any()))
            {
                ProjectedExperiencePeriodInformation projectedExperienceInfo = new ProjectedExperiencePeriodInformation();

                foreach (var premium in item.PremiumInformation)
                {
                    projectedExperienceInfo.PlanIDStandardComponentID = premium.PlanIDStandardComponentID;
                    projectedExperienceInfo.PlanAdjustedIndexRate = premium.PlanAdjustedIndexRate;
                    projectedExperienceInfo.MemberMonths = premium.MemberMonths;
                    projectedExperienceInfo.TotalPremium = premium.TotalPremiumTP;
                    projectedExperienceInfo.EHBPercentageOfTotalPremium = premium.EHBPercentofTP;
                    projectedExperienceInfo.StateMandatedBenefitPortionPercentageOfTotalPremium = premium.StatemandatedbenefitsportionofTPthatareotherthanEHB;
                    projectedExperienceInfo.OtherBenefitPortionOfTotalPremium = premium.OtherbenefitsportionofTP;
                }
                foreach (var claim in item.ClaimsInformation)
                {
                    projectedExperienceInfo.TotalAllowedClaims = claim.TotalAllowedClaimsTAC;
                    projectedExperienceInfo.EHBPercentageOfTotalAllowedClaims = claim.EHBPercentofTAC;
                    projectedExperienceInfo.StateMandatedBenefitPortionPercentageOfTotalAllowedClaims = claim.StatemandatedbenefitsportionofTACthatareotherthanEHB;
                    projectedExperienceInfo.OtherBenefitPortionTotalAllowedClaims = claim.OtherbenefitsportionofTAC;
                    projectedExperienceInfo.NonIssuerObligationAllowedClaims = claim.AllowedClaimswhicharenottheissuersobligation;
                    projectedExperienceInfo.HHSFundPortionDollors = claim.PortionoftheabovepayablebyHHSsfundsonbehalfofinsuredpersonsindollars;
                    projectedExperienceInfo.HHSFundPortionPercentage = claim.PortionofabovepayablebyHHSonbehalfofinsuredpersonasPercentage;
                    projectedExperienceInfo.TotalIncurredClaimsWithIssuerFunds = claim.TotalIncurredclaimspayablewithissuerfunds;
                    projectedExperienceInfo.NetAmountOfReinsurance = claim.NetAmtofRein;
                    projectedExperienceInfo.NetAmountOfRiskAdjustment = claim.NetAmtofRiskAdj;
                    projectedExperienceInfo.IncurredClaimsPMPM = claim.IncurredClaimsPMPM;
                    projectedExperienceInfo.AllowedClaimsPMPM = claim.AllowedClaimsPMPM;
                    projectedExperienceInfo.EHBPortionOfAllowedClaimsPMPM = claim.EHBportionofAllowedClaimsPMPM;
                }
                product.ProjectedExperiencePeriodInformation = projectedExperienceInfo;
            }
        }

        #endregion Plan Product Info
        #endregion Plan Product Info Methods
        #endregion Private Methods
    }
}
