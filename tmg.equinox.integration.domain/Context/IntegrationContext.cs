using System.Data.Entity;
using tmg.equinox.integration.translator.dao.Models.Mapping;
using tmg.equinox.integration.translator.dao.Models;
using tmg.equinox.integration.data.Models.Mapping;
using tmg.equinox.integration.facet.data.Models.Mapping;
using tmg.equinox.integration.facet.data.Models;
using tmg.equinox.integration.data.Models;

namespace tmg.equinox.integration.domain
{
    public partial class IntegrationContext : DbContext, IDbContext
    {
        static IntegrationContext()
        {
            Database.SetInitializer<IntegrationContext>(null);
        }

        public IntegrationContext()
            : base("Name=IntegrationContext")
        {
            this.Database.CommandTimeout = 600;
        }

        public DbSet<ACCM481Arc> ACCM481 { get; set; }
        public DbSet<ACDE481Arc> ACDE481 { get; set; }
        public DbSet<DEDE481Arc> DEDE481 { get; set; }
        public DbSet<IPMC481Arc> IPMC481 { get; set; }
        public DbSet<LTID481Arc> LTID481 { get; set; }
        public DbSet<LTIP481Arc> LTIP481 { get; set; }
        public DbSet<LTLT481Arc> LTLT481 { get; set; }
        public DbSet<LTPR481Arc> LTPR481 { get; set; }
        public DbSet<LTSE481Arc> LTSE481 { get; set; }
        public DbSet<PDBC481Arc> PDBC481 { get; set; }
        public DbSet<PDDS481Arc> PDDS481 { get; set; }
        public DbSet<PDPD481Arc> PDPD481 { get; set; }
        public DbSet<PDPX481Arc> PDPX481 { get; set; }
        public DbSet<PDVC481Arc> PDVC481 { get; set; }
        public DbSet<SEDF481Arc> SEDF481 { get; set; }
        public DbSet<SEPY481Arc> SEPY481 { get; set; }
        public DbSet<SERL481Arc> SERL481 { get; set; }
        public DbSet<SESE481Arc> SESE481 { get; set; }
        public DbSet<SESP481Arc> SESP481 { get; set; }
        public DbSet<SESR481Arc> SESR481 { get; set; }
        public DbSet<SETR481Arc> SETR481 { get; set; }
        public DbSet<PDBCData> PDBCData { get; set; }
        public DbSet<BenefitSet> BenefitSetData { get; set; }
        public DbSet<PDVCData> PDVCData { get; set; }
        public DbSet<ServiceGroupModel> ServiceGroup { get; set; }
        public DbSet<ServiceData> ServiceData { get; set; }
        //public DbSet<ServiceDataSHDW> ServiceDataSHDW { get; set; }
        public DbSet<DEDEData> DEDEData { get; set; }
        //public DbSet<DEDEDataSHDW> DEDEDataSHDW { get; set; }
        public DbSet<LimitData> LimitData { get; set; }
        //public DbSet<LimitDataSHDW> LimitDataSHDW { get; set; }
        public DbSet<LimitServiceData> LimitServiceData { get; set; }
        //public DbSet<LimitServiceDataSHDW> LimitServiceDataSHDW { get; set; }
        public DbSet<LimitProcedureData> LimitProcedureData { get; set; }
        //public DbSet<LimitProcedureDataSHDW> LimitProcedureDataSHDW { get; set; }
        public DbSet<LimitDiagnosisData> LimitDiagnosisData { get; set; }
        //public DbSet<LimitDiagnosisDataSHDW> LimitDiagnosisDataSHDW { get; set; }
        public DbSet<LimitProviderData> LimitProviderData { get; set; }
        //public DbSet<LimitProviderDataSHDW> LimitProviderDataSHDW { get; set; }
        public DbSet<AdditionalServiceData> AdditionalServiceData { get; set; }
        //public DbSet<AdditionalServiceDataSHDW> AdditionalServiceDataSHDW { get; set; }
        public DbSet<AdditionalServiceAltRuleData> AdditionalServiceAltRuleData { get; set; }
        //public DbSet<AdditionalServiceAltRuleDataSHDW> AdditionalServiceAltRuleDataSHDW { get; set; }
        public DbSet<ServicePenaltyData> ServicePenaltyData { get; set; }
        //public DbSet<ServicePenaltyDataSHDW> ServicePenaltyDataSHDW { get; set; }
        public DbSet<ServiceTier0Data> ServiceTier0Data { get; set; }
        //public DbSet<ServiceTier0DataSHDW> ServiceTier0DataSHDW { get; set; }
        public DbSet<ServiceTierOtherData> ServiceTierOtherData { get; set; }
        //public DbSet<ServiceTierOtherDataSHDW> ServiceTierOtherDataSHDW { get; set; }
        public DbSet<ServiceAltRuleData> ServiceAltRuleData { get; set; }
        //public DbSet<ServiceAltRuleDataSHDW> ServiceAltRuleDataSHDW { get; set; }
        public DbSet<ServiceAltRuleTier0Data> ServiceAltRuleTier0Data { get; set; }
        //public DbSet<ServiceAltRuleTier0DataSHDW> ServiceAltRuleTier0DataSHDW { get; set; }
        public DbSet<ServiceAltRuleTierOtherData> ServiceAltRuleTierOtherData { get; set; }
        //public DbSet<ServiceAltRuleTierOtherDataSHDW> ServiceAltRuleTierOtherDataSHDW { get; set; }
        public DbSet<ServiceAltRulePenaltyData> ServiceAltRulePenaltyData { get; set; }
        //public DbSet<ServiceAltRuleTierOtherDataSHDW> ServiceAltRulePenaltyDataSHDW { get; set; } 
        public DbSet<PDDSData> PDDSData { get; set; }
        public DbSet<PDPDData> PDPDData { get; set; }
        public DbSet<ATNDData> ATNDData { get; set; }

