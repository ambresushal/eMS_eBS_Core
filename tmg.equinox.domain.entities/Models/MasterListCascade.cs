using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class MasterListCascade : Entity
    {
        public MasterListCascade()
        {
        }

        public int MasterListCascadeID { get; set; }
        public int MasterListDesignID { get; set; }
        public int MasterListDesignVersionID { get; set; }
        public string MasterListJSONPath { get; set; }
        public int TargetDesignID { get; set; }
        public int TargetDesignVersionID { get; set; }
        public int TargetDocumentType { get; set; }
        public string UpdateExpressionRule { get; set; }
        public string FilterExpressionRule { get; set; }
        public bool IsActive { get; set; }
        public string CompareMacroJSON { get; set; }
        public string MasterListCompareJSON { get; set; }
        public virtual MasterListCascadeTargetDocumentType MasterListCascadeTargetDocumentType { get; set; }
    }
}
