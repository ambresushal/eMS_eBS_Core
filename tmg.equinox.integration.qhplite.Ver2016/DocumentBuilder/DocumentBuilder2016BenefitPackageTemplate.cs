using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using tmg.equinox.integration.qhplite.Ver2016.QHPModel;

namespace tmg.equinox.integration.qhplite.Ver2016.DocumentBuilder
{
    public class DocumentBuilder2016BenefitPackageTemplate : IDocumentBuilder
    {
        #region Private Memebers
        private string _qhpFile;
        private string _defaultJSON;
        private List<QhpDocumentViewModel> _importDocuments;
        private List<PlanBenefitPackage> _packages;
        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Public Methods
        public IList<QhpDocumentViewModel> BuildDocuments(string qhpFile, string defaultJSON)
        {
            this._qhpFile = qhpFile;
            this._defaultJSON = defaultJSON;
            Initialize();
            return _importDocuments;
        }
        #endregion Public Methods

        #region Private Methods
        private void Initialize()
        {
            _importDocuments = new List<QhpDocumentViewModel>();
            IQHPFileReader reader = new QHPFileReader();
            _packages = reader.GetPackagesFromFile(_qhpFile);
            foreach (var package in _packages)
            {
                dynamic document = JsonConvert.DeserializeObject(_defaultJSON);
                GenerateDynamicObjectFromJSON(ref document, package);

                _importDocuments.Add(new QhpDocumentViewModel { DocumentName = package.PackageName, DocumentData = JsonConvert.SerializeObject(document) });
            }
        }

        private void GenerateDynamicObjectFromJSON(ref dynamic document, PlanBenefitPackage package)
        {
            BuildGeneralInformationSection(ref document, package);
            BuildProductDetailsSection(ref document, package);
            BuildBenefitInformationSection(ref document, package);
            BuildCostShareVarianceSection(ref document, package);
            BuildSBCScenarioSection(ref document, package);
            BuildDeductiblesSection(ref document, package);
            BuildHSAHRADetailSection(ref document, package);
            BuildPlanVariantLevelURLsSection(ref document, package);
            BuildAVCalculatorAdditionalBenefitDesign(ref document, package);
            BuildMaximumOutofPocketSection(ref document, package);
            BuildPlanBenefitInformationSection(ref document, package);
        }

        private void BuildGeneralInformationSection(ref dynamic document, PlanBenefitPackage package)
        {
            //Add General Information -> Plan Information Section 
            document.GeneralInformation.PlanInformation.HIOSIssuerID = package.HIOSIssuerID;
            document.GeneralInformation.PlanInformation.IssuerState = package.IssuerState;
            document.GeneralInformation.PlanInformation.MarketCoverage = package.MarketCoverage;
            document.GeneralInformation.PlanInformation.DentalOnlyPlan = package.DentalOnlyPlan;
            document.GeneralInformation.PlanInformation.TIN = package.TIN;

            //Add General Information -> Network List
            document.GeneralInformation.NetworkInformation.NetworkList = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(GetNetworkList()));
        }

        private void BuildProductDetailsSection(ref dynamic document, PlanBenefitPackage package)
        {
            //Add Product Details -> Plan Identifiers Section
            document.ProductDetails.PlanIdentifiers.PlanIdentifierList = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(GetPlanIdentifiersList(package.PlanIdentifiers)));

