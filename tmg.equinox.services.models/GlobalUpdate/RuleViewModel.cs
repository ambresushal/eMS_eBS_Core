using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.GlobalUpdate
{
    public class RuleViewModel : ViewModelBase
    {
        #region Instance Properties
        public Int32 RuleID { get; set; }
        public String RuleName { get; set; }
        public String RuleDescription { get; set; }
        public Int32 RuleTargetTypeID { get; set; }
        public String ResultSuccess { get; set; }
        public String ResultFailure { get; set; }
        public Boolean IsResultSuccessElement { get; set; }
        public Boolean IsResultFailureElement { get; set; }
        public String Message { get; set; }
        public Int32 TargetPropertyID { get; set; }
        public Int32 UIElementID { get; set; }
        public bool RunOnLoad { get; set; }
        #endregion Instance Properties
    }
}
