using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.integration.qhplite.Ver2016.QHPModel;

namespace tmg.equinox.integration.qhplite.Ver2016.DocumentExporter
{
    public class PlanBenefitPackageMapper
    {
        #region Private Memebers

        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor
        public PlanBenefitPackageMapper()
        {

        }
        #endregion Constructor

        #region Public Methods
        public PlanBenefitPackage GetBenefitPackage(string documenName, string documentData)
        {
            PlanBenefitPackage package = new PlanBenefitPackage();
            dynamic document = JsonConvert.DeserializeObject(documentData);
            this.GenerateBenefitPackage(ref package, document);
            return package;
        }
        #endregion Public Methods

        #region Private Methods
        private void GenerateBenefitPackage(ref PlanBenefitPackage package, dynamic document)
        {
            GenerateGeneralInformation(ref package, document);
            GeneratePlanIdentifiers(ref package, document);
            GenerateBenefitInformation(ref package, document);
            GeneratePlanCostSharingAttributes(ref package, document);
            //GenerateSectionIGeneralProductandPlanInformation(ref package, document);
            //GenerateSectionIIComponentsofPremiumIncreasePMPMDollarAmountaboveCurrentAverag(ref package, document);
            //GenrateSectionIIIExperiencePeriodInformation(ref package, document);
            //GenrateSectionIVProjected12monthsfollowingeffectivedate(ref package, document);
        }

        private void GenerateGeneralInformation(ref PlanBenefitPackage package, dynamic document)
        {
            try
            {
                package.HIOSIssuerID = document.GeneralInformation.PlanInformation.HIOSIssuerID;
                package.IssuerState = document.GeneralInformation.PlanInformation.IssuerState;
                package.MarketCoverage = document.GeneralInformation.PlanInformation.MarketCoverage;
                package.DentalOnlyPlan = document.GeneralInformation.PlanInformation.DentalOnlyPlan;
                package.TIN = document.GeneralInformation.PlanInformation.TIN;
            }
            catch (Exception ex)
            {
                string customMsg = "An error occurred while processing Section => General Information";
                Exception customException = new Exception(customMsg, ex);
                ExceptionPolicyWrapper.HandleException(customException, ExceptionPolicies.ExceptionShielding);
            }
        }

        private void GeneratePlanIdentifiers(ref PlanBenefitPackage package, dynamic document)
        {
            try
            {
                package.PlanIdentifiers = new List<PlanIdentifier>();
                foreach (var item in document.ProductDetails.PlanIdentifiers.PlanIdentifierList)
                {
                    PlanIdentifier planIdentifier = new PlanIdentifier();
                    planIdentifier.HIOSPlanID = item.HIOSPlanIDStandardComponent;
                    planIdentifier.PlanMarketingName = item.PlanMarketingName;
                    planIdentifier.HIOSProductID = item.HIOSProductID;
                    planIdentifier.HPID = item.HPID;
                    planIdentifier.NetworkID = item.NetworkID;
                    planIdentifier.ServiceAreaID = item.ServiceAreaID;
                    planIdentifier.FormularyID = item.FormularyID;

                    //Fill Plan attributes
                    GeneratePlanAttributes(ref planIdentifier, document);
                    //Fill Plan StandardAloneDentalOnly
                    GenerateStandardAloneDentalOnly(ref planIdentifier, document);
                    //Fill PlanDates
                    GeneratePlanDates(ref planIdentifier, document);
                    //Fill Geographical Information
                    GenerateGeographicCoverage(ref planIdentifier, document);
                    //Fill AdditionalInformation
                    GenerateAdditionalInformation(ref planIdentifier, document);

                    package.PlanIdentifiers.Add(planIdentifier);
                }
            }
            catch (Exception ex)
            {
                string customMsg = "An error occurred while processing Section => Plan Identifier";
                Exception customException = new Exception(customMsg, ex);
                ExceptionPolicyWrapper.HandleException(customException, ExceptionPolicies.ExceptionShielding);
            }
        }

        private void GeneratePlanCostSharingAttributes(ref PlanBenefitPackage package, dynamic document)
        {
            try
            {
                package.PlanCostSharingAttributes = new List<PlanCostSharingAttributes>();
                foreach (var item in document.CostShareVariance.PlanCostSharingAttributes)
                {
                    PlanCostSharingAttributes attribute = new PlanCostSharingAttributes();

                    attribute.HIOSPlanIDComponentAndVariant = item.HIOSPlanIDStandardComponentVariant;
                    attribute.PlanMarketingName = item.PlanMarketingName;
                    attribute.LevelOfCoverage = item.LevelofCoverageMetalLevel;
                    attribute.CSRVariationType = item.CSRVariationType;
                    attribute.IssuerActuarialValue = item.IssuerActuarialValue;
                    attribute.AVCalculatorOutputNumber = item.AvCalculatorOutputNumber;
                    attribute.MedicalAndDrugDeductiblesIntegrated = item.MedicalAndDrugDeductiblesIntegrated;
                    attribute.MedicalAndDrugOutOfPocketIntegrated = item.MedicalAndDrugMaximumOutofPocketIntegrated;
                    attribute.MultipleInNetworkTiers = item.MultipleInNetworkTiers;
                    attribute.FirstTierUtilization = item.FirstTierUtilization;
                    attribute.SecondTierUtilization = item.SecondTierUtilization;

                    //Generate SBC Scenarios
                    GenerateSBCScenario(ref attribute, document);

                    //Generate Drug Deductibles
                    GenerateDrugDeductibles(ref attribute, document);

                    //Generate Drug Deductibles
                    GenerateDeductibleSubGroups(ref attribute, document);

                    //Generate Plan Maximum Out of Pockets
                    GenerateMaximumOutofPockets(ref attribute, document);
                    //Generate Plan Maximum Out of Pockets
                    GenerateHSAHRADetail(ref attribute, document);
                    //Generate Plan Maximum Out of Pockets
                    GeneratePlanVariantLevelURLs(ref attribute, document);
                    //Fill Plan AVCalculatorAdditionalBenefitDesign
                    GenerateAVCalculatorAdditionalBenefitDesign(ref attribute, document);

                    //Generate Plan Benefit Information Details
                    GeneratePlanBenefitDetails(ref attribute, document);

                    package.PlanCostSharingAttributes.Add(attribute);
                }
            }
            catch (Exception ex)
            {
                string customMsg = "An error occurred while processing Section => Plan Cost Sharing Attributes";
                Exception customException = new Exception(customMsg, ex);
                ExceptionPolicyWrapper.HandleException(customException, ExceptionPolicies.ExceptionShielding);
            }
        }

