using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.services.api.Validators;

namespace tmg.equinox.services.api.Models
{
    [Validator(typeof(WorkFlowValidator))]
    public class WorkFlow
    {
        public int WFStateID { get; set; }
        public int FolderVersionId { get; set; }
        public int TenantID { get; set; }
        public string WFStateName { get; set; }
        public int ApprovalStatusId { get; set; } 
    }
}