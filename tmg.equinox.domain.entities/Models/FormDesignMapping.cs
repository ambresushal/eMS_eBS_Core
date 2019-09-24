using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class FormDesignMapping : Entity
    {
        public int FormDesignMapID { get; set; }
        public int AnchorDesignID { get; set; }
        public int TargetDesignID { get; set; }
    }
}
