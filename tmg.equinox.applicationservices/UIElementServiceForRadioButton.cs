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
        #region Radio Button Element
        public RadioButtonElementModel GetRadioButton(int tenantId, int formDesignVersionId, int uiElementId)
        {
            RadioButtonElementModel radioButtonElementModel = null;
            try
            {
                RadioButtonUIElement radioButtonElement = this._unitOfWork.RepositoryAsync<RadioButtonUIElement>()
                                                            .Query()
                                                            .Filter(c => c.UIElementID == uiElementId)
                                                            .Get()
                                                            .SingleOrDefault();

                if (radioButtonElement != null)
                {
                    radioButtonElementModel = new RadioButtonElementModel();
                    radioButtonElementModel.DefaultValue = radioButtonElement.DefaultValue;// == null ? radioButtonElement.DefaultValue.Value : null;
                    radioButtonElementModel.Enabled = radioButtonElement.Enabled.HasValue ? radioButtonElement.Enabled.Value : false;
                    radioButtonElementModel.FormDesignVersionID = radioButtonElement.FormID;
                    radioButtonElementModel.HasCustomRule = radioButtonElement.HasCustomRule ?? false;
                    radioButtonElementModel.ViewType = radioButtonElement.ViewType;
                    radioButtonElementModel.IsStandard = radioButtonElement.IsStandard;
                    radioButtonElementModel.CustomRule = radioButtonElement.CustomRule;
                    radioButtonElementModel.HelpText = radioButtonElement.HelpText;
                    radioButtonElementModel.Label = this.GetAlternateLabel(formDesignVersionId, uiElementId) ?? radioButtonElement.Label;
                    radioButtonElementModel.OptionLabel = radioButtonElement.OptionLabel;
                    radioButtonElementModel.OptionLabelNo = radioButtonElement.OptionLabelNo;
                    radioButtonElementModel.IsYesNo = radioButtonElement.IsYesNo;
                    radioButtonElementModel.ParentUIElementID = radioButtonElement.ParentUIElementID.HasValue ? radioButtonElement.ParentUIElementID.Value : 0;
                    radioButtonElementModel.IsRequired = radioButtonElement.RequiresValidation;
                    radioButtonElementModel.Sequence = radioButtonElement.Sequence;
                    radioButtonElementModel.TenantID = tenantId;
                    radioButtonElementModel.UIElementID = radioButtonElement.UIElementID;
                    radioButtonElementModel.Visible = radioButtonElement.Visible.HasValue ? radioButtonElement.Visible.Value : false;
                    radioButtonElementModel.TenantID = tenantId;
                    radioButtonElementModel.FormDesignVersionID = formDesignVersionId;
                    radioButtonElementModel.FormDesignID = radioButtonElement.FormID;
                    radioButtonElementModel.AllowGlobalUpdates = radioButtonElement.AllowGlobalUpdates ?? false;
                    radioButtonElementModel.MDMName = radioButtonElement.MDMName;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return radioButtonElementModel;
        }

        public ServiceResult UpdateRadioButton(string userName, int tenantId, int formDesignId, int formDesignVersionId, int uiElementId, bool isEnabled, bool isVisible, bool hasCustomRule, string helpText, bool isRequired, string label, string optionLabel1, string optionLabel2, bool isYesNo, int sequence, bool? defaultValue,
                                                IEnumerable<RuleRowModel> rules, bool modifyRules, bool modifyCustomRules, string customRule, bool allowGlobalUpdates, int viewType, bool isStandard, string mdmName)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                RadioButtonUIElement radioButtonElement = this._unitOfWork.RepositoryAsync<RadioButtonUIElement>().FindById(uiElementId);
                //bool isFormDesignVersionFinalized = this._unitOfWork.RepositoryAsync<FormDesignVersion>().IsFormDesignVersionFinalized(formDesignVersionId);
                bool isUpdate = false;

                //if (isFormDesignVersionFinalized)
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
                        using (var scope = new TransactionScope())
                        {
                            RadioButtonUIElement element = new RadioButtonUIElement();
                            element.UIElementName = radioButtonElement.UIElementName;
                            element.FormID = formDesignID;
                            element.Sequence = radioButtonElement.Sequence;
                            element.ParentUIElementID = radioButtonElement.ParentUIElementID;
                            SetRadioButtonValues(userName, isEnabled, isVisible, hasCustomRule, helpText, isRequired, label,
                                                optionLabel1, optionLabel2, isYesNo, sequence, defaultValue, true, ref element, modifyCustomRules, customRule, allowGlobalUpdates, viewType, isStandard, mdmName);
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

                            if (isRequired)
                            {
                                AddValidator(userName, element.UIElementID, isRequired, null, null, "", "", true);
                            }

                            //add Rules
                            var newRules = ChangeRules(userName, tenantId, formDesignVersionId, radioButtonElement.UIElementID, element.UIElementID, rules, true);
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
                            if (!string.Equals(element.Label, label))
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
                        if (radioButtonElement != null)
                        {
                            sequence = sequence == 0 ? radioButtonElement.Sequence : sequence;

                            SetRadioButtonValues(userName, isEnabled, isVisible, hasCustomRule, helpText, isRequired, label,
                                                optionLabel1, optionLabel2, isYesNo, sequence, defaultValue, false, ref radioButtonElement, modifyCustomRules, customRule, allowGlobalUpdates, viewType, isStandard, mdmName);
                            if (isRequired)
                            {
                                AddValidator(userName, uiElementId, isRequired, null, null, "", "", false);
                            }
                            else
                            {
                                DeleteValidator(uiElementId);
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
                            this._unitOfWork.RepositoryAsync<RadioButtonUIElement>().Update(radioButtonElement);
                            this._unitOfWork.Save();

                            //Update Alternate Label
                            if (!string.Equals(radioButtonElement.Label, label) || this.isExistsInAlternateLabel(radioButtonElement.UIElementID))
                            {
                                this.UpdateAlternateLabel(tenantId, formDesignId, formDesignVersionId, radioButtonElement.UIElementID, label);
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

        public ServiceResult UpdateRadioButton(string userName, int tenantId, int formDesignId, int formDesignVersionId, int uiElementId, bool isEnabled, bool isVisible, string helpText, bool isRequired, string label, string optionLabel1, string optionLabel2, bool isYesNo, int sequence, bool? defaultValue, IEnumerable<RuleRowModel> rules, bool modifyRules, int viewType, int parentUIElementId, string extProp, bool isStandard)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                RadioButtonUIElement radioButtonElement = this._unitOfWork.RepositoryAsync<RadioButtonUIElement>().FindById(uiElementId);
                //bool isFormDesignVersionFinalized = this._unitOfWork.RepositoryAsync<FormDesignVersion>().IsFormDesignVersionFinalized(formDesignVersionId);

                //if (isFormDesignVersionFinalized)
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
                        using (var scope = new TransactionScope())
                        {
                            RadioButtonUIElement element = new RadioButtonUIElement();
                            element.UIElementName = radioButtonElement.UIElementName;
                            element.FormID = formDesignID;
                            element.Sequence = radioButtonElement.Sequence;
                            element.ParentUIElementID = radioButtonElement.ParentUIElementID;
                            SetRadioButtonValues(userName, isEnabled, isVisible, helpText, isRequired, label,
                                                optionLabel1, optionLabel2, isYesNo, sequence, defaultValue, true, ref element, viewType, isStandard);
                            element.ParentUIElementID = parentUIElementId;
                            element.ExtendedProperties = extProp;

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

                            if (isRequired)
                            {
                                AddValidator(userName, element.UIElementID, isRequired, null, null, "", "", true);
                            }

                            //add Rules
                            ChangeRules(userName, tenantId, formDesignVersionId, radioButtonElement.UIElementID, element.UIElementID, rules, true);


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
                        if (radioButtonElement != null)
                        {
                            sequence = sequence == 0 ? radioButtonElement.Sequence : sequence;
                            SetRadioButtonValues(userName, isEnabled, isVisible, helpText, isRequired, label,
                                                optionLabel1, optionLabel2, isYesNo, sequence, defaultValue, false, ref radioButtonElement, viewType, isStandard);
                            radioButtonElement.ParentUIElementID = parentUIElementId;
                            radioButtonElement.ExtendedProperties = extProp;

                            if (isRequired)
                            {
                                AddValidator(userName, uiElementId, isRequired, null, null, "", "", false);
                            }
                            else
                            {
                                DeleteValidator(uiElementId);
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
                            this._unitOfWork.RepositoryAsync<RadioButtonUIElement>().Update(radioButtonElement);
                            this._unitOfWork.Save();

                            //Update Alternate Label
                            if (!string.Equals(radioButtonElement.Label, label) || this.isExistsInAlternateLabel(radioButtonElement.UIElementID))
                            {
                                this.UpdateAlternateLabel(tenantId, formDesignId, formDesignVersionId, radioButtonElement.UIElementID, label);
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


        public ServiceResult AddRadioButton(string userName, int tenantId, int formDesignVersionId, int parentUIElementId, string label, string helpText, int sequence, string elementType, bool isEnable, bool isVisible, bool isRequired, string optionYes, string optionNo, List<RuleRowModel> rules, bool modifyRules, string extProp, int viewType, int sourceUIElementId)
        {
            ServiceResult result = new ServiceResult();
            try
            {
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
                RadioButtonUIElement radioButtonElement = new RadioButtonUIElement();
                radioButtonElement.FormID = formDesignVersion.FormDesignID ?? 0;
                radioButtonElement.ParentUIElementID = parentUIElementId;
                radioButtonElement.UIElementName = this.GetUniqueName(formDesignVersion.FormDesignID ?? 0, ElementTypes.RADIO, radioButtonElement.UIElementID, parentUIElementId);
                radioButtonElement.UIElementDataTypeID = uiElementDataTypeId;
                radioButtonElement.UIElementTypeID = uiElementTypeId;         //TO DO : Fetch from code
                radioButtonElement.Enabled = isEnable;              //Default is set to true
                radioButtonElement.Visible = isVisible;              //Default is set to true
                radioButtonElement.HasCustomRule = false;       //Should always be defaulted to false while adding a record.
                radioButtonElement.HelpText = helpText;
                radioButtonElement.RequiresValidation = isRequired; //Should always be defaulted to false while adding a record.
                radioButtonElement.Label = label;
                radioButtonElement.GeneratedName = GetGeneratedName(label);
                radioButtonElement.OptionLabel = optionYes ?? "Yes";         //Should always be defaulted to Yes while adding a record.
                radioButtonElement.OptionLabelNo = optionNo ?? "No";        //Should always be defaulted to Yes while adding a record.
                radioButtonElement.Sequence = sequence == 0 ? sequenceNo + 1 : sequence;
                radioButtonElement.AddedBy = userName;
                radioButtonElement.AddedDate = DateTime.Now;
                radioButtonElement.ExtendedProperties = extProp;
                radioButtonElement.ViewType = viewType;
                radioButtonElement.IsActive = true;
                using (var scope = new TransactionScope())
                {
                    this._unitOfWork.RepositoryAsync<RadioButtonUIElement>().Insert(radioButtonElement);
                    this._unitOfWork.Save();

                    radioButtonElement.UIElementName = this.GetUniqueName(formDesignVersion.FormDesignID ?? 0, ElementTypes.RADIO, radioButtonElement.UIElementID, parentUIElementId);
                    this._unitOfWork.RepositoryAsync<RadioButtonUIElement>().Update(radioButtonElement);

                    if (modifyRules == true && rules != null)
                    {
                        ChangeRules(userName, tenantId, formDesignVersionId, radioButtonElement.UIElementID, radioButtonElement.UIElementID, rules, false);
                    }

                    //add FormDesignVersionUIElementMap record
                    FormDesignVersionUIElementMap map = new FormDesignVersionUIElementMap();
                    map.FormDesignVersionID = formDesignVersionId;
                    map.EffectiveDate = formDesignVersion.EffectiveDate;
                    map.UIElementID = radioButtonElement.UIElementID;
                    map.Operation = "A";
                    this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Insert(map);
                    this._unitOfWork.Save();

                    scope.Complete();
                    IList<ServiceResultItem> item = new List<ServiceResultItem>();
                    item.Add(new ServiceResultItem { Messages = new string[] { radioButtonElement.UIElementID.ToString() } });
                    item.Add(new ServiceResultItem { Messages = new string[] { GetElementType(radioButtonElement) } });
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

        public ServiceResult AddRadioButton(string userName, int tenantId, int formDesignVersionId, int parentUIElementId, string label, string helpText, int sequence, string elementType, bool isStandard, int viewType)
        {
            ServiceResult result = new ServiceResult();
            try
            {
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
                RadioButtonUIElement radioButtonElement = new RadioButtonUIElement();
                radioButtonElement.FormID = formDesignVersion.FormDesignID ?? 0;
                radioButtonElement.ParentUIElementID = parentUIElementId;
                radioButtonElement.UIElementName = this.GetUniqueName(formDesignVersion.FormDesignID ?? 0, ElementTypes.RADIO, radioButtonElement.UIElementID, parentUIElementId);
                radioButtonElement.UIElementDataTypeID = uiElementDataTypeId;
                radioButtonElement.UIElementTypeID = uiElementTypeId;         //TO DO : Fetch from code
                radioButtonElement.Enabled = true;              //Default is set to true
                radioButtonElement.Visible = true;              //Default is set to true
                radioButtonElement.HasCustomRule = false;       //Should always be defaulted to false while adding a record.
                radioButtonElement.HelpText = helpText;
                radioButtonElement.RequiresValidation = false; //Should always be defaulted to false while adding a record.
                radioButtonElement.Label = label;
                radioButtonElement.GeneratedName = GetGeneratedName(label);
                radioButtonElement.OptionLabel = "Yes";         //Should always be defaulted to Yes while adding a record.
                radioButtonElement.OptionLabelNo = "No";        //Should always be defaulted to Yes while adding a record.
                radioButtonElement.Sequence = sequenceNo + 1;
                radioButtonElement.IsStandard = isStandard;
                radioButtonElement.ViewType = viewType;
                radioButtonElement.AddedBy = userName;
                radioButtonElement.AddedDate = DateTime.Now;
                radioButtonElement.IsActive = true;
                using (var scope = new TransactionScope())
                {
                    this._unitOfWork.RepositoryAsync<RadioButtonUIElement>().Insert(radioButtonElement);
                    this._unitOfWork.Save();

                    radioButtonElement.UIElementName = this.GetUniqueName(formDesignVersion.FormDesignID ?? 0, ElementTypes.RADIO, radioButtonElement.UIElementID, parentUIElementId);
                    this._unitOfWork.RepositoryAsync<RadioButtonUIElement>().Update(radioButtonElement);

                    //add FormDesignVersionUIElementMap record
                    FormDesignVersionUIElementMap map = new FormDesignVersionUIElementMap();
                    map.FormDesignVersionID = formDesignVersionId;
                    map.EffectiveDate = formDesignVersion.EffectiveDate;
                    map.UIElementID = radioButtonElement.UIElementID;
                    map.Operation = "A";
                    this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Insert(map);
                    this._unitOfWork.Save();

                    scope.Complete();
                    IList<ServiceResultItem> item = new List<ServiceResultItem>();
                    item.Add(new ServiceResultItem { Messages = new string[] { radioButtonElement.UIElementID.ToString() } });
                    item.Add(new ServiceResultItem { Messages = new string[] { GetElementType(radioButtonElement) } });
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

        public ServiceResult DeleteRadioButton(int tenantId, int formDesignVersionId, int uiElementId)
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
        #endregion Radio Button Element
        #endregion Public Methods

        #region Private Methods
        private void SetRadioButtonValues(string userName, bool isEnabled, bool isVisible, bool hasCustomRule, string helpText, bool isRequired, string label,
            string optionLabel1, string optionLabel2, bool isYesNo, int sequence, bool? defaultValue, bool isNew, ref RadioButtonUIElement element, bool modifyCustomRules,
            string customRule, bool allowGlobalUpdates, int viewType, bool isStandard, string mdmName)
        {
            element.Enabled = isEnabled;
            element.Visible = isVisible;
            if (modifyCustomRules)
            {
                element.HasCustomRule = hasCustomRule;
                element.CustomRule = customRule;
            }

            element.HasCustomRule = hasCustomRule;
            element.CustomRule = null;
            element.HelpText = helpText;
            element.ViewType = viewType;
            element.UIElementDataTypeID = 4; //for bool
            element.RequiresValidation = isRequired;
            //element.Label = label;
            element.GeneratedName = GetGeneratedName(element.Label);
            element.OptionLabel = optionLabel1;
            element.OptionLabelNo = optionLabel2;
            element.IsYesNo = isYesNo;
            element.DefaultValue = defaultValue;
            element.UIElementTypeID = 1;
            element.IsStandard = isStandard;
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

        private void SetRadioButtonValues(string userName, bool isEnabled, bool isVisible, string helpText, bool isRequired, string label,
            string optionLabel1, string optionLabel2, bool isYesNo, int sequence, bool? defaultValue, bool isNew, ref RadioButtonUIElement element,
            int viewType, bool isStandard)
        {
            element.Enabled = isEnabled;
            element.Visible = isVisible;
            element.CustomRule = null;
            element.HelpText = helpText;
            element.ViewType = viewType;
            element.IsStandard = isStandard;
            element.UIElementDataTypeID = 4; //for bool
            element.RequiresValidation = isRequired;
            //element.Label = label;
            element.GeneratedName = GetGeneratedName(element.Label);
            element.OptionLabel = optionLabel1;
            element.OptionLabelNo = optionLabel2;
            element.IsYesNo = isYesNo;
            element.DefaultValue = defaultValue;
            element.UIElementTypeID = 1;

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
