using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace tmg.equinox.web.App_Start
{
    public class RouteConfig
    {
        public const string SBCREPORT = "SBCReport";
        public const string FaxBackReport = "FaxBackReport";
        public const string BenefitMatrixReport = "BenefitMatrixReport";
        public const string VisionMatrixReport = "VisionMatrixReport";
        public const string VisionFaxBackReport = "VisionFaxBackReport";
        public const string DentalMatrixReport = "DentalMatrixReport";
        public const string STDMatrixReport = "STDMatrixReport";
        public const string DentalFaxBackMatrixReport = "DentalFaxBackMatrixReport";
        public const string SPDReport = "SPDReport";
        public const string BenefitAdministrationMatrix = "BenefitAdministrationMatrixReport";
        public const string reportUrlParameters = "/{accountId}/{formInstanceId}/{tenantId}/{adminFormInstanceId}/{folderVersionId}/";

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();
            routes.MapPageRoute(RouteConfig.SBCREPORT, RouteConfig.SBCREPORT+"/{accountId}/{formInstanceId}/{tenantId}/{adminFormInstanceId}", "~/Report/SBCReport.aspx");
            routes.MapPageRoute(RouteConfig.FaxBackReport, RouteConfig.FaxBackReport + reportUrlParameters, "~/Report/FaxBackReport.aspx");
            routes.MapPageRoute(RouteConfig.BenefitMatrixReport, RouteConfig.BenefitMatrixReport + reportUrlParameters, "~/Report/BenefitMatrixReport.aspx");
            routes.MapPageRoute(RouteConfig.VisionMatrixReport, RouteConfig.VisionMatrixReport + reportUrlParameters, "~/Report/VisionMatrixReport.aspx");
            routes.MapPageRoute(RouteConfig.VisionFaxBackReport, RouteConfig.VisionFaxBackReport + reportUrlParameters, "~/Report/VisionFaxBackReport.aspx");
            routes.MapPageRoute(RouteConfig.DentalMatrixReport, RouteConfig.DentalMatrixReport + reportUrlParameters, "~/Report/DentalMatrixReport.aspx");
            routes.MapPageRoute(RouteConfig.STDMatrixReport, RouteConfig.STDMatrixReport + reportUrlParameters, "~/Report/STDMatrixReport.aspx");
            routes.MapPageRoute(RouteConfig.DentalFaxBackMatrixReport, RouteConfig.DentalFaxBackMatrixReport + reportUrlParameters, "~/Report/DentalFaxBackMatrixReport.aspx");
            routes.MapPageRoute(RouteConfig.SPDReport, RouteConfig.SPDReport + reportUrlParameters, "~/Report/SPDReport.aspx");
            routes.MapPageRoute(RouteConfig.BenefitAdministrationMatrix, RouteConfig.BenefitAdministrationMatrix + reportUrlParameters, "~/Report/BenefitAdministrationMatrix.aspx");
            routes.IgnoreRoute("{*allaspx}", new { allaspx = @".*\.aspx(/.*)?" });

            //route Map for hiding the URL parameters
            routes.MapRoute(
                name: "SBCReportURL",
                url: "Report/SBCReport/{accountId}/{formInstanceId}/{tenantId}/{adminFormInstanceId}",
                defaults: new
                {
                    controller = "Report",
                    action = "SBCReport",
                    accountId = UrlParameter.Optional,
                    formInstanceId = UrlParameter.Optional,
                    tenantId = UrlParameter.Optional,
                    adminFormInstanceId = UrlParameter.Optional
                }
            );

            routes.MapRoute(
                name: "FaxBackReportURL",
                url: "Report/Reports/{accountId}/{formInstanceId}/{tenantId}/{adminFormInstanceId}/{ReportName}/{folderVersionId}",
                defaults: new
                {
                    controller = "Report",
                    action = "Reports",
                    accountId = UrlParameter.Optional,
                    formInstanceId = UrlParameter.Optional,
                    tenantId = UrlParameter.Optional,
                    adminFormInstanceId = UrlParameter.Optional,
                    ReportName = UrlParameter.Optional,
                    folderVersionId = UrlParameter.Optional
                }
            );

            routes.MapRoute(
                name: "BenefitMatrixReportURL",
                url: "Report/Reports/{accountId}/{formInstanceId}/{tenantId}/{adminFormInstanceId}/{ReportName}/{folderVersionId}",
                defaults: new
                {
                    controller = "Report",
                    action = "Reports",
                    accountId = UrlParameter.Optional,
                    formInstanceId = UrlParameter.Optional,
                    tenantId = UrlParameter.Optional,
                    adminFormInstanceId = UrlParameter.Optional,
                    ReportName = UrlParameter.Optional,
                    folderVersionId = UrlParameter.Optional
                }
            );

            routes.MapRoute(
                name: "VisionMatrixReportURL",
                url: "{controller}/{action}/{accountId}/{formInstanceId}/{tenantId}/{adminFormInstanceId}/{ReportName}/{folderVersionId}",
                defaults: new
                {
                    controller = "Report",
                    action = "Reports",
                    accountId = UrlParameter.Optional,
                    formInstanceId = UrlParameter.Optional,
                    tenantId = UrlParameter.Optional,
                    adminFormInstanceId = UrlParameter.Optional,
                    ReportName = UrlParameter.Optional,
                    folderVersionId = UrlParameter.Optional
                }
            );

            routes.MapRoute(
               name: "VisionFaxBackReportURL",
               url: "Report/Reports/{accountId}/{formInstanceId}/{tenantId}/{adminFormInstanceId}/{ReportName}/{folderVersionId}",
               defaults: new
               {
                   controller = "Report",
                   action = "Reports",
                   accountId = UrlParameter.Optional,
                   formInstanceId = UrlParameter.Optional,
                   tenantId = UrlParameter.Optional,
                   adminFormInstanceId = UrlParameter.Optional,
                   ReportName = UrlParameter.Optional,
                   folderVersionId = UrlParameter.Optional
               }
           );

            routes.MapRoute(
               name: "DentalMatrixReportURL",
               url: "{controller}/{action}/{accountId}/{formInstanceId}/{tenantId}/{adminFormInstanceId}/{ReportName}/{folderVersionId}",
               defaults: new
               {
                   controller = "Report",
                   action = "Reports",
                   accountId = UrlParameter.Optional,
                   formInstanceId = UrlParameter.Optional,
                   tenantId = UrlParameter.Optional,
                   adminFormInstanceId = UrlParameter.Optional,
                   ReportName = UrlParameter.Optional,
                   folderVersionId = UrlParameter.Optional
               }
           );

            routes.MapRoute(
             name: "STDMatrixReportURL",
             url: "{controller}/{action}/{accountId}/{formInstanceId}/{tenantId}/{adminFormInstanceId}/{ReportName}/{folderVersionId}",
             defaults: new
             {
                 controller = "Report",
                 action = "Reports",
                 accountId = UrlParameter.Optional,
                 formInstanceId = UrlParameter.Optional,
                 tenantId = UrlParameter.Optional,
                 adminFormInstanceId = UrlParameter.Optional,
                 ReportName = UrlParameter.Optional,
                 folderVersionId = UrlParameter.Optional
             }
         );

            routes.MapRoute(
               name: "DentalFaxBackMatrixReportURL",
               url: "{controller}/{action}/{accountId}/{formInstanceId}/{tenantId}/{adminFormInstanceId}/{ReportName}/{folderVersionId}",
               defaults: new
               {
                   controller = "Report",
                   action = "Reports",
                   accountId = UrlParameter.Optional,
                   formInstanceId = UrlParameter.Optional,
                   tenantId = UrlParameter.Optional,
                   adminFormInstanceId = UrlParameter.Optional,
                   ReportName = UrlParameter.Optional,
                   folderVersionId = UrlParameter.Optional
               }
           );

            routes.MapRoute(
            name: "SPDReportURL",

            url: "{controller}/{action}/{accountId}/{formInstanceId}/{tenantId}/{adminFormInstanceId}/{ReportName}/{folderVersionId}",
            defaults: new
            {
                controller = "Report",
                action = "Reports",
                accountId = UrlParameter.Optional,
                formInstanceId = UrlParameter.Optional,
                tenantId = UrlParameter.Optional,
                adminFormInstanceId = UrlParameter.Optional,
                ReportName = UrlParameter.Optional,
                folderVersionId = UrlParameter.Optional
            });


            routes.MapRoute(
               name: "BenefitAdministrationMatrixReportURL",
               url: "{controller}/{action}/{accountId}/{formInstanceId}/{tenantId}/{adminFormInstanceId}/{ReportName}/{folderVersionId}",
               defaults: new
               {
                   controller = "Report",
                   action = "Reports",
                   accountId = UrlParameter.Optional,
                   formInstanceId = UrlParameter.Optional,
                   tenantId = UrlParameter.Optional,
                   adminFormInstanceId = UrlParameter.Optional,
                   ReportName = UrlParameter.Optional,
                   folderVersionId = UrlParameter.Optional
               }
           );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
              defaults: new { controller = "Account", action = "LogOn", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "NotFound",
                url: "{*url}",
                defaults: new { controller = "Error", action = "PageNotFound" }
            );
        }

    }
}