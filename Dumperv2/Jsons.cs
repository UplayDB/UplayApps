namespace Dumperv2
{
    internal class Jsons
    {
        public class prodconf
        {
            public uint ProductId { get; set; }
            public string Configuration { get; set; }
        }

        public class storeconf
        {
            public uint ProductId { get; set; }
            public string StoreRef { get; set; }
            public Uplay.Store.StorePartner Partner { get; set; }
        }
        public class idmap
        {
            public uint ProductId { get; set; }
            public string Brand { get; set; }
        }

        public class OW
        {
            public uint ProductId { get; set; }
            public string ProductType { get; set; }
            public string State { get; set; }
            public string TargetPartner { get; set; }
            public List<uint> ProductAssociations { get; set; }
            public List<uint> ActivationIds { get; set; }
        }
    }
}
