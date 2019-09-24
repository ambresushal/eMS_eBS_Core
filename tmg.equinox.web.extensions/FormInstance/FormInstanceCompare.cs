using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using tmg.equinox.applicationservices.viewmodels.Reporting;

namespace tmg.equinox.web.FormInstance
{
    public class FormInstanceComparer
    {
        IEnumerable<ReportingViewModel> sourceDataSourceNameList = null;
        IEnumerable<ReportingViewModel> targetDataSourceNameList = null;
        IEnumerable<ReportingViewModel> sourceUIElementList = null;
        IEnumerable<ReportingViewModel> targetUIElementList = null;
        public CompareResult ProcessCompare(IDictionary<string, object> sourceObj, IDictionary<string, object> targetObj, IEnumerable<ReportingViewModel> sourceDataList, IEnumerable<ReportingViewModel> targetDataList, IEnumerable<ReportingViewModel> sourceUIElementDataList, IEnumerable<ReportingViewModel> targetUIElementdataList)
        {
            CompareResult result = new CompareResult();
            result.compareList = new List<CompareFieldResult>();
            sourceDataSourceNameList = sourceDataList;
            targetDataSourceNameList = targetDataList;
            sourceUIElementList = sourceUIElementDataList;
            targetUIElementList = targetUIElementdataList;
            int srcCount = sourceObj.Keys.Count();
            int tarCount = targetObj.Keys.Count();
            //if source has new section
            List<string> sourceExceptList = sourceObj.Keys.Except(targetObj.Keys).ToList();
            //if target has new section
            List<string> targetExceptList = targetObj.Keys.Except(sourceObj.Keys).ToList();

            List<string> intersectList = sourceObj.Keys.Intersect(targetObj.Keys).ToList();

            if (intersectList.Count() > 0)
            {
                foreach (string key in intersectList)
                {
                    IList<ReportingViewModel> checkVisibleElement = IsElementVisible(Convert.ToString(key), sourceUIElementList, targetUIElementList);
                    if (checkVisibleElement[0].Visable == true)
                    {
                        var elementName = checkVisibleElement[0].LabelName;
                        //GetUIElementName(Convert.ToString(key), sourceUIElementList, targetUIElementList);
                        result.compareList.Add(new CompareFieldResult() { SectionName = Convert.ToString(elementName) });
                        int totalRows = result.compareList.Count();
                        CompareFieldList(sourceObj[key], targetObj[key], result.compareList);
                        if (result.compareList.Count() == totalRows)
                        {
                            result.compareList.Add(new CompareFieldResult() { SourceValue = ChangeSummaryConstants.NoChanges, TargetValue = ChangeSummaryConstants.NoChanges });
                        }
                    }
                }
            }
            if (sourceExceptList.Count() > 0)
            {
                foreach (string key in sourceExceptList)
                {
                    IList<ReportingViewModel> checkVisibleElement = IsElementVisible(Convert.ToString(key), sourceUIElementList, targetUIElementList);
                    if (checkVisibleElement[0].Visable == true)
                    {
                        var elementName = checkVisibleElement[0].LabelName;
                        result.compareList.Add(new CompareFieldResult() { SectionName = Convert.ToString(elementName) });
                        int totalRows = result.compareList.Count();
                        CompareFieldList(sourceObj[key], "target", result.compareList);
                        if (result.compareList.Count() == totalRows)
                        {
                            result.compareList.Add(new CompareFieldResult() { SourceValue = ChangeSummaryConstants.NoChanges, TargetValue = ChangeSummaryConstants.NoChanges });
                        }
                    }
                }
            }
            if (targetExceptList.Count() > 0)
            {
                foreach (string key in targetExceptList)
                {
                    IList<ReportingViewModel> checkVisibleElement = IsElementVisible(Convert.ToString(key), sourceUIElementList, targetUIElementList);
                    if (checkVisibleElement[0].Visable == true)
                    {
                        var elementName = checkVisibleElement[0].LabelName;
                        result.compareList.Add(new CompareFieldResult() { SectionName = Convert.ToString(elementName) });
                        int totalRows = result.compareList.Count();
                        CompareFieldList("source", targetObj[key], result.compareList);
                        if (result.compareList.Count() == totalRows)
                        {
                            result.compareList.Add(new CompareFieldResult() { SourceValue = ChangeSummaryConstants.NoChanges, TargetValue = ChangeSummaryConstants.NoChanges });
                        }
                    }
                }
            }
            return result;
        }

