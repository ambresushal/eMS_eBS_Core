using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels
{
    public class QhpUploadTemplateViewModel : ViewModelBase
    {
        #region Public Properties
        public int QhpTemplateID { get; set; }
        public string TemplateName { get; set; }
        public string FileType { get; set; }
        public string TemplateGuid { get; set; }
        public int FolderVersionID { get; set; }
        public int FolderID { get; set; }
        public string UplodedBy { get; set; }
        public DateTime UploadDate { get; set; }
        public bool IsTemplateImported { get; set; }
        public int TenantID { get; set; }
        #endregion Public Properties

        #region Constructor
        public QhpUploadTemplateViewModel()
        {

        }
        #endregion Constructor
    }
}
