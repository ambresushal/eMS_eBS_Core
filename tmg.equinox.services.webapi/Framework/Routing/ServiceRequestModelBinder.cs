using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ModelBinding;
using System.Web.Http.ValueProviders;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.ServiceDesign;
using tmg.equinox.dependencyresolution;
using tmg.equinox.services.webapi.Models;

namespace tmg.equinox.services.webapi.Framework.Routing
{
    public class ServiceRequestModelBinder : System.Web.Http.ModelBinding.IModelBinder
    {
        private IServiceDesignService _serviceDesignService { get; set; }

        public ServiceRequestModelBinder()
        {
            _serviceDesignService = UnityConfig.Resolve<IServiceDesignService>();
        }

        public bool BindModel(System.Web.Http.Controllers.HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            var model = (ServiceRequestModel)bindingContext.Model ?? new ServiceRequestModel();

            var hasPrefix = bindingContext.ValueProvider.ContainsPrefix(bindingContext.ModelName);

            var searchPrefix = (hasPrefix) ? bindingContext.ModelName + "." : "";

            int id = 0;
            if (int.TryParse(GetValue(bindingContext, searchPrefix, "FormInstanceID"), out id))
            {
                model.FormInstanceID = id; // <1>
            }

            int sid = 0;
            if (int.TryParse(GetValue(bindingContext, searchPrefix, "ServiceDesignVersionID"), out sid))
            {
                model.ServiceDesignVersionID = sid; // <1>
                if (sid == 0)
                    return false;
            }

            IDictionary<string, bool> validatationResult = ValidateParameters(bindingContext, actionContext, model, searchPrefix);

            //foreach (var item in validatationResult.Where(c => c.Value == false))
            //{
            //    bindingContext.ModelState.AddModelError(item.Key, "Parameter not provided or value type is wrong.");
            //}

            bindingContext.Model = model;

            return true;
        }

        private string GetValue(ModelBindingContext context, string prefix, string key)
        {
            var result = context.ValueProvider.GetValue(prefix + key); // <4>
            return result == null ? null : result.AttemptedValue;
        }

        private IDictionary<string, bool> ValidateParameters(ModelBindingContext bindingContext, System.Web.Http.Controllers.HttpActionContext actionContext, ServiceRequestModel model, string searchPrefix)
        {
            IDictionary<string, bool> validationResult = new Dictionary<string, bool>();
            try
            {
                var routeData = actionContext.ControllerContext.RouteData;
                string[] ingoreKeys = { "servicedesignversionid", "controller", "action" };

                model.SearchParametersDictionary = new Dictionary<string, object>();

                foreach (var item in routeData.Values.Where(c => !ingoreKeys.Contains(c.Key.ToLower())))
                {
                    model.SearchParametersDictionary.Add(item.Key, GetValue(bindingContext, searchPrefix, item.Key));
                }

                ServiceRouteViewModel serviceRoute = _serviceDesignService.GetServiceDesignRouteList(1, model.ServiceDesignVersionID);

                foreach (var parameter in serviceRoute.ServiceParameterList.Where(c => !ingoreKeys.Contains(c.ParameterName.ToLower())))
                {
                    if (model.SearchParametersDictionary.ContainsKey(parameter.ParameterName))
                    {
                        var data = model.SearchParametersDictionary.Where(c => c.Key == parameter.ParameterName).FirstOrDefault();
                        model.SearchParameterList.Add(parameter);

                        bool isValidValue = false;
                        switch (parameter.DataType)
                        {
                            case "int":
                                int intResult = 0;
                                isValidValue = int.TryParse(data.Value.ToString(), out intResult);
                                break;
                            case "date":
                                DateTime dateResult;
                                isValidValue = DateTime.TryParse(data.Value.ToString(), out dateResult);
                                break;
                            case "string":
                                isValidValue = true;
                                break;
                            case "float":
                                float floatResult = 0;
                                isValidValue = float.TryParse(data.Value.ToString(), out floatResult);
                                break;
                            case "bool":
                                bool boolResult = false;
                                isValidValue = bool.TryParse(data.Value.ToString(), out boolResult);
                                break;
                            default:
                                break;
                        }

                        if (!isValidValue)
                        {
                            bindingContext.ModelState.AddModelError(parameter.ParameterName, "Value for " + parameter.ParameterName + " is not a valid value");
                        }
                    }
                    else
                    {
                        if (parameter.IsRequired)
                        {
                            validationResult.Add(parameter.ParameterName, false);
                            bindingContext.ModelState.AddModelError(parameter.ParameterName, "Value not provided for " + parameter.ParameterName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return validationResult;
        }
    }
}