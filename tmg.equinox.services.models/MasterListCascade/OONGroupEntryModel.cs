using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.viewmodels
{
    public class OONGroupEntryModel
    {
        public string BenefitName { get; set; }
        public string BenefitCode { get; set; }
        public string BenefitType { get; set; }
        public string Package { get; set; }
        public string BenefitGroup { get; set; }
        public string FieldType { get; set; }
        public string FieldSubType { get; set; }
        public string SOTFieldPath { get; set; }
        public bool IsActive { get; set; }
        public string ConditionValue { get; set; }
        public double? Value { get; set; }
        public string ValueType { get; set; }
    }
}
