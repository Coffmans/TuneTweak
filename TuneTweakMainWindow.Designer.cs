namespace TuneTweak
{
    partial class TuneTweakMainWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            BottomToolStripPanel = new ToolStripPanel();
            TopToolStripPanel = new ToolStripPanel();
            RightToolStripPanel = new ToolStripPanel();
            LeftToolStripPanel = new ToolStripPanel();
            ContentPanel = new ToolStripContentPanel();
            btnSelectFiles = new Button();
            btnSelectFolder = new Button();
            label1 = new Label();
            lblStatus = new Label();
            pbConversion = new ProgressBar();
            btnConvert = new Button();
            cboConversionFormat = new ComboBox();
            label3 = new Label();
            groupBox1 = new GroupBox();
            chkReadMetadata = new CheckBox();
            chkNormalizeAudio = new CheckBox();
            chkTrimSilence = new CheckBox();
            picAlbumArt = new PictureBox();
            txtTitle = new TextBox();
            txtArtist = new TextBox();
            txtAlbum = new TextBox();
            txtGenre = new TextBox();
            label2 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            txtYear = new TextBox();
            dgvFilesToConvert = new DataGridView();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picAlbumArt).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvFilesToConvert).BeginInit();
            SuspendLayout();
            // 
            // BottomToolStripPanel
            // 
            BottomToolStripPanel.Location = new Point(0, 0);
            BottomToolStripPanel.Name = "BottomToolStripPanel";
            BottomToolStripPanel.Orientation = Orientation.Horizontal;
            BottomToolStripPanel.RowMargin = new Padding(3, 0, 0, 0);
            BottomToolStripPanel.Size = new Size(0, 0);
            // 
            // TopToolStripPanel
            // 
            TopToolStripPanel.Location = new Point(0, 0);
            TopToolStripPanel.Name = "TopToolStripPanel";
            TopToolStripPanel.Orientation = Orientation.Horizontal;
            TopToolStripPanel.RowMargin = new Padding(3, 0, 0, 0);
            TopToolStripPanel.Size = new Size(0, 0);
            // 
            // RightToolStripPanel
            // 
            RightToolStripPanel.Location = new Point(0, 0);
            RightToolStripPanel.Name = "RightToolStripPanel";
            RightToolStripPanel.Orientation = Orientation.Horizontal;
            RightToolStripPanel.RowMargin = new Padding(3, 0, 0, 0);
            RightToolStripPanel.Size = new Size(0, 0);
            // 
            // LeftToolStripPanel
            // 
            LeftToolStripPanel.Location = new Point(0, 0);
            LeftToolStripPanel.Name = "LeftToolStripPanel";
            LeftToolStripPanel.Orientation = Orientation.Horizontal;
            LeftToolStripPanel.RowMargin = new Padding(3, 0, 0, 0);
            LeftToolStripPanel.Size = new Size(0, 0);
            // 
            // ContentPanel
            // 
            ContentPanel.Size = new Size(150, 125);
            // 
            // btnSelectFiles
            // 
            btnSelectFiles.Location = new Point(205, 18);
            btnSelectFiles.Name = "btnSelectFiles";
            btnSelectFiles.Size = new Size(110, 23);
            btnSelectFiles.TabIndex = 1;
            btnSelectFiles.Text = "Select File(s)";
            btnSelectFiles.UseVisualStyleBackColor = true;
            btnSelectFiles.Click += btnSelectFiles_Click;
            // 
            // btnSelectFolder
            // 
            btnSelectFolder.Location = new Point(321, 18);
            btnSelectFolder.Name = "btnSelectFolder";
            btnSelectFolder.Size = new Size(112, 23);
            btnSelectFolder.TabIndex = 2;
            btnSelectFolder.Text = "Select Folder";
            btnSelectFolder.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(18, 26);
            label1.Name = "label1";
            label1.Size = new Size(91, 15);
            label1.TabIndex = 5;
            label1.Text = "Files To Convert";
            // 
            // lblStatus
            // 
            lblStatus.Location = new Point(18, 574);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(617, 67);
            lblStatus.TabIndex = 8;
            lblStatus.Text = "Status";
            // 
            // pbConversion
            // 
            pbConversion.Location = new Point(22, 648);
            pbConversion.Name = "pbConversion";
            pbConversion.Size = new Size(613, 23);
            pbConversion.TabIndex = 9;
            // 
            // btnConvert
            // 
            btnConvert.Location = new Point(239, 518);
            btnConvert.Name = "btnConvert";
            btnConvert.Size = new Size(112, 23);
            btnConvert.TabIndex = 10;
            btnConvert.Text = "Convert";
            btnConvert.UseVisualStyleBackColor = true;
            btnConvert.Click += btnConvert_Click;
            // 
            // cboConversionFormat
            // 
            cboConversionFormat.DropDownStyle = ComboBoxStyle.DropDownList;
            cboConversionFormat.FormattingEnabled = true;
            cboConversionFormat.Location = new Point(6, 49);
            cboConversionFormat.Name = "cboConversionFormat";
            cboConversionFormat.Size = new Size(118, 23);
            cboConversionFormat.TabIndex = 11;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(6, 31);
            label3.Name = "label3";
            label3.Size = new Size(45, 15);
            label3.TabIndex = 12;
            label3.Text = "Format";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(chkReadMetadata);
            groupBox1.Controls.Add(chkNormalizeAudio);
            groupBox1.Controls.Add(chkTrimSilence);
            groupBox1.Controls.Add(cboConversionFormat);
            groupBox1.Controls.Add(label3);
            groupBox1.Location = new Point(18, 408);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(623, 104);
            groupBox1.TabIndex = 13;
            groupBox1.TabStop = false;
            groupBox1.Text = "Conversion Settings";
            // 
            // chkReadMetadata
            // 
            chkReadMetadata.AutoSize = true;
            chkReadMetadata.Location = new Point(487, 51);
            chkReadMetadata.Name = "chkReadMetadata";
            chkReadMetadata.Size = new Size(105, 19);
            chkReadMetadata.TabIndex = 15;
            chkReadMetadata.Text = "Read Metadata";
            chkReadMetadata.UseVisualStyleBackColor = true;
            // 
            // chkNormalizeAudio
            // 
            chkNormalizeAudio.AutoSize = true;
            chkNormalizeAudio.Location = new Point(288, 51);
            chkNormalizeAudio.Name = "chkNormalizeAudio";
            chkNormalizeAudio.Size = new Size(115, 19);
            chkNormalizeAudio.TabIndex = 14;
            chkNormalizeAudio.Text = "Normalize Audio";
            chkNormalizeAudio.UseVisualStyleBackColor = true;
            // 
            // chkTrimSilence
            // 
            chkTrimSilence.AutoSize = true;
            chkTrimSilence.Location = new Point(145, 51);
            chkTrimSilence.Name = "chkTrimSilence";
            chkTrimSilence.Size = new Size(90, 19);
            chkTrimSilence.TabIndex = 13;
            chkTrimSilence.Text = "Trim Silence";
            chkTrimSilence.UseVisualStyleBackColor = true;
            // 
            // picAlbumArt
            // 
            picAlbumArt.Location = new Point(671, 423);
            picAlbumArt.Name = "picAlbumArt";
            picAlbumArt.Size = new Size(113, 89);
            picAlbumArt.TabIndex = 14;
            picAlbumArt.TabStop = false;
            // 
            // txtTitle
            // 
            txtTitle.Location = new Point(671, 536);
            txtTitle.Name = "txtTitle";
            txtTitle.Size = new Size(467, 23);
            txtTitle.TabIndex = 15;
            // 
            // txtArtist
            // 
            txtArtist.Location = new Point(671, 589);
            txtArtist.Name = "txtArtist";
            txtArtist.Size = new Size(467, 23);
            txtArtist.TabIndex = 16;
            // 
            // txtAlbum
            // 
            txtAlbum.Location = new Point(671, 642);
            txtAlbum.Name = "txtAlbum";
            txtAlbum.Size = new Size(467, 23);
            txtAlbum.TabIndex = 17;
            // 
            // txtGenre
            // 
            txtGenre.Location = new Point(671, 687);
            txtGenre.Name = "txtGenre";
            txtGenre.Size = new Size(379, 23);
            txtGenre.TabIndex = 18;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(671, 515);
            label2.Name = "label2";
            label2.Size = new Size(30, 15);
            label2.TabIndex = 19;
            label2.Text = "Title";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(671, 571);
            label4.Name = "label4";
            label4.Size = new Size(35, 15);
            label4.TabIndex = 20;
            label4.Text = "Artist";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(671, 624);
            label5.Name = "label5";
            label5.Size = new Size(43, 15);
            label5.TabIndex = 21;
            label5.Text = "Album";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(671, 669);
            label6.Name = "label6";
            label6.Size = new Size(38, 15);
            label6.TabIndex = 22;
            label6.Text = "Genre";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(1056, 669);
            label7.Name = "label7";
            label7.Size = new Size(29, 15);
            label7.TabIndex = 24;
            label7.Text = "Year";
            // 
            // txtYear
            // 
            txtYear.Location = new Point(1056, 687);
            txtYear.Name = "txtYear";
            txtYear.Size = new Size(82, 23);
            txtYear.TabIndex = 23;
            // 
            // dgvFilesToConvert
            // 
            dgvFilesToConvert.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvFilesToConvert.Location = new Point(18, 52);
            dgvFilesToConvert.Name = "dgvFilesToConvert";
            dgvFilesToConvert.Size = new Size(1120, 350);
            dgvFilesToConvert.TabIndex = 25;
            dgvFilesToConvert.SelectionChanged += dgvFilesToConvert_SelectionChanged;
            // 
            // TuneTweakMainWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1159, 780);
            Controls.Add(dgvFilesToConvert);
            Controls.Add(label7);
            Controls.Add(txtYear);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label2);
            Controls.Add(txtGenre);
            Controls.Add(txtAlbum);
            Controls.Add(txtArtist);
            Controls.Add(txtTitle);
            Controls.Add(picAlbumArt);
            Controls.Add(groupBox1);
            Controls.Add(btnConvert);
            Controls.Add(pbConversion);
            Controls.Add(lblStatus);
            Controls.Add(label1);
            Controls.Add(btnSelectFolder);
            Controls.Add(btnSelectFiles);
            Name = "TuneTweakMainWindow";
            Text = "Form1";
            Load += TuneTweakMainWindow_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picAlbumArt).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvFilesToConvert).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private ToolStripPanel BottomToolStripPanel;
        private ToolStripPanel TopToolStripPanel;
        private ToolStripPanel RightToolStripPanel;
        private ToolStripPanel LeftToolStripPanel;
        private ToolStripContentPanel ContentPanel;
        private Button btnSelectFiles;
        private Button btnSelectFolder;
        private Label label1;
        private Label lblStatus;
        private ProgressBar pbConversion;
        private Button btnConvert;
        private ComboBox cboConversionFormat;
        private Label label3;
        private GroupBox groupBox1;
        private CheckBox chkReadMetadata;
        private CheckBox chkNormalizeAudio;
        private CheckBox chkTrimSilence;
        private PictureBox picAlbumArt;
        private TextBox txtTitle;
        private TextBox txtArtist;
        private TextBox txtAlbum;
        private TextBox txtGenre;
        private Label label2;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private TextBox txtYear;
        private DataGridView dgvFilesToConvert;
    }
}
