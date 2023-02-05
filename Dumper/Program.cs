using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.ComponentModel;
using UbiServices;
using UbiServices.Records;
using Uplay.Store;
using UplayKit;
using UplayKit.Connection;
using static UbiServices.Public.V3;

namespace Dumper
{
    internal class Program
    {
        static void Main(string[] args)
        {
            LoginJson? login;
            var currentDir = GetParameter(args, "-dir", Environment.CurrentDirectory);
            if (!HasParameter(args, "-skip"))
            {

                if (HasParameter(args, "-b64"))
                {
                    var b64 = GetParameter<string>(args, "-b64");
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
                    string password = Console.ReadLine();
                    login = Login(username, password);
                }
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
                // Last login check
                if (login == null)
                {
                    Console.WriteLine("Login failed");
                    Environment.Exit(1);
                }

                Debug.isDebug = false;
                
                DemuxSocket socket = new();
                Console.WriteLine("Is same Version? " + socket.VersionCheck());
                socket.PushVersion();
                bool IsAuthSuccess = socket.Authenticate(login.Ticket);
                Console.WriteLine("Is Auth Success? " + IsAuthSuccess);
                if (!IsAuthSuccess)
                {
                    Console.WriteLine("Oops something is wrong!");
                    Environment.Exit(1);
                }
                OwnershipConnection ownership = new(socket);
                ownership.PushEvent += Ownership_PushEvent;
                DownloadConnection downloadConnection = new(socket);
                var games_ = ownership.GetOwnedGames(true);
                var games = games_.Where(x => x.LatestManifest.Trim().Length > 0).ToArray();
                List<string> strlist = new();
                List<uint> prodIdList = new();
                if (!Directory.Exists(currentDir + "\\files"))
                {
                    Directory.CreateDirectory(currentDir + "\\files");
                }
                foreach (var game in games)
                {
                    Console.WriteLine($"{game.ProductId}={game.LatestManifest}");
                    strlist.Add($"{game.ProductId}={game.LatestManifest}");
                    prodIdList.Add(game.ProductId);

                    string ownershipToken_1 = ownership.GetOwnershipToken(game.ProductId).Item1;
                    downloadConnection.InitDownloadToken(ownershipToken_1);
                    if (downloadConnection.isServiceSuccess != false)
                    {
                        string manifestUrl_1 = downloadConnection.GetUrl(game.LatestManifest, game.ProductId);
                        var rc1 = new RestClient();
                        File.WriteAllBytes(currentDir + "\\files\\" + game.ProductId + "_" + game.LatestManifest + ".manifest", rc1.DownloadData(new(manifestUrl_1)));

                        var gm = $"{currentDir}\\files\\{game.ProductId}_{game.LatestManifest}.manifest";
                        Dumper.Dump(Parsers.ParseManifestFile(gm), gm.Replace(".manifest", ".txt"));
                        Dumper.DumpAsCSV(Parsers.ParseManifestFile(gm), null, gm.Replace(".manifest", ""), game.LatestManifest, game.ProductId);
                    }
                }
                if (File.Exists(currentDir + "\\latest_manifests.txt"))
                {
                    Console.WriteLine("file are here");
                    var latest_manifests = File.ReadAllLines(currentDir + "\\latest_manifests.txt");
                    foreach (var latest_manifest in latest_manifests)
                    {
                        if (!strlist.Contains(latest_manifest))
                        {
                            if (!prodIdList.Contains(uint.Parse(latest_manifest.Split("=")[0])))
                            {
                                strlist.Add(latest_manifest);
                            }
                            else
                            {
                                Console.WriteLine(latest_manifest + " Probably new manifest!");
                            }

                        }
                    }

                }
                strlist.Sort();
                string write_out = "";
                foreach (var st in strlist)
                {
                    write_out += st + "\n";
                }
                File.WriteAllText(currentDir + "\\latest_manifests.txt", write_out);

                var games2 = games_.Where(x => x.Configuration.Length != 0).ToArray();


                List<prodconf> listconf = new();

                if (File.Exists(currentDir + "\\productconfig.json"))
                {
                    listconf = JsonConvert.DeserializeObject<List<prodconf>>(File.ReadAllText(currentDir + "\\productconfig.json"));
                    Console.WriteLine(listconf.Count);
                }

                foreach (var g in games2)
                {
                    prodconf prodconf = new()
                    {
                        ProductId = g.ProductId,
                        Configuration = g.Configuration
                    };

                    if (listconf.FindAll(x => x.ProductId == g.ProductId).Count <= 0)
                    {
                        Console.WriteLine(JsonConvert.SerializeObject(prodconf));
                        listconf.Add(prodconf);
                    }
                    var listdiffconf = listconf.Where(x => x.ProductId == g.ProductId && x.Configuration != g.Configuration).ToList();

                    if (listdiffconf.Count > 0)
                    {
                        Console.WriteLine(listdiffconf.Count);
                        listconf.Where(x => x.ProductId == g.ProductId).First().Configuration = g.Configuration;
                    }


                }
                Console.WriteLine(listconf.Count);
                File.WriteAllText(currentDir + "\\productconfig.json", JsonConvert.SerializeObject(listconf, Formatting.Indented));



                StoreConnection storeConnection = new(socket);
                storeConnection.Init();
                var store = storeConnection.GetStore();



                List<storeconf> storeconf = new();

                if (File.Exists("storeref.json"))
                {
                    storeconf = JsonConvert.DeserializeObject<List<storeconf>>(File.ReadAllText("storeref.json"));
                    Console.WriteLine(storeconf.Count);
                }
                var storelist = store.StoreProducts.OrderBy(x => x.ProductId).ToList();
                foreach (var storeprod in storelist)
                {
                    if (storeprod.StoreReference.Trim().Length == 0)
                    {
                        continue;
                    }

                    storeconf storeconf_ = new()
                    {
                        ProductId = storeprod.ProductId,
                        StoreRef = storeprod.StoreReference,
                        Partner = storeprod.StorePartner
                    };

                    if (storeconf.FindAll(x => x.ProductId == storeprod.ProductId).Count <= 0)
                    {
                        Console.WriteLine(JsonConvert.SerializeObject(storeconf_));
                        storeconf.Add(storeconf_);
                    }

                    var listdiffconf = storeconf.Where(x => x.ProductId == storeprod.ProductId && x.StoreRef != storeprod.StoreReference).ToList();

                    if (listdiffconf.Count > 0)
                    {
                        Console.WriteLine(listdiffconf.Count);
                        storeconf.Where(x => x.ProductId == storeprod.ProductId).First().StoreRef = storeprod.StoreReference;
                    }

                }
                storeconf = storeconf.OrderBy(x => x.ProductId).ToList();
                Console.WriteLine(storeconf.Count);
                File.WriteAllText("storeref.json", JsonConvert.SerializeObject(storeconf, Formatting.Indented));
                socket.Close();
                Console.WriteLine("Socket Closed");
            }
            if (HasParameter(args, "-gen"))
            {
                List<storeconf> storeconf2 = new();

                if (File.Exists("storeref.json"))
                {
                    storeconf2 = JsonConvert.DeserializeObject<List<storeconf>>(File.ReadAllText("storeref.json"));
                    Console.WriteLine(storeconf2.Count);
                }
                List<string> brands = new() { "Anno", "AC", "Brawlhalla", "BGE", "BIA", "CF", "COL", "COA", "FB", "FC", "FH", "FD", "GR", "IFR", "Monopoly", "Others", "Rayman", "RR", "RC", "RS", "SD", "SouthPark", "ScottPilgrim", "Steep", "Starlink", "Trackmania", "Trials", "TS", "TD", "TCE", "TC", "UNO", "WD" };

                if (!Directory.Exists("Store"))
                {
                    Directory.CreateDirectory("Store");
                }

                foreach (var brand in brands)
                {
                    if (!Directory.Exists("Store\\" + brand))
                    {
                        Directory.CreateDirectory("Store\\" + brand);
                    }

                }

                List<idmap> idmaps = new();

                if (File.Exists("idmap.json"))
                {
                    idmaps = JsonConvert.DeserializeObject<List<idmap>>(File.ReadAllText("idmap.json"));
                }

                List<string> allId = new();
                foreach (var conf in storeconf2)
                {
                    for (int i = 0; i <= 18; i++)
                    {
                        var country = (Enums.CountryCode)i;
                        Thread.Sleep(10);

                        if (conf.StoreRef.Contains("/") || conf.StoreRef.Contains("\\"))
                        {
                            continue;
                        }


                        var callback = UbiServices.Store.Products.GetStoreFrontByProducts(country, new() { conf.StoreRef }, new() { "images", "variations", "prices", "promotions", "availability" }, false);

                        File.WriteAllText($"Store\\{conf.ProductId}_{country.ToString()}.json", JsonConvert.SerializeObject(callback, Formatting.Indented));

                        if (File.Exists($"Store\\{conf.ProductId}_{country.ToString()}.json"))
                        {
                            var file = File.ReadAllText($"Store\\{conf.ProductId}_{country.ToString()}.json", encoding: System.Text.Encoding.UTF8);

                            if (file == "null")
                            {
                                goto MOVE;
                            }

                            var thing = JObject.Parse(file);
                            var c = (int)thing["count"];

                            if (c == 0)
                            {
                                goto MOVE;
                            }

                            JArray resultArray = (JArray)thing["data"];
                            var bidata = JObject.FromObject(resultArray[0]);
                            var brand = bidata["c_productBrandDisplayString"];
                            var subbrand = bidata["c_productSubBrandString"];
                            var id = bidata["id"];
                            var EditionsListString = (JArray)bidata["c_productOtherEditionsListString"];

                            if (!allId.Contains((string)id))
                            {
                                allId.Add((string)id);
                            }

                            if (EditionsListString != null)
                            {
                                foreach (var edition in EditionsListString)
                                {
                                    if (!allId.Contains((string)edition))
                                    {
                                        allId.Add((string)edition);
                                    }

                                }
                            }


                            if (!string.IsNullOrEmpty((string)brand) && !string.IsNullOrEmpty((string)subbrand))
                            {
                                var sid = (string)subbrand;
                                idmaps = containsBrand(conf.ProductId, sid, idmaps, brands);
                            }
                            else if (!string.IsNullOrEmpty((string)brand))
                            {
                                var sid = (string)brand;
                                idmaps = containsBrand(conf.ProductId, sid, idmaps, brands);
                            }
                            else if (!string.IsNullOrEmpty((string)subbrand))
                            {
                                var sid = (string)subbrand;
                                idmaps = containsBrand(conf.ProductId, sid, idmaps, brands);
                            }

                        MOVE:
                            var idmap_2 = idmaps.FirstOrDefault(x => x.ProductId == conf.ProductId);
                            var sermap = JsonConvert.SerializeObject(idmap_2);
                            Console.WriteLine($"{conf.ProductId} ({country.ToString()})");
                            if (sermap == "null")
                            {
                                Console.WriteLine();
                                Console.WriteLine($"FOR OTHERS: {conf.ProductId} ({country.ToString()})");
                                idmap _ = new()
                                {
                                    ProductId = conf.ProductId,
                                    Brand = "Others"
                                };
                                idmaps.Add(_);
                                File.Copy($"Store\\{conf.ProductId}_{country.ToString()}.json", $"Store\\Others\\{conf.ProductId}_{country.ToString()}.json", true);

                            }
                            else
                            {
                                File.Copy($"Store\\{conf.ProductId}_{country.ToString()}.json", $"Store\\{idmap_2.Brand}\\{conf.ProductId}_{country.ToString()}.json", true);
                            }
                            File.Delete($"Store\\{conf.ProductId}_{country.ToString()}.json");
                        }
                    }
                }
                allId.Sort();
                File.WriteAllText("idmap.json", JsonConvert.SerializeObject(idmaps, Formatting.Indented));
                File.WriteAllText("allId.txt", String.Join("\n", allId));
            }

            if (HasParameter(args, "-csv"))
            {
                foreach (var file in Directory.GetFileSystemEntries(currentDir + "\\files", "*.manifest", SearchOption.AllDirectories))
                {
                    var splitted = file.Split("_");
                    Console.WriteLine(splitted[0]);
                    var prodId = uint.Parse(splitted[0].Replace(currentDir + "\\files\\", ""));
                    var manifestId = splitted[1].Replace(".manifest", "");
                    Dumper.Dump(Parsers.ParseManifestFile(file), file.Replace(".manifest", ".txt"));
                    Dumper.DumpAsCSV(Parsers.ParseManifestFile(file), null, file.Replace(".manifest", ""), manifestId, prodId);
                }

            }
            Console.WriteLine("end?");
            Console.ReadLine();
            Console.WriteLine();
        }

