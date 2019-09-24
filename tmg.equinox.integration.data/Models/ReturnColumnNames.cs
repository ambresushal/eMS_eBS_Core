using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class ReturnColumnNames : Entity
    {
        public string ColumnName { get; set; }
        public string ColumnNameDisplayText { get; set; }
    }
}
