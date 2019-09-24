using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.applicationservices.FormDesignDetail
{
    internal class UIElementDesignBuilder
    {
        UIElement element;
        IEnumerable<UIElement> formElementList;
        List<DataSourceDesign> dataSourceList;
        IUnitOfWorkAsync _unitOfWork;
        List<DuplicationDesign> DuplicationList;
        List<ValidationDesign> ValidationList;
        List<Validator> ValidatorList;
        List<UIElement> DuplicatorList;
        List<RuleDesign> rulesList;
        List<ElementRuleMap> ruleMapList;
        MasterLists msLists;
        StringBuilder customRulesJSON;
        List<FormDesignVersionUIElement> _frmDesignVersionElementList;
        int _formDesignVersionId;

        internal UIElementDesignBuilder(UIElement element, IEnumerable<UIElement> formElementList, List<DataSourceDesign> dataSourceList, List<RuleDesign> rulesList,
            List<Validator> validatorList, List<UIElement> duplicatorList, MasterLists msLists, IUnitOfWorkAsync unitOfWork, ref List<ValidationDesign> validationList, ref List<DuplicationDesign> duplicationList, ref List<ElementRuleMap> ruleMapList, ref StringBuilder customRulesJSON, int formDesignVersionId,List<FormDesignVersionUIElement> frmDesignVersionElementList)
        {
            this.element = element;
            this.formElementList = formElementList;
            this.dataSourceList = dataSourceList;
            this._unitOfWork = unitOfWork;
            this.ValidationList = validationList;
            this.DuplicationList = duplicationList;
            this.ValidatorList = validatorList;
            this.DuplicatorList = duplicatorList;
            this.rulesList = rulesList;
            this.ruleMapList = ruleMapList;
            this.msLists = msLists;
            this.customRulesJSON = customRulesJSON;
            this._frmDesignVersionElementList = frmDesignVersionElementList;
            this._formDesignVersionId = formDesignVersionId;
        }

        internal ElementDesign BuildElement(string fullParentName)
        {
            ElementDesign design = new ElementDesign();
            design.ElementID = element.UIElementID;
            design.Name = element.UIElementName;
            design.GeneratedName = element.GeneratedName;
            design.Label = this.GetAlternateLabel() ?? element.Label;
            design.Enabled = element.Enabled.HasValue == true ? element.Enabled.Value : false;
            design.Visible = element.Visible.HasValue == true ? element.Visible.Value : false;
            var ele = this._frmDesignVersionElementList.Where(x => x.UIElementID == element.UIElementID).FirstOrDefault();
            if (ele != null)
            {
                design.Type = ele.TypeDescription;
                design.DataType = ele.ApplicationDataTypeName;
            }
            else
            {
                design.Type = string.Empty;
                design.DataType = string.Empty;
            }
            design.MaxLength = GetMaxLength();
            design.DataTypeID = element.UIElementDataTypeID;
            design.DefaultValue = GetDefaultValue();
            design.HelpText = element.HelpText;
            design.FullName = fullParentName + "." + element.GeneratedName;
            design.HasCustomRule = element.HasCustomRule ?? false;
            design.IsLabel = (element is TextBoxUIElement) ? ((TextBoxUIElement)element).IsLabel : false;
            design.SpellCheck = (element is TextBoxUIElement) ? ((TextBoxUIElement)element).SpellCheck ?? false : false;
            design.Multiline = (element is TextBoxUIElement) ? ((TextBoxUIElement)element).IsMultiline ?? false : false;
            design.MultiSelect = (element is DropDownUIElement) ? ((DropDownUIElement)element).IsMultiSelect : false;
            design.OptionLabel = (element is RadioButtonUIElement) ? ((RadioButtonUIElement)element).OptionLabel : "";
            design.OptionLabelNo = (element is RadioButtonUIElement) ? ((RadioButtonUIElement)element).OptionLabelNo : "";
            design.Items = element is DropDownUIElement ? GetDropDownItems() : null;
            design.Repeater = element is RepeaterUIElement ? GetRepeaterDesign(fullParentName) : null;
            design.Section = element is SectionUIElement ? GetSectionDesign(fullParentName) : null;
            design.MaxDate = element is CalendarUIElement ? ((CalendarUIElement)element).MaxDate : null;
            design.MinDate = element is CalendarUIElement ? ((CalendarUIElement)element).MinDate : null;
            design.IsDropDownTextBox = element is DropDownUIElement ? ((DropDownUIElement)element).IsDropDownTextBox : false;
            design.IsDropDownFilterable = element is DropDownUIElement ? ((DropDownUIElement)element).IsDropDownFilterable : false;
            design.IsSortRequired = element is DropDownUIElement ? ((DropDownUIElement)element).IsSortRequired : false;
            design.CheckDuplicate = element.CheckDuplicate;
            design.IsRichTextBox = (element is TextBoxUIElement) && ((TextBoxUIElement)element).UIElementTypeID == ElementTypes.RICHTEXTBOXID ? true : false; //TODO Optimized
            design.Layout = ViewTypes.GetViewType(element.ViewType);
            design.EffDt = GetEffectiveDate(element.UIElementID);
            design.Op = GetOperation(element.UIElementID);
            design.IsSameSectionRuleSource = element.IsSameSectionRuleSource;
            design.MDMName = element.MDMName;
            AddValidationDesign(fullParentName);
            AddDuplicationDesign(fullParentName);
            AddRulesToExecute(design);

            if (!(element is RepeaterUIElement))
                customRulesJSON.AppendLine(element.CustomRule);

            return design;
        }

        private string GetEffectiveDate(int UIElementID)
        {
            DateTime? effectiveDt = msLists.ElementMap
                                    .Where(c => c.UIElementID == UIElementID)
                                    .Select(c => c.EffectiveDate)
                                    .SingleOrDefault();
            return ((DateTime)effectiveDt).ToString("MM/dd/yyyy");
        }

        private string GetOperation(int UIElementID)
        {
            string operation = msLists.IgnoreElementList
                                    .Where(c => c.UIElementID == UIElementID)
                                    .Select(c => c.Operation)
                                    .SingleOrDefault();
            return operation;
        }

        private RepeaterDesign GetRepeaterDesign(string fullParentName)
        {
            RepeaterDesignBuilder builder = new RepeaterDesignBuilder((RepeaterUIElement)element, formElementList, dataSourceList, rulesList, ValidatorList, DuplicatorList, msLists, _unitOfWork, ref ValidationList, ref DuplicationList, ref ruleMapList, ref customRulesJSON, this._formDesignVersionId,this._frmDesignVersionElementList);
            RepeaterDesign design = builder.BuildRepeater(fullParentName);
            return design;
        }

        private SectionDesign GetSectionDesign(string fullParentName)
        {

            SectionDesignBuilder builder = new SectionDesignBuilder((SectionUIElement)element, formElementList, dataSourceList, rulesList, ValidatorList, DuplicatorList, msLists, _unitOfWork, ref ValidationList, ref DuplicationList, ref ruleMapList, ref customRulesJSON, this._formDesignVersionId, this._frmDesignVersionElementList);
            SectionDesign design = builder.BuildSection(fullParentName);
            return design;
        }

        private void AddValidationDesign(string fullParentName)
        {
            ValidationDesignBuilder builder = new ValidationDesignBuilder(element, formElementList, ValidatorList, msLists.LibraryRegexes, msLists.ElementDataTypes);
            ValidationDesign design = builder.BuildElement(fullParentName);
            if (design != null)
            {
                ValidationList.Add(design);
            }
        }

        private void AddDuplicationDesign(string fullParentName)
        {
            DuplicationDesignBuilder builder = new DuplicationDesignBuilder(element, formElementList, DuplicatorList, msLists.LibraryRegexes, msLists.ElementDataTypes);
            DuplicationDesign design = builder.BuildElement(fullParentName);
            if (design != null)
            {
                DuplicationList.Add(design);
            }
        }

        private void AddRulesToExecute(ElementDesign design)
        {
            var rules = from rule in rulesList
                        where (rule.Expressions.Count(d => d.LeftOperand == design.Name || d.RightOperand == design.Name) > 0)
                            || (rule.IsResultSuccessElement == true && rule.SuccessValue == design.Name)
                            || (rule.IsResultFailureElement == true && rule.FailureValue == design.Name)
                        select rule.RuleID;
            if (rules != null && rules.Count() > 0)
            {
                ElementRuleMap ruleMap = new ElementRuleMap();
                ruleMap.FullName = design.FullName;
                ruleMap.Rules = rules.ToList();
                ruleMapList.Add(ruleMap);
            }
        }

        private string GetUIElementDataType()
        {
            string dataType = string.Empty;
            var desc = (from dType in msLists.ElementDataTypes
                        where dType.ApplicationDataTypeID == element.UIElementDataTypeID
                        select dType.ApplicationDataTypeName).FirstOrDefault();
            if (desc != null)
            {
                dataType = desc;
            }
            return dataType;
        }


        private string GetUIElementType()
        {
            string uiElementType = string.Empty;
            //Since UIElementTypeID column exists in separate tables for TextBox, Radio, Section etc, we would be using 
            //reflection to get the Property Value.
            //As UIElement is inherited by all Elements, All the property of those elements will be available through inheritance & reflection
            PropertyInfo propertyInfo = element.GetType()
                                                .GetProperties()
                                                .Where(l => l.Name == "UIElementTypeID")
                                                .FirstOrDefault();
            if (propertyInfo != null)
            {
                int typeId = Convert.ToInt32(propertyInfo.GetValue(element, null));
                uiElementType = msLists.ElementTypes
                                    .Where(c => c.UIElementTypeID == typeId)
                                    .Select(c => c.Description)
                                    .SingleOrDefault();
            }
            return uiElementType;
        }

        private List<Item> GetDropDownItems()
        {
            List<Item> dropDownItemList = null;
            try
            {
                dropDownItemList = (from i in this._unitOfWork.Repository<DropDownElementItem>()
                                                                 .Query()
                                                                 .Filter(c => c.UIElementID == element.UIElementID)
                                                                 .Get()
                                    orderby i.Sequence
                                    select new Item
                                    {
                                        ItemID = i.ItemID,
                                        ItemValue = i.Value,
                                        ItemText = i.DisplayText
                                    }).ToList();
            }
            catch (Exception)
            {
                throw;
            }
            return dropDownItemList;

        }

        private string GetAlternateLabel()
        {
            string altLabel = null;
            try
            {
                var altLabelRow = msLists.AlternateLabelList
                              .Where(c => c.UIElementID == element.UIElementID).FirstOrDefault();


                if (altLabelRow != null)
                {
                    altLabel = altLabelRow.AlternateLabel;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return altLabel;

        }

        private string GetDefaultValue()
        {
            string defaultValue = string.Empty;
            try
            {
                if (element is TextBoxUIElement)
                {
                    defaultValue = ((TextBoxUIElement)element).DefaultValue;
                }
                else if (element is RadioButtonUIElement)
                {
                    defaultValue = ((RadioButtonUIElement)element).DefaultValue == true ? "True" : ((RadioButtonUIElement)element).DefaultValue == false ? "False" : null;
                }
                else if (element is CheckBoxUIElement)
                {
                    defaultValue = ((CheckBoxUIElement)element).DefaultValue == true ? "True" : ((CheckBoxUIElement)element).DefaultValue == false ? "False" : null;
                }
                else if (element is DropDownUIElement)
                {
                    defaultValue = ((DropDownUIElement)element).SelectedValue;
                }
                else if (element is CalendarUIElement)
                {
                    defaultValue = ((CalendarUIElement)element).DefaultDate.HasValue ? ((CalendarUIElement)element).DefaultDate.Value.ToString("MM/dd/yyyy") : null;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return defaultValue;
        }

        private int GetMaxLength()
        {
            int maxLength = 0;
            if (element is TextBoxUIElement)
            {
                TextBoxUIElement textBoxUIElement = (TextBoxUIElement)element;
                maxLength = textBoxUIElement.MaxLength;
            }
            return maxLength;
        }
    }
}