            //Add Product Details -> Plan Attributes Section     
            document.ProductDetails.PlanAttributes.PlanAttributesList = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(GetPlanAttributesList(package.PlanIdentifiers)));

            //Add Product Details -> Standard Alone Dental Only Section
            document.ProductDetails.StandardAloneDentalOnly.StandardAloneDentalOnlyList = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(GetStandardAloneDentalOnlyList(package.PlanIdentifiers)));

            //Add Product Details -> AV Calculator Additional Benefit Design Section
            //document.ProductDetails.AVCalculatorAdditionalBenefitDesign.AVCalculatorAdditionalBenefitDesignList = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(GetAVCalculatorAdditionalBenefitDesignList(package.PlanIdentifiers)));

            //Add Product Details -> Plan Dates Section
            document.ProductDetails.PlanDates.PlanDatesList = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(GetPlanDatesList(package.PlanIdentifiers)));

            //Add Product Details -> Geographical Coverage Section
            document.ProductDetails.GeographicCoverage.GeographicCoverageList = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(GetGeographicCoverageList(package.PlanIdentifiers)));

            //Add Product Details -> Geographical Coverage Section
            document.ProductDetails.AdditionalInformation.PlanLevelURLs = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(GetPlanLevelURLsList(package.PlanIdentifiers)));
        }

        private void BuildBenefitInformationSection(ref dynamic document, PlanBenefitPackage package)
        {
            //Add Benefit Information Details -> Benefits Section
            document.BenefitInformation.BenefitList = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(GetBenefitList(package.Benefits)));

            //Add Benefit Information Details -> General Information Section
            document.BenefitInformation.GeneralInformation = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(GetBenefitGeneralInformationList(package.Benefits)));
        }

        private void BuildCostShareVarianceSection(ref dynamic document, PlanBenefitPackage package)
        {
            //Add Cost Share Variance -> Plan Cost Sharing Attributes Section
            document.CostShareVariance.PlanCostSharingAttributes = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(GetPlanCostSharingAttributesList(package.PlanCostSharingAttributes)));
        }

        private void BuildSBCScenarioSection(ref dynamic document, PlanBenefitPackage package)
        {
            //Add SBC Scenario -> SBC Scenario Section
            document.SBCScenario.SBCScenarioList = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(GetSBCScenarioList()));

            //Add SBC Scenario -> SBC Scenario Section
            document.SBCScenario.PlanSBCScenarioDetailsList = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(GetPlanSBCScenarioDetailsList(package.PlanCostSharingAttributes, JsonConvert.DeserializeObject(JsonConvert.SerializeObject(document.SBCScenario.PlanSBCScenarioDetailsList)))));
        }

        private void BuildDeductiblesSection(ref dynamic document, PlanBenefitPackage package)
        {
            //Add Deductibles -> Drug Deductibles Section
            document.Deductibles.DrugDeductibles.DrugDeductibleList = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(GetDrugDeductibleList()));

            //Add Deductibles -> Drug Deductibles Details Section
            document.Deductibles.DrugDeductibles.PlanDrugDeductibleDetails = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(GetPlanDrugDeductibleDetailsList(package.PlanCostSharingAttributes)));

            //Add Deductibles -> Deductible SubGroup Section
            var subGroupList = GetDeductibleSubGroupList(package.PlanCostSharingAttributes);
            document.Deductibles.DeductibleSubGroup.DeductibleSubGroupList = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(subGroupList));

            //Add Deductibles -> Deductible SubGroup Section
            document.Deductibles.DeductibleSubGroup.PlanDeductibleSubGroupDetails = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(GetPlanDeductibleSubGroupDetails(package.PlanCostSharingAttributes, subGroupList)));
        }

        private void BuildHSAHRADetailSection(ref dynamic document, PlanBenefitPackage package)
        {
            //Add HSAHRADetail -> HSA HRA Detail List
            document.HSAHRADetail.HSAHRADetailList = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(GetHSAHRADetailList(package.PlanCostSharingAttributes)));
        }
        private void BuildPlanVariantLevelURLsSection(ref dynamic document, PlanBenefitPackage package)
        {
            document.PlanVariantLevelURLs.PlanVariantLevelURLsList = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(GetPlanVariantLevelURLsList(package.PlanCostSharingAttributes)));
        }

        private void BuildAVCalculatorAdditionalBenefitDesign(ref dynamic document, PlanBenefitPackage package)
        {
            document.AVCalculatorAdditionalBenefitDesign.AVCalculatorAdditionalBenefitDesignList = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(GetAVCalculatorAdditionalBenefitDesignList(package.PlanCostSharingAttributes)));
        }

        private void BuildMaximumOutofPocketSection(ref dynamic document, PlanBenefitPackage package)
        {
            //Add Maximum Out of Pocket -> Maximum Out of Pocket Section
            document.MaximumOutofPocket.MaximumOutofPocketList = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(GetMaximumOutofPocketList()));

            //Add Deductibles -> Drug Deductibles Details Section
            document.MaximumOutofPocket.PlanMaximumOutofPocketDetails = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(GetPlanMaximumOutofPocketDetailsList(package.PlanCostSharingAttributes)));
        }

        private void BuildPlanBenefitInformationSection(ref dynamic document, PlanBenefitPackage package)
        {
            //Add Deductibles -> Drug Deductibles Details Section
            document.PlanBenefitInformation.PlanBenefitDetails = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(GetPlanBenefitDetailsList(package.PlanCostSharingAttributes)));
        }

        #region Build Network Details
        private IList<dynamic> GetNetworkList()
        {
            IList<dynamic> networkList = new List<dynamic>();
            networkList.Add(new
            {
                NetworkName = "In Network (Tier 1)"
            });
            networkList.Add(new
            {
                NetworkName = "In Network (Tier 2)"
            });
            networkList.Add(new
            {
                NetworkName = "Out of Network"
            });
            networkList.Add(new
            {
                NetworkName = "Combined In/Out Network"
            });
            return networkList;
        }
        #endregion Build Network Details

        #region Build Product Details
        private List<dynamic> GetPlanIdentifiersList(IList<PlanIdentifier> planIdentifierList)
        {
            List<dynamic> planIdentifiersList = new List<dynamic>();
            foreach (var planIdentifier in planIdentifierList)
            {
                dynamic planIdentifierToAdd = new
                {
                    HIOSPlanIDStandardComponent = planIdentifier.HIOSPlanID,
                    PlanMarketingName = planIdentifier.PlanMarketingName,
                    HIOSProductID = planIdentifier.HIOSProductID,
                    HPID = planIdentifier.HPID,
                    NetworkID = planIdentifier.NetworkID,
                    ServiceAreaID = planIdentifier.ServiceAreaID,
                    FormularyID = planIdentifier.FormularyID,
                };
                planIdentifiersList.Add(planIdentifierToAdd);
            }
            return planIdentifiersList;

        }
        private List<dynamic> GetPlanAttributesList(IList<PlanIdentifier> planIdentifierList)
        {
            List<dynamic> planAttributesList = new List<dynamic>();

            foreach (var planIdentifier in planIdentifierList)
            {
                dynamic planAttributeToAdd = new
                {
                    HIOSPlanIDStandardComponent = planIdentifier.HIOSPlanID,
                    PlanMarketingName = planIdentifier.PlanMarketingName,
                    NewExistingPlan = planIdentifier.PlanAttributes.NewExistingPlan,
                    PlanType = planIdentifier.PlanAttributes.PlanType,
                    LevelofCoverage = planIdentifier.PlanAttributes.LevelOfCoverage,
                    UniquePlanDesign = planIdentifier.PlanAttributes.UniquePlanDesign,
                    QHPNonQHP = planIdentifier.PlanAttributes.QHPNonQHP,
                    NoticePeriodforPregnancy = planIdentifier.PlanAttributes.NoticeRequiredForPregnancy,
                    IsReferralRequiredforSpecialist = planIdentifier.PlanAttributes.IsAReferralRequiredForSpecialist,
                    SpecialistsRequiringaReferral = planIdentifier.PlanAttributes.SpecialistsRequiringAReferral,
                    PlanLevelExclusions = planIdentifier.PlanAttributes.PlanLevelExclusions,
                    LimitedCostSharingPlanVariationEstAdvancedPayment = planIdentifier.PlanAttributes.LimitedCostSharingPlanVariation,
                    DoesthisplanofferCompositeRating = planIdentifier.PlanAttributes.DoesthisplanofferCompositeRating,
                    // HSAEligible = planIdentifier.PlanAttributes.HSAEligible,
                    //HSAHRAEmployerContribution = planIdentifier.PlanAttributes.HSAHRAEmployerContribution,
                    //HSAHRAEmployerContributionAmount = planIdentifier.PlanAttributes.HSAHRAEmployerContributionAmount,
                    ChildOnlyOffering = planIdentifier.PlanAttributes.ChildOnlyOffering,
                    ChildOnlyPlanID = planIdentifier.PlanAttributes.ChildOnlyPlanID,
                    TobaccoWellnessProgramOffered = planIdentifier.PlanAttributes.TobaccoWellnessProgramOffered,
                    DiseaseManagementProgramsOffered = planIdentifier.PlanAttributes.DiseaseManagementProgramsOffered,
                    EHBPercentofTotalPremium = planIdentifier.PlanAttributes.EHBPercentofTotalPremium,
                };
                planAttributesList.Add(planAttributeToAdd);
            }
            return planAttributesList;
        }
        private List<dynamic> GetStandardAloneDentalOnlyList(IList<PlanIdentifier> planIdentifierList)
        {
            List<dynamic> standardAloneDentalOnlyList = new List<dynamic>();

            foreach (var planIdentifier in planIdentifierList)
            {
                dynamic item = new
                {
                    HIOSPlanIDStandardComponent = planIdentifier.HIOSPlanID,
                    PlanMarketingName = planIdentifier.PlanMarketingName,
                    EHBApportionmentforPediatricDental = planIdentifier.StandAloneDentalOnly.EHBApportionmentForPediatricDental,
                    GuaranteedVsEstimatedRate = planIdentifier.StandAloneDentalOnly.GuaranteedVsEstimatedRate,
                };
                standardAloneDentalOnlyList.Add(item);
            }
            return standardAloneDentalOnlyList;
        }
        private List<dynamic> GetAVCalculatorAdditionalBenefitDesignList(IList<PlanCostSharingAttributes> costSharingAttributesList)
        {
            List<dynamic> avCalculatorAdditionalBenefitDesign = new List<dynamic>();

            foreach (var planIdentifier in costSharingAttributesList)
            {
                dynamic item = new
                {
                    HIOSPlanIDStandardComponent = planIdentifier.HIOSPlanIDComponentAndVariant,
                    PlanMarketingName = planIdentifier.PlanMarketingName,
                    MaximumCoinsuranceforSpecialtyDrugs = planIdentifier.AVCalculatorAdditionalBenefitDesign.MaximumCoinsuranceForSpecialityDrugs,
                    MaximumNumberofDaysforCharginganInpatientCopay = planIdentifier.AVCalculatorAdditionalBenefitDesign.MaximumNumberOfDaysForChargingInpatientCopay,
                    BeginPrimaryCareCostSharingAfteraSetNumberofVisits = planIdentifier.AVCalculatorAdditionalBenefitDesign.BeginPrimaryCostSharingAfterSetNumberOfVisits,
                    BeginPrimaryCareDeductibleCoinsuranceAfteraSetNumberofCopays = planIdentifier.AVCalculatorAdditionalBenefitDesign.BeginPrimaryCareDedCoAfterSetNumberOfCopays,
                };
                avCalculatorAdditionalBenefitDesign.Add(item);
            }
            return avCalculatorAdditionalBenefitDesign;
        }
        private List<dynamic> GetPlanDatesList(IList<PlanIdentifier> planIdentifierList)
        {
            List<dynamic> planDatesList = new List<dynamic>();

            foreach (var planIdentifier in planIdentifierList)
            {
                dynamic item = new
                {
                    HIOSPlanIDStandardComponent = planIdentifier.HIOSPlanID,
                    PlanMarketingName = planIdentifier.PlanMarketingName,
                    PlanEffectiveDate = planIdentifier.PlanDates.PlanEffectiveDate,
                    PlanExpirationDate = planIdentifier.PlanDates.PlanExpirationDate,
                };
                planDatesList.Add(item);
            }
            return planDatesList;
        }
        private List<dynamic> GetGeographicCoverageList(IList<PlanIdentifier> planIdentifierList)
        {
            List<dynamic> geographicCoverageList = new List<dynamic>();

            foreach (var planIdentifier in planIdentifierList)
            {
                dynamic item = new
                {
                    HIOSPlanIDStandardComponent = planIdentifier.HIOSPlanID,
                    PlanMarketingName = planIdentifier.PlanMarketingName,
                    OutofCountryCoverage = planIdentifier.GeographicCoverage.OutOfCountryCoverage,
                    OutofCountryCoverageDescription = planIdentifier.GeographicCoverage.OutOfCountryCoverageDescription,
                    OutofServiceAreaCoverage = planIdentifier.GeographicCoverage.OutOfServiceAreaCoverage,
                    OutofServiceAreaCoverageDescription = planIdentifier.GeographicCoverage.OutOfServiceAreaCoverageDescription,
                    NationalNetwork = planIdentifier.GeographicCoverage.NationalNetwork,
                };
                geographicCoverageList.Add(item);
            }
            return geographicCoverageList;
        }
        private List<dynamic> GetPlanLevelURLsList(IList<PlanIdentifier> planIdentifierList)
        {
            List<dynamic> PlanLevelURLs = new List<dynamic>();

            foreach (var planIdentifier in planIdentifierList)
            {
                dynamic item = new
                {
                    HIOSPlanIDStandardComponent = planIdentifier.HIOSPlanID,
                    PlanMarketingName = planIdentifier.PlanMarketingName,
                    //URLforSummaryofBenefitsCoverage = planIdentifier.URLs.URLForSummaryOfBenefitsAndCoverage,
                    URLforEnrollmentPayment = planIdentifier.PlanLevelURLs.URLforEnrollmentPayment,
                    // PlanBrochure = planIdentifier.URLs.PlanBrochure,
                };
                PlanLevelURLs.Add(item);
            }
            return PlanLevelURLs;
        }
        #endregion Build Product Details

        #region Build Benefit Information Details
        private List<dynamic> GetBenefitList(IList<Benefit> benefitList)
        {
            List<dynamic> list = new List<dynamic>();

            foreach (var benefit in benefitList)
            {
                dynamic item = new
                {
                    Benefits = benefit.BenefitInformation.Benefit,
                    EHB = benefit.BenefitInformation.EHB,
                    //StateRequiredBenefit = benefit.BenefitInformation.StateRequiredBenefit,
                };
                list.Add(item);
            }
            return list;
        }
        private List<dynamic> GetBenefitGeneralInformationList(IList<Benefit> benefitList)
        {
            List<dynamic> benefitGeneralInformationList = new List<dynamic>();

            foreach (var benefit in benefitList)
            {
                dynamic item = new
                {
                    Benefits = benefit.BenefitInformation.Benefit,
                    IsCovered = benefit.BenefitInformation.GeneralInformation.IsThisBenefitCovered == "Covered" ? "Yes" : "No",
                    QuantitativeLimitonService = benefit.BenefitInformation.GeneralInformation.QuantativeLimitOfService,
                    LimitQuantity = benefit.BenefitInformation.GeneralInformation.LimitQuantity,
                    LimitUnit = benefit.BenefitInformation.GeneralInformation.LimitUnit,
                    //MinimumStay = benefit.BenefitInformation.GeneralInformation.MinimumStay,
                    Exclusions = benefit.BenefitInformation.GeneralInformation.Exclusions,
                    BenefitExplaination = benefit.BenefitInformation.GeneralInformation.BenefitExplanation,
                    EHBVarianceReason = benefit.BenefitInformation.GeneralInformation.EHBVarianceReason,
                    //SubjecttoDeductibleTier1 = benefit.BenefitInformation.DeductibleAndOutOfPocketExceptions.SubjectToDeductibleTier1,
                    // SubjectToDeductibleTier2 = benefit.BenefitInformation.DeductibleAndOutOfPocketExceptions.SubjectToDeductibleTier2,
                    ExcludedfromInNetworkMOOP = benefit.BenefitInformation.DeductibleAndOutOfPocketExceptions.ExcludedFromInNetworkMOOP,
                    ExcludedfromOutofNetworkMOOP = benefit.BenefitInformation.DeductibleAndOutOfPocketExceptions.ExcludedFromOutOfNetworkMOOP,
                    EHB = benefit.BenefitInformation.EHB,
                    //StateBenefitRequired = benefit.BenefitInformation.StateRequiredBenefit,
                };
                benefitGeneralInformationList.Add(item);
            }
            return benefitGeneralInformationList;
        }
        #endregion Build Benefit Information Details

        #region Build Cost Share Variance Details
        private List<dynamic> GetPlanCostSharingAttributesList(IList<PlanCostSharingAttributes> costSharingAttributesList)
        {
            List<dynamic> planCostSharingAttributesList = new List<dynamic>();

            foreach (var attribute in costSharingAttributesList)
            {
                dynamic item = new
                {
                    HIOSPlanIDStandardComponentVariant = attribute.HIOSPlanIDComponentAndVariant,
                    PlanMarketingName = attribute.PlanMarketingName,
                    LevelofCoverageMetalLevel = attribute.LevelOfCoverage,
                    CSRVariationType = attribute.CSRVariationType,
                    IssuerActuarialValue = attribute.IssuerActuarialValue,
                    AvCalculatorOutputNumber = attribute.AVCalculatorOutputNumber,
                    MedicalAndDrugDeductiblesIntegrated = attribute.MedicalAndDrugDeductiblesIntegrated,
                    MedicalAndDrugMaximumOutofPocketIntegrated = attribute.MedicalAndDrugOutOfPocketIntegrated,
                    MultipleInNetworkTiers = attribute.MultipleInNetworkTiers,
                    FirstTierUtilization = attribute.FirstTierUtilization,
                    SecondTierUtilization = attribute.SecondTierUtilization,
                };
                planCostSharingAttributesList.Add(item);
            }
            return planCostSharingAttributesList;
        }
        #endregion Build Cost Share Variance Details

        #region Build SBC Scenario Details
        private List<dynamic> GetSBCScenarioList()
        {
            List<dynamic> sbcScenarioList = new List<dynamic>();

            dynamic sbcScenario1 = new
            {
                Scenario = "Having a Baby"
            };
            sbcScenarioList.Add(sbcScenario1);

            dynamic sbcScenario2 = new
            {
                Scenario = "Having Diabetes"
            };
            sbcScenarioList.Add(sbcScenario2);

            dynamic sbcScenario3 = new
            {
                Scenario = "Treatment of a Simple Fracture"
            };
            sbcScenarioList.Add(sbcScenario3);

            return sbcScenarioList;
        }
        private List<dynamic> GetPlanSBCScenarioDetailsList(IList<PlanCostSharingAttributes> costSharingAttributesList, dynamic planSBCScenarioDetailsListElement)
        {
            List<dynamic> planSBCScenarioDetailsList = new List<dynamic>();

            foreach (var attribute in costSharingAttributesList)
            {
                dynamic item = new
                {
                    HIOSPlanIDStandardComponentVariant = attribute.HIOSPlanIDComponentAndVariant,
                    PlanMarketingName = attribute.PlanMarketingName,
                    LevelofCoverageMetalLevel = attribute.LevelOfCoverage,
                    CSRVariationType = attribute.CSRVariationType,
                    HavingDiabetesDeductible = (double.Parse(planSBCScenarioDetailsListElement[0].HavingDiabetesDeductible.ToString())).ToString(),

                    HavingDiabetesCoinsurance = (double.Parse(planSBCScenarioDetailsListElement[0].HavingDiabetesCoinsurance.ToString())).ToString(),

                    HavingDiabetesCopayment = (double.Parse(planSBCScenarioDetailsListElement[0].HavingDiabetesCopayment.ToString())).ToString(),

                    HavingDiabetesLimit = (double.Parse(planSBCScenarioDetailsListElement[0].HavingDiabetesLimit.ToString())).ToString(),
                    PlanAndBenefitSBCScenarioList = new dynamic[3]
                    {
                     new {
                             SBCScenario = "Having A Baby",
                             Deductible = attribute.SBCScenario.HavingABaby.Deductible,
                             Coinsurance = attribute.SBCScenario.HavingABaby.Coinsurance,
                             Copayment = attribute.SBCScenario.HavingABaby.Copayment,
                             Limit = attribute.SBCScenario.HavingABaby.Limit,
                         },
                     new {
                             SBCScenario = "Having Diabetes",
                             Deductible = attribute.SBCScenario.HavingDiabetes.Deductible,
                             Coinsurance = attribute.SBCScenario.HavingDiabetes.Coinsurance,
                             Copayment = attribute.SBCScenario.HavingDiabetes.Copayment,
                             Limit = attribute.SBCScenario.HavingDiabetes.Limit,
                         },
                     new {
                             SBCScenario = "Treatment of a Simple Fracture",
                             Deductible = attribute.SBCScenario.TreatmentOfSimpleFracture.Deductible,
                             Coinsurance = attribute.SBCScenario.TreatmentOfSimpleFracture.Coinsurance,
                             Copayment = attribute.SBCScenario.TreatmentOfSimpleFracture.Copayment,
                             Limit = attribute.SBCScenario.TreatmentOfSimpleFracture.Limit,
                         }
                    },
                };
                planSBCScenarioDetailsList.Add(item);
            }
            return planSBCScenarioDetailsList;
        }
        #endregion Build SBC Scenario Details

        #region Build Deductibles Details
        private List<dynamic> GetDrugDeductibleList()
        {
            List<dynamic> drugDeductibleList = new List<dynamic>();

            dynamic deductible1 = new
            {
                Type = "Medical EHB Deductible"
            };
            drugDeductibleList.Add(deductible1);

            dynamic deductible2 = new
            {
                Type = "Drug EHB Deductible"
            };
            drugDeductibleList.Add(deductible2);

            dynamic deductible3 = new
            {
                Type = "Combined Medical and Drug EHB Deductible"
            };
            drugDeductibleList.Add(deductible3);

            return drugDeductibleList;
        }
        private List<dynamic> GetPlanDrugDeductibleDetailsList(IList<PlanCostSharingAttributes> costSharingAttributesList)
        {
            List<dynamic> planDrugDeductibleDetailsList = new List<dynamic>();

            foreach (var attribute in costSharingAttributesList)
            {
                #region Add Medical EHB Deductible
                if (attribute.MedicalEHBDeductible != null)
                {
                    dynamic item = new
                    {
                        HIOSPlanIDStandardComponentVariant = attribute.HIOSPlanIDComponentAndVariant,
                        PlanMarketingName = attribute.PlanMarketingName,
                        CSRVariationType = attribute.CSRVariationType,
                        LevelofCoverageMetalLevel = attribute.LevelOfCoverage,
                        DeductibleDrugType = "Medical EHB Deductible",
                        PlanAndBenefitsTemplateNetworkList = new dynamic[4]
                        {
                            new {
                                  NetworkName = "In Network (Tier 1)",
                                  Individual = attribute.MedicalEHBDeductible.InNetworkIndividual,
                                  Family = attribute.MedicalEHBDeductible.InNetworkFamily,
                                  DefaultCoinsurance = attribute.MedicalEHBDeductible.InNetworkDefaultCoinsurance,
                            },
                            new {
                                  NetworkName = "In Network (Tier 2)",
                                  Individual = attribute.MedicalEHBDeductible.InNetworkTier2Individual,
                                  Family = attribute.MedicalEHBDeductible.InNetworkTier2Family,
                                  DefaultCoinsurance = attribute.MedicalEHBDeductible.InNetworkTier2DefaultCoinsurance,
                            },
                            new {
                                  NetworkName = "Out of Network",
                                  Individual = attribute.MedicalEHBDeductible.OutOfNetworkIndividual,
                                  Family = attribute.MedicalEHBDeductible.OutOfNetworkFamily,
                            },
                            new {
                                  NetworkName = "Combined In/Out Network",
                                  Individual = attribute.MedicalEHBDeductible.CombinedInOutNetworkIndividual,
                                  Family = attribute.MedicalEHBDeductible.CombinedInOutNetworkFamily,
                            }
                        },

                    };
                    planDrugDeductibleDetailsList.Add(item);
                }
                #endregion Add Medical EHB Deductible

                #region Add Drug EHB Deductible
                if (attribute.DrugEHBDeductible != null)
                {
                    dynamic item = new
                    {
                        HIOSPlanIDStandardComponentVariant = attribute.HIOSPlanIDComponentAndVariant,
                        PlanMarketingName = attribute.PlanMarketingName,
                        CSRVariationType = attribute.CSRVariationType,
                        LevelofCoverageMetalLevel = attribute.LevelOfCoverage,
                        DeductibleDrugType = "Drug EHB Deductible",
                        PlanAndBenefitsTemplateNetworkList = new dynamic[4]
                        {
                            new {
                                  NetworkName = "In Network (Tier 1)",
                                  Individual = attribute.DrugEHBDeductible.InNetworkIndividual,
                                  Family = attribute.DrugEHBDeductible.InNetworkFamily,
                                  DefaultCoinsurance = attribute.DrugEHBDeductible.InNetworkDefaultCoinsurance,
                            },
                            new {
                                  NetworkName = "In Network (Tier 2)",
                                  Individual = attribute.DrugEHBDeductible.InNetworkTier2Individual,
                                  Family = attribute.DrugEHBDeductible.InNetworkTier2Family,
                                  DefaultCoinsurance = attribute.DrugEHBDeductible.InNetworkTier2DefaultCoinsurance,
                            },
                            new {
                                  NetworkName = "Out of Network",
                                  Individual = attribute.DrugEHBDeductible.OutOfNetworkIndividual,
                                  Family = attribute.DrugEHBDeductible.OutOfNetworkFamily,
                            },
                            new {
                                  NetworkName = "Combined In/Out Network",
                                  Individual = attribute.DrugEHBDeductible.CombinedInOutNetworkIndividual,
                                  Family = attribute.DrugEHBDeductible.CombinedInOutNetworkFamily,
                            }
                        },

                    };
                    planDrugDeductibleDetailsList.Add(item);
                }
                #endregion Add Drug EHB Deductible

                #region Add Combined Medical & EHB Deductible
                if (attribute.CombinedMedicalEHBDeductible != null)
                {
                    dynamic item = new
                    {
                        HIOSPlanIDStandardComponentVariant = attribute.HIOSPlanIDComponentAndVariant,
                        PlanMarketingName = attribute.PlanMarketingName,
                        CSRVariationType = attribute.CSRVariationType,
                        LevelofCoverageMetalLevel = attribute.LevelOfCoverage,
                        DeductibleDrugType = "Combined Medical and Drug EHB Deductible",
                        PlanAndBenefitsTemplateNetworkList = new dynamic[4]
                        {
                            new {
                                  NetworkName = "In Network (Tier 1)",
                                  Individual = attribute.CombinedMedicalEHBDeductible.InNetworkIndividual,
                                  Family = attribute.CombinedMedicalEHBDeductible.InNetworkFamily,
                                  DefaultCoinsurance = attribute.CombinedMedicalEHBDeductible.InNetworkDefaultCoinsurance,
                            },
                            new {
                                  NetworkName = "In Network (Tier 2)",
                                  Individual = attribute.CombinedMedicalEHBDeductible.InNetworkTier2Individual,
                                  Family = attribute.CombinedMedicalEHBDeductible.InNetworkTier2Family,
                                  DefaultCoinsurance = attribute.CombinedMedicalEHBDeductible.InNetworkTier2DefaultCoinsurance,
                            },
                            new {
                                  NetworkName = "Out of Network",
                                  Individual = attribute.CombinedMedicalEHBDeductible.OutOfNetworkIndividual,
                                  Family = attribute.CombinedMedicalEHBDeductible.OutOfNetworkFamily,
                            },
                            new {
                                  NetworkName = "Combined In/Out Network",
                                  Individual = attribute.CombinedMedicalEHBDeductible.CombinedInOutNetworkIndividual,
                                  Family = attribute.CombinedMedicalEHBDeductible.CombinedInOutNetworkFamily,
                            }
                        },

                    };
                    planDrugDeductibleDetailsList.Add(item);
                }
                #endregion Add Combined Medical and Drug EHB Deductible
            }
            return planDrugDeductibleDetailsList;
        }
        private List<dynamic> GetDeductibleSubGroupList(IList<PlanCostSharingAttributes> costSharingAttributesList)
        {
            List<dynamic> deductibleSubGroupList = new List<dynamic>();

            foreach (var attribute in costSharingAttributesList)
            {
                foreach (var subgroup in attribute.DeductibleSubGroups)
                {
                    dynamic deductibleSubGroup = new
                    {
                        //Type = subgroup.GroupName,
                    };
                    deductibleSubGroupList.Add(deductibleSubGroup);
                }
                break;
            }

            return deductibleSubGroupList;
        }
        private List<dynamic> GetPlanDeductibleSubGroupDetails(IList<PlanCostSharingAttributes> costSharingAttributesList, IList<dynamic> subGroupList)
        {
            List<dynamic> planDeductibleSubGroupDetails = new List<dynamic>();

            foreach (var attribute in costSharingAttributesList)
            {
                foreach (var subgroup in attribute.DeductibleSubGroups)
                {
                    if (!string.IsNullOrEmpty(attribute.HIOSPlanIDComponentAndVariant))
                    //&& subGroupList.Where(c => c.Type == subgroup.GroupName).Any())
                    {
                        dynamic planDeductibleSubGroupDetail = new
                        {
                            HIOSPlanIDStandardComponentVariant = attribute.HIOSPlanIDComponentAndVariant,
                            PlanMarketingName = attribute.PlanMarketingName,
                            CSRVariationType = attribute.CSRVariationType,
                            //DeductibleSubGroup = subgroup.GroupName,
                            PlanAndBenefitsTemplateNetworkList = new dynamic[4]
                            {
                                new {
                                      NetworkName = "In Network (Tier 1)",
                                      Individual = subgroup.InNetworkIndividual,
                                      Family = subgroup.InNetworkFamily,
                                },
                                new {
                                      NetworkName = "In Network (Tier 2)",
                                      Individual = subgroup.InNetworkTier2Individual,
                                      Family = subgroup.InNetworkTier2Family,
                                },
                                new {
                                      NetworkName = "Out of Network",
                                      Individual = subgroup.OutOfNetworkIndividual,
                                      Family = subgroup.OutOfNetworkFamily,
                                },
                                new {
                                      NetworkName = "Combined In/Out Network",
                                      Individual = subgroup.CombinedInOutNetworkIndividual,
                                      Family = subgroup.CombinedInOutNetworkFamily,
                                }
                            },
                        };
                        planDeductibleSubGroupDetails.Add(planDeductibleSubGroupDetail);
                    }
                }
            }

            return planDeductibleSubGroupDetails;
        }
        #endregion Build Deductibles Details

        # region HSAHRADetail

        private object GetHSAHRADetailList(List<PlanCostSharingAttributes> costSharingAttributesList)
        {
            List<dynamic> planHSAHRADetailList = new List<dynamic>();
            foreach (var attribute in costSharingAttributesList)
            {
                dynamic hsahraDetail = new
                {
                    HIOSPlanIDStandardComponentVariant = attribute.HIOSPlanIDComponentAndVariant,
                    PlanMarketingName = attribute.PlanMarketingName,
                    CSRVariationType = attribute.CSRVariationType,
                    LevelofCoverageMetalLevel = attribute.LevelOfCoverage,
                    HSAEligible = attribute.HSAHRADetail.HSAEligible,
                    HSAHRAEmployerContribution = attribute.HSAHRADetail.HSAHRAEmployerContribution,
                    HSAHRAEmployerContributionAmount = attribute.HSAHRADetail.HSAHRAEmployerContributionAmount,
                };
                planHSAHRADetailList.Add(hsahraDetail);
            }
            return planHSAHRADetailList;
        }

        #endregion Build HSAHRADetail

        # region Plan Variant Level URLs
        private object GetPlanVariantLevelURLsList(List<PlanCostSharingAttributes> costSharingAttributesList)
        {
            List<dynamic> PlanVariantLevelURLsList = new List<dynamic>();
            foreach (var attribute in costSharingAttributesList)
            {
                dynamic planVariantLevelURLs = new
                {
                    HIOSPlanIDStandardComponentVariant = attribute.HIOSPlanIDComponentAndVariant,
                    PlanMarketingName = attribute.PlanMarketingName,
                    CSRVariationType = attribute.CSRVariationType,
                    LevelofCoverageMetalLevel = attribute.LevelOfCoverage,
                    URLforSummaryofBenefitsCoverage = attribute.PlanVariantLevelURLs.URLforSummaryofBenefitsCoverage,
                    PlanBrochure = attribute.PlanVariantLevelURLs.PlanBrochure,
                };
                PlanVariantLevelURLsList.Add(planVariantLevelURLs);
            }
            return PlanVariantLevelURLsList;
        }
        #endregion Plan Variant Level URLs

        #region Build Maximum Out of Pocket Details
        private List<dynamic> GetMaximumOutofPocketList()
        {
            List<dynamic> maximumOutofPocketList = new List<dynamic>();

            dynamic outofPocketMax1 = new
            {
                MaximumOutofPocketType = "Maximum Out of Pocket for Medical EHB Benefits"
            };
            maximumOutofPocketList.Add(outofPocketMax1);

            dynamic outofPocketMax2 = new
            {
                MaximumOutofPocketType = "Maximum Out of Pocket for Drug EHB Benefits"
            };
            maximumOutofPocketList.Add(outofPocketMax2);

            dynamic outofPocketMax3 = new
            {
                MaximumOutofPocketType = "Maximum Out of Pocket for Medical and Drug EHB Benefits (Total)"
            };
            maximumOutofPocketList.Add(outofPocketMax3);

            return maximumOutofPocketList;
        }
        private List<dynamic> GetPlanMaximumOutofPocketDetailsList(IList<PlanCostSharingAttributes> costSharingAttributesList)
        {
            List<dynamic> planMaximumOutofPocketDetailsList = new List<dynamic>();

            foreach (var attribute in costSharingAttributesList)
            {
                #region Add Maximum Out of Pocket for Medical EHB Benefits
                if (attribute.MaximumOutOfPocketForMedicalEHBBenefits != null)
                {
                    dynamic item = new
                    {
                        HIOSPlanIDStandardComponentVariant = attribute.HIOSPlanIDComponentAndVariant,
                        PlanMarketingName = attribute.PlanMarketingName,
                        CSRVariationType = attribute.CSRVariationType,
                        LevelofCoverageMetalLevel = attribute.LevelOfCoverage,
                        MaximumOutofPocketType = "Maximum Out of Pocket for Medical EHB Benefits",
                        PlanAndBenefitsTemplateNetworkList = new dynamic[4]
                        {
                            new {
                                  NetworkName = "In Network (Tier 1)",
                                  Individual = attribute.MaximumOutOfPocketForMedicalEHBBenefits.InNetworkIndividual,
                                  Family = attribute.MaximumOutOfPocketForMedicalEHBBenefits.InNetworkFamily,
                            },
                            new {
                                  NetworkName = "In Network (Tier 2)",
                                  Individual = attribute.MaximumOutOfPocketForMedicalEHBBenefits.InNetworkTier2Individual,
                                  Family = attribute.MaximumOutOfPocketForMedicalEHBBenefits.InNetworkTier2Family,
                            },
                            new {
                                  NetworkName = "Out of Network",
                                  Individual = attribute.MaximumOutOfPocketForMedicalEHBBenefits.OutOfNetworkIndividual,
                                  Family = attribute.MaximumOutOfPocketForMedicalEHBBenefits.OutOfNetworkFamily,
                            },
                            new {
                                  NetworkName = "Combined In/Out Network",
                                  Individual = attribute.MaximumOutOfPocketForMedicalEHBBenefits.CombinedInOutNetworkIndividual,
                                  Family = attribute.MaximumOutOfPocketForMedicalEHBBenefits.CombinedInOutNetworkFamily,
                            }
                        },

                    };
                    planMaximumOutofPocketDetailsList.Add(item);
                }
                #endregion Add Maximum Out of Pocket for Medical EHB Benefits

                #region Add Maximum Out of Pocket for Drug EHB Benefits
                if (attribute.MaximumOutOfPocketForDrugEHBBenefits != null)
                {
                    dynamic item = new
                    {
                        HIOSPlanIDStandardComponentVariant = attribute.HIOSPlanIDComponentAndVariant,
                        PlanMarketingName = attribute.PlanMarketingName,
                        CSRVariationType = attribute.CSRVariationType,
                        LevelofCoverageMetalLevel = attribute.LevelOfCoverage,
                        MaximumOutofPocketType = "Maximum Out of Pocket for Drug EHB Benefits",
                        PlanAndBenefitsTemplateNetworkList = new dynamic[4]
                        {
                            new {
                                  NetworkName = "In Network (Tier 1)",
                                  Individual = attribute.MaximumOutOfPocketForDrugEHBBenefits.InNetworkIndividual,
                                  Family = attribute.MaximumOutOfPocketForDrugEHBBenefits.InNetworkFamily,
                            },
                            new {
                                  NetworkName = "In Network (Tier 2)",
                                  Individual = attribute.MaximumOutOfPocketForDrugEHBBenefits.InNetworkTier2Individual,
                                  Family = attribute.MaximumOutOfPocketForDrugEHBBenefits.InNetworkTier2Family,
                            },
                            new {
                                  NetworkName = "Out of Network",
                                  Individual = attribute.MaximumOutOfPocketForDrugEHBBenefits.OutOfNetworkIndividual,
                                  Family = attribute.MaximumOutOfPocketForDrugEHBBenefits.OutOfNetworkFamily,
                            },
                            new {
                                  NetworkName = "Combined In/Out Network",
                                  Individual = attribute.MaximumOutOfPocketForDrugEHBBenefits.CombinedInOutNetworkIndividual,
                                  Family = attribute.MaximumOutOfPocketForDrugEHBBenefits.CombinedInOutNetworkFamily,
                            }
                        },

                    };
                    planMaximumOutofPocketDetailsList.Add(item);
                }
                #endregion Add Maximum Out of Pocket for Drug EHB Benefits

                #region Add Maximum Out of Pocket for Medical and Drug EHB Benefits (Total)
                if (attribute.MaximumOutOfPocketForMedicalAndDrugEHBBenefits != null)
                {
                    dynamic item = new
                    {
                        HIOSPlanIDStandardComponentVariant = attribute.HIOSPlanIDComponentAndVariant,
                        PlanMarketingName = attribute.PlanMarketingName,
                        CSRVariationType = attribute.CSRVariationType,
                        LevelofCoverageMetalLevel = attribute.LevelOfCoverage,
                        MaximumOutofPocketType = "Maximum Out of Pocket for Medical and Drug EHB Benefits (Total)",
                        PlanAndBenefitsTemplateNetworkList = new dynamic[4]
                        {
                            new {
                                  NetworkName = "In Network (Tier 1)",
                                  Individual = attribute.MaximumOutOfPocketForMedicalAndDrugEHBBenefits.InNetworkIndividual,
                                  Family = attribute.MaximumOutOfPocketForMedicalAndDrugEHBBenefits.InNetworkFamily,
                            },
                            new {
                                  NetworkName = "In Network (Tier 2)",
                                  Individual = attribute.MaximumOutOfPocketForMedicalAndDrugEHBBenefits.InNetworkTier2Individual,
                                  Family = attribute.MaximumOutOfPocketForMedicalAndDrugEHBBenefits.InNetworkTier2Family,
                            },
                            new {
                                  NetworkName = "Out of Network",
                                  Individual = attribute.MaximumOutOfPocketForMedicalAndDrugEHBBenefits.OutOfNetworkIndividual,
                                  Family = attribute.MaximumOutOfPocketForMedicalAndDrugEHBBenefits.OutOfNetworkFamily,
                            },
                            new {
                                  NetworkName = "Combined In/Out Network",
                                  Individual = attribute.MaximumOutOfPocketForMedicalAndDrugEHBBenefits.CombinedInOutNetworkIndividual,
                                  Family = attribute.MaximumOutOfPocketForMedicalAndDrugEHBBenefits.CombinedInOutNetworkFamily,
                            }
                        },

                    };
                    planMaximumOutofPocketDetailsList.Add(item);
                }
                #endregion Add Maximum Out of Pocket for Medical and Drug EHB Benefits (Total)
            }
            return planMaximumOutofPocketDetailsList;
        }
        #endregion Build Maximum Out of Pocket Details

        #region Build Plan Benefit Information
        private List<dynamic> GetPlanBenefitDetailsList(IList<PlanCostSharingAttributes> costSharingAttributesList)
        {
            List<dynamic> benefitInformationList = new List<dynamic>();

            foreach (var attribute in costSharingAttributesList)
            {
                foreach (var benefitInformation in attribute.CostSharingBenefitServices)
                {
                    dynamic item = new
                    {
                        HIOSPlanIDStandardComponentVariant = attribute.HIOSPlanIDComponentAndVariant,
                        PlanMarketingName = attribute.PlanMarketingName,
                        LevelofCoverageMetalLevel = attribute.LevelOfCoverage,
                        CSRVariationType = attribute.CSRVariationType,
                        Benefit = benefitInformation.ServiceName,
                        PlanAndBenefitsTemplateNetworkList = new dynamic[3]
                        {
                            new
                            {
                                NetworkName = "In Network (Tier 1)",
                                Copay = benefitInformation.InNetworkCopay,
                                Coinsurance = benefitInformation.InNetworkCoinsurance,
                            },
                            new
                            {
                                NetworkName = "In Network (Tier 2)",
                                Copay = benefitInformation.InNetworkTier2Copay,
                                Coinsurance = benefitInformation.InNetworkTier2Coinsurance,
                            },
                            new
                            {
                                NetworkName = "Out of Network",
                                Copay = benefitInformation.OutOfNetworkCopay,
                                Coinsurance = benefitInformation.OutOfNetworkCoinsurance,
                            },
                        },
                    };

                    benefitInformationList.Add(item);
                }
            }
            return benefitInformationList;
        }
        #endregion Build Plan Benefit Information
        #endregion Private Methods
    }
}
