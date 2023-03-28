using UplayKit;
using CoreLib;
using UplayKit.Connection;
using CSharpDiscordWebhook.NET.Discord;
using Newtonsoft.Json;
using UbiServices.Records;

namespace Tracker
{
    internal class Program
    {
        public static Dictionary<string, string> Webhooks = new();
        public static DiscordWebhook hook_store = null;
        public static DiscordWebhook hook_ow = null;
        public static DiscordWebhook hook_add = null;
        public static List<Uplay.Store.StoreProduct>? StoreProducts = null;
        public static List<json.Game>? OwnedGames = null;
        public static string PrevAppId = string.Empty;
        static void Main(string[] args)
        {
            if (!File.Exists("logdata.json"))
            {
                File.WriteAllText("logdata.json", "[\r\n  {\r\n\t\"Base64Login\":\"\",\r\n\t\"Has2FA\": true,\r\n\t\"RemDeviceToken\":\"\",,\r\n\t\"RemMeToken\":\"\"\r\n\t\"RemName\":\"\",\r\n\t\"RemId\":\"\"\r\n  }\r\n]");
                Console.WriteLine("Please edit your logdata NOW!");
                return;
            }

            if (!File.Exists("webhooks.txt"))
            {
                Console.WriteLine("no webhooks exist, quit");
                return;
            }

            var lines = File.ReadAllLines("webhooks.txt");

            if (File.Exists("OwnedGamesList.json"))
                OwnedGames = JsonConvert.DeserializeObject<List<json.Game>>(File.ReadAllText("OwnedGamesList.json"));
            /*
            if (File.Exists("StoreProductsList.json"))
                StoreProducts = JsonConvert.DeserializeObject<List<Uplay.Store.StoreProduct>>(File.ReadAllText("StoreProductsList.json"));
            */
            Webhooks.Add("ownedgames", lines[0]); //ownedgames
            Webhooks.Add("store", lines[1]); //store
            Webhooks.Add("add", lines[2]); //addational infos

            hook_ow = new DiscordWebhook();
            hook_ow.Uri = new Uri(Webhooks["ownedgames"]);

            hook_store = new DiscordWebhook();
            hook_store.Uri = new Uri(Webhooks["store"]);

            hook_add = new();
            hook_add.Uri = new Uri(Webhooks["add"]);
            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromHours(1);

            var timer = new Timer((e) =>
            {
                Console.WriteLine("time!");
                Work();
            }, null, startTimeSpan, periodTimeSpan);
            for (; ; )
            {
                // add a sleep for 100 mSec to reduce CPU usage
                Thread.Sleep(100);
            }


        }

        static async void Work()
        {
            var loggedList = preLogin();
            Console.WriteLine(loggedList.Count);
            int i = 1;
            if (loggedList.Count > 0)
            {
                foreach (var item in loggedList)
                {
                    if (item != null && item.Ticket != null)
                    {
                        DoTheStuff(item,i);
                        await hook_add.SendAsync(new DiscordMessage()
                        {
                            Content = "User "+ i,
                            Username = "Hourly Update!"
                        });
                        i++;
                    }
                    
                }
            }
            await hook_add.SendAsync(new DiscordMessage()
            {
                Content = "All users done!\nWaiting for an hour!",
                Username = "Hourly Update!"
            });
        }

