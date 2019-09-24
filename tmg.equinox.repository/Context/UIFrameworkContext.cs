using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using tmg.equinox.repository.Models.Mapping;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.Base;
//using tmg.equinox.repository.Models.Mapping.PBP;

namespace tmg.equinox.repository
{
    public partial class UIFrameworkContext : DbContext, IDbContextAsync
    {
        static UIFrameworkContext()
        {
            Database.SetInitializer<UIFrameworkContext>(null);

        }

        public UIFrameworkContext()
            : base("Name=UIFrameworkContext")
        {
            var objectContext = (this as IObjectContextAdapter).ObjectContext;
            objectContext.CommandTimeout = 180;

            //Disable lazy loading & proxy creation        
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;


        }
        //For DBConfiguration intializations
        public static UIFrameworkContext Create()
        {
            return new UIFrameworkContext();
        }

        public IQueryable<T> Table<T>() where T : class
        {
            return this.Set<T>();
        }
        public new IDbSet<T> Set<T>() where T : class
        {
            return base.Set<T>();
        }

        public override int SaveChanges()
        {
            this.ApplyStateChanges();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync()
        {
            return await this.SaveChangesAsync(CancellationToken.None);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {

            var changesAsync = await base.SaveChangesAsync(cancellationToken);

            return changesAsync;
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Configurations.Add(new AccountMap());
            modelBuilder.Configurations.Add(new AccountProductMapMap());
            modelBuilder.Configurations.Add(new AccountFolderMapMap());
            //modelBuilder.Configurations.Add(new ApprovalStatusTypeMap());
            modelBuilder.Configurations.Add(new FolderMap());
            modelBuilder.Configurations.Add(new FolderVersionMap());
            modelBuilder.Configurations.Add(new FolderVersionStateMap());
            modelBuilder.Configurations.Add(new FolderVersionWorkFlowStateMap());
            modelBuilder.Configurations.Add(new FormInstanceMap());
            modelBuilder.Configurations.Add(new FormInstanceDataMapMap());
            modelBuilder.Configurations.Add(new MarketSegmentMap());
            modelBuilder.Configurations.Add(new VersionTypeMap());
            //modelBuilder.Configurations.Add(new WorkFlowStateMap());
            modelBuilder.Configurations.Add(new WorkFlowStateGroupMap());
            modelBuilder.Configurations.Add(new FormDesignHistoryMap());
            modelBuilder.Configurations.Add(new FormInstanceHistoryMap());
            modelBuilder.Configurations.Add(new DateDataValueMap());
            modelBuilder.Configurations.Add(new NumericDataValueMap());
            modelBuilder.Configurations.Add(new StringDataValueMap());
            modelBuilder.Configurations.Add(new AttributeMap());
            modelBuilder.Configurations.Add(new ObjectDefinitionMap());
            modelBuilder.Configurations.Add(new ObjectInstanceMap());
            modelBuilder.Configurations.Add(new ObjectRelationMap());
            modelBuilder.Configurations.Add(new ObjectTreeMap());
            modelBuilder.Configurations.Add(new ObjectVersionMap());
            modelBuilder.Configurations.Add(new ObjectVersionAttribXrefMap());
            modelBuilder.Configurations.Add(new RelationKeyMap());
            modelBuilder.Configurations.Add(new TenantMap());
            modelBuilder.Configurations.Add(new UserMap());
            modelBuilder.Configurations.Add(new UserRoleAssocMap());
            modelBuilder.Configurations.Add(new UserClaimMap());
            modelBuilder.Configurations.Add(new UserActivityMap());
            modelBuilder.Configurations.Add(new ApplicationDataTypeMap());
            modelBuilder.Configurations.Add(new CalendarUIElementMap());
            modelBuilder.Configurations.Add(new CheckBoxUIElementMap());
            modelBuilder.Configurations.Add(new DataCopyModeMap());
            modelBuilder.Configurations.Add(new DataSourceMap());
            modelBuilder.Configurations.Add(new DataSourceElementDisplayModeMap());
            modelBuilder.Configurations.Add(new DataSourceMappingMap());
            modelBuilder.Configurations.Add(new DataSourceOperatorMappingMap());
            modelBuilder.Configurations.Add(new DropDownElementItemMap());
            modelBuilder.Configurations.Add(new DropDownUIElementMap());
            modelBuilder.Configurations.Add(new ExpressionMap());
            modelBuilder.Configurations.Add(new RepeaterKeyFilterMap());
            modelBuilder.Configurations.Add(new TargetRepeaterKeyFilterMap());
            modelBuilder.Configurations.Add(new ComplexOperatorMap());
            modelBuilder.Configurations.Add(new FormDesignMap());
            modelBuilder.Configurations.Add(new FormDesignGroupMap());
            modelBuilder.Configurations.Add(new FormDesignGroupMappingMap());
            modelBuilder.Configurations.Add(new FormDesignVersionMap());
            modelBuilder.Configurations.Add(new FormDesignVersionUIElementMapMap());
            modelBuilder.Configurations.Add(new FormGroupFolderMapMap());
            modelBuilder.Configurations.Add(new FormVersionObjectVersionMapMap());
            modelBuilder.Configurations.Add(new KeyProductUIElementMapMap());
            modelBuilder.Configurations.Add(new LayoutTypeMap());
            modelBuilder.Configurations.Add(new LogicalOperatorTypeMap());
            modelBuilder.Configurations.Add(new OperatorTypeMap());
            modelBuilder.Configurations.Add(new PropertyRuleMapMap());
            modelBuilder.Configurations.Add(new RadioButtonUIElementMap());
            modelBuilder.Configurations.Add(new RegexLibraryMap());
            modelBuilder.Configurations.Add(new RepeaterUIElementMap());
            modelBuilder.Configurations.Add(new RuleMap());
            modelBuilder.Configurations.Add(new RuleTargetTypeMap());
            modelBuilder.Configurations.Add(new SectionUIElementMap());
            modelBuilder.Configurations.Add(new StatusMap());
            modelBuilder.Configurations.Add(new TabUIElementMap());
            modelBuilder.Configurations.Add(new TargetPropertyMap());
            modelBuilder.Configurations.Add(new TextBoxUIElementMap());
            modelBuilder.Configurations.Add(new UIElementMap());
            modelBuilder.Configurations.Add(new UIElementTypeMap());
            modelBuilder.Configurations.Add(new ValidatorMap());
            modelBuilder.Configurations.Add(new FormDesignAccountPropertyMapMap());
            modelBuilder.Configurations.Add(new FolderVersionBatchMap());
            modelBuilder.Configurations.Add(new FormDesignDataPathMap());
            modelBuilder.Configurations.Add(new SBCReportServiceMasterMap());
            modelBuilder.Configurations.Add(new UserRoleMap());
            modelBuilder.Configurations.Add(new DataSourceModeMap());
            modelBuilder.Configurations.Add(new FormDesignTypeMap());
            modelBuilder.Configurations.Add(new JournalMap());
            modelBuilder.Configurations.Add(new JournalActionMap());
            modelBuilder.Configurations.Add(new JournalResponseMap());
            modelBuilder.Configurations.Add(new AutoSaveSettingsMap());
            modelBuilder.Configurations.Add(new FolderLockMap());
            modelBuilder.Configurations.Add(new TemplateMap());
            modelBuilder.Configurations.Add(new TemplateUIMapMap());
            modelBuilder.Configurations.Add(new FormInstanceRepeaterDataMapMap());
            modelBuilder.Configurations.Add(new WorkFlowStateUserMapMap());
            modelBuilder.Configurations.Add(new ApplicableTeamMap());
            modelBuilder.Configurations.Add(new ApplicableTeamUserMapMap());
            modelBuilder.Configurations.Add(new WorkflowTaskMapMap());
            modelBuilder.Configurations.Add(new WorkFlowStateFolderVersionMapMap());
            modelBuilder.Configurations.Add(new EmailLogMap());
            modelBuilder.Configurations.Add(new FormReportMap());
            modelBuilder.Configurations.Add(new ReportCoveredServicesMap());
            modelBuilder.Configurations.Add(new tmg.equinox.repository.Models.Mapping.FormReportVersionMap());
            modelBuilder.Configurations.Add(new tmg.equinox.repository.Models.Mapping.FormReportVersionMapMap());
            modelBuilder.Configurations.Add(new FormInstanceActivityLogMap());
            modelBuilder.Configurations.Add(new FormInstanceViewImpactLogMap());
            modelBuilder.Configurations.Add(new AlternateUIElementLabelMap());
            modelBuilder.Configurations.Add(new CopyFromAuditTrailMap());
            modelBuilder.Configurations.Add(new ConsortiumMap());
            modelBuilder.Configurations.Add(new FormReferenceMapMap());
            modelBuilder.Configurations.Add(new RepeaterKeyUIElementMap());
            modelBuilder.Configurations.Add(new RepeaterUIElementPropertyMap());
            modelBuilder.Configurations.Add(new SyncDocumentLogMap());
            modelBuilder.Configurations.Add(new SyncDocumentMacroMap());
            modelBuilder.Configurations.Add(new SyncGroupLogMap());
            modelBuilder.Configurations.Add(new FolderVersionCategoryMap());
            modelBuilder.Configurations.Add(new FolderVersionGroupMap());
            modelBuilder.Configurations.Add(new ServiceDesignMap());
            modelBuilder.Configurations.Add(new ServiceDesignVersionMap());
            modelBuilder.Configurations.Add(new ServiceDefinitionMap());
            modelBuilder.Configurations.Add(new ServiceDesignVersionServiceDefinitionMapMap());
            modelBuilder.Configurations.Add(new ServiceParameterMap());
            modelBuilder.Configurations.Add(new ExpressionGuMap());
            modelBuilder.Configurations.Add(new FormDesignElementValueMap());
            modelBuilder.Configurations.Add(new GlobalUpdateMap());
            modelBuilder.Configurations.Add(new GlobalUpdateStatusMap());
            modelBuilder.Configurations.Add(new IASElementExportMap());
            modelBuilder.Configurations.Add(new RuleGuMap());
            modelBuilder.Configurations.Add(new IASFileUploadMap());
            modelBuilder.Configurations.Add(new BatchExecutionMap());
            modelBuilder.Configurations.Add(new BatchMap());
            modelBuilder.Configurations.Add(new AuditReportMap());
            modelBuilder.Configurations.Add(new BatchExecutionStatusMap());
            modelBuilder.Configurations.Add(new ErrorLogMap());
            modelBuilder.Configurations.Add(new IASFolderMapMap());
            modelBuilder.Configurations.Add(new IASElementImportMap());
            modelBuilder.Configurations.Add(new IASWizardStepMap());
            modelBuilder.Configurations.Add(new BatchIASMapMap());
            modelBuilder.Configurations.Add(new GlobalUpateExecutionLogMap());
            modelBuilder.Configurations.Add(new WorkFlowCategoryMappingMap());
            modelBuilder.Configurations.Add(new WorkFlowVersionStatesMap());
            modelBuilder.Configurations.Add(new WorkFlowStateMasterMap());
            modelBuilder.Configurations.Add(new TaskListMap());
            modelBuilder.Configurations.Add(new WorkFlowStateApprovalTypeMasterMap());
            modelBuilder.Configurations.Add(new WFVersionStatesApprovalTypeMap());
            modelBuilder.Configurations.Add(new WorkFlowVersionStatesAccessMap());
            modelBuilder.Configurations.Add(new DocumentDesignTypeMap());
            modelBuilder.Configurations.Add(new WFStatesApprovalTypeActionMap());
            modelBuilder.Configurations.Add(new WorkFlowActionMap());
            modelBuilder.Configurations.Add(new FormDesignMappingMap());
            modelBuilder.Configurations.Add(new AccountFolderCreationPermissionMap());
            modelBuilder.Configurations.Add(new InterestedFolderVersionMap());
            modelBuilder.Configurations.Add(new DocumentRuleMap());
            modelBuilder.Configurations.Add(new MasterListCascadeDocumentRuleMap());
            modelBuilder.Configurations.Add(new DocumentRuleTypeMap());
            modelBuilder.Configurations.Add(new DocumentRuleTargetTypeMap());
            modelBuilder.Configurations.Add(new DocumentRuleEventMapMap());
            modelBuilder.Configurations.Add(new DocumentRuleEventTypeMap());
            modelBuilder.Configurations.Add(new SBCollateralProcessQueueMap());
            modelBuilder.Configurations.Add(new MasterListImportMap());
            modelBuilder.Configurations.Add(new FormInstanceProxyNumberMap());
            modelBuilder.Configurations.Add(new OONGroupEntryMap());
            modelBuilder.Configurations.Add(new ResourceLockMap());
            modelBuilder.Configurations.Add(new FormInstanceIDsQueueForReportingMap());
            modelBuilder.Configurations.Add(new FormDesignVersionExtMap());
            modelBuilder.Configurations.Add(new FormDesignUserSettingMap());
            modelBuilder.Configurations.Add(new UploadTemplateMap());
            modelBuilder.Configurations.Add(new NotificationstatusMap());
            modelBuilder.Configurations.Add(new ExitValidateQueueMap());
            modelBuilder.Configurations.Add(new ExitValidateResultMap());
            modelBuilder.Configurations.Add(new MessageDataMap());
            modelBuilder.Configurations.Add(new ApiActivityLogMap());
            modelBuilder.Configurations.Add(new FormInstanceCommentLogMap());
            modelBuilder.Configurations.Add(new DocumentUpdateTrackerMap());
            modelBuilder.Configurations.Add(new SchemaUpdateTrackerMap());
            modelBuilder.Configurations.Add(new MDMLogMap());
            modelBuilder.Configurations.Add(new JsonMap());

            #region Reports
            modelBuilder.Configurations.Add(new ReportTypeMap());
            modelBuilder.Configurations.Add(new ReportFormatTypeMap());
            modelBuilder.Configurations.Add(new TemplateReportFormDesignVersionMapMap());
            modelBuilder.Configurations.Add(new TemplateReportLocationMap());
            modelBuilder.Configurations.Add(new TemplateReportMap());
            modelBuilder.Configurations.Add(new TemplateReportParameterMap());
            modelBuilder.Configurations.Add(new TemplateReportRoleAccessPermissionMap());
            modelBuilder.Configurations.Add(new TemplateReportVersionMap());
            modelBuilder.Configurations.Add(new TemplateReportVersionParameterMap());
            modelBuilder.Configurations.Add(new TemplateReportActivityLogMap());
            modelBuilder.Configurations.Add(new CollateralProcessGovernanceMap());
            modelBuilder.Configurations.Add(new CollateralProcessQueueMap());
            modelBuilder.Configurations.Add(new CollateralProcessUploadMap());
            modelBuilder.Configurations.Add(new CollateralProcessQueueStatusMap());
            modelBuilder.Configurations.Add(new ProcessStatusMasterMap());
            modelBuilder.Configurations.Add(new ReportSettingMap());
            modelBuilder.Configurations.Add(new ReportQueueMap());
            modelBuilder.Configurations.Add(new ReportQueueDetailMap());
            modelBuilder.Configurations.Add(new CollateralImagesMap());

            #endregion

            #region Email Notification Framework
            modelBuilder.Configurations.Add(new EmailTemplateMap());
            modelBuilder.Configurations.Add(new EmailTemplatePlaceHolderMap());
            modelBuilder.Configurations.Add(new EmailTemplatePlaceHolderMappingMap());
            modelBuilder.Configurations.Add(new EmailNotificationQueueMap());
            modelBuilder.Configurations.Add(new EmailNotificationQueueHistoryMap());
            modelBuilder.Configurations.Add(new SettingDefinitionMap());
            #endregion

            #region PBPImport

            modelBuilder.Configurations.Add(new PBPImportDetailsMap());
            modelBuilder.Configurations.Add(new PBPImportQueueMap());
            modelBuilder.Configurations.Add(new PBPMatchConfigMap());
            modelBuilder.Configurations.Add(new PBPUserActionMap());
            modelBuilder.Configurations.Add(new PBPExportQueueMap());
            modelBuilder.Configurations.Add(new PBPDatabaseNameMap());
            modelBuilder.Configurations.Add(new PBPExportToMDBMappingMap());
            modelBuilder.Configurations.Add(new PBPImportTablesMap());
            modelBuilder.Configurations.Add(new PBPDataMapMap());
            modelBuilder.Configurations.Add(new PBPViewMapMap());
            modelBuilder.Configurations.Add(new PBPImportActivityLogMap());
            modelBuilder.Configurations.Add(new PBPMedicareMapMap());
            modelBuilder.Configurations.Add(new PBPExportActivityLogMap());
            modelBuilder.Configurations.Add(new PBPImportEmailNotificationMap());
            modelBuilder.Configurations.Add(new ExportPreQueueMap());
            modelBuilder.Configurations.Add(new ExportPreQueueLogMap());
            modelBuilder.Configurations.Add(new VBIDExportToMDBMappingMap());
            #endregion

            #region Master List Cascade
            modelBuilder.Configurations.Add(new MasterListCascadeTargetDocumentTypeMap());
            modelBuilder.Configurations.Add(new MasterListCascadeStatusMap());
            modelBuilder.Configurations.Add(new MasterListCascadeMap());
            modelBuilder.Configurations.Add(new MasterListCascadeBatchMap());
            modelBuilder.Configurations.Add(new MasterListCascadeBatchDetailMap());
            modelBuilder.Configurations.Add(new ElementDocumentRuleMap());
            modelBuilder.Configurations.Add(new PBPSoftwareVersionMap());
            #endregion

            #region Master List Template
            modelBuilder.Configurations.Add(new MasterListTemplateMap());
            #endregion region

            #region ODM
            modelBuilder.Configurations.Add(new AccessFilesMap());
            modelBuilder.Configurations.Add(new MigrationPlansMap());
            modelBuilder.Configurations.Add(new MigrationBatchsMap());
            modelBuilder.Configurations.Add(new BenefitMappingMap());
            modelBuilder.Configurations.Add(new BenefitsDictionaryMap());
            modelBuilder.Configurations.Add(new MigrationBatchSectionMap());
            modelBuilder.Configurations.Add(new ManualDataUpdatesMap());
            #endregion region

            #region Plan Task User
            modelBuilder.Configurations.Add(new DPFPlanTaskUserMappingMap());
            modelBuilder.Configurations.Add(new TaskCommentsMap());
            #endregion

            modelBuilder.Configurations.Add(new FormInstanceComplianceValidationlogMap());

            #region Config Rule Tester Data
            modelBuilder.Configurations.Add(new ConfigRulesTesterDataMap());
            #endregion region

            #region Config Rule Execution Logger
            modelBuilder.Configurations.Add(new FormInstanceRuleExecutionLogMap()); 
            modelBuilder.Configurations.Add(new FormInstanceRuleExecutionServerLogMap()); 
            #endregion region

			#region QHP Reporting
            modelBuilder.Configurations.Add(new QHPReportingQueueMap());
            modelBuilder.Configurations.Add(new QHPReportingQueueDetailsMap());
            #endregion region
        }
    }
}
