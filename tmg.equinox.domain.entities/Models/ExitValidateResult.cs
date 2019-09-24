using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class ExitValidateResult : Entity
    {
        public int ExitValidateResultID { get; set; }
        public int ExitValidateQueueID { get; set; }
        public int FormInstanceID { get; set; }
        public string ContractNumber { get; set; }
        public string PlanName { get; set; }
        public string Section { get; set; }
        public string Status { get; set; }
        public string Result { get; set; }
        public string Error { get; set; }
        public string Question { get; set; }
        public string Screen { get; set; }
        public string PBPFIELD { get; set; }
        public string PBPCOLUMN { get; set; }

    }
}
