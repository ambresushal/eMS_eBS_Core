using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class InterestedFolderVersion : Entity
    {
        public int InterstedFolderVersionID { get; set; }
        public int FolderVersionID { get; set; }
        public int UserID { get; set; }       
        public string AddedBy { get; set; }
        public System.DateTime AddedDate { get; set; }
        public virtual FolderVersion FolderVersion { get; set; }
        public virtual User User { get; set; }

        //public string UpdatedBy { get; set; }
        //public System.DateTime UpdatedDate { get; set; }
    }

}
