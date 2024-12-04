namespace Dumperv2
{
    internal class Jsons
    {
        public class prodconf
        {
            public uint ProductId { get; set; }
            public string Configuration { get; set; } = string.Empty;
        }

        public class prodmanifests
        {
            public uint ProductId { get; set; }
            public List<string> Manifest { get; set; } = new();
        }

        public class prodserv
        {
            public uint ProductId { get; set; }
            public string SpaceId { get; set; } = string.Empty;
            public string AppId { get; set; } = string.Empty;
        }

        public class storeconf
        {
            public uint ProductId { get; set; }
            public string StoreRef { get; set; } = string.Empty;
            public Uplay.Store.StorePartner Partner { get; set; }
        }
        public class idmap
        {
            public uint ProductId { get; set; }
            public string Brand { get; set; } = string.Empty;
        }

        public class OW
        {
            public uint ProductId { get; set; }
            public string ProductType { get; set; } = string.Empty;
            public List<uint> ProductAssociations { get; set; } = new();
        }
    }
}
