using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class FormInstanceViewImpactLog : Entity
    {
        public int ImpactLoggerID { get; set; }
        public int FormInstanceID { get; set; }
        public int FolderID { get; set; }
        public int FolderVersionID { get; set; }
        public int FormDesignID { get; set; }
        public int FormDesignVersionID { get; set; }
        public int ElementID { get; set; }
        public string ElementName { get; set; }
        public string ElementLabel { get; set; }
        public string ElementPath { get; set; }
        public string ElementPathName { get; set; }
        public string Keys { get; set; }
        public string Description { get; set; }
        public string ImpactedElements { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedLast { get; set; }
        public int DocID { get; set; }
        public virtual FormInstance FormInstance { get; set; }
    }
}
