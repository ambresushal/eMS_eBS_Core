using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class AuditReport : Entity
    {
        #region Constructor

        #endregion Constructor

        #region Instance Properties
        public Int32 AuditReportID { get; set; }

       // public Int32 IASFolderMapID { get; set; }

        public Guid BatchID { get; set; }

        public String UpdateStatus { get; set; }

        public DateTime AddedDate { get; set; }

        public String AddedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public String UpdatedBy { get; set; }

        public virtual Batch Batch { get; set; }
        //public virtual IASFolderMap IASFolderMap { get; set; }
        #endregion Instance Properties
    }
}
