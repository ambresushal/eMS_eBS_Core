using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.Qhp
{
    public class QHPFormInstanceViewModel
    {
        public int FormInstanceID { get; set; }
        public string Name { get; set; }
        public int FolderVersionId { get; set; }
        public int FormDesignVersionID { get; set; }
    }
}
