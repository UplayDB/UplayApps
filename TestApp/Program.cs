using Newtonsoft.Json;
using System.ComponentModel;
using System.IO.Compression;
using System.Net.Sockets;
using System.Text;
using UplayKit.Connection;
using UplayKit;
using Uplay.Ownership;

namespace TestApp;

internal class Program
{
    static void Main(string[] args)
    {
        var login = UbiServices.Public.V3.LoginBase64("");
        if (login != null)
        {
            //File.WriteAllText("login.json", JsonConvert.SerializeObject(login));

            DemuxSocket socket = new();
            socket.WaitInTimeMS = 2;
            socket.VersionCheck();

            socket.PushVersion();

            socket.Authenticate(login.Ticket);

            OwnershipConnection ownershipConnection = new(socket, login.Ticket, login.SessionId);
            ownershipConnection.PushEvent += Ownership_PushEvent;
            var games = ownershipConnection.GetOwnedGames();
            var rsp = ownershipConnection.SendPostRequest<Upstream, Downstream>(new()
            {
                Request = new()
                {
                    GetGameTokenReq = new()
                    {
                        ProductId = 46
                    },
                    UbiSessionId = ownershipConnection.SessionId,
                    UbiTicket = ownershipConnection.Ticket,
                }
            });
            Console.WriteLine(rsp);


        }
    }

    static int IndexOfParam(string[] args, string param)
    {
        for (var x = 0; x < args.Length; ++x)
        {
            if (args[x].Equals(param, StringComparison.OrdinalIgnoreCase))
                return x;
        }

        return -1;
    }

    static bool HasParameter(string[] args, string param)
    {
        return IndexOfParam(args, param) > -1;
    }

    static T GetParameter<T>(string[] args, string param, T defaultValue = default(T))
    {
        var index = IndexOfParam(args, param);

        if (index == -1 || index == (args.Length - 1))
            return defaultValue;

        var strParam = args[index + 1];

        var converter = TypeDescriptor.GetConverter(typeof(T));
        if (converter != null)
        {
            return (T)converter.ConvertFromString(strParam);
        }

        return default(T);
    }

    static string ReadPassword()
    {
        ConsoleKeyInfo keyInfo;
        var password = new StringBuilder();

        do
        {
            keyInfo = Console.ReadKey(true);
            if (keyInfo.Key == ConsoleKey.Backspace)
            {
                if (password.Length > 0)
                {
                    password.Remove(password.Length - 1, 1);
                    Console.Write("\b \b");
                }

                continue;
            }
            /* Printable ASCII characters only */
            var c = keyInfo.KeyChar;
            if (c >= ' ' && c <= '~')
            {
                password.Append(c);
                Console.Write('*');
            }
        } while (keyInfo.Key != ConsoleKey.Enter);

        return password.ToString();
    }


    private static void Ownership_PushEvent(object? sender, Push e)
    {
        Console.WriteLine(e.ToString());
    }
    
}





/*
 * 
 *             /*
    var login = UbiServices.Public.V3.LoginBase64("");
    List<string> AppIds = new();
    for (uint i = 0; i <= 1200; i = i +100)
    {
        var catalogy = UbiServices.Public.V1.Spaces.GetCatalog(login.Ticket,
        login.SessionId,
        false,
        i,
        100);
        File.WriteAllText("Catalogy/" + i + ".json", JsonConvert.SerializeObject(catalogy, Formatting.Indented));
        foreach (var item in catalogy.Games)
        {
            if (item.SiblingGames.Count != 0)
            {
                foreach (var siblingGame in item.SiblingGames)
                {
                    foreach (var platform in siblingGame.Platforms)
                    {
                        if (!AppIds.Contains(platform.ApplicationId))
                            AppIds.Add(platform.ApplicationId);
                    }
                }
                foreach (var platform in item.Platforms)
                {
                    if (!AppIds.Contains(platform.ApplicationId))
                        AppIds.Add(platform.ApplicationId);
                }
            }

        }

    }
    for (uint i = 0; i <= 1200; i = i + 100)
    {
        var catalogy = UbiServices.Public.V1.Spaces.GetCatalog(login.Ticket,
        login.SessionId,
        true,
        i,
        100);
        File.WriteAllText("Catalogy/" + i + ".json", JsonConvert.SerializeObject(catalogy, Formatting.Indented));
        foreach (var item in catalogy.Games)
        {
            if (item.SiblingGames.Count != 0)
            {
                foreach (var siblingGame in item.SiblingGames)
                {
                    foreach (var platform in siblingGame.Platforms)
                    {
                        if (!AppIds.Contains(platform.ApplicationId))
                            AppIds.Add(platform.ApplicationId);
                    }
                }
                foreach (var platform in item.Platforms)
                {
                    if (!AppIds.Contains(platform.ApplicationId))
                        AppIds.Add(platform.ApplicationId);
                }
            }

        }

    }
    File.WriteAllText("AppIds.json", JsonConvert.SerializeObject(AppIds, Formatting.Indented));
    foreach (var line in AppIds)
    {
        Console.WriteLine(line);
        File.WriteAllText("apps/v1_" + line + "_config.json", JsonConvert.SerializeObject(UbiServices.Public.V1.Applications.GetApplicationConfig(line), Formatting.Indented));
        File.WriteAllText("apps/v1_" + line + "_param.json", JsonConvert.SerializeObject(UbiServices.Public.V1.Applications.GetApplicationParameters(line), Formatting.Indented));
        File.WriteAllText("apps/v2_" + line + "_config.json", JsonConvert.SerializeObject(UbiServices.Public.V2.Applications.GetApplicationConfig(line), Formatting.Indented));
        File.WriteAllText("apps/v2_" + line + "_param.json", JsonConvert.SerializeObject(UbiServices.Public.V2.Applications.GetApplicationParameters(line), Formatting.Indented));
    }
    /*
    if (!File.Exists("All_Extract.txt"))
        return;

    var lines = File.ReadAllLines("All_Extract.txt");


    */
