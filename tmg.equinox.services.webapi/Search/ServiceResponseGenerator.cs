using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.ServiceDesign;
using tmg.equinox.applicationservices.viewmodels.ServiceDesign.ServiceDesignVersion;
using tmg.equinox.dependencyresolution;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.services.webapi.Models;

namespace tmg.equinox.services.webapi.Framework
{
    public class ServiceResponseGenerator
    {
        ServiceRequestModel _viewModel;
        ServiceResponse _response { get; set; }

        public ServiceResponseGenerator(ServiceRequestModel model)
        {
            this._viewModel = model;
        }

        public ServiceResponse GetResponse()
        {
            try
            {
                IApiResponseBuilder builder = null;
                if (_viewModel.FormInstanceID != 0)
                    builder = ApiResponseBuilderFactory.GetApiResponseBuilder(ApiResponseBuilderType.FormInstanceResponseBuilder);
                else
                    builder = ApiResponseBuilderFactory.GetApiResponseBuilder(ApiResponseBuilderType.Normal);
                _response = builder.GetResponse(_viewModel);
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return _response;
        }
    }
}