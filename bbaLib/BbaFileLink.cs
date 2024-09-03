using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbaToolS5
{
    // Note: BbaArchive.RemoveFile implementation assumes that BbaFileLink.Remove() is empty
    internal class BbaFileLink : BbaFile
    {
        public BbaFile Linked { get; internal set; }

        internal override long PosWrittenTo { get => Linked.PosWrittenTo; set { throw new NotSupportedException(); } }
        internal override uint WrittenSize { get => Linked.WrittenSize; set { throw new NotSupportedException(); } }
        public override bool ShouldCompess { get => Linked.ShouldCompess; set => Linked.ShouldCompess = value; }

        public override byte[] GetBytes()
        {
            return Linked.GetBytes();
        }

        public override Stream GetStream()
        {
            return Linked.GetStream();
        }

        internal override BbaFile Clone()
        {
            return new BbaFileLink()
            {
                Linked = Linked,
                InternalPath = InternalPath,
            };
        }

        public override string ToString()
        {
            return $"{InternalPath} <- Ref {Linked}";
        }
    }
}
