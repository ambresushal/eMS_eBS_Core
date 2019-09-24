using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.document.rulebuilder.ComplexRuleInterpreter.Extension
{
    public class TokenConstant
    {
        public const string COB_COPAY = "Copay";
        public const string COB_COINS = "Coinsurance";
        public const string COB_IND_DED = "IndDeductible";
        public const string COB_FAM_DED = "FamDeductible";
        public const string COB_ROB = "ReductionofBenefits";
        public const string COC_COPAY = "MinimumCopay";
        public const string COC_COINS = "MinimumCoinsurance";
        public const string ANOC_DED = "Deductible";
        public const string ANOC_OOPM = "OOPM";
        public const string ANOC_MaxPlanBenAmount = "Max Plan Benefit Amount";
        public const string ANOC_MinCopay = "Min Copay";
        public const string ANOC_MaxCopay = "Max Copay";
        public const string ANOC_MinCoins = "Min Coinsurance";
        public const string ANOC_MaxCoins = "Max Coinsurance";
        public const string COC_COPAY_MAX = "MaximumCopay";
        public const string COC_COINS_MAX = "MaximumCoinsuarnce";
        public const string COC_MAX = "Max";
        public const string COC_MINMAX = "MinMax";
        public const string COC_MPBCA = "MaximumPlanBenefitCoverageAmount";
        public const string COC_MPBCP = "MaximumPlanBenefitCoveragePeriodicity";
        public const string COC_MPBCA_VAR = "MPBCA";
        public const string COC_MPBCP_VAR = "MPBCP";
    }
    public class FormatHelper
    {
        static List<string> _formats = new List<string>() { "$#", "#%", "$|%", "COB", "COC", "ANOCCHART" };

        public static string EvaluateFormat(string format, Core.Parser.Result value, string param)
        {
            string result = null;

            if (HasFormat(format))
            {
                switch (format)
                {
                    case "$#":
                        result = DollarFormat(FunctionHelper.GetArgument(value), param);
                        break;
                    case "#%":
                        result = PercentFormat(FunctionHelper.GetArgument(value), param);
                        break;
                    case "$|%":
                        result = DollarOrPercent(FunctionHelper.GetArgument(value), param);
                        break;
                    case "COB":
                        result = COBFormat(value.Token, param);
                        break;
                    case "COC":
                        result = COCFormat(value.Token, param);
                        break;
                    case "ANOCCHART":
                        result = ANOCChartFormat(FunctionHelper.GetArgument(value));
                        break;
                };
            }

            return result;
        }

        private static string DollarFormat(string value, string param)
        {
            string currency = string.Format("{0:C}", value);
            if (!string.IsNullOrEmpty(param))
            {
                currency = ProcessParam(currency, param);
            }
            return currency;
        }

        private static string PercentFormat(string value, string param)
        {
            string percent = string.Format("{0:P}", value);
            if (!string.IsNullOrEmpty(param))
            {
                percent = ProcessParam(percent, param);
            }
            return percent;
        }

        private static string DollarOrPercent(string value, string param)
        {
            string result = string.Empty;
            if (value.Contains("$"))
            {
                result = value + " " + "Copay";
            }
            else if (value.Contains("%"))
            {
                result = value + " " + "Coinsurance";
            }

            if (!string.IsNullOrEmpty(param))
            {
                result = ProcessParam(result, param);
            }

            return result;
        }

        private static string COBFormat(JToken value, string param)
        {
            string result = null;
            StringBuilder sb = new StringBuilder();
            if (value != null)
            {
                sb.Append(IsEmpty(value[TokenConstant.COB_COPAY]) ? "" : "$" + value[TokenConstant.COB_COPAY].ToString() + " Copay " + param + System.Environment.NewLine);
                sb.Append(IsEmpty(value[TokenConstant.COB_COINS]) ? "" : value[TokenConstant.COB_COINS].ToString() + "% Coinsurance ");
                sb.Append(!IsEmpty(value[TokenConstant.COB_IND_DED]) || !IsEmpty(value[TokenConstant.COB_FAM_DED]) ? "after Deductible" : "");
                sb.Append(IsEmpty(value[TokenConstant.COB_ROB]) ? "" : "up to $" + value[TokenConstant.COB_ROB].ToString() + " Reduction of Benefits ");
            }
            if (string.IsNullOrWhiteSpace(sb.ToString()))
            {
                result = "Not Covered";
            }
            result = sb.ToString();
            return result;
        }

        private static string COCFormat(JToken value, string param)
        {
            string result = null;
            StringBuilder sb = new StringBuilder();
            if (value == null)
            {
                sb.Append("Not Covered");
            }
            else
            {
                if (param == TokenConstant.COC_MINMAX)
                {
                    if (!IsEmpty(value[TokenConstant.COC_COPAY_MAX]))
                    {
                        sb.Append(IsEmpty(value[TokenConstant.COC_COPAY]) ? "$0-" : "$" + value[TokenConstant.COC_COPAY].ToString() + "-");
                        sb.Append(value[TokenConstant.COC_COPAY_MAX]);
                        sb.Append(" copay");
                    }
                    else if ((!IsEmpty(value[TokenConstant.COC_COINS_MAX])))
                    {
                        sb.Append(IsEmpty(value[TokenConstant.COC_COINS]) ? "0%-" : value[TokenConstant.COC_COINS].ToString() + "%-");
                        sb.Append(value[TokenConstant.COC_COINS_MAX]);
                        sb.Append("% coinsurance");
                    }
                    else
                    {
                        sb.Append("You pay nothing");
                    }

                }
                else if (param == TokenConstant.COC_MAX)
                {
                    sb.Append(IsEmpty(value[TokenConstant.COC_COPAY_MAX]) ? "" : "$" + value[TokenConstant.COC_COPAY_MAX].ToString() + " copay ");
                    if (!IsEmpty(value[TokenConstant.COC_COPAY_MAX]))
                    {
                        if (!IsEmpty(value[TokenConstant.COC_COINS_MAX]))
                        {
                            sb.Append("<br/>");
                        }
                    }
                    sb.Append(IsEmpty(value[TokenConstant.COC_COINS_MAX]) ? "" : value[TokenConstant.COC_COINS_MAX].ToString() + "% coinsurance ");
                    if (string.IsNullOrWhiteSpace(sb.ToString()))
                    {
                        sb.Append("You pay nothing");

                    }
                }
                else if (param == TokenConstant.COC_MPBCA_VAR)
                {
                    sb.Append(IsEmpty(value[TokenConstant.COC_MPBCA]) ? "nothing" : "$" + value[TokenConstant.COC_MPBCA].ToString());
                    
                }
                else if (param == TokenConstant.COC_MPBCP_VAR)
                {
                    sb.Append(IsEmpty(value[TokenConstant.COC_MPBCP]) ? "" : value[TokenConstant.COC_MPBCP].ToString() );

                }
                else
                {
                    sb.Append(IsEmpty(value[TokenConstant.COC_COPAY]) ? "" : "$" + value[TokenConstant.COC_COPAY].ToString() + " copay ");
                    if (!IsEmpty(value[TokenConstant.COC_COPAY]))
                    {
                        if (!IsEmpty(value[TokenConstant.COC_COINS]))
                        {
                            sb.Append("<br/>");
                        }
                    }
                    sb.Append(IsEmpty(value[TokenConstant.COC_COINS]) ? "" : value[TokenConstant.COC_COINS].ToString() + "% coinsurance ");
                    if (string.IsNullOrWhiteSpace(sb.ToString()))
                    {
                        sb.Append("You pay nothing");

                    }
                }
                
            }
            result = sb.ToString();
            return result;
        }

        private static bool IsEmpty(JToken value)
        {
            string[] exclusion = new string[] { "[]", "{}", "Not Applicable", "Not Covered", "NA", "Not Selected", "", "0", "0.0", "0.00" };
            return exclusion.Contains(value.ToString().Trim());
        }

        private static bool HasFormat(string formatOption)
        {
            return _formats.Contains(formatOption);
        }

        private static string ProcessParam(string value, string param)
        {
            if (param.IndexOf("{0}") >= 0)
            {
                value = string.Format(param, value);
            }
            else
            {
                value = value + " " + param;
            }
            return value;
        }

        private static string ANOCChartFormat(string anocCompareString)
        {
            string result = string.Empty;         
            string[] exclusion = new string[] { "[]", "{}", "Not Applicable", "Not Covered", "NA", "Not Selected", "" };
           
            if (!string.IsNullOrEmpty(anocCompareString))
            {
                string generalServiceCostShareValue = anocCompareString;
                string slidingCostShareValue = string.Empty;
                StringBuilder sb = new StringBuilder();

                if (anocCompareString.Contains("<br/>"))
                {
                    string[] costShareCategory = anocCompareString.Split(new string[] { "<br/>" }, StringSplitOptions.None);
                    slidingCostShareValue = costShareCategory[0];
                    generalServiceCostShareValue = costShareCategory[1];
                }

                sb.Append(!string.IsNullOrEmpty(slidingCostShareValue) && !(exclusion.Contains(slidingCostShareValue)) ? GetANOCChartSlidingServiceString(slidingCostShareValue) : "");
                if (!string.IsNullOrEmpty(sb.ToString()))
                {
                    sb.Append("</br>");
                }
                sb.Append(!string.IsNullOrEmpty(generalServiceCostShareValue) && !(exclusion.Contains(generalServiceCostShareValue)) ? GetANONCChartGeneralServiceString(generalServiceCostShareValue) : "");


                result = string.IsNullOrEmpty(sb.ToString())? "Not Covered" :sb.ToString();
            }
            return result;
        }

        private static Dictionary<string, string> GetANOCCostShareDictionary(string costShareString)
        {
            Dictionary<string, string> costShareDic = new Dictionary<string, string>();

            string[] costShareDetails = costShareString.Split(',');

            foreach (string costShare in costShareDetails)
            {
                string[] costShareKeyValue = costShare.Split('-');
                costShareDic.Add(costShareKeyValue[1], costShareKeyValue[0]);
            }
            return costShareDic;
        }


        private static string GetANONCChartGeneralServiceString(string generalServiceCostShareValue)
        {
            Dictionary<string, string> costShares = GetANOCCostShareDictionary(generalServiceCostShareValue);
            StringBuilder sb = new StringBuilder();

            sb.Append(costShares.ContainsKey(TokenConstant.ANOC_DED) && (!IsEmpty(costShares[TokenConstant.ANOC_DED])) ? "There is a " + costShares[TokenConstant.ANOC_DED] + " Deductible.</br>" : " Deductible is Not Applicable.</br>");
            sb.Append(costShares.ContainsKey(TokenConstant.COB_COPAY) && (!IsEmpty(costShares[TokenConstant.COB_COPAY])) ? "You pay " + costShares[TokenConstant.COB_COPAY] + " for this Benefit.</br>" : "");
            sb.Append(costShares.ContainsKey(TokenConstant.ANOC_MinCopay) && (!IsEmpty(costShares[TokenConstant.ANOC_MinCopay])) ? "You pay " + costShares[TokenConstant.ANOC_MinCopay] + " Minimum Copay for this Benefit.</br>" : "");
            sb.Append(costShares.ContainsKey(TokenConstant.ANOC_MaxCopay) && (!IsEmpty(costShares[TokenConstant.ANOC_MaxCopay])) ? "You pay " + costShares[TokenConstant.ANOC_MaxCopay] + " Maximum Copay for this Benefit.</br>" : "");
            sb.Append(costShares.ContainsKey(TokenConstant.ANOC_MinCoins) && (!IsEmpty(costShares[TokenConstant.ANOC_MinCoins])) ? "You pay " + costShares[TokenConstant.ANOC_MinCoins] + " Minimum Coinsurance for this Benefit.</br>" : "");
            sb.Append(costShares.ContainsKey(TokenConstant.ANOC_MaxCoins) && (!IsEmpty(costShares[TokenConstant.ANOC_MaxCoins])) ? "You pay " + costShares[TokenConstant.ANOC_MaxCoins] + " Maximum Coinsurance for this Benefit.</br>" : "");
            sb.Append(costShares.ContainsKey(TokenConstant.COB_COINS) && (!IsEmpty(costShares[TokenConstant.COB_COINS])) ? "You pay " + costShares[TokenConstant.COB_COINS] + " of the total cost for this benefit.</br>" : "You pay nothing for this Benefit.</br>");
            sb.Append(IsCostShareEmpty(costShares) ? "You pay nothing for this Benefit.</br>" : "");

            //Assumption : OOP Amount and Periodicity will be seperated by _ symbol
            sb.Append(costShares.ContainsKey(TokenConstant.ANOC_OOPM)
                       && !(IsEmpty(costShares[TokenConstant.ANOC_OOPM]))
                      ? "There is a " + costShares[TokenConstant.ANOC_OOPM].Split('_')[0] + " out-of-pocket limit " + costShares[TokenConstant.ANOC_OOPM].Split('_')[1] + ".</br>"
                    : "");

            //Assumption : Maximum Plan Amount and Periodicity will be seperated by _ symbol
            sb.Append(costShares.ContainsKey(TokenConstant.ANOC_MaxPlanBenAmount) && !(IsEmpty(costShares[TokenConstant.ANOC_MaxPlanBenAmount]))
                    ? "There is a "
                    + costShares[TokenConstant.ANOC_MaxPlanBenAmount].Split('_')[0] +
                    " allowance "
                    + costShares[TokenConstant.ANOC_MaxPlanBenAmount].Split('_')[1] + ".</br>"
                    : "");

            sb.Append(costShares.ContainsKey(TokenConstant.ANOC_OOPM) && (IsEmpty(costShares[TokenConstant.ANOC_OOPM])) ? "There is no out-of-pocket limit.</br>" : "");
            sb.Append(costShares.ContainsKey(TokenConstant.ANOC_MaxPlanBenAmount) && (IsEmpty(costShares[TokenConstant.ANOC_MaxPlanBenAmount])) ? " There is no allowance.</br>" : "");

            return sb.ToString();

        }

        private static string GetANOCChartSlidingServiceString(string slidingCostShare)
        {
            StringBuilder sb = new StringBuilder();
            string[] intervals = slidingCostShare.Split(';');

            foreach (string intervalData in intervals)
            {
                string[] intervaleFormats = intervalData.Split(',');
                string intervalLimit = intervaleFormats[0].Split(':')[1].Trim().ToString();
                string benefitPeriod = !IsEmpty(JToken.FromObject(intervaleFormats[2].Trim())) ? intervaleFormats[2] : "";
                string interReportFormat = "You pay a " + intervaleFormats[1] + " copay" + benefitPeriod + " for Days " + intervalLimit + "</br>";
                sb.Append(interReportFormat);
            }

            return sb.ToString();
        }

        private static bool IsCostShareEmpty(Dictionary<string, string> costShareDictionary)
        {

            bool coshShareCheck = ((costShareDictionary.ContainsKey(TokenConstant.COB_COPAY) && (IsEmpty(costShareDictionary[TokenConstant.COB_COPAY]))) || (!costShareDictionary.ContainsKey(TokenConstant.COB_COPAY)))
             && ((costShareDictionary.ContainsKey(TokenConstant.COB_COINS) && (IsEmpty(costShareDictionary[TokenConstant.COB_COINS]))) || (!costShareDictionary.ContainsKey(TokenConstant.COB_COINS)))
             && ((costShareDictionary.ContainsKey(TokenConstant.ANOC_MinCoins) && (IsEmpty(costShareDictionary[TokenConstant.ANOC_MinCoins]))) || (!costShareDictionary.ContainsKey(TokenConstant.ANOC_MinCoins)))
             && ((costShareDictionary.ContainsKey(TokenConstant.ANOC_MaxCoins) && (IsEmpty(costShareDictionary[TokenConstant.ANOC_MaxCoins]))) || (!costShareDictionary.ContainsKey(TokenConstant.ANOC_MaxCoins)))
             && ((costShareDictionary.ContainsKey(TokenConstant.ANOC_MinCopay) && (IsEmpty(costShareDictionary[TokenConstant.ANOC_MinCopay]))) || (!costShareDictionary.ContainsKey(TokenConstant.ANOC_MinCopay)))
             && ((costShareDictionary.ContainsKey(TokenConstant.ANOC_MaxCopay) && (IsEmpty(costShareDictionary[TokenConstant.ANOC_MaxCopay]))) || (!costShareDictionary.ContainsKey(TokenConstant.ANOC_MaxCopay)));

            return coshShareCheck;
        }

    }
}
