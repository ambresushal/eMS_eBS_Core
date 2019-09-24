using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.mlcascade.documentcomparer
{
    public class SectionComparer
    {

        private JToken fieldsMacro;
        private JToken sourceSection;
        private JToken targetSection;
        private string rootSection;
        private List<string> parentSections;
        private string matchType;
        CompareDocumentSource compareSource;

        public SectionComparer(JToken fieldsMacro, JToken sourceSection, JToken targetSection, Dictionary<string, string> labelPathMap, string matchType, CompareDocumentSource compareSource) 
        {

            this.fieldsMacro = fieldsMacro;
            this.sourceSection = sourceSection;
            this.targetSection = targetSection;
            this.matchType = matchType;
            this.compareSource = compareSource;
            string path = fieldsMacro.Path;
            string[] parts = path.Split('.');
            this.rootSection = labelPathMap[parts[0]];
            var index = 0;
            parentSections = new List<string>();
            string parentPath = parts[0];
            foreach (string s in parts) 
            {
                if(index > 0 && index < parts.Length -1)
                {
                    parentPath = parentPath + "." + s;
                    this.parentSections.Add(labelPathMap[parentPath]);
                }
                index++;
            }
        }

        public SectionCompareResult Compare() 
        {
            SectionCompareResult result = new SectionCompareResult();
            result.ParentSections = this.parentSections;
            result.RootSectionName = this.rootSection;
            result.Path = this.fieldsMacro.Parent.Path;
            result.Fields = new List<SectionCompareField>();
            List<JToken> fields= fieldsMacro.Values().ToList();
            if (sourceSection != null && targetSection != null) 
            {
                foreach (JToken field in fields)
                {
                    SectionCompareField compareField = new SectionCompareField();
                    string fieldName = field["Field"].ToString();
                    compareField.FieldName = fieldName;
                    string sourceValue = DocumentJSONReaderHelper.GetSectionFieldValue(sourceSection, fieldName,compareSource);
                    string targetValue = DocumentJSONReaderHelper.GetSectionFieldValue(targetSection, fieldName,compareSource);
                    compareField.SourceValue = sourceValue;
                    compareField.TargetValue = targetValue;
                    if (sourceValue != null)
                    {
                        compareField.SourceValue = sourceValue;
                    }
                    else
                    {
                        compareField.IsMissingInSource = true;
                        compareField.SourceValue = "Missing";
                    }
                    if (targetValue != null)
                    {
                        compareField.TargetValue = targetValue;
                    }
                    else
                    {
                        compareField.IsMissingInTarget = true;
                        compareField.TargetValue = "Missing";
                    }
                    if (!compareField.IsMissingInSource && !compareField.IsMissingInTarget)
                    {
                        if (compareField.SourceValue == compareField.TargetValue)
                        {
                            compareField.IsMatch = true;
                        }
                    }
                    switch (matchType) 
                    {
                        case "Mismatches only":
                            if (compareField.IsMatch == false) 
                            {
                                result.Fields.Add(compareField);
                            }
                            break;
                        case "Matches only":
                            if (compareField.IsMatch == true)
                            {
                                result.Fields.Add(compareField);
                            }
                            break;
                        default:
                            result.Fields.Add(compareField);
                            break;
                    }
                }
            }
            else 
            {
                if (sourceSection == null) 
                {
                    result.IsSectionMissingInSource = true;
                }
                if (targetSection == null)
                {
                    result.IsSectionMissingInTarget = true;
                }
            }
            SetMatchForSection(result);
            return result;
        }

        private void SetMatchForSection(SectionCompareResult result) 
        {
            if (result.Fields != null && result.Fields.Count > 0) 
            {
                result.IsMatch = true;
                foreach (SectionCompareField field in result.Fields) 
                {
                    if (!field.IsMissingInSource && !field.IsMissingInTarget && field.IsMatch == false) 
                    {
                        result.CanSync = true;
                        result.IsMatch = false;
                        break;
                    }
                }
            }
        }
    }
}
