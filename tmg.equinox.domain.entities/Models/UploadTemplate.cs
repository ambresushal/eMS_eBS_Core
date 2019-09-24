using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class UploadTemplate : Entity
    {
        public UploadTemplate()
        {

        }

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

        public virtual Folder Folder { get; set; }
        public virtual FolderVersion FolderVersion { get; set; }
        public virtual Tenant Tenant { get; set; }
    }
}