        static List<LoginJson> preLogin()
        {
            List<LoginJson> ret = new();
            var listlogin = JsonConvert.DeserializeObject<List<json.Root>>(File.ReadAllText("logdata.json"));
            List<string> args = new();

            for (int i = 0; i < listlogin.Count; i++)
            {
                var item = listlogin[i];
                Console.WriteLine("User logged: " + i + " " + item.Base64Login);
                args.Add("-b64");
                args.Add(item.Base64Login);
                if (item.Has2FA)
                {
                    if (!string.IsNullOrEmpty(item.RemMeToken))
                    {
                        Console.WriteLine("RemDeviceToken exist!");
                        args.Add("-ref");
                        args.Add(item.RemMeToken);
                    }
                    if (!string.IsNullOrEmpty(item.RemDeviceToken))
                    {
                        Console.WriteLine("RemDeviceToken exist!");
                        args.Add("-rem");
                        args.Add(item.RemDeviceToken);
                    }
                    if (!string.IsNullOrEmpty(item.RemName) && !string.IsNullOrEmpty(item.RemId))
                    {
                        Console.WriteLine("RemDevice exist!");
                        args.Add("-trustedname");
                        args.Add(item.RemName);
                        args.Add("-trustedid");
                        args.Add(item.RemId);
                    }
                }
                var login = LoginLib.TryLoginWithArgsCLI_RemTicket(args.ToArray(), out bool _);
                if (login == null || string.IsNullOrEmpty(login.Ticket))
                {
                    Console.WriteLine("You know what happened :( [Account is not got a ticket or something went wrong]");
                    continue;
                }

                if (!string.IsNullOrEmpty(login.RememberDeviceTicket))
                {
                    item.RemDeviceToken = login.RememberDeviceTicket;
                }
                if (!string.IsNullOrEmpty(login.RememberMeTicket))
                {
                    item.RemMeToken = login.RememberMeTicket;
                }
                ret.Add(login);
                args = new();
            }
            File.WriteAllText("logdata.json", JsonConvert.SerializeObject(listlogin, Formatting.Indented));
            return ret;
        }

        static void DoTheStuff(LoginJson login, int accountId)
        {
            DemuxSocket demuxSocket = new DemuxSocket(false);
            Console.WriteLine("Opened");
            demuxSocket.WaitInTimeMS = 2;
            demuxSocket.VersionCheck();
            demuxSocket.PushVersion();
            var atuhed = demuxSocket.Authenticate(login.Ticket);
            Console.WriteLine("authed?" + atuhed);
            var ownershipConnection = new OwnershipConnection(demuxSocket);
            ownershipConnection.PushEvent += Ownership_PushEvent;
            Console.WriteLine("y");
            try
            {
                var owtemp = ownershipConnection.GetOwnedGames(false);
                if (owtemp != null)
                {
                    Console.WriteLine("e");
                    OWCheck(owtemp, accountId);
                    File.WriteAllText($"OwnedGamesList.json", JsonConvert.SerializeObject(OwnedGames, Formatting.Indented));
                }
            }
            catch
            { }


            var storeConnection = new StoreConnection(demuxSocket);
            storeConnection.PushEvent += StoreConnection_PushEvent;
            storeConnection.Init();
            var store = storeConnection.GetStore();
            if (store.Result == Uplay.Store.StoreResult.StoreResponseSuccess)
            {
                var storetemp = store.StoreProducts.ToList();
                StoreCheck(storetemp, accountId);
                StoreProducts = storetemp;
                File.WriteAllText($"StoreProductsList.json", JsonConvert.SerializeObject(StoreProducts, Formatting.Indented));
            }

            Thread.Sleep(1000);
            storeConnection.Close();
            ownershipConnection.Close();
            demuxSocket.Close();

        }

        public static async void StoreCheck(List<Uplay.Store.StoreProduct> temp, int internalAccountId)
        {
            if (StoreProducts == null)
            {
                StoreProducts = temp;
                return;
            }
            foreach (var item in temp)
            {
                if (!StoreProducts.Contains(item))
                {
                    Console.WriteLine(item.ProductId);
                    string toContent = "";
                    var prevItem = StoreProducts.Find(x=>x.ProductId == item.ProductId);
                    if (prevItem == null)
                    {
                        toContent += $"ProductId: {item.ProductId}\n";
                        toContent += $"Revision: {item.Revision}\n";
                        toContent += $"Associations: {string.Join(", ", item.Associations.ToList())}\n";
                        toContent += $"Configuration (Base64): {item.Configuration.ToBase64()}\n";
                        toContent += $"Credentials: {item.Credentials}\n";
                        toContent += $"Promotion Score: {item.PromotionScore}\n";
                        toContent += $"Staging: {item.Staging}\n";
                        toContent += $"Store Partner: {item.StorePartner}\n";
                        toContent += $"Store Reference: {item.StoreReference}\n";
                        toContent += $"User Blob: {item.UserBlob}\n";
                        toContent += $"Ownership Associations: " + string.Join(", ", item.OwnershipAssociations.ToList());
                    }
                    else
                    {
                        toContent += $"ProductId: {item.ProductId}\n";
                        if (item.Revision != prevItem.Revision)
                            toContent += $"Revision: {item.Revision}\n";
                        if (item.Associations.ToList() != prevItem.Associations.ToList())
                            toContent += $"Associations: {string.Join(", ", item.Associations.ToList())}\n";
                        if (item.Configuration.ToBase64() != prevItem.Configuration.ToBase64())
                            toContent += $"Configuration (Base64): {item.Configuration.ToBase64()}\n";
                        if (item.Credentials != prevItem.Credentials)
                            toContent += $"Credentials: {item.Credentials}\n";
                        if (item.PromotionScore != prevItem.PromotionScore)
                            toContent += $"Promotion Score: {item.PromotionScore}\n";
                        if (item.Staging != prevItem.Staging)
                            toContent += $"Staging: {item.Staging}\n";
                        if (item.StorePartner != prevItem.StorePartner)
                            toContent += $"Store Partner: {item.StorePartner}\n";
                        if (item.StoreReference != prevItem.StoreReference)
                            toContent += $"Store Reference: {item.StoreReference}\n";
                        if (item.UserBlob != prevItem.UserBlob)
                            toContent += $"User Blob: {item.UserBlob}\n";
                        if (item.OwnershipAssociations.ToList() != prevItem.OwnershipAssociations.ToList())
                            toContent += $"Ownership Associations: " + string.Join(", ", item.OwnershipAssociations.ToList());
                    }

                    await hook_store.SendAsync(new DiscordMessage()
                    {
                        Content = toContent,
                        Username = $"Store Update! ({internalAccountId})"
                    });
                    Thread.Sleep(500);
                }

            };

            StoreProducts.AddRange(temp);
        }


