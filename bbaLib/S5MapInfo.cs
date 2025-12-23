using System.Xml.Schema;
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
        [XmlElement]
        public int[] Key = [];
        public InfoGUID GUID;
        public string? UpdateURL, VersionURL;
        [XmlAttribute("noNamespaceSchemaLocation", Namespace = XmlSchema.InstanceNamespace)]
        public string Schema
        {
            get => "https://raw.githubusercontent.com/mcb5637/s5xmlschema/master/info.xsd";
            // ReSharper disable once ValueParameterNotUsed
            set { }
        }
            

        public struct InfoGUID
        {
            public string Data = "";

            public InfoGUID()
            {
            }
        }
    }

    [XmlRoot("root")]
    public class S5ModPackInfo
    {
        public string LoaderPath = "";
        public string ScriptPath = "";
        public string Version = "";
        [XmlElement]
        public string[] Required = [];
        [XmlElement]
        public string[] Incompatible = [];
        [XmlElement]
        public string[] Override = [];
        public bool DataMod = false, ScriptMod = false, MainmenuMod = false, KeepArchive = false, UserRequestable = false;
        public string? UpdateURL, VersionURL;
        [XmlAttribute("noNamespaceSchemaLocation", Namespace = XmlSchema.InstanceNamespace)]
        public string Schema
        {
            get => "https://raw.githubusercontent.com/mcb5637/s5xmlschema/master/modpack.xsd";
            // ReSharper disable once ValueParameterNotUsed
            set { }
        }
    }
}
