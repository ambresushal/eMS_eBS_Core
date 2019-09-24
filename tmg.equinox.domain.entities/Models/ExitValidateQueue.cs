using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class ExitValidateQueue : Entity
    {
        public int ExitValidateQueueID { get; set; }
        public string Status {get;set;}
        public int JobId { get; set; }
        public int FormInstanceID { get; set; }
        public int FolderVersionID { get; set; }
        public int FolderID { get; set; }
        public string ProductID { get; set; }
        public int UserID { get; set; }
        public string Sections { get; set; }
        public bool SetAsDefault { get; set; }
        public string ExitValidateFilePath { get; set; }
        public string PlanAreaFilePath { get; set; }
        public string VBIDFilePath { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime AddedDate { get; set; }
        public Nullable<DateTime> CompletedDate { get; set; }
        public bool IsLatest { get; set; }
        public bool IsNotificationSent { get; set; }
        public bool IsQueuedForWFStateUpdate { get; set; }
        public string UsersInterestedInStatus { get; set; }
    }
}
