namespace ExtTDG
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dgvGenerators = new System.Windows.Forms.DataGridView();
            this.colIsActive = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colDataClass = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAllowedCharacters = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAnomalyCharacters = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMinLength = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMaxLength = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHasAnomalies = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colIsUnique = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.tbNumItems = new System.Windows.Forms.TextBox();
            this.tbAnomalyChance = new System.Windows.Forms.TextBox();
            this.tbFilePath = new System.Windows.Forms.TextBox();
            this.btnOpenFileDialog = new System.Windows.Forms.Button();
            this.cbAllowOverwrite = new System.Windows.Forms.CheckBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tsStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.tsStatusDuration = new System.Windows.Forms.ToolStripStatusLabel();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGenerators)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dgvGenerators);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(862, 272);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Generators";
            // 
            // dgvGenerators
            // 
            this.dgvGenerators.AllowUserToAddRows = false;
            this.dgvGenerators.AllowUserToDeleteRows = false;
            this.dgvGenerators.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvGenerators.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvGenerators.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colIsActive,
            this.colDataClass,
            this.colAllowedCharacters,
            this.colAnomalyCharacters,
            this.colMinLength,
            this.colMaxLength,
            this.colHasAnomalies,
            this.colIsUnique});
            this.dgvGenerators.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvGenerators.Location = new System.Drawing.Point(3, 16);
            this.dgvGenerators.MultiSelect = false;
            this.dgvGenerators.Name = "dgvGenerators";
            this.dgvGenerators.RowHeadersVisible = false;
            this.dgvGenerators.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvGenerators.Size = new System.Drawing.Size(856, 253);
            this.dgvGenerators.TabIndex = 0;
            // 
            // colIsActive
            // 
            this.colIsActive.HeaderText = "Is active?";
            this.colIsActive.Name = "colIsActive";
            this.colIsActive.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colIsActive.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // colDataClass
            // 
            this.colDataClass.HeaderText = "DataClass";
            this.colDataClass.Name = "colDataClass";
            this.colDataClass.ReadOnly = true;
            // 
            // colAllowedCharacters
            // 
            this.colAllowedCharacters.HeaderText = "Allowed characters";
            this.colAllowedCharacters.Name = "colAllowedCharacters";
            // 
            // colAnomalyCharacters
            // 
            this.colAnomalyCharacters.HeaderText = "Anomaly characters";
            this.colAnomalyCharacters.Name = "colAnomalyCharacters";
            // 
            // colMinLength
            // 
            this.colMinLength.HeaderText = "Minimum length / value";
            this.colMinLength.Name = "colMinLength";
            // 
            // colMaxLength
            // 
            this.colMaxLength.HeaderText = "Maximum length / value";
            this.colMaxLength.Name = "colMaxLength";
            // 
            // colHasAnomalies
            // 
            this.colHasAnomalies.HeaderText = "Has anomalies?";
            this.colHasAnomalies.Name = "colHasAnomalies";
            this.colHasAnomalies.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colHasAnomalies.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // colIsUnique
            // 
            this.colIsUnique.HeaderText = "Is unique?";
            this.colIsUnique.Name = "colIsUnique";
            this.colIsUnique.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colIsUnique.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tableLayoutPanel1);
            this.groupBox2.Location = new System.Drawing.Point(3, 281);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(755, 217);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Session parameters";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnGenerate, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.tbNumItems, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.tbAnomalyChance, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.tbFilePath, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnOpenFileDialog, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.cbAllowOverwrite, 1, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 53.19149F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 46.80851F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(749, 198);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(140, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Number of items to generate";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(134, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Anomaly chance (0.0 - 1.0)";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(93, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Result file location";
            // 
            // btnGenerate
            // 
            this.btnGenerate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnGenerate.Location = new System.Drawing.Point(3, 145);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(194, 50);
            this.btnGenerate.TabIndex = 3;
            this.btnGenerate.Text = "Generate";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // tbNumItems
            // 
            this.tbNumItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbNumItems.Location = new System.Drawing.Point(203, 3);
            this.tbNumItems.Name = "tbNumItems";
            this.tbNumItems.Size = new System.Drawing.Size(343, 20);
            this.tbNumItems.TabIndex = 4;
            this.tbNumItems.Text = "1000";
            // 
            // tbAnomalyChance
            // 
            this.tbAnomalyChance.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbAnomalyChance.Location = new System.Drawing.Point(203, 45);
            this.tbAnomalyChance.Name = "tbAnomalyChance";
            this.tbAnomalyChance.Size = new System.Drawing.Size(343, 20);
            this.tbAnomalyChance.TabIndex = 5;
            this.tbAnomalyChance.Text = "0,1";
            // 
            // tbFilePath
            // 
            this.tbFilePath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbFilePath.Location = new System.Drawing.Point(203, 82);
            this.tbFilePath.Name = "tbFilePath";
            this.tbFilePath.Size = new System.Drawing.Size(343, 20);
            this.tbFilePath.TabIndex = 6;
            // 
            // btnOpenFileDialog
            // 
            this.btnOpenFileDialog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnOpenFileDialog.Location = new System.Drawing.Point(552, 82);
            this.btnOpenFileDialog.Name = "btnOpenFileDialog";
            this.btnOpenFileDialog.Size = new System.Drawing.Size(194, 26);
            this.btnOpenFileDialog.TabIndex = 7;
            this.btnOpenFileDialog.Text = "Select file...";
            this.btnOpenFileDialog.UseVisualStyleBackColor = true;
            this.btnOpenFileDialog.Click += new System.EventHandler(this.btnOpenFileDialog_Click);
            // 
            // cbAllowOverwrite
            // 
            this.cbAllowOverwrite.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cbAllowOverwrite.AutoSize = true;
            this.cbAllowOverwrite.Location = new System.Drawing.Point(203, 118);
            this.cbAllowOverwrite.Name = "cbAllowOverwrite";
            this.cbAllowOverwrite.Size = new System.Drawing.Size(169, 17);
            this.cbAllowOverwrite.TabIndex = 8;
            this.cbAllowOverwrite.Text = "Allow overwrite to existing file?";
            this.cbAllowOverwrite.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsStatusLabel,
            this.tsProgressBar,
            this.tsStatusDuration});
            this.statusStrip1.Location = new System.Drawing.Point(0, 547);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(886, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tsStatusLabel
            // 
            this.tsStatusLabel.Name = "tsStatusLabel";
            this.tsStatusLabel.Size = new System.Drawing.Size(39, 17);
            this.tsStatusLabel.Text = "Ready";
            // 
            // tsProgressBar
            // 
            this.tsProgressBar.Name = "tsProgressBar";
            this.tsProgressBar.Size = new System.Drawing.Size(200, 16);
            // 
            // tsStatusDuration
            // 
            this.tsStatusDuration.Name = "tsStatusDuration";
            this.tsStatusDuration.Size = new System.Drawing.Size(199, 17);
            this.tsStatusDuration.Text = "Last session duration in milliseconds";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.FileName = "results.xlsx";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(886, 569);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "ExtTDG";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvGenerators)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dgvGenerators;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.TextBox tbNumItems;
        private System.Windows.Forms.TextBox tbAnomalyChance;
        private System.Windows.Forms.TextBox tbFilePath;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tsStatusLabel;
        private System.Windows.Forms.ToolStripProgressBar tsProgressBar;
        private System.Windows.Forms.ToolStripStatusLabel tsStatusDuration;
        private System.Windows.Forms.Button btnOpenFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.CheckBox cbAllowOverwrite;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colIsActive;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDataClass;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAllowedCharacters;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAnomalyCharacters;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMinLength;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMaxLength;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colHasAnomalies;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colIsUnique;
    }
}

