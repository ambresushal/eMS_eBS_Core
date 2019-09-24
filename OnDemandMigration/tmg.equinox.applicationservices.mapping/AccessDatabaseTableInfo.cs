using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.pbp
{
    public class AccessDatabaseTableInfo
    {
        public string TableName { get; set; }
        public List<AccessDatabaseTableColumnInfo> accessDatabaseTableColumnInfo { get; set; }
    }
}
