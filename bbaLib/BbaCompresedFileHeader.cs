namespace bbaLib
{
    internal class BbaCompresedFileHeader
    {
        internal UInt32 CompressionHeader = 0x0637f2bd;
        internal UInt32 DataLength;
        internal UInt32 CompressedSize;
        internal UInt32 UncompressedSize;
        internal UInt32 Adler32;

        internal void Read(BinaryReader r)
        {
            CompressionHeader = r.ReadUInt32();
            DataLength = r.ReadUInt32();
            CompressedSize = r.ReadUInt32();
            UncompressedSize = r.ReadUInt32();
            Adler32 = r.ReadUInt32();
        }

        internal void Write(BinaryWriter w)
        {
            w.Write(CompressionHeader);
            w.Write(DataLength);
            w.Write(CompressedSize);
            w.Write(UncompressedSize);
            w.Write(Adler32);
        }
    }
}