        private static void Ownership_PushEvent(object? sender, Uplay.Ownership.Push e)
        {
            File.WriteAllText("Ownership_PushEvent_DUMPER.txt", e.ToString());
        }

        public class prodconf
        {
            public uint ProductId { get; set; }
            public string Configuration { get; set; }
        }

        public class storeconf
        {
            public uint ProductId { get; set; }
            public string StoreRef { get; set; }
            public StorePartner Partner { get; set; }
        }
        public class idmap
        {
            public uint ProductId { get; set; }
            public string Brand { get; set; }
        }


        static List<idmap> containsBrand(uint prod, string brand, List<idmap> idmaps, List<string> brands)
        {
            if (!idmaps.Where(x => x.ProductId == prod).Any())
            {
                if (brands.Select(x => x.ToLower()).ToList().Contains(brand.ToLower()))
                {
                    var br = brands.Where(x => x.ToLower().Contains(brand.ToLower())).FirstOrDefault();
                    if (br == null)
                    {
                        return idmaps;
                    }

                    idmap _ = new()
                    {
                        ProductId = prod,
                        Brand = br
                    };
                    idmaps.Add(_);
                }
                else // shit implementation for brand thing
                {
                    if (brand.ToLower().Contains("trials"))
                    {
                        idmap _ = new()
                        {
                            ProductId = prod,
                            Brand = "Trials"
                        };
                        idmaps.Add(_);
                    }

                    if (brand.ToLower().Contains("far cry"))
                    {
                        idmap _ = new()
                        {
                            ProductId = prod,
                            Brand = "FC"
                        };
                        idmaps.Add(_);
                    }
                    if (brand.ToLower().Contains("anno"))
                    {
                        idmap _ = new()
                        {
                            ProductId = prod,
                            Brand = "Anno"
                        };
                        idmaps.Add(_);
                    }
                    if (brand.Contains("ssassin"))
                    {
                        idmap _ = new()
                        {
                            ProductId = prod,
                            Brand = "AC"
                        };
                        idmaps.Add(_);
                    }
                    if (brand.Contains("ivision"))
                    {
                        idmap _ = new()
                        {
                            ProductId = prod,
                            Brand = "TD"
                        };
                        idmaps.Add(_);
                    }
                    if (brand.Contains("ainbow"))
                    {
                        idmap _ = new()
                        {
                            ProductId = prod,
                            Brand = "RS"
                        };
                        idmaps.Add(_);
                    }
                    if (brand.Contains("hild"))
                    {
                        idmap _ = new()
                        {
                            ProductId = prod,
                            Brand = "COL"
                        };
                        idmaps.Add(_);
                    }
                    if (brand.Contains("war"))
                    {
                        idmap _ = new()
                        {
                            ProductId = prod,
                            Brand = "TCE"
                        };
                        idmaps.Add(_);
                    }
                    if (brand.ToLower().Contains("ghost"))
                    {
                        idmap _ = new()
                        {
                            ProductId = prod,
                            Brand = "GR"
                        };
                        idmaps.Add(_);
                    }
                    if (brand.ToLower().Contains("crew"))
                    {
                        idmap _ = new()
                        {
                            ProductId = prod,
                            Brand = "TC"
                        };
                        idmaps.Add(_);
                    }
                    if (brand.ToLower().Contains("dogs"))
                    {
                        idmap _ = new()
                        {
                            ProductId = prod,
                            Brand = "WD"
                        };
                        idmaps.Add(_);
                    }
                    if (brand.ToLower().Contains("rothers"))
                    {
                        idmap _ = new()
                        {
                            ProductId = prod,
                            Brand = "BIA"
                        };
                        idmaps.Add(_);
                    }
                    if (brand.ToLower().Contains("for honor"))
                    {
                        idmap _ = new()
                        {
                            ProductId = prod,
                            Brand = "FH"
                        };
                        idmaps.Add(_);
                    }
                    if (brand.ToLower().Contains("epublic"))
                    {
                        idmap _ = new()
                        {
                            ProductId = prod,
                            Brand = "RR"
                        };
                        idmaps.Add(_);
                    }
                    if (brand.ToLower().Contains("ilgrim"))
                    {
                        idmap _ = new()
                        {
                            ProductId = prod,
                            Brand = "ScottPilgrim"
                        };
                        idmaps.Add(_);
                    }
                    if (brand.ToLower().Contains("eyond"))
                    {
                        idmap _ = new()
                        {
                            ProductId = prod,
                            Brand = "BGE"
                        };
                        idmaps.Add(_);
                    }
                    if (brand.ToLower().Contains("nteria"))
                    {
                        idmap _ = new()
                        {
                            ProductId = prod,
                            Brand = "COA"
                        };
                        idmaps.Add(_);
                    }
                    if (brand.ToLower().Contains("ayman"))
                    {
                        idmap _ = new()
                        {
                            ProductId = prod,
                            Brand = "Rayman"
                        };
                        idmaps.Add(_);
                    }
                    if (brand.ToLower().Contains("south"))
                    {
                        idmap _ = new()
                        {
                            ProductId = prod,
                            Brand = "SouthPark"
                        };
                        idmaps.Add(_);
                    }
                    if (brand.ToLower().Contains("undered"))
                    {
                        idmap _ = new()
                        {
                            ProductId = prod,
                            Brand = "SD"
                        };
                        idmaps.Add(_);
                    }
                    if (brand.ToLower().Contains("from dust"))
                    {
                        idmap _ = new()
                        {
                            ProductId = prod,
                            Brand = "FD"
                        };
                        idmaps.Add(_);
                    }
                    if (brand.ToLower().Contains("settlers"))
                    {
                        idmap _ = new()
                        {
                            ProductId = prod,
                            Brand = "TS"
                        };
                        idmaps.Add(_);
                    }
                    if (brand.ToLower().Contains("mmortals"))
                    {
                        idmap _ = new()
                        {
                            ProductId = prod,
                            Brand = "IFR"
                        };
                        idmaps.Add(_);
                    }
                    if (brand.ToLower().Contains("hampions"))
                    {
                        idmap _ = new()
                        {
                            ProductId = prod,
                            Brand = "RC"
                        };
                        idmaps.Add(_);
                    }
                    if (brand.ToLower().Contains("fear"))
                    {
                        idmap _ = new()
                        {
                            ProductId = prod,
                            Brand = "CF"
                        };
                        idmaps.Add(_);
                    }
                    if (brand.ToLower().Contains("monopoly"))
                    {
                        idmap _ = new()
                        {
                            ProductId = prod,
                            Brand = "Monopoly"
                        };
                        idmaps.Add(_);
                    }
                }
            }
            return idmaps;

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
    }
}
