using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tmg.equinox.ruleengine
{
    public static class ExpressionHelper
    {
        public static bool Equal(string val1, string val2)
        {
            decimal n1, n2;
            bool result = decimal.TryParse(val1, out n1) && decimal.TryParse(val2, out n2) ? n1 == n2 : string.Equals(val1, val2);
            return result;
        }

        public static bool NotEqual(string val1, string val2)
        {
            decimal n1, n2;
            bool result = decimal.TryParse(val1, out n1) && decimal.TryParse(val2, out n2) ? n1 != n2 : !string.Equals(val1, val2);
            return result;
        }

        public static bool GreaterThan(string val1, string val2, bool hasDate)
        {

            bool result = false;
            if (hasDate)
            {
                DateTime dt1, dt2;
                if (DateTime.TryParse(val1, out dt1) && DateTime.TryParse(val2, out dt2))
                {
                    result = dt1 > dt2;
                }
            }
            else
            {
                decimal n1, n2;
                bool isArray = IsArray(val1);
                if (isArray)
                {
                    int length = val1.Split(',').Count();
                    decimal.TryParse(val2, out n2);
                    result = length > n2; 
                }
                else if (IsCostShare(val1, val2))
                {
                    if (decimal.TryParse(ReplaceSymbol(val1), out n1) && decimal.TryParse(ReplaceSymbol(val2), out n2))
                    {
                        result = n1 > n2;
                    }
                }
                else
                {
                    if (decimal.TryParse(val1, out n1) && decimal.TryParse(val2, out n2))
                    {
                        result = n1 > n2;
                    }
                }
            }
            return result;
        }

        public static bool GreaterThanOrEqual(string val1, string val2, bool hasDate)
        {
            bool result = false;
            if (hasDate)
            {
                DateTime dt1, dt2;
                if (DateTime.TryParse(val1, out dt1) && DateTime.TryParse(val2, out dt2))
                {
                    result = dt1 >= dt2;
                }
            }
            else
            {
                decimal n1, n2;
                bool isArray = IsArray(val1);
                if (isArray)
                {
                    int length = val1.Split(',').Count();
                    decimal.TryParse(val2, out n2);
                    result = length >= n2;
                }
                else if (IsCostShare(val1, val2))
                {
                    if (decimal.TryParse(ReplaceSymbol(val1), out n1) && decimal.TryParse(ReplaceSymbol(val2), out n2))
                    {
                        result = n1 >= n2;
                    }
                }
                else
                {
                    if (decimal.TryParse(val1, out n1) && decimal.TryParse(val2, out n2))
                    {
                        result = n1 >= n2;
                    }
                }

            }
            return result;
        }

        public static bool LessThan(string val1, string val2, bool hasDate)
        {
            bool result = false;
            if (hasDate)
            {
                DateTime dt1, dt2;
                if (DateTime.TryParse(val1, out dt1) && DateTime.TryParse(val2, out dt2))
                {
                    result = dt1 < dt2;
                }
            }
            else
            {
                decimal n1, n2;
                bool isArray = IsArray(val1);
                if (isArray)
                {
                    int length = val1.Split(',').Count();
                    decimal.TryParse(val2, out n2);
                    result = length < n2;
                }
                else if (IsCostShare(val1, val2))
                {
                    if (decimal.TryParse(ReplaceSymbol(val1), out n1) && decimal.TryParse(ReplaceSymbol(val2), out n2))
                    {
                        result = n1 < n2;
                    }
                }
                else
                {   
                    if (decimal.TryParse(val1, out n1) && decimal.TryParse(val2, out n2))
                    {
                        result = n1 < n2;
                    }
                }
                
            }
            return result;
        }

        public static bool LessThanOrEqual(string val1, string val2, bool hasDate)
        {
            bool result = false;
            if (hasDate)
            {
                DateTime dt1, dt2;
                if (DateTime.TryParse(val1, out dt1) && DateTime.TryParse(val2, out dt2))
                {
                    result = dt1 <= dt2;
                }
            }
            else
            {
                decimal n1, n2;
                bool isArray = IsArray(val1);
                if (isArray)
                {
                    int length = val1.Split(',').Count();
                    decimal.TryParse(val2, out n2);
                    result = length <= n2;
                }
                else if (IsCostShare(val1, val2))
                {
                    if (decimal.TryParse(ReplaceSymbol(val1), out n1) && decimal.TryParse(ReplaceSymbol(val2), out n2))
                    {
                        result = n1 <= n2;
                    }
                }
                else
                {
                    if (decimal.TryParse(val1, out n1) && decimal.TryParse(val2, out n2))
                    {
                        result = n1 <= n2;
                    }
                }

                
            }
            return result;
        }

        public static bool Contains(string val1, string val2)
        {
            bool result = false;
            // If val1 is multiselct dropdown
            if (val1.StartsWith("[") && val1.EndsWith("]"))
            {
                List<string> values =JToken.Parse(val1).ToObject<List<string>>();
                if (values != null && values.Count> 0)
                {
                    result = values.IndexOf(val2) > -1;
                }
            }
            else
            {
                result = val1.IndexOf(val2) > -1;
            }
            return result;
        }

        public static bool NotContains(string val1, string val2)
        {
            bool result = val1.IndexOf(val2) < 0;
            return result;
        }
        public static bool IsNull(string value)
        {
            bool result = string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value);
            return result;
        }

        public static string ReplaceSymbol(string val)
        {
            if (!string.IsNullOrEmpty(val))
            {
                return val = val.Replace("$", "").Replace("%", "");
            }
            else
            {
                return val;
            }
        }
        private static bool IsArray(string leftOperand)
        {
            bool isArray = false;
            try
            {
                if (leftOperand!=null)
                {
                    if (leftOperand.StartsWith("[") && leftOperand.EndsWith("]"))
                    {
                        isArray = true;
                    }
                }
            }
            catch (Exception ex)
            {
                isArray = false;
            }
            return isArray;
        }

        private static bool IsCostShare(string leftOperand, string rightOperand)
        {
            bool isCostShare = false;
            try
            {
                if (leftOperand != null && rightOperand != null)
                {
                    if (leftOperand.IndexOf('$') > -1 && rightOperand.IndexOf('$') > -1) isCostShare = true;
                    else if (leftOperand.IndexOf('%') > -1 && rightOperand.IndexOf('%') > -1) isCostShare = true;
                }
            }
            catch (Exception ex)
            {
                isCostShare = false;
            }
            return isCostShare;
        }

    }
}