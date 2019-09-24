using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.facet.data.Models
{
    public partial class ProductMigrationQueueMig : Entity
    {
        public int ProductMigrationQueue1Up { set; get; }
        public string BatchID { set; get; }
        public string ProductID { set; get; }
        public string FolderVersion { set; get; }
        public string MajorFolderVersion { set; get; }
        public int FolderVersionID { set; get; }
        public int FormInstanceID { set; get; }
        public string ServiceGroup { set; get; }
        public DateTime EffectiveDate { set; get; }
        public bool? isProcessed { set; get; }
        //public bool? isFolderConsider { set; get; }
        public bool? HasError { set; get; }
        public string ErrorDescriotion { set; get; }
        public string AddedBy { set; get; }
        public Nullable<System.DateTime> AddedDate { set; get; }
        public bool? isActive { set; get; }
    }
}
