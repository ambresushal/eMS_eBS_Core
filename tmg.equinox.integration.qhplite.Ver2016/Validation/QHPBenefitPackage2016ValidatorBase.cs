using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.qhplite.Ver2016.QHPModel;

namespace tmg.equinox.integration.qhplite.Ver2016.Validation
{
    public abstract class QHPPackageValidatorBase2016
    {
        protected PlanBenefitPackage _package;
        public abstract List<QHPValidationError> ValidateQHPPackage(PlanBenefitPackage package);

        protected string _planID;
        protected string _state;
        protected string _dentalOnly;
        protected string _market;

        protected int _silverCount;
        protected int _goldCount;
        protected bool _qhpIndicator;
        protected bool _isReferralRequiredForSpecialist;
        protected string _metalLevel;
        protected List<string> _planIDs;
        protected string _planType;

        #region "validate header"
        protected virtual void ValidateForNoPlans(ref List<QHPValidationError> errors)
        {
            if (_package.Benefits == null || _package.Benefits.Count == 0)
            {
                errors.AddError(QHPBenefitPackage2016ErrorStrings.NoPlans);
            }
        }
        protected virtual void ValidatePackageHeader(ref List<QHPValidationError> errors)
        {
            ValidateHIOSIssuerID(ref errors);
            ValidateIssuerState(ref errors);
            ValidateMarketCoverage(ref errors);
            ValidateDentalOnly(ref errors);
            ValidateTINNumber(ref errors);
        }
        protected virtual void ValidateHIOSIssuerID(ref List<QHPValidationError> errors)
        {
            string issuerID = _package.HIOSIssuerID;
            int result;
            if (String.IsNullOrEmpty(issuerID))
            {
                errors.AddError(" ", QHPBenefitPackage2016ErrorStrings.IssuerIDInvalid);
            }
            else
            {
                if (issuerID.Length != 5 || !int.TryParse(issuerID, out result))
                {
                    errors.AddError(issuerID, QHPBenefitPackage2016ErrorStrings.IssuerIDInvalid);
                }
            }
        }
        protected virtual void ValidateIssuerState(ref List<QHPValidationError> errors)
        {
            string issuerState = _package.IssuerState;
            _state = issuerState;
            if (String.IsNullOrEmpty(issuerState))
            {
                errors.AddError(" ", QHPBenefitPackage2016ErrorStrings.FromListInvalid);
            }
            else
            {
                if (issuerState.Length != 2 || !QHPNames2016.IssuerStates.Contains(issuerState))
                {
                    errors.AddError(issuerState, QHPBenefitPackage2016ErrorStrings.FromListInvalid);
                }
            }
        }
        protected virtual void ValidateMarketCoverage(ref List<QHPValidationError> errors)
        {
            string marketCoverage = _package.MarketCoverage;
            _market = marketCoverage;
            if (String.IsNullOrEmpty(marketCoverage))
            {
                errors.AddError(" ", QHPBenefitPackage2016ErrorStrings.FromListInvalid);
            }
            else
            {
                if (!QHPNames2016.MarketCoverages.Contains(marketCoverage))
                {
                    errors.AddError(marketCoverage, QHPBenefitPackage2016ErrorStrings.FromListInvalid);
                }
            }
        }
        protected virtual void ValidateDentalOnly(ref List<QHPValidationError> errors)
        {
            string dentalOnly = _package.DentalOnlyPlan;
            _dentalOnly = dentalOnly;
            if (String.IsNullOrEmpty(dentalOnly))
            {
                errors.AddError(" ", QHPBenefitPackage2016ErrorStrings.FromListInvalid);
            }
            else
            {
                if (!QHPNames2016.YesNo.Contains(dentalOnly))
                {
                    errors.AddError(dentalOnly, QHPBenefitPackage2016ErrorStrings.FromListInvalid);
                }
            }
        }
        protected virtual void ValidateTINNumber(ref List<QHPValidationError> errors)
        {
            string tinNumber = _package.TIN;
            if (String.IsNullOrEmpty(tinNumber))
            {
                errors.AddError(" ", QHPBenefitPackage2016ErrorStrings.TINInvalid);
            }
            else
            {
                string regexPattern = "[0-9]{2}-[0-9]{7}$";
                if (System.Text.RegularExpressions.Regex.IsMatch(tinNumber, regexPattern))
                {
                    string tinLeft = tinNumber.Split('-')[0];
                    if (QHPNames2016.InvalidTins.Contains(tinLeft))
                    {
                        errors.AddError(tinNumber, QHPBenefitPackage2016ErrorStrings.TINInvalid);
                    }
                }
                else
                {
                    errors.AddError(tinNumber, QHPBenefitPackage2016ErrorStrings.TINInvalid);
                }
            }
        }
        #endregion

