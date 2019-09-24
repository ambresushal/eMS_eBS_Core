using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class DocumentLocation : Entity
    {
        public int DocumentLocationID { get; set; }
        public string DocumentLocationCode { get; set; }
    }
}
