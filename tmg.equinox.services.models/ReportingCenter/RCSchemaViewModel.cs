using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.ReportingCenter
{
    public class RCSchemaViewModel : ViewModelBase
    {
        public string SchemaName { get; set; }
    }

    public class SchemaViewModel
    {
        public string text { get; set; }
        public string ColType { get; set; }
        public string imageUrl { get; set; }

        public List<SchemaTableViewModel> items { get; set; }

    }
    public class SchemaTableViewModel
    {
        public string text { get; set; }
        public string ColType { get; set; }
        public string imageUrl { get; set; }

        public List<SchemaTableColumnViewModel> items { get; set; }

    }
    public class SchemaTableColumnViewModel
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
        public string ForiegnKeyTableName { get; set; }
        public string ForiegnKeyColumnName { get; set; }
        public string valuePath { get; set; }
        public string ReferenceTable { get; set; }
        public string CustomType { get; set; }

        public string imageUrl { get; set; }

    }

}
