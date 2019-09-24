namespace tmg.equinox.expressionrules.tester
{
    partial class frmExprRules
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.backgroundWorkerProcess = new System.ComponentModel.BackgroundWorker();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.chkClearCache = new System.Windows.Forms.CheckBox();
            this.wbResult = new System.Windows.Forms.WebBrowser();
            this.txtFormDesignVersionId = new System.Windows.Forms.TextBox();
            this.lblFormDesignVersionId = new System.Windows.Forms.Label();
            this.lblResult = new System.Windows.Forms.Label();
            this.lblFolderVersionID = new System.Windows.Forms.Label();
            this.txtFolderVersionID = new System.Windows.Forms.TextBox();
            this.cmdProcessRule = new System.Windows.Forms.Button();
            this.txtFormInstanceID = new System.Windows.Forms.TextBox();
            this.lblFormInstanceID = new System.Windows.Forms.Label();
            this.lblRuleID = new System.Windows.Forms.Label();
            this.txtRuleID = new System.Windows.Forms.TextBox();
            this.richTxtResult = new System.Windows.Forms.RichTextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.rtResult = new System.Windows.Forms.RichTextBox();
            this.btnGenerateRules = new System.Windows.Forms.Button();
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.txtFDVersionID = new System.Windows.Forms.TextBox();
            this.txtFormDesignID = new System.Windows.Forms.TextBox();
            this.lblFilePath = new System.Windows.Forms.Label();
            this.lblFDVersionID = new System.Windows.Forms.Label();
            this.lblFormDesignID = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.btnGenerateDesign = new System.Windows.Forms.Button();
            this.rtPBPDesignView = new System.Windows.Forms.RichTextBox();
            this.tabMisc = new System.Windows.Forms.TabPage();
            this.btnGenerateRuleDesc = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtDesignVersionID = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabMisc.SuspendLayout();
            this.SuspendLayout();
            // 
            // backgroundWorkerProcess
            // 
            this.backgroundWorkerProcess.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerProcess_DoWork);
            this.backgroundWorkerProcess.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerProcess_RunWorkerCompleted);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabMisc);
            this.tabControl1.Location = new System.Drawing.Point(3, 5);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1064, 510);
            this.tabControl1.TabIndex = 14;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.chkClearCache);
            this.tabPage1.Controls.Add(this.wbResult);
            this.tabPage1.Controls.Add(this.txtFormDesignVersionId);
            this.tabPage1.Controls.Add(this.lblFormDesignVersionId);
            this.tabPage1.Controls.Add(this.lblResult);
            this.tabPage1.Controls.Add(this.lblFolderVersionID);
            this.tabPage1.Controls.Add(this.txtFolderVersionID);
            this.tabPage1.Controls.Add(this.cmdProcessRule);
            this.tabPage1.Controls.Add(this.txtFormInstanceID);
            this.tabPage1.Controls.Add(this.lblFormInstanceID);
            this.tabPage1.Controls.Add(this.lblRuleID);
            this.tabPage1.Controls.Add(this.txtRuleID);
            this.tabPage1.Controls.Add(this.richTxtResult);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1056, 484);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Rule Tester";
            // 
            // chkClearCache
            // 
            this.chkClearCache.AutoSize = true;
            this.chkClearCache.Location = new System.Drawing.Point(326, 18);
            this.chkClearCache.Name = "chkClearCache";
            this.chkClearCache.Size = new System.Drawing.Size(132, 17);
            this.chkClearCache.TabIndex = 26;
            this.chkClearCache.Text = "Clear Rule from Cache";
            this.chkClearCache.UseVisualStyleBackColor = true;
            // 
            // wbResult
            // 
            this.wbResult.Location = new System.Drawing.Point(78, 344);
            this.wbResult.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbResult.Name = "wbResult";
            this.wbResult.Size = new System.Drawing.Size(903, 125);
            this.wbResult.TabIndex = 25;
            // 
            // txtFormDesignVersionId
            // 
            this.txtFormDesignVersionId.Location = new System.Drawing.Point(201, 119);
            this.txtFormDesignVersionId.Name = "txtFormDesignVersionId";
            this.txtFormDesignVersionId.Size = new System.Drawing.Size(100, 20);
            this.txtFormDesignVersionId.TabIndex = 24;
            // 
            // lblFormDesignVersionId
            // 
            this.lblFormDesignVersionId.AutoSize = true;
            this.lblFormDesignVersionId.Location = new System.Drawing.Point(78, 119);
            this.lblFormDesignVersionId.Name = "lblFormDesignVersionId";
            this.lblFormDesignVersionId.Size = new System.Drawing.Size(118, 13);
            this.lblFormDesignVersionId.TabIndex = 23;
            this.lblFormDesignVersionId.Text = "Form Design Version ID";
            // 
            // lblResult
            // 
            this.lblResult.AutoSize = true;
            this.lblResult.Location = new System.Drawing.Point(75, 145);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(37, 13);
            this.lblResult.TabIndex = 22;
            this.lblResult.Text = "Result";
            // 
            // lblFolderVersionID
            // 
            this.lblFolderVersionID.AutoSize = true;
            this.lblFolderVersionID.Location = new System.Drawing.Point(78, 83);
            this.lblFolderVersionID.Name = "lblFolderVersionID";
            this.lblFolderVersionID.Size = new System.Drawing.Size(88, 13);
            this.lblFolderVersionID.TabIndex = 21;
            this.lblFolderVersionID.Text = "Folder Version ID";
            // 
            // txtFolderVersionID
            // 
            this.txtFolderVersionID.Location = new System.Drawing.Point(201, 83);
            this.txtFolderVersionID.Name = "txtFolderVersionID";
            this.txtFolderVersionID.Size = new System.Drawing.Size(100, 20);
            this.txtFolderVersionID.TabIndex = 20;
            // 
            // cmdProcessRule
            // 
            this.cmdProcessRule.Location = new System.Drawing.Point(422, 119);
            this.cmdProcessRule.Name = "cmdProcessRule";
            this.cmdProcessRule.Size = new System.Drawing.Size(110, 23);
            this.cmdProcessRule.TabIndex = 19;
            this.cmdProcessRule.Text = "Process Rule";
            this.cmdProcessRule.UseVisualStyleBackColor = true;
            this.cmdProcessRule.Click += new System.EventHandler(this.cmdProcessRule_Click);
            // 
            // txtFormInstanceID
            // 
            this.txtFormInstanceID.Location = new System.Drawing.Point(201, 46);
            this.txtFormInstanceID.Name = "txtFormInstanceID";
            this.txtFormInstanceID.Size = new System.Drawing.Size(100, 20);
            this.txtFormInstanceID.TabIndex = 18;
            // 
            // lblFormInstanceID
            // 
            this.lblFormInstanceID.AutoSize = true;
            this.lblFormInstanceID.Location = new System.Drawing.Point(78, 46);
            this.lblFormInstanceID.Name = "lblFormInstanceID";
            this.lblFormInstanceID.Size = new System.Drawing.Size(88, 13);
            this.lblFormInstanceID.TabIndex = 17;
            this.lblFormInstanceID.Text = "Form Instance ID";
            // 
            // lblRuleID
            // 
            this.lblRuleID.AutoSize = true;
            this.lblRuleID.Location = new System.Drawing.Point(78, 16);
            this.lblRuleID.Name = "lblRuleID";
            this.lblRuleID.Size = new System.Drawing.Size(43, 13);
            this.lblRuleID.TabIndex = 16;
            this.lblRuleID.Text = "Rule ID";
            // 
            // txtRuleID
            // 
            this.txtRuleID.Location = new System.Drawing.Point(201, 16);
            this.txtRuleID.Name = "txtRuleID";
            this.txtRuleID.Size = new System.Drawing.Size(100, 20);
            this.txtRuleID.TabIndex = 15;
            // 
            // richTxtResult
            // 
            this.richTxtResult.Location = new System.Drawing.Point(78, 170);
            this.richTxtResult.Name = "richTxtResult";
            this.richTxtResult.Size = new System.Drawing.Size(903, 142);
            this.richTxtResult.TabIndex = 14;
            this.richTxtResult.Text = "";
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage2.Controls.Add(this.rtResult);
            this.tabPage2.Controls.Add(this.btnGenerateRules);
            this.tabPage2.Controls.Add(this.txtFilePath);
            this.tabPage2.Controls.Add(this.txtFDVersionID);
            this.tabPage2.Controls.Add(this.txtFormDesignID);
            this.tabPage2.Controls.Add(this.lblFilePath);
            this.tabPage2.Controls.Add(this.lblFDVersionID);
            this.tabPage2.Controls.Add(this.lblFormDesignID);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1056, 484);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Rule Generator";
            // 
            // rtResult
            // 
            this.rtResult.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtResult.Location = new System.Drawing.Point(49, 179);
            this.rtResult.Name = "rtResult";
            this.rtResult.Size = new System.Drawing.Size(950, 288);
            this.rtResult.TabIndex = 7;
            this.rtResult.Text = "";
            // 
            // btnGenerateRules
            // 
            this.btnGenerateRules.Location = new System.Drawing.Point(182, 128);
            this.btnGenerateRules.Name = "btnGenerateRules";
            this.btnGenerateRules.Size = new System.Drawing.Size(75, 23);
            this.btnGenerateRules.TabIndex = 6;
            this.btnGenerateRules.Text = "Generate Rules";
            this.btnGenerateRules.UseVisualStyleBackColor = true;
            this.btnGenerateRules.Click += new System.EventHandler(this.btnRuleBuilder_Click);
            // 
            // txtFilePath
            // 
            this.txtFilePath.Location = new System.Drawing.Point(182, 79);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.Size = new System.Drawing.Size(585, 20);
            this.txtFilePath.TabIndex = 5;
            // 
            // txtFDVersionID
            // 
            this.txtFDVersionID.Location = new System.Drawing.Point(182, 47);
            this.txtFDVersionID.Name = "txtFDVersionID";
            this.txtFDVersionID.Size = new System.Drawing.Size(163, 20);
            this.txtFDVersionID.TabIndex = 4;
            // 
            // txtFormDesignID
            // 
            this.txtFormDesignID.Location = new System.Drawing.Point(182, 15);
            this.txtFormDesignID.Name = "txtFormDesignID";
            this.txtFormDesignID.Size = new System.Drawing.Size(163, 20);
            this.txtFormDesignID.TabIndex = 3;
            // 
            // lblFilePath
            // 
            this.lblFilePath.AutoSize = true;
            this.lblFilePath.Location = new System.Drawing.Point(46, 82);
            this.lblFilePath.Name = "lblFilePath";
            this.lblFilePath.Size = new System.Drawing.Size(45, 13);
            this.lblFilePath.TabIndex = 2;
            this.lblFilePath.Text = "FilePath";
            // 
            // lblFDVersionID
            // 
            this.lblFDVersionID.AutoSize = true;
            this.lblFDVersionID.Location = new System.Drawing.Point(46, 50);
            this.lblFDVersionID.Name = "lblFDVersionID";
            this.lblFDVersionID.Size = new System.Drawing.Size(118, 13);
            this.lblFDVersionID.TabIndex = 1;
            this.lblFDVersionID.Text = "Form Design Version ID";
            // 
            // lblFormDesignID
            // 
            this.lblFormDesignID.AutoSize = true;
            this.lblFormDesignID.Location = new System.Drawing.Point(46, 18);
            this.lblFormDesignID.Name = "lblFormDesignID";
            this.lblFormDesignID.Size = new System.Drawing.Size(80, 13);
            this.lblFormDesignID.TabIndex = 0;
            this.lblFormDesignID.Text = "Form Design ID";
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage3.Controls.Add(this.btnGenerateDesign);
            this.tabPage3.Controls.Add(this.rtPBPDesignView);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(1056, 484);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "PBP Design";
            // 
            // btnGenerateDesign
            // 
            this.btnGenerateDesign.Location = new System.Drawing.Point(6, 11);
            this.btnGenerateDesign.Name = "btnGenerateDesign";
            this.btnGenerateDesign.Size = new System.Drawing.Size(159, 23);
            this.btnGenerateDesign.TabIndex = 1;
            this.btnGenerateDesign.Text = "Generate PBP Design";
            this.btnGenerateDesign.UseVisualStyleBackColor = true;
            this.btnGenerateDesign.Click += new System.EventHandler(this.btnGenerateDesign_Click);
            // 
            // rtPBPDesignView
            // 
            this.rtPBPDesignView.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtPBPDesignView.Location = new System.Drawing.Point(5, 40);
            this.rtPBPDesignView.Name = "rtPBPDesignView";
            this.rtPBPDesignView.Size = new System.Drawing.Size(1046, 436);
            this.rtPBPDesignView.TabIndex = 0;
            this.rtPBPDesignView.Text = "";
            // 
            // tabMisc
            // 
            this.tabMisc.Controls.Add(this.txtDesignVersionID);
            this.tabMisc.Controls.Add(this.label1);
            this.tabMisc.Controls.Add(this.btnGenerateRuleDesc);
            this.tabMisc.Location = new System.Drawing.Point(4, 22);
            this.tabMisc.Name = "tabMisc";
            this.tabMisc.Size = new System.Drawing.Size(1056, 484);
            this.tabMisc.TabIndex = 3;
            this.tabMisc.Text = "Misc";
            this.tabMisc.UseVisualStyleBackColor = true;
            // 
            // btnGenerateRuleDesc
            // 
            this.btnGenerateRuleDesc.Location = new System.Drawing.Point(111, 60);
            this.btnGenerateRuleDesc.Name = "btnGenerateRuleDesc";
            this.btnGenerateRuleDesc.Size = new System.Drawing.Size(150, 23);
            this.btnGenerateRuleDesc.TabIndex = 0;
            this.btnGenerateRuleDesc.Text = "Generate Rule Description";
            this.btnGenerateRuleDesc.UseVisualStyleBackColor = true;
            this.btnGenerateRuleDesc.Click += new System.EventHandler(this.btnGenerateRuleDesc_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(33, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(121, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Form Design Version ID:";
            // 
            // txtDesignVersionID
            // 
            this.txtDesignVersionID.Location = new System.Drawing.Point(161, 14);
            this.txtDesignVersionID.Name = "txtDesignVersionID";
            this.txtDesignVersionID.Size = new System.Drawing.Size(100, 20);
            this.txtDesignVersionID.TabIndex = 2;
            // 
            // frmExprRules
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1070, 518);
            this.Controls.Add(this.tabControl1);
            this.Name = "frmExprRules";
            this.Text = "Expression Rules Tester";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabMisc.ResumeLayout(false);
            this.tabMisc.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.ComponentModel.BackgroundWorker backgroundWorkerProcess;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.CheckBox chkClearCache;
        private System.Windows.Forms.WebBrowser wbResult;
        private System.Windows.Forms.TextBox txtFormDesignVersionId;
        private System.Windows.Forms.Label lblFormDesignVersionId;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.Label lblFolderVersionID;
        private System.Windows.Forms.TextBox txtFolderVersionID;
        private System.Windows.Forms.Button cmdProcessRule;
        private System.Windows.Forms.TextBox txtFormInstanceID;
        private System.Windows.Forms.Label lblFormInstanceID;
        private System.Windows.Forms.Label lblRuleID;
        private System.Windows.Forms.TextBox txtRuleID;
        private System.Windows.Forms.RichTextBox richTxtResult;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btnGenerateRules;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.TextBox txtFDVersionID;
        private System.Windows.Forms.TextBox txtFormDesignID;
        private System.Windows.Forms.Label lblFilePath;
        private System.Windows.Forms.Label lblFDVersionID;
        private System.Windows.Forms.Label lblFormDesignID;
        private System.Windows.Forms.RichTextBox rtResult;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.RichTextBox rtPBPDesignView;
        private System.Windows.Forms.Button btnGenerateDesign;
        private System.Windows.Forms.TabPage tabMisc;
        private System.Windows.Forms.Button btnGenerateRuleDesc;
        private System.Windows.Forms.TextBox txtDesignVersionID;
        private System.Windows.Forms.Label label1;
    }
}

