using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.WCReport
{
    public class ReportQueueDetailsViewModel : ViewModelBase
    {
        public int ReportQueueId { get; set; }
        public int FolderId { get; set; }
        public string FolderName { get; set; }
        public int FolderVersionId { get; set; }
        public string FolderVersionNumber { get; set; }
    }
}
