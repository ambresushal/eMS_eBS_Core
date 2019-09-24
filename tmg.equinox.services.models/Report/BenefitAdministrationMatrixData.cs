using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.viewmodels.Report
{
    public class BenefitAdministrationMatrixData
    {
        #region FSA
        public string BAM_Hdr_GrpName { get; set; }
        public string BAM_Hdr_PlanName { get; set; }
        public string BAM_Hdr_CustSrvPh { get; set; }
        public string BAM_Hdr_TrfNo { get; set; }
        public string BAM_Hdr_EffDt { get; set; }
        public string BAM_BP_SpecialAnnouncements { get; set; }
        public string BAM_BP_PlanNews { get; set; }
        public string BAM_BP_PlanYear { get; set; }
        public string BAM_BP_GracePeriod { get; set; }
        public string BAM_BP_PlanType { get; set; }
        public string BAM_BP_PayrollDedSchedules { get; set; }
        public string BAM_BP_DoesPayrollEmpClassCompanyDivision { get; set; }

        public string BAM_BP_EmployeeClassLocation { get; set; }
        public string BAM_BP_PayrollSchedule { get; set; }
        public string BAM_BP_FirstPayrollDate { get; set; }
        public string BAM_BP_IdentifiedBy { get; set; }
        public string BAM_BP_MedicalFSAMin { get; set; }
        public string BAM_BP_MedicalFSAMax { get; set; }
        public string BAM_BP_DepndMin { get; set; }
        public string BAM_BP_DepndMinMax { get; set; }
        public string BAM_BP_EmployeeFunding { get; set; }
        public string BAM_BP_AnnualFundingAmounts { get; set; }
        public string BAM_BP_NoEmployeeFunding { get; set; }
        public string BAM_BP_DebitCards { get; set; }
        public string BAM_BP_IsDebitCardMandatory { get; set; }
        public string BAM_BP_DebitCardsBeIssued { get; set; }
        public string BAM_BP_SpouseDemographicInfo { get; set; }
        public string BAM_BP_AdditionElectronicFundsTransfer { get; set; }
        #endregion

        #region HRA
        public string BAM_HRA_PlanYear { get; set; }
        public string BAM_HRA_SerExclud { get; set; }
        public string BAM_HRA_PayrollDedSchedules { get; set; }
        public string BAM_HRA_DoesPayrollEmpClassCompanyDivision { get; set; }
        public string BAM_HRA_EmployeeClassLocation { get; set; }
        public string BAM_HRA_PayrollSchedule { get; set; }
        public string BAM_HRA_FirstPayrollDate { get; set; }
        public string BAM_HRA_Flag { get; set; }
        public string BAM_HRA_EmployeeFunding { get; set; }
        public string BAM_HRA_AnnualFundingAmounts { get; set; }
        public string BAM_HRA_HealthyRewardsInformation { get; set; }
        public string BAM_HRA_NoEmployeeFunding { get; set; }
        public string BAM_HRA_DebitCards { get; set; }
        public string BAM_HRA_DebitCardsBeIssued { get; set; }
        public string BAM_HRA_SpouseDemographicInfo { get; set; }
        public string BAM_HRA_AdditionElectronicFundsTransfer { get; set; }
        public string BAM_HRA_IsDebitCardMandatory { get; set; }
        public string BAM_HRA_CheckRunDate { get; set; }
        public string BAM_HRA_EnrollmentProcess { get; set; }
        public string BAM_HRA_MemberDateTermination { get; set; }
        public string BAM_HRA_PayRollFile { get; set; }
        public string BAM_HRA_EnrollmentForm { get; set; }
        public string BAM_HRA_ClaimForm { get; set; }
        public string BAM_HRA_HSBFormLink { get; set; }
        public string BAM_HRA_Note { get; set; }
        public string BAM_HRA_IncFile { get; set; }
        public string BAM_HRA_FormLink { get; set; }
        #endregion

        #region HSA
        public string BAM_HSA_PlanYear { get; set; }
        public string BAM_HSA_PayrollDedSchedules { get; set; }
        public string BAM_HSA_EmployeeClassLocation { get; set; }
        public string BAM_HSA_PayrollSchedule { get; set; }
        public string BAM_HSA_FirstPayrollDate { get; set; }
        public string BAM_HSA_Flag { get; set; }
        public string BAM_HSA_EmployeeFunding { get; set; }
        public string BAM_HSA_AnnualFundingAmounts { get; set; }
        public string BAM_HSA_NoEmployeeFunding { get; set; }
        public string BAM_HSA_NewHireFunding { get; set; }
        public string BAM_HSA_HealthyRewardsInformation { get; set; }
        public string BAM_HSA_DebitCards { get; set; }
        public string BAM_HSA_IsDebitCardMandatory { get; set; }
        public string BAM_HSA_DebitCardsBeIssued { get; set; }
        public string BAM_HSA_SpouseDemographicInfo { get; set; }
        public string BAM_HSA_AdditionElectronicFundsTransfer { get; set; }
        public string BAM_HSA_MaxEleAmt { get; set; }
        public string BAM_HSA_CheckRunDate { get; set; }
        public string BAM_HSA_EnrollmentProcess { get; set; }
        public string BAM_HSA_MemberDateTermination { get; set; }
        public string BAM_HSA_PayRollFile { get; set; }
        public string BAM_HSA_IncFile { get; set; }
        
        public string BAM_HSA_EnrollmentForm { get; set; }
        public string BAM_HSA_ClaimForm { get; set; }
        public string BAM_HSA_FormLink { get; set; }
        public string BAM_HSA_HSBFormLink { get; set; }
        public string BAM_HSA_Note { get; set; }
        #endregion

        #region COBRA
        public string BAM_COBRA_BT { get; set; }
        public string BAM_COBRA_ConInfo { get; set; }
        public string BAM_COBRA_VenConInfo { get; set; }
        public string BAM_COBRA_TandReComm { get; set; }
        public string BAM_COBRA_TopA { get; set; }
        public string BAM_COBRA_SDT { get; set; }
        public string BAM_COBRA_Serv { get; set; }
        public string BAM_COBRA_UNameAndEg { get; set; }
        public string BAM_COBRA_PassAndEg { get; set; }
        public string BAM_COBRA_AdminContact { get; set; }
        public string BAM_COBRA_TremClient { get; set; }

            

        #endregion

        #region Genral Info

        public string BAM_BP_Service { get; set; }

        public string BAM_GI_UNameAndEg { get; set; }

        public string BAM_GI_PassAndEg { get; set; }

        public string BAM_GI_ClaimFilingDetails { get; set; }

        public string BAM_GI_NewGroup { get; set; }

        public string BAM_GI_FilingDeadline { get; set; }

        public string BAM_BP_CheckRunDate { get; set; }

        public string BAM_BP_EnrollmentProcess { get; set; }

        public string BAM_BP_MemberDateTermination { get; set; }

        public string BAM_BP_PayRollFile { get; set; }

        public string BAM_BP_EnrollmentForm { get; set; }

        public string BAM_BP_ClaimForm { get; set; }

        public string BAM_BP_FormLink { get; set; }

        public string BAM_BP_HSBFormLink { get; set; }

        public string BAM_BP_Note { get; set; }

        #endregion
    }

    public class PayrollDeductionSchedules
    {
        public string EmployeeClassLocation { get; set; }

        public string PayrollSchedule { get; set; }

        public string FirstPayrollDate { get; set; }

        public string IdentifiedBy { get; set; }

    }
}
