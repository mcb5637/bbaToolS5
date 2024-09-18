using bbaLib;
using LuaSharp;
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

#pragma warning disable CA1416 // Validate platform compatibility
namespace S5xTool
{
    public partial class S5xToolGUI : Form
    {
        private const string InfoXML = "maps\\externalmap\\info.xml";
        private BbaArchive Archive;
        private bool Updating = false;
        private readonly FilePeek peek = new();
        private LuaState L = new LuaState50();

        public S5xToolGUI()
        {
            InitializeComponent();
            Archive = new BbaArchive();
            UpdateList(false, -1);
            AddLuaFuncs();
        }

        private void UpdateList(bool selectLast, int toSelect)
        {
            Updating = true;
            ListBox_Data.Items.Clear();
            foreach (BbaFile f in Archive)
            {
                ListBox_Data.Items.Add(f);
            }
            S5MapInfo inf = Archive.MapInfo;
            if (inf != null)
            {
                if (inf.MPFlag)
                    ComboBox_MPType.SelectedIndex = inf.MPPlayerCount;
                else
                    ComboBox_MPType.SelectedIndex = 0;
                ComboBox_Key.SelectedIndex = inf.Key.FirstOrDefault();
            }
            else
            {
                ComboBox_MPType.SelectedIndex = -1;
                ComboBox_Key.SelectedIndex = -1;
            }
            using Stream stream = Archive.MinimapTexture;
            if (stream != null)
            {
                PicBoxPreviewImg.Image = Image.FromStream(stream);
            }
            else
            {
                PicBoxPreviewImg.Image = null;
            }
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
                CB_Compressed.Enabled = true;
                CB_Compressed.Checked = f.ShouldCompess;
            }
            else
            {
                TB_Rename.Text = "";
                enable = false;
                CB_Compressed.Enabled = false;
                CB_Compressed.Checked = false;
            }
            bool hasInfo = Archive.GetFileByName(BbaArchive.InfoXML) != null;
            BtnRenameTo.Enabled = enable;
            BtnRemove.Enabled = enable;
            BtnExportFile.Enabled = enable;
            BtnPeek.Enabled = enable;
            BtnRandomGUID.Enabled = hasInfo;
            ComboBox_MPType.Enabled = hasInfo;
            ComboBox_Key.Enabled = hasInfo;
            BtnPackScript.Enabled = enable && TB_Rename.Text.EndsWith(".lua");
            BtnCompile.Enabled = enable && TB_Rename.Text.EndsWith(".lua");
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
                        using Stream s = new FileStream(Dlg_Save.FileName, FileMode.Create, FileAccess.Write);
                        using Stream i = f.GetStream();
                        i.CopyTo(s);
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
                    Archive.WriteToBba(Dlg_Save.FileName, null, CB_AutoCompression.Checked);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                UpdateList(false, ListBox_Data.SelectedIndex);
            }
        }

        private void BtnRandomGUID_Click(object sender, EventArgs e)
        {
            try
            {
                if (SetGUID(Archive, GenerateGUID()))
                    UpdateList(sender != null, -1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private bool SetGUID(BbaArchive ar, string guid)
        {
            S5MapInfo i = ar.MapInfo;
            if (i != null)
            {
                i.GUID.Data = guid;
                ar.MapInfo = i;
                return true;
            }
            return false;
        }
        private bool SetNameAndText(BbaArchive ar, string name, string text)
        {
            S5MapInfo i = ar.MapInfo;
            if (i != null)
            {
                i.Name = name;
                i.Desc = text;
                ar.MapInfo = i;
                return true;
            }
            return false;
        }
        private string GetNameAndText(BbaArchive ar, out string name)
        {
            S5MapInfo i = ar.MapInfo;
            if (i != null)
            {
                name = i.Name;
                return i.Desc;
            }
            name = null;
            return null;
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
            S5MapInfo i = Archive.MapInfo;
            if (i != null)
            {
                i.MPFlag = mp;
                i.MPPlayerCount = maxp;
                Archive.MapInfo = i;
                UpdateList(true, -1);
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
            S5MapInfo i = Archive.MapInfo;
            if (i != null)
            {
                i.Key = [key];
                Archive.MapInfo = i;
                UpdateList(true, -1);
            }
            Updating = false;
        }

        private void BtnPeek_Click(object sender, EventArgs e)
        {
            if (ListBox_Data.SelectedItem is BbaFile f)
            {
                using Stream st = f.GetStream();
                StreamReader re = new(st);
                peek.ShowFilePeek(re.ReadToEnd());
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
            Archive.SetMinimapTextureFromFilesystem(n);
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
                    ImportFolderMap(Archive, Dlg_Open.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void ImportFolderMap(BbaArchive a, string path)
        {
            a.AddFileFromFilesystem(path, BbaArchive.InfoXML);
            S5MapInfo i = a.MapInfo;
            if (i == null)
            {
                MessageBox.Show("mapinfo seems invalid");
                UpdateList(false, -1);
                return;
            }
            

            string externalmap = "maps\\externalmap\\";
            string folder = Path.GetDirectoryName(path);
            string mappreview = i.MiniMapTextureName;
            string maptexturefolder = "maps\\user\\" + Path.GetFileName(folder) + "\\";
            if (mappreview.StartsWith(maptexturefolder))
            {
                mappreview = mappreview.Replace(maptexturefolder, "");
                mappreview = Path.Combine(folder, mappreview);
                mappreview = Path.ChangeExtension(mappreview, "png");
            }
            else
            {
                MessageBox.Show("map preview not found (the path in the info.xml is invalid). please add one via Replace Map Image.");
            }
            a.ReadFromFolder(folder, null, true, externalmap, (n) => n != mappreview && n != path);
            if (File.Exists(mappreview))
            {
                ReplaceImage(mappreview);
            }
            else
            {
                MessageBox.Show("map preview not found (file does not exist). please add one via Replace Map Image.");
            }
            i.MiniMapTextureName = "data\\graphics\\Textures\\GUI\\MapPics\\externalmap";
            a.MapInfo = i;
            UpdateList(false, -1);
        }

        private void BtnPackScript_Click(object sender, EventArgs e)
        {
            if (ListBox_Data.SelectedItem is BbaFile f)
            {
                new ScriptPacker().ShowPacker(Archive, f.InternalPath, L);
                UpdateList(false, -1);
            }
        }

        private void BtnCompile_Click(object sender, EventArgs e)
        {
            if (ListBox_Data.SelectedItem is BbaFile f)
            {
                try
                {
                    byte[] nd = ScriptPacker.CompileFile(L, f.GetBytes(), f.InternalPath);
                    Archive.AddFileFromMem(nd, f.InternalPath);
                    UpdateList(false, ListBox_Data.SelectedIndex);
                }
                catch (LuaException er)
                {
                    MessageBox.Show(er.Message);
                }
            }
        }

        private void AddLuaFuncs()
        {
            L.PrepareUserDataType<ArchiveAccess>();
            L.Push("NewArchive");
            L.Push((s) =>
            {
                s.PushObjectAsUserdata(new ArchiveAccess());
                return 1;
            });
            L.SetTable(L.GLOBALSINDEX);
            L.Push("SetArchiveForUI");
            L.Push((s) =>
            {
                BbaArchive a = s.CheckUserdata<ArchiveAccess>(1).A;
                if (a != Archive)
                {
                    Archive.Clear(); // speed up GC
                    Archive = a;
                }
                UpdateList(false, -1);
                return 0;
            });
            L.SetTable(L.GLOBALSINDEX);
            L.Push("GetArchiveForUI");
            L.Push((s) =>
            {
                s.PushObjectAsUserdata(new ArchiveAccess(Archive));
                return 1;
            });
            L.SetTable(L.GLOBALSINDEX);
            L.Push("GenerateGUID");
            L.Push((s) =>
            {
                s.Push(GenerateGUID());
                return 1;
            });
            L.SetTable(L.GLOBALSINDEX);
            L.Push("MapFileSetGUID");
            L.Push((s) =>
            {
                BbaArchive a = s.CheckUserdata<ArchiveAccess>(1).A;
                if (!SetGUID(a, s.ToString(2)))
                    throw new LuaException("no info.xml found");
                return 0;
            });
            L.SetTable(L.GLOBALSINDEX);
            L.Push("MapFileGetNameAndText");
            L.Push((s) =>
            {
                BbaArchive a = s.CheckUserdata<ArchiveAccess>(1).A;
                string n, t;
                t = GetNameAndText(a, out n);
                if (n != null)
                {
                    s.Push(n);
                    s.Push(t);
                }
                else
                {
                    s.Push();
                    s.Push();
                }
                return 2;
            });
            L.SetTable(L.GLOBALSINDEX);
            L.Push("MapFileSetNameAndText");
            L.Push((s) =>
            {
                BbaArchive a = s.CheckUserdata<ArchiveAccess>(1).A;
                if (!SetNameAndText(a, s.ToString(2), s.ToString(3)))
                    throw new LuaException("no info.xml found");
                return 0;
            });
            L.SetTable(L.GLOBALSINDEX);
            L.Push("PackLuaScript");
            L.Push((s) =>
            {
                BbaArchive a = s.CheckUserdata<ArchiveAccess>(1).A;
                string ofile = s.ToString(2);
                string ifile = s.ToString(3);
                string log = "";
                List<string> path = new();
                List<bool> isarch = new();
                foreach (int i in s.IPairs(4))
                {
                    s.Push("Path");
                    s.GetTableRaw(-2);
                    path.Add(s.ToString(-1));
                    s.Pop(1);
                    s.Push("InArchive");
                    s.GetTableRaw(-2);
                    isarch.Add(s.ToBoolean(-1));
                    s.Pop(1);
                }
                bool copy = s.ToBoolean(5);
                bool addloader = s.ToBoolean(6);
                bool compile = s.ToBoolean(7);
                s.Top = 0;
                ScriptPacker.ProcessScript(a, ofile, ifile, path.ToArray(), isarch.ToArray(), copy, addloader, compile, s, ref log);

                s.Push(log);
                return 1;
            });
            L.SetTable(L.GLOBALSINDEX);
            L.Push("ImportFolderMap");
            L.Push((s) =>
            {
                BbaArchive a = s.CheckUserdata<ArchiveAccess>(1).A;
                string info = s.ToString(2);
                ImportFolderMap(a, info);
                return 0;
            });
            L.SetTable(L.GLOBALSINDEX);
            if (Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Blue Byte\\The Settlers - Heritage of Kings", "InstallPath", null) is string r)
            {
                L.Push("S5InstallPath");
                L.Push(r);
                L.SetTable(L.GLOBALSINDEX);
            }
            L.Push("ExternalmapPath");
            L.NewTable();
            L.Push("Path");
            L.Push("maps\\externalmap\\");
            L.SetTable(-3);
            L.Push("InArchive");
            L.Push(true);
            L.SetTable(-3);
            L.SetTable(L.GLOBALSINDEX);
        }

        private void BtnLuaMakro_Click(object sender, EventArgs e)
        {
            Dlg_Open.FilterIndex = 6;
            if (Dlg_Open.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    L.Push("MakroFile");
                    L.Push(Dlg_Open.FileName);
                    L.SetTable(L.GLOBALSINDEX);
                    string s = File.ReadAllText(Dlg_Open.FileName);
                    L.LoadBuffer(s, Path.GetFileName(Dlg_Open.FileName));
                    L.PCall(0, 0);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void Btn_LoadToMem_Click(object sender, EventArgs e)
        {
            Archive.LoadToMemory();
            UpdateList(false, ListBox_Data.SelectedIndex);
        }

        private void CB_Compressed_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            if (ListBox_Data.SelectedItem is BbaFile f)
                f.ShouldCompess = CB_Compressed.Checked;
        }

        private void BtnSearchDuplicates_Click(object sender, EventArgs e)
        {
            Archive.SearchAndLinkDuplicates();
            UpdateList(false, ListBox_Data.SelectedIndex);
        }
    }
}
#pragma warning restore CA1416 // Validate platform compatibility
