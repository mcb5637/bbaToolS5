using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbaToolS5
{
    internal class BbaHeader
    {
        private static readonly byte[] BAFHeaderGlobal = new byte[] { (byte)'B', (byte)'A', (byte)'F' };
        private static readonly byte[] BAHHeaderGlobal = new byte[] { (byte)'B', (byte)'A', (byte)'H' };
        private static readonly byte[] BAfHeaderGlobal = new byte[] { (byte)'B', (byte)'A', (byte)'f' };

        internal byte[] Header = BAFHeaderGlobal;
        internal byte ArchiveVersion = 2;
        internal UInt32 ArchiveLength;

        internal byte[] BAHHeader = BAHHeaderGlobal;
        internal byte BAHVersion = 2;
        internal UInt32 BAHLength = 8;
        internal UInt32 UnknownField = 3;
        internal UInt32 GameVersion = 1;

        internal byte[] FileDataHeader = BAfHeaderGlobal;
        internal byte FileDataVersion = 2;
        internal UInt32 FileDataLength;

        internal void Read(BinaryReader r)
        {
            Header = r.ReadBytes(3);
            if (!Header.SequenceEqual(BAFHeaderGlobal))
                throw new IOException("BAF header mismatch");
            ArchiveVersion = r.ReadByte();
            ArchiveLength = r.ReadUInt32();
            BAHHeader = r.ReadBytes(3);
            if (!BAHHeader.SequenceEqual(BAHHeaderGlobal))
                throw new IOException("BAH header mismatch");
            BAHVersion = r.ReadByte();
            BAHLength = r.ReadUInt32();
            UnknownField = r.ReadUInt32();
            GameVersion = r.ReadUInt32();
            FileDataHeader = r.ReadBytes(3);
            if (!FileDataHeader.SequenceEqual(BAfHeaderGlobal))
                throw new IOException("BAf header mismatch");
            FileDataVersion = r.ReadByte();
            FileDataLength = r.ReadUInt32();
        }

        internal void Write(BinaryWriter w)
        {
            w.Write(Header);
            w.Write(ArchiveVersion);
            w.Write(ArchiveLength);
            w.Write(BAHHeader);
            w.Write(BAHVersion);
            w.Write(BAHLength);
            w.Write(UnknownField);
            w.Write(GameVersion);
            w.Write(FileDataHeader);
            w.Write(FileDataVersion);
            w.Write(FileDataLength);
        }

        internal static long Size => 32;
    }
}
