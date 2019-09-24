using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;
using tmg.equinox.repository.extensions;
using tmg.equinox.domain.entities.Utility;
using tmg.equinox.infrastructure.exceptionhandling;
using Attribute = tmg.equinox.domain.entities.Models.Attribute;
using tmg.equinox.domain.entities;


namespace tmg.equinox.applicationservices
{
    public class DomainModelService : IDomainModelService
    {
        #region Private Memebers
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor
        public DomainModelService(IUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        #endregion Constructor

        #region Public Methods
        public ServiceResult Create(int tenantId, int formDesignVersionId)
        {
            var result = new ServiceResult();

            try
            {
                //Check if formDesignVersion exists in Domain Model tables
                var formVersion = (from designVersion in this._unitOfWork.Repository<FormDesignVersion>()
                                                .Query()
                                                .Filter(e => e.FormDesignVersionID == formDesignVersionId && e.TenantID == tenantId)
                                                .Include(e => e.FormDesign)
                                                .Get()
                                   select new
                                   {
                                       designVersion.FormDesign,
                                       designVersion.VersionNumber,
                                       designVersion.EffectiveDate
                                   }).FirstOrDefault();



                if (formVersion != null)
                {
                    using (var scope = new TransactionScope(TransactionScopeOption.Required,
                                                         TimeSpan.FromMinutes(AppSettings.TransactionTimeOutPeriod)))
                    {
                        var objVersion = new ObjectVersion();

                        var objectVersionName = formVersion.FormDesign.FormName + formVersion.VersionNumber.Trim();

                        if (!this._unitOfWork.RepositoryAsync<ObjectVersion>().IsVersionExists(objectVersionName))
                        {
                            //Insert ObjectVersion
                            objVersion = SaveObjectVersion(formDesignVersionId, objVersion, objectVersionName, formVersion.EffectiveDate.Value);

                            //Retreive UIElement
                            var uiELementList = this._unitOfWork.RepositoryAsync<UIElement>()
                                                    .Query()
                                                    .Include(c => c.Validators)
                                                    .Include(c => c.ApplicationDataType)
                                                    .Include(c => c.FormDesignVersionUIElementMaps)
                                                    .Filter(e => e.FormDesignVersionUIElementMaps.Any(c => c.FormDesignVersionID == formDesignVersionId))
                                                    .AsNoTracking(true)
                                                    .Get()
                                                    .OrderBy(order => order.UIElementID)
                                                    .ToList();

                            //Insert ObjectDefinition
                            var objectDefintionList = SaveObjectDefinitions(tenantId, uiELementList);

                            //Insert ObjectRelation
                            var objectRelationList = SaveObjectRelations(uiELementList, objectDefintionList);

                            //Insert ObjectTree
                            SaveObjectTree(uiELementList, objectDefintionList, objectRelationList, objVersion);

                            //Insert Attributes

                            var attributes = SaveAttributes(uiELementList, objVersion);

                            //Insert ObjectVersionAttribXref
                            SaveObjectVersionAttribXref(attributes, uiELementList, objVersion, objectDefintionList);

                            //Insert RelationKeys and DomainModel Objects (if repeaterUIElement has a data source)
                            SaveDomainModelRelationKeys(uiELementList, tenantId, objVersion);

                            scope.Complete();
                            result.Result = ServiceResultStatus.Success;
                        }
                        else
                        {
                            result.Result = ServiceResultStatus.Failure;
                            result.Items.ToList().Add(new ServiceResultItem { Messages = new string[] { "Given Object Version already exists." } });
                        }
                    }
                }
                else
                {
                    result.Result = ServiceResultStatus.Failure;
                    result.Items.ToList().Add(new ServiceResultItem { Messages = new string[] { "Given FormDesign Version does not exist." } });
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

        public ServiceResult CheckDataSourceMappings(string username, int tenantId, int? formDesignVersionId)
        {

            ServiceResult serviceResult = null;
            List<ServiceResultItem> serviceResultItems = null;
            List<string> FormDesignList = null;

            try
            {
                serviceResult = new ServiceResult();

                var formDesignId = this._unitOfWork.RepositoryAsync<FormDesignVersion>()
                                                    .Query()
                                                    .Filter(e => e.FormDesignVersionID == formDesignVersionId && e.TenantID == tenantId)
                                                    .Get()
                                                    .Select(sel => sel.FormDesignID).SingleOrDefault();


                if (formDesignId != null)
                {
                   FormDesignList = (from dsmap in _unitOfWork.RepositoryAsync<DataSourceMapping>()
                                                               .Query()
                                                               .Include(inc => inc.DataSource)
                                                               .Filter(fil =>
                                                                       fil.FormDesignVersionID == formDesignVersionId)
                                                               .Get().ToList()
                                      join fdv in _unitOfWork.RepositoryAsync<FormDesignVersion>()
                                                             .Query()
                                                             .Include(inc => inc.FormDesign)
                                                             .Get().ToList()
                                          on dsmap.DataSource.FormDesignVersionID equals fdv.FormDesignVersionID
                                      where fdv.StatusID == 1
                                      && dsmap.FormDesignID != fdv.FormDesignID
                                      select fdv.FormDesign.FormName + " " + fdv.VersionNumber).Distinct().ToList();
                }

                if (FormDesignList != null && FormDesignList.Any())
                {
                    serviceResultItems = new List<ServiceResultItem>();
                    serviceResultItems.Add(new ServiceResultItem() { Messages = FormDesignList.ToArray() });
                    serviceResult.Items = serviceResultItems;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                serviceResult = ex.ExceptionMessages();
            }
            return serviceResult;
        }

        public ServiceResult Update(int tenantId, int formDesignVersionId)
        {
            throw new NotImplementedException();
        }

        public ServiceResult Delete(int tenantId, int formDesignVersionId)
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods

        #region Private Methods

        private ObjectVersion SaveObjectVersion(int formDesignVersionId, ObjectVersion objVersion, string objectVersionName,
                                               DateTime formVersionEffectiveDate)
        {
            objVersion = new ObjectVersion();
            objVersion.VersionName = objectVersionName.Trim().RemoveSpecialCharacters();
            objVersion.EffectiveFrom = formVersionEffectiveDate;
            objVersion.EffectiveTo = null;
            this._unitOfWork.RepositoryAsync<ObjectVersion>().Insert(objVersion);
            this._unitOfWork.Save();

            //Insert FormVersionObjectVersionMap
            var formVersionObjectVersionMap = new FormVersionObjectVersionMap();
            formVersionObjectVersionMap.FormDesignVersionID = formDesignVersionId;
            formVersionObjectVersionMap.ObjectVersionID = objVersion.VersionID;
            this._unitOfWork.RepositoryAsync<FormVersionObjectVersionMap>().Insert(formVersionObjectVersionMap);
            this._unitOfWork.Save();
            return objVersion;
        }

        private List<ObjectDefinition> SaveObjectDefinitions(int tenantId, List<UIElement> uiELements)
        {
            var objectDefintions = new List<ObjectDefinition>();
            foreach (var uiElement in uiELements.Where(e => e.IsContainer))
            {
                if (!this._unitOfWork.RepositoryAsync<ObjectDefinition>()
                         .IsObjectDefinitionExists(uiElement.Label.Trim().RemoveSpecialCharacters(), tenantId, false))
                {
                    var objectDefintion = new ObjectDefinition();

                    objectDefintion.ObjectName = uiElement.Label.Trim().RemoveSpecialCharacters().Length > 100 ?
                                                    uiElement.Label.Trim().RemoveSpecialCharacters().Substring(0, 100) :
                                                    uiElement.Label.Trim().RemoveSpecialCharacters();
                    objectDefintion.TenantID = tenantId;
                    objectDefintion.Locked = false;
                    objectDefintions.Add(objectDefintion);
                    this._unitOfWork.RepositoryAsync<ObjectDefinition>().Insert(objectDefintion);
                }
                else
                {
                    var uiElementLabel = uiElement.Label.Trim().RemoveSpecialCharacters();
                    var oldObjectDefinition = this._unitOfWork.RepositoryAsync<ObjectDefinition>()
                                                            .Query()
                                                            .Filter(filter =>
                                                                filter.ObjectName.Equals(uiElementLabel))
                                                            .Get()
                                                            .FirstOrDefault();
                    if (oldObjectDefinition != null)
                    {
                        objectDefintions.Add(oldObjectDefinition);
                    }
                }
            }
            this._unitOfWork.Save();
            return objectDefintions;
        }

        private List<ObjectRelation> SaveObjectRelations(List<UIElement> uiELements, List<ObjectDefinition> objectDefintions)
        {
            var objectRelations = new List<ObjectRelation>();
            foreach (var uiElement in uiELements.Where(e => e.IsContainer && e.ParentUIElementID != null))
            {
                UIElement element = uiElement;
                var objectDef = objectDefintions.FirstOrDefault(e => e.ObjectName == element.Label.Trim().RemoveSpecialCharacters());
                if (objectDef != null)
                {
                    if (!this._unitOfWork.RepositoryAsync<ObjectRelation>()
                             .IsObjectRelationExists(objectDef.OID, uiElement.Label.Trim().RemoveSpecialCharacters() + "Data"
                                                     , uiElement.Label.Trim().RemoveSpecialCharacters().ToCamelCase() + "Data"
                                                     , GetCardinality(uiElement)))
                    {
                        var objectRelation = new ObjectRelation();
                        objectRelation.RelatedObjectID = objectDef.OID;
                        objectRelation.RelationName = uiElement.Label.Trim().RemoveSpecialCharacters() + "Data";
                        objectRelation.RelationNameCamelcase = uiElement.Label.Trim().RemoveSpecialCharacters().ToCamelCase() + "Data";
                        objectRelation.Cardinality = GetCardinality(uiElement);
                        objectRelations.Add(objectRelation);
                        this._unitOfWork.RepositoryAsync<ObjectRelation>().Insert(objectRelation);
                    }
                    else
                    {
                        var oldObjectRelation = this._unitOfWork.RepositoryAsync<ObjectRelation>()
                                                    .Query()
                                                    .Filter(filter => filter.RelatedObjectID == objectDef.OID)
                                                    .Get()
                                                    .FirstOrDefault();
                        if (oldObjectRelation != null)
                        {
                            objectRelations.Add(oldObjectRelation);
                        }
                    }
                }
            }
            this._unitOfWork.Save();
            return objectRelations;
        }

        private void SaveObjectTree(List<UIElement> uiELements, List<ObjectDefinition> objectDefintions, List<ObjectRelation> objectRelations, ObjectVersion objVersion)
        {
            foreach (var uiElement in uiELements.Where(e => e.IsContainer))
            {
                var objectDef = objectDefintions.FirstOrDefault(e => e.ObjectName == uiElement.Label.Trim().RemoveSpecialCharacters());
                if (objectDef != null)
                {
                    var objectTree = new ObjectTree();
                    if (uiElement.ParentUIElementID != null)
                    {

                        var parentElement = uiELements.FirstOrDefault(e => e.UIElementID == uiElement.ParentUIElementID);

                        var parentObj = objectDefintions.FirstOrDefault(e => parentElement != null
                                                                             && e.ObjectName == parentElement.Label.Trim().RemoveSpecialCharacters());

                        var rootElement = uiELements.FirstOrDefault(e => e.ParentUIElementID == null);

                        var rootObj = objectDefintions.FirstOrDefault(e => rootElement != null
                                                                           && e.ObjectName == rootElement.Label.Trim().RemoveSpecialCharacters());

                        var objRelation = objectRelations.FirstOrDefault(e => e.RelatedObjectID == objectDef.OID);

                        if (parentObj != null && rootObj != null && objRelation != null)
                        {
                            if (!this._unitOfWork.RepositoryAsync<ObjectTree>().IsObjectTreeExists(parentObj.OID, rootObj.OID, objRelation.RelationID, objVersion.VersionID))
                            {
                                objectTree.ParentOID = parentObj.OID;
                                objectTree.RootOID = rootObj.OID;
                                objectTree.RelationID = objRelation.RelationID;
                                objectTree.VersionID = objVersion.VersionID;
                                this._unitOfWork.RepositoryAsync<ObjectTree>().Insert(objectTree);
                            }
                        }
                    }
                    else
                    {
                        objectTree.ParentOID = null;
                        objectTree.RootOID = objectDef.OID;
                        objectTree.RelationID = null;
                        objectTree.VersionID = objVersion.VersionID;
                        this._unitOfWork.RepositoryAsync<ObjectTree>().Insert(objectTree);
                    }
                }
            }
            this._unitOfWork.Save();
        }

        private List<Attribute> SaveAttributes(List<UIElement> uiELements, ObjectVersion objectVersion)
        {
            var attributes = new List<Attribute>();
            foreach (var uiElement in uiELements.Where(e => !e.IsContainer))
            {
                if (uiElement.Label == null)
                {
                    uiElement.Label = "Blank";
                    uiElement.GeneratedName = "Blank";
                }

                if (!uiElement.GeneratedName.Equals("Blank"))
                {
                    if (!this._unitOfWork.RepositoryAsync<Attribute>().IsAttributeExists(uiElement.Label.Trim().RemoveSpecialCharacters(),
                                                                                uiElement.Label.ToCamelCase().Trim().RemoveSpecialCharacters(),
                                                                                GetAttributeType(uiElement.ApplicationDataType.ApplicationDataTypeName),
                                                                                GetCardinality(uiElement), uiElement is TextBoxUIElement ?
                                                                                (uiElement as TextBoxUIElement).MaxLength : 0,
                                                                                null, uiElement.Validators.Select(c => c.Regex).FirstOrDefault(),
                                                                                null, false, GetDefaultValue(uiElement), objectVersion.VersionID))
                    {
                        var attribute = new Attribute();

                        attribute.Name = uiElement.Label.Trim().RemoveSpecialCharacters().Length > 100 ?
                                            uiElement.Label.Trim().RemoveSpecialCharacters().Substring(0, 100) :
                                            uiElement.Label.Trim().RemoveSpecialCharacters();
                        attribute.UIElementID = uiElement.UIElementID;
                        attribute.NameCamelcase = uiElement.Label.ToCamelCase().Trim().RemoveSpecialCharacters().Length > 100 ?
                                                    uiElement.Label.ToCamelCase().Trim().RemoveSpecialCharacters().Substring(0, 100) :
                                                    uiElement.Label.ToCamelCase().Trim().RemoveSpecialCharacters();
                        attribute.AttrType = GetAttributeType(uiElement.ApplicationDataType.ApplicationDataTypeName);
                        attribute.Cardinality = GetCardinality(uiElement);
                        attribute.Length = uiElement is TextBoxUIElement ? (uiElement as TextBoxUIElement).MaxLength : 0;
                        attribute.Precision = null;
                        attribute.EditRegex = uiElement.Validators.Select(c => c.Regex).FirstOrDefault();
                        attribute.Formatter = null;
                        attribute.Synthetic = false;
                        attribute.DefaultValue = GetDefaultValue(uiElement);
                        attributes.Add(attribute);
                        this._unitOfWork.RepositoryAsync<Attribute>().Insert(attribute);
                    }
                    else
                    {
                        var uiElementLabel = uiElement.Label.Trim().RemoveSpecialCharacters();

                        var oldAttribute = this._unitOfWork.Repository<Attribute>()
                                       .Query()
                                       .Filter(filter => filter.Name.Equals(uiElementLabel))
                                       .Get()
                                       .FirstOrDefault();

                        if (oldAttribute != null)
                        {
                            attributes.Add(oldAttribute);
                        }
                    }
                }

            }
            this._unitOfWork.Save();
            return attributes;
        }

        private void SaveObjectVersionAttribXref(List<Attribute> attributes, List<UIElement> uiELements, ObjectVersion objVersion,
                                                 List<ObjectDefinition> objectDefintions)
        {
            foreach (var attribute in attributes)
            {
                var objectVersionAttribXref = new ObjectVersionAttribXref();

                var uielementId = uiELements.Where(e => (e.Label.Trim().RemoveSpecialCharacters().Length > 100 ?
                                                            e.Label.Trim().RemoveSpecialCharacters().Substring(0, 100) :
                                                            e.Label.Trim().RemoveSpecialCharacters()) == attribute.Name && e.UIElementID == attribute.UIElementID)
                                            .Select(sel => sel.ParentUIElementID).FirstOrDefault();

                var uiElementlabel = uiELements.Where(e => e.UIElementID == uielementId)
                                               .Select(sel => sel.Label.Trim().RemoveSpecialCharacters())
                                               .FirstOrDefault();

                var oid = objectDefintions.Where(e => e.ObjectName == uiElementlabel)
                                                              .Select(sel => sel.OID)
                                                              .FirstOrDefault();


                if (!this._unitOfWork.RepositoryAsync<ObjectVersionAttribXref>()
                         .IsObjectVersionAttribXrefExists(oid, objVersion.VersionID, attribute.AttrID))
                {
                    objectVersionAttribXref.VersionID = objVersion.VersionID;
                    objectVersionAttribXref.OID = oid;
                    objectVersionAttribXref.AttrID = attribute.AttrID;
                    this._unitOfWork.RepositoryAsync<ObjectVersionAttribXref>().Insert(objectVersionAttribXref);
                }
            }
            this._unitOfWork.Save();
        }

        private void SaveDomainModelRelationKeys(List<UIElement> uiELements, int tenantId, ObjectVersion objVersion)
        {
            try
            {
                if (uiELements != null && uiELements.Any())
                {

                    var objDefList = new List<ObjectDefinationKeyValue>();

                    var dataSourceMappings = _unitOfWork.RepositoryAsync<DataSourceMapping>().Query().Get();

                    var dataSources = _unitOfWork.RepositoryAsync<DataSource>()
                                                 .Query()
                                                 .Include(e => e.SectionUIElements)
                                                 .Include(e => e.RepeaterUIElements)
                                                 .Get()
                                                 .ToList();

                    var sourceMappings = uiELements.Select(uiElement =>
                                                           dataSourceMappings.FirstOrDefault(
                                                               e => e.UIElementID == uiElement.UIElementID))
                                                   .Where(dataSourceMapping => dataSourceMapping != null)
                                                   .ToList();

                    var dataSourceList = (from dsMap in sourceMappings
                                          join ds in dataSources
                                              on dsMap.DataSourceID equals ds.DataSourceID
                                          select new
                                          {
                                              dsMap.DataSourceMappingID,
                                              dsMap.DataSourceID,
                                              dsMap.IsPrimary,
                                              dsMap.UIElementID,
                                              dsMap.MappedUIElementID,
                                              ds.SectionUIElements,
                                              ds.RepeaterUIElements,
                                              ds.Type,
                                              dsMap.DataSourceElementDisplayModeID,
                                              ds.DataSourceName
                                          }).Where(d => d.DataSourceElementDisplayModeID == (int)DisplayMode.INLINE ||
                                                            d.DataSourceElementDisplayModeID == (int)DisplayMode.CHILD).ToList();
                    if (dataSourceList.Any())
                    {
                        var objectDefintionDSList = new List<ObjectDefinition>();

                        foreach (var dataSource in dataSourceList)
                        {
                            var relationKey = new RelationKey();

                            var sourceUIElement = this._unitOfWork.RepositoryAsync<UIElement>()
                                                      .Query()
                                                      .Filter(e =>
                                                              e.UIElementID == dataSource.MappedUIElementID)
                                                      .Get()
                                                      .FirstOrDefault();

                            var targetUIElement =
                                    uiELements.Where(e => e.UIElementID == dataSource.UIElementID).FirstOrDefault();

                            if (dataSource.Type.Equals("Section"))
                            {

                                if (targetUIElement != null && sourceUIElement != null)
                                {
                                    //Save RelationKeys For SectionUIElement
                                    SaveRelationKeysForSectionDataSource(targetUIElement, sourceUIElement, relationKey);
                                }
                            }
                            else if (dataSource.Type.Equals("Repeater"))
                            {

                                var objectDefinition = new ObjectDefinition();

                                var uiElement = uiELements.FirstOrDefault(e1 => e1.UIElementID == dataSource.UIElementID);
                                var parentUiElement = uiELements.FirstOrDefault(e => uiElement != null &&
                                                                                    e.UIElementID == uiElement.ParentUIElementID);

                                if (parentUiElement != null)
                                {
                                    //Save Object Definition
                                    objectDefinition.ObjectName = dataSource.DataSourceName;
                                    objectDefinition.TenantID = tenantId;
                                    objectDefinition.Locked = false;

                                    if(objDefList.Count() == 0 || !objDefList.Any(obj => obj.ObjectName == objectDefinition.ObjectName && 
                                        obj.parentUIElementID == parentUiElement.UIElementID))
                                    {
                                      
                                        this._unitOfWork.RepositoryAsync<ObjectDefinition>().Insert(objectDefinition);
                                        objectDefintionDSList.Add(objectDefinition);
                                        this._unitOfWork.Save();

                                        objDefList.Add(new ObjectDefinationKeyValue
                                        {
                                            ObjectName = objectDefinition.ObjectName,
                                            parentUIElementID = parentUiElement.UIElementID,
                                            NewObjectDefinitionID = objectDefinition.OID
                                        });

                                        SaveDMObjectsForRepeaterDataSource(objectDefinition, objectDefintionDSList,
                                                                           parentUiElement, uiELements, objVersion);
                                    }                                   

                                    //Save Attribute 

                                    var attribute = new Attribute();

                                    if (targetUIElement != null && sourceUIElement != null)
                                    {
                                        var lhsAttribute = SaveDMAttributesForRepeaterDataSource(targetUIElement,
                                                                                                objectDefinition, attribute, objVersion, objDefList);

                                        if (lhsAttribute.AttrID > 0)
                                        {
                                            //SaveRelationKeys For RepeaterUIElement
                                            SaveRelationKeysForRepeaterDataSource(sourceUIElement, lhsAttribute, relationKey);
                                        }

                                    }
                                }
                            }
                        }
                        this._unitOfWork.Save();
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                var exceptionMessages = ex.ExceptionMessages();
            }

        }

        private Attribute SaveDMAttributesForRepeaterDataSource(UIElement targetUIElement, ObjectDefinition objectDefinition,
                                                           Attribute attribute, ObjectVersion objectVersion,List<ObjectDefinationKeyValue> objDefList)
        {
            var targetLabel = targetUIElement.Label.Trim().RemoveSpecialCharacters() +
                              objectDefinition.ObjectName;
            var targetLabelToCamelCase = targetUIElement.Label.ToCamelCase().Trim().RemoveSpecialCharacters() +
                                         objectDefinition.ObjectName;

            var objID = objDefList.Where(fi => fi.parentUIElementID == targetUIElement.ParentUIElementID)
                .Select(sel => sel.NewObjectDefinitionID).FirstOrDefault();

            if (!this._unitOfWork.RepositoryAsync<Attribute>().Query()
                .Include(inc => inc.ObjectVersionAttribXrefs).Get()
                .Where(e => e.Name == (targetLabel.Length > 100 ? targetLabel.Substring(0, 100) : 
                    targetLabel) && e.ObjectVersionAttribXrefs.Any(e1 => e1.OID == objID)).Any())
            {
                attribute.Name = targetLabel.Length > 100
                                 ? targetLabel.Substring(0, 100)
                                 : targetLabel;
                attribute.UIElementID = targetUIElement.UIElementID;
                attribute.NameCamelcase = targetLabelToCamelCase.Length > 100
                                              ? targetLabelToCamelCase.Substring(0, 100)
                                              : targetLabelToCamelCase;
                attribute.AttrType = GetAttributeType(targetUIElement.ApplicationDataType.ApplicationDataTypeName);
                attribute.Cardinality = GetCardinality(targetUIElement);
                attribute.Length = targetUIElement is TextBoxUIElement ? (targetUIElement as TextBoxUIElement).MaxLength : 0;
                attribute.Precision = null;
                attribute.EditRegex = targetUIElement.Validators.Select(c => c.Regex).FirstOrDefault();
                attribute.Formatter = null;
                attribute.Synthetic = false;
                attribute.DefaultValue = GetDefaultValue(targetUIElement);
                this._unitOfWork.RepositoryAsync<Attribute>().Insert(attribute);
                this._unitOfWork.Save();


                //Save ObjectVersionAttributeXref
                var objectVersionAttributeXref = new ObjectVersionAttribXref();
            
                if (!_unitOfWork.RepositoryAsync<ObjectVersionAttribXref>()
                                .IsObjectVersionAttribXrefExists(objID, objectVersion.VersionID,
                                                                 attribute.AttrID))
                {
                    objectVersionAttributeXref.VersionID = objectVersion.VersionID;
                    objectVersionAttributeXref.OID = objectDefinition.OID <= 0 ? objID : objectDefinition.OID;

                    objectVersionAttributeXref.AttrID = attribute.AttrID;
                    this._unitOfWork.RepositoryAsync<ObjectVersionAttribXref>().Insert(objectVersionAttributeXref);
                    this._unitOfWork.Save();
                }
            }

            var objdefinitionID = attribute.ObjectVersionAttribXrefs.Select(sel => sel.OID).FirstOrDefault();

            var parentOID = this._unitOfWork.RepositoryAsync<ObjectTree>()
                                .Query()
                                .Include(inc => inc.ObjectRelation)
                                .Get()
                                .Where(e => e.ObjectRelation.RelatedObjectID == objdefinitionID)
                                .Select(sel => sel.ParentOID)
                                .FirstOrDefault();
            

            var targetLabeltoDelete = targetUIElement.Label.Trim().RemoveSpecialCharacters();
            var targetLabelToDeleteCamelCase = targetUIElement.Label.ToCamelCase().Trim().RemoveSpecialCharacters();

            if (this._unitOfWork.RepositoryAsync<Attribute>()
                     .IsAttributeExists(targetLabeltoDelete.Length > 100 ? targetLabeltoDelete.Substring(0, 100) : targetLabeltoDelete,
                                        targetLabelToDeleteCamelCase.Length > 100
                                        ? targetLabelToDeleteCamelCase.Substring(0, 100) : targetLabelToDeleteCamelCase,
                                        GetAttributeType(targetUIElement.ApplicationDataType.ApplicationDataTypeName),
                                        GetCardinality(targetUIElement), targetUIElement is TextBoxUIElement
                                        ? (targetUIElement as TextBoxUIElement).MaxLength : 0,
                                        null, targetUIElement.Validators.Select(c => c.Regex).FirstOrDefault(),
                                        null, false, GetDefaultValue(targetUIElement), objectVersion.VersionID)) 
            {
               DeleteAttributesForDsMapping(targetLabeltoDelete.Length > 100 ? targetLabeltoDelete.Substring(0, 100) : targetLabeltoDelete,
                                     targetLabelToDeleteCamelCase.Length > 100
                                     ? targetLabelToDeleteCamelCase.Substring(0, 100) : targetLabelToDeleteCamelCase,
                                     GetAttributeType(targetUIElement.ApplicationDataType.ApplicationDataTypeName),
                                     GetCardinality(targetUIElement), targetUIElement is TextBoxUIElement
                                     ? (targetUIElement as TextBoxUIElement).MaxLength : 0,
                                     null, targetUIElement.Validators.Select(c => c.Regex).FirstOrDefault(),
                                     null, false, GetDefaultValue(targetUIElement), objectVersion.VersionID, parentOID);
            }

            return attribute;
        }

        public void DeleteAttributesForDsMapping(string name, 
            string nameCamelCase, string attributeType, string cardinality, int? length, int? precision, string regex, string formatter,
            bool? synthetic, string defaultValue, int versionID,int? parentOID)
        {
            var attrID =  this._unitOfWork.RepositoryAsync<Attribute>()
                             .Query()
                             .Include(inc => inc.ObjectVersionAttribXrefs)
                             .Filter(c => c.Name == name && c.NameCamelcase == nameCamelCase &&
                                    c.AttrType == attributeType && c.Cardinality == cardinality &&
                                    c.Length == length && c.Precision == precision && c.EditRegex == regex &&
                                    c.Formatter == formatter && c.Synthetic == synthetic && c.DefaultValue == defaultValue &&
                                    c.ObjectVersionAttribXrefs.FirstOrDefault().VersionID == versionID)
                             .Get()
                             .Where (e => e.ObjectVersionAttribXrefs.Any(e1 => e1.OID == parentOID.Value))
                             .Select(sel => sel.AttrID)                             
                             .FirstOrDefault();


            var objVerID = this._unitOfWork.RepositoryAsync<ObjectVersionAttribXref>()
                                .Query()
                                .Filter(fil => fil.AttrID == attrID)
                                .Get()
                                .Select(sel => sel.ObjVerID).FirstOrDefault();

            this._unitOfWork.RepositoryAsync<ObjectVersionAttribXref>().Delete(objVerID);

             this._unitOfWork.RepositoryAsync<Attribute>().Delete(attrID);

            this._unitOfWork.Save();

        }

        private void SaveDMObjectsForRepeaterDataSource(ObjectDefinition objectDefinition, List<ObjectDefinition> objectDefintionDSList,
                                                        UIElement uiElement, List<UIElement> uiELementList, ObjectVersion objectVersion)
        {
           
            //Save Object Relation

            var objectRelation = new ObjectRelation();

            objectRelation.RelatedObjectID = objectDefinition.OID;
            objectRelation.RelationName = objectDefinition.ObjectName + "Data";
            objectRelation.RelationNameCamelcase = objectDefinition.ObjectName.ToCamelCase() + "Data";
            objectRelation.Cardinality = "M";

            if (!_unitOfWork.RepositoryAsync<ObjectRelation>()
                           .IsObjectRelationExists(objectRelation.RelatedObjectID.Value, objectRelation.RelationName,
                                                   objectRelation.RelationNameCamelcase, objectRelation.Cardinality))
            {
                this._unitOfWork.RepositoryAsync<ObjectRelation>().Insert(objectRelation);
                this._unitOfWork.Save();

                //Save Object Tree
                var objectTree = new ObjectTree();

                var uiELementLabel = uiElement.Label.Trim().RemoveSpecialCharacters();

                var parentObject = _unitOfWork.RepositoryAsync<ObjectDefinition>().Query().Filter(filter => filter.ObjectName == uiELementLabel).Get().Select(sel => sel.OID).FirstOrDefault();

                var rootElement = uiELementList.Where(e => e.ParentUIElementID == null)
                                               .Select(sel => sel.Label)
                                               .FirstOrDefault();
                if (rootElement != null)
                {
                    var rootUIElementLabel = rootElement.Trim().RemoveSpecialCharacters();

                    var rootObject = _unitOfWork.RepositoryAsync<ObjectDefinition>().Query().Filter(filter => rootElement != null && filter.ObjectName == rootUIElementLabel).Get().Select(sel => sel.OID).FirstOrDefault();

                    if (!_unitOfWork.RepositoryAsync<ObjectTree>()
                            .IsObjectTreeExists(parentObject, rootObject, objectRelation.RelationID, objectVersion.VersionID))
                    {
                        objectTree.RootOID = rootObject;
                        objectTree.ParentOID = parentObject;
                        objectTree.RelationID = objectRelation.RelationID;
                        objectTree.VersionID = objectVersion.VersionID;

                        this._unitOfWork.RepositoryAsync<ObjectTree>().Insert(objectTree);
                        this._unitOfWork.Save();
                    }


                }
            }
        }

        private void SaveRelationKeysForSectionDataSource(UIElement targetUIElement, UIElement sourceUIElement,
                                                          RelationKey relationKey)
        {
            var targetLabel = targetUIElement.Label.Trim().RemoveSpecialCharacters();
            var targetElementID = targetUIElement.UIElementID;


            var lhsAttributeId = this._unitOfWork.RepositoryAsync<Attribute>()
                                     .Query()
                                     .Filter(filter => filter.Name.Equals(targetLabel))
                                     .Get()
                                     .ToList()
                                     .Where(e => e.UIElementID == targetElementID)
                                     .Select(sel => sel.AttrID)
                                     .SingleOrDefault();

            var rhsAttributeId = (from objDef in this._unitOfWork.RepositoryAsync<ObjectDefinition>().Query().Get().ToList()
                                  join objAttribXref in this._unitOfWork.RepositoryAsync<ObjectVersionAttribXref>().Query().Get().ToList()
                                  on objDef.OID equals objAttribXref.OID
                                  into temp
                                  from obj in temp
                                  join attr in this._unitOfWork.RepositoryAsync<Attribute>().Query().Get().ToList()
                                  on obj.AttrID equals attr.AttrID
                                  where attr.Name == sourceUIElement.GeneratedName
                                  select attr.AttrID).FirstOrDefault();

            //SaveRelationKeys(sourceUIElement, relationKey, lhsAttributeId, rhsAttributeId);
        }

        private void SaveRelationKeysForRepeaterDataSource(UIElement sourceUIElement, Attribute lhsAttribute,
                                                          RelationKey relationKey)
        {
            var lhsAttributeId = lhsAttribute.AttrID;

            var rhsAttributeId = (from objDef in this._unitOfWork.RepositoryAsync<ObjectDefinition>().Query().Get().ToList()
                                  join objAttribXref in this._unitOfWork.RepositoryAsync<ObjectVersionAttribXref>().Query().Get().ToList()
                                  on objDef.OID equals objAttribXref.OID
                                  into temp
                                  from obj in temp
                                  join attr in this._unitOfWork.RepositoryAsync<Attribute>().Query().Get().ToList()
                                  on obj.AttrID equals attr.AttrID
                                  where attr.Name == sourceUIElement.GeneratedName
                                  select attr.AttrID).FirstOrDefault();

            //SaveRelationKeys(sourceUIElement, relationKey, lhsAttributeId, rhsAttributeId);
        }

        private void SaveRelationKeys(UIElement sourceUIElement, RelationKey relationKey, int lhsAttributeId, int rhsAttributeId)
        {
            var sourceUIElementName = this._unitOfWork.RepositoryAsync<UIElement>()
                                          .Query()
                                          .Filter(filter =>
                                                  filter.UIElementID == sourceUIElement.ParentUIElementID)
                                          .Get()
                                          .Select(sel => sel.Label)
                                          .FirstOrDefault();

            if (sourceUIElementName != null)
            {
                var uiElementLabel = sourceUIElementName.Trim().RemoveSpecialCharacters();

                var relationId = this._unitOfWork.RepositoryAsync<ObjectRelation>()
                                     .Query()
                                     .Include(obj => obj.ObjectDefinition)
                                     .Filter(
                                         filter => sourceUIElementName != null &&
                                                   filter.ObjectDefinition.ObjectName.Equals(uiElementLabel))
                                     .Get()
                                     .Select(sel => sel.RelationID)
                                     .FirstOrDefault();

                if (!this._unitOfWork.RepositoryAsync<RelationKey>()
                         .IsRelationKeyExists(lhsAttributeId, rhsAttributeId, relationId))
                {
                    relationKey.LHSAttrID = lhsAttributeId;
                    relationKey.RHSAttrID = rhsAttributeId;
                    relationKey.RelationID = relationId;

                    this._unitOfWork.RepositoryAsync<RelationKey>().Insert(relationKey);
                    this._unitOfWork.Save();
                }
            }
        }

        private string GetDefaultValue(UIElement item)
        {
            string returnVal = string.Empty;
            try
            {
                if (item is RadioButtonUIElement)
                {
                    returnVal = (item as RadioButtonUIElement).DefaultValue.ToString() == "false" ? "0" : "1";
                }
                else if (item is TextBoxUIElement)
                {
                    var uiElement = (item as TextBoxUIElement);

                    if (item.UIElementDataTypeID == 1)
                    {
                        returnVal = !string.IsNullOrEmpty(uiElement.DefaultValue) ? uiElement.DefaultValue : "0";
                    }
                    else
                    {
                        returnVal = !string.IsNullOrEmpty(uiElement.DefaultValue) ? uiElement.DefaultValue : string.Empty;
                    }
                }
                else if (item is CheckBoxUIElement)
                {
                    returnVal = (item as CheckBoxUIElement).DefaultValue.ToString() == "false" ? "0" : "1";
                }
                else if (item is CalendarUIElement)
                {
                    returnVal = (item as CalendarUIElement).DefaultDate.ToString();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return returnVal;
        }

        /// <summary>
        /// returns the Java datatype equivalent
        /// </summary>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        private string GetAttributeType(string attributeType)
        {
            string returnVal = string.Empty;
            try
            {
                switch (attributeType)
                {
                    case "int":
                        returnVal = "Integer";
                        break;
                    case "string":
                        returnVal = "String";
                        break;
                    case "date":
                        returnVal = "Date";
                        break;
                    case "bool":
                        returnVal = "Boolean";
                        break;
                    case "float":
                        returnVal = "Double";
                        break;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return returnVal;
        }

        private string GetCardinality(UIElement uiElement)
        {
            string cardinality = string.Empty;
            try
            {
                if (uiElement is RepeaterUIElement)
                {
                    cardinality = "M";
                }
                else if (uiElement is SectionUIElement)
                {
                    cardinality = "O";
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return cardinality;
        }

        #endregion Private Methods

    }

    public class ObjectDefinationKeyValue
    {
        public string ObjectName { get; set; }
        public int parentUIElementID { get; set; }
        public int NewObjectDefinitionID { get; set; }
    }
}