        public static json.Game ParseOW(Uplay.Ownership.OwnedGame game)
        {
            json.Game js = new();
            js.ProductId = game.ProductId;
            js.State = game.State;
            js.UbiservicesSpaceId = game.UbiservicesSpaceId;
            js.UbiservicesAppId = game.UbiservicesAppId;
            js.Configuration = game.Configuration;
            js.ConfigVersion = game.ConfigVersion;
            js.DownloadId = game.DownloadId;
            js.DownloadVersion = game.DownloadVersion;
            js.LatestManifest = game.LatestManifest;
            js.TargetPartner = game.TargetPartner;
            js.UplayId = game.UplayId;
            if (game.UbiservicesDynamicConfig != null)
            {
                js.UbiservicesDynamicConfig = new();
                js.UbiservicesDynamicConfig.GfnAppId = game.UbiservicesDynamicConfig.GfnAppId;
                js.UbiservicesDynamicConfig.LunaAppId = game.UbiservicesDynamicConfig.LunaAppId;

            }
            return js;
        }

        public static List<json.Game> ParseOWList(List<Uplay.Ownership.OwnedGame> temp)
        {
            List<json.Game> ret = new();
            foreach (var item in temp)
            {
                ret.Add(ParseOW(item));
            }
            return ret;
        }



