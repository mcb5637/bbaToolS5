using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace bbaLib
{
    public class BbaArchive : IEnumerable<BbaFile>, IDisposable
    {
        public const string InfoXML = "maps\\externalmap\\info.xml";
        public const string ExternalMapMain = "graphics\\textures\\gui\\mappics\\externalmap.png";
        public const string ExternalMapLow = "graphics\\textureslow\\gui\\mappics\\externalmap.png";
        public const string ExternalMapMed = "graphics\\texturesmed\\gui\\mappics\\externalmap.png";

        internal List<BbaFile> Contents = [];

        public IEnumerator<BbaFile> GetEnumerator()
        {
            return Contents.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Contents).GetEnumerator();
        }

        internal void AddFile(BbaFile f)
        {
            BbaFile? prev = GetFileByName(f.InternalPath);
            if (prev != null)
            {
                foreach (BbaFile c in Contents)
                {
                    if (c is BbaFileLink fl && fl.Linked == prev)
                        fl.Linked = f;
                }
            }
            RemoveFile(f.InternalPath);
            Contents.Add(f);
            f.SetCompressByExtension();
        }

        public BbaFile AddFileFromFilesystem(string path, string internalpath)
        {
            BbaFileFromFilesystem f = new()
            {
                InternalPath = FixPath(internalpath),
                SourceFilePath = path
            };
            AddFile(f);
            return f;
        }

        public BbaFile AddFileFromMem(byte[] data, string internalPath)
        {
            BbaFileFromMem f = new()
            {
                InternalPath = FixPath(internalPath),
                Data = data
            };
            AddFile(f);
            return f;
        }

        public BbaFile AddFileLink(string path, BbaFile to)
        {
            ArgumentNullException.ThrowIfNull(to);
            if (!Contents.Contains(to))
                throw new ArgumentException("to has to be in the archive");
            BbaFileLink f = new()
            {
                InternalPath = FixPath(path),
                Linked = to,
            };
            AddFile(f);
            return f;
        }

        public void AddFileLink(string path, string original)
        {
            BbaFile? to = GetFileByName(original) ?? throw new ArgumentException($"{original} does not exist");
            AddFileLink(path, to);
        }

        public void SearchAndLinkDuplicates()
        {
            Dictionary<BbaFile, BbaFile> duplicates = [];
            for (int i = 0; i < Contents.Count; i++)
            {
                if (Contents[i] is BbaFileLink || duplicates.ContainsKey(Contents[i]))
                    continue;
                for (int j = i+1; j < Contents.Count; j++)
                {
                    if (Contents[j] is BbaFileLink || duplicates.ContainsKey(Contents[j]))
                        continue;
                    if (Contents[i].GetBytes().SequenceEqual(Contents[j].GetBytes()))
                        duplicates[Contents[j]] = Contents[i];
                }
            }
            foreach (var dup in duplicates)
            {
                AddFileLink(dup.Key.InternalPath, dup.Value);
            }
        }

        public void RemoveFile(string intName)
        {
            int i = Contents.FindIndex((x) => x.InternalPath.Equals(intName, StringComparison.OrdinalIgnoreCase));
            if (i >= 0)
            {
                Contents.RemoveAll((BbaFile c) => c is BbaFileLink fl && fl.Linked == Contents[i]);

                Contents[i].Remove();
                Contents.RemoveAt(i);
            }
        }

        public void CopyFile(BbaFile f, string intName)
        {
            BbaFile copy = f.Clone();
            copy.InternalPath = FixPath(intName);
            AddFile(copy);
        }

        public bool RenameFile(string currName, string newName)
        {
            newName = FixPath(newName);
            RemoveFile(newName);
            int i = Contents.FindIndex((x) => x.InternalPath.Equals(currName, StringComparison.OrdinalIgnoreCase));
            if (i >= 0)
            {
                Contents[i].InternalPath = newName;
                return true;
            }
            return false;
        }

        public void SortFiles()
        {
            Contents.Sort();
        }

        private static string FixPath(string path)
        {
            return path.ToLower().Replace("/", "\\");
        }

        public BbaFile? GetFileByName(string name)
        {
            name = FixPath(name);
            return Contents.FirstOrDefault((x) => x.InternalPath.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public void LoadToMemory(Func<BbaFile, bool>? select = null)
        {
            if (select == null)
                select = (BbaFile f) => true;
            BbaFile? f = get();
            while (f != null) {
                AddFileFromMem(f.GetBytes(), f.InternalPath);
                f = get();
            }

            BbaFile? get()
            {
                return Contents.FirstOrDefault((BbaFile fi) => !(fi is BbaFileFromMem || fi is BbaFileLink) && select(fi)); ;
            }
        }

        public void WriteToBba(string file, Action<ProgressStatus>? prog, bool autoCompression, Func<string, bool?>? over)
        {
            if (over == null)
                over = (X) => false;
            file = Path.GetFullPath(file);
            if (File.Exists(file))
            {
                bool? o = over(file);
                if (o == true)
                {
                    LoadToMem();
                }
                else if (o == false)
                {
                    ReadBba(file, (i) => GetFileByName(i) == null, null);
                    LoadToMem();
                }
                else
                {
                    return;
                }
            }
            BbaWriter.WriteBba(this, file, prog, autoCompression);

            void LoadToMem()
            {
                LoadToMemory((BbaFile f) => f is BbaFileFromArchive a && a.SourceFilePath == file);
            }
        }

        public void ReadBba(string file, Func<string, bool>? shouldAdd = null, Action<ProgressStatus>? prog = null)
        {
            file = Path.GetFullPath(file);
            BbaReader.ReadBba(file, this, shouldAdd, prog);
        }

        public void ReadBba(string file, string internalFile)
        {
            ReadBba(file, (string x) => x.Equals(internalFile, StringComparison.OrdinalIgnoreCase), null);
        }

        public void WriteToFolder(string folder, Action<ProgressStatus>? prog, Func<string, bool?>? over)
        {
            if (prog == null)
                prog = (X) => { };
            if (over == null)
                over = (X) => false;
            if (Directory.Exists(folder))
            {
                bool? o = over(folder);
                if (o == true)
                    Directory.Delete(folder, true);
                else if (o == null)
                    return;
            }
            int total = Contents.Count;
            int current = 0;
            ProgressStatus stat = new()
            {
                Step = ProgressStatusStep.WriteFolder_File
            };
            bool remFL = false;
            if (GetFileByName(FileLinksFile) == null)
            {
                byte[]? fl = CreateFileLinks();
                if (fl != null)
                {
                    remFL = true;
                    AddFileFromMem(fl, FileLinksFile);
                }
            }
            foreach (BbaFile f in this)
            {
                string path = Path.Combine(folder, f.InternalPath);
                path = path.Replace("\\", Path.DirectorySeparatorChar.ToString());
                Directory.CreateDirectory(Path.GetDirectoryName(path) ?? throw new ArgumentException("somehow path got messed up"));
                using (FileStream w = new(path, FileMode.Create, FileAccess.Write))
                {
                    using Stream s = f.GetStream();
                    s.CopyTo(w);
                }
                current++;
                stat.Progress = 100 * current / total;
                stat.AdditionalString = f.InternalPath;
                prog(stat);
            }
            if (remFL)
            {
                RemoveFile(FileLinksFile);
            }
        }

        public void ReadFromFolder(string folder, Action<ProgressStatus>? prog = null, bool ignoreHidden = false, string internalbase = "", Func<string, bool>? shouldAdd = null)
        {
            if (!Directory.Exists(folder))
                return;
            if (prog == null)
                prog = (X) => { };
            if (shouldAdd == null)
                shouldAdd = (x) => true;
            ProgressStatus stat = new()
            {
                Step = ProgressStatusStep.ReadFolder_File,
                Progress = 0
            };
            DirectoryInfo d = new(folder);
            ReadFromFolder(d, internalbase, prog, stat, ignoreHidden, shouldAdd);
            BbaFile? filel = GetFileByName(FileLinksFile);
            if (filel != null)
            {
                ResolveFileLinks(filel.GetBytes());
                RemoveFile(FileLinksFile);
            }
        }

        private void ReadFromFolder(DirectoryInfo d, string inter, Action<ProgressStatus> prog, ProgressStatus stat, bool ignorehidden, Func<string, bool> shouldAdd)
        {
            foreach (FileInfo i in d.GetFiles())
            {
                if (ignorehidden && (i.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                    continue;
                if (!shouldAdd(i.FullName))
                    continue;
                string internalpath = Path.Combine(inter, i.Name);
                AddFileFromFilesystem(i.FullName, internalpath);
                stat.AdditionalString = internalpath;
                prog(stat);
            }
            foreach (DirectoryInfo d2 in d.GetDirectories())
            {
                if (ignorehidden && (d2.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                    continue;
                ReadFromFolder(d2, Path.Combine(inter, d2.Name), prog, stat, ignorehidden, shouldAdd);
            }
        }

        public void Clear()
        {
            foreach (BbaFile f in this)
                f.Remove();
            Contents.Clear();
        }

        ~BbaArchive()
        {
            Clear();
        }

        public void Dispose()
        {
            Clear();
        }

        private byte[]? CreateFileLinks()
        {
            List<FileLink> l = [];
            foreach (BbaFile f in this)
            {
                if (f is BbaFileLink lf)
                {
                    l.Add(new FileLink()
                    {
                        From = lf.Linked.InternalPath,
                        To = lf.InternalPath,
                    });
                }
            }
            if (l.Count == 0)
                return null;
            return JsonSerializer.SerializeToUtf8Bytes(l, SourceGenerationContext.Default.ListFileLink);
        }
        private void ResolveFileLinks(byte[] links)
        {
            List<FileLink>? l = JsonSerializer.Deserialize(links, SourceGenerationContext.Default.ListFileLink);
            if (l == null)
                return;
            foreach (FileLink f in l)
            {
                AddFileLink(f.To, f.From);
            }
        }
        public const string FileLinksFile = "FileLinks.json";

        public S5MapInfo? MapInfo
        {
            get
            {
                BbaFile? f = GetFileByName(InfoXML);
                if (f == null)
                    return null;
                using Stream s = f.GetStream();
                try
                {
                    return new XmlSerializer(typeof(S5MapInfo)).Deserialize(s) as S5MapInfo;
                }
                catch (InvalidOperationException)
                {
                    return null;
                }
            }
            set
            {
                if (value == null)
                {
                    RemoveFile(InfoXML);
                    return;
                }
                using MemoryStream s = new();
                new XmlSerializer(typeof(S5MapInfo)).Serialize(s, value);
                AddFileFromMem(s.GetBuffer(), InfoXML);
            }
        }
        public Stream? MinimapTexture
        {
            get => GetFileByName(ExternalMapMain)?.GetStream();
        }
        private void SetMinimapTextureLinks(BbaFile f)
        {
            AddFileLink(ExternalMapLow, f);
            AddFileLink(ExternalMapMed, f);
        }
        public BbaFile SetMinimapTextureFromFilesystem(string file)
        {
            BbaFile f = AddFileFromFilesystem(file, ExternalMapMain);
            SetMinimapTextureLinks(f);
            return f;
        }
        public BbaFile SetMinimapTextureFromMem(byte[] b)
        {
            BbaFile f = AddFileFromMem(b, ExternalMapMain);
            SetMinimapTextureLinks(f);
            return f;
        }

        public static string ModPackXml(string modname)
        {
            return $"modpack/{modname}/modpack.xml";
        }
        public S5ModPackInfo? GetModPackInfo(string modname)
        {
            BbaFile? f = GetFileByName(ModPackXml(modname));
            if (f == null)
                return null;
            using Stream s = f.GetStream();
            try
            {
                return new XmlSerializer(typeof(S5ModPackInfo)).Deserialize(s) as S5ModPackInfo;
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }
        public void SetModPackInfo(string modname, S5ModPackInfo i)
        {
            string n = ModPackXml(modname);
            if (i == null)
            {
                RemoveFile(n);
                return;
            }
            using MemoryStream s = new();
            new XmlSerializer(typeof(S5MapInfo)).Serialize(s, i);
            AddFileFromMem(s.GetBuffer(), n);
        }
    }
    internal class FileLink
    {
        public required string From { get; set; }
        public required string To { get; set; }
    }
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(List<FileLink>))]
    internal partial class SourceGenerationContext : JsonSerializerContext
    {
    }
}
