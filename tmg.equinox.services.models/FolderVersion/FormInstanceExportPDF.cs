using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.FolderVersion
{
    public class FormInstanceExportPDF : ViewModelBase
    {
        public int FormInstanceID { get; set; }
        public int FolderVersionID { get; set; }
        public int FolderId { get; set; }
        public int TenantID { get; set; }
        public int FormDesignVersionID { get; set; }
        public string FormName { get; set; }
        public string AccountName { get; set; }
        public string FolderName { get; set; }
        public string FolderVersionNumber { get; set; }
        public DateTime EffectiveDate { get; set; }       
        public int RoleID { get; set; }
        public System.DateTime GenerationDate { get; set; }
        public int TemplateID { get; set; }
    }
}