        public DbSet<ACDEData> ACDEData { get; set; }
        public DbSet<BSDEData> BSDEData { get; set; }
        public DbSet<SERLData> SERLData { get; set; }
        public DbSet<SESRData> SESRData { get; set; }
        public DbSet<SESEData> SESEData { get; set; }
        public DbSet<ServiceGroupDetailListData> ServiceGroupDetailListData { get; set; }
        public DbSet<ServiceGroupDetailListDataSRC> ServiceGroupDetailListDataSRC { get; set; }
        public DbSet<ServiceListData> ServiceListData { get; set; }
        public DbSet<DisallowedMessagesData> DisallowedMessagesData { get; set; }

        public DbSet<Plugin> Plugins { get; set; }
        public DbSet<PluginProcessorError> PluginProcessorErrors { get; set; }
        public DbSet<PluginTransmissionProcessQueue> PluginTransmissionProcessQueues { get; set; }
        public DbSet<PluginVersion> PluginVersions { get; set; }
        public DbSet<PluginVersionProcessor> PluginVersionProcessors { get; set; }
        public DbSet<PluginVersionProcessorStatus> PluginVersionProcessorStatus { get; set; }
        public DbSet<PluginVersionProcessQueueCommon> PluginVersionProcessQueues { get; set; }
        public DbSet<PluginVersionProcessQueueArc> PluginVersionProcessQueues1 { get; set; }
        public DbSet<PrefixCounter> PrefixCounter { get; set; }
        public DbSet<ELMAH_Error> ELMAH_Error { get; set; }
        public DbSet<PDPDBatch> PDPDBatch { get; set; }

