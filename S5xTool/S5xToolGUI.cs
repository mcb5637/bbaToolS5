using bbaToolS5;
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
        private BbaArchive Archive;
        private bool Updating = false;

        public S5xToolGUI()
        {
            InitializeComponent();
            Archive = new BbaArchive();
            UpdateList();
        }

        private void UpdateList()
        {
            Updating = true;
            ListBox_Data.Items.Clear();
            bool setindex = false;
            foreach (BbaFile f in Archive)
            {
                ListBox_Data.Items.Add(f);
                if (f.InternalPath == InfoXML)
                {
                    XDocument doc = XDocument.Load(f.GetStream());
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
                    if (int.TryParse(doc.Root.Element("Key").Value, out int key))
                        ComboBox_Key.SelectedIndex = key;
                    else
                        ComboBox_Key.SelectedIndex = -1;
                    setindex = true;
                }
            }
            if (!setindex)
            {
                ComboBox_MPType.SelectedIndex = -1;
                ComboBox_Key.SelectedIndex = -1;
            }
            ListBox_Data_SelectedIndexChanged(null, null);
            Updating = false;
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
                UpdateList();
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
                UpdateList();
            }
        }

        private void ListBox_Data_SelectedIndexChanged(object sender, EventArgs e)
        {
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
            BtnRandomGUID.Enabled = hasInfo;
            ComboBox_MPType.Enabled = hasInfo;
            ComboBox_Key.Enabled = hasInfo;
            Updating = false;
        }

        private void BtnRenameTo_Click(object sender, EventArgs e)
        {
            if (ListBox_Data.SelectedItem is BbaFile f)
            {
                Archive.RenameFile(f.InternalPath, TB_Rename.Text);
                UpdateList();
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
            UpdateList();
        }

        private void BtnAddFile_Click(object sender, EventArgs e)
        {
            Dlg_Open.FilterIndex = 3;
            if (Dlg_Open.ShowDialog() == DialogResult.OK)
            {
                string s = Dlg_Open.FileName;
                Archive.AddFileFromFilesystem(s, Path.GetFileName(s));
                UpdateList();
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
                            f.GetStream().CopyTo(s);
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
                XDocument doc = XDocument.Load(f.GetStream());
                doc.Root.Element("GUID").Element("Data").Value = GenerateGUID();
                MemoryStream s = new MemoryStream();
                doc.Save(s);
                Archive.AddFileFromMem(s.ToArray(), InfoXML);
                UpdateList();
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
                XDocument doc = XDocument.Load(f.GetStream());
                doc.Root.Element("MPFlag").Value = mp.ToString().ToLower();
                doc.Root.Element("MPPlayerCount").Value = maxp.ToString();
                MemoryStream s = new MemoryStream();
                doc.Save(s);
                Archive.AddFileFromMem(s.ToArray(), InfoXML);
                UpdateList();
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
                XDocument doc = XDocument.Load(f.GetStream());
                doc.Root.Element("Key").Value = key.ToString();
                MemoryStream s = new MemoryStream();
                doc.Save(s);
                Archive.AddFileFromMem(s.ToArray(), InfoXML);
                UpdateList();
            }
            Updating = false;
        }
    }
}
