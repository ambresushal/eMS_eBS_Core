using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.ServiceDesign;
using tmg.equinox.applicationservices.viewmodels.ServiceDesign.ServiceDesignVersion;
using tmg.equinox.dependencyresolution;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.services.webapi.Framework;
using tmg.equinox.services.webapi.Models;
using tmg.equinox.applicationservices;
using tmg.equinox.applicationservices.viewmodels;

namespace tmg.equinox.services.webapi.Framework
{
    public class GenericApiResponseBuilder : IApiResponseBuilder
    {
        string _formData;
        ServiceDesignVersionDetail _detail;
        ServiceRequestModel _viewModel;
        StringBuilder jsonBuilder;

        private IServiceDesignService _serviceDesignService { get; set; }
        private IFolderVersionServices _folderVersionService { get; set; }
        private IServiceRequestHandlerService _serviceRequestHandlerService { get; set; }

        public GenericApiResponseBuilder()
        {
        }

        public ServiceResponse GetResponse(ServiceRequestModel model)
        {
            ServiceResponse response = new ServiceResponse();
            try
            {
                if (model == null)
                {
                    throw new NotImplementedException("ServiceRequestModel");
                }

                this._viewModel = model;

                Initialize();
                IDictionary<int, string> formDataList = _serviceRequestHandlerService.GetFormInstanceData(_viewModel.ServiceDesignVersionID, _detail.FormDesignVersionID, _viewModel.SearchParameterList, _viewModel.SearchParametersDictionary);
                ResponseBuilder builder = new ResponseBuilder(formDataList, _detail);
                jsonBuilder = builder.BuildResponse();

                ResponseType type = _serviceRequestHandlerService.GetServiceResponseType(_detail.ServiceDesignID);
                response.ResponseType = type;
                response.Response = jsonBuilder.ToString();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return response;
        }

        private void Initialize()
        {
            this._serviceDesignService = UnityConfig.Resolve<IServiceDesignService>();
            this._folderVersionService = UnityConfig.Resolve<IFolderVersionServices>();
            this._serviceRequestHandlerService = UnityConfig.Resolve<IServiceRequestHandlerService>();

            this._detail = _serviceDesignService.GetServiceDesignVersionDetail(_viewModel.TenantID, _viewModel.ServiceDesignVersionID);
            this._formData = _folderVersionService.GetFormInstanceData(_viewModel.TenantID, _viewModel.FormInstanceID);
        }
    }
}