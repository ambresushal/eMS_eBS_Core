using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class ElementLookIn : Entity
    {
        public int UIElementID { get; set; }
        public string Label { get; set; }
        public string GeneratedName { get; set; }
    }
}