        #region "validate plans"
        protected virtual void ValidatePlans(ref List<QHPValidationError> errors)
        {
            if (_package.PlanIdentifiers != null && _package.PlanIdentifiers.Count > 0)
            {
                _planIDs = (from pl in _package.PlanIdentifiers select pl.HIOSPlanID).ToList();
                foreach (PlanIdentifier plan in _package.PlanIdentifiers)
                {
                    if (String.IsNullOrEmpty(_planID))
                    {
                        _planID = plan.HIOSPlanID;
                    }
                    else
                    {
                        if (plan.HIOSPlanID == _planID)
                        {
                            errors.AddError(_planID, QHPBenefitPackage2016ErrorStrings.DuplicatePlanId);
                        }
                    }
                    ValidatePlan(plan, ref errors);
                }
            }
        }
        protected virtual void ValidatePlan(PlanIdentifier plan, ref List<QHPValidationError> errors)
        {
            this.ValidatePlanID(plan, ref errors);
            this.ValidatePlanMarketingName(plan, ref errors);
            this.ValidateProductID(plan, ref errors);
            this.ValidateHPID(plan, ref errors);
            this.ValidateNetworkID(plan, ref errors);
            this.ValidateServiceAreaID(plan, ref errors);
            this.ValidateFormularyID(plan, ref errors);
            this.ValidateNewExistingPlan(plan, ref errors);
            this.ValidatePlanType(plan, ref errors);
            this.ValidateMetalLevel(plan, ref errors);
            this.ValidateYesNoFields(plan, ref errors);
            this.ValidateQHPNonQHP(plan, ref errors);
            this.ValidateSpecialistForReferral(plan, ref errors);
            this.ValidateLimitedCostSharingPlanVariation(plan, ref errors);
            this.ValidateChildOnlyOffering(plan, ref errors);
            this.ValidateDiseaseManagementProgramsOffered(plan, ref errors);
            this.ValidateEHBApportionmentForPediatricDental(plan, ref errors);
            this.ValidateGuaranteedVsEstimatedRates(plan, ref errors);
            //this.ValidateMaxCoinsuranceForSpecialtyDrugs(plan, ref errors);
            //this.ValidateAVCalculatorFields(plan, ref errors);
            this.ValidatePlanEffectiveDate(plan, ref errors);
            this.ValidatePlanExpirationDate(plan, ref errors);
            this.ValidateGeographicCoverage(plan, ref errors);
        }
        protected virtual void ValidatePlanID(PlanIdentifier plan, ref List<QHPValidationError> errors)
        {
            string hiosPlanID = plan.HIOSPlanID;
            if (String.IsNullOrEmpty(hiosPlanID))
            {
                errors.AddError(" ", QHPBenefitPackage2016ErrorStrings.PlanIDInvalid);
            }
            else
            {
                string regexPattern = "[0-9]{5}[a-zA-Z]{2}[0-9]{7}$";
                if (!System.Text.RegularExpressions.Regex.IsMatch(hiosPlanID, regexPattern))
                {
                    errors.AddError(hiosPlanID, QHPBenefitPackage2016ErrorStrings.PlanIDInvalid);
                }
            }
        }
        protected virtual void ValidatePlanMarketingName(PlanIdentifier plan, ref List<QHPValidationError> errors)
        {
            string marketingName = plan.PlanMarketingName;
            if (String.IsNullOrEmpty(marketingName))
            {
                errors.AddError(" ", QHPBenefitPackage2016ErrorStrings.MarketingNameInvalid);
            }
        }
        protected virtual void ValidateProductID(PlanIdentifier plan, ref List<QHPValidationError> errors)
        {
            string productID = plan.HIOSProductID;
            string planID = plan.HIOSPlanID;
            //product id must include plan id
            if (String.IsNullOrEmpty(plan.HIOSProductID))
            {
                errors.AddError(" ", QHPBenefitPackage2016ErrorStrings.ProductIDInvalid);
            }
            else
            {
                if (!planID.Contains(productID))
                {
                    errors.AddError(productID, QHPBenefitPackage2016ErrorStrings.ProductIDNoPlanIDInvalid);
                }
                else
                {
                    string regexPattern = "[0-9]{5}[a-zA-Z]{2}[0-9]{3}$";
                    if (!System.Text.RegularExpressions.Regex.IsMatch(productID, regexPattern))
                    {
                        errors.AddError(productID, QHPBenefitPackage2016ErrorStrings.ProductIDInvalid);
                    }
                }
            }
        }
        protected virtual void ValidateHPID(PlanIdentifier plan, ref List<QHPValidationError> errors)
        {
            string hpID = plan.HPID;
            if (!String.IsNullOrEmpty(hpID))
            {
                long result;
                if (hpID.Length != 10 || !long.TryParse(hpID, out result))
                {
                    errors.AddError(hpID, QHPBenefitPackage2016ErrorStrings.HPIDInvalid);
                }
            }
        }
        protected virtual void ValidateNetworkID(PlanIdentifier plan, ref List<QHPValidationError> errors)
        {
            string networkID = plan.NetworkID;
            if (String.IsNullOrEmpty(networkID))
            {
                errors.AddError(" ", QHPBenefitPackage2016ErrorStrings.NetworkIDInvalid);
            }
            else
            {
                string regexPattern = _state + "N[0-9]{3}$";
                if (!System.Text.RegularExpressions.Regex.IsMatch(networkID, regexPattern))
                {
                    errors.AddError(networkID, QHPBenefitPackage2016ErrorStrings.NetworkIDInvalid);
                }
            }
        }
        protected virtual void ValidateServiceAreaID(PlanIdentifier plan, ref List<QHPValidationError> errors)
        {
            string serviceAreaID = plan.ServiceAreaID;
            if (String.IsNullOrEmpty(serviceAreaID))
            {
                errors.AddError(" ", QHPBenefitPackage2016ErrorStrings.ServiceAreaIDInvalid);
            }
            else
            {
                string regexPattern = _state + "S[0-9]{3}$";
                if (!System.Text.RegularExpressions.Regex.IsMatch(serviceAreaID, regexPattern))
                {
                    errors.AddError(serviceAreaID, QHPBenefitPackage2016ErrorStrings.ServiceAreaIDInvalid);
                }
            }
        }
        protected virtual void ValidateFormularyID(PlanIdentifier plan, ref List<QHPValidationError> errors)
        {
            string formularyID = plan.FormularyID;
            if (String.IsNullOrEmpty(formularyID))
            {
                errors.AddError(" ", QHPBenefitPackage2016ErrorStrings.FormularyIDInvalid);
            }
            else
            {
                string regexPattern = _state + "F[0-9]{3}$";
                if (!System.Text.RegularExpressions.Regex.IsMatch(formularyID, regexPattern))
                {
                    errors.AddError(formularyID, QHPBenefitPackage2016ErrorStrings.FormularyIDInvalid);
                }
            }
        }
        protected virtual void ValidateNewExistingPlan(PlanIdentifier plan, ref List<QHPValidationError> errors)
        {
            string newExistingPlan = plan.PlanAttributes.NewExistingPlan;
            if (String.IsNullOrEmpty(newExistingPlan))
            {
                errors.AddError(" ", QHPBenefitPackage2016ErrorStrings.FromListInvalid);
            }
            else
            {
                if (!QHPNames2016.NewExistingPlans.Contains(newExistingPlan))
                {
                    errors.AddError(newExistingPlan, QHPBenefitPackage2016ErrorStrings.FromListInvalid);
                }
            }
        }
        protected virtual void ValidatePlanType(PlanIdentifier plan, ref List<QHPValidationError> errors)
        {
            string planType = plan.PlanAttributes.PlanType;
            _planType = planType;
            if (String.IsNullOrEmpty(planType))
            {
                errors.AddError(" ", QHPBenefitPackage2016ErrorStrings.FromListInvalid);
            }
            else
            {
                if (!QHPNames2016.PlanTypes.Contains(planType))
                {
                    errors.AddError(planType, QHPBenefitPackage2016ErrorStrings.FromListInvalid);
                }
            }
        }
        protected virtual void ValidateMetalLevel(PlanIdentifier plan, ref List<QHPValidationError> errors)
        {
            string metalLevel = plan.PlanAttributes.LevelOfCoverage;
            _metalLevel = metalLevel;
            if (String.IsNullOrEmpty(metalLevel))
            {
                errors.AddError(" ", QHPBenefitPackage2016ErrorStrings.FromListInvalid);
            }
            else
            {
                if (_dentalOnly == "Yes")
                {
                    if (!QHPNames2016.MetalLevelsDentalOnly.Contains(metalLevel))
                    {
                        errors.AddError(metalLevel, QHPBenefitPackage2016ErrorStrings.FromListInvalid);
                    }
                }
                else
                {
                    if (!QHPNames2016.MetalLevels.Contains(metalLevel))
                    {
                        errors.AddError(metalLevel, QHPBenefitPackage2016ErrorStrings.FromListInvalid);
                    }
                    else
                    {
                        switch (metalLevel)
                        {
                            case "Silver":
                                _silverCount = _silverCount + 1;
                                break;
                            case "Gold":
                                _goldCount = _goldCount + 1;
                                break;
                        }
                    }
                }
            }
        }
        protected virtual void ValidateYesNoFields(PlanIdentifier plan, ref List<QHPValidationError> errors)
        {
            if (_dentalOnly == "No")
            {
                if (String.IsNullOrEmpty(plan.PlanAttributes.UniquePlanDesign) || !QHPNames2016.YesNo.Contains(plan.PlanAttributes.UniquePlanDesign))
                {
                    errors.AddError(plan.PlanAttributes.UniquePlanDesign, QHPBenefitPackage2016ErrorStrings.FromListInvalid);
                }
                if (String.IsNullOrEmpty(plan.PlanAttributes.NoticeRequiredForPregnancy) || !QHPNames2016.YesNo.Contains(plan.PlanAttributes.NoticeRequiredForPregnancy))
                {
                    errors.AddError(plan.PlanAttributes.NoticeRequiredForPregnancy, QHPBenefitPackage2016ErrorStrings.FromListInvalid);
                }
                if (String.IsNullOrEmpty(plan.PlanAttributes.IsAReferralRequiredForSpecialist) || !QHPNames2016.YesNo.Contains(plan.PlanAttributes.IsAReferralRequiredForSpecialist))
                {
                    errors.AddError(plan.PlanAttributes.IsAReferralRequiredForSpecialist, QHPBenefitPackage2016ErrorStrings.FromListInvalid);
                }
                else
                {
                    if (plan.PlanAttributes.IsAReferralRequiredForSpecialist == "Yes")
                    {
                        _isReferralRequiredForSpecialist = true;
                    }
                }
                if (String.IsNullOrEmpty(plan.PlanAttributes.TobaccoWellnessProgramOffered) || !QHPNames2016.YesNo.Contains(plan.PlanAttributes.TobaccoWellnessProgramOffered))
                {
                    errors.AddError(plan.PlanAttributes.TobaccoWellnessProgramOffered, QHPBenefitPackage2016ErrorStrings.FromListInvalid);
                }
            }
        }
        protected virtual void ValidateQHPNonQHP(PlanIdentifier plan, ref List<QHPValidationError> errors)
        {
            string qhpNonQhp = plan.PlanAttributes.QHPNonQHP;
            if (String.IsNullOrEmpty(qhpNonQhp))
            {
                errors.AddError(" ", QHPBenefitPackage2016ErrorStrings.FromListInvalid);
            }
            else
            {
                if (!QHPNames2016.QhpNonQhp.Contains(qhpNonQhp))
                {
                    errors.AddError(qhpNonQhp, QHPBenefitPackage2016ErrorStrings.FromListInvalid);
                }
                else
                {
                    if (qhpNonQhp == "On the Exchange")
                    {
                        _qhpIndicator = true;
                    }
                }
            }
        }
        protected virtual void ValidateSpecialistForReferral(PlanIdentifier plan, ref List<QHPValidationError> errors)
        {
            if (_dentalOnly == "No")
            {
                if (_isReferralRequiredForSpecialist == true)
                {
                    if (String.IsNullOrWhiteSpace(plan.PlanAttributes.SpecialistsRequiringAReferral))
                    {
                        errors.AddError(" ", QHPBenefitPackage2016ErrorStrings.ReferralRequiredInvalid);
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(plan.PlanAttributes.SpecialistsRequiringAReferral))
                    {
                        errors.AddError(plan.PlanAttributes.SpecialistsRequiringAReferral, QHPBenefitPackage2016ErrorStrings.ReferralNotRequiredInvalid);
                    }
                }
            }
        }
        protected virtual void ValidateLimitedCostSharingPlanVariation(PlanIdentifier plan, ref List<QHPValidationError> errors)
        {
            if (_dentalOnly == "No")
            {
                if (_market == "Individual")
                {
                    if (!String.IsNullOrWhiteSpace(plan.PlanAttributes.LimitedCostSharingPlanVariation))
                    {
                        string limitedCSPV = plan.PlanAttributes.LimitedCostSharingPlanVariation.Replace("$", "");
                        decimal result;
                        bool numeric = decimal.TryParse(limitedCSPV, out result);
                        if (numeric == false)
                        {
                            errors.AddError(limitedCSPV, QHPBenefitPackage2016ErrorStrings.LimitedCSPVNumericInvalid);
                        }
                        else if (_metalLevel == "Catastrophic" && result > 0)
                        {
                            errors.AddError(limitedCSPV, QHPBenefitPackage2016ErrorStrings.LimitedCSPVCatastrophicInvalid);
                        }
                    }
                }
            }
        }

