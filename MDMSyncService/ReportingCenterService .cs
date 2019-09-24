using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using tmg.equinox.applicationservices;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.core.logging.Logging;
using tmg.equinox.dependencyresolution;
using tmg.equinox.domain.entities;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;
using tmg.equinox.schema.Base.Model;
using tmg.equinox.schema.sql;
using tmg.equinox.setting.Interface;

namespace MDMSyncService
{
    public partial class ReportingCenterService : ServiceBase
    {
        private Timer timerForServcie = null;
        bool _isprocessing = false;
        private IUnitOfWorkAsync _unitOfWork { get; set; }
        private string UserName { get; set; }
        private IFolderVersionServices _folderVersionServices;
        private IMDMSyncDataService _mDMSyncService;
        private ISettingManager _settingManager;
        private static ILog _logger = LogProvider.For<ReportingCenterService>();

        public ReportingCenterService()
        {
            string[] d = new string[2];
            this.OnStart(d);


            UnityConfig.RegisterComponents();
            //InitializeComponent();
            //ServiceName = "ReportingCenterService";

            if (ConfigurationManager.AppSettings["ReportingCenterService"] != null)
            {
                ServiceName = ConfigurationManager.AppSettings["ReportingCenterService"].ToString();
            }
            //---------------------------
            //InitializeComponent();
            _mDMSyncService = UnityConfig.Resolve<IMDMSyncDataService>();
            _folderVersionServices = UnityConfig.Resolve<IFolderVersionServices>();
            _settingManager = UnityConfig.Resolve<ISettingManager>();
        }

        public object Library { get; private set; }

        protected override void OnStart(string[] args)
        {
            timerForServcie = new System.Timers.Timer(30000);
            this.timerForServcie.Interval = 30000;
            this.timerForServcie.Elapsed += new ElapsedEventHandler(this.timerForServcie_Elapsed);
            this.timerForServcie.Enabled = true;
        }

        protected override void OnStop()
        {
            //Library.WriteErrorLog("Test window service stopped");
        }

        void timerForServcie_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!_isprocessing)
            {
                _isprocessing = true;
                SchemaUpdateTracker currentDesignTracker = null;
                try
                {
                    //Get Data from table MDM.SchemaUpdateTracker where status =1
                    var formDesignstoBeSynced = _mDMSyncService.GetSchemaUpdateTracker();
                    // For each record call the MDM Db schema Update method

                    foreach (SchemaUpdateTracker designTracker in formDesignstoBeSynced)
                    {
                        try
                        {
                            currentDesignTracker = designTracker;
                            designTracker.Status = (int)MSMSyncStatus.Inprogress;
                            designTracker.UpdatedDate = DateTime.Now;
                            _mDMSyncService.UpdateSchemaUpdateTracker(designTracker);
                            var _reportingDataService = new ReportingDataService(UnityConfig.Resolve<IRptUnitOfWorkAsync>(), _mDMSyncService);
                            var generateSchemaService = new GenerateSchemaService(UnityConfig.Resolve<IRptUnitOfWorkAsync>(), _mDMSyncService);

                            var formDesignService = new FormDesignService(UnityConfig.Resolve<IUnitOfWorkAsync>(), null, null, _reportingDataService, generateSchemaService, null, _settingManager);
                            List<JsonDesign> jsonDesigns = formDesignService.GetFormDesignInformation(designTracker.FormdesignID,designTracker.FormdesignVersionID);
                            generateSchemaService.Run(jsonDesigns);
                            //this._folderVersionServices.UpdateReportingCenterDatabase(designTracker.FormdesignID, null);
                            designTracker.Status = (int)MSMSyncStatus.Completed;
                            designTracker.OldJsonHash = designTracker.CurrentJsonHash;
                            designTracker.UpdatedDate = DateTime.Now;
                            _mDMSyncService.UpdateSchemaUpdateTracker(designTracker);
                            AllReportingTablesCache.reportingTableCollection.Remove(designTracker.FormdesignVersionID);
                        }
                        catch (Exception ex)
                        {
                            if (currentDesignTracker != null)
                            {
                                currentDesignTracker.Status = (int)MSMSyncStatus.Errored;
                                currentDesignTracker.UpdatedDate = DateTime.Now;
                                _mDMSyncService.UpdateSchemaUpdateTracker(currentDesignTracker);
                            }
                            _logger.ErrorException("Error while syncing designs", ex);
                        }

                    }

                    //Get Data from table MDM.DocumentUpdateTracker where status =1
                    var InProgress = _mDMSyncService.GetReadyForUpdateDocumentUpdateTracker(2).Count();
                    if (InProgress <= 5)
                    {
                        var forminstanceIdstoBeSynced = _mDMSyncService.GetReadyForUpdateDocumentUpdateTracker(1);
                        //Get inprogress count
                        // For each record call the MDMDb Update method
                        foreach (int formInstanceId in forminstanceIdstoBeSynced)
                        {
                            _mDMSyncService.UpdateDocumentUpdateTracker(formInstanceId, (int)MSMSyncStatus.Inprogress);
                            this._folderVersionServices.UpdateReportingCenterDatabase(formInstanceId, null, true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.ErrorException("Error while syncing documents", ex);
                }
                finally
                {
                    _isprocessing = false;
                }

            }
        }
    }
}
