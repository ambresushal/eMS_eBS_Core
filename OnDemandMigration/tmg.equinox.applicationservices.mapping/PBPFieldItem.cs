using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.pbp
{
    public class PBPFieldItem
    {
        public int ID { get; set; }
        public string FILE { get; set; }
        public string NAME { get; set; }
        public string TYPE { get; set; }
        public double? LENGTH { get; set; }
        public string FIELD_TITLE { get; set; }
        public string TITLE { get; set; }
        public string Codes { get; set; }
        public string CODE_VALUES { get; set; }
        public string YEAR { get; set; }

    }
}
