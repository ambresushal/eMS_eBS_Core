using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.applicationservices.viewmodels.UIElement;


namespace tmg.equinox.web.PBPView
{
    public class ViewImpactDesignHelper
    {
        public string BuildImpactedFieldList(IEnumerable<DocumentRuleExtModel> ruleList, IEnumerable<UIElementRowModel> sourceDocElements, IEnumerable<UIElementRowModel> targetDocElements, FormDesignVersionDetail detail)
        {
            string result = string.Empty;
            List<SourceElement> sourceLementList = new List<SourceElement>();
            List<ImpactedElement> impactedConfigRules = new List<ImpactedElement>();
            Dictionary<string, List<ImpactedElement>> targetConfigRuleImpactObject = new Dictionary<string, List<ImpactedElement>>();
            foreach (var rule in ruleList)
            {
                ImpactedElement impactedElement = BuildImpactedField(rule);

                JToken ruleJSON = JToken.Parse(rule.RuleJSON);
                var sources = ruleJSON.SelectToken(DocumentViewImpactConstant.SOURCES);

                foreach (var src in sources)
                {
                    if (Convert.ToString(src[DocumentViewImpactConstant.SOURCELEMENT]).StartsWith(DocumentViewImpactConstant.ANCHORNAME))
                    {
                        string[] sourceElement = GetElementPath(Convert.ToString(src[DocumentViewImpactConstant.SOURCELEMENT])).Split('.');
                        string fieldName = sourceElement[sourceElement.Length - 1];
                        string elementPath = string.Join(".", sourceElement.Where(s => s != fieldName).ToList());

                        var extSource = sourceLementList.Where(s => s.ElementName == fieldName && s.ElementPath == elementPath).FirstOrDefault();
                        if (extSource == null)
                        {
                            var secEle = sourceDocElements.Where(s => s.GeneratedName == sourceElement[sourceElement.Length - 2]).FirstOrDefault();
                            if (secEle != null)
                            {
                                var ele = sourceDocElements.Where(s => s.GeneratedName == fieldName && s.ParentElementID == secEle.UIElementID).FirstOrDefault();
                                if (ele != null)
                                {
                                    SourceElement source = new SourceElement();
                                    source.ElementID = ele.UIElementID;
                                    source.ElementName = fieldName;
                                    source.ElementLabel = ele.Label;
                                    source.ElementPath = elementPath;
                                    source.ElementPathName = GetParentLabel(sourceElement, sourceDocElements);
                                    source.ImpactedElements = new List<ImpactedElement>();
                                    source.ImpactedElements.Add(impactedElement);
                                    impactedConfigRules = new List<ImpactedElement>();
                                    if (targetConfigRuleImpactObject.ContainsKey(rule.TargetElementPath) == false)
                                    {
                                        impactedConfigRules = GetImpactedPBPViewConfigRules(detail, impactedElement, targetDocElements);
                                        targetConfigRuleImpactObject.Add(rule.TargetElementPath, impactedConfigRules);
                                    }
                                    else
                                    {
                                        impactedConfigRules = targetConfigRuleImpactObject[rule.TargetElementPath];
                                    }
                                    if (impactedConfigRules != null && impactedConfigRules.Count > 0)
                                    {
                                        source.ImpactedElements.AddRange(impactedConfigRules);
                                    }
                                    sourceLementList.Add(source);
                                }
                            }
                        }
                        else
                        {
                            extSource.ImpactedElements.Add(impactedElement);
                            impactedConfigRules = new List<ImpactedElement>();
                            if (targetConfigRuleImpactObject.ContainsKey(rule.TargetElementPath) == false)
                            {
                                impactedConfigRules = GetImpactedPBPViewConfigRules(detail, impactedElement, targetDocElements);
                                targetConfigRuleImpactObject.Add(rule.TargetElementPath, impactedConfigRules);
                            }
                            else
                            {
                                impactedConfigRules = targetConfigRuleImpactObject[rule.TargetElementPath];
                            }
                            if (impactedConfigRules != null && impactedConfigRules.Count > 0)
                            {
                                extSource.ImpactedElements.AddRange(impactedConfigRules);
                            }
                        }
                    }
                }
            }

            return result = JsonConvert.SerializeObject(sourceLementList);
        }

        private string GetParentLabel(string[] elementPaths, IEnumerable<UIElementRowModel> sourceDocElements)
        {
            string sectionElement = "";
            for (int i = 0; i < elementPaths.Length - 1; i++)
            {
                var sec = sourceDocElements.Where(s => s.GeneratedName == elementPaths[i]).FirstOrDefault();
                if (sec != null)
                {
                    sectionElement += sec.Label + " => ";
                }
            }

            return sectionElement.Substring(0, sectionElement.Length - 4);
        }

        private ImpactedElement BuildImpactedField(DocumentRuleExtModel rule)
        {
            string[] elementParts = GetElementPath(rule.TargetElementPath).Split('.');

            ImpactedElement impactedElement = new ImpactedElement();
            impactedElement.ElementID = rule.TargetUIElementID;
            impactedElement.ElementName = rule.TargetUIElementName;
            impactedElement.ElementLabel = rule.TargetUIElementLabel;
            impactedElement.ElementGeneratedName = elementParts[elementParts.Length - 1];
            impactedElement.SectionName = rule.TargetSectionElementName;
            impactedElement.SectionGeneratedName = rule.TargetSectionGeneratedName;
            impactedElement.ElementPath = string.Join(".", elementParts.Where(s => s != impactedElement.ElementName).ToList());
            impactedElement.ElementPathLabel = rule.TargetElementPathLabel;
            impactedElement.UpdateType = DocumentViewImpactConstant.UPDATETYPE;
            impactedElement.Action = DocumentViewImpactConstant.ACTION;

            return impactedElement;
        }

