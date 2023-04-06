
namespace INSane
{
    partial class formScan
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
            this.tbxHost = new System.Windows.Forms.TextBox();
            this.lblSource = new System.Windows.Forms.Label();
            this.btn_scan = new System.Windows.Forms.Button();
            this.cbxSource = new System.Windows.Forms.ComboBox();
            this.lblMode = new System.Windows.Forms.Label();
            this.cbxMode = new System.Windows.Forms.ComboBox();
            this.lblResolution = new System.Windows.Forms.Label();
            this.cbxResolution = new System.Windows.Forms.ComboBox();
            this.cbPageAuto = new System.Windows.Forms.CheckBox();
            this.cbBlankPageDetection = new System.Windows.Forms.CheckBox();
            this.tbBlankPageDetectionSensitivity = new System.Windows.Forms.TrackBar();
            this.lblBlankPageDetectionLow = new System.Windows.Forms.Label();
            this.lblBlankPageDetectionHigh = new System.Windows.Forms.Label();
            this.tbxScanner = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.tbBlankPageDetectionSensitivity)).BeginInit();
            this.SuspendLayout();
            // 
            // tbxHost
            // 
            this.tbxHost.Enabled = false;
            this.tbxHost.Location = new System.Drawing.Point(12, 12);
            this.tbxHost.Name = "tbxHost";
            this.tbxHost.Size = new System.Drawing.Size(128, 20);
            this.tbxHost.TabIndex = 0;
            // 
            // lblSource
            // 
            this.lblSource.AutoSize = true;
            this.lblSource.Location = new System.Drawing.Point(9, 47);
            this.lblSource.Name = "lblSource";
            this.lblSource.Size = new System.Drawing.Size(39, 13);
            this.lblSource.TabIndex = 2;
            this.lblSource.Text = "Einzug";
            // 
            // btn_scan
            // 
            this.btn_scan.Location = new System.Drawing.Point(15, 360);
            this.btn_scan.Name = "btn_scan";
            this.btn_scan.Size = new System.Drawing.Size(271, 39);
            this.btn_scan.TabIndex = 3;
            this.btn_scan.Text = "Scannen";
            this.btn_scan.UseVisualStyleBackColor = true;
            // 
            // cbxSource
            // 
            this.cbxSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxSource.FormattingEnabled = true;
            this.cbxSource.Location = new System.Drawing.Point(12, 64);
            this.cbxSource.Name = "cbxSource";
            this.cbxSource.Size = new System.Drawing.Size(274, 21);
            this.cbxSource.TabIndex = 5;
            this.cbxSource.SelectedIndexChanged += new System.EventHandler(this.cbxSource_SelectedIndexChanged);
            // 
            // lblMode
            // 
            this.lblMode.AutoSize = true;
            this.lblMode.Location = new System.Drawing.Point(9, 92);
            this.lblMode.Name = "lblMode";
            this.lblMode.Size = new System.Drawing.Size(34, 13);
            this.lblMode.TabIndex = 6;
            this.lblMode.Text = "Farbe";
            // 
            // cbxMode
            // 
            this.cbxMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxMode.FormattingEnabled = true;
            this.cbxMode.Location = new System.Drawing.Point(12, 108);
            this.cbxMode.Name = "cbxMode";
            this.cbxMode.Size = new System.Drawing.Size(274, 21);
            this.cbxMode.TabIndex = 7;
            this.cbxMode.SelectedIndexChanged += new System.EventHandler(this.cbxMode_SelectedIndexChanged);
            // 
            // lblResolution
            // 
            this.lblResolution.AutoSize = true;
            this.lblResolution.Location = new System.Drawing.Point(9, 139);
            this.lblResolution.Name = "lblResolution";
            this.lblResolution.Size = new System.Drawing.Size(43, 13);
            this.lblResolution.TabIndex = 8;
            this.lblResolution.Text = "Qualität";
            // 
            // cbxResolution
            // 
            this.cbxResolution.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxResolution.FormattingEnabled = true;
            this.cbxResolution.Location = new System.Drawing.Point(12, 155);
            this.cbxResolution.Name = "cbxResolution";
            this.cbxResolution.Size = new System.Drawing.Size(274, 21);
            this.cbxResolution.TabIndex = 9;
            this.cbxResolution.SelectedIndexChanged += new System.EventHandler(this.cbxResolution_SelectedIndexChanged);
            // 
            // cbPageAuto
            // 
            this.cbPageAuto.AutoSize = true;
            this.cbPageAuto.Location = new System.Drawing.Point(12, 194);
            this.cbPageAuto.Name = "cbPageAuto";
            this.cbPageAuto.Size = new System.Drawing.Size(233, 17);
            this.cbPageAuto.TabIndex = 10;
            this.cbPageAuto.Text = "Seite automatisch zuschneiden / ausrichten";
            this.cbPageAuto.UseVisualStyleBackColor = true;
            this.cbPageAuto.CheckedChanged += new System.EventHandler(this.cbPageAuto_CheckedChanged);
            // 
            // cbBlankPageDetection
            // 
            this.cbBlankPageDetection.AutoSize = true;
            this.cbBlankPageDetection.Location = new System.Drawing.Point(12, 230);
            this.cbBlankPageDetection.Name = "cbBlankPageDetection";
            this.cbBlankPageDetection.Size = new System.Drawing.Size(133, 17);
            this.cbBlankPageDetection.TabIndex = 11;
            this.cbBlankPageDetection.Text = "leere Seitenerkennung";
            this.cbBlankPageDetection.UseVisualStyleBackColor = true;
            this.cbBlankPageDetection.CheckedChanged += new System.EventHandler(this.cbBlankPageDetection_CheckedChanged);
            // 
            // tbBlankPageDetectionSensitivity
            // 
            this.tbBlankPageDetectionSensitivity.Enabled = false;
            this.tbBlankPageDetectionSensitivity.Location = new System.Drawing.Point(12, 253);
            this.tbBlankPageDetectionSensitivity.Maximum = 5;
            this.tbBlankPageDetectionSensitivity.Name = "tbBlankPageDetectionSensitivity";
            this.tbBlankPageDetectionSensitivity.Size = new System.Drawing.Size(273, 45);
            this.tbBlankPageDetectionSensitivity.TabIndex = 12;
            this.tbBlankPageDetectionSensitivity.Value = 5;
            this.tbBlankPageDetectionSensitivity.ValueChanged += new System.EventHandler(this.tbBlankPageDetectionSensitivity_ValueChanged);
            // 
            // lblBlankPageDetectionLow
            // 
            this.lblBlankPageDetectionLow.AutoSize = true;
            this.lblBlankPageDetectionLow.Location = new System.Drawing.Point(12, 284);
            this.lblBlankPageDetectionLow.Name = "lblBlankPageDetectionLow";
            this.lblBlankPageDetectionLow.Size = new System.Drawing.Size(24, 13);
            this.lblBlankPageDetectionLow.TabIndex = 13;
            this.lblBlankPageDetectionLow.Text = "leer";
            // 
            // lblBlankPageDetectionHigh
            // 
            this.lblBlankPageDetectionHigh.AutoSize = true;
            this.lblBlankPageDetectionHigh.Location = new System.Drawing.Point(255, 284);
            this.lblBlankPageDetectionHigh.Name = "lblBlankPageDetectionHigh";
            this.lblBlankPageDetectionHigh.Size = new System.Drawing.Size(30, 13);
            this.lblBlankPageDetectionHigh.TabIndex = 14;
            this.lblBlankPageDetectionHigh.Text = "stark";
            // 
            // tbxScanner
            // 
            this.tbxScanner.Enabled = false;
            this.tbxScanner.Location = new System.Drawing.Point(146, 12);
            this.tbxScanner.Name = "tbxScanner";
            this.tbxScanner.Size = new System.Drawing.Size(140, 20);
            this.tbxScanner.TabIndex = 1;
            // 
            // formScan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(298, 411);
            this.Controls.Add(this.lblBlankPageDetectionHigh);
            this.Controls.Add(this.lblBlankPageDetectionLow);
            this.Controls.Add(this.tbBlankPageDetectionSensitivity);
            this.Controls.Add(this.cbBlankPageDetection);
            this.Controls.Add(this.cbPageAuto);
            this.Controls.Add(this.cbxResolution);
            this.Controls.Add(this.lblResolution);
            this.Controls.Add(this.cbxMode);
            this.Controls.Add(this.lblMode);
            this.Controls.Add(this.cbxSource);
            this.Controls.Add(this.btn_scan);
            this.Controls.Add(this.lblSource);
            this.Controls.Add(this.tbxScanner);
            this.Controls.Add(this.tbxHost);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximumSize = new System.Drawing.Size(314, 450);
            this.MinimumSize = new System.Drawing.Size(314, 450);
            this.Name = "formScan";
            this.Text = "Scannereinstellungen";
            ((System.ComponentModel.ISupportInitialize)(this.tbBlankPageDetectionSensitivity)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbxHost;
        private System.Windows.Forms.Label lblSource;
        public System.Windows.Forms.Button btn_scan;
        private System.Windows.Forms.ComboBox cbxSource;
        private System.Windows.Forms.Label lblMode;
        private System.Windows.Forms.ComboBox cbxMode;
        private System.Windows.Forms.Label lblResolution;
        private System.Windows.Forms.ComboBox cbxResolution;
        private System.Windows.Forms.CheckBox cbPageAuto;
        private System.Windows.Forms.CheckBox cbBlankPageDetection;
        private System.Windows.Forms.TrackBar tbBlankPageDetectionSensitivity;
        private System.Windows.Forms.Label lblBlankPageDetectionLow;
        private System.Windows.Forms.Label lblBlankPageDetectionHigh;
        private System.Windows.Forms.TextBox tbxScanner;
    }
}