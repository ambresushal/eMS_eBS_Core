using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.qhplite.Ver2016.QHPModel;

namespace tmg.equinox.integration.qhplite.Ver2016.XMLExport
{
    public class PlanBenefit2014XmlGenerator : IPlanBenefitXmlGenerator  
    {
        public PlanBenefitTemplateVO GenerateXmlFromPlanBenefitPackages(List<PlanBenefitPackage> packages)
        {
            PlanBenefitTemplateVO toXml = new PlanBenefitTemplateVO();
            if(packages != null && packages.Count > 0)
            {
                toXml.packagesList = new PlanAndBenefitsPackageVO[packages.Count];
                int index = 0;
                foreach (PlanBenefitPackage pkg in packages) 
                {
                    toXml.packagesList[index] = GeneratePackage(pkg);
                    index++;
                }
            }
            return toXml;
        }

        private PlanAndBenefitsPackageVO GeneratePackage(PlanBenefitPackage documentPackage)
        {
            PlanAndBenefitsPackageVO package = new PlanAndBenefitsPackageVO();
            //generate header
            package.header = GenerateHeader(documentPackage);
            //generate benefits attributes list
            package.benefitsList = GenerateBenefitsAttributeList(documentPackage.Benefits);
            //generate plan and benefits list
            package.plansList = GeneratePlanAndBenefitsList(documentPackage);
            return package;
        }

        private HeaderVO GenerateHeader(PlanBenefitPackage package)
        {
            HeaderVO header = new HeaderVO();
            header.dentalPlanOnlyInd = GetCellValueExcelObject(package.DentalOnlyPlan);
            header.issuerId = GetCellValueExcelObject(package.HIOSIssuerID);
            header.marketCoverage = GetCellValueExcelObject(package.MarketCoverage);
            header.tin = GetCellValueExcelObject(package.TIN);
            return header;
        }

        private BenefitAttributeVO[] GenerateBenefitsAttributeList(List<Benefit> benefits)
        {
            BenefitAttributeVO[] benefitAttribs = null;
            if (benefits != null && benefits.Count > 0)
            {
                benefitAttribs = new BenefitAttributeVO[benefits.Count];
                int index=0;
                foreach (Benefit benefit in benefits) 
                {
                    benefitAttribs[index] = GenerateBenefitsAttribute(benefit);
                    index++;
                }
            }
            else 
            {
                benefitAttribs = new BenefitAttributeVO[]{};
            }
            return benefitAttribs;
        }

        private BenefitAttributeVO GenerateBenefitsAttribute(Benefit benefit)
        {
            BenefitAttributeVO benefitAttrib = new BenefitAttributeVO();
            benefitAttrib.benefitTypeCode =  GetCellValueExcelObject(benefit.BenefitInformation.Benefit);
            benefitAttrib.isEHB = GetCellValueExcelObject(benefit.BenefitInformation.EHB);
            benefitAttrib.ehbVarianceReason = GetCellValueExcelObject(benefit.BenefitInformation.GeneralInformation.EHBVarianceReason);
            benefitAttrib.excludedInNetworkMOOP = GetCellValueExcelObject(benefit.BenefitInformation.DeductibleAndOutOfPocketExceptions.ExcludedFromInNetworkMOOP);
            benefitAttrib.excludedOutOfNetworkMOOP = GetCellValueExcelObject(benefit.BenefitInformation.DeductibleAndOutOfPocketExceptions.ExcludedFromOutOfNetworkMOOP);
            benefitAttrib.exclusion = GetCellValueExcelObject(benefit.BenefitInformation.GeneralInformation.Exclusions);
            benefitAttrib.explanation = GetCellValueExcelObject(benefit.BenefitInformation.GeneralInformation.BenefitExplanation);
            benefitAttrib.isBenefitCovered = GetCellValueExcelObject(benefit.BenefitInformation.GeneralInformation.IsThisBenefitCovered);
            benefitAttrib.isStateMandate = GetCellValueExcelObject(benefit.BenefitInformation.StateRequiredBenefit);
            benefitAttrib.minimumStay = GetCellValueExcelObject(benefit.BenefitInformation.GeneralInformation.MinimumStay);
            benefitAttrib.quantityLimit = GetCellValueExcelObject(benefit.BenefitInformation.GeneralInformation.LimitQuantity);
            benefitAttrib.serviceLimit = GetCellValueExcelObject(benefit.BenefitInformation.GeneralInformation.QuantativeLimitOfService);
            //benefitAttrib.DiseaseManagementProgramsOffered= GetCellValueExcelObject(benefit.Pl)
            //benefitAttrib.subjectToDeductibleTier1 = GetCellValueExcelObject(benefit.BenefitInformation.DeductibleAndOutOfPocketExceptions.SubjectToDeductibleTier1);
           // benefitAttrib.subjectToDeductibleTier2 = GetCellValueExcelObject(benefit.BenefitInformation.DeductibleAndOutOfPocketExceptions.SubjectToDeductibleTier2);
            benefitAttrib.unitLimit = GetCellValueExcelObject(benefit.BenefitInformation.GeneralInformation.LimitUnit);
            return benefitAttrib;
        }

