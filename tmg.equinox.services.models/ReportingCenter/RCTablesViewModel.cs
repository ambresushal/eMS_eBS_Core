using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.ReportingCenter
{
    public class RCTablesViewModel : ViewModelBase
    {
        public decimal ID { get; set; }
        public string Name { get; set; }
        public string SchemaName { get; set; }       
    }
}
