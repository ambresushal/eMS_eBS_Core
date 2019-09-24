using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.FormDesign
{
    public class ExcelConfiguration
    {
        public string ColumnHeader { get; set; }
        public string ColumnName { get; set; }
        public int ColumnIndex { get; set; }
        public bool Include { get; set; }
        public string OutputColumnName { get; set; }

    }
}
