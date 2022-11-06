
namespace S5xTool
{
    partial class S5xToolGUI
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
            System.Windows.Forms.Label LBLMPMode;
            System.Windows.Forms.Label LBLKey;
            this.ListBox_Data = new System.Windows.Forms.ListBox();
            this.BtnLoad = new System.Windows.Forms.Button();
            this.BtnSave = new System.Windows.Forms.Button();
            this.Dlg_Open = new System.Windows.Forms.OpenFileDialog();
            this.CB_LoadMerge = new System.Windows.Forms.CheckBox();
            this.BtnRemove = new System.Windows.Forms.Button();
            this.TB_Rename = new System.Windows.Forms.TextBox();
            this.BtnRenameTo = new System.Windows.Forms.Button();
            this.CB_AskAdd = new System.Windows.Forms.CheckBox();
            this.BtnAddFile = new System.Windows.Forms.Button();
            this.BtnExportFile = new System.Windows.Forms.Button();
            this.Dlg_Save = new System.Windows.Forms.SaveFileDialog();
            this.BtnRandomGUID = new System.Windows.Forms.Button();
            this.CBRandomGUID = new System.Windows.Forms.CheckBox();
            this.ComboBox_MPType = new System.Windows.Forms.ComboBox();
            this.ComboBox_Key = new System.Windows.Forms.ComboBox();
            this.BtnPeek = new System.Windows.Forms.Button();
            this.BtnReplaceImage = new System.Windows.Forms.Button();
            this.BtnLoadEx2Bbas = new System.Windows.Forms.Button();
            this.BtnSort = new System.Windows.Forms.Button();
            this.BtnLoadFolder = new System.Windows.Forms.Button();
            this.Dlg_Folder = new System.Windows.Forms.FolderBrowserDialog();
            this.BtnSaveFolder = new System.Windows.Forms.Button();
            this.BtnImportFolderMap = new System.Windows.Forms.Button();
            this.PicBoxPreviewImg = new System.Windows.Forms.PictureBox();
            this.BtnPackScript = new System.Windows.Forms.Button();
            this.BtnCompile = new System.Windows.Forms.Button();
            this.BtnLuaMakro = new System.Windows.Forms.Button();
            this.Btn_LoadToMem = new System.Windows.Forms.Button();
            this.CB_Compressed = new System.Windows.Forms.CheckBox();
            this.BtnSearchDuplicates = new System.Windows.Forms.Button();
            this.CB_AutoCompression = new System.Windows.Forms.CheckBox();
            LBLMPMode = new System.Windows.Forms.Label();
            LBLKey = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.PicBoxPreviewImg)).BeginInit();
            this.SuspendLayout();
            // 
            // LBLMPMode
            // 
            LBLMPMode.AutoSize = true;
            LBLMPMode.Location = new System.Drawing.Point(166, 239);
            LBLMPMode.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LBLMPMode.Name = "LBLMPMode";
            LBLMPMode.Size = new System.Drawing.Size(62, 15);
            LBLMPMode.TabIndex = 13;
            LBLMPMode.Text = "MP Mode:";
            // 
            // LBLKey
            // 
            LBLKey.AutoSize = true;
            LBLKey.Location = new System.Drawing.Point(166, 272);
            LBLKey.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LBLKey.Name = "LBLKey";
            LBLKey.Size = new System.Drawing.Size(48, 15);
            LBLKey.TabIndex = 15;
            LBLKey.Text = "AddOn:";
            // 
            // ListBox_Data
            // 
            this.ListBox_Data.AllowDrop = true;
            this.ListBox_Data.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ListBox_Data.FormattingEnabled = true;
            this.ListBox_Data.ItemHeight = 15;
            this.ListBox_Data.Location = new System.Drawing.Point(581, 14);
            this.ListBox_Data.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ListBox_Data.MinimumSize = new System.Drawing.Size(832, 649);
            this.ListBox_Data.Name = "ListBox_Data";
            this.ListBox_Data.Size = new System.Drawing.Size(832, 649);
            this.ListBox_Data.TabIndex = 0;
            this.ListBox_Data.SelectedIndexChanged += new System.EventHandler(this.ListBox_Data_SelectedIndexChanged);
            this.ListBox_Data.DragDrop += new System.Windows.Forms.DragEventHandler(this.ListBox_Data_DragDrop);
            this.ListBox_Data.DragEnter += new System.Windows.Forms.DragEventHandler(this.ListBox_Data_DragEnter);
            // 
            // BtnLoad
            // 
            this.BtnLoad.Location = new System.Drawing.Point(14, 14);
            this.BtnLoad.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BtnLoad.Name = "BtnLoad";
            this.BtnLoad.Size = new System.Drawing.Size(147, 27);
            this.BtnLoad.TabIndex = 1;
            this.BtnLoad.Text = "Load s5x/bba";
            this.BtnLoad.UseVisualStyleBackColor = true;
            this.BtnLoad.Click += new System.EventHandler(this.BtnLoad_Click);
            // 
            // BtnSave
            // 
            this.BtnSave.Location = new System.Drawing.Point(168, 14);
            this.BtnSave.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BtnSave.Name = "BtnSave";
            this.BtnSave.Size = new System.Drawing.Size(156, 27);
            this.BtnSave.TabIndex = 2;
            this.BtnSave.Text = "Save s5x/bba";
            this.BtnSave.UseVisualStyleBackColor = true;
            this.BtnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // Dlg_Open
            // 
            this.Dlg_Open.Filter = "Maps|*.s5x|Archives|*.bba|All|*.*|Map Images|*.png|Import Map|info.xml|Lua Script" +
    "|*.lua";
            // 
            // CB_LoadMerge
            // 
            this.CB_LoadMerge.AutoSize = true;
            this.CB_LoadMerge.Location = new System.Drawing.Point(14, 47);
            this.CB_LoadMerge.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.CB_LoadMerge.Name = "CB_LoadMerge";
            this.CB_LoadMerge.Size = new System.Drawing.Size(140, 19);
            this.CB_LoadMerge.TabIndex = 3;
            this.CB_LoadMerge.Text = "Load performs Merge";
            this.CB_LoadMerge.UseVisualStyleBackColor = true;
            // 
            // BtnRemove
            // 
            this.BtnRemove.Location = new System.Drawing.Point(14, 383);
            this.BtnRemove.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BtnRemove.Name = "BtnRemove";
            this.BtnRemove.Size = new System.Drawing.Size(147, 27);
            this.BtnRemove.TabIndex = 4;
            this.BtnRemove.Text = "Remove";
            this.BtnRemove.UseVisualStyleBackColor = true;
            this.BtnRemove.Click += new System.EventHandler(this.BtnRemove_Click);
            // 
            // TB_Rename
            // 
            this.TB_Rename.Location = new System.Drawing.Point(168, 352);
            this.TB_Rename.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.TB_Rename.Name = "TB_Rename";
            this.TB_Rename.Size = new System.Drawing.Size(405, 23);
            this.TB_Rename.TabIndex = 5;
            // 
            // BtnRenameTo
            // 
            this.BtnRenameTo.Location = new System.Drawing.Point(14, 350);
            this.BtnRenameTo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BtnRenameTo.Name = "BtnRenameTo";
            this.BtnRenameTo.Size = new System.Drawing.Size(147, 27);
            this.BtnRenameTo.TabIndex = 6;
            this.BtnRenameTo.Text = "Rename To:";
            this.BtnRenameTo.UseVisualStyleBackColor = true;
            this.BtnRenameTo.Click += new System.EventHandler(this.BtnRenameTo_Click);
            // 
            // CB_AskAdd
            // 
            this.CB_AskAdd.AutoSize = true;
            this.CB_AskAdd.Location = new System.Drawing.Point(14, 74);
            this.CB_AskAdd.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.CB_AskAdd.Name = "CB_AskAdd";
            this.CB_AskAdd.Size = new System.Drawing.Size(107, 19);
            this.CB_AskAdd.TabIndex = 7;
            this.CB_AskAdd.Text = "Ask Before Add";
            this.CB_AskAdd.UseVisualStyleBackColor = true;
            // 
            // BtnAddFile
            // 
            this.BtnAddFile.Location = new System.Drawing.Point(14, 100);
            this.BtnAddFile.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BtnAddFile.Name = "BtnAddFile";
            this.BtnAddFile.Size = new System.Drawing.Size(147, 27);
            this.BtnAddFile.TabIndex = 8;
            this.BtnAddFile.Text = "Add File";
            this.BtnAddFile.UseVisualStyleBackColor = true;
            this.BtnAddFile.Click += new System.EventHandler(this.BtnAddFile_Click);
            // 
            // BtnExportFile
            // 
            this.BtnExportFile.Location = new System.Drawing.Point(168, 100);
            this.BtnExportFile.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BtnExportFile.Name = "BtnExportFile";
            this.BtnExportFile.Size = new System.Drawing.Size(156, 27);
            this.BtnExportFile.TabIndex = 9;
            this.BtnExportFile.Text = "Export File";
            this.BtnExportFile.UseVisualStyleBackColor = true;
            this.BtnExportFile.Click += new System.EventHandler(this.BtnExportFile_Click);
            // 
            // Dlg_Save
            // 
            this.Dlg_Save.Filter = "Maps|*.s5x|Archives|*.bba|All|*.*";
            // 
            // BtnRandomGUID
            // 
            this.BtnRandomGUID.Location = new System.Drawing.Point(14, 233);
            this.BtnRandomGUID.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BtnRandomGUID.Name = "BtnRandomGUID";
            this.BtnRandomGUID.Size = new System.Drawing.Size(147, 27);
            this.BtnRandomGUID.TabIndex = 10;
            this.BtnRandomGUID.Text = "Randomize GUID";
            this.BtnRandomGUID.UseVisualStyleBackColor = true;
            this.BtnRandomGUID.Click += new System.EventHandler(this.BtnRandomGUID_Click);
            // 
            // CBRandomGUID
            // 
            this.CBRandomGUID.AutoSize = true;
            this.CBRandomGUID.Checked = true;
            this.CBRandomGUID.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CBRandomGUID.Location = new System.Drawing.Point(168, 47);
            this.CBRandomGUID.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.CBRandomGUID.Name = "CBRandomGUID";
            this.CBRandomGUID.Size = new System.Drawing.Size(115, 19);
            this.CBRandomGUID.TabIndex = 11;
            this.CBRandomGUID.Text = "Randomize GUID";
            this.CBRandomGUID.UseVisualStyleBackColor = true;
            // 
            // ComboBox_MPType
            // 
            this.ComboBox_MPType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBox_MPType.FormattingEnabled = true;
            this.ComboBox_MPType.Items.AddRange(new object[] {
            "SP",
            "MP max 1",
            "MP max 2",
            "MP max 3",
            "MP max 4",
            "MP max 5",
            "MP max 6",
            "MP max 7",
            "MP max 8",
            "MP max 9",
            "MP max 10",
            "MP max 11",
            "MP max 12",
            "MP max 13",
            "MP max 14",
            "MP max 15",
            "MP max 16"});
            this.ComboBox_MPType.Location = new System.Drawing.Point(238, 235);
            this.ComboBox_MPType.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ComboBox_MPType.Name = "ComboBox_MPType";
            this.ComboBox_MPType.Size = new System.Drawing.Size(156, 23);
            this.ComboBox_MPType.TabIndex = 12;
            this.ComboBox_MPType.SelectedIndexChanged += new System.EventHandler(this.ComboBox_MPType_SelectedIndexChanged);
            // 
            // ComboBox_Key
            // 
            this.ComboBox_Key.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBox_Key.FormattingEnabled = true;
            this.ComboBox_Key.Items.AddRange(new object[] {
            "Base",
            "Extra 1",
            "Extra 2"});
            this.ComboBox_Key.Location = new System.Drawing.Point(238, 269);
            this.ComboBox_Key.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ComboBox_Key.Name = "ComboBox_Key";
            this.ComboBox_Key.Size = new System.Drawing.Size(156, 23);
            this.ComboBox_Key.TabIndex = 14;
            this.ComboBox_Key.SelectedIndexChanged += new System.EventHandler(this.ComboBox_Key_SelectedIndexChanged);
            // 
            // BtnPeek
            // 
            this.BtnPeek.Location = new System.Drawing.Point(168, 383);
            this.BtnPeek.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BtnPeek.Name = "BtnPeek";
            this.BtnPeek.Size = new System.Drawing.Size(156, 27);
            this.BtnPeek.TabIndex = 16;
            this.BtnPeek.Text = "Peek At";
            this.BtnPeek.UseVisualStyleBackColor = true;
            this.BtnPeek.Click += new System.EventHandler(this.BtnPeek_Click);
            // 
            // BtnReplaceImage
            // 
            this.BtnReplaceImage.Location = new System.Drawing.Point(14, 267);
            this.BtnReplaceImage.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BtnReplaceImage.Name = "BtnReplaceImage";
            this.BtnReplaceImage.Size = new System.Drawing.Size(147, 27);
            this.BtnReplaceImage.TabIndex = 17;
            this.BtnReplaceImage.Text = "Replace Map Image";
            this.BtnReplaceImage.UseVisualStyleBackColor = true;
            this.BtnReplaceImage.Click += new System.EventHandler(this.BtnReplaceImage_Click);
            // 
            // BtnLoadEx2Bbas
            // 
            this.BtnLoadEx2Bbas.Location = new System.Drawing.Point(331, 14);
            this.BtnLoadEx2Bbas.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BtnLoadEx2Bbas.Name = "BtnLoadEx2Bbas";
            this.BtnLoadEx2Bbas.Size = new System.Drawing.Size(155, 27);
            this.BtnLoadEx2Bbas.TabIndex = 18;
            this.BtnLoadEx2Bbas.Text = "Load Extra2 Bbas";
            this.BtnLoadEx2Bbas.UseVisualStyleBackColor = true;
            this.BtnLoadEx2Bbas.Click += new System.EventHandler(this.BtnLoadEx2Bbas_Click);
            // 
            // BtnSort
            // 
            this.BtnSort.Location = new System.Drawing.Point(331, 100);
            this.BtnSort.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BtnSort.Name = "BtnSort";
            this.BtnSort.Size = new System.Drawing.Size(155, 27);
            this.BtnSort.TabIndex = 19;
            this.BtnSort.Text = "Sort by Name";
            this.BtnSort.UseVisualStyleBackColor = true;
            this.BtnSort.Click += new System.EventHandler(this.BtnSort_Click);
            // 
            // BtnLoadFolder
            // 
            this.BtnLoadFolder.Location = new System.Drawing.Point(14, 134);
            this.BtnLoadFolder.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BtnLoadFolder.Name = "BtnLoadFolder";
            this.BtnLoadFolder.Size = new System.Drawing.Size(147, 27);
            this.BtnLoadFolder.TabIndex = 20;
            this.BtnLoadFolder.Text = "Load Folder";
            this.BtnLoadFolder.UseVisualStyleBackColor = true;
            this.BtnLoadFolder.Click += new System.EventHandler(this.BtnLoadFolder_Click);
            // 
            // BtnSaveFolder
            // 
            this.BtnSaveFolder.Location = new System.Drawing.Point(168, 134);
            this.BtnSaveFolder.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BtnSaveFolder.Name = "BtnSaveFolder";
            this.BtnSaveFolder.Size = new System.Drawing.Size(156, 27);
            this.BtnSaveFolder.TabIndex = 21;
            this.BtnSaveFolder.Text = "Export as Folder";
            this.BtnSaveFolder.UseVisualStyleBackColor = true;
            this.BtnSaveFolder.Click += new System.EventHandler(this.BtnSaveFolder_Click);
            // 
            // BtnImportFolderMap
            // 
            this.BtnImportFolderMap.Location = new System.Drawing.Point(14, 167);
            this.BtnImportFolderMap.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BtnImportFolderMap.Name = "BtnImportFolderMap";
            this.BtnImportFolderMap.Size = new System.Drawing.Size(147, 27);
            this.BtnImportFolderMap.TabIndex = 22;
            this.BtnImportFolderMap.Text = "Import Folder Map";
            this.BtnImportFolderMap.UseVisualStyleBackColor = true;
            this.BtnImportFolderMap.Click += new System.EventHandler(this.BtnImportFolderMap_Click);
            // 
            // PicBoxPreviewImg
            // 
            this.PicBoxPreviewImg.Location = new System.Drawing.Point(14, 417);
            this.PicBoxPreviewImg.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.PicBoxPreviewImg.Name = "PicBoxPreviewImg";
            this.PicBoxPreviewImg.Size = new System.Drawing.Size(222, 202);
            this.PicBoxPreviewImg.TabIndex = 23;
            this.PicBoxPreviewImg.TabStop = false;
            // 
            // BtnPackScript
            // 
            this.BtnPackScript.Location = new System.Drawing.Point(331, 383);
            this.BtnPackScript.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BtnPackScript.Name = "BtnPackScript";
            this.BtnPackScript.Size = new System.Drawing.Size(125, 27);
            this.BtnPackScript.TabIndex = 24;
            this.BtnPackScript.Text = "PackScript";
            this.BtnPackScript.UseVisualStyleBackColor = true;
            this.BtnPackScript.Click += new System.EventHandler(this.BtnPackScript_Click);
            // 
            // BtnCompile
            // 
            this.BtnCompile.Location = new System.Drawing.Point(463, 383);
            this.BtnCompile.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BtnCompile.Name = "BtnCompile";
            this.BtnCompile.Size = new System.Drawing.Size(111, 27);
            this.BtnCompile.TabIndex = 25;
            this.BtnCompile.Text = "Compile";
            this.BtnCompile.UseVisualStyleBackColor = true;
            this.BtnCompile.Click += new System.EventHandler(this.BtnCompile_Click);
            // 
            // BtnLuaMakro
            // 
            this.BtnLuaMakro.Location = new System.Drawing.Point(331, 637);
            this.BtnLuaMakro.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.BtnLuaMakro.Name = "BtnLuaMakro";
            this.BtnLuaMakro.Size = new System.Drawing.Size(243, 27);
            this.BtnLuaMakro.TabIndex = 26;
            this.BtnLuaMakro.Text = "Execute Lua Makro";
            this.BtnLuaMakro.UseVisualStyleBackColor = true;
            this.BtnLuaMakro.Click += new System.EventHandler(this.BtnLuaMakro_Click);
            // 
            // Btn_LoadToMem
            // 
            this.Btn_LoadToMem.Location = new System.Drawing.Point(331, 134);
            this.Btn_LoadToMem.Name = "Btn_LoadToMem";
            this.Btn_LoadToMem.Size = new System.Drawing.Size(155, 27);
            this.Btn_LoadToMem.TabIndex = 27;
            this.Btn_LoadToMem.Text = "Load To Memory";
            this.Btn_LoadToMem.UseVisualStyleBackColor = true;
            this.Btn_LoadToMem.Click += new System.EventHandler(this.Btn_LoadToMem_Click);
            // 
            // CB_Compressed
            // 
            this.CB_Compressed.AutoSize = true;
            this.CB_Compressed.Location = new System.Drawing.Point(463, 417);
            this.CB_Compressed.Name = "CB_Compressed";
            this.CB_Compressed.Size = new System.Drawing.Size(92, 19);
            this.CB_Compressed.TabIndex = 28;
            this.CB_Compressed.Text = "Compressed";
            this.CB_Compressed.UseVisualStyleBackColor = true;
            this.CB_Compressed.CheckedChanged += new System.EventHandler(this.CB_Compressed_CheckedChanged);
            // 
            // BtnSearchDuplicates
            // 
            this.BtnSearchDuplicates.Location = new System.Drawing.Point(168, 167);
            this.BtnSearchDuplicates.Name = "BtnSearchDuplicates";
            this.BtnSearchDuplicates.Size = new System.Drawing.Size(156, 27);
            this.BtnSearchDuplicates.TabIndex = 29;
            this.BtnSearchDuplicates.Text = "Search Duplicates";
            this.BtnSearchDuplicates.UseVisualStyleBackColor = true;
            this.BtnSearchDuplicates.Click += new System.EventHandler(this.BtnSearchDuplicates_Click);
            // 
            // CB_AutoCompression
            // 
            this.CB_AutoCompression.AutoSize = true;
            this.CB_AutoCompression.Location = new System.Drawing.Point(331, 172);
            this.CB_AutoCompression.Name = "CB_AutoCompression";
            this.CB_AutoCompression.Size = new System.Drawing.Size(125, 19);
            this.CB_AutoCompression.TabIndex = 30;
            this.CB_AutoCompression.Text = "Auto Compression";
            this.CB_AutoCompression.UseVisualStyleBackColor = true;
            // 
            // S5xToolGUI
            // 
            this.AcceptButton = this.BtnRenameTo;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1428, 683);
            this.Controls.Add(this.CB_AutoCompression);
            this.Controls.Add(this.BtnSearchDuplicates);
            this.Controls.Add(this.CB_Compressed);
            this.Controls.Add(this.Btn_LoadToMem);
            this.Controls.Add(this.BtnLuaMakro);
            this.Controls.Add(this.BtnCompile);
            this.Controls.Add(this.BtnPackScript);
            this.Controls.Add(this.PicBoxPreviewImg);
            this.Controls.Add(this.BtnImportFolderMap);
            this.Controls.Add(this.BtnSaveFolder);
            this.Controls.Add(this.BtnLoadFolder);
            this.Controls.Add(this.BtnSort);
            this.Controls.Add(this.BtnLoadEx2Bbas);
            this.Controls.Add(this.BtnReplaceImage);
            this.Controls.Add(this.BtnPeek);
            this.Controls.Add(LBLKey);
            this.Controls.Add(this.ComboBox_Key);
            this.Controls.Add(LBLMPMode);
            this.Controls.Add(this.ComboBox_MPType);
            this.Controls.Add(this.CBRandomGUID);
            this.Controls.Add(this.BtnRandomGUID);
            this.Controls.Add(this.BtnExportFile);
            this.Controls.Add(this.BtnAddFile);
            this.Controls.Add(this.CB_AskAdd);
            this.Controls.Add(this.BtnRenameTo);
            this.Controls.Add(this.TB_Rename);
            this.Controls.Add(this.BtnRemove);
            this.Controls.Add(this.CB_LoadMerge);
            this.Controls.Add(this.BtnSave);
            this.Controls.Add(this.BtnLoad);
            this.Controls.Add(this.ListBox_Data);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "S5xToolGUI";
            this.Text = "S5xTool";
            ((System.ComponentModel.ISupportInitialize)(this.PicBoxPreviewImg)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox ListBox_Data;
        private System.Windows.Forms.Button BtnLoad;
        private System.Windows.Forms.Button BtnSave;
        private System.Windows.Forms.OpenFileDialog Dlg_Open;
        private System.Windows.Forms.CheckBox CB_LoadMerge;
        private System.Windows.Forms.Button BtnRemove;
        private System.Windows.Forms.TextBox TB_Rename;
        private System.Windows.Forms.Button BtnRenameTo;
        private System.Windows.Forms.CheckBox CB_AskAdd;
        private System.Windows.Forms.Button BtnAddFile;
        private System.Windows.Forms.Button BtnExportFile;
        private System.Windows.Forms.SaveFileDialog Dlg_Save;
        private System.Windows.Forms.Button BtnRandomGUID;
        private System.Windows.Forms.CheckBox CBRandomGUID;
        private System.Windows.Forms.ComboBox ComboBox_MPType;
        private System.Windows.Forms.ComboBox ComboBox_Key;
        private System.Windows.Forms.Button BtnPeek;
        private System.Windows.Forms.Button BtnReplaceImage;
        private System.Windows.Forms.Button BtnLoadEx2Bbas;
        private System.Windows.Forms.Button BtnSort;
        private System.Windows.Forms.Button BtnLoadFolder;
        private System.Windows.Forms.FolderBrowserDialog Dlg_Folder;
        private System.Windows.Forms.Button BtnSaveFolder;
        private System.Windows.Forms.Button BtnImportFolderMap;
        private System.Windows.Forms.PictureBox PicBoxPreviewImg;
        private System.Windows.Forms.Button BtnPackScript;
        private System.Windows.Forms.Button BtnCompile;
        private System.Windows.Forms.Button BtnLuaMakro;
        private System.Windows.Forms.Button Btn_LoadToMem;
        private System.Windows.Forms.CheckBox CB_Compressed;
        private System.Windows.Forms.Button BtnSearchDuplicates;
        private System.Windows.Forms.CheckBox CB_AutoCompression;
    }
}

