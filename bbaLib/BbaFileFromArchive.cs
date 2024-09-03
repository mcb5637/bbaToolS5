using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbaLib
{
    internal class BbaFileFromArchive : BbaFile
    {
        internal required string SourceFilePath;
        internal uint FileOffset;
        internal uint FileLength;
        internal bool IsCompressed;
        internal required string SourceInternalPath;

        private Stream? s;
        internal Stream? ReadFrom
        {
            get => s;
            set
            {
                StreamCache.RemoveRef(s);
                s = value;
                StreamCache.AddRef(s);
            }
        }

        public override Stream GetStream()
        {
            return new MemoryStream(GetBytes());
        }

        public override byte[] GetBytes()
        {
            if (IsCompressed)
            {
                return GetDataCompressed();
            }
            else
            {
                return GetDataUncompressed();
            }
        }

        private byte[] GetDataUncompressed()
        {
            if (s == null)
                throw new NullReferenceException("already closed");
            BinaryReader r = new(s);
            r.BaseStream.Seek(FileOffset, SeekOrigin.Begin);
            return r.ReadBytes((int)FileLength);
        }

        private byte[] GetDataCompressed()
        {
            if (s == null)
                throw new NullReferenceException("already closed");
            BinaryReader r = new(s);
            r.BaseStream.Seek(FileOffset, SeekOrigin.Begin);
            BbaCompresedFileHeader h = new();
            h.Read(r);
            byte[] data = r.ReadBytes((int)h.CompressedSize);
            return ZlibStream.UncompressBuffer(data);
        }

        public override string ToString()
        {
            return $"{InternalPath} <- {SourceFilePath}\\{SourceInternalPath}";
        }

        internal override void Remove()
        {
            StreamCache.RemoveRef(s);
            s = null;
        }

        internal override BbaFile Clone()
        {
            return new BbaFileFromArchive()
            {
                InternalPath = InternalPath,
                FileOffset = FileOffset,
                FileLength = FileLength,
                IsCompressed = IsCompressed,
                SourceInternalPath = SourceInternalPath,
                SourceFilePath = SourceFilePath,
                ReadFrom = s
            };
        }
    }
}
