using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class EndToEndDurationForFolder:Entity
    {
        public string AccountName { get; set; }
        public string Name { get; set; }
        public System.DateTime AddedDate { get; set; }
        public System.DateTime UpdatedDate { get; set; }        
        public decimal EndToEndDuration { get; set; }
        
    }
}
