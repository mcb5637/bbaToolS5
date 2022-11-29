using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbaToolS5
{
    public class BbaWriter
    {
        public static void WriteBba(BbaArchive a, string file, Action<ProgressStatus> prog = null, bool autoCompression = false)
        {
            using FileStream w = new(file, FileMode.Create, FileAccess.Write);
            WriteBba(a, w, prog, autoCompression);
        }

        public static void WriteBba(BbaArchive a, Stream w, Action<ProgressStatus> prog = null, bool autoCompression = false)
        {
            if (prog == null)
                prog = (X) => { };
            ProgressStatus stat = new()
            {
                Step = ProgressStatusStep.WriteBba_Files,
                Progress = 0
            };

            BinaryWriter wr = new BinaryWriter(w);
            long startpos = w.Position;
            // skip header, written later
            w.Seek(BbaHeader.Size, SeekOrigin.Current);

            int filesize = (int)-w.Position;
            int numFiles = a.Contents.Count;
            int currFile = 0;
            // write files
            foreach (BbaFile f in a)
            {
                if (f is not BbaFileLink)
                {
                    f.PosWrittenTo = w.Position;
                    byte[] file = f.GetBytes();
                    byte[] compressed = null;
                    if (autoCompression || f.ShouldCompess)
                        compressed = ZipTools.CompressBuffer(file);
                    if (autoCompression) {
                        if (compressed.Length + 5 * 4 < file.Length)
                            f.ShouldCompess = true;
                        else
                            f.ShouldCompess = false;
                    }

                    if (f.ShouldCompess)
                    {
                        BbaCompresedFileHeader compfile = new();
                        compfile.UncompressedSize = (uint)file.Length;
                        compfile.Adler32 = Adler.Adler32(29061971, compressed, 0, compressed.Length);
                        compfile.CompressedSize = (uint)compressed.Length;
                        compfile.DataLength = compfile.CompressedSize + 12;
                        compfile.Write(wr);
                        wr.Write(compressed);
                        f.WrittenSize = compfile.UncompressedSize;
                    }
                    else
                    {
                        wr.Write(file);
                        f.WrittenSize = (uint)(w.Position - f.PosWrittenTo);
                    }
                    stat.AdditionalString = f.InternalPath;
                }
                else
                {
                    stat.AdditionalString = f.InternalPath + " link skipped";
                }
                currFile++;
                stat.Progress = 100 * currFile / numFiles;
                prog(stat);
            }
            filesize += (int)w.Position;

            BbaDirStructEntry root = BuildStructure(a, out int Count);
            stat.Step = ProgressStatusStep.WriteBba_Directory;
            stat.Progress = 0;
            // write directories
            MemoryStream direct = new MemoryStream();
            BinaryWriter diwr = new BinaryWriter(direct);
            diwr.Write(Count);
            WriteDirectory(diwr, root, prog, stat);
            BbaDirectoryHeader dirhead = new BbaDirectoryHeader();

            byte[] data = direct.ToArray();
            dirhead.DecompressedSize = (uint)data.Length;
            data = ZipTools.CompressBuffer(data);

            dirhead.Adler32 = Adler.Adler32(29061971, data, 0, data.Length);
            dirhead.CompressedSize = (uint)data.Length;

            if (data.Length % 4 > 0)
            {
                data = data.Concat(new byte[4 - data.Length % 4]).ToArray();
            }
            dirhead.DataLength = (uint)(data.Length + 12);
            dirhead.FileEntrieLength = dirhead.DataLength + 8;
            SHoK_Crypt.Encrypt(data);

            int HashSize = GetHashTableSize(Count);

            dirhead.DirLength = (uint)(dirhead.FileEntrieLength + 8 + (HashSize * BbaHashTableEntry.Size + 4) + 8);
            dirhead.Write(wr);
            wr.Write(data);


            stat.Step = ProgressStatusStep.WriteBba_HashTable;
            stat.AdditionalString = null;


            // write hashtables
            long hashTablePos = w.Position;
            UInt32 mask = (uint)(HashSize - 1);
            BbaHashTableEntry[] hashtable = new BbaHashTableEntry[HashSize];
            WriteHashEntry(root, hashtable, mask);

            BbaHashTableHeader hashheader = new BbaHashTableHeader()
            {
                HashTableSize = (uint)HashSize,
                HashTableLength = (uint)(HashSize * BbaHashTableEntry.Size + 4)
            };
            hashheader.Write(wr);
            currFile = 0;
            foreach (BbaHashTableEntry e in hashtable)
            {
                if (e == null)
                    BbaHashTableEntry.WriteNull(wr);
                else
                    e.Write(wr);
                currFile++;
                if (currFile % 10 ==0)
                {
                    stat.Progress = 100 * currFile / HashSize;
                    prog(stat);
                }
            }

            BbaHeader header = new BbaHeader()
            {
                ArchiveLength = (uint)(dirhead.DirLength + filesize + 32),
                FileDataLength = (uint)filesize
            };
            w.Seek(startpos, SeekOrigin.Begin);
            header.Write(wr);
        }

        private static int GetHashTableSize(int entries)
        {
            int counter = entries + 1;
            int pot = 0;
            do
            {
                counter >>= 1;
                pot++;
            } while (counter != 0);
            int size = 1 << pot;
            if (entries * 1.5 > size)
                return size * 4;
            else
                return size * 2;
        }

        private static void WriteHashEntry(BbaDirStructEntry e, BbaHashTableEntry[] t, UInt32 mask)
        {
            UInt32 hash = SHoK_Hash.GetHash(e.Filename);
            UInt32 masked;
            if (t[0] == null && false)
            {
                masked = 0;
            }
            else
            {
                masked = hash & mask;
                while (t[masked] != null)
                    masked = (masked + 1) & mask;
            }
            t[masked] = new BbaHashTableEntry()
            {
                DirOffset = (uint)e.OwnOffset,
                HashValue = hash
            };
            if (e.NextSiblingLink != null)
                WriteHashEntry(e.NextSiblingLink, t, mask);
            if (e.FirstChildLink != null)
                WriteHashEntry(e.FirstChildLink, t, mask);
        }

        private static Int32 WriteDirectory(BinaryWriter w, BbaDirStructEntry e, Action<ProgressStatus> prog, ProgressStatus stat)
        {
            if (e.Type != BbaOutputType.Directory)
            {
                e.Offset = (uint)e.FileLink.PosWrittenTo;
                e.Size = e.FileLink.WrittenSize;
            }
            long pos = w.BaseStream.Position;
            e.Write(w);
            if (e.NextSiblingLink != null)
            {
                e.NextSibling = WriteDirectory(w, e.NextSiblingLink, prog, stat);
            }
            if (e.FirstChildLink != null)
            {
                e.FirstChild = WriteDirectory(w, e.FirstChildLink, prog, stat);
            }
            long end = w.BaseStream.Position;
            w.BaseStream.Seek(pos, SeekOrigin.Begin);
            e.WriteOffsets(w);
            w.BaseStream.Seek(end, SeekOrigin.Begin);
            e.OwnOffset = (uint)(pos - 4);
            stat.AdditionalString = e.Filename;
            prog(stat);
            return (int)e.OwnOffset;
        }

        private static BbaDirStructEntry BuildStructure(BbaArchive a, out int Count)
        {
            a.Contents.Sort();
            BbaDirStructEntry root = new BbaDirStructEntry()
            {
                Type = BbaOutputType.Directory,
                Timestamp = 0,
                Filename = "."
            };
            Count = 1;
            foreach (BbaFile f in a)
            {
                string[] path = f.InternalPath.Split('\\');
                BbaDirStructEntry c = root;
                string current = "";
                foreach (string p in path.Take(path.Length-1))
                {
                    current = Path.Combine(current, p);
                    BbaDirStructEntry c2 = c.GetChild(current);
                    if (c2 == null)
                    {
                        c2 = new BbaDirStructEntry()
                        {
                            Type = BbaOutputType.Directory,
                            Timestamp = 0,
                            Filename = current
                        };
                        c.AddChild(c2);
                        Count++;
                    }
                    c = c2;
                }
                c.AddChild(new BbaDirStructEntry()
                {
                    Type = f.ShouldCompess ? BbaOutputType.Compressed : BbaOutputType.Uncompressed,
                    Timestamp = 0,
                    Filename = f.InternalPath,
                    FileLink = f
                });
                Count++;
            }
            return root;
        }
    }
}
