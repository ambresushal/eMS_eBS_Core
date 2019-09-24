using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels
{
    public class MasterListVersions
    {
        public int PreviousFolderVersionID { get; set; }
        public int CurrentFolderVersionID { get; set; }
        public int PreviousFormInstanceID { get; set; }
        public int CurrentFormInstanceID { get; set; }
        public DateTime CurrentEffectiveDate { get; set; }
    }
}
