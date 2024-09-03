using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbaToolS5
{
    internal class BbaDirectoryHeader
    {
        private static readonly byte[] BAdHeaderGlobal = [(byte)'B', (byte)'A', (byte)'d'];
        private static readonly byte[] BAeHeaderGlobal = [(byte)'B', (byte)'A', (byte)'e'];

        internal byte[] DirHeader = BAdHeaderGlobal;
        internal byte DirVersion = 2;
        internal UInt32 DirLength;
        internal byte[] FileEntriesHeader = BAeHeaderGlobal;
        internal byte FileEntriesVersion = 2;
        internal UInt32 FileEntrieLength;

        internal UInt32 CompressionHeader = 0x0637f2bd;
        internal UInt32 DataLength;
        internal UInt32 CompressedSize;
        internal UInt32 DecompressedSize;
        internal UInt32 Adler32;

        internal void Read(BinaryReader r)
        {
            DirHeader = r.ReadBytes(3);
            if (!DirHeader.SequenceEqual(BAdHeaderGlobal))
                throw new IOException("BAd header mismatch");
            DirVersion = r.ReadByte();
            DirLength = r.ReadUInt32();
            FileEntriesHeader = r.ReadBytes(3);
            if (!FileEntriesHeader.SequenceEqual(BAeHeaderGlobal))
                throw new IOException("BAe header mismatch");
            FileEntriesVersion = r.ReadByte();
            FileEntrieLength = r.ReadUInt32();
            CompressionHeader = r.ReadUInt32();
            DataLength = r.ReadUInt32();
            CompressedSize = r.ReadUInt32();
            DecompressedSize = r.ReadUInt32();
            Adler32 = r.ReadUInt32();
        }

        internal void Write(BinaryWriter w)
        {
            w.Write(DirHeader);
            w.Write(DirVersion);
            w.Write(DirLength);
            w.Write(FileEntriesHeader);
            w.Write(FileEntriesVersion);
            w.Write(FileEntrieLength);
            w.Write(CompressionHeader);
            w.Write(DataLength);
            w.Write(CompressedSize);
            w.Write(DecompressedSize);
            w.Write(Adler32);
        }
    }
}
