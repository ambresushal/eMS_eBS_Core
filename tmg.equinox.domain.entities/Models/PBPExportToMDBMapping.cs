using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities
{
    public class PBPExportToMDBMapping : Entity
    {
        public int PBPExportToMDBMapping1Up { get; set; }
        public string TableName { get; set; }
        public string FieldName { get; set; }
        public int Length { get; set; }
        public string JsonPath { get; set; }
        public bool IsRepeater { get; set; }
        public bool IsActive { get; set; }
        public string MappingType { get; set; }
        public bool IsBlankAllow { get; set; }
        public string DefaultValue { get; set; }
        public int Year { get; set; }
        public bool IsCustomRule { get; set; }
    }
}
