using System;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Unity.Mvc5;
using System.Configuration;
using tmg.equinox.repository.interfaces;
using tmg.equinox.repository;
using tmg.equinox.applicationservices;
using tmg.equinox.applicationservices.interfaces;
using Microsoft.Practices.Unity.InterceptionExtension;
using tmg.equinox.infrastructure.logging;
using tmg.equinox.reporting.Base.Interface;
using tmg.equinox.reporting.Base;
using tmg.equinox.config;
using tmg.equinox.backgroundjob;
using tmg.equinox.reporting.Interface;
using tmg.equinox.backgroundjob.Base;
using tmg.equinox.reporting;
using System.Collections.Generic;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.domain.entities;
using tmg.equinox.reporting.Base.Mapping;
using tmg.equinox.queueprocess.reporting;
using tmg.equinox.hangfire;
using tmg.equinox.hangfire.Configuration;
using tmg.equinox.emailnotification;
using tmg.equinox.applicationservices.MLImport;
using tmg.equinox.queueprocess.masterlistcascade;
using tmg.equinox.applicationservices.masterlistcascade;
using tmg.equinox.applicationservices.ReportingCenter;
using tmg.equinox.setting.Interface;
using tmg.equinox.setting;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR;
using tmg.equinox.notification;
using tmg.equinox.applicationservices.PBPExport;
using tmg.equinox.queueprocess.exitvalidate;
using tmg.equinox.repository.core;
using tmg.equinox.repository.email;
using tmg.equinox.integration.qhplite;
using tmg.equinox.savetoreportingdbmlcascade;
using tmg.equinox.queueprocess.MLCascadeReportingDBSaveQueue;

namespace tmg.equinox.dependencyresolution
{
    public static class UnityConfig
    {
        public static UnityContainer container;