        private string GetElementPath(string fullPath)
        {
            string result = fullPath;
            var pattern = @"(?<=\[).*?(?=\])";
            var matches = Regex.Matches(fullPath, pattern);
            if (matches.Count > 0)
            {
                result = matches[0].Groups[0].Value;
            }

            return result;
        }
        private List<ImpactedElement> GetImpactedPBPViewConfigRules(FormDesignVersionDetail detail, ImpactedElement impactedElement, IEnumerable<UIElementRowModel> targetDocElements)
        {
            List<ImpactedElement> configRules = new List<ImpactedElement>();
            try
            {
                if (detail != null && impactedElement != null)
                {
                    List<RuleDesign> rules = detail.Rules;
                    if (rules != null && rules.Count > 0)
                    {
                        List<RuleDesign> impactedRules = rules.Where(x => x.Expressions.Where(y => y.LeftOperand == impactedElement.ElementName).ToList().Count > 0).ToList();
                        if (impactedRules != null && impactedRules.Count > 0)
                        {
                            foreach (var rule in impactedRules)
                            {
                                string[] elementParts = GetElementPath(rule.UIElementFullName).Split('.');
                                ImpactedElement element = new ImpactedElement();
                                element.ElementID = rule.UIELementID;
                                element.ElementName = rule.UIElementFormName;
                                element.ElementLabel = GetUIElementLabel(targetDocElements, rule.UIElementName);
                                element.ElementGeneratedName = elementParts[elementParts.Length - 1];
                                element.ElementPath = string.Join(".", elementParts.Where(s => s != impactedElement.ElementName).ToList());
                                GetSectionUIElement(rule.UIElementFullName, targetDocElements, element);
                                element.UpdateType = GetUpdateType(rule.TargetPropertyTypeId);
                                element.Action = DocumentViewImpactConstant.ACTION;
                                configRules.Add(element);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return configRules;
        }
        private string GetUIElementLabel(IEnumerable<UIElementRowModel> targetDocElements, string elementName)
        {
            string uiElementLabel = String.Empty;
            try
            {
                var uiElement = targetDocElements.Where(s => s.GeneratedName == elementName).FirstOrDefault();
                if (uiElement != null)
                {
                    uiElementLabel = uiElement.Label;
                }
            }
            catch (Exception ex)
            {
            }
            return uiElementLabel;
        }

        private void GetSectionUIElement(string fullPath, IEnumerable<UIElementRowModel> targetDocElements, ImpactedElement element)
        {
            try
            {
                if (!String.IsNullOrEmpty(fullPath))
                {
                    string[] path = fullPath.Split('.');
                    if (path.Length > 0)
                    {
                        int count = 0;
                        foreach (string secName in path)
                        {
                            var sectionElement = targetDocElements.Where(s => s.GeneratedName == secName).FirstOrDefault();
                            if (sectionElement != null)
                            {
                                if (count == 0)
                                {
                                    element.SectionName = sectionElement.UIElementName;
                                    element.SectionGeneratedName = secName;
                                }
                                var sectionName = sectionElement.Label;
                                // Skip last as it is element itself and we want path till parent element 
                                if (count != path.Length - 1)
                                {
                                    element.ElementPathLabel += !string.IsNullOrEmpty(sectionName) ? sectionName + " => " : "";
                                }
                                // If Section has config rule then set ElementPathLabel as Section Name
                                if (count == 0 && path.Length == 1)
                                {
                                    element.ElementPathLabel = !string.IsNullOrEmpty(sectionName) ? sectionName + " => " : "";
                                }
                                count++;
                            }
                        }
                        if (!string.IsNullOrEmpty(element.ElementPathLabel))
                        {
                            element.ElementPathLabel = element.ElementPathLabel.Substring(0, element.ElementPathLabel.Length - 4);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private string GetUpdateType(int taargetPropertyID)
        {
            string type = String.Empty;
            try
            {
                switch (taargetPropertyID)
                {
                    case 1:
                        type = DocumentViewImpactConstant.ENABLEDISABLETYPE;
                        break;
                    case 2:
                        type = DocumentViewImpactConstant.RUNVALIDATIONTYPE;
                        break;
                    case 3:
                        type = DocumentViewImpactConstant.VALUETYPE;
                        break;
                    case 4:
                        type = DocumentViewImpactConstant.VISIBILITYTYPE;
                        break;
                    case 5:
                        type = DocumentViewImpactConstant.ISREQUIREDTYPE;
                        break;
                    case 6:
                        type = DocumentViewImpactConstant.ERRORTYPE;
                        break;
                    case 7:
                        type = DocumentViewImpactConstant.EXPRESSIONVALUETYPE;
                        break;
                    case 8:
                        type = DocumentViewImpactConstant.HIGHLIGHTTYPE;
                        break;
                    case 9:
                        type = DocumentViewImpactConstant.DIALOGTYPE;
                        break;
                    case 10:
                        type = DocumentViewImpactConstant.CUSTOMRULETYPE;
                        break;
                }
            }
            catch (Exception ex)
            {
            }
            return type;
        }
    }
}
