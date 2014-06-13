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
            this.components = new System.ComponentModel.Container();
            Quanty.Properties.Settings settings1 = new Quanty.Properties.Settings();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("SEQUENCE - Aminoacid sequence");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("RT MASCOT - Retention time from Mascot");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("RT APEX - Retention Time from Quanty");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("RT Order - Average retention order");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("RT SMD - Dispersion of Ion current over RT");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("SHIFT - Shift of Peak shape compaing with reference");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("CONV - Maximum convolution of peak shape over reference ");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("SMD Ratio - Ratio of Dispersions over reference");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("ESI CORRECTION - ESI Correction factor");
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Integrated Intensity");
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("REF. INT. - Reference integrated intensity");
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("Log1 - decimal logarithm of Integrated intensity");
            System.Windows.Forms.TreeNode treeNode13 = new System.Windows.Forms.TreeNode("Log2 - decimal logarithm of reference intensity");
            System.Windows.Forms.TreeNode treeNode14 = new System.Windows.Forms.TreeNode("HIGHEST INTENSITY - Ion current in most intensive spectrum for this peptide");
            System.Windows.Forms.TreeNode treeNode15 = new System.Windows.Forms.TreeNode("MZ Mascot - m/z value from peptide sequence");
            System.Windows.Forms.TreeNode treeNode16 = new System.Windows.Forms.TreeNode("MZ Quanty - m/z value from spectrum");
            System.Windows.Forms.TreeNode treeNode17 = new System.Windows.Forms.TreeNode("CHARGE - Ion charge");
            System.Windows.Forms.TreeNode treeNode18 = new System.Windows.Forms.TreeNode("PI - Isoelectrtic point for peptide");
            System.Windows.Forms.TreeNode treeNode19 = new System.Windows.Forms.TreeNode("ELEMENTS - Elemental composition of peptide");
            System.Windows.Forms.TreeNode treeNode20 = new System.Windows.Forms.TreeNode("SPECTRA - Number of spectra ");
            System.Windows.Forms.TreeNode treeNode21 = new System.Windows.Forms.TreeNode("IPI - Protein Identifier");
            System.Windows.Forms.TreeNode treeNode22 = new System.Windows.Forms.TreeNode("IPI Desc - Protein Description");
            System.Windows.Forms.TreeNode treeNode23 = new System.Windows.Forms.TreeNode("IPIs - List of protein IDs where peptide can be found");
            System.Windows.Forms.TreeNode treeNode24 = new System.Windows.Forms.TreeNode("Mascot Score - Best score by Mascot");
            System.Windows.Forms.TreeNode treeNode25 = new System.Windows.Forms.TreeNode("Modification Mass - Mass shift due to Post-translational modification");
            System.Windows.Forms.TreeNode treeNode26 = new System.Windows.Forms.TreeNode("ISOTOPE RATIO - Observed isotope ratio for 1st and 2nd isotopes");
            System.Windows.Forms.TreeNode treeNode27 = new System.Windows.Forms.TreeNode("ISOTOPE RATIO THEOR - Isotope ratio for 1st and 2nd isotopes derived from sequenc" +
        "e");
            System.Windows.Forms.TreeNode treeNode28 = new System.Windows.Forms.TreeNode("MOD DECS. - Post-translational modification description");
            System.Windows.Forms.TreeNode treeNode29 = new System.Windows.Forms.TreeNode("CASE - peptide assignment quality description (for .all reports)");
            System.Windows.Forms.TreeNode treeNode30 = new System.Windows.Forms.TreeNode("COMMENT - No peak error and so on");
            System.Windows.Forms.TreeNode treeNode31 = new System.Windows.Forms.TreeNode("Peptide Report", new System.Windows.Forms.TreeNode[] {
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
            treeNode24,
            treeNode25,
            treeNode26,
            treeNode27,
            treeNode28,
            treeNode29,
            treeNode30});
            System.Windows.Forms.TreeNode treeNode32 = new System.Windows.Forms.TreeNode("PROTEIN ID");
            System.Windows.Forms.TreeNode treeNode33 = new System.Windows.Forms.TreeNode("SCORE - Sum of integrated intensities of all found peptides in this run");
            System.Windows.Forms.TreeNode treeNode34 = new System.Windows.Forms.TreeNode("REF. SCORE - Sum of integrated intensities of all found peptides in reference run" +
        "");
            System.Windows.Forms.TreeNode treeNode35 = new System.Windows.Forms.TreeNode("DESCRIPTION - description of protein from database");
            System.Windows.Forms.TreeNode treeNode36 = new System.Windows.Forms.TreeNode("IPIs - List of protein IDs which share peptides with this one");
            System.Windows.Forms.TreeNode treeNode37 = new System.Windows.Forms.TreeNode("PEPTIDES IDENT- Number of peptides identified for this proteins from all runs");
            System.Windows.Forms.TreeNode treeNode38 = new System.Windows.Forms.TreeNode("PEPTIDE FOUND - Number of peptide found in this run for this peptide");
            System.Windows.Forms.TreeNode treeNode39 = new System.Windows.Forms.TreeNode("R-SQUARE - R^2 of peptide intensities correlation for peptides of this protein in" +
        " this and referenced runs");
            System.Windows.Forms.TreeNode treeNode40 = new System.Windows.Forms.TreeNode("SLOPE - slope of less squares plot for peptide intensities in this and referenced" +
        " runs");
            System.Windows.Forms.TreeNode treeNode41 = new System.Windows.Forms.TreeNode("MEDIAN - Median ratio of peptide intensities in this and referenced runs");
            System.Windows.Forms.TreeNode treeNode42 = new System.Windows.Forms.TreeNode("AVERAGE - Average of ratios of peptide intensities in this and referenced runs");
            System.Windows.Forms.TreeNode treeNode43 = new System.Windows.Forms.TreeNode("MIN. CONF. - minimum of confidence interval for peptide ratios for 95% confidence" +
        " probability");
            System.Windows.Forms.TreeNode treeNode44 = new System.Windows.Forms.TreeNode("MAX. CONF. - maximum of confidence interval for peptide ratios for 95% confidence" +
        " probability");
            System.Windows.Forms.TreeNode treeNode45 = new System.Windows.Forms.TreeNode("P-VALUE - probabilyty  to reach such R^2 by chance");
            System.Windows.Forms.TreeNode treeNode46 = new System.Windows.Forms.TreeNode("Protein Report", new System.Windows.Forms.TreeNode[] {
            treeNode32,
            treeNode33,
            treeNode34,
            treeNode35,
            treeNode36,
            treeNode37,
            treeNode38,
            treeNode39,
            treeNode40,
            treeNode41,
            treeNode42,
            treeNode43,
            treeNode44,
            treeNode45});
            System.Windows.Forms.TreeNode treeNode47 = new System.Windows.Forms.TreeNode("SEQUENCE - Aminoacid sequence of peptide");
            System.Windows.Forms.TreeNode treeNode48 = new System.Windows.Forms.TreeNode("PROTEIN ID - database protein identifier");
            System.Windows.Forms.TreeNode treeNode49 = new System.Windows.Forms.TreeNode("PROTEIN DESC. - Protein Description");
            System.Windows.Forms.TreeNode treeNode50 = new System.Windows.Forms.TreeNode("IPIs - List of protein IDs where peptide can be found");
            System.Windows.Forms.TreeNode treeNode51 = new System.Windows.Forms.TreeNode("REF ABUNDANCE - peptide abundance in reference run");
            System.Windows.Forms.TreeNode treeNode52 = new System.Windows.Forms.TreeNode("MODIF - Modification desctription (if any)");
            System.Windows.Forms.TreeNode treeNode53 = new System.Windows.Forms.TreeNode("MZ - Theoretical m/z of peptide");
            System.Windows.Forms.TreeNode treeNode54 = new System.Windows.Forms.TreeNode("CHARGE - main charge state of peptide");
            System.Windows.Forms.TreeNode treeNode55 = new System.Windows.Forms.TreeNode("MASCOT SCORE - Best score by Mascot");
            System.Windows.Forms.TreeNode treeNode56 = new System.Windows.Forms.TreeNode("PI - Isoelectric point for peptide");
            System.Windows.Forms.TreeNode treeNode57 = new System.Windows.Forms.TreeNode("MODIF MASS - Mass shift due to Post-translational modification");
            System.Windows.Forms.TreeNode treeNode58 = new System.Windows.Forms.TreeNode("ELEMENTS - Elemental composition of peptide");
            System.Windows.Forms.TreeNode treeNode59 = new System.Windows.Forms.TreeNode("CASE - peptide assignment quality description (for .all reports)");
            System.Windows.Forms.TreeNode treeNode60 = new System.Windows.Forms.TreeNode("Ratios - relative peptide abundance calculated against reference file peptide abu" +
        "ndance");
            System.Windows.Forms.TreeNode treeNode61 = new System.Windows.Forms.TreeNode("RT Apex - RT peak apex for individual samples");
            System.Windows.Forms.TreeNode treeNode62 = new System.Windows.Forms.TreeNode("MZ Quanti - Measured MZ for each sample ");
            System.Windows.Forms.TreeNode treeNode63 = new System.Windows.Forms.TreeNode("RAW Values - Raw peptide abundances in thermo units");
            System.Windows.Forms.TreeNode treeNode64 = new System.Windows.Forms.TreeNode("All runs peptide report", new System.Windows.Forms.TreeNode[] {
            treeNode47,
            treeNode48,
            treeNode49,
            treeNode50,
            treeNode51,
            treeNode52,
            treeNode53,
            treeNode54,
            treeNode55,
            treeNode56,
            treeNode57,
            treeNode58,
            treeNode59,
            treeNode60,
            treeNode61,
            treeNode62,
            treeNode63});
            System.Windows.Forms.TreeNode treeNode65 = new System.Windows.Forms.TreeNode("PROTEIN ID -  database protein identifier");
            System.Windows.Forms.TreeNode treeNode66 = new System.Windows.Forms.TreeNode("REF ABUNDANCE - Sum of integrated intensities of all found peptides in reference " +
        "run");
            System.Windows.Forms.TreeNode treeNode67 = new System.Windows.Forms.TreeNode("DESCRIPTION  - description of protein from database");
            System.Windows.Forms.TreeNode treeNode68 = new System.Windows.Forms.TreeNode("IPIs - List of protein IDs which share peptides with this one");
            System.Windows.Forms.TreeNode treeNode69 = new System.Windows.Forms.TreeNode("PEPTIDES - Number of peptide found in reference run for this peptide");
            System.Windows.Forms.TreeNode treeNode70 = new System.Windows.Forms.TreeNode("Medians - Median ratio of peptide intensities in this and referenced runs");
            System.Windows.Forms.TreeNode treeNode71 = new System.Windows.Forms.TreeNode("Slopes - slope of less squares plot for peptide intensities in this and reference" +
        "d runs");
            System.Windows.Forms.TreeNode treeNode72 = new System.Windows.Forms.TreeNode("Averages - Average of ratios of peptide intensities in this and referenced runs");
            System.Windows.Forms.TreeNode treeNode73 = new System.Windows.Forms.TreeNode("Min. - minimum of confidence interval for peptide ratios for 95% confidence proba" +
        "bility");
            System.Windows.Forms.TreeNode treeNode74 = new System.Windows.Forms.TreeNode("Max. - maximum of confidence interval for peptide ratios for 95% confidence proba" +
        "bility");
            System.Windows.Forms.TreeNode treeNode75 = new System.Windows.Forms.TreeNode("R-Squares -  R^2 of peptide intensities correlation for peptides of this protein " +
        "in this and referenced runs");
            System.Windows.Forms.TreeNode treeNode76 = new System.Windows.Forms.TreeNode("Matrix Min/Max - Abundance confidence interval for matrices.");
            System.Windows.Forms.TreeNode treeNode77 = new System.Windows.Forms.TreeNode("PPMs Matrix - Protein abundances in ppms");
            System.Windows.Forms.TreeNode treeNode78 = new System.Windows.Forms.TreeNode("RAW Values - Sum of raw peptide abundancxes in Thermo units");
            System.Windows.Forms.TreeNode treeNode79 = new System.Windows.Forms.TreeNode("All runs protein report", new System.Windows.Forms.TreeNode[] {
            treeNode65,
            treeNode66,
            treeNode67,
            treeNode68,
            treeNode69,
            treeNode70,
            treeNode71,
            treeNode72,
            treeNode73,
            treeNode74,
            treeNode75,
            treeNode76,
            treeNode77,
            treeNode78});
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
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
            this.UseFDRCheck = new System.Windows.Forms.CheckBox();
            this.AllCheckBox = new System.Windows.Forms.CheckBox();
            this.checkBox5 = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.StickCheckBox = new System.Windows.Forms.CheckBox();
            this.MetaCheckBox = new System.Windows.Forms.CheckBox();
            this.DatFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.RAWFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.ProgressLabel = new System.Windows.Forms.Label();
            this.OutPathButton = new System.Windows.Forms.Button();
            this.AddRawButton = new System.Windows.Forms.Button();
            this.DeleteRawButton = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panel3 = new System.Windows.Forms.Panel();
            this.RawList = new System.Windows.Forms.ListView();
            this.FileName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Message = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel2 = new System.Windows.Forms.Panel();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.panel4 = new System.Windows.Forms.Panel();
            this.LogList = new System.Windows.Forms.ListView();
            this.TimeColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.MessageHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel1 = new System.Windows.Forms.Panel();
            this.LogSaveButton = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.ReportTree = new System.Windows.Forms.TreeView();
            this.saveLogFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.MaxZeroBox = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.ZeroSubstBox = new System.Windows.Forms.ComboBox();
            this.UseAllCheck = new System.Windows.Forms.CheckBox();
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
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.textBox11 = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.textBox10 = new System.Windows.Forms.TextBox();
            this.textBox9 = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
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
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // DATFileButton
            // 
            this.DATFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DATFileButton.Location = new System.Drawing.Point(819, 15);
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
            this.progressBar1.Size = new System.Drawing.Size(865, 28);
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
            this.groupBox1.Size = new System.Drawing.Size(364, 139);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Filter options";
            // 
            // textBox8
            // 
            this.textBox8.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Quanty.Properties.Settings.Default, "RTOrder", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox8.Location = new System.Drawing.Point(299, 94);
            this.textBox8.Margin = new System.Windows.Forms.Padding(4);
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new System.Drawing.Size(41, 22);
            this.textBox8.TabIndex = 11;
            this.textBox8.Text = global::Quanty.Properties.Settings.Default.RTOrder;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(207, 97);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(84, 17);
            this.label11.TabIndex = 10;
            this.label11.Text = "RT Order ±:";
            // 
            // textBox5
            // 
            this.textBox5.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Quanty.Properties.Settings.Default, "PeptperProt", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox5.Location = new System.Drawing.Point(153, 95);
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
            this.textBox4.Location = new System.Drawing.Point(299, 57);
            this.textBox4.Margin = new System.Windows.Forms.Padding(4);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(41, 22);
            this.textBox4.TabIndex = 7;
            this.textBox4.Text = global::Quanty.Properties.Settings.Default.RTErrorMinutes;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(208, 60);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(69, 17);
            this.label8.TabIndex = 6;
            this.label8.Text = "RT ± min:";
            // 
            // textBox3
            // 
            this.textBox3.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Quanty.Properties.Settings.Default, "RTErrorProc", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox3.Location = new System.Drawing.Point(299, 21);
            this.textBox3.Margin = new System.Windows.Forms.Padding(4);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(41, 22);
            this.textBox3.TabIndex = 5;
            this.textBox3.Text = global::Quanty.Properties.Settings.Default.RTErrorProc;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(208, 26);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 17);
            this.label7.TabIndex = 4;
            this.label7.Text = "RT ± %:";
            // 
            // textBox2
            // 
            this.textBox2.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Quanty.Properties.Settings.Default, "MassError", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox2.Location = new System.Drawing.Point(153, 58);
            this.textBox2.Margin = new System.Windows.Forms.Padding(4);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(41, 22);
            this.textBox2.TabIndex = 3;
            this.textBox2.Text = global::Quanty.Properties.Settings.Default.MassError;
            // 
            // textBox1
            // 
            this.textBox1.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Quanty.Properties.Settings.Default, "MascotScore", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox1.Location = new System.Drawing.Point(153, 21);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(41, 22);
            this.textBox1.TabIndex = 2;
            this.textBox1.Text = global::Quanty.Properties.Settings.Default.MascotScore;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
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
            this.groupBox2.Controls.Add(this.UseFDRCheck);
            this.groupBox2.Controls.Add(this.AllCheckBox);
            this.groupBox2.Controls.Add(this.checkBox5);
            this.groupBox2.Controls.Add(this.checkBox1);
            this.groupBox2.Location = new System.Drawing.Point(395, 368);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox2.Size = new System.Drawing.Size(233, 139);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Misc. Options";
            // 
            // UseFDRCheck
            // 
            this.UseFDRCheck.AutoSize = true;
            this.UseFDRCheck.Enabled = false;
            this.UseFDRCheck.Location = new System.Drawing.Point(8, 29);
            this.UseFDRCheck.Name = "UseFDRCheck";
            this.UseFDRCheck.Size = new System.Drawing.Size(176, 21);
            this.UseFDRCheck.TabIndex = 11;
            this.UseFDRCheck.Text = "Use FDR mascot thres.";
            this.UseFDRCheck.UseVisualStyleBackColor = true;
            this.UseFDRCheck.CheckedChanged += new System.EventHandler(this.UseFDRCheck_CheckedChanged);
            // 
            // AllCheckBox
            // 
            this.AllCheckBox.AutoSize = true;
            this.AllCheckBox.Checked = global::Quanty.Properties.Settings.Default.QuantiAll;
            this.AllCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Quanty.Properties.Settings.Default, "QuantiAll", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.AllCheckBox.Location = new System.Drawing.Point(8, 112);
            this.AllCheckBox.Margin = new System.Windows.Forms.Padding(4);
            this.AllCheckBox.Name = "AllCheckBox";
            this.AllCheckBox.Size = new System.Drawing.Size(206, 21);
            this.AllCheckBox.TabIndex = 10;
            this.AllCheckBox.Text = "Quantify all entries (.all file) ";
            this.AllCheckBox.UseVisualStyleBackColor = true;
            this.AllCheckBox.CheckedChanged += new System.EventHandler(this.AllCheckBox_CheckedChanged);
            // 
            // checkBox5
            // 
            this.checkBox5.AutoSize = true;
            this.checkBox5.Checked = global::Quanty.Properties.Settings.Default.MascotScoreFiltering;
            this.checkBox5.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox5.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Quanty.Properties.Settings.Default, "MascotScoreFiltering", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox5.Location = new System.Drawing.Point(8, 84);
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
            settings1.CaptionText = "Quanty - ver.2.5.4.4";
            settings1.ConvThres = "0.6";
            settings1.CutTails = false;
            settings1.DatFileName = "";
            settings1.DBFileName = "";
            settings1.Deconvolution = true;
            settings1.DispThres = "2";
            settings1.ESICorrect = false;
            settings1.ESIInterval = "1.0";
            settings1.MascotScore = "30";
            settings1.MascotScoreFiltering = true;
            settings1.MassError = "10";
            settings1.Matrix = false;
            settings1.MaxRTWidth = "0.0";
            settings1.MaxZeroes = "";
            settings1.MetaProfile = false;
            settings1.MinRTWidht = "0.0";
            settings1.MyCentroids = "Exact Centroids";
            settings1.Norm = false;
            settings1.OutFilePath = "";
            settings1.PeptperProt = "2";
            settings1.QuantiAll = false;
            settings1.Reference = "";
            settings1.ReportVector = "";
            settings1.RTErrorMinutes = "5";
            settings1.RTErrorProc = "3";
            settings1.RTOrder = "40";
            settings1.RTRes = "0";
            settings1.SettingsKey = "";
            settings1.ShapeFilter = true;
            settings1.StickMode = false;
            settings1.SynchroCharges = true;
            settings1.TimeCorrect = false;
            settings1.Unique = false;
            settings1.UseAll = false;
            settings1.ZeroPValue = "0.05";
            settings1.ZeroSubst = "";
            this.checkBox1.Checked = settings1.Deconvolution;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.DataBindings.Add(new System.Windows.Forms.Binding("Checked", settings1, "Deconvolution", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox1.Location = new System.Drawing.Point(8, 56);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(4);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(170, 21);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "Charge Deconvolution";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // StickCheckBox
            // 
            this.StickCheckBox.AutoSize = true;
            this.StickCheckBox.Checked = global::Quanty.Properties.Settings.Default.StickMode;
            this.StickCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Quanty.Properties.Settings.Default, "StickMode", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.StickCheckBox.Location = new System.Drawing.Point(6, 56);
            this.StickCheckBox.Name = "StickCheckBox";
            this.StickCheckBox.Size = new System.Drawing.Size(137, 21);
            this.StickCheckBox.TabIndex = 13;
            this.StickCheckBox.Text = "Force stick mode";
            this.StickCheckBox.UseVisualStyleBackColor = true;
            // 
            // MetaCheckBox
            // 
            this.MetaCheckBox.AutoSize = true;
            this.MetaCheckBox.Checked = global::Quanty.Properties.Settings.Default.MetaProfile;
            this.MetaCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Quanty.Properties.Settings.Default, "MetaProfile", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.MetaCheckBox.Location = new System.Drawing.Point(6, 25);
            this.MetaCheckBox.Name = "MetaCheckBox";
            this.MetaCheckBox.Size = new System.Drawing.Size(163, 21);
            this.MetaCheckBox.TabIndex = 12;
            this.MetaCheckBox.Text = "Metabolomic profiling";
            this.MetaCheckBox.UseVisualStyleBackColor = true;
            // 
            // DatFileDialog
            // 
            this.DatFileDialog.DefaultExt = "dat";
            this.DatFileDialog.Filter = "Mascot DAT  files (*.dat)|*.dat|Tab-delimited text files (*.txt;*.pept)|*.txt;*.p" +
    "ept";
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
            this.ProgressLabel.Size = new System.Drawing.Size(865, 16);
            this.ProgressLabel.TabIndex = 18;
            // 
            // OutPathButton
            // 
            this.OutPathButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OutPathButton.Location = new System.Drawing.Point(819, 332);
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
            this.DeleteRawButton.Location = new System.Drawing.Point(740, 5);
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
            this.tabControl1.Size = new System.Drawing.Size(865, 230);
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
            this.tabPage1.Size = new System.Drawing.Size(857, 201);
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
            this.panel3.Size = new System.Drawing.Size(849, 156);
            this.panel3.TabIndex = 24;
            // 
            // RawList
            // 
            this.RawList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.FileName,
            this.Message});
            this.RawList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RawList.Location = new System.Drawing.Point(0, 0);
            this.RawList.Name = "RawList";
            this.RawList.ShowItemToolTips = true;
            this.RawList.Size = new System.Drawing.Size(849, 156);
            this.RawList.TabIndex = 0;
            this.RawList.UseCompatibleStateImageBehavior = false;
            this.RawList.View = System.Windows.Forms.View.Details;
            this.RawList.ClientSizeChanged += new System.EventHandler(this.RawList_ClientSizeChanged);
            // 
            // FileName
            // 
            this.FileName.Text = "Input Raw Files:";
            this.FileName.Width = 500;
            // 
            // Message
            // 
            this.Message.Text = "Message:";
            this.Message.Width = 150;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.DeleteRawButton);
            this.panel2.Controls.Add(this.AddRawButton);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(4, 160);
            this.panel2.Margin = new System.Windows.Forms.Padding(4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(849, 37);
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
            this.tabPage2.Size = new System.Drawing.Size(857, 201);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Processing Log:";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.LogList);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(4, 4);
            this.panel4.Margin = new System.Windows.Forms.Padding(4);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(849, 156);
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
            this.LogList.Size = new System.Drawing.Size(849, 156);
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
            this.panel1.Size = new System.Drawing.Size(849, 37);
            this.panel1.TabIndex = 1;
            // 
            // LogSaveButton
            // 
            this.LogSaveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LogSaveButton.Location = new System.Drawing.Point(716, 5);
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
            this.tabPage3.Size = new System.Drawing.Size(857, 201);
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
            treeNode4.Name = "Node0";
            treeNode4.Text = "RT Order - Average retention order";
            treeNode5.Name = "Node6";
            treeNode5.Text = "RT SMD - Dispersion of Ion current over RT";
            treeNode6.Name = "Node7";
            treeNode6.Text = "SHIFT - Shift of Peak shape compaing with reference";
            treeNode7.Name = "Node8";
            treeNode7.Text = "CONV - Maximum convolution of peak shape over reference ";
            treeNode8.Name = "Node9";
            treeNode8.Text = "SMD Ratio - Ratio of Dispersions over reference";
            treeNode9.Name = "Node11";
            treeNode9.Text = "ESI CORRECTION - ESI Correction factor";
            treeNode10.Checked = true;
            treeNode10.Name = "Node12";
            treeNode10.Text = "Integrated Intensity";
            treeNode11.Name = "Node13";
            treeNode11.Text = "REF. INT. - Reference integrated intensity";
            treeNode12.Name = "Node15";
            treeNode12.Text = "Log1 - decimal logarithm of Integrated intensity";
            treeNode13.Name = "Node16";
            treeNode13.Text = "Log2 - decimal logarithm of reference intensity";
            treeNode14.Name = "Node17";
            treeNode14.Text = "HIGHEST INTENSITY - Ion current in most intensive spectrum for this peptide";
            treeNode15.Checked = true;
            treeNode15.Name = "Node18";
            treeNode15.Text = "MZ Mascot - m/z value from peptide sequence";
            treeNode16.Name = "Node19";
            treeNode16.Text = "MZ Quanty - m/z value from spectrum";
            treeNode17.Checked = true;
            treeNode17.Name = "Node20";
            treeNode17.Text = "CHARGE - Ion charge";
            treeNode18.Checked = true;
            treeNode18.Name = "Node0";
            treeNode18.Text = "PI - Isoelectrtic point for peptide";
            treeNode19.Name = "Node0";
            treeNode19.Text = "ELEMENTS - Elemental composition of peptide";
            treeNode20.Name = "Node0";
            treeNode20.Text = "SPECTRA - Number of spectra ";
            treeNode21.Checked = true;
            treeNode21.Name = "Node21";
            treeNode21.Text = "IPI - Protein Identifier";
            treeNode22.Checked = true;
            treeNode22.Name = "Node2";
            treeNode22.Text = "IPI Desc - Protein Description";
            treeNode23.Name = "Node0";
            treeNode23.Text = "IPIs - List of protein IDs where peptide can be found";
            treeNode24.Checked = true;
            treeNode24.Name = "Node22";
            treeNode24.Text = "Mascot Score - Best score by Mascot";
            treeNode25.Checked = true;
            treeNode25.Name = "Node23";
            treeNode25.Text = "Modification Mass - Mass shift due to Post-translational modification";
            treeNode26.Name = "Node24";
            treeNode26.Text = "ISOTOPE RATIO - Observed isotope ratio for 1st and 2nd isotopes";
            treeNode27.Name = "Node25";
            treeNode27.Text = "ISOTOPE RATIO THEOR - Isotope ratio for 1st and 2nd isotopes derived from sequenc" +
    "e";
            treeNode28.Checked = true;
            treeNode28.Name = "Node0";
            treeNode28.Text = "MOD DECS. - Post-translational modification description";
            treeNode29.Name = "Node1";
            treeNode29.Text = "CASE - peptide assignment quality description (for .all reports)";
            treeNode30.Name = "Node27";
            treeNode30.Text = "COMMENT - No peak error and so on";
            treeNode31.Checked = true;
            treeNode31.Name = "Node0";
            treeNode31.Text = "Peptide Report";
            treeNode32.Checked = true;
            treeNode32.Name = "Node28";
            treeNode32.Text = "PROTEIN ID";
            treeNode33.Checked = true;
            treeNode33.Name = "Node29";
            treeNode33.Text = "SCORE - Sum of integrated intensities of all found peptides in this run";
            treeNode34.Checked = true;
            treeNode34.Name = "Node30";
            treeNode34.Text = "REF. SCORE - Sum of integrated intensities of all found peptides in reference run" +
    "";
            treeNode35.Checked = true;
            treeNode35.Name = "Node31";
            treeNode35.Text = "DESCRIPTION - description of protein from database";
            treeNode36.Checked = true;
            treeNode36.Name = "Node1";
            treeNode36.Text = "IPIs - List of protein IDs which share peptides with this one";
            treeNode37.Checked = true;
            treeNode37.Name = "Node32";
            treeNode37.Text = "PEPTIDES IDENT- Number of peptides identified for this proteins from all runs";
            treeNode38.Checked = true;
            treeNode38.Name = "Node33";
            treeNode38.Text = "PEPTIDE FOUND - Number of peptide found in this run for this peptide";
            treeNode39.Checked = true;
            treeNode39.Name = "Node34";
            treeNode39.Text = "R-SQUARE - R^2 of peptide intensities correlation for peptides of this protein in" +
    " this and referenced runs";
            treeNode40.Checked = true;
            treeNode40.Name = "Node35";
            treeNode40.Text = "SLOPE - slope of less squares plot for peptide intensities in this and referenced" +
    " runs";
            treeNode41.Checked = true;
            treeNode41.Name = "Node36";
            treeNode41.Text = "MEDIAN - Median ratio of peptide intensities in this and referenced runs";
            treeNode42.Checked = true;
            treeNode42.Name = "Node37";
            treeNode42.Text = "AVERAGE - Average of ratios of peptide intensities in this and referenced runs";
            treeNode43.Name = "Node38";
            treeNode43.Text = "MIN. CONF. - minimum of confidence interval for peptide ratios for 95% confidence" +
    " probability";
            treeNode44.Name = "Node39";
            treeNode44.Text = "MAX. CONF. - maximum of confidence interval for peptide ratios for 95% confidence" +
    " probability";
            treeNode45.Checked = true;
            treeNode45.Name = "Node40";
            treeNode45.Text = "P-VALUE - probabilyty  to reach such R^2 by chance";
            treeNode46.Checked = true;
            treeNode46.Name = "Node1";
            treeNode46.Text = "Protein Report";
            treeNode47.Checked = true;
            treeNode47.Name = "Node41";
            treeNode47.Text = "SEQUENCE - Aminoacid sequence of peptide";
            treeNode48.Checked = true;
            treeNode48.Name = "Node42";
            treeNode48.Text = "PROTEIN ID - database protein identifier";
            treeNode49.Checked = true;
            treeNode49.Name = "Node3";
            treeNode49.Text = "PROTEIN DESC. - Protein Description";
            treeNode50.Checked = true;
            treeNode50.Name = "Node2";
            treeNode50.Text = "IPIs - List of protein IDs where peptide can be found";
            treeNode51.Checked = true;
            treeNode51.Name = "Node43";
            treeNode51.Text = "REF ABUNDANCE - peptide abundance in reference run";
            treeNode52.Checked = true;
            treeNode52.Name = "Node44";
            treeNode52.Text = "MODIF - Modification desctription (if any)";
            treeNode53.Name = "Node1";
            treeNode53.Text = "MZ - Theoretical m/z of peptide";
            treeNode54.Name = "Node2";
            treeNode54.Text = "CHARGE - main charge state of peptide";
            treeNode55.Name = "Node3";
            treeNode55.Text = "MASCOT SCORE - Best score by Mascot";
            treeNode56.Name = "Node4";
            treeNode56.Text = "PI - Isoelectric point for peptide";
            treeNode57.Name = "Node5";
            treeNode57.Text = "MODIF MASS - Mass shift due to Post-translational modification";
            treeNode58.Name = "Node6";
            treeNode58.Text = "ELEMENTS - Elemental composition of peptide";
            treeNode59.Name = "Node2";
            treeNode59.Text = "CASE - peptide assignment quality description (for .all reports)";
            treeNode60.Checked = true;
            treeNode60.Name = "Node45";
            treeNode60.Text = "Ratios - relative peptide abundance calculated against reference file peptide abu" +
    "ndance";
            treeNode61.Name = "Node0";
            treeNode61.Text = "RT Apex - RT peak apex for individual samples";
            treeNode62.Name = "Node1";
            treeNode62.Text = "MZ Quanti - Measured MZ for each sample ";
            treeNode63.Name = "Node0";
            treeNode63.Text = "RAW Values - Raw peptide abundances in thermo units";
            treeNode64.Name = "Node2";
            treeNode64.Text = "All runs peptide report";
            treeNode65.Checked = true;
            treeNode65.Name = "Node47";
            treeNode65.Text = "PROTEIN ID -  database protein identifier";
            treeNode66.Checked = true;
            treeNode66.Name = "Node48";
            treeNode66.Text = "REF ABUNDANCE - Sum of integrated intensities of all found peptides in reference " +
    "run";
            treeNode67.Checked = true;
            treeNode67.Name = "Node49";
            treeNode67.Text = "DESCRIPTION  - description of protein from database";
            treeNode68.Checked = true;
            treeNode68.Name = "Node3";
            treeNode68.Text = "IPIs - List of protein IDs which share peptides with this one";
            treeNode69.Checked = true;
            treeNode69.Name = "Node50";
            treeNode69.Text = "PEPTIDES - Number of peptide found in reference run for this peptide";
            treeNode70.Checked = true;
            treeNode70.Name = "Node51";
            treeNode70.Text = "Medians - Median ratio of peptide intensities in this and referenced runs";
            treeNode71.Name = "Node0";
            treeNode71.Text = "Slopes - slope of less squares plot for peptide intensities in this and reference" +
    "d runs";
            treeNode72.Name = "Node1";
            treeNode72.Text = "Averages - Average of ratios of peptide intensities in this and referenced runs";
            treeNode73.Name = "Node2";
            treeNode73.Text = "Min. - minimum of confidence interval for peptide ratios for 95% confidence proba" +
    "bility";
            treeNode74.Name = "Node3";
            treeNode74.Text = "Max. - maximum of confidence interval for peptide ratios for 95% confidence proba" +
    "bility";
            treeNode75.Name = "Node4";
            treeNode75.Text = "R-Squares -  R^2 of peptide intensities correlation for peptides of this protein " +
    "in this and referenced runs";
            treeNode76.Name = "Node0";
            treeNode76.Text = "Matrix Min/Max - Abundance confidence interval for matrices.";
            treeNode77.Name = "Node1";
            treeNode77.Text = "PPMs Matrix - Protein abundances in ppms";
            treeNode78.Name = "Node1";
            treeNode78.Text = "RAW Values - Sum of raw peptide abundancxes in Thermo units";
            treeNode79.Name = "Node3";
            treeNode79.Text = "All runs protein report";
            this.ReportTree.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode31,
            treeNode46,
            treeNode64,
            treeNode79});
            this.ReportTree.Size = new System.Drawing.Size(849, 193);
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
            this.groupBox3.Controls.Add(this.MaxZeroBox);
            this.groupBox3.Controls.Add(this.label14);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.ZeroSubstBox);
            this.groupBox3.Controls.Add(this.UseAllCheck);
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
            this.groupBox3.Size = new System.Drawing.Size(866, 118);
            this.groupBox3.TabIndex = 25;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Reference file options";
            // 
            // MaxZeroBox
            // 
            this.MaxZeroBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Quanty.Properties.Settings.Default, "MaxZeroes", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.MaxZeroBox.Location = new System.Drawing.Point(690, 88);
            this.MaxZeroBox.Name = "MaxZeroBox";
            this.MaxZeroBox.Size = new System.Drawing.Size(37, 22);
            this.MaxZeroBox.TabIndex = 42;
            this.MaxZeroBox.Text = global::Quanty.Properties.Settings.Default.MaxZeroes;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(600, 90);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(84, 17);
            this.label14.TabIndex = 41;
            this.label14.Text = "Max. zeroes";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(600, 59);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(80, 17);
            this.label13.TabIndex = 40;
            this.label13.Text = "Zero subst.";
            // 
            // ZeroSubstBox
            // 
            this.ZeroSubstBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Quanty.Properties.Settings.Default, "ZeroSubst", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.ZeroSubstBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ZeroSubstBox.FormattingEnabled = true;
            this.ZeroSubstBox.Items.AddRange(new object[] {
            "0.0",
            "1.0",
            "Empty"});
            this.ZeroSubstBox.Location = new System.Drawing.Point(681, 56);
            this.ZeroSubstBox.Name = "ZeroSubstBox";
            this.ZeroSubstBox.Size = new System.Drawing.Size(85, 24);
            this.ZeroSubstBox.TabIndex = 39;
            this.ZeroSubstBox.Text = global::Quanty.Properties.Settings.Default.ZeroSubst;
            // 
            // UseAllCheck
            // 
            this.UseAllCheck.AutoSize = true;
            this.UseAllCheck.Checked = global::Quanty.Properties.Settings.Default.UseAll;
            this.UseAllCheck.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Quanty.Properties.Settings.Default, "UseAll", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.UseAllCheck.Location = new System.Drawing.Point(434, 57);
            this.UseAllCheck.Name = "UseAllCheck";
            this.UseAllCheck.Size = new System.Drawing.Size(118, 21);
            this.UseAllCheck.TabIndex = 38;
            this.UseAllCheck.Text = "Use All MSMS";
            this.UseAllCheck.UseVisualStyleBackColor = true;
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Checked = global::Quanty.Properties.Settings.Default.ShapeFilter;
            this.checkBox4.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox4.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Quanty.Properties.Settings.Default, "ShapeFilter", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox4.Location = new System.Drawing.Point(253, 57);
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
            this.checkBox3.Location = new System.Drawing.Point(14, 90);
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
            this.checkBox2.Location = new System.Drawing.Point(253, 90);
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
            this.RefCombo.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Quanty.Properties.Settings.Default, "Reference", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.RefCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RefCombo.FormattingEnabled = true;
            this.RefCombo.Items.AddRange(new object[] {
            "None",
            "Matrix"});
            this.RefCombo.Location = new System.Drawing.Point(137, 22);
            this.RefCombo.Margin = new System.Windows.Forms.Padding(4);
            this.RefCombo.Name = "RefCombo";
            this.RefCombo.Size = new System.Drawing.Size(719, 24);
            this.RefCombo.TabIndex = 25;
            this.RefCombo.Text = global::Quanty.Properties.Settings.Default.Reference;
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
            this.DBFileName.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Quanty.Properties.Settings.Default, "DBFileName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.DBFileName.Location = new System.Drawing.Point(97, 54);
            this.DBFileName.Margin = new System.Windows.Forms.Padding(4);
            this.DBFileName.Name = "DBFileName";
            this.DBFileName.ReadOnly = true;
            this.DBFileName.Size = new System.Drawing.Size(713, 22);
            this.DBFileName.TabIndex = 27;
            this.DBFileName.Text = global::Quanty.Properties.Settings.Default.DBFileName;
            // 
            // dbOpenButton
            // 
            this.dbOpenButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.dbOpenButton.Location = new System.Drawing.Point(818, 53);
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
            this.OutPathName.Size = new System.Drawing.Size(695, 22);
            this.OutPathName.TabIndex = 4;
            this.OutPathName.Text = global::Quanty.Properties.Settings.Default.OutFilePath;
            // 
            // DatFileName
            // 
            this.DatFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DatFileName.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Quanty.Properties.Settings.Default, "DatFileName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.DatFileName.Location = new System.Drawing.Point(97, 16);
            this.DatFileName.Margin = new System.Windows.Forms.Padding(4);
            this.DatFileName.Name = "DatFileName";
            this.DatFileName.ReadOnly = true;
            this.DatFileName.Size = new System.Drawing.Size(713, 22);
            this.DatFileName.TabIndex = 0;
            this.DatFileName.Text = global::Quanty.Properties.Settings.Default.DatFileName;
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox4.Controls.Add(this.textBox11);
            this.groupBox4.Controls.Add(this.label17);
            this.groupBox4.Controls.Add(this.textBox10);
            this.groupBox4.Controls.Add(this.textBox9);
            this.groupBox4.Controls.Add(this.label16);
            this.groupBox4.Controls.Add(this.label15);
            this.groupBox4.Controls.Add(this.StickCheckBox);
            this.groupBox4.Controls.Add(this.MetaCheckBox);
            this.groupBox4.Location = new System.Drawing.Point(635, 368);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(251, 139);
            this.groupBox4.TabIndex = 29;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Metabolomics";
            // 
            // textBox11
            // 
            this.textBox11.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Quanty.Properties.Settings.Default, "RTRes", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox11.Location = new System.Drawing.Point(156, 110);
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new System.Drawing.Size(58, 22);
            this.textBox11.TabIndex = 19;
            this.textBox11.Text = global::Quanty.Properties.Settings.Default.RTRes;
            this.textBox11.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(6, 113);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(148, 17);
            this.label17.TabIndex = 18;
            this.label17.Text = "RT peak resolution %:";
            // 
            // textBox10
            // 
            this.textBox10.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Quanty.Properties.Settings.Default, "MinRTWidht", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox10.Location = new System.Drawing.Point(113, 82);
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new System.Drawing.Size(58, 22);
            this.textBox10.TabIndex = 17;
            this.textBox10.Text = global::Quanty.Properties.Settings.Default.MinRTWidht;
            this.textBox10.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox9
            // 
            this.textBox9.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Quanty.Properties.Settings.Default, "MaxRTWidth", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox9.Location = new System.Drawing.Point(187, 82);
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new System.Drawing.Size(58, 22);
            this.textBox9.TabIndex = 16;
            this.textBox9.Text = global::Quanty.Properties.Settings.Default.MaxRTWidth;
            this.textBox9.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(172, 85);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(16, 17);
            this.label16.TabIndex = 15;
            this.label16.Text = "≤";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(6, 85);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(110, 17);
            this.label15.TabIndex = 14;
            this.label15.Text = "RT peak width ≥";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(903, 740);
            this.Controls.Add(this.groupBox4);
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
            this.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Quanty.Properties.Settings.Default, "CaptionText", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(863, 785);
            this.Name = "Form1";
            this.Text = global::Quanty.Properties.Settings.Default.CaptionText;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
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
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
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
        private System.Windows.Forms.CheckBox checkBox5;
        private System.Windows.Forms.CheckBox AllCheckBox;
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
        private System.Windows.Forms.TextBox textBox8;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ListView RawList;
        private System.Windows.Forms.ColumnHeader FileName;
        private System.Windows.Forms.ColumnHeader Message;
        private System.Windows.Forms.CheckBox UseAllCheck;
        private System.Windows.Forms.CheckBox UseFDRCheck;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox ZeroSubstBox;
        private System.Windows.Forms.TextBox MaxZeroBox;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.CheckBox StickCheckBox;
        private System.Windows.Forms.CheckBox MetaCheckBox;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox textBox10;
        private System.Windows.Forms.TextBox textBox9;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox textBox11;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Timer timer1;
    }
}

