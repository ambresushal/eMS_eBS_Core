using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.GlobalUpdateViewModels;

namespace tmg.equinox.applicationservices.viewmodels.GlobalUpdate
{
    public class GlobalUpdateViewModel : ViewModelBase
    {
        public int GlobalUpdateID { get; set; }
        public int TenantID { get; set; }
        public int GlobalUpdateStatusID { get; set; }
        public int WizardStepsID { get; set; }
        public string WizardStepName { get; set; }
        public string GlobalUpdateName { get; set; }
        public System.DateTime? EffectiveDateFrom { get; set; }
        public System.DateTime? EffectiveDateTo { get; set; }
        public bool IsActive { get; set; }
        public string status { get; set; }
        public bool IsIASDownloaded { get; set; }
        public bool IsErrorLogDownloaded { get; set; }
        public System.DateTime? GuAddedDate { get; set; }
        public string RowID { get; set; }
        public List<IASWizardStepViewModel> iasWizardSteps { get; set; }

        public int FolderVersionID { get; set; }
        public string AccountName { get; set; }
        public int FolderID { get; set; }
        public string FolderName { get; set; }
        public string FolderVersionNumber { get; set; }
        public DateTime EffectiveDate { get; set; }
        public int FormInstanceID { get; set; }
        public string FormName { get; set; }
        public string Owner { get; set; }
        public IEnumerable<RoleClaimModel> TestRole { get; set; }

        public int UIElementID { get; set; }
        public string UIElementName { get; set; }
        public string Label { get; set; }
        public string ElementType { get; set; }
        public string Value { get; set; }

        //Realtime Execution Status : AutoRefresh
        public string ExecutionStatusText { get; set; }
        public string ExecutionStatusSymbol { get; set; }
    }
}
