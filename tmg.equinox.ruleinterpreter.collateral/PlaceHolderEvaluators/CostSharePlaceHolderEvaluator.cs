using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace tmg.equinox.ruleinterpretercollateral
{
    public class CostSharePlaceHolderEvaluator : IPlaceHolderEvaluator
    {
        string _placeHolder;
        JToken _dataSource;
        Regex _numericRegex;
        Regex _extractDigitRegex;

        public CostSharePlaceHolderEvaluator(string placeHolder, JToken dataSource)
        {
            _placeHolder = placeHolder;
            _dataSource = dataSource;
            _numericRegex = new Regex(@"^[\$]{0,1}\d*[,]{0,1}\d*[.]{0,1}\d*[%]{0,1}$");
            _extractDigitRegex = new Regex(@"[^\d.]");
        }

        public string Evaluate()
        {
            string result = "";
            string[] parts = _placeHolder.Split(':');
            string functionName = parts[0];
            if(parts.Length > 1)
            {
                string columnNames = parts[1].Trim(new[] { '[', ']' });
                string[] columns = columnNames.Split(',');
                List<CostShareValue> values = new List<CostShareValue>();
                foreach(var col in columns)
                {
                    CostShareValue val = GetCostShareValue(col, _dataSource);
                    if(val.IsValid == true)
                    {
                        values.Add(val);
                    }
                }
                //get Copays
                var copays = values.Where(a => a.CostShareType == "Copay" && a.Value != null).OrderByDescending(b =>b.Value);
                double? copay = null;
                if(copays.Count() > 0)
                {
                    if (functionName == "CS" || functionName == "CSMIN")
                    {
                        copay = copays.Min(a => a.Value);
                    }
                    else
                    {
                        copay = copays.Max(a => a.Value);
                    }
                }
                //get Coinsurances
                var coinsurances = values.Where(a => a.CostShareType == "Coinsurance" && a.Value != null).OrderByDescending(b => b.Value);
                double? coInsurance = null;
                if (coinsurances.Count() > 0)
                {
                    if(functionName == "CS" || functionName == "CSMIN")
                    {
                        coInsurance = coinsurances.Min(a => a.Value);
                    }
                    else
                    {
                        coInsurance = coinsurances.Max(a => a.Value);
                    }
                }
                if(copay != null){
                    result = result + copay.Value.ToString("C0",CultureInfo.GetCultureInfo("en-US").NumberFormat) + " Copay ";
                }
                result = result.Trim();
                if (coInsurance != null)
                {
                    if(!String.IsNullOrEmpty(result))
                    {
                        result = result + ", " + (coInsurance.Value/100).ToString("P0", CultureInfo.GetCultureInfo("en-US").NumberFormat).Replace(" " ,"") + " Coinsurance ";
                    }
                    else
                    {
                        result = result + " " + (coInsurance.Value/100).ToString("P0", CultureInfo.GetCultureInfo("en-US").NumberFormat).Replace(" " ,"") + " Coinsurance ";
                    }
                }
                if(String.IsNullOrEmpty(result))
                {
                    result = "You pay nothing ";
                }
            }
            return result;
        }

        private CostShareValue GetCostShareValue(string property, JToken row)
        {
            CostShareValue value = new CostShareValue();
            value.HasValue = false;
            value.IsValid = false;
            if (property.EndsWith("Copay"))
            {
                value.CostShareType = "Copay";
            }
            else if (property.EndsWith("Coinsurance") || property.EndsWith("Coins"))
            {
                value.CostShareType = "Coinsurance";
            }
            else if(row["CostShareType"] != null)
            {
                string cst = row["CostShareType"].ToString();
                if (cst.Trim().Equals("COPAY", StringComparison.OrdinalIgnoreCase))
                {
                    value.CostShareType = "Copay";
                }
                else if (cst.Trim().Equals("COINSURANCE", StringComparison.OrdinalIgnoreCase))
                {
                    value.CostShareType = "Coinsurance";
                }
                else
                {
                    value.CostShareType = "";
                }
            }
            else
            {
                value.CostShareType = "";
            }
            var tok = row.SelectToken(property);
            if(tok != null)
            {
                string propertyValue = tok.ToString();
                if (!String.IsNullOrEmpty(propertyValue))
                {
                    if(propertyValue.Trim().StartsWith("$"))
                    {
                        value.CostShareType = "Copay";
                        propertyValue = propertyValue.TrimStart('$');
                    }
                    if (propertyValue.Trim().EndsWith("%"))
                    {
                        value.CostShareType = "Coinsurance";
                        propertyValue = propertyValue.TrimEnd('%');
                    }
                    int? intVal = null;
                    double? doubleVal = null;
                    if(TryGetNumeric(propertyValue, ref intVal, ref doubleVal) == true)
                    {
                        if(intVal != null || doubleVal != null)
                        {
                            value.Value = doubleVal != null ? doubleVal : intVal;
                            value.HasValue = true;
                            value.IsValid = true;
                        }
                    }
                }
            }
            return value;
        }

        private bool TryGetNumeric(string input, ref int? intVal, ref double? doubleVal)
        {
            bool isNumeric = false;
            if (!String.IsNullOrEmpty(input))
            {
                Match match = _numericRegex.Match(input);
                if (match.Success == true)
                {
                    double tDoubleVal;
                    int tIntVal;
                    input = _extractDigitRegex.Replace(input, "");
                    if (input.IndexOf('.') > 0)
                    {
                        isNumeric = double.TryParse(input, out tDoubleVal);
                        if (isNumeric == true)
                        {
                            doubleVal = tDoubleVal;
                        }
                    }
                    else
                    {
                        isNumeric = int.TryParse(input, out tIntVal);
                        if (isNumeric == true)
                        {
                            intVal = tIntVal;
                        }
                    }
                }
            }
            return isNumeric;
        }
    }

    public class CostShareValue
    {
        public string CostShareType { get; set; }
        public double? Value { get; set; }
        public bool HasValue { get; set; }

        public bool IsValid { get; set; }
    }
}