        public static async void OWCheck(List<Uplay.Ownership.OwnedGame> temp, int internalAccountId)
        {
            var newtmp = ParseOWList(temp);
            if (OwnedGames == null)
            {
                OwnedGames = ParseOWList(temp);
                return;
            }
            foreach (var item in newtmp)
            {
                string toContent = "";
                if (!OwnedGames.Contains(item))
                {
                    Console.WriteLine(item.ProductId);
                    var prevItem = OwnedGames.Find(x => x.ProductId == item.ProductId);
                    if (prevItem == null)
                    {
                        toContent += $"ProductId: {item.ProductId}\n";
                        //toContent += $"Configuration: {item.Configuration}\n";
                        toContent += $"UbiservicesAppId: {item.UbiservicesAppId}\n";
                        toContent += $"UbiservicesSpaceId: {item.UbiservicesSpaceId}\n";
                        toContent += $"LatestManifest: {item.LatestManifest}\n";
                        toContent += $"TargetPartner: {item.TargetPartner}";
                    }
                    else
                    {
                        toContent += $"ProductId: {item.ProductId}\n";
                        /*
                        if (item.Configuration != prevItem.Configuration)
                        {
                            toContent += $"-Configuration: {prevItem.Configuration}\n";
                            toContent += $"+Configuration: {item.Configuration}\n";
                        }
                        else
                        {
                            toContent += $"Configuration: {item.Configuration}\n";
                        }
                        */
                        if (item.UbiservicesAppId != prevItem.UbiservicesAppId)
                        {
                            toContent += $"-UbiservicesAppId: {prevItem.UbiservicesAppId}\n";
                            toContent += $"+UbiservicesAppId: {item.UbiservicesAppId}\n";
                        }
                        else
                        {
                            toContent += $"UbiservicesAppId: {item.UbiservicesAppId}\n";
                        }
                        if (item.UbiservicesSpaceId != prevItem.UbiservicesSpaceId)
                        {
                            toContent += $"-UbiservicesSpaceId: {prevItem.UbiservicesSpaceId}\n";
                            toContent += $"+UbiservicesSpaceId: {item.UbiservicesSpaceId}\n";
                        }
                        else
                        {
                            toContent += $"UbiservicesSpaceId: {item.UbiservicesSpaceId}\n";
                        }
                        if (item.LatestManifest != prevItem.LatestManifest)
                        {
                            toContent += $"-LatestManifest: {prevItem.LatestManifest}\n";
                            toContent += $"+LatestManifest: {item.LatestManifest}\n";
                        }
                        else
                        {
                            toContent += $"LatestManifest: {item.LatestManifest}\n";
                        }
                        if (item.TargetPartner != prevItem.TargetPartner)
                        {
                            toContent += $"-TargetPartner: {prevItem.TargetPartner}\n";
                            toContent += $"+TargetPartner: {item.TargetPartner}\n";
                        }
                        else
                        {
                            toContent += $"TargetPartner: {item.TargetPartner}\n";
                        }
                    }
                }
                else
                {
                    var prevItem = OwnedGames.Find(x => x.ProductId == item.ProductId);
                    Console.WriteLine(JsonConvert.SerializeObject(item));
                    Console.WriteLine("|");
                    Console.WriteLine(JsonConvert.SerializeObject(prevItem));
                }
                Console.WriteLine("\n");
                if (!string.IsNullOrEmpty(toContent))
                {
                    Console.WriteLine(toContent);
                    await hook_ow.SendAsync(new DiscordMessage()
                    {
                        Content = toContent,
                        Username = $"OW Update! ({internalAccountId})"
                    });
                    //new FileInfo("C:/File/Path.file"))
                    Thread.Sleep(500);
                }
            };
            OwnedGames.AddRange(newtmp);
        }


