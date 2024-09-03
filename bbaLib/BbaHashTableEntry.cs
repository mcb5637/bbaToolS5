using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbaLib
{
    internal class BbaHashTableEntry
    {
        internal UInt32 HashValue;
        internal UInt32 DirOffset;

        internal void Read(BinaryReader r)
        {
            HashValue = r.ReadUInt32();
            DirOffset = r.ReadUInt32();
        }

        internal void Write(BinaryWriter w)
        {
            w.Write(HashValue);
            w.Write(DirOffset);
        }

        internal static void WriteNull(BinaryWriter w)
        {
            w.Write((UInt32)0);
            w.Write((UInt32)0);
        }

        internal static int Size => 8;
    }
}
