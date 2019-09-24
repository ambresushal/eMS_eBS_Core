using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.web.ODMExecuteManager.Interface;
using tmg.equinox.web.ODMExecuteManager.Model;

namespace tmg.equinox.web.ODMExecuteManager
{
    public class MigrationItemDirectMapper : IMigrationItemMapper
    {
        public void MapItem(ref JObject source, ref JObject target, MigrationFieldItem item)
        {
            string sourcePath = item.TableName + "." + item.ColumnName.Replace(" ", "");
            var sourceToken = source.SelectToken(sourcePath);

            if (item.ViewType != "SOT")
            {
                string targetPath = item.DocumentPath;
                var targetToken = target.SelectToken(targetPath);
                var prop = targetToken.Parent as JProperty;
                prop.Value = sourceToken != null ? sourceToken.ToString().Trim() : "";

                if (item.ColumnName == "mrx_ltc_attest_flag" && sourceToken.ToString().Trim() == "1")
                    prop.Value = "true";
            }
            else
            {

                if (item.SOTDocumentPath != null)
                {
                    string targetPath = item.SOTDocumentPath;
                    var targetToken = target.SelectToken(targetPath);
                    if (targetToken != null)
                    {
                        var propValue = "";
                        var prop = targetToken.Parent as JProperty;
                        if (item.ColumnName.Contains("_notes"))
                        {
                            propValue = sourceToken != null ? sourceToken.ToString() : "";
                        }
                        else
                        {
                            propValue = sourceToken != null ? sourceToken.ToString().Trim() : "";
                        }

                        if (item.Dictionaryitems.Count > 0 && !item.IsYesNoField)
                        {
                            var CodeDescription = from dic in item.Dictionaryitems where dic.Codes.Equals(propValue) select dic.CODE_VALUES;
                            if (CodeDescription != null && CodeDescription.Count() > 0)
                                propValue = CodeDescription.First().ToString().Trim();
                        }
                        if (item.SOTPrefix != null) propValue = propValue != "" ? (item.SOTPrefix + propValue) : propValue;
                        if (item.SOTSuffix != null) propValue = propValue != "" ? (propValue + item.SOTSuffix) : propValue;
                        if (propValue == "" && item.IfBlankThenValue != null) propValue = item.IfBlankThenValue;
                        if (item.IsYesNoField)
                        {
                            if (propValue == "1")
                                propValue = "YES";
                            else if (propValue == "2")
                                propValue = "NO";
                            else
                                propValue = string.Empty;

                            if (item.ColumnName == "mrx_ltc_attest_flag" && propValue == "YES")
                                propValue = "true";
                        }
                        if (item.IsCheckBothFields)
                        {
                            if (prop.Value.ToString() == "Not Applicable" || prop.Value.ToString() == "")
                            {
                                prop.Value = propValue;
                                if (item.SetSimilarValues != null)
                                    UpdateSimilarValuesToOtherElement(ref target, item.SetSimilarValues, propValue);
                            }
                            else
                            {
                                if (propValue != "Not Applicable" && propValue != "")
                                {
                                    prop.Value = propValue;
                                    if (item.SetSimilarValues != null)
                                        UpdateSimilarValuesToOtherElement(ref target, item.SetSimilarValues, propValue);
                                }
                            }
                        }
                        else
                        {
                            prop.Value = propValue;
                            if (item.SetSimilarValues != null)
                                UpdateSimilarValuesToOtherElement(ref target, item.SetSimilarValues, propValue);
                        }
                    }
                }
            }
        }

        private void UpdateSimilarValuesToOtherElement(ref JObject target, string otherPaths, string value)
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
                }
            }
        }
    }

}
