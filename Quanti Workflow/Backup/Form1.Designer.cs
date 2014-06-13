namespace MGF_HTM_New {
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.DATFileButton = new System.Windows.Forms.Button();
            this.MGFinFileButton = new System.Windows.Forms.Button();
            this.MGFoutFileButton = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.button1 = new System.Windows.Forms.Button();
            this.DatFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.MGFFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.MGFoutFileName = new System.Windows.Forms.TextBox();
            this.MGFInFileName = new System.Windows.Forms.TextBox();
            this.DatFileName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 16);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "DAT in:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 44);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "MGF in:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 73);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 17);
            this.label3.TabIndex = 2;
            this.label3.Text = "MGF out:";
            // 
            // DATFileButton
            // 
            this.DATFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DATFileButton.Location = new System.Drawing.Point(577, 11);
            this.DATFileButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.DATFileButton.Name = "DATFileButton";
            this.DATFileButton.Size = new System.Drawing.Size(64, 25);
            this.DATFileButton.TabIndex = 1;
            this.DATFileButton.Text = "...";
            this.DATFileButton.UseVisualStyleBackColor = true;
            this.DATFileButton.Click += new System.EventHandler(this.DATFileButton_Click);
            // 
            // MGFinFileButton
            // 
            this.MGFinFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MGFinFileButton.Location = new System.Drawing.Point(577, 39);
            this.MGFinFileButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MGFinFileButton.Name = "MGFinFileButton";
            this.MGFinFileButton.Size = new System.Drawing.Size(64, 25);
            this.MGFinFileButton.TabIndex = 3;
            this.MGFinFileButton.Text = "...";
            this.MGFinFileButton.UseVisualStyleBackColor = true;
            this.MGFinFileButton.Click += new System.EventHandler(this.MGFinFileButton_Click);
            // 
            // MGFoutFileButton
            // 
            this.MGFoutFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MGFoutFileButton.Location = new System.Drawing.Point(577, 68);
            this.MGFoutFileButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MGFoutFileButton.Name = "MGFoutFileButton";
            this.MGFoutFileButton.Size = new System.Drawing.Size(64, 25);
            this.MGFoutFileButton.TabIndex = 5;
            this.MGFoutFileButton.Text = "...";
            this.MGFoutFileButton.UseVisualStyleBackColor = true;
            this.MGFoutFileButton.Click += new System.EventHandler(this.MGFoutFileButton_Click);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(19, 170);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(620, 28);
            this.progressBar1.TabIndex = 15;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(280, 214);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 28);
            this.button1.TabIndex = 10;
            this.button1.Text = "GO!";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // DatFileDialog
            // 
            this.DatFileDialog.DefaultExt = "dat";
            this.DatFileDialog.Filter = "Mascot DAT  files (*.dat)|*.dat";
            this.DatFileDialog.RestoreDirectory = true;
            // 
            // MGFFileDialog
            // 
            this.MGFFileDialog.DefaultExt = "mgf";
            this.MGFFileDialog.FileName = "openFileDialog1";
            this.MGFFileDialog.Filter = "Mascot generic format files (*.mgf)|*.mgf";
            this.MGFFileDialog.RestoreDirectory = true;
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "mgf";
            // 
            // textBox2
            // 
            this.textBox2.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::MGF_HTM_New.Properties.Settings.Default, "FDRThreshold", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox2.Location = new System.Drawing.Point(521, 116);
            this.textBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(41, 22);
            this.textBox2.TabIndex = 9;
            this.textBox2.Text = global::MGF_HTM_New.Properties.Settings.Default.FDRThreshold;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(356, 118);
            this.radioButton2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(151, 21);
            this.radioButton2.TabIndex = 8;
            this.radioButton2.Text = "FDR Threshold (%)";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = global::MGF_HTM_New.Properties.Settings.Default.MScore;
            this.radioButton1.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::MGF_HTM_New.Properties.Settings.Default, "MScore", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.radioButton1.Location = new System.Drawing.Point(92, 118);
            this.radioButton1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(180, 21);
            this.radioButton1.TabIndex = 6;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Mascot score threshold ";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // textBox1
            // 
            this.textBox1.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::MGF_HTM_New.Properties.Settings.Default, "Mascot_Threshold", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox1.Location = new System.Drawing.Point(284, 117);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(47, 22);
            this.textBox1.TabIndex = 7;
            this.textBox1.Text = global::MGF_HTM_New.Properties.Settings.Default.Mascot_Threshold;
            // 
            // MGFoutFileName
            // 
            this.MGFoutFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.MGFoutFileName.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::MGF_HTM_New.Properties.Settings.Default, "MGF_Out", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.MGFoutFileName.Location = new System.Drawing.Point(96, 69);
            this.MGFoutFileName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MGFoutFileName.Name = "MGFoutFileName";
            this.MGFoutFileName.Size = new System.Drawing.Size(472, 22);
            this.MGFoutFileName.TabIndex = 4;
            this.MGFoutFileName.Text = global::MGF_HTM_New.Properties.Settings.Default.MGF_Out;
            // 
            // MGFInFileName
            // 
            this.MGFInFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.MGFInFileName.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::MGF_HTM_New.Properties.Settings.Default, "MGF_in", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.MGFInFileName.Location = new System.Drawing.Point(96, 41);
            this.MGFInFileName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MGFInFileName.Name = "MGFInFileName";
            this.MGFInFileName.ReadOnly = true;
            this.MGFInFileName.Size = new System.Drawing.Size(472, 22);
            this.MGFInFileName.TabIndex = 2;
            this.MGFInFileName.Text = global::MGF_HTM_New.Properties.Settings.Default.MGF_in;
            // 
            // DatFileName
            // 
            this.DatFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.DatFileName.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::MGF_HTM_New.Properties.Settings.Default, "Dat_in", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.DatFileName.Location = new System.Drawing.Point(96, 12);
            this.DatFileName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.DatFileName.Name = "DatFileName";
            this.DatFileName.ReadOnly = true;
            this.DatFileName.Size = new System.Drawing.Size(472, 22);
            this.DatFileName.TabIndex = 0;
            this.DatFileName.Text = global::MGF_HTM_New.Properties.Settings.Default.Dat_in;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 220);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(0, 17);
            this.label4.TabIndex = 16;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(657, 266);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.radioButton2);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.MGFoutFileButton);
            this.Controls.Add(this.MGFinFileButton);
            this.Controls.Add(this.DATFileButton);
            this.Controls.Add(this.MGFoutFileName);
            this.Controls.Add(this.MGFInFileName);
            this.Controls.Add(this.DatFileName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MinimumSize = new System.Drawing.Size(662, 297);
            this.Name = "Form1";
            this.Text = "MGF DAT Filtering  v.2.0.4";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox DatFileName;
        private System.Windows.Forms.TextBox MGFInFileName;
        private System.Windows.Forms.TextBox MGFoutFileName;
        private System.Windows.Forms.Button DATFileButton;
        private System.Windows.Forms.Button MGFinFileButton;
        private System.Windows.Forms.Button MGFoutFileButton;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.OpenFileDialog DatFileDialog;
        private System.Windows.Forms.OpenFileDialog MGFFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label4;


    }
}

