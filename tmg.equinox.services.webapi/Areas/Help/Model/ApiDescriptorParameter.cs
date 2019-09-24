using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.services.webapi.Areas.Help.Model
{
    public class ApiDescriptorParameter
    {
        public string ParameterName { get; set; }
        public string ParameterType { get; set; }
        public bool IsRequired { get; set; }

    }
}
