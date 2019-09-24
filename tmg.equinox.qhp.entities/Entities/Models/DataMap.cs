using System;
using System.Collections.Generic;

namespace tmg.equinox.qhp.entities.Entities.Models
{
    public partial class DataMap 
    {
        public int DataMapID { get; set; }
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
