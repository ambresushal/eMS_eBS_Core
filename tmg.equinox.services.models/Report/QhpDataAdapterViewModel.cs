using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.Report
{
    public class QhpDataAdapterViewModel : ViewModelBase
    {
        public int FolderVersionId { get; set; }
        public int FolderId { get; set; }
        public string FolderVersionNumber { get; set; }
        public int TenantID { get; set; }
        public string AccountName { get; set; }
        public string FolderName { get; set; }

        public bool IsPortfolio { get; set; }
        public int FormDesignId { get; set; }
        public int FormInstanceId { get; set; }
    }
}
