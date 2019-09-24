using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tmg.equinox.services.webapi.Models
{
    public class RouteViewModel
    {
        public string RouteName { get; set; }
        public string Route { get; set; }
        public int ServiceDesignVersionID { get; set; }
        public List<RouteParameterViewModel> RouteParameters { get; set; }

        public RouteViewModel()
        {
            RouteParameters = new List<RouteParameterViewModel>();
        }
    }
}