        private PlanAndBenefitsVO[] GeneratePlanAndBenefitsList(PlanBenefitPackage package)
        {
            PlanAndBenefitsVO[] planAndBenefits = null;
            if (package.PlanIdentifiers != null && package.PlanIdentifiers.Count > 0)
            {
                planAndBenefits = new PlanAndBenefitsVO[package.PlanIdentifiers.Count];
                int index = 0;
                foreach (PlanIdentifier planIdentifier in package.PlanIdentifiers)
                {
                    List<PlanCostSharingAttributes> csServices = new List<PlanCostSharingAttributes>();
                    if (package.PlanCostSharingAttributes != null && package.PlanCostSharingAttributes.Count > 0) 
                    {
                        var csSvs = from cs in package.PlanCostSharingAttributes
                                     where cs.HIOSPlanIDComponentAndVariant.StartsWith(planIdentifier.HIOSPlanID)
                                     select cs;
                        if(csSvs != null && csSvs.Count() > 0)
                        {
                            csServices = csSvs.ToList();
                        }
                    }
                    planAndBenefits[index] = GeneratePlanAndBenefit(planIdentifier, csServices);
                    index++;
                }
            }
            else
            {
                planAndBenefits = new PlanAndBenefitsVO[] { };
            }
            return planAndBenefits;
        }

        private PlanAndBenefitsVO GeneratePlanAndBenefit(PlanIdentifier planIdentifier, List<PlanCostSharingAttributes> costShareVariances)
        {
            PlanAndBenefitsVO planAndBenefit = new PlanAndBenefitsVO();
            //generate plan attribute
            planAndBenefit.planAttributes = GeneratePlanAttribute(planIdentifier);
            //generate cost share variance list
            planAndBenefit.costShareVariancesList = GenerateCostShareVarianceList(costShareVariances);
            return planAndBenefit;
        }