        private List<CompareFieldResult> CompareFieldList(dynamic source, dynamic target, List<CompareFieldResult> compareList)
        {
            if (source is ExpandoObject && target is ExpandoObject)
            {
                var src = source as IDictionary<string, object>;
                var tar = target as IDictionary<string, object>;

                int srcCount = src.Keys.Count();
                int tarCount = tar.Keys.Count();
                //if source has new key
                List<string> sourceExceptList = src.Keys.Except(tar.Keys).ToList();
                //if target has new key
                List<string> targetExceptList = tar.Keys.Except(src.Keys).ToList();
                //to find diffreant field
                List<string> intersectList = src.Keys.Intersect(tar.Keys).ToList();

                if (intersectList.Count() > 0)
                {
                    foreach (string key in intersectList)
                    {
                        IList<ReportingViewModel> checkVisibleElement = IsElementVisible(Convert.ToString(key), sourceUIElementList, targetUIElementList);
                        if (checkVisibleElement[0].Visable == true)
                        {
                            var elementName = checkVisibleElement[0].LabelName;
                            var sourceValue = src[key];
                            var targetValue = tar[key];
                            if (sourceValue is ExpandoObject && targetValue is ExpandoObject)
                            {
                                compareList.Add(new CompareFieldResult() { SubSectionName = Convert.ToString(elementName) });
                                int totalRows = compareList.Count();
                                //for child loop
                                CompareFieldList(sourceValue, targetValue, compareList);
                                if (totalRows == compareList.Count())
                                {
                                    compareList.Add(new CompareFieldResult() { FieldType = "", SourceValue = ChangeSummaryConstants.NoChanges, TargetValue = ChangeSummaryConstants.NoChanges });
                                }
                            }
                            else if (targetValue is System.Collections.Generic.IList<Object> && sourceValue is System.Collections.Generic.IList<Object>)
                            {
                                bool dataSourceName = GetListName(Convert.ToString(key), sourceDataSourceNameList, targetDataSourceNameList);
                                if (!dataSourceName)
                                {
                                    compareList.Add(new CompareFieldResult() { FieldType = Convert.ToString(elementName) });
                                }
                                int listRowNo = compareList.Count();
                                var srList = sourceValue as System.Collections.Generic.IList<Object>;
                                var trList = targetValue as System.Collections.Generic.IList<Object>;
                                int srCount = srList.Count();
                                int trCount = trList.Count();
                                if (srCount > trCount)
                                {
                                    int i = 0;
                                    int rowNo = 1;
                                    foreach (var sourceList in srList)
                                    { //find list in json object
                                        if (i < trCount)
                                        {
                                            compareList.Add(new CompareFieldResult() { FieldType = "Row Id:" + rowNo });
                                            int totalRows = compareList.Count;
                                            string jsonList = JsonConvert.SerializeObject(trList[i]);
                                            var converter = new ExpandoObjectConverter();
                                            ExpandoObject targetList = JsonConvert.DeserializeObject<ExpandoObject>(jsonList, converter);
                                            CompareFieldList(sourceList, targetList, compareList);
                                            if (compareList.Count == totalRows)
                                            {
                                                compareList.RemoveAt((totalRows - 1));
                                            }
                                            rowNo++;
                                        }
                                        else
                                        {
                                            compareList.Add(new CompareFieldResult() { FieldType = "Row Id:" + rowNo });
                                            int totalRows = compareList.Count;
                                            CompareFieldList(sourceList, "target", compareList);
                                            if (compareList.Count == totalRows)
                                            {
                                                compareList.RemoveAt((totalRows - 1));
                                            }
                                            rowNo++;
                                        }
                                        i++;
                                    }
                                }
                                else
                                {
                                    int i = 0;
                                    int rowNo = 1;
                                    foreach (var targetList in trList)
                                    { //find list in json object
                                        if (i < srList.Count)
                                        {
                                            compareList.Add(new CompareFieldResult() { FieldType = "Row Id:" + rowNo });
                                            int totalRows = compareList.Count;
                                            string jsonList = JsonConvert.SerializeObject(srList[i]);
                                            var converter = new ExpandoObjectConverter();
                                            ExpandoObject sourceList = JsonConvert.DeserializeObject<ExpandoObject>(jsonList, converter);
                                            CompareFieldList(sourceList, targetList, compareList);
                                            if (compareList.Count == totalRows)
                                            {
                                                compareList.RemoveAt((totalRows - 1));
                                            }
                                            rowNo++;
                                        }
                                        else
                                        {
                                            compareList.Add(new CompareFieldResult() { FieldType = "Row Id:" + rowNo });
                                            int totalRows = compareList.Count;
                                            CompareFieldList("source", targetList, compareList);
                                            if (compareList.Count == totalRows)
                                            {
                                                compareList.RemoveAt((totalRows - 1));
                                            }
                                            rowNo++;
                                        }
                                        i++;
                                    }
                                }
                                if (compareList.Count() == listRowNo)
                                {
                                    if (compareList[compareList.Count() - 1].FieldType != null)
                                    {
                                        int lenght = compareList[compareList.Count() - 1].FieldType.Length;

                                        if ((lenght > 3 && compareList[compareList.Count() - 1].FieldType.Substring(0, 3) != "Row") || lenght <= 3)
                                        {
                                            if (compareList[compareList.Count() - 1].SourceValue == null && compareList[compareList.Count() - 1].TargetValue == null)
                                                compareList.Add(new CompareFieldResult() { SourceValue = ChangeSummaryConstants.NoChanges, TargetValue = ChangeSummaryConstants.NoChanges });
                                        }
                                    }
                                }
                            }
                            else if (Convert.ToString(sourceValue).ToUpper() != Convert.ToString(targetValue).ToUpper() && Convert.ToString(key).ToUpper() != ("RowIdProperty").ToUpper())
                            {
                                if (String.IsNullOrEmpty(Convert.ToString(sourceValue)))
                                    sourceValue = "<blank>";
                                if (String.IsNullOrEmpty(Convert.ToString(targetValue)))
                                    targetValue = "<blank>";
                                applyCustomUIElement(compareList, Convert.ToString(elementName), Convert.ToString(sourceValue), Convert.ToString(targetValue));
                            }
                        }
                    }
                }
                //new source section or new field
                if (sourceExceptList.Count() > 0)
                {//set as deleted field

                    foreach (string key in sourceExceptList)
                    {
                        IList<ReportingViewModel> checkVisibleElement = IsElementVisible(Convert.ToString(key), sourceUIElementList, targetUIElementList);
                        if (checkVisibleElement[0].Visable == true)
                        {
                            var elementName = checkVisibleElement[0].LabelName;
                            var sourceValue = src[key];
                            if (sourceValue is ExpandoObject)
                            {
                                compareList.Add(new CompareFieldResult() { SubSectionName = Convert.ToString(elementName) });
                                int totalRows = compareList.Count();
                                //new section has added.
                                CompareFieldList(src[key], "target", compareList);
                                if (totalRows == compareList.Count())
                                {
                                    compareList.Add(new CompareFieldResult() { SourceValue = ChangeSummaryConstants.NoChanges, TargetValue = ChangeSummaryConstants.NoChanges });
                                }
                            }
                            else if (sourceValue is System.Collections.Generic.IList<Object>)
                            {
                                bool dataSourceName = GetListName(Convert.ToString(key), sourceDataSourceNameList, targetDataSourceNameList);
                                if (!dataSourceName)
                                {
                                    compareList.Add(new CompareFieldResult() { FieldType = Convert.ToString(elementName) });
                                }
                                var srList = sourceValue as System.Collections.Generic.IList<Object>;
                                int rowNo = 1;
                                foreach (var s in srList)
                                {
                                    compareList.Add(new CompareFieldResult() { FieldType = "Row Id:" + rowNo });
                                    CompareFieldList(s, "target", compareList);
                                    rowNo++;
                                }
                            }
                            else
                            {//for deleted field
                                if (Convert.ToString(key).ToUpper() != ("RowIdProperty").ToUpper())
                                {
                                    if (String.IsNullOrEmpty(Convert.ToString(sourceValue)))
                                        sourceValue = "<blank>";
                                    applyCustomUIElement(compareList, Convert.ToString(elementName), Convert.ToString(sourceValue), ChangeSummaryConstants.NotApplicable);
                                }
                            }
                        }
                    }
                }

                //new source section or new field
                if (targetExceptList.Count() > 0)
                {// set as new field

                    foreach (string key in targetExceptList)
                    {
                        IList<ReportingViewModel> checkVisibleElement = IsElementVisible(Convert.ToString(key), sourceUIElementList, targetUIElementList);
                        if (checkVisibleElement[0].Visable == true)
                        {
                            var elementName = checkVisibleElement[0].LabelName;
                            var targetValue = tar[key];
                            if (targetValue is ExpandoObject)
                            {
                                compareList.Add(new CompareFieldResult() { SubSectionName = Convert.ToString(key) });
                                int totalRows = compareList.Count();
                                CompareFieldList("source", tar[key], compareList);
                                if (totalRows == compareList.Count())
                                {
                                    compareList.Add(new CompareFieldResult() { SourceValue = ChangeSummaryConstants.NoChanges, TargetValue = ChangeSummaryConstants.NoChanges });
                                }
                            }
                            else if (targetValue is System.Collections.Generic.IList<Object>)
                            {
                                bool dataSourceName = GetListName(Convert.ToString(key), sourceDataSourceNameList, targetDataSourceNameList);
                                if (!dataSourceName)
                                {
                                    compareList.Add(new CompareFieldResult() { FieldType = Convert.ToString(key) });
                                }
                                var trList = targetValue as System.Collections.Generic.IList<Object>;
                                int rowNo = 1;
                                foreach (var t in trList)
                                {
                                    compareList.Add(new CompareFieldResult() { FieldType = "Row Id:" + rowNo });
                                    CompareFieldList("source", t, compareList);
                                    rowNo++;
                                }
                            }
                            else
                            {//for New filed
                                if (Convert.ToString(key).ToUpper() != ("RowIdProperty").ToUpper())
                                {
                                    if (String.IsNullOrWhiteSpace(Convert.ToString(targetValue)))
                                        targetValue = "<blank>";
                                    applyCustomUIElement(compareList, Convert.ToString(elementName), ChangeSummaryConstants.NotApplicable, Convert.ToString(targetValue));
                                }
                            }
                        }
                    }
                }
            }
            else if (target is ExpandoObject && source == "source")
            {
                var targetValue = target as IDictionary<string, object>;
                foreach (var targetKey in new List<string>(targetValue.Keys))
                {
                    IList<ReportingViewModel> checkVisibleElement = IsElementVisible(Convert.ToString(targetKey), sourceUIElementList, targetUIElementList);
                    if (checkVisibleElement[0].Visable == true)
                    {
                        var elementName = checkVisibleElement[0].LabelName;
                        var tValue = targetValue[targetKey];
                        if (tValue is ExpandoObject)
                        {
                            compareList.Add(new CompareFieldResult() { SubSectionName = Convert.ToString(elementName) });
                            CompareFieldList("source", targetValue[targetKey], compareList);
                        }
                        else if (tValue is System.Collections.Generic.IList<Object>)
                        {
                            int rowNo = 1;
                            var trList = tValue as System.Collections.Generic.IList<Object>;
                            foreach (var t in trList)
                            {
                                compareList.Add(new CompareFieldResult() { FieldType = "Row Id:" + rowNo });
                                CompareFieldList("source", t, compareList);
                                rowNo++;
                            }
                        }
                        else
                        {//for New filed
                            if (Convert.ToString(targetKey).ToUpper() != ("RowIdProperty").ToUpper())
                            {
                                if (String.IsNullOrEmpty(Convert.ToString(targetValue[targetKey])))
                                    targetValue[targetKey] = "<blank>";
                                applyCustomUIElement(compareList, Convert.ToString(elementName), ChangeSummaryConstants.NotApplicable, Convert.ToString(targetValue[targetKey]));
                            }
                        }
                    }
                }
            }
            else if (source is ExpandoObject && target == "target")
            {
                var sourceValue = source as IDictionary<string, object>;
                foreach (var sourceKey in new List<string>(sourceValue.Keys))
                {
                    IList<ReportingViewModel> checkVisibleElement = IsElementVisible(Convert.ToString(sourceKey), sourceUIElementList, targetUIElementList);
                    if (checkVisibleElement[0].Visable == true)
                    {
                        var elementName = checkVisibleElement[0].LabelName;
                        var sValue = sourceValue[sourceKey];
                        if (sValue is ExpandoObject)
                        {
                            compareList.Add(new CompareFieldResult() { SubSectionName = Convert.ToString(elementName) });
                            CompareFieldList(sValue, "target", compareList);
                        }
                        else if (sValue is System.Collections.Generic.IList<Object>)
                        {
                            var srList = sValue as System.Collections.Generic.IList<Object>;
                            int rowNo = 1;
                            foreach (var s in srList)
                            {
                                compareList.Add(new CompareFieldResult() { FieldType = "Row Id:" + rowNo });
                                CompareFieldList(s, "target", compareList);
                                rowNo++;
                            }
                        }
                        else
                        {//for deleted field
                            if (Convert.ToString(sourceKey).ToUpper() != ("RowIdProperty").ToUpper())
                            {
                                if (String.IsNullOrEmpty(Convert.ToString(sourceValue[sourceKey])))
                                    sourceValue[sourceKey] = "<blank>";
                                applyCustomUIElement(compareList, Convert.ToString(elementName), Convert.ToString(sourceValue[sourceKey]), ChangeSummaryConstants.NotApplicable);
                            }
                        }
                    }
                }
            }
            return compareList;
        }

