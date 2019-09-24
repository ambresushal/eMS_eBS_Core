using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class JournalResponse: Entity
    {
        public int JournalResponseID { get; set; }
        public int JournalID { get; set; }
        public string Description { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string AddedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public virtual Journal Journal { get; set; }
    }
}
