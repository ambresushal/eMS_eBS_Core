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
        #region Private Memebers

        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor

        #endregion Constructor

        #region Public Methods
        #region Check Box Element
        public CheckBoxElementModel GetCheckBox(int tenantId, int formDesignVersionId, int uiElementId)
        {
            CheckBoxElementModel checkBoxElementModel = null;
            try
            {
                CheckBoxUIElement checkBoxElement = this._unitOfWork.RepositoryAsync<CheckBoxUIElement>()
                                                            .Query()
                                                            .Filter(c => c.UIElementID == uiElementId)
                                                            .Get()
                                                            .SingleOrDefault();

                if (checkBoxElement != null)
                {
                    checkBoxElementModel = new CheckBoxElementModel();

                    checkBoxElementModel.Enabled = checkBoxElement.Enabled ?? false;
                    checkBoxElementModel.FormDesignVersionID = checkBoxElement.FormID;
                    checkBoxElementModel.HasCustomRule = checkBoxElement.HasCustomRule ?? false;
                    checkBoxElementModel.CustomRule = checkBoxElement.CustomRule;
                    checkBoxElementModel.HelpText = checkBoxElement.HelpText;
                    checkBoxElementModel.Label = this.GetAlternateLabel(formDesignVersionId, uiElementId) ?? checkBoxElement.Label;
                    checkBoxElementModel.DefaultValue = checkBoxElement.DefaultValue;
                    checkBoxElementModel.ParentUIElementID = checkBoxElement.ParentUIElementID ?? 0;
                    checkBoxElementModel.Sequence = checkBoxElement.Sequence;
                    checkBoxElementModel.UIElementID = checkBoxElement.UIElementID;
                    checkBoxElementModel.Visible = checkBoxElement.Visible ?? false;
                    checkBoxElementModel.OptionLabel = checkBoxElement.OptionLabel;
                    checkBoxElementModel.TenantID = tenantId;
                    checkBoxElementModel.FormDesignVersionID = formDesignVersionId;
                    checkBoxElementModel.FormDesignID = checkBoxElement.FormID;
                    checkBoxElementModel.AllowGlobalUpdates = checkBoxElement.AllowGlobalUpdates ?? false;
                    checkBoxElementModel.ViewType = checkBoxElement.ViewType;
                    checkBoxElementModel.IsStandard = checkBoxElement.IsStandard;
                    checkBoxElementModel.MDMName = checkBoxElement.MDMName;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return checkBoxElementModel;
        }

        public ServiceResult UpdateCheckBox(string userName, int tenantId, int formDesignId, int formDesignVersionId, int uiElementId, bool isEnabled, bool isVisible, bool hasCustomRule, string helpText, bool isRequired, string label, string optionLabel,
            int sequence, bool defaultValue, IEnumerable<RuleRowModel> rules, bool modifyRules, bool modifyCustomRules, string customRule, bool allowGlobalUpdates, int viewType, bool isStandard, string mdmName)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                CheckBoxUIElement checkBoxElement = this._unitOfWork.RepositoryAsync<CheckBoxUIElement>().FindById(uiElementId);

                //bool isFormDesignVersionFinzalized = this._unitOfWork.RepositoryAsync<FormDesignVersion>().IsFormDesignVersionFinalized(formDesignVersionId);
                //bool isUpdate = false;

                //if (isFormDesignVersionFinzalized)
                //{
                //    result.Result = ServiceResultStatus.Failure;
                //    result.Items.ToList().Add(new ServiceResultItem { Messages = new string[] { "Unable to Update finalized Form Design Version" } });
                //}
                //else
                {
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

                        //update effective date of removal 
                        foreach (var item in list.ToList())
                        {
                            item.EffectiveDateOfRemoval = currentMap.EffectiveDate.Value.AddDays(-1);
                            item.Operation = "D";
                            this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Update(item);
                        }


                        CheckBoxUIElement element = new CheckBoxUIElement();
                        element.UIElementName = checkBoxElement.UIElementName;
                        element.FormID = formDesignID;
                        element.Sequence = checkBoxElement.Sequence;
                        element.ParentUIElementID = checkBoxElement.ParentUIElementID;
                        element.Label = checkBoxElement.Label;
                        SetCheckBoxValues(userName, isEnabled, isVisible, hasCustomRule, helpText, defaultValue, label, sequence, optionLabel, true, ref element, modifyCustomRules, customRule, allowGlobalUpdates, viewType, isStandard, mdmName);

                        using (var scope = new TransactionScope())
                        {
                            //add cloned element to FormDesignVersionUIElementMap
                            this._unitOfWork.RepositoryAsync<UIElement>().Insert(element);
                            this._unitOfWork.Save();

                            List<ServiceResultItem> items = new List<ServiceResultItem>();
                            items.Add(new ServiceResultItem() { Messages = new string[] { element.UIElementID.ToString() } });

                            //update DataSourceMapping
                            if (dataSourceMapping != null)
                            {
                                dataSourceMapping.UIElementID = element.UIElementID;
                                this._unitOfWork.RepositoryAsync<DataSourceMapping>().Update(dataSourceMapping);
                            }

                            //add Rules
                            var newRules = ChangeRules(userName, tenantId, formDesignVersionId, checkBoxElement.UIElementID, element.UIElementID, rules, true);
                            //Get Copied Rules Mapping
                            List<string> ruleMap = newRules.Select(s => s.SourceRuleID + ":" + s.RuleId).ToList();
                            items.Add(new ServiceResultItem() { Messages = new string[] { string.Join(",", ruleMap.ToArray()) } });

                            currentMap.UIElementID = element.UIElementID;
                            currentMap.EffectiveDate = currentMap.EffectiveDate;
                            currentMap.Operation = "U";
                            currentMap.EffectiveDateOfRemoval = null;

                            //Update Alternate Label
                            if (!string.Equals(element.Label, label) || this.isExistsInAlternateLabel(element.UIElementID))
                            {
                                this.UpdateAlternateLabel(tenantId, formDesignId, formDesignVersionId, element.UIElementID, label);
                            }

                            //update existing element map from FormDesignVersionUIElementMap to have new UIElementID
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
                        if (checkBoxElement != null)
                        {
                            sequence = sequence == 0 ? checkBoxElement.Sequence : sequence;

                            SetCheckBoxValues(userName, isEnabled, isVisible, hasCustomRule, helpText, defaultValue,
                                label, sequence, optionLabel, false, ref checkBoxElement, modifyCustomRules, customRule, allowGlobalUpdates, viewType, isStandard, mdmName);
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
                            this._unitOfWork.RepositoryAsync<CheckBoxUIElement>().Update(checkBoxElement);
                            this._unitOfWork.Save();

                            //Update Alternate Label
                            if (!string.Equals(checkBoxElement.Label, label) || this.isExistsInAlternateLabel(checkBoxElement.UIElementID))
                            {
                                this.UpdateAlternateLabel(tenantId, formDesignId, formDesignVersionId, checkBoxElement.UIElementID, label);
                            }

                            result.Result = ServiceResultStatus.Success;
                        }
                        else
                        {
                            result.Result = ServiceResultStatus.Failure;
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

        public ServiceResult UpdateCheckBox(string userName, int tenantId, int formDesignId, int formDesignVersionId, int uiElementId, bool isEnabled, bool isVisible, string helpText, bool isRequired, string label, int sequence, bool defaultValue, IEnumerable<RuleRowModel> rules, bool modifyRules, int viewType, int parentUIElementId, string extProp, bool isStandard)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                CheckBoxUIElement checkBoxElement = this._unitOfWork.RepositoryAsync<CheckBoxUIElement>().FindById(uiElementId);

                bool isFormDesignVersionFinzalized = this._unitOfWork.RepositoryAsync<FormDesignVersion>().IsFormDesignVersionFinalized(formDesignVersionId);

                if (isFormDesignVersionFinzalized)
                {
                    result.Result = ServiceResultStatus.Failure;
                    result.Items.ToList().Add(new ServiceResultItem { Messages = new string[] { "Unable to Update finalized Form Design Version" } });
                }
                else
                {
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

                        //update effective date of removal 
                        foreach (var item in list.ToList())
                        {
                            item.EffectiveDateOfRemoval = currentMap.EffectiveDate.Value.AddDays(-1);
                            item.Operation = "D";
                            this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Update(item);
                        }


                        CheckBoxUIElement element = new CheckBoxUIElement();
                        element.UIElementName = checkBoxElement.UIElementName;
                        element.FormID = formDesignID;
                        element.Sequence = checkBoxElement.Sequence;
                        element.ParentUIElementID = checkBoxElement.ParentUIElementID;
                        SetCheckBoxValues(userName, isEnabled, isVisible, helpText, defaultValue, label, sequence, true, ref element, viewType, isStandard);
                        element.ParentUIElementID = parentUIElementId;
                        element.ExtendedProperties = extProp;

                        using (var scope = new TransactionScope())
                        {
                            //add cloned element to FormDesignVersionUIElementMap
                            this._unitOfWork.RepositoryAsync<UIElement>().Insert(element);
                            this._unitOfWork.Save();

                            List<ServiceResultItem> items = new List<ServiceResultItem>();
                            items.Add(new ServiceResultItem() { Messages = new string[] { element.UIElementID.ToString() } });

                            //update DataSourceMapping
                            if (dataSourceMapping != null)
                            {
                                dataSourceMapping.UIElementID = element.UIElementID;
                                this._unitOfWork.RepositoryAsync<DataSourceMapping>().Update(dataSourceMapping);
                            }

                            //add Rules
                            ChangeRules(userName, tenantId, formDesignVersionId, checkBoxElement.UIElementID, element.UIElementID, rules, true);

                            currentMap.UIElementID = element.UIElementID;
                            currentMap.EffectiveDate = currentMap.FormDesignVersion.EffectiveDate;
                            currentMap.Operation = "U";
                            currentMap.EffectiveDateOfRemoval = null;

                            //Update Alternate Label
                            if (!string.Equals(element.Label, label) || this.isExistsInAlternateLabel(element.UIElementID))
                            {
                                this.UpdateAlternateLabel(tenantId, formDesignId, formDesignVersionId, element.UIElementID, label);
                            }

                            //update existing element map from FormDesignVersionUIElementMap to have new UIElementID
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
                        if (checkBoxElement != null)
                        {
                            sequence = sequence == 0 ? checkBoxElement.Sequence : sequence;
                            SetCheckBoxValues(userName, isEnabled, isVisible, helpText, defaultValue, label, sequence, false, ref checkBoxElement, viewType, isStandard);
                            checkBoxElement.ParentUIElementID = parentUIElementId;
                            checkBoxElement.ExtendedProperties = extProp;

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
                            this._unitOfWork.RepositoryAsync<CheckBoxUIElement>().Update(checkBoxElement);
                            this._unitOfWork.Save();

                            //Update Alternate Label
                            if (!string.Equals(checkBoxElement.Label, label) || this.isExistsInAlternateLabel(checkBoxElement.UIElementID))
                            {
                                this.UpdateAlternateLabel(tenantId, formDesignId, formDesignVersionId, checkBoxElement.UIElementID, label);
                            }

                            result.Result = ServiceResultStatus.Success;
                        }
                        else
                        {
                            result.Result = ServiceResultStatus.Failure;
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

        public ServiceResult AddCheckBox(string userName, int tenantId, int formDesignVersionId, int parentUIElementId, string label, string helpText, int sequence, string elementType, bool isStandard, int viewType)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                CheckBoxUIElement checkBoxElement = new CheckBoxUIElement();

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
                checkBoxElement.AddedBy = userName;
                checkBoxElement.AddedDate = DateTime.Now;
                checkBoxElement.Enabled = true;                  //Should always be defaulted to true while adding a record.
                checkBoxElement.FormID = formDesignVersion.FormDesignID ?? 0;
                checkBoxElement.HasCustomRule = false;           //Should always be defaulted to false while adding a record.
                checkBoxElement.HelpText = helpText;
                checkBoxElement.IsActive = true;                 //Should always be defaulted to true while adding a record.
                checkBoxElement.IsContainer = false;             //Since this is a text box, this field should always be set to false
                checkBoxElement.Label = label;
                checkBoxElement.GeneratedName = GetGeneratedName(label);
                checkBoxElement.ParentUIElementID = parentUIElementId;
                checkBoxElement.RequiresValidation = false;      //Should always be defaulted to false while adding a record.
                checkBoxElement.Sequence = sequenceNo + 1;
                checkBoxElement.UIElementDataTypeID = uiElementDataTypeId;
                checkBoxElement.UIElementName = this.GetUniqueName(formDesignVersion.FormDesignID ?? 0, ElementTypes.CHECKBOX, checkBoxElement.UIElementID, parentUIElementId);
                checkBoxElement.UIElementTypeID = uiElementTypeId;   //2- Check box as per UI.UIElementType table entries
                checkBoxElement.Visible = true;                  //Should always be defaulted to true while adding a record.
                checkBoxElement.IsStandard = isStandard;
                checkBoxElement.ViewType = viewType;

                using (var scope = new TransactionScope())
                {
                    this._unitOfWork.RepositoryAsync<CheckBoxUIElement>().Insert(checkBoxElement);
                    this._unitOfWork.Save();

                    checkBoxElement.UIElementName = this.GetUniqueName(formDesignVersion.FormDesignID ?? 0, ElementTypes.CHECKBOX, checkBoxElement.UIElementID, parentUIElementId);
                    this._unitOfWork.RepositoryAsync<CheckBoxUIElement>().Update(checkBoxElement);

                    //add FormDesignVersionUIElementMap record
                    FormDesignVersionUIElementMap map = new FormDesignVersionUIElementMap();
                    map.FormDesignVersionID = formDesignVersionId;
                    map.EffectiveDate = formDesignVersion.EffectiveDate;
                    map.UIElementID = checkBoxElement.UIElementID;
                    map.Operation = "A";
                    this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Insert(map);
                    this._unitOfWork.Save();

                    scope.Complete();
                    IList<ServiceResultItem> item = new List<ServiceResultItem>();
                    item.Add(new ServiceResultItem { Messages = new string[] { checkBoxElement.UIElementID.ToString() } });
                    item.Add(new ServiceResultItem { Messages = new string[] { GetElementType(checkBoxElement) } });
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

        public ServiceResult AddCheckBox(string userName, int tenantId, int formDesignVersionId, int parentUIElementId, string label, string helpText, int sequence, string elementType, bool isEnable, bool isVisible, List<RuleRowModel> rules, bool modifyRules, string extProp, int viewType, int sourceUIElementId)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                CheckBoxUIElement checkBoxElement = new CheckBoxUIElement();

                FormDesignVersion formDesignVersion = this._unitOfWork.RepositoryAsync<FormDesignVersion>()
                                                                        .FindById(formDesignVersionId);
                var fields = this._unitOfWork.RepositoryAsync<UIElement>()
                                            .Query()
                                            .Filter(c => (c.ParentUIElementID == parentUIElementId && c.FormID == formDesignVersion.FormDesignID))
                                            .Get();
                var uiElementDataTypeId = this._unitOfWork.RepositoryAsync<ApplicationDataType>().GetUIElementDataTypeID(elementType);
                var uiElementTypeId = this._unitOfWork.RepositoryAsync<UIElementType>().GetUIElementTypeID(elementType);

                int sequenceNo = 0;
                if (fields != null && fields.Count() > 0 && sequence == 0)
                {
                    sequenceNo = fields.Max(c => c.Sequence);
                }
                checkBoxElement.AddedBy = userName;
                checkBoxElement.AddedDate = DateTime.Now;
                checkBoxElement.Enabled = isEnable;                  //Should always be defaulted to true while adding a record.
                checkBoxElement.FormID = formDesignVersion.FormDesignID ?? 0;
                checkBoxElement.HasCustomRule = false;           //Should always be defaulted to false while adding a record.
                checkBoxElement.HelpText = helpText;
                checkBoxElement.IsActive = true;                 //Should always be defaulted to true while adding a record.
                checkBoxElement.IsContainer = false;             //Since this is a text box, this field should always be set to false
                checkBoxElement.Label = label;
                checkBoxElement.GeneratedName = GetGeneratedName(label);
                checkBoxElement.ParentUIElementID = parentUIElementId;
                checkBoxElement.RequiresValidation = false;      //Should always be defaulted to false while adding a record.
                checkBoxElement.Sequence = sequence == 0 ? sequenceNo + 1 : sequence;
                checkBoxElement.UIElementDataTypeID = uiElementDataTypeId;
                checkBoxElement.UIElementName = this.GetUniqueName(formDesignVersion.FormDesignID ?? 0, ElementTypes.CHECKBOX, checkBoxElement.UIElementID, parentUIElementId);
                checkBoxElement.UIElementTypeID = uiElementTypeId;   //2- Check box as per UI.UIElementType table entries
                checkBoxElement.Visible = isVisible;                  //Should always be defaulted to true while adding a record.
                checkBoxElement.ExtendedProperties = extProp;
                checkBoxElement.ViewType = viewType;
                using (var scope = new TransactionScope())
                {
                    this._unitOfWork.RepositoryAsync<CheckBoxUIElement>().Insert(checkBoxElement);
                    this._unitOfWork.Save();

                    checkBoxElement.UIElementName = this.GetUniqueName(formDesignVersion.FormDesignID ?? 0, ElementTypes.CHECKBOX, checkBoxElement.UIElementID, parentUIElementId);
                    this._unitOfWork.RepositoryAsync<CheckBoxUIElement>().Update(checkBoxElement);

                    if (modifyRules == true && rules != null)
                    {
                        ChangeRules(userName, tenantId, formDesignVersionId, checkBoxElement.UIElementID, checkBoxElement.UIElementID, rules, false);
                    }

                    //add FormDesignVersionUIElementMap record
                    FormDesignVersionUIElementMap map = new FormDesignVersionUIElementMap();
                    map.FormDesignVersionID = formDesignVersionId;
                    map.EffectiveDate = formDesignVersion.EffectiveDate;
                    map.UIElementID = checkBoxElement.UIElementID;
                    map.Operation = "A";
                    this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Insert(map);
                    this._unitOfWork.Save();

                    scope.Complete();
                    IList<ServiceResultItem> item = new List<ServiceResultItem>();
                    item.Add(new ServiceResultItem { Messages = new string[] { checkBoxElement.UIElementID.ToString() } });
                    item.Add(new ServiceResultItem { Messages = new string[] { GetElementType(checkBoxElement) } });
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

        public ServiceResult DeleteCheckBox(int tenantId, int formDesignVersionId, int uiElementId)
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
        #endregion Check Box Element
        #endregion Public Methods

        #region Private Methods
        private void SetCheckBoxValues(string userName, bool isEnabled, bool isVisible, bool hasCustomRule, string helpText, Nullable<bool> defaultValue, string label, int sequence,
            string optionLabel, bool isNew, ref CheckBoxUIElement element, bool modifyCustomRules, string customRule, bool allowGlobalUpdates, int viewType, bool isStandard, string mdmName)
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
            element.OptionLabel = optionLabel;
            //element.Label = label;
            element.GeneratedName = GetGeneratedName(element.Label);
            element.Visible = isVisible;
            element.ViewType = viewType;
            element.IsStandard = isStandard;
            element.UIElementDataTypeID = 4; //for bool
            element.UIElementTypeID = 2;
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
            element.AllowGlobalUpdates = allowGlobalUpdates;
            element.MDMName = mdmName;
        }

        private void SetCheckBoxValues(string userName, bool isEnabled, bool isVisible, string helpText, Nullable<bool> defaultValue, string label, int sequence, bool isNew, ref CheckBoxUIElement element, int viewType, bool isStandard)
        {
            element.DefaultValue = defaultValue;
            element.Enabled = isEnabled;
            element.HelpText = helpText;
            //element.Label = label;
            element.GeneratedName = GetGeneratedName(element.Label);
            element.Visible = isVisible;
            element.ViewType = viewType;
            element.IsStandard = isStandard;
            element.UIElementDataTypeID = 4; //for bool
            element.UIElementTypeID = 2;
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
