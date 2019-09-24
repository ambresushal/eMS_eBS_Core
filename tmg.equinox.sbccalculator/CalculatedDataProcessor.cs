using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.sbccalculator.Helper;
using tmg.equinox.sbccalculator.Model;

namespace tmg.equinox.sbccalculator
{
    public class CalculatedDataProcessor
    {
        public List<BenefitReview> BRGServices;
        public List<NetworkList> networkList;
        public List<DeductibleList> dedList;
        public List<RxBenefitReview> RxBenefitReviewList;
        public string tierName, defaultSBCServices;
        public double IndividualDeductible = 0;
        public List<JToken> defaultSBCServicesList;
        public CoverageExampleWrapper coverageExampleWrapperObj = null;
        public List<CoverageExample> currentCoverageExampleList = new List<CoverageExample>();
        public string _TreatmentType = string.Empty;
        public CalculatedDataProcessor(List<BenefitReview> _brgServices, List<NetworkList> _newtworkList, List<DeductibleList> _dedList, List<RxBenefitReview> _RxBenefitReviewList, CoverageExampleWrapper _coverageExampleWrapper, string _defaultSBCServices)
        {
            this.networkList = _newtworkList;
            this.dedList = _dedList;
            this.tierName = GetINNNetworkName();
            this.RxBenefitReviewList = _RxBenefitReviewList;
            this.BRGServices = GetINTierBRGData(_brgServices);
            this.IndividualDeductible = SBCCostShareHelper.GetIndividualDeductible(this.dedList, this.tierName, false, null);
            this.coverageExampleWrapperObj = _coverageExampleWrapper;
            this.defaultSBCServicesList = JToken.Parse(_defaultSBCServices).ToList();
        }

        public List<CalculatedDataModel> Process()
        {
            List<CalculatedDataModel> calculatedDataList = SBCHelper.GetDefaultRows();
            foreach (var row in calculatedDataList)
            {
                this._TreatmentType = row.TreatmentType;
                this.currentCoverageExampleList = GetCurrentCoverageExample();
                try
                {
                    row.TotalDeductibleMemberCost = GetTotalMemberCostDeductible();
                    row.TotalCoinsuranceMemberCost = GetTotalMemberCostCoinsurance();
                    row.TotalCopayMemberCost = GetTotalMemberCostCopay();

                    row.RoundOffDeductible = SBCRoundOff.GetSBCRoundOff(row.TotalDeductibleMemberCost);
                    row.RoundOffCoinsurance = SBCRoundOff.GetSBCRoundOff(row.TotalCoinsuranceMemberCost);
                    row.RoundOffCopay = SBCRoundOff.GetSBCRoundOff(row.TotalCopayMemberCost);

                    row.HospitalCopay = GetHospitalCopay();
                    row.OtherCopay = GetOtherCopay();
                    row.SpecialistCopay = GetSpecialistCopay();

                    row.TotalMemberCost = GetTotalMemberCost(row.TotalDeductibleMemberCost, row.TotalCoinsuranceMemberCost, row.TotalCopayMemberCost);
                    row.FinalMemberCost = SBCRoundOff.GetSBCRoundOff(row.TotalMemberCost);

                    row.RoundOffLimits = GetRoundOffLimits();
                    row.OverallDeductible = SBCCostShareHelper.GetIndividualDeductible(this.dedList, this.tierName, false, null).ToString();
                    SBCHelper.FormatRow(row);
                }
                catch (Exception ex)
                {
                    Exception custex = new Exception(ex.Message + "\n" + JsonConvert.SerializeObject(row), ex);
                    bool reThrow = ExceptionPolicyWrapper.HandleException(custex, ExceptionPolicies.ExceptionShielding);
                }
            }
            return calculatedDataList;
        }

        #region Private

        private string GetINNNetworkName()
        {
            string networkTierName = string.Empty;
            if (this.networkList.Where(s => s.NetworkTier.Equals("INN")).Count() > 0)
            {
                return networkTierName = this.networkList.Where(s => s.NetworkTier.Equals("INN"))
                                    .Select(s => s.NetworkTier).FirstOrDefault();
            }
            if (this.networkList.Where(s => s.NetworkTier.Equals("INN Tier 1")).Count() > 0)
            {
                return networkTierName = this.networkList.Where(s => s.NetworkTier.Equals("INN Tier 1"))
                                    .Select(s => s.NetworkTier).FirstOrDefault();
            }
            return networkTierName;
        }

