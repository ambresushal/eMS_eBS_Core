using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.sbccalculator.Model;

namespace tmg.equinox.sbccalculator.Helper
{
    public static class SBCCostShareHelper
    {
        public static string GetCopay(string amount)
        {
            string result = SBCConstant.NotApplicable;
            if (!string.IsNullOrEmpty(amount) && !string.Equals(amount, SBCConstant.NotApplicable, StringComparison.OrdinalIgnoreCase))
            {
                decimal copayAmount;
                if (decimal.TryParse(amount, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, CultureInfo.GetCultureInfo("en-US"), out copayAmount))
                {
                    result = "$" + copayAmount;
                }
            }
            return result;
        }

        public static string GetCoinsurance(string amount)
        {
            string result = SBCConstant.NotApplicable;
            string coins = amount.Replace("%", "");
            if (!string.IsNullOrEmpty(coins) && !string.Equals(coins, SBCConstant.NotApplicable, StringComparison.OrdinalIgnoreCase))
            {
                decimal coinsAmount;
                if (decimal.TryParse(coins, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, CultureInfo.CurrentCulture, out coinsAmount))
                {

                    result = coinsAmount.ToString("n2") + "%";
                }
            }
            return result;
        }

        public static string GetDeductible(string amount)
        {
            string result = SBCConstant.NotApplicable;
            if (!string.IsNullOrEmpty(amount) && !string.Equals(amount, SBCConstant.NotApplicable, StringComparison.OrdinalIgnoreCase))
            {
                decimal deductibleAmount;
                if (decimal.TryParse(amount, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, CultureInfo.GetCultureInfo("en-US"), out deductibleAmount))
                {
                    result = "$" + deductibleAmount;
                }
            }
            return result;
        }

        public static double GetIndividualDeductible(List<DeductibleList> dedList, string tierName, bool isRx, List<RxCostShare> rxCostShareList)
        {
            double deductibleAmount = 0, indDeductibleAmount = 0;
            string value = SBCConstant.NotApplicable;
            if (isRx)
            {
                if (rxCostShareList.Count() > 0)
                {
                    value = rxCostShareList.Where(s => s.Network.Equals(tierName))
                            .Select(s => s.RxDeductibleIndividual)
                            .FirstOrDefault();
                }
            }
            else
            {
                if (dedList.Count > 0)
                {
                    value = dedList.Where(s => s.NetworkTier.Equals(tierName) && s.CoverageName.Equals(SBCConstant.Individual))
                            .Select(s => s.DeductibleAmount)
                            .FirstOrDefault();
                }
            }
            if (double.TryParse(value, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, CultureInfo.GetCultureInfo("en-US"), out deductibleAmount))
            {
                indDeductibleAmount = deductibleAmount;
            }
            return indDeductibleAmount;
        }

        public static string GetCostShareValue(string copayAmount, string CoinsAmount)
        {
            string value = SBCConstant.NotApplicable;
            if (copayAmount != SBCConstant.NotApplicable && copayAmount != SBCConstant.NotCovered && !string.IsNullOrEmpty(copayAmount) && copayAmount.Contains("$"))
            {
                value = copayAmount;
            }
            else
            {
                if (CoinsAmount != SBCConstant.NotApplicable && CoinsAmount != SBCConstant.NotCovered && !string.IsNullOrEmpty(CoinsAmount) && CoinsAmount.Contains("%"))
                {
                    value = CoinsAmount;
                }
            }
            return value;
        }
    }

    public static class SBCRoundOff
    {
        public static string GetSBCRoundOff(string val)
        {
            double retVal = 0;
            if (!string.IsNullOrEmpty(val))
            {
                double sourceVal = double.Parse(val);
                sourceVal = Math.Round(sourceVal, 0);
                int length = sourceVal.ToString().Length;
                if (sourceVal > 0)
                    retVal = length == 1 ? sourceVal : length == 2 ? Round(sourceVal, 10) : Round(sourceVal, 100);
            }
            return retVal.ToString();
        }

        private static double Round(double sourceVal, int nearest)
        {
            if (sourceVal % 50 != 0 && sourceVal % 10 == 0)
            {
                sourceVal = sourceVal + 1;
            }
            string strVal = (sourceVal / nearest).ToString();
            int index = strVal.IndexOf(".");
            double result = index == -1 ? sourceVal :
                (double.Parse(strVal.Remove(0, index + 1)) > (nearest / 2) ?
                    Math.Ceiling(sourceVal / nearest) * nearest :
                    Math.Ceiling(sourceVal / nearest) * nearest - nearest);
            return result;
        }

        public static string GetMathRoundOff(string total)
        {
            string result = "0";
            try
            {
                if (total != SBCConstant.NotApplicable && !string.IsNullOrEmpty(total))
                {
                    double Val1, Val3 = 0;
                    Val1 = double.Parse(total);
                    Val3 = Math.Round(Val1 * 100f) / 100f;
                    result = Val3.ToString();
                }
            }
            catch (Exception ex)
            {
                Exception custex = new Exception(ex.Message + "GetMathRoundOff=" + total, ex);
                bool reThrow = ExceptionPolicyWrapper.HandleException(custex, ExceptionPolicies.ExceptionShielding);
            }

            return result;

        }
    }

