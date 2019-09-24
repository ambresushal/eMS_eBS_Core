using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.WCReport
{
    public class ReportQueueViewModel : ViewModelBase
    {
        public int ReportQueueId { get; set; }
        public string ReportName { get; set; }
        public int ReportId { get; set; }
        public int FolderId { get; set; }
        public int FolderVersionId { get; set; }
        public int FormInstanceId { get; set; }
        public string Status { get; set; }
        public DateTime CompletionDate { get; set; }
        public string FileName { get; set; }
        public string DestinationPath { get; set; }
        public int JobId { get; set; }
        public string ErrorMessage { get; set; }
    }
}
