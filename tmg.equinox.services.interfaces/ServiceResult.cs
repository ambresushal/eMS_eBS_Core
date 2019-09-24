using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.interfaces
{
    public class ServiceResult
    {
        public IEnumerable<ServiceResultItem> Items;
        public ServiceResultStatus Result { get; set; }

        #region Constructor
        public ServiceResult()
        {
            //should be initialized with Failure, as this needs to be set as Success if Operation is successful. 
            Result = ServiceResultStatus.Failure;   
            Items = new List<ServiceResultItem>();
        }
        #endregion Constructor
    }
}
