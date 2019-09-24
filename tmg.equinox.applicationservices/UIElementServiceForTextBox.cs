using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.UIElement;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;
using tmg.equinox.repository.extensions;
using System.Transactions;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.domain.entities.Enums;

namespace tmg.equinox.applicationservices
{
    public partial class UIElementService : IUIElementService
    {
        #region Private Members

        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor

        #endregion Constructor

        #region Public Methods
        #region Text Box Methods
        public TextBoxElementModel GetTextBox(int tenantId, int formDesignId, int uiElementId)
        {
            TextBoxElementModel textBoxElementModel = null;
            try
            {
                TextBoxUIElement textBoxElement = this._unitOfWork.RepositoryAsync<TextBoxUIElement>()
                                                            .Query()
                                                            .Filter(c => c.UIElementID == uiElementId)
                                                            .Include(c => c.ApplicationDataType)
                                                            .Include(c => c.Validators)
                                                            .Get()
                                                            .SingleOrDefault();

                if (textBoxElement != null)
                {
                    textBoxElementModel = new TextBoxElementModel();
                    textBoxElementModel.DefaultValue = textBoxElement.DefaultValue;
                    textBoxElementModel.Enabled = textBoxElement.Enabled ?? false;
                    textBoxElementModel.FormDesignVersionID = textBoxElement.FormID;
                    textBoxElementModel.HasCustomRule = textBoxElement.HasCustomRule ?? false;
                    textBoxElementModel.CustomRule = textBoxElement.CustomRule;
                    textBoxElementModel.HelpText = textBoxElement.HelpText;
                    textBoxElementModel.DefaultValue = textBoxElement.DefaultValue;
                    textBoxElementModel.IsLabel = textBoxElement.IsLabel;
                    textBoxElementModel.IsMultiLine = textBoxElement.IsMultiline ?? false;
                    textBoxElementModel.IsRequired = textBoxElement.RequiresValidation;
                    textBoxElementModel.Label = this.GetAlternateLabel(formDesignId, uiElementId) ?? textBoxElement.Label;
                    textBoxElementModel.MaxLength = textBoxElement.MaxLength;
                    textBoxElementModel.ParentUIElementID = textBoxElement.ParentUIElementID ?? 0;
                    textBoxElementModel.Sequence = textBoxElement.Sequence;
                    textBoxElementModel.SpellCheck = textBoxElement.SpellCheck ?? false;
                    textBoxElementModel.AllowGlobalUpdates = textBoxElement.AllowGlobalUpdates ?? false;
                    textBoxElementModel.TenantID = tenantId;
                    textBoxElementModel.UIElementDataTypeDisplayText = textBoxElement.ApplicationDataType.DisplayText;
                    textBoxElementModel.UIElementDataTypeID = textBoxElement.UIElementDataTypeID;
                    textBoxElementModel.UIElementDataTypeName = textBoxElement.ApplicationDataType.ApplicationDataTypeName;
                    textBoxElementModel.UIElementID = textBoxElement.UIElementID;
                    textBoxElementModel.Visible = textBoxElement.Visible ?? false;
                    textBoxElementModel.TenantID = tenantId;
                    textBoxElementModel.ViewType = textBoxElement.ViewType;
                    textBoxElementModel.IsStandard = textBoxElement.IsStandard;
                    textBoxElementModel.FormDesignID = textBoxElement.FormID;
                    textBoxElementModel.MDMName = textBoxElement.MDMName;
                    if (textBoxElement.Validators != null && textBoxElement.Validators.Count > 0)
                    {
                        List<Validator> vals = textBoxElement.Validators.ToList();
                        textBoxElementModel.IsLibraryRegex = vals[0].IsLibraryRegex;
                        textBoxElementModel.LibraryRegexID = vals[0].LibraryRegexID;
                        textBoxElementModel.Regex = vals[0].Regex;
                        textBoxElementModel.CustomRegexMessage = vals[0].Message;
                        textBoxElementModel.MaskFlag = vals[0].MaskFlag;
                    }

                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return textBoxElementModel;
        }

        public ServiceResult UpdateTextBox(string userName, int tenantId, int formDesignId, int formDesignVersionId, int uiElementId, bool isEnabled, bool isVisible, bool hasCustomRule, string helpText, bool isLabel, bool isMultiline, bool isRequired, int uiElementDataTypeId, string defaultValue, string label, int maxLength, int sequence, bool isSpellCheck, bool allowGlobalUpdates,
            Nullable<bool> isLibraryRegex, string regex, string CustomRegexMessage, Nullable<int> libraryRegexId, IEnumerable<RuleRowModel> rules, bool modifyRules, bool modifyCustomRules, string customRule, bool maskFlag, int viewType, bool isStandard, string mdmName)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                //check if this form is Finalized already
                //bool isFormDesignVersionFinalized = this._unitOfWork.RepositoryAsync<FormDesignVersion>().IsFormDesignVersionFinalized(formDesignVersionId);

                //if (isFormDesignVersionFinalized)
                //{
                //    result.Result = ServiceResultStatus.Failure;
                //    result.Items.ToList().Add(new ServiceResultItem { Messages = new string[] { "Unable to Update finalized Form Design Version" } });
                //}
                //else
                {
                    TextBoxUIElement textBoxElement = this._unitOfWork.RepositoryAsync<TextBoxUIElement>().FindById(uiElementId);


                    List<FormDesignVersionUIElementMap> list = this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>()
                                                                                .Query()
                                                                                .Include(c => c.FormDesignVersion)
                                                                                .Include(c => c.FormDesignVersion.Status)
                                                                                .Filter(c => c.UIElementID == uiElementId && c.FormDesignVersionID != formDesignVersionId)
                                                                                .Get()
                                                                                .ToList();
                    FormDesignVersionUIElementMap currentMap = this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>()
                                                                                .Query()
                                                                                .Include(c => c.FormDesignVersion)
                                                                                .Filter(c => c.FormDesignVersionID == formDesignVersionId && c.UIElementID == uiElementId)
                                                                                .Get()
                                                                                .FirstOrDefault();

                    DataSourceMapping dataSourceMapping = this._unitOfWork.RepositoryAsync<DataSourceMapping>()
                                                                                .Query()
                                                                                .Filter(c => c.UIElementID == uiElementId
                                                                                    && c.FormDesignID == formDesignId
                                                                                    && c.FormDesignVersionID == formDesignVersionId)
                                                                                .Get()
                                                                                .FirstOrDefault();

                    int formDesignID = currentMap.FormDesignVersion.FormDesignID.Value;
                    //check if this element exists is a Finalized Form Design Version(which means a new element 
                    //has to be created for this Form Design Version
                    int finalizedStatusID = Convert.ToInt32(tmg.equinox.domain.entities.Status.Finalized);
                    if (list.Any(c => c.FormDesignVersion.StatusID == finalizedStatusID))
                    {
                        //update effective date of removal for finalized form design versions
                        foreach (var item in list.ToList())
                        {
                            item.EffectiveDateOfRemoval = currentMap.EffectiveDate.Value.AddDays(-1);
                            item.Operation = "D";
                            this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Update(item);
                        }
                        //create new element
                        using (var scope = new TransactionScope())
                        {
                            TextBoxUIElement element = new TextBoxUIElement();
                            element.UIElementName = textBoxElement.UIElementName;
                            element.FormID = formDesignID;
                            element.Sequence = textBoxElement.Sequence;
                            element.Label = textBoxElement.Label;
                            element.ParentUIElementID = textBoxElement.ParentUIElementID;
                            SetTextBoxValues(userName, isEnabled, isVisible, hasCustomRule, helpText, isLabel, isMultiline, isRequired, uiElementDataTypeId, defaultValue, label, maxLength, sequence, isSpellCheck, allowGlobalUpdates,
                                isLibraryRegex, regex, CustomRegexMessage, maskFlag, libraryRegexId, true, ref element, modifyCustomRules, customRule, viewType, isStandard, mdmName);

                            this._unitOfWork.RepositoryAsync<TextBoxUIElement>().Insert(element);
                            this._unitOfWork.Save();

                            //Added newly created UIelement Id to Service Result
                            List<ServiceResultItem> items = new List<ServiceResultItem>();
                            items.Add(new ServiceResultItem() { Messages = new string[] { element.UIElementID.ToString() } });

                            //update DataSourceMapping
                            if (dataSourceMapping != null)
                            {
                                dataSourceMapping.UIElementID = element.UIElementID;
                                this._unitOfWork.RepositoryAsync<DataSourceMapping>().Update(dataSourceMapping);
                            }

                            //add Validator
                            AddValidator(userName, element.UIElementID, isRequired, isLibraryRegex, libraryRegexId, regex, CustomRegexMessage, true, maskFlag);
                            //add Rules
                            var newRules = ChangeRules(userName, tenantId, formDesignVersionId, textBoxElement.UIElementID, element.UIElementID, rules, true);
                            //Get Copied Rules Mapping
                            List<string> ruleMap = newRules.Select(s => s.SourceRuleID + ":" + s.RuleId).ToList();
                            items.Add(new ServiceResultItem() { Messages = new string[] { string.Join(",", ruleMap.ToArray()) } });

                            //update existing element map from FormDesignVersionUIElementMap to have new UIElementID
                            currentMap.UIElementID = element.UIElementID;
                            currentMap.EffectiveDate = currentMap.EffectiveDate;
                            currentMap.EffectiveDateOfRemoval = null;
                            currentMap.Operation = "U";

                            //Update Alternate Label
                            if (!string.Equals(element.Label, label) || this.isExistsInAlternateLabel(element.UIElementID))
                            {
                                this.UpdateAlternateLabel(tenantId, formDesignId, formDesignVersionId, element.UIElementID, label);
                            }

                            this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Update(currentMap);
                            this._unitOfWork.Save();
                            this.UpdateDocumentRules(uiElementId, currentMap.UIElementID, currentMap.FormDesignVersionID);
                            result.Items = items;
                            result.Result = ServiceResultStatus.Success;
                            scope.Complete();
                        }
                    }
                    else
                    {
                        if (textBoxElement != null)
                        {
                            using (var scope = new TransactionScope())
                            {
                                sequence = sequence == 0 ? textBoxElement.Sequence : sequence;
                                SetTextBoxValues(userName, isEnabled, isVisible, hasCustomRule, helpText, isLabel, isMultiline, isRequired, uiElementDataTypeId, defaultValue, label, maxLength, sequence, isSpellCheck, allowGlobalUpdates,
                                    isLibraryRegex, regex, CustomRegexMessage, maskFlag, libraryRegexId, false, ref textBoxElement, modifyCustomRules, customRule, viewType, isStandard, mdmName);
                                //change Validator
                                AddValidator(userName, uiElementId, isRequired, isLibraryRegex, libraryRegexId, regex, CustomRegexMessage, false, maskFlag);
                                //change Rules
                                if (modifyRules == true && rules != null)
                                {
                                    ChangeRules(userName, tenantId, formDesignVersionId, uiElementId, uiElementId, rules, false);
                                }
                                else if (modifyRules == true && rules == null)
                                {
                                    IEnumerable<RuleRowModel> currentRules = GetRulesForUIElement(tenantId, formDesignVersionId, uiElementId);
                                    if (currentRules != null || currentRules.Count() > 0)
                                    {
                                        var delRules = from del in currentRules select del;
                                        if (delRules != null)
                                        {
                                            foreach (var delRule in delRules)
                                            {
                                                DeleteRule(delRule);
                                            }
                                        }
                                    }
                                }
                                this._unitOfWork.RepositoryAsync<TextBoxUIElement>().Update(textBoxElement);
                                this._unitOfWork.Save();

                                //Update Alternate Label
                                if (!string.Equals(textBoxElement.Label, label) || this.isExistsInAlternateLabel(textBoxElement.UIElementID))
                                {
                                    this.UpdateAlternateLabel(tenantId, formDesignId, formDesignVersionId, textBoxElement.UIElementID, label);
                                }

                                //Added newly created UIelement Id to Service Result
                                List<ServiceResultItem> items = new List<ServiceResultItem>();
                                items.Add(new ServiceResultItem() { Messages = new string[] { textBoxElement.ParentUIElementID.ToString(), textBoxElement.UIElementID.ToString() } });
                                result.Items = items;
                                result.Result = ServiceResultStatus.Success;
                                scope.Complete();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                result = ex.ExceptionMessages();
            }
            return result;
        }

        public ServiceResult UpdateTextBox(string userName, int tenantId, int formDesignId, int formDesignVersionId, int uiElementId, bool isEnabled, bool isVisible, string helpText, bool isLabel, bool isMultiline, bool isRequired, int uiElementDataTypeId, string defaultValue, string label, int maxLength, int sequence, Nullable<int> libraryRegexId, IEnumerable<RuleRowModel> rules, bool modifyRules, int viewType, int parentUIElementId, string extProp, bool isStandard)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                //check if this form is Finalized already
                //bool isFormDesignVersionFinalized = this._unitOfWork.RepositoryAsync<FormDesignVersion>().IsFormDesignVersionFinalized(formDesignVersionId);

                //if (isFormDesignVersionFinalized)
                //{
                //    result.Result = ServiceResultStatus.Failure;
                //    result.Items.ToList().Add(new ServiceResultItem { Messages = new string[] { "Unable to Update finalized Form Design Version" } });
                //}
                //else
                {
                    TextBoxUIElement textBoxElement = this._unitOfWork.RepositoryAsync<TextBoxUIElement>().FindById(uiElementId);


                    List<FormDesignVersionUIElementMap> list = this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>()
                                                                                .Query()
                                                                                .Include(c => c.FormDesignVersion)
                                                                                .Include(c => c.FormDesignVersion.Status)
                                                                                .Filter(c => c.UIElementID == uiElementId && c.FormDesignVersionID != formDesignVersionId)
                                                                                .Get()
                                                                                .ToList();
                    FormDesignVersionUIElementMap currentMap = this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>()
                                                                                .Query()
                                                                                .Include(c => c.FormDesignVersion)
                                                                                .Filter(c => c.FormDesignVersionID == formDesignVersionId && c.UIElementID == uiElementId)
                                                                                .Get()
                                                                                .FirstOrDefault();

                    DataSourceMapping dataSourceMapping = this._unitOfWork.RepositoryAsync<DataSourceMapping>()
                                                                                .Query()
                                                                                .Filter(c => c.UIElementID == uiElementId
                                                                                    && c.FormDesignID == formDesignId
                                                                                    && c.FormDesignVersionID == formDesignVersionId)
                                                                                .Get()
                                                                                .FirstOrDefault();

                    int formDesignID = currentMap.FormDesignVersion.FormDesignID.Value;
                    //check if this element exists is a Finalized Form Design Version(which means a new element 
                    //has to be created for this Form Design Version
                    int finalizedStatusID = Convert.ToInt32(tmg.equinox.domain.entities.Status.Finalized);
                    if (list.Any(c => c.FormDesignVersion.StatusID == finalizedStatusID))
                    {
                        //update effective date of removal for finalized form design versions
                        foreach (var item in list.ToList())
                        {
                            item.EffectiveDateOfRemoval = currentMap.EffectiveDate.Value.AddDays(-1);
                            item.Operation = "D";
                            this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Update(item);
                        }
                        //create new element
                        using (var scope = new TransactionScope())
                        {
                            TextBoxUIElement element = new TextBoxUIElement();
                            element.UIElementName = textBoxElement.UIElementName;
                            element.FormID = formDesignID;
                            element.Sequence = textBoxElement.Sequence;
                            element.ParentUIElementID = textBoxElement.ParentUIElementID;
                            SetTextBoxValues(userName, isEnabled, isVisible, helpText, isLabel, isMultiline, isRequired, uiElementDataTypeId, defaultValue, label, maxLength, sequence, libraryRegexId, true, ref element, viewType, isStandard);
                            element.ParentUIElementID = parentUIElementId;
                            element.ExtendedProperties = extProp;

                            this._unitOfWork.RepositoryAsync<TextBoxUIElement>().Insert(element);
                            this._unitOfWork.Save();

                            //Added newly created UIelement Id to Service Result
                            List<ServiceResultItem> items = new List<ServiceResultItem>();
                            items.Add(new ServiceResultItem() { Messages = new string[] { element.UIElementID.ToString() } });

                            //update DataSourceMapping
                            if (dataSourceMapping != null)
                            {
                                dataSourceMapping.UIElementID = element.UIElementID;
                                this._unitOfWork.RepositoryAsync<DataSourceMapping>().Update(dataSourceMapping);
                            }

                            //add Validator
                            AddValidator(userName, element.UIElementID, isRequired, libraryRegexId > 0 ? true : false, libraryRegexId, null, null, true, false);
                            //add Rules
                            ChangeRules(userName, tenantId, formDesignVersionId, textBoxElement.UIElementID, element.UIElementID, rules, true);
                            //update existing element map from FormDesignVersionUIElementMap to have new UIElementID
                            currentMap.UIElementID = element.UIElementID;
                            currentMap.EffectiveDate = currentMap.FormDesignVersion.EffectiveDate;
                            currentMap.EffectiveDateOfRemoval = null;
                            currentMap.Operation = "U";

                            //Update Alternate Label
                            if (!string.Equals(element.Label, label) || this.isExistsInAlternateLabel(element.UIElementID))
                            {
                                this.UpdateAlternateLabel(tenantId, formDesignId, formDesignVersionId, element.UIElementID, label);
                            }

                            this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Update(currentMap);
                            this._unitOfWork.Save();
                            this.UpdateDocumentRules(uiElementId, currentMap.UIElementID, currentMap.FormDesignVersionID);
                            result.Items = items;
                            result.Result = ServiceResultStatus.Success;
                            scope.Complete();
                        }
                    }
                    else
                    {
                        if (textBoxElement != null)
                        {
                            using (var scope = new TransactionScope())
                            {
                                sequence = sequence == 0 ? textBoxElement.Sequence : sequence;
                                SetTextBoxValues(userName, isEnabled, isVisible, helpText, isLabel, isMultiline, isRequired, uiElementDataTypeId, defaultValue, label, maxLength, sequence, libraryRegexId, false, ref textBoxElement, viewType, isStandard);
                                textBoxElement.ParentUIElementID = parentUIElementId;
                                textBoxElement.ExtendedProperties = extProp;

                                //change Validator
                                AddValidator(userName, uiElementId, isRequired, libraryRegexId > 0 ? true : false, libraryRegexId, null, null, false, false);
                                //change Rules
                                if (modifyRules == true && rules != null)
                                {
                                    ChangeRules(userName, tenantId, formDesignVersionId, uiElementId, uiElementId, rules, false);
                                }
                                else if (modifyRules == true && rules == null)
                                {
                                    IEnumerable<RuleRowModel> currentRules = GetRulesForUIElement(tenantId, formDesignVersionId, uiElementId);
                                    if (currentRules != null || currentRules.Count() > 0)
                                    {
                                        var delRules = from del in currentRules select del;
                                        if (delRules != null)
                                        {
                                            foreach (var delRule in delRules)
                                            {
                                                DeleteRule(delRule);
                                            }
                                        }
                                    }
                                }
                                this._unitOfWork.RepositoryAsync<TextBoxUIElement>().Update(textBoxElement);
                                this._unitOfWork.Save();

                                //Update Alternate Label
                                if (!string.Equals(textBoxElement.Label, label) || this.isExistsInAlternateLabel(textBoxElement.UIElementID))
                                {
                                    this.UpdateAlternateLabel(tenantId, formDesignId, formDesignVersionId, textBoxElement.UIElementID, label);
                                }

                                //Added newly created UIelement Id to Service Result
                                List<ServiceResultItem> items = new List<ServiceResultItem>();
                                items.Add(new ServiceResultItem() { Messages = new string[] { textBoxElement.ParentUIElementID.ToString(), textBoxElement.UIElementID.ToString() } });
                                result.Items = items;
                                result.Result = ServiceResultStatus.Success;
                                scope.Complete();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                result = ex.ExceptionMessages();
            }
            return result;
        }

        public ServiceResult AddTextBox(string userName, int tenantId, int formDesignVersionId, int parentUIElementId, bool isMultiline, bool isBlank, bool isLabel, string label, string helpText, int sequence, string elementType, bool isStandard, int viewType)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                TextBoxUIElement textBoxElement = new TextBoxUIElement();

                FormDesignVersion formDesignVersion = this._unitOfWork.RepositoryAsync<FormDesignVersion>()
                                                                        .FindById(formDesignVersionId);
                var fields = this._unitOfWork.RepositoryAsync<UIElement>()
                                                            .Query()
                                                            .Filter(c => (c.ParentUIElementID == parentUIElementId && c.FormID == formDesignVersion.FormDesignID))
                                                            .Get();

                var uiElementDataTypeId = this._unitOfWork.RepositoryAsync<ApplicationDataType>().GetUIElementDataTypeID(elementType);
                var uiElementTypeId = this._unitOfWork.RepositoryAsync<UIElementType>().GetUIElementTypeID(elementType);

                int sequenceNo = 0;
                if (fields != null && fields.Count() > 0)
                {
                    sequenceNo = fields.Max(c => c.Sequence);
                }
                textBoxElement.AddedBy = userName;
                textBoxElement.AddedDate = DateTime.Now;
                textBoxElement.Enabled = true;                  //Should always be defaulted to true while adding a record.
                textBoxElement.FormID = formDesignVersion.FormDesignID ?? 0;
                textBoxElement.HasCustomRule = false;           //Should always be defaulted to false while adding a record.
                textBoxElement.HelpText = helpText;
                textBoxElement.IsActive = true;                 //Should always be defaulted to true while adding a record.
                textBoxElement.IsContainer = false;             //Since this is a text box, this field should always be set to false
                textBoxElement.IsLabel = isLabel;                 //Since this is a text box, this field should always be set to false
                textBoxElement.IsMultiline = isMultiline;
                textBoxElement.Label = isBlank ? "Blank" : label;
                textBoxElement.GeneratedName = GetGeneratedName(label);
                textBoxElement.MaxLength = 200;                //This is default value. TODO: //MAKE SURE WE CONFIRM THIS FROM SUSHRUT.
                textBoxElement.ParentUIElementID = parentUIElementId;
                textBoxElement.RequiresValidation = false;      //Should always be defaulted to false while adding a record.
                textBoxElement.Sequence = sequenceNo + 1;
                textBoxElement.SpellCheck = false;              //Should always be defaulted to false while adding a record
                textBoxElement.UIElementDataTypeID = uiElementDataTypeId;
                textBoxElement.UIElementName = this.GetUniqueName(formDesignVersion.FormDesignID ?? 0, ElementTypes.TEXTBOX, textBoxElement.UIElementID, parentUIElementId);
                textBoxElement.UIElementTypeID = uiElementTypeId;
                textBoxElement.IsStandard = isStandard;
                textBoxElement.ViewType = viewType;

                textBoxElement.Visible = true;                  //Should always be defaulted to true while adding a record.

                using (var scope = new TransactionScope())
                {
                    this._unitOfWork.RepositoryAsync<TextBoxUIElement>().Insert(textBoxElement);
                    this._unitOfWork.Save();
                    //update the UIElementName
                    textBoxElement.UIElementName = this.GetUniqueName(formDesignVersion.FormDesignID ?? 0, ElementTypes.TEXTBOX, textBoxElement.UIElementID, parentUIElementId);
                    this._unitOfWork.RepositoryAsync<TextBoxUIElement>().Update(textBoxElement);

                    //add FormDesignVersionUIElementMap record
                    FormDesignVersionUIElementMap map = new FormDesignVersionUIElementMap();
                    map.FormDesignVersionID = formDesignVersionId;
                    map.EffectiveDate = formDesignVersion.EffectiveDate;
                    map.UIElementID = textBoxElement.UIElementID;
                    map.Operation = "A";
                    this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Insert(map);
                    this._unitOfWork.Save();


                    scope.Complete();
                    IList<ServiceResultItem> item = new List<ServiceResultItem>();
                    item.Add(new ServiceResultItem { Messages = new string[] { textBoxElement.UIElementID.ToString() } });
                    item.Add(new ServiceResultItem { Messages = new string[] { GetElementType(textBoxElement) } });

                    result.Items = item;
                    result.Result = ServiceResultStatus.Success;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                result = ex.ExceptionMessages();
            }
            return result;
        }

        public ServiceResult AddTextBox(string userName, int tenantId, int formDesignVersionId, int parentUIElementId, bool isMultiline, bool isBlank, bool isLabel, string label, string helpText, int sequence, string elementType, int dataTypeId, bool isRequired, int maxLength, bool isEnable, bool isVisible, int libraryRegexID, List<RuleRowModel> rules, bool modifyRules, string extProp, string defaultValue, int viewType, int sourceUIElementId)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                TextBoxUIElement textBoxElement = new TextBoxUIElement();

                FormDesignVersion formDesignVersion = this._unitOfWork.RepositoryAsync<FormDesignVersion>()
                                                                        .FindById(formDesignVersionId);
                var fields = this._unitOfWork.RepositoryAsync<UIElement>()
                                                            .Query()
                                                            .Filter(c => (c.ParentUIElementID == parentUIElementId && c.FormID == formDesignVersion.FormDesignID))
                                                            .Get();

                var uiElementTypeId = this._unitOfWork.RepositoryAsync<UIElementType>().GetUIElementTypeID(elementType);

                int sequenceNo = 0;
                if (fields != null && fields.Count() > 0 && sequence == 0)
                {
                    sequenceNo = fields.Max(c => c.Sequence);
                }
                textBoxElement.AddedBy = userName;
                textBoxElement.AddedDate = DateTime.Now;
                textBoxElement.Enabled = isEnable;
                textBoxElement.FormID = formDesignVersion.FormDesignID ?? 0;
                textBoxElement.HasCustomRule = false;           //Should always be defaulted to false while adding a record.
                textBoxElement.HelpText = helpText;
                textBoxElement.IsActive = true;                 //Should always be defaulted to true while adding a record.
                textBoxElement.IsContainer = false;             //Since this is a text box, this field should always be set to false
                textBoxElement.IsLabel = isLabel;                 //Since this is a text box, this field should always be set to false
                textBoxElement.IsMultiline = isMultiline;
                textBoxElement.Label = label;
                textBoxElement.GeneratedName = GetGeneratedName(label);
                textBoxElement.MaxLength = maxLength;
                textBoxElement.ParentUIElementID = parentUIElementId;
                textBoxElement.RequiresValidation = isRequired;
                textBoxElement.Sequence = sequence == 0 ? sequenceNo + 1 : sequence;
                textBoxElement.SpellCheck = false;              //Should always be defaulted to false while adding a record
                textBoxElement.UIElementDataTypeID = dataTypeId;
                textBoxElement.UIElementName = this.GetUniqueName(formDesignVersion.FormDesignID ?? 0, ElementTypes.TEXTBOX, textBoxElement.UIElementID, parentUIElementId);
                textBoxElement.UIElementTypeID = uiElementTypeId;
                textBoxElement.Visible = isVisible;
                textBoxElement.ExtendedProperties = extProp;
                textBoxElement.DefaultValue = defaultValue;
                textBoxElement.ViewType = viewType;
                using (var scope = new TransactionScope())
                {
                    this._unitOfWork.RepositoryAsync<TextBoxUIElement>().Insert(textBoxElement);
                    this._unitOfWork.Save();
                    //update the UIElementName
                    textBoxElement.UIElementName = this.GetUniqueName(formDesignVersion.FormDesignID ?? 0, ElementTypes.TEXTBOX, textBoxElement.UIElementID, parentUIElementId);
                    this._unitOfWork.RepositoryAsync<TextBoxUIElement>().Update(textBoxElement);

                    if (isRequired && libraryRegexID > 0)
                    {
                        Validator objValidator = new Validator()
                        {
                            UIElementID = textBoxElement.UIElementID,
                            IsRequired = true,
                            IsLibraryRegex = true,
                            LibraryRegexID = libraryRegexID,
                            AddedBy = userName,
                            AddedDate = DateTime.Now,
                            IsActive = true
                        };
                        this._unitOfWork.RepositoryAsync<Validator>().Insert(objValidator);
                        this._unitOfWork.Save();
                    }

                    if (modifyRules == true && rules != null)
                    {
                        ChangeRules(userName, tenantId, formDesignVersionId, textBoxElement.UIElementID, textBoxElement.UIElementID, rules, false);
                    }

                    //add FormDesignVersionUIElementMap record
                    FormDesignVersionUIElementMap map = new FormDesignVersionUIElementMap();
                    map.FormDesignVersionID = formDesignVersionId;
                    map.EffectiveDate = formDesignVersion.EffectiveDate;
                    map.UIElementID = textBoxElement.UIElementID;
                    map.Operation = "A";
                    this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Insert(map);
                    this._unitOfWork.Save();


                    scope.Complete();
                    IList<ServiceResultItem> item = new List<ServiceResultItem>();
                    item.Add(new ServiceResultItem { Messages = new string[] { textBoxElement.UIElementID.ToString() } });
                    item.Add(new ServiceResultItem { Messages = new string[] { GetElementType(textBoxElement) } });

                    result.Items = item;
                    result.Result = ServiceResultStatus.Success;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                result = ex.ExceptionMessages();
            }
            return result;
        }

        public ServiceResult DeleteTextBox(int tenantId, int formDesignVersionId, int uiElementId)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                ExceptionMessages exceptionMessage = ExceptionMessages.NULL;
                this._unitOfWork.RepositoryAsync<UIElement>().DeleteElement(_unitOfWork, tenantId, uiElementId, formDesignVersionId, out exceptionMessage);
                if (exceptionMessage.ToDescriptionString() == "")
                {
                    result.Result = ServiceResultStatus.Success;
                }
                else
                {
                    result.Result = ServiceResultStatus.Failure;
                    List<ServiceResultItem> items = new List<ServiceResultItem>();
                    items.Add(new ServiceResultItem { Messages = new string[] { exceptionMessage.ToDescriptionString() } });
                    result.Items = items;
                }
            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return result;
        }


        public ServiceResult ChangeControlText(string userName, int tenantId, int formDesignVersionId, int uiElementId, int uiElementTypeId, string elementType)
        {
            ServiceResult result = new ServiceResult();
            List<Validator> validations = new List<Validator>();
            List<PropertyRuleMap> rules = new List<PropertyRuleMap>();

            try
            {
                //check if this form is Finalized already
                //bool isFormDesignVersionFinalized = this._unitOfWork.RepositoryAsync<FormDesignVersion>().IsFormDesignVersionFinalized(formDesignVersionId);

                //if (isFormDesignVersionFinalized)
                //{
                //    result.Result = ServiceResultStatus.Failure;
                //    result.Items.ToList().Add(new ServiceResultItem { Messages = new string[] { "Unable to Update finalized Form Design Version" } });
                //}
                //else
                {
                    TextBoxUIElement textBoxElement = this._unitOfWork.RepositoryAsync<TextBoxUIElement>().FindById(uiElementId);

                    List<DataSourceMapping> dataSourceMapping = this._unitOfWork.RepositoryAsync<DataSourceMapping>()
                                                                                .Query()
                                                                                .Filter(c => c.MappedUIElementID == uiElementId
                                                                                    && c.FormDesignVersionID == formDesignVersionId)
                                                                                .Get()
                                                                                .ToList();

                    if (elementType.ToUpper() == "LABEL")
                    {
                        validations = this._unitOfWork.RepositoryAsync<Validator>()
                                                                                    .Query()
                                                                                    .Filter(c => c.UIElementID == uiElementId)
                                                                                    .Get()
                                                                                    .ToList();

                        rules = this._unitOfWork.RepositoryAsync<PropertyRuleMap>()
                                                               .Query()
                                                               .Filter(c => c.UIElementID == uiElementId)
                                                               .Get()
                                                               .Where(c => c.TargetPropertyID != 3 && c.TargetPropertyID != 4)
                                                               .ToList();
                    }

                    if (textBoxElement != null)
                    {
                        using (var scope = new TransactionScope())
                        {
                            if (dataSourceMapping.Count > 0)
                            {
                                foreach (var dataSourceMap in dataSourceMapping)
                                {
                                    this._unitOfWork.RepositoryAsync<DataSourceMapping>().Delete(dataSourceMap);
                                }
                            }


                            var uiElementDataTypeId = this._unitOfWork.RepositoryAsync<ApplicationDataType>().GetUIElementDataTypeID(elementType);

                            textBoxElement.UIElementDataTypeID = uiElementDataTypeId;
                            textBoxElement.UIElementTypeID = uiElementTypeId;
                            switch (elementType.ToUpper())
                            {
                                case "TEXTBOX":
                                    textBoxElement.IsLabel = false;
                                    textBoxElement.IsMultiline = false;
                                    break;
                                case "MULTILINE TEXTBOX":
                                    textBoxElement.IsLabel = false;
                                    textBoxElement.IsMultiline = true;
                                    break;
                                case "LABEL":
                                    textBoxElement.IsLabel = true;
                                    textBoxElement.IsMultiline = false;
                                    textBoxElement.MaxLength = 200;
                                    textBoxElement.SpellCheck = false;
                                    textBoxElement.AllowGlobalUpdates = false;
                                    textBoxElement.HasCustomRule = false;
                                    textBoxElement.DefaultValue = null;
                                    if (textBoxElement.RequiresValidation)
                                    {
                                        textBoxElement.RequiresValidation = false;
                                        if (validations.Count > 0)
                                        {
                                            foreach (var valid in validations)
                                            {
                                                this._unitOfWork.RepositoryAsync<Validator>().Delete(valid);
                                            }
                                        }
                                    }
                                    if (rules.Count > 0)
                                    {
                                        foreach (var r in rules)
                                        {
                                            foreach (var expression in this._unitOfWork.RepositoryAsync<Expression>().Query().Filter(c => c.RuleID == r.RuleID).Get().OrderByDescending(c => c.ExpressionID).ToList())
                                            {
                                                this._unitOfWork.RepositoryAsync<Expression>().Delete(expression);
                                            }
                                            this._unitOfWork.RepositoryAsync<PropertyRuleMap>().Delete(r.PropertyRuleMapID);
                                            this._unitOfWork.RepositoryAsync<Rule>().Delete(r.RuleID);
                                        }
                                    }
                                    break;
                                case "RICH TEXTBOX":
                                    textBoxElement.MaxLength = 200;
                                    textBoxElement.SpellCheck = false;
                                    textBoxElement.HasCustomRule = false;
                                    textBoxElement.IsLabel = false;
                                    textBoxElement.IsMultiline = false;
                                    break;
                            }


                            this._unitOfWork.RepositoryAsync<TextBoxUIElement>().Update(textBoxElement);
                            this._unitOfWork.Save();

                            //Added newly created UIelement Id to Service Result
                            List<ServiceResultItem> items = new List<ServiceResultItem>();
                            items.Add(new ServiceResultItem() { Messages = new string[] { textBoxElement.ParentUIElementID.ToString(), textBoxElement.UIElementID.ToString() } });
                            result.Items = items;
                            result.Result = ServiceResultStatus.Success;
                            scope.Complete();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                result = ex.ExceptionMessages();
            }
            return result;
        }

        #endregion Text Box Methods
        #endregion Public Methods

        #region Private Methods
        private void SetTextBoxValues(string userName, bool isEnabled, bool isVisible, bool hasCustomRule, string helpText, bool isLabel, bool isMultiline, bool isRequired, int uiElementDataTypeId, string defaultValue, string label, int maxLength, int sequence, bool isSpellCheck, bool allowGlobalUpdates,
            Nullable<bool> isLibraryRegex, string regex, string CustomRegexMessage, bool maskFlag, Nullable<int> libraryRegexId, bool isNew, ref TextBoxUIElement element, bool modifyCustomRules, string customRule, int viewType, bool isStandard, string mdmName)
        {
            element.DefaultValue = defaultValue;
            element.Enabled = isEnabled;
            element.HasCustomRule = hasCustomRule;
            if (modifyCustomRules)
            {
                element.HasCustomRule = hasCustomRule;
                element.CustomRule = customRule;
            }
            element.HelpText = helpText;
            element.IsLabel = isLabel;
            element.IsMultiline = isMultiline;
            //element.Label = label;
            element.GeneratedName = GetGeneratedName(element.Label);
            element.MaxLength = maxLength;
            element.RequiresValidation = isRequired;
            element.SpellCheck = isSpellCheck;
            element.AllowGlobalUpdates = allowGlobalUpdates;
            element.ViewType = viewType;
            element.IsStandard = isStandard;
            element.MDMName = mdmName;
            if (isLabel)
            {
                element.UIElementDataTypeID = 2; // for string
            }
            else
            {
                element.UIElementDataTypeID = uiElementDataTypeId;
            }
            if (isMultiline == true)
            {
                element.UIElementDataTypeID = 2; //for string
            }
            element.Visible = isVisible;
            element.DefaultValue = defaultValue;
            element.UIElementTypeID = element.UIElementTypeID == 0 ? 3 : element.UIElementTypeID;
            if (isLabel == true)
            {
                element.UIElementTypeID = 10;    //10 - for Label
            }
            if (isMultiline == true)
            {
                element.UIElementTypeID = 4;    //4 - for Multiline
            }
            if (isNew == true)
            {
                element.IsActive = true;
                element.AddedBy = userName;
                element.AddedDate = DateTime.Now;
                element.UpdatedBy = null;
                element.UpdatedDate = null;
            }
            else
            {
                element.UpdatedBy = userName;
                element.UpdatedDate = DateTime.Now;
            }
        }

        private void SetTextBoxValues(string userName, bool isEnabled, bool isVisible, string helpText, bool isLabel, bool isMultiline, bool isRequired, int uiElementDataTypeId, string defaultValue, string label, int maxLength, int sequence, Nullable<int> libraryRegexId, bool isNew, ref TextBoxUIElement element, int viewType, bool isStandard)
        {
            element.DefaultValue = defaultValue;
            element.Enabled = isEnabled;
            element.HelpText = helpText;
            element.IsLabel = isLabel;
            element.IsMultiline = isMultiline;
            //element.Label = label;
            element.GeneratedName = GetGeneratedName(element.Label);
            element.MaxLength = maxLength;
            element.RequiresValidation = isRequired;
            element.ViewType = viewType;
            element.IsStandard = isStandard;
            if (isLabel)
            {
                element.UIElementDataTypeID = 2; // for string
            }
            else
            {
                element.UIElementDataTypeID = uiElementDataTypeId;
            }
            if (isMultiline == true)
            {
                element.UIElementDataTypeID = 2; //for string
            }
            element.Visible = isVisible;
            element.DefaultValue = defaultValue;
            element.UIElementTypeID = element.UIElementTypeID == 0 ? 3 : element.UIElementTypeID;
            if (isLabel == true)
            {
                element.UIElementTypeID = 10;    //10 - for Label
            }
            if (isMultiline == true)
            {
                element.UIElementTypeID = 4;    //4 - for Multiline
            }
            if (isNew == true)
            {
                element.IsActive = true;
                element.AddedBy = userName;
                element.AddedDate = DateTime.Now;
                element.UpdatedBy = null;
                element.UpdatedDate = null;
            }
            else
            {
                element.UpdatedBy = userName;
                element.UpdatedDate = DateTime.Now;
            }
        }
        #endregion Private Methods
    }
}
