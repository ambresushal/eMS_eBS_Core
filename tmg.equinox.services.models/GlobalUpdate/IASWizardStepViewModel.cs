using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.GlobalUpdateViewModels
{
    public class IASWizardStepViewModel : ViewModelBase
    {
        public int IASWizardStepsID { get; set; }
        public int GlobalUpdateID { get; set; }
        public string Name { get; set; }
        public string NameID { get; set; }
        public bool IsActive { get; set; }       
    }
}