        protected virtual void ValidateChildOnlyOffering(PlanIdentifier plan, ref List<QHPValidationError> errors)
        {
            string childOnlyOffering = plan.PlanAttributes.ChildOnlyOffering;
            string childOnlyPlanID = plan.PlanAttributes.ChildOnlyPlanID;
            if (String.IsNullOrWhiteSpace(childOnlyOffering))
            {
                errors.AddError(" ", QHPBenefitPackage2016ErrorStrings.FromListInvalid);
            }
            else if (!QHPNames2016.ChildOnlyOfferings.Contains(childOnlyOffering))
            {
                errors.AddError(childOnlyOffering, QHPBenefitPackage2016ErrorStrings.FromListInvalid);
            }
            else
            {
                if (_dentalOnly == "Yes" && childOnlyOffering == "Allows Adult-Only")
                {
                    errors.AddError(childOnlyOffering, QHPBenefitPackage2016ErrorStrings.ChildOnlyOfferingInvalid);
                }
                if (_dentalOnly == "No")
                {
                    if (childOnlyOffering == "Allows Adult-Only")
                    {
                        if (String.IsNullOrWhiteSpace(childOnlyPlanID))
                        {
                            errors.AddError(" ", QHPBenefitPackage2016ErrorStrings.ChildOnlyPlanIDRequiredInvalid);
                        }
                        else if (childOnlyPlanID == _planID)
                        {
                            errors.AddError(childOnlyPlanID, QHPBenefitPackage2016ErrorStrings.ChildOnlyPlanIDNoMatchInvalid);
                        }
                        else if (!_planIDs.Contains(childOnlyPlanID))
                        {
                            errors.AddError(childOnlyPlanID, QHPBenefitPackage2016ErrorStrings.ChildOnlyPlanIDInPlansInvalid);
                        }
                    }
                    if (childOnlyOffering == "Allows Adult and Child-Only")
                    {
                        if (!String.IsNullOrEmpty(childOnlyPlanID))
                        {
                            errors.AddError(childOnlyPlanID, QHPBenefitPackage2016ErrorStrings.ChildOnlyPlanIDNotRequiredInvalid);
                        }
                    }
                    if (childOnlyOffering == "Allows Child-Only")
                    {
                        if (!String.IsNullOrEmpty(childOnlyPlanID))
                        {
                            errors.AddError(childOnlyPlanID, QHPBenefitPackage2016ErrorStrings.ChildOnlyPlanIDNotRequiredCOInvalid);
                        }
                    }
                }
            }
        }
        protected virtual void ValidateDiseaseManagementProgramsOffered(PlanIdentifier plan, ref List<QHPValidationError> errors)
        {
            if (_dentalOnly == "No")
            {
                string diseaseProgramsOffered = plan.PlanAttributes.DiseaseManagementProgramsOffered;
                if (!String.IsNullOrEmpty(diseaseProgramsOffered))
                {
                    string[] programs = diseaseProgramsOffered.Split(',');
                    foreach (string program in programs)
                    {
                        if (!string.IsNullOrEmpty(program))
                        {
                            if (!QHPNames2016.DiseasePrograms.Contains(program.Trim()))
                            {
                                errors.AddError(program, QHPBenefitPackage2016ErrorStrings.DiseaseProgramsInvalid);
                            }
                        }
                    }
                }
            }
        }
        protected virtual void ValidateEHBApportionmentForPediatricDental(PlanIdentifier plan, ref List<QHPValidationError> errors)
        {
            string ehbApportionmentForPediatricDental = plan.StandAloneDentalOnly.EHBApportionmentForPediatricDental;
            if (_dentalOnly == "Yes")
            {
                int result;
                if (String.IsNullOrEmpty(ehbApportionmentForPediatricDental) || !int.TryParse(ehbApportionmentForPediatricDental.Replace("$", ""), out result))
                {
                    ehbApportionmentForPediatricDental = String.IsNullOrEmpty(ehbApportionmentForPediatricDental) ? " " : ehbApportionmentForPediatricDental;
                    errors.AddError(ehbApportionmentForPediatricDental, QHPBenefitPackage2016ErrorStrings.EHBApportionmentForPediatricDentalReqInvalid);
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(ehbApportionmentForPediatricDental))
                {
                    errors.AddError(ehbApportionmentForPediatricDental, QHPBenefitPackage2016ErrorStrings.EHBApportionmentForPediatricDentalNotReqInvalid);
                }
            }
        }
        protected virtual void ValidateGuaranteedVsEstimatedRates(PlanIdentifier plan, ref List<QHPValidationError> errors)
        {
            string guaranteedVersusEstimatedRate = plan.StandAloneDentalOnly.GuaranteedVsEstimatedRate;
            if (_dentalOnly == "Yes")
            {
                if (String.IsNullOrEmpty(guaranteedVersusEstimatedRate) || !QHPNames2016.GuaranteedVsEstimatedRate.Contains(guaranteedVersusEstimatedRate))
                {
                    guaranteedVersusEstimatedRate = String.IsNullOrEmpty(guaranteedVersusEstimatedRate) ? " " : guaranteedVersusEstimatedRate;
                    errors.AddError(guaranteedVersusEstimatedRate, QHPBenefitPackage2016ErrorStrings.EHBApportionmentForPediatricDentalReqInvalid);
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(guaranteedVersusEstimatedRate))
                {
                    errors.AddError(guaranteedVersusEstimatedRate, QHPBenefitPackage2016ErrorStrings.EHBApportionmentForPediatricDentalNotReqInvalid);
                }
            }
        }
        protected virtual void ValidateMaxCoinsuranceForSpecialtyDrugs(PlanCostSharingAttributes plan, ref List<QHPValidationError> errors)
        {
            string coinsForSpecialityDrugs = plan.AVCalculatorAdditionalBenefitDesign.MaximumCoinsuranceForSpecialityDrugs;
            if (_dentalOnly == "No")
            {
                if (!String.IsNullOrEmpty(coinsForSpecialityDrugs))
                {
                    string regexPattern = @"\$[0-9]{1,10}";
                    if (!System.Text.RegularExpressions.Regex.IsMatch(coinsForSpecialityDrugs, regexPattern))
                    {
                        errors.AddError(coinsForSpecialityDrugs, QHPBenefitPackage2016ErrorStrings.CoinsForSpecialityDrugsInvalid);
                    }
                }
            }
        }
        protected virtual void ValidateAVCalculatorFields(PlanCostSharingAttributes plan, ref List<QHPValidationError> errors)
        {
            if (_dentalOnly == "No")
            {
                string maxDaysCIP = plan.AVCalculatorAdditionalBenefitDesign.MaximumNumberOfDaysForChargingInpatientCopay;
                string beginPCSNoOfVisits = plan.AVCalculatorAdditionalBenefitDesign.BeginPrimaryCostSharingAfterSetNumberOfVisits;
                string beginPCDNoOfCopays = plan.AVCalculatorAdditionalBenefitDesign.BeginPrimaryCareDedCoAfterSetNumberOfCopays;
                string regexPattern = @"[1-9]|10";
                if (!String.IsNullOrEmpty(maxDaysCIP))
                {
                    if (!System.Text.RegularExpressions.Regex.IsMatch(maxDaysCIP, regexPattern))
                    {
                        errors.AddError(maxDaysCIP, QHPBenefitPackage2016ErrorStrings.AVCalculatorDesignInvalid);
                    }
                }
                if (!String.IsNullOrEmpty(beginPCSNoOfVisits))
                {
                    if (!System.Text.RegularExpressions.Regex.IsMatch(beginPCSNoOfVisits, regexPattern))
                    {
                        errors.AddError(beginPCSNoOfVisits, QHPBenefitPackage2016ErrorStrings.AVCalculatorDesignInvalid);
                    }
                }
                if (!String.IsNullOrEmpty(beginPCDNoOfCopays))
                {
                    if (!System.Text.RegularExpressions.Regex.IsMatch(beginPCDNoOfCopays, regexPattern))
                    {
                        errors.AddError(beginPCDNoOfCopays, QHPBenefitPackage2016ErrorStrings.AVCalculatorDesignInvalid);
                    }
                }
            }
        }
        protected virtual void ValidatePlanEffectiveDate(PlanIdentifier plan, ref List<QHPValidationError> errors)
        {
            string planEffectiveDate = plan.PlanDates.PlanEffectiveDate;
            DateTime dtPED;
            if (string.IsNullOrEmpty(planEffectiveDate))
            {
                errors.AddError(planEffectiveDate, QHPBenefitPackage2016ErrorStrings.PlanEffectiveDateInvalid);
            }
            else if (!DateTime.TryParse(planEffectiveDate, out dtPED))
            {
                errors.AddError(planEffectiveDate, QHPBenefitPackage2016ErrorStrings.PlanEffectiveDateInvalid);
            }
            else
            {
                if (_planType == "On the Exchange" || _planType == "Both")
                {
                    if (dtPED.Day != 1 && dtPED.Month != 1)
                    {
                        errors.AddError(planEffectiveDate, QHPBenefitPackage2016ErrorStrings.PlanEffectiveDate0101Invalid);
                    }
                }
            }
        }
        protected virtual void ValidatePlanExpirationDate(PlanIdentifier plan, ref List<QHPValidationError> errors)
        {
        }
        protected virtual void ValidateGeographicCoverage(PlanIdentifier plan, ref List<QHPValidationError> errors)
        {
        }
        #endregion

