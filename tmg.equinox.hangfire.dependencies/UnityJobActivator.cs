using Hangfire;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using tmg.equinox.applicationservices;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.masterlistcascade;
using tmg.equinox.applicationservices.PBPImport;
using tmg.equinox.backgroundjob;
using tmg.equinox.backgroundjob.Base;
using tmg.equinox.core.masterlistcascade;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.integration.qhplite;
//using tmg.equinox.infrastructure.logging;
using tmg.equinox.pbpimport;
using tmg.equinox.pbpimport.Interfaces;
using tmg.equinox.queueprocess.EmailNotification;
using tmg.equinox.queueprocess.EmailNotificationQueue;
using tmg.equinox.queueprocess.exitvalidate;
using tmg.equinox.queueprocess.masterlistcascade;
using tmg.equinox.queueprocess.PBPImport;
using tmg.equinox.queueprocess.QHPReportQueue;
//using tmg.equinox.queueprocess.QHPReportQueue;
using tmg.equinox.queueprocess.reporting;
using tmg.equinox.reporting;
using tmg.equinox.reporting.Base;
using tmg.equinox.reporting.Base.Interface;
using tmg.equinox.reporting.Base.Mapping;
using tmg.equinox.reportingprocess.NotifyTaskAssignment;
using tmg.equinox.repository;
using tmg.equinox.repository.interfaces;
using tmg.equinox.repository.models;
using tmg.equinox.savetoreportingdbmlcascade;

namespace tmg.equinox.hangfire.dependencies
{
    public class UnityJobActivator : JobActivator, IJobActivator
    {
        private readonly IUnityContainer _hangfireContainer;

