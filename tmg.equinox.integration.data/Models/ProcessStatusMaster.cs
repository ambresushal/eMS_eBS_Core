using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public class ProcessStatusMaster : Entity
    {
        public int ProcessStatus1Up { get; set; }
        public string ProcessStatusName { get; set; }
    }
}
