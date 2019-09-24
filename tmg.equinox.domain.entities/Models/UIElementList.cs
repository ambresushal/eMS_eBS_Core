using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class UIElementList : Entity
    {
        public UIElementList()
        {
        }

        public string Section { get; set; }
        public int? Parent { get; set; }
        public string Element { get; set; }
        public int UIElementID { get; set; }
        public string UIElementName { get; set; }
        public bool IsContainer { get; set; }
    }
}
