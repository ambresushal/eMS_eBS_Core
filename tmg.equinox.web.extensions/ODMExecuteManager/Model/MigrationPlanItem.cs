using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.web.ODMExecuteManager.Model
{
    public partial class MigrationPlanItem
    {
        public int MigrationPlanID { get; set; }
        public int BatchID { get; set; }
        public int FileID { get; set; }
        public int FolderId { get; set; }
        public int FolderVersionId { get; set; }
        public int FormInstanceId { get; set; }
        public int FormDesignVersionId { get; set; }
        public bool IsActive { get; set; }
        public string QID { get; set; }
        public string ViewType { get; set; }
    }
}
