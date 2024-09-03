using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace bbaLib
{
    [XmlRoot("root")]
    public class S5MapInfo
    {
        public string? NameKey, DescKey, Name, Desc;
        public int SizeX, SizeY;
        public bool MPFlag;
        public int MPPlayerCount;
        public int MPGameOptionFlagSet;
        public string MiniMapTextureName = "";
        public int Key;
        public InfoGUID GUID;

        public struct InfoGUID
        {
            public string Data = "";

            public InfoGUID()
            {
            }
        }
    }
}
