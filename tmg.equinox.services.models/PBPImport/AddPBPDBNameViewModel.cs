using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.PBPImport
{
    public class AddPBPDBNameViewModel : ViewModelBase
    {
        public string PBPDataBaseName { get; set; }
        public string PBPDataBaseDescription { get; set; }
        public int PBPDatabase1Up { get; set; }
    }
}
