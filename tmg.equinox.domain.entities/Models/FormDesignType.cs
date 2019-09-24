using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class FormDesignType : Entity
    {
        public FormDesignType()
        {
            this.FormDesignVersions = new List<FormDesignVersion>();
        }

        public int FormDesignTypeID { get; set; }
        public string DisplayText { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public virtual ICollection<FormDesignVersion> FormDesignVersions { get; set; }
    }
}
