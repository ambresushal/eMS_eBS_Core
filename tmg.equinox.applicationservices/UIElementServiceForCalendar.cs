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
        #region Calendar Methods
        public CalendarElementModel GetCalendar(int tenantId, int formDesignVersionId, int uiElementId)
        {
            CalendarElementModel calendarElementModel = null;
            try
            {
                CalendarUIElement calendarElement = this._unitOfWork.RepositoryAsync<CalendarUIElement>()
                                                            .Query()
                                                            .Filter(c => c.UIElementID == uiElementId)
                                                            .Get()
                                                            .SingleOrDefault();

                if (calendarElement != null)
                {
                    calendarElementModel = new CalendarElementModel();

                    calendarElementModel.Enabled = calendarElement.Enabled ?? false;
                    calendarElementModel.FormDesignVersionID = calendarElement.FormID;
                    calendarElementModel.HasCustomRule = calendarElement.HasCustomRule ?? false;
                    calendarElementModel.CustomRule = calendarElement.CustomRule;
                    calendarElementModel.HelpText = calendarElement.HelpText;
                    calendarElementModel.ViewType = calendarElement.ViewType;
                    calendarElementModel.IsStandard = calendarElement.IsStandard;
                    calendarElementModel.IsRequired = calendarElement.RequiresValidation;
                    calendarElementModel.Label = this.GetAlternateLabel(formDesignVersionId, uiElementId) ?? calendarElement.Label;
                    calendarElementModel.DefaultDate = calendarElement.DefaultDate;
                    calendarElementModel.MaxDate = calendarElement.MaxDate;
                    calendarElementModel.MinDate = calendarElement.MinDate;
                    calendarElementModel.ParentUIElementID = calendarElement.ParentUIElementID ?? 0;
                    calendarElementModel.Sequence = calendarElement.Sequence;
                    calendarElementModel.TenantID = tenantId;
                    calendarElementModel.UIElementID = calendarElement.UIElementID;
                    calendarElementModel.Visible = calendarElement.Visible ?? false;
                    calendarElementModel.TenantID = tenantId;
                    calendarElementModel.FormDesignVersionID = formDesignVersionId;
                    calendarElementModel.FormDesignID = calendarElement.FormID;
                    calendarElementModel.AllowGlobalUpdates = calendarElement.AllowGlobalUpdates ?? false;
                    calendarElementModel.MDMName = calendarElement.MDMName;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return calendarElementModel;
        }

        public ServiceResult UpdateCalendar(string userName, int tenantId, int formDesignId, int formDesignVersionId, int uiElementId, bool isEnabled,
            bool isVisible, bool hasCustomRule, string helpText, bool isRequired, string label, int sequence,
            Nullable<DateTime> defaultDate, Nullable<DateTime> minDate, Nullable<DateTime> maxDate, IEnumerable<RuleRowModel> rules, bool modifyRules, bool modifyCustomRules, string customRule, bool allowGlobalUpdates, int viewType, bool isStandard, string mdmName)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                CalendarUIElement calendarElement = this._unitOfWork.RepositoryAsync<CalendarUIElement>().Find(uiElementId);
                //bool isFormDesignVersionFinzalized = this._unitOfWork.RepositoryAsync<FormDesignVersion>().IsFormDesignVersionFinalized(formDesignVersionId);

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
                        //to current map effective date - 1
                        foreach (var item in list.ToList())
                        {
                            item.EffectiveDateOfRemoval = currentMap.EffectiveDate.Value.AddDays(-1);
                            item.Operation = "D";
                            this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Update(item);
                        }

                        using (var scope = new TransactionScope())
                        {
                            CalendarUIElement element = new CalendarUIElement();
                            element.UIElementName = calendarElement.UIElementName;
                            element.FormID = formDesignID;
                            element.Sequence = calendarElement.Sequence;
                            element.ParentUIElementID = calendarElement.ParentUIElementID;
                            element.Label = calendarElement.Label;
                            SetCalendarValues(userName, isEnabled, isVisible, hasCustomRule,
                                helpText, calendarElement.UIElementDataTypeID, isRequired, label, sequence, defaultDate, minDate, maxDate, true, ref element, modifyCustomRules,
                                customRule, allowGlobalUpdates, viewType, isStandard, mdmName);
                            //add element to FormDesignVersionUIElementMap
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
                            var newRules = ChangeRules(userName, tenantId, formDesignVersionId, calendarElement.UIElementID, element.UIElementID, rules, true);
                            //Get Copied Rules Mapping
                            List<string> ruleMap = newRules.Select(s => s.SourceRuleID + ":" + s.RuleId).ToList();
                            items.Add(new ServiceResultItem() { Messages = new string[] { string.Join(",", ruleMap.ToArray()) } });

                            //update existing element map from FormDesignVersionUIElementMap to have new UIElementID
                            currentMap.UIElementID = element.UIElementID;
                            currentMap.EffectiveDate = currentMap.EffectiveDate;
                            currentMap.EffectiveDateOfRemoval = null;
                            currentMap.Operation = "U";

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
                        if (calendarElement != null)
                        {
                            sequence = sequence == 0 ? calendarElement.Sequence : sequence;

                            SetCalendarValues(userName, isEnabled, isVisible, hasCustomRule,
                                       helpText, calendarElement.UIElementDataTypeID, isRequired, label, sequence, defaultDate, minDate, maxDate, false, ref calendarElement, modifyCustomRules, customRule, allowGlobalUpdates, viewType, isStandard, mdmName);
                            if (isRequired)
                            {
                                AddValidator(userName, calendarElement.UIElementID, isRequired, null, null, "", "", false);
                            }
                            else
                            {
                                DeleteValidator(calendarElement.UIElementID);
                            }

                            //change Rules
                            if (modifyRules == true && rules != null)
                            {
                                ChangeRules(userName, tenantId, formDesignVersionId, uiElementId, uiElementId, rules, false);
                            }
                            //If all the rules deleted from UI.
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
                            this._unitOfWork.RepositoryAsync<CalendarUIElement>().Update(calendarElement);
                            this._unitOfWork.Save();

                            //Update Alternate Label
                            if (!string.Equals(calendarElement.Label, label) || this.isExistsInAlternateLabel(calendarElement.UIElementID))
                            {
                                this.UpdateAlternateLabel(tenantId, formDesignId, formDesignVersionId, calendarElement.UIElementID, label);
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

        public ServiceResult UpdateCalendar(string userName, int tenantId, int formDesignId, int formDesignVersionId, int uiElementId, bool isEnabled,
            bool isVisible, string helpText, bool isRequired, string label, int sequence, string defaultValue, IEnumerable<RuleRowModel> rules, bool modifyRules, int viewType, int parentUIElementId, string extProp, bool isStandard)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                CalendarUIElement calendarElement = this._unitOfWork.RepositoryAsync<CalendarUIElement>().Find(uiElementId);
                bool isFormDesignVersionFinzalized = this._unitOfWork.RepositoryAsync<FormDesignVersion>().IsFormDesignVersionFinalized(formDesignVersionId);

                DateTime defaultDate;
                DateTime.TryParse(defaultValue, out defaultDate);

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
                        //to current map effective date - 1
                        foreach (var item in list.ToList())
                        {
                            item.EffectiveDateOfRemoval = currentMap.EffectiveDate.Value.AddDays(-1);
                            item.Operation = "D";
                            this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Update(item);
                        }

                        using (var scope = new TransactionScope())
                        {
                            CalendarUIElement element = new CalendarUIElement();
                            element.UIElementName = calendarElement.UIElementName;
                            element.FormID = formDesignID;
                            element.Sequence = calendarElement.Sequence;
                            element.ParentUIElementID = calendarElement.ParentUIElementID;
                            SetCalendarValues(userName, isEnabled, isVisible,
                                helpText, calendarElement.UIElementDataTypeID, isRequired, label, sequence, null, true, ref element,
                                  viewType, isStandard);
                            element.ParentUIElementID = parentUIElementId;
                            element.ExtendedProperties = extProp;

                            //add element to FormDesignVersionUIElementMap
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
                            ChangeRules(userName, tenantId, formDesignVersionId, calendarElement.UIElementID, element.UIElementID, rules, true);

                            //update existing element map from FormDesignVersionUIElementMap to have new UIElementID
                            currentMap.UIElementID = element.UIElementID;
                            currentMap.EffectiveDate = currentMap.FormDesignVersion.EffectiveDate;
                            currentMap.EffectiveDateOfRemoval = null;
                            currentMap.Operation = "U";

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
                        if (calendarElement != null)
                        {
                            sequence = sequence == 0 ? calendarElement.Sequence : sequence;
                            SetCalendarValues(userName, isEnabled, isVisible,
                                       helpText, calendarElement.UIElementDataTypeID, isRequired, label, sequence, null, false, ref calendarElement, viewType, isStandard);
                            calendarElement.ParentUIElementID = parentUIElementId;
                            calendarElement.ExtendedProperties = extProp;

                            if (isRequired)
                            {
                                AddValidator(userName, calendarElement.UIElementID, isRequired, null, null, "", "", false);
                            }
                            else
                            {
                                DeleteValidator(calendarElement.UIElementID);
                            }

                            //change Rules
                            if (modifyRules == true && rules != null)
                            {
                                ChangeRules(userName, tenantId, formDesignVersionId, uiElementId, uiElementId, rules, false);
                            }
                            //If all the rules deleted from UI.
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
                            this._unitOfWork.RepositoryAsync<CalendarUIElement>().Update(calendarElement);
                            this._unitOfWork.Save();

                            //Update Alternate Label
                            if (!string.Equals(calendarElement.Label, label) || this.isExistsInAlternateLabel(calendarElement.UIElementID))
                            {
                                this.UpdateAlternateLabel(tenantId, formDesignId, formDesignVersionId, calendarElement.UIElementID, label);
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

        public ServiceResult AddCalendar(string userName, int tenantId, int formDesignVersionId, int parentUIElementId, string label, string helpText, int sequence, string elementType, bool isEnable, bool isVisible, bool isRequired, List<RuleRowModel> rules, bool modifyRules, string extProp, string defaultValue, int viewType, int sourceUIElementId)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                CalendarUIElement calendarElement = new CalendarUIElement();

                //var uiElementDataTypeId = 

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
                calendarElement.AddedBy = userName;
                calendarElement.AddedDate = DateTime.Now;
                calendarElement.Enabled = isEnable;                  //Should always be defaulted to true while adding a record.
                calendarElement.FormID = formDesignVersion.FormDesignID ?? 0;
                calendarElement.HasCustomRule = false;           //Should always be defaulted to false while adding a record.
                calendarElement.HelpText = helpText;
                calendarElement.IsActive = true;                 //Should always be defaulted to true while adding a record.
                calendarElement.IsContainer = false;             //Since this is a text box, this field should always be set to false
                calendarElement.Label = label;
                calendarElement.GeneratedName = GetGeneratedName(label);
                calendarElement.ParentUIElementID = parentUIElementId;
                calendarElement.RequiresValidation = isRequired;      //Should always be defaulted to false while adding a record.
                calendarElement.Sequence = sequence == 0 ? sequenceNo + 1 : sequence;
                calendarElement.UIElementDataTypeID = uiElementDataTypeId;
                calendarElement.UIElementName = this.GetUniqueName(formDesignVersion.FormDesignID ?? 0, ElementTypes.CALENDAR, calendarElement.UIElementID, parentUIElementId);
                calendarElement.UIElementTypeID = uiElementTypeId;    //6- DropDownListElement as per UI.UIElementType table entries
                calendarElement.Visible = isVisible;                  //Should always be defaulted to true while adding a record.
                calendarElement.ExtendedProperties = extProp;
                calendarElement.ViewType = viewType;
                //DateTime dateValue;
                //if (DateTime.TryParse(defaultValue, out dateValue))
                //{
                //    calendarElement.DefaultDate = string.IsNullOrEmpty(defaultValue) ? null : (DateTime?)dateValue;
                //}

                using (var scope = new TransactionScope())
                {
                    this._unitOfWork.RepositoryAsync<CalendarUIElement>().Insert(calendarElement);
                    this._unitOfWork.Save();

                    calendarElement.UIElementName = this.GetUniqueName(formDesignVersion.FormDesignID ?? 0, ElementTypes.CALENDAR, calendarElement.UIElementID, parentUIElementId);
                    this._unitOfWork.RepositoryAsync<CalendarUIElement>().Update(calendarElement);

                    if (modifyRules == true && rules != null)
                    {
                        ChangeRules(userName, tenantId, formDesignVersionId, calendarElement.UIElementID, calendarElement.UIElementID, rules, false);
                    }

                    //add FormDesignVersionUIElementMap record
                    FormDesignVersionUIElementMap map = new FormDesignVersionUIElementMap();
                    map.FormDesignVersionID = formDesignVersionId;
                    map.EffectiveDate = formDesignVersion.EffectiveDate;
                    map.UIElementID = calendarElement.UIElementID;
                    map.Operation = "A";
                    this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Insert(map);
                    this._unitOfWork.Save();

                    scope.Complete();
                    IList<ServiceResultItem> item = new List<ServiceResultItem>();
                    item.Add(new ServiceResultItem { Messages = new string[] { calendarElement.UIElementID.ToString() } });
                    item.Add(new ServiceResultItem { Messages = new string[] { GetElementType(calendarElement) } });
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

        public ServiceResult AddCalendar(string userName, int tenantId, int formDesignVersionId, int parentUIElementId, string label, string helpText, int sequence, string elementType, bool isStandard, int viewType)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                CalendarUIElement calendarElement = new CalendarUIElement();

                //var uiElementDataTypeId = 

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
                calendarElement.AddedBy = userName;
                calendarElement.AddedDate = DateTime.Now;
                calendarElement.Enabled = true;                  //Should always be defaulted to true while adding a record.
                calendarElement.FormID = formDesignVersion.FormDesignID ?? 0;
                calendarElement.HasCustomRule = false;           //Should always be defaulted to false while adding a record.
                calendarElement.HelpText = helpText;
                calendarElement.IsActive = true;                 //Should always be defaulted to true while adding a record.
                calendarElement.IsContainer = false;             //Since this is a text box, this field should always be set to false
                calendarElement.Label = label;
                calendarElement.GeneratedName = GetGeneratedName(label);
                calendarElement.ParentUIElementID = parentUIElementId;
                calendarElement.RequiresValidation = false;      //Should always be defaulted to false while adding a record.
                calendarElement.Sequence = sequenceNo + 1;
                calendarElement.UIElementDataTypeID = uiElementDataTypeId;
                calendarElement.UIElementName = this.GetUniqueName(formDesignVersion.FormDesignID ?? 0, ElementTypes.CALENDAR, calendarElement.UIElementID, parentUIElementId);
                calendarElement.UIElementTypeID = uiElementTypeId;    //6- DropDownListElement as per UI.UIElementType table entries
                calendarElement.Visible = true;                  //Should always be defaulted to true while adding a record.
                calendarElement.IsStandard = isStandard;
                calendarElement.ViewType = viewType;

                using (var scope = new TransactionScope())
                {
                    this._unitOfWork.RepositoryAsync<CalendarUIElement>().Insert(calendarElement);
                    this._unitOfWork.Save();

                    calendarElement.UIElementName = this.GetUniqueName(formDesignVersion.FormDesignID ?? 0, ElementTypes.CALENDAR, calendarElement.UIElementID, parentUIElementId);
                    this._unitOfWork.RepositoryAsync<CalendarUIElement>().Update(calendarElement);
                    //add FormDesignVersionUIElementMap record
                    FormDesignVersionUIElementMap map = new FormDesignVersionUIElementMap();
                    map.FormDesignVersionID = formDesignVersionId;
                    map.EffectiveDate = formDesignVersion.EffectiveDate;
                    map.UIElementID = calendarElement.UIElementID;
                    map.Operation = "A";
                    this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Insert(map);
                    this._unitOfWork.Save();

                    scope.Complete();
                    IList<ServiceResultItem> item = new List<ServiceResultItem>();
                    item.Add(new ServiceResultItem { Messages = new string[] { calendarElement.UIElementID.ToString() } });
                    item.Add(new ServiceResultItem { Messages = new string[] { GetElementType(calendarElement) } });
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

        public ServiceResult DeleteCalendar(int tenantId, int formDesignVersionId, int uiElementId)
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
        #endregion Calendar Methods
        #endregion Public Methods

        #region Private Methods
        private void SetCalendarValues(string userName, bool isEnabled, bool isVisible, bool hasCustomRule, string helpText, int uiElementDataTypeID, bool isRequired, string label, int sequence, Nullable<DateTime> defaultDate, Nullable<DateTime> minDate, Nullable<DateTime> maxDate, bool isNew, ref CalendarUIElement element, bool modifyCustomRules, string customRule, bool allowGlobalUpdates, int viewType, bool isStandard, string mdmName)
        {
            element.Enabled = isEnabled;
            element.Visible = isVisible;
            if (modifyCustomRules)
            {
                element.HasCustomRule = hasCustomRule;
                element.CustomRule = customRule;
            }
            element.HasCustomRule = hasCustomRule;
            element.HelpText = helpText;
            element.ViewType = viewType;
            element.IsStandard = isStandard;
            element.RequiresValidation = isRequired;
            element.UIElementDataTypeID = uiElementDataTypeID;
            //element.Label = label;
            element.UIElementTypeID = 6;// To Do replace with enum
            element.GeneratedName = GetGeneratedName(element.Label);
            if (defaultDate.HasValue)
            {
                element.DefaultDate = defaultDate;
            }

            element.MaxDate = maxDate;
            element.MinDate = minDate;
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

        private void SetCalendarValues(string userName, bool isEnabled, bool isVisible, string helpText, int uiElementDataTypeID, bool isRequired, string label, int sequence, Nullable<DateTime> defaultDate, bool isNew, ref CalendarUIElement element, int viewType, bool isStandard)
        {
            element.Enabled = isEnabled;
            element.Visible = isVisible;
            element.HelpText = helpText;
            element.ViewType = viewType;
            element.IsStandard = isStandard;
            element.RequiresValidation = isRequired;
            element.UIElementDataTypeID = uiElementDataTypeID;
            //element.Label = label;
            element.UIElementTypeID = 6;// To Do replace with enum
            element.GeneratedName = GetGeneratedName(element.Label);
            if (defaultDate.HasValue)
            {
                element.DefaultDate = defaultDate;
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