        public DbSet<ACCMTRANS> ACCMTRANS { get; set; }
        public DbSet<ACDETRANS> ACDETRANS { get; set; }
        public DbSet<DEDETRANS> DEDETRANS { get; set; }
        public DbSet<LTIDTRANS> LTIDTRANS { get; set; }
        public DbSet<LTIPTRANS> LTIPTRANS { get; set; }
        public DbSet<LTLTTRANS> LTLTTRANS { get; set; }
        public DbSet<LTPRTRANS> LTPRTRANS { get; set; }
        public DbSet<LTSETRANS> LTSETRANS { get; set; }
        public DbSet<PDBCTRANS> PDBCTRANS { get; set; }
        public DbSet<PDDSTRANS> PDDSTRANS { get; set; }
        public DbSet<PDPDTRANS> PDPDTRANS { get; set; }
        public DbSet<PDVCTRANS> PDVCTRANS { get; set; }
        public DbSet<SEDFTRANS> SEDFTRANS { get; set; }
        public DbSet<SEPYTRANS> SEPYTRANS { get; set; }
        public DbSet<SERLTRANS> SERLTRANS { get; set; }
        public DbSet<SESETRANS> SESETRANS { get; set; }
        public DbSet<SEDSSRC> SEDSTRANS { get; set; }
        public DbSet<SESPTRANS> SESPTRANS { get; set; }
        public DbSet<SESRTRANS> SESRTRANS { get; set; }
        public DbSet<SETRTRANS> SETRTRANS { get; set; }
        public DbSet<SEDSMaster> SEDSMaster { get; set; }
        public DbSet<SEDSDescriptionsMaster> SEDSDescriptionsMaster { get; set; }

        public DbSet<ACCMREP> ACCMREP { get; set; }
        public DbSet<ACDEREP> ACDEREP { get; set; }
        public DbSet<DEDEMaster> DEDEMaster { get; set; }
        public DbSet<LTIDMaster> LTIDMaster { get; set; }
        public DbSet<LTIPMaster> LTIPMaster { get; set; }
        public DbSet<LTLTMaster> LTLTMaster { get; set; }
        public DbSet<LTPRMaster> LTPRMaster { get; set; }
        public DbSet<LTSEMaster> LTSEMaster { get; set; }
        public DbSet<PDBCMaster> PDBCMaster { get; set; }
        public DbSet<PDDSMaster> PDDSMaster { get; set; }
        public DbSet<PDPDMaster> PDPDMaster { get; set; }
        public DbSet<PDVCMaster> PDVCMaster { get; set; }
        public DbSet<SEDFREP> SEDFREP { get; set; }
        public DbSet<SEPYMaster> SEPYMaster { get; set; }
        public DbSet<SEPYTESTREP> SEPYTESTREP { get; set; }
        public DbSet<SERLMaster> SERLMaster { get; set; }
        public DbSet<SESEMaster> SESEMaster { get; set; }
        public DbSet<SESPMaster> SESPMaster { get; set; }
        public DbSet<SESRMaster> SESRMaster { get; set; }
        public DbSet<SETRMaster> SETRMaster { get; set; }
        public DbSet<BSBSMaster> BSBSMaster { get; set; }
        public DbSet<BSDLMaster> BSDLMaster { get; set; }
        public DbSet<BSTXMaster> BSTXMaster { get; set; }
        public DbSet<EBCLMaster> EBCLMaster { get; set; }
        public DbSet<PDPXMaster> PDPXMaster { get; set; }
        public DbSet<ATNDMaster> ATNDMaster { get; set; }
        public DbSet<ATXRMaster> ATXRMaster { get; set; }
        public DbSet<ATNTMaster> ATNTMaster { get; set; }

