using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.VendorFeedReport
{
    public class VendorFeedReportViewModel:ViewModelBase
    {
        public int FolderID { get; set; }
        public int FolderVersionID { get; set; }
        public int FormInstanceID { get; set; }   
        public string GroupID { get; set; }     
        public string GroupName { get; set; }       
        public DateTime? EffectiveDate { get; set; }
        public string VendorName { get; set; }
        public string VendorType { get; set; }
        public string FormData { get; set; }
    }
}
