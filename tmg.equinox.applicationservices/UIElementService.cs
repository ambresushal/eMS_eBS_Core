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
using System.Text.RegularExpressions;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.domain.entities;
using tmg.equinox.applicationservices.viewmodels.CompareSync;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using tmg.equinox.applicationservices.viewmodels.FolderVersion;
using tmg.equinox.applicationservices.viewmodels.FormDesign;

namespace tmg.equinox.applicationservices
{
    public partial class UIElementService : IUIElementService
    {
        #region Private Members
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor
        public UIElementService(IUnitOfWorkAsync unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion Constructor

        #region Public Methods
        private string GetUIElementParentName(int? parentElementId, List<UIElement> elementList, List<AlternateUIElementLabel> alternateLabels)
        {
            string parentName = "";
            try
            {
                if (parentElementId != null)
                {
                    var result = (from element in elementList
                                  where element.UIElementID == parentElementId
                                  select element).FirstOrDefault();
                    if (result != null)
                    {
                        parentName = alternateLabels.Where(e => e.UIElementID == parentElementId).Select(s => s.AlternateLabel).FirstOrDefault() ?? result.Label;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return parentName;
        }

        private string GetUIElementParentPath(int? parentElementId, List<UIElement> elementList, List<AlternateUIElementLabel> alternateLabels, bool removeTab)
        {
            List<string> parentNames = new List<string>();
            try
            {
                if (parentElementId != null)
                {
                    while (parentElementId != null)
                    {
                        var result = (from element in elementList where element.UIElementID == parentElementId select element).FirstOrDefault();
                        if (result != null)
                        {
                            string lbl = alternateLabels.Where(e => e.UIElementID == parentElementId).Select(s => s.AlternateLabel).FirstOrDefault() ?? result.Label;
                            parentNames.Add(lbl);
                            parentElementId = result.ParentUIElementID;
                        }
                    }
                    parentNames.Reverse();
                    if (parentNames.Count > 1 || removeTab)
                    {
                        parentNames.RemoveAt(0);
                    }
                    return string.Join("=>", parentNames.ToArray());
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return "";
        }

        public List<FormInstanceViewModel> GetInstances(int formDesignVersionId)
        {
            List<FormInstanceViewModel> formInstanceViewModel = new List<FormInstanceViewModel>();
            formInstanceViewModel = (from fi in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                                     where fi.FolderVersionID == formDesignVersionId
                                     select new FormInstanceViewModel
                                     {
                                         FormInstanceID = fi.FormInstanceID,
                                         Name = fi.Name
                                     }).ToList();

            return formInstanceViewModel;
        }
        public List<UIElementRowModel> GetRepeaterElementForLookIn(int formDesignVersionId)
        {
            List<UIElementRowModel> elementList = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@formDesignVersionId", System.Data.SqlDbType.Int);
                param[0].Value = formDesignVersionId;

                var result = this._unitOfWork.RepositoryAsync<ElementLookIn>().ExecuteSql("exec [dbo].[uspGetRepeaterElementList] @formDesignVersionId", param).ToList();
                elementList = (from row in result
                               select new UIElementRowModel
                               {
                                   UIElementID = row.UIElementID,
                                   Label = row.Label,
                                   GeneratedName = row.GeneratedName
                               }).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return elementList;
        }

        public List<UIElement> GetUIElementsListByFormDesignId(int tenantId, int formDesignVersionId)
        {
            var elementList = (from ele in this._unitOfWork.RepositoryAsync<UIElement>().Query().Get()
                               join fde in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Query().Get() on ele.UIElementID equals fde.UIElementID
                               join prm in this._unitOfWork.RepositoryAsync<PropertyRuleMap>().Query().Get() on ele.UIElementID equals prm.UIElementID
                               join r in this._unitOfWork.RepositoryAsync<Rule>().Query().Get() on prm.RuleID equals r.RuleID
                               where r.RuleDescription == null && fde.FormDesignVersionID == formDesignVersionId
                               select ele).ToList();

            return elementList;
        }
        public ConfigViewRowModel GetUIElementList(int tenantId, int formDesignVersionId)
        {
            ConfigViewRowModel uiElementList = new ConfigViewRowModel();

            try
            {
                var elementList = this._unitOfWork.RepositoryAsync<UIElement>().GetUIElementListForFormDesignVersion(tenantId, formDesignVersionId).ToList();
                var alternateLabels = this._unitOfWork.RepositoryAsync<AlternateUIElementLabel>().Get().Where(s => s.FormDesignVersionID == formDesignVersionId).ToList();
                var validations = (from val in this._unitOfWork.RepositoryAsync<Validator>().Query().Get()
                                   join e in this._unitOfWork.RepositoryAsync<UIElement>().Query().Get() on val.UIElementID equals e.UIElementID
                                   join ver in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Get() on e.UIElementID equals ver.UIElementID
                                   where ver.FormDesignVersionID == formDesignVersionId
                                   select val).ToList();

                var dropdownItems = (from val in this._unitOfWork.RepositoryAsync<DropDownElementItem>().Query().Get()
                                     join e in this._unitOfWork.RepositoryAsync<UIElement>().Query().Get() on val.UIElementID equals e.UIElementID
                                     join ver in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Get() on e.UIElementID equals ver.UIElementID
                                     where ver.FormDesignVersionID == formDesignVersionId
                                     select val).ToList();

                var repeaterKeys = (from key in this._unitOfWork.RepositoryAsync<RepeaterKeyUIElement>().Query().Get()
                                    join e in this._unitOfWork.RepositoryAsync<UIElement>().Query().Get() on key.UIElementID equals e.UIElementID
                                    join ver in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Get() on e.UIElementID equals ver.UIElementID
                                    where ver.FormDesignVersionID == formDesignVersionId
                                    select key).ToList();

                var layouts = this._unitOfWork.RepositoryAsync<tmg.equinox.domain.entities.Models.LayoutType>().Query().Get().ToList();

                var formats = (from frm in this._unitOfWork.RepositoryAsync<RegexLibrary>().Query().Get()
                               select frm).ToDictionary(kvp => kvp.LibraryRegexID, kvp => kvp.LibraryRegexName);

                if (elementList.Count() > 0)
                {
                    var elements = (from i in elementList
                                    select new UIElementRowViewModel
                                    {
                                        DataType = i is TextBoxUIElement ? i.ApplicationDataType.DisplayText : "",
                                        ApplicationDataTypeID = i.UIElementDataTypeID,
                                        ElementType = GetElementType(i),
                                        HasOptions = i is DropDownUIElement ? "10" : i is RadioButtonUIElement ? "20" : i is SectionUIElement || i is RepeaterUIElement ? "01" : "00",
                                        IsMultiselect = i is DropDownUIElement ? ((DropDownUIElement)i).IsMultiSelect : false,
                                        Label = alternateLabels.Where(e => e.UIElementID == i.UIElementID).Select(s => s.AlternateLabel).FirstOrDefault() ?? (i.Label == null ? "[Blank]" : i.Label),
                                        MaxLength = i is TextBoxUIElement ? Convert.ToString(((TextBoxUIElement)i).MaxLength) : "",
                                        Required = i.RequiresValidation,
                                        Sequence = i.Sequence,
                                        UIElementID = i.UIElementID,
                                        UIElementName = i.UIElementName,
                                        Parent = GetUIElementParentPath(i.ParentUIElementID, elementList, alternateLabels, false),
                                        ParentUIElementId = i.ParentUIElementID,
                                        IsKey = repeaterKeys.Where(s => s.UIElementID == i.UIElementID).Count() > 0 ? true : false,
                                        IsVisible = i.Visible,
                                        IsEnable = i.Enabled,
                                        HasRules = i is SectionUIElement || i is RepeaterUIElement ? "01" : "00",
                                        HasExpRules = i is SectionUIElement || i is RepeaterUIElement ? "01" : "00",
                                        DataSource = i is DropDownUIElement ? "10" : i is SectionUIElement || i is RepeaterUIElement ? "11" : "00",
                                        Formats = GetValidatorFormat(i, validations, formats),
                                        LibraryRegexID = GetFormatLibraryID(i, validations, formats),
                                        HelpText = i.HelpText,
                                        IsContainer = i.IsContainer,
                                        Items = (from c in dropdownItems
                                                 where c.UIElementID == i.UIElementID
                                                 select new DropDownItemModel
                                                 {
                                                     ItemID = c.ItemID,
                                                     Value = c.Value,
                                                     DisplayText = c.DisplayText,
                                                     Sequence = c.Sequence.HasValue ? c.Sequence.Value : 0
                                                 }).ToList(),
                                        HasConfig = i is RepeaterUIElement ? "10" : i is SectionUIElement ? "01" : "00",
                                        HasRoleAccess = i is SectionUIElement ? "11" : i is RepeaterUIElement ? "01" : "00",
                                        AdvancedConfiguration = i is RepeaterUIElement ? GetAdvanceConfiguration(i) : null,
                                        ExtProp = string.IsNullOrEmpty(i.ExtendedProperties) ? new JObject() : JObject.Parse(i.ExtendedProperties),
                                        Layout = GetLayoutTypeNameById(i, layouts),
                                        DefaultValue = GetDefaultValue(i),
                                        ViewType = i.ViewType == 1 ? "Folder View" : i.ViewType == 2 ? "SOT View" : "Both",
                                        AllowBulkUpdate = i is RepeaterUIElement ? ((RepeaterUIElement)i).AllowBulkUpdate : false,
                                        OptionYes = i is RadioButtonUIElement ? ((RadioButtonUIElement)i).OptionLabel : "",
                                        OptionNo = i is RadioButtonUIElement ? ((RadioButtonUIElement)i).OptionLabelNo : "",
                                        CustomHtml = i is SectionUIElement ? ((SectionUIElement)i).CustomHtml : "",
                                        IsSameSectionRuleSource = i.IsSameSectionRuleSource
                                    }).ToList();

                    if (elements.Count > 0)
                    {
                        uiElementList.SectionElements = new List<ParentList>();
                        elements.ForEach(e => { e.Element = e.Label; e.Section = e.Parent; });
                        var parentList = (from e in elements where e.ElementType == "Section" || e.ElementType == "Repeater" || e.ElementType == "Tab" select e).ToList();
                        foreach (var item in parentList)
                        {
                            ParentList parent = new ParentList();
                            parent.Value = item.UIElementID;
                            parent.ElementType = item.ElementType;
                            var parentName = GetUIElementParentPath(item.ParentUIElementId, elementList, alternateLabels, true);
                            parentName = string.IsNullOrEmpty(parentName) ? item.Label : (parentName + "=>" + item.Label);
                            parent.DisplayText = parentName;
                            uiElementList.SectionElements.Add(parent);
                        }

                        elements.RemoveAt(0);
                        uiElementList.UIElementList = elements;
                    }

                    uiElementList.DataTypes = (from dt in this._unitOfWork.RepositoryAsync<domain.entities.Models.ApplicationDataType>().Get()
                                               select new DropdownList
                                               {
                                                   Value = dt.ApplicationDataTypeID,
                                                   DisplayText = dt.DisplayText
                                               }).ToList();

                    uiElementList.DataFormats = (from dt in this._unitOfWork.RepositoryAsync<domain.entities.Models.RegexLibrary>().Get()
                                                 select new DropdownList
                                                 {
                                                     Value = dt.LibraryRegexID,
                                                     DisplayText = dt.LibraryRegexName.TrimEnd()
                                                 }).ToList();

                    uiElementList.Comments = new List<Comments>();
                    uiElementList.ExtendedProperties = new List<ExtendedProperties>();

                    var designVersion = this._unitOfWork.RepositoryAsync<FormDesignVersionExt>().Get().Where(s => s.FormDesignVersionID == formDesignVersionId).FirstOrDefault();
                    if (designVersion != null)
                    {
                        if (!string.IsNullOrEmpty(designVersion.Comments))
                            uiElementList.Comments = JsonConvert.DeserializeObject<List<Comments>>(designVersion.Comments);

                        if (!string.IsNullOrEmpty(designVersion.ExtendedColNames))
                            uiElementList.ExtendedProperties = JsonConvert.DeserializeObject<List<ExtendedProperties>>(designVersion.ExtendedColNames);
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return uiElementList;
        }

        public IEnumerable<ElementListViewModel> GetUIElementListForExpressionRuleBuilder(int tenantId, int formDesignVersionId, string searchFor = "")
        {
            IList<ElementListViewModel> uiElementList = null;
            try
            {
                SqlParameter paramFrmDesignVersionID = new SqlParameter("@FormDesignVersionID", formDesignVersionId);
                SqlParameter paramSearchFor = new SqlParameter("@SearchFor", searchFor);
                List<UIElementListForExpressionBuilder> frmDesignVersionElementList = this._unitOfWork.Repository<UIElementListForExpressionBuilder>()
                                                                                .ExecuteSql("exec [dbo].[uspGetElementListForExpressionBuilder] @FormDesignVersionID, @SearchFor", paramFrmDesignVersionID, paramSearchFor)
                                                                                .ToList();
                uiElementList = (from ele in frmDesignVersionElementList
                                 select new ElementListViewModel()
                                 {
                                     UIElementID = ele.UIElementID,
                                     UIElementName = ele.UIElementName,
                                     ElementLabel = ele.ElementLabel,
                                     ElementFullPath = ele.ElementFullPath,
                                     ElementJSONPath = ele.ElementJSONPath,
                                     Parent = ele.Parent,
                                     IsContainer = ele.IsContainer
                                 }).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return uiElementList;
        }

        public List<UIElementSummaryModel> GetUIElementListMod(int tenantId, int formDesignVersionId)
        {
            List<UIElementSummaryModel> resultElements = new List<UIElementSummaryModel>();


            try
            {
                var elementList = this._unitOfWork.RepositoryAsync<UIElement>().GetUIElementListForFormDesignVersion(tenantId, formDesignVersionId).ToList();
                var alternateLabels = this._unitOfWork.RepositoryAsync<AlternateUIElementLabel>().Get().Where(s => s.FormDesignVersionID == formDesignVersionId).ToList();

                if (elementList.Count() > 0)
                {
                    var elements = (from i in elementList
                                    select new UIElementSummaryModel
                                    {
                                        ElementType = GetElementType(i),
                                        Label = alternateLabels.Where(e => e.UIElementID == i.UIElementID).Select(s => s.AlternateLabel).FirstOrDefault() ?? (i.Label == null ? "[Blank]" : i.Label),
                                        UIElementID = i.UIElementID,
                                        UIElementName = i.UIElementName,
                                        Parent = GetUIElementParentName(i.ParentUIElementID, elementList, alternateLabels),
                                        ParentUIElementId = i.ParentUIElementID,
                                        IsVisible = i.Visible,
                                        IsEnable = i.Enabled,
                                    }).ToList();
                    if (elements != null)
                    {
                        if (elements.Count > 0)
                        {
                            elements.ForEach(e => { e.Element = e.Label; e.Section = e.Parent; });
                        }
                        resultElements = elements.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return resultElements;
        }
        public string GetUIElementTypeByID(int uiElementId)
        {
            string type = string.Empty;
            try
            {
                var uielement = this._unitOfWork.RepositoryAsync<UIElement>().Get().Where(s => s.UIElementID == uiElementId).FirstOrDefault();
                type = GetElementType(uielement);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return type;
        }

        public UIElementRowModel GetUIElementByID(int uielementId)
        {
            UIElementRowModel uielement = null;
            try
            {
                uielement = (from e in _unitOfWork.RepositoryAsync<UIElement>().Get()
                             where e.UIElementID == uielementId
                             select new UIElementRowModel
                             {
                                 UIElementName = e.UIElementName,
                                 Label = e.Label,
                                 GeneratedName = e.GeneratedName
                             }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }

            return uielement;
        }
        private AdvancedConfiguration GetAdvanceConfiguration(UIElement element)
        {
            AdvancedConfiguration config = new AdvancedConfiguration();
            config.DisplayTopHeader = ((RepeaterUIElement)element).DisplayTopHeader;
            config.DisplayTitle = ((RepeaterUIElement)element).DisplayTitle;
            config.FrozenColCount = ((RepeaterUIElement)element).FrozenColCount;
            config.FrozenRowCount = ((RepeaterUIElement)element).FrozenRowCount;
            config.AllowPaging = ((RepeaterUIElement)element).AllowPaging;
            config.RowsPerPage = ((RepeaterUIElement)element).RowsPerPage;
            config.AllowExportToExcel = ((RepeaterUIElement)element).AllowExportToExcel;
            config.AllowExportToCSV = ((RepeaterUIElement)element).AllowExportToCSV;
            config.FilterMode = ((RepeaterUIElement)element).FilterMode;

            return config;
        }

        private string GetDefaultValue(UIElement element)
        {
            string defaultValue = string.Empty;
            if (element is TextBoxUIElement)
            {
                defaultValue = ((TextBoxUIElement)element).DefaultValue;
            }
            else if (element is DropDownUIElement)
            {
                defaultValue = ((DropDownUIElement)element).SelectedValue;
            }
            else if (element is CalendarUIElement)
            {
                defaultValue = Convert.ToString(((CalendarUIElement)element).DefaultDate);
            }

            return defaultValue;
        }

        private string GetLayoutTypeNameById(UIElement element, List<domain.entities.Models.LayoutType> layouts)
        {
            string layout = string.Empty;
            int layoutTypeId = 0;
            try
            {
                if (element is SectionUIElement)
                {
                    layoutTypeId = ((SectionUIElement)element).LayoutTypeID;
                }
                else if (element is RepeaterUIElement)
                {
                    layoutTypeId = ((RepeaterUIElement)element).LayoutTypeID;
                }
                layout = layouts.Where(s => s.LayoutTypeID == layoutTypeId).Select(s => s.DisplayText).FirstOrDefault();
            }
            catch (Exception)
            {
            }

            return layout;
        }

        private string GetValidatorFormat(UIElement element, List<Validator> validations, Dictionary<int, string> formats)
        {
            string format = "";
            try
            {
                var validator = element.Validators.Where(v => v.UIElementID == element.UIElementID).FirstOrDefault();
                if (validator != null && validator.IsLibraryRegex == true)
                {
                    format = formats[Convert.ToInt32(validator.LibraryRegexID)].TrimEnd();
                }
            }
            catch (Exception)
            {
            }
            return format;
        }

        private int? GetFormatLibraryID(UIElement element, List<Validator> validations, Dictionary<int, string> formats)
        {
            int? libraryId = null;
            try
            {
                var validator = element.Validators.Where(v => v.UIElementID == element.UIElementID).FirstOrDefault();
                if (validator != null && validator.IsLibraryRegex == true)
                {
                    libraryId = validator.LibraryRegexID;
                }
            }
            catch (Exception)
            {
            }
            return libraryId;
        }

        public ServiceResult UpdateCommentsForUIElement(int formDesignId, int formDesignVersionId, string comments, string userName, string extendedProperties)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                if (!string.IsNullOrEmpty(comments))
                {
                    var designVersion = this._unitOfWork.RepositoryAsync<FormDesignVersionExt>().Get().Where(v => v.FormDesignVersionID == formDesignVersionId).FirstOrDefault();
                    if (designVersion != null)
                    {
                        designVersion.Comments = comments;
                        designVersion.ExtendedColNames = extendedProperties;
                        this._unitOfWork.RepositoryAsync<FormDesignVersionExt>().Update(designVersion, true);

                    }
                    else
                    {
                        FormDesignVersionExt versionExt = new FormDesignVersionExt()
                        {
                            FormDesignVersionID = formDesignVersionId,
                            FormDesignID = formDesignId,
                            Comments = comments,
                            ExtendedColNames = extendedProperties,
                            AddedBy = userName,
                            AddedDate = DateTime.Now
                        };
                        this._unitOfWork.RepositoryAsync<FormDesignVersionExt>().Insert(versionExt);
                    }
                    this._unitOfWork.Save();
                    result.Result = ServiceResultStatus.Success;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                result.Result = ServiceResultStatus.Failure;
            }

            return result;
        }

        public ServiceResult UpdateRuleDescription(IEnumerable<RuleRowModel> rules)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                foreach (var objRule in rules)
                {
                    Rule rule = this._unitOfWork.RepositoryAsync<Rule>().Get().Where(v => v.RuleID == objRule.RuleId).FirstOrDefault();
                    rule.RuleDescription = rule.RuleDescription;
                    this._unitOfWork.RepositoryAsync<Rule>().Update(rule);
                }
                this._unitOfWork.Save();

                result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                result.Result = ServiceResultStatus.Failure;
            }
            return result;
        }

        public string GetCommentsForUIElement(int formDesignVersionId)
        {
            string comments = string.Empty;
            try
            {
                var designVersion = this._unitOfWork.RepositoryAsync<FormDesignVersionExt>().Get()
                                    .Where(v => v.FormDesignVersionID == formDesignVersionId)
                                    .Select(s => s.Comments)
                                    .FirstOrDefault();
                if (designVersion != null)
                {
                    comments = designVersion;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }

            return comments;
        }

        public ServiceResult UpdateFormDesignExcelConfiguration(int formDesignId, int formDesignVersionId, string configuration, string userName)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                if (!string.IsNullOrEmpty(configuration))
                {
                    var designVersion = this._unitOfWork.RepositoryAsync<FormDesignVersionExt>().Get().Where(v => v.FormDesignVersionID == formDesignVersionId).FirstOrDefault();
                    if (designVersion != null)
                    {
                        designVersion.ExcelConfiguration = configuration;
                        this._unitOfWork.RepositoryAsync<FormDesignVersionExt>().Update(designVersion, true);

                    }
                    else
                    {
                        FormDesignVersionExt versionExt = new FormDesignVersionExt()
                        {
                            FormDesignVersionID = formDesignVersionId,
                            FormDesignID = formDesignId,
                            ExcelConfiguration = configuration,
                            AddedBy = userName,
                            AddedDate = DateTime.Now
                        };
                        this._unitOfWork.RepositoryAsync<FormDesignVersionExt>().Insert(versionExt);
                    }
                    this._unitOfWork.Save();
                    result.Result = ServiceResultStatus.Success;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                result.Result = ServiceResultStatus.Failure;
            }

            return result;
        }

        public string GetFormDesignExcelConfiguration(int formDesignVersionId)
        {
            string comments = string.Empty;
            try
            {
                var designVersion = this._unitOfWork.RepositoryAsync<FormDesignVersionExt>().Get()
                                    .Where(v => v.FormDesignVersionID == formDesignVersionId)
                                    .Select(s => s.ExcelConfiguration)
                                    .FirstOrDefault();
                if (designVersion != null)
                {
                    comments = designVersion;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }

            return comments;
        }

        public IEnumerable<UIElementRowModel> GetUIElementListForFormDesignVersion(int tenantId, int formDesignVersionId)
        {
            IList<UIElementRowModel> uiElementRowModelList = null;
            try
            {
                string elementPath = "";
                //get all elements
                var elementList = this._unitOfWork.RepositoryAsync<UIElement>().GetUIElementListForFormDesignVersion(tenantId, formDesignVersionId).ToList();
                var alternateLabels = this._unitOfWork.RepositoryAsync<AlternateUIElementLabel>().Get().Where(s => s.FormDesignVersionID == formDesignVersionId).ToList();
                SqlParameter paramFrmDesignVersionID = new SqlParameter("@FormDesignVersionID", formDesignVersionId);
                List<FormDesignVersionUIElement> frmDesignVersionElementList = this._unitOfWork.Repository<FormDesignVersionUIElement>().ExecuteSql("exec [dbo].[uspGetFormDesignVersionUIElement] @FormDesignVersionID", paramFrmDesignVersionID).ToList();

                if (elementList.Count() > 0)
                {
                    uiElementRowModelList = (from i in elementList
                                             select new UIElementRowModel
                                             {
                                                 //AddedBy = i.AddedBy,
                                                 //AddedDate = i.AddedDate,
                                                 DataType = i.ApplicationDataType.ApplicationDataTypeName,
                                                 ElementType = GetElementType(i),
                                                 Label = alternateLabels.Where(e => e.UIElementID == i.UIElementID).Select(s => s.AlternateLabel).FirstOrDefault() ?? (i.Label == null ? "[Blank]" : i.Label),
                                                 //level = i.ParentUIElementID.HasValue ? GetRowLevel(i.ParentUIElementID, elementList, ref elementPath) : 0,
                                                 level = GetRowLevelPath(i.UIElementID, frmDesignVersionElementList, ref elementPath),
                                                 //UIElementPath = elementPath + i.GeneratedName,
                                                 UIElementPath = elementPath,
                                                 MaxLength = i is TextBoxUIElement ? ((TextBoxUIElement)i).MaxLength : 0,
                                                 Required = i.Validators.Count > 0 ? i.Validators.FirstOrDefault().IsRequired == true ? "Yes" : "No" : "No",
                                                 Sequence = i.Sequence,
                                                 UIElementID = i.UIElementID,
                                                 UIElementName = i.UIElementName,
                                                 parent = i.ParentUIElementID.HasValue ? i.ParentUIElementID.Value.ToString() : "0",
                                                 ParentElementID = i.ParentUIElementID,
                                                 //isLeaf = i.ParentUIElementID.HasValue ? IsLeafRow(i.UIElementID, elementList) : false,
                                                 isLeaf = !i.IsContainer,
                                                 IsMappedUIElement = i.DataSourceMappings1.Any(ui => ui.MappedUIElementID == i.UIElementID
                                                                                                     && ui.FormDesignVersionID == formDesignVersionId),
                                                 hasLoadFromServer = i is RepeaterUIElement ? ((RepeaterUIElement)i).LoadFromServer : false,
                                                 IsVisible = i.Visible,
                                                 IsRepeaterKey = IsRepeaterKeyElement(i.UIElementID),
                                                 //UpdatedBy = i.UpdatedBy,
                                                 //UpdatedDate = i.UpdatedDate,
                                                 isExt = true,
                                                 loaded = true,
                                                 GeneratedName = i.GeneratedName,
                                                 IsStandard = i.IsStandard,
                                                 Enabled = i.Enabled

                                             }).ToList();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return uiElementRowModelList;
        }

        public List<UIElementRowModel> GetUIElementByNames(int formDesignVersionId, List<string> elementNames)
        {
            List<UIElementRowModel> uielements = null;
            try
            {
                uielements = (from e in _unitOfWork.RepositoryAsync<UIElement>().Get()
                              join map in _unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Get()
                              on e.UIElementID equals map.UIElementID
                              join nm in elementNames on e.UIElementName equals nm
                              where map.FormDesignVersionID == formDesignVersionId
                              select new UIElementRowModel
                              {
                                  UIElementName = e.UIElementName,
                                  Label = e.Label
                              }).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }

            return uielements;
        }

        public List<RepeaterKeyFilterModel> GetTargetKeyFilter(int ruleId)
        {
            List<RepeaterKeyFilterModel> keyFilterModel = null;
            try
            {
                keyFilterModel = (from keyfilter in _unitOfWork.RepositoryAsync<TargetRepeaterKeyFilter>().Get()
                                  join prm in _unitOfWork.RepositoryAsync<PropertyRuleMap>().Get()
                                  on keyfilter.RuleID equals prm.RuleID
                                  join ui in _unitOfWork.RepositoryAsync<UIElement>().Get()
                                  on prm.UIElementID equals ui.UIElementID
                                  where keyfilter.RuleID == ruleId
                                  select new RepeaterKeyFilterModel
                                  {
                                      UIElementID = ui.UIElementID,
                                      Label = ui.Label,
                                      UIElementPath = keyfilter.RepeaterKey,
                                      FilterValue = keyfilter.RepeaterKeyValue,
                                      isChecked = false
                                  }).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return keyFilterModel;
        }

        public List<RepeaterKeyFilterModel> GetKeyFilter(int ruleId, bool isRightOperand)
        {
            List<RepeaterKeyFilterModel> keyFilterModel = null;
            try
            {
                keyFilterModel = (from keyfilter in _unitOfWork.RepositoryAsync<RepeaterKeyFilter>().Get()
                                  join e in _unitOfWork.RepositoryAsync<Expression>().Get()
                                    on keyfilter.ExpressionID equals e.ExpressionID
                                  join prm in _unitOfWork.RepositoryAsync<PropertyRuleMap>().Get()
                                  on e.RuleID equals prm.RuleID
                                  join ui in _unitOfWork.RepositoryAsync<UIElement>().Get()
                                  on prm.UIElementID equals ui.UIElementID
                                  where e.RuleID == ruleId && keyfilter.IsRightOperand == isRightOperand
                                  select new RepeaterKeyFilterModel
                                  {
                                      UIElementID = ui.UIElementID,
                                      Label = ui.Label,
                                      UIElementPath = keyfilter.RepeaterKey,
                                      FilterValue = keyfilter.RepeaterKeyValue,
                                      isChecked = false
                                  }).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return keyFilterModel;
        }

        public int GetUIElementIDByNames(int formDesignVersionId, string elementNames)
        {
            List<UIElementRowModel> uielements = null;
            try
            {
                uielements = (from e in _unitOfWork.RepositoryAsync<UIElement>().Get()
                              join map in _unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Get()
                              on e.UIElementID equals map.UIElementID
                              where map.FormDesignVersionID == formDesignVersionId
                              && e.UIElementName == elementNames
                              select new UIElementRowModel
                              {
                                  UIElementName = e.UIElementName,
                                  Label = e.Label,
                                  UIElementID = e.UIElementID
                              }).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }

            return uielements.FirstOrDefault().UIElementID;
        }

        //Methods added for Document Compare/Sync functionality
        public IEnumerable<DocumentElementViewModel> GetRepeaterUIElement(int tenantId, int formDesignVersionId)
        {
            IList<DocumentElementViewModel> elements = null;
            try
            {
                elements = (from e in this.GetUIElementListForFormDesignVersion(tenantId, formDesignVersionId)
                            where e.ElementType == "Repeater"
                            select new DocumentElementViewModel
                            {
                                ElementType = e.ElementType,
                                isChecked = false,
                                isLeaf = e.isLeaf,
                                Label = e.Label,
                                level = e.level,
                                parent = e.parent,
                                ParentElementID = e.ParentElementID,
                                UIElementID = e.UIElementID,
                                UIElementPath = e.UIElementPath
                            }).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return elements;
        }

        public IEnumerable<DocumentElementViewModel> GetDocumentElementList(int tenantId, int formDesignVersionId)
        {
            IList<DocumentElementViewModel> elements = null;
            try
            {
                elements = (from e in this.GetUIElementListForFormDesignVersion(tenantId, formDesignVersionId)
                            where e.isLeaf == false
                            select new DocumentElementViewModel
                            {
                                ElementType = e.ElementType,
                                isChecked = false,
                                isLeaf = e.isLeaf,
                                Label = e.Label,
                                level = e.level,
                                expanded = true,
                                parent = e.parent,
                                ParentElementID = e.ParentElementID,
                                UIElementID = e.UIElementID,
                                UIElementPath = e.UIElementPath
                            }).ToList();


                foreach (var item in elements)
                {
                    int count = elements.Where(x => x.ParentElementID == item.UIElementID).Count();
                    if (count == 0) { item.isLeaf = true; }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return elements;
        }

        public IEnumerable<DocumentElementBaseModel> GetExpressionRepeaterKeyList(int tenantId, int parentUIElementId, int expressionId, bool rightOnly)
        {
            List<DocumentElementBaseModel> repeaterKeys = null;
            try
            {
                if (expressionId > 0)
                {
                    var filters = (from r in _unitOfWork.RepositoryAsync<RepeaterKeyFilter>().Get()
                                   where r.ExpressionID == expressionId && r.IsRightOperand == rightOnly
                                   select r);

                    repeaterKeys = (from e in _unitOfWork.RepositoryAsync<UIElement>().Get()
                                    join r in filters on e.UIElementName equals r.RepeaterKey into joined
                                    from j in joined.DefaultIfEmpty()
                                    join key in _unitOfWork.RepositoryAsync<RepeaterKeyUIElement>().Get() on e.UIElementID equals key.UIElementID
                                    where key.RepeaterUIElementID == parentUIElementId
                                    select new DocumentElementBaseModel
                                    {
                                        UIElementID = e.UIElementID,
                                        Label = e.Label,
                                        isChecked = true,
                                        UIElementPath = e.UIElementName,
                                        FilterValue = j == null ? string.Empty : j.RepeaterKeyValue
                                    }).ToList();
                }
                else
                {
                    repeaterKeys = this.GetRepeaterKeyList(tenantId, parentUIElementId).ToList();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return repeaterKeys;
        }

        public IEnumerable<DocumentElementBaseModel> GetTargetRepeaterKeyList(int tenantId, int parentUIElementId, int ruleId)
        {
            List<DocumentElementBaseModel> repeaterKeys = null;
            try
            {
                if (ruleId > 0)
                {
                    var filters = (from r in _unitOfWork.RepositoryAsync<TargetRepeaterKeyFilter>().Get()
                                   where r.RuleID == ruleId
                                   select r);

                    repeaterKeys = (from e in _unitOfWork.RepositoryAsync<UIElement>().Get()
                                    join r in filters on e.UIElementName equals r.RepeaterKey into joined
                                    from j in joined.DefaultIfEmpty()
                                    join key in _unitOfWork.RepositoryAsync<RepeaterKeyUIElement>().Get() on e.UIElementID equals key.UIElementID
                                    where key.RepeaterUIElementID == parentUIElementId
                                    select new DocumentElementBaseModel
                                    {
                                        UIElementID = e.UIElementID,
                                        Label = e.Label,
                                        isChecked = true,
                                        UIElementPath = e.UIElementName,
                                        FilterValue = j == null ? string.Empty : j.RepeaterKeyValue
                                    }).ToList();
                }
                else
                {
                    repeaterKeys = this.GetRepeaterKeyList(tenantId, parentUIElementId).ToList();
                }

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return repeaterKeys;
        }

        private IEnumerable<DocumentElementBaseModel> GetRepeaterKeyList(int tenantId, int parentUIElementId)
        {
            List<DocumentElementBaseModel> repeaterKeys = null;
            try
            {
                repeaterKeys = (from e in _unitOfWork.RepositoryAsync<UIElement>().Get()
                                join key in _unitOfWork.RepositoryAsync<RepeaterKeyUIElement>().Get()
                                on e.UIElementID equals key.UIElementID
                                where key.RepeaterUIElementID == parentUIElementId
                                select new DocumentElementBaseModel
                                {
                                    UIElementID = e.UIElementID,
                                    Label = e.Label,
                                    isChecked = false,
                                    UIElementPath = e.UIElementName
                                }).ToList();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return repeaterKeys;
        }

        public IEnumerable<DocumentElementBaseModel> GetRepeaterKeyList(int tenantId, int formDesignVersionId, int parentUIElementId)
        {
            List<DocumentElementBaseModel> repeaterKeys = null;
            try
            {
                var elementList = this._unitOfWork.RepositoryAsync<UIElement>().GetUIElementListForFormDesignVersion(tenantId, formDesignVersionId).ToList();

                repeaterKeys = (from r in elementList
                                where r.ParentUIElementID == parentUIElementId
                                select new DocumentElementBaseModel
                                {
                                    UIElementID = r.UIElementID,
                                    Label = r.Label,
                                    isChecked = false,
                                    UIElementPath = (GetElementFullPath(parentUIElementId, elementList) + r.GeneratedName),
                                    ElementType = GetElementType(r)
                                }).ToList();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            //return repeaterKeys.Where(s => s.ElementType == "Label");
            return repeaterKeys;
        }

        public IEnumerable<DocumentElementBaseModel> GetElementChildrenList(int tenantId, int formDesignVersionId, int parentUIElementId)
        {
            List<DocumentElementBaseModel> elementChildren = null;
            try
            {
                var elementList = this._unitOfWork.RepositoryAsync<UIElement>().GetUIElementListForFormDesignVersion(tenantId, formDesignVersionId).ToList();

                elementChildren = (from e in elementList
                                   where e.ParentUIElementID == parentUIElementId
                                   select new DocumentElementBaseModel
                                   {
                                       UIElementID = e.UIElementID,
                                       Label = e.Label,
                                       GeneratedName = e.GeneratedName,
                                       isChecked = false,
                                       UIElementPath = (GetElementFullPath(parentUIElementId, elementList) + e.GeneratedName),
                                       ElementType = GetElementType(e)
                                   }).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            if (elementChildren != null && elementChildren.Count > 0)
            {
                string fullName = elementChildren[0].UIElementPath;
                fullName = fullName.Substring(0, fullName.LastIndexOf('.'));

                //List<string> keys = DocumentMacroSyncConfig.GetConfiguredRepeaterKeys(fullName); 
                List<int> key = new List<int>();
                key = (from r in this._unitOfWork.RepositoryAsync<RepeaterKeyUIElement>().Get()
                       where r.RepeaterUIElementID == parentUIElementId
                       select
                        r.UIElementID
                       ).ToList<int>();

                elementChildren = elementChildren.Where(t => !key.Contains(t.UIElementID)).ToList();
            }

            return elementChildren.Where(e => e.ElementType != "Label" && e.ElementType != "Repeater" && e.ElementType != "Section" && e.ElementType != "[Blank]");
        }
        //End

        public IEnumerable<UIElementSeqModel> GetChildUIElements(int tenantId, int formDesignVersionId, int parentUIElementId)
        {
            List<UIElementSeqModel> uiElementRowModelList = null;
            List<RepeaterKeyUIElement> keyElements = null;
            try
            {
                UIElement parentElement = this._unitOfWork.RepositoryAsync<UIElement>()
                                            .Query().Filter(c => c.UIElementID == parentUIElementId)
                                            .Get().FirstOrDefault();


                List<UIElement> elements = this._unitOfWork.RepositoryAsync<UIElement>()
                                                            .Query()
                                                            .Include(c => c.FormDesignVersionUIElementMaps)
                                                            .Include(c => c.DataSourceMappings1)
                                                            .Filter(c => (c.FormDesignVersionUIElementMaps.Any(f => f.FormDesignVersionID == formDesignVersionId) && c.ParentUIElementID == parentUIElementId))
                                                            .Get().ToList();

                //If parent element is a repeater then get the key elements
                if (parentElement is RepeaterUIElement)
                {
                    keyElements = this._unitOfWork.RepositoryAsync<RepeaterKeyUIElement>()
                                        .Query().Filter(c => c.RepeaterUIElementID == parentUIElementId)
                                        .Get().ToList();
                }
                //End

                var elementModels = from el in elements
                                    select new UIElementSeqModel
                                    {
                                        Label = el.Label,
                                        Sequence = el.Sequence,
                                        UIElementID = el.UIElementID,
                                        ElementType = GetElementType(el),
                                        IsKey = keyElements == null ? false : keyElements.Any(c => c.UIElementID == el.UIElementID),
                                        CheckDuplicate = Convert.ToBoolean(el.CheckDuplicate),
                                        IsRepeaterKeyMapped = IsRepeaterKeyElement(el.UIElementID),
                                        IsElementMapped = el.DataSourceMappings1.Any(ui => ui.MappedUIElementID == el.UIElementID
                                                                                                     && ui.FormDesignVersionID == formDesignVersionId),
                                        IsStandard = el.IsStandard
                                    };

                uiElementRowModelList = elementModels.ToList();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return uiElementRowModelList;
        }
        public IEnumerable<UIElementSeqModel> GetApplicableRepeaterChildUIElementsForDuplicationCheck(int tenantId, int formDesignVersionId, int parentUIElementId)
        {
            IList<UIElementSeqModel> availableUIElementList = null;
            try
            {
                IList<UIElementSeqModel> uiElementRowModelList = (from el in this._unitOfWork.RepositoryAsync<UIElement>()
                                                           .Query()
                                                           .Include(c => c.FormDesignVersionUIElementMaps)
                                                           .Filter(c => (c.FormDesignVersionUIElementMaps.Any(f => f.FormDesignVersionID == formDesignVersionId)
                                                                           && c.ParentUIElementID == parentUIElementId)
                                                                           && (((c is TextBoxUIElement) ? (c as TextBoxUIElement).IsLabel == false ? true : false : false)
                                                                           || (c is DropDownUIElement)
                                                                           || (c is CalendarUIElement))
                                                                           && c.Visible == true && c.Enabled == true)
                                                           .Get().ToList()
                                                                  select new UIElementSeqModel
                                                                  {
                                                                      Label = el.Label,
                                                                      Sequence = el.Sequence,
                                                                      UIElementID = el.UIElementID,
                                                                      ElementType = GetElementType(el),
                                                                      CheckDuplicate = el.CheckDuplicate
                                                                  }).ToList();

                availableUIElementList = new List<UIElementSeqModel>();

                foreach (var item in uiElementRowModelList)
                {
                    if (!_unitOfWork.Repository<DataSourceMapping>()
                                        .Query()
                                        .Filter(c => c.UIElementID == item.UIElementID && c.FormDesignVersionID == formDesignVersionId)
                                        .Get()
                                        .Any())
                    {
                        availableUIElementList.Add(item);
                    }
                    else if (!_unitOfWork.Repository<DataSourceMapping>()
                                            .Query()
                                            .Filter(c => c.UIElementID == item.UIElementID
                                                    && (c.DataSourceElementDisplayModeID == (int)(DisplayMode.INLINE) || c.DataSourceElementDisplayModeID == (int)(DisplayMode.CHILD))
                                                    && c.FormDesignVersionID == formDesignVersionId)
                                            .Get()
                                            .Any())
                    {
                        availableUIElementList.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return availableUIElementList;
        }
        public int GetID(int uiElementID, string idType)
        {
            return this._unitOfWork.RepositoryAsync<UIElement>().GetElementID(uiElementID, idType);
        }

        public ServiceResult UpdateAlternateLabel(int tenantId, int formDesignId, int formDesignVersionId, int uiElementId, string alternateLabel)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                var data = this._unitOfWork.RepositoryAsync<AlternateUIElementLabel>()
                    .Query()
                    .Filter(d => d.FormDesignVersionID == formDesignVersionId && d.UIElementID == uiElementId)
                    .Get().FirstOrDefault();

                if (data != null)
                {
                    data.AlternateLabel = alternateLabel;

                    this._unitOfWork.RepositoryAsync<AlternateUIElementLabel>().Update(data);
                    this._unitOfWork.Save();

                    result.Result = ServiceResultStatus.Success;
                }
                else
                {
                    AlternateUIElementLabel altLabel = new AlternateUIElementLabel();
                    altLabel.FormDesignID = formDesignId;
                    altLabel.FormDesignVersionID = formDesignVersionId;
                    altLabel.UIElementID = uiElementId;
                    altLabel.AlternateLabel = alternateLabel;

                    this._unitOfWork.RepositoryAsync<AlternateUIElementLabel>().Insert(altLabel);
                    this._unitOfWork.Save();

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
        /// <summary>
        /// method to get the list of all the section Ids by formDesignVersionId
        /// </summary>
        /// <param name="formDesignVersionId"></param>
        /// <returns></returns>
        public List<int> GetSectionListByFormDesignVersionId(int formDesignVersionId)
        {
            var uiElementList = this._unitOfWork.RepositoryAsync<SectionUIElement>()
                                                            .Query()
                                                            .Include(c => c.FormDesignVersionUIElementMaps)
                                                            .Filter(p => p.IsActive == true &&
                                                                p.FormDesignVersionUIElementMaps.Any(c => c.FormDesignVersionID == formDesignVersionId))
                                                            .Get()
                                                            .Select(p => p.UIElementID)
                                                            .ToList();
            return uiElementList;

        }

        public ServiceResult UpdateDocumentRules(int oldUiElementId, int newUiElementId, int formDesignVersionID)
        {
            ServiceResult Result = new ServiceResult();
            try
            {
                DocumentRule documentRule = this._unitOfWork.RepositoryAsync<DocumentRule>().Get()
                                   .Where(x => x.TargetUIElementID == oldUiElementId
                                   && x.FormDesignVersionID == formDesignVersionID)
                                   .FirstOrDefault();
                if (documentRule != null)
                {
                    documentRule.TargetUIElementID = newUiElementId;
                    this._unitOfWork.RepositoryAsync<DocumentRule>().Update(documentRule);
                    this._unitOfWork.Save();
                    Result.Result = ServiceResultStatus.Success;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Result;
        }

        public bool IsNewUIElementCreationRequired(int uielementId, int formDesignVersionID)
        {
            bool createNew = false;
            try
            {
                List<FormDesignVersionUIElementMap> list = this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>()
                                                            .Query()
                                                            .Include(c => c.FormDesignVersion)
                                                            .Filter(c => c.UIElementID == uielementId && c.FormDesignVersionID != formDesignVersionID)
                                                            .Get()
                                                            .ToList();
                int finalizedStatusID = Convert.ToInt32(tmg.equinox.domain.entities.Status.Finalized);
                if (list.Any(c => c.FormDesignVersion.StatusID == finalizedStatusID))
                {
                    createNew = true;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return createNew;
        }

        #endregion Public Methods

        #region Private Methods
        private string GetUniqueName(int formID, string elementType, int elementId, int parentID)
        {
            string elementName = string.Empty;
            try
            {
                FormDesign formDesign = this._unitOfWork.RepositoryAsync<FormDesign>().FindById(formID);
                if (formDesign != null)
                {
                    elementName = formDesign.Abbreviation + elementType + elementId.ToString();

                    if (parentID != 0)
                    {
                        string parentName = this._unitOfWork.RepositoryAsync<UIElement>().FindById(parentID).UIElementName;

                        if (!string.IsNullOrEmpty(parentName))
                        {
                            if (parentName.Contains("Repeater"))
                            {
                                elementName = elementName.Insert(0, parentName + "_1_");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return elementName;
        }

        private string GetGeneratedName(string label)
        {
            string generatedName = "";
            if (!String.IsNullOrEmpty(label))
            {
                Regex regex = new Regex("[^a-zA-Z0-9]");
                generatedName = regex.Replace(label, String.Empty);
                if (generatedName.Length > 70)
                {
                    generatedName = generatedName.Substring(0, 70);
                }

                Regex checkDigits = new Regex("^[0-9]");

                //if Label starts with numeric characters, this will append a character at the beginning.
                if (checkDigits.IsMatch(label, 0))
                {
                    generatedName = "a" + generatedName;
                }
            }
            return generatedName;
        }

        private bool IsLeafRow(int? elementID, List<UIElement> models)
        {
            try
            {
                foreach (UIElement element in models)
                {
                    if (element.ParentUIElementID == elementID)
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return true;
        }

        private string GetElementType(UIElement uielement)
        {
            string uIElementType = string.Empty;
            try
            {
                if (uielement is RadioButtonUIElement)
                {
                    uIElementType = ElementTypes.list[0];
                }
                else if (uielement is CheckBoxUIElement)
                {
                    uIElementType = ElementTypes.list[1];
                }
                else if (uielement is TextBoxUIElement)
                {
                    switch (((TextBoxUIElement)uielement).UIElementTypeID)
                    {
                        case 3:
                            uIElementType = ElementTypes.list[2];
                            break;
                        case 4:
                            uIElementType = ElementTypes.list[3];
                            break;
                        case 10:
                            uIElementType = ElementTypes.list[9];
                            break;
                        case 11:
                            uIElementType = ElementTypes.list[10];
                            break;
                        case 13:
                            uIElementType = ElementTypes.list[12];
                            break;
                    }
                }
                else if (uielement is DropDownUIElement)
                {
                    switch (((DropDownUIElement)uielement).UIElementTypeID)
                    {
                        case 5:
                            uIElementType = ElementTypes.list[4];
                            break;
                        case 12:
                            uIElementType = ElementTypes.list[11];
                            break;
                    }
                }
                else if (uielement is CalendarUIElement)
                {
                    uIElementType = ElementTypes.list[5];
                }
                else if (uielement is SectionUIElement)
                {
                    uIElementType = ElementTypes.list[8];
                }
                else if (uielement is RepeaterUIElement)
                {
                    uIElementType = ElementTypes.list[6];
                }
                else if (uielement is TabUIElement)
                {
                    uIElementType = ElementTypes.list[7];
                }
                else
                {
                    uIElementType = "-";
                }

            }
            catch (Exception ex)
            {
                throw;
            }
            return uIElementType;
        }

        private int GetRowLevel(int? parentID, List<UIElement> elementList, ref string elementPath)
        {
            int level = 0;
            string path = "";
            try
            {
                while (parentID != null)
                {
                    level++;
                    var result = from element in elementList
                                 where element.UIElementID == parentID
                                 select element;

                    var objUIElement = result.Single();
                    parentID = objUIElement.ParentUIElementID;
                    if (parentID != null)
                        path = (objUIElement.GeneratedName + "." + path);
                }
                elementPath = path;
            }
            catch (Exception ex)
            {
                throw;
            }
            return level;
        }
        private int GetRowLevelPath(int UIElementID, List<FormDesignVersionUIElement> frmDesignVersionElementList, ref string elementPath)
        {
            int level = 0;
            string path = "";
            try
            {
                if (frmDesignVersionElementList != null)
                {
                    var element = frmDesignVersionElementList.Where(x => x.UIElementID == UIElementID).FirstOrDefault();
                    if (element != null)
                    {
                        path = element.UIElementFullName;
                        if (element.UIElementTypeID != 8)
                        {
                            level = path.Split('.').Count();
                        }
                    }
                }
                elementPath = path;
            }
            catch (Exception ex)
            {
                throw;
            }
            return level;
        }

        private string GetElementFullPath(int? parentID, List<UIElement> elementList)
        {
            string path = "";
            try
            {
                while (parentID != null)
                {
                    var result = from element in elementList
                                 where element.UIElementID == parentID
                                 select element;

                    var objUIElement = result.Single();
                    parentID = objUIElement.ParentUIElementID;
                    if (parentID != null)
                        path = (objUIElement.GeneratedName + "." + path);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return path;
        }

        private string GetAlternateLabel(int formDesignVersionId, int elementId)
        {
            string altLabel = null;

            var result = this._unitOfWork.RepositoryAsync<AlternateUIElementLabel>()
                .Query()
                .Filter(a => a.UIElementID == elementId && a.FormDesignVersionID == formDesignVersionId)
                .Get().FirstOrDefault();
            if (result != null)
            {
                altLabel = result.AlternateLabel;
            }

            return altLabel;
        }

        private bool IsRepeaterKeyElement(int uiElementID)
        {

            var isKeyElement = (from repeaterKeyUIElement in this._unitOfWork.RepositoryAsync<RepeaterKeyUIElement>().Query().Filter(x => x.UIElementID == uiElementID).Get()
                                join dataSourceMapping in this._unitOfWork.RepositoryAsync<DataSourceMapping>().Get()
                                                       on repeaterKeyUIElement.UIElementID equals dataSourceMapping.MappedUIElementID
                                join repeaterUIElement in this._unitOfWork.RepositoryAsync<RepeaterUIElement>().Get()
                                                       on dataSourceMapping.DataSourceID equals repeaterUIElement.DataSourceID
                                where repeaterKeyUIElement.RepeaterUIElementID == repeaterUIElement.UIElementID
                                select repeaterKeyUIElement).FirstOrDefault();

            return isKeyElement != null;
        }

        private bool isExistsInAlternateLabel(int uiElementId)
        {

            AlternateUIElementLabel alternateUIElement = this._unitOfWork.RepositoryAsync<AlternateUIElementLabel>().Get().Where(x => x.UIElementID == uiElementId).FirstOrDefault();
            if (alternateUIElement != null)
                return true;
            return false;
        }
        #endregion Private Methods

        public List<ruleinterpreter.model.CompiledDocumentRule> GetAllDocumentRuleBySection(string sectionName, int formDesignVersionId, string sourcePath)
        {
            List<int> targetUiElementList = this._unitOfWork.RepositoryAsync<DocumentRule>().Get()
                .Where(s => s.TargetElementPath.StartsWith(sectionName)
                && s.FormDesignVersionID.Equals(formDesignVersionId)
                )
                .Select(s => s.TargetUIElementID).Distinct().ToList();
            List<ruleinterpreter.model.CompiledDocumentRule> compiledDocumentRuleList = new List<ruleinterpreter.model.CompiledDocumentRule>();

            foreach (var item in targetUiElementList)
            {
                compiledDocumentRuleList.Add(GetCompiledRuleJSON(item, formDesignVersionId, true));
            }

            List<ruleinterpreter.model.CompiledDocumentRule> targetCompiledDocumentRuleList = new List<ruleinterpreter.model.CompiledDocumentRule>();

            foreach (var item in compiledDocumentRuleList)
            {
                if (item.SourceContainer.RuleSources.Where(s => s.SourcePath.Contains(sourcePath)).Count() > 0)
                {
                    targetCompiledDocumentRuleList.Add(item);
                }
            }
            return targetCompiledDocumentRuleList;
        }

    }
}
