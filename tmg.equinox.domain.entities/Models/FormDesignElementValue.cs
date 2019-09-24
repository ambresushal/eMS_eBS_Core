using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class FormDesignElementValue : Entity
    {

        public int FormDesignElementValueID { get; set; }
        public int GlobalUpdateID { get; set; }
        public int FormDesignID { get; set; }
        public int FormDesignVersionID { get; set; }
        public int UIElementID { get; set; }
        public string ElementFullPath { get; set; }
        public bool IsUpdated { get; set; }
        public string NewValue { get; set; }
        public string ElementHeaderName { get; set; }
        public DateTime AddedDate { get; set; }
        public String AddedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public virtual FormDesign FormDesign { get; set; }
        public virtual FormDesignVersion FormDesignVersion { get; set; }
        public virtual GlobalUpdate GlobalUpdate { get; set; }
        public virtual UIElement UIElement { get; set; }
    }
}
