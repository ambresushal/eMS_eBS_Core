using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.EmailNotitication
{
   public class EmailLogger
    {
        public string AccountName { get; set; }
        public int? AccountID { get; set; }
        public int UserId { get; set; }
        public string FolderName { get; set; }
        public int FolderID { get; set; }
        public int FolderVersionID { get; set; }
        public int FolderVersionStateID { get; set; }
        public int TenantID { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string FolderVersionNumber { get; set; }
        public DateTime FolderEffectiveDate { get; set; }
        public string WorkFlowState { get; set; }
        public string WorkFlowStatus { get; set; }
        public int ApprovedWorkFlowStateID { get; set; }
        public string ApprovedWorkFlowStateName { get; set; }
        public int CurrentWorkFlowStateID { get; set; }
        public string Comment { get; set; }
        public string EmailContent { get; set; }
        public string AddedBy { get; set; }
        public string Name { get; set; }
        public string EmailID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string URL { get; set; }
    }
}
