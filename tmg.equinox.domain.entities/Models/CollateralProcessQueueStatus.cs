using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.domain.entities;

namespace tmg.equinox.domain.entities.Models
{
    public  class CollateralProcessQueueStatus : Entity
    {
        public int CollateralProcessQueue1Up { get; set; }
        public int CollateralProcessQueueStatus1Up { get; set; }
        public string Message { get; set; }
    }
}
