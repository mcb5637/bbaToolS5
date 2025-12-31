using System.Xml;

namespace bbaLib
{
    public class S5MapInfo
    {
        public string? NameKey, DescKey, Name, Desc;
        public int SizeX, SizeY;
        public bool MPFlag;
        public int MPPlayerCount;
        public int MPGameOptionFlagSet;
        public string MiniMapTextureName = "";
        public int[] Key = [];
        public string GUID = "";
        public string? UpdateURL, VersionURL;
        
        public static S5MapInfo? FromXML(Stream s)
        {
            XmlDocument doc = new();
            doc.Load(s);
            var root = doc.DocumentElement;
            if (root == null)
                return null;

            S5MapInfo r = new()
            {
                NameKey = SingleNodeS(root, "NameKey"),
                DescKey = SingleNodeS(root, "DescKey"),
                Name = SingleNodeS(root, "Name"),
                Desc = SingleNodeS(root, "Desc"),
                SizeX =  SingleNodeI(root, "SizeX"),
                SizeY =  SingleNodeI(root, "SizeY"),
                MPFlag = SingleNodeB(root, "MPFlag"),
                MPPlayerCount = SingleNodeI(root, "MPPlayerCount"),
                MPGameOptionFlagSet =  SingleNodeI(root, "MPGameOptionFlagSet"),
                MiniMapTextureName = SingleNodeS(root, "MiniMapTextureName") ?? "",
                Key = [],
                GUID = root.SelectSingleNode("GUID")?.SelectSingleNode("Data")?.InnerText ?? "",
                UpdateURL = SingleNodeS(root, "UpdateURL"),
                VersionURL = SingleNodeS(root, "VersionURL"),
            };
            var k = root.SelectNodes("Key");
            if (k != null)
            {
                r.Key = new int[k.Count];
                for (int i = 0; i < k.Count; ++i)
                {
                    r.Key[i] = int.Parse(k[i]?.InnerText ?? "0");
                }
            }
            return r;

            string? SingleNodeS(XmlNode n, string key)
            {
                return n.SelectSingleNode(key)?.InnerText;
            }
            int SingleNodeI(XmlNode n, string key)
            {
                var c = SingleNodeS(n, key);
                if (c == null)
                    return 0;
                return int.Parse(c);
            }
            bool SingleNodeB(XmlNode n, string key)
            {
                var c = SingleNodeS(n, key);
                if (c == null)
                    return false;
                return bool.Parse(c);
            }
        }

        public void ToXML(Stream s)
        {
            XmlDocument doc = new();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", null));
            XmlElement root = doc.CreateElement("root");
            root.SetAttribute("noNamespaceSchemaLocation", "http://www.w3.org/2001/XMLSchema-instance", 
                "https://raw.githubusercontent.com/mcb5637/s5xmlschema/master/info.xsd");
            doc.AppendChild(root);

            MakeChild(root, "NameKey", NameKey);
            MakeChild(root, "DescKey", DescKey);
            MakeChild(root, "Name", Name);
            MakeChild(root, "Desc", Desc);
            MakeChild(root, "SizeX", SizeX.ToString());
            MakeChild(root, "SizeY", SizeY.ToString());
            MakeChild(root, "MPFlag", MPFlag.ToString().ToLower());
            MakeChild(root, "MPPlayerCount", MPPlayerCount.ToString());
            MakeChild(root, "MPGameOptionFlagSet", MPGameOptionFlagSet.ToString());
            MakeChild(root, "MiniMapTextureName", MiniMapTextureName);
            foreach (int k in Key)
                MakeChild(root, "Key", k.ToString());
            XmlElement guid = doc.CreateElement("GUID");
            MakeChild(guid, "Data", GUID);
            root.AppendChild(guid);
            MakeChild(root, "UpdateURL", UpdateURL);
            MakeChild(root, "VersionURL", VersionURL);
            
            
            doc.Save(s);

            void MakeChild(XmlNode parent, string name, string? content)
            {
                if (content == null)
                    return;
                XmlElement n = doc.CreateElement(name);
                if (content == "")
                    n.IsEmpty = true;
                else
                    n.InnerText = content;
                parent.AppendChild(n);
            }
        }
    }

