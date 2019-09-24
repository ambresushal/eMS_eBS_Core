using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class FormInstanceComplianceValidationlog : Entity
    {
        public int logId { get; set; }
        public int FormInstanceID { get; set; }
        public System.DateTime AddedDate { get; set; }

        public int No { get; set; }
        public string Error { get; set; }
        public string AddedBy { get; set; }
        public string ComplianceType { get; set; }
        public string ValidationType { get; set; }

        public int? CollateralProcessQueue1Up { get; set; }

    }
}
