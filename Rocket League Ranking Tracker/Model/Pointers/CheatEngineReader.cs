using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Rocket_League_Ranking_Tracker.Model.Pointers
{
    class CheatEngineReader
    {
        static private XmlTextReader reader = null;
        static Dictionary<string, string> pointers = new Dictionary<string,string>() ;

        public static void loadXML()
        {
            if (pointers.Count <= 0)  {
                reader = new XmlTextReader("RocketLeague.CT");

                while (reader.ReadToFollowing("CheatEntry"))
                {
                    reader.ReadToFollowing("Description");
                    string name = reader.ReadElementContentAsString().Trim('"');

                    reader.ReadToFollowing("Address");
                    string adress = reader.ReadElementContentAsString();
                    string offsets = "";

                    reader.ReadToFollowing("Offsets");
                    reader.Read();
                    while (reader.Read() && reader.NodeType != XmlNodeType.EndElement)
                    {
                        offsets = "+" + reader.ReadElementContentAsString() + offsets;
                    }

                    pointers.Add(name, adress + offsets);
                }
                reader.Close();
            }
        }

        public static string getPointers(string id)
        {
            loadXML();
            return pointers[id];
        }

    }
}
