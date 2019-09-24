using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.PBPImport
{
   public class ODMConfigrationViewModel : ViewModelBase
    {
        public IList<ODMPlanConfigViewModel> MatchPlanList;
        public IList<ODMPlanConfigViewModel> MisMatchPlanList;
        public string FileName;
        public string OriginalFileName;
        public string Description;
        public int Year;
        
        public ODMConfigrationViewModel()
        {
            MatchPlanList = new List<ODMPlanConfigViewModel>();
            MisMatchPlanList = new List<ODMPlanConfigViewModel>();
        }
    }
}
