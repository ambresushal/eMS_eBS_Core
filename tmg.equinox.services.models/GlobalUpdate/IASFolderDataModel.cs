using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.GlobalUpdate
{
    public class IASFolderDataModel
    {
        public int IASFolderMapID { get; set; }
        public int GlobalUpdateID { get; set; }
        public string AccountName { get; set; }
        public int FolderID { get; set; }
        public string FolderName { get; set; }
        public int FolderVersionID { get; set; }
        public string FolderVersionNumber { get; set; }
        public System.DateTime EffectiveDate { get; set; }
        public int FormInstanceID { get; set; }
        public string FormName { get; set; }
        public string Owner { get; set; }
        public int FormDesignID { get; set; }
    }
}
