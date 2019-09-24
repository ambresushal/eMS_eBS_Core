using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.ServiceDesign;
using tmg.equinox.dependencyresolution;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.services.webapi.Models;

namespace tmg.equinox.services.webapi.Framework.Routing
{
    public class RouteBuilder
    {
        #region Private Members
        private IServiceDesignService _serviceDesignService { get; set; }
        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor
        public RouteBuilder()
        {
            _serviceDesignService = UnityConfig.Resolve<IServiceDesignService>();
            if (_serviceDesignService == null)
            {
                throw new NullReferenceException("_serviceDesignService is null");
            }
        }
        #endregion Constructor

        #region Public Methods
        public IList<RouteViewModel> Build()
        {
            IList<RouteViewModel> routeList = new List<RouteViewModel>();
            try
            {
                IList<ServiceRouteViewModel> serviceDesignRouteList = _serviceDesignService.GetServiceDesignRouteList(1);

                foreach (var item in serviceDesignRouteList)
                {
                    RouteViewModel route = new RouteViewModel();
                    route.RouteName = item.ServiceDesignName.Replace(" ", "") + "" + item.VersionNumber.Replace(".", "") + "" + item.ServiceDesignMethodName;

                    route.Route = RouteUrlConstructor.Create(item.VersionNumber, item.FormDesignName, item.ServiceDesignMethodName);
                    route.ServiceDesignVersionID = item.ServiceDesignVersionId;

                    route.RouteParameters = new List<RouteParameterViewModel>();

                    foreach (var parameter in item.ServiceParameterList)
                    {
                        RouteParameterViewModel routeParameter = new RouteParameterViewModel
                        {
                            ParameterName = parameter.ParameterName,
                            IsRequired = parameter.IsRequired,
                            DataType = parameter.DataType
                        };

                        route.RouteParameters.Add(routeParameter);
                    }
                    routeList.Add(route);
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return routeList;
        }
        #endregion Public Methods

        #region Private Methods

        #endregion Private Methods
    }
}