using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Resources;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Rocket_League_Ranking_Tracker.Model.Pointers
{
    class CheatEngineReader
    {
        static private string url = @"https://api.github.com/repos/lindell/Rocket-League-Ranking-Tracker/contents/Rocket League%20Ranking%20Tracker/Model/Pointers/RocketLeague.CT";
        private static string shaFile = "latest-sha.txt";
        private static string cheatEngineTableFile = "RocketLeague.CT";
        static private XmlTextReader reader = null;
        static Dictionary<string, string> pointers = new Dictionary<string,string>() ;

        public static void loadXML()
        {
            if (pointers.Count <= 0)  {
                updateToNewTable();
                Console.WriteLine("Reading XML");

                reader = new XmlTextReader(cheatEngineTableFile);

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
            try
            {
                return pointers[id];
            }
            catch (KeyNotFoundException e)
            {
                return "";
            }
        }

        public static void updateToNewTable()
        {
            Console.WriteLine("Updating...");
            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.UserAgent = "Rocket League Ranking Tracker";
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(Response));
                    object objResponse = jsonSerializer.ReadObject(response.GetResponseStream());
                    Response jsonResponse = objResponse as Response;


                    if (jsonResponse.Sha == ShaFromFile())
                    {
                        Console.WriteLine("SHA is the same. No update needed.");
                    }
                    else
                    {
                        Console.WriteLine("SHA missmatch. Time for update!");
                        var base64EncodedBytes = System.Convert.FromBase64String(jsonResponse.Content);
                        var fileData = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);

                        ShaToFile(jsonResponse.Sha);
                        SetCheatEngineData(fileData);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed");
                Console.WriteLine(e.Message);
            }
        }

        private static void SetCheatEngineData(string data)
        {
            using (StreamWriter file = new System.IO.StreamWriter(cheatEngineTableFile, false))
            {
                file.Write(data);
            }
        }

        private static void ShaToFile(string sha)
        {
            using (StreamWriter file = new System.IO.StreamWriter(shaFile))
            {
                file.WriteLine(sha);
            }
        }

        private static string ShaFromFile()
        {
            string line;
            using (StreamReader file = new System.IO.StreamReader(shaFile))
            {
                line = file.ReadLine();
            }
            return line;
        }

        [DataContract]
        public class Response
        {
            [DataMember(Name = "sha")]
            public string Sha { get; set; }
            [DataMember(Name = "content")]
            public string Content { get; set; }
        }

    }
}
