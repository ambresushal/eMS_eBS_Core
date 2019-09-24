using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.CollateralWindowService
{
    public class SBCollateralProcessQueueViewModel
    {
        public int FolderID { get; set; }
        public string FolderName { get; set; }
        public int FolderVersionID { get; set; }
        public string FolderVersionNumber { get; set; }
        public int FormInstanceID { get; set; }
        public string FormInstanceName { get; set; }
        public int FormDesignID { get; set; }
        public int FormDesignVersionID { get; set; }
        public DateTime FolderVersionEffectiveDate { get; set; }
    }
}
