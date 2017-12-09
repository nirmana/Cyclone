namespace CodeCloneAnalysis_StandAlone
{
    partial class Form1
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
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.txtProjectName = new System.Windows.Forms.TextBox();
            this.OR = new System.Windows.Forms.Label();
            this.btnResetForm = new System.Windows.Forms.Button();
            this.btnAnalyzeCode = new System.Windows.Forms.Button();
            this.cmbInterval = new System.Windows.Forms.ComboBox();
            this.chkOneTimeAnalyse = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtGitSourceUrl = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtLocalSourceUrl = new System.Windows.Forms.TextBox();
            this.btnBrowseLocalSource = new System.Windows.Forms.Button();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.OldAnalysisGrid = new System.Windows.Forms.DataGridView();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.chkTimeToTimeAnalyze = new System.Windows.Forms.CheckBox();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OldAnalysisGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(20, 94);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.tabControl1.RightToLeftLayout = true;
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1104, 509);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.txtProjectName);
            this.tabPage2.Controls.Add(this.OR);
            this.tabPage2.Controls.Add(this.btnResetForm);
            this.tabPage2.Controls.Add(this.btnAnalyzeCode);
            this.tabPage2.Controls.Add(this.cmbInterval);
            this.tabPage2.Controls.Add(this.chkTimeToTimeAnalyze);
            this.tabPage2.Controls.Add(this.chkOneTimeAnalyse);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.txtGitSourceUrl);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.txtLocalSourceUrl);
            this.tabPage2.Controls.Add(this.btnBrowseLocalSource);
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPage2.Size = new System.Drawing.Size(1096, 476);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "New";
            this.tabPage2.UseVisualStyleBackColor = true;
            this.tabPage2.Click += new System.EventHandler(this.tabPage2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(54, 41);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 20);
            this.label1.TabIndex = 20;
            this.label1.Text = "Project Name";
            // 
            // txtProjectName
            // 
            this.txtProjectName.Location = new System.Drawing.Point(54, 66);
            this.txtProjectName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtProjectName.Name = "txtProjectName";
            this.txtProjectName.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtProjectName.Size = new System.Drawing.Size(959, 26);
            this.txtProjectName.TabIndex = 19;
            this.txtProjectName.TextChanged += new System.EventHandler(this.txtProjectName_TextChanged);
            // 
            // OR
            // 
            this.OR.AutoSize = true;
            this.OR.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(186)));
            this.OR.Location = new System.Drawing.Point(505, 183);
            this.OR.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.OR.Name = "OR";
            this.OR.Size = new System.Drawing.Size(82, 47);
            this.OR.TabIndex = 18;
            this.OR.Text = "OR";
            // 
            // btnResetForm
            // 
            this.btnResetForm.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnResetForm.ForeColor = System.Drawing.Color.Black;
            this.btnResetForm.Location = new System.Drawing.Point(654, 361);
            this.btnResetForm.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnResetForm.Name = "btnResetForm";
            this.btnResetForm.Size = new System.Drawing.Size(166, 57);
            this.btnResetForm.TabIndex = 16;
            this.btnResetForm.Text = "Reset";
            this.btnResetForm.UseVisualStyleBackColor = true;
            this.btnResetForm.Click += new System.EventHandler(this.btnResetForm_Click);
            // 
            // btnAnalyzeCode
            // 
            this.btnAnalyzeCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAnalyzeCode.ForeColor = System.Drawing.Color.Black;
            this.btnAnalyzeCode.Location = new System.Drawing.Point(852, 361);
            this.btnAnalyzeCode.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnAnalyzeCode.Name = "btnAnalyzeCode";
            this.btnAnalyzeCode.Size = new System.Drawing.Size(165, 57);
            this.btnAnalyzeCode.TabIndex = 15;
            this.btnAnalyzeCode.Text = "Analyse";
            this.btnAnalyzeCode.UseVisualStyleBackColor = true;
            this.btnAnalyzeCode.Click += new System.EventHandler(this.btnAnalyzeCode_Click);
            // 
            // cmbInterval
            // 
            this.cmbInterval.AllowDrop = true;
            this.cmbInterval.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInterval.FormattingEnabled = true;
            this.cmbInterval.Location = new System.Drawing.Point(809, 280);
            this.cmbInterval.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmbInterval.Name = "cmbInterval";
            this.cmbInterval.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmbInterval.Size = new System.Drawing.Size(204, 28);
            this.cmbInterval.TabIndex = 14;
            this.cmbInterval.UseWaitCursor = true;
            this.cmbInterval.SelectedIndexChanged += new System.EventHandler(this.cmbInterval_SelectedIndexChanged);
            // 
            // chkOneTimeAnalyse
            // 
            this.chkOneTimeAnalyse.AutoSize = true;
            this.chkOneTimeAnalyse.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkOneTimeAnalyse.Cursor = System.Windows.Forms.Cursors.Default;
            this.chkOneTimeAnalyse.Location = new System.Drawing.Point(293, 280);
            this.chkOneTimeAnalyse.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkOneTimeAnalyse.Name = "chkOneTimeAnalyse";
            this.chkOneTimeAnalyse.Size = new System.Drawing.Size(163, 24);
            this.chkOneTimeAnalyse.TabIndex = 11;
            this.chkOneTimeAnalyse.Text = "One Time Analyse";
            this.chkOneTimeAnalyse.UseVisualStyleBackColor = true;
            this.chkOneTimeAnalyse.CheckStateChanged += new System.EventHandler(this.chkOneTimeAnalyse_CheckStateChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(622, 119);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 20);
            this.label3.TabIndex = 10;
            this.label3.Text = "Git Hub URL";
            // 
            // txtGitSourceUrl
            // 
            this.txtGitSourceUrl.Location = new System.Drawing.Point(622, 143);
            this.txtGitSourceUrl.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtGitSourceUrl.Name = "txtGitSourceUrl";
            this.txtGitSourceUrl.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtGitSourceUrl.Size = new System.Drawing.Size(391, 26);
            this.txtGitSourceUrl.TabIndex = 9;
            this.txtGitSourceUrl.TextChanged += new System.EventHandler(this.txtGitSourceUrl_TextChanged);
            this.txtGitSourceUrl.Enter += new System.EventHandler(this.txtGitSourceUrl_Enter);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(54, 119);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 20);
            this.label2.TabIndex = 7;
            this.label2.Text = "Local File Path";
            // 
            // txtLocalSourceUrl
            // 
            this.txtLocalSourceUrl.Location = new System.Drawing.Point(54, 143);
            this.txtLocalSourceUrl.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtLocalSourceUrl.Name = "txtLocalSourceUrl";
            this.txtLocalSourceUrl.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtLocalSourceUrl.Size = new System.Drawing.Size(391, 26);
            this.txtLocalSourceUrl.TabIndex = 5;
            this.txtLocalSourceUrl.Enter += new System.EventHandler(this.txtLocalSourceUrl_Enter);
            // 
            // btnBrowseLocalSource
            // 
            this.btnBrowseLocalSource.Location = new System.Drawing.Point(293, 198);
            this.btnBrowseLocalSource.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnBrowseLocalSource.Name = "btnBrowseLocalSource";
            this.btnBrowseLocalSource.Size = new System.Drawing.Size(152, 35);
            this.btnBrowseLocalSource.TabIndex = 4;
            this.btnBrowseLocalSource.Text = "Browse";
            this.btnBrowseLocalSource.UseVisualStyleBackColor = true;
            this.btnBrowseLocalSource.Click += new System.EventHandler(this.btnBrowseLocalSource_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.OldAnalysisGrid);
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPage1.Size = new System.Drawing.Size(1096, 476);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Existing";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // OldAnalysisGrid
            // 
            this.OldAnalysisGrid.AllowUserToAddRows = false;
            this.OldAnalysisGrid.AllowUserToDeleteRows = false;
            this.OldAnalysisGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.OldAnalysisGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.OldAnalysisGrid.Location = new System.Drawing.Point(4, 8);
            this.OldAnalysisGrid.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.OldAnalysisGrid.Name = "OldAnalysisGrid";
            this.OldAnalysisGrid.ReadOnly = true;
            this.OldAnalysisGrid.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.OldAnalysisGrid.RowHeadersWidth = 25;
            this.OldAnalysisGrid.Size = new System.Drawing.Size(1092, 451);
            this.OldAnalysisGrid.TabIndex = 0;
            this.OldAnalysisGrid.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // chkTimeToTimeAnalyze
            // 
            this.chkTimeToTimeAnalyze.AutoSize = true;
            this.chkTimeToTimeAnalyze.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkTimeToTimeAnalyze.Cursor = System.Windows.Forms.Cursors.Default;
            this.chkTimeToTimeAnalyze.Location = new System.Drawing.Point(622, 280);
            this.chkTimeToTimeAnalyze.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkTimeToTimeAnalyze.Name = "chkTimeToTimeAnalyze";
            this.chkTimeToTimeAnalyze.Size = new System.Drawing.Size(185, 24);
            this.chkTimeToTimeAnalyze.TabIndex = 12;
            this.chkTimeToTimeAnalyze.Text = "Time to Time Analyse";
            this.chkTimeToTimeAnalyze.UseVisualStyleBackColor = true;
            this.chkTimeToTimeAnalyze.CheckedChanged += new System.EventHandler(this.chkTimeToTimeAnalyze_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1138, 622);
            this.Controls.Add(this.tabControl1);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Padding = new System.Windows.Forms.Padding(30, 92, 30, 31);
            this.Resizable = false;
            this.RightToLeftLayout = true;
            this.Text = "Code Clone Analysis";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.OldAnalysisGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtGitSourceUrl;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtLocalSourceUrl;
        private System.Windows.Forms.Button btnBrowseLocalSource;
        private System.Windows.Forms.CheckBox chkOneTimeAnalyse;
        private System.Windows.Forms.ComboBox cmbInterval;
        private System.Windows.Forms.Button btnAnalyzeCode;
        private System.Windows.Forms.Button btnResetForm;
        private System.Windows.Forms.Label OR;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtProjectName;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.DataGridView OldAnalysisGrid;
        private System.Windows.Forms.CheckBox chkTimeToTimeAnalyze;

    }
}

