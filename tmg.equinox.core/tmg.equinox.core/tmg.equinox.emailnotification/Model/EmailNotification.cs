using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities.Models;

namespace tmg.equinox.emailnotification.Model
{
    public class EmailNotificationInfo
    { 
        public EmailTemplateInfo TemplateInfo { get; set; }
        public DateTime ToBeSendDateTime { get; set; }
        public string FromAddress { get; set; }
        public string FromDisplayName { get; set; }
        public List<string> ToAddresses { get; set; }
        public List<string> CCAddresses { get; set; }
        public List<string> BCCAddresses { get; set; }
        public bool IsImportance { get; set; }
        public List<string> Attachments { get; set; }
        public string Source { get; set; }
        public string SourceDescription { get; set; }
        public bool? IsHTML { get; set; } 
    }
}
