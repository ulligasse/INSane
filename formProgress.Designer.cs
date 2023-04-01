
namespace INSane
{
    partial class formProgress
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
            this.pbScanProgress = new System.Windows.Forms.ProgressBar();
            this.lbl_pages = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // pbScanProgress
            // 
            this.pbScanProgress.Location = new System.Drawing.Point(13, 13);
            this.pbScanProgress.Name = "pbScanProgress";
            this.pbScanProgress.Size = new System.Drawing.Size(324, 32);
            this.pbScanProgress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.pbScanProgress.TabIndex = 0;
            // 
            // lbl_pages
            // 
            this.lbl_pages.AutoSize = true;
            this.lbl_pages.Location = new System.Drawing.Point(118, 84);
            this.lbl_pages.Name = "lbl_pages";
            this.lbl_pages.Size = new System.Drawing.Size(110, 13);
            this.lbl_pages.TabIndex = 1;
            this.lbl_pages.Text = "Gescannte Seite(n): 0";
            this.lbl_pages.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // formProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(349, 106);
            this.Controls.Add(this.lbl_pages);
            this.Controls.Add(this.pbScanProgress);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximumSize = new System.Drawing.Size(365, 145);
            this.MinimumSize = new System.Drawing.Size(365, 145);
            this.Name = "formProgress";
            this.Text = "Scanvorgang";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar pbScanProgress;
        private System.Windows.Forms.Label lbl_pages;
    }
}