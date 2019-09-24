using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class WorkFlowAction : Entity
    {
        public int ActionID { get; set; }
        public string ActionName { get; set; }
    }
}