        private PlanAttributeVO GeneratePlanAttribute(PlanIdentifier planIdentifier)
        {
            PlanAttributeVO planAttribs = new PlanAttributeVO();
            planAttribs.beginPrimaryCareCostSharingAfterSetNumberVisits = GetCellValueExcelObject(planIdentifier.AVCalculatorAdditionalBenefitDesign.BeginPrimaryCostSharingAfterSetNumberOfVisits);
            planAttribs.beginPrimaryCareDeductibleOrCoinsuranceAfterSetNumberCopays = GetCellValueExcelObject(planIdentifier.AVCalculatorAdditionalBenefitDesign.BeginPrimaryCareDedCoAfterSetNumberOfCopays);
            planAttribs.childOnlyOffering = GetCellValueExcelObject(planIdentifier.PlanAttributes.ChildOnlyOffering);
            planAttribs.childOnlyPlanID = GetCellValueExcelObject(planIdentifier.PlanAttributes.ChildOnlyPlanID);
            planAttribs.ehbApportionmentForPediatricDental = GetCellValueExcelObject(planIdentifier.StandAloneDentalOnly.EHBApportionmentForPediatricDental);
            //planAttribs.empContributionAmountForHSAOrHRA = GetCellValueExcelObject(planIdentifier.PlanAttributes.HSAHRAEmployerContributionAmount);
           // planAttribs.employerHSAHRAContributionIndicator = GetCellValueExcelObject(planIdentifier.PlanAttributes.HSAHRAEmployerContribution);
            planAttribs.enrollmentPaymentURL = GetCellValueExcelObject(planIdentifier.PlanLevelURLs.URLforEnrollmentPayment);
            planAttribs.formularyID = GetCellValueExcelObject(planIdentifier.FormularyID);
            planAttribs.guaranteedVsEstimatedRate = GetCellValueExcelObject(planIdentifier.StandAloneDentalOnly.GuaranteedVsEstimatedRate);
            planAttribs.healthCareSpecialistReferralType = GetCellValueExcelObject(planIdentifier.PlanAttributes.SpecialistsRequiringAReferral);
            planAttribs.hiosProductID = GetCellValueExcelObject(planIdentifier.HIOSProductID);
            planAttribs.hpid = GetCellValueExcelObject(planIdentifier.HPID);
            //planAttribs.hsaEligibility = GetCellValueExcelObject(planIdentifier.PlanAttributes.HSAEligible);
            planAttribs.insurancePlanBenefitExclusionText = GetCellValueExcelObject(planIdentifier.PlanAttributes.PlanLevelExclusions);
            planAttribs.insurancePlanPregnancyNoticeReqInd = GetCellValueExcelObject(planIdentifier.PlanAttributes.NoticeRequiredForPregnancy);
            planAttribs.isDiseaseMgmtProgramsOffered = GetCellValueExcelObject(planIdentifier.PlanAttributes.DiseaseManagementProgramsOffered);
            planAttribs.isNewPlan = GetCellValueExcelObject(planIdentifier.PlanAttributes.NewExistingPlan);
            planAttribs.isSpecialistReferralRequired = GetCellValueExcelObject(planIdentifier.PlanAttributes.IsAReferralRequiredForSpecialist);
            planAttribs.isWellnessProgramOffered = GetCellValueExcelObject(planIdentifier.PlanAttributes.TobaccoWellnessProgramOffered);
            planAttribs.maximumCoinsuranceForSpecialtyDrugs = GetCellValueExcelObject(planIdentifier.AVCalculatorAdditionalBenefitDesign.MaximumCoinsuranceForSpecialityDrugs);
            planAttribs.maxNumDaysForChargingInpatientCopay = GetCellValueExcelObject(planIdentifier.AVCalculatorAdditionalBenefitDesign.MaximumNumberOfDaysForChargingInpatientCopay);
            planAttribs.metalLevel = GetCellValueExcelObject(planIdentifier.PlanAttributes.LevelOfCoverage); //TODO: need to determine
            planAttribs.nationalNetwork = GetCellValueExcelObject(planIdentifier.GeographicCoverage.NationalNetwork);
            planAttribs.networkID = GetCellValueExcelObject(planIdentifier.NetworkID);
            planAttribs.outOfCountryCoverage = GetCellValueExcelObject(planIdentifier.GeographicCoverage.OutOfCountryCoverage);
            planAttribs.outOfCountryCoverageDescription = GetCellValueExcelObject(planIdentifier.GeographicCoverage.OutOfCountryCoverageDescription);
            planAttribs.outOfServiceAreaCoverage = GetCellValueExcelObject(planIdentifier.GeographicCoverage.OutOfServiceAreaCoverage);
            planAttribs.outOfServiceAreaCoverageDescription = GetCellValueExcelObject(planIdentifier.GeographicCoverage.OutOfServiceAreaCoverageDescription);
            //planAttribs.planBrochure = GetCellValueExcelObject(planIdentifier.URLs.PlanBrochure);
            planAttribs.planEffectiveDate = GetCellValueExcelObject(planIdentifier.PlanDates.PlanEffectiveDate);
            planAttribs.planExpirationDate = GetCellValueExcelObject(planIdentifier.PlanDates.PlanExpirationDate);
            planAttribs.planMarketingName = GetCellValueExcelObject(planIdentifier.PlanMarketingName);
            planAttribs.planType = GetCellValueExcelObject(planIdentifier.PlanAttributes.PlanType);
            planAttribs.qhpOrNonQhp = GetCellValueExcelObject(planIdentifier.PlanAttributes.QHPNonQHP);
            planAttribs.serviceAreaID = GetCellValueExcelObject(planIdentifier.ServiceAreaID);
            planAttribs.standardComponentID = GetCellValueExcelObject(planIdentifier.HIOSPlanID); //TODO: need to determine
            //planAttribs.summaryBenefitAndCoverageURL = GetCellValueExcelObject(planIdentifier.URLs.URLForSummaryOfBenefitsAndCoverage);
            planAttribs.uniquePlanDesign = GetCellValueExcelObject(planIdentifier.PlanAttributes.UniquePlanDesign);
            return planAttribs;
        }

