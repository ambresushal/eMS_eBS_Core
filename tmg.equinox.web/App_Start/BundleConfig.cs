using System.Web.Optimization;
using BundleTransformer.Core.Builders;
using BundleTransformer.Core.Orderers;
using BundleTransformer.Core.Resolvers;
using BundleTransformer.Core.Transformers;
using BundleTransformer.Core.Bundles;
using System;
using System.Configuration;

namespace tmg.equinox.web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {

            RegisterJavascriptBundlesToMinify(bundles);
            //RegisterJavascriptBundlesToUglify(bundles);
            RegisterStyleBundles(bundles);
#if DEBUG
            BundleTable.EnableOptimizations = false;
#else
            BundleTable.EnableOptimizations = false;
#endif
        }
        private static void RegisterStyleBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/css")
                            .Include("~/Content/bootstrap.css")
                            .Include("~/Content/bootstrap-responsive.css")
                            .Include("~/Content/site.css"));

            bundles.Add(new StyleBundle("~/kendo")
                            .Include("~/Content/css/DatabaseUI/kendo.common.min.css")
                            .Include("~/Content/css/DatabaseUI/kendo.default.min.css")
                            .Include("~/Content/css/DatabaseUI/kendo.dataviz.min.css")
                            .Include("~/Content/css/DatabaseUI/kendo.mobile.all.min.css"));
        }

        /// <summary>
        /// Method is for Minify the Javascripts file
        /// </summary>
        /// <param name="bundles"></param>
        private static void RegisterJavascriptBundlesToMinify(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                  "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jQueryBundle")
                            .Include("~/Scripts/Framework/External/jquery-1.11.0.js")
                            .Include("~/Scripts/Framework/External/jquery-ui-1.9.2.custom.min.js")
                            .Include("~/Scripts/Framework/External/jquery.inputmask.js")
                            .Include("~/Scripts/Framework/External/jquery.inputmask.extensions.js")
                            .Include("~/Scripts/Framework/External/jquery.inputmask.numeric.extensions.js")
                            .Include("~/Scripts/Framework/External/jquery.inputmask.regex.extensions.js")
                            .Include("~/Scripts/Framework/External/jquery.autocomplete.extensions.js")
                            .Include("~/Scripts/Framework/External/Notification/jquery.signalR-2.2.2.min.js")
                            .Include("~/Scripts/Framework/External/Notification/Notify.js")
                            );

            bundles.Add(new ScriptBundle("~/bundles/GridBundle")
                            .Include("~/Scripts/Framework/External/jquery.jqGrid.js")
                            .Include("~/Scripts/Framework/External/grid.locale-en.js")
                            .Include("~/Scripts/Framework/External/handsontable.full.js"));

            bundles.Add(new ScriptBundle("~/bundles/PqGridBundle")
                            .Include("~/Scripts/Framework/External/PQGrid/pqgrid.min.js")
                            .Include("~/Scripts/Framework/External/PQGrid/pqselect.min.js")
                            .Include("~/Scripts/Framework/External/PQGrid/touch-punch.min.js")
                            .Include("~/Scripts/Framework/External/PQGrid/jszip.min.js")
                            .Include("~/Scripts/Framework/External/bootstrap-multiselect.0.9.13.js")
                             //  .Include("~/Scripts/Framework/External/ag-grid.min.js")
                             //   .Include("~/Scripts/Framework/External/ag-grid.noStyle.js"));

                             .Include("~/Scripts/Framework/External/ag-grid-enterprise.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/JsRenderBundle")

             .Include("~/Scripts/Framework/External/jsrender.js").Include("~/Scripts/Framework/External/jsrender.extension.js")

                         );

            bundles.Add(new ScriptBundle("~/bundles/DialogBundle")
               .Include("~/Scripts/Framework/Internal/dialogs.js"));

            bundles.Add(new ScriptBundle("~/bundles/MLCascadeBundle")
               .Include("~/Scripts/MasterList/cascademasterList.js"));

            bundles.Add(new ScriptBundle("~/bundles/AjaxBundle")
                        .Include("~/Scripts/Framework/Internal/ajaxnotifier.js")
                        .Include("~/Scripts/Framework/Internal/ajaxwrapper.js"));

            bundles.Add(new ScriptBundle("~/bundles/GlobalBundle")
                       .Include("~/Scripts/Framework/External/linq.js")
                       .Include("~/Scripts/Framework/Internal/globalvariables.js")
                       .Include("~/Scripts/Framework/Internal/messages.js")
                       .Include("~/Scripts/Framework/Internal/floatingsectionheaders.js")
                       .Include("~/Scripts/ErrorLog/errorhandler.js")
                       .Include("~/Scripts/Framework/Internal/globalutilities.js")
                       .Include("~/Scripts/Framework/Internal/gridutilities.js")
                       .Include("~/Scripts/Framework/Internal/jqGridtoCsv.js")
                       .Include("~/Scripts/RoutingTask/routingTaskDialog.js")
                      .Include("~/Scripts/Framework/Internal/agGridtoCsv.js")
                       .Include("~/Scripts/NotificationStatus/Notification.js")
                       .Include("~/Scripts/Framework/Internal/ajaxwrapper.js")
                       .Include("~/Scripts/Framework/Internal/dialogs.js")
                       .Include("~/Scripts/Framework/Internal/locktimer.js")
                       .Include("~/Scripts/SectionLockPopUp/SectionLockPopUp.js"));

            bundles.Add(new ScriptBundle("~/bundles/LayoutBundle")
                       .Include("~/Scripts/Layout/fontresizer.js")
                       .Include("~/Scripts/Layout/layout.js")
                       .Include("~/Scripts/DashBoard/globalClaims.js")
                       .Include("~/Scripts/layout/tmg-layout.js")
                       .Include("~/Scripts/NotificationStatus/Notification.js"));


            var expresionBuilder = string.Format("~/Scripts/FolderVersion/expressionBuilderExtension{0}.js", GetApplicationName());



            bundles.Add(new ScriptBundle("~/bundles/FolderVersionBundle")
                      .Include("~/Scripts/Framework/External/tinymce/plugins/lance/js/annotationsui.min.js")
                      .Include("~/Scripts/Framework/External/tinymce/plugins/lance/js/app.min.js")
                      .Include("~/Scripts/Framework/External/tinymce/plugins/lance/js/jquery.autogrow.min.js")
                      .Include("~/Scripts/Framework/Internal/table.hilight.js")
                      .Include("~/Scripts/FolderVersion/repeaterBuilderAG.js")
                      .Include("~/Scripts/FolderVersion/ruleProcessor.js")
                      .Include("~/Scripts/FolderVersion/repeaterRuleProcessor.js")
                      .Include("~/Scripts/FolderVersion/createformdialog.js")
                      .Include("~/Scripts/FolderVersion/openDocumentDialog.js")
                      .Include("~/Scripts/FolderVersion/errorManager.js")
                      .Include("~/Scripts/FolderVersion/viewImpactReport.js")
                      .Include("~/Scripts/FolderVersion/annotationManager.js")
                      .Include("~/Scripts/FolderVersion/tabmanager.js")
                      .Include("~/Scripts/FolderVersion/sotmanager.js")
                      .Include("~/Scripts/FolderVersion/folderManager.js")
                      .Include("~/Scripts/FolderVersion/folderstatus.js")
                      .Include("~/Scripts/FolderVersion/folderVersion.js")
                      .Include("~/Scripts/FolderVersion/printPreview.js")
                      .Include("~/Scripts/FolderVersion/folderVersionWorkflow.js")
                      .Include("~/Scripts/FolderVersion/formvalidationmanager.js")
                      .Include("~/Scripts/FolderVersion/formInstanceBuilder.js")
                      .Include("~/Scripts/FolderVersion/formInstanceBuilder.sot.js")
                      .Include("~/Scripts/FolderVersion/majorMinorVersion.js")
                      .Include("~/Scripts/FolderVersion/majorMinorVersionML.js")
                       .Include("~/Scripts/FolderVersion/repeaterdialog.js")
                       .Include("~/Scripts/FolderVersion/repeaterdialogAG.js")
                       .Include("~/Scripts/FolderVersion/repeaterCellDialog.js")
                      .Include("~/Scripts/FolderVersion/repeaterBuilderPQ.js")
                      .Include("~/Scripts/FolderVersion/repeaterdialog.js")
                      .Include("~/Scripts/FolderVersion/datasourcemanualmappingAG.js")
                      .Include("~/Scripts/FolderVersion/userSettingDialog.js")
                      .Include("~/Scripts/FolderVersion/validationManager.js")
                      .Include("~/Scripts/FolderVersion/versionHistory.js")
                      .Include("~/Scripts/FolderVersion/versionHistoryML.js")
                      .Include("~/Scripts/FormDesign/previewformdesign.js")
                      .Include("~/Scripts/FolderVersion/retroaccountdialog.js")
                      .Include("~/Scripts/FolderVersion/fieldMaskValidator.js")
                      .Include("~/Scripts/FolderVersion/datasourcesync.js")
                      .Include("~/Scripts/FolderVersion/journalEntry.js")
                      .Include("~/Scripts/FolderVersion/journalEntryDialog.js")
                      .Include("~/Scripts/FolderVersion/datasourcemanualmappingPQ.js")
                      .Include("~/Scripts/FolderVersion/childPopupManager.js")
                      .Include("~/Scripts/FolderVersion/journalManager.js")
                      .Include("~/Scripts/FolderVersion/journalReport.js")
                      .Include("~/Scripts/FolderVersion/productShareReport.js")
                      .Include("~/Scripts/PrintTemplate/pdfConfigurationDesign.js")
                      .Include("~/Scripts/FolderVersion/folderMember.js")
                      .Include("~/Scripts/FolderVersion/ebrules.js")
                      .Include("~/Scripts/FolderVersion/masterlist.js")
                      .Include("~/Scripts/FolderVersion/customrulePQ.js")
                      .Include("~/Scripts/FolderVersion/customruleHandler.js")
                      .Include("~/Scripts/FolderVersion/prefixReuse.js")
                      .Include("~/Scripts/FolderVersion/expressionbuilder.js")
                      .Include("~/Scripts/Framework/External/tinymce/tinymce.js")
                      .Include("~/Scripts/Framework/External/tinymce/jquery.tinymce.min.js")
                      .Include(expresionBuilder)
                      .Include("~/Scripts/FolderVersion/mlImport.js")
                      .Include("~/Scripts/FolderVersion/keybuilder.js")
                      .Include("~/Scripts/FolderVersion/formDesignVersionPreLoader.js")
                      .Include("~/Scripts/FolderVersion/elementeventextensions.js")
                      .Include("~/Scripts/FolderVersion/elementeventextensionsAG.js")
                      .Include("~/Scripts/Framework/Internal/query-builder.standalone.js")
                        .Include("~/Scripts/NotificationStatus/Notification.js")
                      .Include("~/Scripts/FolderVersion/exitValidate.js")
                      .Include("~/Scripts/FolderVersion/vbid.extension.js")
                      );

            bundles.Add(new ScriptBundle("~/bundles/FormDesignBundle")
                      .Include("~/Scripts/Framework/External/tinymce/plugins/lance/js/annotationsui.min.js")
                      .Include("~/Scripts/Framework/External/tinymce/plugins/lance/js/app.min.js")
                      .Include("~/Scripts/Framework/External/tinymce/plugins/lance/js/jquery.autogrow.min.js")
                      .Include("~/Scripts/FormDesign/customregexdialog.js")
                      .Include("~/Scripts/FormDesign/dataSourcedialog.js")
                      .Include("~/Scripts/FormDesign/dropdownitemsdialog.js")
                      .Include("~/Scripts/FormDesign/fieldlistdialog.js")
                      .Include("~/Scripts/FormDesign/paramadvancedconfigurationdialog.js")
                      .Include("~/Scripts/FormDesign/repeaterTemplateDialog.js")
                      .Include("~/Scripts/FormDesign/expressionRulesDialog.js")
                      .Include("~/Scripts/FormDesign/expressionRulesDialogNew.js")
                      .Include("~/Scripts/FormDesign/expressionRuleTester.js")
                      .Include("~/Scripts/FormDesign/formdesign.js")
                      .Include("~/Scripts/FormDesign/formdesignversion.js")
                      .Include("~/Scripts/FormDesign/formdesignview.default.js")
                      .Include("~/Scripts/FormDesign/formdesignview.config.js")
                      .Include("~/Scripts/FormDesign/formDesignActivityLogger.js")
                      .Include("~/Scripts/FormDesign/rulesdialog.js")

                      .Include("~/Scripts/FormDesign/rulesTesterDialog.js")
                      .Include("~/Scripts/FormDesign/rulesTesterProcessor.js")


                      .Include("~/Scripts/FormDesign/sectionlistdialog.js")
                      .Include("~/Scripts/FormDesign/expressionbuilder.js")
                      .Include("~/Scripts/FormDesign/uielementpropertygrid.js")
                      .Include("~/Scripts/FormDesign/previewformdesign.js")
                      .Include("~/Scripts/FormDesign/roleaccesspermission.js")
                      .Include("~/Scripts/FormDesign/duplicationCheckDialog.js")
                      .Include("~/Scripts/Framework/External/tinymce/tinymce.js")
                      .Include("~/Scripts/Framework/External/tinymce/jquery.tinymce.min.js")
                      .Include("~/Scripts/Framework/Internal/query-builder.standalone.js")
                      .Include("~/Scripts/DashBoard/dashboard.js")
                        .Include("~/Scripts/NotificationStatus/Notification.js")
                      );

            bundles.Add(new ScriptBundle("~/bundles/RulesManagerBundle")
                        .Include("~/Scripts/Framework/External/ag-grid-enterprise.min.js")
                        .Include("~/Scripts/FormDesign/rulesTesterDialog.js")
                        .Include("~/Scripts/FormDesign/rulesTesterProcessor.js")
                        .Include("~/Scripts/RulesManager/rulesmanager.js")
                        .Include("~/Scripts/RulesManager/gridbuilder.js")
                        .Include("~/Scripts/RulesManager/expressionbuilder.js")
                        .Include("~/Scripts/RulesManager/designer.js")
                        .Include("~/Scripts/RulesManager/assigntarget.js")
                        .Include("~/Scripts/RulesManager/assigntemplate.js")
                        .Include("~/Scripts/RulesManager/comparer.js")
                        .Include("~/Scripts/RulesManager/tester.js")
                        .Include("~/Scripts/RulesManager/analyzer.js")
                        .Include("~/Scripts/Framework/Internal/query-builder.standalone.js")
                        .Include("~/Scripts/Framework/External/Treant/raphael.js")
                        .Include("~/Scripts/Framework/External/Treant/jquery.mousewheel.js")
                        .Include("~/Scripts/Framework/External/Treant/jquery.easing.js")
                        .Include("~/Scripts/Framework/External/Treant/perfect-scrollbar.js")
                        .Include("~/Scripts/Framework/External/Treant/treant.js")
                        );

            bundles.Add(new ScriptBundle("~/bundles/ConsumerAccountBundle")
                            .Include("~/Scripts/ConsumerAccount/accountsearch.js")
                            .Include("~/Scripts/ConsumerAccount/manageaccount.js")
                            .Include("~/Scripts/ConsumerAccount/folderdialog.js")
                            .Include("~/Scripts/ConsumerAccount/portfoliosearch.js")
                            .Include("~/Scripts/Framework/Internal/globalutilities.js")
                            .Include("~/Scripts/Framework/Internal/gridutilities.js")
                            .Include("~/Scripts/DashBoard/dashboard.js")
                             // .Include("~/Scripts/FolderVersionReport/FolderVersionReport.js")
                             .Include("~/Scripts/NotificationStatus/Notification.js")
                           );

            bundles.Add(new ScriptBundle("~/bundles/ConsortiumBundle")
                           .Include("~/Scripts/Consortium/consortium.js"));

            bundles.Add(new ScriptBundle("~/bundles/DashboardBundle")
                .Include("~/Scripts/DashBoard/dashboard.js")
                .Include("~/Scripts/DashBoard/globalClaims.js")
                .Include("~/Scripts/Framework/Internal/globalutilities.js")
                .Include("~/Scripts/Framework/Internal/gridutilities.js")
                .Include("~/Scripts/NotificationStatus/Notification.js")

                //.Include("~/Scripts/FolderVersionReport/FolderVersionReport.js")
                );

            bundles.Add(new ScriptBundle("~/bundles/BootstrapBundle")
                            .Include("~/Scripts/Framework/External/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/LogOffBundle")
                .Include("~/Scripts/LogOff/logoff.js"));

            bundles.Add(new ScriptBundle("~/bundles/UtilsBundle")
                .Include("~/Scripts/Framework/Internal/uiblocker.js")
                .Include("~/Scripts/Framework/Internal/globalutilities.js")
                      .Include("~/Scripts/FormDesign/formUtilities.js")
                .Include("~/Scripts/Framework/Internal/jqGridtoCsv.js")
                .Include("~/Scripts/Framework/Internal/gridutilities.js")
                      .Include("~/Scripts/Framework/Internal/agGridtoCsv.js"));


            bundles.Add(new ScriptBundle("~/bundles/FormDesignGroupBundle")
               .Include("~/Scripts/FormGroupDesign/formdesigngroup.js")
                 );

            bundles.Add(new ScriptBundle("~/bundles/ActivityLoggerBundle")
                .Include("~/Scripts/Framework/Internal/ruleExecutionLogger.js")
              .Include("~/Scripts/Framework/Internal/activitylogger.js"));

            bundles.Add(new ScriptBundle("~/bundles/FolderVersionReportBundle")
                     .Include("~/Scripts/FolderVersionReport/folderVersionReport.js")
                     .Include("~/Scripts/Report/AuditChecklistReport.js")
                     .Include("~/Scripts/Framework/Internal/globalutilities.js"));

            bundles.Add(new ScriptBundle("~/bundles/QHPTemplateBundle")
                    .Include("~/Scripts/FolderVersionReport/qhptemplate.js")
                    .Include("~/Scripts/Framework/Internal/globalutilities.js"));

            bundles.Add(new ScriptBundle("~/bundles/Import")
                   .Include("~/Scripts/Facets/Import/import.js")
                   .Include("~/Scripts/Facets/Import/datacleanup.js")
                   .Include("~/Scripts/Facets/Import/createMasterList.js")
                   .Include("~/Scripts/Facets/Import/createProduct.js"));

            bundles.Add(new ScriptBundle("~/bundles/Scheduler")
               .Include("~/Scripts/Facets/Scheduler/scheduler.js")
               .Include("~/Scripts/Facets/Scheduler/scheduler.js")
               //.Include("~/Scripts/Facets/DocumentMapper/documentMapper.js")
               .Include("~/Scripts/Framework/Internal/globalutilities.js"));

            bundles.Add(new ScriptBundle("~/bundles/Translator")
                    .Include("~/Scripts/Facets/Translator/translator.js")
                    .Include("~/Scripts/Facets/Translator/history.js")
                    .Include("~/Scripts/Framework/Internal/globalutilities.js"));

            bundles.Add(new ScriptBundle("~/bundles/Transmitter")
                    .Include("~/Scripts/Facets/Transmitter/transmitter.js")
                    .Include("~/Scripts/Framework/Internal/globalutilities.js"));

            bundles.Add(new ScriptBundle("~/bundles/Configuration")
                    .Include("~/Scripts/Facets/Configuration/configuration.js")
                    .Include("~/Scripts/Framework/Internal/globalutilities.js"));

            bundles.Add(new ScriptBundle("~/bundles/SettingsBundle")
                   .Include("~/Scripts/Settings/settings.js")
                   .Include("~/Scripts//FolderVersion/workFlowSetting.js")
                   .Include("~/Scripts/Settings/userManagementSettings.js")
                   .Include("~/Scripts/Settings/customWorkflowSettings.js")
                   .Include("~/Scripts/Settings/masterWorkflowSettings.js")
                   .Include("~/Scripts/Settings/Tasklist.js")
                   .Include("~/Scripts/DashBoard/dashboard.js")
                   .Include("~/Scripts/Settings/WorkflowTaskMapping.js")
                    .Include("~/Scripts/NotificationStatus/Notification.js"));

            bundles.Add(new ScriptBundle("~/bundles/DocumentMatchBundle")
                .Include("~/Scripts/Framework/Internal/globalutilities.js")
                .Include("~/Scripts/FolderVersionReport/documentmatch.js"));

            bundles.Add(new ScriptBundle("~/bundles/PrintTemplateBundle")
            .Include("~/Scripts/PrintTemplate/pdfConfigurationDesign.js"));

            bundles.Add(new ScriptBundle("~/bundles/DocumentCollateralBundle")
            .Include("~/Scripts/DocumentCollateral/reportdesign.js")
            .Include("~/Scripts/DashBoard/dashboard.js")
                       .Include("~/Scripts/Framework/External/ag-grid-enterprise.min.js")

            );

            bundles.Add(new ScriptBundle("~/bundles/ResearchWorkstationBundle")
            .Include("~/Scripts/Facets/Research/search.js",
                     "~/Scripts/Facets/Research/details/details.js",
                     "~/Scripts/Facets/Research/compare/compare.js",
                     "~/Scripts/Facets/Research/details/sepyDetails.js",
                     "~/Scripts/Facets/Research/details/dedeDetails.js",
                     "~/Scripts/Facets/Research/details/ltltDetails.js",
                     "~/Scripts/Facets/Research/details/BSBSDetails.js",
                     "~/Scripts/Facets/Research/details/EBCLDetails.js",
                     "~/Scripts/Facets/Research/details/BSDLDetails.js",
                     "~/Scripts/Facets/Research/details/MLDetails.js"
                     ));

            bundles.Add(new ScriptBundle("~/bundles/CollateralBundle")
             .Include("~/Scripts/DocumentCollateral/CollateralTemplate.js")
             .Include("~/Scripts/DocumentCollateral/reportConfigurationGrid.js")
             .Include("~/Scripts/Framework/Internal/globalutilities.js")
             //.Include("~/Scripts/DocumentCollateral/generatedocument.js")
             .Include("~/Scripts/DocumentCollateral/parametersDialog.js")
             .Include("~/Scripts/DocumentCollateral/useractivity.js")
             .Include("~/Scripts/NotificationStatus/Notification.js")
              );

            bundles.Add(new ScriptBundle("~/bundles/DocumentSyncBundle")
            .Include("~/Scripts/Framework/External/jquery.steps.js",
                 "~/Scripts/DocumentSync/selectSourceDocument.js",
                 "~/Scripts/DocumentSync/setupCompare.js",
                 "~/Scripts/DocumentSync/setRepeaterCriteria.js",
                 "~/Scripts/DocumentSync/selectTargetDocuments.js",
                 "~/Scripts/DocumentSync/compareDocuments.js",
                 "~/Scripts/DocumentSync/syncDocuments.js",
                 "~/Scripts/DocumentSync/documentSync.js",
                 "~/Scripts/NotificationStatus/Notification.js",
                 "~/Scripts/DashBoard/dashboard.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/PBPImportBundle")
          .Include("~/Scripts/Framework/External/jquery.steps.js",
               "~/Scripts/PBPImport/pbpimport.js", "~/Scripts/PBPImport/PBPExport.js",
               "~/Scripts/PBPImport/pbpDatabase.js",
                "~/Scripts/PBPImport/odm.js",
                "~/Scripts/PBPImport/mdbComparer.js",
                "~/Scripts/DashBoard/dashboard.js",
                 "~/Scripts/NotificationStatus/Notification.js",
                "~/Scripts/PBPImport/exitValidateExport.js"
          ));

            bundles.Add(new ScriptBundle("~/bundles/CCRIntergarationBundles")
               .Include("~/Scripts/CCRIntegration/translationQueue.js")
               .Include("~/Scripts/CCRIntegration/tableDetails.js")
               .Include("~/Scripts/CCRIntegration/translatedPlan.js")
               .Include("~/Scripts/CCRIntegration/provisionDetails.js")
               .Include("~/Scripts/Framework/External/ag-grid-enterprise.min.js")               
                //.Include("~/Scripts/Framework/External/bootstrap-multiselect.0.9.13.js")
                );

            bundles.Add(new ScriptBundle("~/bundles/QueryManager")
                           .Include("~/Scripts/QueryManager/QueryManager.js"));

            bundles.Add(new ScriptBundle("~/bundles/ServiceVersionBundle")
               .Include("~/Scripts/ServiceDesign/servicedesign.js")
               .Include("~/Scripts/ServiceDesign/servicedesignversion.js")
               .Include("~/Scripts/ServiceDesign/servicedefinitionpropertygrid.js")
               .Include("~/Scripts/ServiceDesign/searchparameterdialog.js")
               .Include("~/Scripts/ServiceDesign/servicedesignversionoutput.js"));

            bundles.Add(new ScriptBundle("~/bundles/GlobalUpdateBundle")
            .Include("~/Scripts/GlobalUpdate/generateIAS.js")
            .Include("~/Scripts/GlobalUpdate/rulesdialoggu.js")
            .Include("~/Scripts/FormDesign/expressionbuilder.js")
            .Include("~/Scripts/Framework/Internal/messages.js")
            .Include("~/Scripts/Framework/Internal/globalutilities.js")
            .Include("~/Scripts/Framework/Internal/gridutilities.js")
            .Include("~/Scripts/GlobalUpdate/uiguelementpropertygrid.js")
            .Include("~/Scripts/Framework/External/jquery.bootstrap.wizard.js")
            .Include("~/Scripts/Framework/External/jquery.inputmask.js")
             .Include("~/Scripts/NotificationStatus/Notification.js")
            );


            bundles.Add(new ScriptBundle("~/bundles/ProductivityDashboardBundle")
            .Include("~/Scripts/ProductivityDashboard/productivityDashboard.js")
            .Include("~/Scripts/ProductivityDashboard/productivityDashboard_Grids.js")
            .Include("~/Scripts/Framework/Internal/globalutilities.js")
            .Include("~/Scripts/Framework/External/ProductivityDashboard/highcharts.js")
            .Include("~/Scripts/Framework/External/ProductivityDashboard/highcharts_exporting.js"));


            bundles.Add(new ScriptBundle("~/bundles/TinyMCEBundle")
                       .Include("~/Scripts/Framework/External/tinymce/tinymce.js")
                       .Include("~/Scripts/Framework/External/tinymce/jquery.tinymce.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/CodeMirrorBundle")
                    .Include("~/Scripts/Framework/External/codemirror/lib/codemirror.js")
                    .Include("~/Scripts/Framework/External/codemirror/lib/javascript.js")
                    .Include("~/Scripts/Framework/External/codemirror/lib/javascript-hint.js")
                    .Include("~/Scripts/Framework/External/codemirror/lib/fullscreen.js")
                    .Include("~/Scripts/Framework/External/codemirror/lib/closebrackets.js")
                    .Include("~/Scripts/Framework/External/codemirror/lib/show-hint.js"));

            bundles.Add(new ScriptBundle("~/bundles/beautifyBundle")
                       .Include("~/Scripts/Framework/External/beautify/beautify.js"));

            //        bundles.Add(new ScriptBundle("~/bundles/TinyMCEBundle")
            //.IncludeDirectory("~/Scripts/Framework/External/tinymce", "*.js", true));
            /*
               .Include("~/Scripts/Framework/External/tinymce/tinymce.js")
               .Include("~/Scripts/Framework/External/tinymce/src/rangy-core.js")
               .Include("~/Scripts/Framework/External/tinymce/src/polyfills.js")
               .Include("~/Scripts/Framework/External/tinymce/src/ice.js")
               .Include("~/Scripts/Framework/External/tinymce/src/dom.js")
               .Include("~/Scripts/Framework/External/tinymce/src/icePlugin.js")
               .Include("~/Scripts/Framework/External/tinymce/src/icePluginManager.js")
               .Include("~/Scripts/Framework/External/tinymce/src/bookmark.js")
               .Include("~/Scripts/Framework/External/tinymce/src/selection.js")
               .Include("~/Scripts/Framework/External/tinymce/src/plugins/IceAddTitlePlugin.js")
               .Include("~/Scripts/Framework/External/tinymce/src/plugins/IceCopyPastePlugin.js")
               .Include("~/Scripts/Framework/External/tinymce/src/plugins/IceEmdashPlugin.js")
               .Include("~/Scripts/Framework/External/tinymce/src/plugins/IceSmartQuotesPlugin.js")
               .Include("~/Scripts/Framework/External/tinymce/jquery.tinymce.min.js")
               .Include("~/Scripts/Framework/External/tinymce/plugins/flite/js/flite-interface.js"));*/



            bundles.Add(new ScriptBundle("~/bundles/ReportingCenterBundle")
                 .Include("~/Scripts/ReportingCenter/reportingcenter.js")
                 .Include("~/Scripts/DashBoard/dashboard.js")
                  .Include("~/Scripts/NotificationStatus/Notification.js")
                 );

            bundles.Add(new ScriptBundle("~/bundles/ReportingCenterDBUIBundle")
                .Include("~/Scripts/ReportingCenter/DatabaseUI/kendo.all.min.js")
                 .Include("~/Scripts/NotificationStatus/Notification.js")
                );

            bundles.Add(new ScriptBundle("~/bundles/MLBundle")
                .Include("~/Scripts/MasterList/masterList.js")
                );

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
           "~/Scripts/Framework/External/Notification/modernizr-*"));
        }

        private static string GetApplicationName()
        {
            if (tmg.equinox.config.Config.GetApplicationName().ToLower() == "ebenefitsync")
                return "-eBS";
            else
                return "-eMS";
        }

        private static string GetClientName()
        {

            var appNameSetting = ConfigurationManager.AppSettings["clientName"];
            string appName = "floridablue";
            if (appNameSetting != null)
            {
                appName = appNameSetting.ToString() ?? appName;
            }
            return appName;

        }
        /// <summary>
        /// Method is for Uglify the javascripts file
        /// Need to set DefualtMinifier="UglifyJsMinifier"
        /// </summary>
        /// <param name="bundles"></param>
        private static void RegisterJavascriptBundlesToUglify(BundleCollection bundles)
        {
            var nullOrderer = new NullOrderer();
            var scripttransformer = new ScriptTransformer();

            var JQueryBundle = new CustomScriptBundle("~/Bundles/jQueryBundle")
                        .Include("~/Scripts/Framework/External/jquery-1.11.0.js")
                        .Include("~/Scripts/Framework/External/jquery-ui-1.9.2.custom.min.js");
            JQueryBundle.Transforms.Add(scripttransformer);
            JQueryBundle.Orderer = nullOrderer;
            bundles.Add(JQueryBundle);

            var GridBundle = new CustomScriptBundle("~/Bundles/GridBundle")
                        .Include("~/Scripts/Framework/External/jquery.jqGrid.js")
                        .Include("~/Scripts/Framework/External/grid.locale-en.js")
                        .Include("~/Scripts/Framework/External/grid.locale-en.js")
                        .Include("~/Scripts/Framework/External/handsontable.full.js");
            GridBundle.Orderer = nullOrderer;
            bundles.Add(GridBundle);

            var PqGridBundle = new CustomScriptBundle("~/bundles/PqGridBundle")
                            .Include("~/Scripts/Framework/External/PQGrid/pqgrid.min.js")
                            .Include("~/Scripts/Framework/External/PQGrid/pqselect.min.js")
                            .Include("~/Scripts/Framework/External/PQGrid/touch-punch.min.js")
                            .Include("~/Scripts/Framework/External/PQGrid/jszip.min.js");
            PqGridBundle.Orderer = nullOrderer;
            bundles.Add(PqGridBundle);

            var JsRenderBundle = new CustomScriptBundle("~/Bundles/JsRenderBundle")
                        .Include("~/Scripts/Framework/External/jsrender.js")
                        .Include("~/Scripts/Framework/External/jsrender.extension.js");
            JsRenderBundle.Orderer = nullOrderer;
            bundles.Add(JsRenderBundle);


            var DialogBundle = new CustomScriptBundle("~/Bundles/DialogBundle")
                        .Include("~/Scripts/Framework/Internal/dialogs.js");
            DialogBundle.Transforms.Add(scripttransformer);
            DialogBundle.Orderer = nullOrderer;
            bundles.Add(DialogBundle);

            var AjaxBundle = new CustomScriptBundle("~/Bundles/AjaxBundle")
                       .Include("~/Scripts/Framework/Internal/ajaxnotifier.js")
                       .Include("~/Scripts/Framework/Internal/ajaxwrapper.js");
            AjaxBundle.Transforms.Add(scripttransformer);
            AjaxBundle.Orderer = nullOrderer;
            bundles.Add(AjaxBundle);

            var GlobalBundle = new CustomScriptBundle("~/Bundles/GlobalBundle")
                      .Include("~/Scripts/Framework/External/linq.js")
                      .Include("~/Scripts/Framework/Internal/globalvariables.js")
                      .Include("~/Scripts/Framework/Internal/messages.js")
                      .Include("~/Scripts/Framework/Internal/floatingsectionheaders.js")
                      .Include("~/Scripts/ErrorLog/errorhandler.js")
                      .Include("~/Scripts/Framework/Internal/ajaxwrapper.js")
                      .Include("~/Scripts/Framework/Internal/dialogs.js")
                      .Include("~/Scripts/Framework/Internal/locktimer.js")
                       .Include("~/Scripts/NotificationStatus/Notification.js");


            GlobalBundle.Transforms.Add(scripttransformer);
            GlobalBundle.Orderer = nullOrderer;
            bundles.Add(GlobalBundle);

            var LayoutBundle = new Bundle("~/Bundles/LayoutBundle")
             .Include("~/Scripts/Layout/fontresizer.js")
                      .Include("~/Scripts/Layout/layout.js")
                      .Include("~/Scripts/DashBoard/globalClaims.js")
             .Include("~/Scripts/NotificationStatus/Notification.js");
            LayoutBundle.Transforms.Add(scripttransformer);
            LayoutBundle.Orderer = nullOrderer;
            bundles.Add(LayoutBundle);


            var FolderVersionBundle = new Bundle("~/Bundles/FolderVersionBundle")
                      .Include("~/Scripts/Framework/Internal/table.hilight.js")
                      .Include("~/Scripts/FolderVersion/repeaterBuilderAG.js")
                      .Include("~/Scripts/FolderVersion/createformdialog.js")
                      .Include("~/Scripts/FolderVersion/openDocumentDialog.js")
                      .Include("~/Scripts/FolderVersion/errorManager.js")
                      .Include("~/Scripts/FolderVersion/viewImpactReport.js")
                      .Include("~/Scripts/FolderVersion/annotationManager.js")
                      .Include("~/Scripts/FolderVersion/tabmanager.js")
                      .Include("~/Scripts/FolderVersion/sotmanager.js")
                      .Include("~/Scripts/FolderVersion/folderManager.js")
                      .Include("~/Scripts/FolderVersion/folderstatus.js")
                      .Include("~/Scripts/FolderVersion/folderVersion.js")
                      .Include("~/Scripts/FolderVersion/printPreview.js")
                      .Include("~/Scripts/FolderVersion/folderVersionWorkflow.js")
                      .Include("~/Scripts/FolderVersion/formvalidationmanager.js")
                      .Include("~/Scripts/FolderVersion/formInstanceBuilder.js")
                      .Include("~/Scripts/FolderVersion/formInstanceBuilder.sot.js")
                      .Include("~/Scripts/FolderVersion/majorMinorVersion.js")
                      .Include("~/Scripts/FolderVersion/repeaterdialog.js")
                      .Include("~/Scripts/FolderVersion/repeaterBuilderPQ.js")
                      .Include("~/Scripts/FolderVersion/datasourcemanualmappingAG.js")
                      .Include("~/Scripts/FolderVersion/masterlist.js")
                      .Include("~/Scripts/FolderVersion/userSettingDialog.js")
                      .Include("~/Scripts/FolderVersion/validationManager.js")
                      .Include("~/Scripts/FolderVersion/versionHistory.js")
                      .Include("~/Scripts/FolderVersion/datasourcesync.js")
                      .Include("~/Scripts/FormDesign/previewformdesign.js")
                      .Include("~/Scripts/FolderVersion/retroaccountdialog.js")
                      .Include("~/Scripts/FolderVersion/datasourcemanualmappingPQ.js")
                      .Include("~/Scripts/FolderVersion/childPopupManager.js")
                      .Include("~/Scripts/PrintTemplate/pdfConfigurationDesign.js")
                      .Include("~/Scripts/FolderVersion/folderMember.js")
                      .Include("~/Scripts/FolderVersion/ebrules.js")
                      .Include("~/Scripts/FolderVersion/prefixReuse.js")
                      .Include("~/Scripts/Framework/Internal/gridutilities.js")
                      .Include("~/Scripts/FolderVersion/formDesignVersionPreLoader.js")
                      .Include("~/Scripts/NotificationStatus/Notification.js")
                      .Include("~/Scripts/FolderVersion/exitValidate.js");

            FolderVersionBundle.Transforms.Add(scripttransformer);
            FolderVersionBundle.Orderer = nullOrderer;
            bundles.Add(FolderVersionBundle);

            var FormDesignBundle = new Bundle("~/Bundles/FormDesignBundle")
                     .Include("~/Scripts/FormDesign/dataSourcedialog.js")
                     .Include("~/Scripts/FormDesign/dropdownitemsdialog.js")
                     .Include("~/Scripts/FormDesign/fieldlistdialog.js")
                     .Include("~/Scripts/FormDesign/paramadvancedconfigurationdialog.js")
                     .Include("~/Scripts/FormDesign/repeaterTemplateDialog.js")
                     .Include("~/Scripts/FormDesign/expressionRulesDialog.js")
                     .Include("~/Scripts/FormDesign/expressionRulesDialogNew.js")
                     .Include("~/Scripts/FormDesign/expressionRuleTester.js")
                     .Include("~/Scripts/FormDesign/formdesign.js")
                     .Include("~/Scripts/FormDesign/formdesignversion.js")
                     .Include("~/Scripts/FormDesign/formdesignview.default.js")
                     .Include("~/Scripts/FormDesign/formdesignview.config.js")
                     .Include("~/Scripts/FormDesign/formDesignActivityLogger.js")
                     .Include("~/Scripts/FormDesign/rulesdialog.js")

                     .Include("~/Scripts/FormDesign/rulesTesterDialog.js")
                     .Include("~/Scripts/FormDesign/rulesTesterProcessor.js")


                     .Include("~/Scripts/FormDesign/sectionlistdialog.js")
                     .Include("~/Scripts/FormDesign/uielementpropertygrid.js")
                     .Include("~/Scripts/FormDesign/expressionbuilder.js")
                     .Include("~/Scripts/FormDesign/previewformdesign.js")
             .Include("~/Scripts/NotificationStatus/Notification.js");
            FormDesignBundle.Transforms.Add(scripttransformer);
            FormDesignBundle.Orderer = nullOrderer;
            FormDesignBundle.Transforms.Add(scripttransformer);
            bundles.Add(FormDesignBundle);


            var ConsumerAccountBundle = new Bundle("~/Bundles/ConsumerAccountBundle")
                       .Include("~/Scripts/ConsumerAccount/accountsearch.js")
                       .Include("~/Scripts/ConsumerAccount/manageaccount.js")
                       .Include("~/Scripts/ConsumerAccount/folderdialog.js")
                       .Include("~/Scripts/ConsumerAccount/portfoliosearch.js")
                       .Include("~/Scripts/Framework/Internal/gridutilities.js")
                       .Include("~/Scripts/DashBoard/dashboard.js")
                       .Include("~/Scripts/Framework/Internal/globalutilities.js");
            //.Include("~/Scripts/FolderVersionReport/folderVersionReport.js");
            ConsumerAccountBundle.Transforms.Add(scripttransformer);
            ConsumerAccountBundle.Orderer = nullOrderer;
            bundles.Add(ConsumerAccountBundle);

            var DashboardBundle = new Bundle("~/Bundles/DashboardBundle")
                        .Include("~/Scripts/DashBoard/dashboard.js")
                        .Include("~/Scripts/DashBoard/globalClaims.js");
            DashboardBundle.Transforms.Add(scripttransformer);
            DashboardBundle.Orderer = nullOrderer; ;
            DashboardBundle.Transforms.Add(scripttransformer);
            bundles.Add(DashboardBundle);



            var BootstrapBundle = new Bundle("~/Bundles/BootstrapBundle")
                        .Include("~/Scripts/Framework/External/bootstrap.js");
            BootstrapBundle.Transforms.Add(scripttransformer);
            BootstrapBundle.Orderer = nullOrderer;
            bundles.Add(BootstrapBundle);


            var LogOffBundle = new Bundle("~/Bundles/LogOffBundle")
                       .Include("~/Scripts/LogOff/logoff.js");
            LogOffBundle.Transforms.Add(scripttransformer);
            LogOffBundle.Orderer = nullOrderer;
            bundles.Add(LogOffBundle);

            var UtilsBundle = new Bundle("~/Bundles/UtilsBundle")
                      .Include("~/Scripts/Framework/Internal/uiblocker.js")
                      .Include("~/Scripts/Framework/Internal/globalutilities.js")
                      .Include("~/Scripts/Framework/Internal/jqGridtoCsv.js")
                      .Include("~/Scripts/Framework/Internal/gridutilities.js")
                      .Include("~/Scripts/Framework/Internal/agGridtoCsv.js");

            UtilsBundle.Transforms.Add(scripttransformer);
            UtilsBundle.Orderer = nullOrderer;
            bundles.Add(UtilsBundle);

            var FormDesignGroupBundle = new Bundle("~/Bundles/FormDesignGroupBundle")
                      .Include("~/Scripts/FormGroupDesign/formdesigngroup.js");
            FormDesignGroupBundle.Transforms.Add(scripttransformer);
            FormDesignGroupBundle.Orderer = nullOrderer;
            bundles.Add(FormDesignGroupBundle);

            var ActivityLoggerBundle = new Bundle("~/Bundles/ActivityLoggerBundle")
                     .Include("~/Scripts/Framework/Internal/activitylogger.js")
                      .Include("~/Scripts/Framework/Internal/ruleExecutionLogger.js");
            ActivityLoggerBundle.Transforms.Add(scripttransformer);
            ActivityLoggerBundle.Orderer = nullOrderer;
            bundles.Add(ActivityLoggerBundle);

            var FolderVersionReport = new Bundle("~/bundles/FolderVersionReportBundle")
                     .Include("~/Scripts/FolderVersionReport/folderVersionReport.js")
                     .Include("~/Scripts/Framework/Internal/globalutilities.js")
                     .Include("~/Scripts/Framework/Internal/gridutilities.js")
                     .Include("~/Scripts/Report/AuditChecklistReport.js");

            FolderVersionReport.Transforms.Add(scripttransformer);
            FolderVersionReport.Orderer = nullOrderer;
            bundles.Add(FolderVersionReport);


            var PrintTemplate = new Bundle("~/bundles/PrintTemplateBundle")
                       .Include("~/Scripts/PrintTemplate/pdfConfigurationDesign.js");

            PrintTemplate.Transforms.Add(scripttransformer);
            PrintTemplate.Orderer = nullOrderer;
            bundles.Add(PrintTemplate);

            bundles.Add(new ScriptBundle("~/bundles/QueryManager")
                           .Include("~/Scripts/QueryManager/QueryManager.js"));

            var serviceVersionBundle = new ScriptBundle("~/bundles/ServiceVersionBundle")
                                        .Include("~/Scripts/ServiceDesign/servicedesign.js")
                                        .Include("~/Scripts/ServiceDesign/servicedesignversion.js")
                                        .Include("~/Scripts/ServiceDesign/servicedefinitionpropertygrid.js")
                                        .Include("~/Scripts/ServiceDesign/searchparameterdialog.js")
                                        .Include("~/Scripts/ServiceDesign/servicedesignversionoutput.js");

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                       "~/Scripts/Framework/External/Notification/modernizr-*"));

            serviceVersionBundle.Transforms.Add(scripttransformer);
            serviceVersionBundle.Orderer = nullOrderer;
            bundles.Add(serviceVersionBundle);

        }
    }
}