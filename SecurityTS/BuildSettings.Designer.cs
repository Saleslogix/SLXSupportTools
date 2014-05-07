namespace SecurityTS
{
    partial class BuildSettings
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
            this.dgvBSettings = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            this.txtebs = new System.Windows.Forms.TextBox();
            this.txtbs = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtAssemblies = new System.Windows.Forms.TextBox();
            this.txtBuildPath = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.Machine = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SolutionFolder = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AssembliesFolder = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBSettings)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvBSettings
            // 
            this.dgvBSettings.AllowUserToAddRows = false;
            this.dgvBSettings.AllowUserToDeleteRows = false;
            this.dgvBSettings.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvBSettings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBSettings.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Machine,
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column5,
            this.Column6,
            this.SolutionFolder,
            this.AssembliesFolder});
            this.dgvBSettings.Location = new System.Drawing.Point(12, 12);
            this.dgvBSettings.Name = "dgvBSettings";
            this.dgvBSettings.ReadOnly = true;
            this.dgvBSettings.RowHeadersVisible = false;
            this.dgvBSettings.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvBSettings.Size = new System.Drawing.Size(588, 128);
            this.dgvBSettings.TabIndex = 0;
            this.dgvBSettings.SelectionChanged += new System.EventHandler(this.dgvBSettings_SelectionChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(254, 301);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(64, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtebs
            // 
            this.txtebs.Location = new System.Drawing.Point(102, 204);
            this.txtebs.Multiline = true;
            this.txtebs.Name = "txtebs";
            this.txtebs.Size = new System.Drawing.Size(501, 42);
            this.txtebs.TabIndex = 2;
            // 
            // txtbs
            // 
            this.txtbs.Location = new System.Drawing.Point(102, 252);
            this.txtbs.Multiline = true;
            this.txtbs.Name = "txtbs";
            this.txtbs.Size = new System.Drawing.Size(501, 43);
            this.txtbs.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(38, 219);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Expected:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(53, 265);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Actual:";
            // 
            // txtAssemblies
            // 
            this.txtAssemblies.Location = new System.Drawing.Point(102, 174);
            this.txtAssemblies.Name = "txtAssemblies";
            this.txtAssemblies.Size = new System.Drawing.Size(501, 20);
            this.txtAssemblies.TabIndex = 6;
            // 
            // txtBuildPath
            // 
            this.txtBuildPath.Location = new System.Drawing.Point(102, 148);
            this.txtBuildPath.Name = "txtBuildPath";
            this.txtBuildPath.Size = new System.Drawing.Size(501, 20);
            this.txtBuildPath.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(38, 151);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Build Path:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 177);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Assemblies Path:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 252);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(92, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Build Search Path";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 206);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(92, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Build Search Path";
            // 
            // Machine
            // 
            this.Machine.HeaderText = "Machine";
            this.Machine.Name = "Machine";
            this.Machine.ReadOnly = true;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Domain";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "User";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "Version";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "Match";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            // 
            // Column5
            // 
            this.Column5.HeaderText = "Column5";
            this.Column5.Name = "Column5";
            this.Column5.ReadOnly = true;
            this.Column5.Visible = false;
            // 
            // Column6
            // 
            this.Column6.HeaderText = "Column6";
            this.Column6.Name = "Column6";
            this.Column6.ReadOnly = true;
            this.Column6.Visible = false;
            // 
            // SolutionFolder
            // 
            this.SolutionFolder.HeaderText = "SolutionFolder";
            this.SolutionFolder.Name = "SolutionFolder";
            this.SolutionFolder.ReadOnly = true;
            this.SolutionFolder.Visible = false;
            // 
            // AssembliesFolder
            // 
            this.AssembliesFolder.HeaderText = "AssembliesFolder";
            this.AssembliesFolder.Name = "AssembliesFolder";
            this.AssembliesFolder.ReadOnly = true;
            this.AssembliesFolder.Visible = false;
            // 
            // BuildSettings
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(612, 329);
            this.ControlBox = false;
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtBuildPath);
            this.Controls.Add(this.txtAssemblies);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtbs);
            this.Controls.Add(this.txtebs);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dgvBSettings);
            this.Name = "BuildSettings";
            this.Text = "BuildSettings";
            this.Load += new System.EventHandler(this.BuildSettings_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBSettings)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvBSettings;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtebs;
        private System.Windows.Forms.TextBox txtbs;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtAssemblies;
        private System.Windows.Forms.TextBox txtBuildPath;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Machine;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn SolutionFolder;
        private System.Windows.Forms.DataGridViewTextBoxColumn AssembliesFolder;
    }
}