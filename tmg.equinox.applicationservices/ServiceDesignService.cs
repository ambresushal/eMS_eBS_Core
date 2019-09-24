using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml.Linq;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.ServiceDesignDetail;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.ServiceDesign;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.domain.entities.Utility;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.applicationservices
{
    public class ServiceDesignService : IServiceDesignService
    {
        #region Private Members
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        private ILoggingService _loggingService { get; set; }
        private IServiceDefinitionService _serviceDefinitionService { get; set; }
        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor
        public ServiceDesignService(IUnitOfWorkAsync unitOfWork, IServiceDefinitionService serviceDefinitionService)
        {
            this._unitOfWork = unitOfWork;
            this._serviceDefinitionService = serviceDefinitionService;
        }
        #endregion Constructor

        #region Public Methods
        public IEnumerable<ServiceDesignRowModel> GetServiceDesignList(int tenantId)
        {
            IList<ServiceDesignRowModel> serviceDesignList = null;
            try
            {
                serviceDesignList = (from c in this._unitOfWork.RepositoryAsync<ServiceDesign>()
                                                                        .Query()
                                                                        .Filter(c => c.TenantID == tenantId && c.IsActive == true)
                                                                        .Get()
                                     select new ServiceDesignRowModel
                                     {
                                         ServiceDesignId = c.ServiceDesignID,
                                         ServiceName = c.ServiceName,
                                         ServiceMethodName = c.ServiceMethodName,
                                         TenantID = c.TenantID,
                                         DoesReturnAList = c.DoesReturnList
                                     }).ToList();

                if (serviceDesignList.Count() == 0)
                    serviceDesignList = null;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return serviceDesignList;
        }

        public ServiceResult AddServiceDesign(string userName, int tenantId, string serviceName, string serviceMethodName, bool doesReturnAList, bool IsReturnJSON)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                ServiceDesign itemToAdd = new ServiceDesign();
                itemToAdd.ServiceName = serviceName;
                itemToAdd.ServiceMethodName = serviceMethodName;
                itemToAdd.IsActive = true;
                itemToAdd.AddedBy = userName;
                itemToAdd.AddedDate = DateTime.Now;
                itemToAdd.TenantID = tenantId;
                itemToAdd.DoesReturnList = doesReturnAList;
                itemToAdd.IsReturnJSON = IsReturnJSON;

                //Call to repository method to insert record.
                this._unitOfWork.RepositoryAsync<ServiceDesign>().Insert(itemToAdd);
                this._unitOfWork.Save();

                //Return success result
                result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                //Get all the exception/inner exception messages and set the return code to failure
                result = ex.ExceptionMessages();
            }
            return result;
        }

        public ServiceResult UpdateServiceDesign(string userName, int tenantId, int serviceDesignId, string serviceName, string serviceMethodName)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                ServiceDesign itemToUpdate = this._unitOfWork.RepositoryAsync<ServiceDesign>()
                                                               .FindById(serviceDesignId);

                if (itemToUpdate != null)
                {
                    itemToUpdate.ServiceName = serviceName;
                    itemToUpdate.ServiceMethodName = serviceMethodName;
                    itemToUpdate.UpdatedBy = userName;
                    itemToUpdate.UpdatedDate = DateTime.Now;
                    //Call to repository method to Update record.
                    this._unitOfWork.RepositoryAsync<ServiceDesign>().Update(itemToUpdate);
                    this._unitOfWork.Save();

                    //Return success result
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

        public ServiceResult DeleteServiceDesign(string userName, int tenantId, int serviceDesignId)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                ServiceDesign itemToUpdate = this._unitOfWork.RepositoryAsync<ServiceDesign>()
                                                               .FindById(serviceDesignId);

                if (itemToUpdate != null)
                {
                    itemToUpdate.IsActive = false;
                    itemToUpdate.UpdatedBy = userName;
                    itemToUpdate.UpdatedDate = DateTime.Now;
                    //Call to repository method to Update record.
                    this._unitOfWork.RepositoryAsync<ServiceDesign>().Update(itemToUpdate);
                    this._unitOfWork.Save();

                    //Return success result
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

        public IEnumerable<ServiceDesignVersionRowModel> GetServiceDesignVersionList(int tenantId, int serviceDesignId)
        {
            IList<ServiceDesignVersionRowModel> serviceDesignVersionList = null;
            try
            {
                serviceDesignVersionList = (from c in this._unitOfWork.RepositoryAsync<ServiceDesignVersion>()
                                                                                 .Query()
                                                                                 .Include(c => c.FormDesign)
                                                                                 .Include(c => c.FormDesignVersion)
                                                                                 .Filter(c => c.ServiceDesignID == serviceDesignId && c.IsActive == true)
                                                                                 .OrderBy(c => c.OrderByDescending(d => d.AddedDate))
                                                                                 .Get()
                                            select new ServiceDesignVersionRowModel
                                            {
                                                ServiceDesignVersionId = c.ServiceDesignVersionID,
                                                ServiceDesignId = c.ServiceDesignID,
                                                VersionNumber = c.VersionNumber,
                                                EffectiveDate = c.EffectiveDate,
                                                FormDesignID = c.FormDesignID,
                                                FormDesignName = c.FormDesign != null ? c.FormDesign.FormName : string.Empty,
                                                FormDesignVersionID = c.FormDesignVersionID,
                                                FormDesignVersionNumber = c.FormDesignVersion != null ? c.FormDesignVersion.VersionNumber : string.Empty,
                                                IsFinalized = c.IsFinalized
                                            }).ToList();
                if (serviceDesignVersionList.Count() == 0)
                    serviceDesignVersionList = null;

            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
            }
            return serviceDesignVersionList;
        }

        public ServiceResult AddServiceDesignVersion(string userName, int tenantId, int serviceDesignId, DateTime effectiveDate, string versionNumber, int formDesignID, int formDesignVersionID)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                ServiceDesignVersion itemToAdd = new ServiceDesignVersion();
                int status = Convert.ToInt32(tmg.equinox.domain.entities.Status.InProgress);

                itemToAdd.ServiceDesignID = serviceDesignId;
                itemToAdd.VersionNumber = GetNextVersionNumber(versionNumber, isMajorVersion: true, isAddNewVersion: true);
                itemToAdd.FormDesignID = formDesignID;
                itemToAdd.FormDesignVersionID = formDesignVersionID;
                itemToAdd.IsFinalized = false;
                itemToAdd.IsActive = true;
                itemToAdd.EffectiveDate = effectiveDate;
                itemToAdd.AddedBy = userName;
                itemToAdd.AddedDate = DateTime.Now;
                itemToAdd.TenantID = tenantId;

                //Call to repository method to insert record.
                this._unitOfWork.RepositoryAsync<ServiceDesignVersion>().Insert(itemToAdd);

                ServiceDefinition service = new ServiceDefinition();
                service.UIElementFullPath = string.Empty;
                service.UIElementDataTypeID = Convert.ToInt32(tmg.equinox.domain.entities.ApplicationDataType.STRING);
                service.UIElementTypeID = 8; // 8 - Tab UIElementType
                service.UIElementID = this._unitOfWork.Repository<FormDesignVersionUIElementMap>()
                                                            .Query()
                                                            .Include(c => c.UIElement)
                                                            .Filter(c => c.FormDesignVersionID == formDesignVersionID && c.UIElement.ParentUIElementID == null)
                                                            .Get()
                                                            .Select(c => c.UIElementID)
                                                            .FirstOrDefault();
                service.DisplayName = this._unitOfWork.Repository<ServiceDesign>()
                                                            .Query()
                                                            .Filter(c => c.ServiceDesignID == serviceDesignId)
                                                            .Get()
                                                            .Select(c => c.ServiceMethodName)
                                                            .FirstOrDefault();
                service.ParentServiceDefinitionID = null;
                service.ServiceDesignID = serviceDesignId;
                service.Sequence = 1;
                service.IsKey = false;
                service.IsRequired = false;
                service.AddedBy = userName;
                service.AddedDate = DateTime.Now;
                service.TenantID = tenantId;

                this._unitOfWork.Repository<ServiceDefinition>().Insert(service);

                ServiceDesignVersionServiceDefinitionMap map = new ServiceDesignVersionServiceDefinitionMap();
                map.ServiceDesignVersionID = itemToAdd.ServiceDesignVersionID;
                map.ServiceDefinitionID = service.ServiceDefinitionID;
                map.EffectiveDate = effectiveDate;

                this._unitOfWork.Repository<ServiceDesignVersionServiceDefinitionMap>().Insert(map);

                using (var scope = new TransactionScope())
                {
                    this._unitOfWork.Save();
                    scope.Complete();
                }

                //this._serviceDefinitionService.AddDefaultServiceParameter(tenantId, serviceDesignId, map.ServiceDesignVersionID, 1/*for int data type*/, "FormInstanceID", true, 0, userName);

                //Return success result
                result.Result = ServiceResultStatus.Success;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow) throw ex;
                result = ex.ExceptionMessages();
                result.Result = ServiceResultStatus.Failure;
            }
            return result;
        }

        public ServiceResult UpdateServiceDesignVersion(string userName, int tenantId, int serviceDesignVersionId, int formDesignVersionId, DateTime effectiveDate)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                ServiceDesignVersion itemToUpdate = this._unitOfWork.RepositoryAsync<ServiceDesignVersion>()
                                                               .FindById(serviceDesignVersionId);
                DateTime previuosEffectiveDate = DateTime.Now;

                if (itemToUpdate != null)
                {
                    previuosEffectiveDate = itemToUpdate.EffectiveDate;
                    //itemToUpdate.VersionNumber = versionNumber;
                    itemToUpdate.EffectiveDate = effectiveDate;
                    itemToUpdate.UpdatedBy = userName;
                    itemToUpdate.UpdatedDate = DateTime.Now;

                    if (itemToUpdate.FormDesignVersionID != formDesignVersionId)
                    {
                        itemToUpdate.FormDesignVersionID = formDesignVersionId;
                    }

                    //Call to repository method to Update record.
                    this._unitOfWork.RepositoryAsync<ServiceDesignVersion>().Update(itemToUpdate);

                    //Perservice this only when the new  and the previous dates do not match
                    if (!effectiveDate.ToShortDateString().Equals(previuosEffectiveDate.ToShortDateString()))
                    {
                        //Update  the effective dates  of the  service design version uielement mapping also 
                        IList<ServiceDesignVersionServiceDefinitionMap> allmappingsToUpdate =
                            this._unitOfWork.RepositoryAsync<ServiceDesignVersionServiceDefinitionMap>()
                                                        .Query()
                                                        .Include(inc => inc.ServiceDesignVersion)
                                                        .Filter(c => c.ServiceDesignVersion.ServiceDesignID == itemToUpdate.ServiceDesignID)
                                                        .Get().ToList();
                        IList<ServiceDesignVersionServiceDefinitionMap> mappingsToUpdate =
                            allmappingsToUpdate.Where(c => c.ServiceDesignVersionID == serviceDesignVersionId).ToList();
                        if (mappingsToUpdate != null && mappingsToUpdate.Count() > 0)
                        {
                            foreach (ServiceDesignVersionServiceDefinitionMap map in mappingsToUpdate)
                            {
                                if (!map.EffectiveDate.ToShortDateString().Equals(effectiveDate.ToShortDateString()))
                                {
                                    map.EffectiveDate = effectiveDate;
                                    this._unitOfWork.RepositoryAsync<ServiceDesignVersionServiceDefinitionMap>().Update(map);
                                }
                            }
                        }

                        //Update the Removal Effective Date of all the uielement mappings for all the prior service design version mappings(if effective date of removal matches the updating service design versions effective date -1
                        //to the current effective date -1
                        IList<ServiceDesignVersionServiceDefinitionMap> toUpdateRemovalEffectiveDate =
                            allmappingsToUpdate.Where(c =>
                                                c.ServiceDesignVersionID != serviceDesignVersionId &&
                                                c.ServiceDesignVersion.ServiceDesignID == itemToUpdate.ServiceDesignID).ToList();
                        foreach (ServiceDesignVersionServiceDefinitionMap mappingtoupdate in toUpdateRemovalEffectiveDate)
                        {
                            if (mappingtoupdate.CancelDate.HasValue == true)
                            {
                                if (mappingtoupdate.CancelDate.Value.ToShortDateString().Equals(previuosEffectiveDate.AddDays(-1).ToShortDateString()))
                                {
                                    mappingtoupdate.CancelDate = effectiveDate.AddDays(-1);
                                    this._unitOfWork.RepositoryAsync<ServiceDesignVersionServiceDefinitionMap>().Update(mappingtoupdate);
                                }
                            }
                        }
                    }
                    using (var scope = new TransactionScope())
                    {
                        this._unitOfWork.Save();
                        scope.Complete();
                        result.Result = ServiceResultStatus.Success;
                    }
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

        public ServiceResult FinalizeServiceDesignVersion(string userName, int tenantId, int serviceDesignVersionId)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                ServiceDesignVersion itemToUpdate = this._unitOfWork.RepositoryAsync<ServiceDesignVersion>()
                                                               .Query()
                                                               .Filter(c => c.ServiceDesignVersionID == serviceDesignVersionId)
                                                               .Get()
                                                               .FirstOrDefault();
                if (itemToUpdate != null)
                {
                    itemToUpdate.IsFinalized = true;
                    itemToUpdate.UpdatedBy = userName;
                    itemToUpdate.UpdatedDate = DateTime.Now;
                    itemToUpdate.ServiceDesignData = this.GetServiceDesignVersionDetail(tenantId, serviceDesignVersionId).GetJsonDataObject();

                    //Call to repository method to Update record.
                    this._unitOfWork.RepositoryAsync<ServiceDesignVersion>().Update(itemToUpdate);
                    this._unitOfWork.Save();

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

        public ServiceResult DeleteServiceDesignVersion(string userName, int tenantId, int serviceDesignVersionId, int serviceDesignId)
        {
            throw new NotImplementedException();
        }

        public ServiceResult CopyServiceDesignVersion(string userName, int tenantId, int previousServiceDesignVersionId, int serviceDesignId, DateTime effectiveDate, string versionNumber)
        {
            ServiceResult result = new ServiceResult();
            try
            {
                ServiceDesignVersion copyVersion = this._unitOfWork.Repository<ServiceDesignVersion>().FindById(previousServiceDesignVersionId);
                if (copyVersion != null)
                {
                    ServiceDesignVersion serviceVersion = new ServiceDesignVersion();
                    serviceVersion.ServiceDesignID = serviceDesignId;
                    serviceVersion.TenantID = tenantId;
                    serviceVersion.EffectiveDate = effectiveDate;
                    serviceVersion.AddedBy = userName;
                    serviceVersion.AddedDate = DateTime.Now;
                    serviceVersion.VersionNumber = GetNextVersionNumber(versionNumber, isMajorVersion: true, isAddNewVersion: true);
                    serviceVersion.FormDesignID = copyVersion.FormDesignID;
                    serviceVersion.FormDesignVersionID = copyVersion.FormDesignVersionID;
                    serviceVersion.IsActive = true;

                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(AppSettings.TransactionTimeOutPeriod)))
                    {
                        //Call to repository method to insert record.
                        this._unitOfWork.RepositoryAsync<ServiceDesignVersion>().Insert(serviceVersion);
                        this.CopyServiceDefinitionServiceDesignVersionMap(previousServiceDesignVersionId, effectiveDate, serviceVersion.ServiceDesignVersionID);
                        this.CopyServiceParameters(previousServiceDesignVersionId, serviceVersion.ServiceDesignVersionID, userName);
                        this._unitOfWork.Save();

                        scope.Complete();
                        result.Result = ServiceResultStatus.Success;
                    }

                }
            }
            catch (Exception ex)
            {
                result = ex.ExceptionMessages();
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return result;
        }

        public ServiceDesignVersionDetail GetServiceDesignVersionDetail(int tenantID, int serviceDesignVersionID)
        {
            ServiceDesignVersionDetail detail = new ServiceDesignVersionDetail();
            try
            {
                ServiceDesignVersion version = this._unitOfWork.RepositoryAsync<ServiceDesignVersion>().FindById(serviceDesignVersionID);

                if (version != null)
                {
                    ServiceDesignBuilder builder = new ServiceDesignBuilder(tenantID, version.FormDesignID, version.FormDesignVersionID, version.ServiceDesignID, serviceDesignVersionID, _unitOfWork);
                    detail = builder.BuildServiceDesign();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return detail;
        }

        public IList<ServiceRouteViewModel> GetServiceDesignRouteList(int tenantID)
        {
            IList<ServiceRouteViewModel> serviceDesignList = new List<ServiceRouteViewModel>();
            try
            {
                serviceDesignList = (from s in this._unitOfWork.Repository<ServiceDesignVersion>()
                                                                .Query()
                                                                .Include(c => c.ServiceDetail)
                                                                .Include(c => c.FormDesign)
                                                                .Include(c => c.FormDesignVersion)
                                                                .Include(c => c.ServiceParameters)
                                         //TODO: Mandar - set IsFinalized = true condition when commiting the code.
                                                                .Filter(c => c.IsActive == true)// && c.IsFinalized == true)
                                                                .Get()
                                     select new ServiceRouteViewModel
                                     {
                                         ServiceDesignId = s.ServiceDesignID,
                                         ServiceDesignName = s.ServiceDetail.ServiceName,
                                         ServiceDesignMethodName = s.ServiceDetail.ServiceMethodName,
                                         ServiceDesignVersionId = s.ServiceDesignVersionID,
                                         VersionNumber = s.VersionNumber,
                                         FormDesignID = s.FormDesignID,
                                         FormDesignName = s.FormDesign.FormName,
                                         FormDesignVersionID = s.FormDesignVersionID,
                                         ServiceParameterList = (from c in s.ServiceParameters
                                                                 where c.IsActive == true
                                                                 select new ServiceRouteParameterViewModel
                                                                 {
                                                                     ParameterName = c.GeneratedName,
                                                                     IsRequired = c.IsRequired,
                                                                     DataType = c.ApplicationDataType.ApplicationDataTypeName,
                                                                     UIElementFullPath = c.UIElementFullPath
                                                                 }).ToList()
                                     }).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return serviceDesignList;
        }

        public ServiceRouteViewModel GetServiceDesignRouteList(int tenantID, int serviceDesignVersionID)
        {
            return this.GetServiceDesignRouteList(tenantID).Where(c => c.ServiceDesignVersionId == serviceDesignVersionID).FirstOrDefault();
        }

        public ServiceDesignPreviewViewModel GetServiceDesignPreview(int tenantID, int serviceDesignID, int serviceDesignVersionID)
        {
            ServiceDesignPreviewViewModel preview = new ServiceDesignPreviewViewModel();
            try
            {
                string json = this.GetServiceDesignVersionDetail(tenantID, serviceDesignVersionID).GetJsonDataObject();
                preview.JsonOutput = JObject.Parse(json).ToString();

                XDocument document = JsonConvert.DeserializeXNode(json, "root");
                preview.XmlOutput = XDocument.Parse(document.ToString()).ToString();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return preview;
        }
        #endregion Public Methods

        #region Private Methods
        private string GetNextVersionNumber(string versionNumber, bool isMajorVersion, bool isAddNewVersion)
        {
            string newVersionNumber = "";
            double result;
            Regex re = new Regex(@"^\d{1,3}\.\d{1,2}");
            if (String.IsNullOrEmpty(versionNumber) || re.Match(versionNumber).Length == 0)
            {
                newVersionNumber = "1.0";
            }
            else if (isMajorVersion == false)
            {
                if (isAddNewVersion == true)
                {
                    bool covertedDoubleNumber = double.TryParse(versionNumber, out result);
                    if (covertedDoubleNumber)
                    {
                        newVersionNumber = (result + 0.01).ToString();
                    }
                    else
                    {
                        throw new NotSupportedException("The version Number cannot be converted to double data type");
                    }
                }
                else
                {
                    newVersionNumber = versionNumber;
                }
            }
            else
            {
                bool convertedNumber = double.TryParse(versionNumber, out result);
                if (convertedNumber)
                {
                    newVersionNumber = ((Math.Floor(result) + 1)) + ".0";
                }
                else
                {
                    throw new NotSupportedException("The version Number cannot be converted to double data type");
                }
            }
            return newVersionNumber;
        }

        private void CopyServiceDefinitionServiceDesignVersionMap(int serviceDesignVersionID, DateTime effectiveDate, int newServiceDesignVersionID)
        {
            IList<ServiceDesignVersionServiceDefinitionMap> elementMapList = null;
            elementMapList = this._unitOfWork.RepositoryAsync<ServiceDesignVersionServiceDefinitionMap>()
                                                        .Query()
                                                        .Filter(c => c.ServiceDesignVersionID == serviceDesignVersionID/*&& c.IsActive == true*/)
                                                        .Get()
                                                        .ToList();


            if (elementMapList != null && elementMapList.Count() > 0)
            {
                foreach (ServiceDesignVersionServiceDefinitionMap item in elementMapList)
                {
                    ServiceDesignVersionServiceDefinitionMap map = new ServiceDesignVersionServiceDefinitionMap
                    {
                        EffectiveDate = effectiveDate,
                        ServiceDesignVersionID = newServiceDesignVersionID,
                        ServiceDefinitionID = item.ServiceDefinitionID
                    };

                    this._unitOfWork.RepositoryAsync<ServiceDesignVersionServiceDefinitionMap>().Insert(map);
                }
            }
        }

        private void CopyServiceParameters(int serviceDesignVersionID, int newServiceDesignVersionID, string username)
        {
            IEnumerable<ServiceParameter> paramList = this._unitOfWork.Repository<ServiceParameter>()
                                                                        .Query()
                                                                        .Filter(c => c.ServiceDesignVersionID == serviceDesignVersionID && c.IsActive == true)
                                                                        .Get();

            if (paramList != null)
            {
                foreach (var item in paramList)
                {
                    ServiceParameter parameter = new ServiceParameter
                    {
                        Name = item.Name,
                        GeneratedName = item.GeneratedName,
                        DataTypeID = item.DataTypeID,
                        IsRequired = item.IsRequired,
                        ServiceDesignID = item.ServiceDesignID,
                        ServiceDesignVersionID = newServiceDesignVersionID,
                        AddedBy = username,
                        AddedDate = DateTime.Now,
                        IsActive = item.IsActive,
                        TenantID = item.TenantID,
                        UIElementID = item.UIElementID,
                        UIElementFullPath = item.UIElementFullPath,
                    };

                    this._unitOfWork.Repository<ServiceParameter>().Insert(parameter);
                }
            }
        }
        #endregion Private Methods
    }
}
