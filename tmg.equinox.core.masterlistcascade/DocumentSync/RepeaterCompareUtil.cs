using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace tmg.equinox.mlcascade.documentcomparer.RepeaterCompareUtils
{
    public class RepeaterMatchUtil
    {

        public static List<JToken> GetMatchesInRepeater(JToken repeaterData, JToken keysMacro, string matchType)
        {
            List<JToken> items = ((JArray)repeaterData).ToList();
            List<JToken> matches = new List<JToken>();
            List<string> keyValues = new List<string>();
            if (items != null && items.Count > 0) 
            {
                //generate keys row
                List<JToken> keys = keysMacro.Values().ToList();
                bool hasKeysToCompare = false;
                string compareExpressionTemplate = "$[?({0})]";
                string compareExpression = "";
                foreach (JToken key in keys) 
                {
                    string keyValue = key["Key"].ToString();
                    string matchValue = "";
                    if (matchType == "Source") 
                    {
                        matchValue = key["SourceValue"].ToString();
                    }
                    else 
                    {
                        matchValue = key["TargetValue"].ToString();
                    }
                    if (!String.IsNullOrEmpty(matchValue)) 
                    {
                        if(compareExpression != "")
                        {
                            compareExpression = compareExpression + " && ";
                        }
                        compareExpression = compareExpression + "@." + keyValue + " == '" + matchValue + "'";
                        keyValues.Add(keyValue);
                        hasKeysToCompare = true;
                    }
                }
                if (hasKeysToCompare) 
                {
                    compareExpression = String.Format(compareExpressionTemplate, compareExpression);
                    var rowMatches = repeaterData.SelectTokens(compareExpression);
                    if (rowMatches != null && rowMatches.Count() > 0)
                    {
                        matches = rowMatches.ToList();
                    }
                }
                else 
                {
                    if (repeaterData.Children().Count() > 0) 
                    {
                        matches = repeaterData.Children().ToList();
                    }
                }
            }
            return matches;
        }

        public static JToken GetMatchingRowFromTargetRepeater(JToken sourceRepeaterRow, List<JToken> targetRepeaterData, JToken keysMacro,string matchType)
        {
            JToken match = null;
            List<JToken> sourceRepeaterRowContainer = new List<JToken>();
            JToken sourceRepeaterRowClone = sourceRepeaterRow.DeepClone();
            sourceRepeaterRowContainer.Add(sourceRepeaterRowClone);
            List<string> keyValues = new List<string>();
            if (targetRepeaterData != null && targetRepeaterData.Count > 0)
            {
                //generate keys row
                List<JToken> keys = keysMacro.Values().ToList();
                bool hasKeysToCompare = false;
                foreach (JToken key in keys)
                {
                    string keyValue = key["Key"].ToString();
                    keyValues.Add(keyValue);
                    string sourceValue = "";
                    string targetValue = "";
                    if (matchType == "Source")
                    {
                        sourceValue = key["SourceValue"].ToString();
                        targetValue = key["TargetValue"].ToString();
                    }
                    else 
                    {
                        sourceValue = key["TargetValue"].ToString();
                        targetValue = key["SourceValue"].ToString();
                    }
 
                    //need to get matching row from Target
                    if (sourceValue != targetValue && sourceRepeaterRowClone[keyValue] != null) 
                    {
                        sourceRepeaterRowClone[keyValue] = targetValue;
                    }
                    hasKeysToCompare = true;
                }
                if (hasKeysToCompare)
                {
                    var primaryRowMatches = targetRepeaterData.Intersect(sourceRepeaterRowContainer, new RepeaterEqualityComparer(keyValues));
                    if (primaryRowMatches != null && primaryRowMatches.Count() > 0)
                    {
                        match = primaryRowMatches.First();
                    }
                }
            }
            return match;
        }

        public static JToken GetMatchingRowInRepeater(JToken repeaterData, List<RepeaterCompareKey> keys, string matchType)
        {
            List<JToken> items = ((JArray)repeaterData).ToList();
            JToken match = null;
            List<string> keyValues = new List<string>();
            if (items != null && items.Count > 0)
            {
                //generate keys row
                bool hasKeysToCompare = false;
                string compareExpressionTemplate = "$[?({0})]";
                string compareExpression = "";
                foreach (RepeaterCompareKey key in keys)
                {
                    string keyValue = key.KeyName;
                    string matchValue = "";
                    if (matchType == "Source")
                    {
                        matchValue = key.SourceKey;
                    }
                    else
                    {
                        matchValue = key.TargetKey;
                    }
                    if (!String.IsNullOrEmpty(matchValue))
                    {
                        if (compareExpression != "")
                        {
                            compareExpression = compareExpression + " && ";
                        }
                        compareExpression = compareExpression + "@." + keyValue + " == '" + matchValue + "'";
                        keyValues.Add(keyValue);
                        hasKeysToCompare = true;
                    }
                }
                if (hasKeysToCompare)
                {
                    compareExpression = String.Format(compareExpressionTemplate, compareExpression);
                    var rowMatches = repeaterData.SelectTokens(compareExpression);
                    if (rowMatches != null && rowMatches.Count() > 0)
                    {
                        match = rowMatches.First();
                    }
                }
            }
            return match;
        }

    }

    public static class StringExtensions 
    {
        public static string ToAlphaNumeric(this string self, params char[] allowedCharacters)
        {
            return new string(Array.FindAll(self.ToCharArray(), c => char.IsLetterOrDigit(c) || allowedCharacters.Contains(c)));
        }
    }
}