        public DbSet<ACCMSRC> ACCMSRC { get; set; }
        public DbSet<ACDESRC> ACDESRC { get; set; }
        public DbSet<DEDESRC> DEDESRC { get; set; }
        public DbSet<LTIDSRC> LTIDSRC { get; set; }
        public DbSet<LTIPSRC> LTIPSRC { get; set; }
        public DbSet<LTLTSRC> LTLTSRC { get; set; }
        public DbSet<LTPRSRC> LTPRSRC { get; set; }
        public DbSet<LTSESRC> LTSESRC { get; set; }
        public DbSet<PDBCSRC> PDBCSRC { get; set; }
        public DbSet<PDDSSRC> PDDSSRC { get; set; }
        public DbSet<PDPDSRC> PDPDSRC { get; set; }
        public DbSet<PDVCSRC> PDVCSRC { get; set; }
        public DbSet<SEDFSRC> SEDFSRC { get; set; }
        public DbSet<SEPYSRC> SEPYSRC { get; set; }
        public DbSet<SERLSRC> SERLSRC { get; set; }
        public DbSet<SESESRC> SESESRC { get; set; }
        public DbSet<SESPSRC> SESPSRC { get; set; }
        public DbSet<SESRSRC> SESRSRC { get; set; }
        public DbSet<SETRSRC> SETRSRC { get; set; }
        public DbSet<BSBSSRC> BSBSSRC { get; set; }
        public DbSet<EBCLSRC> EBCLSRC { get; set; }
        public DbSet<PDPXSRC> PDPXSRC { get; set; }
        public DbSet<BSTXSRC> BSTXSRC { get; set; }
        public DbSet<BSDLSRC> BSDLSRC { get; set; }
        public DbSet<CBCNamingConventions> CBCNamingConventions { get; set; }
        public DbSet<PDPX> PDPX { get; set; }
        public DbSet<ATNDSRC> ATNDSRC { get; set; }
        public DbSet<ATXRSRC> ATXRSRC { get; set; }
        public DbSet<ATNTSRC> ATNTSRC { get; set; }
        public DbSet<BSDESRC> BSDESRC { get; set; }
        public DbSet<SESERULEConfig> SESERULEConfig { get; set; }
        public DbSet<SESERuleConfigMaster> SESERuleConfigMaster { get; set; }
        public DbSet<ACDEMaster> ACDEMaster { get; set; }        
        public DbSet<SESERuleCategory> SESERuleCategory { get; set; }
        public DbSet<ServiceMaster> ServiceMaster { get; set; }
        public DbSet<SESEIDList> SESEIDList { get; set; }
        public DbSet<SEPYMasterListSRC> SEPYMasterListSRC { get; set; }
        public DbSet<LTSEMasterListSRC> LTSEMasterListSRC { get; set; }
        public DbSet<ProductListSRC> ProductListSRC { get; set; }
        public DbSet<ProductListdbo> ProductListdbo { get; set; }
        public DbSet<PrefixDescriptionLog> PrefixDescriptionLog { get; set; }
        public DbSet<DisallowedMessagesSRC> DisallowedMessagesSRC { get; set; }
        public DbSet<PDBCdbo> PDBCdbo { get; set; }
        public DbSet<TOSModelSESEIDAssoc> TOSModelSESEIDAssoc { get; set; }

        public DbSet<PDPDJsonData> PDPDJsonData { get; set; }
        public DbSet<DEDEJsonData> DEDEJsonData { get; set; }
        public DbSet<LTLTJsonData> LTLTJsonData { get; set; }
        public DbSet<LTSEJsonData> LTSEJsonData { get; set; }
        public DbSet<LTIPJsonData> LTIPJsonData { get; set; }
        public DbSet<LTIDJsonData> LTIDJsonData { get; set; }
        public DbSet<LTPRJsonData> LTPRJsonData { get; set; }
        public DbSet<ServiceJsonData> ServiceJsonData { get; set; }
        public DbSet<SESPJsonData> SESPJsonData { get; set; }
        public DbSet<SETRJsonData> SETRJsonData { get; set; }

