using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class MasterListCascadeBatchDetail : Entity
    {
        public int MasterListCascadeDetailID { get; set; }

        public int MasterListCascadeBatchID { get; set; }

        public int MasterListCascadeID { get; set; }

        public int TargetFolderID { get; set; }

        public int PreviousFolderVersionID { get; set; }

        public int PreviousFormInstanceID { get; set; }

        public int NewFolderVersionID { get; set; }

        public int NewFormInstanceID { get; set; }

        public bool IsTargetMasterList { get; set; }

        public int Status { get; set; }

        public DateTime ProcessedDate { get; set; }

        public string Message { get; set; }

        public virtual MasterListCascade MasterListCascade { get; set; }

        public virtual MasterListCascadeBatch MasterListCascadeBatch { get; set; }

        public virtual MasterListCascadeStatus MasterListCascadeStatus { get; set; }
    }
}
