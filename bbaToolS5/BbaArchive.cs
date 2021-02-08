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
            RemoveFile(f.InternalPath);
            Contents.Add(f);
            f.SetCompressByExtension();
        }

        public void AddFileFromFilesystem(string path, string internalpath)
        {
            AddFile(new BbaFileFromFilesystem()
            {
                InternalPath = internalpath.ToLower(),
                SourceFilePath = path
            });
        }

        public void AddFileFromMem(byte[] data, string internalPath)
        {
            AddFile(new BbaFileFromMem()
            {
                InternalPath = internalPath.ToLower(),
                Data = data
            });
        }

        public void RemoveFile(string intName)
        {
            int i = Contents.FindIndex((x) => x.InternalPath.Equals(intName));
            if (i >= 0)
            {
                Contents[i].Remove();
                Contents.RemoveAt(i);
            }
        }

        public BbaFile GetFileByName(string name)
        {
            return Contents.FirstOrDefault((x) => x.InternalPath.Equals(name));
        }

        public void WriteToBba(string file, Action<ProgressStatus> prog = null)
        {
            BbaWriter.WriteBba(this, file, prog);
        }

        public void ReadBba(string file, Func<string, bool> shouldAdd = null, Action<ProgressStatus> prog = null)
        {
            BbaReader.ReadBba(file, this, shouldAdd, prog);
        }

        public void ReadBba(string file, string internalFile)
        {
            ReadBba(file, (x) => x.Equals(internalFile), null);
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

        public void ReadFromFolder(string folder, Action<ProgressStatus> prog = null)
        {
            if (!Directory.Exists(folder))
                return;
            if (prog == null)
                prog = (X) => { };
            ProgressStatus stat = new ProgressStatus();
            stat.Step = ProgressStatusStep.ReadFolder_File;
            stat.Progress = 0;
            DirectoryInfo d = new DirectoryInfo(folder);
            ReadFromFolder(d, "", prog, stat);
        }

        private void ReadFromFolder(DirectoryInfo d, string inter, Action<ProgressStatus> prog, ProgressStatus stat)
        {
            foreach (FileInfo i in d.GetFiles())
            {
                string internalpath = Path.Combine(inter, i.Name);
                AddFileFromFilesystem(i.FullName, internalpath);
                stat.AdditionalString = internalpath;
                prog(stat);
            }
            foreach (DirectoryInfo d2 in d.GetDirectories())
            {
                ReadFromFolder(d2, Path.Combine(inter, d2.Name), prog, stat);
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
