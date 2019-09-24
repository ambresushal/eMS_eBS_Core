using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class WorkFlowStateFolderVersionMap : Entity
    {
        public int WFStateFolderVersionMapID { get; set; }
        public int ApplicableTeamID { get; set; }
        public int FolderID { get; set; }
        public int FolderVersionID { get; set; }
        public string AddedBy { get; set; }
        public System.DateTime AddedDate { get; set; }
        public virtual FolderVersion FolderVersion { get; set; }
        public virtual Folder Folder { get; set; }
        public virtual ApplicableTeam ApplicableTeam { get; set; }


    }
}