        public static void RegisterComponents()
        {
            var first = new InjectionProperty("Order", 1);

            bool isInterceptionEnabled = false;
            isInterceptionEnabled = AuditConfig.EnableAuditThroughInterception;
          

            container = new UnityContainer();

            //To let unity support interception
            container.AddNewExtension<Interception>();

            //Resolve UnitOfWork Dependency here 
            container.RegisterType<IUnitOfWork, UnitOfWork>();
           
            //Resolve UnitOfWork Dependency here 
            container.RegisterType<IUnitOfWorkAsync, UnitOfWork>();

            container.RegisterType<IRptUnitOfWorkAsync, RptUnitOfWork>("UI");
            container.RegisterType<IRptUnitOfWorkAsync, RptUnitOfWork>();
            container.RegisterType<IRptUnitOfWorkAsync, CoreRptUnitOfWork>();
            //Resolve UnitOfWork Dependency here 
            container.RegisterType<IUnitOfWorkAsync, CoreUnitOfWork>("Core");
            container.RegisterType<IUnitOfWorkAsync, EmailUnitOfWork>("Email");

            
            // register Injection factory to resolve the dependency
            container.RegisterType<Func<string, IUnitOfWorkAsync>>(
                        new InjectionFactory(c =>
                        new Func<string, IUnitOfWorkAsync>(name => c.Resolve<IUnitOfWorkAsync>(name)))
                        );

            container.RegisterType<Func<string, IRptUnitOfWorkAsync>>(
                        new InjectionFactory(c =>
                        new Func<string, IRptUnitOfWorkAsync>(name => c.Resolve<IRptUnitOfWorkAsync>(name)))
                        );
            //Resolve infrastructure dependency here
            container.RegisterType<ILog, Logger>();            

            //Resolve Service Dependencies here.
            container.RegisterType<IConsumerAccountService, ConsumerAccountService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>()); 
            container.RegisterType<IAccountService, AccountService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IFolderVersionServices, FolderVersionServices>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IFormInstanceDataServices, FormInstanceDataServices>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IFormInstanceService, FormInstanceService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>()); 
            container.RegisterType<IWorkFlowStateServices, WorkFlowStateServices>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>()); 
            container.RegisterType<IFormDesignService, FormDesignService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IUIElementService, UIElementService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>()); 
            container.RegisterType<IFormDesignGroupService, FormDesignGroupService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>()); 
            container.RegisterType<IDomainModelService, DomainModelService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>()); 
            container.RegisterType<IAuditRequestService, AuditRequestService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>()); 
            container.RegisterType<IMasterListService, MasterListService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IMasterListCascadeService, MasterListCascadeService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IFormInstanceRepeaterService, FormInstanceRepeaterService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IConsortiumService, ConsortiumService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            
            container.RegisterType<ILoggingService, LoggingService>();
            container.RegisterType<IDataSourceService, DataSourceService>();
            container.RegisterType<IDashboardService, DashboardService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IDataValueService, DataValueService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>()); 
            container.RegisterType<IPortfolioService, PortfolioService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IReportService, ReportService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IJournalReportService, JournalReportService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IAutoSaveSettingsService, AutoSaveSettingsService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());

            container.RegisterType<ICollateralService, CollateralService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());

            container.RegisterType<ICollateralWindowServices, CollateralWindowService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());

            container.RegisterType<IDocumentCollateralService, DocumentCollateralService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
                        
            container.RegisterType<IFolderVersionReportService, FolderVersionReportService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IFormInstanceViewImpactLogService, FormInstanceViewImpactLogService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IFormInstanceRuleExecutionLogService, FormInstanceRuleExecutionLogService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());

            container.RegisterType<ITemplateReportService, TemplateReportService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<ICustomRuleService, CustomRuleServices>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IWorkFlowSettingsService, WorkFlowSettingsService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IUserManagementService, UserManagementSettings>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<ISyncDocumentService, SyncDocumentService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());

            container.RegisterType<IServiceDesignService, ServiceDesignService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IServiceDefinitionService, ServiceDefinitionService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IServiceRequestHandlerService, ServiceRequestHandlerService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());

            container.RegisterType<IGlobalUpdateService, GlobalUpdateService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IIASDocumentService, IASDocumentService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IGlobalUpdateBatchService, GlobalUpdateBatchService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            
            container.RegisterType<IWorkFlowCategoryMappingService, WorkFlowCategoryMappingService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IWorkFlowVersionStatesService, WorkFlowVersionStatesService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IWorkFlowMasterService, WorkFlowMasterService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IWFVersionStatesApprovalTypeService, WFVersionStatesApprovalTypeService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IWorkFlowVersionStatesAccessService, WorkFlowVersionStatesAccessService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());

            container.RegisterType<IFolderLockService, FolderLockService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());

            container.RegisterType<IPBPImportService, PBPImportService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IPBPExportServices, PBPExportService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IOdmService, OdmService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());

            container.RegisterType<IReportMasterService, ReportMasterService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IReportQueueServices, ReportQueueServices>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IReportRepository, ReportRepository>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IReportingCenterSchemaService, ReportingCenterSchemaService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());


            //Reporting Center Database Interfaces
            container.RegisterType<IGenerateSchemaService, GenerateSchemaService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IReportingDataService, ReportingDataService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());

            //Email Notification Framework Interfaces
            container.RegisterType<IEmailNotificationService, EmailNotificationService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());

            //ML Import Interfaces
            container.RegisterType<IMLImportHelperService, MLImportHelperService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IBaselineMasterListService, BaselineMasterListService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());

            //Hangfire interfaces
            container.RegisterType<IBackgroundJobConfiguration, BackgroundJobConfiguration>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IConfiguration, HangfireConfiguration>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());

            container.RegisterType<IBackgroundJobServerFactory, HangfireBackgroundJobServer>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());

            container.RegisterType<ILogProviderFactory, HangfireLogProviderFactory>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());

            container.RegisterType<IReportExecuter<BaseJobInfo>, ExcelReportExecuter<BaseJobInfo>>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IReportEnqueueService, ReportingEnqueueService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IBackgroundJob<ReportQueueInfo>, BackgroundJob<ReportQueueInfo>>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IBackgroundJobManager, HangfireBackgroundJobManager>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IJobActivator, JobActivatorNotImplemented>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IMasterListCascadeEnqueueService, MasterListCascadeEnqueueService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IReportingDBEnqueueService, ReportingDBSaveAfterMLCascadeService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IBackgroundJob<MasterListCascadeQueueInfo>, BackgroundJob<MasterListCascadeQueueInfo>>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<ITaskListService, TaskListService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IWorkFlowTaskMappingService, WorkFlowTaskMappingService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IPlanTaskUserMappingService, PlanTaskUserMappingService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IQhpDataAdapterServices, QhpDataAdapterService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IQhpService, QhpService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IQhpIntegrationService, QhpIntegrationService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IQHPReportQueueManager, QHPReportQueueManager>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IExitValidateEnqueueService, ExitValidateEnqueueService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IExitValidateService, ExitValidateService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());

            //Configure policy -  this sets rule as set in the config file
            container.Configure<Interception>().AddPolicy("logMethodDuration")
                                                .AddMatchingRule<EnabledMatchingRule>(
                                                new InjectionConstructor(new InjectionParameter(isInterceptionEnabled)))
                                                .AddCallHandler<LoggingCallHandler>(new ContainerControlledLifetimeManager(), new InjectionConstructor(), first);

            container.RegisterType<IMLImportHelperService, MLImportHelperService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IResourceLockService, ResourceLockService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType <ISectionLockService, SectionLockService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());

            container.RegisterType<ISettingRepository, SettingRepository>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<ISettingProvider, SettingProvider>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<ISettingManager, SettingManager>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<INotificationService, NotificationService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IMDMSyncDataService, MDMSyncDataService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<IExportPreQueueService, ExportPreQueueService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            container.RegisterType<ICCRTranslatorService, CCRTranslatorService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
container.RegisterType<IRulesManagerService, RulesManagerService>(new InterceptionBehavior<PolicyInjectionBehavior>(), new Interceptor<InterfaceInterceptor>());
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            var unityHubActivator = new UnityHubActivator(container);
            GlobalHost.DependencyResolver.Register(typeof(IHubActivator), () => unityHubActivator);
        }

        public static T Resolve<T>()
        {
            return container.Resolve<T>();
        }

    }

    public class UnityHubActivator : IHubActivator
    {
        private readonly IUnityContainer _container;

        public UnityHubActivator(IUnityContainer container)
        {
            _container = container;
        }

        public IHub Create(HubDescriptor descriptor)
        {
            return (IHub)_container.Resolve(descriptor.HubType);
        }
    }

    //Custom rule  class
    public class EnabledMatchingRule : IMatchingRule
    {
        private bool isEnabled;

        public EnabledMatchingRule(bool isEnabled)
        {
            this.isEnabled = isEnabled;
        }

        public bool Matches(System.Reflection.MethodBase member)
        {
            return isEnabled;
        }
    }
}