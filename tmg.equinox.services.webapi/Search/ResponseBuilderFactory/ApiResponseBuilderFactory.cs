using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.infrastructure.exceptionhandling;

namespace tmg.equinox.services.webapi.Framework
{
    public static class ApiResponseBuilderFactory
    {
        public static IApiResponseBuilder GetApiResponseBuilder(ApiResponseBuilderType type)
        {
            IApiResponseBuilder responseBuilder = null;
            try
            {
                switch (type)
                {
                    case ApiResponseBuilderType.FormInstanceResponseBuilder:
                        responseBuilder = new FormInstanceApiResponseBuilder();
                        break;
                    case ApiResponseBuilderType.Normal:
                        responseBuilder = new GenericApiResponseBuilder();
                        break;
                    default:
                        throw new NotImplementedException("No such Type as " + type.ToString());
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return responseBuilder;
        }
    }
}