        private CostShareVarianceVO[] GenerateCostShareVarianceList(List<PlanCostSharingAttributes> costShareVariances)
        {
            CostShareVarianceVO[] variances = null;
            if (costShareVariances != null && costShareVariances.Count > 0)
            {
                variances = new CostShareVarianceVO[costShareVariances.Count];
                int index = 0;
                foreach (PlanCostSharingAttributes costShareVariance in costShareVariances)
                {
                    variances[index] = GetCostShareVariance(costShareVariance);
                    index++;
                }
            }
            else
            {
                variances = new CostShareVarianceVO[] { };
            }
            return variances;
        }

        private CostShareVarianceVO GetCostShareVariance(PlanCostSharingAttributes costShareVariance)
        {
            CostShareVarianceVO costShare = new CostShareVarianceVO();
            costShare.avCalculatorOutputNumber = GetCellValueExcelObject(costShareVariance.AVCalculatorOutputNumber);
            costShare.csrVariationType = GetCellValueExcelObject(costShareVariance.CSRVariationType);
            costShare.firstTierUtilization = GetCellValueExcelObject(costShareVariance.FirstTierUtilization);
            costShare.issuerActuarialValue = GetCellValueExcelObject(costShareVariance.IssuerActuarialValue);
            costShare.medicalAndDrugDeductiblesIntegrated = GetCellValueExcelObject(costShareVariance.MedicalAndDrugDeductiblesIntegrated);
            costShare.medicalAndDrugMaxOutOfPocketIntegrated = GetCellValueExcelObject(costShareVariance.MedicalAndDrugOutOfPocketIntegrated);
            costShare.metalLevel = GetCellValueExcelObject(costShareVariance.LevelOfCoverage);
            costShare.multipleProviderTiers = GetCellValueExcelObject(costShareVariance.MultipleInNetworkTiers);
            costShare.planId = GetCellValueExcelObject(costShareVariance.HIOSPlanIDComponentAndVariant);
            costShare.planMarketingName = GetCellValueExcelObject(costShareVariance.PlanMarketingName);
            costShare.firstTierUtilization = GetCellValueExcelObject(costShareVariance.FirstTierUtilization);
            costShare.secondTierUtilization = GetCellValueExcelObject(costShareVariance.SecondTierUtilization);

            //generate sbc scenario
            costShare.sbc = GenerateSBCScenario(costShareVariance.SBCScenario);
            //generate moop list
            costShare.moopList = GenerateMOOPList(costShareVariance.MaximumOutOfPocketForMedicalEHBBenefits,
                                                  costShareVariance.MaximumOutOfPocketForDrugEHBBenefits,
                                                  costShareVariance.MaximumOutOfPocketForMedicalAndDrugEHBBenefits);
            //generate plan deductible list
            costShare.planDeductibleList = GeneratePlanDeductibleList(costShareVariance.MedicalEHBDeductible,
                                                                      costShareVariance.DrugEHBDeductible,
                                                                      costShareVariance.CombinedMedicalEHBDeductible,
                                                                      costShareVariance.DeductibleSubGroups);
            //generate service visit list
            costShare.serviceVisitList = GenerateServiceVisitList(costShareVariance.CostSharingBenefitServices);
            return costShare;
        }

