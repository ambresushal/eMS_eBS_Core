using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class ManualDataUpdates : Entity
    {
        public int ManualDataUpdateID { get; set; }
        public string QID { get; set; }
        public string ViewType { get; set; }
        public string DocumentPath { get; set; }
        public string DataValue { get; set; }
        public bool IsArray { get; set; }
    }
}
