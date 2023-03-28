using Newtonsoft.Json;

namespace Tracker
{
    internal class json
    {
        public class Root
        {
            [JsonProperty("Base64Login", NullValueHandling = NullValueHandling.Ignore)]
            public string Base64Login { get; set; }

            [JsonProperty("Has2FA", NullValueHandling = NullValueHandling.Ignore)]
            public bool Has2FA { get; set; }

            [JsonProperty("RemDeviceToken", NullValueHandling = NullValueHandling.Ignore)]
            public string RemDeviceToken { get; set; }

            [JsonProperty("RemMeToken", NullValueHandling = NullValueHandling.Ignore)]
            public string RemMeToken { get; set; }

            [JsonProperty("RemName", NullValueHandling = NullValueHandling.Ignore)]
            public string RemName { get; set; }

            [JsonProperty("RemId", NullValueHandling = NullValueHandling.Ignore)]
            public string RemId { get; set; }
        }

        public class Game
        {
            [JsonProperty("ProductId", NullValueHandling = NullValueHandling.Ignore)]
            public uint ProductId { get; set; }

            [JsonProperty("ConfigVersion", NullValueHandling = NullValueHandling.Ignore)]
            public uint ConfigVersion { get; set; }

            [JsonProperty("DownloadId", NullValueHandling = NullValueHandling.Ignore)]
            public uint DownloadId { get; set; }

            [JsonProperty("DownloadVersion", NullValueHandling = NullValueHandling.Ignore)]
            public uint DownloadVersion { get; set; }


            [JsonProperty("State", NullValueHandling = NullValueHandling.Ignore)]
            public uint State { get; set; }

            [JsonProperty("Configuration", NullValueHandling = NullValueHandling.Ignore)]
            public string Configuration { get; set; }

            [JsonProperty("UplayId", NullValueHandling = NullValueHandling.Ignore)]
            public uint UplayId { get; set; }

            [JsonProperty("LatestManifest", NullValueHandling = NullValueHandling.Ignore)]
            public string LatestManifest { get; set; }

            [JsonProperty("UbiservicesSpaceId", NullValueHandling = NullValueHandling.Ignore)]
            public string UbiservicesSpaceId { get; set; }

            [JsonProperty("UbiservicesAppId", NullValueHandling = NullValueHandling.Ignore)]
            public string UbiservicesAppId { get; set; }

            [JsonProperty("TargetPartner", NullValueHandling = NullValueHandling.Ignore)]
            public Uplay.Ownership.OwnedGame.Types.TargetPartner TargetPartner { get; set; }

            [JsonProperty("UbiservicesDynamicConfig", NullValueHandling = NullValueHandling.Ignore)]
            public UbiservicesDynamicConfig UbiservicesDynamicConfig { get; set; }
        }

        public class UbiservicesDynamicConfig
        {
            [JsonProperty("LunaAppId", NullValueHandling = NullValueHandling.Ignore)]
            public string LunaAppId { get; set; }

            [JsonProperty("GfnAppId", NullValueHandling = NullValueHandling.Ignore)]
            public string GfnAppId { get; set; }
        }
    }
}
