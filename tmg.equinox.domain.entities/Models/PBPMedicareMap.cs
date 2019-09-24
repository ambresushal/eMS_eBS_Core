using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class PBPMedicareMap:Entity
    {
        public int PBPMedicareMapID { get; set; }
        public string ElementPath { get; set; }
        public string FieldPath { get; set; }
        public string PBPTableName { get; set; }
        public string PBPFieldName { get; set; }
        public bool IsCustomRule { get; set; }
        public int CustomRuleTypeId { get; set; }
        public int Year { get; set; }
        public bool IsActive { get; set; }
        public bool IsFullMigration { get; set; }
    }
}
