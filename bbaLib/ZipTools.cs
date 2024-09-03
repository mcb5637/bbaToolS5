using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbaToolS5
{
    internal class ZipTools
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