    public class S5ModPackInfo
    {
        public string LoaderPath = "";
        public string ScriptPath = "";
        public string? MainmenuPath;
        public string Version = "";
        public string? Description;
        public string[] Required = [];
        public string[] Incompatible = [];
        public string[] Override = [];
        public bool DataMod = false, ScriptMod = false, MainmenuMod = false, KeepArchive = false, UserRequestable = false, ScriptLib = false;
        public string? UpdateURL, VersionURL;
        
        public static S5ModPackInfo? FromXML(Stream s)
        {
            XmlDocument doc = new();
            doc.Load(s);
            var root = doc.DocumentElement;
            if (root == null)
                return null;

            S5ModPackInfo r = new()
            {
                LoaderPath = SingleNodeS(root, "LoaderPath") ?? "",
                ScriptPath = SingleNodeS(root, "ScriptPath") ?? "",
                MainmenuPath = SingleNodeS(root, "MainmenuPath"),
                Version = SingleNodeS(root, "Version") ?? "",
                Description = SingleNodeS(root, "Description"),
                DataMod = SingleNodeB(root, "DataMod"),
                ScriptMod = SingleNodeB(root, "ScriptMod"),
                MainmenuMod = SingleNodeB(root, "MainmenuMod"),
                KeepArchive = SingleNodeB(root, "KeepArchive"),
                UserRequestable = SingleNodeB(root, "UserRequestable"),
                ScriptLib = SingleNodeB(root, "ScriptLib"),
                UpdateURL = SingleNodeS(root, "UpdateURL"),
                VersionURL =  SingleNodeS(root, "VersionURL"),
            };
            var k = root.SelectNodes("Required");
            if (k != null)
            {
                r.Required = new string[k.Count];
                for (int i = 0; i < k.Count; ++i)
                {
                    r.Required[i] = k[i]?.InnerText ?? "";
                }
            }
            k = root.SelectNodes("Incompatible");
            if (k != null)
            {
                r.Incompatible = new string[k.Count];
                for (int i = 0; i < k.Count; ++i)
                {
                    r.Incompatible[i] = k[i]?.InnerText ?? "";
                }
            }
            k = root.SelectNodes("Override");
            if (k != null)
            {
                r.Override = new string[k.Count];
                for (int i = 0; i < k.Count; ++i)
                {
                    r.Override[i] = k[i]?.InnerText ?? "";
                }
            }

            return r;
            
            string? SingleNodeS(XmlNode n, string key)
            {
                return n.SelectSingleNode(key)?.InnerText;
            }
            bool SingleNodeB(XmlNode n, string key)
            {
                var c = SingleNodeS(n, key);
                if (c == null)
                    return false;
                return bool.Parse(c);
            }
        }

        public void ToXML(Stream s)
        {
            XmlDocument doc = new();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", null));
            XmlElement root = doc.CreateElement("root");
            root.SetAttribute("noNamespaceSchemaLocation", "http://www.w3.org/2001/XMLSchema-instance", 
                "https://raw.githubusercontent.com/mcb5637/s5xmlschema/master/modpack.xsd");
            doc.AppendChild(root);
            
            MakeChild(root, "LoaderPath", LoaderPath);
            MakeChild(root, "ScriptPath", ScriptPath);
            MakeChild(root, "MainmenuPath", MainmenuPath);
            MakeChild(root, "Version", Version);
            MakeChild(root, "Description", Description);
            foreach (var e in  Required)
                MakeChild(root, "Required", e);
            foreach (var e in  Incompatible)
                MakeChild(root, "Incompatible", e);
            foreach (var e in  Override)
                MakeChild(root, "Override", e);
            MakeChild(root, "DataMod",  DataMod.ToString().ToLower());
            MakeChild(root, "ScriptMod",  ScriptMod.ToString().ToLower());
            MakeChild(root, "MainmenuMod", MainmenuMod.ToString().ToLower());
            MakeChild(root, "KeepArchive", KeepArchive.ToString().ToLower());
            MakeChild(root, "UserRequestable", UserRequestable.ToString().ToLower());
            MakeChild(root, "ScriptLib", ScriptLib.ToString().ToLower());
            MakeChild(root, "UpdateURL", UpdateURL);
            MakeChild(root, "VersionURL", VersionURL);
            
            doc.Save(s);

            void MakeChild(XmlNode parent, string name, string? content)
            {
                if (content == null)
                    return;
                XmlElement n = doc.CreateElement(name);
                if (content == "")
                    n.IsEmpty = true;
                else
                    n.InnerText = content;
                parent.AppendChild(n);
            }
        }
    }
}
