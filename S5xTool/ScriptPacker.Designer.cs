
namespace S5xTool
{
    partial class ScriptPacker
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
            this.label1 = new System.Windows.Forms.Label();
            this.TBOutFile = new System.Windows.Forms.TextBox();
            this.TXTInputFile = new System.Windows.Forms.Label();
            this.BtnInOpen = new System.Windows.Forms.Button();
            this.CBInOverride = new System.Windows.Forms.CheckBox();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.BtnRun = new System.Windows.Forms.Button();
            this.TbInFile = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.CBInArchive1 = new System.Windows.Forms.CheckBox();
            this.TBPath1 = new System.Windows.Forms.TextBox();
            this.BTNPath1 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.BTNPath2 = new System.Windows.Forms.Button();
            this.TBPath2 = new System.Windows.Forms.TextBox();
            this.CBInArchive2 = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.BTNPath3 = new System.Windows.Forms.Button();
            this.TBPath3 = new System.Windows.Forms.TextBox();
            this.CBInArchive3 = new System.Windows.Forms.CheckBox();
            this.OpenFile = new System.Windows.Forms.OpenFileDialog();
            this.OpenFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.CBCopy = new System.Windows.Forms.CheckBox();
            this.CBAddLoader = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Output File (in archive):";
            // 
            // TBOutFile
            // 
            this.TBOutFile.Location = new System.Drawing.Point(134, 12);
            this.TBOutFile.Name = "TBOutFile";
            this.TBOutFile.Size = new System.Drawing.Size(185, 20);
            this.TBOutFile.TabIndex = 1;
            // 
            // TXTInputFile
            // 
            this.TXTInputFile.AutoSize = true;
            this.TXTInputFile.Location = new System.Drawing.Point(12, 43);
            this.TXTInputFile.Name = "TXTInputFile";
            this.TXTInputFile.Size = new System.Drawing.Size(53, 13);
            this.TXTInputFile.TabIndex = 2;
            this.TXTInputFile.Text = "Input File:";
            // 
            // BtnInOpen
            // 
            this.BtnInOpen.Location = new System.Drawing.Point(325, 38);
            this.BtnInOpen.Name = "BtnInOpen";
            this.BtnInOpen.Size = new System.Drawing.Size(75, 23);
            this.BtnInOpen.TabIndex = 4;
            this.BtnInOpen.Text = "Open";
            this.BtnInOpen.UseVisualStyleBackColor = true;
            this.BtnInOpen.Click += new System.EventHandler(this.BtnInOpen_Click);
            // 
            // CBInOverride
            // 
            this.CBInOverride.AutoSize = true;
            this.CBInOverride.Location = new System.Drawing.Point(406, 42);
            this.CBInOverride.Name = "CBInOverride";
            this.CBInOverride.Size = new System.Drawing.Size(93, 17);
            this.CBInOverride.TabIndex = 5;
            this.CBInOverride.Text = "Override Input";
            this.CBInOverride.UseVisualStyleBackColor = true;
            // 
            // BtnCancel
            // 
            this.BtnCancel.Location = new System.Drawing.Point(713, 415);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(75, 23);
            this.BtnCancel.TabIndex = 6;
            this.BtnCancel.Text = "Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            this.BtnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // BtnRun
            // 
            this.BtnRun.Location = new System.Drawing.Point(632, 415);
            this.BtnRun.Name = "BtnRun";
            this.BtnRun.Size = new System.Drawing.Size(75, 23);
            this.BtnRun.TabIndex = 7;
            this.BtnRun.Text = "Go";
            this.BtnRun.UseVisualStyleBackColor = true;
            this.BtnRun.Click += new System.EventHandler(this.BtnRun_Click);
            // 
            // TbInFile
            // 
            this.TbInFile.Location = new System.Drawing.Point(134, 40);
            this.TbInFile.Name = "TbInFile";
            this.TbInFile.Size = new System.Drawing.Size(185, 20);
            this.TbInFile.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 151);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Search order:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.BTNPath1);
            this.groupBox1.Controls.Add(this.TBPath1);
            this.groupBox1.Controls.Add(this.CBInArchive1);
            this.groupBox1.Location = new System.Drawing.Point(15, 167);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(430, 50);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            // 
            // CBInArchive1
            // 
            this.CBInArchive1.AutoSize = true;
            this.CBInArchive1.Location = new System.Drawing.Point(6, 19);
            this.CBInArchive1.Name = "CBInArchive1";
            this.CBInArchive1.Size = new System.Drawing.Size(74, 17);
            this.CBInArchive1.TabIndex = 0;
            this.CBInArchive1.Text = "In Archive";
            this.CBInArchive1.UseVisualStyleBackColor = true;
            // 
            // TBPath1
            // 
            this.TBPath1.Location = new System.Drawing.Point(86, 17);
            this.TBPath1.Name = "TBPath1";
            this.TBPath1.Size = new System.Drawing.Size(218, 20);
            this.TBPath1.TabIndex = 1;
            // 
            // BTNPath1
            // 
            this.BTNPath1.Location = new System.Drawing.Point(310, 15);
            this.BTNPath1.Name = "BTNPath1";
            this.BTNPath1.Size = new System.Drawing.Size(75, 23);
            this.BTNPath1.TabIndex = 2;
            this.BTNPath1.Text = "Open";
            this.BTNPath1.UseVisualStyleBackColor = true;
            this.BTNPath1.Click += new System.EventHandler(this.BTNPath1_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.BTNPath2);
            this.groupBox2.Controls.Add(this.TBPath2);
            this.groupBox2.Controls.Add(this.CBInArchive2);
            this.groupBox2.Location = new System.Drawing.Point(15, 223);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(430, 50);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            // 
            // BTNPath2
            // 
            this.BTNPath2.Location = new System.Drawing.Point(310, 15);
            this.BTNPath2.Name = "BTNPath2";
            this.BTNPath2.Size = new System.Drawing.Size(75, 23);
            this.BTNPath2.TabIndex = 2;
            this.BTNPath2.Text = "Open";
            this.BTNPath2.UseVisualStyleBackColor = true;
            this.BTNPath2.Click += new System.EventHandler(this.BTNPath2_Click);
            // 
            // TBPath2
            // 
            this.TBPath2.Location = new System.Drawing.Point(86, 17);
            this.TBPath2.Name = "TBPath2";
            this.TBPath2.Size = new System.Drawing.Size(218, 20);
            this.TBPath2.TabIndex = 1;
            this.TBPath2.Text = "maps\\externalmap";
            // 
            // CBInArchive2
            // 
            this.CBInArchive2.AutoSize = true;
            this.CBInArchive2.Checked = true;
            this.CBInArchive2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CBInArchive2.Location = new System.Drawing.Point(6, 19);
            this.CBInArchive2.Name = "CBInArchive2";
            this.CBInArchive2.Size = new System.Drawing.Size(74, 17);
            this.CBInArchive2.TabIndex = 0;
            this.CBInArchive2.Text = "In Archive";
            this.CBInArchive2.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.BTNPath3);
            this.groupBox3.Controls.Add(this.TBPath3);
            this.groupBox3.Controls.Add(this.CBInArchive3);
            this.groupBox3.Location = new System.Drawing.Point(15, 279);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(430, 50);
            this.groupBox3.TabIndex = 12;
            this.groupBox3.TabStop = false;
            // 
            // BTNPath3
            // 
            this.BTNPath3.Location = new System.Drawing.Point(310, 15);
            this.BTNPath3.Name = "BTNPath3";
            this.BTNPath3.Size = new System.Drawing.Size(75, 23);
            this.BTNPath3.TabIndex = 2;
            this.BTNPath3.Text = "Open";
            this.BTNPath3.UseVisualStyleBackColor = true;
            this.BTNPath3.Click += new System.EventHandler(this.BTNPath3_Click);
            // 
            // TBPath3
            // 
            this.TBPath3.Location = new System.Drawing.Point(86, 17);
            this.TBPath3.Name = "TBPath3";
            this.TBPath3.Size = new System.Drawing.Size(218, 20);
            this.TBPath3.TabIndex = 1;
            // 
            // CBInArchive3
            // 
            this.CBInArchive3.AutoSize = true;
            this.CBInArchive3.Location = new System.Drawing.Point(6, 19);
            this.CBInArchive3.Name = "CBInArchive3";
            this.CBInArchive3.Size = new System.Drawing.Size(74, 17);
            this.CBInArchive3.TabIndex = 0;
            this.CBInArchive3.Text = "In Archive";
            this.CBInArchive3.UseVisualStyleBackColor = true;
            // 
            // OpenFile
            // 
            this.OpenFile.Filter = "Lua|*.lua|All|*.*";
            // 
            // CBCopy
            // 
            this.CBCopy.AutoSize = true;
            this.CBCopy.Location = new System.Drawing.Point(686, 167);
            this.CBCopy.Name = "CBCopy";
            this.CBCopy.Size = new System.Drawing.Size(102, 17);
            this.CBCopy.TabIndex = 13;
            this.CBCopy.Text = "Copy to one File";
            this.CBCopy.UseVisualStyleBackColor = true;
            // 
            // CBAddLoader
            // 
            this.CBAddLoader.AutoSize = true;
            this.CBAddLoader.Checked = true;
            this.CBAddLoader.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CBAddLoader.Location = new System.Drawing.Point(686, 190);
            this.CBAddLoader.Name = "CBAddLoader";
            this.CBAddLoader.Size = new System.Drawing.Size(81, 17);
            this.CBAddLoader.TabIndex = 14;
            this.CBAddLoader.Text = "Add Loader";
            this.CBAddLoader.UseVisualStyleBackColor = true;
            // 
            // ScriptPacker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.CBAddLoader);
            this.Controls.Add(this.CBCopy);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TbInFile);
            this.Controls.Add(this.BtnRun);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.CBInOverride);
            this.Controls.Add(this.BtnInOpen);
            this.Controls.Add(this.TXTInputFile);
            this.Controls.Add(this.TBOutFile);
            this.Controls.Add(this.label1);
            this.Name = "ScriptPacker";
            this.Text = "ScriptPacker";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TBOutFile;
        private System.Windows.Forms.Label TXTInputFile;
        private System.Windows.Forms.Button BtnInOpen;
        private System.Windows.Forms.CheckBox CBInOverride;
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.Button BtnRun;
        private System.Windows.Forms.TextBox TbInFile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button BTNPath1;
        private System.Windows.Forms.TextBox TBPath1;
        private System.Windows.Forms.CheckBox CBInArchive1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button BTNPath2;
        private System.Windows.Forms.TextBox TBPath2;
        private System.Windows.Forms.CheckBox CBInArchive2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button BTNPath3;
        private System.Windows.Forms.TextBox TBPath3;
        private System.Windows.Forms.CheckBox CBInArchive3;
        private System.Windows.Forms.OpenFileDialog OpenFile;
        private System.Windows.Forms.FolderBrowserDialog OpenFolder;
        private System.Windows.Forms.CheckBox CBCopy;
        private System.Windows.Forms.CheckBox CBAddLoader;
    }
}