        private static async void StoreConnection_PushEvent(object? sender, Uplay.Store.Push e)
        {
            File.AppendAllText("StorePush.txt",e.ToString());

            if (e.RevisionsUpdatedPush != null)
            {
                await hook_store.SendAsync(new DiscordMessage()
                {
                    Content = "Store Data Type: " +
                    e.RevisionsUpdatedPush.StoreDataType.ToString()
                    + "\nRemoved Products: "
                    + string.Join(", ", e.RevisionsUpdatedPush.RemovedProducts.ToList()),
                    Username = "Revision Update! (First)"
                });
                string toContent = "";
                var updateInfos = e.RevisionsUpdatedPush.UpdateInfo.ToList();
                int i = 0;
                foreach (var updateInfo in updateInfos)
                {
                    var storeprods = StoreProducts.Where(x => x.ProductId == updateInfo.ProductId).FirstOrDefault();
                    if (storeprods == null)
                    {
                        //New Store!
                        toContent += $"New ProductId: {updateInfo.ProductId}\n";
                        toContent += $"Revision: {updateInfo.Revision}\n";
                        toContent += $"Ownership Associations: " + string.Join(", ", updateInfo.OwnershipAssociations.ToList());
                    }
                    else
                    {
                        //old store
                        toContent += $"ProductId: {updateInfo.ProductId}\n";
                        toContent += $"Revision change: From {storeprods.Revision} To {updateInfo.Revision}\n";
                        toContent += $"Ownership Associations change: From {string.Join(", ", storeprods.OwnershipAssociations.ToList())} To {string.Join(", ", updateInfo.OwnershipAssociations.ToList())}";
                    }
                    //StoreGet(updateInfo.ProductId);


                    await hook_store.SendAsync(new DiscordMessage()
                    {
                        Content = toContent,
                        Username = $"Revision Update! ({i+1}/{e.RevisionsUpdatedPush.UpdateInfo.Count})"
                    });
                    i++;
                }
            }
            if (e.StoreUpdate != null)
            {
                string toContent = ""; 
                await hook_store.SendAsync(new DiscordMessage()
                {

                    Content = "Store Products Update Count: " +
                    e.StoreUpdate.StoreProducts.Count
                    + "\nRemoved Products"
                    + string.Join(", ", e.StoreUpdate.RemovedProducts.ToList()),
                    Username = "Store Update!"
                });
                var updateInfos = e.StoreUpdate.StoreProducts.ToList();
                foreach (var updateInfo in updateInfos)
                {
                    var storeprods = StoreProducts.Where(x => x.ProductId == updateInfo.ProductId).FirstOrDefault();
                    if (storeprods == null)
                    {
                        //New Store!
                        toContent += $"ProductId: {updateInfo.ProductId}\n";
                        toContent += $"Revision: {updateInfo.Revision}\n";
                        toContent += $"Associations: {string.Join(", ", updateInfo.Associations.ToList())}\n";
                        toContent += $"Configuration (Base64): {updateInfo.Configuration.ToBase64()}\n";
                        toContent += $"Credentials: {updateInfo.Credentials}\n";
                        toContent += $"Promotion Score: {updateInfo.PromotionScore}\n";
                        toContent += $"Staging: {updateInfo.Staging}\n";
                        toContent += $"Store Partner: {updateInfo.StorePartner}\n";
                        toContent += $"Store Reference: {updateInfo.StoreReference}\n";
                        toContent += $"User Blob: {updateInfo.UserBlob}\n";
                        toContent += $"Ownership Associations: " + string.Join(", ", updateInfo.OwnershipAssociations.ToList());
                    }
                    else
                    {
                        //old store
                        toContent += "Old Store Update: ";
                        toContent += $"ProductId: {updateInfo.ProductId}\n";
                        toContent += $"Revision: {updateInfo.Revision}\n";
                        toContent += $"Associations: {string.Join(", ", updateInfo.Associations.ToList())}\n";
                        toContent += $"Configuration (Base64): {updateInfo.Configuration.ToBase64()}\n";
                        toContent += $"Credentials: {updateInfo.Credentials}\n";
                        toContent += $"Promotion Score: {updateInfo.PromotionScore}\n";
                        toContent += $"Staging: {updateInfo.Staging}\n";
                        toContent += $"Store Partner: {updateInfo.StorePartner}\n";
                        toContent += $"Store Reference: {updateInfo.StoreReference}\n";
                        toContent += $"User Blob: {updateInfo.UserBlob}\n";
                        toContent += $"Ownership Associations: {string.Join(", ", updateInfo.OwnershipAssociations.ToList())}";
                    }
                    await hook_store.SendAsync(new DiscordMessage()
                    {

                        Content = toContent,
                        Username = "Store Update!"
                    });
                    //StoreGet(updateInfo.ProductId);
                }
            }
        }
        /*
        public static void StoreGet(uint ProdId)
        { 
            string toContent = "";
            var upsell = storeConnection.GetData(Uplay.Store.StoreType.Upsell, new() { ProdId });
            var ingame = storeConnection.GetData(Uplay.Store.StoreType.Ingame, new() { ProdId });

            var upprod = upsell.Products[0];
            toContent += "Upsell Info:\n";
            toContent += $"ProductId: {upprod.ProductId}\n";
            toContent += $"Revision: {upprod.Revision}\n";
            toContent += $"Associations: {string.Join(", ", upprod.Associations.ToList())}\n";
            toContent += $"Configuration (Base64): {upprod.Configuration.ToBase64()}\n";
            toContent += $"Credentials: {upprod.Credentials}\n";
            toContent += $"Promotion Score: {upprod.PromotionScore}\n";
            toContent += $"Staging: {upprod.Staging}\n";
            toContent += $"Store Partner: {upprod.StorePartner}\n";
            toContent += $"Store Reference: {upprod.StoreReference}\n";
            toContent += $"User Blob: {upprod.UserBlob}\n";
            toContent += $"Ownership Associations: " + string.Join(", ", upprod.OwnershipAssociations.ToList());
            SendContentST(toContent);

            var inpord = ingame.Products[0];
            toContent = "Ingame Info:\n";
            toContent += $"ProductId: {inpord.ProductId}\n";
            toContent += $"Revision: {inpord.Revision}\n";
            toContent += $"Associations: {string.Join(", ", inpord.Associations.ToList())}\n";
            toContent += $"Configuration (Base64): {inpord.Configuration.ToBase64()}\n";
            toContent += $"Credentials: {inpord.Credentials}\n";
            toContent += $"Promotion Score: {inpord.PromotionScore}\n";
            toContent += $"Staging: {inpord.Staging}\n";
            toContent += $"Store Partner: {inpord.StorePartner}\n";
            toContent += $"Store Reference: {inpord.StoreReference}\n";
            toContent += $"User Blob: {inpord.UserBlob}\n";
            toContent += $"Ownership Associations: " + string.Join(", ", inpord.OwnershipAssociations.ToList());
            SendContentST(toContent);
            
            StoreProducts = storeConnection.GetStore().StoreProducts.ToList();
        }
        */
        private static async void Ownership_PushEvent(object? sender, Uplay.Ownership.Push e)
        {
            File.AppendAllText("OwnershipPush.txt", e.ToString());
            
            if (e.SubscriptionPush != null)
            {
                await hook_ow.SendAsync(new DiscordMessage()
                {
                    Content = "Subscription Type:"
                    + e.SubscriptionPush.SubscriptionType 
                    + "Subscription State:\n" 
                    + e.SubscriptionPush.SubscriptionState.ToString(),
                    Username = "Subscription Update!"
                });
            }
            if (e.OwnedGamePush != null)
            {
                await hook_ow.SendAsync(new DiscordMessage()
                {
                    Content = "TODO",
                    Username = "Ownedgames Update!"
                });
            }
        }