        private void applyCustomUIElement(List<CompareFieldResult> compareList, string fieldName, string sourceValue, string targetValue)
        {
            if ((string.IsNullOrEmpty(sourceValue) && !string.IsNullOrEmpty(targetValue)) || (!string.IsNullOrEmpty(sourceValue) && string.IsNullOrEmpty(targetValue)))
            {
                if (string.IsNullOrWhiteSpace(sourceValue))
                    sourceValue = "<blank>";
                else
                    targetValue = "<blank>";
            }
            var sourceUIElementProperties = sourceUIElementList
                                        .Where(x => x.UILabelName.Equals(fieldName)
                                            && x.RadioOptionLabelYes != null && x.RadioOptionLabelNo != null
                                        ).ToList();

            var targetUIElementProperties = targetUIElementList
                            .Where(x => x.UILabelName.Equals(fieldName)
                             && x.RadioOptionLabelYes != null && x.RadioOptionLabelNo != null
                            ).ToList();

            var UIElementProperties = sourceUIElementProperties;

            if (sourceUIElementProperties.Count() == 0)
                UIElementProperties = targetUIElementProperties;

            if (targetUIElementProperties.Count() == 0)
                UIElementProperties = sourceUIElementProperties;

            if (UIElementProperties.Count() > 0)
            {
                if (sourceValue.ToLower() == "true")
                {
                    sourceValue = UIElementProperties[0].RadioOptionLabelYes;
                }
                if (sourceValue.ToLower() == "false")
                {
                    sourceValue = UIElementProperties[0].RadioOptionLabelNo;
                }
                if (targetValue.ToLower() == "false")
                {
                    targetValue = UIElementProperties[0].RadioOptionLabelNo;
                }
                if (targetValue.ToLower() == "true")
                {
                    targetValue = UIElementProperties[0].RadioOptionLabelYes;
                }
            }

            compareList.Add(new CompareFieldResult() { FieldType = Convert.ToString(fieldName), SourceValue = sourceValue, TargetValue = targetValue });
        }

