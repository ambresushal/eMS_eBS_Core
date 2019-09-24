using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.reporting.Interface;
using tmg.equinox.domain.entities.Enums;
using tmg.equinox.reporting.Reports;

namespace tmg.equinox.reporting
{
    public class ReportFactory
    {
        public static IReportExecuter<QueueItem> ReportInstance<QueueItem>(enumReportType reportType)
        {

            IReportExecuter<QueueItem> reportExecuter = null;

            switch (reportType)
            {
                case enumReportType.PDP_SOT_Post_Benchmark:
                    reportExecuter = new PDPSOTPostBenchmarkExecutor<QueueItem>();
                    break;
                case enumReportType.T_ID_CARD_Support:
                    reportExecuter = new TIDCardExecutor<QueueItem>();
                    break;
                case enumReportType.Benefits_Member_Facing_Post_Benchmark:
                    reportExecuter = new BenefitsMemberFacingPostBenchmarkExecutor<QueueItem>();
                    break;
                case enumReportType.IT_Crosswalk:
                    reportExecuter = new ITCrosswalkExecutor<QueueItem>();
                    break;
                case enumReportType.WellCare_IKA_Website_Data_Variables:
                    reportExecuter = new WellCareIKAWebsiteDataVariablesExecutor<QueueItem>();
                    break;
                case enumReportType.WellCare_Web_App_Benefit_Plan_For_IKA:
                    reportExecuter = new WellCareWebAppBenefitPlanForIKAExecutor<QueueItem>();
                    break;
                case enumReportType.LOB_Mapping:
                    reportExecuter = new LOBMappingExecutor<QueueItem>();
                    break;
                case enumReportType.IKA_Benefit_Template:
                    reportExecuter = new IKABenefitTemplateExecutor<QueueItem>();
                    break;
                case enumReportType.SOTNonMemberFacingPostBenchmark:
                    reportExecuter = new SOTNonMemberFacingPostBenchmarkExecutor<QueueItem>();
                    break;
                case enumReportType.MSP_Chart:
                    reportExecuter = new MSPChartExecutor<QueueItem>();
                    break;
                case enumReportType.High_Level_Benefits_By_Plan_Post_Benchmark:
                    reportExecuter = new HighLevelBenefitsbyPlanPostBenchmarkExecutor<QueueItem>();
                    break;
                case enumReportType.CTS_PLAN_MASTER_FINAL:
                    reportExecuter = new CtsPlanMasterFinalExecutor<QueueItem>();
                    break;
                case enumReportType.ANOC_Campaign_SOT_Data_For_Plan_Information_Transfer:
                    reportExecuter = new ANOCCampaignSOTDataForPlanInformationTransferExecutor<QueueItem>();
                    break;
                case enumReportType.PDP_Plan_FINAL:
                    reportExecuter = new PdpPlanFinalExecutor<QueueItem>();
                    break;
                case enumReportType.PDP_EOC_SB_Copay_Grid_FINAL:
                    reportExecuter = new PdpEocSBCopayGridFINALExecutor<QueueItem>();
                    break;
                case enumReportType.PDP_EOC_Tables_FINAL:
                    reportExecuter = new PdpEocTablesFinalExecutor<QueueItem>();
                    break;
                case enumReportType.LIS_Grids_For_Website_Formulas:
                    reportExecuter = new LISGridsForWebsiteFormulasExecutor<QueueItem>();
                    break;
                case enumReportType.DATA_PLAN_PREMIUM_FINAL_PostBM:
                    reportExecuter = new DataPlanPremiumFinalPostBMExecutor<QueueItem>();
                    break;
                case enumReportType.CCP_Part_D_BMLs_CCP_Part_D_BMLs_FINAL:
                    reportExecuter = new CCPPartDBMLsCCPPartDBMLsFINALExecutor<QueueItem>();
                    break;
                case enumReportType.Plan_Info_Plan_Info:
                    reportExecuter = new PlanInfoPlanInfoExecutor<QueueItem>();
                    break;
                case enumReportType.DRX_SB_Template_Final:
                    reportExecuter = new DRxSBTemplateFinalExecutor<QueueItem>();
                    break;
                case enumReportType.CCP_Plan_By_County_Final:
                    reportExecuter = new CCP_PlanByCountyFinalExecutor<QueueItem>();
                    break;
                case enumReportType.Ancillary_100_Vendor_Report:
                    reportExecuter = new Ancillary100VendorReport<QueueItem>();
                    break;
                case enumReportType.DIV_125_Panel_Report:
                    reportExecuter = new DIV125PanelReport<QueueItem>();
                    break;
                case enumReportType.Required_300_Materials_Report_MA_MAPD:
                    reportExecuter = new Required300MaterialsReportMAMAPD<QueueItem>();
                    break;
                case enumReportType.State_450_Product_Summary_SNP:
                    reportExecuter = new State450ProductSummarySNP<QueueItem>();
                    break;
                case enumReportType.State_450_Product_Summary_Non_SNP:
                    reportExecuter = new State450ProductSummaryNonSNP<QueueItem>();
                    break;
                case enumReportType.Service_Area_Expansions_And_Exits_475:
                    reportExecuter = new ServiceAreaExpansionsAndExits475<QueueItem>();
                    break;
            }
            return reportExecuter;
        }
    }
}
