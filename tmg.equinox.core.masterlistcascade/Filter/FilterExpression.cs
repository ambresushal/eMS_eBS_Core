using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.core.masterlistcascade.filter
{
    public class FilterExpression
    {
        public string SourcePath { get; set; }
        public string SourceFields { get; set; }
        public string SourceType { get; set; }
        public string Operator { get; set; }
        public string TargetPath { get; set; }
        public string TargetType { get; set; }
        public string TargetFields { get; set; }
    }
}
