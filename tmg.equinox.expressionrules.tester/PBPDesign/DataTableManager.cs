using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignGenerator
{
    public class DataTableManager
    {
        public static DataTable GetTables()
        {
            DataAccessLayer db = new DataAccessLayer();
            string query = "SELECT o.NAME,i.rowcnt FROM sysindexes AS i  INNER JOIN sys.tables AS o ON i.id = o.object_id  where SCHEMA_NAME(schema_id) = 'dbo'";
            DataTable dt = db.ExecuteSelectQuery(query, null);
            return dt;
        }

        public static DataTable GetTableColumns(string tableName)
        {
            DataAccessLayer db = new DataAccessLayer();
            string query = "SELECT c.name 'Column Name',t.Name 'Data type',c.max_length 'Max Length'FROM sys.columns c INNER JOIN sys.types t ON c.user_type_id = t.user_type_id WHERE c.object_id = OBJECT_ID('" + tableName + "')";
            DataTable dt = db.ExecuteSelectQuery(query, null);
            return dt;
        }
    }
}
