using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbaToolS5
{
    public class BbaArchive : IEnumerable<BbaFile>
    {
        internal List<BbaFile> Contents = new List<BbaFile>();

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
            BbaFile prev = GetFileByName(f.InternalPath);
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

        public void AddFileFromFilesystem(string path, string internalpath)
        {
            AddFile(new BbaFileFromFilesystem()
            {
                InternalPath = FixPath(internalpath),
                SourceFilePath = path
            });
        }

        public void AddFileFromMem(byte[] data, string internalPath)
        {
            AddFile(new BbaFileFromMem()
            {
                InternalPath = FixPath(internalPath),
                Data = data
            });
        }

        public void AddFileLink(string path, BbaFile to)
        {
            if (to == null)
                throw new ArgumentNullException(nameof(to));
            if (!Contents.Contains(to))
                throw new ArgumentException("to has to be in the archive");
            AddFile(new BbaFileLink()
            {
                InternalPath = FixPath(path),
                Linked = to,
            });
        }

        public void AddFileLink(string path, string original)
        {
            AddFileLink(path, GetFileByName(original));
        }

        public void SearchAndLinkDuplicates()
        {
            Dictionary<BbaFile, BbaFile> duplicates = new();
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
            int i = Contents.FindIndex((x) => x.InternalPath.Equals(intName));
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
            int i = Contents.FindIndex((x) => x.InternalPath.Equals(currName));
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

        public BbaFile GetFileByName(string name)
        {
            return Contents.FirstOrDefault((x) => x.InternalPath.Equals(name));
        }

        public void LoadToMemory(Func<BbaFile, bool> select = null)
        {
            if (select == null)
                select = (BbaFile f) => true;
            BbaFile f = get();
            while (f != null) {
                AddFileFromMem(f.GetBytes(), f.InternalPath);
                f = get();
            }

            BbaFile get()
            {
                return Contents.FirstOrDefault((BbaFile fi) => !(fi is BbaFileFromMem || fi is BbaFileLink) && select(fi)); ;
            }
        }

        public void WriteToBba(string file, Action<ProgressStatus> prog = null, bool autoCompression = false)
        {
            file = Path.GetFullPath(file);
            LoadToMemory((BbaFile f) => f is BbaFileFromArchive a && a.SourceFilePath == file);
            BbaWriter.WriteBba(this, file, prog, autoCompression);
        }

        public void ReadBba(string file, Func<string, bool> shouldAdd = null, Action<ProgressStatus> prog = null)
        {
            file = Path.GetFullPath(file);
            BbaReader.ReadBba(file, this, shouldAdd, prog);
        }

        public void ReadBba(string file, string internalFile)
        {
            ReadBba(file, (string x) => x.Equals(internalFile), null);
        }

        public void WriteToFolder(string folder, Action<ProgressStatus> prog = null)
        {
            if (prog == null)
                prog = (X) => { };
            if (Directory.Exists(folder))
                Directory.Delete(folder, true);
            int total = Contents.Count;
            int current = 0;
            ProgressStatus stat = new ProgressStatus();
            stat.Step = ProgressStatusStep.WriteFolder_File;
            foreach (BbaFile f in this)
            {
                string path = Path.Combine(folder, f.InternalPath);
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                using (FileStream w = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    using (Stream s = f.GetStream())
                    {
                        s.CopyTo(w);
                    }
                }
                current++;
                stat.Progress = 100 * current / total;
                stat.AdditionalString = f.InternalPath;
                prog(stat);
            }
        }

        public void ReadFromFolder(string folder, Action<ProgressStatus> prog = null, bool ignoreHidden = false, string internalbase = "", Func<string, bool> shouldAdd = null)
        {
            if (!Directory.Exists(folder))
                return;
            if (prog == null)
                prog = (X) => { };
            if (shouldAdd == null)
                shouldAdd = (x) => true;
            ProgressStatus stat = new ProgressStatus();
            stat.Step = ProgressStatusStep.ReadFolder_File;
            stat.Progress = 0;
            DirectoryInfo d = new DirectoryInfo(folder);
            ReadFromFolder(d, internalbase, prog, stat, ignoreHidden, shouldAdd);
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
    }
}
