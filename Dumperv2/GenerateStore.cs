using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UbiServices;
using static Dumperv2.Jsons;

namespace Dumperv2
{
    internal class GenerateStore
    {
        public static void Work(bool onlyEng = false)
        {
            List<storeconf> storeconf2 = new();

            if (File.Exists("storeref.json"))
            {
                storeconf2 = JsonConvert.DeserializeObject<List<storeconf>>(File.ReadAllText("storeref.json"))!;
                Console.WriteLine(storeconf2.Count);
            }
            List<string> brands = new() { "Anno", "AC", "Brawlhalla", "BGE", "BIA", "CF", "COL", "COA", "FB", "FC", "FH", "FD", "GR", "IFR", "Monopoly", "Others", "Rayman", "RR", "RC", "RS", "SD", "SouthPark", "ScottPilgrim", "Steep", "Starlink", "Trackmania", "Trials", "TS", "TD", "TCE", "TC", "UNO", "WD" };

            if (!Directory.Exists("Store"))
            {
                Directory.CreateDirectory("Store");
            }

            foreach (var brand in brands)
            {
                if (!Directory.Exists(Path.Combine("Store", brand)))
                {
                    Directory.CreateDirectory(Path.Combine("Store", brand));
                }
            }

            List<idmap> idmaps = new();

            if (File.Exists("idmap.json"))
            {
                idmaps = JsonConvert.DeserializeObject<List<idmap>>(File.ReadAllText("idmap.json"))!;
            }

            List<string> allId = new();
            foreach (var conf in storeconf2)
            {
                if (onlyEng)
                {
                    var country = Enums.CountryCode.eu;
                    Thread.Sleep(10);

                    if (conf.StoreRef.Contains("/") || conf.StoreRef.Contains("\\"))
                    {
                        continue;
                    }

                    var callback = UbiServices.Store.Products.GetStoreFrontByProducts(country, new() { conf.StoreRef }, new() { "images", "variations", "prices", "promotions", "availability" }, false);

                    string file_path = Path.Combine("Store", $"{conf.ProductId}_{country.ToString()}.json");

                    File.WriteAllText(file_path, JsonConvert.SerializeObject(callback, Formatting.Indented));

                    if (File.Exists(file_path))
                    {
                        var file = File.ReadAllText(file_path, encoding: System.Text.Encoding.UTF8);

                        if (file == "null")
                        {
                            goto MOVE;
                        }

                        var thing = JObject.Parse(file);
                        if (thing == null)
                            continue;
                        if (!thing.ContainsKey("count"))
                            continue;
                        var c = (int)thing["count"]!;

                        if (c == 0)
                        {
                            goto MOVE;
                        }
                        if (!thing.ContainsKey("data"))
                            continue;
                        JArray resultArray = (JArray)thing["data"]!;
                        var bidata = JObject.FromObject(resultArray[0]);
                        var brand = bidata["c_productBrandDisplayString"];
                        var subbrand = bidata["c_productSubBrandString"];
                        var id = bidata["id"]!;
                        var EditionsListString = (JArray)bidata["c_productOtherEditionsListString"]!;

                        if (!allId.Contains((string)id!))
                        {
                            allId.Add((string)id!);
                        }

                        if (EditionsListString != null)
                        {
                            foreach (var edition in EditionsListString)
                            {
                                if (!allId.Contains((string)edition!))
                                {
                                    allId.Add((string)edition!);
                                }

                            }
                        }


                        if (!string.IsNullOrEmpty((string)brand!) && !string.IsNullOrEmpty((string)subbrand!))
                        {
                            var sid = (string)subbrand!;
                            idmaps = containsBrand(conf.ProductId, sid, idmaps, brands);
                        }
                        else if (!string.IsNullOrEmpty((string)brand!))
                        {
                            var sid = (string)brand!;
                            idmaps = containsBrand(conf.ProductId, sid, idmaps, brands);
                        }
                        else if (!string.IsNullOrEmpty((string)subbrand!))
                        {
                            var sid = (string)subbrand!;
                            idmaps = containsBrand(conf.ProductId, sid, idmaps, brands);
                        }

                    MOVE:
                        var idmap_2 = idmaps.FirstOrDefault(x => x.ProductId == conf.ProductId);
                        var sermap = JsonConvert.SerializeObject(idmap_2);
                        Console.WriteLine($"{conf.ProductId} ({country.ToString()}) - ({conf.StoreRef})");
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
                            File.Copy(file_path, Path.Combine("Store", "Others", $"{conf.ProductId}_{country.ToString()}.json"), true);

                        }
                        else
                        {
                            File.Copy(file_path, Path.Combine("Store", idmap_2!.Brand, $"{conf.ProductId}_{country.ToString()}.json"), true);
                        }
                        File.Delete(file_path);
                    }
                }
                else
                {
                    for (int i = 0; i <= 18; i++)
                    {
                        var country = (Enums.CountryCode)i;
                        Thread.Sleep(10);

                        if (conf.StoreRef.Contains("/") || conf.StoreRef.Contains("\\"))
                        {
                            continue;
                        }

                        string file_path = Path.Combine("Store", $"{conf.ProductId}_{country.ToString()}.json");
                        var callback = UbiServices.Store.Products.GetStoreFrontByProducts(country, new() { conf.StoreRef }, new() { "images", "variations", "prices", "promotions", "availability" }, false);

                        File.WriteAllText(file_path, JsonConvert.SerializeObject(callback, Formatting.Indented));

                        if (File.Exists(file_path))
                        {
                            var file = File.ReadAllText(file_path, encoding: System.Text.Encoding.UTF8);

                            if (file == "null")
                            {
                                goto MOVE;
                            }

                            var thing = JObject.Parse(file);
                            var c = (int)thing["count"]!;

                            if (c == 0)
                            {
                                goto MOVE;
                            }

                            JArray resultArray = (JArray)thing["data"]!;
                            var bidata = JObject.FromObject(resultArray[0]);
                            var brand = bidata["c_productBrandDisplayString"];
                            var subbrand = bidata["c_productSubBrandString"];
                            var id = bidata["id"];
                            var EditionsListString = (JArray)bidata["c_productOtherEditionsListString"]!;

                            if (!allId.Contains((string)id!))
                            {
                                allId.Add((string)id!);
                            }

                            if (EditionsListString != null)
                            {
                                foreach (var edition in EditionsListString)
                                {
                                    if (!allId.Contains((string)edition!))
                                    {
                                        allId.Add((string)edition!);
                                    }

                                }
                            }


                            if (!string.IsNullOrEmpty((string)brand!) && !string.IsNullOrEmpty((string)subbrand!))
                            {
                                var sid = (string)subbrand!;
                                idmaps = containsBrand(conf.ProductId, sid, idmaps, brands);
                            }
                            else if (!string.IsNullOrEmpty((string)brand!))
                            {
                                var sid = (string)brand!;
                                idmaps = containsBrand(conf.ProductId, sid, idmaps, brands);
                            }
                            else if (!string.IsNullOrEmpty((string)subbrand!))
                            {
                                var sid = (string)subbrand!;
                                idmaps = containsBrand(conf.ProductId, sid, idmaps, brands);
                            }

                        MOVE:
                            var idmap_2 = idmaps.FirstOrDefault(x => x.ProductId == conf.ProductId);
                            var sermap = JsonConvert.SerializeObject(idmap_2);
                            Console.WriteLine($"{conf.ProductId} ({country.ToString()}) - ({conf.StoreRef})");
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
                                File.Copy(file_path, Path.Combine("Store", "Others", $"{conf.ProductId}_{country.ToString()}.json"), true);

                            }
                            else
                            {
                                File.Copy(file_path, Path.Combine("Store", idmap_2!.Brand, $"{conf.ProductId}_{country.ToString()}.json"), true);
                            }
                            File.Delete(file_path);
                        }
                    }
                }               
            }
            allId.Sort();
            File.WriteAllText("idmap.json", JsonConvert.SerializeObject(idmaps, Formatting.Indented));
            File.WriteAllText("allId.txt", String.Join("\n", allId));
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
    }
}
