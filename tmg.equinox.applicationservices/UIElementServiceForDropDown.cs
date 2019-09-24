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
        #region Drop Down methods
        public DropDownElementModel GetDropDown(int tenantId, int formDesignVersionId, int uiElementId)
        {
            DropDownElementModel dropDownElementModel = null;
            try
            {
                DropDownUIElement dropDownElement = this._unitOfWork.RepositoryAsync<DropDownUIElement>()
                                                            .Query()
                                                            .Filter(c => c.UIElementID == uiElementId)
                                                            .Include(c => c.Validators)
                                                            .Get()
                                                            .SingleOrDefault();

                if (dropDownElement != null)
                {
                    dropDownElementModel = new DropDownElementModel();

                    dropDownElementModel.Enabled = dropDownElement.Enabled ?? false;
                    dropDownElementModel.FormDesignVersionID = dropDownElement.FormID;
                    dropDownElementModel.HasCustomRule = dropDownElement.HasCustomRule ?? false;
                    dropDownElementModel.CustomRule = dropDownElement.CustomRule;
                    dropDownElementModel.ViewType = dropDownElement.ViewType;
                    dropDownElementModel.IsStandard = dropDownElement.IsStandard;
                    dropDownElementModel.HelpText = dropDownElement.HelpText;
                    dropDownElementModel.IsRequired = dropDownElement.RequiresValidation;
                    dropDownElementModel.Label = this.GetAlternateLabel(formDesignVersionId, uiElementId) ?? dropDownElement.Label;
                    dropDownElementModel.ParentUIElementID = dropDownElement.ParentUIElementID ?? 0;
                    dropDownElementModel.Sequence = dropDownElement.Sequence;
                    dropDownElementModel.TenantID = tenantId;
                    dropDownElementModel.UIElementID = dropDownElement.UIElementID;
                    dropDownElementModel.Visible = dropDownElement.Visible ?? false;
                    dropDownElementModel.UIElementDataTypeID = dropDownElement.UIElementDataTypeID;
                    dropDownElementModel.Items = (from c in this._unitOfWork.RepositoryAsync<DropDownElementItem>()
                                                               .Query()
                                                               .OrderBy(c => c.OrderBy(d => d.Sequence))
                                                               .Filter(c => c.UIElementID == dropDownElement.UIElementID)
                                                               .Get()
                                                  select new DropDownItemModel
                                                  {
                                                      ItemID = c.ItemID,
                                                      Value = c.Value,
                                                      DisplayText = c.DisplayText,
                                                      Sequence = c.Sequence.HasValue ? c.Sequence.Value : 0
                                                  }).ToList();
                    dropDownElementModel.SelectedValue = dropDownElement.SelectedValue;
                    dropDownElementModel.IsMultiSelect = dropDownElement.IsMultiSelect;
                    dropDownElementModel.TenantID = tenantId;
                    dropDownElementModel.FormDesignVersionID = formDesignVersionId;
                    dropDownElementModel.FormDesignID = dropDownElement.FormID;
                    dropDownElementModel.IsDropDownTextBox = dropDownElement.IsDropDownTextBox ?? false; //Is DropDownText Element 12/16/14
                    dropDownElementModel.IsSortRequired = dropDownElement.IsSortRequired ?? false;
                    dropDownElementModel.AllowGlobalUpdates = dropDownElement.AllowGlobalUpdates ?? false;
                    dropDownElementModel.IsDropDownFilterable = dropDownElement.IsDropDownFilterable;
                    dropDownElementModel.MDMName = dropDownElement.MDMName;
                    if (dropDownElement.Validators != null && dropDownElement.Validators.Count > 0)
                    {
                        List<Validator> vals = dropDownElement.Validators.ToList();
                        dropDownElementModel.IsLibraryRegex = vals[0].IsLibraryRegex;
                        dropDownElementModel.LibraryRegexID = vals[0].LibraryRegexID;
                        //dropDownElementModel.Regex = vals[0].Regex;
                        //dropDownElementModel.CustomRegexMessage = vals[0].Message;
                        //dropDownElementModel.MaskFlag = vals[0].MaskFlag;
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return dropDownElementModel;
        }

        public ServiceResult UpdateDropDown(string userName, int tenantId, int formDesignId, int formDesignVersionId, int uiElementId, bool isEnabled, bool isVisible, bool hasCustomRule, string helpText, bool isRequired, string selectedValue, string label, int sequence, IEnumerable<DropDownItemModel> Items, IEnumerable<RuleRowModel> rules, bool modifyRules, bool modifyCustomRules, string customRule, int uiElementDataTypeID, bool isDropDownTextBox, bool IsSortRequired, Nullable<bool> isLibraryRegex, Nullable<int> libraryRegexId, bool allowGlobalUpdates, bool isDropDownFilterable, int viewType, bool isMultiSelect, bool isStandard, string mdmName)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                //bool isFormDesignVersionFinalized = this._unitOfWork.RepositoryAsync<FormDesignVersion>().IsFormDesignVersionFinalized(formDesignVersionId);

                //if (isFormDesignVersionFinalized)
                //{
                //    result.Result = ServiceResultStatus.Failure;
                //    result.Items.ToList().Add(new ServiceResultItem { Messages = new string[] { "Unable to Update finalized Form Design Version" } });
                //}
                //else
                {
                    DropDownUIElement dropDownElement = this._unitOfWork.RepositoryAsync<DropDownUIElement>()
                                                                                .Query()
                                                                                .Include(c => c.DropDownElementItems)
                                                                                .Filter(c => c.UIElementID == uiElementId)
                                                                                .Get().FirstOrDefault();



                    List<FormDesignVersionUIElementMap> list = this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>()
                                                                                .Query()
                                                                                .Include(c => c.FormDesignVersion)
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

                        using (var scope = new TransactionScope())
                        {
                            sequence = sequence == 0 ? dropDownElement.Sequence : sequence;
                            DropDownUIElement element = new DropDownUIElement();
                            element.UIElementName = dropDownElement.UIElementName;
                            element.Sequence = dropDownElement.Sequence;
                            element.FormID = currentMap.FormDesignVersion.FormDesignID.Value;
                            element.ParentUIElementID = dropDownElement.ParentUIElementID;
                            element.Label = dropDownElement.Label;
                            //add cloned element to FormDesignVersionUIElementMap

                            SetDropDownValues(userName, isEnabled, isVisible, hasCustomRule, helpText, uiElementDataTypeID, dropDownElement.UIElementTypeID, isMultiSelect, isRequired, selectedValue, label, sequence, Items, true, ref element, modifyCustomRules, customRule, isDropDownTextBox, IsSortRequired, allowGlobalUpdates, isDropDownFilterable, viewType, isStandard, mdmName);

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

                            if (isRequired == true && isLibraryRegex == true)
                            {
                                AddValidator(userName, element.UIElementID, isRequired, isLibraryRegex, libraryRegexId, "", "", false);
                            }

                            //add Rules
                            var newRules = ChangeRules(userName, tenantId, formDesignVersionId, dropDownElement.UIElementID, element.UIElementID, rules, true);
                            //Get Copied Rules Mapping
                            List<string> ruleMap = newRules.Select(s => s.SourceRuleID + ":" + s.RuleId).ToList();
                            items.Add(new ServiceResultItem() { Messages = new string[] { string.Join(",", ruleMap.ToArray()) } });

                            currentMap.UIElementID = element.UIElementID;
                            currentMap.EffectiveDate = currentMap.EffectiveDate;
                            currentMap.EffectiveDateOfRemoval = null;
                            currentMap.Operation = "U";

                            //update existing element map from FormDesignVersionUIElementMap to have new UIElementID
                            this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Update(currentMap);
                            this._unitOfWork.Save();

                            //Update Alternate Label
                            if (!string.Equals(element.Label, label) || this.isExistsInAlternateLabel(element.UIElementID))
                            {
                                this.UpdateAlternateLabel(tenantId, formDesignId, formDesignVersionId, element.UIElementID, label);
                            }
                            this.UpdateDocumentRules(uiElementId, currentMap.UIElementID, currentMap.FormDesignVersionID);
                            result.Items = items;
                            result.Result = ServiceResultStatus.Success;

                            scope.Complete();
                        }
                    }
                    else
                    {
                        if (dropDownElement != null)
                        {
                            sequence = sequence == 0 ? dropDownElement.Sequence : sequence;
                            SetDropDownValues(userName, isEnabled, isVisible, hasCustomRule, helpText, uiElementDataTypeID, dropDownElement.UIElementTypeID, isMultiSelect, isRequired, selectedValue, label, sequence, Items, false, ref dropDownElement, modifyCustomRules, customRule, isDropDownTextBox, IsSortRequired, allowGlobalUpdates, isDropDownFilterable, viewType, isStandard, mdmName);

                            if (isRequired == true || isLibraryRegex == true)
                            {
                                AddValidator(userName, dropDownElement.UIElementID, isRequired, isLibraryRegex, libraryRegexId, "", "", false);
                            }
                            else
                            {
                                DeleteValidator(dropDownElement.UIElementID);
                            }
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
                            dropDownElement.UpdatedBy = userName;
                            dropDownElement.UpdatedDate = DateTime.Now;
                            this._unitOfWork.RepositoryAsync<DropDownUIElement>().Update(dropDownElement);
                            this._unitOfWork.Save();

                            //Update Alternate Label
                            if (!string.Equals(dropDownElement.Label, label) || this.isExistsInAlternateLabel(dropDownElement.UIElementID))
                            {
                                this.UpdateAlternateLabel(tenantId, formDesignId, formDesignVersionId, dropDownElement.UIElementID, label);
                            }

                            //Added newly created UIelement Id to Service Result
                            List<ServiceResultItem> items = new List<ServiceResultItem>();
                            items.Add(new ServiceResultItem() { Messages = new string[] { dropDownElement.ParentUIElementID.ToString(), dropDownElement.UIElementID.ToString() } });
                            result.Items = items;
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

        public ServiceResult UpdateDropDown(string userName, int tenantId, int formDesignId, int formDesignVersionId, int uiElementId, bool isEnabled, bool isVisible, string helpText, bool isRequired, string label, int sequence, IEnumerable<DropDownItemModel> Items, IEnumerable<RuleRowModel> rules, bool modifyRules, bool isDropDownTextBox, Nullable<int> libraryRegexId, int viewType, bool isMultiSelect, int parentUIElementId, string extProp, string defaultValue, bool isStandard)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                //bool isFormDesignVersionFinalized = this._unitOfWork.RepositoryAsync<FormDesignVersion>().IsFormDesignVersionFinalized(formDesignVersionId);

                //if (isFormDesignVersionFinalized)
                //{
                //    result.Result = ServiceResultStatus.Failure;
                //    result.Items.ToList().Add(new ServiceResultItem { Messages = new string[] { "Unable to Update finalized Form Design Version" } });
                //}
                //else
                {
                    DropDownUIElement dropDownElement = this._unitOfWork.RepositoryAsync<DropDownUIElement>()
                                                                                .Query()
                                                                                .Include(c => c.DropDownElementItems)
                                                                                .Filter(c => c.UIElementID == uiElementId)
                                                                                .Get().FirstOrDefault();



                    List<FormDesignVersionUIElementMap> list = this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>()
                                                                                .Query()
                                                                                .Include(c => c.FormDesignVersion)
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

                        using (var scope = new TransactionScope())
                        {
                            sequence = sequence == 0 ? dropDownElement.Sequence : sequence;
                            DropDownUIElement element = new DropDownUIElement();
                            element.UIElementName = dropDownElement.UIElementName;
                            element.Sequence = dropDownElement.Sequence;
                            element.FormID = currentMap.FormDesignVersion.FormDesignID.Value;
                            element.ParentUIElementID = dropDownElement.ParentUIElementID;
                            element.ExtendedProperties = extProp;
                            //add cloned element to FormDesignVersionUIElementMap

                            SetDropDownValues(userName, isEnabled, isVisible, helpText, dropDownElement.UIElementTypeID, isMultiSelect, isRequired, label, sequence, Items, true, ref element, isDropDownTextBox, viewType, isStandard);
                            element.ParentUIElementID = parentUIElementId;
                            element.SelectedValue = defaultValue;

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

                            if (isRequired == true && libraryRegexId > 0)
                            {
                                AddValidator(userName, element.UIElementID, isRequired, true, libraryRegexId, "", "", false);
                            }

                            //add Rules
                            ChangeRules(userName, tenantId, formDesignVersionId, dropDownElement.UIElementID, element.UIElementID, rules, true);

                            currentMap.UIElementID = element.UIElementID;
                            currentMap.EffectiveDate = currentMap.FormDesignVersion.EffectiveDate;
                            currentMap.EffectiveDateOfRemoval = null;
                            currentMap.Operation = "U";

                            //update existing element map from FormDesignVersionUIElementMap to have new UIElementID
                            this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Update(currentMap);
                            this._unitOfWork.Save();

                            //Update Alternate Label
                            if (!string.Equals(element.Label, label) || this.isExistsInAlternateLabel(element.UIElementID))
                            {
                                this.UpdateAlternateLabel(tenantId, formDesignId, formDesignVersionId, element.UIElementID, label);
                            }
                            this.UpdateDocumentRules(uiElementId, currentMap.UIElementID, currentMap.FormDesignVersionID);
                            result.Items = items;
                            result.Result = ServiceResultStatus.Success;

                            scope.Complete();
                        }
                    }
                    else
                    {
                        if (dropDownElement != null)
                        {
                            sequence = sequence == 0 ? dropDownElement.Sequence : sequence;
                            SetDropDownValues(userName, isEnabled, isVisible, helpText, dropDownElement.UIElementTypeID, isMultiSelect, isRequired, label, sequence, Items, false, ref dropDownElement, isDropDownTextBox, viewType, isStandard);
                            dropDownElement.ParentUIElementID = parentUIElementId;
                            dropDownElement.ExtendedProperties = extProp;
                            dropDownElement.SelectedValue = defaultValue;

                            if (isRequired == true || libraryRegexId > 0)
                            {
                                AddValidator(userName, dropDownElement.UIElementID, isRequired, true, libraryRegexId, "", "", false);
                            }
                            else
                            {
                                DeleteValidator(dropDownElement.UIElementID);
                            }
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
                            dropDownElement.UpdatedBy = userName;
                            dropDownElement.UpdatedDate = DateTime.Now;
                            this._unitOfWork.RepositoryAsync<DropDownUIElement>().Update(dropDownElement);
                            this._unitOfWork.Save();

                            //Update Alternate Label
                            if (!string.Equals(dropDownElement.Label, label) || this.isExistsInAlternateLabel(dropDownElement.UIElementID))
                            {
                                this.UpdateAlternateLabel(tenantId, formDesignId, formDesignVersionId, dropDownElement.UIElementID, label);
                            }

                            //Added newly created UIelement Id to Service Result
                            List<ServiceResultItem> items = new List<ServiceResultItem>();
                            items.Add(new ServiceResultItem() { Messages = new string[] { dropDownElement.ParentUIElementID.ToString(), dropDownElement.UIElementID.ToString() } });
                            result.Items = items;
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

        public ServiceResult AddDropDown(string userName, int tenantId, int formDesignVersionId, int parentUIElementId, string label, string helpText, int sequence, string elementType, bool isDropDownTextBox, bool isStandard, int viewType)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                DropDownUIElement dropDownElement = new DropDownUIElement();

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
                dropDownElement.AddedBy = userName;
                dropDownElement.AddedDate = DateTime.Now;
                dropDownElement.Enabled = true;                  //Should always be defaulted to true while adding a record.
                dropDownElement.FormID = formDesignVersion.FormDesignID ?? 0;
                dropDownElement.HasCustomRule = false;           //Should always be defaulted to false while adding a record.
                dropDownElement.HelpText = helpText;
                dropDownElement.IsActive = true;                 //Should always be defaulted to true while adding a record.
                dropDownElement.IsContainer = false;             //Since this is a text box, this field should always be set to false
                dropDownElement.Label = label;
                dropDownElement.GeneratedName = GetGeneratedName(label);
                dropDownElement.ParentUIElementID = parentUIElementId;
                dropDownElement.RequiresValidation = false;      //Should always be defaulted to false while adding a record.
                dropDownElement.Sequence = sequenceNo + 1;
                dropDownElement.UIElementDataTypeID = uiElementDataTypeId;
                dropDownElement.UIElementName = this.GetUniqueName(formDesignVersion.FormDesignID ?? 0, ElementTypes.DROPDOWN, dropDownElement.UIElementID, parentUIElementId);
                dropDownElement.UIElementTypeID = uiElementTypeId;    //5- DropDownListElement as per UI.UIElementType table entries
                dropDownElement.Visible = true;                  //Should always be defaulted to true while adding a record.
                dropDownElement.IsDropDownTextBox = isDropDownTextBox;
                dropDownElement.IsStandard = isStandard;
                dropDownElement.ViewType = viewType;
                //dropDownElement.IsSortRequired = isSortRequired;

                using (var scope = new TransactionScope())
                {
                    this._unitOfWork.RepositoryAsync<DropDownUIElement>().Insert(dropDownElement);
                    this._unitOfWork.Save();

                    dropDownElement.UIElementName = this.GetUniqueName(formDesignVersion.FormDesignID ?? 0, ElementTypes.DROPDOWN, dropDownElement.UIElementID, parentUIElementId);
                    this._unitOfWork.RepositoryAsync<DropDownUIElement>().Update(dropDownElement);

                    //add FormDesignVersionUIElementMap record
                    FormDesignVersionUIElementMap map = new FormDesignVersionUIElementMap();
                    map.FormDesignVersionID = formDesignVersionId;
                    map.EffectiveDate = formDesignVersion.EffectiveDate;
                    map.UIElementID = dropDownElement.UIElementID;
                    map.Operation = "A";
                    this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Insert(map);
                    this._unitOfWork.Save();

                    scope.Complete();
                    IList<ServiceResultItem> item = new List<ServiceResultItem>();
                    item.Add(new ServiceResultItem { Messages = new string[] { dropDownElement.UIElementID.ToString() } });
                    item.Add(new ServiceResultItem { Messages = new string[] { GetElementType(dropDownElement) } });
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

        public ServiceResult AddDropDown(string userName, int tenantId, int formDesignVersionId, int parentUIElementId, string label, string helpText, int sequence, string elementType, bool isDropDownTextBox, bool isEnable, bool isVisible, bool isRequired, bool isMultiselect, List<DropDownItemModel> items, List<RuleRowModel> rules, bool modifyRules, string extProp, string defaultValue, int viewType, int sourceUIElementId)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                DropDownUIElement dropDownElement = new DropDownUIElement();

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
                dropDownElement.AddedBy = userName;
                dropDownElement.AddedDate = DateTime.Now;
                dropDownElement.Enabled = isEnable;                  //Should always be defaulted to true while adding a record.
                dropDownElement.FormID = formDesignVersion.FormDesignID ?? 0;
                dropDownElement.HasCustomRule = false;           //Should always be defaulted to false while adding a record.
                dropDownElement.HelpText = helpText;
                dropDownElement.IsActive = true;                 //Should always be defaulted to true while adding a record.
                dropDownElement.IsContainer = false;             //Since this is a text box, this field should always be set to false
                dropDownElement.Label = label;
                dropDownElement.GeneratedName = GetGeneratedName(label);
                dropDownElement.ParentUIElementID = parentUIElementId;
                dropDownElement.RequiresValidation = isRequired;      //Should always be defaulted to false while adding a record.
                dropDownElement.Sequence = sequence == 0 ? sequenceNo + 1 : sequence;
                dropDownElement.UIElementDataTypeID = uiElementDataTypeId;
                dropDownElement.UIElementName = this.GetUniqueName(formDesignVersion.FormDesignID ?? 0, ElementTypes.DROPDOWN, dropDownElement.UIElementID, parentUIElementId);
                dropDownElement.UIElementTypeID = uiElementTypeId;    //5- DropDownListElement as per UI.UIElementType table entries
                dropDownElement.Visible = isVisible;                  //Should always be defaulted to true while adding a record.
                dropDownElement.IsDropDownTextBox = isDropDownTextBox;
                dropDownElement.IsMultiSelect = isMultiselect;
                //dropDownElement.IsSortRequired = isSortRequired;
                dropDownElement.ExtendedProperties = extProp;
                dropDownElement.ViewType = viewType;
                dropDownElement.SelectedValue = defaultValue;

                using (var scope = new TransactionScope())
                {
                    this._unitOfWork.RepositoryAsync<DropDownUIElement>().Insert(dropDownElement);
                    this._unitOfWork.Save();

                    dropDownElement.UIElementName = this.GetUniqueName(formDesignVersion.FormDesignID ?? 0, ElementTypes.DROPDOWN, dropDownElement.UIElementID, parentUIElementId);
                    this._unitOfWork.RepositoryAsync<DropDownUIElement>().Update(dropDownElement);

                    //add Dropdown Item

                    if (items != null && items.Count() > 0)
                    {
                        foreach (var obj in items)
                        {
                            DropDownElementItem newItem = new DropDownElementItem();
                            newItem.UIElementID = dropDownElement.UIElementID;
                            newItem.Sequence = obj.Sequence;
                            newItem.Value = obj.Value;
                            newItem.DisplayText = obj.DisplayText;
                            newItem.AddedBy = userName;
                            newItem.AddedDate = DateTime.Now;
                            this._unitOfWork.RepositoryAsync<DropDownElementItem>().Insert(newItem);
                        }
                        this._unitOfWork.Save();
                    }

                    if (modifyRules == true && rules != null)
                    {
                        ChangeRules(userName, tenantId, formDesignVersionId, dropDownElement.UIElementID, dropDownElement.UIElementID, rules, false);
                    }

                    //add FormDesignVersionUIElementMap record
                    FormDesignVersionUIElementMap map = new FormDesignVersionUIElementMap();
                    map.FormDesignVersionID = formDesignVersionId;
                    map.EffectiveDate = formDesignVersion.EffectiveDate;
                    map.UIElementID = dropDownElement.UIElementID;
                    map.Operation = "A";
                    this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Insert(map);
                    this._unitOfWork.Save();

                    scope.Complete();
                    IList<ServiceResultItem> item = new List<ServiceResultItem>();
                    item.Add(new ServiceResultItem { Messages = new string[] { dropDownElement.UIElementID.ToString() } });
                    item.Add(new ServiceResultItem { Messages = new string[] { GetElementType(dropDownElement) } });
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

        public ServiceResult DeleteDropDown(int tenantId, int formDesignVersionId, int uiElementId)
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


        public ServiceResult ChangeControlDrop(string userName, int tenantId, int formDesignVersionId, int uiElementId, int uiElementTypeId, string elementType, bool isDropDownTextBox)
        {
            ServiceResult result = new ServiceResult();
            List<Validator> validations = new List<Validator>();

            try
            {
                //check if this form is Finalized already
                bool isFormDesignVersionFinalized = this._unitOfWork.RepositoryAsync<FormDesignVersion>().IsFormDesignVersionFinalized(formDesignVersionId);

                if (isFormDesignVersionFinalized)
                {
                    result.Result = ServiceResultStatus.Failure;
                    result.Items.ToList().Add(new ServiceResultItem { Messages = new string[] { "Unable to Update finalized Form Design Version" } });
                }
                else
                {
                    DropDownUIElement dropDownElement = this._unitOfWork.RepositoryAsync<DropDownUIElement>().FindById(uiElementId);

                    List<DataSourceMapping> dataSourceMapping = this._unitOfWork.RepositoryAsync<DataSourceMapping>()
                                                                                .Query()
                                                                                .Filter(c => c.MappedUIElementID == uiElementId
                                                                                    && c.FormDesignVersionID == formDesignVersionId)
                                                                                .Get()
                                                                                .ToList();

                    if (elementType.ToUpper() == "DROPDOWN LIST")
                    {
                        validations = this._unitOfWork.RepositoryAsync<Validator>()
                                                                                    .Query()
                                                                                    .Filter(c => c.UIElementID == uiElementId)
                                                                                    .Get()
                                                                                    .ToList();

                    }

                    if (dropDownElement != null)
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

                            dropDownElement.UIElementDataTypeID = uiElementDataTypeId;
                            dropDownElement.UIElementTypeID = uiElementTypeId;
                            dropDownElement.IsDropDownTextBox = isDropDownTextBox;
                            switch (elementType.ToUpper())
                            {
                                case "DROPDOWN LIST":

                                    if (dropDownElement.RequiresValidation)
                                    {
                                        if (validations.Count > 0)
                                        {
                                            foreach (var valid in validations)
                                            {
                                                valid.IsLibraryRegex = null;
                                                valid.LibraryRegexID = null;
                                                this._unitOfWork.RepositoryAsync<Validator>().Update(valid);
                                            }
                                        }
                                    }
                                    break;
                            }


                            this._unitOfWork.RepositoryAsync<DropDownUIElement>().Update(dropDownElement);
                            this._unitOfWork.Save();

                            //Added newly created UIelement Id to Service Result
                            List<ServiceResultItem> items = new List<ServiceResultItem>();
                            items.Add(new ServiceResultItem() { Messages = new string[] { dropDownElement.ParentUIElementID.ToString(), dropDownElement.UIElementID.ToString() } });
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

        #endregion Drop Down methods
        #endregion Public Methods

        #region Private Methods
        private void SetDropDownValues(string userName, bool isEnabled, bool isVisible, bool hasCustomRule, string helpText, int uiElementDataTypeId, int uiElementTypeID, bool isMultiSelect, bool isRequired, string selectedValue, string label, int sequence, IEnumerable<DropDownItemModel> Items, bool isNew, ref DropDownUIElement element, bool modifyCustomRules, string customRule, bool isDropDownTextBox, bool isSortRequired, bool allowGlobalUpdates, bool isDropDownFilterable, int viewType, bool isStandard, string mdmName)
        {
            element.UIElementTypeID = uiElementTypeID;
            element.UIElementDataTypeID = uiElementDataTypeId;
            element.IsMultiSelect = isMultiSelect;
            element.Enabled = isEnabled;
            element.ViewType = viewType;
            element.IsStandard = isStandard;
            element.Visible = isVisible;
            element.IsDropDownTextBox = isDropDownTextBox;
            element.IsDropDownFilterable = isDropDownFilterable;
            element.IsSortRequired = isSortRequired;
            element.AllowGlobalUpdates = allowGlobalUpdates;
            element.MDMName = mdmName;
            if (isDropDownTextBox == true)
            {
                element.UIElementDataTypeID = 2;
                element.UIElementTypeID = 12;       //for DropDown TextBox
            }
            else
            {
                element.UIElementTypeID = 5;
            }

            if (modifyCustomRules)
            {
                element.HasCustomRule = hasCustomRule;
                element.CustomRule = customRule;
            }
            element.HasCustomRule = hasCustomRule;
            element.HelpText = helpText;
            element.RequiresValidation = isRequired;

            element.SelectedValue = selectedValue;
            //element.Label = label;
            element.GeneratedName = GetGeneratedName(element.Label);
            if (isNew == true)
            {
                if (Items != null && Items.Count() > 0)
                {
                    foreach (var item in Items)
                    {
                        DropDownElementItem newItem = new DropDownElementItem();
                        newItem.UIElementID = element.UIElementID;
                        newItem.Sequence = item.Sequence;
                        newItem.Value = item.Value;
                        newItem.DisplayText = item.DisplayText;
                        newItem.AddedBy = userName;
                        newItem.AddedDate = DateTime.Now;
                        element.DropDownElementItems.Add(newItem);
                    }
                }
                element.IsActive = true;
                element.AddedBy = userName;
                element.AddedDate = DateTime.Now;
                element.UpdatedBy = null;
                element.UpdatedDate = null;
            }
            else
            {
                //foreach (var elem in element.DropDownElementItems)
                //{
                //    DropDownItemModel ddItem = Items.Where(c => c.ItemID == elem.ItemID).FirstOrDefault();
                //    if (ddItem == null)
                //    {
                //        this._unitOfWork.Repository<DropDownElementItem>().Delete(elem);
                //    }
                //}

                //Above iteration was throwing error, becuase the delete operation was performed on the same list.
                //Following is the way out for the above code.
                var ddElementItems = element.DropDownElementItems;
                //if condition for count 1 because it was throwing error while deleting last item in the dropdownlist.
                if (ddElementItems.Count == 1)
                {
                    if (Items == null)
                    {
                        this._unitOfWork.RepositoryAsync<DropDownElementItem>().Delete(ddElementItems.ElementAt(0));
                    }
                }
                else
                {
                    for (int i = ddElementItems.Count() - 1; i >= 0; i--)
                    {
                        if (Items == null)
                        {
                            this._unitOfWork.RepositoryAsync<DropDownElementItem>().Delete(ddElementItems.ElementAt(0));
                        }
                        else
                        {
                            DropDownItemModel ddItem = Items.Where(c => c.ItemID == ddElementItems.ElementAt(i).ItemID).FirstOrDefault();
                            if (ddItem == null)
                            {
                                this._unitOfWork.RepositoryAsync<DropDownElementItem>().Delete(ddElementItems.ElementAt(i));
                            }
                        }
                    }
                }

                if (Items != null && Items.Count() > 0)
                {
                    foreach (var item in Items)
                    {
                        if (item.ItemID > 0)
                        {
                            DropDownElementItem ddItem = element.DropDownElementItems.Where(c => c.ItemID == item.ItemID).FirstOrDefault();
                            ddItem.UpdatedBy = userName;
                            ddItem.UpdatedDate = DateTime.Now;
                            ddItem.DisplayText = item.DisplayText;
                            ddItem.Value = item.Value;
                            ddItem.Sequence = item.Sequence;
                            this._unitOfWork.RepositoryAsync<DropDownElementItem>().Update(ddItem);
                        }
                        else
                        {
                            DropDownElementItem newItem = new DropDownElementItem();
                            newItem.UIElementID = element.UIElementID;
                            newItem.Sequence = item.Sequence;
                            newItem.Value = item.Value;
                            newItem.DisplayText = item.DisplayText;
                            newItem.AddedBy = userName;
                            newItem.AddedDate = DateTime.Now;
                            this._unitOfWork.RepositoryAsync<DropDownElementItem>().Insert(newItem);
                        }
                    }
                }

                element.UpdatedBy = userName;
                element.UpdatedDate = DateTime.Now;
            }
        }

        private void SetDropDownValues(string userName, bool isEnabled, bool isVisible, string helpText, int uiElementTypeID, bool isMultiSelect, bool isRequired, string label, int sequence, IEnumerable<DropDownItemModel> Items, bool isNew, ref DropDownUIElement element, bool isDropDownTextBox, int viewType, bool isStandard)
        {
            element.UIElementTypeID = uiElementTypeID;
            element.IsMultiSelect = isMultiSelect;
            element.Enabled = isEnabled;
            element.ViewType = viewType;
            element.IsStandard = isStandard;
            element.Visible = isVisible;
            element.IsDropDownTextBox = isDropDownTextBox;
            if (isDropDownTextBox == true)
            {
                element.UIElementDataTypeID = 2;
                element.UIElementTypeID = 12;       //for DropDown TextBox
            }
            else
            {
                element.UIElementTypeID = 5;
            }

            element.HelpText = helpText;
            element.RequiresValidation = isRequired;

            //element.Label = label;
            element.GeneratedName = GetGeneratedName(element.Label);
            if (isNew == true)
            {
                if (Items != null && Items.Count() > 0)
                {
                    foreach (var item in Items)
                    {
                        DropDownElementItem newItem = new DropDownElementItem();
                        newItem.UIElementID = element.UIElementID;
                        newItem.Sequence = item.Sequence;
                        newItem.Value = item.Value;
                        newItem.DisplayText = item.DisplayText;
                        newItem.AddedBy = userName;
                        newItem.AddedDate = DateTime.Now;
                        element.DropDownElementItems.Add(newItem);
                    }
                }
                element.IsActive = true;
                element.AddedBy = userName;
                element.AddedDate = DateTime.Now;
                element.UpdatedBy = null;
                element.UpdatedDate = null;
            }
            else
            {
                //Above iteration was throwing error, becuase the delete operation was performed on the same list.
                //Following is the way out for the above code.
                var ddElementItems = element.DropDownElementItems;
                //if condition for count 1 because it was throwing error while deleting last item in the dropdownlist.
                if (ddElementItems.Count == 1)
                {
                    if (Items == null)
                    {
                        this._unitOfWork.RepositoryAsync<DropDownElementItem>().Delete(ddElementItems.ElementAt(0));
                    }
                }
                else
                {
                    for (int i = ddElementItems.Count() - 1; i >= 0; i--)
                    {
                        if (Items == null)
                        {
                            this._unitOfWork.RepositoryAsync<DropDownElementItem>().Delete(ddElementItems.ElementAt(0));
                        }
                        else
                        {
                            DropDownItemModel ddItem = Items.Where(c => c.ItemID == ddElementItems.ElementAt(i).ItemID).FirstOrDefault();
                            if (ddItem == null)
                            {
                                this._unitOfWork.RepositoryAsync<DropDownElementItem>().Delete(ddElementItems.ElementAt(i));
                            }
                        }
                    }
                }

                if (Items != null && Items.Count() > 0)
                {
                    foreach (var item in Items)
                    {
                        if (item.ItemID > 0)
                        {
                            DropDownElementItem ddItem = element.DropDownElementItems.Where(c => c.ItemID == item.ItemID).FirstOrDefault();
                            ddItem.UpdatedBy = userName;
                            ddItem.UpdatedDate = DateTime.Now;
                            ddItem.DisplayText = item.DisplayText;
                            ddItem.Value = item.Value;
                            ddItem.Sequence = item.Sequence;
                            this._unitOfWork.RepositoryAsync<DropDownElementItem>().Update(ddItem);
                        }
                        else
                        {
                            DropDownElementItem newItem = new DropDownElementItem();
                            newItem.UIElementID = element.UIElementID;
                            newItem.Sequence = item.Sequence;
                            newItem.Value = item.Value;
                            newItem.DisplayText = item.DisplayText;
                            newItem.AddedBy = userName;
                            newItem.AddedDate = DateTime.Now;
                            this._unitOfWork.RepositoryAsync<DropDownElementItem>().Insert(newItem);
                        }
                    }
                }

                element.UpdatedBy = userName;
                element.UpdatedDate = DateTime.Now;
            }
        }
        #endregion Private Methods
    }
}
