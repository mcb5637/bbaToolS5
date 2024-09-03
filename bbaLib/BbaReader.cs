using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbaToolS5
{
    public static class BbaReader
    {
        public static BbaArchive ReadBba(string file, BbaArchive? ar = null, Func<string, bool>? shouldAdd = null, Action<ProgressStatus>? prog = null)
        {
            return ReadBba(file, new FileStream(file, FileMode.Open, FileAccess.Read), ar, shouldAdd, prog);
        }

        public static BbaArchive ReadBba(string file, Stream inp, BbaArchive? ar = null, Func<string, bool>? shouldAdd = null, Action<ProgressStatus>? prog = null)
        {
            if (ar == null)
                ar = new BbaArchive();
            if (shouldAdd == null)
                shouldAdd = (x) => true;
            if (prog == null)
                prog = (X) => { };

            ProgressStatus status = new()
            {
                Step = ProgressStatusStep.ReadBba_Header
            };

            StreamCache.AddRef(inp);

            BinaryReader r = new(inp);
            // read header
            BbaHeader fileheader = new();
            fileheader.Read(r);

            status.Progress = 100;
            status.AdditionalData = (int)fileheader.ArchiveLength;
            prog(status);
            status.AdditionalData = 0;

            // read directory
            status.Step = ProgressStatusStep.ReadBba_Directory;

            r.BaseStream.Seek(fileheader.FileDataLength, SeekOrigin.Current); // skip to directory info
            BbaDirectoryHeader dirhead = new();
            dirhead.Read(r);

            int cryptsize = (int)(dirhead.DataLength - 12);
            byte[] dirdata = r.ReadBytes(cryptsize);
            SHoK_Crypt.Decrypt(dirdata);

            byte[] data = ZlibStream.UncompressBuffer(dirdata);
            BinaryReader r2 = new(new MemoryStream(data));

            int numFiles = r2.ReadInt32();

            status.Progress = 100;
            status.AdditionalData = numFiles;
            prog(status);
            status.AdditionalData = 0;

            // read hasthable
            status.Step = ProgressStatusStep.ReadBba_HashTable;

            BbaHashTableHeader hashheader = new();
            hashheader.Read(r);

            int hashTableSize = (int)hashheader.HashTableSize;
            status.AdditionalData = hashTableSize;
            prog(status);
            status.AdditionalData = 0;

            BbaHashTableEntry hashentry = new();
            bool found = false;

            for (int i = 0; i < hashTableSize; i++)
            {
                BbaHashTableEntry hashentry2 = new();
                hashentry2.Read(r);
                if (hashentry2.HashValue != 0)
                {
                    BbaDirStructEntry e2 = ReadDirStructEntry(r2, hashentry2.DirOffset);
                    if (".".Equals(e2.Filename))
                    {
                        found = true;
                        hashentry = hashentry2;
                        break;
                    }
                }
                status.Progress = 100 * i / hashTableSize;
                prog(status);
            }
            if (!found)
                throw new IOException("no root element found");


            // load file descriptions
            status.Step = ProgressStatusStep.ReadBba_FileCatalog;

            int processedFiles = 0;
            BbaDirStructEntry e = ProcessDirStructEntry(ar, r2, hashentry.DirOffset, file, inp, shouldAdd, prog, numFiles, ref processedFiles, status);
            if (!e.Filename.Equals("."))
                throw new IOException("hashtable not pointing to root");

            StreamCache.RemoveRef(inp);

            return ar;
        }

        private static BbaDirStructEntry ProcessDirStructEntry(BbaArchive ar, BinaryReader r2, long offset, string file, Stream inp, Func<string, bool> shouldAdd,
            Action<ProgressStatus> prog, int numFiles, ref int processed, ProgressStatus status)
        {
            BbaDirStructEntry e = ReadDirStructEntry(r2, offset);
            if (e.Type != BbaOutputType.Directory)
            {
                if (shouldAdd(e.Filename))
                {
                    BbaFile? linked = ar.Contents.Find((f) => f is BbaFileFromArchive fa && fa.FileOffset == e.Offset);
                    if (linked != null)
                    {
                        ar.AddFileLink(e.Filename, linked);
                    }
                    else
                    {
                        ar.AddFile(new BbaFileFromArchive()
                        {
                            InternalPath = e.Filename,
                            SourceFilePath = file,
                            FileOffset = e.Offset,
                            FileLength = e.Size,
                            IsCompressed = e.Type == BbaOutputType.Compressed,
                            SourceInternalPath = e.Filename,
                            ReadFrom = inp
                        });
                    }
                    status.AdditionalString = e.Filename;
                    status.Progress = processed * 100 / numFiles;
                    prog(status);
                }
                processed++;
            }
            else if (e.FirstChild != -1)
            {
                processed++;
                ProcessDirStructEntry(ar, r2, e.FirstChild, file, inp, shouldAdd, prog, numFiles, ref processed, status);
            }
            if (e.NextSibling != -1)
            {
                ProcessDirStructEntry(ar, r2, e.NextSibling, file, inp, shouldAdd, prog, numFiles, ref processed, status);
            }
            return e;
        }

        private static BbaDirStructEntry ReadDirStructEntry(BinaryReader r2, long offset)
        {
            BbaDirStructEntry e = new();
            r2.BaseStream.Seek(offset + 4, SeekOrigin.Begin);
            e.Read(r2);
            return e;
        }
    }
}