        public static async void SendContentOW(string content)
        {
            await hook_ow.SendAsync(new DiscordMessage()
            { 
                Content = content,
                Username = "Content Manager",
                AvatarUrl = new("https://cdn.discordapp.com/attachments/1088196917642670175/1088543467531149362/b70cac644aab52b32b57c9299ab4b45f.png")
            });
        }

        public static async void SendContentST(string content)
        {
            await hook_store.SendAsync(new DiscordMessage()
            {
                Content = content,
                Username = "Content Manager",
                AvatarUrl = new("https://cdn.discordapp.com/attachments/1088196917642670175/1088543467531149362/b70cac644aab52b32b57c9299ab4b45f.png")
            });
        }


        /*
        
        DiscordMessage message = new DiscordMessage();
message.Content = "Example message, ping @everyone, <@userid>";
message.TTS = true; //read message to everyone on the channel
message.Username = "Webhook username";
message.AvatarUrl = "http://url-of-image";

//embeds
DiscordEmbed embed = new DiscordEmbed();
embed.Title = "Embed title";
embed.Description = "Embed description";
embed.Url = new Uri("Embed Url");
embed.Timestamp = new DiscordTimestamp(DateTime.Now);
embed.Color = new DiscordColor(Color.Red); //alpha will be ignored, you can use any RGB color
embed.Footer = new EmbedFooter() {Text="Footer Text", IconUrl = new Uri("http://url-of-image")};
embed.Image = new EmbedMedia() {Url= new Uri("Media URL"), Width=150, Height=150}; //valid for thumb and video
embed.Provider = new EmbedProvider() {Name="Provider Name", Url=new Uri("Provider Url") };
embed.Author = new EmbedAuthor() {Name="Author Name", Url= new Uri("Author Url"), IconUrl=new Uri("http://url-of-image")};

//fields
embed.Fields = new List<EmbedField>();
embed.Fields.Add(new EmbedField() {Name="Field Name", Value="Field Value", InLine=true });
embed.Fields.Add(new EmbedField() {Name="Field Name 2", Value="Field Value 2", InLine=true });

//set embed
message.Embeds = new List<DiscordEmbed>();
message.Embeds.Add(embed);

//Allowed mentions
message.AllowedMentions = new AllowedMentions();

message.AllowedMentions.Parse.Add(AllowedMentionType.Role); //Allow role mentions
message.AllowedMentions.Parse.Add(AllowedMentionType.User); //Allow user mentions
message.AllowedMentions.Parse.Add(AllowedMentionType.Everyone); //Allow @everyone mentions]

message.AllowedMentions.Roles = new List<ulong>();
message.AllowedMentions.Roles.Add(000000000000000000); //Allow mention the role with this id

message.AllowedMentions.Users = new List<ulong>();
message.AllowedMentions.Users.Add(000000000000000000); //Allow mention the user this this id
           
        
        */
    }
}