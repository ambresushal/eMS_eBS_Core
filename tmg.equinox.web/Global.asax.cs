using System;
using System.Data.Entity.Infrastructure.Interception;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using tmg.equinox.backgroundjob;
using tmg.equinox.dependencyresolution;
using tmg.equinox.infrastructure.exceptionhandling;
using tmg.equinox.queueprocess.EmailNotification;
using tmg.equinox.queueprocess.EmailNotificationQueue;
//using tmg.equinox.queueprocess.OnDemandMigrationQueue;
using tmg.equinox.reportingprocess.NotifyTaskAssignment;
using tmg.equinox.web.Controllers;
using tmg.equinox.queueprocess.QHPReportQueue;

namespace tmg.equinox.web.App_Start
{
    public class MvcApplication : System.Web.HttpApplication
    {

        protected void Application_BeginRequest()
        {

        }

        protected void Application_Start()
        {
            Kentor.AuthServices.Configuration.Options.GlobalEnableSha256XmlSignatures();

            AreaRegistration.RegisterAllAreas();
            UnityConfig.RegisterComponents();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            if (AuditConfig.EnableEntityFrameworkQueryLog)
            {
                DbInterception.Add(new CommandInterceptor());
            }
            QhpConfig.InitializeQhpConfigSettings();

            //Add to Hangfire Queue
            IBackgroundJobManager hangFireJobManager = UnityConfig.Resolve<IBackgroundJobManager>();

            if (hangFireJobManager != null)
            {
                EmailEnqueue emailEnqueue = new EmailEnqueue(hangFireJobManager);
                EmailNotificationQueue emailNotificationQueue = new EmailNotificationQueue(hangFireJobManager);
                QHPReportQueue qhpReportQueue = new QHPReportQueue(hangFireJobManager);

                if (emailEnqueue != null)
                {
                    //schedule the job in hangfire which will send email  notification if workflow is in PSOT Preparation State for more than 7 days
                    emailEnqueue.Enqueue(new EmailQueueInfo { Name = "PSOTPreparationStateEmailNotification" });
                }

                if (emailNotificationQueue != null)
                    emailNotificationQueue.ExecuteEmailNotificationQueue(new EmailNotificationQueueInfo { Name = "ExecuteEmailNotificationQueue" });

                if (qhpReportQueue != null)
                    qhpReportQueue.ExecuteQHPReportQueue(new QHPReportQueueInfo { Name = "ExecuteQHPReportQueue" });
            }
            if (hangFireJobManager != null)
            {
                NotifyTaskAssignment notifyTaskEnqueue = new NotifyTaskAssignment(hangFireJobManager);
                NotifyTaskAssignmentService notifyTaskAssignmentService = new NotifyTaskAssignmentService(hangFireJobManager);

                if (notifyTaskEnqueue != null)
                {
                    //schedule the job in hangfire which will send email  notification if workflow is in PSOT Preparation State for more than 7 days
                    notifyTaskEnqueue.Enqueue(new NotifyTaskAssignmentInfo { Name = "NotifyTaskAssignmentInfo" });
                }
                if (notifyTaskAssignmentService != null)
                {
                    notifyTaskAssignmentService.CreateJob(new NotifyTaskAssignmentInfo { Name = "NotifyTaskAssignmentInfo" });
                }

            }
            if (hangFireJobManager != null)
            {
                //PBPImportEnqueue pBPImportEnqueue = new PBPImportEnqueue(hangFireJobManager);
                //PBPImportEmailNotificationQueueService pBPImportEmailNotificationQueueService = new PBPImportEmailNotificationQueueService(hangFireJobManager);
                //if (pBPImportEnqueue != null)
                //{
                //    //schedule the job in hangfire which will send email  notification if workflow is in PSOT Preparation State for more than 7 days
                //    pBPImportEnqueue.Enqueue(new PBPImportQueueInfo { Name = "PBPImportEmailNotification" });
                //}
                //
                //if (pBPImportEmailNotificationQueueService != null)
                //    pBPImportEmailNotificationQueueService.CreateJob(new PBPImportEmailNotificationQueueInfo { Name = "PBPImportEmailNotification" });
            }

            //CachingManager.InitializeCache();
        }

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            /*HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                IdentityManager.ExecutePostAuthenticate(authCookie);
            }*/
        }
        //needed for handling the unhandled exceptions/errors and returning a view to the user as with the filters the 404 is not handled
        protected void Application_Error(object sender, EventArgs e)
        {
            try
            {
                var httpContext = ((MvcApplication)sender).Context;
                var currentController = string.Empty;
                var currentAction = string.Empty;
                var currentRouteData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));

                if (currentRouteData != null)
                {
                    if (currentRouteData.Values["controller"] != null && !String.IsNullOrEmpty(currentRouteData.Values["controller"].ToString()))
                    {
                        currentController = currentRouteData.Values["controller"].ToString();
                    }

                    if (currentRouteData.Values["action"] != null && !String.IsNullOrEmpty(currentRouteData.Values["action"].ToString()))
                    {
                        currentAction = currentRouteData.Values["action"].ToString();
                    }
                }

                var ex = Server.GetLastError();
                //Providing an ability to log this unhandled exception too

                bool rethrow = ExceptionPolicyWrapper.HandleExceptionAsync(ex, ExceptionPolicies.ExceptionShielding);

                var controller = new ErrorController();
                var routeData = new RouteData();
                var action = "Index";

                if (ex is HttpException)
                {
                    var httpEx = ex as HttpException;

                    switch (httpEx.GetHttpCode())
                    {
                        case 404:
                        case 400:
                            action = "PageNotFound";
                            break;

                            // others if any
                    }
                }

                httpContext.ClearError();
                Server.ClearError();
                httpContext.Response.Clear();
                httpContext.Response.StatusCode = ex is HttpException ? ((HttpException)ex).GetHttpCode() : 500;
                httpContext.Response.TrySkipIisCustomErrors = true;

                routeData.Values["controller"] = "Error";
                routeData.Values["action"] = action;

                controller.ViewData.Model = new HandleErrorInfo(ex, currentController, currentAction);
                ((IController)controller).Execute(new RequestContext(new HttpContextWrapper(httpContext), routeData));
            }
            catch
            {
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            if (HttpContext.Current.Session["CurrrentSessionId"] == null)
            {
                HttpContext.Current.Session.Add("CurrrentSessionId", Guid.NewGuid().ToString());
            }
        }
    }
}
