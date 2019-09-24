using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class EmailNotificationQueueHistory : Entity
    {
        public int ID { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime ToBeSendDateTime { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public string FromAddress { get; set; }
        public string FromDisplayName { get; set; }
        public string ToAddresses { get; set; }
        public string CCAddresses { get; set; }
        public string BCCAddresses { get; set; }
        public bool EmailImportance { get; set; }
        public string Attachments { get; set; }
        public string Source { get; set; }
        public string SourceDescription { get; set; }
        public bool IsHTML { get; set; } = false;
        public DateTime SentDateTime { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
    }
}
