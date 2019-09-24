using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.data;


namespace tmg.equinox.integration.translator.dao.Models
{
    public class ProcessGovernance:Entity
    {
        public int ProcessGovernance1up { get; set; }
        public int Processor1Up { get; set; }
        public int ProcessStatus1Up { get; set; }
        public int ProcessType { get; set; }
        public DateTime? RunDate { get; set; }
        public int InRecoveryMode { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public bool IsActive { get; set; }
        public bool HasError { get; set; }
        public string ErrorDescription { get; set; }

    }
}