        #region PlanIdentifiers Information
        private void GeneratePlanAttributes(ref PlanIdentifier planIdentifier, dynamic document)
        {
            try
            {
                planIdentifier.PlanAttributes = new PlanAttributes();
                foreach (var item in document.ProductDetails.PlanAttributes.PlanAttributesList)
                {
                    if (item.HIOSPlanIDStandardComponent == planIdentifier.HIOSPlanID)
                    {
                        planIdentifier.HIOSPlanID = item.HIOSPlanIDStandardComponent;
                        planIdentifier.PlanMarketingName = item.PlanMarketingName;
                        planIdentifier.PlanAttributes.NewExistingPlan = item.NewExistingPlan;
                        planIdentifier.PlanAttributes.PlanType = item.PlanType;
                        planIdentifier.PlanAttributes.LevelOfCoverage = item.LevelofCoverage;
                        planIdentifier.PlanAttributes.DesignType = item.DesignType;
                        planIdentifier.PlanAttributes.UniquePlanDesign = item.UniquePlanDesign;
                        planIdentifier.PlanAttributes.QHPNonQHP = item.QHPNonQHP;
                        planIdentifier.PlanAttributes.NoticeRequiredForPregnancy = item.NoticePeriodforPregnancy;
                        planIdentifier.PlanAttributes.IsAReferralRequiredForSpecialist = item.IsReferralRequiredforSpecialist;
                        planIdentifier.PlanAttributes.SpecialistsRequiringAReferral = item.SpecialistsRequiringaReferral;
                        planIdentifier.PlanAttributes.PlanLevelExclusions = item.PlanLevelExclusions;
                        planIdentifier.PlanAttributes.LimitedCostSharingPlanVariation = item.LimitedCostSharingPlanVariationEstAdvancedPayment;
                        planIdentifier.PlanAttributes.DoesthisplanofferCompositeRating = item.DoesthisplanofferCompositeRating;
                        //planIdentifier.PlanAttributes.HSAEligible = item.HSAEligible;
                        //planIdentifier.PlanAttributes.HSAHRAEmployerContribution = item.HSAHRAEmployerContribution;
                        //planIdentifier.PlanAttributes.HSAHRAEmployerContributionAmount = item.HSAHRAEmployerContributionAmount;
                        planIdentifier.PlanAttributes.ChildOnlyOffering = item.ChildOnlyOffering;
                        planIdentifier.PlanAttributes.ChildOnlyPlanID = item.ChildOnlyPlanID;
                        planIdentifier.PlanAttributes.TobaccoWellnessProgramOffered = item.TobaccoWellnessProgramOffered;
                        if (item.DiseaseManagementProgramsOffered != null)
                        {
                            string dmpo = Convert.ToString(item.DiseaseManagementProgramsOffered).Replace("[", "").Replace("]", "").Replace("\"", "");
                            planIdentifier.PlanAttributes.DiseaseManagementProgramsOffered = dmpo;
                        }
                        planIdentifier.PlanAttributes.EHBPercentofTotalPremium = item.EHBPercentofTotalPremium;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                string customMsg = "An error occurred while processing Section => Plan Attributes";
                Exception customException = new Exception(customMsg, ex);
                ExceptionPolicyWrapper.HandleException(customException, ExceptionPolicies.ExceptionShielding);
            }
        }

        private void GenerateStandardAloneDentalOnly(ref PlanIdentifier planIdentifier, dynamic document)
        {
            try
            {
                foreach (var item in document.ProductDetails.StandardAloneDentalOnly.StandardAloneDentalOnlyList)
                {
                    if (item.HIOSPlanIDStandardComponent == planIdentifier.HIOSPlanID)
                    {
                        planIdentifier.StandAloneDentalOnly = new StandAloneDentalOnly();
                        planIdentifier.StandAloneDentalOnly.EHBApportionmentForPediatricDental = item.EHBApportionmentforPediatricDental;
                        planIdentifier.StandAloneDentalOnly.GuaranteedVsEstimatedRate = item.GuaranteedVsEstimatedRate;

                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                string customMsg = "An error occurred while processing Section => Stand Alone Dental Only";
                Exception customException = new Exception(customMsg, ex);
                ExceptionPolicyWrapper.HandleException(customException, ExceptionPolicies.ExceptionShielding);
            }
        }

        private void GenerateAVCalculatorAdditionalBenefitDesign(ref PlanCostSharingAttributes attribute, dynamic document)
        {
            try
            {
                attribute.AVCalculatorAdditionalBenefitDesign = new AVCalculatorAdditionalBenefitDesign();

                foreach (var item in document.ProductDetails.AVCalculatorAdditionalBenefitDesign.AVCalculatorAdditionalBenefitDesignList)
                {
                    if (item.HIOSPlanIDStandardComponent == attribute.HIOSPlanIDComponentAndVariant)
                    {
                        attribute.AVCalculatorAdditionalBenefitDesign.MaximumCoinsuranceForSpecialityDrugs = item.MaximumCoinsuranceforSpecialtyDrugs;
                        attribute.AVCalculatorAdditionalBenefitDesign.MaximumNumberOfDaysForChargingInpatientCopay = item.MaximumNumberofDaysforCharginganInpatientCopay;
                        attribute.AVCalculatorAdditionalBenefitDesign.BeginPrimaryCostSharingAfterSetNumberOfVisits = item.BeginPrimaryCareCostSharingAfteraSetNumberofVisits;
                        attribute.AVCalculatorAdditionalBenefitDesign.BeginPrimaryCareDedCoAfterSetNumberOfCopays = item.BeginPrimaryCareDeductibleCoinsuranceAfteraSetNumberofCopays;

                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                string customMsg = "An error occurred while processing Section => AVCalculator Additional Benefit Design";
                Exception customException = new Exception(customMsg, ex);
                ExceptionPolicyWrapper.HandleException(customException, ExceptionPolicies.ExceptionShielding);
            }
        }

        private void GeneratePlanDates(ref PlanIdentifier planIdentifier, dynamic document)
        {
            try
            {
                planIdentifier.PlanDates = new PlanDates();

                foreach (var item in document.ProductDetails.PlanDates.PlanDatesList)
                {
                    if (item.HIOSPlanIDStandardComponent == planIdentifier.HIOSPlanID)
                    {
                        planIdentifier.PlanDates.PlanEffectiveDate = item.PlanEffectiveDate;
                        planIdentifier.PlanDates.PlanExpirationDate = item.PlanExpirationDate;

                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                string customMsg = "An error occurred while processing Section => Plan Dates";
                Exception customException = new Exception(customMsg, ex);
                ExceptionPolicyWrapper.HandleException(customException, ExceptionPolicies.ExceptionShielding);
            }
        }

        private void GenerateGeographicCoverage(ref PlanIdentifier planIdentifier, dynamic document)
        {
            try
            {
                planIdentifier.GeographicCoverage = new GeographicCoverage();

                foreach (var item in document.ProductDetails.GeographicCoverage.GeographicCoverageList)
                {
                    if (item.HIOSPlanIDStandardComponent == planIdentifier.HIOSPlanID)
                    {
                        planIdentifier.GeographicCoverage.OutOfCountryCoverage = item.OutofCountryCoverage;
                        planIdentifier.GeographicCoverage.OutOfCountryCoverageDescription = item.OutofCountryCoverageDescription;
                        planIdentifier.GeographicCoverage.OutOfServiceAreaCoverage = item.OutofServiceAreaCoverage;
                        planIdentifier.GeographicCoverage.OutOfServiceAreaCoverageDescription = item.OutofServiceAreaCoverageDescription;
                        planIdentifier.GeographicCoverage.NationalNetwork = item.NationalNetwork;

                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                string customMsg = "An error occurred while processing Section => Geographic Coverage";
                Exception customException = new Exception(customMsg, ex);
                ExceptionPolicyWrapper.HandleException(customException, ExceptionPolicies.ExceptionShielding);
            }
        }

        private void GenerateAdditionalInformation(ref PlanIdentifier planIdentifier, dynamic document)
        {
            try
            {
                planIdentifier.PlanLevelURLs = new PlanLevelURLs();
                foreach (var item in document.ProductDetails.AdditionalInformation.PlanLevelURLs)
                {
                    if (item.HIOSPlanIDStandardComponent == planIdentifier.HIOSPlanID)
                    {
                        //planIdentifier.URLs.URLForSummaryOfBenefitsAndCoverage = item.URLforSummaryofBenefitsCoverage;
                        planIdentifier.PlanLevelURLs.URLforEnrollmentPayment = item.URLforEnrollmentPayment;
                        //planIdentifier.URLs.PlanBrochure = item.PlanBrochure;

                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                string customMsg = "An error occurred while processing Section => Additional Information Plan Level URLs";
                Exception customException = new Exception(customMsg, ex);
                ExceptionPolicyWrapper.HandleException(customException, ExceptionPolicies.ExceptionShielding);
            }
        }
        #endregion Product Details Information

        #region Generate Benefit Information
        private void GenerateBenefitInformation(ref PlanBenefitPackage package, dynamic document)
        {
            try
            {
                package.Benefits = new List<Benefit>();
                foreach (var item in document.BenefitInformation.BenefitList)
                {
                    Benefit benefit = new Benefit();
                    benefit.BenefitInformation = new BenefitInformation();
                    benefit.BenefitInformation.Benefit = item.Benefits;
                    benefit.BenefitInformation.EHB = item.EHB;
                    //benefit.BenefitInformation.StateRequiredBenefit = item.StateRequiredBenefit;

                    //Fill Benefit General Information
                    GenerateGeneralInformation(ref benefit, document);

                    package.Benefits.Add(benefit);
                }
            }
            catch (Exception ex)
            {
                string customMsg = "An error occurred while processing Section => Benefit Information";
                Exception customException = new Exception(customMsg, ex);
                ExceptionPolicyWrapper.HandleException(customException, ExceptionPolicies.ExceptionShielding);
            }
        }
        private void GenerateGeneralInformation(ref Benefit benefit, dynamic document)
        {
            try
            {
                foreach (var item in document.BenefitInformation.GeneralInformation)
                {
                    benefit.BenefitInformation.GeneralInformation = new GeneralInformation();

                    if (item.Benefits == benefit.BenefitInformation.Benefit)
                    {
                        benefit.BenefitInformation.GeneralInformation.IsThisBenefitCovered = item.IsCovered == "Yes" || item.IsCovered == "Covered" ? "Covered" : "Not Covered";
                        benefit.BenefitInformation.GeneralInformation.QuantativeLimitOfService = item.QuantitativeLimitonService == "Yes" ? item.QuantitativeLimitonService : string.Empty;
                        benefit.BenefitInformation.GeneralInformation.LimitQuantity = item.LimitQuantity;
                        benefit.BenefitInformation.GeneralInformation.LimitUnit = item.LimitUnit;
                        //benefit.BenefitInformation.GeneralInformation.MinimumStay = item.MinimumStay;
                        benefit.BenefitInformation.GeneralInformation.Exclusions = item.Exclusions;
                        benefit.BenefitInformation.GeneralInformation.BenefitExplanation = item.BenefitExplanation;
                        benefit.BenefitInformation.GeneralInformation.EHBVarianceReason = item.EHBVarianceReason;

                        benefit.BenefitInformation.DeductibleAndOutOfPocketExceptions = new OutOfPocketExceptions();
                        //benefit.BenefitInformation.DeductibleAndOutOfPocketExceptions.SubjectToDeductibleTier1 = (item.IsCovered == "Yes") ? (item.SubjecttoDeductibleTier1 == null || item.SubjecttoDeductibleTier1 == "") ? "No" : item.SubjecttoDeductibleTier1 : "";
                        //benefit.BenefitInformation.DeductibleAndOutOfPocketExceptions.SubjectToDeductibleTier2 = (item.IsCovered == "Yes") ? (item.SubjectToDeductibleTier2 == null || item.SubjectToDeductibleTier2 == "") ? "No" : item.SubjectToDeductibleTier2 : "";
                        //benefit.BenefitInformation.DeductibleAndOutOfPocketExceptions.ExcludedFromInNetworkMOOP = (item.IsCovered == "Yes" || item.IsCovered == "Covered") ? (item.ExcludedfromInNetworkMOOP == null || item.ExcludedfromInNetworkMOOP == "") ? "No" : item.ExcludedfromInNetworkMOOP : "";
                        //benefit.BenefitInformation.DeductibleAndOutOfPocketExceptions.ExcludedFromOutOfNetworkMOOP = (item.IsCovered == "Yes" || item.IsCovered == "Covered" || item.IsCovered == "" || item.IsCovered == null) ? (item.ExcludedfromOutofNetworkMOOP == null || item.ExcludedfromOutofNetworkMOOP == "") ? "No" : item.ExcludedfromOutofNetworkMOOP : "";
                        benefit.BenefitInformation.DeductibleAndOutOfPocketExceptions.ExcludedFromInNetworkMOOP = item.ExcludedfromInNetworkMOOP;
                        benefit.BenefitInformation.DeductibleAndOutOfPocketExceptions.ExcludedFromOutOfNetworkMOOP = item.ExcludedfromOutofNetworkMOOP;
                        benefit.BenefitInformation.EHB = item.EHB == "Yes" ? item.EHB : string.Empty;
                        //benefit.BenefitInformation.StateRequiredBenefit = item.StateBenefitRequired;

                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                string customMsg = "An error occurred while processing Section => Benefit Information - General Information";
                Exception customException = new Exception(customMsg, ex);
                ExceptionPolicyWrapper.HandleException(customException, ExceptionPolicies.ExceptionShielding);
            }
        }
        #endregion Generate Benefit Information

        #region Plan Cost Sharing - SBC Scenario
        private void GenerateSBCScenario(ref PlanCostSharingAttributes attribute, dynamic document)
        {
            try
            {
                attribute.SBCScenario = new SBCScenario();
                foreach (var item in document.SBCScenario.PlanSBCScenarioDetailsList)
                {
                    if (attribute.HIOSPlanIDComponentAndVariant == (string)item.HIOSPlanIDStandardComponentVariant
                        && attribute.PlanMarketingName == (string)item.PlanMarketingName)
                    {
                        foreach (var sbc in item.PlanAndBenefitSBCScenarioList)
                        {
                            if (sbc.SBCScenario.ToString().ToLower() == "Having a Baby".ToLower())
                            {
                                attribute.SBCScenario.HavingABaby = new HavingABaby();
                                attribute.SBCScenario.HavingABaby.Deductible = sbc.Deductible;
                                attribute.SBCScenario.HavingABaby.Coinsurance = sbc.Coinsurance;
                                attribute.SBCScenario.HavingABaby.Copayment = sbc.Copayment;
                                attribute.SBCScenario.HavingABaby.Limit = sbc.Limit;
                            }
                            else if (sbc.SBCScenario.ToString().ToLower() == "Having Diabetes".ToLower())
                            {
                                attribute.SBCScenario.HavingDiabetes = new HavingDiabetes();
                                attribute.SBCScenario.HavingDiabetes.Deductible = sbc.Deductible;
                                attribute.SBCScenario.HavingDiabetes.Coinsurance = sbc.Coinsurance;
                                attribute.SBCScenario.HavingDiabetes.Copayment = sbc.Copayment;
                                attribute.SBCScenario.HavingDiabetes.Limit = sbc.Limit;
                            }
                            else if (sbc.SBCScenario.ToString().ToLower() == "Treatment of a Simple Fracture".ToLower())
                            {
                                attribute.SBCScenario.TreatmentOfSimpleFracture = new HavingATreatment();
                                attribute.SBCScenario.TreatmentOfSimpleFracture.Deductible = sbc.Deductible;
                                attribute.SBCScenario.TreatmentOfSimpleFracture.Coinsurance = sbc.Coinsurance;
                                attribute.SBCScenario.TreatmentOfSimpleFracture.Copayment = sbc.Copayment;
                                attribute.SBCScenario.TreatmentOfSimpleFracture.Limit = sbc.Limit;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string customMsg = "An error occurred while processing Section => SBC Scenario";
                Exception customException = new Exception(customMsg, ex);
                ExceptionPolicyWrapper.HandleException(customException, ExceptionPolicies.ExceptionShielding);
            }
        }
        #endregion Plan Cost Sharing - SBC Scenario

        #region Plan Cost Sharing - Deductibles
        private void GenerateDrugDeductibles(ref PlanCostSharingAttributes attribute, dynamic document)
        {
            try
            {
                foreach (var item in document.Deductibles.DrugDeductibles.PlanDrugDeductibleDetails)
                {
                    if (attribute.HIOSPlanIDComponentAndVariant == (string)item.HIOSPlanIDStandardComponentVariant
                        && attribute.PlanMarketingName == (string)item.PlanMarketingName
                        && attribute.CSRVariationType == (string)item.CSRVariationType)
                    {
                        if (item.DeductibleDrugType == "Medical EHB Deductible")
                        {
                            attribute.MedicalEHBDeductible = new MedicalEHBDeductible();
                            foreach (var deductible in item.PlanAndBenefitsTemplateNetworkList)
                            {
                                if (deductible.NetworkName == "In Network (Tier 1)")
                                {
                                    attribute.MedicalEHBDeductible.InNetworkIndividual = deductible.Individual;
                                    attribute.MedicalEHBDeductible.InNetworkFamily = deductible.Family;
                                    attribute.MedicalEHBDeductible.InNetworkDefaultCoinsurance = deductible.DefaultCoinsurance;
                                }
                                else if (deductible.NetworkName == "In Network (Tier 2)")
                                {
                                    attribute.MedicalEHBDeductible.InNetworkTier2Individual = deductible.Individual;
                                    attribute.MedicalEHBDeductible.InNetworkTier2Family = deductible.Family;
                                    attribute.MedicalEHBDeductible.InNetworkTier2DefaultCoinsurance = deductible.DefaultCoinsurance;
                                }
                                else if (deductible.NetworkName == "Out of Network")
                                {
                                    attribute.MedicalEHBDeductible.OutOfNetworkIndividual = deductible.Individual;
                                    attribute.MedicalEHBDeductible.OutOfNetworkFamily = deductible.Family;
                                }
                                else if (deductible.NetworkName == "Combined In/Out Network")
                                {
                                    attribute.MedicalEHBDeductible.CombinedInOutNetworkIndividual = deductible.Individual;
                                    attribute.MedicalEHBDeductible.CombinedInOutNetworkFamily = deductible.Family;
                                }
                            }
                        }
                        else if (item.DeductibleDrugType == "Drug EHB Deductible")
                        {
                            attribute.DrugEHBDeductible = new DrugEHBDeductible();
                            foreach (var deductible in item.PlanAndBenefitsTemplateNetworkList)
                            {
                                if (deductible.NetworkName == "In Network (Tier 1)")
                                {
                                    attribute.DrugEHBDeductible.InNetworkIndividual = deductible.Individual;
                                    attribute.DrugEHBDeductible.InNetworkFamily = deductible.Family;
                                    attribute.DrugEHBDeductible.InNetworkDefaultCoinsurance = deductible.DefaultCoinsurance;
                                }
                                else if (deductible.NetworkName == "In Network (Tier 2)")
                                {
                                    attribute.DrugEHBDeductible.InNetworkTier2Individual = deductible.Individual;
                                    attribute.DrugEHBDeductible.InNetworkTier2Family = deductible.Family;
                                    attribute.DrugEHBDeductible.InNetworkTier2DefaultCoinsurance = deductible.DefaultCoinsurance;
                                }
                                else if (deductible.NetworkName == "Out of Network")
                                {
                                    attribute.DrugEHBDeductible.OutOfNetworkIndividual = deductible.Individual;
                                    attribute.DrugEHBDeductible.OutOfNetworkFamily = deductible.Family;
                                }
                                else if (deductible.NetworkName == "Combined In/Out Network")
                                {
                                    attribute.DrugEHBDeductible.CombinedInOutNetworkIndividual = deductible.Individual;
                                    attribute.DrugEHBDeductible.CombinedInOutNetworkFamily = deductible.Family;
                                }
                            }
                        }
                        else if (item.DeductibleDrugType == "Combined Medical and Drug EHB Deductible")
                        {
                            attribute.CombinedMedicalEHBDeductible = new CombinedMedicalEHBDeductible();
                            foreach (var deductible in item.PlanAndBenefitsTemplateNetworkList)
                            {
                                if (deductible.NetworkName == "In Network (Tier 1)")
                                {
                                    attribute.CombinedMedicalEHBDeductible.InNetworkIndividual = deductible.Individual;
                                    attribute.CombinedMedicalEHBDeductible.InNetworkFamily = deductible.Family;
                                    attribute.CombinedMedicalEHBDeductible.InNetworkDefaultCoinsurance = deductible.DefaultCoinsurance;
                                }
                                else if (deductible.NetworkName == "In Network (Tier 2)")
                                {
                                    attribute.CombinedMedicalEHBDeductible.InNetworkTier2Individual = deductible.Individual;
                                    attribute.CombinedMedicalEHBDeductible.InNetworkTier2Family = deductible.Family;
                                    attribute.CombinedMedicalEHBDeductible.InNetworkTier2DefaultCoinsurance = deductible.DefaultCoinsurance;
                                }
                                else if (deductible.NetworkName == "Out of Network")
                                {
                                    attribute.CombinedMedicalEHBDeductible.OutOfNetworkIndividual = deductible.Individual;
                                    attribute.CombinedMedicalEHBDeductible.OutOfNetworkFamily = deductible.Family;
                                }
                                else if (deductible.NetworkName == "Combined In/Out Network")
                                {
                                    attribute.CombinedMedicalEHBDeductible.CombinedInOutNetworkIndividual = deductible.Individual;
                                    attribute.CombinedMedicalEHBDeductible.CombinedInOutNetworkFamily = deductible.Family;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string customMsg = "An error occurred while processing Section => Drug Deductible";
                Exception customException = new Exception(customMsg, ex);
                ExceptionPolicyWrapper.HandleException(customException, ExceptionPolicies.ExceptionShielding);
            }
        }

        private void GenerateDeductibleSubGroups(ref PlanCostSharingAttributes attribute, dynamic document)
        {
            try
            {
                attribute.DeductibleSubGroups = new List<DeductibleSubGroup>();
                foreach (var item in document.Deductibles.DeductibleSubGroup.PlanDeductibleSubGroupDetails)
                {
                    if (attribute.HIOSPlanIDComponentAndVariant == (string)item.HIOSPlanIDStandardComponentVariant
                        && attribute.PlanMarketingName == (string)item.PlanMarketingName
                        && attribute.CSRVariationType == (string)item.CSRVariationType)
                    {
                        DeductibleSubGroup subGroup = new DeductibleSubGroup();
                        //  subGroup.GroupName = item.DeductibleSubGroup;
                        foreach (var deductible in item.PlanAndBenefitsTemplateNetworkList)
                        {
                            if (deductible.NetworkName == "In Network (Tier 1)")
                            {
                                subGroup.InNetworkIndividual = deductible.Individual;
                                subGroup.InNetworkFamily = deductible.Family;
                            }
                            else if (deductible.NetworkName == "In Network (Tier 2)")
                            {
                                subGroup.InNetworkTier2Individual = deductible.Individual;
                                subGroup.InNetworkTier2Family = deductible.Family;
                            }
                            else if (deductible.NetworkName == "Out of Network")
                            {
                                subGroup.OutOfNetworkIndividual = deductible.Individual;
                                subGroup.OutOfNetworkFamily = deductible.Family;
                            }
                            else if (deductible.NetworkName == "Combined In/Out Network")
                            {
                                subGroup.CombinedInOutNetworkIndividual = deductible.Individual;
                                subGroup.CombinedInOutNetworkFamily = deductible.Family;
                            }
                        }

                        attribute.DeductibleSubGroups.Add(subGroup);
                    }
                }
            }
            catch (Exception ex)
            {
                string customMsg = "An error occurred while processing Section => Deductible Sub-Group";
                Exception customException = new Exception(customMsg, ex);
                ExceptionPolicyWrapper.HandleException(customException, ExceptionPolicies.ExceptionShielding);
            }
        }
        #endregion Plan Cost Sharing - Deductibles

        #region Plan Cost Sharing - Maximum Out of Pocket
        private void GenerateMaximumOutofPockets(ref PlanCostSharingAttributes attribute, dynamic document)
        {
            try
            {
                foreach (var item in document.MaximumOutofPocket.PlanMaximumOutofPocketDetails)
                {
                    if (attribute.HIOSPlanIDComponentAndVariant == (string)item.HIOSPlanIDStandardComponentVariant
                        && attribute.PlanMarketingName == (string)item.PlanMarketingName
                        && attribute.CSRVariationType == (string)item.CSRVariationType)
                    {
                        if (item.MaximumOutofPocketType == "Maximum Out of Pocket for Medical EHB Benefits")
                        {
                            attribute.MaximumOutOfPocketForMedicalEHBBenefits = new MaximumOutOfPocketForMedicalEHBBenefits();
                            attribute.MaximumOutOfPocketForMedicalEHBBenefits.MaximumOutofPocketType = item.MaximumOutofPocketType;
                            foreach (var oopm in item.PlanAndBenefitsTemplateNetworkList)
                            {
                                if (oopm.NetworkName == "In Network (Tier 1)")
                                {
                                    attribute.MaximumOutOfPocketForMedicalEHBBenefits.InNetworkIndividual = oopm.Individual;
                                    attribute.MaximumOutOfPocketForMedicalEHBBenefits.InNetworkFamily = oopm.Family;
                                }
                                else if (oopm.NetworkName == "In Network (Tier 2)")
                                {
                                    attribute.MaximumOutOfPocketForMedicalEHBBenefits.InNetworkTier2Individual = oopm.Individual;
                                    attribute.MaximumOutOfPocketForMedicalEHBBenefits.InNetworkTier2Family = oopm.Family;
                                }
                                else if (oopm.NetworkName == "Out of Network")
                                {
                                    attribute.MaximumOutOfPocketForMedicalEHBBenefits.OutOfNetworkIndividual = oopm.Individual;
                                    attribute.MaximumOutOfPocketForMedicalEHBBenefits.OutOfNetworkFamily = oopm.Family;
                                }
                                else if (oopm.NetworkName == "Combined In/Out Network")
                                {
                                    attribute.MaximumOutOfPocketForMedicalEHBBenefits.CombinedInOutNetworkIndividual = oopm.Individual;
                                    attribute.MaximumOutOfPocketForMedicalEHBBenefits.CombinedInOutNetworkFamily = oopm.Family;
                                }
                            }
                        }
                        else if (item.MaximumOutofPocketType == "Maximum Out of Pocket for Drug EHB Benefits")
                        {
                            attribute.MaximumOutOfPocketForDrugEHBBenefits = new MaximumOutOfPocketForDrugEHBBenefits();
                            attribute.MaximumOutOfPocketForDrugEHBBenefits.MaximumOutofPocketType = item.MaximumOutofPocketType;
                            foreach (var oopm in item.PlanAndBenefitsTemplateNetworkList)
                            {
                                if (oopm.NetworkName == "In Network (Tier 1)")
                                {
                                    attribute.MaximumOutOfPocketForDrugEHBBenefits.InNetworkIndividual = oopm.Individual;
                                    attribute.MaximumOutOfPocketForDrugEHBBenefits.InNetworkFamily = oopm.Family;
                                }
                                else if (oopm.NetworkName == "In Network (Tier 2)")
                                {
                                    attribute.MaximumOutOfPocketForDrugEHBBenefits.InNetworkTier2Individual = oopm.Individual;
                                    attribute.MaximumOutOfPocketForDrugEHBBenefits.InNetworkTier2Family = oopm.Family;
                                }
                                else if (oopm.NetworkName == "Out of Network")
                                {
                                    attribute.MaximumOutOfPocketForDrugEHBBenefits.OutOfNetworkIndividual = oopm.Individual;
                                    attribute.MaximumOutOfPocketForDrugEHBBenefits.OutOfNetworkFamily = oopm.Family;
                                }
                                else if (oopm.NetworkName == "Combined In/Out Network")
                                {
                                    attribute.MaximumOutOfPocketForDrugEHBBenefits.CombinedInOutNetworkIndividual = oopm.Individual;
                                    attribute.MaximumOutOfPocketForDrugEHBBenefits.CombinedInOutNetworkFamily = oopm.Family;
                                }
                            }
                        }
                        else if (item.MaximumOutofPocketType == "Maximum Out of Pocket for Medical and Drug EHB Benefits (Total)")
                        {
                            attribute.MaximumOutOfPocketForMedicalAndDrugEHBBenefits = new MaximumOutOfPocketForMedicalAndDrugEHBBenefits();
                            //attribute.MaximumOutOfPocketForMedicalAndDrugEHBBenefits.MaximumOutofPocketType = item.MaximumOutofPocketType;
                            foreach (var oopm in item.PlanAndBenefitsTemplateNetworkList)
                            {
                                if (oopm.NetworkName == "In Network (Tier 1)")
                                {
                                    attribute.MaximumOutOfPocketForMedicalAndDrugEHBBenefits.InNetworkIndividual = oopm.Individual;
                                    attribute.MaximumOutOfPocketForMedicalAndDrugEHBBenefits.InNetworkFamily = oopm.Family;
                                }
                                else if (oopm.NetworkName == "In Network (Tier 2)")
                                {
                                    attribute.MaximumOutOfPocketForMedicalAndDrugEHBBenefits.InNetworkTier2Individual = oopm.Individual;
                                    attribute.MaximumOutOfPocketForMedicalAndDrugEHBBenefits.InNetworkTier2Family = oopm.Family;
                                }
                                else if (oopm.NetworkName == "Out of Network")
                                {
                                    attribute.MaximumOutOfPocketForMedicalAndDrugEHBBenefits.OutOfNetworkIndividual = oopm.Individual;
                                    attribute.MaximumOutOfPocketForMedicalAndDrugEHBBenefits.OutOfNetworkFamily = oopm.Family;
                                }
                                else if (oopm.NetworkName == "Combined In/Out Network")
                                {
                                    attribute.MaximumOutOfPocketForMedicalAndDrugEHBBenefits.CombinedInOutNetworkIndividual = oopm.Individual;
                                    attribute.MaximumOutOfPocketForMedicalAndDrugEHBBenefits.CombinedInOutNetworkFamily = oopm.Family;
                                }
                            }
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                string customMsg = "An error occurred while processing Section => Maximum Out of pocket";
                Exception customException = new Exception(customMsg, ex);
                ExceptionPolicyWrapper.HandleException(customException, ExceptionPolicies.ExceptionShielding);
            }
        }
        #endregion Plan Cost Sharing - HSA/HRA Detail
        private void GenerateHSAHRADetail(ref PlanCostSharingAttributes attribute, dynamic document)
        {
            try
            {
                attribute.HSAHRADetail = new HSAHRADetail();
                foreach (var item in document.HSAHRADetail.HSAHRADetailList)
                {
                    if (attribute.HIOSPlanIDComponentAndVariant == (string)item.HIOSPlanIDStandardComponentVariant)
                    {
                        attribute.HSAHRADetail.HSAEligible = item.HSAEligible;
                        attribute.HSAHRADetail.HSAHRAEmployerContribution = item.HSAHRAEmployerContribution;
                        attribute.HSAHRADetail.HSAHRAEmployerContributionAmount = item.HSAHRAEmployerContributionAmount;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                string customMsg = "An error occurred while processing Section => HSA/HRA Detail";
                Exception customException = new Exception(customMsg, ex);
                ExceptionPolicyWrapper.HandleException(customException, ExceptionPolicies.ExceptionShielding);
            }
        }
        #region Plan Cost Sharing - HSA/HRA Detail

        #endregion Plan Cost Sharing - Plan Variant Level URLs
        private void GeneratePlanVariantLevelURLs(ref PlanCostSharingAttributes attribute, dynamic document)
        {
            try
            {
                attribute.PlanVariantLevelURLs = new PlanVariantLevelURLs();
                foreach (var item in document.PlanVariantLevelURLs.PlanVariantLevelURLsList)
                {
                    if (attribute.HIOSPlanIDComponentAndVariant == (string)item.HIOSPlanIDStandardComponentVariant)
                    {
                        attribute.PlanVariantLevelURLs.URLforSummaryofBenefitsCoverage = item.URLforSummaryofBenefitsCoverage;
                        attribute.PlanVariantLevelURLs.PlanBrochure = item.PlanBrochure;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                string customMsg = "An error occurred while processing Section => Plan Variant Level URLs";
                Exception customException = new Exception(customMsg, ex);
                ExceptionPolicyWrapper.HandleException(customException, ExceptionPolicies.ExceptionShielding);
            }
        }
        #region Plan Cost Sharing - Plan Variant Level URLs

        #endregion Plan Cost Sharing - Deductibles

        #region Plan Cost Sharing - Benefit Details
        private void GeneratePlanBenefitDetails(ref PlanCostSharingAttributes attribute, dynamic document)
        {
            try
            {
                attribute.CostSharingBenefitServices = new List<CostSharingBenefitService>();

                foreach (var item in document.PlanBenefitInformation.PlanBenefitDetails)
                {
                    if (attribute.HIOSPlanIDComponentAndVariant == (string)item.HIOSPlanIDStandardComponentVariant
                        && attribute.PlanMarketingName == (string)item.PlanMarketingName)
                    {
                        if (!attribute.CostSharingBenefitServices.Where(c => c.ServiceName == item.Benefit.ToString()).Any())
                        {
                            CostSharingBenefitService benefitService = new CostSharingBenefitService();
                            benefitService.ServiceName = item.Benefit;

                            foreach (var benefitInfo in item.BenefitsTemplateNetworkList)
                            {
                                if (benefitInfo.NetworkName == "In Network (Tier 1)")
                                {
                                    benefitService.InNetworkCopay = benefitInfo.Copay;
                                    benefitService.InNetworkCoinsurance = benefitInfo.Coinsurance;
                                }
                                else if (benefitInfo.NetworkName == "In Network (Tier 2)")
                                {
                                    benefitService.InNetworkTier2Copay = benefitInfo.Copay;
                                    benefitService.InNetworkTier2Coinsurance = benefitInfo.Coinsurance;
                                }
                                else if (benefitInfo.NetworkName == "Out of Network")
                                {
                                    benefitService.OutOfNetworkCopay = benefitInfo.Copay;
                                    benefitService.OutOfNetworkCoinsurance = benefitInfo.Coinsurance;
                                }
                            }
                            attribute.CostSharingBenefitServices.Add(benefitService);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string customMsg = "An error occurred while processing Section => Benefit Details";
                Exception customException = new Exception(customMsg, ex);
                ExceptionPolicyWrapper.HandleException(customException, ExceptionPolicies.ExceptionShielding);
            }
        }
        #endregion Plan Cost Sharing - Benefit Details

        #region URRTPlanProductInfo
        private void GenerateSectionIGeneralProductandPlanInformation(ref PlanBenefitPackage package, dynamic document)
        {
            try
            {
                package.SectionIGeneralProductandPlanInformation = new List<SectionIGeneralProductandPlanInformation>();
                foreach (var item in document.URRTPlanProductInfo.SectionIGeneralProductandPlanInformation.ProductandPlanInformation)
                {
                    SectionIGeneralProductandPlanInformation sectionI = new SectionIGeneralProductandPlanInformation();
                    sectionI.Product = item.Product;
                    sectionI.ProductID = item.ProductID;
                    sectionI.Metal = item.Metal;
                    sectionI.PlanType = item.PlanType;
                    sectionI.PlanName = item.PlanName;
                    sectionI.PlanIDStandardComponentID = item.PlanIDStandardComponentID;
                    sectionI.AVMetalValue = item.AVMetalValue;
                    sectionI.AVPricingValue = item.AVPricingValue;
                    sectionI.ExchangePlan = item.ExchangePlan;
                    sectionI.HistoricalRateIncreaseCalendarYear2 = item.HistoricalRateIncreaseCalendarYear2;
                    sectionI.HistoricalRateIncreaseCalendarYear1 = item.HistoricalRateIncreaseCalendarYear1;
                    sectionI.HistoricalRateIncreaseCalendarYear0 = item.HistoricalRateIncreaseCalendarYear0;
                    sectionI.EffectiveDateofProposedRates = item.EffectiveDateofProposedRates;
                    sectionI.RateChangePercentoverpreviousfiling = item.RateChangePercentoverpreviousfiling;
                    sectionI.CumulativeRateChangePercentover12mosprior = item.CumulativeRateChangePercentover12mosprior;
                    sectionI.ProjectedPerRateChangePercentageoverExperiencePeriod = item.ProjectedPerRateChangePercentageoverExperiencePeriod;
                    sectionI.ProductThresholdRateIncreasePercentage = item.ProductThresholdRateIncreasePercentage;
                    package.SectionIGeneralProductandPlanInformation.Add(sectionI);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void GenerateSectionIIComponentsofPremiumIncreasePMPMDollarAmountaboveCurrentAverag(ref PlanBenefitPackage package, dynamic document)
        {
            package.SectionIIComponentsofPremiumIncreasePMPMDollarAmountaboveCurrentAverag = new List<SectionIIComponentsofPremiumIncreasePMPMDollarAmountaboveCurrentAverag>();
            foreach (var item in document.URRTPlanProductInfo.SectionIIComponentsofPremiumIncreasePMPMDollarAmountaboveCurrentAverag.ComponentsofPremiumIncrease)
            {
                SectionIIComponentsofPremiumIncreasePMPMDollarAmountaboveCurrentAverag sectionII = new SectionIIComponentsofPremiumIncreasePMPMDollarAmountaboveCurrentAverag();

                sectionII.PlanName = item.PlanName;
                sectionII.PlanIDStandardComponentID = item.PlanIDStandardComponentID;
                sectionII.Inpatient = item.Inpatient;
                sectionII.Outpatient = item.Outpatient;
                sectionII.Professional = item.Professional;
                sectionII.PrescriptionDrug = item.PrescriptionDrug;
                sectionII.Other = item.Other;
                sectionII.Capitation = item.Capitation;
                sectionII.Administration = item.Administration;
                sectionII.TaxesFees = item.TaxesFees;
                sectionII.RiskProfitCharge = item.RiskProfitCharge;
                sectionII.TotalRateIncrease = item.TotalRateIncrease;
                sectionII.MemberCostShareIncrease = item.MemberCostShareIncrease;
                sectionII.AverageCurrentRatePMPM = item.AverageCurrentRatePMPM;
                sectionII.ProjectedMemberMonths = item.ProjectedMemberMonths;

                package.SectionIIComponentsofPremiumIncreasePMPMDollarAmountaboveCurrentAverag.Add(sectionII);
            }

        }

        private void GenrateSectionIIIExperiencePeriodInformation(ref PlanBenefitPackage package, dynamic document)
        {
            try
            {
                package.SectionIIIExperiencePeriodInformation = new List<SectionIIIExperiencePeriodInformation>();
                SectionIIIExperiencePeriodInformation sectionIII = new SectionIIIExperiencePeriodInformation();
                GenrateSectionIIIPremiumInformation(ref sectionIII, document);
                GenrateSectionIIIClaimsInformation(ref sectionIII, document);
                package.SectionIIIExperiencePeriodInformation.Add(sectionIII);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void GenrateSectionIIIPremiumInformation(ref SectionIIIExperiencePeriodInformation sectionIII, dynamic document)
        {
            try
            {
                sectionIII.PremiumInformation = new List<PremiumInformation>();
                foreach (var item in document.URRTPlanProductInfo.SectionIIIExperiencePeriodInformation.PremiumInformation)
                {
                    PremiumInformation premiumInformation = new PremiumInformation();
                    premiumInformation.PlanName = item.PlanName;
                    premiumInformation.PlanIDStandardComponentID = item.PlanIDStandardComponentID;
                    premiumInformation.AverageRatePMPM = item.AverageRatePMPM;
                    premiumInformation.MemberMonths = item.MemberMonths;
                    premiumInformation.TotalPremiumTP = item.TotalPremiumTP;
                    premiumInformation.EHBPercentofTP = item.EHBPercentofTP;
                    premiumInformation.StatemandatedbenefitsportionofTPthatareotherthanEHB = item.StatemandatedbenefitsportionofTPthatareotherthanEHB;
                    premiumInformation.OtherbenefitsportionofTP = item.OtherbenefitsportionofTP;
                    sectionIII.PremiumInformation.Add(premiumInformation);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void GenrateSectionIIIClaimsInformation(ref SectionIIIExperiencePeriodInformation sectionIII, dynamic document)
        {
            try
            {
                sectionIII.ClaimsInformation = new List<ClaimsInformation>();
                foreach (var item in document.URRTPlanProductInfo.SectionIIIExperiencePeriodInformation.ClaimsInformation)
                {
                    ClaimsInformation claimsInformation = new ClaimsInformation();
                    claimsInformation.PlanName = item.PlanName;
                    claimsInformation.PlanIDStandardComponentID = item.PlanIDStandardComponentID;
                    claimsInformation.TotalAllowedClaimsTAC = item.TotalAllowedClaimsTAC;
                    claimsInformation.EHBPercentofTAC = item.EHBPercentofTAC;
                    claimsInformation.StatemandatedbenefitsportionofTACthatareotherthanEHB = item.StatemandatedbenefitsportionofTACthatareotherthanEHB;
                    claimsInformation.OtherbenefitsportionofTAC = item.OtherbenefitsportionofTAC;
                    claimsInformation.AllowedClaimswhicharenottheissuersobligation = item.AllowedClaimswhicharenottheissuersobligation;
                    claimsInformation.PortionoftheabovepayablebyHHSsfundsonbehalfofinsuredpersonsindollars = item.PortionoftheabovepayablebyHHSsfundsonbehalfofinsuredpersonsindollars;
                    claimsInformation.PortionofabovepayablebyHHSonbehalfofinsuredpersonasPercentage = item.PortionofabovepayablebyHHSonbehalfofinsuredpersonasPercentage;
                    claimsInformation.TotalIncurredclaimspayablewithissuerfunds = item.TotalIncurredclaimspayablewithissuerfunds;
                    claimsInformation.NetAmtofRein = item.NetAmtofRein;
                    claimsInformation.NetAmtofRiskAdj = item.NetAmtofRiskAdj;
                    claimsInformation.IncurredClaimsPMPM = item.IncurredClaimsPMPM;
                    claimsInformation.AllowedClaimsPMPM = item.AllowedClaimsPMPM;
                    claimsInformation.EHBportionofAllowedClaimsPMPM = item.EHBportionofAllowedClaimsPMPM;


                    sectionIII.ClaimsInformation.Add(claimsInformation);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void GenrateSectionIVProjected12monthsfollowingeffectivedate(ref PlanBenefitPackage package, dynamic document)
        {
            try
            {
                package.SectionIVProjected12monthsfollowingeffectivedate = new List<SectionIVProjected12monthsfollowingeffectivedate>();
                SectionIVProjected12monthsfollowingeffectivedate sectionIV = new SectionIVProjected12monthsfollowingeffectivedate();
                GenrateSectionIVPremiumInformation(ref sectionIV, document);
                GenrateSectionIVClaimsInformation(ref sectionIV, document);
                package.SectionIVProjected12monthsfollowingeffectivedate.Add(sectionIV);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void GenrateSectionIVClaimsInformation(ref SectionIVProjected12monthsfollowingeffectivedate sectionIV, dynamic document)
        {
            try
            {
                sectionIV.ClaimsInformation = new List<ClaimsInformation>();
                foreach (var item in document.URRTPlanProductInfo.SectionIIIExperiencePeriodInformation.ClaimsInformation)
                {
                    ClaimsInformation claimsInformation = new ClaimsInformation();

                    claimsInformation.PlanName = item.PlanName;
                    claimsInformation.PlanIDStandardComponentID = item.PlanIDStandardComponentID;
                    claimsInformation.TotalAllowedClaimsTAC = item.TotalAllowedClaimsTAC;
                    claimsInformation.EHBPercentofTAC = item.EHBPercentofTAC;
                    claimsInformation.StatemandatedbenefitsportionofTACthatareotherthanEHB = item.StatemandatedbenefitsportionofTACthatareotherthanEHB;
                    claimsInformation.OtherbenefitsportionofTAC = item.OtherbenefitsportionofTAC;
                    claimsInformation.AllowedClaimswhicharenottheissuersobligation = item.AllowedClaimswhicharenottheissuersobligation;
                    claimsInformation.PortionoftheabovepayablebyHHSsfundsonbehalfofinsuredpersonsindollars = item.PortionoftheabovepayablebyHHSsfundsonbehalfofinsuredpersonsindollars;
                    claimsInformation.PortionofabovepayablebyHHSonbehalfofinsuredpersonasPercentage = item.PortionofabovepayablebyHHSonbehalfofinsuredpersonasPercentage;
                    claimsInformation.TotalIncurredclaimspayablewithissuerfunds = item.TotalIncurredclaimspayablewithissuerfunds;
                    claimsInformation.NetAmtofRein = item.NetAmtofRein;
                    claimsInformation.NetAmtofRiskAdj = item.NetAmtofRiskAdj;
                    claimsInformation.IncurredClaimsPMPM = item.IncurredClaimsPMPM;
                    claimsInformation.AllowedClaimsPMPM = item.AllowedClaimsPMPM;
                    claimsInformation.EHBportionofAllowedClaimsPMPM = item.EHBportionofAllowedClaimsPMPM;
                    sectionIV.ClaimsInformation.Add(claimsInformation);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void GenrateSectionIVPremiumInformation(ref SectionIVProjected12monthsfollowingeffectivedate sectionIV, dynamic document)
        {
            try
            {
                sectionIV.PremiumInformation = new List<PremiumInformation>();
                foreach (var item in document.URRTPlanProductInfo.SectionIVProjected12monthsfollowingeffectivedate.PremiumInformation)
                {
                    PremiumInformation premiumInformation = new PremiumInformation();
                    premiumInformation.PlanName = item.PlanName;
                    premiumInformation.PlanIDStandardComponentID = item.PlanIDStandardComponentID;
                    premiumInformation.PlanAdjustedIndexRate = item.PlanAdjustedIndexRate;
                    premiumInformation.MemberMonths = item.MemberMonths;
                    premiumInformation.TotalPremiumTP = item.TotalPremiumTP;
                    premiumInformation.EHBPercentofTP = item.EHBPercentofTP;
                    premiumInformation.StatemandatedbenefitsportionofTPthatareotherthanEHB = item.StatemandatedbenefitsportionofTPthatareotherthanEHB;
                    premiumInformation.OtherbenefitsportionofTP = item.OtherbenefitsportionofTP;
                    sectionIV.PremiumInformation.Add(premiumInformation);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #endregion Private Methods
    }
}
