using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.FolderVersionReport
{
    public class JournalActionViewModel : ViewModelBase
    {
        public int ActionId { get; set; }
        public string ActionName { get; set;}
        public bool IsActive { get; set; }
    }
}
