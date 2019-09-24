using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class IASFolderMap : Entity
    {
        public IASFolderMap()
        {

        }

        public int IASFolderMapID { get; set; }
        public int GlobalUpdateID { get; set; }
        public int FolderID { get; set; }        
        public int FolderVersionID { get; set; }
        public int FormInstanceID { get; set; }
        public string FormName { get; set; }
        public string FolderVersionNumber { get; set; }
        public string FolderName { get; set; }        
        public string Owner { get; set; }
        public System.DateTime EffectiveDate { get; set; }
        public string AccountName { get; set; }
        public string AddedBy { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public virtual GlobalUpdate GlobalUpdate { get; set; }
        public virtual ICollection<IASElementImport> IASElementImports { get; set; }
        public virtual ICollection<IASElementExport> IASElementExports { get; set; }
        public virtual Folder Folder { get; set; }
        public virtual FolderVersion FolderVersion { get; set; }
        public virtual FormInstance FormInstance { get; set; }
    }
}
