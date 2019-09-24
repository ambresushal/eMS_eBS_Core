using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.anocchart.Model;

namespace tmg.equinox.anocchart.RuleProcessor
{
 public class ChangeSummaryServices
    {
        ANOCChartSource _source = new ANOCChartSource();
        public ChangeSummaryServices(JObject previousMedicareJsonData, JObject nextMedicareJsonData, JObject previousPBPViewJsonData,JObject nextPBPViewJsonData, JObject masterListAnocJsonData, JObject anocViewJsonData, JObject masterListANOCEOCJsonData, AnocchartHelper anocHelper)
        {
            _source = new Model.ANOCChartSource() { PreviousMedicareJsonData = previousMedicareJsonData, NextMedicareJsonData = nextMedicareJsonData, PreviousPBPViewJsonData = previousPBPViewJsonData, NextPBPViewJsonData = nextPBPViewJsonData, MasterListANOCEOCJsonData = masterListANOCEOCJsonData, AnocHelper = anocHelper };
        }

        public List<DoctorOfficeVisit> GetDoctorOfficeVisitSummaryChange()
        {
            ProcessDoctorOfficeVisitServices processService = new ProcessDoctorOfficeVisitServices(_source);
            return processService.DoctorOfficeVisitsChange();
        }

        public List<InpatientHospitalStays> GetInpatientHospitalStays()
        {
            ProcessInpatientHospitalServices processService = new ProcessInpatientHospitalServices(_source);
            return processService.InpatientServicesChange();
        }

        public List<CostShareDetails> CostShareDetailsChange()
        {
            ProcessCostShareDetails Obj = new ProcessCostShareDetails(_source);
            return Obj.CostShareDetailsChange();
        }
    }
}