/*
Console.WriteLine(uplay_r1.UPLAY_Start(410,0));
Console.WriteLine(uplay_r1.UPLAY_PARTY_Init(3));
Console.WriteLine(uplay_r1.UPLAY_FRIENDS_Init(0));
Console.WriteLine(uplay_r1.UPLAY_INSTALLER_Init(0));
Console.WriteLine(uplay_r1.UPLAY_USER_IsConnected());
Console.WriteLine(uplay_r1.UPLAY_USER_IsOwned(410));
Console.WriteLine(uplay_r1.UPLAY_Update());
Console.WriteLine(uplay_r1.UPLAY_Quit());

//AppID = "6c6b8cd7-d901-4cd5-8279-07ba92088f06";
Console.WriteLine(String.Join(",", args));
        Console.ReadLine();
        LoginJson? login;
        if (HasParameter(args, "-b64"))
        {
            var b64 = GetParameter(args, "-b64", "");
            Console.WriteLine(b64);
            login = LoginBase64(b64);
        }
        else if ((HasParameter(args, "-username") || HasParameter(args, "-user")) && (HasParameter(args, "-password") || HasParameter(args, "-pass")))
        {
            var username = GetParameter<string>(args, "-username") ?? GetParameter<string>(args, "-user");
            var password = GetParameter<string>(args, "-password") ?? GetParameter<string>(args, "-pass");
            login = Login(username, password);
        }
        else
        {
            Console.WriteLine("Please enter your Email:");
            string username = Console.ReadLine()!;
            Console.WriteLine("Please enter your Password:");
            string password = ReadPassword();
            login = Login(username, password);
        }

        if (login != null && login.Ticket == null)
        {
            Console.WriteLine("Your account has 2FA, please enter your code:");
            var code2fa = Console.ReadLine();
            if (code2fa == null)
            {
                Console.WriteLine("Code cannot be empty!");
                Environment.Exit(1);
            }
            login = Login2FA(login.TwoFactorAuthenticationTicket, code2fa);
        }
        // Last login check
        if (login == null)
        {
            Console.WriteLine("Login failed");
            Environment.Exit(1);
        }
        //var login = UbiServices.Public.V3.LoginBase64("");
        if (login != null)
        {
            File.WriteAllText("login.json", JsonConvert.SerializeObject(login));

            Debug.isDebug = true;
            DemuxSocket socket = new();
            socket.WaitInTimeMS = 2;
            socket.VersionCheck();

            socket.PushVersion();

            socket.Authenticate(login.Ticket);

            OwnershipConnection ownershipConnection = new(socket);
            ownershipConnection.PushEvent += Ownership_PushEvent;
            var games = ownershipConnection.GetOwnedGames();
            Console.WriteLine("owned games: " + games.Count);
            Console.WriteLine("please enter your cdkey:");
            var rsp = ownershipConnection.RegisterOwnershipByCdKey(Console.ReadLine());
            Console.WriteLine(rsp.Result);
            Console.WriteLine(rsp.ToString());

            //CloudSaveConnection cloudSaveConnection = new(socket);
            //games.ForEach(x=> yeet(x.ProductId, cloudSaveConnection, ownershipConnection, socket, login.NameOnPlatform));




            /*
            StoreConnection storeConnection = new(socket);
            storeConnection.PushEvent += StoreConnection_PushEvent;
            storeConnection.Init();
            /*
            ChannelProfile.GetPendingChannels(login.Ticket,login.SessionId);
            ChannelProfile.GetActiveChannels(login.Ticket, login.SessionId);

            WebSocket webSocket = new(login.SessionId, login.Ticket);
            webSocket.Start();
            Console.ReadLine();
            webSocket.Stop();
            */
            /*
            socket.StopCheck();
            Console.ReadLine();
            
            var post = new Uplay.GameStarter.Upstream()
            {
                Req = new()
                {
                    StartReq = new()
                    {
                        LauncherVersion = 10815,
                        UplayId = 46,
                        SteamGame = false,
                        GameVersion = 6,
                        ProductId = 46,
                        ExecutablePath = "D:\\Games\\Far Cry 3\\bin\\farcry3_d3d11.exe",
                        ExecutableArguments = "-language=English",
                        Platform = Uplay.GameStarter.StartReq.Types.Platform.Uplay,
                        TimeStart = (ulong)DateTime.Now.ToFileTime()
                    }
                }
            };

            var up = Formatters.FormatUpstream(post.ToByteArray());
            Console.WriteLine(BitConverter.ToString(up));
            var down = socket.SendBytes(up);

            Console.WriteLine(BitConverter.ToString(down));

            Console.ReadLine();
            socket.StartCheck();
            */
            /*
            Console.WriteLine("end?");
            Console.ReadLine();
            Console.WriteLine();
            socket.Disconnect();
            */


            /*
            var spaceId = "1ee4f5d4-3a0b-4e55-ad3d-b398e014c02a";
            File.WriteAllText("login.json", JsonConvert.SerializeObject(login));

            var appparam = V1.Applications.GetApplicationParameters(AppID);
            File.WriteAllText("appparam.json", JsonConvert.SerializeObject(appparam));

            var appconf = V1.Applications.GetApplicationConfig(AppID);
            File.WriteAllText("appconf.json", JsonConvert.SerializeObject(appconf));

            var items = V1.Spaces.GetSpaceAllItems(spaceId, login.Ticket);
            File.WriteAllText("items_pre.json", JsonConvert.SerializeObject(items));

            File.WriteAllText("spaceparam.json", JsonConvert.SerializeObject(V1.Spaces.GetSpaceParameters(spaceId)));

            var compressedItems = items["compressedItems"];

            var b64com = (string)compressedItems;

            var bs = ByteString.FromBase64(b64com);

            File.WriteAllBytes("items_pre",bs.ToArray());
            var decpm = Decompress(bs.ToArray());
            
            var itemsjson = Encoding.UTF8.GetString(decpm.ToArray());
            var des = JsonConvert.DeserializeObject(itemsjson);
            var ser = JsonConvert.SerializeObject(des, Formatting.Indented);
            File.WriteAllText("items_out.json", ser);
            /*
            File.WriteAllText("login_rem.txt", JsonConvert.SerializeObject(login));
            if (login.Ticket == null)
            {
                Console.WriteLine("Your account has 2FA, please enter your code:");
                var code2fa = Console.ReadLine();
                if (code2fa == null)
                {
                    Console.WriteLine("Code cannot be empty!");
                    Environment.Exit(1);
                }
                login = Login2FA(login.TwoFactorAuthenticationTicket, code2fa);
            }
            File.WriteAllText("login_2fa.txt", JsonConvert.SerializeObject(login));
            */
            /*

            */

        /*
        List<UbiServices.Records.Request> requests = new();
        requests.Add(
            new()
            {   IndexName = "ie_product_suggestion",
                Params = "hitsPerPage=100&page=0&highlightPreTag=__ais-highlight__&highlightPostTag=__%2Fais-highlight__&facets=%5B%5D&tagFilters=&analytics=false"
            }
        );

        


        var result = UbiServices.Store.AlgoliaSearch.PostStoreAlgoliaSearch(requests);

        if (result != null)
        {
            JArray resultArray = (JArray)result["results"];

            for (int i = 0; i < resultArray.Count; i++)
            {
                JObject resultsAObject = JObject.FromObject(resultArray[i]);
                JArray resultsAHits = (JArray)resultsAObject["hits"];
                //Console.WriteLine(o2);
                for (int i2 = 0; i2 < resultsAHits.Count; i2++)
                {
                    JObject finalJson = JObject.FromObject(resultsAHits[i2]);
                    JValue title = (JValue)finalJson["title"];
                    JValue id = (JValue)finalJson["id"];
                    Console.WriteLine($"{title} ({id})");
                }
            }
        }
        */
