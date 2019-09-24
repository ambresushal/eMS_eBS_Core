using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.Report
{
   public class VisionFaxBackReportData : VisionMatrixReportData
    {
       public string VFB_GroupName { get; set; }
       public string VFB_PlanName { get; set; }
       public string VFB_UCRPercentile { get; set; }
       public string VFB_Ntw_NtwInfo_Value { get; set; }
       public string VFB_Ntw_Type { get; set; }
       public string VFB_AaccumulationPeriod { get; set; }
       public string VFB_Ntw_NtwMail_Value { get; set; }
       public string VFB_CustCareNo { get; set; }
       public string VFB_Max_T1 { get; set; }
       public string VFB_Max_T2 { get; set; }
       public string VFB_Max_T3 { get; set; }
       public string VFB_IsTherePlanMaximum { get; set; }
    }
}
