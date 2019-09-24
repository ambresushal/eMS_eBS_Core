using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Models
{
    public partial class MasterListImport : Entity
    {
        public int MLImportID { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public int FormInstanceID { get; set; }
        public string Comment { get; set; }
        public DateTime AddedDate { get; set; }
        public string AddedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
       public string Status { get; set; }
    }
}
