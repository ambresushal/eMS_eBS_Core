using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using tmg.equinox.backgroundjob;
using tmg.equinox.hangfire;
using tmg.equinox.hangfire.Configuration;
using tmg.equinox.queueprocess.masterlistcascade;

namespace tmg.equinox.masterlistcascade.tester
{
    public partial class frmMasterListTester : Form
    {
        public frmMasterListTester()
        {
            InitializeComponent();
        }

        private void cmdTest_Click(object sender, EventArgs e)
        {
            //queue Master List Cascade
            IBackgroundJobServerFactory factory = new HangfireBackgroundJobServer();
            ILogProviderFactory logFactory = new HangfireLogProviderFactory();
            BackgroundJobConfiguration config = new BackgroundJobConfiguration();
            IConfiguration iconfig = new HangfireConfiguration(factory,logFactory);
            MasterListCascadeEnqueue enqueue = new MasterListCascadeEnqueue(new HangfireBackgroundJobManager(config, iconfig));
            //{ QueueId = ImportID, UserId = createdBy, FeatureId = ImportID.ToString(), Name = "PBP Import for PBPImportQueueID: " + ImportID.ToString(), AssemblyName = "tmg.equinox.applicationservices", ClassName = "PBPImportCustomQueue" }
            MasterListCascadeQueueInfo info = new MasterListCascadeQueueInfo();
            info = new MasterListCascadeQueueInfo();
            info.AssemblyName = "tmg.equinox.core.masterlistcascade";
            info.ClassName = "MasterListCascadeManager";
            info.FeatureId = "MLC";
            info.FormDesignID = 2377;
            info.FormDesignVersionID = 2357;
            info.Name = "Master  List Cascade";
            info.UserId = "admin";
            enqueue.Enqueue(info);

        }
    }
}