        public DbSet<SETRAltJsonData> SETRAltJsonData { get; set; }
        public DbSet<SESPAltJsonData> SESPAltJsonData { get; set; }
        public DbSet<PDVCJsonData> PDVCJsonData { get; set; }
        public DbSet<ProductMigrationQueue> ProductMigrationQueue { get; set; }
        public DbSet<ProductMigrationQueueMig> ProductMigrationQueueMig { get; set; }
        public DbSet<FacetTranslatorQueue> FacetTranslatorQueue { get; set; }
        public DbSet<ProcessGovernance> ProcessGovernance { get; set; }
        public DbSet<PDVCHash> PDVCHash { get; set; }
        public DbSet<ProductLevelHash> ProductLevelHash { get; set; }
        public DbSet<ProcessStatusMaster> ProcessStatusMaster { get; set; }
        public DbSet<FacetTransmitterQueue> FacetTransmitterQueue { get; set; }
        public DbSet<BenefitHash> BenefitHash { get; set; }
        public DbSet<RemainingServices> RemainingServices { get; set; }
        public DbSet<SEPY> SEPY { get; set; }
        public DbSet<SEPYData> SEPYData { get; set; }
        public DbSet<TransmitterQuery> TransmitterQuery { get; set; }
        public DbSet<FacetProductTransmissionSet> FacetProductTransmissionSet { get; set; }
        public DbSet<FacetMLTransmissionSet> FacetMLTransmissionSet { get; set; }
        public DbSet<FacetsMasterlistTranslator> FacetsMasterlistTranslator { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ACCM481ArcMap());
            modelBuilder.Configurations.Add(new ACDE481ArcMap());
            modelBuilder.Configurations.Add(new DEDE481ArcMap());
            modelBuilder.Configurations.Add(new IPMC481ArcMap());
            modelBuilder.Configurations.Add(new LTID481ArcMap());
            modelBuilder.Configurations.Add(new LTIP481ArcMap());
            modelBuilder.Configurations.Add(new LTLT481ArcMap());
            modelBuilder.Configurations.Add(new LTPR481ArcMap());
            modelBuilder.Configurations.Add(new LTSE481ArcMap());
            modelBuilder.Configurations.Add(new PDBC481ArcMap());
            modelBuilder.Configurations.Add(new PDDS481ArcMap());
            modelBuilder.Configurations.Add(new PDPD481ArcMap());
            modelBuilder.Configurations.Add(new PDPX481ArcMap());
            modelBuilder.Configurations.Add(new PDVC481ArcMap());
            modelBuilder.Configurations.Add(new SEDF481ArcMap());
            modelBuilder.Configurations.Add(new SEPY481ArcMap());
            modelBuilder.Configurations.Add(new SERL481ArcMap());
            modelBuilder.Configurations.Add(new SESE481ArcMap());
            modelBuilder.Configurations.Add(new SESP481ArcMap());
            modelBuilder.Configurations.Add(new SESR481ArcMap());
            modelBuilder.Configurations.Add(new SETR481ArcMap());
            modelBuilder.Configurations.Add(new PluginMap());
            modelBuilder.Configurations.Add(new PluginProcessorErrorMap());
            modelBuilder.Configurations.Add(new PluginTransmissionProcessQueueMap());
            modelBuilder.Configurations.Add(new PluginVersionMap());
            modelBuilder.Configurations.Add(new PluginVersionProcessorMap());
            modelBuilder.Configurations.Add(new PluginVersionProcessorStatusMap());
            modelBuilder.Configurations.Add(new PluginVersionProcessQueueCommonMap());
            modelBuilder.Configurations.Add(new PluginVersionProcessQueueArcMap());
            modelBuilder.Configurations.Add(new PrefixCounterMap());
            modelBuilder.Configurations.Add(new ELMAH_ErrorMap());
            modelBuilder.Configurations.Add(new PDPDBatchMap());

            modelBuilder.Configurations.Add(new ACCMTRANSMap());
            modelBuilder.Configurations.Add(new ACDETRANSMap());
            modelBuilder.Configurations.Add(new DEDETRANSMap());
            modelBuilder.Configurations.Add(new LTIDTRANSMap());
            modelBuilder.Configurations.Add(new LTIPTRANSMap());
            modelBuilder.Configurations.Add(new LTLTTRANSMap());
            modelBuilder.Configurations.Add(new LTPRTRANSMap());
            modelBuilder.Configurations.Add(new LTSETRANSMap());
            modelBuilder.Configurations.Add(new PDBCTRANSMap());
            modelBuilder.Configurations.Add(new PDDSTRANSMap());
            modelBuilder.Configurations.Add(new PDPDTRANSMap());
            modelBuilder.Configurations.Add(new PDVCTRANSMap());
            modelBuilder.Configurations.Add(new SEDFTRANSMap());
            modelBuilder.Configurations.Add(new SEPYTRANSMap());
            modelBuilder.Configurations.Add(new SERLTRANSMap());
            modelBuilder.Configurations.Add(new SESETRANSMap());
            modelBuilder.Configurations.Add(new SEDSSRCMap());
            modelBuilder.Configurations.Add(new SESPTRANSMap());
            modelBuilder.Configurations.Add(new SESRTRANSMap());
            modelBuilder.Configurations.Add(new SETRTRANSMap());

