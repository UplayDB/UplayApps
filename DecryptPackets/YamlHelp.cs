using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using YamlDotNet.Core.Tokens;
using System.Buffers.Text;

namespace DecryptPackets;

internal class YamlHelp
{
    internal class Peers
    {
        public int peer { get; set; }
        public string host { get; set; }
        public int port { get; set; }
    }

    internal class Packets
    {
        public int packet { get; set; }
        public int peer { get; set; }
        public int index { get; set; }
        public double timestamp { get; set; }
        public string data { get; set; }
    }


    internal class dumpedyml
    {
        public List<Peers> peers { get; set; } = new();
        public List<Packets> packets { get; set; } = new();
    }


    public static void Test(string file)
    {
        var input = new StringReader(File.ReadAllText(file));

        var deserializer = new DeserializerBuilder()
            .WithTagMapping(new YamlDotNet.Core.TagName("tag:yaml.org,2002:binary"), typeof(string))
            .Build();

        dumpedyml yml = deserializer.Deserialize<dumpedyml>(input);
        foreach (var item in yml.packets)
        {
            List<byte> bytes = new();
            if (item.data.Contains("\n"))
            {
                var splitted = item.data.Split("\n");
                foreach (var item1 in splitted)
                {
                    bytes.AddRange(Convert.FromBase64String(item1));
                }
            }
            var data = item.data.Replace("\n", string.Empty).Replace("\t", string.Empty);
            
            Console.WriteLine(data);
            if (bytes.Count == 0)
            {
                bytes = Convert.FromBase64String(data).ToList();
            }
            var force_out = Program.Force(bytes.ToArray().Skip(4).ToArray());
            Console.WriteLine(force_out);
            Console.WriteLine("end.");
        }
        
    }
}