        private List<BenefitReview> GetINTierBRGData(List<BenefitReview> _brgServices)
        {
            return _brgServices.Where(s => s.NetworkTier.Equals(this.tierName)).ToList();
        }

        private string GetTotalMemberCostDeductible()
        {
            double amount = 0;

            foreach (var item in this.currentCoverageExampleList)
            {
                if (item.MemberCostDeductible != SBCConstant.NotApplicable && !string.IsNullOrEmpty(item.MemberCostDeductible))
                {
                    amount = amount + SBCHelper.ExtractValue(item.MemberCostDeductible);
                }
            }
            return SBCRoundOff.GetMathRoundOff(amount.ToString());
        }

        private string GetTotalMemberCostCoinsurance()
        {
            double amount = 0;

            foreach (var item in this.currentCoverageExampleList)
            {
                if (item.MemberCostCoinsurance != SBCConstant.NotApplicable && !string.IsNullOrEmpty(item.MemberCostCoinsurance))
                {
                    amount = amount + SBCHelper.ExtractValue(item.MemberCostCoinsurance);

                }
            }
            return SBCRoundOff.GetMathRoundOff(amount.ToString());
        }

        private string GetTotalMemberCostCopay()
        {
            string total = "0";
            double amount = 0;

            foreach (var item in this.currentCoverageExampleList)
            {
                if (item.MemberCostCopay != SBCConstant.NotApplicable && !string.IsNullOrEmpty(item.MemberCostCopay))
                {
                    amount = amount + SBCHelper.ExtractValue(item.MemberCostCopay); ;

                }
            }
            total = SBCRoundOff.GetMathRoundOff(amount.ToString());
            return total;
        }

        private string GetSpecialistCopay()
        {
            string result = "$"+SBCConstant.ZeroValue;
            string benefitServiceCode = defaultSBCServicesList.Where(s => s.SelectToken("TreatmentType").ToString()
                          .Equals(this._TreatmentType)
                          && s.SelectToken("ServiceKey").ToString()
                          .Equals("Specialist"))
                           .Select(s => s.SelectToken("BenefitServiceCode").ToString())
                           .FirstOrDefault();

            if (!string.IsNullOrEmpty(benefitServiceCode))
            {
                BenefitReview service = this.BRGServices.Where(s => s.BenefitServiceCode.Equals(benefitServiceCode))
                     .FirstOrDefault();
                if (service != null)
                {
                    result = SBCCostShareHelper.GetCostShareValue(service.Copay, service.Coinsurance);
                }
            }
            return result;
        }

        private string GetHospitalCopay()
        {
            string benefitServiceCode = "", result = "$" + SBCConstant.ZeroValue; ;

            benefitServiceCode = defaultSBCServicesList.Where(s => s.SelectToken("TreatmentType").ToString()
                                .Equals(this._TreatmentType)
                                && s.SelectToken("ServiceKey").ToString()
                                .Equals("Hospital"))
                                .Select(s => s.SelectToken("BenefitServiceCode").ToString())
                                .FirstOrDefault();

            if (!string.IsNullOrEmpty(benefitServiceCode))
            {
                BenefitReview service = this.BRGServices.Where(s => s.BenefitServiceCode.Equals(benefitServiceCode))
                     .FirstOrDefault();
                if (service != null)
                {
                    result = SBCCostShareHelper.GetCostShareValue(service.Copay, service.Coinsurance);
                }
            }
            return result;
        }