            modelBuilder.Configurations.Add(new ACCMREPMap());
            modelBuilder.Configurations.Add(new ACDEREPMap());
            modelBuilder.Configurations.Add(new DEDEMasterMap());
            modelBuilder.Configurations.Add(new LTIDMasterMap());
            modelBuilder.Configurations.Add(new LTIPMasterMap());
            modelBuilder.Configurations.Add(new LTLTMasterMap());
            modelBuilder.Configurations.Add(new LTPRMasterMap());
            modelBuilder.Configurations.Add(new LTSEMasterMap());
            modelBuilder.Configurations.Add(new PDBCMasterMap());
            modelBuilder.Configurations.Add(new PDDSMasterMap());
            modelBuilder.Configurations.Add(new PDPDMasterMap());
            modelBuilder.Configurations.Add(new PDVCMasterMap());
            modelBuilder.Configurations.Add(new SEDFREPMap());
            modelBuilder.Configurations.Add(new SEPYMasterMap());
            modelBuilder.Configurations.Add(new SEPYTESTREPMap());
            modelBuilder.Configurations.Add(new SERLMasterMap());
            modelBuilder.Configurations.Add(new SESEMasterMap());
            modelBuilder.Configurations.Add(new SESPMasterMap());
            modelBuilder.Configurations.Add(new SESRMasterMap());
            modelBuilder.Configurations.Add(new SETRMasterMap());
            modelBuilder.Configurations.Add(new BSBSMasterMap());
            modelBuilder.Configurations.Add(new BSDLMasterMap());
            modelBuilder.Configurations.Add(new BSTXMasterMap());
            modelBuilder.Configurations.Add(new EBCLMasterMap());
            modelBuilder.Configurations.Add(new PDPXMasterMap());
            modelBuilder.Configurations.Add(new ATNDMasterMap());
            modelBuilder.Configurations.Add(new ATNTMasterMap());
            modelBuilder.Configurations.Add(new ATXRMasterMap());

            modelBuilder.Configurations.Add(new ACCMSRCMap());
            modelBuilder.Configurations.Add(new ACDESRCMap());
            modelBuilder.Configurations.Add(new DEDESRCMap());
            modelBuilder.Configurations.Add(new LTIDSRCMap());
            modelBuilder.Configurations.Add(new LTIPSRCMap());
            modelBuilder.Configurations.Add(new LTLTSRCMap());
            modelBuilder.Configurations.Add(new LTPRSRCMap());
            modelBuilder.Configurations.Add(new LTSESRCMap());
            modelBuilder.Configurations.Add(new PDBCSRCMap());
            modelBuilder.Configurations.Add(new PDDSSRCMap());
            modelBuilder.Configurations.Add(new PDPDSRCMap());
            modelBuilder.Configurations.Add(new PDVCSRCMap());
            modelBuilder.Configurations.Add(new SEDFSRCMap());
            modelBuilder.Configurations.Add(new SEPYSRCMap());
            modelBuilder.Configurations.Add(new SERLSRCMap());
            modelBuilder.Configurations.Add(new SESESRCMap());
            modelBuilder.Configurations.Add(new SESPSRCMap());
            modelBuilder.Configurations.Add(new SESRSRCMap());
            modelBuilder.Configurations.Add(new SETRSRCMap());
            modelBuilder.Configurations.Add(new BSBSSRCMap());
            modelBuilder.Configurations.Add(new EBCLSRCMap());
            modelBuilder.Configurations.Add(new PDPXSRCMap());
            modelBuilder.Configurations.Add(new BSTXSRCMap());
            modelBuilder.Configurations.Add(new BSDLSRCMap());
            modelBuilder.Configurations.Add(new CBCNamingConventionsMap());
            modelBuilder.Configurations.Add(new PDPXMap());
            modelBuilder.Configurations.Add(new ATNDSRCMap());
            modelBuilder.Configurations.Add(new ATXRSRCMap());
            modelBuilder.Configurations.Add(new ATNTSRCMap());
            modelBuilder.Configurations.Add(new BSDESRCMap());
            modelBuilder.Configurations.Add(new SESERULEConfigMap());
            modelBuilder.Configurations.Add(new SESERuleConfigMasterMap());
            modelBuilder.Configurations.Add(new ACDEMasterMap());                       
            modelBuilder.Configurations.Add(new SESERuleCategoryMap());
            modelBuilder.Configurations.Add(new ServiceMasterMap());
            modelBuilder.Configurations.Add(new SESEIDListMap());
            modelBuilder.Configurations.Add(new SEPYMasterListSRCMap());
            modelBuilder.Configurations.Add(new LTSEMasterListSRCMap());
            modelBuilder.Configurations.Add(new ProductListSRCMap());
            modelBuilder.Configurations.Add(new PrefixDescriptionLogMap());
            modelBuilder.Configurations.Add(new DisallowedMessagesSRCMap());
            modelBuilder.Configurations.Add(new ProductListdboMap());
            modelBuilder.Configurations.Add(new PDBCdboMap());
            modelBuilder.Configurations.Add(new TOSModelSESEIDAssocMap());

