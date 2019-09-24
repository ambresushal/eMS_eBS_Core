using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.ServiceDesignDetail;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.ServiceDesign;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.repository.interfaces;

namespace tmg.equinox.applicationservices
{
    public class ServiceRequestHandlerService : IServiceRequestHandlerService
    {
        #region Private Members
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor
        public ServiceRequestHandlerService(IUnitOfWorkAsync unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        #endregion Constructor

        #region Public Methods
        public bool ValidateFormInstance(int formInstanceID, int serviceDesignVersionID, int formDesignVersionID)
        {
            bool isValid = false;
            try
            {
                IList<ServiceResultItem> resultItemList = new List<ServiceResultItem>();

                isValid = this._unitOfWork.Repository<FormInstance>()
                                                            .Query()
                                                            .Filter(c => c.FormInstanceID == formInstanceID && c.IsActive == true)
                                                            .Get()
                                                            .Any();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return isValid;
        }

        public bool CheckFormInstanceFormDesignID(int formInstanceID, int formDesignVersionID)
        {
            bool isValid = false;
            try
            {
                int? formDesignID = this._unitOfWork.Repository<FormDesignVersion>()
                                                    .Query()
                                                    .Filter(c => c.FormDesignVersionID == formDesignVersionID)
                                                    .Get()
                                                    .Select(c => c.FormDesignID)
                                                    .FirstOrDefault();
                if (formDesignID.HasValue)
                {
                    isValid = this._unitOfWork.Repository<FormInstance>()
                                                                .Query()
                                                                .Filter(c => c.FormInstanceID == formInstanceID && c.IsActive == true && c.FormDesignID == formDesignID.Value)
                                                                .Get()
                                                                .Any();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return isValid;
        }

        public IDictionary<int, string> GetFormInstanceData(int serviceDesignVersionID, int formDesignVersionID, IList<ServiceRouteParameterViewModel> searchParametersList, IDictionary<string, object> searchValues)
        {
            IDictionary<int, string> formInstanceData = new Dictionary<int, string>();
            try
            {
                IList<int> formInstanceIDList = this.SearchFormInstance(serviceDesignVersionID, formDesignVersionID, searchParametersList, searchValues);

                formInstanceData = this._unitOfWork.Repository<FormInstanceDataMap>()
                                                        .Query()
                                                        .Filter(c => formInstanceIDList.Contains(c.FormInstanceID))
                                                        .Get()
                                                        .Select(c => new { c.FormInstanceID, c.FormData })
                                                        .ToDictionary(c => c.FormInstanceID, c => c.FormData);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return formInstanceData;
        }

        public ResponseType GetServiceResponseType(int serviceDesignID)
        {
            ResponseType type = ResponseType.Json;
            try
            {
                bool IsReturnJSON = this._unitOfWork.Repository<ServiceDesign>()
                                                    .Query()
                                                    .Filter(c => c.ServiceDesignID == serviceDesignID)
                                                    .Get()
                                                    .Select(c => c.IsReturnJSON)
                                                    .FirstOrDefault();

                if (!IsReturnJSON)
                    type = ResponseType.Xml;
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return type;
        }
        #endregion Public Methods

        #region Private Methods
        private IList<int> SearchFormInstance(int serviceDesignVersionID, int formDesignVersionID, IList<ServiceRouteParameterViewModel> searchParametersList, IDictionary<string, object> searchValues)
        {
            IList<int> formInstanceList = new List<int>();
            try
            {
                int? formDesignID = this._unitOfWork.Repository<FormDesignVersion>()
                                                    .Query()
                                                    .Filter(c => c.FormDesignVersionID == formDesignVersionID)
                                                    .Get()
                                                    .Select(c => c.FormDesignID)
                                                    .FirstOrDefault();
                if (formDesignID.HasValue)
                {
                    IList<FormInstanceDataMap> formDataMapList = (from c in this._unitOfWork.Repository<FormInstanceDataMap>()
                                                                                                .Query()
                                                                                                .Include(c => c.FormInstance)
                                                                                                .Filter(c => c.FormInstance.FormDesignID == formDesignID.Value
                                                                                                                && c.FormInstance.IsActive == true)
                                                                                                .Get()
                                                                  select c).ToList();
                    JsonParser parser = new JsonParser();
                    foreach (var item in formDataMapList)
                    {
                        bool hasMatch = false;
                        foreach (var param in searchParametersList)
                        {
                            string searchValue = searchValues.Where(c => c.Key == param.ParameterName)
                                                                .Select(c => c.Value)
                                                                .FirstOrDefault()
                                                                .ToString();
                            hasMatch = parser.HasValue(item.FormData, param.UIElementFullPath, searchValue);
                            if (hasMatch)
                                formInstanceList.Add(item.FormInstanceID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return formInstanceList;
        }
        #endregion Private Methods
    }
}
