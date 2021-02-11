
namespace S5xTool
{
    partial class FilePeek
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
            this.TB_Data = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // TB_Data
            // 
            this.TB_Data.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TB_Data.Location = new System.Drawing.Point(12, 12);
            this.TB_Data.MinimumSize = new System.Drawing.Size(1075, 621);
            this.TB_Data.Multiline = true;
            this.TB_Data.Name = "TB_Data";
            this.TB_Data.ReadOnly = true;
            this.TB_Data.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TB_Data.Size = new System.Drawing.Size(1075, 621);
            this.TB_Data.TabIndex = 0;
            this.TB_Data.WordWrap = false;
            // 
            // FilePeek
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1099, 645);
            this.Controls.Add(this.TB_Data);
            this.Name = "FilePeek";
            this.Text = "S5xTool - File Peek";
            this.Shown += new System.EventHandler(this.FilePeek_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TB_Data;
    }
}