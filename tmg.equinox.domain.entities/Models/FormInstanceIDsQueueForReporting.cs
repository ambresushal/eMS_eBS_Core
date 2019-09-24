using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
   public class FormInstanceIDsQueueForReporting : Entity
    {
        public int ID { get; set; }
        public int FormInstanceId { get; set; }
        public bool IsActive{ get; set; }
    }
}
