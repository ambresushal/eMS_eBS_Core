using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.Report
{
    public class DentalMatrixReportData
    {
        public string DM_BP_SpecialAnnouncements { get; set; }
        public string DM_BP_PlanNews { get; set; }
        public string DM_Hdr_GrpName { get; set; }
        public string DM_Hdr_PlanName { get; set; }
        public string DM_Hdr_CustSrvPh { get; set; }
        public string DM_Hdr_TrfNo { get; set; }
        public string DM_Hdr_EffDt { get; set; }
        //public string DM_BO_Dental { get; set; }
        public string DM_BO_Dental1 { get; set; }
        public string DM_BO_Dental2 { get; set; }
        public string DM_BP_CalYr_Frm { get; set; }
        public string DM_BP_CalYr_To { get; set; }
        public string DM_COB_Value1 { get; set; }
        public string DM_COB_Value2 { get; set; }
        public string DM_ChlAg_Thrg { get; set; }
        public string DM_ChlAg_End { get; set; }
        public string DM_ChlAg_Incap1 { get; set; }
        public string DM_ChlAg_Incap2 { get; set; }
        public string DM_DE_COV { get; set; }
        public string DM_DE_NotCOV { get; set; }
        public string DM_Subro_Value1 { get; set; }
        public string DM_Subro_Value2 { get; set; }
        public string DM_Subro_Value2_1 { get; set; }
        public string DM_Subro_Value2_2 { get; set; }
        public string DM_Subro_Value3 { get; set; }
        public string DM_Ntw_NtwInfo_MasterNtw { get; set; }
        public string DM_Ntw_UCR_Value { get; set; }
        public string DM_ClmInfo_EOB { get; set; }
        public string DM_ClmInfo_ClmpayCycl { get; set; }
        public string DM_AdmSrv_EmpID { get; set; }
        public string DM_ClmInfo_ClmTmfrm { get; set; }
        public string DM_TrmnDate_Value { get; set; }
        public string DM_Ntw_NtwInfo_Value { get; set; }

        //GENERAL INFORMATION SECTION
        public string DM_Ded_IsApplicable { get; set; }
        public string DM_T1_Ded_CvrgLvls { get; set; }
        public string DM_T2_Ded_CvrgLvls { get; set; }
        public string DM_T3_Ded_CvrgLvls { get; set; }
        public string DM_T4_Ded_CvrgLvls { get; set; }

        public string DM_Cop_IsApplicable { get; set; }
        public string DM_T1_Copays { get; set; }
        public string DM_T2_Copays { get; set; }
        public string DM_T3_Copays { get; set; }
        public string DM_T4_Copays { get; set; }

        public string DM_Coin_IsApplicable { get; set; }
        public string DM_T1_Coins { get; set; }
        public string DM_T2_Coins { get; set; }
        public string DM_T3_Coins { get; set; }
        public string DM_T4_Coins { get; set; }

        public string DM_OopMax_T1 { get; set; }
        public string DM_OopMax_T2 { get; set; }
        public string DM_OopMax_T3 { get; set; }
        public string DM_OopMax_T4 { get; set; }
        public string DM_DplMax_T1 { get; set; }
        public string DM_DplMax_T2 { get; set; }
        public string DM_DplMax_T3 { get; set; }
        public string DM_DplMax_T4 { get; set; }

        public string DM_SpMax_T1 { get; set; }
        public string DM_SpMax_T2 { get; set; }
        public string DM_SpMax_T3 { get; set; }

        public string DM_Missing_Tooth_Clause { get; set; }

        public string DM_SpecBenfitMax_T1 { get; set; }
        public string DM_SpecBenfitMax_T2 { get; set; }
        public string DM_SpecBenfitMax_T3 { get; set; }
        public string DM_SpecBenfitMax_T4 { get; set; }

        public string DM_Ovr_Plan_Max_IsCovered { get; set; }

        public string DM_Pre_Treatment_Estimate { get; set; }

        public string DM_ISCostSharesSame_InOutOfNnetwork { get; set; }

        public string DM_Additional_ADA_Benefits { get; set; }

        public string DM_Ntw_NtwInfo_Value1 { get; set; }

    }

    public class DentalFaxBackMatrixReportData
    {
        public string DFM_PlanAccum { get; set; }
        public string DFM_CustServNo { get; set; }

        public string DFM_NtwInfo1 { get; set; }
        public string DFM_NtwInfo2 { get; set; }
        public string DFM_NtwInfo3 { get; set; }
        public string DFM_NtwInfo4 { get; set; }
        public string DFM_NtwInfo5 { get; set; }
        public string DFM_Hdr_GrpName { get; set; }
        public string DFM_Hdr_PlanName { get; set; }

        public string DFM_NtwWebAddrs { get; set; }
        public string DFM_NtwPhNo { get; set; }
        public string DFM_NtwInfo_ClmAddrss { get; set; }
        public string DFM_NtwInfo_ClmEDI { get; set; }
        public string DFM_Pre_Treatment_Estimate2 { get; set; }
        public string DFM_Missing_Tooth_Clause { get; set; } //

        public string DFM_Pre_Treatment_Estimate3 { get; set; }
        public string DFM_Pre_Treatment_Estimate1 { get; set; }


        public string DFM_OverallPlanMxm_T1 { get; set; }
        public string DFM_OverallPlanMxm_T2 { get; set; }
        public string DFM_OverallPlanMxm_T3 { get; set; }
        public string DFM_OverallPlanMxm_T4 { get; set; }

        //GENERAL INFORMATION SECTION

        public string DM_Ded_IsApplicable { get; set; }
        public string DM_Cop_IsApplicable { get; set; }
        public string DM_Coin_IsApplicable { get; set; }
        public string DM_Ovr_Plan_Max_IsCovered { get; set; }

        public string DM_T1_Ded_CvrgLvls { get; set; }
        public string DM_T2_Ded_CvrgLvls { get; set; }
        public string DM_T3_Ded_CvrgLvls { get; set; }
        public string DM_T4_Ded_CvrgLvls { get; set; }


        public string DM_T1_Copays { get; set; }
        public string DM_T2_Copays { get; set; }
        public string DM_T3_Copays { get; set; }
        public string DM_T4_Copays { get; set; }


        public string DM_T1_Coins { get; set; }
        public string DM_T2_Coins { get; set; }
        public string DM_T3_Coins { get; set; }
        public string DM_T4_Coins { get; set; }

        public string DM_BO_Extra1 { get; set; }
        public string DM_BO_Extra2 { get; set; }
        public string DM_BO_Extra3 { get; set; }
        public string DM_Additional_ADA_Benefits { get; set; }
    }

    public class PlaceHolders
    {
        public string Key { get; set; }
        public string Val { get; set; }
    }

    public class DentalService : ICloneable
    {

        public string ServiceName { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string BenefitClass { get; set; }
        public string Tier1Data { get; set; }
        public string Tier2Data { get; set; }
        public string Tier3Data { get; set; }
        public string Tier4Data { get; set; }
        public string AddInfo { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}


