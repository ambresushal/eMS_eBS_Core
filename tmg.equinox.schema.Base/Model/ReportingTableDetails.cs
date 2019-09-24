using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.schema.Base.Model
{
   public  class ReportingTableDetails
    {
        public long TableID { get; set; }
        public string TableName { get; set; }
        public string TableSchemaName { get; set; }
        public string TableParentName { get; set; }
        public int TableDesignId { get; set; }
        public int TableDesignVersionId { get; set; }
        public DateTime TableCreationDate { get; set; }
        public string TableLabel { get; set; }
        public string TableDescription { get; set; }
        public string TableDesignType { get; set; }
        public string TableDocumentPath { get; set; }
        public long ColumnID { get; set; }
        public long ColumnReportingTableInfo_ID { get; set; }
        public string ColumnName { get; set; }
        public string ColumnDataType { get; set; }
        public string ColumnLength { get; set; }
        public bool ColumnisNullable { get; set; }
        public bool ColumnIsPrimaryKey { get; set; }
        public bool ColumnisUnique { get; set; }
        public bool ColumnIsIdentity { get; set; }
        public int ColumnIdentityIncrement { get; set; }
        public int ColumnIdentitySeed { get; set; }
        public bool ColumnisForiegnKey { get; set; }
        public string ColumnForiegnKeyTableName { get; set; }
        public string ColumnForiegnKeyColumnName { get; set; }
        public string ColumnvaluePath { get; set; }
        public string ColumnReferenceTable { get; set; }
        public string ColumnCustomType { get; set; }
        public string ColumnLabel { get; set; }
        public string ColumnDescription { get; set; }

        public string DesignVersionNumber { get; set; }
    }
}
