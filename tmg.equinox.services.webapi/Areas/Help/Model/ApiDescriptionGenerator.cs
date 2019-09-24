using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels;
using tmg.equinox.applicationservices.viewmodels.ServiceDesign;
using tmg.equinox.dependencyresolution;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.services.webapi.Framework.Routing;
using tmg.equinox.services.webapi.Models;
using System.Web.Mvc; 

namespace tmg.equinox.services.webapi.Areas.Help.Model
{
    public class ApiDescriptionGenerator
    {
        private IServiceDefinitionService _serviceDefinitionService { get; set; }
        private IServiceDesignService _serviceDesignService { get; set; }

        public ApiDescriptionGenerator()
        {
            _serviceDefinitionService = UnityConfig.Resolve<IServiceDefinitionService>();
            _serviceDesignService = UnityConfig.Resolve<IServiceDesignService>();
        }

        public IList<ApiDescriptor> GetDecscriptor()
        {
            IList<ApiDescriptor> apiDescriptorList = new List<ApiDescriptor>();
            try
            {
                RouteBuilder builder = new RouteBuilder();
                IList<RouteViewModel> routeList = builder.Build();

                apiDescriptorList = (from r in routeList
                                     select new ApiDescriptor
                                     {
                                         ServiceDesignVersionID = r.ServiceDesignVersionID,
                                         ID = r.RouteName,
                                         RelativePath = r.Route,
                                         HttpMethod = System.Net.Http.HttpMethod.Get,
                                         Parameters = (from p in r.RouteParameters
                                                       select new ApiDescriptorParameter
                                                       {
                                                           ParameterName = p.ParameterName,
                                                           ParameterType = p.DataType,
                                                           IsRequired = p.IsRequired
                                                       }).ToList()
                                     }).ToList();
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return apiDescriptorList;
        }

        public ApiDescriptor GetDescription(string apiid)
        {
            ApiDescriptor descriptor = new ApiDescriptor();
            try
            {
                descriptor = GetDecscriptor()
                                .Where(c => c.ID == apiid)
                                .FirstOrDefault();

                ServiceRouteViewModel viewModel = this._serviceDesignService.GetServiceDesignRouteList(1, descriptor.ServiceDesignVersionID);

                if (viewModel != null)
                {
                    ServiceDesignVersionDetail detail = this._serviceDesignService.GetServiceDesignVersionDetail(1, descriptor.ServiceDesignVersionID);

                    if (detail != null)
                    {
                        descriptor.RequestUrl = RouteUrlConstructor.Create(detail.VersionNumber, detail.FormDesignName, detail.ServiceMethodName) + RouteUrlConstructor.CreateParameters(viewModel.ServiceParameterList);
                        descriptor.ResponseType = detail.ResponseType;
                        string json = detail.GetJsonDataObject();

                        if (detail.ResponseType == ResponseType.Json)
                        {
                            descriptor.Output = JObject.Parse(json).ToString();
                        }
                        else
                        {
                            XDocument document = JsonConvert.DeserializeXNode(json, "root");
                            descriptor.Output = XDocument.Parse(document.ToString()).ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                bool reThrow = ExceptionPolicyWrapper.HandleException(ex, ExceptionPolicies.ExceptionShielding);
                if (reThrow)
                    throw;
            }
            return descriptor;
        }
    }
}