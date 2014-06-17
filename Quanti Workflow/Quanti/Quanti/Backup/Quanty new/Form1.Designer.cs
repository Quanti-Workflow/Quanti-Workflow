namespace Quanty {
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
            Quanty.Properties.Settings settings1 = new Quanty.Properties.Settings();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("SEQUENCE - Aminoacid sequence");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("RT MASCOT - Retention time from Mascot");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("RT APEX - Retention Time from Quanty");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("RT SMD - Dispersion of Ion current over RT");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("SHIFT - Shift of Peak shape compaing with reference");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("CONV - Maximum convolution of peak shape over reference ");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("SMD Ratio - Ratio of Dispersions over reference");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("ESI CORRECTION - ESI Correction factor");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Integrated Intensity");
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("REF. INT. - Reference integrated intensity");
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("Log1 - decimal logarithm of Integrated intensity");
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("Log2 - decimal logarithm of reference intensity");
            System.Windows.Forms.TreeNode treeNode13 = new System.Windows.Forms.TreeNode("HIGHEST INTENSITY - Ion current in most intensive spectrum for this peptide");
            System.Windows.Forms.TreeNode treeNode14 = new System.Windows.Forms.TreeNode("MZ Mascot - m/z value from peptide sequence");
            System.Windows.Forms.TreeNode treeNode15 = new System.Windows.Forms.TreeNode("MZ Quanty - m/z value from spectrum");
            System.Windows.Forms.TreeNode treeNode16 = new System.Windows.Forms.TreeNode("CHARGE - Ion charge");
            System.Windows.Forms.TreeNode treeNode17 = new System.Windows.Forms.TreeNode("PI - Isoelectrtic point for peptide");
            System.Windows.Forms.TreeNode treeNode18 = new System.Windows.Forms.TreeNode("SPECTRA - Number of spectra ");
            System.Windows.Forms.TreeNode treeNode19 = new System.Windows.Forms.TreeNode("IPI - Protein Identifier");
            System.Windows.Forms.TreeNode treeNode20 = new System.Windows.Forms.TreeNode("Mascot Score - Best score by Mascot");
            System.Windows.Forms.TreeNode treeNode21 = new System.Windows.Forms.TreeNode("Modification Mass - Mass shift due to Post-translational modification");
            System.Windows.Forms.TreeNode treeNode22 = new System.Windows.Forms.TreeNode("ISOTOPE RATIO - Observed isotope ratio for 1st and 2nd isotopes");
            System.Windows.Forms.TreeNode treeNode23 = new System.Windows.Forms.TreeNode("ISOTOPE RATIO THEOR - Isotope ratio for 1st and 2nd isotopes derived from sequenc" +
                    "e");
            System.Windows.Forms.TreeNode treeNode24 = new System.Windows.Forms.TreeNode("COMMENT");
            System.Windows.Forms.TreeNode treeNode25 = new System.Windows.Forms.TreeNode("Peptide Report", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4,
            treeNode5,
            treeNode6,
            treeNode7,
            treeNode8,
            treeNode9,
            treeNode10,
            treeNode11,
            treeNode12,
            treeNode13,
            treeNode14,
            treeNode15,
            treeNode16,
            treeNode17,
            treeNode18,
            treeNode19,
            treeNode20,
            treeNode21,
            treeNode22,
            treeNode23,
            treeNode24});
            System.Windows.Forms.TreeNode treeNode26 = new System.Windows.Forms.TreeNode("PROTEIN ID");
            System.Windows.Forms.TreeNode treeNode27 = new System.Windows.Forms.TreeNode("SCORE - Sum of integrated intensities of all found peptides in this run");
            System.Windows.Forms.TreeNode treeNode28 = new System.Windows.Forms.TreeNode("REF. SCORE - Sum of integrated intensities of all found peptides in reference run" +
                    "");
            System.Windows.Forms.TreeNode treeNode29 = new System.Windows.Forms.TreeNode("DESCRIPTION - description of protein from database");
            System.Windows.Forms.TreeNode treeNode30 = new System.Windows.Forms.TreeNode("PEPTIDES IDENT- Number of peptides identified for this proteins from all runs");
            System.Windows.Forms.TreeNode treeNode31 = new System.Windows.Forms.TreeNode("PEPTIDE FOUND - Number of peptide found in this run for this peptide");
            System.Windows.Forms.TreeNode treeNode32 = new System.Windows.Forms.TreeNode("R-SQUARE - R^2 of peptide intensities correlation for peptides of this protein in" +
                    " this and referenced runs");
            System.Windows.Forms.TreeNode treeNode33 = new System.Windows.Forms.TreeNode("SLOPE - slope of less squares plot for peptide intensities in this and referenced" +
                    " runs");
            System.Windows.Forms.TreeNode treeNode34 = new System.Windows.Forms.TreeNode("MEDIAN - Median ratio of peptide intensities in this and referenced runs");
            System.Windows.Forms.TreeNode treeNode35 = new System.Windows.Forms.TreeNode("AVERAGE - Average of ratios of peptide intensities in this and referenced runs");
            System.Windows.Forms.TreeNode treeNode36 = new System.Windows.Forms.TreeNode("MIN. CONF. - minimum of confidence interval for peptide ratios for 95% confidence" +
                    " probability");
            System.Windows.Forms.TreeNode treeNode37 = new System.Windows.Forms.TreeNode("MAX. CONF. - maximum of confidence interval for peptide ratios for 95% confidence" +
                    " probability");
            System.Windows.Forms.TreeNode treeNode38 = new System.Windows.Forms.TreeNode("P-VALUE - probabilyty  to reach such R^2 by chance");
            System.Windows.Forms.TreeNode treeNode39 = new System.Windows.Forms.TreeNode("Protein Report", new System.Windows.Forms.TreeNode[] {
            treeNode26,
            treeNode27,
            treeNode28,
            treeNode29,
            treeNode30,
            treeNode31,
            treeNode32,
            treeNode33,
            treeNode34,
            treeNode35,
            treeNode36,
            treeNode37,
            treeNode38});
            System.Windows.Forms.TreeNode treeNode40 = new System.Windows.Forms.TreeNode("SEQUENCE - Aminoacid sequence of peptide");
            System.Windows.Forms.TreeNode treeNode41 = new System.Windows.Forms.TreeNode("PROTEIN ID - database protein identifier");
            System.Windows.Forms.TreeNode treeNode42 = new System.Windows.Forms.TreeNode("REF ABUNDANCE - peptide abundance in reference run");
            System.Windows.Forms.TreeNode treeNode43 = new System.Windows.Forms.TreeNode("MODIF - Modification desctription (if any)");
            System.Windows.Forms.TreeNode treeNode44 = new System.Windows.Forms.TreeNode("Ratios - relative peptide abundance calculated against reference file peptide abu" +
                    "ndance");
            System.Windows.Forms.TreeNode treeNode45 = new System.Windows.Forms.TreeNode("All runs peptide report", new System.Windows.Forms.TreeNode[] {
            treeNode40,
            treeNode41,
            treeNode42,
            treeNode43,
            treeNode44});
            System.Windows.Forms.TreeNode treeNode46 = new System.Windows.Forms.TreeNode("PROTEIN ID -  database protein identifier");
            System.Windows.Forms.TreeNode treeNode47 = new System.Windows.Forms.TreeNode("REF ABUNDANCE - Sum of integrated intensities of all found peptides in reference " +
                    "run");
            System.Windows.Forms.TreeNode treeNode48 = new System.Windows.Forms.TreeNode("DESCRIPTION  - description of protein from database");
            System.Windows.Forms.TreeNode treeNode49 = new System.Windows.Forms.TreeNode("PEPTIDES - Number of peptide found in reference run for this peptide");
            System.Windows.Forms.TreeNode treeNode50 = new System.Windows.Forms.TreeNode("Medians - Median ratio of peptide intensities in this and referenced runs");
            System.Windows.Forms.TreeNode treeNode51 = new System.Windows.Forms.TreeNode("Slopes - slope of less squares plot for peptide intensities in this and reference" +
                    "d runs");
            System.Windows.Forms.TreeNode treeNode52 = new System.Windows.Forms.TreeNode("Averages - Average of ratios of peptide intensities in this and referenced runs");
            System.Windows.Forms.TreeNode treeNode53 = new System.Windows.Forms.TreeNode("Min. - - minimum of confidence interval for peptide ratios for 95% confidence pro" +
                    "bability");
            System.Windows.Forms.TreeNode treeNode54 = new System.Windows.Forms.TreeNode("Max. - maximum of confidence interval for peptide ratios for 95% confidence proba" +
                    "bility");
            System.Windows.Forms.TreeNode treeNode55 = new System.Windows.Forms.TreeNode("R-Squares -  R^2 of peptide intensities correlation for peptides of this protein " +
                    "in this and referenced runs");
            System.Windows.Forms.TreeNode treeNode56 = new System.Windows.Forms.TreeNode("All runs protein report", new System.Windows.Forms.TreeNode[] {
            treeNode46,
            treeNode47,
            treeNode48,
            treeNode49,
            treeNode50,
            treeNode51,
            treeNode52,
            treeNode53,
            treeNode54,
            treeNode55});
            this.DATFileButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.AllCheckBox = new System.Windows.Forms.CheckBox();
            this.checkBox5 = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.DatFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.RAWFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.ProgressLabel = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.RawList = new System.Windows.Forms.ListBox();
            this.OutPathButton = new System.Windows.Forms.Button();
            this.AddRawButton = new System.Windows.Forms.Button();
            this.DeleteRawButton = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.panel4 = new System.Windows.Forms.Panel();
            this.LogList = new System.Windows.Forms.ListView();
            this.TimeColumn = new System.Windows.Forms.ColumnHeader();
            this.MessageHeader = new System.Windows.Forms.ColumnHeader();
            this.panel1 = new System.Windows.Forms.Panel();
            this.LogSaveButton = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.ReportTree = new System.Windows.Forms.TreeView();
            this.saveLogFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.RefCombo = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.DBFileName = new System.Windows.Forms.TextBox();
            this.dbOpenButton = new System.Windows.Forms.Button();
            this.DBFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.OutPathName = new System.Windows.Forms.TextBox();
            this.DatFileName = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // DATFileButton
            // 
            this.DATFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DATFileButton.Location = new System.Drawing.Point(705, 15);
            this.DATFileButton.Margin = new System.Windows.Forms.Padding(4);
            this.DATFileButton.Name = "DATFileButton";
            this.DATFileButton.Size = new System.Drawing.Size(68, 25);
            this.DATFileButton.TabIndex = 1;
            this.DATFileButton.Text = "...";
            this.DATFileButton.UseVisualStyleBackColor = true;
            this.DATFileButton.Click += new System.EventHandler(this.DATFileButton_Click);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 336);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 17);
            this.label3.TabIndex = 10;
            this.label3.Text = "Output path:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 20);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 17);
            this.label1.TabIndex = 7;
            this.label1.Text = "DAT in:";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.Location = new System.Drawing.Point(335, 698);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 28);
            this.button1.TabIndex = 8;
            this.button1.Text = "GO!";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(23, 655);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(4);
            this.progressBar1.Maximum = 102;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(751, 28);
            this.progressBar1.Step = 1;
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 17;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.textBox8);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.textBox5);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.textBox4);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.textBox3);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Location = new System.Drawing.Point(23, 368);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(369, 139);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Filter options";
            // 
            // textBox8
            // 
            this.textBox8.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Quanty.Properties.Settings.Default, "RTOrder", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox8.Location = new System.Drawing.Point(312, 94);
            this.textBox8.Margin = new System.Windows.Forms.Padding(4);
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new System.Drawing.Size(41, 22);
            this.textBox8.TabIndex = 11;
            this.textBox8.Text = global::Quanty.Properties.Settings.Default.RTOrder;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(220, 97);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(84, 17);
            this.label11.TabIndex = 10;
            this.label11.Text = "RT Order ±:";
            // 
            // textBox5
            // 
            this.textBox5.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Quanty.Properties.Settings.Default, "PeptperProt", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox5.Location = new System.Drawing.Point(160, 95);
            this.textBox5.Margin = new System.Windows.Forms.Padding(4);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(41, 22);
            this.textBox5.TabIndex = 9;
            this.textBox5.Text = global::Quanty.Properties.Settings.Default.PeptperProt;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(9, 98);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(140, 17);
            this.label9.TabIndex = 8;
            this.label9.Text = "Min peptides/protein:";
            // 
            // textBox4
            // 
            this.textBox4.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Quanty.Properties.Settings.Default, "RTErrorMinutes", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox4.Location = new System.Drawing.Point(312, 57);
            this.textBox4.Margin = new System.Windows.Forms.Padding(4);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(41, 22);
            this.textBox4.TabIndex = 7;
            this.textBox4.Text = global::Quanty.Properties.Settings.Default.RTErrorMinutes;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(221, 60);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(69, 17);
            this.label8.TabIndex = 6;
            this.label8.Text = "RT ± min:";
            // 
            // textBox3
            // 
            this.textBox3.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Quanty.Properties.Settings.Default, "RTErrorProc", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox3.Location = new System.Drawing.Point(312, 21);
            this.textBox3.Margin = new System.Windows.Forms.Padding(4);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(41, 22);
            this.textBox3.TabIndex = 5;
            this.textBox3.Text = global::Quanty.Properties.Settings.Default.RTErrorProc;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(221, 26);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 17);
            this.label7.TabIndex = 4;
            this.label7.Text = "RT ± %:";
            // 
            // textBox2
            // 
            this.textBox2.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Quanty.Properties.Settings.Default, "MassError", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox2.Location = new System.Drawing.Point(160, 58);
            this.textBox2.Margin = new System.Windows.Forms.Padding(4);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(41, 22);
            this.textBox2.TabIndex = 3;
            this.textBox2.Text = global::Quanty.Properties.Settings.Default.MassError;
            // 
            // textBox1
            // 
            this.textBox1.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Quanty.Properties.Settings.Default, "MascotScore", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox1.Location = new System.Drawing.Point(160, 21);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(41, 22);
            this.textBox1.TabIndex = 2;
            this.textBox1.Text = global::Quanty.Properties.Settings.Default.MascotScore;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 60);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(88, 17);
            this.label6.TabIndex = 1;
            this.label6.Text = "Mass ± ppm:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 26);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(114, 17);
            this.label5.TabIndex = 0;
            this.label5.Text = "Mascot Score >=";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox2.Controls.Add(this.AllCheckBox);
            this.groupBox2.Controls.Add(this.checkBox5);
            this.groupBox2.Controls.Add(this.checkBox1);
            this.groupBox2.Location = new System.Drawing.Point(412, 368);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox2.Size = new System.Drawing.Size(361, 139);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Misc. Options";
            // 
            // AllCheckBox
            // 
            this.AllCheckBox.AutoSize = true;
            this.AllCheckBox.Checked = global::Quanty.Properties.Settings.Default.QuantiAll;
            this.AllCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Quanty.Properties.Settings.Default, "QuantiAll", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.AllCheckBox.Location = new System.Drawing.Point(9, 81);
            this.AllCheckBox.Margin = new System.Windows.Forms.Padding(4);
            this.AllCheckBox.Name = "AllCheckBox";
            this.AllCheckBox.Size = new System.Drawing.Size(255, 21);
            this.AllCheckBox.TabIndex = 10;
            this.AllCheckBox.Text = "Quantify all mascot entries (.all file) ";
            this.AllCheckBox.UseVisualStyleBackColor = true;
            // 
            // checkBox5
            // 
            this.checkBox5.AutoSize = true;
            this.checkBox5.Checked = global::Quanty.Properties.Settings.Default.MascotScoreFiltering;
            this.checkBox5.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox5.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Quanty.Properties.Settings.Default, "MascotScoreFiltering", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox5.Location = new System.Drawing.Point(9, 53);
            this.checkBox5.Margin = new System.Windows.Forms.Padding(4);
            this.checkBox5.Name = "checkBox5";
            this.checkBox5.Size = new System.Drawing.Size(186, 21);
            this.checkBox5.TabIndex = 9;
            this.checkBox5.Text = "Use best mascot peptide";
            this.checkBox5.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            settings1.ConvThres = "0.6";
            settings1.CutTails = false;
            settings1.DatFileName = "";
            settings1.Deconvolution = true;
            settings1.DispThres = "2";
            settings1.ESICorrect = false;
            settings1.ESIInterval = "0.9";
            settings1.MascotScore = "30";
            settings1.MascotScoreFiltering = true;
            settings1.MassError = "10";
            settings1.Matrix = false;
            settings1.MyCentroids = "Exact Centroids";
            settings1.Norm = false;
            settings1.OutFilePath = "";
            settings1.PeptperProt = "2";
            settings1.QuantiAll = false;
            settings1.ReportVector = "";
            settings1.RTErrorMinutes = "5";
            settings1.RTErrorProc = "3";
            settings1.RTOrder = "40";
            settings1.SettingsKey = "";
            settings1.ShapeFilter = true;
            settings1.SynchroCharges = true;
            settings1.TimeCorrect = false;
            settings1.Unique = false;
            settings1.ZeroPValue = "0.05";
            this.checkBox1.Checked = settings1.Deconvolution;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.DataBindings.Add(new System.Windows.Forms.Binding("Checked", settings1, "Deconvolution", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox1.Location = new System.Drawing.Point(9, 25);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(4);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(170, 21);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "Charge Deconvolution";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // DatFileDialog
            // 
            this.DatFileDialog.DefaultExt = "dat";
            this.DatFileDialog.Filter = "Mascot DAT  files (*.dat)|*.dat";
            this.DatFileDialog.RestoreDirectory = true;
            // 
            // RAWFileDialog
            // 
            this.RAWFileDialog.DefaultExt = "mgf";
            this.RAWFileDialog.FileName = "openRawFileDialog";
            this.RAWFileDialog.Filter = "RAW Thermo files (*.raw)|*.raw";
            this.RAWFileDialog.Multiselect = true;
            this.RAWFileDialog.RestoreDirectory = true;
            // 
            // ProgressLabel
            // 
            this.ProgressLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ProgressLabel.Location = new System.Drawing.Point(23, 633);
            this.ProgressLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ProgressLabel.Name = "ProgressLabel";
            this.ProgressLabel.Size = new System.Drawing.Size(751, 16);
            this.ProgressLabel.TabIndex = 18;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            // 
            // RawList
            // 
            this.RawList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RawList.FormattingEnabled = true;
            this.RawList.HorizontalScrollbar = true;
            this.RawList.ItemHeight = 16;
            this.RawList.Location = new System.Drawing.Point(0, 0);
            this.RawList.Margin = new System.Windows.Forms.Padding(4);
            this.RawList.Name = "RawList";
            this.RawList.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.RawList.Size = new System.Drawing.Size(735, 148);
            this.RawList.TabIndex = 20;
            // 
            // OutPathButton
            // 
            this.OutPathButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OutPathButton.Location = new System.Drawing.Point(705, 332);
            this.OutPathButton.Margin = new System.Windows.Forms.Padding(4);
            this.OutPathButton.Name = "OutPathButton";
            this.OutPathButton.Size = new System.Drawing.Size(68, 25);
            this.OutPathButton.TabIndex = 3;
            this.OutPathButton.Text = "...";
            this.OutPathButton.UseVisualStyleBackColor = true;
            this.OutPathButton.Click += new System.EventHandler(this.OutPathButton_Click);
            // 
            // AddRawButton
            // 
            this.AddRawButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AddRawButton.Location = new System.Drawing.Point(4, 5);
            this.AddRawButton.Margin = new System.Windows.Forms.Padding(4);
            this.AddRawButton.Name = "AddRawButton";
            this.AddRawButton.Size = new System.Drawing.Size(109, 28);
            this.AddRawButton.TabIndex = 21;
            this.AddRawButton.Text = "Add...";
            this.AddRawButton.UseVisualStyleBackColor = true;
            this.AddRawButton.Click += new System.EventHandler(this.AddRawButton_Click);
            // 
            // DeleteRawButton
            // 
            this.DeleteRawButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.DeleteRawButton.Location = new System.Drawing.Point(626, 5);
            this.DeleteRawButton.Margin = new System.Windows.Forms.Padding(4);
            this.DeleteRawButton.Name = "DeleteRawButton";
            this.DeleteRawButton.Size = new System.Drawing.Size(109, 28);
            this.DeleteRawButton.TabIndex = 22;
            this.DeleteRawButton.Text = "Delete";
            this.DeleteRawButton.UseVisualStyleBackColor = true;
            this.DeleteRawButton.Click += new System.EventHandler(this.DeleteRawButton_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(23, 95);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(751, 230);
            this.tabControl1.TabIndex = 23;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.panel3);
            this.tabPage1.Controls.Add(this.panel2);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage1.Size = new System.Drawing.Size(743, 201);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Input Raw Files:";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.RawList);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(4, 4);
            this.panel3.Margin = new System.Windows.Forms.Padding(0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(735, 156);
            this.panel3.TabIndex = 24;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.DeleteRawButton);
            this.panel2.Controls.Add(this.AddRawButton);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(4, 160);
            this.panel2.Margin = new System.Windows.Forms.Padding(4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(735, 37);
            this.panel2.TabIndex = 23;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.panel4);
            this.tabPage2.Controls.Add(this.panel1);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage2.Size = new System.Drawing.Size(743, 201);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Processing iLog:";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.LogList);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(4, 4);
            this.panel4.Margin = new System.Windows.Forms.Padding(4);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(735, 156);
            this.panel4.TabIndex = 2;
            // 
            // LogList
            // 
            this.LogList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.TimeColumn,
            this.MessageHeader});
            this.LogList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogList.FullRowSelect = true;
            this.LogList.Location = new System.Drawing.Point(0, 0);
            this.LogList.Margin = new System.Windows.Forms.Padding(4);
            this.LogList.Name = "LogList";
            this.LogList.ShowGroups = false;
            this.LogList.ShowItemToolTips = true;
            this.LogList.Size = new System.Drawing.Size(735, 156);
            this.LogList.TabIndex = 0;
            this.LogList.UseCompatibleStateImageBehavior = false;
            this.LogList.View = System.Windows.Forms.View.Details;
            this.LogList.ClientSizeChanged += new System.EventHandler(this.LogList_ClientSizeChanged);
            // 
            // TimeColumn
            // 
            this.TimeColumn.Text = "Time:";
            this.TimeColumn.Width = 120;
            // 
            // MessageHeader
            // 
            this.MessageHeader.Text = "Message:";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.LogSaveButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(4, 160);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(735, 37);
            this.panel1.TabIndex = 1;
            // 
            // LogSaveButton
            // 
            this.LogSaveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LogSaveButton.Location = new System.Drawing.Point(626, 5);
            this.LogSaveButton.Margin = new System.Windows.Forms.Padding(4);
            this.LogSaveButton.Name = "LogSaveButton";
            this.LogSaveButton.Size = new System.Drawing.Size(109, 28);
            this.LogSaveButton.TabIndex = 0;
            this.LogSaveButton.Text = "Save...";
            this.LogSaveButton.UseVisualStyleBackColor = true;
            this.LogSaveButton.Click += new System.EventHandler(this.LogSaveButton_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.ReportTree);
            this.tabPage3.Location = new System.Drawing.Point(4, 25);
            this.tabPage3.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage3.Size = new System.Drawing.Size(743, 201);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Reporting";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // ReportTree
            // 
            this.ReportTree.CheckBoxes = true;
            this.ReportTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ReportTree.Location = new System.Drawing.Point(4, 4);
            this.ReportTree.Margin = new System.Windows.Forms.Padding(4);
            this.ReportTree.Name = "ReportTree";
            treeNode1.Checked = true;
            treeNode1.Name = "Node26";
            treeNode1.Text = "SEQUENCE - Aminoacid sequence";
            treeNode2.Checked = true;
            treeNode2.Name = "Node4";
            treeNode2.Text = "RT MASCOT - Retention time from Mascot";
            treeNode3.Checked = true;
            treeNode3.Name = "Node5";
            treeNode3.Text = "RT APEX - Retention Time from Quanty";
            treeNode4.Name = "Node6";
            treeNode4.Text = "RT SMD - Dispersion of Ion current over RT";
            treeNode5.Name = "Node7";
            treeNode5.Text = "SHIFT - Shift of Peak shape compaing with reference";
            treeNode6.Name = "Node8";
            treeNode6.Text = "CONV - Maximum convolution of peak shape over reference ";
            treeNode7.Name = "Node9";
            treeNode7.Text = "SMD Ratio - Ratio of Dispersions over reference";
            treeNode8.Name = "Node11";
            treeNode8.Text = "ESI CORRECTION - ESI Correction factor";
            treeNode9.Checked = true;
            treeNode9.Name = "Node12";
            treeNode9.Text = "Integrated Intensity";
            treeNode10.Name = "Node13";
            treeNode10.Text = "REF. INT. - Reference integrated intensity";
            treeNode11.Name = "Node15";
            treeNode11.Text = "Log1 - decimal logarithm of Integrated intensity";
            treeNode12.Name = "Node16";
            treeNode12.Text = "Log2 - decimal logarithm of reference intensity";
            treeNode13.Name = "Node17";
            treeNode13.Text = "HIGHEST INTENSITY - Ion current in most intensive spectrum for this peptide";
            treeNode14.Checked = true;
            treeNode14.Name = "Node18";
            treeNode14.Text = "MZ Mascot - m/z value from peptide sequence";
            treeNode15.Name = "Node19";
            treeNode15.Text = "MZ Quanty - m/z value from spectrum";
            treeNode16.Checked = true;
            treeNode16.Name = "Node20";
            treeNode16.Text = "CHARGE - Ion charge";
            treeNode17.Checked = true;
            treeNode17.Name = "Node0";
            treeNode17.Text = "PI - Isoelectrtic point for peptide";
            treeNode18.Name = "Node0";
            treeNode18.Text = "SPECTRA - Number of spectra ";
            treeNode19.Checked = true;
            treeNode19.Name = "Node21";
            treeNode19.Text = "IPI - Protein Identifier";
            treeNode20.Checked = true;
            treeNode20.Name = "Node22";
            treeNode20.Text = "Mascot Score - Best score by Mascot";
            treeNode21.Checked = true;
            treeNode21.Name = "Node23";
            treeNode21.Text = "Modification Mass - Mass shift due to Post-translational modification";
            treeNode22.Name = "Node24";
            treeNode22.Text = "ISOTOPE RATIO - Observed isotope ratio for 1st and 2nd isotopes";
            treeNode23.Name = "Node25";
            treeNode23.Text = "ISOTOPE RATIO THEOR - Isotope ratio for 1st and 2nd isotopes derived from sequenc" +
                "e";
            treeNode24.Checked = true;
            treeNode24.Name = "Node27";
            treeNode24.Text = "COMMENT";
            treeNode25.Checked = true;
            treeNode25.Name = "Node0";
            treeNode25.Text = "Peptide Report";
            treeNode26.Checked = true;
            treeNode26.Name = "Node28";
            treeNode26.Text = "PROTEIN ID";
            treeNode27.Checked = true;
            treeNode27.Name = "Node29";
            treeNode27.Text = "SCORE - Sum of integrated intensities of all found peptides in this run";
            treeNode28.Name = "Node30";
            treeNode28.Text = "REF. SCORE - Sum of integrated intensities of all found peptides in reference run" +
                "";
            treeNode29.Checked = true;
            treeNode29.Name = "Node31";
            treeNode29.Text = "DESCRIPTION - description of protein from database";
            treeNode30.Checked = true;
            treeNode30.Name = "Node32";
            treeNode30.Text = "PEPTIDES IDENT- Number of peptides identified for this proteins from all runs";
            treeNode31.Checked = true;
            treeNode31.Name = "Node33";
            treeNode31.Text = "PEPTIDE FOUND - Number of peptide found in this run for this peptide";
            treeNode32.Checked = true;
            treeNode32.Name = "Node34";
            treeNode32.Text = "R-SQUARE - R^2 of peptide intensities correlation for peptides of this protein in" +
                " this and referenced runs";
            treeNode33.Name = "Node35";
            treeNode33.Text = "SLOPE - slope of less squares plot for peptide intensities in this and referenced" +
                " runs";
            treeNode34.Checked = true;
            treeNode34.Name = "Node36";
            treeNode34.Text = "MEDIAN - Median ratio of peptide intensities in this and referenced runs";
            treeNode35.Name = "Node37";
            treeNode35.Text = "AVERAGE - Average of ratios of peptide intensities in this and referenced runs";
            treeNode36.Name = "Node38";
            treeNode36.Text = "MIN. CONF. - minimum of confidence interval for peptide ratios for 95% confidence" +
                " probability";
            treeNode37.Name = "Node39";
            treeNode37.Text = "MAX. CONF. - maximum of confidence interval for peptide ratios for 95% confidence" +
                " probability";
            treeNode38.Checked = true;
            treeNode38.Name = "Node40";
            treeNode38.Text = "P-VALUE - probabilyty  to reach such R^2 by chance";
            treeNode39.Checked = true;
            treeNode39.Name = "Node1";
            treeNode39.Text = "Protein Report";
            treeNode40.Checked = true;
            treeNode40.Name = "Node41";
            treeNode40.Text = "SEQUENCE - Aminoacid sequence of peptide";
            treeNode41.Checked = true;
            treeNode41.Name = "Node42";
            treeNode41.Text = "PROTEIN ID - database protein identifier";
            treeNode42.Checked = true;
            treeNode42.Name = "Node43";
            treeNode42.Text = "REF ABUNDANCE - peptide abundance in reference run";
            treeNode43.Checked = true;
            treeNode43.Name = "Node44";
            treeNode43.Text = "MODIF - Modification desctription (if any)";
            treeNode44.Checked = true;
            treeNode44.Name = "Node45";
            treeNode44.Text = "Ratios - relative peptide abundance calculated against reference file peptide abu" +
                "ndance";
            treeNode45.Checked = true;
            treeNode45.Name = "Node2";
            treeNode45.Text = "All runs peptide report";
            treeNode46.Checked = true;
            treeNode46.Name = "Node47";
            treeNode46.Text = "PROTEIN ID -  database protein identifier";
            treeNode47.Checked = true;
            treeNode47.Name = "Node48";
            treeNode47.Text = "REF ABUNDANCE - Sum of integrated intensities of all found peptides in reference " +
                "run";
            treeNode48.Checked = true;
            treeNode48.Name = "Node49";
            treeNode48.Text = "DESCRIPTION  - description of protein from database";
            treeNode49.Checked = true;
            treeNode49.Name = "Node50";
            treeNode49.Text = "PEPTIDES - Number of peptide found in reference run for this peptide";
            treeNode50.Checked = true;
            treeNode50.Name = "Node51";
            treeNode50.Text = "Medians - Median ratio of peptide intensities in this and referenced runs";
            treeNode51.Name = "Node0";
            treeNode51.Text = "Slopes - slope of less squares plot for peptide intensities in this and reference" +
                "d runs";
            treeNode52.Name = "Node1";
            treeNode52.Text = "Averages - Average of ratios of peptide intensities in this and referenced runs";
            treeNode53.Name = "Node2";
            treeNode53.Text = "Min. - - minimum of confidence interval for peptide ratios for 95% confidence pro" +
                "bability";
            treeNode54.Name = "Node3";
            treeNode54.Text = "Max. - maximum of confidence interval for peptide ratios for 95% confidence proba" +
                "bility";
            treeNode55.Name = "Node4";
            treeNode55.Text = "R-Squares -  R^2 of peptide intensities correlation for peptides of this protein " +
                "in this and referenced runs";
            treeNode56.Checked = true;
            treeNode56.Name = "Node3";
            treeNode56.Text = "All runs protein report";
            this.ReportTree.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode25,
            treeNode39,
            treeNode45,
            treeNode56});
            this.ReportTree.Size = new System.Drawing.Size(735, 193);
            this.ReportTree.TabIndex = 0;
            this.ReportTree.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.ReportTree_AfterCheck);
            // 
            // saveLogFileDialog
            // 
            this.saveLogFileDialog.DefaultExt = "log";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.textBox7);
            this.groupBox3.Controls.Add(this.checkBox4);
            this.groupBox3.Controls.Add(this.checkBox3);
            this.groupBox3.Controls.Add(this.checkBox2);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.textBox6);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.RefCombo);
            this.groupBox3.Location = new System.Drawing.Point(21, 511);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox3.Size = new System.Drawing.Size(752, 118);
            this.groupBox3.TabIndex = 25;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Reference file options";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(11, 91);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(168, 17);
            this.label10.TabIndex = 37;
            this.label10.Text = "P-value for protein zeros:";
            // 
            // textBox7
            // 
            this.textBox7.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Quanty.Properties.Settings.Default, "ZeroPValue", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox7.Location = new System.Drawing.Point(184, 87);
            this.textBox7.Margin = new System.Windows.Forms.Padding(4);
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new System.Drawing.Size(49, 22);
            this.textBox7.TabIndex = 36;
            this.textBox7.Text = global::Quanty.Properties.Settings.Default.ZeroPValue;
            this.textBox7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Checked = global::Quanty.Properties.Settings.Default.ShapeFilter;
            this.checkBox4.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox4.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Quanty.Properties.Settings.Default, "ShapeFilter", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox4.Location = new System.Drawing.Point(577, 54);
            this.checkBox4.Margin = new System.Windows.Forms.Padding(4);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(161, 21);
            this.checkBox4.TabIndex = 35;
            this.checkBox4.Text = "Peak Shape Filtering";
            this.checkBox4.UseVisualStyleBackColor = true;
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Checked = global::Quanty.Properties.Settings.Default.Norm;
            this.checkBox3.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Quanty.Properties.Settings.Default, "Norm", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox3.Location = new System.Drawing.Point(280, 58);
            this.checkBox3.Margin = new System.Windows.Forms.Padding(4);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(116, 21);
            this.checkBox3.TabIndex = 34;
            this.checkBox3.Text = "Normalization";
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Checked = global::Quanty.Properties.Settings.Default.CutTails;
            this.checkBox2.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Quanty.Properties.Settings.Default, "CutTails", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox2.Location = new System.Drawing.Point(577, 90);
            this.checkBox2.Margin = new System.Windows.Forms.Padding(4);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(80, 21);
            this.checkBox2.TabIndex = 33;
            this.checkBox2.Text = "Cut tails";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 59);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(150, 17);
            this.label4.TabIndex = 29;
            this.label4.Text = "ESI correction interval:";
            // 
            // textBox6
            // 
            this.textBox6.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Quanty.Properties.Settings.Default, "ESIInterval", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox6.Location = new System.Drawing.Point(184, 55);
            this.textBox6.Margin = new System.Windows.Forms.Padding(4);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(49, 22);
            this.textBox6.TabIndex = 27;
            this.textBox6.Text = global::Quanty.Properties.Settings.Default.ESIInterval;
            this.textBox6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 26);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 17);
            this.label2.TabIndex = 26;
            this.label2.Text = "Reference file:";
            // 
            // RefCombo
            // 
            this.RefCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.RefCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RefCombo.FormattingEnabled = true;
            this.RefCombo.Items.AddRange(new object[] {
            "None"});
            this.RefCombo.Location = new System.Drawing.Point(137, 22);
            this.RefCombo.Margin = new System.Windows.Forms.Padding(4);
            this.RefCombo.Name = "RefCombo";
            this.RefCombo.Size = new System.Drawing.Size(605, 24);
            this.RefCombo.TabIndex = 25;
            this.RefCombo.TextChanged += new System.EventHandler(this.RefCombo_TextChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(19, 58);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(36, 17);
            this.label12.TabIndex = 26;
            this.label12.Text = "db3:";
            // 
            // DBFileName
            // 
            this.DBFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.DBFileName.Location = new System.Drawing.Point(97, 54);
            this.DBFileName.Margin = new System.Windows.Forms.Padding(4);
            this.DBFileName.Name = "DBFileName";
            this.DBFileName.ReadOnly = true;
            this.DBFileName.Size = new System.Drawing.Size(599, 22);
            this.DBFileName.TabIndex = 27;
            // 
            // dbOpenButton
            // 
            this.dbOpenButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.dbOpenButton.Location = new System.Drawing.Point(704, 53);
            this.dbOpenButton.Margin = new System.Windows.Forms.Padding(4);
            this.dbOpenButton.Name = "dbOpenButton";
            this.dbOpenButton.Size = new System.Drawing.Size(68, 25);
            this.dbOpenButton.TabIndex = 28;
            this.dbOpenButton.Text = "...";
            this.dbOpenButton.UseVisualStyleBackColor = true;
            this.dbOpenButton.Click += new System.EventHandler(this.dbOpenButton_Click);
            // 
            // DBFileDialog
            // 
            this.DBFileDialog.DefaultExt = "db3";
            this.DBFileDialog.Filter = "Quanti database files (*.db3)|*.db3";
            this.DBFileDialog.RestoreDirectory = true;
            // 
            // OutPathName
            // 
            this.OutPathName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.OutPathName.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Quanty.Properties.Settings.Default, "OutFilePath", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.OutPathName.Location = new System.Drawing.Point(115, 332);
            this.OutPathName.Margin = new System.Windows.Forms.Padding(4);
            this.OutPathName.Name = "OutPathName";
            this.OutPathName.ReadOnly = true;
            this.OutPathName.Size = new System.Drawing.Size(581, 22);
            this.OutPathName.TabIndex = 4;
            this.OutPathName.Text = global::Quanty.Properties.Settings.Default.OutFilePath;
            // 
            // DatFileName
            // 
            this.DatFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.DatFileName.DataBindings.Add(new System.Windows.Forms.Binding("Text", settings1, "DatFileName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.DatFileName.Location = new System.Drawing.Point(97, 16);
            this.DatFileName.Margin = new System.Windows.Forms.Padding(4);
            this.DatFileName.Name = "DatFileName";
            this.DatFileName.ReadOnly = true;
            this.DatFileName.Size = new System.Drawing.Size(599, 22);
            this.DatFileName.TabIndex = 0;
            this.DatFileName.Text = settings1.DatFileName;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(789, 740);
            this.Controls.Add(this.dbOpenButton);
            this.Controls.Add(this.DBFileName);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.ProgressLabel);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.OutPathButton);
            this.Controls.Add(this.DATFileButton);
            this.Controls.Add(this.OutPathName);
            this.Controls.Add(this.DatFileName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(794, 771);
            this.Name = "Form1";
            this.Text = "Quanty - ver.2.4.4.0";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button DATFileButton;
        private System.Windows.Forms.TextBox OutPathName;
        private System.Windows.Forms.TextBox DatFileName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.OpenFileDialog DatFileDialog;
        private System.Windows.Forms.OpenFileDialog RAWFileDialog;
        private System.Windows.Forms.Label ProgressLabel;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.CheckBox checkBox5;
        private System.Windows.Forms.CheckBox AllCheckBox;
        private System.Windows.Forms.ListBox RawList;
        private System.Windows.Forms.Button OutPathButton;
        private System.Windows.Forms.Button AddRawButton;
        private System.Windows.Forms.Button DeleteRawButton;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button LogSaveButton;
        private System.Windows.Forms.ListView LogList;
        private System.Windows.Forms.ColumnHeader TimeColumn;
        private System.Windows.Forms.ColumnHeader MessageHeader;
        private System.Windows.Forms.SaveFileDialog saveLogFileDialog;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox RefCombo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TreeView ReportTree;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox DBFileName;
        private System.Windows.Forms.Button dbOpenButton;
        private System.Windows.Forms.OpenFileDialog DBFileDialog;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.TextBox textBox7;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBox8;
        private System.Windows.Forms.Label label11;
    }
}

