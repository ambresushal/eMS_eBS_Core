using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using tmg.equinox.applicationservices;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.ServiceDesign;
using tmg.equinox.applicationservices.viewmodels.ServiceDesign.ServiceDesignVersion;
using tmg.equinox.dependencyresolution;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.services.webapi.Framework;
using tmg.equinox.services.webapi.Models;

namespace tmg.equinox.services.webapi.Framework
{
    public class FormInstanceApiResponseBuilder : IApiResponseBuilder
    {
        #region Private Members
        string _formData;
        ServiceDesignVersionDetail _detail;
        ServiceRequestModel _viewModel;

        private IServiceDesignService _serviceDesignService { get; set; }
        private IFolderVersionServices _folderVersionService { get; set; }
        private IServiceRequestHandlerService _serviceRequestHandlerService { get; set; }
        #endregion Private Members

        #region Constructor
        public FormInstanceApiResponseBuilder()
        {

        }
        #endregion Constructor

        #region Public Methods
        public ServiceResponse GetResponse(ServiceRequestModel model)
        {
            ServiceResponse response = new ServiceResponse();
            try
            {
                StringBuilder jsonBuilder = new StringBuilder();
                if (model == null)
                {
                    throw new NotImplementedException("ServiceRequestModel");
                }

                this._viewModel = model;

                Initialize();

                if (this._viewModel.FormInstanceID == 0)
                {
                    throw new ArgumentNullException("FormInstanceID", string.Format(ExceptionMessages.FormInstanceIDIsNull, _viewModel.FormInstanceID));
                }
                else if (!this._serviceRequestHandlerService.ValidateFormInstance(this._viewModel.FormInstanceID, this._viewModel.ServiceDesignVersionID, this._detail.FormDesignVersionID))
                {
                    throw new ItemNotFoundException(string.Format(ExceptionMessages.FormInstanceIDNotFound, _viewModel.FormInstanceID));
                }
                else if (!this._serviceRequestHandlerService.CheckFormInstanceFormDesignID(this._viewModel.FormInstanceID, this._detail.FormDesignVersionID))
                {
                    throw new InvalidFormInstanceIDException(string.Format(ExceptionMessages.InvalidFormInstanceID, _viewModel.FormInstanceID));
                }
                else
                {
                    ResponseType type = _serviceRequestHandlerService.GetServiceResponseType(_detail.ServiceDesignID);
                    ResponseBuilder builder = new ResponseBuilder(_formData, _detail, _viewModel.FormInstanceID);
                    jsonBuilder = builder.BuildResponse();
                    response.ResponseType = type;
                    response.Response = jsonBuilder.ToString();
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return response;
        }
        #endregion Public Methods

        #region Private Methods
        private void Initialize()
        {
            this._serviceDesignService = UnityConfig.Resolve<IServiceDesignService>();
            this._folderVersionService = UnityConfig.Resolve<IFolderVersionServices>();
            this._serviceRequestHandlerService = UnityConfig.Resolve<IServiceRequestHandlerService>();

            this._detail = _serviceDesignService.GetServiceDesignVersionDetail(_viewModel.TenantID, _viewModel.ServiceDesignVersionID);
            this._formData = _folderVersionService.GetFormInstanceData(_viewModel.TenantID, _viewModel.FormInstanceID);
        }
        #endregion Private Methods
    }
}