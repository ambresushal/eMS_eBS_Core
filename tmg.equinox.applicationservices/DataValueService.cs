using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Transactions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.FormContent;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.domain.entities.Utility;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.repository.interfaces;
using Attribute = tmg.equinox.applicationservices.viewmodels.Attribute;

namespace tmg.equinox.applicationservices
{
    public class DataValueService : IDataValueService
    {
        #region Variables

        private IUnitOfWorkAsync _unitOfWork;
        public List<DataValueViewModel> DataValueViewModelList { get; set; }
        public List<UIElement> UIElements { get; set; }
        public long RootObjInstanceID { get; set; }
        public List<DataValueExpando> DataValueExpandoList { get; set; }

        #endregion

        #region Constructor

        public DataValueService(IUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
            UIElements = new List<UIElement>();
            DataValueViewModelList = new List<DataValueViewModel>();
            DataValueExpandoList = new List<DataValueExpando>();
        }

        #endregion

        #region Public Methods

        public ServiceResult SaveDataValuesFromFolderVersion(int tenantId, int folderVersionId, int formInstanceId, string formInstanceData)
        {
            ServiceResult result = null;

            var formDesignVersionId = _unitOfWork.RepositoryAsync<FormInstance>()
                                                .Query()
                                                .Filter(fil => fil.FolderVersionID == folderVersionId && fil.FormInstanceID == formInstanceId)
                                                .Get()
                                                .Select(sel => sel.FormDesignVersionID)
                                                .SingleOrDefault();
            var converter = new ExpandoObjectConverter();

            dynamic jsonObject = JsonConvert.DeserializeObject<ExpandoObject>(formInstanceData, converter);

            result = GetDeserializedJsonObject(jsonObject, formDesignVersionId, tenantId);

            return result;
        }

        public ServiceResult GetDeserializedJsonObject(dynamic jsonObject, int formDesignVersionId, int tenantId)
        {
            var result = new ServiceResult();

            try
            {
                //Flattens the jsonObject with dictionary Key = 'ElementName' and Value = 'Value'
                List<DataValueExpando> flatten = Flatten(jsonObject, null);

                //Fetched  ObjectVersion 
                var objectVersionFormVersion = this._unitOfWork.RepositoryAsync<FormVersionObjectVersionMap>()
                                                   .Query()
                                                   .Filter(
                                                       filter => filter.FormDesignVersionID == formDesignVersionId)
                                                   .Get()
                                                   .SingleOrDefault();

                var objectVersionAttribXrefs = _unitOfWork.RepositoryAsync<ObjectVersionAttribXref>()
                                                          .Query()
                                                          .Include(include => include.ObjectDefinition)
                                                          .Get()
                                                          .ToList();

                var attributes = _unitOfWork.RepositoryAsync<domain.entities.Models.Attribute>()
                                            .Query()
                                            .Get()
                                            .ToList();

                //Applying Join between Attribute and ObjAttribXref using know ObjectVersion and Flatten Dictionary
                var attributeXrefList = (from objAttribXref in objectVersionAttribXrefs
                                         join attribute in attributes
                                             on objAttribXref.AttrID equals attribute.AttrID
                                         select new ObjectAttributeXrefViewModel
                                            {
                                                AttrID = objAttribXref.AttrID,
                                                ObjVerID = objAttribXref.ObjVerID,
                                                VersionID = objAttribXref.VersionID,
                                                OID = objAttribXref.OID,
                                                Name = attribute.Name,
                                                AttrType = attribute.AttrType,
                                                Cardinality = attribute.Cardinality,
                                                OName = objAttribXref.ObjectDefinition.ObjectName,
                                                DefaultValue = attribute.DefaultValue
                                            }).Where(e => flatten.Any(name => objectVersionFormVersion != null &&
                                                                               (name.Key == e.Name &&
                                                                                e.VersionID ==
                                                                                objectVersionFormVersion
                                                                                    .ObjectVersionID))).ToList();


                //Fetch ObjectDefinitions that is present in attributeXrefList
                var distinctOIDs = attributeXrefList.Select(sel => new ObjectAttributeXrefViewModel()
                                                                    {
                                                                        OID = sel.OID,
                                                                        OName = sel.OName,
                                                                        VersionID = sel.VersionID
                                                                    }).Distinct();


                //Fetch ObjectDefinitions that is present in ObjectTree
                var ObjectDefinitionIDList = _unitOfWork.RepositoryAsync<ObjectTree>()
                                                        .Query()
                                                        .Include(include => include.ObjectDefinition)
                                                        .Filter(filter => filter.ParentOID != null &&
                                                                filter.VersionID == objectVersionFormVersion.ObjectVersionID)
                                                        .Get()
                                                        .Select(sel => new ObjectAttributeXrefViewModel
                                                                        {
                                                                            OID = sel.ParentOID.Value,
                                                                            OName = sel.ObjectDefinition.ObjectName,
                                                                            VersionID = sel.VersionID
                                                                        })
                                                        .Distinct().ToList();

                //Combine two sets of ObjectDefinitions
                var objectDefintionList = ObjectDefinitionIDList.ToList().Union(distinctOIDs.AsEnumerable());

                //Fetch UIElements from FormDesignVersionId
                var UIElements = _unitOfWork.RepositoryAsync<UIElement>()
                                            .Query()
                                            .Include(include =>
                                                     include.FormDesignVersionUIElementMaps)
                                            .Filter(filter =>
                                                    filter.FormDesignVersionUIElementMaps
                                                    .Any(formVersion =>
                                                          formVersion.FormDesignVersionID == formDesignVersionId))
                                            .Get()
                                            .OrderBy(order => order.UIElementID)
                                            .ToList();

                //Prepare DataValues that need to be saved
                var temp = 1;
                PrepareDataValuesToSave(UIElements, objectDefintionList, temp, attributeXrefList, flatten);

                //Saving into multiple tables of 'Data' schema.
                SaveDataValues();
                var items = new List<ServiceResultItem>();
                items.Add(new ServiceResultItem() { Messages = new[] { RootObjInstanceID.ToString() } });
                result = new ServiceResult() { Result = ServiceResultStatus.Success, Items = items };

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                result = ex.ExceptionMessages();
            }
            return result;
        }

