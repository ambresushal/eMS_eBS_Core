using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using tmg.equinox.applicationservices.viewmodels.ServiceDesign;

namespace tmg.equinox.services.webapi
{
    public class RouteUrlConstructor
    {
        public static string Create(string versionNumber, string formDesignName, string serviceDesignMethodName)
        {
            return "api" + "/" + "V" + versionNumber.Substring(0, versionNumber.IndexOf(".")) + "/" + formDesignName + "/"
                                            + serviceDesignMethodName + "/";
        }

        public static string CreateParameters(List<ServiceRouteParameterViewModel> serviceParameterList)
        {
            StringBuilder parameterString = new StringBuilder();
            for (int i = 0; i < serviceParameterList.Count(); i++)
            {
                if (serviceParameterList[i].IsRequired)
                {
                    parameterString.Append("{" + serviceParameterList[i].ParameterName + "}");
                }
                else
                {
                    parameterString.Append("[" + serviceParameterList[i].ParameterName + "]");
                }

                if (i < serviceParameterList.Count() - 1)
                {
                    parameterString.Append("/");
                }
            }
            return parameterString.ToString();
        }
    }
}