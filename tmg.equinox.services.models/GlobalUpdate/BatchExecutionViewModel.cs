using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.GlobalUpdate
{
    public class BatchExecutionViewModel : ViewModelBase
    {
        #region Instance Properties
        public Int32 BatchExecutionID { get; set; }
        public Guid BatchID { get; set; }
        public Int32 BatchExecutionStatusID { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string BatchName { get; set; }
        public string RollBackComments { get; set; }
        public string BatchExecutionStatus { get; set; }
        //public bool RollBakThreholdFlag { get; set; }
        public int RollBackThrehold { get; set; }
        public bool RollBackThreholdFlag
        {
            get
            {
                return DateTime.Now.Subtract(EndDateTime).TotalHours < RollBackThrehold ? true : false;
            }
        }
        #endregion Instance Properties
    }
}
