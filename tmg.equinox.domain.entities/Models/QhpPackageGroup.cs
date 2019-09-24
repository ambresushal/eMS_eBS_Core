using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class QhpPackageGroup : Entity
    {
        public int ForminstanceID { get; set; }
        public string Hashcode { get; set; }
        public int GroupID { get; set; }
    }
}
