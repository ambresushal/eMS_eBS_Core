using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.web.ODMExecuteManager.Model
{
    public class TableInfo
    {
        public string TableName { get; set; }

        public List<string> ColumnNames { get; set; }

        public bool IsArray { get; set; }
    }
}