        #endregion

        #region Private Methods

        private List<DataValueExpando> Flatten(ExpandoObject expando, string parent)
        {
            var d = expando as IDictionary<string, object>;
            foreach (KeyValuePair<string, object> kvp in d)
            {
                var dataVaueExpando = new DataValueExpando();
                if (kvp.Value is ExpandoObject)
                {
                    //dataVaueExpando.Value = kvp.Value;
                    dataVaueExpando.Value = string.Empty;
                    var expandoValue = (ExpandoObject)kvp.Value;
                    Flatten(expandoValue, kvp.Key);
                    dataVaueExpando.Key = kvp.Key;
                    DataValueExpandoList.Add(dataVaueExpando);
                }
                else if (kvp.Value is List<Object>)
                {
                    var valueList = (List<Object>)kvp.Value;
                    dataVaueExpando.Value = string.Empty;
                    foreach (Object item in valueList)
                    {
                        if (item is ExpandoObject)
                        {
                            Flatten(item as ExpandoObject, kvp.Key);
                        }
                    }
                    dataVaueExpando.Key = kvp.Key;
                    DataValueExpandoList.Add(dataVaueExpando);
                }
                else
                {
                    dataVaueExpando.Key = kvp.Key;
                    dataVaueExpando.Parent = parent;
                    if (kvp.Value.Equals("true") || kvp.Value.Equals("false"))
                    {
                        dataVaueExpando.Value = kvp.Value.Equals("true") ? "1" : "0";
                    }
                    else
                    {
                        dataVaueExpando.Value = kvp.Value;
                    }
                    DataValueExpandoList.Add(dataVaueExpando);
                }
            }
            return DataValueExpandoList;
        }

        private void PopulateDataValueEelments(List<ObjectAttributeXrefViewModel> attributeXrefList, UIElement uiElement,
                                                 DataValueViewModel dataValueViewModel, List<UIElement> UIElements)
        {
            var parentUiElement = UIElements.Where(c => c.UIElementID == uiElement.ParentUIElementID).FirstOrDefault();
            var tempAttribute = attributeXrefList.FirstOrDefault(e => e.Name.Equals(uiElement.GeneratedName) && e.OName == parentUiElement.GeneratedName);

            if (tempAttribute != null)
                dataValueViewModel.Attribute = new Attribute()
                {
                    AttrID = tempAttribute.AttrID,
                    AttrType = tempAttribute.AttrType,
                    Name = tempAttribute.Name,
                    Cardinality = tempAttribute.Cardinality,
                    ObjVerID = tempAttribute.ObjVerID
                };

            var parentElementID = DataValueViewModelList.Where(e =>
                                                               UIElements.Any(ui =>
                                                                              ui.UIElementID == uiElement.ParentUIElementID &&
                                                                              ui.GeneratedName == e.ElementName))
                                                        .Select(sel => sel.ElementID)
                                                        .FirstOrDefault();

            dataValueViewModel.ParentElementID = parentElementID;

            DataValueViewModelList.Add(dataValueViewModel);
        }

