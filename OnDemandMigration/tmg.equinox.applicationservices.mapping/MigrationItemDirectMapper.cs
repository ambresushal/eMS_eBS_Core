using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.pbp
{
    public class MigrationItemDirectMapper : IMigrationItemMapper
    {
        public void MapItem(ref JObject source, ref JObject target, MigrationFieldItem item, MigrationPlanItem plan)
        {
            string sourcePath = item.TableName + "." + item.ColumnName.Replace(" ", "");
            var sourceToken = source.SelectToken(sourcePath);

            if (item.ViewType != "SOT")
            {
                string targetPath = item.DocumentPath;
                var targetToken = target.SelectToken(targetPath);
                var prop = targetToken.Parent as JProperty;
                prop.Value = sourceToken != null ? sourceToken.ToString().Trim() : "";

                string logMessage = item.TableName + "," + item.ColumnName + "," + item.DocumentPath + "," + prop.Value.ToString().Replace(",", ";");
                logMessage = Regex.Replace(logMessage, @"\t|\n|\r", "");
                DMUtilityLog.WriteDMUtilityLog(plan.QID, item.ViewType, logMessage);
            }
            else
            {
                if (item.SOTDocumentPath != null)
                {
                    string targetPath = item.SOTDocumentPath;
                    var targetToken = target.SelectToken(targetPath);
                    if (targetToken != null)
                    {
                        var prop = targetToken.Parent as JProperty;
                        var propValue = sourceToken != null ? sourceToken.ToString().Trim() : "";
                        if (item.Dictionaryitems.Count > 0)
                        {
                            var CodeDescription = from dic in item.Dictionaryitems where dic.Codes.Equals(propValue) select dic.CODE_VALUES;
                            if (CodeDescription != null && CodeDescription.Count() > 0)
                                propValue = CodeDescription.First().ToString().Trim();
                        }
                        if (item.SOTPrefix != null) propValue = propValue != "" ? (item.SOTPrefix + propValue) : propValue;
                        if (item.SOTSuffix != null) propValue = propValue != "" ? (propValue + item.SOTSuffix) : propValue;
                        if (propValue == "" && item.IfBlankThenValue != null) propValue = item.IfBlankThenValue;
                        if (item.IsYesNoField) propValue = propValue == "1" ? "YES" : "NO";
                        if (item.IsCheckBothFields)
                        {
                            if (prop.Value.ToString() == "Not Applicable" || prop.Value.ToString() == "")
                            {
                                prop.Value = propValue;
                                if (item.SetSimilarValues != null)
                                    UpdateSimilarValuesToOtherElement(ref target, item.SetSimilarValues, propValue, item, plan);

                                string logMessage = item.TableName + "," + item.ColumnName + "," + item.SOTDocumentPath + "," + prop.Value.ToString().Replace(",", ";");
                                logMessage = Regex.Replace(logMessage, @"\t|\n|\r", "");
                                DMUtilityLog.WriteDMUtilityLog(plan.QID, item.ViewType, logMessage);
                            }
                            else
                            {
                                if (propValue != "Not Applicable" && propValue != "")
                                {
                                    prop.Value = propValue;
                                    if (item.SetSimilarValues != null)
                                        UpdateSimilarValuesToOtherElement(ref target, item.SetSimilarValues, propValue, item, plan);

                                    string logMessage = item.TableName + "," + item.ColumnName + "," + item.SOTDocumentPath + "," + prop.Value.ToString().Replace(",", ";");
                                    logMessage = Regex.Replace(logMessage, @"\t|\n|\r", "");
                                    DMUtilityLog.WriteDMUtilityLog(plan.QID, item.ViewType, logMessage);
                                }
                            }
                        }
                        else
                        {
                            prop.Value = propValue;
                            if (item.SetSimilarValues != null)
                                UpdateSimilarValuesToOtherElement(ref target, item.SetSimilarValues, propValue, item, plan);

                            string logMessage = item.TableName + "," + item.ColumnName + "," + item.SOTDocumentPath + "," + prop.Value.ToString().Replace(",", ";");
                            logMessage = Regex.Replace(logMessage, @"\t|\n|\r", "");
                            DMUtilityLog.WriteDMUtilityLog(plan.QID, item.ViewType, logMessage);
                        }
                    }
                }
            }
        }

        private void UpdateSimilarValuesToOtherElement(ref JObject target, string otherPaths, string value, MigrationFieldItem item, MigrationPlanItem plan)
        {
            string[] documentPaths = otherPaths.Split(',');
            foreach (string documentPath in documentPaths)
            {
                if (documentPath != "" || documentPath != null)
                {
                    string targetPath = documentPath;
                    var targetToken = target.SelectToken(targetPath);
                    var prop = targetToken.Parent as JProperty;
                    prop.Value = value;

                    string logMessage = item.TableName + "," + item.ColumnName + "," + item.SOTDocumentPath + "," + prop.Value.ToString().Replace(",", ";");
                    logMessage = Regex.Replace(logMessage, @"\t|\n|\r", "");
                    DMUtilityLog.WriteDMUtilityLog(plan.QID, item.ViewType, logMessage);
                }
            }
        }
    }
}
