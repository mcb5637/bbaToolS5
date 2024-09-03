using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbaLib
{
    public abstract class BbaFile : IComparable<BbaFile>
    {
        internal static string[] CompressedExt = [".xml", ".lua", ".bin", ".fdb", ".fx", ".txt"];
        internal static string[] NeverCompressExt = [".wav", ".mp3"];

        public string InternalPath
        {
            get;
            internal set;
        } = "";

        internal virtual long PosWrittenTo { get; set; }
        internal virtual uint WrittenSize { get; set; }
        public virtual bool ShouldCompess { get; set; }
        // needs to be read as stream from the bba => cannot be compressed (any sound file)
        public bool NeverCompress => NeverCompressExt.Contains(Path.GetExtension(InternalPath));

        public abstract Stream GetStream();
        public abstract byte[] GetBytes();

        public int CompareTo(BbaFile? other)
        {
            if (other == null)
                return 1;
            return InternalPath.CompareTo(other.InternalPath);
        }

        public void SetCompressByExtension()
        {
            string ext = Path.GetExtension(InternalPath);
            ShouldCompess = CompressedExt.Contains(ext);
        }

        internal virtual void Remove()
        {

        }

        internal abstract BbaFile Clone();
    }
}
