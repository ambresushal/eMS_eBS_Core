using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using tmg.equinox.applicationservices.FormInstanceDetail;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.FormDesign;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.repository.extensions;
using tmg.equinox.repository.interfaces;
using FolderVersionState = tmg.equinox.domain.entities.Enums.FolderVersionState;

namespace tmg.equinox.applicationservices
{
    public partial class FormInstanceRepeaterService : IFormInstanceRepeaterService
    {
        #region Private Memebers
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        private ILoggingService _loggingService { get; set; }
        private IFolderVersionServices _folderVersionServices { get; set; }

        #endregion Private Memebers

        #region Constructor
        public FormInstanceRepeaterService(IUnitOfWorkAsync unitOfWork, ILoggingService loggingService, IFolderVersionServices folderVersionService)
        {
            this._unitOfWork = unitOfWork;
            this._loggingService = loggingService;
            this._folderVersionServices = folderVersionService;
        }
        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// This method is used to get FormInstanceRepeater data.
        /// </summary>
        /// <param name="formInstanceId"></param>
        /// <param name="fullName"></param>
        /// <param name="gridPagingRequest"></param>
        /// <returns></returns>
        public GridPagingResponse<object> GetFormInstanceRepeaterData(int formInstanceId, string fullName, GridPagingRequest gridPagingRequest)
        {
            IList<object> formInstanceRepeaterDatalist = new List<object>();
            int count = 0;
            ServiceResult result = new ServiceResult();

            try
            {
                SearchCriteria criteria = new SearchCriteria();
                criteria = JqGridHelper.GetCriteria(gridPagingRequest.filters);

                FormInstanceRepeaterDataMap formInstanceRepeater = this._unitOfWork.RepositoryAsync<FormInstanceRepeaterDataMap>().Query()
                                                         .Filter(e => e.FormInstanceID == formInstanceId && e.FullName == fullName && e.IsActive == true)
                                                         .Get().FirstOrDefault();

                string[] Elements = fullName.Split('.');
                string repeaterName = Elements.Last();

                if (formInstanceRepeater != null)
                {
                    JObject repeaterInstanceObject = JObject.Parse(formInstanceRepeater.RepeaterData);
                    JArray formInstanceRepeaterData = (JArray)repeaterInstanceObject[repeaterName];
                    List<JToken> formInstanceRepeaterDataList = formInstanceRepeaterData.ToList();

                    //if search element in grid
                    List<JToken> filteredRepeaterDataList = new List<JToken>();
                    if (gridPagingRequest._search == true)
                    {
                        int ruleCount = criteria.rules.Count();
                        int j = 0;
                        foreach (var rule in criteria.rules)
                        {
                            j++;
                            filteredRepeaterDataList = formInstanceRepeaterDataList.Where(c => c[rule.field].ToString().Contains(rule.data)).ToList();
                            if (ruleCount > j)
                            {
                                formInstanceRepeaterDataList = filteredRepeaterDataList;
                                filteredRepeaterDataList = new List<JToken>();
                            }
                        }
                    }
                    else
                    {
                        filteredRepeaterDataList = formInstanceRepeaterDataList;
                    }
                    //for Ascending/ Descending order
                    if (gridPagingRequest.sord == "desc")
                    {
                        List<JToken> repeaterDescList = filteredRepeaterDataList.OrderByDescending(c => (int)c["RowIDProperty"]).ToList();
                        formInstanceRepeaterDatalist = repeaterDescList.Select(m => (object)m).ToList()
                                             .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count);
                    }
                    else
                    {
                        List<JToken> repeaterAscList = filteredRepeaterDataList.OrderBy(c => (int)c["RowIDProperty"]).ToList();
                        formInstanceRepeaterDatalist = repeaterAscList.Select(m => (object)m).ToList()
                                             .GetPage(gridPagingRequest.page, gridPagingRequest.rows, out count);
                    }

                }
                else
                {
                    formInstanceRepeaterDatalist = new List<object>();
                }
                if (formInstanceRepeaterDatalist.Count() == 0)
                    gridPagingRequest.page = 0;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                //Get all the exception/inner exception messages and set the return code to failure
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
            }
            return new GridPagingResponse<object>(gridPagingRequest.page, count, gridPagingRequest.rows, formInstanceRepeaterDatalist);
        }
        /// <summary>
        /// This method is used to save existing data of repeater in FormInstanceRepeaterDataMap table for which LoadFromServer is true 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formInstanceId"></param>
        /// <param name="formDesignVersionId"></param>
        /// <param name="fullNameList"></param>
        /// <param name="repeaterData"></param>
        /// <returns></returns>
        public ServiceResult SaveFormInstanceRepeaterData(int formInstanceId, string[] fullNameList, string repeaterData, string userName, int formDesignVersionId)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                using (var scope = new TransactionScope())
                {
                    foreach (string fullPath in fullNameList)
                    {
                        FormInstanceRepeaterDataMap itemToUpdate = this._unitOfWork.RepositoryAsync<FormInstanceRepeaterDataMap>().Query()
                                                          .Filter(e => e.FormInstanceID == formInstanceId && e.FullName == fullPath)
                                                          .Get().FirstOrDefault();

                        string[] elements = fullPath.Split('.');
                        string sectionName = elements.First();
                        string repeaterName = elements.Last();


                        string sectionId = (from uiElement in this._unitOfWork.RepositoryAsync<UIElement>().Get()
                                            join uiElementMap in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Get()
                                            on uiElement.UIElementID equals uiElementMap.UIElementID
                                            where uiElementMap.FormDesignVersionID == formDesignVersionId && uiElement.GeneratedName == sectionName
                                            select uiElement.UIElementName).FirstOrDefault();

                        int repeaterUIElementId = (from uiElement in this._unitOfWork.RepositoryAsync<UIElement>().Get()
                                                   join uiElementMap in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Get()
                                                   on uiElement.UIElementID equals uiElementMap.UIElementID
                                                   where uiElementMap.FormDesignVersionID == formDesignVersionId && uiElement.GeneratedName == repeaterName
                                                   select uiElement.UIElementID).FirstOrDefault();

                        List<string> repeaterDuplicationElementList = (from uiElement in this._unitOfWork.RepositoryAsync<UIElement>().Get()
                                                                       join uiElementMap in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Get()
                                                                       on uiElement.UIElementID equals uiElementMap.UIElementID
                                                                       where uiElementMap.FormDesignVersionID == formDesignVersionId && uiElement.ParentUIElementID == repeaterUIElementId && uiElement.Visible == true && uiElement.CheckDuplicate == true
                                                                       select uiElement.GeneratedName).ToList();

                        if (itemToUpdate == null)
                        {

                            result = AddFormInstanceRepeaterData(formInstanceId, fullPath, sectionId, repeaterUIElementId, repeaterData, userName);


                            //add in FormInstanceRepeaterDataMap table

                        }
                        else
                        {
                            //update if already present
                            result = UpdateFormInstanceRepeaterData(itemToUpdate, formInstanceId, fullPath, repeaterData, repeaterDuplicationElementList);
                        }

                    }
                    scope.Complete();
                }

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                //Get all the exception/inner exception messages and set the return code to failure
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
            }
            return result;
        }

        /// <summary>
        /// This method is used to check duplicate records 
        /// </summary>       
        /// <param name="formInstanceId"></param>
        /// <param name="fullname"></param>
        /// <param name="rowData"></param>
        /// <param name="repeaterData"></param>
        /// <returns></returns>
        public bool CheckDuplication(int formInstanceId, string fullname, string rowData, List<string> repaterRowId, List<string> duplicationObject)
        {
            bool isDuplicateRowFound = false;
            var converter = new ExpandoObjectConverter();
            if (repaterRowId == null)
                repaterRowId = new List<string>();
            dynamic rowObject = JsonConvert.DeserializeObject<ExpandoObject>(rowData, converter);
            try
            {
                FormInstanceRepeaterDataMap repeterFormInstance = this._unitOfWork.Repository<FormInstanceRepeaterDataMap>()
                                                                      .Query()
                                                                      .Filter(c => c.FormInstanceID == formInstanceId && c.FullName == fullname)
                                                                      .Get()
                                                                      .FirstOrDefault();
                if (repeterFormInstance != null)
                {
                    dynamic repeaterObject = JsonConvert.DeserializeObject<ExpandoObject>(repeterFormInstance.RepeaterData, converter);
                    var repeaterInstanceData = repeaterObject as IDictionary<string, object>;
                    var rData = rowObject as IDictionary<string, object>;
                    List<int> indexer = new List<int>();
                    foreach (string key in repeaterInstanceData.Keys)
                    {
                        var repeaterData = repeaterInstanceData[key] as System.Collections.Generic.IList<Object>;
                        for (int idx = 0; idx < repeaterData.Count(); idx++)
                        {
                            IDictionary<string, object> singleRowData = repeaterData[idx] as IDictionary<string, object>;
                            if (!repaterRowId.Contains(singleRowData["RowIDProperty"].ToString()))
                            {
                                int matchCount = 0;
                                IDictionary<string, object> singleRow = rData as IDictionary<string, object>;
                                foreach (var ele in duplicationObject)
                                {
                                    if (singleRowData.ContainsKey(ele) && singleRow.ContainsKey(ele))
                                    {
                                        if (singleRowData[ele].ToString() == singleRow[ele].ToString())
                                        {
                                            matchCount++;

                                        }
                                    }
                                }//if match the all element then use break to avoid next loop search
                                if (matchCount == duplicationObject.Count())
                                {
                                    //indexer.Add(idx);
                                    isDuplicateRowFound = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (indexer.Count() > 1)
                    {
                        isDuplicateRowFound = true;
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return isDuplicateRowFound;
        }

        /// <summary>
        /// This method on valiodation to check duplicate records where loadfromServer is true.
        /// </summary>       
        /// <param name="formInstanceId"></param>
        /// <param name="fullNameList"></param>
        /// <param name="formDesignVersionId"></param>       
        /// <returns></returns>
        public IDictionary<string, object> GetDuplicatedElementsList(string fullNameList, int formInstanceId, int formDesignVersionId)
        {
            var converter = new ExpandoObjectConverter();
            IDictionary<string, object> values = new Dictionary<string, object>();

            try
            {
                List<string> repeaterFullNameList = JsonConvert.DeserializeObject<List<string>>(fullNameList, converter);

                foreach (string fullName in repeaterFullNameList)
                {
                    IList<string> matchList = new List<string>();
                    IList<object> duplicateValues = new List<object>();
                    if (!string.IsNullOrEmpty(fullName))
                    {
                        FormInstanceRepeaterDataMap repeaterFormInstance = this._unitOfWork.Repository<FormInstanceRepeaterDataMap>()
                                                                       .Query()
                                                                       .Filter(c => c.FormInstanceID == formInstanceId && c.FullName == fullName)
                                                                       .Get()
                                                                       .FirstOrDefault();


                        if (repeaterFormInstance != null)
                        {
                            dynamic repeaterObject = JsonConvert.DeserializeObject<ExpandoObject>(repeaterFormInstance.RepeaterData, converter);
                            var repeaterInstanceData = repeaterObject as IDictionary<string, object>;
                            foreach (string key in repeaterInstanceData.Keys)
                            {
                                var repeaterData = repeaterInstanceData[key] as System.Collections.Generic.IList<Object>;
                                var repeaterCloneData = repeaterInstanceData[key] as System.Collections.Generic.IList<Object>;


                                IList<string> duplicationObjectList = (from formDesignVersionUIElementMap in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Get()
                                                                       join uIElement in this._unitOfWork.RepositoryAsync<UIElement>()
                                                                       .Get()
                                                                       on formDesignVersionUIElementMap.UIElementID equals uIElement.UIElementID
                                                                       where (uIElement.ParentUIElementID == repeaterFormInstance.RepeaterUIElementID
                                                                            && formDesignVersionUIElementMap.FormDesignVersionID == formDesignVersionId
                                                                            && uIElement.CheckDuplicate == true)
                                                                       select uIElement.GeneratedName).ToList();

                                for (int index = 0; index < repeaterData.Count; index++)
                                {
                                    IDictionary<string, object> singleRowData = repeaterData[index] as IDictionary<string, object>;

                                    IList<object> duplicateValuesClone = new List<object>();
                                    for (int idx = 0; idx < repeaterCloneData.Count; idx++)
                                    {
                                        if (duplicationObjectList.Count > 0)
                                        {
                                            int matchCount = 0;
                                            IDictionary<string, object> repeaterCloneSingleRowData = repeaterCloneData[idx] as IDictionary<string, object>;
                                            if (singleRowData["RowIDProperty"].ToString() != repeaterCloneSingleRowData["RowIDProperty"].ToString() && !matchList.Contains(repeaterCloneSingleRowData["RowIDProperty"].ToString()))
                                            {
                                                foreach (var element in duplicationObjectList)
                                                {
                                                    if (singleRowData.ContainsKey(element) && repeaterCloneSingleRowData.ContainsKey(element))
                                                    {
                                                        if (singleRowData[element].ToString().Equals(repeaterCloneSingleRowData[element].ToString()))
                                                        {
                                                            matchCount++;
                                                        }
                                                    }
                                                }
                                                if (duplicationObjectList.Count == matchCount)
                                                {
                                                    duplicateValuesClone.Add(repeaterCloneSingleRowData);
                                                    duplicateValues.Add(repeaterCloneSingleRowData);
                                                    matchList.Add(repeaterCloneSingleRowData["RowIDProperty"].ToString());
                                                }
                                            }
                                        }
                                    }
                                    if (duplicateValuesClone.Count() > 0)
                                    {
                                        duplicateValues.Add(singleRowData);
                                        matchList.Add(singleRowData["RowIDProperty"].ToString());
                                    }
                                }
                            }
                        }
                    }

                    if (duplicateValues.Count() > 0)
                    {
                        values.Add(fullName, duplicateValues);
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;

            }
            return values;
        }

        /// <summary>
        /// This method is used to save existing data of repeater in FormInstanceRepeaterDataMap table for which LoadFromServer is true and delete 
        /// the same data from FormInstanceDataMap table on Finalize
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formDesignVersionId"></param>
        /// <param name="enteredBy"></param>
        /// <returns></returns>
        public ServiceResult SaveRepeaterFormInstanceDataOnFinalize(int tenantId, int formDesignVersionId, string enteredBy)
        {
            ServiceResult result = null;
            try
            {
                result = new ServiceResult();
                FormDesignVersion formDesignversion = this._unitOfWork.RepositoryAsync<FormDesignVersion>()
                                                               .FindById(formDesignVersionId);

                if (formDesignversion != null)
                {
                    // Get the formInstances(Ids) which use the Latest Finalized design version at the time of rendering and which are not in Released Folder.
                    var formInstanceIdList = GetFormInstanceIdList(tenantId, formDesignversion.FormDesignID.Value);
                    if (formInstanceIdList.Count() == 0)
                        result.Result = ServiceResultStatus.Success;
                    else
                    {
                        foreach (var formInstanceId in formInstanceIdList)
                        {
                            string formInstanceData = _folderVersionServices.GetFormInstanceData(tenantId, formInstanceId);
                            IList<int> getUpdatedRepeaterElementList = new List<int>();

                            IList<int> uielementIdList = GetLoadFromServerEnabledRepeaterIdList(tenantId, formDesignVersionId, formInstanceId, ref getUpdatedRepeaterElementList);
                            if (uielementIdList.Count > 0)
                            {
                                foreach (var uielementId in uielementIdList)
                                {
                                    FormInstanceRepeaterDataBuilder formInstanceRepeaterBuilder = new FormInstanceRepeaterDataBuilder(this._unitOfWork);
                                    string fullName = formInstanceRepeaterBuilder.GetRepeaterFullName(uielementId);
                                    IList<string> repeaterJsonDataList = new List<string>();

                                    IDictionary<string, string> JsonDataList = formInstanceRepeaterBuilder.GetSplicedRepeaterAndUpdatedFormInstanceData(formInstanceData, fullName);

                                    FormInstanceRepeaterDataMap formInstanceRepeaterDataMap = GetFormInstanceRepeaterData(tenantId, formInstanceId, uielementId);
                                    string[] elements = fullName.Split('.');
                                    if (formInstanceRepeaterDataMap != null)
                                    {
                                        if (JsonDataList != null && JsonDataList.Count > 0)
                                        {
                                            if (JsonDataList.ContainsKey(elements.Last()))
                                            {
                                                formInstanceRepeaterDataMap.RepeaterData = JsonDataList[elements.Last()];
                                                formInstanceRepeaterDataMap.UpdatedDate = DateTime.Now;
                                                this._unitOfWork.RepositoryAsync<FormInstanceRepeaterDataMap>().Update(formInstanceRepeaterDataMap);
                                                this._unitOfWork.Save();

                                                result.Result = ServiceResultStatus.Success;
                                            }

                                        }
                                        result.Result = ServiceResultStatus.Success;
                                    }

                                    else
                                    {
                                        if (JsonDataList != null && JsonDataList.Count > 0)
                                        {
                                            using (var scope = new TransactionScope())
                                            {
                                                result = this.AddFormInstanceRepeaterDataMapOnFinalize(formInstanceId, JsonDataList[elements.Last()], tenantId, enteredBy, uielementId, fullName, formDesignVersionId);
                                                if (JsonDataList[elements.First()] != null)
                                                {
                                                    if (JsonDataList.ContainsKey(elements.First()))
                                                    {
                                                        // Update FormInstanceDataMap with updated repeater data(empty array). 
                                                        result = _folderVersionServices.UpdateFormInstanceData(tenantId, formInstanceId, JsonDataList[elements.First()]);
                                                    }
                                                }
                                                this._unitOfWork.Save();
                                                result.Result = ServiceResultStatus.Success;
                                                scope.Complete();
                                            }
                                        }
                                        result.Result = ServiceResultStatus.Success;
                                    }

                                }
                            }
                            result.Result = ServiceResultStatus.Success;
                            if (getUpdatedRepeaterElementList.Count() > 0)
                            {
                                result.Result = UpdateRepeaterInstanceData(tenantId, getUpdatedRepeaterElementList, formInstanceId, formDesignVersionId);
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
                if (reThrow)
                    throw;

            }
            return result;

        }

        /// <summary>
        /// This method is used to add repeater data in FormInsatnceRepeaterDatamap table.
        /// </summary>
        /// <param name="formInstanceID"></param>
        /// <param name="repeaterData"></param>
        /// <param name="tenantId"></param>
        /// <param name="enteredBy"></param>
        /// <param name="repeaterUIElementID"></param>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public ServiceResult AddFormInstanceRepeaterDataMapOnFinalize(int formInstanceId, string repeaterData, int tenantId, string enteredBy, int repeaterUIElementID, string fullName, int formDesignVersionId)
        {
            ServiceResult result = null;
            try
            {
                result = new ServiceResult();
                FormInstanceDataMap formInstance = this._unitOfWork.RepositoryAsync<FormInstanceDataMap>()
                                                                    .Query()
                                                                    .Filter(c => c.FormInstanceID == formInstanceId)
                                                                    .Get()
                                                                    .FirstOrDefault();
                string[] elements = fullName.Split('.');
                string sectionName = elements.First();
                string repeaterName = elements.Last();


                string sectionId = (from uiElement in this._unitOfWork.RepositoryAsync<UIElement>().Get()
                                    join uiElementMap in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>().Get()
                                    on uiElement.UIElementID equals uiElementMap.UIElementID
                                    where uiElementMap.FormDesignVersionID == formDesignVersionId && uiElement.GeneratedName == sectionName
                                    select uiElement.UIElementName).FirstOrDefault();

                FormInstanceRepeaterDataMap formInstanceRepeaterDataMap = new FormInstanceRepeaterDataMap();
                formInstanceRepeaterDataMap.FormInstanceID = formInstanceId;
                formInstanceRepeaterDataMap.FormInstanceDataMapID = formInstance.FormInstanceDataMapID;
                formInstanceRepeaterDataMap.RepeaterUIElementID = repeaterUIElementID;
                formInstanceRepeaterDataMap.FullName = fullName;
                formInstanceRepeaterDataMap.RepeaterData = repeaterData;
                formInstanceRepeaterDataMap.SectionID = sectionId;
                formInstanceRepeaterDataMap.IsActive = true;
                formInstanceRepeaterDataMap.AddedDate = DateTime.Now;

                this._unitOfWork.RepositoryAsync<FormInstanceRepeaterDataMap>().Insert(formInstanceRepeaterDataMap);
                this._unitOfWork.Save();

                result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;

            }
            return result;
        }


        public string GetMasterListFormInstanceData(int formInstanceId)
        {
            IDictionary<string, string> formInstanceData = null;
            string masterListData = string.Empty;

            try
            {
                formInstanceData = (from formInstanceDataMap in this._unitOfWork.RepositoryAsync<FormInstanceDataMap>().Get()
                                    join formInstance in this._unitOfWork.RepositoryAsync<FormInstance>().Get()
                                        on formInstanceDataMap.FormInstanceID equals formInstance.FormInstanceID
                                    where formInstance.FormInstanceID == formInstanceId
                                    select new
                                    {
                                        Name = formInstance.Name,
                                        json = formInstanceDataMap.FormData
                                    }).ToDictionary(x => x.Name, x => x.json);


                FormInstanceRepeaterDataBuilder formInstanceRepeaterBuilder = new FormInstanceRepeaterDataBuilder(this._unitOfWork);
                if (formInstanceData != null)
                {
                    masterListData = formInstanceRepeaterBuilder.GetUpdatedMasterListJsonDataWithRepeaterData(formInstanceId, formInstanceData);
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }

            return masterListData;
        }

        private ServiceResultStatus UpdateRepeaterInstanceData(int tenantId, IList<int> getUpdatedRepeaterElementList, int formInstanceId, int formDesignVersionId)
        {
            ServiceResult result = new ServiceResult();

            try
            {
                foreach (int uielementId in getUpdatedRepeaterElementList)
                {
                    IList<string> repeaterDesignChildElementList = (from formDesignVersionUIElementMap in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>()
                                                        .Get()
                                                                    join uIElement in this._unitOfWork.RepositoryAsync<UIElement>()
                                                                    .Get()
                                                                    on formDesignVersionUIElementMap.UIElementID equals uIElement.UIElementID
                                                                    where (uIElement.ParentUIElementID == uielementId
                                                                           && formDesignVersionUIElementMap.FormDesignVersionID == formDesignVersionId)
                                                                    select uIElement.GeneratedName).ToList();

                    FormInstanceRepeaterDataMap formInstanceRepeaterDataMap = GetFormInstanceRepeaterData(tenantId, formInstanceId, uielementId);

                    if (formInstanceRepeaterDataMap != null)
                    {
                        var element = formInstanceRepeaterDataMap.FullName.Split('.');

                        var converter = new ExpandoObjectConverter();
                        dynamic forminstanceRepeaterObject = JsonConvert.DeserializeObject<ExpandoObject>(formInstanceRepeaterDataMap.RepeaterData, converter);
                        var formInstanceRepeaterData = forminstanceRepeaterObject as IDictionary<string, object>;
                        IList<object> repeaterDeltaObjectList = formInstanceRepeaterData[element.Last()] as IList<object>;
                        IDictionary<string, object> firstRowElement = repeaterDeltaObjectList[0] as IDictionary<string, object>;
                        IList<string> renderedColumnList = firstRowElement.Keys.ToList();
                        renderedColumnList.Remove("RowIDProperty");

                        List<string> intersectElemetList = repeaterDesignChildElementList.Intersect(renderedColumnList).ToList();

                        List<string> newElement = repeaterDesignChildElementList.Except(intersectElemetList).ToList();
                        List<string> deletedElement = renderedColumnList.Except(intersectElemetList).ToList();



                        if (newElement.Count() > 0 || deletedElement.Count() > 0)
                        {
                            foreach (var key in deletedElement)
                            {
                                for (int i = 0; i < repeaterDeltaObjectList.Count(); i++)
                                {
                                    IDictionary<string, object> repeaterRowData = repeaterDeltaObjectList[i] as IDictionary<string, object>;
                                    repeaterRowData.Remove(key);
                                }
                            }
                            foreach (var newkey in newElement)
                            {
                                for (int i = 0; i < repeaterDeltaObjectList.Count(); i++)
                                {
                                    IDictionary<string, object> repeaterRowData = repeaterDeltaObjectList[i] as IDictionary<string, object>;
                                    repeaterRowData.Add(newkey, String.Empty);
                                }
                            }
                        }

                        string jsonString = JsonConvert.SerializeObject(formInstanceRepeaterData);

                        formInstanceRepeaterDataMap.RepeaterData = jsonString;
                        formInstanceRepeaterDataMap.UpdatedDate = DateTime.Now;
                        this._unitOfWork.RepositoryAsync<FormInstanceRepeaterDataMap>().Update(formInstanceRepeaterDataMap);
                        this._unitOfWork.Save();

                        result.Result = ServiceResultStatus.Success;
                    }
                    result.Result = ServiceResultStatus.Success;
                }
            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return result.Result;
        }




        #endregion

        #region Private Methods

        /// <summary>
        /// This method is used to get Repeater UIElement Ids for which LoadFromServer is true.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formDesignVersionId"></param>
        /// <returns></returns>
        private IList<int> GetLoadFromServerEnabledRepeaterIdList(int tenantId, int formDesignVersionId, int formInstanceId, ref IList<int> updatedRepeaterElementList)
        {
            IList<int> filteredRepeaterUIElementIds = new List<int>();
            try
            {
                IList<int> repeaterUIElementIds = (from formDesignVersionUIElementMap in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>()
                                                         .Get()
                                                   join repeaterUIElement in this._unitOfWork.RepositoryAsync<RepeaterUIElement>()
                                                   .Get()
                                                   on formDesignVersionUIElementMap.UIElementID equals repeaterUIElement.UIElementID
                                                   where (repeaterUIElement.LoadFromServer == true && formDesignVersionUIElementMap.FormDesignVersionID == formDesignVersionId)
                                                   select repeaterUIElement.UIElementID).ToList();


                //Skip the uielements which are already present in FormInstanceRepeaterDataMap table
                if (repeaterUIElementIds.Count > 0)
                {
                    foreach (var elementsId in repeaterUIElementIds)
                    {
                        FormInstanceRepeaterDataMap formInstanceRepeaterDataMap = this._unitOfWork.RepositoryAsync<FormInstanceRepeaterDataMap>()
                                                                            .Query()
                                                                            .Filter(c => c.RepeaterUIElementID == elementsId && c.FormInstanceID == formInstanceId)
                                                                            .Get().FirstOrDefault();

                        IList<string> repeaterChildElementList = (from formDesignVersionUIElementMap in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>()
                                                         .Get()
                                                                  join uIElement in this._unitOfWork.RepositoryAsync<UIElement>()
                                                                  .Get()
                                                                  on formDesignVersionUIElementMap.UIElementID equals uIElement.UIElementID
                                                                  where (uIElement.ParentUIElementID == elementsId
                                                                         && formDesignVersionUIElementMap.FormDesignVersionID == formDesignVersionId)
                                                                  select uIElement.GeneratedName).ToList();

                        if (formInstanceRepeaterDataMap == null)
                        {
                            filteredRepeaterUIElementIds.Add(elementsId);
                            updatedRepeaterElementList.Add(elementsId);
                        }

                        bool isValidRepeaterChildElement = CheckIsUpdateRepaterChildElement(repeaterChildElementList, formInstanceRepeaterDataMap);

                        if (isValidRepeaterChildElement)
                        {
                            updatedRepeaterElementList.Add(elementsId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return filteredRepeaterUIElementIds;
        }



        private bool CheckIsUpdateRepaterChildElement(IList<string> repeaterDesignChildElementList, FormInstanceRepeaterDataMap formInstanceRepeaterDataMap)
        {
            bool isValid = false;

            if (formInstanceRepeaterDataMap != null)
            {
                var element = formInstanceRepeaterDataMap.FullName.Split('.');

                var converter = new ExpandoObjectConverter();
                dynamic forminstanceRepeaterObject = JsonConvert.DeserializeObject<ExpandoObject>(formInstanceRepeaterDataMap.RepeaterData, converter);
                var formInstanceRepeaterData = forminstanceRepeaterObject as IDictionary<string, object>;

                IList<object> repeaterDeltaObjectList = formInstanceRepeaterData[element.Last()] as IList<object>;
                if (repeaterDeltaObjectList.Count() > 0)
                {
                    IDictionary<string, object> elementarry = repeaterDeltaObjectList[0] as IDictionary<string, object>;
                    IList<string> renderedRepeaterColumnList = elementarry.Keys.ToList();
                    renderedRepeaterColumnList.Remove("RowIDProperty");

                    List<string> intersectElements = repeaterDesignChildElementList.Intersect(renderedRepeaterColumnList).ToList();

                    List<string> newElement = repeaterDesignChildElementList.Except(intersectElements).ToList();
                    List<string> deletedElement = renderedRepeaterColumnList.Except(intersectElements).ToList();

                    if (newElement.Count() > 0 || deletedElement.Count() > 0)
                    {
                        isValid = true;
                    }
                }

            }
            return isValid;
        }



        /// <summary>
        /// This method is used to get formInstances(Ids) which use the Latest Finalized design version at the time of rendering and which are not in Released Folder.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="formDesignId"></param>
        /// <returns></returns>
        private IList<int> GetFormInstanceIdList(int tenantId, int formDesignId)
        {
            IList<int> formInstanceIdList = null;


            try
            {
                List<FormDesignVersion> versionList = this._unitOfWork.RepositoryAsync<FormDesignVersion>()
                                                                           .Query()
                                                                           .Filter(c => c.FormDesignID == formDesignId)
                                                                           .Get().ToList();


                formInstanceIdList = (from formDesign in this._unitOfWork.RepositoryAsync<FormDesign>()
                                                                      .Get()
                                      join formDesignVersion in this._unitOfWork.RepositoryAsync<FormDesignVersion>()
                                      .Get()
                                      .OrderByDescending(p => p.EffectiveDate)
                                      on formDesign.FormID equals formDesignVersion.FormDesignID
                                      join forminstance in this._unitOfWork.RepositoryAsync<FormInstance>()
                                      .Get()
                                      on formDesignVersion.FormDesignVersionID equals forminstance.FormDesignVersionID
                                      join folderVersion in this._unitOfWork.RepositoryAsync<FolderVersion>()
                                      .Get().Where(c => c.FolderVersionStateID == (int)FolderVersionState.INPROGRESS)
                                      on forminstance.FolderVersionID equals folderVersion.FolderVersionID
                                      join forminstancedatamap in this._unitOfWork.RepositoryAsync<FormInstanceDataMap>()
                                      .Get()
                                      on forminstance.FormInstanceID equals forminstancedatamap.FormInstanceID
                                      where (formDesign.TenantID == tenantId && forminstance.IsActive == true && formDesign.FormID == formDesignId && formDesignVersion.StatusID == ((versionList.Count == 1) ? (int)domain.entities.Status.InProgress : (int)domain.entities.Status.Finalized))   // If form design has only one version and not finalized, then take data from In-Progress version.
                                      select forminstancedatamap.FormInstanceID).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return formInstanceIdList;
        }

        private FormInstanceRepeaterDataMap GetFormInstanceRepeaterData(int tenantId, int formInstanceId, int repeaterUIElementId)
        {
            FormInstanceRepeaterDataMap formInstanceRepeaterDataMap = null;

            try
            {
                formInstanceRepeaterDataMap = this._unitOfWork.RepositoryAsync<FormInstanceRepeaterDataMap>()
                                                                       .Query()
                                                                       .Filter(c => c.FormInstanceID == formInstanceId && c.RepeaterUIElementID == repeaterUIElementId)
                                                                       .Get()
                                                                       .FirstOrDefault();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return formInstanceRepeaterDataMap;
        }

        private ServiceResult UpdateFormInstanceRepeaterData(FormInstanceRepeaterDataMap itemToUpdate, int formInstanceId, string fullPath, string updatedFormInstanceRepeaterData, List<string> repeaterDuplicationElement)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                //get repater Data & convert into json string
                FormInstanceRepeaterParcer formInstanceRepeaterParcer = new FormInstanceRepeaterParcer(this._unitOfWork);
                string updatedformInstanceRepeaterJsonString = formInstanceRepeaterParcer.MergeFormInstanceRepeaterData(itemToUpdate, fullPath, updatedFormInstanceRepeaterData, repeaterDuplicationElement);

                itemToUpdate.RepeaterData = updatedformInstanceRepeaterJsonString;
                itemToUpdate.UpdatedDate = DateTime.Now;
                this._unitOfWork.RepositoryAsync<FormInstanceRepeaterDataMap>().Update(itemToUpdate);
                this._unitOfWork.Save();
                result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                result.Result = ServiceResultStatus.Failure;
                if (reThrow) throw ex;
                //Get all the exception/inner exception messages and set the return code to failure
                result = ex.ExceptionMessages();
            }
            return result;
        }


        private ServiceResult AddFormInstanceRepeaterData(int formInstanceId, string fullPath, string sectionId, int repeaterUIElement, string repeaterData, string userName)
        {
            ServiceResult result = new ServiceResult();
            try
            {


                string formInstanceDataMapId = this._unitOfWork.Repository<FormInstanceDataMap>()
                                                                      .Query()
                                                                      .Filter(c => c.FormInstanceID == formInstanceId)
                                                                      .Get()
                                                                      .Select(p => p.FormInstanceDataMapID).FirstOrDefault().ToString();
                //get repater Data & convert into json string
                FormInstanceRepeaterParcer formInstanceRepeaterParcer = new FormInstanceRepeaterParcer(this._unitOfWork);
                string formInstanceRepeaterJsonString = formInstanceRepeaterParcer.GetFormInstanceRepeaterJsonString(fullPath, repeaterData);

                FormInstanceRepeaterDataMap itemToAdd = new FormInstanceRepeaterDataMap();
                itemToAdd.FormInstanceDataMapID = Convert.ToInt32(formInstanceDataMapId);
                itemToAdd.FormInstanceID = formInstanceId;
                itemToAdd.SectionID = sectionId;
                itemToAdd.RepeaterUIElementID = Convert.ToInt32(repeaterUIElement);
                itemToAdd.FullName = fullPath;
                itemToAdd.RepeaterData = formInstanceRepeaterJsonString;
                itemToAdd.AddedBy = userName;
                itemToAdd.AddedDate = DateTime.Now;
                itemToAdd.IsActive = true;
                //Call to repository method to insert record.
                this._unitOfWork.RepositoryAsync<FormInstanceRepeaterDataMap>().Insert(itemToAdd);
                this._unitOfWork.Save();
                result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                result.Result = ServiceResultStatus.Failure;
                if (reThrow) throw ex;
                //Get all the exception/inner exception messages and set the return code to failure
                result = ex.ExceptionMessages();
            }
            return result;
        }

        #endregion Private Methods










    }
}
