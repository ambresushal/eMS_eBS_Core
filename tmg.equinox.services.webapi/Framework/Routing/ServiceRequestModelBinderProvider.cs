using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.ModelBinding;
using System.Web.Http.ValueProviders;
using tmg.equinox.services.webapi.Models;

namespace tmg.equinox.services.webapi.Framework.Routing
{
    public class ServiceRequestModelBinderProvider : IModelBinder
    {
        public bool BindModel(System.Web.Http.Controllers.HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType != typeof(ServiceRequestModel))
            {
                return false;
            }

            ValueProviderResult val = bindingContext.ValueProvider.GetValue(
                bindingContext.ModelName);
            if (val == null)
            {
                return false;
            }

            string key = val.RawValue as string;
            if (key == null)
            {
                bindingContext.ModelState.AddModelError(
                    bindingContext.ModelName, "Wrong value type");
                return false;
            }

            ServiceRequestModel result = new ServiceRequestModel();
            //if (_locations.TryGetValue(key, out result) || GeoPoint.TryParse(key, out result))
            {
                bindingContext.Model = result;
                return true;
            }

            bindingContext.ModelState.AddModelError(
                bindingContext.ModelName, "Cannot convert value to Location");
            return false;
        }
    }
}
