using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.ReportingCenter
{
   public  class RCTableDetailsViewModel : ViewModelBase
    {
        public long TableID { get; set; }
        public string SchemaName { get; set; }
        public int ReportingTableInfo_ID { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string ParentName { get; set; }
        public DateTime CreationDate { get; set; }
        public string DesignType { get; set; }
        public string DataType { get; set; }
        public string Length { get; set; }
        public bool isNullable { get; set; } = false;
        public bool IsPrimaryKey { get; set; } = false;
        public bool isUnique { get; set; } = false;
        public bool IsIdentity { get; set; } = false;
        public int IdentityIncrement { get; set; } = 1;
        public int IdentitySeed { get; set; } = 1;
        public bool isForiegnKey { get; set; } = false;
        public string ForiegnKeyTableName { get; set; }
        public string ForiegnKeyColumnName { get; set; }
        public string valuePath { get; set; }
        public string ReferenceTable { get; set; }
        public string CustomType { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
    }
}
