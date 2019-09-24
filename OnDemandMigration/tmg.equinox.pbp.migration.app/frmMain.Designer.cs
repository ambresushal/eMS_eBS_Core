namespace tmg.equinox.pbp.migration.app
{
    partial class frmMain
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
            this.MigrationWorker = new System.ComponentModel.BackgroundWorker();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.txtConsole = new System.Windows.Forms.TextBox();
            this.cmbMigrationType = new System.Windows.Forms.ComboBox();
            this.cmbViewType = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // MigrationWorker
            // 
            this.MigrationWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.MigrationWorker_DoWork);
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(756, 462);
            this.btnRun.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(100, 28);
            this.btnRun.TabIndex = 1;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(864, 462);
            this.btnExit.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(100, 28);
            this.btnExit.TabIndex = 2;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // txtConsole
            // 
            this.txtConsole.BackColor = System.Drawing.Color.White;
            this.txtConsole.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtConsole.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtConsole.ForeColor = System.Drawing.Color.Green;
            this.txtConsole.Location = new System.Drawing.Point(16, 100);
            this.txtConsole.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtConsole.Multiline = true;
            this.txtConsole.Name = "txtConsole";
            this.txtConsole.ReadOnly = true;
            this.txtConsole.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtConsole.Size = new System.Drawing.Size(945, 354);
            this.txtConsole.TabIndex = 3;
            // 
            // cmbMigrationType
            // 
            this.cmbMigrationType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMigrationType.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbMigrationType.ForeColor = System.Drawing.Color.Black;
            this.cmbMigrationType.FormattingEnabled = true;
            this.cmbMigrationType.Items.AddRange(new object[] {
            "Use blank template for migration",
            "Use migrated template for migration"});
            this.cmbMigrationType.Location = new System.Drawing.Point(16, 63);
            this.cmbMigrationType.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbMigrationType.Name = "cmbMigrationType";
            this.cmbMigrationType.Size = new System.Drawing.Size(920, 24);
            this.cmbMigrationType.TabIndex = 5;
            // 
            // cmbViewType
            // 
            this.cmbViewType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbViewType.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbViewType.ForeColor = System.Drawing.Color.Black;
            this.cmbViewType.FormattingEnabled = true;
            this.cmbViewType.Items.AddRange(new object[] {
            "SOT",
            "PBP"});
            this.cmbViewType.Location = new System.Drawing.Point(16, 15);
            this.cmbViewType.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbViewType.Name = "cmbViewType";
            this.cmbViewType.Size = new System.Drawing.Size(920, 24);
            this.cmbViewType.TabIndex = 6;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(980, 495);
            this.Controls.Add(this.cmbViewType);
            this.Controls.Add(this.cmbMigrationType);
            this.Controls.Add(this.txtConsole);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnRun);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "frmMain";
            this.Text = "eMedicareSync Migration";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.ComponentModel.BackgroundWorker MigrationWorker;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.TextBox txtConsole;
        private System.Windows.Forms.ComboBox cmbMigrationType;
        private System.Windows.Forms.ComboBox cmbViewType;
    }
}

