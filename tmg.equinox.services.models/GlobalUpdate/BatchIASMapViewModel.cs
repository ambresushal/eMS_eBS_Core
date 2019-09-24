using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.GlobalUpdate
{
    public class BatchIASMapViewModel:ViewModelBase
    {

        #region Instance Properties

        public int BatchIASMapID { get; set; }

        public Guid BatchID { get; set; }

        public int GlobalUpdateID { get; set; }

        #endregion Instance Properties
    }
}