    public static class SBCHelper
    {
        public static string GetNewAllowAmount(string serviceAllowAmount, string calAmount)
        {
            string result = SBCConstant.NotApplicable;
            double Value = 0;
            try
            {
                if (!string.IsNullOrEmpty(serviceAllowAmount) && !string.Equals(serviceAllowAmount, SBCConstant.NotApplicable, StringComparison.OrdinalIgnoreCase))
                {
                    double allowAmount = 0;
                    if (double.TryParse(serviceAllowAmount, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, CultureInfo.GetCultureInfo("en-US"), out allowAmount))
                    {
                        Value = allowAmount - Convert.ToDouble(calAmount);
                    }
                }
            }
            catch (Exception ex)
            {
                Exception custex = new Exception(ex.Message + "GetNewAllowAmount=" + Value, ex);
                bool reThrow = ExceptionPolicyWrapper.HandleException(custex, ExceptionPolicies.ExceptionShielding);
                Value = 0;
            }
            result = "$" + Value;
            return result;
        }

        public static List<CalculatedDataModel> GetDefaultRows()
        {
            List<string> CoverageTypeList = new List<string>
            { SBCConstant.Diabetes, SBCConstant.Fracture, SBCConstant.Maternity };
            List<CalculatedDataModel> repeaterList = new List<CalculatedDataModel>();
            foreach (var cvg in CoverageTypeList)
            {
                repeaterList.Add(new CalculatedDataModel() { TreatmentType = cvg });
            }
            return repeaterList;
        }

        public static bool IsRxService(CoverageExample example)
        {
            bool isRxServiceResult = false;

            if (!string.IsNullOrEmpty(example.RxService) && !string.IsNullOrEmpty(example.RxTierType))
            {
                isRxServiceResult = true;
            }
            return isRxServiceResult;
        }

        public static string KeyGenrator(CoverageExample example)
        {
            string key = string.Empty;
            if (!string.IsNullOrEmpty(example.BenefitServiceCode))
            {
                key = example.BenefitServiceCode + "=>" + example.DateofService;
            }
            else
            {
                key = example.RxService + "=>" + example.RxTierType + "=>" + example.DateofService;
            }
            return key.Trim();
        }

        public static void FormatRow(CoverageExample example)
        {
            example.MemberCostCoinsurance = ApplyCulture(example.MemberCostCoinsurance);
            example.MemberCostCopay = ApplyCulture(example.MemberCostCopay);
            example.MemberCostDeductible = ApplyCulture(example.MemberCostDeductible);
            example.RemainingDeductible = ApplyCulture(example.RemainingDeductible);
        }

        public static void FormatRow(CalculatedDataModel row)
        {
            row.TotalDeductibleMemberCost = SBCHelper.ApplyCulture(row.TotalDeductibleMemberCost);
            row.TotalCoinsuranceMemberCost = SBCHelper.ApplyCulture(row.TotalCoinsuranceMemberCost);
            row.TotalCopayMemberCost = SBCHelper.ApplyCulture(row.TotalCopayMemberCost);
            row.RoundOffDeductible = SBCHelper.ApplyCulture(row.RoundOffDeductible);
            row.RoundOffCoinsurance = SBCHelper.ApplyCulture(row.RoundOffCoinsurance);
            row.RoundOffCopay = SBCHelper.ApplyCulture(row.RoundOffCopay);
            row.RoundOffLimits = SBCHelper.ApplyCulture(row.RoundOffLimits);
            row.TotalMemberCost = SBCHelper.ApplyCulture(row.TotalMemberCost);
            row.FinalMemberCost = SBCHelper.ApplyCulture(row.FinalMemberCost);
            row.OverallDeductible = SBCHelper.ApplyCulture(row.OverallDeductible);
        }

        public static string ApplyCulture(string value)
        {
            decimal amount;
            string result = string.Empty;
            if (value != SBCConstant.NotApplicable && decimal.TryParse(value, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, CultureInfo.GetCultureInfo("en-US"), out amount))
            {
                result = amount.ToString("C2", CultureInfo.GetCultureInfo("en-US"));
            }
            else
            {
                result = value;
            }
            return result;
        }

        public static double ExtractValue(string value)
        {
            double amount = 0;
            double.TryParse(value, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, CultureInfo.GetCultureInfo("en-US"), out amount);
            return amount;
        }

        public static void SetDefaultCostShareValues(CoverageExample example)
        {
            if (string.IsNullOrEmpty(example.Copay))
            {
                example.Copay = SBCConstant.NotApplicable;
            }
            if (string.IsNullOrEmpty(example.Coinsurance))
            {
                example.Coinsurance = SBCConstant.NotApplicable;
            }
            if (string.IsNullOrEmpty(example.Deductible))
            {
                example.Deductible = SBCConstant.NotApplicable;
            }
            if (string.IsNullOrEmpty(example.AllowedAmount))
            {
                example.AllowedAmount = "$0";
            }
            example.AllowedAmount = example.AllowedAmount.Trim();
            example.CostShareApplies = "No";
            example.Covered = "No";
            if (example.AllowedAmount == "$0" || example.AllowedAmount == "$0.00")
            {
                if (example.Covered == "Yes")
                {
                    example.MemberCostCopay = SBCConstant.ZeroValue;
                    example.MemberCostCoinsurance = SBCConstant.ZeroValue;
                    example.MemberCostDeductible = SBCConstant.ZeroValue;
                }
                else
                {
                    example.MemberCostCopay = SBCConstant.NotApplicable;
                    example.MemberCostCoinsurance = SBCConstant.NotApplicable;
                    example.MemberCostDeductible = SBCConstant.NotApplicable;
                }
            }
            else
            {
                example.MemberCostCopay = SBCConstant.NotApplicable;
                example.MemberCostCoinsurance = SBCConstant.NotApplicable;
                example.MemberCostDeductible = SBCConstant.NotApplicable;
            }
            example.RemainingDeductible = SBCConstant.NotApplicable;
        }

        public static string IsCovered(string covered)
        {
            if (covered.Equals("Yes") || covered.Equals("true") || covered.Equals("True"))
            {
                return "Yes";
            }
            else
            {
               return "No";
            }
        }
    }
}
