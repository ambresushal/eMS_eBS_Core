using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities;

namespace tmg.equinox.repository.models
{
    public partial class ReportQueue : Entity
    {
        public ReportQueue()
        {
            this.ReportQueueDetails = new List<ReportQueueDetail>();
        }
        public int ReportQueueId { get; set; }        
        public int ReportId { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public string Status { get; set; }
        public DateTime CompletionDate { get; set; }
        public string FileName { get; set; }
        public string DestinationPath { get; set; }
        public int JobId { get; set; }
        public string ErrorMessage { get; set; }
        public virtual ICollection<ReportQueueDetail> ReportQueueDetails { get; set; }
    }
}
