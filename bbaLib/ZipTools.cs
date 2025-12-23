using Ionic.Zlib;

namespace bbaLib
{
    internal static class ZipTools
    {
        internal static byte[] CompressBuffer(byte[] b)
        {
            using MemoryStream ms = new();
            using (ZlibStream compressor = new(ms, CompressionMode.Compress, CompressionLevel.BestCompression))
            {
                compressor.Write(b, 0, b.Length);
            }
            return ms.ToArray();
        }
    }
}
