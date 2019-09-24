using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.mlcascade.documentcomparer
{
    public class SectionCompareResult : CompareResult
    {
        public List<SectionCompareField> Fields { get; set; }
        public bool IsSectionMissingInTarget { get; set; }
        public bool IsSectionMissingInSource { get; set; }
        public bool CanSync { get; set; }
        public bool IsMatch { get; set; }
    }

    public class SectionCompareField
    {
        public string FieldName { get; set; }
        public bool IsMissingInTarget { get; set; }
        public bool IsMissingInSource{ get; set; }
        public string SourceValue { get; set; }
        public string TargetValue { get; set; }
        public string DataType { get; set; }
        public bool IsMatch { get; set; }
    }
}