        #region "validate benefits"
        protected virtual void ValidateBenefits(ref List<QHPValidationError> errors)
        {

        }
        protected virtual void ValidateBenefit(Benefit benefit, ref List<QHPValidationError> errors)
        {
            ValidateIsCovered(benefit, ref errors);
            ValidateQuantitativeLimitOnPrice(benefit, ref errors);
            ValidateLimitQuantity(benefit, ref errors);
            ValidateLimitUnit(benefit, ref errors);
            ValidateMinimumStay(benefit, ref errors);
            ValidateEHBVariance(benefit, ref errors);
            ValidateSubjectDeductibleTier1(benefit, ref errors);
            ValidateSubjectDeductibleTier2(benefit, ref errors);


        }
        protected virtual void ValidateIsCovered(Benefit benefit, ref List<QHPValidationError> errors)
        {
        }
        protected virtual void ValidateQuantitativeLimitOnPrice(Benefit benefit, ref List<QHPValidationError> errors)
        {
        }
        protected virtual void ValidateLimitQuantity(Benefit benefit, ref List<QHPValidationError> errors)
        {
        }
        protected virtual void ValidateLimitUnit(Benefit benefit, ref List<QHPValidationError> errors)
        {
        }
        protected virtual void ValidateMinimumStay(Benefit benefit, ref List<QHPValidationError> errors)
        {
        }
        protected virtual void ValidateEHBVariance(Benefit benefit, ref List<QHPValidationError> errors)
        {
        }
        protected virtual void ValidateSubjectDeductibleTier1(Benefit benefit, ref List<QHPValidationError> errors)
        {
        }
        protected virtual void ValidateSubjectDeductibleTier2(Benefit benefit, ref List<QHPValidationError> errors)
        {
        }
        protected virtual void ValidateExcludedInMOOPInNW(Benefit benefit, ref List<QHPValidationError> errors)
        {
        }
        protected virtual void ValidateExcludedInMOOPOutNW(Benefit benefit, ref List<QHPValidationError> errors)
        {
        }
        #endregion

