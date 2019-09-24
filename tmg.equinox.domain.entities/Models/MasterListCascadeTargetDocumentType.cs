using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class MasterListCascadeTargetDocumentType : Entity
    {
        public MasterListCascadeTargetDocumentType()
        {
            MasterListCascades = new HashSet<MasterListCascade>();
        }

        public int DocumentTypeID { get; set; }

        public string DocumentType { get; set; }

       public virtual ICollection<MasterListCascade> MasterListCascades { get; set; }
    }
}
