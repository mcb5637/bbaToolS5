using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbaToolS5
{
    public abstract class BbaFile : IComparable<BbaFile>
    {
        internal static string[] CompressedExt = new string[] { ".xml", ".lua", ".bin", ".fdb", ".fx", ".txt" };

        public string InternalPath
        {
            get;
            internal set;
        }

        internal virtual long PosWrittenTo { get; set; }
        internal virtual uint WrittenSize { get; set; }
        public virtual bool ShouldCompess { get; set; }

        public abstract Stream GetStream();
        public abstract byte[] GetBytes();

        public int CompareTo(BbaFile other)
        {
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
