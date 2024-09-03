using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbaLib
{
    internal class BbaDirStructEntry
    {
        internal BbaOutputType Type;
        internal UInt32 Offset = 0;
        internal UInt32 Size = 0;
        //internal UInt16 NameLength;
        //internal UInt16 DirPart;
        internal Int32 FirstChild = -1;
        internal Int32 NextSibling = -1;
        internal UInt64 Timestamp;
        internal string Filename = "";

        internal BbaDirStructEntry? FirstChildLink;
        internal BbaDirStructEntry? NextSiblingLink;
        internal BbaFile? FileLink;
        internal uint OwnOffset;

        internal void WriteOffsets(BinaryWriter f)
        {
            f.Seek(16, SeekOrigin.Current);
            f.Write(FirstChild);
            f.Write(NextSibling);
        }

        internal void Write(BinaryWriter f)
        {
            f.Write((UInt32)Type);
            f.Write(Offset);
            f.Write(Size);
            byte[] filenamebytes = Encoding.UTF8.GetBytes(Filename);
            f.Write((UInt16)filenamebytes.Length);
            f.Write((UInt16)(Filename.LastIndexOf('\\')+1));
            f.Write(FirstChild);
            f.Write(NextSibling);
            f.Write(Timestamp);
            f.Write(filenamebytes);
            //if (filenamebytes.Length % 4 != 0)
            {
                int padding = (4 - filenamebytes.Length % 4);
                for (int i = 0; i < padding; i++)
                    f.Write((byte)0); 
            }
        }

        internal void Read(BinaryReader r)
        {
            Type = (BbaOutputType)r.ReadUInt32();
            Offset = r.ReadUInt32();
            Size = r.ReadUInt32();
            UInt16 l = r.ReadUInt16();
            UInt16 DirPart = r.ReadUInt16();
            FirstChild = r.ReadInt32();
            NextSibling = r.ReadInt32();
            Timestamp = r.ReadUInt64();
            Filename = Encoding.UTF8.GetString(r.ReadBytes(l));
            //if (l % 4 != 0)
                r.ReadBytes(4 - l % 4);
        }

        internal void AddSibling(BbaDirStructEntry a)
        {
            if (NextSiblingLink != null)
            {
                NextSiblingLink.AddSibling(a);
                return;
            }
            NextSiblingLink = a;
        }

        internal void AddChild(BbaDirStructEntry a)
        {
            if (FirstChildLink != null)
            {
                FirstChildLink.AddSibling(a);
                return;
            }
            FirstChildLink = a;
        }

        internal BbaDirStructEntry? GetSibling(string name)
        {
            if (name.Equals(Filename))
                return this;
            if (NextSiblingLink != null)
                return NextSiblingLink.GetSibling(name);
            return null;
        }

        internal BbaDirStructEntry? GetChild(string name)
        {
            if (FirstChildLink != null)
                return FirstChildLink.GetSibling(name);
            return null;
        }
    }
}
