using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.infrastructure.util.extensions;

namespace tmg.equinox.reporting
{
    public class ReportHelper
    {
        public  string RemoveCRLFFromString(string pString)
        {
            //Return empty string if null passed
            if (pString == null)
                return "";

            //Remove carriage returns
            var str = pString.Replace("\n", "").Replace("\r", "");

            //If len is more than 9 chars trim it
            return str.Length > 9 ? str.Substring(0, 9) : str;
        }

        public string CheckType(string input)
        {
            decimal n; bool isNumeric;
            string type = "string";
            if (input.Length > 0)
            {
                isNumeric = decimal.TryParse(input, out n);
                if (isNumeric)
                {
                    type = "numeric";
                }
                else
                {
                    type = "string";
                }
            }
            return type;
        }

        #region Number formatting functions
        //For Number Formating Optional Decimal (Used for SOT Rest sections)
        public string FormatNumberWithoutDecimals(string input)
        {
            string output = "";
            if (string.IsNullOrEmpty(input) == true) { return output; }
            decimal dec = 0;
            string orignalinput = input;
            if (input.StartsWith("$"))
            {
                input = input.Replace("$", "");
                decimal n; bool isNumeric;
                if (input.Length > 0)
                {
                    isNumeric = decimal.TryParse(input, out n);
                    if (isNumeric)
                    {
                        dec = Convert.ToDecimal(input);
                    }
                    else
                    {
                        return orignalinput;
                    }
                }
                else
                {
                    dec = 0;
                }
                output = dec.ToString("C", new CultureInfo("en-US"));
                if (output.Length > 3)
                {
                    output = output.Replace(".00", "");
                }
            }
            else
            {
                if (input == "YES" || input == "NO")
                {
                    input = input.ToUpperFirstLetter();
                }
                return input;
            }
            return output;
        }

        public string CheckDecimalAndFormat(string input)
        {
            string output = "";
            if (string.IsNullOrEmpty(input) == true) { return output; }
            if (input.Contains("."))
            {
                output = FormatNumberWithDecimals(input);
                if (input.Contains(".00"))
                    output = output.Replace(".00", "");

            }
            else
            {
                output = FormatNumberWithoutDecimals(input);
            }
            return output;
        }

        public string FormatStringWithDecimals(string input)
        {
            decimal dec = 0;
            string output = "";
            if (string.IsNullOrEmpty(input) == true) { return output; }
            string orignalinput = input;
            decimal n; bool isNumeric;
            string[] words;
            bool bracket = false;
            input = input.Replace("\r", " ");
            words = input.Split(' ');
            for (int i = 0; i < words.Count(); i++)
            {
                output = "";
                input = words[i];
                if (input.StartsWith("$"))
                {
                    input = input.Replace("$", "");
                    if (input.Length > 0)
                    {
                        isNumeric = decimal.TryParse(input, out n);
                        if (isNumeric)
                        {
                            dec = Convert.ToDecimal(input);
                        }
                        else
                        {
                            output = input;
                        }
                    }
                    else
                    {
                        dec = 0;
                    }
                    output = dec.ToString("C", new CultureInfo("en-US"));
                }
                else
                {
                    if (input.Contains('('))
                    {
                        input = input.Replace("(", "");
                        bracket = true;
                    }
                    input = input.Replace("$", "");
                    if (input.Length > 0)
                    {
                        isNumeric = decimal.TryParse(input, out n);
                        if (isNumeric)
                        {
                            dec = Convert.ToDecimal(input);
                            output = dec.ToString("C", new CultureInfo("en-US"));
                        }
                        else
                        {
                            output = input;
                        }
                        if (bracket)
                        {
                            output = "(" + output;
                            bracket = false;
                        }
                    }
                }
                words[i] = output;
            }
            output = string.Join(" ", words);
            return output;
        }

