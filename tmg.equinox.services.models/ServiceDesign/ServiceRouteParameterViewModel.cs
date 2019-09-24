using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.ServiceDesign
{
    public class ServiceRouteParameterViewModel
    {
        public string ParameterName { get; set; }
        public string DataType { get; set; }
        public bool IsRequired { get; set; }
        public string UIElementFullPath { get; set; }
    }
}
