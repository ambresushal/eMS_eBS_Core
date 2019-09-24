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
    public class SbcCalculatorProcessor
    {
        private List<BenefitReview> _bRGServices;
        private List<CoverageExample> _masterListlServices, _existingCoverageExampleList;
        private Dictionary<string, double> DateCategorykeys = new Dictionary<string, double>();
        private double _individualDeductible = 0, rxIndividualDeductible=0;
        private List<NetworkList> _networkList;
        private List<DeductibleList> _dedList;
        private List<RxBenefitReview> _rxBenefitReviewList;
        private string _coverageType = string.Empty, _tierName = string.Empty, _key = string.Empty;
        private List<RxCostShare> rxCostShareList;

        public SbcCalculatorProcessor(List<BenefitReview> _brgServices, List<CoverageExample> _masterListlServices, List<NetworkList> _newtworkList, List<DeductibleList> _dedList, List<RxBenefitReview> _RxBenefitReviewList, List<CoverageExample> _existingCoverageExampleList, string _coverageType, List<RxCostShare> _rxCostShareList)
        {
            this._masterListlServices = _masterListlServices;
            this._networkList = _newtworkList;
            this._dedList = _dedList;
            this.rxCostShareList = _rxCostShareList;
            this._tierName = GetINNNetworkName();
            this._rxBenefitReviewList = _RxBenefitReviewList;
            this._bRGServices = GetINTierBRGData(_brgServices);
            this._individualDeductible = SBCCostShareHelper.GetIndividualDeductible(this._dedList, this._tierName, false, this.rxCostShareList);
            this._existingCoverageExampleList = _existingCoverageExampleList;
            this._coverageType = _coverageType;
            this.rxIndividualDeductible = this._individualDeductible;
        }

        #region Public

        public List<CoverageExample> Process()
        {
            this._masterListlServices = this._masterListlServices
                                       .Where(s => s.CoverageType.Equals(_coverageType))
                                       .OrderBy(t => Convert.ToInt32(t.Sequence))
                                       .ToList();

            foreach (var item in this._masterListlServices)
            {
                try
                {
                    SetCostShareValues(item);
                    SetProcessingRule(item);
                    ProcessSBCCoverageService(item);
                    SBCHelper.FormatRow(item);
                }
                catch (Exception ex)
                {
                    Exception custex = new Exception(ex.Message + "\n" + JsonConvert.SerializeObject(item), ex);
                    bool reThrow = ExceptionPolicyWrapper.HandleException(custex, ExceptionPolicies.ExceptionShielding);
                }
            }
            return this._masterListlServices;
        }

        public void SetCostShareValues(CoverageExample example)
        {
            SBCHelper.SetDefaultCostShareValues(example);
            if (!string.IsNullOrEmpty(example.BenefitServiceCode))
            {
                BenefitReview service = this._bRGServices
                                        .Where(s => s.BenefitServiceCode.Equals(example.BenefitServiceCode))
                                        .FirstOrDefault();

                if (service != null)
                {
                    example.Copay = SBCCostShareHelper.GetCopay(service.Copay).Trim();
                    example.Coinsurance = SBCCostShareHelper.GetCoinsurance(service.Coinsurance).Trim();
                    example.Deductible = SBCCostShareHelper.GetDeductible(service.IndDeductible).Trim();
                    example.Covered = SBCHelper.IsCovered(service.Covered);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(example.RxService) && !string.IsNullOrEmpty(example.RxTierType))
                {
                    RxBenefitReview Service = _rxBenefitReviewList
                                         .Where(s => s.Network.Equals(_tierName)
                                         && s.RxService.Equals(example.RxService)
                                         && s.RxTierType.Equals(example.RxTierType))
                                         .FirstOrDefault();
                    if (Service != null)
                    {
                        example.Copay = SBCCostShareHelper.GetCopay(Service.Copay).Trim();
                        example.Coinsurance = SBCCostShareHelper.GetCoinsurance(Service.Coinsurance).Trim();
                        example.Deductible = SBCCostShareHelper.GetDeductible(Service.IndividualDeductible.ToString()).Trim();
                        example.Covered = SBCHelper.IsCovered(Service.Covered);
                    }
                }
            }
            example.CostShareApplies = example.Covered;
        }

        public void SetProcessingRule(CoverageExample example)
        {
            if (example.Covered == "Yes")
            {
                example = IsManualOverrideService(example);
                if (string.IsNullOrEmpty(example.ProcessingRule))
                {
                    example.ProcessingRule = ProcessingRuleManager.GetProcessRule(example);
                }
            }
            else
            {
                example.ProcessingRule = SBCConstant.NoRule;
            }
        }

        public void ProcessSBCCoverageService(CoverageExample example)
        {
            List<KeyValuePair<string, int>> ruleExecutionSeq = ProcessingRuleManager.GetRuleExecutionSequence(example.ProcessingRule);

            AddKey(example);
            foreach (KeyValuePair<string, int> sequence in ruleExecutionSeq)
            {
                try
                {
                    if (sequence.Value == 1)
                    {
                        example.CalculatedAllowedAmount = example.AllowedAmount;
                    }
                    switch (sequence.Key)
                    {
                        case SBCConstant.Copay:
                            ProcessCopay(example);
                            break;
                        case SBCConstant.Coinsurance:
                            ProcessCoinsurance(example);
                            break;
                        case SBCConstant.Deductible:
                            ProcessDeductible(example);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Exception custex = new Exception(ex.Message + "\n ProcessSBCCoverageService=" + JsonConvert.SerializeObject(example), ex);
                    bool reThrow = ExceptionPolicyWrapper.HandleException(custex, ExceptionPolicies.ExceptionShielding);
                }
            }
            example.RemainingDeductible = this._individualDeductible.ToString();
        }
        #endregion

        #region Member Copay, Coins,Deductible Processing

        private void ProcessCopay(CoverageExample example)
        {
            double checkFloat;
            string calCopay = String.Empty;
            string getAllowedAmt = String.Empty;
            string tempCopayAmt = String.Empty;
            string result;
            calCopay = example.Copay.Contains("$") ?
                        double.TryParse(example.Copay.Substring(1).ToString(), out checkFloat) ?
                            example.Copay.Substring(1).ToString() : SBCConstant.NotApplicable
                      : double.TryParse(example.Copay.ToString(), out checkFloat) ?
                            example.Copay.ToString() : SBCConstant.NotApplicable;

            getAllowedAmt = example.CalculatedAllowedAmount.Contains("$") ?
                        double.TryParse(example.CalculatedAllowedAmount.Substring(1).ToString(), out checkFloat) ?
                            example.CalculatedAllowedAmount.Substring(1).ToString() : SBCConstant.NotApplicable
                      : double.TryParse(example.CalculatedAllowedAmount.ToString(), out checkFloat) ?
                            example.CalculatedAllowedAmount.ToString() : SBCConstant.NotApplicable;
            calCopay = calCopay.Trim();
            getAllowedAmt = getAllowedAmt.Trim();
            if (DateCategorykeys.ContainsKey(_key))
            {
                if (DateCategorykeys[_key].ToString() == SBCConstant.NotApplicable)
                {
                    tempCopayAmt = SBCConstant.ZeroValue;
                }
                else
                {
                    tempCopayAmt = DateCategorykeys[_key].ToString();
                }
            }

            if (calCopay == SBCConstant.NotApplicable || getAllowedAmt == SBCConstant.NotApplicable || tempCopayAmt == SBCConstant.ZeroValue)
            {
                result = SBCConstant.ZeroValue;
            }
            else
            {
                if (DateCategorykeys.ContainsKey(_key))
                {
                    if (Convert.ToDouble(tempCopayAmt) < Convert.ToDouble(calCopay))
                    {
                        calCopay = tempCopayAmt.ToString();
                    }

                }
                if (Convert.ToDouble(calCopay) > Convert.ToDouble(getAllowedAmt))
                {
                    result = getAllowedAmt;
                }
                else
                {
                    result = calCopay;
                }
            }
            updateTempCopay(result);
            example.MemberCostCopay = result;
            example.CalculatedAllowedAmount = SBCHelper.GetNewAllowAmount(example.CalculatedAllowedAmount, example.MemberCostCopay);

        }

        private void ProcessCoinsurance(CoverageExample example)
        {
            string result = string.Empty;
            double checkFloat;
            string calCoins = String.Empty;
            string getAllowedAmt = String.Empty;
            calCoins = example.Coinsurance.Contains("%") ?
                        double.TryParse(example.Coinsurance.Substring(0, example.Coinsurance.Length - 1).ToString(), out checkFloat) ?
                            example.Coinsurance.Substring(0, example.Coinsurance.Length - 1).ToString() : SBCConstant.NotApplicable
                      : double.TryParse(example.Coinsurance.ToString(), out checkFloat) ?
                            example.Coinsurance.ToString() : SBCConstant.NotApplicable;

            getAllowedAmt = example.CalculatedAllowedAmount.Contains("$") ?
                        double.TryParse(example.CalculatedAllowedAmount.Substring(1).ToString(), out checkFloat) ?
                            example.CalculatedAllowedAmount.Substring(1).ToString() : SBCConstant.NotApplicable
                      : double.TryParse(example.CalculatedAllowedAmount.ToString(), out checkFloat) ?
                            example.CalculatedAllowedAmount.ToString() : SBCConstant.NotApplicable;

            if (calCoins == SBCConstant.NotApplicable || getAllowedAmt == SBCConstant.NotApplicable)
            {
                result = SBCConstant.ZeroValue;
            }
            else
            {
                result = Math.Round(((Convert.ToDouble(getAllowedAmt) * Convert.ToDouble(calCoins)) / 100), 2).ToString();
            }
            example.MemberCostCoinsurance = result;
            example.CalculatedAllowedAmount = SBCHelper.GetNewAllowAmount(example.CalculatedAllowedAmount, example.MemberCostCoinsurance);
        }

        private string ProcessDeductible(CoverageExample example)
        {
            double overAllDeductibleVal = this._individualDeductible;
            double checkFloat;
            string calDed = String.Empty;
            string getAllowedAmt = String.Empty;
            string result;

            calDed = example.Deductible.Contains("$") ?
                        double.TryParse(example.Deductible.Substring(1).ToString(), out checkFloat) ?
                            example.Deductible.Substring(1).ToString() : SBCConstant.NotApplicable
                      : double.TryParse(example.Deductible.ToString(), out checkFloat) ?
                            example.Deductible.ToString() : SBCConstant.NotApplicable;

            getAllowedAmt = example.CalculatedAllowedAmount.Contains("$") ?
                        double.TryParse(example.CalculatedAllowedAmount.Substring(1).ToString(), out checkFloat) ?
                            example.CalculatedAllowedAmount.Substring(1).ToString() : SBCConstant.NotApplicable
                      : double.TryParse(example.CalculatedAllowedAmount.ToString(), out checkFloat) ?
                            example.CalculatedAllowedAmount.ToString() : SBCConstant.NotApplicable;
            calDed = calDed.Trim();
            getAllowedAmt = getAllowedAmt.Trim();
            if (calDed == SBCConstant.NotApplicable || getAllowedAmt == SBCConstant.NotApplicable || overAllDeductibleVal == Convert.ToDouble(SBCConstant.ZeroValue))
            {
                result = SBCConstant.ZeroValue;
            }
            else
            {
                if (overAllDeductibleVal < Convert.ToDouble(calDed))
                {
                    calDed = overAllDeductibleVal.ToString();
                }
                if (Convert.ToDouble(calDed) > Convert.ToDouble(getAllowedAmt))
                {

                    result = getAllowedAmt;
                }
                else
                {
                    result = calDed;
                }
            }
            example.MemberCostDeductible = result;
            updateOverAllDeductible(result);

            example.CalculatedAllowedAmount = SBCHelper.GetNewAllowAmount(example.CalculatedAllowedAmount, example.MemberCostDeductible);

            return result;
        }

        #endregion Member Copay, Coins,Deductible Processing

        #region Private

        private void AddKey(CoverageExample example)
        {
            double allowAmount = 0;
            if (example.Copay != SBCConstant.NotApplicable && !string.IsNullOrEmpty(example.Copay))
            {
                string amount = example.Copay.Replace("$", "");
                allowAmount = double.Parse(amount);
            }
            this._key = SBCHelper.KeyGenrator(example);
            if (!DateCategorykeys.Keys.Contains(this._key))
            {
                DateCategorykeys.Add(this._key, allowAmount);
            }
        }

        private void updateOverAllDeductible(string dedVal)
        {
            double overAllDeductibleVal = this._individualDeductible;

            if (dedVal == SBCConstant.NotApplicable || dedVal == "Not Covered")
            {
                dedVal = SBCConstant.ZeroValue;
            }
            else
            {
                if (overAllDeductibleVal > Convert.ToDouble(dedVal))
                {
                    overAllDeductibleVal = overAllDeductibleVal - Convert.ToDouble(dedVal);
                    overAllDeductibleVal = Math.Round(overAllDeductibleVal, 2);
                }
                else
                {
                    overAllDeductibleVal = Convert.ToDouble(SBCConstant.ZeroValue);
                }
            }
            this._individualDeductible = overAllDeductibleVal;
        }

        private void updateTempCopay(string copayVal)
        {
            if (DateCategorykeys.ContainsKey(this._key))
            {
                double currentVal = Convert.ToDouble(DateCategorykeys[this._key].ToString().Replace("$", ""));
                currentVal = currentVal - Convert.ToDouble(copayVal);
                DateCategorykeys[this._key] = Math.Round(currentVal, 2);
            }
        }

        private string GetINNNetworkName()
        {
            string networkTierName = string.Empty;
            if (this._networkList.Where(s => s.NetworkTier.Equals("INN")).Count() > 0)
            {
                return networkTierName = this._networkList.Where(s => s.NetworkTier.Equals("INN"))
                                    .Select(s => s.NetworkTier).FirstOrDefault();
            }
            if (this._networkList.Where(s => s.NetworkTier.Equals("INN Tier 1")).Count() > 0)
            {
                return networkTierName = this._networkList.Where(s => s.NetworkTier.Equals("INN Tier 1"))
                                    .Select(s => s.NetworkTier).FirstOrDefault();
            }
            return networkTierName;
        }

        private CoverageExample IsManualOverrideService(CoverageExample example)
        {
            if (example.ManualOverride == null)
            {
                example.ManualOverride = "No";
            }
            CoverageExample existingExample = this._existingCoverageExampleList
                                        .Where(s => s.Sequence.Equals(example.Sequence)
                                        &&
                                        s.ManualOverride == "Yes")
                                        .FirstOrDefault();
            if (existingExample != null)
            {
                example.ProcessingRule = existingExample.ProcessingRule;
                example.ManualOverride = "Yes";
            }
            return example;
        }

        private List<BenefitReview> GetINTierBRGData(List<BenefitReview> _brgServices)
        {
            return _brgServices.Where(s => s.NetworkTier.Equals(this._tierName)).ToList();
        }

        #endregion
    }
}
