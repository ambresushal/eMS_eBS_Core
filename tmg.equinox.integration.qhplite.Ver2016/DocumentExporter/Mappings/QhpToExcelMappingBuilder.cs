using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.integration.qhplite.Ver2016
{
    internal class QhpToExcelMappingBuilder
    {
        #region Private Memebers
        private IList<QhpToExcelMap> MappingsList { get; set; }
        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor
        public QhpToExcelMappingBuilder()
        {
            this.MappingsList = new List<QhpToExcelMap>();
        }
        #endregion Constructor

        #region Public Methods
        public void BuildMappings()
        {
            BuildHeaderMappings();
            BuildPlanIdentifiers();
            BuildPlanAttributes();
            BuildStandAlonDentalOnlyAttributes();
            BuildPlanDateAttributes();
            BuildGeographicalCoverageAttributes();
            BuildPlanLevelURLs();
            BuildBenefitInformation();
            BuildBenefitGeneralInformation();
            BuildOutofPocketExceptions();
            BuildPlanCostSharingAttributes();
            BuildSBCScenarioAttributes();
            BuildMaximumOutOfPocketMaxAttributes();
            BuildDeductibleAttributes();
            BuildHSAHRADetail();
            BuildPlanVariantLevelURLs();
            BuildAVCalculatorAttributes();
            BuildDeductibleSubGroupAttributes();
            BuildBenefitPlanDetailsAttributes();
        }

        public QhpToExcelMap GetMap(string formPropertyName, string parentPropertyName, QHPSheetType sheeType)
        {
            try
            {
                if (string.IsNullOrEmpty(parentPropertyName))
                {
                    return this.MappingsList
                            .Where(c => c.DomainPropertyName.Equals(formPropertyName, StringComparison.InvariantCultureIgnoreCase)
                                        && c.QhpSheetType == sheeType)
                            .FirstOrDefault();
                }
                else
                {
                    return this.MappingsList
                               .Where(c => c.DomainPropertyName.Equals(formPropertyName, StringComparison.InvariantCultureIgnoreCase)
                                && (c.ParentPropertyName != null ? c.ParentPropertyName.Equals(parentPropertyName, StringComparison.InvariantCultureIgnoreCase) : true)
                                 && c.QhpSheetType == sheeType)
                               .FirstOrDefault();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public QhpToExcelMap GetDeductibleSubGroupStartingCell()
        {
            return MappingsList.Where(c => c.QhpSheetType == QHPSheetType.CostShareVariance && c.ParentPropertyName == "DeductibleSubGroups" && c.ColumnName == "CL").FirstOrDefault();
        }
        #endregion Public Methods

        #region Private Methods
        private void BuildHeaderMappings()
        {
            QhpToExcelMap hiosIssuerIDMap = new QhpToExcelMap();
            hiosIssuerIDMap.FormPropertyName = "HIOSIssuerID";
            hiosIssuerIDMap.DomainPropertyName = "HIOSIssuerID";
            hiosIssuerIDMap.ColumnName = "B";
            hiosIssuerIDMap.RowIndex = 2;
            hiosIssuerIDMap.CellFormat.CellAlignment = "Right";
            hiosIssuerIDMap.CellFormat.DataType = "int";

            this.MappingsList.Add(hiosIssuerIDMap);

            QhpToExcelMap issuerStateMap = new QhpToExcelMap();
            issuerStateMap.FormPropertyName = "IssuerState";
            issuerStateMap.DomainPropertyName = "IssuerState";
            issuerStateMap.ColumnName = "B";
            issuerStateMap.RowIndex = 3;
            issuerStateMap.CellFormat.CellAlignment = "Left";
            issuerStateMap.CellFormat.DataType = "string";

            this.MappingsList.Add(issuerStateMap);

            QhpToExcelMap marketCoverageMap = new QhpToExcelMap();
            marketCoverageMap.FormPropertyName = "MarketCoverage";
            marketCoverageMap.DomainPropertyName = "MarketCoverage";
            marketCoverageMap.ColumnName = "B";
            marketCoverageMap.RowIndex = 4;
            marketCoverageMap.CellFormat.CellAlignment = "Left";
            marketCoverageMap.CellFormat.DataType = "string";

            this.MappingsList.Add(marketCoverageMap);

            QhpToExcelMap dentalOnlyPlanMap = new QhpToExcelMap();
            dentalOnlyPlanMap.FormPropertyName = "DentalOnlyPlan";
            dentalOnlyPlanMap.DomainPropertyName = "DentalOnlyPlan";
            dentalOnlyPlanMap.ColumnName = "B";
            dentalOnlyPlanMap.RowIndex = 5;
            dentalOnlyPlanMap.CellFormat.CellAlignment = "Left";
            dentalOnlyPlanMap.CellFormat.DataType = "string";

            this.MappingsList.Add(dentalOnlyPlanMap);

            QhpToExcelMap tinMap = new QhpToExcelMap();
            tinMap.FormPropertyName = "TIN";
            tinMap.DomainPropertyName = "TIN";
            tinMap.ColumnName = "B";
            tinMap.RowIndex = 6;
            tinMap.CellFormat.CellAlignment = "Left";
            tinMap.CellFormat.DataType = "string";

            this.MappingsList.Add(tinMap);
        }

        private void BuildPlanIdentifiers()
        {
            QhpToExcelMap HIOSPlanIDStandardComponentMap = new QhpToExcelMap();
            HIOSPlanIDStandardComponentMap.FormPropertyName = "HIOSPlanIDStandardComponent";
            HIOSPlanIDStandardComponentMap.DomainPropertyName = "HIOSPlanID";
            HIOSPlanIDStandardComponentMap.ColumnName = "A";
            HIOSPlanIDStandardComponentMap.RowIndex = 9;
            HIOSPlanIDStandardComponentMap.QhpSheetType = QHPSheetType.BenefitPackage;
            HIOSPlanIDStandardComponentMap.CellFormat.CellAlignment = "Right";
            HIOSPlanIDStandardComponentMap.CellFormat.DataType = "string";

            this.MappingsList.Add(HIOSPlanIDStandardComponentMap);

            QhpToExcelMap PlanMarketingNameMap = new QhpToExcelMap();
            PlanMarketingNameMap.FormPropertyName = "PlanMarketingName";
            PlanMarketingNameMap.DomainPropertyName = "PlanMarketingName";
            PlanMarketingNameMap.ColumnName = "B";
            PlanMarketingNameMap.RowIndex = 9;
            PlanMarketingNameMap.QhpSheetType = QHPSheetType.BenefitPackage;
            PlanMarketingNameMap.CellFormat.CellAlignment = "Left";
            PlanMarketingNameMap.CellFormat.DataType = "string";

            this.MappingsList.Add(PlanMarketingNameMap);

            QhpToExcelMap HIOSProductIDMap = new QhpToExcelMap();
            HIOSProductIDMap.FormPropertyName = "HIOSProductID";
            HIOSProductIDMap.DomainPropertyName = "HIOSProductID";
            HIOSProductIDMap.ColumnName = "C";
            HIOSProductIDMap.RowIndex = 9;
            HIOSProductIDMap.QhpSheetType = QHPSheetType.BenefitPackage;
            HIOSProductIDMap.CellFormat.CellAlignment = "Left";
            HIOSProductIDMap.CellFormat.DataType = "int";

            this.MappingsList.Add(HIOSProductIDMap);

            QhpToExcelMap HPIDMap = new QhpToExcelMap();
            HPIDMap.FormPropertyName = "HPID";
            HPIDMap.DomainPropertyName = "HPID";
            HPIDMap.ColumnName = "D";
            HPIDMap.RowIndex = 9;
            HPIDMap.QhpSheetType = QHPSheetType.BenefitPackage;
            HPIDMap.CellFormat.CellAlignment = "Right";
            HPIDMap.CellFormat.DataType = "int";

            this.MappingsList.Add(HPIDMap);

            QhpToExcelMap NetworkIDMap = new QhpToExcelMap();
            NetworkIDMap.FormPropertyName = "NetworkID";
            NetworkIDMap.DomainPropertyName = "NetworkID";
            NetworkIDMap.ColumnName = "E";
            NetworkIDMap.RowIndex = 9;
            NetworkIDMap.QhpSheetType = QHPSheetType.BenefitPackage;
            NetworkIDMap.CellFormat.CellAlignment = "Left";
            NetworkIDMap.CellFormat.DataType = "string";

            this.MappingsList.Add(NetworkIDMap);

            QhpToExcelMap ServiceAreaIDMap = new QhpToExcelMap();
            ServiceAreaIDMap.FormPropertyName = "ServiceAreaID";
            ServiceAreaIDMap.DomainPropertyName = "ServiceAreaID";
            ServiceAreaIDMap.ColumnName = "F";
            ServiceAreaIDMap.RowIndex = 9;
            ServiceAreaIDMap.QhpSheetType = QHPSheetType.BenefitPackage;
            ServiceAreaIDMap.CellFormat.CellAlignment = "Left";
            ServiceAreaIDMap.CellFormat.DataType = "string";

            this.MappingsList.Add(ServiceAreaIDMap);

            QhpToExcelMap FormularyIDMap = new QhpToExcelMap();
            FormularyIDMap.FormPropertyName = "FormularyID";
            FormularyIDMap.DomainPropertyName = "FormularyID";
            FormularyIDMap.ColumnName = "G";
            FormularyIDMap.RowIndex = 9;
            FormularyIDMap.QhpSheetType = QHPSheetType.BenefitPackage;
            FormularyIDMap.CellFormat.CellAlignment = "Left";
            FormularyIDMap.CellFormat.DataType = "string";

            this.MappingsList.Add(FormularyIDMap);
        }

        private void BuildPlanAttributes()
        {
            QhpToExcelMap NewExistingPlanMap = new QhpToExcelMap();
            NewExistingPlanMap.FormPropertyName = "NewExistingPlan";
            NewExistingPlanMap.DomainPropertyName = "NewExistingPlan";
            NewExistingPlanMap.ColumnName = "H";
            NewExistingPlanMap.RowIndex = 9;
            NewExistingPlanMap.QhpSheetType = QHPSheetType.BenefitPackage;
            NewExistingPlanMap.CellFormat.CellAlignment = "Left";
            NewExistingPlanMap.CellFormat.DataType = "string";

            this.MappingsList.Add(NewExistingPlanMap);

            QhpToExcelMap PlanTypeMap = new QhpToExcelMap();
            PlanTypeMap.FormPropertyName = "PlanType";
            PlanTypeMap.DomainPropertyName = "PlanType";
            PlanTypeMap.ColumnName = "I";
            PlanTypeMap.RowIndex = 9;
            PlanTypeMap.QhpSheetType = QHPSheetType.BenefitPackage;
            PlanTypeMap.CellFormat.CellAlignment = "Left";
            PlanTypeMap.CellFormat.DataType = "string";

            this.MappingsList.Add(PlanTypeMap);

            QhpToExcelMap LevelOfCoverageMap = new QhpToExcelMap();
            LevelOfCoverageMap.FormPropertyName = "LevelofCoverage";
            LevelOfCoverageMap.DomainPropertyName = "LevelOfCoverage";
            LevelOfCoverageMap.ColumnName = "J";
            LevelOfCoverageMap.RowIndex = 9;
            LevelOfCoverageMap.QhpSheetType = QHPSheetType.BenefitPackage;
            LevelOfCoverageMap.CellFormat.CellAlignment = "Left";
            LevelOfCoverageMap.CellFormat.DataType = "string";

            this.MappingsList.Add(LevelOfCoverageMap);

            QhpToExcelMap DesignTypeMap = new QhpToExcelMap();
            DesignTypeMap.FormPropertyName = "DesignType";
            DesignTypeMap.DomainPropertyName = "DesignType";
            DesignTypeMap.ColumnName = "K";
            DesignTypeMap.RowIndex = 9;
            DesignTypeMap.QhpSheetType = QHPSheetType.BenefitPackage;
            DesignTypeMap.CellFormat.CellAlignment = "Left";
            DesignTypeMap.CellFormat.DataType = "string";

            this.MappingsList.Add(DesignTypeMap);

            QhpToExcelMap UniquePlanDesignMap = new QhpToExcelMap();
            UniquePlanDesignMap.FormPropertyName = "UniquePlanDesign";
            UniquePlanDesignMap.DomainPropertyName = "UniquePlanDesign";
            UniquePlanDesignMap.ColumnName = "L";
            UniquePlanDesignMap.RowIndex = 9;
            UniquePlanDesignMap.QhpSheetType = QHPSheetType.BenefitPackage;
            UniquePlanDesignMap.CellFormat.CellAlignment = "Left";
            UniquePlanDesignMap.CellFormat.DataType = "string";

            this.MappingsList.Add(UniquePlanDesignMap);

            QhpToExcelMap QHPNonQHPMap = new QhpToExcelMap();
            QHPNonQHPMap.FormPropertyName = "QHPNonQHP";
            QHPNonQHPMap.DomainPropertyName = "QHPNonQHP";
            QHPNonQHPMap.ColumnName = "M";
            QHPNonQHPMap.RowIndex = 9;
            QHPNonQHPMap.QhpSheetType = QHPSheetType.BenefitPackage;
            QHPNonQHPMap.CellFormat.CellAlignment = "Left";
            QHPNonQHPMap.CellFormat.DataType = "string";

            this.MappingsList.Add(QHPNonQHPMap);

            QhpToExcelMap NoticeRequiredForPregnancyMap = new QhpToExcelMap();
            NoticeRequiredForPregnancyMap.FormPropertyName = "NoticePeriodforPregnancy";
            NoticeRequiredForPregnancyMap.DomainPropertyName = "NoticeRequiredForPregnancy";
            NoticeRequiredForPregnancyMap.ColumnName = "N";
            NoticeRequiredForPregnancyMap.RowIndex = 9;
            NoticeRequiredForPregnancyMap.QhpSheetType = QHPSheetType.BenefitPackage;
            NoticeRequiredForPregnancyMap.CellFormat.CellAlignment = "Left";
            NoticeRequiredForPregnancyMap.CellFormat.DataType = "string";

            this.MappingsList.Add(NoticeRequiredForPregnancyMap);

            QhpToExcelMap IsAReferralRequiredForSpecialistMap = new QhpToExcelMap();
            IsAReferralRequiredForSpecialistMap.FormPropertyName = "IsReferralRequiredforSpecialist";
            IsAReferralRequiredForSpecialistMap.DomainPropertyName = "IsAReferralRequiredForSpecialist";
            IsAReferralRequiredForSpecialistMap.ColumnName = "O";
            IsAReferralRequiredForSpecialistMap.RowIndex = 9;
            IsAReferralRequiredForSpecialistMap.QhpSheetType = QHPSheetType.BenefitPackage;
            IsAReferralRequiredForSpecialistMap.CellFormat.CellAlignment = "Left";
            IsAReferralRequiredForSpecialistMap.CellFormat.DataType = "string";

            this.MappingsList.Add(IsAReferralRequiredForSpecialistMap);

            QhpToExcelMap SpecialistsRequiringAReferralMap = new QhpToExcelMap();
            SpecialistsRequiringAReferralMap.FormPropertyName = "SpecialistsRequiringaReferral";
            SpecialistsRequiringAReferralMap.DomainPropertyName = "SpecialistsRequiringAReferral";
            SpecialistsRequiringAReferralMap.ColumnName = "P";
            SpecialistsRequiringAReferralMap.RowIndex = 9;
            SpecialistsRequiringAReferralMap.QhpSheetType = QHPSheetType.BenefitPackage;
            SpecialistsRequiringAReferralMap.CellFormat.CellAlignment = "Left";
            SpecialistsRequiringAReferralMap.CellFormat.DataType = "string";

            this.MappingsList.Add(SpecialistsRequiringAReferralMap);

            QhpToExcelMap PlanLevelExclusionsMap = new QhpToExcelMap();
            PlanLevelExclusionsMap.FormPropertyName = "PlanLevelExclusions";
            PlanLevelExclusionsMap.DomainPropertyName = "PlanLevelExclusions";
            PlanLevelExclusionsMap.ColumnName = "Q";
            PlanLevelExclusionsMap.RowIndex = 9;
            PlanLevelExclusionsMap.QhpSheetType = QHPSheetType.BenefitPackage;
            PlanLevelExclusionsMap.CellFormat.CellAlignment = "Left";
            PlanLevelExclusionsMap.CellFormat.DataType = "string";

            this.MappingsList.Add(PlanLevelExclusionsMap);

            QhpToExcelMap LimitedCostSharingPlanVariationMap = new QhpToExcelMap();
            LimitedCostSharingPlanVariationMap.FormPropertyName = "LimitedCostSharingPlanVariationEstAdvancedPayment";
            LimitedCostSharingPlanVariationMap.DomainPropertyName = "LimitedCostSharingPlanVariation";
            LimitedCostSharingPlanVariationMap.ColumnName = "R";
            LimitedCostSharingPlanVariationMap.RowIndex = 9;
            LimitedCostSharingPlanVariationMap.QhpSheetType = QHPSheetType.BenefitPackage;
            LimitedCostSharingPlanVariationMap.CellFormat.CellAlignment = "Right";
            LimitedCostSharingPlanVariationMap.CellFormat.DataType = "decimal";
            LimitedCostSharingPlanVariationMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(LimitedCostSharingPlanVariationMap);//

            QhpToExcelMap DoesthisplanofferCompositeRatingMap = new QhpToExcelMap();
            DoesthisplanofferCompositeRatingMap.FormPropertyName = "DoesthisplanofferCompositeRating";
            DoesthisplanofferCompositeRatingMap.DomainPropertyName = "DoesthisplanofferCompositeRating";
            DoesthisplanofferCompositeRatingMap.ColumnName = "S";
            DoesthisplanofferCompositeRatingMap.RowIndex = 9;
            DoesthisplanofferCompositeRatingMap.QhpSheetType = QHPSheetType.BenefitPackage;
            DoesthisplanofferCompositeRatingMap.CellFormat.CellAlignment = "Left";
            DoesthisplanofferCompositeRatingMap.CellFormat.DataType = "string";

            this.MappingsList.Add(DoesthisplanofferCompositeRatingMap);          

            QhpToExcelMap ChildOnlyOfferingMap = new QhpToExcelMap();
            ChildOnlyOfferingMap.FormPropertyName = "ChildOnlyOffering";
            ChildOnlyOfferingMap.DomainPropertyName = "ChildOnlyOffering";
            ChildOnlyOfferingMap.ColumnName = "T";
            ChildOnlyOfferingMap.RowIndex = 9;
            ChildOnlyOfferingMap.QhpSheetType = QHPSheetType.BenefitPackage;
            ChildOnlyOfferingMap.CellFormat.CellAlignment = "Left";
            ChildOnlyOfferingMap.CellFormat.DataType = "string";

            this.MappingsList.Add(ChildOnlyOfferingMap);

            QhpToExcelMap ChildOnlyPlanIDMap = new QhpToExcelMap();
            ChildOnlyPlanIDMap.FormPropertyName = "ChildOnlyPlanID";
            ChildOnlyPlanIDMap.DomainPropertyName = "ChildOnlyPlanID";
            ChildOnlyPlanIDMap.ColumnName = "U";
            ChildOnlyPlanIDMap.RowIndex = 9;
            ChildOnlyPlanIDMap.QhpSheetType = QHPSheetType.BenefitPackage;
            ChildOnlyPlanIDMap.CellFormat.CellAlignment = "Left";
            ChildOnlyPlanIDMap.CellFormat.DataType = "string";

            this.MappingsList.Add(ChildOnlyPlanIDMap);

            QhpToExcelMap TobaccoWellnessProgramOfferedMap = new QhpToExcelMap();
            TobaccoWellnessProgramOfferedMap.FormPropertyName = "TobaccoWellnessProgramOffered";
            TobaccoWellnessProgramOfferedMap.DomainPropertyName = "TobaccoWellnessProgramOffered";
            TobaccoWellnessProgramOfferedMap.ColumnName = "V";
            TobaccoWellnessProgramOfferedMap.RowIndex = 9;
            TobaccoWellnessProgramOfferedMap.QhpSheetType = QHPSheetType.BenefitPackage;
            TobaccoWellnessProgramOfferedMap.CellFormat.CellAlignment = "Left";
            TobaccoWellnessProgramOfferedMap.CellFormat.DataType = "string";

            this.MappingsList.Add(TobaccoWellnessProgramOfferedMap);

            QhpToExcelMap DiseaseManagementProgramsOfferedMap = new QhpToExcelMap();
            DiseaseManagementProgramsOfferedMap.FormPropertyName = "DiseaseManagementProgramsOffered";
            DiseaseManagementProgramsOfferedMap.DomainPropertyName = "DiseaseManagementProgramsOffered";
            DiseaseManagementProgramsOfferedMap.ColumnName = "W";
            DiseaseManagementProgramsOfferedMap.RowIndex = 9;
            DiseaseManagementProgramsOfferedMap.QhpSheetType = QHPSheetType.BenefitPackage;
            DiseaseManagementProgramsOfferedMap.CellFormat.CellAlignment = "Left";
            DiseaseManagementProgramsOfferedMap.CellFormat.DataType = "string";

            this.MappingsList.Add(DiseaseManagementProgramsOfferedMap);

            QhpToExcelMap EHBPercentofTotalPremiumMap = new QhpToExcelMap();
            EHBPercentofTotalPremiumMap.FormPropertyName = "EHBPercentofTotalPremium";
            EHBPercentofTotalPremiumMap.DomainPropertyName = "EHBPercentofTotalPremium";
            EHBPercentofTotalPremiumMap.ColumnName = "X";
            EHBPercentofTotalPremiumMap.RowIndex = 9;
            EHBPercentofTotalPremiumMap.QhpSheetType = QHPSheetType.BenefitPackage;
            EHBPercentofTotalPremiumMap.CellFormat.CellAlignment = "Left";
            EHBPercentofTotalPremiumMap.CellFormat.DataType = "percentage";
            EHBPercentofTotalPremiumMap.CellFormat.CellPostFix = "%";

            this.MappingsList.Add(EHBPercentofTotalPremiumMap);
        }

        private void BuildStandAlonDentalOnlyAttributes()
        {
            QhpToExcelMap EHBApportionmentForPediatricDentalMap = new QhpToExcelMap();
            EHBApportionmentForPediatricDentalMap.FormPropertyName = "EHBApportionmentforPediatricDental";
            EHBApportionmentForPediatricDentalMap.DomainPropertyName = "EHBApportionmentForPediatricDental";
            EHBApportionmentForPediatricDentalMap.ColumnName = "Y";
            EHBApportionmentForPediatricDentalMap.RowIndex = 9;
            EHBApportionmentForPediatricDentalMap.QhpSheetType = QHPSheetType.BenefitPackage;
            EHBApportionmentForPediatricDentalMap.CellFormat.CellAlignment = "Left";
            EHBApportionmentForPediatricDentalMap.CellFormat.DataType = "string";

            this.MappingsList.Add(EHBApportionmentForPediatricDentalMap);

            QhpToExcelMap GuaranteedVsEstimatedRateMap = new QhpToExcelMap();
            GuaranteedVsEstimatedRateMap.FormPropertyName = "GuaranteedVsEstimatedRate";
            GuaranteedVsEstimatedRateMap.DomainPropertyName = "GuaranteedVsEstimatedRate";
            GuaranteedVsEstimatedRateMap.ColumnName = "Z";
            GuaranteedVsEstimatedRateMap.RowIndex = 9;
            GuaranteedVsEstimatedRateMap.QhpSheetType = QHPSheetType.BenefitPackage;
            GuaranteedVsEstimatedRateMap.CellFormat.CellAlignment = "Left";
            GuaranteedVsEstimatedRateMap.CellFormat.DataType = "string";

            this.MappingsList.Add(GuaranteedVsEstimatedRateMap);
        }

        private void BuildPlanDateAttributes()
        {
            QhpToExcelMap PlanEffectiveDateMap = new QhpToExcelMap();
            PlanEffectiveDateMap.FormPropertyName = "PlanEffectiveDate";
            PlanEffectiveDateMap.DomainPropertyName = "PlanEffectiveDate";
            PlanEffectiveDateMap.ColumnName = "AA";
            PlanEffectiveDateMap.RowIndex = 9;
            PlanEffectiveDateMap.QhpSheetType = QHPSheetType.BenefitPackage;
            PlanEffectiveDateMap.CellFormat.CellAlignment = "Center";
            PlanEffectiveDateMap.CellFormat.DataType = "date";

            this.MappingsList.Add(PlanEffectiveDateMap);

            QhpToExcelMap PlanExpirationDateMap = new QhpToExcelMap();
            PlanExpirationDateMap.FormPropertyName = "PlanExpirationDate";
            PlanExpirationDateMap.DomainPropertyName = "PlanExpirationDate";
            PlanExpirationDateMap.ColumnName = "AB";
            PlanExpirationDateMap.RowIndex = 9;
            PlanExpirationDateMap.QhpSheetType = QHPSheetType.BenefitPackage;
            PlanExpirationDateMap.CellFormat.CellAlignment = "Center";
            PlanExpirationDateMap.CellFormat.DataType = "date";

            this.MappingsList.Add(PlanExpirationDateMap);
        }

        private void BuildGeographicalCoverageAttributes()
        {
            QhpToExcelMap OutOfCountryCoverageMap = new QhpToExcelMap();
            OutOfCountryCoverageMap.FormPropertyName = "OutofCountryCoverage";
            OutOfCountryCoverageMap.DomainPropertyName = "OutOfCountryCoverage";
            OutOfCountryCoverageMap.ColumnName = "AC";
            OutOfCountryCoverageMap.RowIndex = 9;
            OutOfCountryCoverageMap.QhpSheetType = QHPSheetType.BenefitPackage;
            OutOfCountryCoverageMap.CellFormat.CellAlignment = "Left";
            OutOfCountryCoverageMap.CellFormat.DataType = "string";

            this.MappingsList.Add(OutOfCountryCoverageMap);

            QhpToExcelMap OutOfCountryCoverageDescriptionMap = new QhpToExcelMap();
            OutOfCountryCoverageDescriptionMap.FormPropertyName = "OutofCountryCoverageDescription";
            OutOfCountryCoverageDescriptionMap.DomainPropertyName = "OutOfCountryCoverageDescription";
            OutOfCountryCoverageDescriptionMap.ColumnName = "AD";
            OutOfCountryCoverageDescriptionMap.RowIndex = 9;
            OutOfCountryCoverageDescriptionMap.QhpSheetType = QHPSheetType.BenefitPackage;
            OutOfCountryCoverageDescriptionMap.CellFormat.CellAlignment = "Left";
            OutOfCountryCoverageDescriptionMap.CellFormat.DataType = "string";

            this.MappingsList.Add(OutOfCountryCoverageDescriptionMap);

            QhpToExcelMap OutOfServiceAreaCoverageMap = new QhpToExcelMap();
            OutOfServiceAreaCoverageMap.FormPropertyName = "OutofServiceAreaCoverage";
            OutOfServiceAreaCoverageMap.DomainPropertyName = "OutOfServiceAreaCoverage";
            OutOfServiceAreaCoverageMap.ColumnName = "AE";
            OutOfServiceAreaCoverageMap.RowIndex = 9;
            OutOfServiceAreaCoverageMap.QhpSheetType = QHPSheetType.BenefitPackage;
            OutOfServiceAreaCoverageMap.CellFormat.CellAlignment = "Left";
            OutOfServiceAreaCoverageMap.CellFormat.DataType = "string";

            this.MappingsList.Add(OutOfServiceAreaCoverageMap);

            QhpToExcelMap OutOfServiceAreaCoverageDescriptionMap = new QhpToExcelMap();
            OutOfServiceAreaCoverageDescriptionMap.FormPropertyName = "OutofServiceAreaCoverageDescription";
            OutOfServiceAreaCoverageDescriptionMap.DomainPropertyName = "OutOfServiceAreaCoverageDescription";
            OutOfServiceAreaCoverageDescriptionMap.ColumnName = "AF";
            OutOfServiceAreaCoverageDescriptionMap.RowIndex = 9;
            OutOfServiceAreaCoverageDescriptionMap.QhpSheetType = QHPSheetType.BenefitPackage;
            OutOfServiceAreaCoverageDescriptionMap.CellFormat.CellAlignment = "Left";
            OutOfServiceAreaCoverageDescriptionMap.CellFormat.DataType = "string";

            this.MappingsList.Add(OutOfServiceAreaCoverageDescriptionMap);

            QhpToExcelMap NationalNetworkMap = new QhpToExcelMap();
            NationalNetworkMap.FormPropertyName = "NationalNetwork";
            NationalNetworkMap.DomainPropertyName = "NationalNetwork";
            NationalNetworkMap.ColumnName = "AG";
            NationalNetworkMap.RowIndex = 9;
            NationalNetworkMap.QhpSheetType = QHPSheetType.BenefitPackage;
            NationalNetworkMap.CellFormat.CellAlignment = "Left";
            NationalNetworkMap.CellFormat.DataType = "string";

            this.MappingsList.Add(NationalNetworkMap);
        }

        private void BuildPlanLevelURLs()
        {
            QhpToExcelMap URLforEnrollmentPaymentMap = new QhpToExcelMap();
            URLforEnrollmentPaymentMap.FormPropertyName = "URLforEnrollmentPayment";
            URLforEnrollmentPaymentMap.DomainPropertyName = "URLForEnrollmentPayment";
            URLforEnrollmentPaymentMap.ColumnName = "AH";
            URLforEnrollmentPaymentMap.RowIndex = 9;
            URLforEnrollmentPaymentMap.QhpSheetType = QHPSheetType.BenefitPackage;
            URLforEnrollmentPaymentMap.CellFormat.CellAlignment = "Left";
            URLforEnrollmentPaymentMap.CellFormat.DataType = "string";

            this.MappingsList.Add(URLforEnrollmentPaymentMap);
        }

        private void BuildBenefitInformation()
        {
            QhpToExcelMap BenefitsMap = new QhpToExcelMap();
            BenefitsMap.FormPropertyName = "Benefits";
            BenefitsMap.DomainPropertyName = "Benefit";
            BenefitsMap.ColumnName = "A";
            BenefitsMap.RowIndex = 61;
            BenefitsMap.QhpSheetType = QHPSheetType.BenefitPackage;
            BenefitsMap.CellFormat.CellAlignment = "Center";
            BenefitsMap.CellFormat.DataType = "string";

            this.MappingsList.Add(BenefitsMap);

            QhpToExcelMap EHBMap = new QhpToExcelMap();
            EHBMap.FormPropertyName = "EHB";
            EHBMap.DomainPropertyName = "EHB";
            EHBMap.ColumnName = "C";
            EHBMap.RowIndex = 61;
            EHBMap.QhpSheetType = QHPSheetType.BenefitPackage;
            EHBMap.CellFormat.CellAlignment = "Left";
            EHBMap.CellFormat.DataType = "string";

            this.MappingsList.Add(EHBMap);

            //QhpToExcelMap StateRequiredBenefitMap = new QhpToExcelMap();
            //StateRequiredBenefitMap.FormPropertyName = "StateRequiredBenefit";
            //StateRequiredBenefitMap.DomainPropertyName = "StateRequiredBenefit";
            //StateRequiredBenefitMap.ColumnName = "D";
            //StateRequiredBenefitMap.RowIndex = 61;
            //StateRequiredBenefitMap.QhpSheetType = QHPSheetType.BenefitPackage;
            //StateRequiredBenefitMap.CellFormat.CellAlignment = "Left";
            //StateRequiredBenefitMap.CellFormat.DataType = "string";

            //this.MappingsList.Add(StateRequiredBenefitMap);
        }

        private void BuildBenefitGeneralInformation()
        {
            QhpToExcelMap IsThisBenefitCoveredMap = new QhpToExcelMap();
            IsThisBenefitCoveredMap.FormPropertyName = "IsCovered";
            IsThisBenefitCoveredMap.DomainPropertyName = "IsThisBenefitCovered";
            IsThisBenefitCoveredMap.ColumnName = "D";
            IsThisBenefitCoveredMap.RowIndex = 61;
            IsThisBenefitCoveredMap.QhpSheetType = QHPSheetType.BenefitPackage;
            IsThisBenefitCoveredMap.CellFormat.CellAlignment = "Left";
            IsThisBenefitCoveredMap.CellFormat.DataType = "string";

            this.MappingsList.Add(IsThisBenefitCoveredMap);

            QhpToExcelMap QuantativeLimitOfServiceMap = new QhpToExcelMap();
            QuantativeLimitOfServiceMap.FormPropertyName = "QuantitativeLimitonService";
            QuantativeLimitOfServiceMap.DomainPropertyName = "QuantativeLimitOfService";
            QuantativeLimitOfServiceMap.ColumnName = "E";
            QuantativeLimitOfServiceMap.RowIndex = 61;
            QuantativeLimitOfServiceMap.QhpSheetType = QHPSheetType.BenefitPackage;
            QuantativeLimitOfServiceMap.CellFormat.CellAlignment = "Left";
            QuantativeLimitOfServiceMap.CellFormat.DataType = "string";

            this.MappingsList.Add(QuantativeLimitOfServiceMap);

            QhpToExcelMap LimitQuantityMap = new QhpToExcelMap();
            LimitQuantityMap.FormPropertyName = "LimitQuantity";
            LimitQuantityMap.DomainPropertyName = "LimitQuantity";
            LimitQuantityMap.ColumnName = "F";
            LimitQuantityMap.RowIndex = 61;
            LimitQuantityMap.QhpSheetType = QHPSheetType.BenefitPackage;
            LimitQuantityMap.CellFormat.CellAlignment = "Right";
            LimitQuantityMap.CellFormat.DataType = "int";

            this.MappingsList.Add(LimitQuantityMap);

            QhpToExcelMap LimitUnitMap = new QhpToExcelMap();
            LimitUnitMap.FormPropertyName = "LimitUnit";
            LimitUnitMap.DomainPropertyName = "LimitUnit";
            LimitUnitMap.ColumnName = "G";
            LimitUnitMap.RowIndex = 61;
            LimitUnitMap.QhpSheetType = QHPSheetType.BenefitPackage;
            LimitUnitMap.CellFormat.CellAlignment = "Left";
            LimitUnitMap.CellFormat.DataType = "string";

            this.MappingsList.Add(LimitUnitMap);

            //QhpToExcelMap MinimumStayMap = new QhpToExcelMap();
            //MinimumStayMap.FormPropertyName = "MinimumStay";
            //MinimumStayMap.DomainPropertyName = "MinimumStay";
            //MinimumStayMap.ColumnName = "I";
            //MinimumStayMap.RowIndex = 61;
            //MinimumStayMap.QhpSheetType = QHPSheetType.BenefitPackage;
            //MinimumStayMap.CellFormat.CellAlignment = "Left";
            //MinimumStayMap.CellFormat.DataType = "string";

            //this.MappingsList.Add(MinimumStayMap);

            QhpToExcelMap ExclusionsMap = new QhpToExcelMap();
            ExclusionsMap.FormPropertyName = "Exclusions";
            ExclusionsMap.DomainPropertyName = "Exclusions";
            ExclusionsMap.ColumnName = "H";
            ExclusionsMap.RowIndex = 61;
            ExclusionsMap.QhpSheetType = QHPSheetType.BenefitPackage;
            ExclusionsMap.CellFormat.CellAlignment = "Left";
            ExclusionsMap.CellFormat.DataType = "string";

            this.MappingsList.Add(ExclusionsMap);

            QhpToExcelMap BenefitExplainationMap = new QhpToExcelMap();
            BenefitExplainationMap.FormPropertyName = "BenefitExplaination";
            BenefitExplainationMap.DomainPropertyName = "BenefitExplanation";
            BenefitExplainationMap.ColumnName = "I";
            BenefitExplainationMap.RowIndex = 61;
            BenefitExplainationMap.QhpSheetType = QHPSheetType.BenefitPackage;
            BenefitExplainationMap.CellFormat.CellAlignment = "Left";
            BenefitExplainationMap.CellFormat.DataType = "string";

            this.MappingsList.Add(BenefitExplainationMap);

            QhpToExcelMap EHBVarianceReasonMap = new QhpToExcelMap();
            EHBVarianceReasonMap.FormPropertyName = "EHBVarianceReason";
            EHBVarianceReasonMap.DomainPropertyName = "EHBVarianceReason";
            EHBVarianceReasonMap.ColumnName = "J";
            EHBVarianceReasonMap.RowIndex = 61;
            EHBVarianceReasonMap.QhpSheetType = QHPSheetType.BenefitPackage;
            EHBVarianceReasonMap.CellFormat.CellAlignment = "Left";
            EHBVarianceReasonMap.CellFormat.DataType = "string";

            this.MappingsList.Add(EHBVarianceReasonMap);
        }

        private void BuildOutofPocketExceptions()
        {
            QhpToExcelMap ExcludedFromInNetworkMOOPMap = new QhpToExcelMap();
            ExcludedFromInNetworkMOOPMap.FormPropertyName = "ExcludedfromInNetworkMOOP";
            ExcludedFromInNetworkMOOPMap.DomainPropertyName = "ExcludedFromInNetworkMOOP";
            ExcludedFromInNetworkMOOPMap.ColumnName = "K";
            ExcludedFromInNetworkMOOPMap.RowIndex = 61;
            ExcludedFromInNetworkMOOPMap.QhpSheetType = QHPSheetType.BenefitPackage;
            ExcludedFromInNetworkMOOPMap.CellFormat.CellAlignment = "Left";
            ExcludedFromInNetworkMOOPMap.CellFormat.DataType = "string";

            this.MappingsList.Add(ExcludedFromInNetworkMOOPMap);

            QhpToExcelMap ExcludedFromOutOfNetworkMOOPMap = new QhpToExcelMap();
            ExcludedFromOutOfNetworkMOOPMap.FormPropertyName = "ExcludedfromOutofNetworkMOOP";
            ExcludedFromOutOfNetworkMOOPMap.DomainPropertyName = "ExcludedFromOutOfNetworkMOOP";
            ExcludedFromOutOfNetworkMOOPMap.ColumnName = "L";
            ExcludedFromOutOfNetworkMOOPMap.RowIndex = 61;
            ExcludedFromOutOfNetworkMOOPMap.QhpSheetType = QHPSheetType.BenefitPackage;
            ExcludedFromOutOfNetworkMOOPMap.CellFormat.CellAlignment = "Left";
            ExcludedFromOutOfNetworkMOOPMap.CellFormat.DataType = "string";

            this.MappingsList.Add(ExcludedFromOutOfNetworkMOOPMap);
        }

        private void BuildPlanCostSharingAttributes()
        {
            QhpToExcelMap HIOSPlanIDVariantMap = new QhpToExcelMap();
            HIOSPlanIDVariantMap.FormPropertyName = "HIOSPlanIDComponentAndVariant";
            HIOSPlanIDVariantMap.DomainPropertyName = "HIOSPlanIDComponentAndVariant";
            HIOSPlanIDVariantMap.ColumnName = "A";
            HIOSPlanIDVariantMap.RowIndex = 4;
            HIOSPlanIDVariantMap.QhpSheetType = QHPSheetType.CostShareVariance;
            HIOSPlanIDVariantMap.CellFormat.CellAlignment = "Left";
            HIOSPlanIDVariantMap.CellFormat.DataType = "string";

            this.MappingsList.Add(HIOSPlanIDVariantMap);

            QhpToExcelMap PlanMarketingNameMap = new QhpToExcelMap();
            PlanMarketingNameMap.FormPropertyName = "PlanMarketingName";
            PlanMarketingNameMap.DomainPropertyName = "PlanMarketingName";
            PlanMarketingNameMap.ColumnName = "B";
            PlanMarketingNameMap.RowIndex = 4;
            PlanMarketingNameMap.QhpSheetType = QHPSheetType.CostShareVariance;
            PlanMarketingNameMap.CellFormat.CellAlignment = "Left";
            PlanMarketingNameMap.CellFormat.DataType = "string";

            this.MappingsList.Add(PlanMarketingNameMap);

            QhpToExcelMap LevelOfCoverageMap = new QhpToExcelMap();
            LevelOfCoverageMap.FormPropertyName = "LevelOfCoverage";
            LevelOfCoverageMap.DomainPropertyName = "LevelOfCoverage";
            LevelOfCoverageMap.ColumnName = "C";
            LevelOfCoverageMap.RowIndex = 4;
            LevelOfCoverageMap.QhpSheetType = QHPSheetType.CostShareVariance;
            LevelOfCoverageMap.CellFormat.CellAlignment = "Left";
            LevelOfCoverageMap.CellFormat.DataType = "string";

            this.MappingsList.Add(LevelOfCoverageMap);

            QhpToExcelMap CSRVariationTypeMap = new QhpToExcelMap();
            CSRVariationTypeMap.FormPropertyName = "CSRVariationType";
            CSRVariationTypeMap.DomainPropertyName = "CSRVariationType";
            CSRVariationTypeMap.ColumnName = "D";
            CSRVariationTypeMap.RowIndex = 4;
            CSRVariationTypeMap.QhpSheetType = QHPSheetType.CostShareVariance;
            CSRVariationTypeMap.CellFormat.CellAlignment = "Left";
            CSRVariationTypeMap.CellFormat.DataType = "string";

            this.MappingsList.Add(CSRVariationTypeMap);

            QhpToExcelMap IssuerActuarialValueMap = new QhpToExcelMap();
            IssuerActuarialValueMap.FormPropertyName = "IssuerActuarialValue";
            IssuerActuarialValueMap.DomainPropertyName = "IssuerActuarialValue";
            IssuerActuarialValueMap.ColumnName = "E";
            IssuerActuarialValueMap.RowIndex = 4;
            IssuerActuarialValueMap.QhpSheetType = QHPSheetType.CostShareVariance;
            IssuerActuarialValueMap.CellFormat.CellAlignment = "Right";
            IssuerActuarialValueMap.CellFormat.DataType = "decimal";
            IssuerActuarialValueMap.CellFormat.CellPostFix = "%";

            this.MappingsList.Add(IssuerActuarialValueMap);

            QhpToExcelMap AVCalculatorOutputNumberMap = new QhpToExcelMap();
            AVCalculatorOutputNumberMap.FormPropertyName = "AvCalculatorOutputNumber";
            AVCalculatorOutputNumberMap.DomainPropertyName = "AVCalculatorOutputNumber";
            AVCalculatorOutputNumberMap.ColumnName = "F";
            AVCalculatorOutputNumberMap.RowIndex = 4;
            AVCalculatorOutputNumberMap.QhpSheetType = QHPSheetType.CostShareVariance;
            AVCalculatorOutputNumberMap.CellFormat.CellAlignment = "Right";
            AVCalculatorOutputNumberMap.CellFormat.DataType = "decimal";
            AVCalculatorOutputNumberMap.CellFormat.CellPostFix = "%";

            this.MappingsList.Add(AVCalculatorOutputNumberMap);

            QhpToExcelMap MedicalAndDrugDeductiblesIntegratedMap = new QhpToExcelMap();
            MedicalAndDrugDeductiblesIntegratedMap.FormPropertyName = "MedicalAndDrugDeductiblesIntegrated";
            MedicalAndDrugDeductiblesIntegratedMap.DomainPropertyName = "MedicalAndDrugDeductiblesIntegrated";
            MedicalAndDrugDeductiblesIntegratedMap.ColumnName = "G";
            MedicalAndDrugDeductiblesIntegratedMap.RowIndex = 4;
            MedicalAndDrugDeductiblesIntegratedMap.QhpSheetType = QHPSheetType.CostShareVariance;
            MedicalAndDrugDeductiblesIntegratedMap.CellFormat.CellAlignment = "Left";
            MedicalAndDrugDeductiblesIntegratedMap.CellFormat.DataType = "string";

            this.MappingsList.Add(MedicalAndDrugDeductiblesIntegratedMap);

            QhpToExcelMap MedicalAndDrugOutOfPocketIntegratedMap = new QhpToExcelMap();
            MedicalAndDrugOutOfPocketIntegratedMap.FormPropertyName = "MedicalAndDrugOutOfPocketIntegrated";
            MedicalAndDrugOutOfPocketIntegratedMap.DomainPropertyName = "MedicalAndDrugOutOfPocketIntegrated";
            MedicalAndDrugOutOfPocketIntegratedMap.ColumnName = "H";
            MedicalAndDrugOutOfPocketIntegratedMap.RowIndex = 4;
            MedicalAndDrugOutOfPocketIntegratedMap.QhpSheetType = QHPSheetType.CostShareVariance;
            MedicalAndDrugOutOfPocketIntegratedMap.CellFormat.CellAlignment = "Left";
            MedicalAndDrugOutOfPocketIntegratedMap.CellFormat.DataType = "string";

            this.MappingsList.Add(MedicalAndDrugOutOfPocketIntegratedMap);

            QhpToExcelMap MultipleInNetworkTiersMap = new QhpToExcelMap();
            MultipleInNetworkTiersMap.FormPropertyName = "MultipleInNetworkTiers";
            MultipleInNetworkTiersMap.DomainPropertyName = "MultipleInNetworkTiers";
            MultipleInNetworkTiersMap.ColumnName = "I";
            MultipleInNetworkTiersMap.RowIndex = 4;
            MultipleInNetworkTiersMap.QhpSheetType = QHPSheetType.CostShareVariance;
            MultipleInNetworkTiersMap.CellFormat.CellAlignment = "Left";
            MultipleInNetworkTiersMap.CellFormat.DataType = "string";

            this.MappingsList.Add(MultipleInNetworkTiersMap);

            QhpToExcelMap FirstTierUtilizationMap = new QhpToExcelMap();
            FirstTierUtilizationMap.FormPropertyName = "FirstTierUtilization";
            FirstTierUtilizationMap.DomainPropertyName = "FirstTierUtilization";
            FirstTierUtilizationMap.ColumnName = "J";
            FirstTierUtilizationMap.RowIndex = 4;
            FirstTierUtilizationMap.QhpSheetType = QHPSheetType.CostShareVariance;
            FirstTierUtilizationMap.CellFormat.CellAlignment = "Right";
            FirstTierUtilizationMap.CellFormat.DataType = "int";
            FirstTierUtilizationMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(FirstTierUtilizationMap);

            QhpToExcelMap SecondTierUtilizationMap = new QhpToExcelMap();
            SecondTierUtilizationMap.FormPropertyName = "SecondTierUtilization";
            SecondTierUtilizationMap.DomainPropertyName = "SecondTierUtilization";
            SecondTierUtilizationMap.ColumnName = "K";
            SecondTierUtilizationMap.RowIndex = 4;
            SecondTierUtilizationMap.QhpSheetType = QHPSheetType.CostShareVariance;
            SecondTierUtilizationMap.CellFormat.CellAlignment = "Right";
            SecondTierUtilizationMap.CellFormat.DataType = "int";
            SecondTierUtilizationMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(SecondTierUtilizationMap);
        }

        private void BuildSBCScenarioAttributes()
        {
            QhpToExcelMap Deductible1Map = new QhpToExcelMap();
            Deductible1Map.ParentPropertyName = "HavingABaby";
            Deductible1Map.FormPropertyName = "Deductible";
            Deductible1Map.DomainPropertyName = "Deductible";
            Deductible1Map.ColumnName = "L";
            Deductible1Map.RowIndex = 4;
            Deductible1Map.QhpSheetType = QHPSheetType.CostShareVariance;
            Deductible1Map.CellFormat.CellAlignment = "Right";
            Deductible1Map.CellFormat.DataType = "dollar";
            Deductible1Map.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(Deductible1Map);

            QhpToExcelMap Coinsurance1Map = new QhpToExcelMap();
            Coinsurance1Map.ParentPropertyName = "HavingABaby";
            Coinsurance1Map.FormPropertyName = "Coinsurance";
            Coinsurance1Map.DomainPropertyName = "Coinsurance";
            Coinsurance1Map.ColumnName = "N";
            Coinsurance1Map.RowIndex = 4;
            Coinsurance1Map.QhpSheetType = QHPSheetType.CostShareVariance;
            Coinsurance1Map.CellFormat.CellAlignment = "Right";
            Coinsurance1Map.CellFormat.DataType = "dollar";
            Coinsurance1Map.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(Coinsurance1Map);

            QhpToExcelMap Copayment1Map = new QhpToExcelMap();
            Copayment1Map.ParentPropertyName = "HavingABaby";
            Copayment1Map.FormPropertyName = "Copayment";
            Copayment1Map.DomainPropertyName = "Copayment";
            Copayment1Map.ColumnName = "M";
            Copayment1Map.RowIndex = 4;
            Copayment1Map.QhpSheetType = QHPSheetType.CostShareVariance;
            Copayment1Map.CellFormat.CellAlignment = "Right";
            Copayment1Map.CellFormat.DataType = "dollar";
            Copayment1Map.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(Copayment1Map);

            QhpToExcelMap Limits1Map = new QhpToExcelMap();
            Limits1Map.ParentPropertyName = "HavingABaby";
            Limits1Map.FormPropertyName = "Limit";
            Limits1Map.DomainPropertyName = "Limit";
            Limits1Map.ColumnName = "O";
            Limits1Map.RowIndex = 4;
            Limits1Map.QhpSheetType = QHPSheetType.CostShareVariance;
            Limits1Map.CellFormat.CellAlignment = "Right";
            Limits1Map.CellFormat.DataType = "dollar";
            Limits1Map.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(Limits1Map);

            QhpToExcelMap Deductible2Map = new QhpToExcelMap();
            Deductible2Map.ParentPropertyName = "HavingDiabetes";
            Deductible2Map.FormPropertyName = "Deductible";
            Deductible2Map.DomainPropertyName = "Deductible";
            Deductible2Map.ColumnName = "P";
            Deductible2Map.RowIndex = 4;
            Deductible2Map.QhpSheetType = QHPSheetType.CostShareVariance;
            Deductible2Map.CellFormat.CellAlignment = "Right";
            Deductible2Map.CellFormat.DataType = "dollar";
            Deductible2Map.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(Deductible2Map);


            QhpToExcelMap Coinsurance2Map = new QhpToExcelMap();
            Coinsurance2Map.ParentPropertyName = "HavingDiabetes";
            Coinsurance2Map.FormPropertyName = "Coinsurance";
            Coinsurance2Map.DomainPropertyName = "Coinsurance";
            Coinsurance2Map.ColumnName = "R";
            Coinsurance2Map.RowIndex = 4;
            Coinsurance2Map.QhpSheetType = QHPSheetType.CostShareVariance;
            Coinsurance2Map.CellFormat.CellAlignment = "Right";
            Coinsurance2Map.CellFormat.DataType = "dollar";
            Coinsurance2Map.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(Coinsurance2Map);

            QhpToExcelMap Copayment2Map = new QhpToExcelMap();
            Copayment2Map.ParentPropertyName = "HavingDiabetes";
            Copayment2Map.FormPropertyName = "Copayment";
            Copayment2Map.DomainPropertyName = "Copayment";
            Copayment2Map.ColumnName = "Q";
            Copayment2Map.RowIndex = 4;
            Copayment2Map.QhpSheetType = QHPSheetType.CostShareVariance;
            Copayment2Map.CellFormat.CellAlignment = "Right";
            Copayment2Map.CellFormat.DataType = "dollar";
            Copayment2Map.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(Copayment2Map);

            QhpToExcelMap Limits2Map = new QhpToExcelMap();
            Limits2Map.ParentPropertyName = "HavingDiabetes";
            Limits2Map.FormPropertyName = "Limit";
            Limits2Map.DomainPropertyName = "Limit";
            Limits2Map.ColumnName = "S";
            Limits2Map.RowIndex = 4;
            Limits2Map.QhpSheetType = QHPSheetType.CostShareVariance;
            Limits2Map.CellFormat.CellAlignment = "Right";
            Limits2Map.CellFormat.DataType = "dollar";
            Limits2Map.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(Limits2Map);


            QhpToExcelMap Deductible3Map = new QhpToExcelMap();
            Deductible3Map.ParentPropertyName = "TreatmentOfSimpleFracture";
            Deductible3Map.FormPropertyName = "Deductible";
            Deductible3Map.DomainPropertyName = "Deductible";
            Deductible3Map.ColumnName = "T";
            Deductible3Map.RowIndex = 4;
            Deductible3Map.QhpSheetType = QHPSheetType.CostShareVariance;
            Deductible3Map.CellFormat.CellAlignment = "Right";
            Deductible3Map.CellFormat.DataType = "dollar";
            Deductible3Map.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(Deductible3Map);

            QhpToExcelMap Coinsurance3Map = new QhpToExcelMap();
            Coinsurance3Map.ParentPropertyName = "TreatmentOfSimpleFracture";
            Coinsurance3Map.FormPropertyName = "Coinsurance";
            Coinsurance3Map.DomainPropertyName = "Coinsurance";
            Coinsurance3Map.ColumnName = "V";
            Coinsurance3Map.RowIndex = 4;
            Coinsurance3Map.QhpSheetType = QHPSheetType.CostShareVariance;
            Coinsurance3Map.CellFormat.CellAlignment = "Right";
            Coinsurance3Map.CellFormat.DataType = "dollar";
            Coinsurance3Map.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(Coinsurance3Map);

            QhpToExcelMap Copayment3Map = new QhpToExcelMap();
            Copayment3Map.ParentPropertyName = "TreatmentOfSimpleFracture";
            Copayment3Map.FormPropertyName = "Copayment";
            Copayment3Map.DomainPropertyName = "Copayment";
            Copayment3Map.ColumnName = "U";
            Copayment3Map.RowIndex = 4;
            Copayment3Map.QhpSheetType = QHPSheetType.CostShareVariance;
            Copayment3Map.CellFormat.CellAlignment = "Right";
            Copayment3Map.CellFormat.DataType = "dollar";
            Copayment3Map.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(Copayment3Map);

            QhpToExcelMap Limits3Map = new QhpToExcelMap();
            Limits3Map.ParentPropertyName = "TreatmentOfSimpleFracture";
            Limits3Map.FormPropertyName = "Limit";
            Limits3Map.DomainPropertyName = "Limit";
            Limits3Map.ColumnName = "W";
            Limits3Map.RowIndex = 4;
            Limits3Map.QhpSheetType = QHPSheetType.CostShareVariance;
            Limits3Map.CellFormat.CellAlignment = "Right";
            Limits3Map.CellFormat.DataType = "dollar";
            Limits3Map.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(Limits3Map);
        }

        private void BuildMaximumOutOfPocketMaxAttributes()
        {   
            QhpToExcelMap MaximumOutOfPocketForMedicalEHBBenefitsInNetworkIndividualMap = new QhpToExcelMap();
            MaximumOutOfPocketForMedicalEHBBenefitsInNetworkIndividualMap.ParentPropertyName = "MaximumOutOfPocketForMedicalEHBBenefits";
            MaximumOutOfPocketForMedicalEHBBenefitsInNetworkIndividualMap.FormPropertyName = "InNetworkIndividual";
            MaximumOutOfPocketForMedicalEHBBenefitsInNetworkIndividualMap.DomainPropertyName = "InNetworkIndividual";
            MaximumOutOfPocketForMedicalEHBBenefitsInNetworkIndividualMap.ColumnName = "X";
            MaximumOutOfPocketForMedicalEHBBenefitsInNetworkIndividualMap.RowIndex = 4;
            MaximumOutOfPocketForMedicalEHBBenefitsInNetworkIndividualMap.QhpSheetType = QHPSheetType.CostShareVariance;
            MaximumOutOfPocketForMedicalEHBBenefitsInNetworkIndividualMap.CellFormat.CellAlignment = "Right";
            MaximumOutOfPocketForMedicalEHBBenefitsInNetworkIndividualMap.CellFormat.DataType = "int";
            MaximumOutOfPocketForMedicalEHBBenefitsInNetworkIndividualMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(MaximumOutOfPocketForMedicalEHBBenefitsInNetworkIndividualMap);

            QhpToExcelMap MaximumOutOfPocketForMedicalEHBBenefitsInNetworkFamilyMap = new QhpToExcelMap();
            MaximumOutOfPocketForMedicalEHBBenefitsInNetworkFamilyMap.ParentPropertyName = "MaximumOutOfPocketForMedicalEHBBenefits";
            MaximumOutOfPocketForMedicalEHBBenefitsInNetworkFamilyMap.FormPropertyName = "InNetworkFamily";
            MaximumOutOfPocketForMedicalEHBBenefitsInNetworkFamilyMap.DomainPropertyName = "InNetworkFamily";
            MaximumOutOfPocketForMedicalEHBBenefitsInNetworkFamilyMap.ColumnName = "Y";
            MaximumOutOfPocketForMedicalEHBBenefitsInNetworkFamilyMap.RowIndex = 4;
            MaximumOutOfPocketForMedicalEHBBenefitsInNetworkFamilyMap.QhpSheetType = QHPSheetType.CostShareVariance;
            MaximumOutOfPocketForMedicalEHBBenefitsInNetworkFamilyMap.CellFormat.CellAlignment = "Right";
            MaximumOutOfPocketForMedicalEHBBenefitsInNetworkFamilyMap.CellFormat.DataType = "int";
            MaximumOutOfPocketForMedicalEHBBenefitsInNetworkFamilyMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(MaximumOutOfPocketForMedicalEHBBenefitsInNetworkFamilyMap);

            QhpToExcelMap MaximumOutOfPocketForMedicalEHBBenefitsInNetworkTier2IndividualMap = new QhpToExcelMap();
            MaximumOutOfPocketForMedicalEHBBenefitsInNetworkTier2IndividualMap.ParentPropertyName = "MaximumOutOfPocketForMedicalEHBBenefits";
            MaximumOutOfPocketForMedicalEHBBenefitsInNetworkTier2IndividualMap.FormPropertyName = "InNetworkTier2Individual";
            MaximumOutOfPocketForMedicalEHBBenefitsInNetworkTier2IndividualMap.DomainPropertyName = "InNetworkTier2Individual";
            MaximumOutOfPocketForMedicalEHBBenefitsInNetworkTier2IndividualMap.ColumnName = "Z";
            MaximumOutOfPocketForMedicalEHBBenefitsInNetworkTier2IndividualMap.RowIndex = 4;
            MaximumOutOfPocketForMedicalEHBBenefitsInNetworkTier2IndividualMap.QhpSheetType = QHPSheetType.CostShareVariance;
            MaximumOutOfPocketForMedicalEHBBenefitsInNetworkTier2IndividualMap.CellFormat.CellAlignment = "Right";
            MaximumOutOfPocketForMedicalEHBBenefitsInNetworkTier2IndividualMap.CellFormat.DataType = "int";
            MaximumOutOfPocketForMedicalEHBBenefitsInNetworkTier2IndividualMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(MaximumOutOfPocketForMedicalEHBBenefitsInNetworkTier2IndividualMap);

            QhpToExcelMap MaximumOutOfPocketForMedicalEHBBenefitsInNetworkTier2FamilyMap = new QhpToExcelMap();
            MaximumOutOfPocketForMedicalEHBBenefitsInNetworkTier2FamilyMap.ParentPropertyName = "MaximumOutOfPocketForMedicalEHBBenefits";
            MaximumOutOfPocketForMedicalEHBBenefitsInNetworkTier2FamilyMap.FormPropertyName = "InNetworkTier2Family";
            MaximumOutOfPocketForMedicalEHBBenefitsInNetworkTier2FamilyMap.DomainPropertyName = "InNetworkTier2Family";
            MaximumOutOfPocketForMedicalEHBBenefitsInNetworkTier2FamilyMap.ColumnName = "AA";
            MaximumOutOfPocketForMedicalEHBBenefitsInNetworkTier2FamilyMap.RowIndex = 4;
            MaximumOutOfPocketForMedicalEHBBenefitsInNetworkTier2FamilyMap.QhpSheetType = QHPSheetType.CostShareVariance;
            MaximumOutOfPocketForMedicalEHBBenefitsInNetworkTier2FamilyMap.CellFormat.CellAlignment = "Right";
            MaximumOutOfPocketForMedicalEHBBenefitsInNetworkTier2FamilyMap.CellFormat.DataType = "int";
            MaximumOutOfPocketForMedicalEHBBenefitsInNetworkTier2FamilyMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(MaximumOutOfPocketForMedicalEHBBenefitsInNetworkTier2FamilyMap);

            QhpToExcelMap MaximumOutOfPocketForMedicalEHBBenefitsOutOfNetworkIndividualMap = new QhpToExcelMap();
            MaximumOutOfPocketForMedicalEHBBenefitsOutOfNetworkIndividualMap.ParentPropertyName = "MaximumOutOfPocketForMedicalEHBBenefits";
            MaximumOutOfPocketForMedicalEHBBenefitsOutOfNetworkIndividualMap.FormPropertyName = "OutOfNetworkIndividual";
            MaximumOutOfPocketForMedicalEHBBenefitsOutOfNetworkIndividualMap.DomainPropertyName = "OutOfNetworkIndividual";
            MaximumOutOfPocketForMedicalEHBBenefitsOutOfNetworkIndividualMap.ColumnName = "AB";
            MaximumOutOfPocketForMedicalEHBBenefitsOutOfNetworkIndividualMap.RowIndex = 4;
            MaximumOutOfPocketForMedicalEHBBenefitsOutOfNetworkIndividualMap.QhpSheetType = QHPSheetType.CostShareVariance;
            MaximumOutOfPocketForMedicalEHBBenefitsOutOfNetworkIndividualMap.CellFormat.CellAlignment = "Right";
            MaximumOutOfPocketForMedicalEHBBenefitsOutOfNetworkIndividualMap.CellFormat.DataType = "int";
            MaximumOutOfPocketForMedicalEHBBenefitsOutOfNetworkIndividualMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(MaximumOutOfPocketForMedicalEHBBenefitsOutOfNetworkIndividualMap);

            QhpToExcelMap MaximumOutOfPocketForMedicalEHBBenefitsOutOfNetworkFamilyMap = new QhpToExcelMap();
            MaximumOutOfPocketForMedicalEHBBenefitsOutOfNetworkFamilyMap.ParentPropertyName = "MaximumOutOfPocketForMedicalEHBBenefits";
            MaximumOutOfPocketForMedicalEHBBenefitsOutOfNetworkFamilyMap.FormPropertyName = "OutOfNetworkFamily";
            MaximumOutOfPocketForMedicalEHBBenefitsOutOfNetworkFamilyMap.DomainPropertyName = "OutOfNetworkFamily";
            MaximumOutOfPocketForMedicalEHBBenefitsOutOfNetworkFamilyMap.ColumnName = "AC";
            MaximumOutOfPocketForMedicalEHBBenefitsOutOfNetworkFamilyMap.RowIndex = 4;
            MaximumOutOfPocketForMedicalEHBBenefitsOutOfNetworkFamilyMap.QhpSheetType = QHPSheetType.CostShareVariance;
            MaximumOutOfPocketForMedicalEHBBenefitsOutOfNetworkFamilyMap.CellFormat.CellAlignment = "Right";
            MaximumOutOfPocketForMedicalEHBBenefitsOutOfNetworkFamilyMap.CellFormat.DataType = "int";
            MaximumOutOfPocketForMedicalEHBBenefitsOutOfNetworkFamilyMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(MaximumOutOfPocketForMedicalEHBBenefitsOutOfNetworkFamilyMap);

            QhpToExcelMap MaximumOutOfPocketForMedicalEHBBenefitsCombinedInOutNetworkIndividualMap = new QhpToExcelMap();
            MaximumOutOfPocketForMedicalEHBBenefitsCombinedInOutNetworkIndividualMap.ParentPropertyName = "MaximumOutOfPocketForMedicalEHBBenefits";
            MaximumOutOfPocketForMedicalEHBBenefitsCombinedInOutNetworkIndividualMap.FormPropertyName = "CombinedInOutNetworkIndividual";
            MaximumOutOfPocketForMedicalEHBBenefitsCombinedInOutNetworkIndividualMap.DomainPropertyName = "CombinedInOutNetworkIndividual";
            MaximumOutOfPocketForMedicalEHBBenefitsCombinedInOutNetworkIndividualMap.ColumnName = "AD";
            MaximumOutOfPocketForMedicalEHBBenefitsCombinedInOutNetworkIndividualMap.RowIndex = 4;
            MaximumOutOfPocketForMedicalEHBBenefitsCombinedInOutNetworkIndividualMap.QhpSheetType = QHPSheetType.CostShareVariance;
            MaximumOutOfPocketForMedicalEHBBenefitsCombinedInOutNetworkIndividualMap.CellFormat.CellAlignment = "Right";
            MaximumOutOfPocketForMedicalEHBBenefitsCombinedInOutNetworkIndividualMap.CellFormat.DataType = "int";
            MaximumOutOfPocketForMedicalEHBBenefitsCombinedInOutNetworkIndividualMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(MaximumOutOfPocketForMedicalEHBBenefitsCombinedInOutNetworkIndividualMap);

            QhpToExcelMap MaximumOutOfPocketForMedicalEHBBenefitsCombinedInOutNetworkFamilyMap = new QhpToExcelMap();
            MaximumOutOfPocketForMedicalEHBBenefitsCombinedInOutNetworkFamilyMap.ParentPropertyName = "MaximumOutOfPocketForMedicalEHBBenefits";
            MaximumOutOfPocketForMedicalEHBBenefitsCombinedInOutNetworkFamilyMap.FormPropertyName = "CombinedInOutNetworkFamily";
            MaximumOutOfPocketForMedicalEHBBenefitsCombinedInOutNetworkFamilyMap.DomainPropertyName = "CombinedInOutNetworkFamily";
            MaximumOutOfPocketForMedicalEHBBenefitsCombinedInOutNetworkFamilyMap.ColumnName = "AE";
            MaximumOutOfPocketForMedicalEHBBenefitsCombinedInOutNetworkFamilyMap.RowIndex = 4;
            MaximumOutOfPocketForMedicalEHBBenefitsCombinedInOutNetworkFamilyMap.QhpSheetType = QHPSheetType.CostShareVariance;
            MaximumOutOfPocketForMedicalEHBBenefitsCombinedInOutNetworkFamilyMap.CellFormat.CellAlignment = "Right";
            MaximumOutOfPocketForMedicalEHBBenefitsCombinedInOutNetworkFamilyMap.CellFormat.DataType = "int";
            MaximumOutOfPocketForMedicalEHBBenefitsCombinedInOutNetworkFamilyMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(MaximumOutOfPocketForMedicalEHBBenefitsCombinedInOutNetworkFamilyMap);

            QhpToExcelMap MaximumOutOfPocketForDrugEHBBenefitsInNetworkIndividualMap = new QhpToExcelMap();
            MaximumOutOfPocketForDrugEHBBenefitsInNetworkIndividualMap.ParentPropertyName = "MaximumOutOfPocketForDrugEHBBenefits";
            MaximumOutOfPocketForDrugEHBBenefitsInNetworkIndividualMap.FormPropertyName = "InNetworkIndividual";
            MaximumOutOfPocketForDrugEHBBenefitsInNetworkIndividualMap.DomainPropertyName = "InNetworkIndividual";
            MaximumOutOfPocketForDrugEHBBenefitsInNetworkIndividualMap.ColumnName = "AF";
            MaximumOutOfPocketForDrugEHBBenefitsInNetworkIndividualMap.RowIndex = 4;
            MaximumOutOfPocketForDrugEHBBenefitsInNetworkIndividualMap.QhpSheetType = QHPSheetType.CostShareVariance;
            MaximumOutOfPocketForDrugEHBBenefitsInNetworkIndividualMap.CellFormat.CellAlignment = "Right";
            MaximumOutOfPocketForDrugEHBBenefitsInNetworkIndividualMap.CellFormat.DataType = "decimal";
            MaximumOutOfPocketForDrugEHBBenefitsInNetworkIndividualMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(MaximumOutOfPocketForDrugEHBBenefitsInNetworkIndividualMap);

            QhpToExcelMap MaximumOutOfPocketForDrugEHBBenefitsInNetworkFamilyMap = new QhpToExcelMap();
            MaximumOutOfPocketForDrugEHBBenefitsInNetworkFamilyMap.ParentPropertyName = "MaximumOutOfPocketForDrugEHBBenefits";
            MaximumOutOfPocketForDrugEHBBenefitsInNetworkFamilyMap.FormPropertyName = "InNetworkFamily";
            MaximumOutOfPocketForDrugEHBBenefitsInNetworkFamilyMap.DomainPropertyName = "InNetworkFamily";
            MaximumOutOfPocketForDrugEHBBenefitsInNetworkFamilyMap.ColumnName = "AG";
            MaximumOutOfPocketForDrugEHBBenefitsInNetworkFamilyMap.RowIndex = 4;
            MaximumOutOfPocketForDrugEHBBenefitsInNetworkFamilyMap.QhpSheetType = QHPSheetType.CostShareVariance;
            MaximumOutOfPocketForDrugEHBBenefitsInNetworkFamilyMap.CellFormat.CellAlignment = "Right";
            MaximumOutOfPocketForDrugEHBBenefitsInNetworkFamilyMap.CellFormat.DataType = "decimal";
            MaximumOutOfPocketForDrugEHBBenefitsInNetworkFamilyMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(MaximumOutOfPocketForDrugEHBBenefitsInNetworkFamilyMap);

            QhpToExcelMap MaximumOutOfPocketForDrugEHBBenefitsInNetworkTier2IndividualMap = new QhpToExcelMap();
            MaximumOutOfPocketForDrugEHBBenefitsInNetworkTier2IndividualMap.ParentPropertyName = "MaximumOutOfPocketForDrugEHBBenefits";
            MaximumOutOfPocketForDrugEHBBenefitsInNetworkTier2IndividualMap.FormPropertyName = "InNetworkTier2Individual";
            MaximumOutOfPocketForDrugEHBBenefitsInNetworkTier2IndividualMap.DomainPropertyName = "InNetworkTier2Individual";
            MaximumOutOfPocketForDrugEHBBenefitsInNetworkTier2IndividualMap.ColumnName = "AH";
            MaximumOutOfPocketForDrugEHBBenefitsInNetworkTier2IndividualMap.RowIndex = 4;
            MaximumOutOfPocketForDrugEHBBenefitsInNetworkTier2IndividualMap.QhpSheetType = QHPSheetType.CostShareVariance;
            MaximumOutOfPocketForDrugEHBBenefitsInNetworkTier2IndividualMap.CellFormat.CellAlignment = "Right";
            MaximumOutOfPocketForDrugEHBBenefitsInNetworkTier2IndividualMap.CellFormat.DataType = "decimal";
            MaximumOutOfPocketForDrugEHBBenefitsInNetworkTier2IndividualMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(MaximumOutOfPocketForDrugEHBBenefitsInNetworkTier2IndividualMap);

            QhpToExcelMap MaximumOutOfPocketForDrugEHBBenefitsInNetworkTier2FamilyMap = new QhpToExcelMap();
            MaximumOutOfPocketForDrugEHBBenefitsInNetworkTier2FamilyMap.ParentPropertyName = "MaximumOutOfPocketForDrugEHBBenefits";
            MaximumOutOfPocketForDrugEHBBenefitsInNetworkTier2FamilyMap.FormPropertyName = "InNetworkTier2Family";
            MaximumOutOfPocketForDrugEHBBenefitsInNetworkTier2FamilyMap.DomainPropertyName = "InNetworkTier2Family";
            MaximumOutOfPocketForDrugEHBBenefitsInNetworkTier2FamilyMap.ColumnName = "AI";
            MaximumOutOfPocketForDrugEHBBenefitsInNetworkTier2FamilyMap.RowIndex = 4;
            MaximumOutOfPocketForDrugEHBBenefitsInNetworkTier2FamilyMap.QhpSheetType = QHPSheetType.CostShareVariance;
            MaximumOutOfPocketForDrugEHBBenefitsInNetworkTier2FamilyMap.CellFormat.CellAlignment = "Right";
            MaximumOutOfPocketForDrugEHBBenefitsInNetworkTier2FamilyMap.CellFormat.DataType = "decimal";
            MaximumOutOfPocketForDrugEHBBenefitsInNetworkTier2FamilyMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(MaximumOutOfPocketForDrugEHBBenefitsInNetworkTier2FamilyMap);

            QhpToExcelMap MaximumOutOfPocketForDrugEHBBenefitsOutOfNetworkIndividualMap = new QhpToExcelMap();
            MaximumOutOfPocketForDrugEHBBenefitsOutOfNetworkIndividualMap.ParentPropertyName = "MaximumOutOfPocketForDrugEHBBenefits";
            MaximumOutOfPocketForDrugEHBBenefitsOutOfNetworkIndividualMap.FormPropertyName = "OutOfNetworkIndividual";
            MaximumOutOfPocketForDrugEHBBenefitsOutOfNetworkIndividualMap.DomainPropertyName = "OutOfNetworkIndividual";
            MaximumOutOfPocketForDrugEHBBenefitsOutOfNetworkIndividualMap.ColumnName = "AJ";
            MaximumOutOfPocketForDrugEHBBenefitsOutOfNetworkIndividualMap.RowIndex = 4;
            MaximumOutOfPocketForDrugEHBBenefitsOutOfNetworkIndividualMap.QhpSheetType = QHPSheetType.CostShareVariance;
            MaximumOutOfPocketForDrugEHBBenefitsOutOfNetworkIndividualMap.CellFormat.CellAlignment = "Right";
            MaximumOutOfPocketForDrugEHBBenefitsOutOfNetworkIndividualMap.CellFormat.DataType = "decimal";
            MaximumOutOfPocketForDrugEHBBenefitsOutOfNetworkIndividualMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(MaximumOutOfPocketForDrugEHBBenefitsOutOfNetworkIndividualMap);

            QhpToExcelMap MaximumOutOfPocketForDrugEHBBenefitsOutOfNetworkFamilyMap = new QhpToExcelMap();
            MaximumOutOfPocketForDrugEHBBenefitsOutOfNetworkFamilyMap.ParentPropertyName = "MaximumOutOfPocketForDrugEHBBenefits";
            MaximumOutOfPocketForDrugEHBBenefitsOutOfNetworkFamilyMap.FormPropertyName = "OutOfNetworkFamily";
            MaximumOutOfPocketForDrugEHBBenefitsOutOfNetworkFamilyMap.DomainPropertyName = "OutOfNetworkFamily";
            MaximumOutOfPocketForDrugEHBBenefitsOutOfNetworkFamilyMap.ColumnName = "AK";
            MaximumOutOfPocketForDrugEHBBenefitsOutOfNetworkFamilyMap.RowIndex = 4;
            MaximumOutOfPocketForDrugEHBBenefitsOutOfNetworkFamilyMap.QhpSheetType = QHPSheetType.CostShareVariance;
            MaximumOutOfPocketForDrugEHBBenefitsOutOfNetworkFamilyMap.CellFormat.CellAlignment = "Right";
            MaximumOutOfPocketForDrugEHBBenefitsOutOfNetworkFamilyMap.CellFormat.DataType = "decimal";
            MaximumOutOfPocketForDrugEHBBenefitsOutOfNetworkFamilyMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(MaximumOutOfPocketForDrugEHBBenefitsOutOfNetworkFamilyMap);

            QhpToExcelMap MaximumOutOfPocketForDrugEHBBenefitsCombinedInOutNetworkIndividualMap = new QhpToExcelMap();
            MaximumOutOfPocketForDrugEHBBenefitsCombinedInOutNetworkIndividualMap.ParentPropertyName = "MaximumOutOfPocketForDrugEHBBenefits";
            MaximumOutOfPocketForDrugEHBBenefitsCombinedInOutNetworkIndividualMap.FormPropertyName = "CombinedInOutNetworkIndividual";
            MaximumOutOfPocketForDrugEHBBenefitsCombinedInOutNetworkIndividualMap.DomainPropertyName = "CombinedInOutNetworkIndividual";
            MaximumOutOfPocketForDrugEHBBenefitsCombinedInOutNetworkIndividualMap.ColumnName = "AL";
            MaximumOutOfPocketForDrugEHBBenefitsCombinedInOutNetworkIndividualMap.RowIndex = 4;
            MaximumOutOfPocketForDrugEHBBenefitsCombinedInOutNetworkIndividualMap.QhpSheetType = QHPSheetType.CostShareVariance;
            MaximumOutOfPocketForDrugEHBBenefitsCombinedInOutNetworkIndividualMap.CellFormat.CellAlignment = "Right";
            MaximumOutOfPocketForDrugEHBBenefitsCombinedInOutNetworkIndividualMap.CellFormat.DataType = "decimal";
            MaximumOutOfPocketForDrugEHBBenefitsCombinedInOutNetworkIndividualMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(MaximumOutOfPocketForDrugEHBBenefitsCombinedInOutNetworkIndividualMap);

            QhpToExcelMap MaximumOutOfPocketForDrugEHBBenefitsCombinedInOutNetworkFamilyMap = new QhpToExcelMap();
            MaximumOutOfPocketForDrugEHBBenefitsCombinedInOutNetworkFamilyMap.ParentPropertyName = "MaximumOutOfPocketForDrugEHBBenefits";
            MaximumOutOfPocketForDrugEHBBenefitsCombinedInOutNetworkFamilyMap.FormPropertyName = "CombinedInOutNetworkFamily";
            MaximumOutOfPocketForDrugEHBBenefitsCombinedInOutNetworkFamilyMap.DomainPropertyName = "CombinedInOutNetworkFamily";
            MaximumOutOfPocketForDrugEHBBenefitsCombinedInOutNetworkFamilyMap.ColumnName = "AM";
            MaximumOutOfPocketForDrugEHBBenefitsCombinedInOutNetworkFamilyMap.RowIndex = 4;
            MaximumOutOfPocketForDrugEHBBenefitsCombinedInOutNetworkFamilyMap.QhpSheetType = QHPSheetType.CostShareVariance;
            MaximumOutOfPocketForDrugEHBBenefitsCombinedInOutNetworkFamilyMap.CellFormat.CellAlignment = "Right";
            MaximumOutOfPocketForDrugEHBBenefitsCombinedInOutNetworkFamilyMap.CellFormat.DataType = "decimal";
            MaximumOutOfPocketForDrugEHBBenefitsCombinedInOutNetworkFamilyMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(MaximumOutOfPocketForDrugEHBBenefitsCombinedInOutNetworkFamilyMap);
            
            QhpToExcelMap MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkIndividualMap = new QhpToExcelMap();
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkIndividualMap.ParentPropertyName = "MaximumOutOfPocketForMedicalAndDrugEHBBenefits";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkIndividualMap.FormPropertyName = "InNetworkIndividual";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkIndividualMap.DomainPropertyName = "InNetworkIndividual";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkIndividualMap.ColumnName = "AN";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkIndividualMap.RowIndex = 4;
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkIndividualMap.QhpSheetType = QHPSheetType.CostShareVariance;
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkIndividualMap.CellFormat.CellAlignment = "Right";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkIndividualMap.CellFormat.DataType = "decimal";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkIndividualMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkIndividualMap);

            QhpToExcelMap MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkFamilyMap = new QhpToExcelMap();
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkFamilyMap.ParentPropertyName = "MaximumOutOfPocketForMedicalAndDrugEHBBenefits";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkFamilyMap.FormPropertyName = "InNetworkFamily";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkFamilyMap.DomainPropertyName = "InNetworkFamily";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkFamilyMap.ColumnName = "AO";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkFamilyMap.RowIndex = 4;
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkFamilyMap.QhpSheetType = QHPSheetType.CostShareVariance;
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkFamilyMap.CellFormat.CellAlignment = "Right";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkFamilyMap.CellFormat.DataType = "decimal";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkFamilyMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkFamilyMap);

            QhpToExcelMap MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkTier2IndividualMap = new QhpToExcelMap();
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkTier2IndividualMap.ParentPropertyName = "MaximumOutOfPocketForMedicalAndDrugEHBBenefits";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkTier2IndividualMap.FormPropertyName = "InNetworkTier2Individual";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkTier2IndividualMap.DomainPropertyName = "InNetworkTier2Individual";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkTier2IndividualMap.ColumnName = "AP";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkTier2IndividualMap.RowIndex = 4;
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkTier2IndividualMap.QhpSheetType = QHPSheetType.CostShareVariance;
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkTier2IndividualMap.CellFormat.CellAlignment = "Right";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkTier2IndividualMap.CellFormat.DataType = "decimal";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkTier2IndividualMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkTier2IndividualMap);

            QhpToExcelMap MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkTier2FamilyMap = new QhpToExcelMap();
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkTier2FamilyMap.ParentPropertyName = "MaximumOutOfPocketForMedicalAndDrugEHBBenefits";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkTier2FamilyMap.FormPropertyName = "InNetworkTier2Family";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkTier2FamilyMap.DomainPropertyName = "InNetworkTier2Family";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkTier2FamilyMap.ColumnName = "AQ";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkTier2FamilyMap.RowIndex = 4;
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkTier2FamilyMap.QhpSheetType = QHPSheetType.CostShareVariance;
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkTier2FamilyMap.CellFormat.CellAlignment = "Right";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkTier2FamilyMap.CellFormat.DataType = "decimal";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkTier2FamilyMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(MaximumOutOfPocketForMedicalAndDrugEHBBenefitsInNetworkTier2FamilyMap);

            QhpToExcelMap MaximumOutOfPocketForMedicalAndDrugEHBBenefitsOutOfNetworkIndividualMap = new QhpToExcelMap();
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsOutOfNetworkIndividualMap.ParentPropertyName = "MaximumOutOfPocketForMedicalAndDrugEHBBenefits";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsOutOfNetworkIndividualMap.FormPropertyName = "OutOfNetworkIndividual";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsOutOfNetworkIndividualMap.DomainPropertyName = "OutOfNetworkIndividual";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsOutOfNetworkIndividualMap.ColumnName = "AR";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsOutOfNetworkIndividualMap.RowIndex = 4;
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsOutOfNetworkIndividualMap.QhpSheetType = QHPSheetType.CostShareVariance;
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsOutOfNetworkIndividualMap.CellFormat.CellAlignment = "Right";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsOutOfNetworkIndividualMap.CellFormat.DataType = "decimal";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsOutOfNetworkIndividualMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(MaximumOutOfPocketForMedicalAndDrugEHBBenefitsOutOfNetworkIndividualMap);

            QhpToExcelMap MaximumOutOfPocketForMedicalAndDrugEHBBenefitsOutOfNetworkFamilyMap = new QhpToExcelMap();
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsOutOfNetworkFamilyMap.ParentPropertyName = "MaximumOutOfPocketForMedicalAndDrugEHBBenefits";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsOutOfNetworkFamilyMap.FormPropertyName = "OutOfNetworkFamily";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsOutOfNetworkFamilyMap.DomainPropertyName = "OutOfNetworkFamily";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsOutOfNetworkFamilyMap.ColumnName = "AS";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsOutOfNetworkFamilyMap.RowIndex = 4;
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsOutOfNetworkFamilyMap.QhpSheetType = QHPSheetType.CostShareVariance;
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsOutOfNetworkFamilyMap.CellFormat.CellAlignment = "Right";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsOutOfNetworkFamilyMap.CellFormat.DataType = "decimal";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsOutOfNetworkFamilyMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(MaximumOutOfPocketForMedicalAndDrugEHBBenefitsOutOfNetworkFamilyMap);

            QhpToExcelMap MaximumOutOfPocketForMedicalAndDrugEHBBenefitsCombinedInOutNetworkIndividualMap = new QhpToExcelMap();
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsCombinedInOutNetworkIndividualMap.ParentPropertyName = "MaximumOutOfPocketForMedicalAndDrugEHBBenefits";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsCombinedInOutNetworkIndividualMap.FormPropertyName = "CombinedInOutNetworkIndividual";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsCombinedInOutNetworkIndividualMap.DomainPropertyName = "CombinedInOutNetworkIndividual";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsCombinedInOutNetworkIndividualMap.ColumnName = "AT";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsCombinedInOutNetworkIndividualMap.RowIndex = 4;
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsCombinedInOutNetworkIndividualMap.QhpSheetType = QHPSheetType.CostShareVariance;
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsCombinedInOutNetworkIndividualMap.CellFormat.CellAlignment = "Right";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsCombinedInOutNetworkIndividualMap.CellFormat.DataType = "decimal";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsCombinedInOutNetworkIndividualMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(MaximumOutOfPocketForMedicalAndDrugEHBBenefitsCombinedInOutNetworkIndividualMap);

            QhpToExcelMap MaximumOutOfPocketForMedicalAndDrugEHBBenefitsCombinedInOutNetworkFamilyMap = new QhpToExcelMap();
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsCombinedInOutNetworkFamilyMap.ParentPropertyName = "MaximumOutOfPocketForMedicalAndDrugEHBBenefits";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsCombinedInOutNetworkFamilyMap.FormPropertyName = "CombinedInOutNetworkFamily";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsCombinedInOutNetworkFamilyMap.DomainPropertyName = "CombinedInOutNetworkFamily";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsCombinedInOutNetworkFamilyMap.ColumnName = "AU";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsCombinedInOutNetworkFamilyMap.RowIndex = 4;
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsCombinedInOutNetworkFamilyMap.QhpSheetType = QHPSheetType.CostShareVariance;
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsCombinedInOutNetworkFamilyMap.CellFormat.CellAlignment = "Right";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsCombinedInOutNetworkFamilyMap.CellFormat.DataType = "decimal";
            MaximumOutOfPocketForMedicalAndDrugEHBBenefitsCombinedInOutNetworkFamilyMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(MaximumOutOfPocketForMedicalAndDrugEHBBenefitsCombinedInOutNetworkFamilyMap);
        }

        private void BuildDeductibleAttributes()
        {
            QhpToExcelMap MedicalEHBDeductibleInNetworkIndividualMap = new QhpToExcelMap();
            MedicalEHBDeductibleInNetworkIndividualMap.ParentPropertyName = "MedicalEHBDeductible";
            MedicalEHBDeductibleInNetworkIndividualMap.FormPropertyName = "InNetworkIndividual";
            MedicalEHBDeductibleInNetworkIndividualMap.DomainPropertyName = "InNetworkIndividual";
            MedicalEHBDeductibleInNetworkIndividualMap.ColumnName = "AV";
            MedicalEHBDeductibleInNetworkIndividualMap.RowIndex = 4;
            MedicalEHBDeductibleInNetworkIndividualMap.QhpSheetType = QHPSheetType.CostShareVariance;
            MedicalEHBDeductibleInNetworkIndividualMap.CellFormat.CellAlignment = "Right";
            MedicalEHBDeductibleInNetworkIndividualMap.CellFormat.DataType = "decimal";
            MedicalEHBDeductibleInNetworkIndividualMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(MedicalEHBDeductibleInNetworkIndividualMap);

            QhpToExcelMap MedicalEHBDeductibleInNetworkFamilyMap = new QhpToExcelMap();
            MedicalEHBDeductibleInNetworkFamilyMap.ParentPropertyName = "MedicalEHBDeductible";
            MedicalEHBDeductibleInNetworkFamilyMap.FormPropertyName = "InNetworkFamily";
            MedicalEHBDeductibleInNetworkFamilyMap.DomainPropertyName = "InNetworkFamily";
            MedicalEHBDeductibleInNetworkFamilyMap.ColumnName = "AW";
            MedicalEHBDeductibleInNetworkFamilyMap.RowIndex = 4;
            MedicalEHBDeductibleInNetworkFamilyMap.QhpSheetType = QHPSheetType.CostShareVariance;
            MedicalEHBDeductibleInNetworkFamilyMap.CellFormat.CellAlignment = "Right";
            MedicalEHBDeductibleInNetworkFamilyMap.CellFormat.DataType = "decimal";
            MedicalEHBDeductibleInNetworkFamilyMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(MedicalEHBDeductibleInNetworkFamilyMap);

            QhpToExcelMap MedicalEHBDeductibleInNetworkDefaultCoinsuranceMap = new QhpToExcelMap();
            MedicalEHBDeductibleInNetworkDefaultCoinsuranceMap.ParentPropertyName = "MedicalEHBDeductible";
            MedicalEHBDeductibleInNetworkDefaultCoinsuranceMap.FormPropertyName = "InNetworkDefaultCoinsurance";
            MedicalEHBDeductibleInNetworkDefaultCoinsuranceMap.DomainPropertyName = "InNetworkDefaultCoinsurance";
            MedicalEHBDeductibleInNetworkDefaultCoinsuranceMap.ColumnName = "AX";
            MedicalEHBDeductibleInNetworkDefaultCoinsuranceMap.RowIndex = 4;
            MedicalEHBDeductibleInNetworkDefaultCoinsuranceMap.QhpSheetType = QHPSheetType.CostShareVariance;
            MedicalEHBDeductibleInNetworkDefaultCoinsuranceMap.CellFormat.CellAlignment = "Right";
            MedicalEHBDeductibleInNetworkDefaultCoinsuranceMap.CellFormat.DataType = "decimal";
            MedicalEHBDeductibleInNetworkDefaultCoinsuranceMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(MedicalEHBDeductibleInNetworkDefaultCoinsuranceMap);

            QhpToExcelMap MedicalEHBDeductibleInNetworkTier2IndividualMap = new QhpToExcelMap();
            MedicalEHBDeductibleInNetworkTier2IndividualMap.ParentPropertyName = "MedicalEHBDeductible";
            MedicalEHBDeductibleInNetworkTier2IndividualMap.FormPropertyName = "InNetworkTier2Individual";
            MedicalEHBDeductibleInNetworkTier2IndividualMap.DomainPropertyName = "InNetworkTier2Individual";
            MedicalEHBDeductibleInNetworkTier2IndividualMap.ColumnName = "AY";
            MedicalEHBDeductibleInNetworkTier2IndividualMap.RowIndex = 4;
            MedicalEHBDeductibleInNetworkTier2IndividualMap.QhpSheetType = QHPSheetType.CostShareVariance;
            MedicalEHBDeductibleInNetworkTier2IndividualMap.CellFormat.CellAlignment = "Right";
            MedicalEHBDeductibleInNetworkTier2IndividualMap.CellFormat.DataType = "decimal";
            MedicalEHBDeductibleInNetworkTier2IndividualMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(MedicalEHBDeductibleInNetworkTier2IndividualMap);

            QhpToExcelMap MedicalEHBDeductibleInNetworkTier2FamilyMap = new QhpToExcelMap();
            MedicalEHBDeductibleInNetworkTier2FamilyMap.ParentPropertyName = "MedicalEHBDeductible";
            MedicalEHBDeductibleInNetworkTier2FamilyMap.FormPropertyName = "InNetworkTier2Family";
            MedicalEHBDeductibleInNetworkTier2FamilyMap.DomainPropertyName = "InNetworkTier2Family";
            MedicalEHBDeductibleInNetworkTier2FamilyMap.ColumnName = "AZ";
            MedicalEHBDeductibleInNetworkTier2FamilyMap.RowIndex = 4;
            MedicalEHBDeductibleInNetworkTier2FamilyMap.QhpSheetType = QHPSheetType.CostShareVariance;
            MedicalEHBDeductibleInNetworkTier2FamilyMap.CellFormat.CellAlignment = "Right";
            MedicalEHBDeductibleInNetworkTier2FamilyMap.CellFormat.DataType = "decimal";
            MedicalEHBDeductibleInNetworkTier2FamilyMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(MedicalEHBDeductibleInNetworkTier2FamilyMap);

            QhpToExcelMap MedicalEHBDeductibleInNetworkTier2DefaultCoinsuranceMap = new QhpToExcelMap();
            MedicalEHBDeductibleInNetworkTier2DefaultCoinsuranceMap.ParentPropertyName = "MedicalEHBDeductible";
            MedicalEHBDeductibleInNetworkTier2DefaultCoinsuranceMap.FormPropertyName = "InNetworkTier2DefaultCoinsurance";
            MedicalEHBDeductibleInNetworkTier2DefaultCoinsuranceMap.DomainPropertyName = "InNetworkTier2DefaultCoinsurance";
            MedicalEHBDeductibleInNetworkTier2DefaultCoinsuranceMap.ColumnName = "BA";
            MedicalEHBDeductibleInNetworkTier2DefaultCoinsuranceMap.RowIndex = 4;
            MedicalEHBDeductibleInNetworkTier2DefaultCoinsuranceMap.QhpSheetType = QHPSheetType.CostShareVariance;
            MedicalEHBDeductibleInNetworkTier2DefaultCoinsuranceMap.CellFormat.CellAlignment = "Right";
            MedicalEHBDeductibleInNetworkTier2DefaultCoinsuranceMap.CellFormat.DataType = "decimal";
            MedicalEHBDeductibleInNetworkTier2DefaultCoinsuranceMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(MedicalEHBDeductibleInNetworkTier2DefaultCoinsuranceMap);

            QhpToExcelMap MedicalEHBDeductibleOutOfNetworkIndividualMap = new QhpToExcelMap();
            MedicalEHBDeductibleOutOfNetworkIndividualMap.ParentPropertyName = "MedicalEHBDeductible";
            MedicalEHBDeductibleOutOfNetworkIndividualMap.FormPropertyName = "OutOfNetworkIndividual";
            MedicalEHBDeductibleOutOfNetworkIndividualMap.DomainPropertyName = "OutOfNetworkIndividual";
            MedicalEHBDeductibleOutOfNetworkIndividualMap.ColumnName = "BB";
            MedicalEHBDeductibleOutOfNetworkIndividualMap.RowIndex = 4;
            MedicalEHBDeductibleOutOfNetworkIndividualMap.QhpSheetType = QHPSheetType.CostShareVariance;
            MedicalEHBDeductibleOutOfNetworkIndividualMap.CellFormat.CellAlignment = "Right";
            MedicalEHBDeductibleOutOfNetworkIndividualMap.CellFormat.DataType = "decimal";
            MedicalEHBDeductibleOutOfNetworkIndividualMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(MedicalEHBDeductibleOutOfNetworkIndividualMap);

            QhpToExcelMap MedicalEHBDeductibleOutOfNetworkFamilyMap = new QhpToExcelMap();
            MedicalEHBDeductibleOutOfNetworkFamilyMap.ParentPropertyName = "MedicalEHBDeductible";
            MedicalEHBDeductibleOutOfNetworkFamilyMap.FormPropertyName = "OutOfNetworkFamily";
            MedicalEHBDeductibleOutOfNetworkFamilyMap.DomainPropertyName = "OutOfNetworkFamily";
            MedicalEHBDeductibleOutOfNetworkFamilyMap.ColumnName = "BC";
            MedicalEHBDeductibleOutOfNetworkFamilyMap.RowIndex = 4;
            MedicalEHBDeductibleOutOfNetworkFamilyMap.QhpSheetType = QHPSheetType.CostShareVariance;
            MedicalEHBDeductibleOutOfNetworkFamilyMap.CellFormat.CellAlignment = "Right";
            MedicalEHBDeductibleOutOfNetworkFamilyMap.CellFormat.DataType = "decimal";
            MedicalEHBDeductibleOutOfNetworkFamilyMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(MedicalEHBDeductibleOutOfNetworkFamilyMap);

            QhpToExcelMap MedicalEHBDeductibleCombinedInOutNetworkIndividualMap = new QhpToExcelMap();
            MedicalEHBDeductibleCombinedInOutNetworkIndividualMap.ParentPropertyName = "MedicalEHBDeductible";
            MedicalEHBDeductibleCombinedInOutNetworkIndividualMap.FormPropertyName = "CombinedInOutNetworkIndividual";
            MedicalEHBDeductibleCombinedInOutNetworkIndividualMap.DomainPropertyName = "CombinedInOutNetworkIndividual";
            MedicalEHBDeductibleCombinedInOutNetworkIndividualMap.ColumnName = "BD";
            MedicalEHBDeductibleCombinedInOutNetworkIndividualMap.RowIndex = 4;
            MedicalEHBDeductibleCombinedInOutNetworkIndividualMap.QhpSheetType = QHPSheetType.CostShareVariance;
            MedicalEHBDeductibleCombinedInOutNetworkIndividualMap.CellFormat.CellAlignment = "Right";
            MedicalEHBDeductibleCombinedInOutNetworkIndividualMap.CellFormat.DataType = "decimal";
            MedicalEHBDeductibleCombinedInOutNetworkIndividualMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(MedicalEHBDeductibleCombinedInOutNetworkIndividualMap);

            QhpToExcelMap MedicalEHBDeductibleCombinedInOutNetworkFamilyMap = new QhpToExcelMap();
            MedicalEHBDeductibleCombinedInOutNetworkFamilyMap.ParentPropertyName = "MedicalEHBDeductible";
            MedicalEHBDeductibleCombinedInOutNetworkFamilyMap.FormPropertyName = "CombinedInOutNetworkFamily";
            MedicalEHBDeductibleCombinedInOutNetworkFamilyMap.DomainPropertyName = "CombinedInOutNetworkFamily";
            MedicalEHBDeductibleCombinedInOutNetworkFamilyMap.ColumnName = "BE";
            MedicalEHBDeductibleCombinedInOutNetworkFamilyMap.RowIndex = 4;
            MedicalEHBDeductibleCombinedInOutNetworkFamilyMap.QhpSheetType = QHPSheetType.CostShareVariance;
            MedicalEHBDeductibleCombinedInOutNetworkFamilyMap.CellFormat.CellAlignment = "Right";
            MedicalEHBDeductibleCombinedInOutNetworkFamilyMap.CellFormat.DataType = "decimal";
            MedicalEHBDeductibleCombinedInOutNetworkFamilyMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(MedicalEHBDeductibleCombinedInOutNetworkFamilyMap);

            QhpToExcelMap DrugEHBDeductibleDeductibleTypeMap = new QhpToExcelMap();
            DrugEHBDeductibleDeductibleTypeMap.ParentPropertyName = "DrugEHBDeductible";
            DrugEHBDeductibleDeductibleTypeMap.FormPropertyName = "DeductibleDrugType";
            DrugEHBDeductibleDeductibleTypeMap.DomainPropertyName = "DeductibleDrugType";
            DrugEHBDeductibleDeductibleTypeMap.ColumnName = "BF";
            DrugEHBDeductibleDeductibleTypeMap.RowIndex = 1;
            DrugEHBDeductibleDeductibleTypeMap.IncrementStep = 10;
            DrugEHBDeductibleDeductibleTypeMap.QhpSheetType = QHPSheetType.CostShareVariance;
            DrugEHBDeductibleDeductibleTypeMap.CellFormat.CellAlignment = "Left";
            DrugEHBDeductibleDeductibleTypeMap.CellFormat.DataType = "string";

            this.MappingsList.Add(DrugEHBDeductibleDeductibleTypeMap);

            QhpToExcelMap DrugEHBDeductibleInNetworkIndividualMap = new QhpToExcelMap();
            DrugEHBDeductibleInNetworkIndividualMap.ParentPropertyName = "DrugEHBDeductible";
            DrugEHBDeductibleInNetworkIndividualMap.FormPropertyName = "InNetworkIndividual";
            DrugEHBDeductibleInNetworkIndividualMap.DomainPropertyName = "InNetworkIndividual";
            DrugEHBDeductibleInNetworkIndividualMap.ColumnName = "BF";
            DrugEHBDeductibleInNetworkIndividualMap.RowIndex = 4;
            DrugEHBDeductibleInNetworkIndividualMap.QhpSheetType = QHPSheetType.CostShareVariance;
            DrugEHBDeductibleInNetworkIndividualMap.CellFormat.CellAlignment = "Right";
            DrugEHBDeductibleInNetworkIndividualMap.CellFormat.DataType = "decimal";
            DrugEHBDeductibleInNetworkIndividualMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(DrugEHBDeductibleInNetworkIndividualMap);

            QhpToExcelMap DrugEHBDeductibleInNetworkFamilyMap = new QhpToExcelMap();
            DrugEHBDeductibleInNetworkFamilyMap.ParentPropertyName = "DrugEHBDeductible";
            DrugEHBDeductibleInNetworkFamilyMap.FormPropertyName = "InNetworkFamily";
            DrugEHBDeductibleInNetworkFamilyMap.DomainPropertyName = "InNetworkFamily";
            DrugEHBDeductibleInNetworkFamilyMap.ColumnName = "BG";
            DrugEHBDeductibleInNetworkFamilyMap.RowIndex = 4;
            DrugEHBDeductibleInNetworkFamilyMap.QhpSheetType = QHPSheetType.CostShareVariance;
            DrugEHBDeductibleInNetworkFamilyMap.CellFormat.CellAlignment = "Right";
            DrugEHBDeductibleInNetworkFamilyMap.CellFormat.DataType = "decimal";
            DrugEHBDeductibleInNetworkFamilyMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(DrugEHBDeductibleInNetworkFamilyMap);

            QhpToExcelMap DrugEHBDeductibleInNetworkDefaultCoinsuranceMap = new QhpToExcelMap();
            DrugEHBDeductibleInNetworkDefaultCoinsuranceMap.ParentPropertyName = "DrugEHBDeductible";
            DrugEHBDeductibleInNetworkDefaultCoinsuranceMap.FormPropertyName = "InNetworkDefaultCoinsurance";
            DrugEHBDeductibleInNetworkDefaultCoinsuranceMap.DomainPropertyName = "InNetworkDefaultCoinsurance";
            DrugEHBDeductibleInNetworkDefaultCoinsuranceMap.ColumnName = "BH";
            DrugEHBDeductibleInNetworkDefaultCoinsuranceMap.RowIndex = 4;
            DrugEHBDeductibleInNetworkDefaultCoinsuranceMap.QhpSheetType = QHPSheetType.CostShareVariance;
            DrugEHBDeductibleInNetworkDefaultCoinsuranceMap.CellFormat.CellAlignment = "Right";
            DrugEHBDeductibleInNetworkDefaultCoinsuranceMap.CellFormat.DataType = "decimal";
            DrugEHBDeductibleInNetworkDefaultCoinsuranceMap.CellFormat.CellPrefix = "%";

            this.MappingsList.Add(DrugEHBDeductibleInNetworkDefaultCoinsuranceMap);

            QhpToExcelMap DrugEHBDeductibleInNetworkTier2IndividualMap = new QhpToExcelMap();
            DrugEHBDeductibleInNetworkTier2IndividualMap.ParentPropertyName = "DrugEHBDeductible";
            DrugEHBDeductibleInNetworkTier2IndividualMap.FormPropertyName = "InNetworkTier2Individual";
            DrugEHBDeductibleInNetworkTier2IndividualMap.DomainPropertyName = "InNetworkTier2Individual";
            DrugEHBDeductibleInNetworkTier2IndividualMap.ColumnName = "BI";
            DrugEHBDeductibleInNetworkTier2IndividualMap.RowIndex = 4;
            DrugEHBDeductibleInNetworkTier2IndividualMap.QhpSheetType = QHPSheetType.CostShareVariance;
            DrugEHBDeductibleInNetworkTier2IndividualMap.CellFormat.CellAlignment = "Right";
            DrugEHBDeductibleInNetworkTier2IndividualMap.CellFormat.DataType = "decimal";
            DrugEHBDeductibleInNetworkTier2IndividualMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(DrugEHBDeductibleInNetworkTier2IndividualMap);

            QhpToExcelMap DrugEHBDeductibleInNetworkTier2FamilyMap = new QhpToExcelMap();
            DrugEHBDeductibleInNetworkTier2FamilyMap.ParentPropertyName = "DrugEHBDeductible";
            DrugEHBDeductibleInNetworkTier2FamilyMap.FormPropertyName = "InNetworkTier2Family";
            DrugEHBDeductibleInNetworkTier2FamilyMap.DomainPropertyName = "InNetworkTier2Family";
            DrugEHBDeductibleInNetworkTier2FamilyMap.ColumnName = "BJ";
            DrugEHBDeductibleInNetworkTier2FamilyMap.RowIndex = 4;
            DrugEHBDeductibleInNetworkTier2FamilyMap.QhpSheetType = QHPSheetType.CostShareVariance;
            DrugEHBDeductibleInNetworkTier2FamilyMap.CellFormat.CellAlignment = "Right";
            DrugEHBDeductibleInNetworkTier2FamilyMap.CellFormat.DataType = "decimal";
            DrugEHBDeductibleInNetworkTier2FamilyMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(DrugEHBDeductibleInNetworkTier2FamilyMap);

            QhpToExcelMap DrugEHBDeductibleInNetworkTier2DefaultCoinsuranceMap = new QhpToExcelMap();
            DrugEHBDeductibleInNetworkTier2DefaultCoinsuranceMap.ParentPropertyName = "DrugEHBDeductible";
            DrugEHBDeductibleInNetworkTier2DefaultCoinsuranceMap.FormPropertyName = "InNetworkTier2DefaultCoinsurance";
            DrugEHBDeductibleInNetworkTier2DefaultCoinsuranceMap.DomainPropertyName = "InNetworkTier2DefaultCoinsurance";
            DrugEHBDeductibleInNetworkTier2DefaultCoinsuranceMap.ColumnName = "BK";
            DrugEHBDeductibleInNetworkTier2DefaultCoinsuranceMap.RowIndex = 4;
            DrugEHBDeductibleInNetworkTier2DefaultCoinsuranceMap.QhpSheetType = QHPSheetType.CostShareVariance;
            DrugEHBDeductibleInNetworkTier2DefaultCoinsuranceMap.CellFormat.CellAlignment = "Right";
            DrugEHBDeductibleInNetworkTier2DefaultCoinsuranceMap.CellFormat.DataType = "decimal";
            DrugEHBDeductibleInNetworkTier2DefaultCoinsuranceMap.CellFormat.CellPrefix = "%";

            this.MappingsList.Add(DrugEHBDeductibleInNetworkTier2DefaultCoinsuranceMap);

            QhpToExcelMap DrugEHBDeductibleOutOfNetworkIndividualMap = new QhpToExcelMap();
            DrugEHBDeductibleOutOfNetworkIndividualMap.ParentPropertyName = "DrugEHBDeductible";
            DrugEHBDeductibleOutOfNetworkIndividualMap.FormPropertyName = "OutOfNetworkIndividual";
            DrugEHBDeductibleOutOfNetworkIndividualMap.DomainPropertyName = "OutOfNetworkIndividual";
            DrugEHBDeductibleOutOfNetworkIndividualMap.ColumnName = "BL";
            DrugEHBDeductibleOutOfNetworkIndividualMap.RowIndex = 4;
            DrugEHBDeductibleOutOfNetworkIndividualMap.QhpSheetType = QHPSheetType.CostShareVariance;
            DrugEHBDeductibleOutOfNetworkIndividualMap.CellFormat.CellAlignment = "Right";
            DrugEHBDeductibleOutOfNetworkIndividualMap.CellFormat.DataType = "decimal";
            DrugEHBDeductibleOutOfNetworkIndividualMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(DrugEHBDeductibleOutOfNetworkIndividualMap);

            QhpToExcelMap DrugEHBDeductibleOutOfNetworkFamilyMap = new QhpToExcelMap();
            DrugEHBDeductibleOutOfNetworkFamilyMap.ParentPropertyName = "DrugEHBDeductible";
            DrugEHBDeductibleOutOfNetworkFamilyMap.FormPropertyName = "OutOfNetworkFamily";
            DrugEHBDeductibleOutOfNetworkFamilyMap.DomainPropertyName = "OutOfNetworkFamily";
            DrugEHBDeductibleOutOfNetworkFamilyMap.ColumnName = "BM";
            DrugEHBDeductibleOutOfNetworkFamilyMap.RowIndex = 4;
            DrugEHBDeductibleOutOfNetworkFamilyMap.QhpSheetType = QHPSheetType.CostShareVariance;
            DrugEHBDeductibleOutOfNetworkFamilyMap.CellFormat.CellAlignment = "Right";
            DrugEHBDeductibleOutOfNetworkFamilyMap.CellFormat.DataType = "decimal";
            DrugEHBDeductibleOutOfNetworkFamilyMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(DrugEHBDeductibleOutOfNetworkFamilyMap);

            QhpToExcelMap DrugEHBDeductibleCombinedInOutNetworkIndividualMap = new QhpToExcelMap();
            DrugEHBDeductibleCombinedInOutNetworkIndividualMap.ParentPropertyName = "DrugEHBDeductible";
            DrugEHBDeductibleCombinedInOutNetworkIndividualMap.FormPropertyName = "CombinedInOutNetworkIndividual";
            DrugEHBDeductibleCombinedInOutNetworkIndividualMap.DomainPropertyName = "CombinedInOutNetworkIndividual";
            DrugEHBDeductibleCombinedInOutNetworkIndividualMap.ColumnName = "BN";
            DrugEHBDeductibleCombinedInOutNetworkIndividualMap.RowIndex = 4;
            DrugEHBDeductibleCombinedInOutNetworkIndividualMap.QhpSheetType = QHPSheetType.CostShareVariance;
            DrugEHBDeductibleCombinedInOutNetworkIndividualMap.CellFormat.CellAlignment = "Right";
            DrugEHBDeductibleCombinedInOutNetworkIndividualMap.CellFormat.DataType = "decimal";
            DrugEHBDeductibleCombinedInOutNetworkIndividualMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(DrugEHBDeductibleCombinedInOutNetworkIndividualMap);

            QhpToExcelMap DrugEHBDeductibleCombinedInOutNetworkFamilyMap = new QhpToExcelMap();
            DrugEHBDeductibleCombinedInOutNetworkFamilyMap.ParentPropertyName = "DrugEHBDeductible";
            DrugEHBDeductibleCombinedInOutNetworkFamilyMap.FormPropertyName = "CombinedInOutNetworkFamily";
            DrugEHBDeductibleCombinedInOutNetworkFamilyMap.DomainPropertyName = "CombinedInOutNetworkFamily";
            DrugEHBDeductibleCombinedInOutNetworkFamilyMap.ColumnName = "BO";
            DrugEHBDeductibleCombinedInOutNetworkFamilyMap.RowIndex = 4;
            DrugEHBDeductibleCombinedInOutNetworkFamilyMap.QhpSheetType = QHPSheetType.CostShareVariance;
            DrugEHBDeductibleCombinedInOutNetworkFamilyMap.CellFormat.CellAlignment = "Right";
            DrugEHBDeductibleCombinedInOutNetworkFamilyMap.CellFormat.DataType = "decimal";
            DrugEHBDeductibleCombinedInOutNetworkFamilyMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(DrugEHBDeductibleCombinedInOutNetworkFamilyMap);

            QhpToExcelMap CombinedMedicalEHBDeductibleDeductibleTypeMap = new QhpToExcelMap();
            CombinedMedicalEHBDeductibleDeductibleTypeMap.ParentPropertyName = "CombinedMedicalEHBDeductible";
            CombinedMedicalEHBDeductibleDeductibleTypeMap.FormPropertyName = "DeductibleDrugType";
            CombinedMedicalEHBDeductibleDeductibleTypeMap.DomainPropertyName = "DeductibleDrugType";
            CombinedMedicalEHBDeductibleDeductibleTypeMap.ColumnName = "BP";
            CombinedMedicalEHBDeductibleDeductibleTypeMap.RowIndex = 1;
            CombinedMedicalEHBDeductibleDeductibleTypeMap.IncrementStep = 10;
            CombinedMedicalEHBDeductibleDeductibleTypeMap.QhpSheetType = QHPSheetType.CostShareVariance;
            CombinedMedicalEHBDeductibleDeductibleTypeMap.CellFormat.CellAlignment = "string";
            CombinedMedicalEHBDeductibleDeductibleTypeMap.CellFormat.DataType = "Left";

            this.MappingsList.Add(CombinedMedicalEHBDeductibleDeductibleTypeMap);

            QhpToExcelMap CombinedMedicalEHBDeductibleInNetworkIndividualMap = new QhpToExcelMap();
            CombinedMedicalEHBDeductibleInNetworkIndividualMap.ParentPropertyName = "CombinedMedicalEHBDeductible";
            CombinedMedicalEHBDeductibleInNetworkIndividualMap.FormPropertyName = "InNetworkIndividual";
            CombinedMedicalEHBDeductibleInNetworkIndividualMap.DomainPropertyName = "InNetworkIndividual";
            CombinedMedicalEHBDeductibleInNetworkIndividualMap.ColumnName = "BP";
            CombinedMedicalEHBDeductibleInNetworkIndividualMap.RowIndex = 4;
            CombinedMedicalEHBDeductibleInNetworkIndividualMap.QhpSheetType = QHPSheetType.CostShareVariance;
            CombinedMedicalEHBDeductibleInNetworkIndividualMap.CellFormat.CellAlignment = "Right";
            CombinedMedicalEHBDeductibleInNetworkIndividualMap.CellFormat.DataType = "decimal";
            CombinedMedicalEHBDeductibleInNetworkIndividualMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(CombinedMedicalEHBDeductibleInNetworkIndividualMap);

            QhpToExcelMap CombinedMedicalEHBDeductibleInNetworkFamilyMap = new QhpToExcelMap();
            CombinedMedicalEHBDeductibleInNetworkFamilyMap.ParentPropertyName = "CombinedMedicalEHBDeductible";
            CombinedMedicalEHBDeductibleInNetworkFamilyMap.FormPropertyName = "InNetworkFamily";
            CombinedMedicalEHBDeductibleInNetworkFamilyMap.DomainPropertyName = "InNetworkFamily";
            CombinedMedicalEHBDeductibleInNetworkFamilyMap.ColumnName = "BQ";
            CombinedMedicalEHBDeductibleInNetworkFamilyMap.RowIndex = 4;
            CombinedMedicalEHBDeductibleInNetworkFamilyMap.QhpSheetType = QHPSheetType.CostShareVariance;
            CombinedMedicalEHBDeductibleInNetworkFamilyMap.CellFormat.CellAlignment = "Right";
            CombinedMedicalEHBDeductibleInNetworkFamilyMap.CellFormat.DataType = "decimal";
            CombinedMedicalEHBDeductibleInNetworkFamilyMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(CombinedMedicalEHBDeductibleInNetworkFamilyMap);

            QhpToExcelMap CombinedMedicalEHBDeductibleInNetworkDefaultCoinsuranceMap = new QhpToExcelMap();
            CombinedMedicalEHBDeductibleInNetworkDefaultCoinsuranceMap.ParentPropertyName = "CombinedMedicalEHBDeductible";
            CombinedMedicalEHBDeductibleInNetworkDefaultCoinsuranceMap.FormPropertyName = "InNetworkDefaultCoinsurance";
            CombinedMedicalEHBDeductibleInNetworkDefaultCoinsuranceMap.DomainPropertyName = "InNetworkDefaultCoinsurance";
            CombinedMedicalEHBDeductibleInNetworkDefaultCoinsuranceMap.ColumnName = "BR";
            CombinedMedicalEHBDeductibleInNetworkDefaultCoinsuranceMap.RowIndex = 4;
            CombinedMedicalEHBDeductibleInNetworkDefaultCoinsuranceMap.QhpSheetType = QHPSheetType.CostShareVariance;
            CombinedMedicalEHBDeductibleInNetworkDefaultCoinsuranceMap.CellFormat.CellAlignment = "Right";
            CombinedMedicalEHBDeductibleInNetworkDefaultCoinsuranceMap.CellFormat.DataType = "decimal";
            CombinedMedicalEHBDeductibleInNetworkDefaultCoinsuranceMap.CellFormat.CellPrefix = "%";

            this.MappingsList.Add(CombinedMedicalEHBDeductibleInNetworkDefaultCoinsuranceMap);

            QhpToExcelMap CombinedMedicalEHBDeductibleInNetworkTier2IndividualMap = new QhpToExcelMap();
            CombinedMedicalEHBDeductibleInNetworkTier2IndividualMap.ParentPropertyName = "CombinedMedicalEHBDeductible";
            CombinedMedicalEHBDeductibleInNetworkTier2IndividualMap.FormPropertyName = "InNetworkTier2Individual";
            CombinedMedicalEHBDeductibleInNetworkTier2IndividualMap.DomainPropertyName = "InNetworkTier2Individual";
            CombinedMedicalEHBDeductibleInNetworkTier2IndividualMap.ColumnName = "BS";
            CombinedMedicalEHBDeductibleInNetworkTier2IndividualMap.RowIndex = 4;
            CombinedMedicalEHBDeductibleInNetworkTier2IndividualMap.QhpSheetType = QHPSheetType.CostShareVariance;
            CombinedMedicalEHBDeductibleInNetworkTier2IndividualMap.CellFormat.CellAlignment = "Right";
            CombinedMedicalEHBDeductibleInNetworkTier2IndividualMap.CellFormat.DataType = "decimal";
            CombinedMedicalEHBDeductibleInNetworkTier2IndividualMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(CombinedMedicalEHBDeductibleInNetworkTier2IndividualMap);

            QhpToExcelMap CombinedMedicalEHBDeductibleInNetworkTier2FamilyMap = new QhpToExcelMap();
            CombinedMedicalEHBDeductibleInNetworkTier2FamilyMap.ParentPropertyName = "CombinedMedicalEHBDeductible";
            CombinedMedicalEHBDeductibleInNetworkTier2FamilyMap.FormPropertyName = "InNetworkTier2Family";
            CombinedMedicalEHBDeductibleInNetworkTier2FamilyMap.DomainPropertyName = "InNetworkTier2Family";
            CombinedMedicalEHBDeductibleInNetworkTier2FamilyMap.ColumnName = "BT";
            CombinedMedicalEHBDeductibleInNetworkTier2FamilyMap.RowIndex = 4;
            CombinedMedicalEHBDeductibleInNetworkTier2FamilyMap.QhpSheetType = QHPSheetType.CostShareVariance;
            CombinedMedicalEHBDeductibleInNetworkTier2FamilyMap.CellFormat.CellAlignment = "Right";
            CombinedMedicalEHBDeductibleInNetworkTier2FamilyMap.CellFormat.DataType = "decimal";
            CombinedMedicalEHBDeductibleInNetworkTier2FamilyMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(CombinedMedicalEHBDeductibleInNetworkTier2FamilyMap);

            QhpToExcelMap CombinedMedicalEHBDeductibleInNetworkTier2DefaultCoinsuranceMap = new QhpToExcelMap();
            CombinedMedicalEHBDeductibleInNetworkTier2DefaultCoinsuranceMap.ParentPropertyName = "CombinedMedicalEHBDeductible";
            CombinedMedicalEHBDeductibleInNetworkTier2DefaultCoinsuranceMap.FormPropertyName = "InNetworkTier2DefaultCoinsurance";
            CombinedMedicalEHBDeductibleInNetworkTier2DefaultCoinsuranceMap.DomainPropertyName = "InNetworkTier2DefaultCoinsurance";
            CombinedMedicalEHBDeductibleInNetworkTier2DefaultCoinsuranceMap.ColumnName = "BU";
            CombinedMedicalEHBDeductibleInNetworkTier2DefaultCoinsuranceMap.RowIndex = 4;
            CombinedMedicalEHBDeductibleInNetworkTier2DefaultCoinsuranceMap.QhpSheetType = QHPSheetType.CostShareVariance;
            CombinedMedicalEHBDeductibleInNetworkTier2DefaultCoinsuranceMap.CellFormat.CellAlignment = "Right";
            CombinedMedicalEHBDeductibleInNetworkTier2DefaultCoinsuranceMap.CellFormat.DataType = "decimal";
            CombinedMedicalEHBDeductibleInNetworkTier2DefaultCoinsuranceMap.CellFormat.CellPrefix = "%";

            this.MappingsList.Add(CombinedMedicalEHBDeductibleInNetworkTier2DefaultCoinsuranceMap);

            QhpToExcelMap CombinedMedicalEHBDeductibleOutOfNetworkIndividualMap = new QhpToExcelMap();
            CombinedMedicalEHBDeductibleOutOfNetworkIndividualMap.ParentPropertyName = "CombinedMedicalEHBDeductible";
            CombinedMedicalEHBDeductibleOutOfNetworkIndividualMap.FormPropertyName = "OutOfNetworkIndividual";
            CombinedMedicalEHBDeductibleOutOfNetworkIndividualMap.DomainPropertyName = "OutOfNetworkIndividual";
            CombinedMedicalEHBDeductibleOutOfNetworkIndividualMap.ColumnName = "BV";
            CombinedMedicalEHBDeductibleOutOfNetworkIndividualMap.RowIndex = 4;
            CombinedMedicalEHBDeductibleOutOfNetworkIndividualMap.QhpSheetType = QHPSheetType.CostShareVariance;
            CombinedMedicalEHBDeductibleOutOfNetworkIndividualMap.CellFormat.CellAlignment = "Right";
            CombinedMedicalEHBDeductibleOutOfNetworkIndividualMap.CellFormat.DataType = "decimal";
            CombinedMedicalEHBDeductibleOutOfNetworkIndividualMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(CombinedMedicalEHBDeductibleOutOfNetworkIndividualMap);

            QhpToExcelMap CombinedMedicalEHBDeductibleOutOfNetworkFamilyMap = new QhpToExcelMap();
            CombinedMedicalEHBDeductibleOutOfNetworkFamilyMap.ParentPropertyName = "CombinedMedicalEHBDeductible";
            CombinedMedicalEHBDeductibleOutOfNetworkFamilyMap.FormPropertyName = "OutOfNetworkFamily";
            CombinedMedicalEHBDeductibleOutOfNetworkFamilyMap.DomainPropertyName = "OutOfNetworkFamily";
            CombinedMedicalEHBDeductibleOutOfNetworkFamilyMap.ColumnName = "BW";
            CombinedMedicalEHBDeductibleOutOfNetworkFamilyMap.RowIndex = 4;
            CombinedMedicalEHBDeductibleOutOfNetworkFamilyMap.QhpSheetType = QHPSheetType.CostShareVariance;
            CombinedMedicalEHBDeductibleOutOfNetworkFamilyMap.CellFormat.CellAlignment = "Right";
            CombinedMedicalEHBDeductibleOutOfNetworkFamilyMap.CellFormat.DataType = "decimal";
            CombinedMedicalEHBDeductibleOutOfNetworkFamilyMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(CombinedMedicalEHBDeductibleOutOfNetworkFamilyMap);

            QhpToExcelMap CombinedMedicalEHBDeductibleCombinedInOutNetworkIndividualMap = new QhpToExcelMap();
            CombinedMedicalEHBDeductibleCombinedInOutNetworkIndividualMap.ParentPropertyName = "CombinedMedicalEHBDeductible";
            CombinedMedicalEHBDeductibleCombinedInOutNetworkIndividualMap.FormPropertyName = "CombinedInOutNetworkIndividual";
            CombinedMedicalEHBDeductibleCombinedInOutNetworkIndividualMap.DomainPropertyName = "CombinedInOutNetworkIndividual";
            CombinedMedicalEHBDeductibleCombinedInOutNetworkIndividualMap.ColumnName = "BX";
            CombinedMedicalEHBDeductibleCombinedInOutNetworkIndividualMap.RowIndex = 4;
            CombinedMedicalEHBDeductibleCombinedInOutNetworkIndividualMap.QhpSheetType = QHPSheetType.CostShareVariance;
            CombinedMedicalEHBDeductibleCombinedInOutNetworkIndividualMap.CellFormat.CellAlignment = "Right";
            CombinedMedicalEHBDeductibleCombinedInOutNetworkIndividualMap.CellFormat.DataType = "decimal";
            CombinedMedicalEHBDeductibleCombinedInOutNetworkIndividualMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(CombinedMedicalEHBDeductibleCombinedInOutNetworkIndividualMap);

            QhpToExcelMap CombinedMedicalEHBDeductibleCombinedInOutNetworkFamilyMap = new QhpToExcelMap();
            CombinedMedicalEHBDeductibleCombinedInOutNetworkFamilyMap.ParentPropertyName = "CombinedMedicalEHBDeductible";
            CombinedMedicalEHBDeductibleCombinedInOutNetworkFamilyMap.FormPropertyName = "CombinedInOutNetworkFamily";
            CombinedMedicalEHBDeductibleCombinedInOutNetworkFamilyMap.DomainPropertyName = "CombinedInOutNetworkFamily";
            CombinedMedicalEHBDeductibleCombinedInOutNetworkFamilyMap.ColumnName = "BY";
            CombinedMedicalEHBDeductibleCombinedInOutNetworkFamilyMap.RowIndex = 4;
            CombinedMedicalEHBDeductibleCombinedInOutNetworkFamilyMap.QhpSheetType = QHPSheetType.CostShareVariance;
            CombinedMedicalEHBDeductibleCombinedInOutNetworkFamilyMap.CellFormat.CellAlignment = "Right";
            CombinedMedicalEHBDeductibleCombinedInOutNetworkFamilyMap.CellFormat.DataType = "decimal";
            CombinedMedicalEHBDeductibleCombinedInOutNetworkFamilyMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(CombinedMedicalEHBDeductibleCombinedInOutNetworkFamilyMap);
        }

        private void BuildHSAHRADetail()
        {
             QhpToExcelMap HSAEligibleMap = new QhpToExcelMap();            
              HSAEligibleMap.ParentPropertyName = "HSAHRADetail";
              HSAEligibleMap.FormPropertyName = "HSAEligible";
              HSAEligibleMap.DomainPropertyName = "HSAEligible";
              HSAEligibleMap.ColumnName = "BZ";
              HSAEligibleMap.RowIndex = 4;
              HSAEligibleMap.QhpSheetType = QHPSheetType.CostShareVariance;
              HSAEligibleMap.CellFormat.CellAlignment = "Left";
              HSAEligibleMap.CellFormat.DataType = "String";          
    
              this.MappingsList.Add(HSAEligibleMap);
    
              QhpToExcelMap HSAHRAEmployerContributionMap = new QhpToExcelMap();
              HSAHRAEmployerContributionMap.ParentPropertyName = "HSAHRADetail";
              HSAHRAEmployerContributionMap.FormPropertyName = "HSAHRAEmployerContribution";
              HSAHRAEmployerContributionMap.DomainPropertyName = "HSAHRAEmployerContribution";
              HSAHRAEmployerContributionMap.ColumnName = "CA";
              HSAHRAEmployerContributionMap.RowIndex = 4;
              HSAHRAEmployerContributionMap.QhpSheetType = QHPSheetType.CostShareVariance;
              HSAHRAEmployerContributionMap.CellFormat.CellAlignment = "Left";
              HSAHRAEmployerContributionMap.CellFormat.DataType = "String";

              this.MappingsList.Add(HSAHRAEmployerContributionMap);
    
              QhpToExcelMap HSAHRAEmployerContributionAmountMap = new QhpToExcelMap();
              HSAHRAEmployerContributionAmountMap.ParentPropertyName = "HSAHRADetail";
              HSAHRAEmployerContributionAmountMap.FormPropertyName = "HSAHRAEmployerContributionAmount";
              HSAHRAEmployerContributionAmountMap.DomainPropertyName = "HSAHRAEmployerContributionAmount";
              HSAHRAEmployerContributionAmountMap.ColumnName = "CB";
              HSAHRAEmployerContributionAmountMap.RowIndex = 4;
              HSAHRAEmployerContributionAmountMap.QhpSheetType = QHPSheetType.CostShareVariance;
              HSAHRAEmployerContributionAmountMap.CellFormat.CellAlignment = "Left";
              HSAHRAEmployerContributionAmountMap.CellFormat.DataType = "String";
              this.MappingsList.Add(HSAHRAEmployerContributionAmountMap);
        }

        private void BuildPlanVariantLevelURLs()
        {
            QhpToExcelMap URLforSummaryofBenefitsCoverageMap = new QhpToExcelMap();
            URLforSummaryofBenefitsCoverageMap.ParentPropertyName = "PlanVariantLevelURLs";
            URLforSummaryofBenefitsCoverageMap.FormPropertyName = "URLforSummaryofBenefitsCoverage";
            URLforSummaryofBenefitsCoverageMap.DomainPropertyName = "URLforSummaryofBenefitsCoverage";
            URLforSummaryofBenefitsCoverageMap.ColumnName = "CC";
            URLforSummaryofBenefitsCoverageMap.RowIndex = 4;
            URLforSummaryofBenefitsCoverageMap.QhpSheetType = QHPSheetType.CostShareVariance;
            URLforSummaryofBenefitsCoverageMap.CellFormat.CellAlignment = "Left";
            URLforSummaryofBenefitsCoverageMap.CellFormat.DataType = "String";
            this.MappingsList.Add(URLforSummaryofBenefitsCoverageMap);

            QhpToExcelMap PlanBrochureMap = new QhpToExcelMap();
            PlanBrochureMap.ParentPropertyName = "PlanVariantLevelURLs";
            PlanBrochureMap.FormPropertyName = "PlanBrochure";
            PlanBrochureMap.DomainPropertyName = "PlanBrochure";
            PlanBrochureMap.ColumnName = "CD";
            PlanBrochureMap.RowIndex = 4;
            PlanBrochureMap.QhpSheetType = QHPSheetType.CostShareVariance;
            PlanBrochureMap.CellFormat.CellAlignment = "Left";
            PlanBrochureMap.CellFormat.DataType = "String";
            this.MappingsList.Add(PlanBrochureMap);
        }

        private void BuildAVCalculatorAttributes()
        {
            QhpToExcelMap MaximumCoinsuranceForSpecialityDrugsMap = new QhpToExcelMap();
            MaximumCoinsuranceForSpecialityDrugsMap.FormPropertyName = "MaximumCoinsuranceForSpecialityDrugs";
            MaximumCoinsuranceForSpecialityDrugsMap.ParentPropertyName = "AVCalculatorAdditionalBenefitDesign";
            MaximumCoinsuranceForSpecialityDrugsMap.DomainPropertyName = "MaximumCoinsuranceForSpecialityDrugs";
            MaximumCoinsuranceForSpecialityDrugsMap.ColumnName = "CE";
            MaximumCoinsuranceForSpecialityDrugsMap.RowIndex = 4;
            MaximumCoinsuranceForSpecialityDrugsMap.QhpSheetType = QHPSheetType.CostShareVariance;
            MaximumCoinsuranceForSpecialityDrugsMap.CellFormat.CellAlignment = "Right";
            MaximumCoinsuranceForSpecialityDrugsMap.CellFormat.DataType = "decimal";
            MaximumCoinsuranceForSpecialityDrugsMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(MaximumCoinsuranceForSpecialityDrugsMap);

            QhpToExcelMap MaximumNumberOfDaysForChargingInpatientCopayMap = new QhpToExcelMap();
            MaximumNumberOfDaysForChargingInpatientCopayMap.FormPropertyName = "MaximumNumberOfDaysForChargingInpatientCopay";
            MaximumNumberOfDaysForChargingInpatientCopayMap.DomainPropertyName = "MaximumNumberOfDaysForChargingInpatientCopay";
            MaximumNumberOfDaysForChargingInpatientCopayMap.ParentPropertyName = "AVCalculatorAdditionalBenefitDesign";
            MaximumNumberOfDaysForChargingInpatientCopayMap.ColumnName = "CF";
            MaximumNumberOfDaysForChargingInpatientCopayMap.RowIndex = 4;
            MaximumNumberOfDaysForChargingInpatientCopayMap.QhpSheetType = QHPSheetType.CostShareVariance;
            MaximumNumberOfDaysForChargingInpatientCopayMap.CellFormat.CellAlignment = "Right";
            MaximumNumberOfDaysForChargingInpatientCopayMap.CellFormat.DataType = "int";

            this.MappingsList.Add(MaximumNumberOfDaysForChargingInpatientCopayMap);

            QhpToExcelMap BeginPrimaryCostSharingAfterSetNumberOfVisitsMap = new QhpToExcelMap();
            BeginPrimaryCostSharingAfterSetNumberOfVisitsMap.FormPropertyName = "BeginPrimaryCostSharingAfterSetNumberOfVisits";
            BeginPrimaryCostSharingAfterSetNumberOfVisitsMap.DomainPropertyName = "BeginPrimaryCostSharingAfterSetNumberOfVisits";
            BeginPrimaryCostSharingAfterSetNumberOfVisitsMap.ParentPropertyName = "AVCalculatorAdditionalBenefitDesign";
            BeginPrimaryCostSharingAfterSetNumberOfVisitsMap.ColumnName = "CG";
            BeginPrimaryCostSharingAfterSetNumberOfVisitsMap.RowIndex = 4;
            BeginPrimaryCostSharingAfterSetNumberOfVisitsMap.QhpSheetType = QHPSheetType.CostShareVariance;
            BeginPrimaryCostSharingAfterSetNumberOfVisitsMap.CellFormat.CellAlignment = "Right";
            BeginPrimaryCostSharingAfterSetNumberOfVisitsMap.CellFormat.DataType = "int";

            this.MappingsList.Add(BeginPrimaryCostSharingAfterSetNumberOfVisitsMap);

            QhpToExcelMap BeginPrimaryCareDedCoAfterSetNumberOfCopaysMap = new QhpToExcelMap();
            BeginPrimaryCareDedCoAfterSetNumberOfCopaysMap.FormPropertyName = "BeginPrimaryCareDedCoAfterSetNumberOfCopays";
            BeginPrimaryCareDedCoAfterSetNumberOfCopaysMap.DomainPropertyName = "BeginPrimaryCareDedCoAfterSetNumberOfCopays";
            BeginPrimaryCareDedCoAfterSetNumberOfCopaysMap.ParentPropertyName = "AVCalculatorAdditionalBenefitDesign";
            BeginPrimaryCareDedCoAfterSetNumberOfCopaysMap.ColumnName = "CH";
            BeginPrimaryCareDedCoAfterSetNumberOfCopaysMap.RowIndex = 4;
            BeginPrimaryCareDedCoAfterSetNumberOfCopaysMap.QhpSheetType = QHPSheetType.CostShareVariance;
            BeginPrimaryCareDedCoAfterSetNumberOfCopaysMap.CellFormat.CellAlignment = "Right";
            BeginPrimaryCareDedCoAfterSetNumberOfCopaysMap.CellFormat.DataType = "int";

            this.MappingsList.Add(BeginPrimaryCareDedCoAfterSetNumberOfCopaysMap);
        }

        private void BuildDeductibleSubGroupAttributes()
        {
            //QhpToExcelMap DeductibleSubGroupGroupNameMap = new QhpToExcelMap();
            //DeductibleSubGroupGroupNameMap.ParentPropertyName = "DeductibleSubGroups";
            //DeductibleSubGroupGroupNameMap.FormPropertyName = "GroupName";
            //DeductibleSubGroupGroupNameMap.DomainPropertyName = "GroupName";
            //DeductibleSubGroupGroupNameMap.ColumnName = "BV";
            //DeductibleSubGroupGroupNameMap.RowIndex = 1;
            //DeductibleSubGroupGroupNameMap.IncrementStep = 8;
            //DeductibleSubGroupGroupNameMap.IsHeader = true;
            //DeductibleSubGroupGroupNameMap.QhpSheetType = QHPSheetType.CostShareVariance;
            //DeductibleSubGroupGroupNameMap.CellFormat.CellAlignment = "Left";
            //DeductibleSubGroupGroupNameMap.CellFormat.DataType = "string";

            //this.MappingsList.Add(DeductibleSubGroupGroupNameMap);

            QhpToExcelMap DeductibleSubGroupInNetworkIndividualMap = new QhpToExcelMap();
            DeductibleSubGroupInNetworkIndividualMap.ParentPropertyName = "DeductibleSubGroups";
            DeductibleSubGroupInNetworkIndividualMap.FormPropertyName = "InNetworkIndividual";
            DeductibleSubGroupInNetworkIndividualMap.DomainPropertyName = "InNetworkIndividual";
            DeductibleSubGroupInNetworkIndividualMap.ColumnName = "CL";
            DeductibleSubGroupInNetworkIndividualMap.RowIndex = 4;
            DeductibleSubGroupInNetworkIndividualMap.QhpSheetType = QHPSheetType.CostShareVariance;
            DeductibleSubGroupInNetworkIndividualMap.CellFormat.CellAlignment = "Right";
            DeductibleSubGroupInNetworkIndividualMap.CellFormat.DataType = "decimal";
            DeductibleSubGroupInNetworkIndividualMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(DeductibleSubGroupInNetworkIndividualMap);

            QhpToExcelMap DeductibleSubGroupInNetworkFamilyMap = new QhpToExcelMap();
            DeductibleSubGroupInNetworkFamilyMap.ParentPropertyName = "DeductibleSubGroups";
            DeductibleSubGroupInNetworkFamilyMap.FormPropertyName = "InNetworkFamily";
            DeductibleSubGroupInNetworkFamilyMap.DomainPropertyName = "InNetworkFamily";
            DeductibleSubGroupInNetworkFamilyMap.ColumnName = "CM";
            DeductibleSubGroupInNetworkFamilyMap.RowIndex = 4;
            DeductibleSubGroupInNetworkFamilyMap.QhpSheetType = QHPSheetType.CostShareVariance;
            DeductibleSubGroupInNetworkFamilyMap.CellFormat.CellAlignment = "Right";
            DeductibleSubGroupInNetworkFamilyMap.CellFormat.DataType = "decimal";
            DeductibleSubGroupInNetworkFamilyMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(DeductibleSubGroupInNetworkFamilyMap);

            QhpToExcelMap DeductibleSubGroupInNetworkTier2IndividualMap = new QhpToExcelMap();
            DeductibleSubGroupInNetworkTier2IndividualMap.ParentPropertyName = "DeductibleSubGroups";
            DeductibleSubGroupInNetworkTier2IndividualMap.FormPropertyName = "InNetworkTier2Individual";
            DeductibleSubGroupInNetworkTier2IndividualMap.DomainPropertyName = "InNetworkTier2Individual";
            DeductibleSubGroupInNetworkTier2IndividualMap.ColumnName = "CN";
            DeductibleSubGroupInNetworkTier2IndividualMap.RowIndex = 4;
            DeductibleSubGroupInNetworkTier2IndividualMap.QhpSheetType = QHPSheetType.CostShareVariance;
            DeductibleSubGroupInNetworkTier2IndividualMap.CellFormat.CellAlignment = "Right";
            DeductibleSubGroupInNetworkTier2IndividualMap.CellFormat.DataType = "decimal";
            DeductibleSubGroupInNetworkTier2IndividualMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(DeductibleSubGroupInNetworkTier2IndividualMap);

            QhpToExcelMap DeductibleSubGroupInNetworkTier2FamilyMap = new QhpToExcelMap();
            DeductibleSubGroupInNetworkTier2FamilyMap.ParentPropertyName = "DeductibleSubGroups";
            DeductibleSubGroupInNetworkTier2FamilyMap.FormPropertyName = "InNetworkTier2Family";
            DeductibleSubGroupInNetworkTier2FamilyMap.DomainPropertyName = "InNetworkTier2Family";
            DeductibleSubGroupInNetworkTier2FamilyMap.ColumnName = "CO";
            DeductibleSubGroupInNetworkTier2FamilyMap.RowIndex = 4;
            DeductibleSubGroupInNetworkTier2FamilyMap.QhpSheetType = QHPSheetType.CostShareVariance;
            DeductibleSubGroupInNetworkTier2FamilyMap.CellFormat.CellAlignment = "Right";
            DeductibleSubGroupInNetworkTier2FamilyMap.CellFormat.DataType = "decimal";
            DeductibleSubGroupInNetworkTier2FamilyMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(DeductibleSubGroupInNetworkTier2FamilyMap);

            QhpToExcelMap DeductibleSubGroupOutOfNetworkIndividualMap = new QhpToExcelMap();
            DeductibleSubGroupOutOfNetworkIndividualMap.ParentPropertyName = "DeductibleSubGroups";
            DeductibleSubGroupOutOfNetworkIndividualMap.FormPropertyName = "OutOfNetworkIndividual";
            DeductibleSubGroupOutOfNetworkIndividualMap.DomainPropertyName = "OutOfNetworkIndividual";
            DeductibleSubGroupOutOfNetworkIndividualMap.ColumnName = "CP";
            DeductibleSubGroupOutOfNetworkIndividualMap.RowIndex = 4;
            DeductibleSubGroupOutOfNetworkIndividualMap.QhpSheetType = QHPSheetType.CostShareVariance;
            DeductibleSubGroupOutOfNetworkIndividualMap.CellFormat.CellAlignment = "Right";
            DeductibleSubGroupOutOfNetworkIndividualMap.CellFormat.DataType = "decimal";
            DeductibleSubGroupOutOfNetworkIndividualMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(DeductibleSubGroupOutOfNetworkIndividualMap);

            QhpToExcelMap DeductibleSubGroupOutOfNetworkFamilyMap = new QhpToExcelMap();
            DeductibleSubGroupOutOfNetworkFamilyMap.ParentPropertyName = "DeductibleSubGroups";
            DeductibleSubGroupOutOfNetworkFamilyMap.FormPropertyName = "OutOfNetworkFamily";
            DeductibleSubGroupOutOfNetworkFamilyMap.DomainPropertyName = "OutOfNetworkFamily";
            DeductibleSubGroupOutOfNetworkFamilyMap.ColumnName = "CQ";
            DeductibleSubGroupOutOfNetworkFamilyMap.RowIndex = 4;
            DeductibleSubGroupOutOfNetworkFamilyMap.QhpSheetType = QHPSheetType.CostShareVariance;
            DeductibleSubGroupOutOfNetworkFamilyMap.CellFormat.CellAlignment = "Right";
            DeductibleSubGroupOutOfNetworkFamilyMap.CellFormat.DataType = "decimal";
            DeductibleSubGroupOutOfNetworkFamilyMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(DeductibleSubGroupOutOfNetworkFamilyMap);

            QhpToExcelMap DeductibleSubGroupCombinedInOutNetworkIndividualMap = new QhpToExcelMap();
            DeductibleSubGroupCombinedInOutNetworkIndividualMap.ParentPropertyName = "DeductibleSubGroups";
            DeductibleSubGroupCombinedInOutNetworkIndividualMap.FormPropertyName = "CombinedInOutNetworkIndividual";
            DeductibleSubGroupCombinedInOutNetworkIndividualMap.DomainPropertyName = "CombinedInOutNetworkIndividual";
            DeductibleSubGroupCombinedInOutNetworkIndividualMap.ColumnName = "CR";
            DeductibleSubGroupCombinedInOutNetworkIndividualMap.RowIndex = 4;
            DeductibleSubGroupCombinedInOutNetworkIndividualMap.QhpSheetType = QHPSheetType.CostShareVariance;
            DeductibleSubGroupCombinedInOutNetworkIndividualMap.CellFormat.CellAlignment = "Right";
            DeductibleSubGroupCombinedInOutNetworkIndividualMap.CellFormat.DataType = "decimal";
            DeductibleSubGroupCombinedInOutNetworkIndividualMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(DeductibleSubGroupCombinedInOutNetworkIndividualMap);

            QhpToExcelMap DeductibleSubGroupCombinedInOutNetworkFamilyMap = new QhpToExcelMap();
            DeductibleSubGroupCombinedInOutNetworkFamilyMap.ParentPropertyName = "DeductibleSubGroups";
            DeductibleSubGroupCombinedInOutNetworkFamilyMap.FormPropertyName = "CombinedInOutNetworkFamily";
            DeductibleSubGroupCombinedInOutNetworkFamilyMap.DomainPropertyName = "CombinedInOutNetworkFamily";
            DeductibleSubGroupCombinedInOutNetworkFamilyMap.ColumnName = "CS";
            DeductibleSubGroupCombinedInOutNetworkFamilyMap.RowIndex = 4;
            DeductibleSubGroupCombinedInOutNetworkFamilyMap.QhpSheetType = QHPSheetType.CostShareVariance;
            DeductibleSubGroupCombinedInOutNetworkFamilyMap.CellFormat.CellAlignment = "Right";
            DeductibleSubGroupCombinedInOutNetworkFamilyMap.CellFormat.DataType = "decimal";
            DeductibleSubGroupCombinedInOutNetworkFamilyMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(DeductibleSubGroupCombinedInOutNetworkFamilyMap);
        }
    
        private void BuildBenefitPlanDetailsAttributes()
        {
            QhpToExcelMap ServiceNameMap = new QhpToExcelMap();
            ServiceNameMap.FormPropertyName = "Benefit";
            ServiceNameMap.DomainPropertyName = "ServiceName";
            ServiceNameMap.ColumnName = "DJ";
            ServiceNameMap.RowIndex = 1;
            ServiceNameMap.IncrementStep = 6;
            ServiceNameMap.IsHeader = true;
            ServiceNameMap.QhpSheetType = QHPSheetType.CostShareVariance;
            ServiceNameMap.CellFormat.CellAlignment = "Left";
            ServiceNameMap.CellFormat.DataType = "string";

            this.MappingsList.Add(ServiceNameMap);

            QhpToExcelMap InNetworkCopayMap = new QhpToExcelMap();
            InNetworkCopayMap.ParentPropertyName = "CostSharingBenefitServices";
            InNetworkCopayMap.FormPropertyName = "InNetworkCopay";
            InNetworkCopayMap.DomainPropertyName = "InNetworkCopay";
            InNetworkCopayMap.ColumnName = "DJ";
            InNetworkCopayMap.RowIndex = 4;
            InNetworkCopayMap.QhpSheetType = QHPSheetType.CostShareVariance;
            InNetworkCopayMap.CellFormat.CellAlignment = "Right";
            InNetworkCopayMap.CellFormat.DataType = "decimal";
            InNetworkCopayMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(InNetworkCopayMap);

            QhpToExcelMap InNetworkTier2CopayMap = new QhpToExcelMap();
            InNetworkTier2CopayMap.ParentPropertyName = "CostSharingBenefitServices";
            InNetworkTier2CopayMap.FormPropertyName = "InNetworkTier2Copay";
            InNetworkTier2CopayMap.DomainPropertyName = "InNetworkTier2Copay";
            InNetworkTier2CopayMap.ColumnName = "DK";
            InNetworkTier2CopayMap.RowIndex = 4;
            InNetworkTier2CopayMap.QhpSheetType = QHPSheetType.CostShareVariance;
            InNetworkTier2CopayMap.CellFormat.CellAlignment = "Right";
            InNetworkTier2CopayMap.CellFormat.DataType = "decimal";
            InNetworkTier2CopayMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(InNetworkTier2CopayMap);

            QhpToExcelMap OutOfNetworkCopayMap = new QhpToExcelMap();
            OutOfNetworkCopayMap.ParentPropertyName = "CostSharingBenefitServices";
            OutOfNetworkCopayMap.FormPropertyName = "OutOfNetworkCopay";
            OutOfNetworkCopayMap.DomainPropertyName = "OutOfNetworkCopay";
            OutOfNetworkCopayMap.ColumnName = "DL";
            OutOfNetworkCopayMap.RowIndex = 4;
            OutOfNetworkCopayMap.QhpSheetType = QHPSheetType.CostShareVariance;
            OutOfNetworkCopayMap.CellFormat.CellAlignment = "Right";
            OutOfNetworkCopayMap.CellFormat.DataType = "decimal";
            OutOfNetworkCopayMap.CellFormat.CellPrefix = "$";

            this.MappingsList.Add(OutOfNetworkCopayMap);


            QhpToExcelMap InNetworkCoinsuranceMap = new QhpToExcelMap();
            InNetworkCoinsuranceMap.ParentPropertyName = "CostSharingBenefitServices";
            InNetworkCoinsuranceMap.FormPropertyName = "InNetworkCoinsurance";
            InNetworkCoinsuranceMap.DomainPropertyName = "InNetworkCoinsurance";
            InNetworkCoinsuranceMap.ColumnName = "DM";
            InNetworkCoinsuranceMap.RowIndex = 4;
            InNetworkCoinsuranceMap.QhpSheetType = QHPSheetType.CostShareVariance;
            InNetworkCoinsuranceMap.CellFormat.CellAlignment = "Right";
            InNetworkCoinsuranceMap.CellFormat.DataType = "decimal";
            InNetworkCoinsuranceMap.CellFormat.CellPrefix = "%";

            this.MappingsList.Add(InNetworkCoinsuranceMap);

            QhpToExcelMap InNetworkTier2CoinsuranceMap = new QhpToExcelMap();
            InNetworkTier2CoinsuranceMap.ParentPropertyName = "CostSharingBenefitServices";
            InNetworkTier2CoinsuranceMap.FormPropertyName = "InNetworkTier2Coinsurance";
            InNetworkTier2CoinsuranceMap.DomainPropertyName = "InNetworkTier2Coinsurance";
            InNetworkTier2CoinsuranceMap.ColumnName = "DN";
            InNetworkTier2CoinsuranceMap.RowIndex = 4;
            InNetworkTier2CoinsuranceMap.QhpSheetType = QHPSheetType.CostShareVariance;
            InNetworkTier2CoinsuranceMap.CellFormat.CellAlignment = "Right";
            InNetworkTier2CoinsuranceMap.CellFormat.DataType = "decimal";
            InNetworkTier2CoinsuranceMap.CellFormat.CellPrefix = "%";

            this.MappingsList.Add(InNetworkTier2CoinsuranceMap);

            QhpToExcelMap OutOfNetworkCoinsuranceMap = new QhpToExcelMap();
            OutOfNetworkCoinsuranceMap.ParentPropertyName = "CostSharingBenefitServices";
            OutOfNetworkCoinsuranceMap.FormPropertyName = "OutOfNetworkCoinsurance";
            OutOfNetworkCoinsuranceMap.DomainPropertyName = "OutOfNetworkCoinsurance";
            OutOfNetworkCoinsuranceMap.ColumnName = "DO";
            OutOfNetworkCoinsuranceMap.RowIndex = 4;
            OutOfNetworkCoinsuranceMap.QhpSheetType = QHPSheetType.CostShareVariance;
            OutOfNetworkCoinsuranceMap.CellFormat.CellAlignment = "Right";
            OutOfNetworkCoinsuranceMap.CellFormat.DataType = "decimal";
            OutOfNetworkCoinsuranceMap.CellFormat.CellPrefix = "%";

            this.MappingsList.Add(OutOfNetworkCoinsuranceMap);
        }
        #endregion Private Methods
    }
}
