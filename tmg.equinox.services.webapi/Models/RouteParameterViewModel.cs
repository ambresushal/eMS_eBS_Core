using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.services.webapi.Models
{
    public class RouteParameterViewModel
    {
        public string ParameterName { get; set; }
        public string DataType { get; set; }
        public bool IsRequired { get; set; }
    }
}
