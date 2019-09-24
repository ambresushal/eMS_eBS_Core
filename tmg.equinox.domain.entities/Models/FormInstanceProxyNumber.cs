using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class FormInstanceProxyNumber : Entity
    {
        public int ProxyNumberID { get; set; }
        public string ProxyNumber { get; set; }
        public int? FormInstanceID { get; set; }
        public bool IsUsed { get; set; }

    }
}