        public UnityJobActivator(IUnityContainer hangfireContainer)
        {
            _hangfireContainer = hangfireContainer;
            _hangfireContainer.RegisterType<IWorkFlowVersionStatesAccessService, WorkFlowVersionStatesAccessService>(new PerResolveLifetimeManager());
            _hangfireContainer.RegisterType<IWorkFlowStateServices, WorkFlowStateServices>(new PerResolveLifetimeManager());
            _hangfireContainer.RegisterType<IReportConfig, ReportConfig>(new PerResolveLifetimeManager());
            _hangfireContainer.RegisterType<IDbRepostory, ReportDbRepository>(new PerResolveLifetimeManager());
            _hangfireContainer.RegisterType<IMapper<IList<ReportMappingField>>, Mapper>(new PerResolveLifetimeManager());
            _hangfireContainer.RegisterType<IMapper<string>, SQLMapper>(new PerResolveLifetimeManager());
            _hangfireContainer.RegisterType<IReportManager<BaseJobInfo>, ReportManager<BaseJobInfo>>(new PerResolveLifetimeManager());
            _hangfireContainer.RegisterType<IMasterListCascadeManager<BaseJobInfo>, MasterListCascadeManager<BaseJobInfo>>(new PerResolveLifetimeManager());
            _hangfireContainer.RegisterType<IReportDBSaveManager<BaseJobInfo>, ReportDBSaveManager<BaseJobInfo>>(new PerResolveLifetimeManager());
            _hangfireContainer.RegisterType<IRptUnitOfWorkAsync, RptUnitOfWork>(new PerResolveLifetimeManager());
            _hangfireContainer.RegisterType<IBackgroundJob<ReportQueueInfo>, ReportBackgroundJob>(new PerResolveLifetimeManager());
            _hangfireContainer.RegisterType<IUnitOfWorkAsync, UnitOfWork>(new PerResolveLifetimeManager());
            _hangfireContainer.RegisterType<IBackgroundJob<PBPImportQueueInfo>, PBPImportBackgroundJob>(new PerResolveLifetimeManager());
            _hangfireContainer.RegisterType<IBackgroundJob<EmailQueueInfo>, EmailBackgroundJob>(new PerResolveLifetimeManager());
            _hangfireContainer.RegisterType<IBackgroundJob<MasterListCascadeQueueInfo>, MasterListCascadeBackgroundJob>(new PerResolveLifetimeManager());
            _hangfireContainer.RegisterType<IPBPImportService, PBPImportService>(new PerResolveLifetimeManager());
            _hangfireContainer.RegisterType<IPBPImportActivityLogServices, PBPImportActivityLogServices>(new PerResolveLifetimeManager());
            _hangfireContainer.RegisterType<IPBPImportHelperServices, PBPImportHelperServices>(new PerResolveLifetimeManager());
            _hangfireContainer.RegisterType<IPBPMappingServices, PBPMappingServices>(new PerResolveLifetimeManager());
            _hangfireContainer.RegisterType<IFormDesignService, FormDesignService>(new PerResolveLifetimeManager());
            _hangfireContainer.RegisterType<IFormInstanceDataServices, FormInstanceDataServices>(new PerResolveLifetimeManager());
            _hangfireContainer.RegisterType<IFolderVersionServices, FolderVersionServices>(new PerResolveLifetimeManager());
            _hangfireContainer.RegisterType<ILoggingService, LoggingService>(new PerResolveLifetimeManager());
            _hangfireContainer.RegisterType<IDomainModelService, DomainModelService>(new PerResolveLifetimeManager());
            _hangfireContainer.RegisterType<IWorkFlowCategoryMappingService, WorkFlowCategoryMappingService>(new PerResolveLifetimeManager());
            _hangfireContainer.RegisterType<IAutoSaveSettingsService, AutoSaveSettingsService>(new PerResolveLifetimeManager());
            _hangfireContainer.RegisterType<ISQLImportOperations, SqlImportOperations>(new PerResolveLifetimeManager());
            _hangfireContainer.RegisterType<IAccessDbContext, AccessDbContext>(new PerResolveLifetimeManager());
            _hangfireContainer.RegisterType<infrastructure.logging.ILog, infrastructure.logging.Logger>(new PerResolveLifetimeManager());
            _hangfireContainer.RegisterType<IPBPExportServices, PBPExportService>(new PerResolveLifetimeManager());
            _hangfireContainer.RegisterType<IMasterListCascadeService, MasterListCascadeService>(new PerResolveLifetimeManager());
            _hangfireContainer.RegisterType<IPBPImportEmailNotificationService, PBPImportEmailNotificationService>(new PerResolveLifetimeManager());
            _hangfireContainer.RegisterType<IFormInstanceService, FormInstanceService>(new PerResolveLifetimeManager());
            _hangfireContainer.RegisterType<IMasterListService, MasterListService>(new PerResolveLifetimeManager());
            _hangfireContainer.RegisterType<IUIElementService, UIElementService>(new PerResolveLifetimeManager());
            _hangfireContainer.RegisterType<IEmailNotificationQueueService, EmailNotificationQueueService>(new PerResolveLifetimeManager());
            _hangfireContainer.RegisterType<INotifyTaskAssignmentService, NotifyTaskAssignmentService>(new PerResolveLifetimeManager());
            _hangfireContainer.RegisterType<IQHPReportQueueService, QHPReportQueueService>(new PerResolveLifetimeManager());
            _hangfireContainer.RegisterType<IQHPReportQueueManager, QHPReportQueueManager>(new PerResolveLifetimeManager());
            
            _hangfireContainer.RegisterType<IBackgroundJob<ExitValidateQueueInfo>, ExitValidateBackgroundJob>(new PerResolveLifetimeManager());
            _hangfireContainer.RegisterType<IBackgroundJob<ExitValidateQueueInfo>, ExitValidateBackgroundJobLowPriority>(new PerResolveLifetimeManager());
            _hangfireContainer.RegisterType<IExitValidateService, ExitValidateService>(new PerResolveLifetimeManager());

        }

        public override object ActivateJob(Type type)
        {
            return _hangfireContainer.Resolve(type);
        }

        public T Resolve<T>()
        {
            return _hangfireContainer.Resolve<T>();
        }
    }
}