        #region "validate cost share"
        protected virtual void ValidateCostShareEmpty(ref List<QHPValidationError> errors)
        {
        }
        protected virtual void ValidateCostShares(ref List<QHPValidationError> errors)
        {
        }
        protected virtual void ValidateCostShare(PlanCostSharingAttributes costShare, ref List<QHPValidationError> errors)
        {
            this.ValidateIssuerAV(costShare, ref errors);
            this.ValidateAVCalculatorOutputNumber(costShare, ref errors);
            this.ValidateMedicalAndDrugYesNo(costShare, ref errors);
            this.ValidateMultipleInNetworkTiers(costShare, ref errors);
            this.ValidateFirstTierUtilization(costShare, ref errors);
            this.ValidateSecondTierUtilization(costShare, ref errors);
            this.ValidateSBCDollarAmounts(costShare, ref errors);
            this.ValidateAVCalculatorFields(costShare, ref errors);
            this.ValidateMaxCoinsuranceForSpecialtyDrugs(costShare, ref errors);
            this.ValidateCostShareMOOPS(costShare, ref errors);
            this.ValidateCostShareDeductibles(costShare, ref errors);
            this.ValidateServiceVisitValues(costShare, ref errors);
            this.ValidateHSAHRAEmployerContribution(costShare, ref errors);
        }
        protected virtual void ValidateIssuerAV(PlanCostSharingAttributes costShare, ref List<QHPValidationError> errors)
        {
        }
        protected virtual void ValidateAVCalculatorOutputNumber(PlanCostSharingAttributes costShare, ref List<QHPValidationError> errors)
        {
        }
        protected virtual void ValidateMedicalAndDrugYesNo(PlanCostSharingAttributes costShare, ref List<QHPValidationError> errors)
        {
        }
        protected virtual void ValidateMultipleInNetworkTiers(PlanCostSharingAttributes costShare, ref List<QHPValidationError> errors)
        {
        }
        protected virtual void ValidateFirstTierUtilization(PlanCostSharingAttributes costShare, ref List<QHPValidationError> errors)
        {
        }
        protected virtual void ValidateSecondTierUtilization(PlanCostSharingAttributes costShare, ref List<QHPValidationError> errors)
        {
        }
        protected virtual void ValidateSBCDollarAmounts(PlanCostSharingAttributes costShare, ref List<QHPValidationError> errors)
        {
        }
        protected virtual void ValidateCostShareMOOPS(PlanCostSharingAttributes costShare, ref List<QHPValidationError> errors)
        {
        }
        protected virtual void ValidateCostShareDeductibles(PlanCostSharingAttributes costShare, ref List<QHPValidationError> errors)
        {
        }
        protected virtual void ValidateServiceVisitValues(PlanCostSharingAttributes costShare, ref List<QHPValidationError> errors)
        {
        }
        protected virtual void ValidateHSAHRAEmployerContribution(PlanCostSharingAttributes costShare, ref List<QHPValidationError> errors)
        {
            if (_dentalOnly == "No")
            {
                string hsaHra = costShare.HSAHRADetail.HSAHRAEmployerContribution;
                string hsaHraAmount = costShare.HSAHRADetail.HSAHRAEmployerContributionAmount;
                if (_market == "SHOP (Small Group)")
                {
                    if (String.IsNullOrWhiteSpace(hsaHra))
                    {
                        errors.AddError(" ", QHPBenefitPackage2016ErrorStrings.HSAHRAContribSmallGroupInvalid);
                    }
                    else if (!QHPNames2016.YesNo.Contains(hsaHra))
                    {
                        errors.AddError(hsaHra, QHPBenefitPackage2016ErrorStrings.HSAHRAContribSmallGroupInvalid);
                    }
                    else
                    {
                        if (hsaHra == "Yes")
                        {
                            int result;
                            if (!int.TryParse(hsaHraAmount, out result))
                            {
                                errors.AddError(hsaHraAmount, QHPBenefitPackage2016ErrorStrings.HSAHRAContribAmountYesInvalid);
                            }
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(hsaHraAmount))
                            {
                                errors.AddError(hsaHraAmount, QHPBenefitPackage2016ErrorStrings.HSAHRAContribAmountNoInvalid);
                            }
                        }
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(hsaHra))
                    {
                        errors.AddError(hsaHra, QHPBenefitPackage2016ErrorStrings.HSAHRAContribInvalid);
                    }
                    if (!String.IsNullOrEmpty(hsaHraAmount))
                    {
                        errors.AddError(hsaHraAmount, QHPBenefitPackage2016ErrorStrings.HSAHRAContribInvalid);
                    }
                }
                if (String.IsNullOrEmpty(costShare.HSAHRADetail.HSAEligible) || !QHPNames2016.YesNo.Contains(costShare.HSAHRADetail.HSAEligible))
                {
                    errors.AddError(costShare.HSAHRADetail.HSAEligible, QHPBenefitPackage2016ErrorStrings.FromListInvalid);
                }
            }
        }

        #endregion

    }
}