        //For Number Formatting with Decimal alawys (Used for SOT Premium)
        public string FormatNumberWithDecimalsYESNO(string input)
        {
            decimal dec = 0;
            string output = "";
            if (string.IsNullOrEmpty(input) == true) { return output; }
            string orignalinput = input;
            decimal n; bool isNumeric;
            if (input.StartsWith("$"))
            {
                input = input.Replace("$", "");
                if (input.Length > 0)
                {
                    isNumeric = decimal.TryParse(input, out n);
                    if (isNumeric)
                    {
                        dec = Convert.ToDecimal(input);
                    }
                    else
                    {
                        return orignalinput;
                    }
                }
                else
                {
                    dec = 0;
                }
                output = dec.ToString("C", new CultureInfo("en-US"));
            }
            else
            {
                isNumeric = decimal.TryParse(input, out n);
                if (isNumeric)
                {
                    dec = Convert.ToDecimal(input);
                    output = dec.ToString("C", new CultureInfo("en-US"));
                    return output;
                }
                else
                {
                    input = RemoveCRLFFromString(input);
                    input = input.Replace("'", "");
                    return input;
                }
            }
            if (output.StartsWith("("))
            {
                output = output.Replace("(", "-");
                output = output.Replace(")", "");
            }

            return output;
        }

        public string FormatNumberWithDecimals(string input)
        {
            decimal dec = 0;
            string output = "";
            if (string.IsNullOrEmpty(input) == true) { return output; }
            string orignalinput = input;
            decimal n; bool isNumeric;
            if (input.StartsWith("$"))
            {
                input = input.Replace("$", "");
                if (input.Length > 0)
                {
                    isNumeric = decimal.TryParse(input, out n);
                    if (isNumeric)
                    {
                        dec = Convert.ToDecimal(input);
                    }
                    else
                    {
                        return orignalinput;
                    }
                }
                else
                {
                    dec = 0;
                }
                output = dec.ToString("C", new CultureInfo("en-US"));
            }
            else
            {
                isNumeric = decimal.TryParse(input, out n);
                if (isNumeric)
                {
                    dec = Convert.ToDecimal(input);
                    output = dec.ToString("C", new CultureInfo("en-US"));
                    if (output.StartsWith("("))
                    {
                        output = output.Replace("(", "-");
                        output = output.Replace(")", "");
                    }
                    return output;
                }
                else
                {
                    if(input=="YES" || input=="NO")
                    {
                        input = input.ToUpperFirstLetter();
                    }
                    return input;
                }
            }
            if (output.StartsWith("("))
            {
                output = output.Replace("(", "-");
                output = output.Replace(")", "");
            }
            return output;
        }

        public string FormatNumberWithDecimalsWithoutDollar(string input)
        {
            decimal dec = 0;
            string output = "";
            if (string.IsNullOrEmpty(input) == true) { return output; }
            string orignalinput = input;
            decimal n; bool isNumeric;
            if (input.StartsWith("$"))
            {
                input = input.Replace("$", "");
                if (input.Length > 0)
                {
                    isNumeric = decimal.TryParse(input, out n);
                    if (isNumeric)
                    {
                        dec = Convert.ToDecimal(input);
                    }
                    else
                    {
                        return orignalinput;
                    }
                }
                else
                {
                    dec = 0;
                }
                output = dec.ToString("C", new CultureInfo("en-US"));
                output = output.Replace("$", "");
            }
            else
            {
                isNumeric = decimal.TryParse(input, out n);
                if (isNumeric)
                {
                    dec = Convert.ToDecimal(input);
                    output = dec.ToString("C", new CultureInfo("en-US"));
                    output = output.Replace("$", "");
                    return output;
                }
                else
                {
                    return input;
                }
            }
            if (output.StartsWith("("))
            {
                output = output.Replace("(", "-");
                output = output.Replace(")", "");
            }
            return output;
        }

