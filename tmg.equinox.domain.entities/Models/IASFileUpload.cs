using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class IASFileUpload : Entity
    {
        public IASFileUpload()
        {

        }

        public int IASFileUploadID { get; set; }
        public int GlobalUpdateID { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string TemplateGuid { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public virtual GlobalUpdate GlobalUpdate { get; set; }
    }
}
