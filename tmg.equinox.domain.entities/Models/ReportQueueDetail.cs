using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class ReportQueueDetail : Entity
    {
        //public ReportDetails()
        //{
        //    this.ReportDetails = new List<ReportDetails>();
        //}

        public int ReportDetailsId { get; set; }
        public int ReportQueueId { get; set; }
        public int FolderId { get; set; }
        public int FolderVersionId { get; set; }
        public int FormInstanceId { get; set; }
        public string Status { get; set; }
        public ReportQueue ReportQueues { get; set; }
    }
}

