using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public class FormInstanceActivityLog : Entity
    {
        public int FormInstanceActivityLogID { get; set; }
        public int FormInstanceID { get; set; }
        public int FolderID { get; set; }
        public int FolderVersionID { get; set; }
        public int FormDesignID { get; set; }
        public int FormDesignVersionID { get; set; }
        public string ElementPath { get; set; }
        public string Field { get; set; }
        public string RowNumber { get; set; }
        public string Description { get; set; }
        public string AddedBy { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedLast { get; set; }
        public virtual FormInstance FormInstance { get; set; }
        public virtual FolderVersion FolderVersion { get; set; }
        public virtual FormDesign FormDesign { get; set; }
        public virtual FormDesignVersion FormDesignVersion { get; set; }
        public virtual Folder Folder { get; set; }
        public bool IsNewRecord { get; set; }
        public int DocID { get; set; }

    }
}
