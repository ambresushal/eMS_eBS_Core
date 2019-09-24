using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.DashBoard
{
    public class CommentViewModel
    {
        public int TaskID { get; set; }
        public string Comment { get; set; }
        public DateTime Datetimestamp { get; set; }
        public string Status { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string AddedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public int FolderVersionID { get; set; }
        public string Attachment { get; set; }
        public string FolderVersionNumber { get; set; }
        public string filename { get; set; }
    }
}
