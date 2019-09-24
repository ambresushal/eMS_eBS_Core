using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class PBPImportTables : Entity
    {
        public int PBPTableID { get; set; }
        public string PBPTableName { get; set; }
        public int PBPTableSequence { get; set; }
        public string EBSTableName { get; set; }
    }
}
