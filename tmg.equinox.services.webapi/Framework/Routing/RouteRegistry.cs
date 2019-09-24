using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Http.Routing.Constraints;
using System.Web.Routing;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.services.webapi.Models;

namespace tmg.equinox.services.webapi.Framework.Routing
{
    public class RouteRegistry : IRouteRegistry
    {
        #region Private Members

        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor
        public void RegisterRoutes(HttpRouteCollection routes)
        {
            try
            {

                RouteBuilder builder = new RouteBuilder();
                IList<RouteViewModel> routeList = builder.Build();

                foreach (var route in routeList.OrderByDescending(c => c.RouteParameters.Count()))
                {
                    if (!routes.Any(c => c.RouteTemplate == route.Route))
                    {
                        var routedefaults = new Dictionary<string, object>();
                        routedefaults.Add("controller", "Product");
                        routedefaults.Add("action", "Get");
                        routedefaults.Add("serviceDesignVersionID", route.ServiceDesignVersionID);
                        //routedefaults.Add("formInstanceID", 0);

                        var routeconstraints = new Dictionary<string, object>();
                        //routeconstraints.Add("formInstanceID", new IntRouteConstraint());

                        foreach (var parameter in route.RouteParameters.OrderByDescending(c => c.IsRequired))
                        {
                            if (!routedefaults.Any(c => c.Key.ToLower() == parameter.ParameterName.ToLower()))
                            {
                                if (parameter.IsRequired)
                                    routedefaults.Add(parameter.ParameterName, RouteParameter.Optional);
                                else
                                    routedefaults.Add(parameter.ParameterName, string.Empty);

                                route.Route += "/{" + parameter.ParameterName + "}";
                            }

                            //if (!routeconstraints.Any(c => c.Key.ToLower() == parameter.ParameterName.ToLower()))
                            //{
                            //    IHttpRouteConstraint constraint = GetRouteConstraint(parameter.DataType);
                            //    if (constraint != null)
                            //    {
                            //        routeconstraints.Add(parameter.ParameterName, constraint);
                            //    }
                            //}
                        }

                        routes.MapHttpRoute(
                                        name: route.RouteName,
                                        routeTemplate: route.Route.Replace(@"//","/"),
                                        defaults: routedefaults,
                                        constraints: routeconstraints
                                    );
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
        }
        #endregion Constructor

        #region Public Methods

        #endregion Public Methods

        #region Private Methods
        private void RegisterDefaultRoutes(HttpRouteCollection routes)
        {

        }

        private IHttpRouteConstraint GetRouteConstraint(string datatype)
        {
            IHttpRouteConstraint constraint = null;
            try
            {
                switch (datatype)
                {
                    case "int":
                        constraint = new IntRouteConstraint();
                        break;
                    case "date":
                        constraint = new DateTimeRouteConstraint();
                        break;
                    case "string":
                        constraint = new LengthRouteConstraint(100);
                        break;
                    case "float":
                        constraint = new DoubleRouteConstraint();
                        break;
                    case "bool":
                        constraint = new BoolRouteConstraint();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return constraint;
        }
        #endregion Private Methods
    }
}