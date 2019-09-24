using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.exitvalidate
{
    public class ExitValidateResultViewModel : ViewModelBase
    {
        public int ExitValidateResultID { get; set; }
        public int ExitValidateQueueID { get; set; }
        public int FormInstanceID { get; set; }
        public string ContractNumber { get; set; }
        public string PlanName { get; set; }
        public string Section { get; set; }
        public string Status { get; set; }
        public string Result { get; set; }
        public string Error { get; set; }
        public string Question { get; set; }
        public string Screen { get; set; }
        public string PBPViewSection { get; set; }
        public List<ErrorsList> Errors { get; set; }
        public string PlanID { get; set; }
        public string Snapshotfile { get; set; }
        public int CountErrors { get; set; }
        public int ExcelRowID { get; set; }

        // Field Mapping for navigate and focus 
        //public string SectionID { get; set; }
        //public string SectionName { get; set; }
        //public string GeneratedName { get; set; }
        //public string RowNum { get; set; }
        //public string FormInstance { get; set; }
        //public string ElementID { get; set; }
        //public string RowIdProperty { get; set; }
        //public string JsonPath { get; set; }
    }

    public class ErrorsList
    {
        public string ERROR { get; set; }
        public string QUESTION { get; set; }
        public string SCREEN { get; set; }
        public string FIELD { get; set; }
        public string COLUMN { get; set; }
    }
}