        private SBCVO GenerateSBCScenario(SBCScenario sbc)
        {
            SBCVO sbcScenario = new SBCVO();
            if (sbc != null) 
            {
                sbcScenario.havingBabyCoInsurance = GetCellValueExcelObject(sbc.HavingABaby.Coinsurance);
                sbcScenario.havingBabyCoPayment = GetCellValueExcelObject(sbc.HavingABaby.Copayment);
                sbcScenario.havingBabyDeductible = GetCellValueExcelObject(sbc.HavingABaby.Deductible);
                sbcScenario.havingBabyLimit = GetCellValueExcelObject(sbc.HavingABaby.Limit);
                sbcScenario.havingDiabetesCoInsurance = GetCellValueExcelObject(sbc.HavingDiabetes.Coinsurance);
                sbcScenario.havingDiabetesCopay = GetCellValueExcelObject(sbc.HavingDiabetes.Copayment);
                sbcScenario.havingDiabetesDeductible = GetCellValueExcelObject(sbc.HavingDiabetes.Deductible);
                sbcScenario.havingDiabetesLimit = GetCellValueExcelObject(sbc.HavingDiabetes.Limit);
            }
            return sbcScenario;
        }

        private MoopVO[] GenerateMOOPList(MaximumOutOfPocketForMedicalEHBBenefits medEHB, MaximumOutOfPocketForDrugEHBBenefits drugEHB, 
            MaximumOutOfPocketForMedicalAndDrugEHBBenefits  medAndDrugEHB)
        {
            MoopVO[] moopList = null;
            int validRecords = (medEHB == null ? 0 : 1) + (drugEHB == null ? 0 : 1) + (medAndDrugEHB == null ? 0 : 1);
            if (validRecords > 0)
            {
                int index = 0;
                moopList = new MoopVO[validRecords];
                if (medEHB != null)
                {
                    MoopVO moopMed = new MoopVO();
                    moopMed.combinedInOutNetworkFamilyAmount = GetCellValueExcelObject(medEHB.CombinedInOutNetworkFamily);
                    moopMed.combinedInOutNetworkIndividualAmount = GetCellValueExcelObject(medEHB.CombinedInOutNetworkIndividual);
                    moopMed.inNetworkTier1FamilyAmount = GetCellValueExcelObject(medEHB.InNetworkFamily);
                    moopMed.inNetworkTier1IndividualAmount = GetCellValueExcelObject(medEHB.InNetworkIndividual);
                    moopMed.inNetworkTier2FamilyAmount = GetCellValueExcelObject(medEHB.InNetworkTier2Family);
                    moopMed.inNetworkTier2IndividualAmount = GetCellValueExcelObject(medEHB.InNetworkTier2Individual);
                    moopMed.outOfNetworkFamilyAmount = GetCellValueExcelObject(medEHB.OutOfNetworkFamily);
                    moopMed.outOfNetworkIndividualAmount = GetCellValueExcelObject(medEHB.OutOfNetworkIndividual);
                    moopList[index] = moopMed;
                    index++;
                }
                if (drugEHB != null)
                {
                    MoopVO moopDrug = new MoopVO();
                    moopDrug.combinedInOutNetworkFamilyAmount = GetCellValueExcelObject(drugEHB.CombinedInOutNetworkFamily);
                    moopDrug.combinedInOutNetworkIndividualAmount = GetCellValueExcelObject(drugEHB.CombinedInOutNetworkIndividual);
                    moopDrug.inNetworkTier1FamilyAmount = GetCellValueExcelObject(drugEHB.InNetworkFamily);
                    moopDrug.inNetworkTier1IndividualAmount = GetCellValueExcelObject(drugEHB.InNetworkIndividual);
                    moopDrug.inNetworkTier2FamilyAmount = GetCellValueExcelObject(drugEHB.InNetworkTier2Family);
                    moopDrug.inNetworkTier2IndividualAmount = GetCellValueExcelObject(drugEHB.InNetworkTier2Individual);
                    moopDrug.outOfNetworkFamilyAmount = GetCellValueExcelObject(drugEHB.OutOfNetworkFamily);
                    moopDrug.outOfNetworkIndividualAmount = GetCellValueExcelObject(drugEHB.OutOfNetworkIndividual);
                    moopList[index] = moopDrug;
                    index++;
                }
                if (medAndDrugEHB != null)
                {
                    MoopVO moopMedAndDrug = new MoopVO();
                    moopMedAndDrug.combinedInOutNetworkFamilyAmount = GetCellValueExcelObject(medAndDrugEHB.CombinedInOutNetworkFamily);
                    moopMedAndDrug.combinedInOutNetworkIndividualAmount = GetCellValueExcelObject(medAndDrugEHB.CombinedInOutNetworkIndividual);
                    moopMedAndDrug.inNetworkTier1FamilyAmount = GetCellValueExcelObject(medAndDrugEHB.InNetworkFamily);
                    moopMedAndDrug.inNetworkTier1IndividualAmount = GetCellValueExcelObject(medAndDrugEHB.InNetworkIndividual);
                    moopMedAndDrug.inNetworkTier2FamilyAmount = GetCellValueExcelObject(medAndDrugEHB.InNetworkTier2Family);
                    moopMedAndDrug.inNetworkTier2IndividualAmount = GetCellValueExcelObject(medAndDrugEHB.InNetworkTier2Individual);
                    moopMedAndDrug.outOfNetworkFamilyAmount = GetCellValueExcelObject(medAndDrugEHB.OutOfNetworkFamily);
                    moopMedAndDrug.outOfNetworkIndividualAmount = GetCellValueExcelObject(medAndDrugEHB.OutOfNetworkIndividual);
                    moopList[index] = moopMedAndDrug;
                }
            }
            else 
            {
                moopList = new MoopVO[] { };
            }
            return moopList;
        }

