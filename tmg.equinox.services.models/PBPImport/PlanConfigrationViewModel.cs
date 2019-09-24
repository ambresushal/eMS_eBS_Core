using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.PBPImport
{
   public class PlanConfigurationViewModel : ViewModelBase
    {
        public IList<PBPPlanConfigViewModel> MatchPlanList;
        public IList<PBPPlanConfigViewModel> MisMatchPlanList;
        public int PBPImportQueueID { get; set; }
        public PlanConfigurationViewModel()
        {
            MatchPlanList = new List<PBPPlanConfigViewModel>();
            MisMatchPlanList = new List<PBPPlanConfigViewModel>();
        }
    }
}