            modelBuilder.Configurations.Add(new PDPDJsonDataMap());
            modelBuilder.Configurations.Add(new DEDEJsonDataMap());
            modelBuilder.Configurations.Add(new LTLTJsonDataMap());
            modelBuilder.Configurations.Add(new LTSEJsonDataMap());
            modelBuilder.Configurations.Add(new LTIPJsonDataMap());
            modelBuilder.Configurations.Add(new LTIDJsonDataMap());
            modelBuilder.Configurations.Add(new LTPRJsonDataMap());
            modelBuilder.Configurations.Add(new ServiceJsonDataMap());
            modelBuilder.Configurations.Add(new SESPJsonDataMap());
            modelBuilder.Configurations.Add(new SETRJsonDataMap());

            modelBuilder.Configurations.Add(new SETRAltJsonDataMap());
            modelBuilder.Configurations.Add(new SESPAltJsonDataMap());
            modelBuilder.Configurations.Add(new PDVCJsonDataMap());
            modelBuilder.Configurations.Add(new SEDSMasterMap());
            modelBuilder.Configurations.Add(new SEDSDescriptionsMasterMap());
            modelBuilder.Configurations.Add(new ProductMigrationQueueMap());
            modelBuilder.Configurations.Add(new ProductMigrationQueueMigMap());
            modelBuilder.Configurations.Add(new PDBCDataMap());
            modelBuilder.Configurations.Add(new BenefitSetDataMap());
            modelBuilder.Configurations.Add(new PDVCDataMap());
            modelBuilder.Configurations.Add(new ServiceGroupDataMap());
            modelBuilder.Configurations.Add(new ServiceDataMap());
            //modelBuilder.Configurations.Add(new ServiceDataSHDWMap());
            modelBuilder.Configurations.Add(new DEDEDataMap());
            modelBuilder.Configurations.Add(new LimitDataMap());
            //modelBuilder.Configurations.Add(new LimitDataSHDWMap());
            modelBuilder.Configurations.Add(new LimitServiceDataMap());
            //modelBuilder.Configurations.Add(new LimitServiceDataSHDWMap());
            modelBuilder.Configurations.Add(new LimitProcedureDataMap());
            //modelBuilder.Configurations.Add(new LimitProcedureDataSHDWMap());
            modelBuilder.Configurations.Add(new LimitDiagnosisDataMap());
            //modelBuilder.Configurations.Add(new LimitDiagnosisDataSHDWMap());
            modelBuilder.Configurations.Add(new LimitProviderDataMap());
            //modelBuilder.Configurations.Add(new LimitProviderDataSHDWMap());
            modelBuilder.Configurations.Add(new AdditionalServiceDataMap());
            //modelBuilder.Configurations.Add(new AdditionalServiceDataSHDWMap());
            modelBuilder.Configurations.Add(new AdditionalServiceAltDataMap());
            //modelBuilder.Configurations.Add(new AdditionalServiceAltDataSHDWMap());

