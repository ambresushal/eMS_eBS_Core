using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.qhp.entities.Entities.Models
{
    public class QhpDataMapModel
    {
        public string JsonAttribute { get; set; }
        public string QhpAttribute { get; set; }
        public string JsonXPath { get; set; }
        public string FieldType { get; set; }
        public string Comments { get; set; }
        public string Version { get; set; }
        public string RelationType { get; set; }
        public bool IsParent { get; set; }
        public bool IsChild { get; set; }
    }
}
