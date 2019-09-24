using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class OONGroupEntry : Entity
    {
        public int OONGroupEntryID { get; set; }
        public string BenefitName { get; set; }
        public string BenefitCode { get; set; }
        public string BenefitType { get; set; }
        public string Package { get; set; }
        public string BenefitGroup { get; set; }
        public string FieldType { get; set; }
        public string FieldSubType { get; set; }
        public string SOTFieldPath { get; set; }
        public bool IsActive { get; set; }
        public int FormDesignVersionId { get; set; }
    }
}
