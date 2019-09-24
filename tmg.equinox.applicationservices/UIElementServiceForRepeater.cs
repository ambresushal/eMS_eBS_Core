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
        #region Repeater Element

        public bool CheckIsRepeaterColumnKeyElement(int uiElementID)
        {
            bool isKeyRepeaterColumn = false;
            try
            {
                isKeyRepeaterColumn = this._unitOfWork.RepositoryAsync<RepeaterKeyUIElement>()
                                    .Query()
                                    .Filter(c => c.UIElementID == uiElementID)
                                    .Get()
                                    .Any();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return isKeyRepeaterColumn;
        }

        public RepeaterElementModel GetRepeater(int tenantId, int formDesignVersionId, int uiElementId)
        {
            RepeaterElementModel viewModel = null;
            try
            {
                RepeaterUIElement element = this._unitOfWork.RepositoryAsync<RepeaterUIElement>()
                                                            .Query()
                                                            .Include(c => c.DataSource)
                                                            .Filter(c => c.UIElementID == uiElementId)
                                                            .Get()
                                                            .SingleOrDefault();

                if (element != null)
                {
                    viewModel = new RepeaterElementModel();

                    if (element.DataSource != null &&
                        (this._unitOfWork.RepositoryAsync<FormDesignVersion>().IsFormDesignVersionFinalized(element.DataSource.FormDesignVersionID)))
                    {
                        viewModel.IsDataSourceEnabled = false;
                    }
                    else
                    {
                        viewModel.IsDataSourceEnabled = true;
                    }
                    if (element.LoadFromServer == true)
                    {
                        viewModel.IsLoadFromServerEnabled = false;
                    }
                    else
                    {
                        viewModel.IsLoadFromServerEnabled = true;
                    }
                    viewModel.ChildCount = element.ChildCount ?? 0;
                    viewModel.Enabled = element.Enabled ?? false;
                    viewModel.HasCustomRule = element.HasCustomRule ?? false;
                    viewModel.CustomRule = element.CustomRule;
                    viewModel.HelpText = element.HelpText;
                    viewModel.Label = this.GetAlternateLabel(formDesignVersionId, uiElementId) ?? element.Label;
                    viewModel.LayoutTypeID = element.LayoutTypeID;
                    viewModel.ParentUIElementID = element.ParentUIElementID ?? 0;
                    viewModel.UIElementID = element.UIElementID;
                    viewModel.Visible = element.Visible ?? false;
                    viewModel.TenantID = tenantId;
                    viewModel.FormDesignVersionID = formDesignVersionId;
                    viewModel.IsDataSource = element.DataSource != null ? true : false;
                    viewModel.IsDataRequired = element.IsDataRequired;
                    viewModel.DataSourceName = element.DataSource != null ? element.DataSource.DataSourceName : string.Empty;
                    viewModel.DataSourceDescription = element.DataSource != null ? element.DataSource.DataSourceDescription : string.Empty;
                    viewModel.LoadFromServer = element.LoadFromServer;
                    viewModel.AllowBulkUpdate = element.AllowBulkUpdate;
                    viewModel.IsStandard = element.IsStandard;
                    // Properties for Configuring Param Query Features 
                    viewModel.DisplayTopHeader = element.DisplayTopHeader;
                    viewModel.DisplayTitle = element.DisplayTitle;
                    viewModel.FrozenColCount = element.FrozenColCount;
                    viewModel.FrozenRowCount = element.FrozenRowCount;
                    viewModel.AllowPaging = element.AllowPaging;
                    viewModel.RowsPerPage = element.RowsPerPage;
                    viewModel.AllowExportToExcel = element.AllowExportToExcel;
                    viewModel.AllowExportToCSV = element.AllowExportToCSV;
                    viewModel.FilterMode = element.FilterMode;
                    viewModel.MDMName = element.MDMName;
                    RepeaterUIElementProperties repeaterElementProperties = this._unitOfWork.RepositoryAsync<RepeaterUIElementProperties>()
                                                                                .Query()
                                                                                .Filter(c => c.RepeaterUIElementID == uiElementId)
                                                                                .Get()
                                                                                .SingleOrDefault();
                    if (repeaterElementProperties != null)
                    {
                        viewModel.RepeaterUIElementProperties = new RepeaterUIElementPropertyModel();
                        viewModel.RepeaterUIElementProperties.RowTemplate = repeaterElementProperties.RowTemplate;
                        viewModel.RepeaterUIElementProperties.HeaderTemplate = repeaterElementProperties.HeaderTemplate;
                        viewModel.RepeaterUIElementProperties.FooterTemplate = repeaterElementProperties.FooterTemplate;
                    }
                }
                else
                {
                    viewModel = null;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return viewModel;
        }



        public ServiceResult UpdateRepeater(string userName, int tenantId, int formDesignId, int formDesignVersionId, int uiElementId, bool isEnabled, bool isVisible, bool hasCustomRule, string helpText, bool isRequired, string label, int childCount, int sequence, int layoutTypeId, bool isDataSource, string dataSourceName, string dataSourceDescription, IEnumerable<RuleRowModel> rules, bool modifyRules, bool modifyCustomRules, string customRule, bool loadFromServer, bool isDataRequired, bool allowBulkUpdate, RepeaterElementModel advancedConfiguration, RepeaterUIElementPropertyModel repeaterTemplates, bool isStandard,string mdmName)
        {
            ServiceResult result = new ServiceResult();

            try
            {
                RepeaterUIElement repeaterElement = this._unitOfWork.RepositoryAsync<RepeaterUIElement>()
                                                                    .FindById(uiElementId);

                //bool isFormDesignVersionFinzalized = this._unitOfWork.RepositoryAsync<FormDesignVersion>().IsFormDesignVersionFinalized(formDesignVersionId);
                //if (isFormDesignVersionFinzalized)
                //{
                //    result.Result = ServiceResultStatus.Failure;
                //    result.Items.ToList().Add(new ServiceResultItem { Messages = new string[] { "Unable to Update finalized Form Design Version" } });
                //}
                //else
                {
                    int finalizedStatusID = Convert.ToInt32(tmg.equinox.domain.entities.Status.Finalized);
                    bool isUsedInFinalizedFormVersion = this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>()
                                                                        .Query()
                                                                        .Include(c => c.FormDesignVersion)
                                                                        .Filter(c => c.UIElementID == uiElementId)
                                                                        .Filter(c => c.FormDesignVersion.StatusID == finalizedStatusID)
                                                                        .Get()
                                                                        .Any();
                    if (isUsedInFinalizedFormVersion)
                    {
                        result.Result = ServiceResultStatus.Failure;
                        result.Items.ToList().Add(new ServiceResultItem { Messages = new string[] { "Unable to Update finalized Form Design Version" } });
                    }
                }
                if (repeaterElement != null)
                {

                    if (loadFromServer == true)
                    {
                        result = AllowLoadFromServer(isDataSource, uiElementId);
                        if (result.Result == ServiceResultStatus.Failure)
                        {
                            return result;
                        }
                        result = AddNewLoadFromServerEnabledUIElement(userName, tenantId, formDesignId, formDesignVersionId, uiElementId, isEnabled, isVisible, hasCustomRule, helpText, isRequired, label, childCount, sequence, layoutTypeId, isDataSource, dataSourceName, dataSourceDescription, rules, modifyRules, modifyCustomRules, customRule, loadFromServer, repeaterElement, advancedConfiguration);
                        if (result.Result == ServiceResultStatus.Success)
                            return result;

                    }
                    if (isDataSource)
                    {
                        if (!CheckForUniqueDataSourceName(repeaterElement.DataSourceID, dataSourceName))
                        {
                            result.Result = ServiceResultStatus.Failure;

                            List<ServiceResultItem> items = new List<ServiceResultItem>();
                            items.Add(new ServiceResultItem { Messages = new string[] { "Please enter unique data source name.", repeaterElement.ToString() } });
                            result.Items = items;
                            return result;
                        }
                    }
                    else
                    {
                        if (CheckDataSourceisInUse(repeaterElement.DataSourceID))
                        {
                            result.Result = ServiceResultStatus.Failure;

                            List<ServiceResultItem> items = new List<ServiceResultItem>();
                            items.Add(new ServiceResultItem { Messages = new string[] { "Data source is already in use. You can not delete it.", repeaterElement.ToString() } });
                            result.Items = items;
                            return result;
                        }
                    }

                    //code to update parameters here. 
                    sequence = sequence == 0 ? repeaterElement.Sequence : sequence;
                    repeaterElement.LayoutTypeID = layoutTypeId;
                    repeaterElement.Enabled = isEnabled;
                    repeaterElement.HelpText = helpText;
                    repeaterElement.RequiresValidation = isRequired;
                    //repeaterElement.Label = label;
                    repeaterElement.GeneratedName = GetGeneratedName(repeaterElement.Label);
                    repeaterElement.Visible = isVisible;
                    repeaterElement.Sequence = sequence;
                    repeaterElement.ChildCount = childCount;
                    repeaterElement.LayoutTypeID = layoutTypeId;
                    repeaterElement.LoadFromServer = loadFromServer;
                    repeaterElement.IsDataRequired = isDataRequired;
                    repeaterElement.AllowBulkUpdate = allowBulkUpdate;
                    repeaterElement.IsStandard = isStandard;
                    repeaterElement.MDMName = mdmName;
                    if (advancedConfiguration != null)
                    {
                        // Set properties for Configuring ParamQuery advanced Features 
                        repeaterElement.DisplayTopHeader = advancedConfiguration.DisplayTopHeader;
                        repeaterElement.DisplayTitle = advancedConfiguration.DisplayTitle;
                        repeaterElement.FrozenColCount = advancedConfiguration.FrozenColCount;
                        repeaterElement.FrozenRowCount = advancedConfiguration.FrozenRowCount;
                        repeaterElement.AllowPaging = advancedConfiguration.AllowPaging;
                        repeaterElement.RowsPerPage = advancedConfiguration.RowsPerPage;
                        repeaterElement.AllowExportToExcel = advancedConfiguration.AllowExportToExcel;
                        repeaterElement.AllowExportToCSV = advancedConfiguration.AllowExportToCSV;
                        repeaterElement.FilterMode = advancedConfiguration.FilterMode ?? "Local Header Filtering";
                    }

                    if (repeaterTemplates != null)
                    {
                        RepeaterUIElementProperties repeaterElementProperties = this._unitOfWork.RepositoryAsync<RepeaterUIElementProperties>()
                                                                                .Query()
                                                                                .Filter(c => c.RepeaterUIElementID == uiElementId)
                                                                                .Get()
                                                                                .SingleOrDefault();
                        // update if exist
                        if (repeaterElementProperties != null)
                        {
                            repeaterElementProperties.RowTemplate = repeaterTemplates.RowTemplate;
                            repeaterElementProperties.HeaderTemplate = repeaterTemplates.HeaderTemplate;
                            repeaterElementProperties.FooterTemplate = repeaterTemplates.FooterTemplate;
                            this._unitOfWork.RepositoryAsync<RepeaterUIElementProperties>().Update(repeaterElementProperties);
                        }
                        // Insert if not exist
                        else
                        {
                            repeaterElementProperties = new RepeaterUIElementProperties();
                            repeaterElementProperties.RepeaterUIElementID = uiElementId;
                            repeaterElementProperties.RowTemplate = repeaterTemplates.RowTemplate;
                            repeaterElementProperties.HeaderTemplate = repeaterTemplates.HeaderTemplate;
                            repeaterElementProperties.FooterTemplate = repeaterTemplates.FooterTemplate;
                            this._unitOfWork.RepositoryAsync<RepeaterUIElementProperties>().Insert(repeaterElementProperties);
                        }
                    }

                    if (modifyCustomRules)
                    {
                        repeaterElement.HasCustomRule = hasCustomRule;
                        repeaterElement.CustomRule = customRule;
                    }

                    //Update the datasource  name if datasource already present
                    DataSource repeaterDataSource = this._unitOfWork.RepositoryAsync<DataSource>()
                                                        .Query()
                                                        .Filter(c => c.DataSourceID == repeaterElement.DataSourceID
                                                                        && c.FormDesignID == formDesignId)
                                                        .Get()
                                                        .FirstOrDefault();


                    if (repeaterDataSource != null && isDataSource == true)
                    {
                        if (!(this._unitOfWork.RepositoryAsync<FormDesignVersion>().IsFormDesignVersionFinalized(repeaterDataSource.FormDesignVersionID)))
                        {
                            repeaterDataSource.DataSourceName = dataSourceName;
                            repeaterDataSource.DataSourceDescription = dataSourceDescription;
                            this._unitOfWork.RepositoryAsync<DataSource>().Update(repeaterDataSource);
                        }
                        else
                        {
                            result.Result = ServiceResultStatus.Failure;
                            List<ServiceResultItem> items = new List<ServiceResultItem>();
                            items.Add(new ServiceResultItem { Messages = new string[] { "Data source is associated with Finalized FormDesignVersion. You can not Update it.", repeaterElement.ToString() } });
                            result.Items = items;
                            return result;
                        }
                    }
                    else if (repeaterDataSource == null && isDataSource == true)
                    {
                        DataSource newrepeaterDataSource = new DataSource();
                        newrepeaterDataSource.FormDesignID = formDesignId;
                        newrepeaterDataSource.FormDesignVersionID = formDesignVersionId;
                        newrepeaterDataSource.DataSourceName = dataSourceName;
                        newrepeaterDataSource.DataSourceDescription = dataSourceDescription;
                        newrepeaterDataSource.Type = "Repeater";
                        newrepeaterDataSource.AddedBy = userName;
                        newrepeaterDataSource.AddedDate = DateTime.Now;
                        this._unitOfWork.RepositoryAsync<DataSource>().Insert(newrepeaterDataSource);
                        repeaterElement.DataSource = newrepeaterDataSource;
                    }
                    else if (isDataSource == false)
                    {

                        if (repeaterDataSource != null)
                        {

                            if (!(this._unitOfWork.RepositoryAsync<FormDesignVersion>().IsFormDesignVersionFinalized(repeaterDataSource.FormDesignVersionID)))
                            {
                                this._unitOfWork.RepositoryAsync<DataSource>().Delete(repeaterDataSource);
                            }
                            else
                            {
                                result.Result = ServiceResultStatus.Failure;
                                List<ServiceResultItem> items = new List<ServiceResultItem>();
                                items.Add(new ServiceResultItem { Messages = new string[] { "Data source is associated with Finalized FormDesignVersion. You can not delete it.", repeaterElement.ToString() } });
                                result.Items = items;
                                return result;
                            }
                        }
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

                    this._unitOfWork.RepositoryAsync<RepeaterUIElement>().Update(repeaterElement);

                    this._unitOfWork.Save();

                    //Update Alternate Label
                    if (!string.Equals(repeaterElement.Label, label) || this.isExistsInAlternateLabel(repeaterElement.UIElementID))
                    {
                        this.UpdateAlternateLabel(tenantId, formDesignId, formDesignVersionId, repeaterElement.UIElementID, label);

                    }

                    result.Result = ServiceResultStatus.Success;
                }
                else
                {
                    result.Result = ServiceResultStatus.Failure;
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

        public ServiceResult UpdateRepeater(string userName, int tenantId, int formDesignId, int formDesignVersionId, int uiElementId, bool isEnabled, bool isVisible, string helpText, bool isRequired, string label, int sequence, IEnumerable<RuleRowModel> rules, bool modifyRules, RepeaterElementModel advancedConfiguration, int parentUIElementId, string extProp, string layout, int viewType, bool allowBulkUpdate, bool isStandard)
        {
            ServiceResult result = new ServiceResult();

            try
            {
                RepeaterUIElement repeaterElement = this._unitOfWork.RepositoryAsync<RepeaterUIElement>()
                                                                    .FindById(uiElementId);
                int layOutTypeID = this._unitOfWork.RepositoryAsync<LayoutType>().Get().Where(t => t.DisplayText == layout).Select(s => s.LayoutTypeID).FirstOrDefault();

                bool isFormDesignVersionFinzalized = this._unitOfWork.RepositoryAsync<FormDesignVersion>().IsFormDesignVersionFinalized(formDesignVersionId);
                if (isFormDesignVersionFinzalized)
                {
                    result.Result = ServiceResultStatus.Failure;
                    result.Items.ToList().Add(new ServiceResultItem { Messages = new string[] { "Unable to Update finalized Form Design Version" } });
                }
                else
                {
                    int finalizedStatusID = Convert.ToInt32(tmg.equinox.domain.entities.Status.Finalized);
                    bool isUsedInFinalizedFormVersion = this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>()
                                                                        .Query()
                                                                        .Include(c => c.FormDesignVersion)
                                                                        .Filter(c => c.UIElementID == uiElementId)
                                                                        .Filter(c => c.FormDesignVersion.StatusID == finalizedStatusID)
                                                                        .Get()
                                                                        .Any();
                    if (isUsedInFinalizedFormVersion)
                    {
                        result.Result = ServiceResultStatus.Failure;
                        result.Items.ToList().Add(new ServiceResultItem { Messages = new string[] { "Unable to Update finalized Form Design Version" } });
                    }
                }
                if (repeaterElement != null)
                {

                    //code to update parameters here. 
                    sequence = sequence == 0 ? repeaterElement.Sequence : sequence;
                    repeaterElement.Enabled = isEnabled;
                    repeaterElement.HelpText = helpText;
                    repeaterElement.RequiresValidation = isRequired;
                    repeaterElement.GeneratedName = GetGeneratedName(repeaterElement.Label);
                    repeaterElement.Visible = isVisible;
                    repeaterElement.Sequence = sequence;
                    repeaterElement.ParentUIElementID = parentUIElementId;
                    repeaterElement.ExtendedProperties = extProp;
                    repeaterElement.LayoutTypeID = layOutTypeID == 0 ? 5 : layOutTypeID;
                    repeaterElement.ViewType = viewType;
                    repeaterElement.IsStandard = isStandard;
                    repeaterElement.AllowBulkUpdate = allowBulkUpdate;

                    if (advancedConfiguration != null)
                    {
                        // Set properties for Configuring ParamQuery advanced Features 
                        repeaterElement.DisplayTopHeader = advancedConfiguration.DisplayTopHeader;
                        repeaterElement.DisplayTitle = advancedConfiguration.DisplayTitle;
                        repeaterElement.FrozenColCount = advancedConfiguration.FrozenColCount;
                        repeaterElement.FrozenRowCount = advancedConfiguration.FrozenRowCount;
                        repeaterElement.AllowPaging = advancedConfiguration.AllowPaging;
                        repeaterElement.RowsPerPage = advancedConfiguration.RowsPerPage;
                        repeaterElement.AllowExportToExcel = advancedConfiguration.AllowExportToExcel;
                        repeaterElement.AllowExportToCSV = advancedConfiguration.AllowExportToCSV;
                        repeaterElement.FilterMode = advancedConfiguration.FilterMode ?? "Local Header Filtering";
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

                    this._unitOfWork.RepositoryAsync<RepeaterUIElement>().Update(repeaterElement);

                    this._unitOfWork.Save();

                    //Update Alternate Label
                    if (!string.Equals(repeaterElement.Label, label) || this.isExistsInAlternateLabel(repeaterElement.UIElementID))
                    {
                        this.UpdateAlternateLabel(tenantId, formDesignId, formDesignVersionId, repeaterElement.UIElementID, label);

                    }

                    result.Result = ServiceResultStatus.Success;
                }
                else
                {
                    result.Result = ServiceResultStatus.Failure;
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

        private void CloneRepeaterChildElements(int? parentElementID, int destinationElementID, string userName, DateTime addedDate)
        {
            if (parentElementID.HasValue)
            {
                var childItems = this._unitOfWork.RepositoryAsync<UIElement>()
                                                                    .Query()
                                                                    .Filter(c => c.ParentUIElementID == parentElementID)
                                                                    .Get()
                                                                    .ToList();

                foreach (var childItem in childItems)
                {
                    if (childItem is CalendarUIElement)
                    {
                        var element = (childItem as CalendarUIElement).Clone(userName, DateTime.Now);
                        element.ParentUIElementID = destinationElementID;
                        this._unitOfWork.RepositoryAsync<UIElement>().Insert(element);
                    }
                    else if (childItem is CheckBoxUIElement)
                    {
                        var element = (childItem as CheckBoxUIElement).Clone(userName, DateTime.Now);
                        element.ParentUIElementID = destinationElementID;
                        this._unitOfWork.RepositoryAsync<UIElement>().Insert(element);
                    }
                    else if (childItem is DropDownUIElement)
                    {
                        var element = (childItem as DropDownUIElement).Clone(userName, DateTime.Now);
                        element.ParentUIElementID = destinationElementID;
                        this._unitOfWork.RepositoryAsync<UIElement>().Insert(element);
                    }
                    else if (childItem is RadioButtonUIElement)
                    {
                        var element = (childItem as CheckBoxUIElement).Clone(userName, DateTime.Now);
                        element.ParentUIElementID = destinationElementID;
                        this._unitOfWork.RepositoryAsync<UIElement>().Insert(element);
                    }
                    else if (childItem is TextBoxUIElement)
                    {
                        var element = (childItem as TextBoxUIElement).Clone(userName, DateTime.Now);
                        element.ParentUIElementID = destinationElementID;
                        this._unitOfWork.RepositoryAsync<UIElement>().Insert(element);
                    }
                }
            }
        }

        public ServiceResult AddRepeater(string userName, int tenantId, int formDesignVersionId, int parentUIElementId, string label, string helpText, int sequence, string elementType, bool isStandard, int viewType)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                RepeaterUIElement repeaterElement = new RepeaterUIElement();

                FormDesignVersion formDesignVersion = this._unitOfWork.RepositoryAsync<FormDesignVersion>()
                                                          .FindById(formDesignVersionId);

                int layOutTypeID = this._unitOfWork.RepositoryAsync<LayoutType>().Query().Filter(l => l.LayoutTypeCode == "1COL").Get().Select(l => l.LayoutTypeID).FirstOrDefault(); // For 1Column grid layout

                int childCount = this._unitOfWork.RepositoryAsync<UIElement>()
                                                    .Query()
                                                    .Filter(c => c.ParentUIElementID == parentUIElementId)
                                                    .Get()
                                                    .Count();
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
                repeaterElement.LayoutTypeID = layOutTypeID;
                repeaterElement.ChildCount = childCount + 1;

                //Retrieve the UIElementType for Repeater, assuming here that it's code in database will be RPT 
                repeaterElement.UIElementTypeID = uiElementTypeId;
                //Retrieve the ApplicationDataType for Repeater, assuming here that it's code in database will be NA 
                repeaterElement.AddedBy = userName;
                repeaterElement.AddedDate = DateTime.Now;                                       //NOTE - Setting up default values here
                repeaterElement.UIElementName = this.GetUniqueName(formDesignVersion.FormDesignID ?? 0, ElementTypes.REPEATER, repeaterElement.UIElementID, parentUIElementId);
                repeaterElement.Enabled = true;                                                 //Enable
                repeaterElement.HasCustomRule = false;                                          //HasCustomRule
                repeaterElement.IsActive = true;                                                //IsActive
                repeaterElement.IsContainer = true;                                             //IsContainer
                repeaterElement.Label = label;
                repeaterElement.GeneratedName = GetGeneratedName(label);
                repeaterElement.FormID = formDesignVersion.FormDesignID ?? 0;
                repeaterElement.ParentUIElementID = parentUIElementId;
                repeaterElement.RequiresValidation = false;                                     //RequiredValidation 
                repeaterElement.Sequence = sequenceNo + 1;
                repeaterElement.Visible = true;
                repeaterElement.UIElementDataTypeID = uiElementDataTypeId;
                repeaterElement.IsStandard = isStandard;
                repeaterElement.ViewType = viewType;

                // Properties for Configuring Param Query Features 
                repeaterElement.DisplayTopHeader = false;
                repeaterElement.DisplayTitle = false;
                repeaterElement.FrozenColCount = 0;
                repeaterElement.FrozenRowCount = 0;
                repeaterElement.AllowPaging = false;
                repeaterElement.RowsPerPage = 0;
                repeaterElement.AllowExportToExcel = false;
                repeaterElement.AllowExportToCSV = false;
                repeaterElement.FilterMode = "Local Header Filtering";


                using (var scope = new TransactionScope())
                {
                    this._unitOfWork.RepositoryAsync<RepeaterUIElement>().Insert(repeaterElement);
                    this._unitOfWork.Save();

                    repeaterElement.UIElementName = this.GetUniqueName(formDesignVersion.FormDesignID ?? 0, ElementTypes.REPEATER, repeaterElement.UIElementID, parentUIElementId);
                    this._unitOfWork.RepositoryAsync<RepeaterUIElement>().Update(repeaterElement);

                    //add FormDesignVersionUIElementMap record
                    FormDesignVersionUIElementMap map = new FormDesignVersionUIElementMap();
                    map.FormDesignVersionID = formDesignVersionId;
                    map.EffectiveDate = formDesignVersion.EffectiveDate;
                    map.UIElementID = repeaterElement.UIElementID;
                    this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Insert(map);
                    this._unitOfWork.Save();

                    scope.Complete();
                    IList<ServiceResultItem> item = new List<ServiceResultItem>();
                    item.Add(new ServiceResultItem { Messages = new string[] { repeaterElement.UIElementID.ToString() } });
                    item.Add(new ServiceResultItem { Messages = new string[] { GetElementType(repeaterElement) } });

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

        public ServiceResult AddRepeater(string userName, int tenantId, int formDesignVersionId, int parentUIElementId, string label, string helpText, int sequence, string elementType, bool isEnable, bool isVisible, List<RuleRowModel> rules, bool modifyRules, RepeaterElementModel advancedConfiguration, string extProp, string layout, int viewType, bool allowBulkUpdate, int sourceUIElementId)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                RepeaterUIElement repeaterElement = new RepeaterUIElement();

                FormDesignVersion formDesignVersion = this._unitOfWork.RepositoryAsync<FormDesignVersion>()
                                                          .FindById(formDesignVersionId);

                int layOutTypeID = this._unitOfWork.RepositoryAsync<LayoutType>().Get().Where(t => t.DisplayText == layout).Select(s => s.LayoutTypeID).FirstOrDefault();

                int childCount = this._unitOfWork.RepositoryAsync<UIElement>()
                                                    .Query()
                                                    .Filter(c => c.ParentUIElementID == parentUIElementId)
                                                    .Get()
                                                    .Count();
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
                repeaterElement.LayoutTypeID = layOutTypeID == 0 ? 5 : layOutTypeID;
                repeaterElement.ChildCount = childCount + 1;

                //Retrieve the UIElementType for Repeater, assuming here that it's code in database will be RPT 
                repeaterElement.UIElementTypeID = uiElementTypeId;
                //Retrieve the ApplicationDataType for Repeater, assuming here that it's code in database will be NA 
                repeaterElement.AddedBy = userName;
                repeaterElement.AddedDate = DateTime.Now;                                       //NOTE - Setting up default values here
                repeaterElement.UIElementName = this.GetUniqueName(formDesignVersion.FormDesignID ?? 0, ElementTypes.REPEATER, repeaterElement.UIElementID, parentUIElementId);
                repeaterElement.Enabled = isEnable;                                                 //Enable
                repeaterElement.HasCustomRule = false;                                          //HasCustomRule
                repeaterElement.IsActive = true;                                                //IsActive
                repeaterElement.IsContainer = true;                                             //IsContainer
                repeaterElement.Label = label;
                repeaterElement.GeneratedName = GetGeneratedName(label);
                repeaterElement.FormID = formDesignVersion.FormDesignID ?? 0;
                repeaterElement.ParentUIElementID = parentUIElementId;
                repeaterElement.RequiresValidation = false;                                     //RequiredValidation 
                repeaterElement.Sequence = sequence == 0 ? sequenceNo + 1 : sequence;
                repeaterElement.Visible = isVisible;
                repeaterElement.UIElementDataTypeID = uiElementDataTypeId;
                repeaterElement.ExtendedProperties = extProp;
                repeaterElement.ViewType = viewType;
                repeaterElement.AllowBulkUpdate = allowBulkUpdate;

                if (advancedConfiguration != null)
                {
                    // Properties for Configuring Param Query Features 
                    repeaterElement.DisplayTopHeader = advancedConfiguration.DisplayTopHeader;
                    repeaterElement.DisplayTitle = advancedConfiguration.DisplayTitle;
                    repeaterElement.FrozenColCount = advancedConfiguration.FrozenColCount;
                    repeaterElement.FrozenRowCount = advancedConfiguration.FrozenRowCount;
                    repeaterElement.AllowPaging = advancedConfiguration.AllowPaging;
                    repeaterElement.RowsPerPage = advancedConfiguration.RowsPerPage;
                    repeaterElement.AllowExportToExcel = advancedConfiguration.AllowExportToExcel;
                    repeaterElement.AllowExportToCSV = advancedConfiguration.AllowExportToCSV;
                    repeaterElement.FilterMode = advancedConfiguration.FilterMode;
                }
                else
                {
                    repeaterElement.DisplayTopHeader = false;
                    repeaterElement.DisplayTitle = false;
                    repeaterElement.FrozenColCount = 0;
                    repeaterElement.FrozenRowCount = 0;
                    repeaterElement.AllowPaging = false;
                    repeaterElement.RowsPerPage = 0;
                    repeaterElement.AllowExportToExcel = false;
                    repeaterElement.AllowExportToCSV = false;
                    repeaterElement.FilterMode = "Local Header Filtering";
                }

                using (var scope = new TransactionScope())
                {
                    this._unitOfWork.RepositoryAsync<RepeaterUIElement>().Insert(repeaterElement);
                    this._unitOfWork.Save();

                    repeaterElement.UIElementName = this.GetUniqueName(formDesignVersion.FormDesignID ?? 0, ElementTypes.REPEATER, repeaterElement.UIElementID, parentUIElementId);
                    this._unitOfWork.RepositoryAsync<RepeaterUIElement>().Update(repeaterElement);

                    //change Rules
                    if (modifyRules == true && rules != null)
                    {
                        ChangeRules(userName, tenantId, formDesignVersionId, repeaterElement.UIElementID, repeaterElement.UIElementID, rules, false);
                    }

                    //add FormDesignVersionUIElementMap record
                    FormDesignVersionUIElementMap map = new FormDesignVersionUIElementMap();
                    map.FormDesignVersionID = formDesignVersionId;
                    map.EffectiveDate = formDesignVersion.EffectiveDate;
                    map.UIElementID = repeaterElement.UIElementID;
                    this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Insert(map);
                    this._unitOfWork.Save();

                    scope.Complete();
                    IList<ServiceResultItem> item = new List<ServiceResultItem>();
                    item.Add(new ServiceResultItem { Messages = new string[] { repeaterElement.UIElementID.ToString() } });
                    item.Add(new ServiceResultItem { Messages = new string[] { GetElementType(repeaterElement) } });

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

        public ServiceResult DeleteRepeater(int tenantId, int formDesignVersionId, int uiElementId)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                ExceptionMessages exceptionMessage = ExceptionMessages.NULL;
                DeleteRepeaterUIElementProperties(uiElementId);
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

        public ServiceResult AddDefaultKeyForRepeater(int tenantId, int formDesignVersionId, int parentUIElementId)
        {
            ServiceResult result = new ServiceResult();
            List<RepeaterKeyUIElement> keyElements = null;
            try
            {


                var checkrepeater = this._unitOfWork.RepositoryAsync<RepeaterUIElement>().Query().Filter(c => c.UIElementID == parentUIElementId).Get().Count();
                // .FindById(parentUIElementId);

                if (checkrepeater > 0)
                {
                    var childelements = this._unitOfWork.RepositoryAsync<UIElement>()
                                                             .Query()
                                                             .Include(c => c.FormDesignVersionUIElementMaps)
                                                             .Filter(c => (c.FormDesignVersionUIElementMaps.Any(f => f.FormDesignVersionID == formDesignVersionId) && c.ParentUIElementID == parentUIElementId))
                                                             .Get().ToList();
                    if (childelements != null && childelements.Count > 0)
                    {
                        if (childelements.Count == 1)
                        {
                            keyElements = this._unitOfWork.RepositoryAsync<RepeaterKeyUIElement>()
                                       .Query().Filter(c => c.RepeaterUIElementID == parentUIElementId)
                                       .Get().ToList();

                            if (keyElements == null || keyElements.Count == 0)
                            {
                                RepeaterKeyUIElement addKey = new RepeaterKeyUIElement();
                                addKey.RepeaterUIElementID = parentUIElementId;
                                addKey.UIElementID = childelements[0].UIElementID;
                                this._unitOfWork.RepositoryAsync<RepeaterKeyUIElement>().Insert(addKey);
                                this._unitOfWork.Save();
                                result.Result = ServiceResultStatus.Success;
                            }
                        }
                    }
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

        public ServiceResult UpdateRepeaterKeyElement(int tenantId, int parentElementId, IDictionary<int, bool> keyUiElements)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                UIElement parentElement = this._unitOfWork.RepositoryAsync<UIElement>()
                                            .Query().Filter(c => c.UIElementID == parentElementId)
                                            .Get().FirstOrDefault();

                if (parentElement is RepeaterUIElement)
                {
                    foreach (var item in keyUiElements)
                    {
                        RepeaterKeyUIElement el = this._unitOfWork.RepositoryAsync<RepeaterKeyUIElement>().Query().Filter(c => c.UIElementID == item.Key).Get().FirstOrDefault();
                        if (el != null)
                        {
                            this._unitOfWork.RepositoryAsync<RepeaterKeyUIElement>().Delete(el);
                        }
                    }

                    foreach (var keyElement in keyUiElements.Where(c => c.Value == true))
                    {
                        RepeaterKeyUIElement element = new RepeaterKeyUIElement();
                        element.RepeaterUIElementID = parentElementId;
                        element.UIElementID = keyElement.Key;

                        this._unitOfWork.RepositoryAsync<RepeaterKeyUIElement>().Insert(element);
                    }

                    this._unitOfWork.Save();
                }
                result.Result = ServiceResultStatus.Success;
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

        #endregion Repeater Element
        #endregion Public Methods

        #region Private Methods
        private bool CheckUseDataSourceForRepeater(int repeaterUIElementId)
        {
            bool checkUseDataSource = false;
            if (repeaterUIElementId != null)
            {
                var uielementDetails = this._unitOfWork.RepositoryAsync<DataSourceMapping>()
                                                                    .Query()
                                                                    .Filter(c => c.UIElement.ParentUIElementID == repeaterUIElementId)
                                                                    .Get().ToList();
                if (uielementDetails.Count > 0)
                {
                    checkUseDataSource = true;
                }
            }
            return checkUseDataSource;
        }

        /// <summary>
        /// This method is used to check whether atleast one check duplicate is true for the child elements of repeater.
        /// </summary>
        /// <param name="uiElementId"></param>
        /// <returns></returns>
        private bool IsCheckDuplicateEnabled(int uiElementId)
        {
            bool checkDuplicate = false;
            if (uiElementId != null)
            {
                checkDuplicate = this._unitOfWork.RepositoryAsync<UIElement>()
                                .Query()
                                .Filter(c => c.ParentUIElementID == uiElementId)
                                .Get()
                                .Any(p => p.CheckDuplicate.Equals(true));
            }
            return checkDuplicate;
        }

        private IList<PropertyRuleMap> CheckRulesAndCustomRulesEnabled(int uielementId)
        {
            IList<PropertyRuleMap> repeaterChildElement = this._unitOfWork.RepositoryAsync<PropertyRuleMap>()
                                                             .Query()
                                                             .Include(c => c.UIElement)
                                                             .Get()
                                                             .Where(p => p.UIElement.ParentUIElementID == uielementId || p.UIElement.UIElementID == uielementId)
                                                             .ToList();

            //IList<PropertyRuleMap> repeater = this._unitOfWork.RepositoryAsync<PropertyRuleMap>()
            //                                                 .Query()
            //                                                 .Include(c => c.UIElement)
            //                                                 .Get()
            //                                                 .Where(p => p.UIElement.UIElementID == uielementId)
            //                                                 .ToList();

            //if (repeaterChildElement.Count() > 0 || repeater.Count() > 0)
            //{

            //}
            return repeaterChildElement;
        }

        /// <summary>
        /// This method is used to add new UIElementID for repeater with its child elements if LoadFromServer is made true for existing repeater in next version. This functionality is only applicable for 'Master List' 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="tenantId"></param>
        /// <param name="formDesignId"></param>
        /// <param name="formDesignVersionId"></param>
        /// <param name="uiElementId"></param>
        /// <param name="isEnabled"></param>
        /// <param name="isVisible"></param>
        /// <param name="hasCustomRule"></param>
        /// <param name="helpText"></param>
        /// <param name="isRequired"></param>
        /// <param name="label"></param>
        /// <param name="childCount"></param>
        /// <param name="sequence"></param>
        /// <param name="layoutTypeId"></param>
        /// <param name="isDataSource"></param>
        /// <param name="dataSourceName"></param>
        /// <param name="dataSourceDescription"></param>
        /// <param name="rules"></param>
        /// <param name="modifyRules"></param>
        /// <param name="modifyCustomRules"></param>
        /// <param name="customRule"></param>
        /// <param name="loadFromServer"></param>
        /// <param name="repeaterElement"></param>
        /// <returns></returns>
        private ServiceResult AddNewLoadFromServerEnabledUIElement(string userName, int tenantId, int formDesignId, int formDesignVersionId, int uiElementId, bool isEnabled, bool isVisible, bool hasCustomRule, string helpText, bool isRequired, string label, int childCount, int sequence, int layoutTypeId, bool isDataSource, string dataSourceName, string dataSourceDescription, IEnumerable<RuleRowModel> rules, bool modifyRules, bool modifyCustomRules, string customRule, bool loadFromServer, RepeaterUIElement repeaterElement, RepeaterElementModel advancedConfiguration)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                // Get previous version's repeater element details.
                List<UIElementMap> previousElementsList = (from repeaterUIElement in this._unitOfWork.RepositoryAsync<RepeaterUIElement>().Get()
                                                           join uiElement in this._unitOfWork.RepositoryAsync<UIElement>().Get()
                                                           on repeaterUIElement.UIElementID equals uiElement.UIElementID
                                                           join formDesignVersionUIElementMap in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Get()
                                                           on uiElement.UIElementID equals formDesignVersionUIElementMap.UIElementID
                                                           where (formDesignVersionUIElementMap.FormDesignVersionID != formDesignVersionId && uiElement.UIElementID == uiElementId)
                                                           select new UIElementMap
                                                           {
                                                               FormDesignVersionId = formDesignVersionUIElementMap.FormDesignVersionID,
                                                               UIElementId = uiElement.UIElementID,
                                                               LoadFromServer = repeaterElement.LoadFromServer,
                                                               FormDesignVersionStatusId = formDesignVersionUIElementMap.FormDesignVersion.StatusID
                                                           }).ToList();


                FormDesignVersionUIElementMap currentMap = this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>()
                                                                            .Query()
                                                                            .Include(c => c.FormDesignVersion)
                                                                            .Filter(c => c.FormDesignVersionID == formDesignVersionId && c.UIElementID == uiElementId)
                                                                            .Get()
                                                                            .FirstOrDefault();


                int formDesignID = currentMap.FormDesignVersion.FormDesignID.Value;
                int finalizedStatusID = Convert.ToInt32(tmg.equinox.domain.entities.Status.Finalized);
                if (previousElementsList.Count > 0)
                {
                    if (previousElementsList.Any(c => c.FormDesignVersionStatusId == finalizedStatusID))
                    {
                        //update effective date of removal for finalized form design versions
                        foreach (var item in previousElementsList)
                        {

                            FormDesignVersionUIElementMap list = this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>()
                                                                                    .Query()
                                                                                    .Include(c => c.FormDesignVersion)
                                                                                    .Include(c => c.FormDesignVersion.Status)
                                                                                    .Filter(c => c.FormDesignVersionID == item.FormDesignVersionId && c.UIElementID == uiElementId)
                                                                                    .Get().FirstOrDefault();
                            list.EffectiveDateOfRemoval = currentMap.EffectiveDate.Value.AddDays(-1);

                            this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Update(list);


                            using (var scope = new TransactionScope())
                            {
                                RepeaterUIElement element = new RepeaterUIElement();
                                sequence = sequence == 0 ? repeaterElement.Sequence : sequence;
                                element.LayoutTypeID = layoutTypeId;
                                element.Enabled = isEnabled;
                                element.HelpText = helpText;
                                element.RequiresValidation = isRequired;
                                element.Label = label;
                                element.GeneratedName = GetGeneratedName(label);
                                element.Visible = isVisible;
                                element.Sequence = sequence;
                                element.ChildCount = childCount;
                                element.LayoutTypeID = layoutTypeId;
                                element.LoadFromServer = loadFromServer;
                                element.AddedDate = DateTime.Now;
                                element.AddedBy = userName;
                                element.FormID = formDesignID;
                                element.IsActive = true;
                                element.IsContainer = true;
                                element.ParentUIElementID = repeaterElement.ParentUIElementID;
                                element.UIElementDataTypeID = repeaterElement.UIElementDataTypeID;
                                element.UIElementTypeID = repeaterElement.UIElementTypeID;
                                if (advancedConfiguration != null)
                                {
                                    // Set properties for Configuring ParamQuery advanced Features 
                                    element.DisplayTopHeader = advancedConfiguration.DisplayTopHeader;
                                    element.DisplayTitle = advancedConfiguration.DisplayTitle;
                                    element.FrozenColCount = advancedConfiguration.FrozenColCount;
                                    element.FrozenRowCount = advancedConfiguration.FrozenRowCount;
                                    element.AllowPaging = advancedConfiguration.AllowPaging;
                                    element.RowsPerPage = advancedConfiguration.RowsPerPage;
                                    element.AllowExportToExcel = advancedConfiguration.AllowExportToExcel;
                                    element.AllowExportToCSV = advancedConfiguration.AllowExportToCSV;
                                    element.FilterMode = advancedConfiguration.FilterMode ?? "Local Header Filtering";
                                }

                                if (modifyCustomRules)
                                {
                                    element.HasCustomRule = hasCustomRule;
                                    element.CustomRule = customRule;
                                }

                                this._unitOfWork.RepositoryAsync<RepeaterUIElement>().Insert(element);
                                this._unitOfWork.Save();

                                element.UIElementName = this.GetUniqueName(formDesignID, ElementTypes.REPEATER, element.UIElementID, (int)element.ParentUIElementID);
                                this._unitOfWork.RepositoryAsync<RepeaterUIElement>().Update(element);

                                List<ServiceResultItem> items = new List<ServiceResultItem>();
                                items.Add(new ServiceResultItem() { Messages = new string[] { element.UIElementID.ToString() } });

                                IEnumerable<UIElementSeqModel> oldChildUIElementList = GetChildUIElements(tenantId, formDesignVersionId, uiElementId);


                                // Add child elements for newly added repeater
                                if (oldChildUIElementList.Count() > 0)
                                {
                                    foreach (var childElement in oldChildUIElementList)
                                    {
                                        UIElementAddModel model = new UIElementAddModel();
                                        model.AddedBy = userName;
                                        model.AddedDate = DateTime.Now;
                                        model.FormDesignVersionID = formDesignVersionId;
                                        model.Label = childElement.Label;
                                        model.ParentUIElementID = (int)element.UIElementID;
                                        model.RoleClaim = childElement.RoleClaim;
                                        model.TenantID = tenantId;
                                        model.ElementType = childElement.ElementType;
                                        result = AddLoadFromServerEnabledRepeaterChildElements(model, userName);

                                    }

                                }

                                IEnumerable<UIElementSeqModel> newChildUIElements = GetChildUIElements(tenantId, formDesignVersionId, element.UIElementID);

                                IDictionary<string, bool> checkDuplicateEnabledUIElementIds = new Dictionary<string, bool>();
                                foreach (var childUIElement in oldChildUIElementList)
                                {
                                    checkDuplicateEnabledUIElementIds.Add(childUIElement.Label, childUIElement.CheckDuplicate);
                                }
                                result = UpdateLoadFromServerEnabledChildCheckDuplicateFields(userName, tenantId, formDesignVersionId, element.UIElementID, checkDuplicateEnabledUIElementIds);

                                if (result.Result == ServiceResultStatus.Success)
                                {
                                    currentMap.UIElementID = element.UIElementID;
                                    currentMap.EffectiveDate = currentMap.FormDesignVersion.EffectiveDate;
                                    currentMap.EffectiveDateOfRemoval = null;

                                    this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Update(currentMap);
                                    this._unitOfWork.Save();
                                    result.Items = items;
                                    result.Result = ServiceResultStatus.Success;
                                }

                                scope.Complete();
                            }

                        }
                    }

                }

                else
                {
                    List<ServiceResultItem> items = new List<ServiceResultItem>();
                    items.Add(new ServiceResultItem() { Messages = new string[] { "No elements found for which LoadFromServer is true in previous version." } });
                    result.Result = ServiceResultStatus.Failure;
                    return result;
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

        /// <summary>
        /// This method is used to validate LoadFromServer control
        /// </summary>
        /// <returns></returns>
        private ServiceResult AllowLoadFromServer(bool isDataSource, int uiElementId)
        {
            ServiceResult result = new ServiceResult();
            string validationMessage = string.Empty;
            result.Result = ServiceResultStatus.Success;
            validationMessage = "Please remove following functionality to enable LoadFromServer control: ";
            // This code is used to check whether a DataSource is used for the child elements of repeater.
            if (isDataSource)
            {
                result.Result = ServiceResultStatus.Failure;
                validationMessage += " IsDataSource";
            }
            if (CheckUseDataSourceForRepeater(uiElementId))
            {
                result.Result = ServiceResultStatus.Failure;
                validationMessage += " UseDataSource";

            }
            IList<PropertyRuleMap> rules = CheckRulesAndCustomRulesEnabled(uiElementId);

            if (rules.Count > 0)
            {
                if (rules.Any(p => (bool)p.IsCustomRule))
                {
                    List<string> customRuleEnabledUIElementsList = new List<string>();
                    validationMessage += " Custom Rules applied to: ";
                    var customRulesList = rules.Where(p => (bool)p.IsCustomRule == true).ToList();
                    foreach (var rule in customRulesList)
                    {
                        string generatedName = rule.UIElement.GeneratedName;
                        validationMessage += "" + generatedName + " ";
                    }
                    result.Result = ServiceResultStatus.Failure;
                }
                if (rules.Where(p => (bool)p.IsCustomRule == true).Count() < rules.Count())
                {
                    validationMessage += " Rules applied to: ";
                    var ruleList = rules.Where(p => (bool)p.IsCustomRule != true).ToList();

                    foreach (var rule in ruleList)
                    {
                        string generatedName = rule.UIElement.GeneratedName;
                        validationMessage += "" + generatedName + " ";
                    }
                    result.Result = ServiceResultStatus.Failure;
                }
            }
            if (result.Result == ServiceResultStatus.Failure)
            {

                List<ServiceResultItem> items = new List<ServiceResultItem>();
                items.Add(new ServiceResultItem { Messages = new string[] { validationMessage } });
                result.Items = items;
                return result;
            }
            // Validation for CheckDuplicate
            if (!IsCheckDuplicateEnabled(uiElementId))
            {
                result.Result = ServiceResultStatus.Failure;

                List<ServiceResultItem> items = new List<ServiceResultItem>();
                items.Add(new ServiceResultItem { Messages = new string[] { "If repeater is to be loaded from server, atleast one 'CheckDuplicate' needs to be checked for the repeater" } });
                result.Items = items;
                return result;
            }
            return result;
        }


        /// <summary>
        /// This method is used to add child elements for newly added LoadFromServer enabled repeater.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        private ServiceResult AddLoadFromServerEnabledRepeaterChildElements(UIElementAddModel model, string userName)
        {
            ServiceResult result = new ServiceResult();
            if (model.ElementType != null)
                switch (model.ElementType.ToUpper())
                {
                    case "TEXTBOX":
                    case "[BLANK]":
                    case "LABEL":
                    case "MULTILINE TEXTBOX":
                        bool isBlank = model.ElementType == "[Blank]" ? true : false;
                        bool isLabel = model.ElementType == "Label" || model.ElementType == "[Blank]" ? true : false;
                        bool isMultiline = model.ElementType == "Multiline TextBox" ? true : false;
                        //by default text box needs to have String as defult data type selected. 2 - aligns to the String Value in UI.ApplicationDataType Table in database.
                        result = AddTextBox(userName, model.TenantID, model.FormDesignVersionID, model.ParentUIElementID, isMultiline, isBlank, isLabel, model.Label, "", 0, model.ElementType, true, 3);
                        break;
                    case "CHECKBOX":
                        result = AddCheckBox(userName, model.TenantID, model.FormDesignVersionID, model.ParentUIElementID, model.Label, "", 0, model.ElementType, true, 3);
                        break;
                    case "DROPDOWN LIST":
                    case "DROPDOWN TEXTBOX":
                        bool isDropDownTextBox = model.ElementType == "Dropdown TextBox" ? true : false;
                        result = AddDropDown(userName, model.TenantID, model.FormDesignVersionID, model.ParentUIElementID, model.Label, "", 0, model.ElementType, isDropDownTextBox, true, 3);
                        break;
                    case "CALENDAR":
                        result = AddCalendar(userName, model.TenantID, model.FormDesignVersionID, model.ParentUIElementID, model.Label, "", 0, model.ElementType, true, 3);
                        break;
                }
            return result;
        }

        /// <summary>
        /// This method is used to update check duplicate values for newly added LoadFromServer enabled elements in repeater
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="tenantId"></param>
        /// <param name="formDesignVersionId"></param>
        /// <param name="parentElementId"></param>
        /// <param name="uiElementId"></param>
        /// <returns></returns>
        private ServiceResult UpdateLoadFromServerEnabledChildCheckDuplicateFields(string userName, int tenantId, int formDesignVersionId, int parentElementId, IDictionary<string, bool> uiElementId)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                var fields = this._unitOfWork.RepositoryAsync<UIElement>()
                                                            .Query()
                                                            .Filter(c => c.ParentUIElementID == parentElementId)
                                                            .Get();
                List<UIElement> fieldList = fields.ToList();

                foreach (UIElement field in fieldList)
                {
                    field.CheckDuplicate = uiElementId[field.Label];
                    field.UpdatedBy = userName;
                    field.UpdatedDate = DateTime.Now;
                    this._unitOfWork.RepositoryAsync<UIElement>().Update(field);
                }
                this._unitOfWork.Save();
                result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                result = ex.ExceptionMessages();
            }
            return result;
        }

        private void DeleteRepeaterUIElementProperties(int uielementID)
        {
            RepeaterUIElementProperties repProp = this._unitOfWork.RepositoryAsync<RepeaterUIElementProperties>().Query().Filter(c => c.RepeaterUIElementID == uielementID).Get().FirstOrDefault();
            if (repProp != null)
            {
                this._unitOfWork.RepositoryAsync<RepeaterUIElementProperties>().Delete(repProp);
            }
        }
        #endregion Private Methods

        private class UIElementMap
        {
            internal int FormDesignVersionId { get; set; }
            internal int UIElementId { get; set; }
            internal bool LoadFromServer { get; set; }
            internal int FormDesignVersionStatusId { get; set; }

        }
    }
}
