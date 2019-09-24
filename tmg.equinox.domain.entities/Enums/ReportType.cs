using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.domain.entities.Enums
{
    public enum enumReportType
    {      
        PDP_SOT_Post_Benchmark = 1,
        T_ID_CARD_Support = 2,
        Benefits_Member_Facing_Post_Benchmark = 3,
        IT_Crosswalk = 4,
        WellCare_IKA_Website_Data_Variables = 5,
        WellCare_Web_App_Benefit_Plan_For_IKA = 6,
        LOB_Mapping = 7,
        IKA_Benefit_Template = 8,
        SOTNonMemberFacingPostBenchmark = 9,
        MSP_Chart = 10,
        High_Level_Benefits_By_Plan_Post_Benchmark = 11,
        CTS_PLAN_MASTER_FINAL = 12,
        ANOC_Campaign_SOT_Data_For_Plan_Information_Transfer = 13,
        PDP_Plan_FINAL = 14,
        PDP_EOC_SB_Copay_Grid_FINAL = 15,
        PDP_EOC_Tables_FINAL = 16,
        LIS_Grids_For_Website_Formulas = 17,
        DATA_PLAN_PREMIUM_FINAL_PostBM = 18,
        CCP_Part_D_BMLs_CCP_Part_D_BMLs_FINAL = 19,
        Plan_Info_Plan_Info = 20,
        DRX_SB_Template_Final = 21,
        CCP_Plan_By_County_Final = 22
    }


    public static  class EnumExtension
    {
        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }

}
