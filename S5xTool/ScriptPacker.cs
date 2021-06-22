﻿using bbaToolS5;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace S5xTool
{
    public partial class ScriptPacker : Form
    {
        private BbaArchive archive;

        public ScriptPacker()
        {
            InitializeComponent();
        }

        public void ShowPacker(BbaArchive ar, string file)
        {
            archive = ar;
            TBOutFile.Text = file;
            CBInOverride.Checked = false;
            TbInFile.Text = "";

            ShowDialog();
        }

        private void BtnInOpen_Click(object sender, EventArgs e)
        {
            if (OpenFile.ShowDialog()==DialogResult.OK)
            {
                CBInOverride.Checked = true;
                TbInFile.Text = OpenFile.FileName;
            }
        }

        private void BTNPath1_Click(object sender, EventArgs e)
        {
            if (OpenFolder.ShowDialog()==DialogResult.OK)
            {
                CBInArchive1.Checked = false;
                TBPath1.Text = OpenFolder.SelectedPath;
            }
        }

        private void BTNPath2_Click(object sender, EventArgs e)
        {

            if (OpenFolder.ShowDialog() == DialogResult.OK)
            {
                CBInArchive2.Checked = false;
                TBPath2.Text = OpenFolder.SelectedPath;
            }
        }

        private void BTNPath3_Click(object sender, EventArgs e)
        {

            if (OpenFolder.ShowDialog() == DialogResult.OK)
            {
                CBInArchive3.Checked = false;
                TBPath3.Text = OpenFolder.SelectedPath;
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnRun_Click(object sender, EventArgs e)
        {
            string outf = TBOutFile.Text.Replace("maps\\externalmap\\", "").Replace(".lua", "");
            string inf = CBInOverride.Checked ? TbInFile.Text : TBOutFile.Text;
            string log = "";
            ProcessScript(archive, outf, inf, new string[] {
                TBPath1.Text, TBPath2.Text, TBPath3.Text
            }, new bool[] {
                CBInArchive1.Checked, CBInArchive2.Checked, CBInArchive3.Checked
            }, CBCopy.Checked, CBAddLoader.Checked, CBCompile.Checked, ref log);
            if (log.Length > 0)
                MessageBox.Show(log);
            Close();
        }

        internal static void ProcessScript(BbaArchive a, string oufile, string infile, string[] paths, bool[] isarch, bool copy, bool addloader, bool compile, ref string log)
        {
            LinkedList<string> def = new LinkedList<string>();
            if (copy)
                def.AddLast("CopyToOneFile");
            if (compile)
                def.AddLast("Compile");
            string of = oufile;
            if (of == "mapscript" && addloader)
            {
                of = "mapscript_packed";
                MemoryStream ms = new MemoryStream();
                StreamWriter wr = new StreamWriter(ms);
                wr.WriteLine("-- Warning: Map script and additional map data is included as extra files in this archive. Do not save this map file from editor!");
                wr.WriteLine("-- Warnung: Mapscript und weitere Mapdaten in diesem Archiv als extra Dateien enthalten. Nicht im Editor Speichern!");
                if (copy)
                {
                    wr.WriteLine($"Script.Load(\"data/maps/externalmap/mapscript_packed.lua{(compile ? "c" : "")}\")");
                }
                else
                {
                    ProcessSingleFile(a, "s5CommunityLib/packer/devLoad", "s5CommunityLib/packer/devLoad", paths, isarch, new LinkedList<string>(), def, copy, null, compile, ref log);
                    wr.WriteLine($"Script.Load(\"data/maps/externalmap/s5CommunityLib/packer/devLoad.lua{(compile ? "c" : "")}\")");
                    wr.WriteLine($"mcbPacker.Paths = {{\"data/maps/externalmap/\", \".lua{(compile ? "c" : "")}\"}}");
                    wr.WriteLine("mcbPacker.require(\"mapscript_packed\")");
                }
                wr.Flush();
                a.AddFileFromMem(ms.ToArray(), "maps/externalmap/mapscript.lua");
            }
            ProcessSingleFile(a, of, infile, paths, isarch, new LinkedList<string>(), def, copy, null, compile, ref log);
        }

        private static void ProcessSingleFile(BbaArchive a, string oufile, string infile, string[] paths, bool[] isarch,
            LinkedList<string> loaded, LinkedList<string> defines, bool copytogether, MemoryStream copy, bool compile, ref string log)
        {
            loaded.AddLast(infile);
            MemoryStream m = (copytogether && copy!=null) ? copy : new MemoryStream();
            StreamWriter wr = new StreamWriter(m);

            using (Stream s = SearchFile(a, paths, isarch, infile))
            {
                if (s == null)
                {
                    log += $"ERROR: did not find file {infile}!\n";
                    return;
                }
                StreamReader sr = new StreamReader(s);
                string line = sr.ReadLine();
                while (line != null)
                {
                    Match req = Regex.Match(line, "^\\s*mcbPacker\\.require\\(\"(?<f>[a-zA-Z/_1-90]+)\"\\)$");
                    Match force1 = Regex.Match(line, "^\\s*mcbPacker\\.forceLoad\\(\"(?<f>[a-zA-Z/_1-90]+)\"\\)$");
                    Match force2 = Regex.Match(line, "^\\s*mcbPacker\\.forceLoad\\((?<f>.+)\\)$");
                    Match uncomment = Regex.Match(line, "^\\s*\\-\\-mcbPacker\\.uncomment\\s(?<f>.+)$");
                    Match def = Regex.Match(line, "^\\s*\\-\\-mcbPacker\\.define:(?<f>\\w+)$");
                    Match udef = Regex.Match(line, "^\\s*\\-\\-mcbPacker\\.undefine:(?<f>\\w+)$");
                    Match uncifdef = Regex.Match(line, "^\\s*\\-\\-mcbPacker\\.uncommentIfDef:(?<def>\\w+) (?<lin>.+)$");
                    Match igifdef = Regex.Match(line, "\\-\\-mcbPacker\\.ignoreIfDef:(?<def>\\w+)$");
                    Match returnifdef = Regex.Match(line, "^\\s*\\-\\-mcbPacker\\.returnIfDef:(?<def>\\w+)$");
                    Match logstr = Regex.Match(line, "^\\s*\\-\\-mcbPacker\\.log (?<f>.+)$");
                    bool writeline = true;
                    if (req.Success)
                    {
                        if (!loaded.Contains(req.Groups["f"].Value))
                        {
                            //log += $"Info: loading {req.Groups["f"].Value} from {infile}\n";
                            wr.Flush();
                            ProcessSingleFile(a, req.Groups["f"].Value, req.Groups["f"].Value, paths, isarch, loaded, defines, copytogether, m, compile, ref log);
                        }
                        if (copytogether)
                            writeline = false;
                    }
                    else if (force1.Success)
                    {
                        //log += $"Info: force load {force1.Groups["f"].Value} from {infile}\n";
                        wr.Flush();
                        ProcessSingleFile(a, force1.Groups["f"].Value, force1.Groups["f"].Value, paths, isarch, new LinkedList<string>(), defines, copytogether, null, compile, ref log);
                        if (copytogether)
                        {
                            writeline = false;
                            wr.WriteLine($"Script.Load(\"data/maps/externalmap/{force1.Groups["f"].Value}.lua{(compile ? "c" : "")}\")");
                        }
                    }
                    else if (force2.Success)
                    {
                        log += $"warning: force load {force2.Groups["f"].Value} from {infile}, cannot resolve automatically\n";
                        if (copytogether)
                        {
                            writeline = false;
                            wr.WriteLine($"Script.Load(\"data/maps/externalmap/\"..{force2.Groups["f"].Value}..\".lua{(compile ? "c" : "")}\")");
                        }
                    }
                    else if (Regex.Match(line, "\\-\\-mcbPacker\\.ignore$").Success)
                    {
                        writeline = false;
                    }
                    else if (uncomment.Success)
                    {
                        writeline = false;
                        wr.WriteLine(uncomment.Groups["f"].Value);
                    }
                    else if (def.Success)
                    {
                        writeline = false;
                        if (def.Groups["f"].Value == "CopyToOneFile" && def.Groups["f"].Value == "Compile")
                        {
                            log += $"ERROR: tried to define {def.Groups["f"].Value} from {infile}\n";
                        }
                        else
                        {
                            if (!defines.Contains(def.Groups["f"].Value))
                                defines.AddLast(def.Groups["f"].Value);
                        }
                    }
                    else if (udef.Success)
                    {
                        if (udef.Groups["f"].Value == "CopyToOneFile" && def.Groups["f"].Value == "Compile")
                        {
                            log += $"ERROR: tried to undefine {udef.Groups["f"].Value} from {infile}\n";
                        }
                        else
                        {
                            writeline = false;
                            defines.Remove(udef.Groups["f"].Value);
                        }
                    }
                    else if (uncifdef.Success)
                    {
                        writeline = false;
                        if (defines.Contains(uncifdef.Groups["def"].Value))
                            wr.WriteLine(uncifdef.Groups["lin"].Value);
                    }
                    else if (igifdef.Success && defines.Contains(igifdef.Groups["def"].Value))
                    {
                        writeline = false;
                    }
                    else if (returnifdef.Success && defines.Contains(returnifdef.Groups["def"].Value))
                    {
                        break;
                    }
                    else if (Regex.Match(line, "^\\-\\-mcbPacker\\.deprecated").Success)
                    {
                        log += $"warning: requiring deprecated file {infile}\n";
                    }
                    else if (logstr.Success)
                    {
                        log += $"Info: log {logstr.Groups["f"].Value} from {infile}\n";
                    }

                    if (writeline)
                        wr.WriteLine(line);
                    line = sr.ReadLine();
                }
            }

            wr.Flush();
            if (!(copytogether && copy != null))
            {
                byte[] data = m.ToArray();
                if (compile)
                {
                    try
                    {
                        data = CompileFile(data, oufile);
                    }
                    catch (LuaError e)
                    {
                        log += $"Error: lua compile: {e.Message}\n";
                    }
                }

                a.AddFileFromMem(data, $"maps\\externalmap\\{oufile}.lua{(compile ? "c" : "")}");
            }
        }

        private static Stream SearchFile(BbaArchive a, string[] paths, bool[] isarch, string name)
        {
            if (name.EndsWith(".lua"))
            {
                if (File.Exists(name))
                    return new FileStream(name, FileMode.Open, FileAccess.Read);
                else
                {
                    BbaFile f = a.GetFileByName(name);
                    if (f != null)
                        return f.GetStream();
                    return null;
                }
            }
            for (int i = 0; i < paths.Length; i++)
            {
                if (paths[i] == null || paths[i].Length == 0)
                    continue;
                if (isarch[i])
                {
                    BbaFile f = a.GetFileByName(paths[i] + "\\" + name + ".lua");
                    if (f != null)
                        return f.GetStream();
                }
                else
                {
                    string p = Path.Combine(paths[i], name + ".lua");
                    if (File.Exists(p))
                        return new FileStream(p, FileMode.Open, FileAccess.Read);
                }
            }
            return null;
        }

        internal static byte[] CompileFile(byte[] data, string name)
        {
            LuaState50 L = new LuaState50();
            L.LoadBuffer(data, name);
            byte[] r = L.Dump();
            L.Pop(1);
            return r;
        }
    }
}