        public string FormatStringWithDecimalsInFirstPlace(string input)
        {
            decimal dec = 0;
            string output = "";
            if (string.IsNullOrEmpty(input) == true) { return output; }
            string orignalinput = input;
            decimal n; bool isNumeric;
            string[] words;
            words = input.Split(' ');
            for (int i = 0; i < words.Count(); i++)
            {
                input = words[i];
                if (i == 0)
                {
                    if (input.StartsWith("$"))
                    {
                        input = input.Replace("$", "");
                        if (input.Length > 0)
                        {
                            isNumeric = decimal.TryParse(input, out n);
                            if (isNumeric)
                            {
                                dec = Convert.ToDecimal(input);
                            }
                            else
                            {
                                output = input;
                            }
                        }
                        else
                        {
                            dec = 0;
                        }
                        output = dec.ToString("C", new CultureInfo("en-US"));
                    }
                    else
                    {
                        input = input.Replace("$", "");
                        if (input.Length > 0)
                        {
                            isNumeric = decimal.TryParse(input, out n);
                            if (isNumeric)
                            {
                                dec = Convert.ToDecimal(input);
                                output = dec.ToString("C", new CultureInfo("en-US"));
                            }
                            else
                            {
                                output = input;
                            }
                        }
                    }
                }
                else
                {
                    if (input.StartsWith("$"))
                    {
                        input = input.Replace("$", "");
                        if (input.Length > 0)
                        {
                            isNumeric = decimal.TryParse(input, out n);
                            if (isNumeric)
                            {
                                dec = Convert.ToDecimal(input);
                            }
                            else
                            {
                                output = input;
                            }
                        }
                        else
                        {
                            dec = 0;
                        }
                        output = dec.ToString("C", new CultureInfo("en-US"));
                    }
                    else
                    {
                        output = input;
                    }
                }
                words[i] = output;
            }
            output = string.Join(" ", words);
            if (output.Contains(".00"))
                output = output.Replace(".00", "");
            return output;
        }

        public string FormatStringWithDecimalsSkipDays(string input)
        {
            decimal dec = 0;
            string output = "";
            if (string.IsNullOrEmpty(input) == true) { return output; }
            string orignalinput = input;
            decimal n; bool isNumeric;
            string[] words;
            bool bracket = false;
            input = input.Replace("\r", " ");
            words = input.Split(' ');
            for (int i = 0; i < words.Count(); i++)
            {
                output = "";
                input = words[i];
                if (input.StartsWith("$"))
                {
                    input = input.Replace("$", "");
                    if (input.Length > 0)
                    {
                        isNumeric = decimal.TryParse(input, out n);
                        if (isNumeric)
                        {
                            dec = Convert.ToDecimal(input);
                        }
                        else
                        {
                            output = input;
                        }
                    }
                    else
                    {
                        dec = 0;
                    }
                    output = dec.ToString("C", new CultureInfo("en-US"));
                }
                else
                {
                    output = input;
                }
                words[i] = output;
            }
            output = string.Join(" ", words);
            return output;
        }

        #endregion Number formatting functions

        #region Calulation functions
        //May Needed for Calulations in Reporting
        public int ConvertToInt(string input)
        {
            if (input == null)
            {
                return 0;
            }
            input = input.Replace("%", "");
            input = input.Replace("$", "");

            int n; bool isNumeric;
            if (input.Length > 0)
            {
                isNumeric = int.TryParse(input, out n);
            }
            else
            {
                n = 0;
            }
            return n;
        }

        public decimal ConvertToDecimal(string input)
        {
            if (input == null)
            {
                return 0;
            }
            input = input.Replace("%", "");
            input = input.Replace("$", "");

            decimal n; bool isNumeric;
            if (input.Length > 0)
            {
                isNumeric = decimal.TryParse(input, out n);
            }
            else
            {
                n = 0;
            }
            return n;
        }

        public string CheckAppender(string input, string multipliedValue)
        {
            if (input.Contains("%") == true && !multipliedValue.Contains("%"))
            {
                multipliedValue = multipliedValue + "%";
            }
            if (input.Contains("$") == true && !multipliedValue.Contains("$"))
            {
                multipliedValue = "$" + multipliedValue;
            }
            return multipliedValue;
        }

        public string ConvertDecimaltoInt(string input)
        {
            if (input == null) return "NA";
            string Output = "";
            Decimal n; bool isNumeric;
            if (input.Length > 0)
            {
                isNumeric = Decimal.TryParse(input, out n);
                if (isNumeric)
                {
                    if (n < 1)
                    {
                        n = n * 100;
                        n = Math.Round(n, 0);
                        Output = n.ToString() + "%";
                    }
                    else
                    {
                        Output = "$" + n.ToString();
                    }
                }
                else
                {
                    return input;
                }
            }
            else
            {
                Output = "$0";
            }
            return Output;
        }
        #endregion Calulation functions

    }
}
