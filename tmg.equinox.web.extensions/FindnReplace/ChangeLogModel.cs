using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.web.FindnReplace
{
    public class ChangeLogModel
    {
        public int FormInstanceID { get; set; }
        public string ElementPath { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string Key { get; set; }
    }
}
