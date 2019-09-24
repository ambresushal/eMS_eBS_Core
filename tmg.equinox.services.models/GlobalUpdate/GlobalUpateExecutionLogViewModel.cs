using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.GlobalUpdate
{
    public class GlobalUpateExecutionLogViewModel
    {
        public int GlobalUpateExecutionLogID { get; set; }
        public Guid BatchID { get; set; }
        public int OldFolderVersionID { get; set; }

        public int NewFolderVersionID { get; set; }

        public string Result { get; set; }

        public string Comments { get; set; }

        public string NewFolderVersionNumber { get; set; }

    }
}
