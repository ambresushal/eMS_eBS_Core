using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.UIElement;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.repository.interfaces;
using tmg.equinox.repository.extensions;
using System.Transactions;
using System.Text.RegularExpressions;
using tmg.equinox.applicationservices.viewmodels.ServiceDesign;

namespace tmg.equinox.applicationservices
{
    public class ServiceDefinitionService : IServiceDefinitionService
    {
        #region Private Members
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        private ILoggingService _loggingService { get; set; }
        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor
        public ServiceDefinitionService(IUnitOfWorkAsync unitOfWork, ILoggingService loggingService)
        {
            this._unitOfWork = unitOfWork;
            this._loggingService = loggingService;
        }
        #endregion Constructor

        #region Public Methods
        public IEnumerable<ServiceDefinitionRowModel> GetServiceDefinitionListForServiceDesignVersion(int tenantId, int serviceDesignVersionID)
        {
            IList<ServiceDefinitionRowModel> uiElementRowModelList = null;
            try
            {
                //get all elements
                var elementList = this._unitOfWork.RepositoryAsync<ServiceDefinition>().GetServiceDefinitionListForServiceDesignVersion(tenantId, serviceDesignVersionID).ToList();

                if (elementList.Count() > 0)
                {
                    uiElementRowModelList = (from i in elementList
                                             select new ServiceDefinitionRowModel
                                             {
                                                 ServiceDefinitionID = i.ServiceDefinitionID,
                                                 DisplayName = i.DisplayName,
                                                 UIElementDataTypeID = i.UIElementDataTypeID,
                                                 UIElementDataType = i.ApplicationDataType.ApplicationDataTypeName,
                                                 UIElementType = GetElementType(i.UIElement),
                                                 UIElementTypeID = i.UIElementTypeID,
                                                 UIElementLabel = i.UIElement.Label,
                                                 Required = i.IsRequired == true ? "Yes" : "No",
                                                 Sequence = i.Sequence,
                                                 UIElementID = i.UIElementID,
                                                 ParentServiceDefinitionID = i.ParentServiceDefinitionID,
                                                 level = i.ParentServiceDefinitionID.HasValue ? GetRowLevel(i.ParentServiceDefinitionID, elementList) : 0,
                                                 parent = i.ParentServiceDefinitionID.HasValue ? i.ParentServiceDefinitionID.Value.ToString() : "0",
                                                 isLeaf = i.ParentServiceDefinitionID.HasValue ? IsLeafRow(i.ServiceDefinitionID, elementList) : false,
                                                 isExt = true,
                                                 loaded = true
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

        public ServiceResult AddServiceDefinition(int tenantID, int formDesignVersionID, int serviceDesignVersionID, int uiElementID, int parentServiceDefinitionID, string username, bool isKey, bool addChildKeys, bool addChildElements)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                UIElement uielement = this._unitOfWork.Repository<UIElement>().FindById(uiElementID);
                ServiceDesignVersion serviceDesignVersion = this._unitOfWork.Repository<ServiceDesignVersion>().FindById(serviceDesignVersionID);
                if (uielement != null && serviceDesignVersion != null)
                {
                    ServiceDefinition servicedef = new ServiceDefinition();
                    servicedef.UIElementID = uiElementID;
                    servicedef.ParentServiceDefinitionID = parentServiceDefinitionID;
                    servicedef.UIElementDataTypeID = uielement.UIElementDataTypeID;
                    servicedef.UIElementTypeID = GetElementTypeID(uielement);
                    servicedef.DisplayName = uielement.Label;
                    servicedef.IsKey = isKey;
                    servicedef.IsRequired = false;
                    servicedef.UIElementFullPath = GetUIElementFullPath(uielement.UIElementID, formDesignVersionID);
                    servicedef.ServiceDesignID = serviceDesignVersion.ServiceDesignID;
                    int sequenceNo = 1;
                    var fields = this._unitOfWork.RepositoryAsync<ServiceDefinition>()
                                                            .Query()
                                                            .Filter(c => (c.ParentServiceDefinitionID == parentServiceDefinitionID && c.ServiceDesignID == serviceDesignVersion.ServiceDesignID))
                                                            .Get();
                    if (fields != null && fields.Count() > 0)
                    {
                        sequenceNo = fields.Max(c => c.Sequence);
                    }
                    servicedef.Sequence = sequenceNo + 1;
                    servicedef.AddedBy = username;
                    servicedef.AddedDate = DateTime.Now;
                    servicedef.TenantID = tenantID;
                    servicedef.IsPartOfDataSource = false;

                    this._unitOfWork.Repository<ServiceDefinition>().Insert(servicedef);

                    ServiceDesignVersionServiceDefinitionMap map = new ServiceDesignVersionServiceDefinitionMap();
                    map.ServiceDefinitionID = servicedef.ServiceDefinitionID;
                    map.ServiceDesignVersionID = serviceDesignVersionID;
                    map.EffectiveDate = serviceDesignVersion.EffectiveDate;

                    this._unitOfWork.Repository<ServiceDesignVersionServiceDefinitionMap>().Insert(map);

                    using (var scope = new TransactionScope())
                    {
                        this._unitOfWork.Save();
                        scope.Complete();

                        IList<ServiceResultItem> item = new List<ServiceResultItem>();
                        item.Add(new ServiceResultItem { Messages = new string[] { servicedef.ServiceDefinitionID.ToString() } });

                        result.Items = item;
                        result.Result = ServiceResultStatus.Success;
                    }

                    if (uielement.IsContainer)
                    {
                        if (uielement.IsRepeater())
                        {
                            var childElementIDs = this._unitOfWork.RepositoryAsync<UIElement>()
                                                                    .Query()
                                                                    .Filter(c => c.ParentUIElementID == uielement.UIElementID)
                                                                    .Get()
                                                                    .Select(c => c.UIElementID)
                                                                    .ToList();

                            bool hasDataSource = this._unitOfWork.RepositoryAsync<DataSourceMapping>()
                                                                    .Query()
                                                                    .Filter(c => childElementIDs.Contains(c.UIElementID))
                                                                    .Get()
                                                                    .Any();
                            if (hasDataSource || addChildElements)
                            {
                                //call function to copy all the repeater elements & store inside the service definition table.
                                this.AddRepeaterChildElements(uiElementID, servicedef.ServiceDefinitionID, formDesignVersionID, serviceDesignVersion.ServiceDesignID, serviceDesignVersion.ServiceDesignVersionID, serviceDesignVersion.EffectiveDate, tenantID, username);
                            }
                            else if (addChildElements)
                            {
                                this.AddRepeaterKeyElements(uiElementID, servicedef.ServiceDefinitionID, formDesignVersionID, serviceDesignVersion.ServiceDesignID, serviceDesignVersion.ServiceDesignVersionID, serviceDesignVersion.EffectiveDate, tenantID, username);
                            }
                        }
                        else
                        {
                            //call function to copy all the section elements & store inside the service definition table.
                            this.AddSectionChildElements(uiElementID, servicedef.ServiceDefinitionID, formDesignVersionID, serviceDesignVersion.ServiceDesignID, serviceDesignVersion.ServiceDesignVersionID, serviceDesignVersion.EffectiveDate, tenantID, username);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
                result = ex.ExceptionMessages();
            }
            return result;
        }

        private void AddRepeaterChildElements(int repeaterUIElementID, int serviceDefinitionID, int formDesignVersionID, int serviceDesignID, int serviceDesignVersionID, DateTime serviceDesignVersionEffectiveDate, int tenantID, string username)
        {
            try
            {
                IList<UIElement> repeaterElementList = (from c in this._unitOfWork.Repository<UIElement>()
                                                                         .Query()
                                                                         .Filter(d => d.ParentUIElementID == repeaterUIElementID)
                                                                         .Get()
                                                        select c).ToList();

                if (repeaterElementList.Count() > 1)
                {
                    foreach (var item in repeaterElementList)
                    {
                        ServiceDefinition servicedef = new ServiceDefinition();
                        servicedef.UIElementID = item.UIElementID;
                        servicedef.ParentServiceDefinitionID = serviceDefinitionID;
                        servicedef.UIElementDataTypeID = item.UIElementDataTypeID;
                        servicedef.UIElementTypeID = GetElementTypeID(item);
                        servicedef.DisplayName = item.Label;
                        servicedef.IsKey = _unitOfWork.Repository<RepeaterKeyUIElement>()
                                                        .Query()
                                                        .Filter(c => c.RepeaterUIElementID == repeaterUIElementID && c.UIElementID == item.UIElementID)
                                                        .Get()
                                                        .Any();
                        servicedef.IsRequired = false;
                        servicedef.UIElementFullPath = GetUIElementFullPath(item.UIElementID, formDesignVersionID);
                        servicedef.ServiceDesignID = serviceDesignID;
                        int sequenceNo = 1;
                        var fields = this._unitOfWork.RepositoryAsync<ServiceDefinition>()
                                                                .Query()
                                                                .Filter(c => (c.ParentServiceDefinitionID == serviceDefinitionID && c.ServiceDesignID == serviceDesignID))
                                                                .Get();
                        if (fields != null && fields.Count() > 0)
                        {
                            sequenceNo = fields.Max(c => c.Sequence);
                        }
                        servicedef.Sequence = sequenceNo + 1;
                        servicedef.AddedBy = username;
                        servicedef.AddedDate = DateTime.Now;
                        servicedef.TenantID = tenantID;
                        servicedef.IsPartOfDataSource = this._unitOfWork.RepositoryAsync<DataSourceMapping>()
                                                                            .Query()
                                                                            .Filter(c => c.UIElementID == item.UIElementID)
                                                                            .Get()
                                                                            .Any();

                        this._unitOfWork.Repository<ServiceDefinition>().Insert(servicedef);

                        ServiceDesignVersionServiceDefinitionMap map = new ServiceDesignVersionServiceDefinitionMap();
                        map.ServiceDefinitionID = servicedef.ServiceDefinitionID;
                        map.ServiceDesignVersionID = serviceDesignVersionID;
                        map.EffectiveDate = serviceDesignVersionEffectiveDate;

                        this._unitOfWork.Repository<ServiceDesignVersionServiceDefinitionMap>().Insert(map);

                        this._unitOfWork.Save();
                    }

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void AddRepeaterKeyElements(int repeaterUIElementID, int serviceDefinitionID, int formDesignVersionID, int serviceDesignID, int serviceDesignVersionID, DateTime serviceDesignVersionEffectiveDate, int tenantID, string username)
        {
            try
            {
                IList<UIElement> repeaterElementList = (from u in this._unitOfWork.Repository<RepeaterKeyUIElement>()
                                                                          .Query()
                                                                          .Filter(c => c.RepeaterUIElementID == repeaterUIElementID)
                                                                          .Get()
                                                        join c in this._unitOfWork.Repository<UIElement>()
                                                                         .Query()
                                                                         .Filter(d => d.ParentUIElementID == repeaterUIElementID)
                                                                         .Get()
                                                        on u.UIElementID equals c.UIElementID
                                                        select c).ToList();

                if (repeaterElementList.Count() > 1)
                {
                    foreach (var item in repeaterElementList)
                    {
                        ServiceDefinition servicedef = new ServiceDefinition();
                        servicedef.UIElementID = item.UIElementID;
                        servicedef.ParentServiceDefinitionID = serviceDefinitionID;
                        servicedef.UIElementDataTypeID = item.UIElementDataTypeID;
                        servicedef.UIElementTypeID = GetElementTypeID(item);
                        servicedef.DisplayName = item.Label;
                        servicedef.IsKey = true;
                        servicedef.IsRequired = false;
                        servicedef.UIElementFullPath = GetUIElementFullPath(item.UIElementID, formDesignVersionID);
                        servicedef.ServiceDesignID = serviceDesignID;
                        int sequenceNo = 1;
                        var fields = this._unitOfWork.RepositoryAsync<ServiceDefinition>()
                                                                .Query()
                                                                .Filter(c => (c.ParentServiceDefinitionID == serviceDefinitionID && c.ServiceDesignID == serviceDesignID))
                                                                .Get();
                        if (fields != null && fields.Count() > 0)
                        {
                            sequenceNo = fields.Max(c => c.Sequence);
                        }
                        servicedef.Sequence = sequenceNo + 1;
                        servicedef.AddedBy = username;
                        servicedef.AddedDate = DateTime.Now;
                        servicedef.TenantID = tenantID;
                        servicedef.IsPartOfDataSource = this._unitOfWork.RepositoryAsync<DataSourceMapping>()
                                                                            .Query()
                                                                            .Filter(c => c.UIElementID == item.UIElementID)
                                                                            .Get()
                                                                            .Any();

                        this._unitOfWork.Repository<ServiceDefinition>().Insert(servicedef);

                        ServiceDesignVersionServiceDefinitionMap map = new ServiceDesignVersionServiceDefinitionMap();
                        map.ServiceDefinitionID = servicedef.ServiceDefinitionID;
                        map.ServiceDesignVersionID = serviceDesignVersionID;
                        map.EffectiveDate = serviceDesignVersionEffectiveDate;

                        this._unitOfWork.Repository<ServiceDesignVersionServiceDefinitionMap>().Insert(map);

                        this._unitOfWork.Save();
                    }

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void AddSectionChildElements(int sectionUIElementID, int serviceDefinitionID, int formDesignVersionID, int serviceDesignID, int serviceDesignVersionID, DateTime serviceDesignVersionEffectiveDate, int tenantID, string username)
        {
            try
            {
                IList<UIElement> childElementList = (from c in this._unitOfWork.Repository<UIElement>()
                                                                         .Query()
                                                                         .Filter(d => d.ParentUIElementID == sectionUIElementID)
                                                                         .Get()
                                                     select c).ToList();

                if (childElementList.Count() > 1)
                {
                    foreach (var item in childElementList)
                    {
                        if (!item.IsBlank())
                        {
                            ServiceDefinition servicedef = new ServiceDefinition();
                            servicedef.UIElementID = item.UIElementID;
                            servicedef.ParentServiceDefinitionID = serviceDefinitionID;
                            servicedef.UIElementDataTypeID = item.UIElementDataTypeID;
                            servicedef.UIElementTypeID = GetElementTypeID(item);
                            servicedef.DisplayName = item.Label;
                            servicedef.IsKey = false;
                            servicedef.IsRequired = false;
                            servicedef.UIElementFullPath = GetUIElementFullPath(item.UIElementID, formDesignVersionID);
                            servicedef.ServiceDesignID = serviceDesignID;
                            int sequenceNo = 1;
                            var fields = this._unitOfWork.RepositoryAsync<ServiceDefinition>()
                                                                    .Query()
                                                                    .Filter(c => (c.ParentServiceDefinitionID == serviceDefinitionID && c.ServiceDesignID == serviceDesignID))
                                                                    .Get();
                            if (fields != null && fields.Count() > 0)
                            {
                                sequenceNo = fields.Max(c => c.Sequence);
                            }
                            servicedef.Sequence = sequenceNo + 1;
                            servicedef.AddedBy = username;
                            servicedef.AddedDate = DateTime.Now;
                            servicedef.TenantID = tenantID;
                            servicedef.IsPartOfDataSource = this._unitOfWork.RepositoryAsync<DataSourceMapping>()
                                                                                .Query()
                                                                                .Filter(c => c.UIElementID == item.UIElementID)
                                                                                .Get()
                                                                                .Any();

                            this._unitOfWork.Repository<ServiceDefinition>().Insert(servicedef);

                            ServiceDesignVersionServiceDefinitionMap map = new ServiceDesignVersionServiceDefinitionMap();
                            map.ServiceDefinitionID = servicedef.ServiceDefinitionID;
                            map.ServiceDesignVersionID = serviceDesignVersionID;
                            map.EffectiveDate = serviceDesignVersionEffectiveDate;

                            this._unitOfWork.Repository<ServiceDesignVersionServiceDefinitionMap>().Insert(map);

                            this._unitOfWork.Save();

                            if (item is SectionUIElement)
                            {
                                this.AddSectionChildElements(item.UIElementID, servicedef.ServiceDefinitionID, formDesignVersionID, serviceDesignID, serviceDesignVersionID, serviceDesignVersionEffectiveDate, tenantID, username);
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Deletes the service definition.
        /// </summary>
        /// <param name="tenantID">The tenant identifier.</param>
        /// <param name="serviceDesignVersionID">The service design version identifier.</param>
        /// <param name="serviceDefinitionID">The service definition identifier.</param>
        /// <returns></returns>
        public ServiceResult DeleteServiceDefinition(int tenantID, int serviceDesignVersionID, int serviceDefinitionID)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                string exceptionMessage = string.Empty;
                this._unitOfWork.RepositoryAsync<ServiceDefinition>().DeleteServiceDefinition(_unitOfWork, tenantID, serviceDefinitionID, serviceDesignVersionID, out exceptionMessage);
                if (string.IsNullOrEmpty(exceptionMessage))
                {
                    result.Result = ServiceResultStatus.Success;
                }
                else
                {
                    result.Result = ServiceResultStatus.Failure;
                    List<ServiceResultItem> items = new List<ServiceResultItem>();
                    items.Add(new ServiceResultItem { Messages = new string[] { exceptionMessage } });
                    result.Items = items;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
                result = ex.ExceptionMessages();
            }
            return result;
        }

        public ServiceResult UpdateServiceDefinition(ServiceDefinitionViewModel viewModel, string userName)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                ServiceDefinition serviceDefinition = this._unitOfWork.Repository<ServiceDefinition>().FindById(viewModel.ServiceDefinitionID);
                bool isFinalized = this._unitOfWork.RepositoryAsync<ServiceDesignVersion>().IsFinalized(viewModel.ServiceDesignVersionID);
                //TODO: check if the Service Design Version is finalized
                //if finalized, update the ServiceDesignVersionServiceDefinitionMap table with CancelDate as EffectiveDate of respective ServiceDesignVersion - 1
                // & add new element
                //update element otherwise
                if (serviceDefinition != null)
                {
                    if (isFinalized)
                    {
                        result.Result = ServiceResultStatus.Failure;
                        result.Items.ToList().Add(new ServiceResultItem { Messages = new string[] { "Unable to Update finalized Form Design Version" } });
                    }
                    else
                    {
                        List<ServiceDesignVersionServiceDefinitionMap> list = this._unitOfWork.RepositoryAsync<ServiceDesignVersionServiceDefinitionMap>()
                                                                                    .Query()
                                                                                    .Include(c => c.ServiceDesignVersion)
                                                                                    .Filter(c => c.ServiceDefinitionID == viewModel.ServiceDefinitionID && c.ServiceDesignVersionID != viewModel.ServiceDesignVersionID)
                                                                                    .Get()
                                                                                    .ToList();
                        ServiceDesignVersionServiceDefinitionMap currentMap = this._unitOfWork.RepositoryAsync<ServiceDesignVersionServiceDefinitionMap>()
                                                                                    .Query()
                                                                                    .Include(c => c.ServiceDesignVersion)
                                                                                    .Filter(c => c.ServiceDesignVersionID == viewModel.ServiceDesignVersionID && c.ServiceDefinitionID == viewModel.ServiceDefinitionID)
                                                                                    .Get()
                                                                                    .FirstOrDefault();
                        if (list.Any(c => c.ServiceDesignVersion.IsFinalized == true))
                        {
                            //update effective date of removal 
                            //to current map effective date - 1
                            foreach (var item in list.ToList())
                            {
                                item.CancelDate = currentMap.EffectiveDate.AddDays(-1);
                                this._unitOfWork.RepositoryAsync<ServiceDesignVersionServiceDefinitionMap>().Update(item);
                            }

                            ServiceDefinition servicedef = new ServiceDefinition();
                            servicedef.UIElementID = serviceDefinition.UIElementID;
                            servicedef.ParentServiceDefinitionID = serviceDefinition.ParentServiceDefinitionID;
                            servicedef.UIElementDataTypeID = viewModel.UIElementDataTypeID == 0 ? serviceDefinition.UIElementDataTypeID : viewModel.UIElementDataTypeID;
                            servicedef.UIElementTypeID = serviceDefinition.UIElementTypeID;
                            servicedef.DisplayName = viewModel.DisplayName ?? serviceDefinition.DisplayName;
                            servicedef.IsKey = serviceDefinition.IsKey;
                            servicedef.IsRequired = viewModel.IsRequired;
                            servicedef.UIElementFullPath = serviceDefinition.UIElementFullPath;
                            servicedef.ServiceDesignID = serviceDefinition.ServiceDesignID;
                            int sequenceNo = 1;
                            var fields = this._unitOfWork.RepositoryAsync<ServiceDefinition>()
                                                                    .Query()
                                                                    .Filter(c => (c.ParentServiceDefinitionID == serviceDefinition.ParentServiceDefinitionID && c.ServiceDesignID == serviceDefinition.ServiceDesignID))
                                                                    .Get();
                            if (fields != null && fields.Count() > 0)
                            {
                                sequenceNo = fields.Max(c => c.Sequence);
                            }
                            servicedef.Sequence = sequenceNo + 1;
                            servicedef.AddedBy = userName;
                            servicedef.AddedDate = DateTime.Now;
                            servicedef.TenantID = serviceDefinition.TenantID;
                            servicedef.IsPartOfDataSource = this._unitOfWork.RepositoryAsync<DataSourceMapping>()
                                                                                .Query()
                                                                                .Filter(c => c.UIElementID == viewModel.UIElementID)
                                                                                .Get()
                                                                                .Any();

                            this._unitOfWork.Repository<ServiceDefinition>().Insert(servicedef);

                            ServiceDesignVersionServiceDefinitionMap map = new ServiceDesignVersionServiceDefinitionMap();
                            map.ServiceDefinitionID = servicedef.ServiceDefinitionID;
                            map.ServiceDesignVersionID = viewModel.ServiceDesignVersionID;
                            map.EffectiveDate = currentMap.EffectiveDate;

                            this._unitOfWork.Repository<ServiceDesignVersionServiceDefinitionMap>().Insert(map);

                            using (var scope = new TransactionScope())
                            {
                                this._unitOfWork.Save();
                                scope.Complete();

                                IList<ServiceResultItem> item = new List<ServiceResultItem>();
                                item.Add(new ServiceResultItem { Messages = new string[] { servicedef.ServiceDefinitionID.ToString() } });

                                result.Items = item;
                                result.Result = ServiceResultStatus.Success;
                            }

                            UIElement uielement = this._unitOfWork.RepositoryAsync<UIElement>().FindById(serviceDefinition.UIElementID);

                            if (uielement.IsRepeater())
                            {
                                var childElementIDs = this._unitOfWork.RepositoryAsync<UIElement>()
                                                                .Query()
                                                                .Filter(c => c.ParentUIElementID == uielement.UIElementID)
                                                                .Get()
                                                                .Select(c => c.UIElementID)
                                                                .ToList();

                                bool hasDataSource = this._unitOfWork.RepositoryAsync<DataSourceMapping>()
                                                                        .Query()
                                                                        .Filter(c => childElementIDs.Contains(c.UIElementID))
                                                                        .Get()
                                                                        .Any();

                                if (hasDataSource)
                                {
                                    //call function to copy all the repeater elements & store inside the service definition table.
                                    this.AddRepeaterChildElements(serviceDefinition.UIElementID, servicedef.ServiceDefinitionID, currentMap.ServiceDesignVersion.FormDesignVersionID, currentMap.ServiceDesignVersion.ServiceDesignID, currentMap.ServiceDesignVersion.ServiceDesignVersionID, currentMap.ServiceDesignVersion.EffectiveDate, serviceDefinition.TenantID, userName);
                                }
                                else
                                    this.AddRepeaterKeyElements(serviceDefinition.UIElementID, servicedef.ServiceDefinitionID, currentMap.ServiceDesignVersion.FormDesignVersionID, serviceDefinition.ServiceDesignID, currentMap.ServiceDesignVersionID, currentMap.ServiceDesignVersion.EffectiveDate, serviceDefinition.TenantID, userName);
                            }
                        }
                        else
                        {
                            serviceDefinition.UpdatedBy = userName;
                            serviceDefinition.UpdatedDate = DateTime.Now;
                            serviceDefinition.IsRequired = serviceDefinition.UIElementDataTypeID == 2 ? serviceDefinition.IsRequired : viewModel.IsRequired;
                            serviceDefinition.DisplayName = serviceDefinition.UIElementDataTypeID == 2 ? serviceDefinition.DisplayName : viewModel.DisplayName;
                            serviceDefinition.UIElementDataTypeID = viewModel.UIElementDataTypeID == 0 ? serviceDefinition.UIElementDataTypeID : viewModel.UIElementDataTypeID;

                            this._unitOfWork.Repository<ServiceDefinition>().Update(serviceDefinition);

                            this._unitOfWork.Save();

                            IList<ServiceResultItem> item = new List<ServiceResultItem>();
                            item.Add(new ServiceResultItem { Messages = new string[] { serviceDefinition.ServiceDefinitionID.ToString() } });

                            result.Items = item;
                            result.Result = ServiceResultStatus.Success;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
                result = ex.ExceptionMessages();
            }
            return result;
        }

        public ServiceDefinitionViewModel GetServiceDefinitionDetails(int tenantID, int serviceDefinitionID)
        {
            ServiceDefinitionViewModel viewModel = new ServiceDefinitionViewModel();
            try
            {
                ServiceDefinition serviceDef = this._unitOfWork.Repository<ServiceDefinition>()
                                                                    .Query()
                                                                    .Filter(c => c.ServiceDefinitionID == serviceDefinitionID)
                                                                    .Include(c => c.UIElement)
                                                                    .Include(c => c.UIElementType)
                                                                    .Get()
                                                                    .SingleOrDefault();
                if (serviceDef != null)
                {
                    viewModel.ServiceDefinitionID = serviceDef.ServiceDefinitionID;
                    viewModel.UIElementFullPath = serviceDef.UIElementFullPath;
                    viewModel.UIElementDataTypeID = serviceDef.UIElementDataTypeID;
                    viewModel.UIElementTypeID = serviceDef.UIElementTypeID;
                    viewModel.UIElementType = serviceDef.UIElementType.DisplayText;
                    viewModel.UIElementID = serviceDef.UIElementID;
                    viewModel.Label = serviceDef.UIElement.Label;
                    viewModel.DisplayName = serviceDef.DisplayName;
                    viewModel.ParentServiceDefinitionID = serviceDef.ParentServiceDefinitionID;
                    viewModel.Sequence = serviceDef.Sequence;
                    viewModel.IsKey = serviceDef.IsKey;
                    viewModel.IsRequired = serviceDef.IsRequired;
                    viewModel.ServiceDesignID = serviceDef.ServiceDesignID;
                    viewModel.TenantID = serviceDef.TenantID;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
            }
            return viewModel;
        }

        public IEnumerable<ServiceParameterRowModel> GetServiceParameterList(int tenantID, int serviceDesignID, int serviceDesignVersionID)
        {
            IList<ServiceParameterRowModel> serviceParameterList = null;
            try
            {
                var serviceParameters = this._unitOfWork.Repository<ServiceParameter>()
                                                           .Query()
                                                           .Filter(c => c.TenantID == tenantID && c.ServiceDesignVersionID == serviceDesignVersionID
                                                                       && c.ServiceDesignID == serviceDesignID && c.IsActive == true)
                                                           .Include(c => c.ApplicationDataType)
                                                           .Include(c => c.ServiceDesignVersion)
                                                           .OrderBy(c => c.OrderBy(d => d.Name))
                                                           .Get()
                                                           .ToList();

                serviceParameterList = (from c in serviceParameters
                                        select new ServiceParameterRowModel
                                        {
                                            ServiceParameterID = c.ServiceParameterID,
                                            Name = c.Name,
                                            IsRequired = c.IsRequired,
                                            ServiceDesignID = c.ServiceDesignID,
                                            ServiceDesignVersionID = c.ServiceDesignVersionID,
                                            DataTypeID = c.DataTypeID,
                                            DataTypeName = c.ApplicationDataType.DisplayText,
                                            UIElementID = c.UIElementID ?? 0,
                                            UIElementFullPath = GetFormattedFullPath(c.UIElementID, c.ServiceDesignVersion.FormDesignVersionID)
                                        }).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return serviceParameterList;
        }

        public ServiceResult AddServiceParameter(int tenantID, int serviceDesignID, int serviceDesignVersionID, int dataTypeID, string name, bool isRequired, int uielementID, string userName)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                ServiceDesignVersion version = this._unitOfWork.Repository<ServiceDesignVersion>().FindById(serviceDesignVersionID);
                if (version != null)
                {
                    ServiceParameter parameter = new ServiceParameter();
                    parameter.DataTypeID = dataTypeID;
                    parameter.Name = name;
                    parameter.GeneratedName = GetGeneratedName(name);
                    parameter.IsRequired = isRequired;
                    parameter.ServiceDesignID = serviceDesignID;
                    parameter.ServiceDesignVersionID = serviceDesignVersionID;
                    parameter.UIElementID = uielementID;
                    parameter.UIElementFullPath = GetUIElementFullPath(uielementID, version.FormDesignVersionID);
                    parameter.IsActive = true;
                    parameter.AddedBy = userName;
                    parameter.AddedDate = DateTime.Now;
                    parameter.TenantID = tenantID;

                    this._unitOfWork.Repository<ServiceParameter>().Insert(parameter);
                    this._unitOfWork.Save();

                    result.Result = ServiceResultStatus.Success;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
                result = ex.ExceptionMessages();
            }
            return result;
        }

        public ServiceResult AddDefaultServiceParameter(int tenantID, int serviceDesignID, int serviceDesignVersionID, int dataTypeID, string name, bool isRequired, string userName)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                ServiceDesignVersion version = this._unitOfWork.Repository<ServiceDesignVersion>().FindById(serviceDesignVersionID);
                if (version != null)
                {
                    ServiceParameter parameter = new ServiceParameter();
                    parameter.DataTypeID = dataTypeID;
                    parameter.Name = name;
                    parameter.GeneratedName = GetGeneratedName(name);
                    parameter.IsRequired = isRequired;
                    parameter.ServiceDesignID = serviceDesignID;
                    parameter.ServiceDesignVersionID = serviceDesignVersionID;
                    parameter.UIElementFullPath = "FormInstanceID";
                    parameter.IsActive = true;
                    parameter.AddedBy = userName;
                    parameter.AddedDate = DateTime.Now;
                    parameter.TenantID = tenantID;

                    this._unitOfWork.Repository<ServiceParameter>().Insert(parameter);
                    this._unitOfWork.Save();

                    result.Result = ServiceResultStatus.Success;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
                result = ex.ExceptionMessages();
            }
            return result;
        }

        public ServiceResult UpdateServiceParameter(int tenantID, int serviceParameterID, int serviceDesignID, int serviceDesignVersionID, int dataTypeID, string name, bool isRequired, int uielementID, string userName)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                ServiceParameter parameter = this._unitOfWork.Repository<ServiceParameter>()
                                                                .FindById(serviceParameterID);

                ServiceDesignVersion version = this._unitOfWork.Repository<ServiceDesignVersion>().FindById(serviceDesignVersionID);

                if (parameter != null && version != null)
                {
                    parameter.DataTypeID = dataTypeID;
                    parameter.Name = name;
                    parameter.GeneratedName = GetGeneratedName(name);
                    parameter.IsRequired = isRequired;
                    parameter.UIElementID = uielementID;
                    parameter.UIElementFullPath = GetUIElementFullPath(uielementID, version.FormDesignVersionID);
                    parameter.UpdatedBy = userName;
                    parameter.UpdatedDate = DateTime.Now;

                    this._unitOfWork.Repository<ServiceParameter>().Update(parameter);
                    this._unitOfWork.Save();

                    result.Result = ServiceResultStatus.Success;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
                result = ex.ExceptionMessages();
            }
            return result;
        }

        public ServiceResult DeleteServiceParameter(int tenantID, int serviceParameterID, int serviceDesignID, int serviceDesignVersionID, string userName)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                ServiceParameter parameter = this._unitOfWork.Repository<ServiceParameter>()
                                                                .Query()
                                                                .Filter(c => c.ServiceParameterID == serviceParameterID && c.ServiceDesignID == serviceDesignID
                                                                                && c.ServiceDesignVersionID == serviceDesignVersionID)
                                                                .Get()
                                                                .FirstOrDefault();

                if (parameter != null)
                {
                    parameter.IsActive = false;
                    parameter.UpdatedBy = userName;
                    parameter.UpdatedDate = DateTime.Now;

                    this._unitOfWork.Repository<ServiceParameter>().Update(parameter);
                    this._unitOfWork.Save();

                    result.Result = ServiceResultStatus.Success;
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw ex;
                result = ex.ExceptionMessages();
            }
            return result;
        }

        public IEnumerable<UIElementModel> GetUIElementList(int tenantID, int formDesignVersionID, int formDesignID)
        {
            IList<UIElementModel> uielementList = new List<UIElementModel>();
            try
            {
                var elementList = this._unitOfWork.RepositoryAsync<UIElement>().GetUIElementListForFormDesignVersion(tenantID, formDesignVersionID).ToList();

                if (elementList.Count() > 0)
                {
                    string[] excludeList = new string[] { "Tab", "Repater", "Section", "[Blank]" };
                    uielementList = (from i in elementList
                                     select new UIElementModel
                                     {
                                         DataType = i.ApplicationDataType.ApplicationDataTypeName,
                                         ElementType = GetElementType(i),
                                         Label = i.Label == null ? "[Blank]" : i.Label,
                                         UIElementID = i.UIElementID,
                                         ParentUIElementID = i.ParentUIElementID ?? 0,
                                         UIelementFullPath = GetFormattedFullPath(i, elementList)
                                     }).ToList().Where(c => !excludeList.Contains(c.ElementType)).ToList();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return uielementList;
        }
        #endregion Public Methods

        #region Private Methods
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

        private int GetElementTypeID(UIElement uielement)
        {
            int uIElementType = 0;
            try
            {
                if (uielement is RadioButtonUIElement)
                {
                    uIElementType = (uielement as RadioButtonUIElement).UIElementTypeID;
                }
                else if (uielement is CheckBoxUIElement)
                {
                    uIElementType = (uielement as CheckBoxUIElement).UIElementTypeID;
                }
                else if (uielement is TextBoxUIElement)
                {
                    uIElementType = (uielement as TextBoxUIElement).UIElementTypeID;
                }
                else if (uielement is DropDownUIElement)
                {
                    uIElementType = (uielement as DropDownUIElement).UIElementTypeID;
                }
                else if (uielement is CalendarUIElement)
                {
                    uIElementType = (uielement as CalendarUIElement).UIElementTypeID;
                }
                else if (uielement is SectionUIElement)
                {
                    uIElementType = (uielement as SectionUIElement).UIElementTypeID;
                }
                else if (uielement is RepeaterUIElement)
                {
                    uIElementType = (uielement as RepeaterUIElement).UIElementTypeID;
                }
                else if (uielement is TabUIElement)
                {
                    uIElementType = (uielement as TabUIElement).UIElementTypeID;
                }
                else
                {
                    throw new NotSupportedException("UIElementType ID");
                }

            }
            catch (Exception ex)
            {
                throw;
            }
            return uIElementType;
        }

        private int GetRowLevel(int? parentID, List<ServiceDefinition> serviceDefinitionList)
        {
            int level = 0;
            try
            {
                while (parentID != null)
                {
                    level++;
                    var result = from element in serviceDefinitionList
                                 where element.ServiceDefinitionID == parentID
                                 select element;

                    parentID = result.Single().ParentServiceDefinitionID;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return level;
        }

        private bool IsLeafRow(int? serviceDefinitionID, List<ServiceDefinition> serviceDefinitionList)
        {
            try
            {
                foreach (ServiceDefinition element in serviceDefinitionList)
                {
                    if (element.ParentServiceDefinitionID == serviceDefinitionID)
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

        private string GetUIElementFullPath(int uielementID, int formDesignVersionID)
        {
            string fullName = "";
            try
            {
                IList<UIElement> formElementList = (from u in this._unitOfWork.RepositoryAsync<UIElement>()
                                                                .Query()
                                                                .Get()
                                                    join fd in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>()
                                                                            .Query()
                                                                            .Get()
                                                    on u.UIElementID equals fd.UIElementID
                                                    where fd.FormDesignVersionID == formDesignVersionID
                                                    select u).ToList();

                UIElement element = (from elem in formElementList
                                     where elem.UIElementID == uielementID
                                     select elem).FirstOrDefault();
                if (element != null)
                {
                    fullName = GetFullPath(element, formElementList);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return fullName;
        }

        public string GetFullPath(UIElement element, IEnumerable<UIElement> formElementList)
        {
            string fullName = "";
            try
            {
                int currentElementID = element.UIElementID;
                int parentUIElementID = element.ParentUIElementID.HasValue ? element.ParentUIElementID.Value : 0;
                fullName = element.GeneratedName;
                while (parentUIElementID > 0)
                {
                    element = (from elem in formElementList
                               where elem.UIElementID == parentUIElementID
                               select elem).FirstOrDefault();
                    parentUIElementID = element.ParentUIElementID.HasValue ? element.ParentUIElementID.Value : 0;
                    if (parentUIElementID > 0)
                    {
                        fullName = element.GeneratedName + "." + fullName;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return fullName;
        }

        private string GetFormattedFullPath(int? uielementID, int formDesignVersionID)
        {
            string fullName = "";
            try
            {
                if (uielementID.HasValue)
                {
                    IList<UIElement> formElementList = (from u in this._unitOfWork.RepositoryAsync<UIElement>()
                                                                    .Query()
                                                                    .Get()
                                                        join fd in this._unitOfWork.RepositoryAsync<FormDesignVersionUIElementMap>()
                                                                                .Query()
                                                                                .Get()
                                                        on u.UIElementID equals fd.UIElementID
                                                        where fd.FormDesignVersionID == formDesignVersionID
                                                        select u).ToList();

                    UIElement element = (from elem in formElementList
                                         where elem.UIElementID == uielementID
                                         select elem).FirstOrDefault();
                    if (element != null)
                    {
                        fullName = GetFormattedFullPath(element, formElementList);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return fullName;
        }

        public string GetFormattedFullPath(UIElement element, IEnumerable<UIElement> formElementList)
        {
            string fullName = "";
            try
            {
                int currentElementID = element.UIElementID;
                int parentUIElementID = element.ParentUIElementID.HasValue ? element.ParentUIElementID.Value : 0;
                fullName = element.Label;
                while (parentUIElementID > 0)
                {
                    element = (from elem in formElementList
                               where elem.UIElementID == parentUIElementID
                               select elem).FirstOrDefault();
                    parentUIElementID = element.ParentUIElementID.HasValue ? element.ParentUIElementID.Value : 0;
                    if (parentUIElementID > 0)
                    {
                        fullName = element.Label + "=>" + fullName;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return fullName;
        }

        private string GetGeneratedName(string name)
        {
            string generatedName = "";
            if (!String.IsNullOrEmpty(name))
            {
                Regex regex = new Regex("[^a-zA-Z0-9]");
                generatedName = regex.Replace(name, String.Empty);
                if (generatedName.Length > 70)
                {
                    generatedName = generatedName.Substring(0, 70);
                }

                Regex checkDigits = new Regex("[^0-9]");

                //if Label contains only numeric characters, this will append a character at the beginning.
                if (!checkDigits.IsMatch(name, 0))
                {
                    generatedName = "a" + generatedName;
                }
            }
            return generatedName;
        }
        #endregion Private Methods

    }
}
