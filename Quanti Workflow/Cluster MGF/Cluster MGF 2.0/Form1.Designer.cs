namespace Cluster_MGF {
    partial class Form1 {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.MGFlist = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.AddButton = new System.Windows.Forms.Button();
            this.DeleteButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.OutMgfBox = new System.Windows.Forms.TextBox();
            this.SaveMGFbutton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.RTMinBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.MassDevBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.MS2PeaksBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.MS2DaBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.GoButton = new System.Windows.Forms.Button();
            this.openMGFDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveMGFDialog = new System.Windows.Forms.SaveFileDialog();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.ProgressLabel = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // MGFlist
            // 
            this.MGFlist.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MGFlist.FormattingEnabled = true;
            this.MGFlist.HorizontalScrollbar = true;
            this.MGFlist.ItemHeight = 16;
            this.MGFlist.Location = new System.Drawing.Point(12, 25);
            this.MGFlist.Name = "MGFlist";
            this.MGFlist.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.MGFlist.Size = new System.Drawing.Size(837, 132);
            this.MGFlist.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Input MGFs:";
            // 
            // AddButton
            // 
            this.AddButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AddButton.Location = new System.Drawing.Point(12, 177);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(110, 23);
            this.AddButton.TabIndex = 1;
            this.AddButton.Text = "Add...";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // DeleteButton
            // 
            this.DeleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.DeleteButton.Location = new System.Drawing.Point(739, 177);
            this.DeleteButton.Name = "DeleteButton";
            this.DeleteButton.Size = new System.Drawing.Size(110, 23);
            this.DeleteButton.TabIndex = 2;
            this.DeleteButton.Text = "Delete";
            this.DeleteButton.UseVisualStyleBackColor = true;
            this.DeleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 218);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "Output MGF:";
            // 
            // OutMgfBox
            // 
            this.OutMgfBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.OutMgfBox.Location = new System.Drawing.Point(104, 215);
            this.OutMgfBox.Name = "OutMgfBox";
            this.OutMgfBox.Size = new System.Drawing.Size(701, 22);
            this.OutMgfBox.TabIndex = 3;
            // 
            // SaveMGFbutton
            // 
            this.SaveMGFbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.SaveMGFbutton.Location = new System.Drawing.Point(811, 215);
            this.SaveMGFbutton.Name = "SaveMGFbutton";
            this.SaveMGFbutton.Size = new System.Drawing.Size(37, 20);
            this.SaveMGFbutton.TabIndex = 4;
            this.SaveMGFbutton.Text = "...";
            this.SaveMGFbutton.UseVisualStyleBackColor = true;
            this.SaveMGFbutton.Click += new System.EventHandler(this.SaveMGFbutton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.RTMinBox);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.MassDevBox);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(12, 247);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(398, 86);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Filters:";
            // 
            // textBox1
            // 
            this.textBox1.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Cluster_MGF.Properties.Settings.Default, "SpecPerClust", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox1.Location = new System.Drawing.Point(172, 51);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(52, 22);
            this.textBox1.TabIndex = 4;
            this.textBox1.Text = global::Cluster_MGF.Properties.Settings.Default.SpecPerClust;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(160, 17);
            this.label3.TabIndex = 3;
            this.label3.Text = "Min. spectra per cluster:";
            // 
            // RTMinBox
            // 
            this.RTMinBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Cluster_MGF.Properties.Settings.Default, "RTDevMin", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.RTMinBox.Location = new System.Drawing.Point(337, 19);
            this.RTMinBox.Name = "RTMinBox";
            this.RTMinBox.Size = new System.Drawing.Size(37, 22);
            this.RTMinBox.TabIndex = 1;
            this.RTMinBox.Text = global::Cluster_MGF.Properties.Settings.Default.RTDevMin;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(204, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(130, 17);
            this.label5.TabIndex = 2;
            this.label5.Text = "RT Deviation (min):";
            // 
            // MassDevBox
            // 
            this.MassDevBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Cluster_MGF.Properties.Settings.Default, "MassDev", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.MassDevBox.Location = new System.Drawing.Point(161, 19);
            this.MassDevBox.Name = "MassDevBox";
            this.MassDevBox.Size = new System.Drawing.Size(37, 22);
            this.MassDevBox.TabIndex = 0;
            this.MassDevBox.Text = global::Cluster_MGF.Properties.Settings.Default.MassDev;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(149, 17);
            this.label4.TabIndex = 0;
            this.label4.Text = "Mass Deviation (ppm):";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox2.Controls.Add(this.textBox8);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.MS2PeaksBox);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.MS2DaBox);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Location = new System.Drawing.Point(439, 247);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(410, 53);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "MS/MS";
            // 
            // textBox8
            // 
            this.textBox8.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Cluster_MGF.Properties.Settings.Default, "MS2PeakMatches", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox8.Location = new System.Drawing.Point(298, 22);
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new System.Drawing.Size(37, 22);
            this.textBox8.TabIndex = 2;
            this.textBox8.Text = global::Cluster_MGF.Properties.Settings.Default.MS2PeakMatches;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(207, 25);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(93, 17);
            this.label9.TabIndex = 4;
            this.label9.Text = "# of matches:";
            // 
            // MS2PeaksBox
            // 
            this.MS2PeaksBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Cluster_MGF.Properties.Settings.Default, "MS2PeaksNumber", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.MS2PeaksBox.Location = new System.Drawing.Point(164, 22);
            this.MS2PeaksBox.Name = "MS2PeaksBox";
            this.MS2PeaksBox.Size = new System.Drawing.Size(37, 22);
            this.MS2PeaksBox.TabIndex = 1;
            this.MS2PeaksBox.Text = global::Cluster_MGF.Properties.Settings.Default.MS2PeaksNumber;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(79, 25);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(79, 17);
            this.label8.TabIndex = 2;
            this.label8.Text = "# of Peaks:";
            // 
            // MS2DaBox
            // 
            this.MS2DaBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Cluster_MGF.Properties.Settings.Default, "MS2Da", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.MS2DaBox.Location = new System.Drawing.Point(36, 22);
            this.MS2DaBox.Name = "MS2DaBox";
            this.MS2DaBox.Size = new System.Drawing.Size(37, 22);
            this.MS2DaBox.TabIndex = 0;
            this.MS2DaBox.Text = global::Cluster_MGF.Properties.Settings.Default.MS2Da;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 25);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(30, 17);
            this.label7.TabIndex = 0;
            this.label7.Text = "Da:";
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(15, 381);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(837, 23);
            this.progressBar1.TabIndex = 12;
            // 
            // GoButton
            // 
            this.GoButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.GoButton.Location = new System.Drawing.Point(349, 423);
            this.GoButton.Name = "GoButton";
            this.GoButton.Size = new System.Drawing.Size(75, 23);
            this.GoButton.TabIndex = 10;
            this.GoButton.Text = "Go";
            this.GoButton.UseVisualStyleBackColor = true;
            this.GoButton.Click += new System.EventHandler(this.GoButton_Click);
            // 
            // openMGFDialog
            // 
            this.openMGFDialog.DefaultExt = "MGF";
            this.openMGFDialog.Filter = "Mascot generic files (*.mgf)|*.mgf";
            this.openMGFDialog.Multiselect = true;
            this.openMGFDialog.RestoreDirectory = true;
            // 
            // saveMGFDialog
            // 
            this.saveMGFDialog.DefaultExt = "MGF";
            this.saveMGFDialog.Filter = "Mascot generic files (*.mgf)|*.mgf";
            this.saveMGFDialog.OverwritePrompt = false;
            this.saveMGFDialog.RestoreDirectory = true;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // ProgressLabel
            // 
            this.ProgressLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ProgressLabel.AutoSize = true;
            this.ProgressLabel.Location = new System.Drawing.Point(18, 352);
            this.ProgressLabel.Name = "ProgressLabel";
            this.ProgressLabel.Size = new System.Drawing.Size(0, 17);
            this.ProgressLabel.TabIndex = 13;
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(861, 457);
            this.Controls.Add(this.ProgressLabel);
            this.Controls.Add(this.GoButton);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.SaveMGFbutton);
            this.Controls.Add(this.OutMgfBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.DeleteButton);
            this.Controls.Add(this.AddButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.MGFlist);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(879, 492);
            this.Name = "Form1";
            this.Text = "MGF Cluster and Align 2.1.1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox MGFlist;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.Button DeleteButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox OutMgfBox;
        private System.Windows.Forms.Button SaveMGFbutton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox MassDevBox;
        private System.Windows.Forms.TextBox RTMinBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox MS2PeaksBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox MS2DaBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox8;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button GoButton;
        private System.Windows.Forms.OpenFileDialog openMGFDialog;
        private System.Windows.Forms.SaveFileDialog saveMGFDialog;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Label ProgressLabel;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label3;
    }
}

