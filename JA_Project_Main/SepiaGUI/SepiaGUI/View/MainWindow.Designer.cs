namespace WindowsFormsApp1
{
    partial class MainApp
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
            this.UploadImage = new System.Windows.Forms.Button();
            this.uploadImageTextBox = new System.Windows.Forms.TextBox();
            this.InsertImage = new System.Windows.Forms.PictureBox();
            this.CsharpRadioButton = new System.Windows.Forms.RadioButton();
            this.AsmRadioButton = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.threadsGroupBox = new System.Windows.Forms.GroupBox();
            this.coresLabel = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.physicalProcessorsLabel = new System.Windows.Forms.Label();
            this.logicalProcessorsLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.ActiveThreadsComboBox = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.SepiaComboBox = new System.Windows.Forms.ComboBox();
            this.ConvertButton = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.LogRichTextBox = new System.Windows.Forms.RichTextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.ConvertImage = new System.Windows.Forms.PictureBox();
            this.SepiaTone = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.InsertImage)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.threadsGroupBox.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ConvertImage)).BeginInit();
            this.SuspendLayout();
            // 
            // UploadImage
            // 
            this.UploadImage.Location = new System.Drawing.Point(265, 269);
            this.UploadImage.Name = "UploadImage";
            this.UploadImage.Size = new System.Drawing.Size(102, 23);
            this.UploadImage.TabIndex = 0;
            this.UploadImage.Text = "Upload Image";
            this.UploadImage.UseVisualStyleBackColor = true;
            this.UploadImage.Click += new System.EventHandler(this.uploadImageButton);
            // 
            // uploadImageTextBox
            // 
            this.uploadImageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.uploadImageTextBox.Location = new System.Drawing.Point(5, 272);
            this.uploadImageTextBox.Name = "uploadImageTextBox";
            this.uploadImageTextBox.ReadOnly = true;
            this.uploadImageTextBox.Size = new System.Drawing.Size(251, 20);
            this.uploadImageTextBox.TabIndex = 1;
            // 
            // InsertImage
            // 
            this.InsertImage.Location = new System.Drawing.Point(5, 19);
            this.InsertImage.Name = "InsertImage";
            this.InsertImage.Size = new System.Drawing.Size(367, 236);
            this.InsertImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.InsertImage.TabIndex = 2;
            this.InsertImage.TabStop = false;
            // 
            // CsharpRadioButton
            // 
            this.CsharpRadioButton.AutoSize = true;
            this.CsharpRadioButton.Location = new System.Drawing.Point(6, 42);
            this.CsharpRadioButton.Name = "CsharpRadioButton";
            this.CsharpRadioButton.Size = new System.Drawing.Size(39, 17);
            this.CsharpRadioButton.TabIndex = 3;
            this.CsharpRadioButton.TabStop = true;
            this.CsharpRadioButton.Text = "C#";
            this.CsharpRadioButton.UseVisualStyleBackColor = true;
            // 
            // AsmRadioButton
            // 
            this.AsmRadioButton.AutoSize = true;
            this.AsmRadioButton.Location = new System.Drawing.Point(6, 19);
            this.AsmRadioButton.Name = "AsmRadioButton";
            this.AsmRadioButton.Size = new System.Drawing.Size(48, 17);
            this.AsmRadioButton.TabIndex = 4;
            this.AsmRadioButton.TabStop = true;
            this.AsmRadioButton.Text = "ASM";
            this.AsmRadioButton.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.Controls.Add(this.CsharpRadioButton);
            this.groupBox1.Controls.Add(this.AsmRadioButton);
            this.groupBox1.Location = new System.Drawing.Point(2, 344);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(129, 94);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "DLL Type";
            // 
            // threadsGroupBox
            // 
            this.threadsGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.threadsGroupBox.Controls.Add(this.coresLabel);
            this.threadsGroupBox.Controls.Add(this.label5);
            this.threadsGroupBox.Controls.Add(this.physicalProcessorsLabel);
            this.threadsGroupBox.Controls.Add(this.logicalProcessorsLabel);
            this.threadsGroupBox.Controls.Add(this.label2);
            this.threadsGroupBox.Controls.Add(this.label1);
            this.threadsGroupBox.Controls.Add(this.ActiveThreadsComboBox);
            this.threadsGroupBox.Location = new System.Drawing.Point(386, 263);
            this.threadsGroupBox.Name = "threadsGroupBox";
            this.threadsGroupBox.Size = new System.Drawing.Size(194, 175);
            this.threadsGroupBox.TabIndex = 6;
            this.threadsGroupBox.TabStop = false;
            this.threadsGroupBox.Text = "Active Threads";
            // 
            // coresLabel
            // 
            this.coresLabel.AutoSize = true;
            this.coresLabel.Location = new System.Drawing.Point(158, 97);
            this.coresLabel.Name = "coresLabel";
            this.coresLabel.Size = new System.Drawing.Size(34, 13);
            this.coresLabel.TabIndex = 9;
            this.coresLabel.Text = "Cnum";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(18, 97);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Cores";
            // 
            // physicalProcessorsLabel
            // 
            this.physicalProcessorsLabel.AutoSize = true;
            this.physicalProcessorsLabel.Location = new System.Drawing.Point(159, 75);
            this.physicalProcessorsLabel.Name = "physicalProcessorsLabel";
            this.physicalProcessorsLabel.Size = new System.Drawing.Size(34, 13);
            this.physicalProcessorsLabel.TabIndex = 11;
            this.physicalProcessorsLabel.Text = "Pnum";
            // 
            // logicalProcessorsLabel
            // 
            this.logicalProcessorsLabel.AutoSize = true;
            this.logicalProcessorsLabel.Location = new System.Drawing.Point(159, 53);
            this.logicalProcessorsLabel.Name = "logicalProcessorsLabel";
            this.logicalProcessorsLabel.Size = new System.Drawing.Size(33, 13);
            this.logicalProcessorsLabel.TabIndex = 10;
            this.logicalProcessorsLabel.Text = "Lnum";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Physical Processors";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Logical Processors";
            // 
            // ActiveThreadsComboBox
            // 
            this.ActiveThreadsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ActiveThreadsComboBox.FormattingEnabled = true;
            this.ActiveThreadsComboBox.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "27",
            "28",
            "29",
            "30",
            "31",
            "32",
            "33",
            "34",
            "35",
            "36",
            "37",
            "38",
            "39",
            "40",
            "41",
            "42",
            "43",
            "44",
            "45",
            "46",
            "47",
            "48",
            "49",
            "50",
            "51",
            "52",
            "53",
            "54",
            "55",
            "56",
            "57",
            "58",
            "59",
            "60",
            "61",
            "62",
            "63",
            "64"});
            this.ActiveThreadsComboBox.Location = new System.Drawing.Point(6, 19);
            this.ActiveThreadsComboBox.Name = "ActiveThreadsComboBox";
            this.ActiveThreadsComboBox.Size = new System.Drawing.Size(121, 21);
            this.ActiveThreadsComboBox.TabIndex = 7;
            // 
            // groupBox3
            // 
            this.groupBox3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox3.Controls.Add(this.SepiaComboBox);
            this.groupBox3.Controls.Add(this.SepiaTone);
            this.groupBox3.Controls.Add(this.ConvertButton);
            this.groupBox3.Controls.Add(this.UploadImage);
            this.groupBox3.Controls.Add(this.uploadImageTextBox);
            this.groupBox3.Controls.Add(this.InsertImage);
            this.groupBox3.Location = new System.Drawing.Point(2, 2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(378, 336);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Image";
            // 
            // SepiaComboBox
            // 
            this.SepiaComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SepiaComboBox.FormattingEnabled = true;
            this.SepiaComboBox.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "27",
            "28",
            "29",
            "30",
            "31",
            "32",
            "33",
            "34",
            "35",
            "36",
            "37",
            "38",
            "39",
            "40",
            "41",
            "42",
            "43",
            "44",
            "45",
            "46",
            "47",
            "48",
            "49",
            "50"});
            this.SepiaComboBox.Location = new System.Drawing.Point(8, 308);
            this.SepiaComboBox.Name = "SepiaComboBox";
            this.SepiaComboBox.Size = new System.Drawing.Size(121, 21);
            this.SepiaComboBox.TabIndex = 9;
            // 
            // ConvertButton
            // 
            this.ConvertButton.Location = new System.Drawing.Point(135, 306);
            this.ConvertButton.Name = "ConvertButton";
            this.ConvertButton.Size = new System.Drawing.Size(237, 23);
            this.ConvertButton.TabIndex = 8;
            this.ConvertButton.Text = "Convert";
            this.ConvertButton.UseVisualStyleBackColor = true;
            this.ConvertButton.Click += new System.EventHandler(this.convertImageButton);
            // 
            // groupBox4
            // 
            this.groupBox4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox4.Controls.Add(this.LogRichTextBox);
            this.groupBox4.Location = new System.Drawing.Point(123, 337);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(251, 101);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Log";
            // 
            // LogRichTextBox
            // 
            this.LogRichTextBox.Location = new System.Drawing.Point(38, 7);
            this.LogRichTextBox.Name = "LogRichTextBox";
            this.LogRichTextBox.Size = new System.Drawing.Size(187, 94);
            this.LogRichTextBox.TabIndex = 9;
            this.LogRichTextBox.Text = "";
            this.LogRichTextBox.TextChanged += new System.EventHandler(this.LogRichTextBox_TextChanged);
            // 
            // groupBox5
            // 
            this.groupBox5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox5.Controls.Add(this.ConvertImage);
            this.groupBox5.Location = new System.Drawing.Point(386, 2);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(390, 255);
            this.groupBox5.TabIndex = 10;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "ConvertedImage";
            // 
            // ConvertImage
            // 
            this.ConvertImage.Location = new System.Drawing.Point(21, 19);
            this.ConvertImage.Name = "ConvertImage";
            this.ConvertImage.Size = new System.Drawing.Size(361, 236);
            this.ConvertImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ConvertImage.TabIndex = 0;
            this.ConvertImage.TabStop = false;
            // 
            // SepiaTone
            // 
            this.SepiaTone.AutoSize = true;
            this.SepiaTone.Location = new System.Drawing.Point(6, 292);
            this.SepiaTone.Name = "SepiaTone";
            this.SepiaTone.Size = new System.Drawing.Size(59, 13);
            this.SepiaTone.TabIndex = 9;
            this.SepiaTone.Text = "SepiaTone";
            // 
            // MainApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.threadsGroupBox);
            this.Controls.Add(this.groupBox1);
            this.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainApp";
            this.Text = "MainApp";
            this.Load += new System.EventHandler(this.windowLoad);
            ((System.ComponentModel.ISupportInitialize)(this.InsertImage)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.threadsGroupBox.ResumeLayout(false);
            this.threadsGroupBox.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ConvertImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button UploadImage;
        private System.Windows.Forms.TextBox uploadImageTextBox;
        private System.Windows.Forms.PictureBox InsertImage;
        private System.Windows.Forms.RadioButton CsharpRadioButton;
        private System.Windows.Forms.RadioButton AsmRadioButton;
        private System.Windows.Forms.GroupBox threadsGroupBox;
        private System.Windows.Forms.ComboBox ActiveThreadsComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label logicalProcessorsLabel;
        private System.Windows.Forms.Label physicalProcessorsLabel;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button ConvertButton;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label coresLabel;
        private System.Windows.Forms.RichTextBox LogRichTextBox;
        private System.Windows.Forms.ComboBox SepiaComboBox;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.PictureBox ConvertImage;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label SepiaTone;
    }
}