            modelBuilder.Configurations.Add(new ServicePenaltyDataMap());
            //modelBuilder.Configurations.Add(new ServicePenaltyDataSHDWMap());
            modelBuilder.Configurations.Add(new ServiceTier0DataMap());
            //modelBuilder.Configurations.Add(new ServiceTier0DataSHDWMap());
            modelBuilder.Configurations.Add(new ServiceTierOtherDataMap());
            //modelBuilder.Configurations.Add(new ServiceTierOtherDataSHDWMap());
            modelBuilder.Configurations.Add(new ServiceAltRuleDataMap());
            //modelBuilder.Configurations.Add(new ServiceAltRuleDataSHDWMap());
            modelBuilder.Configurations.Add(new ServiceAltRuleTier0DataMap());
            //modelBuilder.Configurations.Add(new ServiceAltRuleTier0DataSHDWMap());
            modelBuilder.Configurations.Add(new ServiceAltRuleTierOtherDataMap());
            //modelBuilder.Configurations.Add(new ServiceAltRuleTierOtherDataSHDWMap());
            modelBuilder.Configurations.Add(new ServiceAltRulePenaltyDataMap());
            //modelBuilder.Configurations.Add(new ServiceAltRulePenaltyDataSHDWMap());
            modelBuilder.Configurations.Add(new BenefitSummaryInfoDataMap());
            modelBuilder.Configurations.Add(new BenefitSummaryDetailsDataMap());
            modelBuilder.Configurations.Add(new EBCLDataMap());
            modelBuilder.Configurations.Add(new PDDSDataMap());
            modelBuilder.Configurations.Add(new PDPDDataMap());
            modelBuilder.Configurations.Add(new FacetTranslatorQueueMap());
            modelBuilder.Configurations.Add(new ProcessGovernanceMap());
            modelBuilder.Configurations.Add(new PDVCHashDataMap());
            modelBuilder.Configurations.Add(new ProductLevelHashDataMap());
            modelBuilder.Configurations.Add(new ProcessStatusMasterDataMap());
            modelBuilder.Configurations.Add(new FacetTransmitterQueueMap());
            modelBuilder.Configurations.Add(new BenefitHashMap());
            modelBuilder.Configurations.Add(new RemainingServicesMap());
            modelBuilder.Configurations.Add(new SEPYMap());
            modelBuilder.Configurations.Add(new SEPYDataMap());
            modelBuilder.Configurations.Add(new TransmitterQueryMap());
            modelBuilder.Configurations.Add(new FacetProductTransmissionSetMap());
            modelBuilder.Configurations.Add(new FacetMLTransmissionSetMap());
            modelBuilder.Configurations.Add(new ATNDDataMap());
            modelBuilder.Configurations.Add(new FacetsMasterListTranslatorMap());
            modelBuilder.Configurations.Add(new FacetsMasterListTransmitterMap());
            modelBuilder.Configurations.Add(new ACDEDataMap());
            modelBuilder.Configurations.Add(new BSDEDataMap());
            modelBuilder.Configurations.Add(new SERLDataMap());
            modelBuilder.Configurations.Add(new SESRDataMap());
            modelBuilder.Configurations.Add(new SESEDataMap());
            modelBuilder.Configurations.Add(new ServiceGroupDetailListDataMap());
            modelBuilder.Configurations.Add(new ServiceGroupDetailListDataSRCMap());
            modelBuilder.Configurations.Add(new ServiceListDataMap());
            modelBuilder.Configurations.Add(new DisallowedMessagesDataMap());
        }

        public System.Linq.IQueryable<T> Table<T>() where T : class
        {
            return this.Set<T>();
        }

        public new IDbSet<T> Set<T>() where T : class
        {
            return base.Set<T>();
        }
    }
}
