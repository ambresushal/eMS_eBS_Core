using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels
{
    public class ServiceDesignRowModel : ViewModelBase
    {
        public int ServiceDesignId { get; set; }
        public string ServiceName { get; set; }
        public string ServiceMethodName { get; set; }
        public int TenantID { get; set; }
        public bool DoesReturnAList { get; set; }
    }
}
