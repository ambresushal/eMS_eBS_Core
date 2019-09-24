using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.Consortium
{
    public class ConsortiumViewModel : ViewModelBase
    {
        public int? ConsortiumID { get; set; }
        public string ConsortiumName { get; set; }
        public bool IsActive { get; set; }
    }
}
