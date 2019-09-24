using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.documentcomparer
{
    public class RepeaterCompareResult : CompareResult
    {
        public string RepeaterName { get; set; }
        public List<RepeaterCompareKey> Keys { get; set; }
        public List<RepeaterCompareField> Fields { get; set; }
        public List<RepeaterCompareRow> Rows { get; set; }
        public bool IsRepeaterMissingInSource { get; set; }
        public bool IsRepeaterMissingInTarget { get; set; }
        public string ChildContainerName { get; set; }
        public bool CanSync { get; set; }
        public bool IsMatch { get; set; }
    }

    public class RepeaterCompareRow
    {
        public List<RepeaterCompareKey> Keys { get; set; }
        public List<RepeaterCompareField> Fields { get; set; }
        public List<RepeaterCompareRow> ChildRows { get; set; }
        public bool CanSync { get; set; }
        public bool IsMatch { get; set; }
        public JToken MissingRowInTarget { get; set; }
        public bool MissingRowInSource { get; set; }
    }

    public class RepeaterCompareKey
    {
        public string KeyLabel { get; set; }
        public string KeyName { get; set; }
        public string SourceKey { get; set; }
        public bool IsMissingInSource { get; set; }
        public bool IsMissingInTarget { get; set; }
        public string TargetKey { get; set; }
        public bool SourceTargetKeyMatch { get; set; }
    }

    public class RepeaterCompareField
    {
        public string SourceValue { get; set; }
        public string TargetValue { get; set; }
        public bool IsMissingInSource{ get; set; }
        public bool IsMissingInTarget { get; set; }
        public bool IsMatch { get; set; }
        public string FieldName { get; set; }
        public string FieldLabel { get; set; }
        public string DataType { get; set; }
    }
}
