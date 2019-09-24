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
    public class FMTPlaceHolderFunctionEvaluator : IPlaceHolderFunctionEvaluator
    {
        string _placeHolder;
        JToken _dataSource;
        Dictionary<string, JToken> _sources;
        Regex _numericRegex;
        Regex _extractDigitRegex;
        List<LanguageFormats> _languageFormats;
        public FMTPlaceHolderFunctionEvaluator(string placeHolder, JToken dataSource, Dictionary<string, JToken> sources, List<LanguageFormats> languageFormats)
        {
            _placeHolder = placeHolder;
            _dataSource = dataSource;
            _sources = sources;
            _numericRegex = new Regex(@"^[\$]{0,1}\d*[,]{0,1}\d*[.]{0,1}\d*[%]{0,1}$");
            _extractDigitRegex = new Regex(@"[^\d.]");
            _languageFormats = languageFormats;
        }

        public string Evaluate()
        {
            string result = "";
            //get parameters
            string extract = _placeHolder.Replace("FMT:", "");
            extract = extract.Trim(new char[] { '[', ']' });
            extract = extract.Replace("/,", "&com;");

            if (HasExceptionalCase(extract))
            {
                return HandleExceptionalCase(extract);
            }
            string[] parameters = extract.Split(',');
            for (int i = 0; i < parameters.Length; i++)
            {
                parameters[i] = parameters[i].Replace("&com;", ",");
            }

            //get first parameter
            if (parameters != null && parameters.Length > 0)
            {
                if (parameters.Length == 2)
                {
                    string firstParam = parameters[0];
                    firstParam = firstParam.Insert(0, "{{");
                    firstParam = firstParam.Insert(firstParam.Length, "}}");
                    string secondParam = parameters[1];
                    List<JToken> tokens = new List<JToken>();
                    tokens.Add(_dataSource);
                    PlaceHolderProcessor proc = new PlaceHolderProcessor(firstParam, tokens, _sources);
                    firstParam = proc.Process();
                    result = ProcessFormat(firstParam, secondParam);
                }
                if (parameters.Length == 3)
                {
                    string firstParam = parameters[0];
                    firstParam = firstParam.Insert(0, "{{");
                    firstParam = firstParam.Insert(firstParam.Length, "}}");
                    string secondParam = parameters[1];
                    secondParam = secondParam.Insert(0, "{{");
                    secondParam = secondParam.Insert(secondParam.Length, "}}");
                    string thirdParam = parameters[2];
                    List<JToken> tokens = new List<JToken>();
                    tokens.Add(_dataSource);
                    PlaceHolderProcessor proc = new PlaceHolderProcessor(firstParam, tokens, _sources);
                    firstParam = proc.Process();
                    proc = new PlaceHolderProcessor(secondParam, tokens, _sources);
                    secondParam = proc.Process();
                    result = ProcessFormat(firstParam, secondParam, thirdParam);
                }
                if (parameters.Length == 4)
                {
                    string firstParam = parameters[0];
                    firstParam = firstParam.Insert(0, "{{");
                    firstParam = firstParam.Insert(firstParam.Length, "}}");
                    string secondParam = parameters[1];
                    string thirdParam = parameters[2];
                    string fourthParam = parameters[3];
                    List<JToken> tokens = new List<JToken>();
                    tokens.Add(_dataSource);
                    PlaceHolderProcessor proc = new PlaceHolderProcessor(firstParam, tokens, _sources);
                    firstParam = proc.Process();
                    if (fourthParam.ToUpper() == "STATELIST")
                    {
                        secondParam = secondParam.Insert(0, "{{");
                        secondParam = secondParam.Insert(secondParam.Length, "}}");
                        proc = new PlaceHolderProcessor(secondParam, tokens, _sources);
                        secondParam = proc.Process();
                    }
                    result = ProcessFormat(firstParam, secondParam, thirdParam, fourthParam);
                }
                if (parameters.Length == 5)
                {
                    string firstParam = parameters[0];
                    firstParam = firstParam.Insert(0, "{{");
                    firstParam = firstParam.Insert(firstParam.Length, "}}");
                    string secondParam = parameters[1];
                    string thirdParam = parameters[2];
                    thirdParam = thirdParam.Insert(0, "{{");
                    thirdParam = thirdParam.Insert(thirdParam.Length, "}}");
                    string fourthParam = parameters[3];
                    string fifthParam = parameters[4];
                    List<JToken> tokens = new List<JToken>();
                    tokens.Add(_dataSource);
                    PlaceHolderProcessor proc = new PlaceHolderProcessor(firstParam, tokens, _sources);
                    firstParam = proc.Process();
                    proc = new PlaceHolderProcessor(thirdParam, tokens, _sources);
                    thirdParam = proc.Process();
                    result = ProcessFormat(firstParam, secondParam, thirdParam, fourthParam, fifthParam);
                }
            }
            return result;
        }

        private string ProcessFormat(string firstParam, string secondParam)
        {
            string result = "";
            switch (secondParam)
            {
                case "PN":
                    result = ProcessFMTPN(firstParam);
                    break;
                case "PHN":
                    result = ProcessFMTPHN(firstParam);
                    break;
                case "PNT":
                    result = ProcessFMTPNT(firstParam);
                    break;
                case "NTW":
                    result = ProcessFMTNTW(firstParam);
                    break;
                case "NTNW":
                    result = ProcessFMTNTNW(firstParam);
                    break;
                case "TC":
                    result = ProcessFMTTC(firstParam);
                    break;
                case "PHXT":
                    result = ProcessFMTPHXT(firstParam);
                    break;
                case "PHIF":
                    result = ProcessFMTPHIF(firstParam);
                    break;
                case "STATENAME":
                    result = ProcessSTATENAME(firstParam);
                    break;
                case "COUNTYLIST":
                    result = ProcessCOUNTYLIST(firstParam);
                    break;
                case "URL":
                    result = ProcessURL(firstParam);
                    break;
                case "HURL":
                    result = ProcessHURL(firstParam);
                    break;
                case "C2":
                    result = ProcessCurrency2(firstParam);
                    break;
                case "CURRENCY":
                    result = ProcessCURRENCY(firstParam, "C");
                    break;
                case "CURRENCY0":
                    result = ProcessCURRENCY(firstParam, "C0");
                    break;
                case "NUMBER":
                    result = ProcessNUMBER(firstParam);
                    break;
                case "LC":
                    result = ProcessFMTLC(firstParam);
                    break;
                case "PT":
                    result = ProcessFMTPT(firstParam);
                    break;
                case "CS":
                    result = ProcessFMTCS(firstParam);
                    break;
                case "CSA":
                    result = ProcessFMTCSA(firstParam);
                    break;
            }
            return result;
        }

        private string ProcessFormat(string firstParam, string secondParam, string thirdParam)
        {
            string result = "";
            switch (thirdParam)
            {
                case "STATELIST":
                    result = ProcessSTATELIST(firstParam, secondParam);
                    break;
                case "JOIN":
                    result = ProcessJOIN(firstParam, secondParam);
                    break;
                case "EXCEPT":
                    result = ProcessEXCEPT(firstParam, secondParam);
                    break;
                case "EXTRACT":
                    result = ProcessEXTRACT(firstParam, secondParam);
                    break;
                case "SUM":
                    result = ProcessSUM(firstParam, secondParam);
                    break;
                case "ADDDAYS":
                    result = ProcessADDDAYS(firstParam, secondParam);
                    break;
                case "FORMATDATE":
                    result = ProcessFORMATDATE(firstParam, secondParam);
                    break;
                case "GETDATE":
                    result = ProcessGetDate(firstParam, secondParam);
                    break;
                case "SUB":
                    result = ProcessSUB(firstParam, secondParam);
                    break;
            }
            return result;
        }

        private string ProcessFormat(string firstParam, string secondParam, string thirdParam, string fourthParam)
        {
            string result = "";
            switch (fourthParam)
            {
                case "REPLACE":
                    result = ProcessREPLACE(firstParam, secondParam, thirdParam);
                    break;
                case "EXCEPT":
                    result = ProcessEXCEPT(firstParam, secondParam, thirdParam);
                    break;
                case "JOIN":
                    result = ProcessJOIN(firstParam, secondParam, thirdParam == "true" ? true : false);
                    break;
                case "STATELIST":
                    result = ProcessSTATELIST(firstParam, secondParam, thirdParam);
                    break;
            }
            return result;
        }

        private string ProcessFormat(string firstParam, string secondParam, string thirdParam, string fourthParam, string fifthParam)
        {
            string result = "";
            switch (fifthParam)
            {
                case "EXCEPT":
                    result = ProcessEXCEPT(firstParam, secondParam, thirdParam, fourthParam);
                    break;
                case "EXCEPTOR":
                    result = ProcessEXCEPT(firstParam, secondParam, thirdParam, fourthParam).Replace("and", "and/or");
                    break;
                case "STATELIST":
                    result = ProcessSTATELIST(firstParam, thirdParam, secondParam,fourthParam);
                    break;

            }
            return result;
        }

        private string ProcessFMTPN(string input)
        {
            string result = "";
            result = Regex.Replace(input, RegexConstants.FMT_PN, "");
            result = result.Trim();
            return result;
        }
        private string ProcessFMTPHN(string input)
        {
            string result = "";
            result = input.Replace("(", "").Replace(")", "-").Replace(" ", "");
            result = result.Trim();
            return result;
        }
        private string ProcessFMTPNT(string input)
        {
            string result = "";
            result = Regex.Replace(input, RegexConstants.FMT_PN, "$1");
            result = result.Replace("HAP", "").Trim();
            return result;
        }

        //only supports 1 to 6 - can be enhanced for any numeric using humanizer library
        private string ProcessFMTNTW(string input)
        {
            string words = "";
            int val;
            if (int.TryParse(input, out val) == true)
            {
                var unitsMap = new[]
                {
                     "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen"
                };
                var tensMap = new[]
                {
                    "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety"
                };
                if (val < 20) words += unitsMap[val];
                else
                {
                    words += tensMap[val / 10];
                    if ((val % 10) > 0) words += "-" + unitsMap[val % 10];
                }
            }
            return words;
        }

        //only supports 1 to 6 - can be enhanced for any numeric using humanizer library 
        private string ProcessFMTNTNW(string input)
        {
            string result = input;
            int val;
            if (int.TryParse(input, out val) == true)
            {
                switch (val)
                {
                    case 1:
                        result = "one (1)";
                        break;
                    case 2:
                        result = "two (2)";
                        break;
                    case 3:
                        result = "three (3)";
                        break;
                    case 4:
                        result = "four (4)";
                        break;
                    case 5:
                        result = "five (5)";
                        break;
                    case 6:
                        result = "six (6)";
                        break;
                }
            }
            return result;
        }

        private string ProcessFMTTC(string input)
        {
            string result = "";
            input = input.ToLower();
            result = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input);
            return result;
        }

        private string ProcessFMTPHXT(string input)
        {
            string result = "";
            result = Regex.Replace(input, RegexConstants.FMT_PHXT, "$1");
            return result;
        }

        private string ProcessFMTPHIF(string input)
        {
            string result = "";
            result = Regex.Replace(input, RegexConstants.FMT_PHF, "1-$1-");
            return result;
        }

        private string ProcessURL(string input)
        {
            string result = "";
            if (!String.IsNullOrEmpty(input))
            {
                input = input.Trim();
                input = input.Replace(" ", "");
                input = input.Replace("http://", "");
                input.Replace("https://", "");
                if (!input.StartsWith("www."))
                {
                    input = input.Insert(0, "www.");
                }
                input = input.ToLower();
                result = input;
            }
            return result;
        }

        private string ProcessHURL(string input)
        {
            string result = string.Empty;

            if (!String.IsNullOrEmpty(input))
            {
                result = ProcessURL(input);
                result = "http://" + result;
            }
            return result;
        }
        private string ProcessCurrency2(string input)
        {
            string result = "";
            result = Regex.Replace(input, RegexConstants.FMT_PHF, "1-$1-");
            return result;
        }
        private string ProcessNUMBER(string input)
        {
            string result = "";
            if (input != null && input != "")
            {
                string resultNumber = Regex.Replace(input, "[^.0-9]", "");
                if (resultNumber != "")
                {
                    result = double.Parse(resultNumber).ToString();
                }
            }
            return result;
        }
        private string ProcessCOUNTYLIST(string counties)
        {
            string result = "";
            if (!String.IsNullOrEmpty(counties))
            {
                string[] countyArray = counties.Split(',');
                for (int idx = 0; idx < countyArray.Length; idx++)
                {
                    countyArray[idx] = countyArray[idx].Trim();
                }
                Array.Sort(countyArray);
                if (countyArray.Length > 1)
                {
                    for (int idx = 0; idx < countyArray.Length; idx++)
                    {
                        if (idx == countyArray.Length - 1)
                        {
                            result = result + " and " + countyArray[idx];
                        }
                        else
                        {
                            result = result + ", " + countyArray[idx];
                        }
                    }
                    result = result.TrimStart(',');
                }
                else
                {
                    result = countyArray[0];
                }
            }
            return result;
        }

        private string ProcessSTATENAME(string state)
        {
            string stateName = "";
            if (!String.IsNullOrEmpty(state))
            {
                string[] stateNames = state.Split(',');
                int count = stateNames.Length;
                for (int idx = 0; idx < count; idx++)
                {
                    string st = stateNames[idx];
                    List<StateCounty> stateCountyList = StatesAndCounties.GetStateCountyList();
                    st = stateCountyList.Where(a => a.StateCode == st.Trim()).Select(a => a.StateName).FirstOrDefault();
                    if ((idx == (count - 1)) && (count > 1))
                    {
                        stateName = stateName + " and " + st;
                    }
                    else
                    {
                        stateName = stateName + "," + st;
                    }
                }
                stateName = stateName.TrimStart(',');
            }
            return stateName;
        }

        private string ProcessSTATELIST(string states, string counties, string variation = null, string suffix = null)
        {
            string result = "";
            bool error = false;
            string countiessuffix = suffix == null ? string.Empty : suffix;
            bool isHTMLInclude = (variation == null || variation.ToUpper() == "WITHCOUNTY") ? true : false;
            string strAddCountyWord = (variation != null && variation.ToUpper().Contains("COUNTY")) ? " county" : "";
            bool onlyContents = (variation != null && (variation.ToUpper() == "CONTENT" || variation.ToUpper() == "CONTENTWITHCOUNTY")) ? true : false;

            try
            {
                result = ProcessSTATELIST2(states, counties, strAddCountyWord, isHTMLInclude, onlyContents, countiessuffix);
            }
            catch
            {
                error = true;
            }
            if (!String.IsNullOrEmpty(states) && error == true)
            {
                List<StateCounty> stateCountyList = StatesAndCounties.GetStateCountyList();
                string[] stateList = (from sta in states.Split(',') select sta.Trim()).ToArray();
                string[] countyList = new string[] { };
                if (!String.IsNullOrEmpty(counties))
                {
                    countyList = (from cty in counties.Split(',') select cty.Trim()).ToArray();
                }
                foreach (string state in stateList)
                {
                    List<StateCounty> sctList = stateCountyList.Where(a => a.StateCode == state).ToList();
                    List<string> countyNames = sctList.Select(a => a.CountyName).ToList();
                    var stateCounties = countyList.Intersect(countyNames, StringComparer.InvariantCultureIgnoreCase);
                    string stateName = "";
                    if (sctList != null && sctList.Count > 0)
                    {
                        stateName = sctList.First().StateName;
                    }
                    string stateCountyNames = "";
                    if (stateCounties != null && stateCounties.Count() > 0)
                    {
                        string[] stateCountyArray = stateCounties.ToArray();
                        Array.Sort(stateCountyArray);
                        for (int idx = 0; idx < stateCountyArray.Length; idx++)
                        {
                            if (idx < stateCounties.Count() - 1 || stateCounties.Count() == 1)
                            {
                                stateCountyNames = stateCountyNames + ", " + stateCountyArray[idx] + strAddCountyWord;
                            }
                            else
                            {
                                stateCountyNames = stateCountyNames + " and " + stateCountyArray[idx] + strAddCountyWord + "<br/>";
                            }
                        }
                        stateCountyNames = stateCountyNames.TrimStart(new char[] { ',', ' ' });
                        stateCountyNames = stateCountyNames.Remove(stateCountyNames.LastIndexOf("<br/>"));
                    }
                    if (!String.IsNullOrEmpty(stateName))
                    {
                        string format = this.GetStateListStaticText(1, stateCounties.Count(), isHTMLInclude, onlyContents);
                        result = result + String.Format(format, stateName, stateCountyNames);
                    }
                }
            }
            return result;
        }

        private string ProcessSTATELIST2(string states, string counties, string strAddCountyWord, bool isHTMLInclude, bool onlyContents, string countiessuffix)
        {
            string result = "";
            if (!String.IsNullOrEmpty(states))
            {
                List<StateCounty> stateCountyList = StatesAndCounties.GetStateCountyList();
                string[] stateList = (from sta in states.Split(',') select sta.Trim()).ToArray();
                string[] countyList = new string[] { };
                if (!String.IsNullOrEmpty(counties))
                {
                    countyList = (from cty in counties.Split(',') select cty.Trim()).ToArray();
                }
                Dictionary<string, List<string>> stateCountyDict = new Dictionary<string, List<string>>();
                int stateIdx = 0;
                List<string> stateCountiesList = new List<string>();
                for (int cIdx = 0; cIdx < countyList.Length; cIdx++)
                {
                    if (cIdx == 0)
                    {
                        stateCountiesList = new List<string>();
                        stateCountiesList.Add(countyList[cIdx]);
                        if (stateIdx >= stateList.Length)
                        {
                            break;
                        }
                        stateCountyDict.Add(stateList[stateIdx], stateCountiesList);
                    }
                    else
                    {
                        if (String.Compare(countyList[cIdx], 0, countyList[cIdx - 1], 0, 1) >= 0)
                        {
                            List<string> cts = stateCountyDict[stateList[stateIdx]];
                            cts.Add(countyList[cIdx]);
                        }
                        else
                        {
                            stateCountiesList = new List<string>();
                            stateCountiesList.Add(countyList[cIdx]);
                            stateIdx++;
                            if (stateIdx >= stateList.Length)
                            {
                                break;
                            }
                            stateCountyDict.Add(stateList[stateIdx], stateCountiesList);
                        }
                    }
                }
                foreach (string state in stateList)
                {
                    List<string> stCountyList = new List<string>();
                    if (stateCountyDict.ContainsKey(state))
                    {
                        stCountyList = stateCountyDict[state];
                    }
                    //get county list for state
                    List<StateCounty> sctList = stateCountyList.Where(a => a.StateCode == state).ToList();
                    List<string> countyNames = sctList.Select(a => a.CountyName).ToList();
                    if (stCountyList != null && stCountyList.Count > 0)
                    {
                        var stateCounties = stCountyList.Intersect(countyNames, StringComparer.InvariantCultureIgnoreCase);
                        string stateName = "";
                        if (sctList != null && sctList.Count > 0)
                        {
                            stateName = sctList.First().StateName;
                        }
                        string stateCountyNames = "";
                        if (stateCounties != null && stateCounties.Count() > 0)
                        {
                            string[] stateCountyArray = stateCounties.ToArray();
                            Array.Sort(stateCountyArray);
                            for (int idx = 0; idx < stateCountyArray.Length; idx++)
                            {
                                if (idx < stateCounties.Count() - 1 || stateCounties.Count() == 1)
                                {
                                    stateCountyNames = stateCountyNames + ", " + stateCountyArray[idx] + strAddCountyWord;
                                }
                                else
                                {
                                    stateCountyNames = stateCountyNames + " and " + stateCountyArray[idx] + strAddCountyWord + countiessuffix + "<br/>";
                                }
                            }
                            stateCountyNames = stateCountyNames.TrimStart(new char[] { ',', ' ' });
                            
                        }
                        if (!String.IsNullOrEmpty(stateName))
                        {
                            string format = this.GetStateListStaticText(1, stateCounties.Count(), isHTMLInclude, onlyContents);
                            result = result + String.Format(format, stateName, stateCountyNames);
                        }
                    }
                }
            }
            result = result.Remove(result.LastIndexOf("<br/>"));
            return result;
        }

        private string GetStateListStaticText(int states, int counties, bool isHTMLInclude, bool onlyContents)
        {
            string text = string.Empty;
            string HTMLFirstPart = "<p style=\"margin-right: 0in; margin-left: 0in; font-size: 12pt; font-family: 'Times New Roman', serif; \">";
            string HTMLLastPart = "</p>";

            if (states == 1 && counties <= 0)
            {
                text = (isHTMLInclude ? HTMLFirstPart : "") + (!onlyContents ? "Our service area includes this state: " : "") + "{0}" + (isHTMLInclude ? HTMLLastPart : "");
            }
            else if (states > 1 && counties <= 0)
            {
                text = (isHTMLInclude ? HTMLFirstPart : "") + (!onlyContents ? "Our service area includes these states: " : "") + "{0}" + (isHTMLInclude ? HTMLLastPart : "");
            }
            else if (counties == 1)
            {
                text = (isHTMLInclude ? HTMLFirstPart : "") + (!onlyContents ? "Our service area includes this county in " : "") + "{0}: {1}" + (isHTMLInclude ? HTMLLastPart : "");
            }
            else if (counties > 1)
            {
                text = (isHTMLInclude ? HTMLFirstPart : "") + (!onlyContents ? "Our service area includes these counties in " : "") + "{0}: {1}" + (isHTMLInclude ? HTMLLastPart : "");
            }
            return text;
        }

        private string ProcessCURRENCY(string input, string format)
        {
            NumberStyles style = NumberStyles.Number | NumberStyles.AllowCurrencySymbol;
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            string currency = string.Empty;
            if (!string.IsNullOrEmpty(input))
            {
                double result;
                if (double.TryParse(input, style, culture, out result))
                {
                    if (string.Equals("C0", format, StringComparison.OrdinalIgnoreCase))
                    {
                        string[] values = result.ToString().Split('.');
                        string fraction = "00";
                        if (values.Length > 1)
                        {
                            fraction = values[1];
                        }
                        currency = result.ToString(fraction != "00" ? "C" : "C0");
                    }
                    else
                    {
                        currency = result.ToString(format);
                    }
                }
            }

            return currency;
        }

        private string ProcessFMTLC(string input)
        {
            string result = input;
            if (!string.IsNullOrWhiteSpace(input))
            {
                result = input.ToLower();
            }
            return result;
        }

        private string ProcessFMTPT(string input)
        {
            string result = input;
            if (!string.IsNullOrWhiteSpace(input))
            {
                result = result.Replace("Local ", "");
                result = result.Replace("Regional ", "");
            }
            return result;
        }

        private string ProcessREPLACE(string firstParam, string secondParam, string thirdParam)
        {
            string result = string.Empty;
            if (!string.IsNullOrWhiteSpace(firstParam) && !string.IsNullOrWhiteSpace(secondParam))
            {
                result = firstParam.Replace(secondParam, thirdParam);
            }
            return result;
        }

        private string ProcessJOIN(string firstParam, string secondParam, bool appendTier = false)
        {
            secondParam = secondParam.Replace("{{", "").Replace("}}", "");
            string result = string.Empty;
            if (!string.IsNullOrWhiteSpace(firstParam))
            {
                string[] items = JArray.Parse(firstParam).Select(s => (string)s).ToArray();
                if (string.Equals(secondParam, "true", StringComparison.OrdinalIgnoreCase))
                {
                    List<int> lstIndex = items.Where(s => s.IndexOf("1") >= 0).Select(s => s.IndexOf("1")).ToList();
                    lstIndex.Sort();
                    if (lstIndex[0] == 0)
                    {
                        lstIndex.RemoveAt(0);
                        lstIndex.Insert(lstIndex.Count, 0);
                    }
                    int index = 0;
                    foreach (var item in lstIndex)
                    {
                        index = index + 1;
                        if (index == lstIndex.Count)
                        {
                            result = result + (appendTier ? "Tier " : "") + (item == 0 ? items[0].Length : item);
                        }
                        else
                        {
                            result = result + (appendTier ? "Tier " : "") + item + (index == lstIndex.Count - 1 ? " and " : ", ");
                        }
                    }
                }
                else
                {
                    result = string.Join(", ", items.Take(items.Count() - 1)) + (items.Count() > 1 ? " and " : "") + items.LastOrDefault();
                }
            }

            return result;
        }

        private string ProcessJOIN(string firstParam, bool appendTier, bool appendDescription, string[] tierDescriptions)
        {
            string result = string.Empty;
            if (!string.IsNullOrWhiteSpace(firstParam))
            {
                string[] items = JArray.Parse(firstParam).Select(s => (string)s).ToArray();
                if (appendTier)
                {
                    List<int> lstIndex = items.Where(s => s.IndexOf("1") >= 0).Select(s => s.IndexOf("1")).ToList();
                    lstIndex.Sort();
                    if (lstIndex[0] == 0)
                    {
                        lstIndex.RemoveAt(0);
                        lstIndex.Insert(lstIndex.Count, 0);
                    }
                    int index = 0;
                    foreach (var item in lstIndex)
                    {
                        index = index + 1;
                        if (index == lstIndex.Count)
                        {
                            result = result + (appendTier ? "Tier " : "") + (item == 0 ? items[0].Length : item) + (appendDescription ? " (" + tierDescriptions[(item == 0 ? items[0].Length : item) - 1] + ")" : "");
                        }
                        else
                        {
                            result = result + (appendTier ? "Tier " : "") + item + (appendDescription ? " (" + tierDescriptions[item - 1] + ")" : "") + (index == lstIndex.Count - 1 ? " and " : ", ");
                        }
                    }
                }
                else if (!appendTier && appendDescription)
                {
                    List<int> lstIndex = items.Where(s => s.IndexOf("1") >= 0).Select(s => s.IndexOf("1")).ToList();
                    lstIndex.Sort();
                    if (lstIndex[0] == 0)
                    {
                        lstIndex.RemoveAt(0);
                        lstIndex.Insert(lstIndex.Count, 0);
                    }
                    int index = 0;
                    foreach (var item in lstIndex)
                    {
                        index = index + 1;
                        if (index == lstIndex.Count)
                        {
                            result = result + (appendDescription ? tierDescriptions[(item == 0 ? items[0].Length : item) - 1] : "");
                        }
                        else
                        {
                            result = result + (appendDescription ? tierDescriptions[item - 1] : "") + (index == lstIndex.Count - 1 ? " and " : ", ");
                        }
                    }
                }
                else
                {
                    List<int> lstIndex = items.Where(s => s.IndexOf("1") >= 0).Select(s => s.IndexOf("1")).ToList();
                    lstIndex.Sort();
                    if (lstIndex[0] == 0)
                    {
                        lstIndex.RemoveAt(0);
                        lstIndex.Insert(lstIndex.Count, 0);
                    }
                    int index = 0;
                    foreach (var item in lstIndex)
                    {
                        index = index + 1;
                        if (index == lstIndex.Count)
                        {
                            result = result + (item == 0 ? items[0].Length : item);
                        }
                        else
                        {
                            result = result + item + (index == lstIndex.Count - 1 ? " and " : ", ");
                        }
                    }
                    //result = string.Join(", ", items.Take(items.Count() - 1)) + (items.Count() > 1 ? " and " : "") + items.LastOrDefault();
                }
            }
            return result;
        }
        private string ProcessEXCEPT(string firstParam, string secondParam)
        {
            string[] allItems = new string[] { "010000", "001000", "000100", "000010", "000001", "100000" };
            string result = string.Empty;
            if (!string.IsNullOrWhiteSpace(secondParam))
            {
                string[] selectedItems = new string[] { "" };
                if (!string.IsNullOrWhiteSpace(firstParam))
                {
                    selectedItems = JArray.Parse(firstParam).Select(s => (string)s).ToArray();
                }
                string[] nonSelectedItems = allItems.Except(selectedItems).ToArray();

                string val = "[";
                foreach (var item in nonSelectedItems)
                {
                    val = val + "\"" + item + "\"" + ",";
                }
                val = val.TrimEnd(',') + "]";

                result = ProcessJOIN(val, secondParam, true);
            }
            return result;
        }

        private string ProcessEXCEPT(string firstParam, string secondParam, string thirdParam, string fourthParam)
        {
            string result = string.Empty;
            string[] tierValues = new string[] { "010000", "001000", "000100", "000010", "000001", "100000" };
            string[] tierDescriptions = null;

            int tiers = 0;
            string[] allItems = null;
            if (int.TryParse(thirdParam, out tiers))
            {
                allItems = new string[tiers];
                Array.Copy(tierValues, allItems, tiers);
            }

            if (!string.IsNullOrWhiteSpace(secondParam))
            {
                string[] selectedItems = new string[] { "" };
                if (!string.IsNullOrWhiteSpace(firstParam))
                {
                    selectedItems = JArray.Parse(firstParam).Select(s => (string)s).ToArray();
                }
                string[] nonSelectedItems = allItems.Except(selectedItems).ToArray();

                string val = "[";
                foreach (var item in nonSelectedItems)
                {
                    val = val + "\"" + item + "\"" + ",";
                }
                val = val.TrimEnd(',') + "]";
                bool isFormated = false; bool.TryParse(secondParam, out isFormated);
                bool descriptionRequired = false;
                if (!String.IsNullOrEmpty(fourthParam))
                {
                    descriptionRequired = true;
                    tierDescriptions = fourthParam.Split('|');
                }
                result = ProcessJOIN(val, isFormated, descriptionRequired, tierDescriptions);
            }
            return result;
        }
        private string ProcessEXCEPT(string firstParam, string secondParam, string thirdParam)
        {
            string result = string.Empty;
            if (!string.IsNullOrWhiteSpace(secondParam))
            {
                List<int> numbers = new List<int>();
                for (int i = 1; i <= Convert.ToInt32(secondParam); i++)
                {
                    numbers.Add(i);
                }

                string[] selectedItems = new string[] { "" };
                if (!string.IsNullOrWhiteSpace(firstParam))
                {
                    selectedItems = firstParam.Split(new char[] { ',' });
                }
                string[] nonSelectedItems = numbers.Select(i => i.ToString()).ToArray().Except(selectedItems).ToArray();

                string val = "[";
                foreach (var item in nonSelectedItems)
                {
                    val = val + "\"" + item + "\"" + ",";
                }
                val = val.TrimEnd(',') + "]";

                result = ProcessJOIN(val, secondParam, thirdParam == "true" ? true : false);
            }
            return result;
        }

        private string ProcessEXTRACT(string firstParam, string secondParam)
        {
            string result = string.Empty;
            secondParam = secondParam.Replace("{{", "").Replace("}}", "");
            string[] sents = firstParam.Split(new char[] { '.', '\r', '\n' });
            foreach (string item in sents)
            {
                if (item.IndexOf(secondParam) >= 0)
                {
                    result = result + item;
                }
            }

            return result;
        }

        private string ProcessFMTCS(string input)
        {
            bool isCopay = false;
            bool isCoins = false;
            string currency = string.Empty;
            if (!string.IsNullOrEmpty(input))
            {
                if (input.IndexOf("$") >= 0)
                {
                    isCopay = true;
                    input = input.Replace("$", "");
                }
                else if (input.IndexOf("%") > 0)
                {
                    isCoins = true;
                    input = input.Replace("%", "");
                }
                double result;
                if (double.TryParse(input, out result))
                {
                    currency = isCopay ? result.ToString("C0") : result.ToString("F0");
                    currency = isCopay ? (currency + " copayment") : isCoins ? (currency + "%" + " of total cost") : currency;
                }
            }

            return currency;
        }
        private bool HasExceptionalCase(string input)
        {
            bool flag = false;
            if (input.Contains("REPLACECOMMA"))
            {
                flag = true;
            }
            return flag;
        }
        private string HandleExceptionalCase(string input)
        {
            if (input.Contains("REPLACECOMMA"))
            {
                return ProcessReplaceComma(input);
            }
            else
            {
                return input;
            }
        }

        private string ProcessReplaceComma(string input)
        {
            List<string> removealStr = new List<string>(new string[] { "[", "]", "{", "}", "REPLACECOMMA", "," });

            if (!string.IsNullOrEmpty(input))
            {
                foreach (var item in removealStr)
                {
                    input = input.Replace(item, "");
                }
            }
            return input;
        }
        private string ProcessFMTCSA(string input)
        {
            bool isCopay = false;
            bool isCoins = false;
            string currency = string.Empty;
            if (!string.IsNullOrEmpty(input))
            {
                if (input.IndexOf("$") >= 0)
                {
                    isCopay = true;
                    input = input.Replace("$", "");
                }
                else if (input.IndexOf("%") > 0)
                {
                    isCoins = true;
                    input = input.Replace("%", "");
                }
                double result;
                if (double.TryParse(input, out result))
                {
                    currency = isCopay ? result.ToString("C0") : result.ToString("F0");
                    currency = isCopay ? (currency + " per prescription") : isCoins ? (currency + "%" + " of the total cost") : currency;
                }
            }

            return currency;
        }

        private string ProcessGetDate(string input, string format)
        {
            format = format.Replace("{", "").Replace("}", "");
            string currentDate = string.Empty;
            currentDate = DateTime.Now.ToString(format);
            return currentDate;
        }

        private string ProcessADDDAYS(string firstParam, string secondParam)
        {
            int addDaysInt;
            string result = string.Empty;

            firstParam = firstParam.Replace("{", "").Replace("}", "");
            secondParam = secondParam.Replace("{", "").Replace("}", "");

            if (!string.IsNullOrEmpty(firstParam) && int.TryParse(secondParam, out addDaysInt))
            {
                DateTime dtResult = DateTime.Parse(firstParam).AddDays(addDaysInt);
                result = dtResult.ToShortDateString();
            }
            return result;
        }

        private string ProcessFORMATDATE(string firstParam, string secondParam)
        {
            string result = string.Empty;
            firstParam = firstParam.Replace("{", "").Replace("}", "");
            secondParam = secondParam.Replace("{", "").Replace("}", "");

            if (!string.IsNullOrEmpty(firstParam) && !string.IsNullOrEmpty(secondParam))
            {
                secondParam = secondParam.Replace("-", ",");
                result = DateTime.Parse(firstParam).ToString(secondParam);
            }
            return result;
        }

        private string ProcessSUM(string firstParam, string secondParam)
        {
            firstParam = string.IsNullOrEmpty(firstParam) ? "0" : firstParam;
            secondParam = string.IsNullOrEmpty(secondParam) ? "0" : secondParam;

            secondParam = secondParam.Replace("{{", "").Replace("}}", "");
            string result = string.Empty;
            if (!string.IsNullOrWhiteSpace(firstParam) && !string.IsNullOrWhiteSpace(secondParam))
            {
                int val1; int val2;
                if (Int32.TryParse(firstParam, out val1) && Int32.TryParse(secondParam, out val2))
                {
                    result = Convert.ToString(val1 + val2);
                }
            }

            return result;
        }

        private string ProcessSUB(string firstParam, string secondParam)
        {
            string appendSymbol = (firstParam.Contains("$") || secondParam.Contains("$")) ? "$" : string.Empty;
            appendSymbol = (firstParam.Contains("%") || secondParam.Contains("%")) ? "%" : string.Empty;

            firstParam = firstParam.Replace("{{", "").Replace("}}", "").Replace("%", "").Replace("$", "").Replace(",", "");
            secondParam = secondParam.Replace("{{", "").Replace("}}", "").Replace("%", "").Replace("$", "").Replace(",", "");
            string result = string.Empty;
            if (!string.IsNullOrWhiteSpace(firstParam) && !string.IsNullOrWhiteSpace(secondParam))
            {
                int val1; int val2;
                if (Int32.TryParse(firstParam, out val1) && Int32.TryParse(secondParam, out val2))
                {
                    result = Convert.ToString(val1 - val2);
                }
            }
            result = appendSymbol == "$" ? appendSymbol + result : (appendSymbol == "%" ? result + appendSymbol : result);

            return result;
        }
    }
}
