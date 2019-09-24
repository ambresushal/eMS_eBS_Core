using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.PBPImport
{
    public class PBPExportToMDBMappingViewModel
    {
        public string TableName { get; set; }
        public string FieldName { get; set; }
        public int Length { get; set; }
        public string JsonPath { get; set; }
        public bool IsRepeater { get; set; }
        public string MappingType { get; set; }
        public bool IsActive { get; set; }
        public bool IsBlankAllow { get; set; }
        public string DefaultValue { get; set; }
        public bool IsCustomRule { get; set; }
    }
}
