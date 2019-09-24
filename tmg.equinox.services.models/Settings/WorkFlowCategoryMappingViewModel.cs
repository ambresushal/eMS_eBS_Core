using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.Settings
{
    public class WorkFlowCategoryMappingViewModel : ViewModelBase
    {
        public int TenantID { get; set; }
        public int WorkFlowVersionID { get; set; }
        public int FolderVersionCategoryID { get; set; }
        public int AccountType { get; set; }
        public int WorkFlowType { get; set; }
        public string EffectiveDate { get; set; }
        public string CategoryName { get; set; }
        public string WorkFlowtypeName { get; set; }
        public string AccountTypeName { get; set; }
        public bool? IsFinalized { get; set; }
    }
}
