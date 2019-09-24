using System;
using System.Collections.Generic;
using tmg.equinox.integration.data;
using tmg.equinox.integration.data.Models;
using tmg.equinox.integration.translator.dao.Models;

namespace tmg.equinox.integration.facet.data.Models
{
    public class TranslationLog : Entity
    {
        public List<ShowActivitylog> LstShowActivitylog = new List<ShowActivitylog>();
        public List<FacetTranslatorQueue> LstFacetTranslatorQueue = new List<FacetTranslatorQueue>();
        public List<ProcessGovernance> LstProcessGovernance = new List<ProcessGovernance>();
        public List<PDVCHash> LstPDVCHash = new List<PDVCHash>();
        public List<ProductLevelHash> LstProductLevelHash = new List<ProductLevelHash>();
        public List<BenefitHash> LstBenefitHash = new List<BenefitHash>();
        public List<PDVCData> LstPDVCData = new List<PDVCData>();
        public List<SEPY> LstSEPY = new List<SEPY>();
        public List<PDBCData> LstPDBC = new List<PDBCData>();
        public List<ServiceData> LstServiceData = new List<ServiceData>();
        public List<ServiceTier0Data> LstServiceTier0Data = new List<ServiceTier0Data>();
        public List<ServiceTierOtherData> LstServiceTierOtherData = new List<ServiceTierOtherData>();
        public List<ServiceAltRuleData> LstServiceAltRuleData = new List<ServiceAltRuleData>();
        public List<ServiceAltRuleTier0Data> LstServiceAltRuleTier0Data = new List<ServiceAltRuleTier0Data>();
        public List<ServiceAltRuleTierOtherData> LstServiceAltRuleTierOtherData = new List<ServiceAltRuleTierOtherData>();
        public List<DEDEData> LstDEDEData = new List<DEDEData>();
        public List<LimitData> LstLimitData = new List<LimitData>();
        public List<LimitServiceData> LstLimitServiceData = new List<LimitServiceData>();
        public List<LimitDiagnosisData> LstLimitDiagnosisData = new List<LimitDiagnosisData>();
        public List<LimitProcedureData> LstLimitProcedureData = new List<LimitProcedureData>();
        public List<SEPYData> LstSEPYData = new List<SEPYData>();
        public List<RemainingServices> LstRemainingServices = new List<RemainingServices>();
        public List<ShowNewServiceRulesSummary> LstShowNewServiceRulesSummary = new List<ShowNewServiceRulesSummary>();
        public List<ShowNewServiceRulesConfiguration> LstShowNewServiceRulesConfiguration = new List<ShowNewServiceRulesConfiguration>();
    }

    public class TranslationLogExcelSheets
    {
        public string CSV { get; set; }
        public string SheetName { get; set; }
    }
}
