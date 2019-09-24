using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using tmg.equinox.applicationservices.pbp;
using tmg.equinox.ODM.expressionrules.executor;
using tmg.equinox.pbp.logging;

namespace tmg.equinox.pbp.migration.app
{
    public partial class frmMain : Form
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public frmMain()
        {
            InitializeComponent();
            cmbMigrationType.SelectedIndex = 0;
            cmbViewType.SelectedIndex = 0;
        }

        private void MigrationWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string viewType = GetMigrationViewType();
            WriteMessageToTextBox("==================================================================");
            WriteMessageToTextBox("[" + viewType + "] Migration process has been started.");
            WriteMessageToTextBox("==================================================================");

            //get list of Access files to migrate
            IPBPMigrationService migrationService = new PBPMigrationService();
            List<PBPFile> files = migrationService.GetFilesToMigrate();
            WriteMessageToTextBox("Get all access database file name to be migrate.");
            if (files != null && files.Count > 0)
            {
                try
                {
                    int batchId = migrationService.GetNewBatchId();
                    foreach (PBPFile file in files)
                    {
                        WriteMessageToTextBox("Processing file [" + file.FileName + "].");

                        //get QID's to migrate for file from the Migration database
                        List<MigrationPlanItem> qidListToMigrate = migrationService.GetMigrationListForFile(file.FileID, viewType);
                        WriteMessageToTextBox("Get all QIDs for processing..!");

                        //get QID's/Plans from the Access file
                        IPBPAccessService accessService = new PBPAccessService();
                        string mdbPath = file.FileName;
                        mdbPath = AppSettingsManager.AccessFilePath + mdbPath;
                        List<string> fileQIDList = accessService.GetQIDList(mdbPath);

                        DataTable dtManualChangesApply = GetDataTableOfManualChanges(AppSettingsManager.AccessFilePath + "ManualChangesApply.CSV");

                        ////To get the char fields lenght from access database
                        //List<AccessDatabaseTableInfo> accessTableDetails = accessService.GetAccessDatabaseTables(mdbPath);  
                        foreach (var qid in qidListToMigrate)
                        {
                            WriteMessageToTextBox("Get QID [" + qid.QID + "] for processing..!");
                            //initialize logger context
                            AppLogger.SetContext(batchId, qid.FormInstanceId, qid.FormDesignVersionId, qid.QID);
                            if (fileQIDList.Contains(qid.QID))
                            {
                                AppLogger.Info("Processing");
                                DMUtilityLog.WriteDMUtilityLog(qid.QID, viewType, null);
                                //get Migration mappings 
                                MigrationMapper mapper = migrationService.GetMigrationMapper(qid.FormDesignVersionId, viewType);
                                if (mapper == null)
                                {
                                    WriteMessageToTextBox("Table list is not found for QID [" + qid.QID + "] ");
                                    continue;
                                }
                                List<PBPTable> tables = mapper.GetPBPTables();

                                //get JSON for QID from Access File
                                JObject qidInstance = accessService.GetQIDData(mdbPath, tables, qid.QID);
                                WriteMessageToTextBox("Get JSON template for QID [" + qid.QID + "]..!");

                                //get FormInstance JSON
                                IPBPDocumentService documentService = new PBPDocumentService();
                                JObject formInstance = GetMigrationType() == 0 ? migrationService.GetDefaultJSONForFormDesignVersion(qid.FormDesignVersionId) : documentService.GetJSONDocument(qid.FormInstanceId);
                                if (viewType == "SOT")
                                {
                                    if (formInstance["DMSection"] != null)
                                        formInstance["DMSection"].Parent.Remove();

                                    formInstance.Add(new JProperty("DMSection", JToken.FromObject(qidInstance)));

                                }

                                //process Migration Mappings
                                WriteMessageToTextBox("Update JSON template with data values for QID [" + qid.QID + "]..!");
                                mapper.ProcessMappings(qidInstance, formInstance, qid);

                                // Expression Rule Execution of ViewType ==SOT
                                if (viewType == "SOT")
                                {
                                    try
                                    {
                                        RuleExecutor executor = new RuleExecutor();
                                        formInstance = executor.ProcessExpressionRules(qid, formInstance);

                                        //Update Manual changes into JSON
                                        UpdateManualChange(qid.QID, dtManualChangesApply, ref formInstance);
                                    }
                                    catch (Exception ex)
                                    {
                                        WriteMessageToTextBox("Migration process expression rule execution has been closed. Due to error [" + ex.Message + "]", true, true);
                                    }
                                }

                                //save FormInstance JSON
                                WriteMessageToTextBox("Update JSON into Well Care database.");
                                documentService.UpdateDocumentJSON(qid.FormInstanceId, formInstance.ToString(Newtonsoft.Json.Formatting.None));
                                WriteMessageToTextBox("QID [" + qid.QID + "] has been migrated successfully..!");
                            }
                        }
                        WriteMessageToTextBox("File [" + file.FileName + "] process complete.");
                    }
                }
                catch (Exception ex)
                {
                    WriteMessageToTextBox("Migration process has been closed. Due to error [" + ex.Message + "]", true, true);
                    throw ex;
                }
            }
            WriteMessageToTextBox("Migration process has been completed successfully.", true);
        }

        private void WriteMessageToTextBox(string message, bool isComplete = false, bool isException = false)
        {
            txtConsole.BeginInvoke(new Action(() => { txtConsole.AppendText(Environment.NewLine + message); txtConsole.ScrollToCaret(); }));
            if (isComplete)
                btnRun.BeginInvoke(new Action(() => { btnRun.Enabled = true; cmbMigrationType.Enabled = true; cmbViewType.Enabled = true; if (isException) txtConsole.ForeColor = Color.Red; }));
        }
        private int GetMigrationType()
        {
            int MigrationType = 0;
            cmbMigrationType.Invoke(new MethodInvoker(delegate () { MigrationType = cmbMigrationType.SelectedIndex; }));
            return MigrationType;
        }
        private string GetMigrationViewType()
        {
            string GetMigrationViewType = "";
            cmbViewType.Invoke(new MethodInvoker(delegate () { GetMigrationViewType = cmbViewType.Text; }));
            return GetMigrationViewType;
        }
        private void btnRun_Click(object sender, EventArgs e)
        {
            btnRun.Enabled = false;
            cmbMigrationType.Enabled = false;
            cmbViewType.Enabled = false;
            LogConfiguration.ConfigureLog();
            MigrationWorker.RunWorkerAsync();
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private DataTable GetDataTableOfManualChanges(string path)
        {
            DataTable dtCSVData = new DataTable();
            dtCSVData.Columns.Add("QID", typeof(string));
            dtCSVData.Columns.Add("JSONPATH", typeof(string));
            dtCSVData.Columns.Add("VALUE", typeof(string));

            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string currentLine;
                    while ((currentLine = sr.ReadLine()) != null)
                    {
                        string[] line = currentLine.Split(',');
                        dtCSVData.Rows.Add(line[0], line[1], line[2]);
                        //Console.WriteLine(currentLine);
                    }
                }
            }
            return dtCSVData;
        }
        private void UpdateManualChange(string qid, DataTable dtChanges, ref JObject target)
        {
            DataRow[] rowsQID = dtChanges.Select("QID = '" + qid + "'");
            foreach (DataRow row in rowsQID)
            {
                var targetToken = target.SelectToken(row.Field<string>("JSONPATH"));
                var prop = targetToken.Parent as JProperty;
                string tagetValue = row.Field<string>("VALUE").Replace("|", ",");

                if (tagetValue.Contains("["))
                    prop.Value = JArray.Parse(tagetValue);
                else
                    prop.Value = tagetValue;
            }
        }
    }
}
