using DesignGenerator;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using tmg.equinox.applicationservices.interfaces;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.dependencyresolution;
using tmg.equinox.web.FormDesignManager;
using tmg.equinox.web.FormInstanceManager;
using tmg.equinox.web.FormInstanceProcessor;
using tmg.equinox.web.FormInstanceProcessor.ExpressionBuilder;
using tmg.equinox.web.Framework.Caching;
using tmg.equinox.web.RuleEngine.RuleDescription;
using tmg.equinox.web.sourcehandler;

namespace tmg.equinox.expressionrules.tester
{
    public partial class frmExprRules : Form
    {
        public frmExprRules()
        {
            InitializeComponent();
            txtFormInstanceID.Text = "154109";
            txtRuleID.Text = "6361";
            txtFormDesignVersionId.Text = "2422";
            txtFolderVersionID.Text = "8388";
            txtFilePath.Text = @"D:\PROJECTS\Equinox\NET\HNE\DEV\tmg.equinox.expressionrules.tester\Files\HNE_EOC_RulesDefination.xlsx";
        }

        private void cmdProcessRule_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            richTxtResult.Text = "";
            wbResult.DocumentText = "";
            this.backgroundWorkerProcess.RunWorkerAsync(100);
        }

        private void backgroundWorkerProcess_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                int ruleID;
                int.TryParse(txtRuleID.Text, out ruleID);

                //parameters for ExpressionRuleTreeProcessor
                //formInstanceID
                int formInstanceId;
                int.TryParse(txtFormInstanceID.Text, out formInstanceId);
                UnityConfig.RegisterComponents();
                //uiElementService
                IUIElementService uiElementService = UnityConfig.Resolve<IUIElementService>();

                //folderVersionID
                int folderVersionId;
                int.TryParse(txtFolderVersionID.Text, out folderVersionId);

                //formDesignVerionId
                int formDesignVersionId;
                int.TryParse(txtFormDesignVersionId.Text, out formDesignVersionId);

                //sourceDBManager
                int tenantId = 1;
                int? userId = 19;
                IFormInstanceDataServices formInstanceDataService = UnityConfig.Resolve<IFormInstanceDataServices>();
                string currentUserName = "superuser";

                IFolderVersionServices folderVersionService = UnityConfig.Resolve<IFolderVersionServices>();
                FormInstanceDataManager formInstanceDataManager = new FormInstanceDataManager(tenantId, userId, formInstanceDataService, currentUserName, folderVersionService);
                IFormDesignService fdService = UnityConfig.Resolve<IFormDesignService>();
                IMasterListService mlService = UnityConfig.Resolve<IMasterListService>();
                SourceHandlerDBManager sourceDBManager = new SourceHandlerDBManager(tenantId, folderVersionService, formInstanceDataManager, fdService, mlService);

                IFormDesignService formDesignService = UnityConfig.Resolve<IFormDesignService>();
                //FormInstanceDataManager formInstanceDataManager
                FormDesignVersionManager formDesignVersionMgr = new FormDesignVersionManager(tenantId, formDesignVersionId, formDesignService);
                FormDesignVersionDetail detail = formDesignVersionMgr.GetFormDesignVersion(true);
                IFormInstanceService formInstanceService = UnityConfig.Resolve<IFormInstanceService>();

                //int? userId
                CurrentRequestContext requestContext = new CurrentRequestContext();
                ExpressionRuleTreeProcessor processor = new ExpressionRuleTreeProcessor(formInstanceId, uiElementService, folderVersionId,
                    sourceDBManager, formDesignService, formInstanceDataManager, detail, formInstanceService, userId, requestContext);
                if (chkClearCache.Checked == true)
                {
                    CompiledDcoumentCacheHandler handler = new CompiledDcoumentCacheHandler();
                    handler.Remove(ruleID);
                }
                JToken result = processor.ProcessRuleTest(ruleID);
                if (result == null)
                {
                    e.Result = "Rule did not return any value";
                }
                else
                {
                    e.Result = result;
                }
            }
            catch (Exception ex)
            {
            }

        }

        private void backgroundWorkerProcess_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            richTxtResult.Text = e.Result.ToString();
            wbResult.DocumentText = "<html>" + e.Result.ToString() + "</html>";
            this.Cursor = Cursors.Default;
        }

        private void btnRuleBuilder_Click(object sender, EventArgs e)
        {
            int formDesignID = Convert.ToInt32(txtFormDesignID.Text);
            int formDesignVersionID = Convert.ToInt32(txtFDVersionID.Text);

            RuleBuilder builder = new RuleBuilder(txtFilePath.Text);
            List<RuleDefinition> result = builder.Build(formDesignID, formDesignVersionID);

            List<string> queries = new List<string>();
            foreach (RuleDefinition rule in result)
            {
                queries.Add(rule.GetQuery());
                queries.Add("\r\n");
            }

            rtResult.Text = string.Join("\r\n", queries.ToArray());
        }

        private void btnGenerateDesign_Click(object sender, EventArgs e)
        {
            DesignBuilder builder = new DesignBuilder();
            var result = builder.Build();
            rtPBPDesignView.Text = JsonConvert.SerializeObject(result, Formatting.Indented);
        }

        private void btnGenerateRuleDesc_Click(object sender, EventArgs e)
        {
            string script = string.Empty;
            string upTemp = "UPDATE [UI].[RULE] SET RuleDescription = '{0}' WHERE RuleID = {1}";
            if (!string.IsNullOrEmpty(txtFormDesignVersionId.Text))
            {
                int formDesignVersionid = Convert.ToInt32(txtDesignVersionID.Text);

                UnityConfig.RegisterComponents();
                IUIElementService uiElementService = UnityConfig.Resolve<IUIElementService>();

                var elementList = uiElementService.GetUIElementList(1, formDesignVersionid);
                foreach (var element in elementList.UIElementList)
                {
                    var rules = uiElementService.GetRulesForUIElement(1, formDesignVersionid, element.UIElementID);
                    if (rules != null && rules.Count() > 0)
                    {
                        RuleTextManager ruleMgr = new RuleTextManager(rules.ToList());
                        List<string> elements = ruleMgr.GetLeftOperands();
                        var uielement = uiElementService.GetUIElementByNames(formDesignVersionid, elements);
                        rules = ruleMgr.GenerateRuleText(uielement);

                        foreach (var rule in rules)
                        {
                            script = script + string.Format(upTemp, rule.RuleDescription, rule.RuleId);
                            script = script + Environment.NewLine;
                        }
                    }
                }

                System.IO.File.WriteAllText(@"C:\Users\mvaishnav\Documents\" + formDesignVersionid.ToString(), script);
            }
            else
            {
                MessageBox.Show("Please enter Form Design Version ID.");
            }

        }
    }
}
