using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tmg.equinox.applicationservices.interfaces
{
    public class ServiceResultItem
    {
        public string[] Messages {get; set;}
        public ServiceResultStatus Status {get; set;}
    }

    public class ServiceResultResponse : ServiceResultItem
    {
        public new object[] Messages { get; set; }
    }
}