        private bool GetListName(string listName, IEnumerable<ReportingViewModel> sourceDataSourceNameList, IEnumerable<ReportingViewModel> targetDataSourceNameList)
        {
            if (sourceDataSourceNameList != null && targetDataSourceNameList != null)
            {
                var sourceDataSourceName = sourceDataSourceNameList.Where(x => x.DataSourceName.Equals(listName)).ToList();
                var targetDataSourceName = targetDataSourceNameList.Where(x => x.DataSourceName.Equals(listName)).ToList();
                if (sourceDataSourceName.Count() == 0 && targetDataSourceName.Count() == 0)
                    return false;
                else
                    return true;
            }
            else
                return false;
        }

        private IList<ReportingViewModel> IsElementVisible(string uiElementName, IEnumerable<ReportingViewModel> sourceUIElementList, IEnumerable<ReportingViewModel> targetUIElementList)
        {
            List<ReportingViewModel> UIElementInfo = new List<ReportingViewModel>();
            var sourceUIElement = sourceUIElementList.Where(x => x.GeneratedName.Equals(uiElementName) && x.Visable != false).ToList();
            var targetUIElement = targetUIElementList.Where(x => x.GeneratedName.Equals(uiElementName) && x.Visable != false).ToList();
            if (sourceUIElement.Count() > 0)
            {
                UIElementInfo.Add(new ReportingViewModel { LabelName = sourceUIElement[0].UILabelName, Visable = sourceUIElement[0].Visable });
            }
            else if (targetUIElement.Count() > 0)
            {
                UIElementInfo.Add(new ReportingViewModel { LabelName = targetUIElement[0].UILabelName, Visable = targetUIElement[0].Visable });
            }
            else
            {
                UIElementInfo.Add(new ReportingViewModel { LabelName = null, Visable = false });
            }
            return UIElementInfo;
        }
    }
}