        private string GetOtherCopay()
        {
            string result = "$" + SBCConstant.ZeroValue; ;
            string benefitServiceCode = defaultSBCServicesList
                                    .Where(s => s.SelectToken("TreatmentType").ToString().Equals(this._TreatmentType)
                                     && s.SelectToken("ServiceKey").ToString().Equals("Other"))
                                     .Select(s => s.SelectToken("BenefitServiceCode").ToString())
                                    .FirstOrDefault();
            if (!string.IsNullOrEmpty(benefitServiceCode))
            {
                BenefitReview service = this.BRGServices.Where(s => s.BenefitServiceCode.Equals(benefitServiceCode))
                     .FirstOrDefault();
                if (service != null)
                {
                    result = SBCCostShareHelper.GetCostShareValue(service.Copay, service.Coinsurance);
                }
            }
            return result;
        }

        private string GetTotalMemberCost(string TotalDeductibleMemberCost, string TotalCoinsuranceMemberCost, string TotalCopayMemberCost)
        {
            double result = Convert.ToDouble(TotalDeductibleMemberCost) + Convert.ToDouble(TotalCoinsuranceMemberCost) + Convert.ToDouble(TotalCopayMemberCost);
            return result.ToString();
        }

        private string GetRoundOffLimits()
        {
            string total = "0";
            double amount = 0;
            List<CoverageExample> currentInstance = null;
            switch (this._TreatmentType)
            {
                case SBCConstant.Maternity:
                    currentInstance = this.coverageExampleWrapperObj.MaternityCoverageExample;
                    break;
                case SBCConstant.Diabetes:
                    currentInstance = this.coverageExampleWrapperObj.DiabetesCoverageExample;
                    break;
                case SBCConstant.Fracture:
                    currentInstance = this.coverageExampleWrapperObj.FractureCoverageExample;
                    break;
            }
            decimal sumOfAmount = 0;
            foreach (var item in currentInstance)
            {
                string isCovered = IsServiceCovered(item.BenefitServiceCode, item.RxService, item.RxTierType);
                
                if ((isCovered.Equals("No") || string.IsNullOrEmpty(isCovered)) && item.AllowedAmount != SBCConstant.NotApplicable && !string.IsNullOrEmpty(item.AllowedAmount))
                {
                    decimal allowAmount;
                    if (decimal.TryParse(item.AllowedAmount, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, CultureInfo.GetCultureInfo("en-US"), out allowAmount))
                    {
                        sumOfAmount = sumOfAmount + allowAmount;
                    }
                }
            }
            total = SBCRoundOff.GetSBCRoundOff(sumOfAmount.ToString());
            return total;
        }

        private List<CoverageExample> GetCurrentCoverageExample()
        {
            List<CoverageExample> currentInstance = null;
            switch (this._TreatmentType)
            {
                case SBCConstant.Maternity:
                    currentInstance = this.coverageExampleWrapperObj.MaternityCoverageExample;
                    break;
                case SBCConstant.Diabetes:
                    currentInstance = this.coverageExampleWrapperObj.DiabetesCoverageExample;
                    break;
                case SBCConstant.Fracture:
                    currentInstance = this.coverageExampleWrapperObj.FractureCoverageExample;
                    break;
            }
            currentInstance = currentInstance.Where(s => s.CostShareApplies.Equals("Yes")
                              && s.ProcessingRule != SBCConstant.NoRule
            ).ToList();
            return currentInstance;
        }

        private string IsServiceCovered(string benefitServiceCode, string rxService, string RxTierType)
        {
            string Covered = string.Empty;

            if (!string.IsNullOrEmpty(benefitServiceCode) && string.IsNullOrEmpty(rxService) && string.IsNullOrEmpty(RxTierType))
            {
                BenefitReview service = this.BRGServices.Where(s => s.BenefitServiceCode.Equals(benefitServiceCode))
                                            .FirstOrDefault();
                if (service != null)
                {
                    Covered = SBCHelper.IsCovered(service.Covered);
                }
            }

            if (string.IsNullOrEmpty(benefitServiceCode) && !string.IsNullOrEmpty(rxService) && !string.IsNullOrEmpty(RxTierType))
            {
                RxBenefitReview service = RxBenefitReviewList.Where(s => s.RxService.Equals(rxService) && s.RxTierType.Equals(RxTierType))
                             .FirstOrDefault();
                if (service != null)
                {
                    Covered = SBCHelper.IsCovered(service.Covered);
                }
            }
            return Covered;
        }
        #endregion Private
    }
}
