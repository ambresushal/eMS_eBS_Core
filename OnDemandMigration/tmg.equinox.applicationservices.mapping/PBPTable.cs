using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.pbp
{
    public class PBPTable
    {
        public string TableName { get; set; }

        public List<string> ColumnNames { get; set; }

        public bool IsArray { get; set; }

    }
}
