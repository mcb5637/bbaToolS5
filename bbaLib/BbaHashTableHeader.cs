namespace bbaLib
{
    internal class BbaHashTableHeader
    {
        private static readonly byte[] BAhHeaderGlobal = "BAh"u8.ToArray();

        internal byte[] Header = BAhHeaderGlobal;
        internal byte HeaderVersion = 2;
        internal UInt32 HashTableLength;
        internal UInt32 HashTableSize;

        internal void Read(BinaryReader r)
        {
            Header = r.ReadBytes(3);
            if (!Header.SequenceEqual(BAhHeaderGlobal))
                throw new IOException("BAh header mismatch");
            HeaderVersion = r.ReadByte();
            HashTableLength = r.ReadUInt32();
            HashTableSize = r.ReadUInt32();
        }

        internal void Write(BinaryWriter w)
        {
            w.Write(Header);
            w.Write(HeaderVersion);
            w.Write(HashTableLength);
            w.Write(HashTableSize);
        }

        internal static long Size => 12;
    }
}
