using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.MasterList;
using tmg.equinox.applicationservices.viewmodels.UIElement;
using tmg.equinox.applicationservices.viewmodels.FormDesign;
using tmg.equinox.applicationservices.viewmodels.FormContent;
using tmg.equinox.applicationservices.viewmodels.CompareSync;
using tmg.equinox.ruleinterpreter.model;
using tmg.equinox.applicationservices.viewmodels.DocumentRule;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.ruleinterpreter.RuleCompiler;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IUIElementService
    {
        /* get Ordered UIElement List for the Form Design Version
         * list should be ordered in the following manner in oreder of sequence:
         * Parent 1
         * Parent 1 - Child 1
         * Parent 1 - Child 2
         * Parent 1 - Child 2- Child2_Child1
         * Parent 1 - Child 2- Child2_Child2
         * Parent 2
         * Parent 2 - Child 1
         * Parent 2 - Child 2 
         * --- and so on
         * multiple levels of hierarchy should be supported
         * order elements at the same level by sequence
         * */
        IEnumerable<UIElementRowModel> GetUIElementListForFormDesignVersion(int tenantId, int formDesignVersionId);
        UIElementRowModel GetUIElementByID(int uielementId);
        ConfigViewRowModel GetUIElementList(int tenantId, int formDesignVersionId);

        List<UIElementRowModel> GetUIElementByNames(int formDesignVersionId, List<string> elementNames);

        int GetUIElementIDByNames(int formDesignVersionId, string elementNames);
        DateTime GetFormDesignVersionEffectiveDate(int formDesignVersionId);
        List<DocumentViewListViewModel> GetDocumentViewListForExpressionRules(int formDesignId);
        int GetFormDesignVersionID(int FormDesignID, DateTime effecttiveDate);
        List<RepeaterKeyFilterModel> GetTargetKeyFilter(int ruleId);
        List<RepeaterKeyFilterModel> GetKeyFilter(int ruleId, bool isRightOperand);
        List<UIElement> GetUIElementsListByFormDesignId(int tenantId, int formDesignVersionId);
        ServiceResult UpdateRuleDescription(IEnumerable<RuleRowModel> rules);
        ServiceResult UpdateCommentsForUIElement(int formDesignId, int formDesignVersionId, string comments, string userName, string extendedProperties);
        string GetCommentsForUIElement(int formDesignVersionId);

        ServiceResult UpdateFormDesignExcelConfiguration(int formDesignId, int formDesignVersionId, string configuration, string userName);
        string GetFormDesignExcelConfiguration(int formDesignVersionId);

        //Methods added for Document Compare/Sync functionality
        IEnumerable<DocumentElementViewModel> GetRepeaterUIElement(int tenantId, int formDesignVersionId);
        IEnumerable<DocumentElementViewModel> GetDocumentElementList(int tenantId, int formDesignVersionId);
        IEnumerable<DocumentElementBaseModel> GetExpressionRepeaterKeyList(int tenantId, int parentUIElementId, int expressionId, bool rightOnly);
        IEnumerable<DocumentElementBaseModel> GetTargetRepeaterKeyList(int tenantId, int parentUIElementId, int ruleId);
        IEnumerable<DocumentElementBaseModel> GetRepeaterKeyList(int tenantId, int formDesignVersionId, int parentUIElementId);
        IEnumerable<DocumentElementBaseModel> GetElementChildrenList(int tenantId, int formDesignVersionId, int parentUIElementId);
        bool IsNewUIElementCreationRequired(int uielementId, int formDesignVersionID);
        //End
        #region "Section Methods"
        /* get Section Details
         *  */
        SectionElementModel GetSectionDesignDetail(int tenantId, int formDesignVersionId, int sectionElementId);

        /// <summary>
        /// Add Section to Form
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formDesignVersionId"></param>
        /// <param name="parentUIElementId"></param>
        /// <param name="label"></param>
        /// <param name="sequence"></param>
        /// <returns></returns>
        ServiceResult AddSectionDesign(string userName, int tenantId, int formDesignVersionId, int parentUIElementId, string label, string elementType, bool isStandard, int viewType);
        ServiceResult AddSectionDesign(string userName, int tenantId, int formDesignVersionId, int parentUIElementId, string label, string elementType, bool isEnable, bool isVisible, int sequence, IEnumerable<RuleRowModel> rules, bool modifyRules, string extProp, string layout, int viewType, int sourceUIElementId, string customHtml);

        /// <summary>
        /// Update Section Detail
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formDesignVersionId"></param>
        /// <param name="sectionElementId"></param>
        /// <param name="isEnabled"></param>
        /// <param name="helpText"></param>
        /// <param name="isRequired"></param>
        /// <param name="label"></param>
        /// <param name="layoutTypeId"></param>
        /// <param name="isVisible"></param>
        /// <returns></returns>
        ServiceResult UpdateSectionDesign(string userName, int tenantId, int formDesignId, int formDesignVersionId, int sectionElementId, bool isEnabled, string helpText, bool isRequired, bool hasCustomRule, string label, int layoutTypeId, bool isVisible, bool isDataSource, string datasourceName, string dataSourceDescription, IEnumerable<RuleRowModel> rules, bool modifyRules, bool modifyCustomRules, string customRule, string customHtml, int viewType, bool isStandard, string mdmName);
        ServiceResult UpdateSectionDesign(string userName, int tenantId, int formDesignId, int formDesignVersionId, int sectionElementId, bool isEnabled, string helpText, bool isRequired, string label, bool isVisible, IEnumerable<RuleRowModel> rules, bool modifyRules, int viewType, int parentUIElementId, string extProp, string layout, string customHtml, bool isStandard);

        /// <summary>
        /// Get list of Sections for a Form Design Version
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formDesignVersionId"></param>
        /// <param name="uiElementId"></param>
        /// <returns></returns>
        IEnumerable<SectionElementModel> GetSectionList(int tenantId, int formDesignVersionId, int uiElementId);

        /// <summary>
        /// Update a set of Section Sequences
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formDesignVersionId"></param>
        /// <param name="uiElementIdSequences"></param>
        /// <returns></returns>
        ServiceResult UpdateSectionSequences(string userName, int tenantId, int formDesignVersionId, IDictionary<int, int> uiElementIdSequences);

        ServiceResult DeleteSection(int tenantId, int formDesignVersionId, int uiElementId);
        List<int> GetSectionListByFormDesignVersionId(int formDesignVersionId);

        #endregion

        #region "Common Methods for all UI Elements"

        IEnumerable<UIElementSeqModel> GetChildUIElements(int tenantId, int formDesignVersionId, int parentUIElementId);
        IEnumerable<UIElementSeqModel> GetApplicableRepeaterChildUIElementsForDuplicationCheck(int tenantId, int formDesignVersionId, int parentUIElementId);
        /// <summary>
        /// Get Validator
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formDesignVersionId"></param>
        /// <param name="uiElementId"></param>
        /// <returns></returns>
        ValidatorModel GetValidator(int tenantId, int formDesignVersionId, int uiElementId);

        /// <summary>
        /// Add Validator
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formDesignVersionId"></param>
        /// <param name="uiElementId"></param>
        /// <param name="libraryRegexId"></param>
        /// <param name="isLibraryRegex"></param>
        /// <param name="isRequired"></param>
        /// <param name="regex"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        ServiceResult AddValidator(string userName, int tenantId, int formDesignVersionId, int uiElementId, int libraryRegexId, bool isLibraryRegex, bool isRequired, string regex, string message);

        /// <summary>
        /// Update Validator
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formDesignVersionId"></param>
        /// <param name="uiElementId"></param>
        /// <param name="validatorId"></param>
        /// <param name="libraryRegexId"></param>
        /// <param name="isLibraryRegex"></param>
        /// <param name="isRequired"></param>
        /// <param name="regex"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        ServiceResult UpdateValidator(string userName, int tenantId, int formDesignVersionId, int uiElementId, int validatorId, int libraryRegexId, bool isLibraryRegex, bool isRequired, string regex, string message);

        /// <summary>
        /// Get Rules with Expressions
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formDesignVersionId"></param>
        /// <param name="uiElementId"></param>
        /// <returns></returns>
        IEnumerable<RuleRowModel> GetRulesForUIElement(int tenantId, int formDesignVersionId, int uiElementId);

        /// <summary>
        /// Add Expressions to a Rule
        /// </summary>
        /// <param name="expressions"></param>
        /// <returns></returns>
        ServiceResult AddExpressions(string userName, IEnumerable<ExpressionRowModel> expressions);

        /// <summary>
        /// Update Expressions of a Rule
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="expressions"></param>
        /// <returns></returns>
        ServiceResult UpdateExpressions(string userName, IEnumerable<ExpressionRowModel> expressions);

        /// <summary>
        /// Delete Expressions from a Rule
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="expressionIds"></param>
        /// <returns></returns>
        ServiceResult DeleteExpressions(string userName, int tenantId, int formDesignVersionId, int uiElementId, int ruleId, IEnumerable<int> expressionIds);

        #endregion

        #region "TextBox Methods"
        TextBoxElementModel GetTextBox(int tenantId, int formDesignVersionId, int uiElementId);

        ServiceResult UpdateTextBox(string userName, int tenantId, int formDesignId, int formDesignVersionId, int uiElementId,
            bool isEnabled, bool isVisible, bool hasCustomRule, string helpText, bool isLabel,
            bool isMultiline, bool isRequired, int uiElementDataTypeId, string defaultValue, string label, int maxLength,
            int sequence, bool isSpellCheck, bool allowGlobalUpdates, bool? isLibraryRegex, string regex, string CustomRegexMessage, int? libraryRegexId, IEnumerable<RuleRowModel> rules,
            bool modifyRules, bool modifyCustomRules, string customRule, bool maskFlag, int viewType, bool isStandard, string mdmName);
        ServiceResult UpdateTextBox(string userName, int tenantId, int formDesignId, int formDesignVersionId, int uiElementId, bool isEnabled, bool isVisible, string helpText, bool isLabel, bool isMultiline, bool isRequired, int uiElementDataTypeId, string defaultValue, string label, int maxLength, int sequence, Nullable<int> libraryRegexId, IEnumerable<RuleRowModel> rules, bool modifyRules, int viewType, int parentUIElementId, string extProp, bool isStandard);

        ServiceResult AddTextBox(string userName, int tenantId, int formDesignVersionId, int parentUIElementId, bool isMultiline, bool isBlank, bool isLabel, string label, string helpText, int sequence, string elementType, bool isStandard, int viewType);
        ServiceResult AddTextBox(string userName, int tenantId, int formDesignVersionId, int parentUIElementId, bool isMultiline, bool isBlank, bool isLabel, string label, string helpText, int sequence, string elementType, int dataTypeId, bool isRequired, int maxLength, bool isEnable, bool isVisible, int libraryRegexID, List<RuleRowModel> rules, bool modifyRules, string extProp, string defaultValue, int viewType, int sourceUIElementId);
        ServiceResult DeleteTextBox(int tenantId, int formDesignVersionId, int uiElementId);
        ServiceResult ChangeControlText(string userName, int tenantId, int formDesignVersionId, int uiElementId, int uiElementTypeId, string elementType);


        #endregion

        #region "DropDown Methods"
        DropDownElementModel GetDropDown(int tenantId, int formDesignVersionId, int uiElementId);

        ServiceResult UpdateDropDown(string userName, int tenantId, int formDesignId, int formDesignVersionId, int uiElementId, bool isEnabled, bool isVisible, bool hasCustomRule, string helpText,
            bool isRequired, string selectedValue, string label, int sequence, IEnumerable<DropDownItemModel> Items, IEnumerable<RuleRowModel> rules, bool modifyRules, bool modifyCustomRules, string customRule, int uiElementDataTypeID, bool isDropDownTextBox, bool IsSortRequired, Nullable<bool> isLibraryRegex, Nullable<int> libraryRegexId, bool allowGlobalUpdates, bool isDropDownFilterable, int viewType, bool isMultiSelect, bool isStandard,string mdmName);
        ServiceResult UpdateDropDown(string userName, int tenantId, int formDesignId, int formDesignVersionId, int uiElementId, bool isEnabled, bool isVisible, string helpText, bool isRequired, string label, int sequence, IEnumerable<DropDownItemModel> Items, IEnumerable<RuleRowModel> rules, bool modifyRules, bool isDropDownTextBox, Nullable<int> libraryRegexId, int viewType, bool isMultiSelect, int parentUIElementId, string extProp, string defaultValue, bool isStandard);

        ServiceResult AddDropDown(string userName, int tenantId, int formDesignVersionId, int parentUIElementId, string label, string helpText, int sequence, string elementType, bool isDropDownTextBox, bool isStandard, int viewType);
        ServiceResult AddDropDown(string userName, int tenantId, int formDesignVersionId, int parentUIElementId, string label, string helpText, int sequence, string elementType, bool isDropDownTextBox, bool isEnable, bool isVisible, bool isRequired, bool isMultiselect, List<DropDownItemModel> items, List<RuleRowModel> rules, bool modifyRules, string extProp, string defaultValue, int viewType, int sourceUIElementId);

        ServiceResult DeleteDropDown(int tenantId, int formDesignVersionId, int uiElementId);

        ServiceResult ChangeControlDrop(string userName, int tenantId, int formDesignVersionId, int uiElementId, int uiElementTypeId, string elementType, bool isDropDownTextBox);


        #endregion


        #region "Calendar Methods"
        CalendarElementModel GetCalendar(int tenantId, int formDesignVersionId, int uiElementId);

        ServiceResult UpdateCalendar(string userName, int tenantId, int formDesignId, int formDesignVersionId, int uiElementId, bool isEnabled, bool isVisible, bool hasCustomRule, string helpText,
            bool isRequired, string label, int sequence, Nullable<DateTime> defaultDate,
            Nullable<DateTime> minDate, Nullable<DateTime> maxDate, IEnumerable<RuleRowModel> rules, bool modifyRules, bool modifyCustomRules, string customRule, bool globalUpdate, int viewType, bool isStandard,string mdmName);
        ServiceResult UpdateCalendar(string userName, int tenantId, int formDesignId, int formDesignVersionId, int uiElementId, bool isEnabled,
            bool isVisible, string helpText, bool isRequired, string label, int sequence,
            string defaultValue, IEnumerable<RuleRowModel> rules, bool modifyRules, int viewType, int parentUIElementId, string extProp, bool isStandard);

        ServiceResult AddCalendar(string userName, int tenantId, int formDesignVersionId, int parentUIElementId, string label, string helpText, int sequence, string elementType, bool isStandard, int viewType);
        ServiceResult AddCalendar(string userName, int tenantId, int formDesignVersionId, int parentUIElementId, string label, string helpText, int sequence, string elementType, bool isEnable, bool isVisible, bool isRequired, List<RuleRowModel> rules, bool modifyRules, string extProp, string defaultValue, int viewType, int sourceUIElementId);

        ServiceResult DeleteCalendar(int tenantId, int formDesignVersionId, int uiElementId);

        #endregion


        #region "CheckBox Methods"
        CheckBoxElementModel GetCheckBox(int tenantId, int formDesignVersionId, int uiElementId);

        ServiceResult UpdateCheckBox(string userName, int tenantId, int formDesignId, int formDesignVersionId, int uiElementId, bool isEnabled, bool isVisible, bool hasCustomRule, string helpText,
            bool isRequired, string label, string optionLabel, int sequence, bool defaultValue, IEnumerable<RuleRowModel> rules, bool modifyRules, bool modifyCustomRules, string customRule, bool allowGlobalUpdates, int viewType, bool isStandard,string mdmName);
        ServiceResult UpdateCheckBox(string userName, int tenantId, int formDesignId, int formDesignVersionId, int uiElementId, bool isEnabled, bool isVisible, string helpText, bool isRequired, string label, int sequence, bool defaultValue, IEnumerable<RuleRowModel> rules, bool modifyRules, int viewType, int parentUIElementId, string extProp, bool isStandard);

        ServiceResult AddCheckBox(string userName, int tenantId, int formDesignVersionId, int parentUIElementId, string label, string helpText, int sequence, string elementType, bool isStandard, int viewType);
        ServiceResult AddCheckBox(string userName, int tenantId, int formDesignVersionId, int parentUIElementId, string label, string helpText, int sequence, string elementType, bool isEnable, bool isVisible, List<RuleRowModel> rules, bool modifyRules, string extProp, int viewType, int sourceUIElementId);

        ServiceResult DeleteCheckBox(int tenantId, int formDesignVersionId, int uiElementId);

        #endregion


        #region "RadioButton Methods"
        RadioButtonElementModel GetRadioButton(int tenantId, int formDesignVersionId, int uiElementId);

        ServiceResult UpdateRadioButton(string userName, int tenantId, int formDesignId, int formDesignVersionId, int uiElementId, bool isEnabled, bool isVisible, bool hasCustomRule, string helpText,
            bool isRequired, string label, string optionLabel1, string optionLabel2, bool isYesNo, int sequence, bool? defaultValue, IEnumerable<RuleRowModel> rules, bool modifyRules, bool modifyCustomRules, string customRule, bool allowGlobalUpdates, int viewType, bool isStandard,string mdmName);
        ServiceResult UpdateRadioButton(string userName, int tenantId, int formDesignId, int formDesignVersionId, int uiElementId, bool isEnabled, bool isVisible, string helpText, bool isRequired, string label, string optionLabel1, string optionLabel2, bool isYesNo, int sequence, bool? defaultValue, IEnumerable<RuleRowModel> rules, bool modifyRules, int viewType, int parentUIElementId, string extProp, bool isStandard);

        ServiceResult AddRadioButton(string userName, int tenantId, int formDesignVersionId, int parentUIElementId, string label, string helpText, int sequence, string elementType, bool isStandard, int viewType);
        ServiceResult AddRadioButton(string userName, int tenantId, int formDesignVersionId, int parentUIElementId, string label, string helpText, int sequence, string elementType, bool isEnable, bool isVisible, bool isRequired, string optionYes, string optionNo, List<RuleRowModel> rules, bool modifyRules, string extProp, int viewType, int sourceUIElementId);

        ServiceResult DeleteRadioButton(int tenantId, int formDesignVersionId, int uiElementId);

        #endregion

        #region "Repeater Methods"
        RepeaterElementModel GetRepeater(int tenantId, int formDesignVersionId, int uiElementId);

        ServiceResult UpdateRepeater(string userName, int tenantId, int formDesignId, int formDesignVersionId, int uiElementId, bool isEnabled, bool isVisible, bool hasCustomRule, string helpText,
            bool isRequired, string label, int childCount, int sequence, int layoutTypeId, bool isDatasource, string datasourceName, string dataSourceDescription, IEnumerable<RuleRowModel> rules, bool modifyRules, bool modifyCustomRules, string customRule, bool loadFromServer, bool isDataRequired, bool allowBulkUpdate, RepeaterElementModel advancedConfiguration, RepeaterUIElementPropertyModel repeaterTemplates, bool isStandard, string mdmName);
        ServiceResult UpdateRepeater(string userName, int tenantId, int formDesignId, int formDesignVersionId, int uiElementId, bool isEnabled, bool isVisible, string helpText, bool isRequired, string label, int sequence, IEnumerable<RuleRowModel> rules, bool modifyRules, RepeaterElementModel advancedConfiguration, int parentUIElementId, string extProp, string layout, int viewType, bool allowBulkUpdate, bool isStandard);

        ServiceResult AddRepeater(string userName, int tenantId, int formDesignVersionId, int parentUIElementId, string label, string helpText, int sequence, string elementType, bool isStandard, int viewType);
        ServiceResult AddRepeater(string userName, int tenantId, int formDesignVersionId, int parentUIElementId, string label, string helpText, int sequence, string elementType, bool isEnable, bool isVisible, List<RuleRowModel> rules, bool modifyRules, RepeaterElementModel advancedConfiguration, string extProp, string layout, int viewType, bool allowBulkUpdate, int sourceUIElementId);

        ServiceResult DeleteRepeater(int tenantId, int formDesignVersionId, int uiElementId);

        ServiceResult AddDefaultKeyForRepeater(int tenantId, int formDesignVersionId, int parentUIElementId);

        #endregion

        #region "Tab Methods"
        ServiceResult AddTab(string userName, int tenantId, int formDesignVersionId, string label, string helpText);

        /// <summary>
        /// Updates the tab element.
        /// </summary>
        /// <param name="tenantId">tenant identifier.</param>
        /// <param name="UIElementID">UIElement identifier.</param>
        /// <param name="customRule">custom Rule</param>
        /// <returns></returns>        
        ServiceResult UpdateTab(string userName, int tenantId, int UIElementID, bool hasCustomRule, bool modifyCustomRules, string customRule);

        /// <summary>
        /// Gets the tab element details.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="tabElementId">Uielement identifier</param>
        /// <returns></returns>
        TabElementModel GetTabDetail(int tenantId, int tabElementId);
        #endregion

        List<UIElementRowModel> GetRepeaterElementForLookIn(int formDesignVersionId);
        List<FormInstanceViewModel> GetInstances(int formDesignVersionId);
        ServiceResult UpdateFieldSequences(string userName, int tenantId, int formDesignVersionId, int parentElementId, IDictionary<int, int> uiElementIdSequences);

        ServiceResult UpdateRepeaterKeyElement(int tenantId, int parentElementId, IDictionary<int, bool> keyUiElements);

        ServiceResult UpdateFieldCheckDuplicate(string userName, int tenantId, int formDesignVersionId, int parentElementId, IDictionary<int, bool> uiElementIdSequences);

        IEnumerable<LogicalOperatorTypeViewModel> GetDataSourceElementDisplayMode(int tenantId);

        IEnumerable<OperatorTypeViewModel> GetOperatorTypes(int tenantId);

        IEnumerable<TargetPropertyViewModel> GetTargetPropertyTypes(int tenantId);

        int GetID(int sourceUIElementId, string idType);

        ServiceResult UpdateAlternateLabel(int tenantId, int formDesignId, int formDesignVersionId, int uiElementId, string alternateLabel);

        bool CheckIsRepeaterColumnKeyElement(int uiElementID);
        IEnumerable<ElementListViewModel> GetUIElementListForExpressionRuleBuilder(int tenantId, int formDesignVersionId, string searchFor);
        List<UIElementSummaryModel> GetUIElementListMod(int tenantId, int formDesignVersionId);
        ServiceResult UpdatePBPViewImpacts(string userName, int formDesignVersionId, string data);

        #region "Expression Builder Methods"

        IEnumerable<DocumentRuleModel> GetDocumentRule(int uiElementId, int formDesignId, int formDesignVersionId);
        CompiledDocumentRule GetCompiledRuleJSON(int targetElementID, string targetElementPath);
        CompiledDocumentRule GetCompiledRuleJSON(int targetElementID, int formDesignVersionId, bool uIElementNameNeeded);
        CompiledDocumentRule GetCompiledRuleJSONForDSExpRule(int targetElementID, int formDesignVersionId);
        IEnumerable<DocumentRuleTypeModel> GetDocumentRuleType();
        ServiceResult SaveDocumentRule(string userName, int uiElementId, string uiElementType, int formDesignId, int formDesignVersionId, IEnumerable<DocumentRuleModel> nRules);
        ServiceResult DeleteDocumentRule(int dRuleId);
        IEnumerable<DocumentRuleModel> GetAllDocumentRule(int formDesignVersionId);
        ServiceResult UpdateCompileJSONRule(string userName, Dictionary<int, string> data);
        List<DocumentRuleData> GetAllDocumentRuleForTree(int formDesignVersionId, IEnumerable<DocumentRuleModel> documentRules);

        IEnumerable<DocumentRuleExtModel> GetAllExpressionRulesForImpactList(int formDesignVersionId);
        ServiceResult UpdateRuleTreeJSON(string userName, int formDesignVersionId, string data);
        List<DocumentRuleData> GetAllDocumentRuleForEventTree(int formDesignVersionId, IEnumerable<DocumentRuleModel> documentRules);
        string GetCompiledPBPViewImpacts();
        string GetFormName(int formDesignVersionId);
        string GetDocumentRule(int ruleId);
        List<string> ViewList(int formDesignId);
        ServiceResult UpdateEventTreeJSON(string userName, int formDesignVersionId, string data);
        List<SourceDesignDetails> GetParentDesignDetails(int folderVersionId, int anchorDesigId, int formInstanceId);
        string GetUIElementFullPath(int uielementID, int formDesignVersionID);
        ServiceResult UpdateDocumentRules(int oldUiElementId, int newUiElementId, int formDesignVersionID);

        #endregion
        List<RuleRowModel> GetAllRulesForDesignVersion(int formDesignVersionId);

        #region Configuration Rule Tester Data
        ServiceResult SaveConfigurationRuleTesterData(int tenantId, string currentUserName, string designRulesTesterData);
        List<ConfigRulesTesterData> GetFormDesignVersionUIElementsTestData(int tenantId, int formDesignVersionId, int elementId);

        #endregion

        List<ruleinterpreter.model.CompiledDocumentRule> GetAllDocumentRuleBySection(string section, int formDesignVersionId, string sourcePath);

        bool IsMDMNameUnique(int tenantId, int formDesignId, string mdmName, int uiElementId, string uiElementType);
        string GetRuleType(int documentRuleTypeID);
        string GetUIElementTypeByID(int uiElementId);
        ServiceResult CopyElement(int tenantId, int formDesignVersionId, int uiElementId, string elementType, string userName);
        string GetDocumentRuleJsonData(int dRulesID);
        List<ExpressionRuleGeneratorModel.Sources> UpdateFormDesignIdInSourceData(List<ExpressionRuleGeneratorModel.Sources> sourceData, DateTime effecttiveDate);
    }
}
