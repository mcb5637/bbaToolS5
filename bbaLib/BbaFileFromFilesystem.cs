using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbaToolS5
{
    internal class BbaFileFromFilesystem : BbaFile
    {
        internal required string SourceFilePath;

        public override Stream GetStream()
        {
            return new FileStream(SourceFilePath, FileMode.Open);
        }

        public override byte[] GetBytes()
        {
            return File.ReadAllBytes(SourceFilePath);
        }

        public override string ToString()
        {
            return $"{InternalPath} <- {SourceFilePath}";
        }

        internal override BbaFile Clone()
        {
            return new BbaFileFromFilesystem()
            {
                InternalPath = InternalPath,
                SourceFilePath = SourceFilePath
            };
        }
    }
}
