using Microsoft.Practices.Unity;
using System.Web.Http;
using Unity.WebApi;
using tmg.equinox.repository.interfaces;
using tmg.equinox.repository;
using tmg.equinox.applicationservices;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.infrastructure.logging;
using tmg.equinox.backgroundjob;
using tmg.equinox.hangfire.Configuration;
using tmg.equinox.hangfire;
using tmg.equinox.reporting.Interface;
using tmg.equinox.backgroundjob.Base;
using tmg.equinox.reporting;
using tmg.equinox.queueprocess.reporting;
using tmg.equinox.queueprocess.masterlistcascade;
using tmg.equinox.queueprocess.exitvalidate;
using tmg.equinox.applicationservices.MLImport;
using tmg.equinox.setting.Interface;
using tmg.equinox.notification;
using tmg.equinox.setting;
using tmg.equinox.applicationservices.PBPExport;
using tmg.equinox.reporting.Base.Interface;
using tmg.equinox.reporting.Base;
using tmg.equinox.applicationservices.ReportingCenter;

namespace tmg.equinox.services.api
{
    public static class UnityConfig
    {

        public static UnityContainer container;

        public static void RegisterComponents()
        {
            //container = new UnityContainer();
            tmg.equinox.dependencyresolution.UnityConfig.RegisterComponents();
            
            /*
            //Resolve UnitOfWork Dependency here 
            container.RegisterType<IUnitOfWork, UnitOfWork>();
           
            //Resolve UnitOfWork Dependency here 
            container.RegisterType<IUnitOfWorkAsync, UnitOfWork>();

            
            container.RegisterType<IRptUnitOfWorkAsync, RptUnitOfWork>();

            //Resolve infrastructure dependency here
            container.RegisterType<ILog, Logger>();

            //Resolve Service Dependencies here.
            container.RegisterType<IConsumerAccountService, ConsumerAccountService>();
            container.RegisterType<IAccountService, AccountService>();
            container.RegisterType<IFolderVersionServices, FolderVersionServices>();
            container.RegisterType<IFormInstanceDataServices, FormInstanceDataServices>();
            container.RegisterType<IFormInstanceService, FormInstanceService>();
            container.RegisterType<IWorkFlowStateServices, WorkFlowStateServices>();
            container.RegisterType<IFormDesignService, FormDesignService>();
            container.RegisterType<IUIElementService, UIElementService>();
            container.RegisterType<IFormDesignGroupService, FormDesignGroupService>();
            container.RegisterType<IDomainModelService, DomainModelService>();
            container.RegisterType<IAuditRequestService, AuditRequestService>();
            container.RegisterType<IMasterListService, MasterListService>();
            container.RegisterType<IFormInstanceRepeaterService, FormInstanceRepeaterService>();
            container.RegisterType<IConsortiumService, ConsortiumService>();

            container.RegisterType<ILoggingService, LoggingService>();
            container.RegisterType<IDataSourceService, DataSourceService>();
            container.RegisterType<IDashboardService, DashboardService>();
            container.RegisterType<IDataValueService, DataValueService>();
            container.RegisterType<IPortfolioService, PortfolioService>();
            container.RegisterType<IReportService, ReportService>();
            container.RegisterType<IJournalReportService, JournalReportService>();
            container.RegisterType<IAutoSaveSettingsService, AutoSaveSettingsService>();

            container.RegisterType<ICollateralService, CollateralService>();

            container.RegisterType<ICollateralWindowServices, CollateralWindowService>();

            container.RegisterType<IDocumentCollateralService, DocumentCollateralService>();

            container.RegisterType<IFolderVersionReportService, FolderVersionReportService>();


            container.RegisterType<ITemplateReportService, TemplateReportService>();
            container.RegisterType<ICustomRuleService, CustomRuleServices>();
            container.RegisterType<IWorkFlowSettingsService, WorkFlowSettingsService>();
            container.RegisterType<IUserManagementService, UserManagementSettings>();
            container.RegisterType<ISyncDocumentService, SyncDocumentService>();

            container.RegisterType<IServiceDesignService, ServiceDesignService>();
            container.RegisterType<IServiceDefinitionService, ServiceDefinitionService>();
            container.RegisterType<IServiceRequestHandlerService, ServiceRequestHandlerService>();

            container.RegisterType<IGlobalUpdateService, GlobalUpdateService>();
            container.RegisterType<IIASDocumentService, IASDocumentService>();
            container.RegisterType<IGlobalUpdateBatchService, GlobalUpdateBatchService>();
            container.RegisterType<IWorkFlowCategoryMappingService, WorkFlowCategoryMappingService>();
            container.RegisterType<IWorkFlowVersionStatesService, WorkFlowVersionStatesService>();
            container.RegisterType<IWorkFlowMasterService, WorkFlowMasterService>();
            container.RegisterType<IWFVersionStatesApprovalTypeService, WFVersionStatesApprovalTypeService>();
            container.RegisterType<IWorkFlowVersionStatesAccessService, WorkFlowVersionStatesAccessService>();
            

             container.RegisterType<IApiActivityLogService, ApiActivityLogService>();
            container.RegisterType<IFolderService, FolderService>();
           
            
            //Hangfire interfaces
            container.RegisterType<IBackgroundJobConfiguration, BackgroundJobConfiguration>();
            container.RegisterType<IConfiguration, HangfireConfiguration>();
            container.RegisterType<IBackgroundJobServerFactory, HangfireBackgroundJobServer>();
            container.RegisterType<ILogProviderFactory, HangfireLogProviderFactory>();
            container.RegisterType<IBackgroundJobManager, HangfireBackgroundJobManager>();
            container.RegisterType<IJobActivator, JobActivatorNotImplemented>();


            container.RegisterType<IPBPImportService, PBPImportService>();
            container.RegisterType<IPBPExportServices, PBPExportService>();
            container.RegisterType<IOdmService, OdmService>();

            container.RegisterType<IReportMasterService, ReportMasterService>();
            container.RegisterType<IReportQueueServices, ReportQueueServices>();
            container.RegisterType<IReportRepository, ReportRepository>();
            container.RegisterType<IReportingCenterSchemaService, ReportingCenterSchemaService>();


            //Reporting Center Database Interfaces
            container.RegisterType<IGenerateSchemaService, GenerateSchemaService>();
            container.RegisterType<IReportingDataService, ReportingDataService>();

               //ML Import Interfaces
          
            //Hangfire interfaces
            container.RegisterType<IBackgroundJobConfiguration, BackgroundJobConfiguration>();
            container.RegisterType<IConfiguration, HangfireConfiguration>();

            container.RegisterType<IBackgroundJobServerFactory, HangfireBackgroundJobServer>();

            container.RegisterType<ILogProviderFactory, HangfireLogProviderFactory>();

            container.RegisterType<IReportExecuter<BaseJobInfo>, ExcelReportExecuter<BaseJobInfo>>();
            container.RegisterType<IReportEnqueueService, ReportingEnqueueService>();
            container.RegisterType<IBackgroundJob<ReportQueueInfo>, BackgroundJob<ReportQueueInfo>>();
            container.RegisterType<IBackgroundJobManager, HangfireBackgroundJobManager>();
            container.RegisterType<IJobActivator, JobActivatorNotImplemented>();
            container.RegisterType<IMasterListCascadeEnqueueService, MasterListCascadeEnqueueService>();
            container.RegisterType<IBackgroundJob<MasterListCascadeQueueInfo>, BackgroundJob<MasterListCascadeQueueInfo>>();
            container.RegisterType<ITaskListService, TaskListService>();
            container.RegisterType<IWorkFlowTaskMappingService, WorkFlowTaskMappingService>();
            container.RegisterType<IPlanTaskUserMappingService, PlanTaskUserMappingService>();
            container.RegisterType<IQhpDataAdapterServices, QhpDataAdapterService>();
            container.RegisterType<IQhpService, QhpService>();
            container.RegisterType<IQhpIntegrationService, QhpIntegrationService>();
            container.RegisterType<IExitValidateEnqueueService, ExitValidateEnqueueService>();
            container.RegisterType<IExitValidateService, ExitValidateService>();

            //Configure policy -  this sets rule as set in the config file
           
            container.RegisterType<IMLImportHelperService, MLImportHelperService>();
            container.RegisterType<IResourceLockService, ResourceLockService>();


            container.RegisterType<ISettingRepository, SettingRepository>();
            container.RegisterType<ISettingProvider, SettingProvider>();
            container.RegisterType<ISettingManager, SettingManager>();
            container.RegisterType<INotificationService, NotificationService>();

            container.RegisterType<IExportPreQueueService, ExportPreQueueService>();
            */
            container = tmg.equinox.dependencyresolution.UnityConfig.container;
            container.RegisterType<IFolderService, FolderService>();

            container.RegisterType<IApiActivityLogService, ApiActivityLogService>();
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);


        }

        public static T Resolve<T>()
        {
            return container.Resolve<T>();
        }
    }
}