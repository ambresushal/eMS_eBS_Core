using System;
using System.Collections.Generic;
using System.Linq;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.UIElement;
using tmg.equinox.applicationservices.viewmodels.FormDesign;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;
using System.Transactions;
using tmg.equinox.infrastructure.exceptionhandling;
using System.Diagnostics.Contracts;
using tmg.equinox.applicationservices.viewmodels.MasterList;
using tmg.equinox.domain.entities;
using tmg.equinox.domain.entities.Utility;

namespace tmg.equinox.applicationservices
{
    public partial class DataSourceService : IDataSourceService
    {
        #region Private Memebers
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        private IUIElementService _uiElementService { get; set; }
        private ILoggingService _loggingService { get; set; }
        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor
        public DataSourceService(IUnitOfWorkAsync unitOfWork, IUIElementService elementService, ILoggingService loggingService)
        {
            this._unitOfWork = unitOfWork;
            this._uiElementService = elementService;
            this._loggingService = loggingService;
        }
        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// Get DataSources
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formDesignVersionId"></param>
        /// <param name="uiElementId"></param>
        /// <returns></returns>
        public IEnumerable<DataSourceRowModel> GeDataSourcesForUIElementType(int tenantId, int uiElementId, string uiElementType, int formDesignId, int formDesignVersionId)
        {
            Contract.Requires(tenantId > 0, "Invalid tenantId");
            Contract.Requires(uiElementId > 0, "Invalid uiElementId");
            Contract.Requires(uiElementType != null, "Invalid uiElementType");
            ServiceResult result = new ServiceResult();
            IList<DataSourceRowModel> rowModelList = null;
            try
            {
                if (uiElementType.ToLower() == domain.entities.UIElementType.SECTION.ToString().ToLower())
                {
                    rowModelList = GetSectionsData(uiElementId, rowModelList);
                }

                else if (uiElementType == "Repeater")
                {

                    rowModelList = GetRepeaterData(uiElementId, rowModelList, formDesignVersionId);

                }
                else
                {
                    rowModelList = GetDropdownData(uiElementId, rowModelList);
                }


                if (rowModelList.Count() == 0)
                    rowModelList = null;
                else
                    SetCurrentDs(rowModelList, uiElementId, uiElementType, formDesignId, formDesignVersionId);
                
                // Get only last DataSource
                if (rowModelList != null)
                {
                    List<int> idList= rowModelList.GroupBy(x => x.DataSourceId).Select(x => x.Max(a => a.UIElementID)).ToList();
                    if (idList != null)
                    {
                        rowModelList = (from grp in rowModelList
                                        join ids in idList
                                        on grp.UIElementID equals ids
                                        select grp).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return rowModelList;
        }

        /// <summary>
        /// Get DataSource UIEklements
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formDesignVersionId"></param>
        /// <param name="uiElementId"></param>
        /// <returns></returns>
        public IEnumerable<UIElementRowModel> GetDataSourceUIElements(int tenantId, int uiElementId, string uiElementType, int dataSourceId, int formDesignId, int formDesignVersionId)
        {
            Contract.Requires(tenantId > 0, "Invalid tenantId");
            Contract.Requires(uiElementId > 0, "Invalid uiElementId");
            Contract.Requires(uiElementType != null, "Invalid uiElementType");
            Contract.Requires(dataSourceId > 0, "Invalid dataSourceId");
            ServiceResult result = new ServiceResult();
            List<UIElementRowModel> rowModelList = null;
            try
            {
                var defaultElementType = ResolveUIElementType(uiElementType);

                //Get the  UIElement used as current datasource
                var parentUIElementId = 0;

                parentUIElementId = GetUIelementIdOfContainer(dataSourceId, defaultElementType, parentUIElementId);


                if (parentUIElementId != 0)
                {
                    var targetVersionEffectiveDate = this._unitOfWork.RepositoryAsync<FormDesignVersion>()
                                                                .Query()
                                                                .Filter(f => f.FormDesignVersionID == formDesignVersionId)
                                                                .Get()
                                                                .Select(f => f.EffectiveDate)
                                                                .FirstOrDefault();


                    var sourceformDesignId = this._unitOfWork.RepositoryAsync<DataSource>()
                                                    .Query()
                                                    .Filter(fil => fil.DataSourceID == dataSourceId)
                                                    .Get()
                                                    .Select(sel => sel.FormDesignID)
                                                    .FirstOrDefault();

                    var isFinalizedVersionExists = IsFinalizedFormDesignVersionExists(sourceformDesignId);

                    var sourceFormDesignVersionId = 0;

                    if (isFinalizedVersionExists)
                    {
                        var sourceFormdesignVersion = this._unitOfWork.RepositoryAsync<FormDesignVersion>()
                                                                   .Query()
                                                                   .Filter(fil => fil.FormDesignID == sourceformDesignId &&
                                                                       fil.StatusID == (int)domain.entities.Status.Finalized)
                                                                   .Get()
                                                                   .OrderByDescending(ord => ord.EffectiveDate)
                                                                   .ThenByDescending(ord => ord.FormDesignVersionID)
                                                                   .FirstOrDefault(e => e.EffectiveDate <= targetVersionEffectiveDate);

                        if (sourceFormdesignVersion != null)
                        {
                            sourceFormDesignVersionId = sourceFormdesignVersion.FormDesignVersionID;
                        }

                    }
                    else
                    {
                        sourceFormDesignVersionId = this._unitOfWork.RepositoryAsync<FormDesignVersion>()
                                                                       .Query()
                                                                       .Filter(fil => fil.FormDesignID == sourceformDesignId)
                                                                       .Get()
                                                                       .Select(f => f.FormDesignVersionID)
                                                                       .FirstOrDefault();
                    }

                    //Get all the datasources of a given parent uielement which is the datasource
                    var uiElementslList = (from ui in this._unitOfWork.RepositoryAsync<UIElement>().Get()
                                           join uimap in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Get()
                                           on ui.UIElementID equals uimap.UIElementID
                                           where ui.ParentUIElementID == parentUIElementId
                                           && ((formDesignId == sourceformDesignId) ?
                                           uimap.FormDesignVersionID == formDesignVersionId :
                                           uimap.FormDesignVersionID == sourceFormDesignVersionId)
                                           select ui).ToList();


                    //Get mappings for the uielement 
                    List<DataSourceMapping> filter = GetMappingsForUielement(uiElementId, uiElementType,
                                                     dataSourceId, formDesignId, formDesignVersionId);


                    List<UIElement> mappedUIElement = new List<UIElement>();
                    foreach (var mapping in filter)
                    {
                        UIElement element = (from c in this._unitOfWork.RepositoryAsync<UIElement>()
                                                .Query().
                                                 Filter(c => c.UIElementID == mapping.UIElementID).
                                                 Get()
                                             select c).FirstOrDefault();
                        mappedUIElement.Add(element);
                    }
                    rowModelList = GetDataSourceMappingForContainer(uiElementType, formDesignId, formDesignVersionId,
                                  rowModelList, defaultElementType, uiElementslList, filter, mappedUIElement);

                }
                if (rowModelList.Count() == 0)
                    rowModelList = null;
            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return rowModelList;
        }

        /// <summary>
        /// Method to check if the Data Source Name is unique.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formDesignVersionId"></param>
        /// <param name="dataSourceName"></param>
        /// <returns></returns>
        public bool IsDataSourceNameUnique(int tenantId, int formDesignVersionId, string dataSourceName, int uiElementId, string uiElementType)
        {
            Contract.Requires(tenantId > 0, "Invalid tenantId");
            Contract.Requires(uiElementId > 0, "Invalid uiElementId");
            Contract.Requires(formDesignVersionId > 0, "Invalid formDesignVersionId");
            Contract.Requires(uiElementType != null, "Invalid uiElementType");
            Contract.Requires(dataSourceName != null, "Invalid dataSourceName");
            ServiceResult result = new ServiceResult();
            bool isUnique = false;
            int? dataSourceId = null;
            try
            {
                switch (uiElementType)
                {
                    case "Dropdown List": break;
                    case "Dropdown TextBox": break;
                    case "Section":
                        dataSourceId = this._unitOfWork.RepositoryAsync<SectionUIElement>().Query().Filter(s => s.UIElementID == uiElementId).Get().FirstOrDefault().DataSourceID;
                        break;
                    case "Repeater":
                        dataSourceId = this._unitOfWork.RepositoryAsync<RepeaterUIElement>().Query().Filter(r => r.UIElementID == uiElementId).Get().FirstOrDefault().DataSourceID;
                        break;
                }
                if (dataSourceId != null && dataSourceId.HasValue)
                {
                    isUnique = !this._unitOfWork.RepositoryAsync<DataSource>()
                                                    .Query()
                                                    .Filter(c => c.DataSourceName.ToLower() == dataSourceName.ToLower() && c.DataSourceID != dataSourceId.Value)
                                                    .Get()
                                                    .Any();
                }
                else
                {
                    isUnique = !this._unitOfWork.RepositoryAsync<DataSource>()
                                                    .Query()
                                                    .Filter(c => c.DataSourceName.ToLower() == dataSourceName.ToLower())
                                                    .Get()
                                                    .Any();
                }
            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return isUnique;
        }

        /// <summary>
        /// Update DataSource UIElements
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formDesignVersionId"></param>
        /// <param name="uiElementId"></param>
        /// <returns></returns>
        public ServiceResult UpdateDataSource(List<DataSourceUiElementMappingModel> dataSourceUIElementModelList, int uiElementId, int tenantId, bool isEmptyDelete, string uiElementType, int formDesignId, int formDesignVersionId, List<int> existingDSIds, string userName)
        {
            Contract.Requires(tenantId > 0, "Invalid tenantId");
            Contract.Requires(uiElementId > 0, "Invalid uiElementId");
            Contract.Requires(uiElementType != null, "Invalid uiElementType");
            ServiceResult result = new ServiceResult();
            try
            {
                var dropDownDataSourceId = 0;

                if (dataSourceUIElementModelList != null && !isEmptyDelete)
                {
                    // Added Elements to child repeater 
                    if (uiElementType == "Repeater")
                    {
                        List<DataSourceUiElementMappingModel> generateKeyElements = dataSourceUIElementModelList.Where(x => x.IsKey == true && x.UIElementID == 0).ToList();
                        foreach (var item in generateKeyElements)
                        {
                            UIElement sourceUIElement = (from uiElement in this._unitOfWork.RepositoryAsync<UIElement>().Query().Filter(x => x.UIElementID == item.MappedUIElementID).Get()
                                                         select uiElement).FirstOrDefault();

                            UIElement parentUIElement = (from uiElement in this._unitOfWork.RepositoryAsync<UIElement>().Query().Filter(x => x.ParentUIElementID == uiElementId && x.Label == sourceUIElement.Label).Get()
                                                         select uiElement).FirstOrDefault();
                            if (parentUIElement == null)
                            {
                                _uiElementService.AddTextBox(userName, tenantId, formDesignVersionId, uiElementId, false, false, true, sourceUIElement.Label, "", 0, tmg.equinox.domain.entities.UIElementType.LABEL.ToString(),true,3);
                            }
                            int generatedTargetKey = (from target in this._unitOfWork.RepositoryAsync<UIElement>().Query().Filter(x => x.Label == sourceUIElement.Label && x.ParentUIElementID == uiElementId).Get() select target.UIElementID).FirstOrDefault();

                            dataSourceUIElementModelList.Where(x => x.MappedUIElementID == item.MappedUIElementID).ToList().ForEach(x => x.UIElementID = generatedTargetKey);

                        }
                    }
                    //Retrive distinct dataSourceIds from the input list
                    List<int> dataSourceIDs = dataSourceUIElementModelList.Select(c => c.DataSourceId).Distinct().ToList();
                    if (uiElementType == "Dropdown List" && dataSourceIDs.Count == 1)
                    {
                        dropDownDataSourceId = dataSourceIDs.FirstOrDefault();
                    }
                    if (uiElementType == "Dropdown TextBox" && dataSourceIDs.Count == 1)
                    {
                        dropDownDataSourceId = dataSourceIDs.FirstOrDefault();
                    }
                    List<DataSourceElementDisplayMode> displayModes = (from mode in this._unitOfWork.RepositoryAsync<DataSourceElementDisplayMode>()
                                                                             .Query()
                                                                             .Get()
                                                                       select mode).ToList();

                    List<DataSourceMode> dataSourceModeList = (from item in this._unitOfWork.RepositoryAsync<DataSourceMode>()
                                                                   .Query()
                                                                   .Get()
                                                               select item).ToList();
                    //Use Transaction scope for checking circular mapping after update Datasource Mapping
                    using (var scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(AppSettings.TransactionTimeOutPeriod)))
                    {
                        //If previously datasource is checked but next time remove whole datasource mapping for specific datasource 
                        DeleteUncheckedDataSourceMapping(formDesignId, formDesignVersionId, existingDSIds, dataSourceIDs);

                        foreach (var dataSourceID in dataSourceIDs)
                        {
                            List<DataSourceUiElementMappingModel> dataSourceMappingModelList = dataSourceUIElementModelList
                                                                                                .Where(c => c.DataSourceId == dataSourceID)
                                                                                                .ToList();
                            if (uiElementType != "Dropdown List" && uiElementType != "Dropdown TextBox")
                            {
                                //Delete existing datasource list which are not include in input list
                                DeleteExistingDataSourceMapping(formDesignId, formDesignVersionId, dataSourceMappingModelList, isEmptyDelete);
                            }

                            //Add dataSource mapping if that is not found n database , if found update taht data source list
                            UpdateDateSourceMapping(uiElementType, formDesignId, formDesignVersionId, displayModes, dataSourceModeList, dataSourceMappingModelList);
                        }


                        bool circularMappingExist = false;
                        var dataSourceMappingList = this._unitOfWork.RepositoryAsync<DataSourceMapping>()
                                                                           .Query()
                                                                           .Get()
                                                                           .Where(e => dataSourceIDs.Any(id => e.DataSourceID == id))
                                                                           .ToList();

                        foreach (DataSourceMapping dataSourceMappingModel in dataSourceMappingList)
                        {
                            //Check for circular Mapping of DataSource  
                            // If circular mapping found then updates not reflected in db
                            if (dataSourceMappingModel.UIElementID != dataSourceMappingModel.MappedUIElementID)
                            {
                                circularMappingExist = CheckForCircularDataSourceMapping(dataSourceMappingModel.UIElementID, dataSourceMappingModel.MappedUIElementID);
                                if (circularMappingExist)
                                {
                                    break;
                                }
                            }
                        }
                        if (!circularMappingExist)
                        {
                            result.Result = ServiceResultStatus.Success;
                            if (uiElementType == "Dropdown List" || uiElementType == "Dropdown TextBox")
                            {
                                ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem() { Messages = new string[] { dropDownDataSourceId.ToString() } });
                            }
                            scope.Complete();
                        }
                        else
                        {
                            result.Result = ServiceResultStatus.Failure;
                            ((IList<ServiceResultItem>)(result.Items)).Add(new ServiceResultItem() { Messages = new string[] { "Circular reference of datasource Mapping not allowed." } });
                        }
                    }

                }
                else if (dataSourceUIElementModelList != null && isEmptyDelete)
                {
                    //Delete all existing datasourcemapping for input elements ids if all are exclude from the list
                    DeleteExistingDataSourceMapping(formDesignId, formDesignVersionId, dataSourceUIElementModelList, isEmptyDelete);
                    result.Result = ServiceResultStatus.Success;
                }
            }
            catch (Exception ex)
            {
                result.Result = ServiceResultStatus.Failure;
                result = ex.ExceptionMessages();
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;

            }
            return result;
        }

        public IEnumerable<KeyValue> GetDataSourceFilterOperators()
        {
            ServiceResult result = new ServiceResult();
            IEnumerable<KeyValue> dataSourceFilterOperatorList = null;
            try
            {
                dataSourceFilterOperatorList = (from c in this._unitOfWork.RepositoryAsync<DataSourceOperatorMapping>()
                                                                               .Query()
                                                                               .Get()
                                                select new KeyValue
                                                {
                                                    Key = c.DataSourceOperatorID,
                                                    Value = c.DataSourceOperatorCode
                                                }).ToList();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                result = ex.ExceptionMessages();
            }
            return dataSourceFilterOperatorList;

        }

        /// <summary>
        /// Add DataSource UIElements Mapping
        /// </summary>
        /// <param name="uiElementId"></param>
        /// <param name="dataSourceId"></param>
        /// <param name="mappedUIElementId"></param>
        /// <returns></returns>
        public ServiceResult AddDataSourceUIElementMapping(int uiElementId, int dataSourceId, int mappedUIElementId)
        {
            Contract.Requires(uiElementId > 0, "Invalid uiElementId");
            Contract.Requires(dataSourceId > 0, "Invalid dataSourceId");
            Contract.Requires(mappedUIElementId > 0, "Invalid mappedUIElementId");
            ServiceResult result = new ServiceResult();
            try
            {
                var element = (from el in this._unitOfWork.RepositoryAsync<UIElement>()
                                                            .Query()
                                                            .Filter(c => c.UIElementID == uiElementId)
                                                            .Get()
                               select el).FirstOrDefault();

                if (element != null)
                {
                }

                DataSourceMapping mappingUpdate = (from c in this._unitOfWork.RepositoryAsync<DataSourceMapping>()
                                                                           .Query()
                                                                           .Filter(c => c.DataSourceID == dataSourceId && c.UIElementID == uiElementId)
                                                                           .Get()
                                                   select c).FirstOrDefault();
                if (mappingUpdate == null)
                {
                    DataSourceMapping mapping = new DataSourceMapping();
                    mapping.UIElementID = uiElementId;
                    mapping.DataSourceID = dataSourceId;
                    mapping.MappedUIElementID = mappedUIElementId;
                    this._unitOfWork.RepositoryAsync<DataSourceMapping>().Insert(mapping);
                    this._unitOfWork.Save();
                    result.Result = ServiceResultStatus.Success;
                }
                else
                {
                    mappingUpdate.DataSourceID = dataSourceId;
                    mappingUpdate.MappedUIElementID = mappedUIElementId;
                    this._unitOfWork.RepositoryAsync<DataSourceMapping>().Update(mappingUpdate);
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
        /// Get DataSourceElementDisplayModeModel
        /// </summary>
        /// <returns>List DataSourceElementDisplayModeModel </returns>
        public IEnumerable<DataSourceElementDisplayModeModel> GetDataSourceElementDisplayMode()
        {
            ServiceResult result = new ServiceResult();
            IEnumerable<DataSourceElementDisplayModeModel> displayModeList = null;
            try
            {
                displayModeList = (from c in this._unitOfWork.RepositoryAsync<DataSourceElementDisplayMode>()
                                                                               .Query()
                                                                               .Get()
                                   select new DataSourceElementDisplayModeModel
                                   {
                                       DataSourceElementDisplayModeID = c.DataSourceElementDisplayModeID,
                                       DisplayMode = c.DisplayMode
                                   }).ToList();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                result = ex.ExceptionMessages();
            }
            return displayModeList;
        }

        /// <summary>
        /// Get DataSourceDisplayModeModel
        /// </summary>
        /// <returns>List DataSourceElementDisplayModeModel </returns>
        public IEnumerable<DataSourceModeViewModel> GetDataSourceDisplayMode()
        {
            ServiceResult result = new ServiceResult();
            IEnumerable<DataSourceModeViewModel> displayDataSourceModeList = null;
            try
            {
                displayDataSourceModeList = (from c in this._unitOfWork.RepositoryAsync<DataSourceMode>()
                                                                               .Query()
                                                                               .Get()
                                             select new DataSourceModeViewModel
                                             {
                                                 DataSourceModeID = c.DataSourceModeID,
                                                 DataSourceModeType = c.DataSourceModeType
                                             }).ToList();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                result = ex.ExceptionMessages();
            }
            return displayDataSourceModeList;
        }

        /// <summary>
        /// Get DataCopyMode
        /// </summary>
        /// <returns>List DataCopyMode</returns>
        public IEnumerable<KeyValue> GetDataCopyMode()
        {
            ServiceResult result = new ServiceResult();
            IEnumerable<KeyValue> dataCopyModeList = null;
            try
            {
                dataCopyModeList = (from c in this._unitOfWork.RepositoryAsync<DataCopyMode>()
                                                                               .Query()
                                                                               .Get()
                                    select new KeyValue
                                    {
                                        Key = c.DataCopyModeID,
                                        Value = c.CopyData
                                    }).ToList();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                result = ex.ExceptionMessages();
            }
            return dataCopyModeList;
        }

        /// <summary>
        /// Update DataSource Display Mode
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="uiElementID"></param>
        /// <param name="displayModeID"></param>
        /// <returns></returns>
        public ServiceResult UpdateDataSourceDisplayMode(int tenantID, int uiElementID, int displayModeID)
        {
            Contract.Requires(tenantID > 0, "Invalid tenantID");
            Contract.Requires(uiElementID > 0, "Invalid uiElementID");
            Contract.Requires(displayModeID > 0, "Invalid displayModeID");
            //update UI Element Display Mode
            ServiceResult result = new ServiceResult();
            try
            {
                var element = (from elem in this._unitOfWork.RepositoryAsync<UIElement>()
                                .Query()
                                .Filter(c => c.UIElementID == uiElementID)
                                .Get()
                               select elem).FirstOrDefault();
                if (element != null)
                {
                    element.DataSourceElementDisplayModeID = displayModeID;
                    this._unitOfWork.RepositoryAsync<UIElement>().Update(element);
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

        public IEnumerable<UIElementSeqModel> GetChildUIElementsForDataSource(int tenantId, int formDesignId, int formDesignVersionId, int parentUIElementId, bool isKey)
        {
            List<UIElementSeqModel> uiElementRowModelList = null;

            try
            {
                List<UIElement> uiElementList = new List<UIElement>();
                var uiElements = this._unitOfWork.RepositoryAsync<UIElement>()
                                                            .Query()
                                                            .Filter(c => c.ParentUIElementID == parentUIElementId)
                                                            .Get()
                                                            .ToList();

                foreach (var uiElement in uiElements)
                {
                    var toMapUIElementType = GetElementType(uiElement);
                    if (!isKey)
                    {
                        if (toMapUIElementType != "Dropdown List" && toMapUIElementType != "Dropdown TextBox")
                        {

                            if (!(this._unitOfWork.RepositoryAsync<DataSourceMapping>()
                                                                    .Query()
                                                                    .Filter(c => c.UIElementID == uiElement.UIElementID
                                                                                 && c.FormDesignID == formDesignId
                                                                                 && c.FormDesignVersionID == formDesignVersionId)
                                                                    .Get()
                                                                    .Any()))
                            {
                                uiElementList.Add(uiElement);
                            }


                        }
                        else
                        {
                            uiElementList.Add(uiElement);
                        }
                    }
                    else
                    {
                        if (!(this._unitOfWork.RepositoryAsync<DataSourceMapping>()
                                                                .Query()
                                                                .Filter(c => c.UIElementID == uiElement.UIElementID
                                                                                && c.FormDesignID == formDesignId
                                                                                && c.FormDesignVersionID == formDesignVersionId)
                                                                .Get()
                                                                .Any()) && toMapUIElementType == "Label")
                        {
                            uiElementList.Add(uiElement);
                        }
                    }
                }

                var elementModels = from el in uiElementList
                                    select new UIElementSeqModel
                                    {
                                        Label = el.Label,
                                        Sequence = el.Sequence,
                                        UIElementID = el.UIElementID,
                                        ElementType = GetElementType(el)
                                    };

                uiElementRowModelList = elementModels.ToList();

            }
            catch (Exception ex)
            {
                throw;
            }
            return uiElementRowModelList;
        }

        public bool GetDataSourceMappingExistence(int uiElementId, int formDesignId, int formDesignVersionId)
        {
            var isMappingExist = false;
            try
            {

                isMappingExist = this._unitOfWork.Repository<DataSourceMapping>()
                                               .Query()
                                               .Filter(c => (c.UIElementID == uiElementId
                                                   && c.FormDesignID == formDesignId
                                                   && c.FormDesignVersionID == formDesignVersionId)
                                                         || (c.MappedUIElementID == uiElementId
                                                            && c.FormDesignID == formDesignId
                                                            && c.FormDesignVersionID == formDesignVersionId))
                                               .Get()
                                               .Any();

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return isMappingExist;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Return List of DropDown Data
        /// </summary>
        /// <param name="dropDownControlId"></param>
        /// <param name="IList<DataSourceRowModel>"></param>
        /// <param name="defaultUIElementType"></param>
        /// <param name="formId"></param>
        /// <param name="List<UIElement>"></param>
        /// <returns>DropDown Data</returns>
        private IList<DataSourceRowModel> GetDropdownData(int dropDownControlId, IList<DataSourceRowModel> rowModelList)
        {
            var parentUiElementID = (from ui in this._unitOfWork.RepositoryAsync<UIElement>()
                                                   .Query()
                                                   .Filter(ui => ui.UIElementID == dropDownControlId)
                                                   .Get()
                                     select (ui.ParentUIElementID)).FirstOrDefault();

            rowModelList = (from ru in this._unitOfWork.RepositoryAsync<RepeaterUIElement>().Get()
                            join ds in this._unitOfWork.Repository<DataSource>().Query().Include(c => c.DataSourceMappings).Get()
                            on ru.DataSourceID equals ds.DataSourceID
                            join fd in this._unitOfWork.Repository<FormDesign>().Get()
                            on ds.FormDesignID equals fd.FormID
                            where (ru.UIElementID != parentUiElementID && ru.DataSourceID > 0 && ru.DataSourceID != null)
                            select new DataSourceRowModel
                            {
                                DataSourceId = ds.DataSourceID,
                                DataSourceName = ds.DataSourceName,
                                DataSourceDescription = ds.DataSourceDescription,
                                DataSourceType = ds.Type,
                                IsCurrentDS = (ds.DataSourceMappings.Where(s => s.UIElementID == dropDownControlId).FirstOrDefault() != null ? "Y" : "N"),
                                AddedBy = ds.AddedBy,
                                AddedDate = ds.AddedDate,
                                UpdatedBy = ds.UpdatedBy,
                                UpdatedDate = ds.UpdatedDate,
                                DocumentName = fd.FormName,
                                UIElementID=ru.UIElementID
                            }).ToList();

            return rowModelList;
        }

        /// <summary>
        /// Return List of Repeater Data
        /// </summary>
        /// <param name="dropDownControlId"></param>
        /// <param name="IList<DataSourceRowModel>"></param>
        /// <param name="defaultUIElementType"></param>
        /// <param name="formId"></param>
        /// <param name="List<UIElement>"></param>
        /// <returns>Repeater Data</returns>
        private IList<DataSourceRowModel> GetRepeaterData(int uiElementId, IList<DataSourceRowModel> rowModelList, int formDesignVersionID)
        {
            var isPrimaryDataSourceMapping = (from ui in this._unitOfWork.Repository<UIElement>().Get()
                                              join dsm in this._unitOfWork.Repository<DataSourceMapping>().Get()
                                              on ui.UIElementID equals dsm.UIElementID
                                              where (ui.ParentUIElementID == uiElementId && dsm.IsPrimary == true)
                                              select ui).FirstOrDefault();
            int isPrimeId = 0;
            if (isPrimaryDataSourceMapping != null)
                isPrimeId = isPrimaryDataSourceMapping.UIElementID;

            rowModelList = (from su in this._unitOfWork.RepositoryAsync<RepeaterUIElement>().Get()
                            join ds in this._unitOfWork.Repository<DataSource>().Get()
                            on su.DataSourceID equals ds.DataSourceID
                            join fd in this._unitOfWork.Repository<FormDesign>().Get()
                            on ds.FormDesignID equals fd.FormID
                            where (su.UIElementID != uiElementId && su.DataSourceID > 0 && su.DataSourceID != null)
                            select new DataSourceRowModel
                            {
                                DataSourceId = ds.DataSourceID,
                                DataSourceName = ds.DataSourceName,
                                DataSourceDescription = ds.DataSourceDescription,
                                DataSourceType = ds.Type,
                                IsPrimary = ds.DataSourceMappings.Where(s => s.UIElementID == isPrimeId).FirstOrDefault() != null ? "Y" : "N",
                                IsCurrentDS = "N",
                                AddedBy = ds.AddedBy,
                                AddedDate = ds.AddedDate,
                                UpdatedBy = ds.UpdatedBy,
                                UpdatedDate = ds.UpdatedDate,
                                DocumentName = fd.FormName,
                                UIElementID=su.UIElementID,
                                DisplayMode = ds.DataSourceMappings.Where(e => e.FormDesignVersionID == formDesignVersionID)
                                                .Select(c => c.DataSourceElementDisplayModeID).FirstOrDefault()
                            }).ToList();



            return rowModelList;
        }

        /// <summary>
        /// Return List of Section Data
        /// </summary>
        /// <param name="dropDownControlId"></param>
        /// <param name="IList<DataSourceRowModel>"></param>
        /// <param name="defaultUIElementType"></param>
        /// <param name="formId"></param>
        /// <param name="List<UIElement>"></param>
        /// <returns>Section Data</returns>
        private IList<DataSourceRowModel> GetSectionsData(int uiElementId, IList<DataSourceRowModel> rowModelList)
        {
            rowModelList = (from su in this._unitOfWork.RepositoryAsync<SectionUIElement>().Get()
                            join ds in this._unitOfWork.Repository<DataSource>().Get()
                            on su.DataSourceID equals ds.DataSourceID
                            join fd in this._unitOfWork.Repository<FormDesign>().Get()
                            on ds.FormDesignID equals fd.FormID
                            where (su.UIElementID != uiElementId && su.DataSourceID > 0 && su.DataSourceID != null)//&& ds.Type == defaultUIElementType )
                            select new DataSourceRowModel
                            {
                                DataSourceId = ds.DataSourceID,
                                DataSourceName = ds.DataSourceName,
                                DataSourceDescription = ds.DataSourceDescription,
                                DataSourceType = ds.Type,
                                IsCurrentDS = "N",
                                AddedBy = ds.AddedBy,
                                AddedDate = ds.AddedDate,
                                UpdatedBy = ds.UpdatedBy,
                                UpdatedDate = ds.UpdatedDate,
                                DocumentName = fd.FormName,
                                UIElementID=su.UIElementID

                            }).ToList();

            return rowModelList;
        }

        /// <summary>
        /// Set Current DataSource
        /// </summary>
        /// <param name="IList<DataSourceRowModel>"></param>
        /// <param name="uiElementId"></param>
        /// <param name="uiElementType"></param>
        /// <returns></returns>
        private void SetCurrentDs(IList<DataSourceRowModel> rowModelList, int uiElementId, String uiElementType, int formDesignId, int formDesignVersionId)
        {
            List<DataSourceMapping> dataSourceMappings = null;

            if (uiElementType == "Repeater")
            {
                dataSourceMappings = (from c in this._unitOfWork.RepositoryAsync<DataSourceMapping>()
                                 .Query()
                                 .Filter(c => c.DataSourceElementDisplayModeID != (int)DisplayMode.DROPDOWN)
                                 .Get()
                                      select c).ToList();
            }
            else
            {
                dataSourceMappings = (from c in this._unitOfWork.RepositoryAsync<DataSourceMapping>()
                                .Query().Get()
                                      select c).ToList();
            }

            var uielements = (from c in this._unitOfWork.RepositoryAsync<UIElement>()
                              .Query().Get()
                              select c).ToList();

            if (dataSourceMappings == null || uielements == null)
                return;

            var distinctDataSourceIds = (from ds in dataSourceMappings
                                         join ui in uielements
                                         on ds.UIElementID equals ui.UIElementID
                                         where ui.ParentUIElementID == uiElementId
                                         select ds).Distinct().ToList();

            int idx = 0;
            foreach (DataSourceRowModel rw in rowModelList)
            {
                if (distinctDataSourceIds.Where(p => p.DataSourceID == rw.DataSourceId
                                                     && p.FormDesignID == formDesignId
                                                     && p.FormDesignVersionID == formDesignVersionId)
                                         .FirstOrDefault() != null)
                {
                    rw.IsCurrentDS = "Y";
                    rw.DisplayMode = distinctDataSourceIds.Where(p => p.DataSourceID == rw.DataSourceId
                                                                    && p.FormDesignID == formDesignId
                                                                    && p.FormDesignVersionID == formDesignVersionId)
                                                                     .Select(p => p.DataSourceElementDisplayModeID)
                                                                     .FirstOrDefault();
                    rw.DispalyDataSourceMode = distinctDataSourceIds.Where(p => p.DataSourceID == rw.DataSourceId
                                                                      && p.FormDesignID == formDesignId
                                                                      && p.FormDesignVersionID == formDesignVersionId)
                                                                      .Select(p => p.DataSourceModeID)
                                                                      .FirstOrDefault();
                    idx++;
                }
            }

        }

        /// <summary>
        /// Return Element Type
        /// </summary>
        /// <param name="UIElement"></param>
        /// <returns>Element Type</returns>
        private string GetElementType(UIElement uielement)
        {
            ServiceResult result = new ServiceResult();
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
                result = ex.ExceptionMessages();
                throw;
            }
            return uIElementType;
        }

        /// <summary>
        /// Resolve Element Type
        /// </summary>
        /// <param name="UIElement"></param>
        /// <returns>Element Type</returns>
        private string ResolveUIElementType(string uiElementType)
        {
            string defaultUIElementType = uiElementType;
            switch (uiElementType)
            {
                case "Multiline TextBox":
                case "Textbox":
                case "Calendar":
                case "Rich TextBox":
                case "Radio Button":
                    defaultUIElementType = "Section";
                    break;
                case "Dropdown List":
                case "Dropdown TextBox":
                    defaultUIElementType = "Repeater";
                    break;
                    //Include the other cases as required
            }

            return defaultUIElementType;
        }

        /// <summary>
        /// Return DispalyModeId
        /// </summary>
        /// <param name="UIElement"></param>
        /// <returns>DispalyModeId</returns>
        private int GetDisplayModeID(string uiElementType, List<DataSourceElementDisplayMode> displayModes)
        {
            int displayModeID = 0;
            switch (uiElementType)
            {
                case "Dropdown List":
                case "Dropdown TextBox":
                    displayModeID = displayModes.FirstOrDefault(c => c.DisplayMode == "Dropdown").DataSourceElementDisplayModeID;
                    break;
                case "Section":
                    displayModeID = displayModes.FirstOrDefault(c => c.DisplayMode == "Section").DataSourceElementDisplayModeID;
                    break;
                case "Repeater":
                    displayModeID = displayModes.FirstOrDefault(c => c.DisplayMode == "Primary").DataSourceElementDisplayModeID;
                    break;
            }
            return displayModeID;
        }

        private bool CheckForCircularDataSourceMapping(int targetUIElementId, int sourceUIElementId)
        {
            var result = false;

            var formDesignVersionId = this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>()
                                                                        .Query()
                                                                        .Filter(c => c.UIElementID == sourceUIElementId)
                                                                        .Get()
                                                                        .Select(c => c.FormDesignVersionID)
                                                                        .FirstOrDefault();
            //Return FormID of sourceUIElement
            int formDesignId = this._uiElementService.GetID(sourceUIElementId, "FormID");

            //Return ParentUIElement of targetUIElement
            var targetParentUIElementId = this._uiElementService.GetID(targetUIElementId, "ParentID");

            //Return ParentUIElement of sourceUIElement
            var sourceParentUIElementId = this._uiElementService.GetID(sourceUIElementId, "ParentID");

            var childUIelementList = this._unitOfWork.RepositoryAsync<UIElement>()
                                                                        .Query()
                                                                        .Filter(c => c.ParentUIElementID == sourceParentUIElementId
                                                                                     && c.UIElementID != sourceUIElementId)
                                                                        .Get()
                                                                        .Select(c => c.UIElementID)
                                                                        .ToList();

            if (targetParentUIElementId != sourceParentUIElementId)
            {
                //Return DataSourceMapping List for targetUIElement
                var targetMappingRowModelList = this._unitOfWork.RepositoryAsync<DataSourceMapping>()
                                                                        .Query()
                                                                        .Filter(c => c.MappedUIElementID == targetUIElementId
                                                                                    && c.FormDesignID == formDesignId
                                                                                    && c.FormDesignVersionID == formDesignVersionId)
                                                                        .Get()
                                                                        .ToList();
                if (targetMappingRowModelList.Count() > 1)
                {
                    targetMappingRowModelList = targetMappingRowModelList.Where(f => childUIelementList.Any(Id => f.UIElementID == Id)).ToList();
                }

                //Return DataSourceMapping for sourceUIElement
                var sourceMappingRowModel = this._unitOfWork.RepositoryAsync<DataSourceMapping>()
                                                                        .Query()
                                                                        .Filter(c => c.UIElementID == sourceUIElementId
                                                                                    && c.FormDesignID == formDesignId
                                                                                    && c.FormDesignVersionID == formDesignVersionId)
                                                                        .Get()
                                                                        .FirstOrDefault();
                if (targetMappingRowModelList.Any())
                {
                    foreach (DataSourceMapping targetMappingRowModel in targetMappingRowModelList)
                    {

                        //Recursive call to check CircularDataSourceMapping exist or not
                        if (targetMappingRowModel != null && sourceMappingRowModel != null)
                        {

                            //Return ParentUIElement of targetUIElement
                            var targetUIElementParentId = this._uiElementService.GetID(targetMappingRowModel.UIElementID, "ParentID");

                            //Return ParentUIElement of sourceUIElement
                            var sourceUIElementParentId = this._uiElementService.GetID(sourceMappingRowModel.UIElementID, "ParentID");

                            if (targetUIElementParentId == sourceUIElementParentId)
                            {
                                return true;
                            }
                            else
                            {
                                result = CheckForCircularDataSourceMapping(targetMappingRowModel.UIElementID, sourceMappingRowModel.MappedUIElementID);
                                if (result)
                                    return result;
                            }
                        }
                        else if (targetMappingRowModel == null && sourceMappingRowModel == null)
                        {
                            return false;
                        }
                        else if (targetMappingRowModel != null)
                        {
                            result = CheckForCircularDataSourceMapping(targetMappingRowModel.UIElementID, sourceUIElementId);
                            if (result)
                                return result;
                        }
                        else if (sourceMappingRowModel != null)
                        {
                            result = CheckForCircularDataSourceMapping(targetUIElementId, sourceMappingRowModel.MappedUIElementID);
                            if (result)
                                return result;
                        }
                    }
                }
                else
                {
                    if (targetMappingRowModelList.Count() < 1 && sourceMappingRowModel == null)
                    {
                        return false;
                    }
                    else if (sourceMappingRowModel != null)
                    {
                        //Return ParentUIElement of targetUIElement
                        var uiElementParentId = this._uiElementService.GetID(targetUIElementId, "ParentID");

                        //Return ParentUIElement of sourceUIElement
                        var mappedUIElementParentId = this._uiElementService.GetID(sourceMappingRowModel.MappedUIElementID, "ParentID");

                        if (uiElementParentId == mappedUIElementParentId)
                        {
                            return true;
                        }
                        else
                        {
                            result = CheckForCircularDataSourceMapping(targetUIElementId, sourceMappingRowModel.MappedUIElementID);
                            if (result)
                                return result;
                        }
                    }
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        //Add or Update DataSourceMapping
        private void UpdateDateSourceMapping(string uiElementType, int formDesignId, int formDesignVersionId, List<DataSourceElementDisplayMode> displayModes, List<DataSourceMode> dataSourceModeList, List<DataSourceUiElementMappingModel> dataSourceMappingModelList)
        {
            foreach (var dataSourceMappingModel in dataSourceMappingModelList)
            {

                int dataSourceDisplayModeID = 0;
                int dataSourceModeID = 0;
                if (dataSourceMappingModel.IsPrimary == true)
                {
                    dataSourceDisplayModeID = GetDisplayModeID(uiElementType, displayModes);
                    dataSourceModeID = (int)dataSourceMappingModel.DataSourceModeID;
                }
                else
                {
                    if (dataSourceMappingModel.DataSourceElementDisplayModeID.HasValue == false)
                    {
                        dataSourceDisplayModeID = GetDisplayModeID(uiElementType, displayModes);
                    }
                    else
                    {
                        dataSourceDisplayModeID = dataSourceMappingModel.DataSourceElementDisplayModeID.HasValue ? dataSourceMappingModel.DataSourceElementDisplayModeID.Value : (int)DisplayMode.NONE;
                    }
                    if (dataSourceMappingModel.DataSourceModeID.HasValue == false)
                    {
                        dataSourceModeID = GetDataSourceMode(uiElementType, dataSourceModeList);
                    }
                    else
                    {
                        dataSourceModeID = (int)dataSourceMappingModel.DataSourceModeID;
                    }
                }
                if (uiElementType != "Dropdown List" && uiElementType != "Dropdown TextBox")
                {
                    var uiElement = this._unitOfWork.RepositoryAsync<UIElement>().FindById(dataSourceMappingModel.UIElementID);
                    var toMapUIElementType = GetElementType(uiElement);
                    DataSourceMapping toUpdateDataSource = null;
                    if (toMapUIElementType != "Dropdown List" && toMapUIElementType != "Dropdown TextBox")
                    {
                        toUpdateDataSource = this._unitOfWork.RepositoryAsync<DataSourceMapping>()
                                                               .Query()
                                                               .Filter(c => c.UIElementID == dataSourceMappingModel.UIElementID
                                                                        && c.FormDesignID == formDesignId
                                                                        && c.FormDesignVersionID == formDesignVersionId)
                                                               .Get()
                                                               .FirstOrDefault();
                    }
                    else
                    {
                        toUpdateDataSource = this._unitOfWork.RepositoryAsync<DataSourceMapping>()
                                                              .Query()
                                                              .Filter(c => c.UIElementID == dataSourceMappingModel.UIElementID
                                                                       && c.DataSourceElementDisplayModeID == (int)DisplayMode.INLINE
                                                                       && c.FormDesignID == formDesignId
                                                                       && c.FormDesignVersionID == formDesignVersionId)
                                                              .Get()
                                                              .FirstOrDefault();
                    }

                    if (toUpdateDataSource != null)
                    {
                        SetDataSourceMapping(formDesignId, formDesignVersionId, dataSourceMappingModel, dataSourceDisplayModeID, dataSourceModeID, toUpdateDataSource);
                        this._unitOfWork.RepositoryAsync<DataSourceMapping>().Update(toUpdateDataSource);
                        this._unitOfWork.Save();

                    }
                    else
                    {
                        DataSourceMapping toAddMapping = new DataSourceMapping();
                        SetDataSourceMapping(formDesignId, formDesignVersionId, dataSourceMappingModel, dataSourceDisplayModeID, dataSourceModeID, toAddMapping);
                        this._unitOfWork.RepositoryAsync<DataSourceMapping>().Insert(toAddMapping);
                        this._unitOfWork.Save();

                    }
                }
                else
                {
                    var isInlineConfigurationExist = this._unitOfWork.RepositoryAsync<DataSourceMapping>()
                                                           .Query()
                                                           .Filter(c => c.UIElementID == dataSourceMappingModel.UIElementID
                                                                    && c.DataSourceID == dataSourceMappingModel.DataSourceId
                                                                    && c.DataSourceElementDisplayModeID == (int)DisplayMode.INLINE
                                                                    && c.FormDesignID == formDesignId
                                                                    && c.FormDesignVersionID == formDesignVersionId)
                                                           .Get()
                                                           .Any();
                    if (!isInlineConfigurationExist)
                    {

                        var toUpdateDataSource = this._unitOfWork.RepositoryAsync<DataSourceMapping>()
                                                               .Query()
                                                               .Filter(c => c.UIElementID == dataSourceMappingModel.UIElementID
                                                                        && c.DataSourceElementDisplayModeID == (int)DisplayMode.DROPDOWN
                                                                        && c.FormDesignID == formDesignId
                                                                        && c.FormDesignVersionID == formDesignVersionId)
                                                               .Get()
                                                               .FirstOrDefault();
                        if (toUpdateDataSource != null)
                        {
                            SetDataSourceMapping(formDesignId, formDesignVersionId, dataSourceMappingModel, dataSourceDisplayModeID, dataSourceModeID, toUpdateDataSource);
                            this._unitOfWork.RepositoryAsync<DataSourceMapping>().Update(toUpdateDataSource);
                            this._unitOfWork.Save();

                        }
                        else
                        {
                            DataSourceMapping toAddMapping = new DataSourceMapping();
                            SetDataSourceMapping(formDesignId, formDesignVersionId, dataSourceMappingModel, dataSourceDisplayModeID, dataSourceModeID, toAddMapping);
                            this._unitOfWork.RepositoryAsync<DataSourceMapping>().Insert(toAddMapping);
                            this._unitOfWork.Save();
                        }
                    }
                }

            }
        }

        private static void SetDataSourceMapping(int formDesignId, int formDesignVersionId, DataSourceUiElementMappingModel dataSourceMappingModel, int dataSourceDisplayModeID, int dataSourceModeID, DataSourceMapping toAddMapping)
        {
            toAddMapping.DataSourceID = dataSourceMappingModel.DataSourceId;
            toAddMapping.UIElementID = dataSourceMappingModel.UIElementID;
            toAddMapping.MappedUIElementID = dataSourceMappingModel.MappedUIElementID;
            toAddMapping.IsPrimary = dataSourceMappingModel.IsPrimary;
            toAddMapping.DataSourceElementDisplayModeID = dataSourceDisplayModeID;
            toAddMapping.DataSourceModeID = dataSourceModeID;
            toAddMapping.DataCopyModeID = dataSourceMappingModel.DataCopyModeID;
            toAddMapping.DataSourceFilter = dataSourceMappingModel.DataSourceFilter;
            toAddMapping.FormDesignID = formDesignId;
            toAddMapping.FormDesignVersionID = formDesignVersionId;
            toAddMapping.IsKey = dataSourceMappingModel.IsKey;
            toAddMapping.DataSourceOperatorID = dataSourceMappingModel.DataSourceMappingOperatorID > 0 ? dataSourceMappingModel.DataSourceMappingOperatorID : null;
        }

        //Delete all existing datasourcemapping
        private void DeleteExistingDataSourceMapping(int formDesignId, int formDesignVersionId, List<DataSourceUiElementMappingModel> dataSourceMappingModelList, bool isEmptyDelete)
        {
            List<int> toDeleteUIElementIdList = dataSourceMappingModelList.Select(c => c.UIElementID).ToList();

            var uiElementModelList = (from list in this._unitOfWork.RepositoryAsync<UIElement>()
                                                                .Query()
                                                                .Filter(f => toDeleteUIElementIdList.Any(Id => f.UIElementID == Id))
                                                                .Get()
                                      select new
                                      {
                                          UIElementID = list.UIElementID,
                                          ParentUIElementID = list.ParentUIElementID
                                      }).ToList();

            List<int> dataSourceIDs = dataSourceMappingModelList.Select(c => c.DataSourceId).Distinct().ToList();

            foreach (var dataSourceId in dataSourceIDs)
            {

                List<DataSourceMapping> toDeleteDataSourceMappingList = this._unitOfWork.RepositoryAsync<DataSourceMapping>()
                                                                .Query()
                                                                .Include(f => f.UIElement)
                                                                .Get()
                                                                .Where(e => (isEmptyDelete ? toDeleteUIElementIdList.Any(id => e.UIElementID == id) : toDeleteUIElementIdList.All(id => e.UIElementID != id))
                                                                        && e.DataSourceID == dataSourceId
                                                                        && e.FormDesignID == formDesignId
                                                                        && e.FormDesignVersionID == formDesignVersionId)
                                                                .ToList();



                if (toDeleteDataSourceMappingList != null && toDeleteDataSourceMappingList.Count() > 0)
                {
                    foreach (var toDeleteDataSourceMapping in toDeleteDataSourceMappingList)
                    {
                        if (uiElementModelList.Any(p => p.ParentUIElementID == toDeleteDataSourceMapping.UIElement.ParentUIElementID))
                        {
                            this._unitOfWork.RepositoryAsync<DataSourceMapping>().Delete(toDeleteDataSourceMapping);
                        }
                    }
                }
            }
            this._unitOfWork.Save();
        }

        private void DeleteUncheckedDataSourceMapping(int formDesignId, int formDesignVersionId, List<int> existingDSIds, List<int> dataSourceIDs)
        {
            if (existingDSIds != null && existingDSIds.Count > 0)
            {
                var toDeleteDataSourceIdList = existingDSIds.Where(f => !dataSourceIDs.Any(a => a == f)).ToList();
                if (toDeleteDataSourceIdList.Count > 0)
                {

                    var toDeleteDataSourceMappingList = this._unitOfWork.RepositoryAsync<DataSourceMapping>()
                                                                .Query()
                                                                .Filter(e => toDeleteDataSourceIdList.Any(Id => e.DataSourceID == Id)
                                                                        && e.FormDesignID == formDesignId
                                                                        && e.FormDesignVersionID == formDesignVersionId)
                                                                .Get()
                                                                .ToList();

                    if (toDeleteDataSourceMappingList != null && toDeleteDataSourceMappingList.Count() > 0)
                    {
                        foreach (var toDeleteDataSourceMapping in toDeleteDataSourceMappingList)
                        {
                            this._unitOfWork.RepositoryAsync<DataSourceMapping>().Delete(toDeleteDataSourceMapping);
                        }
                        this._unitOfWork.Save();
                    }
                }
            }
        }

        private List<UIElementRowModel> GetDataSourceMappingForContainer(string uiElementType, int formDesignId, int formDesignVersionId, List<UIElementRowModel> rowModelList, string defaultElementType, List<UIElement> uiElementslList, List<DataSourceMapping> filter, List<UIElement> mappedUIElement)
        {
            if (uiElementType != "Dropdown List" && uiElementType != "Dropdown TextBox")
            {
                List<UIElement> getPrimaryRecords = new List<UIElement>();
                List<UIElement> records = new List<UIElement>();
                if (defaultElementType == "Repeater")
                {
                    var mappingsdata = (from c in this._unitOfWork.RepositoryAsync<DataSourceMapping>()
                                                                            .Query()
                                                                            .Filter(d => (d.DataSourceElementDisplayModeID == (int)DisplayMode.INLINE || d.DataSourceElementDisplayModeID == (int)DisplayMode.CHILD)
                                                                            && d.FormDesignID == formDesignId
                                                                            && d.FormDesignVersionID == formDesignVersionId)
                                                                            .Get()
                                        select c).ToList();


                    getPrimaryRecords = (from u in uiElementslList
                                         join g in mappingsdata
                                         on u.UIElementID equals g.UIElementID
                                         select u).ToList();

                    var exceptRecords = uiElementslList.Except(getPrimaryRecords);

                    foreach (var r in exceptRecords)
                        records.Add(r);
                }


                if (records.Count > 0)
                {
                    rowModelList = (from u in uiElementslList
                                    join g in records
                                    on u.UIElementID equals g.UIElementID
                                    where u.GetType() != typeof(SectionUIElement)
                                    && u.GetType() != typeof(RepeaterUIElement)

                                    select new UIElementRowModel
                                    {
                                        UIElementID = u.UIElementID,
                                        ElementType = GetElementType(u),
                                        UIElementName = u.UIElementName,
                                        loaded = filter.Any(m => m.MappedUIElementID == u.UIElementID) ? true : false,
                                        Label = u.Label,
                                        AddedBy = u.AddedBy,
                                        AddedDate = u.AddedDate,
                                        UpdatedBy = u.UpdatedBy,
                                        UpdatedDate = u.UpdatedDate,
                                        MappedUIElementId = mappedUIElement.Count > 0 && filter.Any(m => m.MappedUIElementID == u.UIElementID) ? mappedUIElement.Where(p => p.UIElementID == filter.Find(m => m.MappedUIElementID == u.UIElementID).UIElementID).FirstOrDefault().UIElementID : 0,
                                        MappedUIElementName = mappedUIElement.Count > 0 && filter.Any(m => m.MappedUIElementID == u.UIElementID) ? mappedUIElement.Where(p => p.UIElementID == filter.Find(m => m.MappedUIElementID == u.UIElementID).UIElementID).FirstOrDefault().Label : string.Empty,
                                        MappedUIElementType = mappedUIElement.Count > 0 && filter.Any(m => m.MappedUIElementID == u.UIElementID) ? GetElementType(mappedUIElement.Where(p => p.UIElementID == filter.Find(m => m.MappedUIElementID == u.UIElementID).UIElementID).FirstOrDefault()) : string.Empty,
                                        DataCopyModeID = mappedUIElement.Count > 0 && filter.Any(m => m.MappedUIElementID == u.UIElementID) ? mappedUIElement.Where(p => p.UIElementID == filter.Find(m => m.MappedUIElementID == u.UIElementID).UIElementID).FirstOrDefault().DataSourceMappings.FirstOrDefault().DataCopyModeID : 0,
                                        DataSourceFilterValue = mappedUIElement.Count > 0 && filter.Any(m => m.MappedUIElementID == u.UIElementID) ? mappedUIElement.Where(p => p.UIElementID == filter.Find(m => m.MappedUIElementID == u.UIElementID).UIElementID).FirstOrDefault().DataSourceMappings.FirstOrDefault().DataSourceFilter : string.Empty,
                                        DataSourceFilterOperatorID = mappedUIElement.Count > 0 && filter.Any(m => m.MappedUIElementID == u.UIElementID) ? mappedUIElement.Where(p => p.UIElementID == filter.Find(m => m.MappedUIElementID == u.UIElementID).UIElementID).FirstOrDefault().DataSourceMappings.FirstOrDefault().DataSourceOperatorID : null,
                                        IsKey = mappedUIElement.Count > 0 && filter.Any(m => m.MappedUIElementID == u.UIElementID) ? mappedUIElement.Where(p => p.UIElementID == filter.Find(m => m.MappedUIElementID == u.UIElementID).UIElementID).FirstOrDefault().DataSourceMappings.FirstOrDefault().IsKey : false,
                                        IsRepeaterKey = IsRepeaterKey(u.UIElementID)
                                    }).ToList();

                    var unMappedRowModelList = GetUIElementRowModelForUnMappedElements(filter);

                    if (unMappedRowModelList != null && unMappedRowModelList.Any())
                    {
                        rowModelList.AddRange(unMappedRowModelList);
                    }
                }
                else
                {

                    rowModelList = (from u in uiElementslList
                                    where u.GetType() != typeof(SectionUIElement) && u.GetType() != typeof(RepeaterUIElement)
                                    select new UIElementRowModel
                                    {
                                        UIElementID = u.UIElementID,
                                        ElementType = GetElementType(u),
                                        UIElementName = u.UIElementName,
                                        loaded = filter.Any(m => m.MappedUIElementID == u.UIElementID) ? true : false,
                                        Label = u.Label,
                                        AddedBy = u.AddedBy,
                                        AddedDate = u.AddedDate,
                                        UpdatedBy = u.UpdatedBy,
                                        UpdatedDate = u.UpdatedDate,
                                        MappedUIElementId = mappedUIElement.Count > 0 && filter.Any(m => m.MappedUIElementID == u.UIElementID) ? mappedUIElement.Where(p => p.UIElementID == filter.Find(m => m.MappedUIElementID == u.UIElementID).UIElementID).FirstOrDefault().UIElementID : 0,
                                        MappedUIElementName = mappedUIElement.Count > 0 && filter.Any(m => m.MappedUIElementID == u.UIElementID) ? mappedUIElement.Where(p => p.UIElementID == filter.Find(m => m.MappedUIElementID == u.UIElementID).UIElementID).FirstOrDefault().Label : string.Empty,
                                        DataCopyModeID = mappedUIElement.Count > 0 && filter.Any(m => m.MappedUIElementID == u.UIElementID) ? mappedUIElement.Where(p => p.UIElementID == filter.Find(m => m.MappedUIElementID == u.UIElementID).UIElementID).FirstOrDefault().DataSourceMappings.FirstOrDefault().DataCopyModeID : 0
                                    }).ToList();
                }

            }
            else
            {
                rowModelList = (from u in uiElementslList
                                where u.GetType() != typeof(SectionUIElement) && u.GetType() != typeof(RepeaterUIElement)
                                && u.GetType() != typeof(CalendarUIElement) && u.GetType() != typeof(RadioButtonUIElement)
                                && u.GetType() != typeof(CheckBoxUIElement)
                                select new UIElementRowModel
                                {
                                    UIElementID = u.UIElementID,
                                    ElementType = GetElementType(u),
                                    UIElementName = u.UIElementName,
                                    loaded = filter.Any(m => m.MappedUIElementID == u.UIElementID) ? true : false,
                                    Label = u.Label,
                                    AddedBy = u.AddedBy,
                                    AddedDate = u.AddedDate,
                                    UpdatedBy = u.UpdatedBy,
                                    UpdatedDate = u.UpdatedDate,
                                    MappedUIElementId = mappedUIElement.Count > 0 && filter.Any(m => m.MappedUIElementID == u.UIElementID) ? mappedUIElement.FirstOrDefault().UIElementID : 0,
                                    MappedUIElementName = mappedUIElement.Count > 0 && filter.Any(m => m.MappedUIElementID == u.UIElementID) ? mappedUIElement.FirstOrDefault().Label : string.Empty
                                }).ToList();
            }
            return rowModelList;
        }

        private List<UIElementRowModel> GetUIElementRowModelForUnMappedElements(List<DataSourceMapping> filter)
        {
            List<UIElementRowModel> rowModelList = new List<UIElementRowModel>();
            List<int> unMappedList = new List<int>();

            foreach (var item in filter)
            {
                if (item.UIElementID == item.MappedUIElementID)
                {
                    unMappedList.Add(item.UIElementID);
                }
            }

            if (unMappedList.Any())
            {
                var unMappedElementList = this._unitOfWork.RepositoryAsync<UIElement>()
                                          .Query()
                                          .Filter(e => unMappedList.Any(id => id == e.UIElementID))
                                          .Get()
                                          .ToList();

                foreach (var item in unMappedElementList)
                {
                    rowModelList.Add(new UIElementRowModel
                    {
                        UIElementID = item.UIElementID,
                        ElementType = GetElementType(item),
                        UIElementName = item.UIElementName,
                        loaded = filter.Any(m => m.MappedUIElementID == item.UIElementID) ? true : false,
                        Label = item.Label,
                        AddedBy = item.AddedBy,
                        AddedDate = item.AddedDate,
                        UpdatedBy = item.UpdatedBy,
                        UpdatedDate = item.UpdatedDate,
                        MappedUIElementId = item.UIElementID,
                        MappedUIElementName = item.Label,
                        MappedUIElementType = GetElementType(item),
                        DataCopyModeID = null,
                        DataSourceFilterValue = null,
                        DataSourceFilterOperatorID = null,
                        IsKey = false,
                    });
                }
            }

            return rowModelList;
        }

        private List<DataSourceMapping> GetMappingsForUielement(int uiElementId, string uiElementType, int dataSourceId, int formDesignId, int formDesignVersionId)
        {
            List<DataSourceMapping> uiElementMappings = null;
            List<UIElement> selectedParentChildList = new List<UIElement>();
            List<DataSourceMapping> filter = new List<DataSourceMapping>();
            if (uiElementType != "Dropdown List" && uiElementType != "Dropdown TextBox")
            {
                uiElementMappings = (from c in this._unitOfWork.RepositoryAsync<DataSourceMapping>()
                                                    .Query()
                                                    .Filter(c => c.DataSourceID == dataSourceId
                                                                && c.DataSourceElementDisplayModeID != (int)DisplayMode.DROPDOWN
                                                               && c.FormDesignID == formDesignId
                                                               && c.FormDesignVersionID == formDesignVersionId)
                                                     .Get()
                                     select c).ToList();

                //Get all the datasources of a given parent uielement which is the datasource
                selectedParentChildList = (from c in this._unitOfWork.RepositoryAsync<UIElement>()
                                                                       .Query()
                                                                       .Filter(c => c.ParentUIElementID == uiElementId)
                                                                       .Get()
                                           select c).ToList();

                filter = (from c in selectedParentChildList
                          join m in uiElementMappings
                          on c.UIElementID equals m.UIElementID
                          where c.ParentUIElementID == uiElementId
                          select m).ToList();

            }
            else
            {
                uiElementMappings = (from c in this._unitOfWork.RepositoryAsync<DataSourceMapping>()
                                                                     .Query()
                                                                     .Filter(c => c.DataSourceID == dataSourceId
                                                                     && c.FormDesignID == formDesignId
                                                                     && c.FormDesignVersionID == formDesignVersionId)
                                                                     .Get()
                                     select c).ToList();

                selectedParentChildList = (from c in this._unitOfWork.RepositoryAsync<UIElement>()
                                                                          .Query()
                                                                          .Filter(c => c.UIElementID == uiElementId)
                                                                          .Get()
                                           select c).ToList();

                filter = (from c in selectedParentChildList
                          join m in uiElementMappings
                          on c.UIElementID equals m.UIElementID
                          select m).ToList();
            }
            return filter;
        }

        private int GetUIelementIdOfContainer(int dataSourceId, string defaultElementType, int parentUIElementId)
        {
            if (defaultElementType == "Section")
            {
                parentUIElementId = (from c in this._unitOfWork.RepositoryAsync<SectionUIElement>()
                                                                       .Query()
                                                                       .Filter(c => c.DataSourceID == dataSourceId)
                                                                       .Get()
                                                                       .OrderByDescending(c => c.UIElementID)
                                     select c.UIElementID).FirstOrDefault();
            }
            else if (defaultElementType == "Repeater")
            {
                parentUIElementId = (from c in this._unitOfWork.RepositoryAsync<RepeaterUIElement>()
                                                                      .Query()
                                                                      .Filter(c => c.DataSourceID == dataSourceId)
                                                                      .Get()
                                                                      .OrderByDescending(c => c.UIElementID)
                                     select c.UIElementID).FirstOrDefault();
            }
            return parentUIElementId;
        }

        public bool IsFinalizedFormDesignVersionExists(int formDesignId)
        {
            var isFinalizedVersionExists = this._unitOfWork.RepositoryAsync<FormDesignVersion>()
                                               .Query()
                                               .Filter(fil => fil.FormDesignID == formDesignId &&
                                                   fil.StatusID == (int)domain.entities.Status.Finalized)
                                               .Get()
                                               .Any();
            return isFinalizedVersionExists;
        }

        private int GetDataSourceMode(string uiElementType, List<DataSourceMode> dataSourceModeList)
        {
            int dataSourceModeID = 0;
            switch (uiElementType)
            {
                case "Dropdown List":
                case "Dropdown TextBox":
                    dataSourceModeID = dataSourceModeList.FirstOrDefault(c => c.DataSourceModeType == "Auto").DataSourceModeID;
                    break;
                case "Section":
                    dataSourceModeID = dataSourceModeList.FirstOrDefault(c => c.DataSourceModeType == "Auto").DataSourceModeID;
                    break;
                case "Repeater":
                    dataSourceModeID = dataSourceModeList.FirstOrDefault(c => c.DataSourceModeType == "Auto").DataSourceModeID;
                    break;
            }
            return dataSourceModeID;
        }

        private bool IsRepeaterKey(int uiElementId)
        {

            int uiElementTypeId = (from uiElement in this._unitOfWork.RepositoryAsync<UIElement>().Query().Filter(x => x.UIElementID == uiElementId).Get()
                                   join parentUIElement in this._unitOfWork.RepositoryAsync<UIElement>().Get() on uiElement.ParentUIElementID equals parentUIElement.UIElementID
                                   select parentUIElement.UIElementDataTypeID).FirstOrDefault();

            if (uiElementTypeId == (int)tmg.equinox.domain.entities.UIElementType.REPEATER)
            {
                int keyCount = (from s in this._unitOfWork.RepositoryAsync<RepeaterKeyUIElement>().Query().Filter(x => x.UIElementID == uiElementId).Get() select s).Count();
                return keyCount > 0;
            }

            else
            {
                return false;
            }
        }
        #endregion Private Methods
    }
}
