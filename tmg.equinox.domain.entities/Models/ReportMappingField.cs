using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class ReportMappingField
    {

        public string DataFieldName { get; set; }
        public string DBFieldame { get; set; }
        public bool isRule { get; set; }
        public string RuleName { get; set; }
        public string Expression { get; set; }
        public string DisplayName { get; set; }
        
    }
}
