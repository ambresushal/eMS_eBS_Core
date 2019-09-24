using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpreter.ComplexRuleInterpreter.Helper
{
    public class CostShareHelper
    {
        public static string Evaluate(string MinOrMax, string CopayOrCoins, string InTierMin, string InTierMax, string InTier1Min, string InTier1Max, string InTier2Min, string InTier2Max, string InTier3Min, string InTier3Max)
        {
            string result = string.Empty;
            List<string> parameters = new List<string>() { InTierMin, InTier1Min, InTier2Min, InTier3Min};
            List<string> maxparametrs = new List<string>() {  InTierMax, InTier1Max, InTier2Max, InTier3Max };
            List<decimal> Copayments = new List<decimal>();
            List<int> Coinsurances = new List<int>();
            List<decimal> MaxCopayments = new List<decimal>();
            List<int> MaxCoinsurances = new List<int>();

            bool hasCopay = false; bool hasCoins = false;

            foreach (var cs in parameters)
            {
                if (!string.IsNullOrEmpty(cs))
                {
                    if (cs.IndexOf("$") >= 0)
                    {
                        decimal number;
                        bool isNumber = Decimal.TryParse(cs.TrimStart(new char[] { '$' }), out number);
                        if (isNumber)
                            Copayments.Add(Convert.ToDecimal(String.Format("{0:0.00}", number)));
                    }
                    else if (cs.IndexOf("%") >= 0)
                    {
                        int number;
                        bool isNumber = Int32.TryParse(cs.TrimEnd(new char[] { '%' }), out number);
                        if (isNumber)
                            Coinsurances.Add(number);
                    }
                }
            }
            foreach (var cs in maxparametrs)
            {
                if (!string.IsNullOrEmpty(cs))
                {
                    if (cs.IndexOf("$") >= 0)
                    {
                        decimal number;
                        bool isNumber = Decimal.TryParse(cs.TrimStart(new char[] { '$' }), out number);
                        if (isNumber)
                            MaxCopayments.Add(Convert.ToDecimal(String.Format("{0:0.00}", number)));
                    }
                    else if (cs.IndexOf("%") >= 0)
                    {
                        int number;
                        bool isNumber = Int32.TryParse(cs.TrimEnd(new char[] { '%' }), out number);
                        if (isNumber)
                            MaxCoinsurances.Add(number);
                    }
                }
            }

            hasCopay = Copayments.Count > 0 || MaxCopayments.Count > 0;
            hasCoins = Coinsurances.Count > 0 || MaxCoinsurances.Count > 0;

            
            string outputcopay = string.Empty;
            string outputcoins = string.Empty;

            if (hasCopay && !hasCoins)
                outputcopay = string.Equals(MinOrMax, "Min", StringComparison.OrdinalIgnoreCase) ? Convert.ToString(Copayments.Min()) : Convert.ToString(MaxCopayments.Max());
            else if (!hasCopay && hasCoins)
                outputcoins = string.Equals(MinOrMax, "Min", StringComparison.OrdinalIgnoreCase) ? Convert.ToString(Coinsurances.Min()) : Convert.ToString(MaxCoinsurances.Max());
            else if (hasCopay && hasCoins)
            {
                if (string.Equals(MinOrMax, "Min", StringComparison.OrdinalIgnoreCase))
                {
                    outputcopay = Copayments.Count > 0 ? Convert.ToString(Copayments.Min()) : Convert.ToString(MaxCopayments.Min());
                    outputcoins = Coinsurances.Count > 0 ? Convert.ToString(Coinsurances.Min()) : Convert.ToString(MaxCoinsurances.Min());
                }
                else
                {
                    outputcopay = MaxCopayments.Count > 0 ? Convert.ToString(MaxCopayments.Max()) : Convert.ToString(Copayments.Max());
                    outputcoins = MaxCoinsurances.Count > 0 ? Convert.ToString(MaxCoinsurances.Max()) : Convert.ToString(Coinsurances.Max());
                }

            }

            if (string.Equals(CopayOrCoins, "Copay", StringComparison.OrdinalIgnoreCase) && hasCopay)
            {
                result = outputcopay;
            }

            if (string.Equals(CopayOrCoins, "Coins", StringComparison.OrdinalIgnoreCase) && hasCoins)
            {
                result = outputcoins;
            }

            return result;
        }
    }
}
