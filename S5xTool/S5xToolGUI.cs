﻿using bbaToolS5;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace S5xTool
{
    public partial class S5xToolGUI : Form
    {
        private const string InfoXML = "maps\\externalmap\\info.xml";
        private const string ExternalMapMain = "graphics\\textures\\gui\\mappics\\externalmap.png";
        private const string ExternalMapLow = "graphics\\textureslow\\gui\\mappics\\externalmap.png";
        private const string ExternalMapMed = "graphics\\texturesmed\\gui\\mappics\\externalmap.png";
        private BbaArchive Archive;
        private bool Updating = false;
        private readonly FilePeek peek = new FilePeek();

        public S5xToolGUI()
        {
            InitializeComponent();
            Archive = new BbaArchive();
            UpdateList(false, -1);
        }

        private void UpdateList(bool selectLast, int toSelect)
        {
            Updating = true;
            ListBox_Data.Items.Clear();
            bool setindex = false;
            bool setPic = false;
            foreach (BbaFile f in Archive)
            {
                ListBox_Data.Items.Add(f);
                if (f.InternalPath == InfoXML)
                {
                    using (Stream stream = f.GetStream())
                    {
                        XDocument doc = XDocument.Load(stream);
                        if (bool.TryParse(doc.Root.Element("MPFlag").Value, out bool mp) && mp)
                        {
                            if (int.TryParse(doc.Root.Element("MPPlayerCount").Value, out int mpcount))
                            {
                                ComboBox_MPType.SelectedIndex = mpcount;
                            }
                            else
                            {
                                ComboBox_MPType.SelectedIndex = 1;
                            }
                        }
                        else
                        {
                            ComboBox_MPType.SelectedIndex = 0;
                        }
                        if (int.TryParse(doc.Root.Element("Key")?.Value, out int key))
                            ComboBox_Key.SelectedIndex = key;
                        else
                            ComboBox_Key.SelectedIndex = -1;
                        setindex = true; 
                    }
                }
                else if (f.InternalPath == ExternalMapMain)
                {
                    using (Stream stream = f.GetStream())
                    {
                        PicBoxPreviewImg.Image = Image.FromStream(stream);
                        setPic = true;
                    }
                }
            }
            if (!setindex)
            {
                ComboBox_MPType.SelectedIndex = -1;
                ComboBox_Key.SelectedIndex = -1;
            }
            if (!setPic)
                PicBoxPreviewImg.Image = null;
            if (selectLast)
                ListBox_Data.SelectedIndex = ListBox_Data.Items.Count - 1;
            else if (toSelect > 0)
                ListBox_Data.SelectedIndex = toSelect;
            Updating = false;
            ListBox_Data_SelectedIndexChanged(null, null);
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            Dlg_Open.FilterIndex = 1;
            if (Dlg_Open.ShowDialog() == DialogResult.OK)
            {
                if (!CB_LoadMerge.Checked)
                    Archive.Clear();
                try
                {
                    Func<string, bool> ch = null;
                    if (CB_AskAdd.Checked)
                        ch = AskForAdd;
                    Archive.ReadBba(Dlg_Open.FileName, ch);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                UpdateList(false, -1);
            }
        }

        private bool AskForAdd(string file)
        {
            return MessageBox.Show(file, "Add?", MessageBoxButtons.YesNo) == DialogResult.Yes;
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            if (ListBox_Data.SelectedItem is BbaFile f)
            {
                Archive.RemoveFile(f.InternalPath);
                UpdateList(false, -1);
            }
        }

        private void ListBox_Data_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            Updating = true;
            bool enable;
            if (ListBox_Data.SelectedItem is BbaFile f)
            {
                TB_Rename.Text = f.InternalPath;
                enable = true;
            }
            else
            {
                TB_Rename.Text = "";
                enable = false;
            }
            bool hasInfo = Archive.GetFileByName(InfoXML) != null;
            BtnRenameTo.Enabled = enable;
            BtnRemove.Enabled = enable;
            BtnExportFile.Enabled = enable;
            BtnPeek.Enabled = enable;
            BtnRandomGUID.Enabled = hasInfo;
            ComboBox_MPType.Enabled = hasInfo;
            ComboBox_Key.Enabled = hasInfo;
            Updating = false;
        }

        private void BtnRenameTo_Click(object sender, EventArgs e)
        {
            if (ListBox_Data.SelectedItem is BbaFile f)
            {
                int s = ListBox_Data.SelectedIndex;
                Archive.RenameFile(f.InternalPath, TB_Rename.Text);
                UpdateList(false, s);
            }
        }

        private void ListBox_Data_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void ListBox_Data_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[]) e.Data.GetData(DataFormats.FileDrop);
            foreach (string s in files)
                Archive.AddFileFromFilesystem(s, Path.GetFileName(s));
            UpdateList(true, -1);
        }

        private void BtnAddFile_Click(object sender, EventArgs e)
        {
            Dlg_Open.FilterIndex = 3;
            if (Dlg_Open.ShowDialog() == DialogResult.OK)
            {
                string s = Dlg_Open.FileName;
                Archive.AddFileFromFilesystem(s, Path.GetFileName(s));
                UpdateList(true, -1);
            }
        }

        private void BtnExportFile_Click(object sender, EventArgs e)
        {
            if (ListBox_Data.SelectedItem is BbaFile f)
            {
                Dlg_Save.FilterIndex = 3;
                Dlg_Save.FileName = Path.GetFileName(f.InternalPath);
                if (Dlg_Save.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (Stream s = new FileStream(Dlg_Save.FileName, FileMode.Create, FileAccess.Write))
                        {
                            using (Stream i = f.GetStream())
                            {
                                i.CopyTo(s);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            Dlg_Save.FilterIndex = 1;
            Dlg_Save.FileName = "";
            if (Dlg_Save.ShowDialog() == DialogResult.OK)
            {
                if (CBRandomGUID.Checked)
                    BtnRandomGUID_Click(null, null);
                try
                {
                    Archive.WriteToBba(Dlg_Save.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void BtnRandomGUID_Click(object sender, EventArgs e)
        {
            BbaFile f = Archive.GetFileByName(InfoXML);
            if (f != null)
            {
                using (Stream st = f.GetStream())
                {
                    XDocument doc = XDocument.Load(st);
                    doc.Root.Element("GUID").Element("Data").Value = GenerateGUID();
                    MemoryStream s = new MemoryStream();
                    doc.Save(s);
                    Archive.AddFileFromMem(s.ToArray(), InfoXML);
                    UpdateList(sender != null, -1);
                }
            }
        }

        private static string GenerateGUID()
        {
            return Guid.NewGuid().ToString();
        }

        private void ComboBox_MPType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            if (ComboBox_MPType.SelectedItem == null)
                return;
            bool mp = (string)ComboBox_MPType.SelectedItem != "SP";
            int maxp = 1;
            if (mp)
                maxp = ComboBox_MPType.SelectedIndex;
            Updating = true;
            BbaFile f = Archive.GetFileByName(InfoXML);
            if (f != null)
            {
                using (Stream st = f.GetStream())
                {
                    XDocument doc = XDocument.Load(st);
                    doc.Root.Element("MPFlag").Value = mp.ToString().ToLower();
                    doc.Root.Element("MPPlayerCount").Value = maxp.ToString();
                    MemoryStream s = new MemoryStream();
                    doc.Save(s);
                    Archive.AddFileFromMem(s.ToArray(), InfoXML);
                    UpdateList(true, -1);
                }
            }
            Updating = false;
        }

        private void ComboBox_Key_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            if (ComboBox_Key.SelectedItem == null)
                return;
            int key = ComboBox_Key.SelectedIndex;
            Updating = true;
            BbaFile f = Archive.GetFileByName(InfoXML);
            if (f != null)
            {
                using (Stream st = f.GetStream())
                {
                    XDocument doc = XDocument.Load(st);
                    if (doc.Root.Element("Key")==null)
                    {
                        doc.Root.Add(new XElement("Key", key.ToString()));
                    }
                    else
                    {
                        doc.Root.Element("Key").Value = key.ToString();
                    }
                    MemoryStream s = new MemoryStream();
                    doc.Save(s);
                    Archive.AddFileFromMem(s.ToArray(), InfoXML);
                    UpdateList(true, -1);
                }
            }
            Updating = false;
        }

        private void BtnPeek_Click(object sender, EventArgs e)
        {
            if (ListBox_Data.SelectedItem is BbaFile f)
            {
                using (Stream st = f.GetStream())
                {
                    StreamReader re = new StreamReader(st);
                    peek.ShowFilePeek(re.ReadToEnd());
                }
            }
        }

        private void BtnReplaceImage_Click(object sender, EventArgs e)
        {
            Dlg_Open.FilterIndex = 4;
            if (Dlg_Open.ShowDialog() == DialogResult.OK)
            {
                ReplaceImage(Dlg_Open.FileName);
                UpdateList(false, -1);
            }
        }

        private void ReplaceImage(string n)
        {
            Archive.AddFileFromFilesystem(n, ExternalMapMain);
            BbaFile f = Archive.GetFileByName(ExternalMapMain);
            Archive.CopyFile(f, ExternalMapMed);
            Archive.CopyFile(f, ExternalMapLow);
        }

        private void BtnLoadEx2Bbas_Click(object sender, EventArgs e)
        {
            try
            {
                string p = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Blue Byte\\The Settlers - Heritage of Kings", "InstallPath", null) as string;
                if (p == null)
                {
                    MessageBox.Show("registry not set", "error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                Archive.Clear();
                Archive.ReadBba(Path.Combine(p, "base\\data.bba"));
                Archive.ReadBba(Path.Combine(p, "base\\lang.bba"));
                Archive.ReadBba(Path.Combine(p, "extra2\\bba\\patch.bba"));
                Archive.ReadBba(Path.Combine(p, "extra2\\bba\\lang.bba"));
                Archive.ReadBba(Path.Combine(p, "extra2\\bba\\data.bba"));
                Archive.ReadBba(Path.Combine(p, "extra2\\bba\\patche2.bba"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            UpdateList(false, -1);
        }

        private void BtnSort_Click(object sender, EventArgs e)
        {
            Archive.SortFiles();
            UpdateList(false, -1);
        }

        private void BtnLoadFolder_Click(object sender, EventArgs e)
        {
            if (Dlg_Folder.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (!CB_LoadMerge.Checked)
                        Archive.Clear();
                    Archive.ReadFromFolder(Dlg_Folder.SelectedPath);
                    UpdateList(false, -1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void BtnSaveFolder_Click(object sender, EventArgs e)
        {
            if (Dlg_Folder.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Archive.WriteToFolder(Dlg_Folder.SelectedPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void BtnImportFolderMap_Click(object sender, EventArgs e)
        {
            Dlg_Open.FilterIndex = 5;
            if (Dlg_Open.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (!CB_LoadMerge.Checked)
                        Archive.Clear();
                    string x = "maps\\externalmap\\";
                    string extmap = Path.GetFileName(Path.GetDirectoryName(Dlg_Open.FileName)) + ".png";
                    //MessageBox.Show(dirname);
                    foreach (string f in Directory.GetFiles(Path.GetDirectoryName(Dlg_Open.FileName)))
                    {
                        string n = Path.GetFileName(f);
                        if (n==extmap)
                        {
                            ReplaceImage(f);
                        }
                        else
                        {
                            Archive.AddFileFromFilesystem(f, x + n);
                        }
                    }
                    UpdateList(false, -1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
    }
}
