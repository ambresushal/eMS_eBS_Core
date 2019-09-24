using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.data;

namespace tmg.equinox.integration.translator.dao.Models
{
    public partial class ProductListSRC: Entity
    {
        public string ServiceGroup { get; set; }
        public string AccountName { get; set; }
        public string FolderName { get; set; }
        public string FolderVersionNumber { get; set; }
        public bool IsReleasedVersion { get; set; }
        public int ProcessGovernance1Up { get; set; }
        public int FolderVersionStateID { get; set; }
        public int FolderID { get; set; }
        public int FolderVersionID { get; set; }
        public string ProductID { get; set; }
        public int FormInstanceID { get; set; }
        public int? ProductList1up { get; set; }
        public int? ProcessStatus1up { get; set; }
        public DateTime? AddedDate { get; set; }
    }
}
