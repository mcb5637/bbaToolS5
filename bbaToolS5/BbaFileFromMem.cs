using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbaToolS5
{
    internal class BbaFileFromMem : BbaFile
    {
        public byte[] Data;

        public override byte[] GetBytes()
        {
            return Data;
        }

        public override Stream GetStream()
        {
            return new MemoryStream(Data);
        }

        public override string ToString()
        {
            return $"{InternalPath} <- Memory block";
        }
    }
}