        private PlanDeductibleVO[] GeneratePlanDeductibleList(MedicalEHBDeductible medEHB, DrugEHBDeductible drugEHB, 
            CombinedMedicalEHBDeductible combinedMedEHB, List<DeductibleSubGroup> dedSubGroups)
        {
            PlanDeductibleVO[] deductibleList = null;
            int validRecords = (medEHB == null ? 0 : 1) + (drugEHB == null ? 0 : 1) + (combinedMedEHB == null ? 0 : 1)
                + ((dedSubGroups == null || dedSubGroups.Count == 0) ? 0 : 1);
            if (validRecords > 0)
            {
                int index = 0;
                deductibleList = new PlanDeductibleVO[validRecords];
                if (medEHB != null)
                {
                    PlanDeductibleVO med = new PlanDeductibleVO();
                    med.coinsuranceInNetworkTier1 = GetCellValueExcelObject(medEHB.InNetworkDefaultCoinsurance);
                    med.coinsuranceInNetworkTier2 = GetCellValueExcelObject(medEHB.InNetworkTier2DefaultCoinsurance);
                    med.combinedInOrOutNetworkFamily = GetCellValueExcelObject(medEHB.CombinedInOutNetworkFamily);
                    med.combinedInOrOutNetworkIndividual = GetCellValueExcelObject(medEHB.CombinedInOutNetworkIndividual);
                    med.deductibleType = GetCellValueExcelObject("Medical EHB Deductible");
                    med.inNetworkTier1Family = GetCellValueExcelObject(medEHB.InNetworkFamily);
                    med.inNetworkTier1Individual = GetCellValueExcelObject(medEHB.InNetworkIndividual);
                    med.inNetworkTierTwoFamily = GetCellValueExcelObject(medEHB.InNetworkTier2Family);
                    med.inNetworkTierTwoIndividual = GetCellValueExcelObject(medEHB.InNetworkTier2Individual);
                    med.outOfNetworkFamily = GetCellValueExcelObject(medEHB.OutOfNetworkFamily);
                    med.outOfNetworkIndividual = GetCellValueExcelObject(medEHB.OutOfNetworkIndividual);
                    deductibleList[index] = med;
                    index++;
                }
                if (drugEHB != null) 
                {
                    PlanDeductibleVO drug = new PlanDeductibleVO();
                    drug.coinsuranceInNetworkTier1 = GetCellValueExcelObject(drugEHB.InNetworkDefaultCoinsurance);
                    drug.coinsuranceInNetworkTier2 = GetCellValueExcelObject(drugEHB.InNetworkTier2DefaultCoinsurance);
                    drug.combinedInOrOutNetworkFamily = GetCellValueExcelObject(drugEHB.CombinedInOutNetworkFamily);
                    drug.combinedInOrOutNetworkIndividual = GetCellValueExcelObject(drugEHB.CombinedInOutNetworkIndividual);
                    drug.deductibleType = GetCellValueExcelObject("Drug EHB Deductible");
                    drug.inNetworkTier1Family = GetCellValueExcelObject(drugEHB.InNetworkFamily);
                    drug.inNetworkTier1Individual = GetCellValueExcelObject(drugEHB.InNetworkIndividual);
                    drug.inNetworkTierTwoFamily = GetCellValueExcelObject(drugEHB.InNetworkTier2Family);
                    drug.inNetworkTierTwoIndividual = GetCellValueExcelObject(drugEHB.InNetworkTier2Individual);
                    drug.outOfNetworkFamily = GetCellValueExcelObject(drugEHB.OutOfNetworkFamily);
                    drug.outOfNetworkIndividual = GetCellValueExcelObject(drugEHB.OutOfNetworkIndividual);
                    deductibleList[index] = drug;
                    index++;
                }
                if (combinedMedEHB != null) 
                {
                    PlanDeductibleVO combined = new PlanDeductibleVO();
                    combined.coinsuranceInNetworkTier1 = GetCellValueExcelObject(combinedMedEHB.InNetworkDefaultCoinsurance);
                    combined.coinsuranceInNetworkTier2 = GetCellValueExcelObject(combinedMedEHB.InNetworkTier2DefaultCoinsurance);
                    combined.combinedInOrOutNetworkFamily = GetCellValueExcelObject(combinedMedEHB.CombinedInOutNetworkFamily);
                    combined.combinedInOrOutNetworkIndividual = GetCellValueExcelObject(combinedMedEHB.CombinedInOutNetworkIndividual);
                    combined.deductibleType = GetCellValueExcelObject("Combined Medical and Drug EHB Deductible");
                    combined.inNetworkTier1Family = GetCellValueExcelObject(combinedMedEHB.InNetworkFamily);
                    combined.inNetworkTier1Individual = GetCellValueExcelObject(combinedMedEHB.InNetworkIndividual);
                    combined.inNetworkTierTwoFamily = GetCellValueExcelObject(combinedMedEHB.InNetworkTier2Family);
                    combined.inNetworkTierTwoIndividual = GetCellValueExcelObject(combinedMedEHB.InNetworkTier2Individual);
                    combined.outOfNetworkFamily = GetCellValueExcelObject(combinedMedEHB.OutOfNetworkFamily);
                    combined.outOfNetworkIndividual = GetCellValueExcelObject(combinedMedEHB.OutOfNetworkIndividual);
                    deductibleList[index] = combined;
                    index++;
                }

                if (dedSubGroups != null && dedSubGroups.Count > 0) 
                {
                    foreach (DeductibleSubGroup subGroup in dedSubGroups) 
                    {
                        PlanDeductibleVO subGroupDeductible = new PlanDeductibleVO();
                        subGroupDeductible.combinedInOrOutNetworkFamily = GetCellValueExcelObject(subGroup.CombinedInOutNetworkFamily);
                        subGroupDeductible.combinedInOrOutNetworkIndividual = GetCellValueExcelObject(subGroup.CombinedInOutNetworkIndividual);
                       // subGroupDeductible.deductibleType = GetCellValueExcelObject(subGroup.GroupName);
                        subGroupDeductible.inNetworkTier1Family = GetCellValueExcelObject(subGroup.InNetworkFamily);
                        subGroupDeductible.inNetworkTier1Individual = GetCellValueExcelObject(subGroup.InNetworkIndividual);
                        subGroupDeductible.inNetworkTierTwoFamily = GetCellValueExcelObject(subGroup.InNetworkTier2Family);
                        subGroupDeductible.inNetworkTierTwoIndividual = GetCellValueExcelObject(subGroup.InNetworkTier2Individual);
                        subGroupDeductible.outOfNetworkFamily = GetCellValueExcelObject(subGroup.OutOfNetworkFamily);
                        subGroupDeductible.outOfNetworkIndividual = GetCellValueExcelObject(subGroup.OutOfNetworkIndividual);
                        deductibleList[index] = subGroupDeductible;
                        index++;
                    }
                }
            }
            else 
            {
                deductibleList = new PlanDeductibleVO[] { };
            }
            return deductibleList;
        }

