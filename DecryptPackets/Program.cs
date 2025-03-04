using Google.Protobuf;
using UplayKit;

namespace DecryptPackets
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            if (args.Contains("-yml"))
            {
                var ret = args.Where(x => x.Contains("-yml")).Select((x, y) => y).FirstOrDefault();
                string? yml = args.ElementAtOrDefault(ret+1);
                if (yml == null)
                    return;
                YamlHelp.Test(yml);
                return;
            }

            if (!File.Exists("data.bin"))
            {
                Console.WriteLine("data.bin not exist, cannot read from it.");
                return;
            }

            byte[] byteArr = File.ReadAllBytes("data.bin");
            if (args.Contains("-format"))
            {
                if (!File.Exists("decrpyt.txt"))
                {
                    Console.WriteLine("decrpyt.txt not exist, cannot read from it.");
                    return;
                }
                string fromfile = File.ReadAllText("decrpyt.txt");
                ByteString bytestring = ByteString.FromBase64(fromfile);
                byteArr = bytestring.ToArray();
                Console.WriteLine("Reading done");
            }
            if (args.Contains("-format-write"))
            {
                if (!File.Exists("decrpyt.txt"))
                {
                    Console.WriteLine("decrpyt.txt not exist, cannot read from it.");
                    return;
                }
                string fromfile = File.ReadAllText("decrpyt.txt");
                ByteString bytestring = ByteString.FromBase64(fromfile);
                var todecrypt = bytestring.ToArray();
                var len = BitConverter.ToUInt32(todecrypt.Take(4).ToArray());
                File.WriteAllText("len.txt", "len: " + len);
                byteArr = todecrypt.Skip(4).ToArray();
                Console.WriteLine("Reading done");
            }
            if (args.Contains("-force"))
            {
                Force(byteArr);
                return;
            }

            Console.WriteLine("Proto types:\ndemux,channel,client,cloudsave,control,crash,denuvo,download,friends,gamestarter,overlay,ownership,party,playtime,bang,steam,store,uplay,uplayservice,utility,wegame");

            /*
            //0a-06-08-01-12-02-08-01
            string text = Console.ReadLine();
            byte[] byteArr = text.Split('-').Select(p => byte.Parse(p,System.Globalization.NumberStyles.HexNumber)).ToArray();
            */
            Console.WriteLine("Bytes: " + BitConverter.ToString(byteArr));
            Console.WriteLine("Proto:");
            string packetname = Console.ReadLine()!;
            Console.WriteLine("stream type: (up/down)");
            string up_or_down = Console.ReadLine()!;
            Console.WriteLine("push/rsp/req/event:");
            string push_r = Console.ReadLine()!;
            string WriteOut = "";
            switch (packetname)
            {
                #region DEMUX
                case "demux":
                    switch (up_or_down)
                    {
                        case "up":
                            switch (push_r)
                            {
                                case "req":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.Demux.Upstream.Parser.ParseFrom(byteArr).Request;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                        if (!demux_down_rsp.ServiceRequest.Data.IsEmpty)
                                        {
                                            byte[] byarra = demux_down_rsp.ServiceRequest.Data.ToArray();
                                            Console.WriteLine("Bytes: " + BitConverter.ToString(byarra));
                                        }
                                    }
                                    catch { }
                                    break;
                                case "push":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.Demux.Upstream.Parser.ParseFrom(byteArr).Push;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                        if (!demux_down_rsp.Data.Data.IsEmpty)
                                        {
                                            byte[] byarra = demux_down_rsp.Data.Data.ToArray();
                                            Console.WriteLine("Bytes: " + BitConverter.ToString(byarra));
                                        }
                                    }
                                    catch { }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "down":
                            switch (push_r)
                            {
                                case "rsp":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.Demux.Downstream.Parser.ParseFrom(byteArr).Response;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                        if (!demux_down_rsp.ServiceRsp.Data.IsEmpty)
                                        {
                                            byte[] byarra = demux_down_rsp.ServiceRsp.Data.ToArray();
                                            Console.WriteLine("Bytes: " + BitConverter.ToString(byarra));
                                        }
                                    }
                                    catch { }
                                    break;
                                case "push":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.Demux.Downstream.Parser.ParseFrom(byteArr).Push;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                        if (!demux_down_rsp.Data.Data.IsEmpty)
                                        {
                                            byte[] byarra = demux_down_rsp.Data.Data.ToArray();
                                            Console.WriteLine("Bytes: " + BitConverter.ToString(byarra));
                                        }
                                    }
                                    catch (Exception ex) { File.WriteAllText("ex.txt",ex.ToString()); }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                #endregion
                #region Channel
                case "channel":
                    switch (up_or_down)
                    {
                        case "up":
                            switch (push_r)
                            {
                                case "req":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.Channel.Upstream.Parser.ParseFrom(byteArr).Req;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "down":
                            switch (push_r)
                            {
                                case "rsp":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.Channel.Downstream.Parser.ParseFrom(byteArr).Rsp;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                case "event":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.Channel.Downstream.Parser.ParseFrom(byteArr).Event;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                #endregion
                #region ClientConfiguration
                case "client":
                    switch (up_or_down)
                    {
                        case "up":
                            switch (push_r)
                            {
                                case "req":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.ClientConfiguration.Upstream.Parser.ParseFrom(byteArr).Request;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "down":
                            switch (push_r)
                            {
                                case "rsp":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.ClientConfiguration.Downstream.Parser.ParseFrom(byteArr).Response;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                #endregion
                #region CloudsaveService
                case "cloudsave":
                    switch (up_or_down)
                    {
                        case "up":
                            switch (push_r)
                            {
                                case "req":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.CloudsaveService.Upstream.Parser.ParseFrom(byteArr).Request;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "down":
                            switch (push_r)
                            {
                                case "rsp":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.CloudsaveService.Downstream.Parser.ParseFrom(byteArr).Response;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                #endregion
                #region CrashReporter
                case "crash":
                    switch (up_or_down)
                    {
                        case "up":
                            switch (push_r)
                            {
                                case "req":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.CrashReporter.Upstream.Parser.ParseFrom(byteArr).Req;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "down":
                            switch (push_r)
                            {
                                case "rsp":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.CrashReporter.Downstream.Parser.ParseFrom(byteArr).Rsp;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                #endregion
                #region DenuvoService
                case "denuvo":
                    switch (up_or_down)
                    {
                        case "up":
                            switch (push_r)
                            {
                                case "req":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.DenuvoService.Upstream.Parser.ParseFrom(byteArr).Request;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "down":
                            switch (push_r)
                            {
                                case "rsp":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.DenuvoService.Downstream.Parser.ParseFrom(byteArr).Response;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                #endregion
                #region DownloadService
                case "download":
                    switch (up_or_down)
                    {
                        case "up":
                            switch (push_r)
                            {
                                case "req":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.DownloadService.Upstream.Parser.ParseFrom(byteArr).Request;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "down":
                            switch (push_r)
                            {
                                case "rsp":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.DownloadService.Downstream.Parser.ParseFrom(byteArr).Response;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                #endregion
                #region Friends
                case "friends":
                    switch (up_or_down)
                    {
                        case "up":
                            switch (push_r)
                            {
                                case "req":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.Friends.Upstream.Parser.ParseFrom(byteArr).Request;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "down":
                            switch (push_r)
                            {
                                case "rsp":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.Friends.Downstream.Parser.ParseFrom(byteArr).Response;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                case "push":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.Friends.Downstream.Parser.ParseFrom(byteArr).Push;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                #endregion
                #region GameStarter
                case "gamestarter":
                    switch (up_or_down)
                    {
                        case "up":
                            switch (push_r)
                            {
                                case "req":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.GameStarter.Upstream.Parser.ParseFrom(byteArr).Req;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "down":
                            switch (push_r)
                            {
                                case "rsp":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.GameStarter.Downstream.Parser.ParseFrom(byteArr).Rsp;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                case "push":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.GameStarter.Downstream.Parser.ParseFrom(byteArr).Push;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                #endregion
                #region Overlay
                case "overlay":
                    switch (up_or_down)
                    {
                        case "up":
                            switch (push_r)
                            {
                                case "req":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.Overlay.Upstream.Parser.ParseFrom(byteArr).Req;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "down":
                            switch (push_r)
                            {
                                case "rsp":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.Overlay.Downstream.Parser.ParseFrom(byteArr).Rsp;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                case "push":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.Overlay.Downstream.Parser.ParseFrom(byteArr).Push;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                #endregion
                #region Ownership
                case "ownership":
                    switch (up_or_down)
                    {
                        case "up":
                            switch (push_r)
                            {
                                case "req":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.Ownership.Upstream.Parser.ParseFrom(byteArr).Request;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "down":
                            switch (push_r)
                            {
                                case "rsp":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.Ownership.Downstream.Parser.ParseFrom(byteArr).Response;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                case "push":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.Ownership.Downstream.Parser.ParseFrom(byteArr).Push;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                default:
                                    try
                                    {
                                        var demux_down = Uplay.Ownership.Downstream.Parser.ParseFrom(byteArr);
                                        Console.WriteLine(demux_down);
                                        WriteOut = demux_down.ToString();
                                    }
                                    catch { }
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                #endregion
                #region Party
                case "party":
                    switch (up_or_down)
                    {
                        case "up":
                            switch (push_r)
                            {
                                case "req":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.Party.Upstream.Parser.ParseFrom(byteArr).Request;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "down":
                            switch (push_r)
                            {
                                case "rsp":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.Party.Downstream.Parser.ParseFrom(byteArr).Response;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                case "push":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.Party.Downstream.Parser.ParseFrom(byteArr).Push;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                #endregion
                #region Playtime
                case "playtime":
                    switch (up_or_down)
                    {
                        case "up":
                            switch (push_r)
                            {
                                case "req":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.Playtime.Upstream.Parser.ParseFrom(byteArr).Request;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "down":
                            switch (push_r)
                            {
                                case "rsp":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.Playtime.Downstream.Parser.ParseFrom(byteArr).Response;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                #endregion
                #region Pcbang
                case "bang":
                    switch (up_or_down)
                    {
                        case "up":
                            switch (push_r)
                            {
                                case "req":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.Pcbang.Upstream.Parser.ParseFrom(byteArr).Request;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "down":
                            switch (push_r)
                            {
                                case "rsp":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.Pcbang.Downstream.Parser.ParseFrom(byteArr).Response;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                #endregion
                #region SteamService
                case "steam":
                    switch (up_or_down)
                    {
                        case "up":
                            switch (push_r)
                            {
                                case "req":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.SteamService.Upstream.Parser.ParseFrom(byteArr).Request;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "down":
                            switch (push_r)
                            {
                                case "rsp":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.SteamService.Downstream.Parser.ParseFrom(byteArr).Response;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                #endregion
                #region Store
                case "store":
                    switch (up_or_down)
                    {
                        case "up":
                            switch (push_r)
                            {
                                case "req":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.Store.Upstream.Parser.ParseFrom(byteArr).Request;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "down":
                            switch (push_r)
                            {
                                case "rsp":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.Store.Downstream.Parser.ParseFrom(byteArr).Response;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                case "push":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.Store.Downstream.Parser.ParseFrom(byteArr).Push;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                #endregion
                #region Uplay
                case "uplay":
                    switch (up_or_down)
                    {
                        case "up":
                            switch (push_r)
                            {
                                case "req":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.Uplay.Req.Parser.ParseFrom(byteArr);
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "down":
                            switch (push_r)
                            {
                                case "rsp":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.Uplay.Rsp.Parser.ParseFrom(byteArr);
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                #endregion
                #region UplayService
                case "uplayservice":
                    switch (up_or_down)
                    {
                        case "up":
                            switch (push_r)
                            {
                                case "req":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.UplayService.Upstream.Parser.ParseFrom(byteArr).V1Req;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "down":
                            switch (push_r)
                            {
                                case "rsp":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.UplayService.Downstream.Parser.ParseFrom(byteArr).V1Rsp;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                case "push":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.UplayService.Downstream.Parser.ParseFrom(byteArr).V1Push;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                #endregion
                #region Utility
                case "utility":
                    switch (up_or_down)
                    {
                        case "up":
                            switch (push_r)
                            {
                                case "req":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.Utility.Upstream.Parser.ParseFrom(byteArr).Request;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "down":
                            switch (push_r)
                            {
                                case "rsp":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.Utility.Downstream.Parser.ParseFrom(byteArr).Response;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                #endregion
                #region WegameService
                case "wegame":
                    switch (up_or_down)
                    {
                        case "up":
                            switch (push_r)
                            {
                                case "req":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.WegameService.Upstream.Parser.ParseFrom(byteArr).Request;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "down":
                            switch (push_r)
                            {
                                case "rsp":
                                    try
                                    {
                                        var demux_down_rsp = Uplay.WegameService.Downstream.Parser.ParseFrom(byteArr).Response;
                                        Console.WriteLine(demux_down_rsp);
                                        WriteOut = demux_down_rsp.ToString();
                                    }
                                    catch { }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                #endregion
                default:
                    break;


            }
            if (WriteOut != "")
            {
                string filename = $"Converted_{packetname}_{up_or_down}_{push_r}";

                if (File.Exists(filename + ".txt"))
                {
                    filename = filename + "_" + RandomString();
                    bool filethigny = false;
                    while (filethigny == false)
                    {
                        filethigny = !File.Exists(filename + ".txt");
                        filename += RandomString();
                    }
                }


                File.WriteAllText($"{filename}.txt", WriteOut + "\n" + BitConverter.ToString(byteArr));
            }

            Console.WriteLine("END");
        }

        public static string RandomString()
        {
            Random res = new();

            // String that contain both alphabets and numbers
            string str = "abcdefghijklmnopqrstuvwxyz0123456789";
            int size = 2;

            // Initializing the empty string
            string randomstring = "";

            for (int i = 0; i < size; i++)
            {

                // Selecting a index randomly
                int x = res.Next(str.Length);

                // Appending the character at the 
                // index to the random alphanumeric string.
                randomstring += str[x];
            }
            return randomstring;
        }

        public static void CheckOutWrite(string towrite)
        {
            if (towrite.Trim() != "{}" || towrite.Trim() != "{ }" || !string.IsNullOrEmpty(towrite.Trim()))
            {
                File.WriteAllText($"SavedDecrypt/{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.txt", towrite);
            }
        }

        public static string Force(byte[] array)
        {
            List<string> str = new();
            string ret = string.Empty;
            try
            {
                str.Add(Uplay.Demux.Downstream.Parser.ParseFrom(array).Response.ToString());
            }
            catch (Exception ex)
            {
                str.Add(ex.ToString());
            }
            try
            {
                Uplay.Demux.Push push = Uplay.Demux.Downstream.Parser.ParseFrom(array).Push;
                str.Add(push.ToString());
                if (!push.Data.Data.IsEmpty)
                {
                    byte[] array2 = push.Data.Data.ToArray();
                    Force(array2.Skip(4).ToArray());
                    ret += "Bytes: " + BitConverter.ToString(array2) + "\n";
                }
            }
            catch (Exception ex)
            {
                str.Add(ex.ToString());
            }
            try
            {
                str.Add(Uplay.Demux.Upstream.Parser.ParseFrom(array).Request.ToString());
            }
            catch (Exception ex)
            {
                str.Add(ex.ToString());
            }
            try
            {
                Uplay.Demux.Push push = Uplay.Demux.Upstream.Parser.ParseFrom(array).Push;
                str.Add(push.ToString());
                if (!push.Data.Data.IsEmpty)
                {
                    byte[] array2 = push.Data.Data.ToArray();
                    Force(array2.Skip(4).ToArray());
                    ret += "Bytes: " + BitConverter.ToString(array2) + "\n";
                }
            }
            catch (Exception ex)
            {
                str.Add(ex.ToString());
            }
            try
            {
                str.Add(Uplay.Channel.Upstream.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }
            try
            {
                str.Add(Uplay.Channel.Downstream.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }

            try
            {
                str.Add(Uplay.ClientConfiguration.Upstream.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }
            try
            {
                str.Add(Uplay.ClientConfiguration.Downstream.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }

            try
            {
                str.Add(Uplay.CloudsaveService.Upstream.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }
            try
            {
                str.Add(Uplay.CloudsaveService.Downstream.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }

            try
            {
                str.Add(Uplay.CrashReporter.Upstream.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }
            try
            {
                str.Add(Uplay.CrashReporter.Downstream.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }

            try
            {
                str.Add(Uplay.DenuvoService.Upstream.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }
            try
            {
                str.Add(Uplay.DenuvoService.Downstream.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }

            try
            {
                str.Add(Uplay.DownloadService.Upstream.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }
            try
            {
                str.Add(Uplay.DownloadService.Downstream.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }

            try
            {
                str.Add(Uplay.Friends.Upstream.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }
            try
            {
                str.Add(Uplay.Friends.Downstream.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }

            try
            {
                str.Add(Uplay.GameCatalogExtension.Upstream.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }
            try
            {
                str.Add(Uplay.GameCatalogExtension.Downstream.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }

            try
            {
                str.Add(Uplay.GameStarter.Upstream.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }
            try
            {
                str.Add(Uplay.GameStarter.Downstream.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }

            try
            {
                str.Add(Uplay.Orbitdll.Req.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }
            try
            {
                str.Add(Uplay.Orbitdll.Rsp.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }

            try
            {
                str.Add(Uplay.Overlay.Upstream.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }
            try
            {
                str.Add(Uplay.Overlay.Downstream.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }

            try
            {
                str.Add(Uplay.Ownership.Upstream.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }
            try
            {
                str.Add(Uplay.Ownership.Downstream.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }

            try
            {
                str.Add(Uplay.Party.Upstream.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }
            try
            {
                str.Add(Uplay.Party.Downstream.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }

            try
            {
                str.Add(Uplay.Pcbang.Upstream.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }
            try
            {
                str.Add(Uplay.Pcbang.Downstream.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }

            try
            {
                str.Add(Uplay.Playtime.Upstream.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }
            try
            {
                str.Add(Uplay.Playtime.Downstream.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }

            try
            {
                str.Add(Uplay.SteamService.Upstream.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }
            try
            {
                str.Add(Uplay.SteamService.Downstream.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }

            try
            {
                str.Add(Uplay.Store.Upstream.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }
            try
            {
                str.Add(Uplay.Store.Downstream.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }

            try
            {
                str.Add(Uplay.Uplay.Req.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }
            try
            {
                str.Add(Uplay.Uplay.Rsp.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }

            try
            {
                str.Add(Uplay.Uplayauxdll.Req.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }
            try
            {
                str.Add(Uplay.Uplayauxdll.Rsp.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }

            try
            {
                str.Add(Uplay.Uplaydll.Req.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }
            try
            {
                str.Add(Uplay.Uplaydll.Rsp.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }

            try
            {
                str.Add(Uplay.Uplayprotocol.Req.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }
            try
            {
                str.Add(Uplay.Uplayprotocol.Rsp.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }

            try
            {
                str.Add(Uplay.UplayService.Upstream.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }
            try
            {
                str.Add(Uplay.UplayService.Downstream.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }

            try
            {
                str.Add(Uplay.Utility.Upstream.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }
            try
            {
                str.Add(Uplay.Utility.Downstream.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }

            try
            {
                str.Add(Uplay.WegameService.Upstream.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }
            try
            {
                str.Add(Uplay.WegameService.Downstream.Parser.ParseFrom(array).ToString());
            }
            catch
            {
            }
            CheckOutWrite(string.Join("\n", str));
            return ret;
        }
    }
}
