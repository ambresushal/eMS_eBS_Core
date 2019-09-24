using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.FolderVersion
{
   public class FormTypeViewModel : ViewModelBase
    {
        public int FormVersionDesignID { get; set; }
        public string FormTypeName { get; set; }
        public int? FormDesignID { get; set; }
    }
}