        private ServiceVisitVO[] GenerateServiceVisitList(List<CostSharingBenefitService> services)
        {
            ServiceVisitVO[] serviceVisits = null;
            if (services != null && services.Count > 0)
            {
                serviceVisits = new ServiceVisitVO[services.Count];
                int index = 0;
                foreach (CostSharingBenefitService service in services)
                {
                    serviceVisits[index] = GenerateServiceVisit(service);
                    index++;
                }
            }
            else
            {
                serviceVisits = new ServiceVisitVO[] { };
            }
            return serviceVisits;
        }

        private ServiceVisitVO GenerateServiceVisit(CostSharingBenefitService service)
        {
            ServiceVisitVO serviceVisit = new ServiceVisitVO();
            serviceVisit.coInsuranceInNetworkTier1 = GetCellValueExcelObject(service.InNetworkCoinsurance);
            serviceVisit.coInsuranceInNetworkTier2 = GetCellValueExcelObject(service.InNetworkTier2Coinsurance);
            serviceVisit.coInsuranceOutOfNetwork = GetCellValueExcelObject(service.OutOfNetworkCoinsurance);
            serviceVisit.copayInNetworkTier1 = GetCellValueExcelObject(service.InNetworkCopay);
            serviceVisit.copayInNetworkTier2 = GetCellValueExcelObject(service.InNetworkTier2Copay);
            serviceVisit.copayOutOfNetwork = GetCellValueExcelObject(service.OutOfNetworkCopay);
            serviceVisit.visitType = GetCellValueExcelObject(service.ServiceName);
            return serviceVisit;
        }

        private ExcelCellVO GetCellValueExcelObject(string value) 
        {
            return new ExcelCellVO {cellValue= value};
        }
    }
}
