using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.ReportingCenter
{
    public class ReportingSchemaJsonModel
    {
        public string  text { get; set; }
        public string ColType { get; set; }        
        public string imageUrl { get; set; }
        List<TableJson> items { get; set; }

    }

    public class TableJson
    {
        public string text { get; set; }
        public string ColType { get; set; }        
        public string imageUrl { get; set; }
        List<ColumnJson> items { get; set; }
    }

    public class ColumnJson
    {
        public string text { get; set; }
        public string ColType { get; set; }        
        public string Length { get; set; }
        public string Description { get; set; }
        public string IsNullable { get; set; }
        public string IsForiegnKey { get; set; }
        public string IsPrimaryKey { get; set; }
        public string IsIdentity { get; set; }
        public string IsUnique { get; set; }
        public string imageUrl { get; set; }
    }
}
