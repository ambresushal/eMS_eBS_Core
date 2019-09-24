using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.FolderVersionReport
{
    public class JournalResponseViewModel: ViewModelBase
    {
        public int JournalResponseID { get; set; }
        public int JournalID { get; set; }
        public string Description { get; set; }
        //public System.DateTime AddedDate { get; set; }
        //public string AddedBy { get; set; }
        //public Nullable<System.DateTime> UpdatedDate { get; set; }
        //public string UpdatedBy { get; set; }
    }
}
