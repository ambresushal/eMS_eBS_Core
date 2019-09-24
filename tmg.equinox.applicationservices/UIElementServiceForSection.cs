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
        #region Section Element Methods
        public SectionElementModel GetSectionDesignDetail(int tenantId, int formDesignVersionId, int sectionElementId)
        {
            SectionElementModel viewModel = null;
            try
            {
                SectionUIElement element = this._unitOfWork.RepositoryAsync<SectionUIElement>()
                                                            .Query()
                                                            .Include(c => c.LayoutType)
                                                            .Include(c => c.DataSource)
                                                            .Filter(c => c.UIElementID == sectionElementId)
                                                            .Get()
                                                            .SingleOrDefault();

                if (element != null)
                {
                    viewModel = new SectionElementModel();


                    if (element.DataSource != null &&
                       (this._unitOfWork.RepositoryAsync<FormDesignVersion>().IsFormDesignVersionFinalized(element.DataSource.FormDesignVersionID)))
                    {
                        viewModel.IsDataSourceEnabled = false;
                    }
                    else
                    {
                        viewModel.IsDataSourceEnabled = true;
                    }

                    viewModel.ChildCount = element.ChildCount;
                    viewModel.Enabled = element.Enabled ?? false;
                    viewModel.HasCustomRule = element.HasCustomRule ?? false;
                    viewModel.CustomRule = element.CustomRule;
                    viewModel.Label = this.GetAlternateLabel(formDesignVersionId, sectionElementId) ?? element.Label;
                    viewModel.HelpText = element.HelpText;
                    viewModel.Sequence = element.Sequence;
                    viewModel.LayoutType = element.LayoutType.DisplayText;
                    viewModel.LayoutTypeID = element.LayoutTypeID;
                    viewModel.ParentUIElementID = element.ParentUIElementID ?? 0;
                    viewModel.UIElementID = element.UIElementID;
                    viewModel.Visible = element.Visible ?? false;
                    viewModel.TenantID = tenantId;
                    viewModel.IsDataSource = element.DataSource != null ? true : false;
                    viewModel.DataSourceName = element.DataSource != null ? element.DataSource.DataSourceName : string.Empty;
                    viewModel.FormDesignVersionID = formDesignVersionId;
                    viewModel.DataSourceDescription = element.DataSource != null ? element.DataSource.DataSourceDescription : string.Empty;
                    viewModel.CustomHtml = element.CustomHtml;
                    viewModel.ViewType = element.ViewType;
                    viewModel.IsStandard = element.IsStandard;
                    viewModel.MDMName = element.MDMName;
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

        public ServiceResult AddSectionDesign(string userName, int tenantId, int formDesignVersionId, int parentUIElementId, string label, string elementType, bool isStandard, int viewType)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                SectionUIElement sectionElement = new SectionUIElement();

                FormDesignVersion formDesignVersion = this._unitOfWork.RepositoryAsync<FormDesignVersion>()
                                                                        .Find(formDesignVersionId);

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

                int layOutTypeID = this._unitOfWork.RepositoryAsync<LayoutType>().Query().Filter(l => l.LayoutTypeCode == "1COL").Get().Select(l => l.LayoutTypeID).FirstOrDefault(); // For 1Column grid layout

                int childCount = this._unitOfWork.RepositoryAsync<UIElement>()
                                                    .Query()
                                                    .Filter(c => c.ParentUIElementID == parentUIElementId)
                                                    .Get()
                                                    .Count();

                sectionElement.LayoutTypeID = layOutTypeID;
                sectionElement.ChildCount = childCount + 1;

                //Retrieve the UIElementType for Section, assuming here that it's code in database will be SEC 
                sectionElement.UIElementTypeID = uiElementTypeId;

                sectionElement.AddedBy = userName;
                sectionElement.AddedDate = DateTime.Now;                                       //NOTE - Setting up default values here
                sectionElement.UIElementName = this.GetUniqueName(formDesignVersion.FormDesignID ?? 0, ElementTypes.SECTION, sectionElement.UIElementID, parentUIElementId);
                sectionElement.Enabled = true;                                                 //Enable
                sectionElement.HasCustomRule = false;                                          //HasCustomRule
                sectionElement.IsActive = true;                                                //IsActive
                sectionElement.IsContainer = true;                                             //IsContainer
                sectionElement.Label = label;
                sectionElement.GeneratedName = GetGeneratedName(label);
                sectionElement.FormID = formDesignVersion.FormDesignID ?? 0;
                sectionElement.ParentUIElementID = parentUIElementId;
                sectionElement.RequiresValidation = false;                                     //RequiredValidation 
                sectionElement.Sequence = sequenceNo + 1;
                sectionElement.Visible = true;
                sectionElement.UIElementDataTypeID = uiElementDataTypeId;
                sectionElement.CustomHtml = null;
                sectionElement.ViewType = viewType;
                sectionElement.IsStandard = isStandard;
                using (var scope = new TransactionScope())
                {
                    this._unitOfWork.RepositoryAsync<SectionUIElement>().Insert(sectionElement);
                    this._unitOfWork.Save();

                    sectionElement.UIElementName = this.GetUniqueName(formDesignVersion.FormDesignID ?? 0, ElementTypes.SECTION, sectionElement.UIElementID, parentUIElementId);
                    this._unitOfWork.RepositoryAsync<SectionUIElement>().Update(sectionElement);


                    //add FormDesignVersionUIElementMap record
                    FormDesignVersionUIElementMap map = new FormDesignVersionUIElementMap();
                    map.FormDesignVersionID = formDesignVersionId;
                    map.EffectiveDate = formDesignVersion.EffectiveDate;
                    map.UIElementID = sectionElement.UIElementID;
                    this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Insert(map);
                    this._unitOfWork.Save();

                    scope.Complete();
                    IList<ServiceResultItem> item = new List<ServiceResultItem>();
                    item.Add(new ServiceResultItem { Messages = new string[] { sectionElement.UIElementID.ToString() } });
                    item.Add(new ServiceResultItem { Messages = new string[] { GetElementType(sectionElement) } });
                    result.Items = item;
                    result.Result = ServiceResultStatus.Success;
                }
                //setClaims for new section
                if (result.Result == ServiceResultStatus.Success)
                    identitymanagement.IdentityManager.AddDefaultResourceClaims(identitymanagement.Enums.ResourceType.SECTION, sectionElement.UIElementID);

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                result = ex.ExceptionMessages();
            }
            return result;
        }

        public ServiceResult AddSectionDesign(string userName, int tenantId, int formDesignVersionId, int parentUIElementId, string label, string elementType, bool isEnable, bool isVisible, int sequence, IEnumerable<RuleRowModel> rules, bool modifyRules, string extProp, string layout, int viewType, int sourceUIElementId, string customHtml)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                SectionUIElement sectionElement = new SectionUIElement();

                FormDesignVersion formDesignVersion = this._unitOfWork.RepositoryAsync<FormDesignVersion>()
                                                                        .Find(formDesignVersionId);

                var fields = this._unitOfWork.RepositoryAsync<UIElement>()
                                                            .Query()
                                                            .Filter(c => (c.ParentUIElementID == parentUIElementId && c.FormID == formDesignVersion.FormDesignID))
                                                            .Get();
                var uiElementDataTypeId = this._unitOfWork.RepositoryAsync<ApplicationDataType>().GetUIElementDataTypeID(elementType);
                var uiElementTypeId = this._unitOfWork.RepositoryAsync<UIElementType>().GetUIElementTypeID(elementType);
                int layOutTypeID = this._unitOfWork.RepositoryAsync<LayoutType>().Get().Where(t => t.DisplayText == layout).Select(s => s.LayoutTypeID).FirstOrDefault();

                int sequenceNo = 0;
                if (fields != null && fields.Count() > 0 && sequence == 0)
                {
                    sequenceNo = fields.Max(c => c.Sequence);
                }

                int childCount = this._unitOfWork.RepositoryAsync<UIElement>()
                                                    .Query()
                                                    .Filter(c => c.ParentUIElementID == parentUIElementId)
                                                    .Get()
                                                    .Count();

                sectionElement.LayoutTypeID = layOutTypeID == 0 ? 1 : layOutTypeID;
                sectionElement.ChildCount = childCount + 1;

                //Retrieve the UIElementType for Section, assuming here that it's code in database will be SEC 
                sectionElement.UIElementTypeID = uiElementTypeId;

                sectionElement.AddedBy = userName;
                sectionElement.AddedDate = DateTime.Now;                                       //NOTE - Setting up default values here
                sectionElement.UIElementName = this.GetUniqueName(formDesignVersion.FormDesignID ?? 0, ElementTypes.SECTION, sectionElement.UIElementID, parentUIElementId);
                sectionElement.Enabled = isEnable;                                                 //Enable
                sectionElement.HasCustomRule = false;                                          //HasCustomRule
                sectionElement.IsActive = true;                                                //IsActive
                sectionElement.IsContainer = true;                                             //IsContainer
                sectionElement.Label = label;
                sectionElement.GeneratedName = GetGeneratedName(label);
                sectionElement.FormID = formDesignVersion.FormDesignID ?? 0;
                sectionElement.ParentUIElementID = parentUIElementId;
                sectionElement.RequiresValidation = false;                                     //RequiredValidation 
                sectionElement.Sequence = sequence == 0 ? sequenceNo + 1 : sequence;
                sectionElement.Visible = isVisible;
                sectionElement.UIElementDataTypeID = uiElementDataTypeId;
                sectionElement.CustomHtml = customHtml;
                sectionElement.ExtendedProperties = extProp;
                sectionElement.ViewType = viewType;

                using (var scope = new TransactionScope())
                {
                    this._unitOfWork.RepositoryAsync<SectionUIElement>().Insert(sectionElement);
                    this._unitOfWork.Save();

                    sectionElement.UIElementName = this.GetUniqueName(formDesignVersion.FormDesignID ?? 0, ElementTypes.SECTION, sectionElement.UIElementID, parentUIElementId);
                    this._unitOfWork.RepositoryAsync<SectionUIElement>().Update(sectionElement);

                    //change Rules
                    if (modifyRules == true && rules != null)
                    {
                        ChangeRules(userName, tenantId, formDesignVersionId, sectionElement.UIElementID, sectionElement.UIElementID, rules, false);
                    }

                    //add FormDesignVersionUIElementMap record
                    FormDesignVersionUIElementMap map = new FormDesignVersionUIElementMap();
                    map.FormDesignVersionID = formDesignVersionId;
                    map.EffectiveDate = formDesignVersion.EffectiveDate;
                    map.UIElementID = sectionElement.UIElementID;
                    this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Insert(map);
                    this._unitOfWork.Save();

                    scope.Complete();
                    IList<ServiceResultItem> item = new List<ServiceResultItem>();
                    item.Add(new ServiceResultItem { Messages = new string[] { sectionElement.UIElementID.ToString() } });
                    item.Add(new ServiceResultItem { Messages = new string[] { GetElementType(sectionElement) } });
                    result.Items = item;
                    result.Result = ServiceResultStatus.Success;
                }
                //setClaims for new section
                if (result.Result == ServiceResultStatus.Success)
                    identitymanagement.IdentityManager.AddDefaultResourceClaims(identitymanagement.Enums.ResourceType.SECTION, sectionElement.UIElementID);

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                result = ex.ExceptionMessages();
            }
            return result;
        }

        public ServiceResult UpdateSectionDesign(string userName, int tenantId, int formDesignId, int formDesignVersionId, int sectionElementId, bool isEnabled, string helpText, bool isRequired, bool hasCustomRule, string label, int layoutTypeId, bool isVisible, bool isDataSource, string datasourceName, string dataSourceDescription, IEnumerable<RuleRowModel> rules, bool modifyRules, bool modifyCustomRules, string customRule, string customHtml, int viewType, bool isStandard,string mdmName)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                SectionUIElement sectionElement = this._unitOfWork.RepositoryAsync<SectionUIElement>()
                                                                    .FindById(sectionElementId);

                //bool isFormDesignVersionFinzalized = this._unitOfWork.RepositoryAsync<FormDesignVersion>().IsFormDesignVersionFinalized(formDesignVersionId);
                //if (isFormDesignVersionFinzalized)
                //{
                //    result.Result = ServiceResultStatus.Failure;
                //    result.Items.ToList().Add(new ServiceResultItem { Messages = new string[] { "Unable to Update finalized Form Design Version" } });
                //}
                //else
                {
                    bool isUsedInFinalizedFormVersion = this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>()
                                                                        .Query()
                                                                        .Include(c => c.FormDesignVersion)
                                                                        .Include(c => c.FormDesignVersion.Status)
                                                                        .Filter(c => c.UIElementID == sectionElementId)
                                                                        .Filter(c => c.FormDesignVersion.Status.Status1 == viewmodels.Status.FINALIZED)
                                                                        .Get()
                                                                        .Any();
                    if (isUsedInFinalizedFormVersion)
                    {
                        result.Result = ServiceResultStatus.Failure;
                        result.Items.ToList().Add(new ServiceResultItem { Messages = new string[] { "Unable to Update finalized Form Design Version" } });
                    }
                }

                if (sectionElementId != null)
                {
                    if (isDataSource)
                    {
                        if (!CheckForUniqueDataSourceName(sectionElement.DataSourceID, datasourceName))
                        {
                            result.Result = ServiceResultStatus.Failure;

                            List<ServiceResultItem> items = new List<ServiceResultItem>();
                            items.Add(new ServiceResultItem { Messages = new string[] { "Please enter unique data source name.", sectionElementId.ToString() } });
                            result.Items = items;
                            return result;
                        }
                    }
                    else
                    {
                        if (CheckDataSourceisInUse(sectionElement.DataSourceID))
                        {
                            result.Result = ServiceResultStatus.Failure;

                            List<ServiceResultItem> items = new List<ServiceResultItem>();
                            items.Add(new ServiceResultItem { Messages = new string[] { "Data source is already in use. You can not delete it.", sectionElementId.ToString() } });
                            result.Items = items;
                            return result;
                        }

                    }


                    //code to update parameters here. 
                    sectionElement.LayoutTypeID = layoutTypeId;
                    sectionElement.Enabled = isEnabled;
                    sectionElement.HelpText = helpText;
                    sectionElement.HasCustomRule = hasCustomRule;
                    sectionElement.RequiresValidation = isRequired;
                    //sectionElement.Label = label;
                    sectionElement.GeneratedName = GetGeneratedName(sectionElement.Label);
                    sectionElement.Visible = isVisible;
                    sectionElement.CustomHtml = customHtml;
                    sectionElement.ViewType = viewType;
                    sectionElement.IsStandard = isStandard;
                    sectionElement.MDMName = mdmName;               
                    if (modifyCustomRules)
                    {
                        sectionElement.HasCustomRule = hasCustomRule;
                        sectionElement.CustomRule = customRule;
                    }



                    //Update the datasource  name if datasource already present
                    DataSource sectionDataSource = this._unitOfWork.RepositoryAsync<DataSource>()
                                                          .Query()
                                                          .Filter(c => c.DataSourceID == sectionElement.DataSourceID
                                                                        && c.FormDesignID == formDesignId)
                                                          .Get()
                                                          .FirstOrDefault();



                    if (sectionDataSource != null && isDataSource == true)
                    {
                        if (!(this._unitOfWork.RepositoryAsync<FormDesignVersion>().IsFormDesignVersionFinalized(sectionDataSource.FormDesignVersionID)))
                        {
                            sectionDataSource.DataSourceName = datasourceName;
                            sectionDataSource.DataSourceDescription = dataSourceDescription;
                            this._unitOfWork.RepositoryAsync<DataSource>().Update(sectionDataSource);
                        }
                        else
                        {
                            result.Result = ServiceResultStatus.Failure;
                            List<ServiceResultItem> items = new List<ServiceResultItem>();
                            items.Add(new ServiceResultItem { Messages = new string[] { "Data source is associated with Finalized FormDesignVersion. You can not Update it.", sectionElementId.ToString() } });
                            result.Items = items;
                            return result;
                        }
                    }
                    else if (sectionDataSource == null && isDataSource == true)
                    {
                        DataSource newsectionDataSource = new DataSource();
                        newsectionDataSource.FormDesignID = formDesignId;
                        newsectionDataSource.FormDesignVersionID = formDesignVersionId;
                        newsectionDataSource.DataSourceName = datasourceName;
                        newsectionDataSource.DataSourceDescription = dataSourceDescription;
                        newsectionDataSource.Type = "Section";
                        newsectionDataSource.AddedBy = userName;
                        newsectionDataSource.AddedDate = DateTime.Now;
                        this._unitOfWork.RepositoryAsync<DataSource>().Insert(newsectionDataSource);
                        sectionElement.DataSource = newsectionDataSource;
                    }
                    else if (isDataSource == false)
                    {
                        if (sectionDataSource != null)
                        {

                            if (!(this._unitOfWork.RepositoryAsync<FormDesignVersion>().IsFormDesignVersionFinalized(sectionDataSource.FormDesignVersionID)))
                            {
                                this._unitOfWork.RepositoryAsync<DataSource>().Delete(sectionDataSource);
                            }
                            else
                            {
                                result.Result = ServiceResultStatus.Failure;
                                List<ServiceResultItem> items = new List<ServiceResultItem>();
                                items.Add(new ServiceResultItem { Messages = new string[] { "Data source is associated with Finalized FormDesignVersion. You can not delete it.", sectionElementId.ToString() } });
                                result.Items = items;
                                return result;
                            }
                        }
                    }

                    //change Rules
                    if (modifyRules == true && rules != null)
                    {
                        ChangeRules(userName, tenantId, formDesignVersionId, sectionElementId, sectionElementId, rules, false);
                    }
                    //If all the rules deleted from UI.
                    else if (modifyRules == true && rules == null)
                    {
                        IEnumerable<RuleRowModel> currentRules = GetRulesForUIElement(tenantId, formDesignVersionId, sectionElementId);
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

                    this._unitOfWork.RepositoryAsync<SectionUIElement>().Update(sectionElement);
                    this._unitOfWork.Save();

                    //Update Alternate Label
                    if (!string.Equals(sectionElement.Label, label) || this.isExistsInAlternateLabel(sectionElement.UIElementID))
                    {
                        this.UpdateAlternateLabel(tenantId, formDesignId, formDesignVersionId, sectionElement.UIElementID, label);
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

        public ServiceResult UpdateSectionDesign(string userName, int tenantId, int formDesignId, int formDesignVersionId, int sectionElementId, bool isEnabled, string helpText, bool isRequired, string label, bool isVisible, IEnumerable<RuleRowModel> rules, bool modifyRules, int viewType, int parentUIElementId, string extProp, string layout, string customHtml, bool isStandard)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                SectionUIElement sectionElement = this._unitOfWork.RepositoryAsync<SectionUIElement>()
                                                                    .FindById(sectionElementId);
                int layOutTypeID = this._unitOfWork.RepositoryAsync<LayoutType>().Get().Where(t => t.DisplayText == layout).Select(s => s.LayoutTypeID).FirstOrDefault();

                bool isFormDesignVersionFinzalized = this._unitOfWork.RepositoryAsync<FormDesignVersion>().IsFormDesignVersionFinalized(formDesignVersionId);
                if (isFormDesignVersionFinzalized)
                {
                    result.Result = ServiceResultStatus.Failure;
                    result.Items.ToList().Add(new ServiceResultItem { Messages = new string[] { "Unable to Update finalized Form Design Version" } });
                }
                else
                {
                    bool isUsedInFinalizedFormVersion = this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>()
                                                                        .Query()
                                                                        .Include(c => c.FormDesignVersion)
                                                                        .Include(c => c.FormDesignVersion.Status)
                                                                        .Filter(c => c.UIElementID == sectionElementId)
                                                                        .Filter(c => c.FormDesignVersion.Status.Status1 == viewmodels.Status.FINALIZED)
                                                                        .Get()
                                                                        .Any();
                    if (isUsedInFinalizedFormVersion)
                    {
                        result.Result = ServiceResultStatus.Failure;
                        result.Items.ToList().Add(new ServiceResultItem { Messages = new string[] { "Unable to Update finalized Form Design Version" } });
                    }
                }

                if (sectionElementId != null)
                {

                    //code to update parameters here. 
                    sectionElement.Enabled = isEnabled;
                    sectionElement.LayoutTypeID = layOutTypeID == 0 ? 1 : layOutTypeID;
                    sectionElement.HelpText = helpText;
                    sectionElement.RequiresValidation = isRequired;
                    sectionElement.GeneratedName = GetGeneratedName(sectionElement.Label);
                    sectionElement.Visible = isVisible;
                    sectionElement.ViewType = viewType;
                    sectionElement.ParentUIElementID = parentUIElementId;
                    sectionElement.ExtendedProperties = extProp;
                    sectionElement.ViewType = viewType;
                    sectionElement.IsStandard = isStandard;
                    sectionElement.CustomHtml = customHtml;

                    //Update the datasource  name if datasource already present
                    DataSource sectionDataSource = this._unitOfWork.RepositoryAsync<DataSource>()
                                                          .Query()
                                                          .Filter(c => c.DataSourceID == sectionElement.DataSourceID
                                                                        && c.FormDesignID == formDesignId)
                                                          .Get()
                                                          .FirstOrDefault();

                    //change Rules
                    if (modifyRules == true && rules != null)
                    {
                        ChangeRules(userName, tenantId, formDesignVersionId, sectionElementId, sectionElementId, rules, false);
                    }

                    //If all the rules deleted from UI.
                    else if (modifyRules == true && rules == null)
                    {
                        IEnumerable<RuleRowModel> currentRules = GetRulesForUIElement(tenantId, formDesignVersionId, sectionElementId);
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

                    this._unitOfWork.RepositoryAsync<SectionUIElement>().Update(sectionElement);
                    this._unitOfWork.Save();

                    //Update Alternate Label
                    if (!string.Equals(sectionElement.Label, label) || this.isExistsInAlternateLabel(sectionElement.UIElementID))
                    {
                        this.UpdateAlternateLabel(tenantId, formDesignId, formDesignVersionId, sectionElement.UIElementID, label);
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

        private bool CheckDataSourceisInUse(int? DataSourceID)
        {
            if (DataSourceID != null)
            {
                var dataSourceExists = this._unitOfWork.RepositoryAsync<DataSourceMapping>()
                    .Query()
                    .Filter(c => c.DataSourceID == DataSourceID.Value)
                    .Get().FirstOrDefault();

                if (dataSourceExists == null)
                    return false;
                else
                    return true;
            }
            return false;
        }

        private bool CheckForUniqueDataSourceName(int? DataSourceID, string datasourceName)
        {
            if (DataSourceID != null)
            {
                var dataSourceExists = this._unitOfWork.RepositoryAsync<DataSource>().Query()
                    .Filter(c => c.DataSourceID != DataSourceID.Value && c.DataSourceName == datasourceName)
                            .Get().FirstOrDefault();

                if (dataSourceExists != null)
                    return false;
                else
                    return true;
            }
            else
            {
                var dataSourceExists = this._unitOfWork.RepositoryAsync<DataSource>()
                                           .Query()
                                           .Filter(c => c.DataSourceName == datasourceName)
                                           .Get().FirstOrDefault();
                if (dataSourceExists != null)
                    return false;
                else
                    return true;
            }
            return true;
        }

        public IEnumerable<SectionElementModel> GetSectionList(int tenantId, int formDesignVersionId, int uiElementId)
        {
            IList<SectionElementModel> sectionModelList = null;
            try
            {

                var sections = this._unitOfWork.RepositoryAsync<SectionUIElement>()
                                                            .Query()
                                                            .Include(c => c.LayoutType)
                                                            .Include(c => c.FormDesignVersionUIElementMaps)
                                                            .Filter(c => (c.FormDesignVersionUIElementMaps.Any(f => f.FormDesignVersionID == formDesignVersionId) && c.ParentUIElementID == uiElementId))
                                                            .Get();
                var sectionModels = from se in sections
                                    select new SectionElementModel
                                    {
                                        ChildCount = se.ChildCount,
                                        Enabled = se.Enabled ?? false,
                                        HasCustomRule = se.HasCustomRule ?? false,
                                        HelpText = se.HelpText,
                                        LayoutTypeID = se.LayoutTypeID,
                                        Label = se.Label,
                                        Sequence = se.Sequence,
                                        LayoutType = se.LayoutType.DisplayText,
                                        ParentUIElementID = se.ParentUIElementID ?? 0,
                                        UIElementID = se.UIElementID,
                                        Visible = se.Visible ?? false,
                                        TenantID = tenantId,
                                        FormDesignVersionID = formDesignVersionId
                                    };

                sectionModelList = sectionModels.ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return sectionModelList;
        }

        public ServiceResult UpdateSectionSequences(string userName, int tenantId, int formDesignVersionId, IDictionary<int, int> uiElementIdSequences)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                var sections = this._unitOfWork.RepositoryAsync<SectionUIElement>()
                                                            .Query()
                                                            .Filter(c => (uiElementIdSequences.Keys.Contains(c.UIElementID)))
                                                            .Get();
                List<SectionUIElement> sectionList = sections.ToList();

                foreach (SectionUIElement section in sectionList)
                {
                    section.Sequence = uiElementIdSequences[section.UIElementID];
                    section.UpdatedBy = userName;
                    section.UpdatedDate = DateTime.Now;
                    this._unitOfWork.RepositoryAsync<SectionUIElement>().Update(section);
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

        public ServiceResult DeleteSection(int tenantId, int formDesignVersionId, int uiElementId)
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
                if (result.Result == ServiceResultStatus.Success)
                    identitymanagement.IdentityManager.RemoveResourceClaims(identitymanagement.Enums.ResourceType.SECTION, uiElementId);
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
        #endregion Section Element Methods
        #endregion Public Methods

        #region Private Methods

        #endregion Private Methods
    }
}