        private void PrepareDataValuesToSave(List<UIElement> UIElements, IEnumerable<ObjectAttributeXrefViewModel> objectDefintionList, int temp, List<ObjectAttributeXrefViewModel> attributeXrefList,
                                            List<DataValueExpando> flatten)
        {
            try
            {

                foreach (var uiElement in UIElements)
                {
                    if (uiElement.IsContainer)
                    {
                        if (objectDefintionList.ToList().Any(obj => obj.OName.Equals(uiElement.GeneratedName)))
                        {
                            if (DataValueViewModelList == null)
                                DataValueViewModelList = new List<DataValueViewModel>();
                            var dataValueViewModel = new DataValueViewModel();

                            dataValueViewModel.IsContainer = true;
                            dataValueViewModel.ElementID = temp++;
                            dataValueViewModel.ElementName = uiElement.GeneratedName;
                            if (uiElement.ParentUIElementID == null)
                            {
                                dataValueViewModel.ParentElementID = null;
                                dataValueViewModel.IsRoot = true;
                            }
                            else
                            {
                                var parentElementID = DataValueViewModelList.Where(e =>
                                                                                   UIElements.Any(ui =>
                                                                                                  ui.UIElementID ==
                                                                                                  uiElement.ParentUIElementID &&
                                                                                                  ui.GeneratedName == e.ElementName))
                                                                            .Select(sel => sel.ElementID)
                                                                            .FirstOrDefault();


                                dataValueViewModel.ParentElementID = parentElementID;
                            }

                            DataValueViewModelList.Add(dataValueViewModel);
                        }
                    }
                    else
                    {
                        if (attributeXrefList.Any(e => e.Name.Equals(uiElement.GeneratedName)))
                        {
                            var parentUiElement = UIElements.Where(c => c.UIElementID == uiElement.ParentUIElementID).FirstOrDefault();
                            if (DataValueViewModelList == null)
                                DataValueViewModelList = new List<DataValueViewModel>();

                            if (UIElements.Any(ui => ui.UIElementID == uiElement.ParentUIElementID &&
                                                     ui is RepeaterUIElement))
                            {
                                int rowID = 0;
                                foreach (var element in flatten.Where(e => e.Key.Equals(uiElement.GeneratedName) && (e.Parent == parentUiElement.GeneratedName)).ToList())
                                {
                                    var dataValueViewModel = new DataValueViewModel();
                                    dataValueViewModel.RowIDInfo = ++rowID;
                                    dataValueViewModel.ElementID = temp++;
                                    dataValueViewModel.ElementName = uiElement.GeneratedName;
                                    dataValueViewModel.Value = element.Value.ToString();

                                    dataValueViewModel.IsRepeater = true;

                                    PopulateDataValueEelments(attributeXrefList, uiElement, dataValueViewModel, UIElements);
                                }
                            }
                            else
                            {
                                var dataValueViewModel = new DataValueViewModel();

                                dataValueViewModel.ElementID = temp++;
                                dataValueViewModel.ElementName = uiElement.GeneratedName;
                                var valueList = flatten.Where(e => e.Key.Equals(uiElement.GeneratedName))
                                                                  .Select(sel => sel.Value.ToString()).ToList();
                                if (valueList.Count() > 0)
                                {

                                }
                                dataValueViewModel.Value = valueList.FirstOrDefault();

                                PopulateDataValueEelments(attributeXrefList, uiElement, dataValueViewModel, UIElements);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SaveDataValues()
        {
            #region INSERT WITH INSERT ALL SAVE CHANGES ASYNC
            try
            {

                var newInstanceId = (from obj in this._unitOfWork.Repository<ObjectInstance>().Get()
                                     select (long?)obj.ObjectInstanceID).Max() ?? 0;

                newInstanceId = newInstanceId + 1;

                var container = DataValueViewModelList.Where(dv => dv.IsContainer).ToList();


                var rootConatiner = container.Where(x => x.IsRoot).Select(x => new ObjectInstance { ObjectInstanceID = newInstanceId }).FirstOrDefault();
                this._unitOfWork.Repository<ObjectInstance>().Insert(rootConatiner);
                this._unitOfWork.Save();

                RootObjInstanceID = rootConatiner.ObjectInstanceID;

                List<ObjectInstance> section = (from dataValue1 in DataValueViewModelList.Where(dataValue1 => dataValue1.IsContainer)
                                                select new ObjectInstance
                                                {
                                                    ObjectInstanceID = ++newInstanceId

                                                }).ToList();

                this._unitOfWork.Repository<ObjectInstance>().InsertRange(section);

                List<StringDataValue> containerDataValues = (from dataValue1 in container
                                                             select new StringDataValue
                                                             {
                                                                 ObjInstanceID = dataValue1.ElementID,
                                                                 ObjVerID = dataValue1.IsContainer ? (int?)null : dataValue1.Attribute.ObjVerID,
                                                                 Value = dataValue1.IsContainer ? "" : dataValue1.Value,
                                                                 ParentObjInstanceID = dataValue1.ParentElementID,
                                                                 RowIDInfo = dataValue1.IsRepeater ? dataValue1.RowIDInfo : (int?)null,
                                                                 RootObjInstanceID = dataValue1.ParentElementID != null ? RootObjInstanceID : dataValue1.ElementID
                                                             }).ToList();

                this._unitOfWork.Repository<StringDataValue>().InsertRange(containerDataValues);

                var validNumericValues = DataValueViewModelList.Where(dataValue1 =>
                                                                dataValue1.Attribute != null && (dataValue1.Attribute.AttrType == "Integer" ||
                                                                dataValue1.Attribute.AttrType == "Boolean" ||
                                                                dataValue1.Attribute.AttrType == "Double") && !string.IsNullOrEmpty(dataValue1.Value));

                List<NumericDataValue> numericDataValues = (from dataValue1 in validNumericValues
                                                            select new NumericDataValue
                                                            {

                                                                ObjVerID = dataValue1.Attribute.ObjVerID,
                                                                Value = Convert.ToDecimal(dataValue1.Value == "" ? 
                                                                        "0" : 
                                                                        ((dataValue1.Value == "false" || dataValue1.Value.ToLower() == "no") ?
                                                                            "0" : ((dataValue1.Value.ToLower() == "true" || 
                                                                            dataValue1.Value.ToLower() == "yes") ? 
                                                                                "1" : dataValue1.Value))),
                                                                ObjInstanceID = dataValue1.ElementID,
                                                                ParentObjInstanceID = dataValue1.ParentElementID.Value,
                                                                RootObjInstanceID = RootObjInstanceID,
                                                                RowIDInfo = dataValue1.IsRepeater ? dataValue1.RowIDInfo : (int?)null
                                                            }).ToList();


                this._unitOfWork.Repository<NumericDataValue>().InsertRange(numericDataValues);

                List<StringDataValue> stringDataValues = (from dataValue1 in
                                                              (DataValueViewModelList.Where(dataValue1 =>
                                                                  dataValue1.Attribute != null &&
                                                              dataValue1.Attribute.AttrType == "String"))
                                                          select new StringDataValue
                                                          {
                                                              ObjInstanceID = dataValue1.ElementID,
                                                              ObjVerID = dataValue1.IsContainer ? (int?)null : dataValue1.Attribute.ObjVerID,
                                                              Value = dataValue1.IsContainer ? "" : dataValue1.Value,
                                                              ParentObjInstanceID = dataValue1.ParentElementID,
                                                              RowIDInfo = dataValue1.IsRepeater ? dataValue1.RowIDInfo : (int?)null,
                                                              RootObjInstanceID = dataValue1.ParentElementID != null ? RootObjInstanceID : dataValue1.ElementID
                                                          }).ToList();

                this._unitOfWork.Repository<StringDataValue>().InsertRange(stringDataValues);

                var validDateValues = DataValueViewModelList.Where(dataValue1 =>
                                                              dataValue1.Attribute != null &&
                                                          dataValue1.Attribute.AttrType == "Date" && !string.IsNullOrEmpty(dataValue1.Value));
                List<DateDataValue> dateDataValues = (from dataValue1 in validDateValues
                                                      select new DateDataValue
                                                      {
                                                          ObjVerID = dataValue1.Attribute.ObjVerID,
                                                          Value = Convert.ToDateTime(dataValue1.Value),
                                                          ObjInstanceID = dataValue1.ElementID,
                                                          ParentObjInstanceID = dataValue1.ParentElementID.Value,
                                                          RootObjInstanceID = RootObjInstanceID,
                                                          RowIDInfo = dataValue1.IsRepeater ? dataValue1.RowIDInfo : (int?)null
                                                      }).ToList();

                this._unitOfWork.Repository<DateDataValue>().InsertRange(dateDataValues);

                this._unitOfWork.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw;
            }
            #endregion
        }
        #region OLD CODE FOR SAVE DATA VALUES
        //foreach (var dataValue in DataValueViewModelList)
        //{
        //    if (dataValue.IsContainer)
        //    {
        //        var objectInstance = new ObjectInstance();
        //        this._unitOfWork.RepositoryAsync<ObjectInstance>().Insert(objectInstance);
        //        this._unitOfWork.Save();

        //        if (dataValue.IsRoot)
        //        {

        //            RootObjInstanceID = objectInstance.ObjectInstanceID;
        //        }

        //        SaveStringDataValue(dataValue);
        //    }
        //    else
        //    {
        //        if (dataValue.Attribute.AttrType == "Integer" || dataValue.Attribute.AttrType == "Boolean" ||
        //            dataValue.Attribute.AttrType == "Double")
        //        {
        //            //Save NumericDataValue

        //            SaveNumericDataValue(dataValue);
        //        }
        //        else if (dataValue.Attribute.AttrType == "String")
        //        {
        //            //Save StringDataValue

        //           SaveStringDataValue(dataValue);
        //        }
        //        else if (dataValue.Attribute.AttrType == "Date")
        //        {
        //            //Save DateDataValue

        //            SaveDateDataValue(dataValue);
        //        }
        //    }
        //}
        //this._unitOfWork.Save();


        //private void SaveDateDataValue(DataValueViewModel dataValue)
        //{
        //    var dateDataValue = new DateDataValue();

        //    dateDataValue.ObjVerID = dataValue.Attribute.ObjVerID;

        //    dateDataValue.Value = Convert.ToDateTime(dataValue.Value);

        //    dateDataValue.ObjInstanceID = dataValue.ElementID;

        //    dateDataValue.ParentObjInstanceID = dataValue.ParentElementID.Value;

        //    dateDataValue.RootObjInstanceID = RootObjInstanceID;

        //    dateDataValue.RowIDInfo = dataValue.IsRepeater ? dataValue.RowIDInfo : (int?) null;

        //    this._unitOfWork.RepositoryAsync<DateDataValue>().Insert(dateDataValue);
        //}

        //private void SaveNumericDataValue(DataValueViewModel dataValue)
        //{
        //    var numericDataValue = new NumericDataValue();

        //    numericDataValue.ObjVerID = dataValue.Attribute.ObjVerID;

        //    numericDataValue.Value = Convert.ToDecimal(dataValue.Value);

        //    numericDataValue.ObjInstanceID = dataValue.ElementID;

        //    numericDataValue.ParentObjInstanceID = dataValue.ParentElementID.Value;

        //    numericDataValue.RootObjInstanceID = RootObjInstanceID;

        //    numericDataValue.RowIDInfo = dataValue.IsRepeater ? dataValue.RowIDInfo : (int?) null;

        //    this._unitOfWork.RepositoryAsync<NumericDataValue>().Insert(numericDataValue);
        //}

        //private void SaveStringDataValue(DataValueViewModel dataValue)
        //{
        //    var stringDataVal = new StringDataValue();

        //    stringDataVal.ObjInstanceID = dataValue.ElementID;
        //    stringDataVal.ObjVerID = dataValue.IsContainer ? (int?) null: dataValue.Attribute.ObjVerID;
        //    stringDataVal.Value = dataValue.IsContainer ? "" : dataValue.Value;
        //    stringDataVal.ParentObjInstanceID = dataValue.ParentElementID;
        //    stringDataVal.RowIDInfo = dataValue.IsRepeater ? dataValue.RowIDInfo : (int?) null;
        //    stringDataVal.RootObjInstanceID = dataValue.ParentElementID != null ? RootObjInstanceID : dataValue.ElementID;

        //    this._unitOfWork.RepositoryAsync<StringDataValue>().Insert(stringDataVal);
        //}
        #endregion

        #endregion
    }
}
