using Google.Protobuf;
using PipeProxy;
using System.IO.Pipes;
using System.Reflection.Metadata;

namespace PipeProxxy
{
    internal class Program
    {
        public static Dictionary<string, NamedPipeServerStream> NameServer = new();
        public static Dictionary<string, NamedPipeClientStream> NameClient = new();
        public static Dictionary<string, bool> NameServerBool = new();
        public static Dictionary<string, bool> NameClientBool = new();

        public static Dictionary<string, KillMe> NameKill = new();

        public static List<string> PipeNames = new();
        public static byte[] FormatUpstream(byte[] rawMessage)
        {
            BlobWriter blobWriter = new(4);
            blobWriter.WriteUInt32BE((uint)rawMessage.Length);
            var returner = blobWriter.ToArray().Concat(rawMessage).ToArray();
            blobWriter.Clear();
            return returner;
        }

        static void Main(string[] args)
        {
            PipeNames.Add("\\terminal_1_uplay_service_ipc_pipe_");
            PipeNames.Add("\\terminal_1_uplay_protocol_ipc_pipe_");
            PipeNames.Add("\\terminal_1_uplay_overlay_ipc_pipe_");
            PipeNames.Add("\\terminal_1_uplay_ipc_pipe_");
            PipeNames.Add("\\terminal_1_uplay_crash_reporter_ipc_pipe_");
            PipeNames.Add("\\terminal_1_uplay_aux_ipc_pipe_");
            PipeNames.Add("\\terminal_1_uplay_api_process_ipc_pipe_"); 
            PipeNames.Add("\\terminal_1_orbit_ipc_pipe_");
            PipeNames.Add("\\terminal_1_game_start_ipc_pipe_");
            
            Console.WriteLine("Hello Reverser!");

            if (args.Contains("launch"))
            {
                var serv = new Thread(NamedClientPipeStart);
                serv.Start("\\terminal_1_game_start_ipc_pipe_");

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

                var up = FormatUpstream(post.ToByteArray());
                Console.WriteLine(BitConverter.ToString(up));


                NameClient["\\terminal_1_game_start_ipc_pipe_"].Write(up);

                Console.ReadLine();

                serv.Interrupt();
                NamedClientPipeStop("\\terminal_1_game_start_ipc_pipe_");
            }
            else
            {
                
                foreach (string name in PipeNames)
                {
                    try
                    {
                        if (!name.Contains("process"))
                            continue;

                        var longRunning = new KillMe(name);
                        NameKill.Add(name, longRunning);
                        Thread myThread = new Thread(longRunning.ExecuteLongRunningTask);

                        myThread.Start();
                    }
                    catch
                    { 
                    
                    }

                }

                Console.ReadLine();
                Console.WriteLine("After this we quit!");
                Console.ReadLine();
                PipeNames.ForEach(x => NamedServerPipeStop(x));
            }          
        }

        static void NamedServerPipeStop(string pipename)
        {
            if (NameKill.TryGetValue(pipename, out var pipeServer))
            {
                pipeServer.Cancel = true;
            }
            Console.WriteLine($"[Server | {pipename}] Disconnect!");
        }
        private static void NamedClientPipeStart(object? obj)
        {
            NamedClientPipeStart(obj.ToString());
        }
        static void NamedClientPipeStart(string pipename)
        {
            Console.WriteLine("[Client] " + pipename);
            NameClientBool.Add(pipename, true);
            var pipeClient = new NamedPipeClientStream(".",pipename,PipeDirection.InOut, PipeOptions.Asynchronous);
            NameClient.Add(pipename, pipeClient);
            bool connectedOrWaiting = false;
            Byte[] buffer = new Byte[65535];
            Console.WriteLine("[Client] Starting...");
            while (NameClientBool[pipename])
            {
                if (!connectedOrWaiting)
                {
                    pipeClient.Connect();

                    connectedOrWaiting = true;
                }

                if (pipeClient.IsConnected)
                {
                    Console.WriteLine("[Client] IsConnected!");

                    Int32 count = pipeClient.Read(buffer, 0, 65535);

                    if (count > 0)
                    {
                        MemoryStream ms = new(count);
                        ms.Write(buffer, 0, count);
                        var BufferDone = ms.ToArray();
                        ms.Dispose();
                        ms.Close();

                        File.WriteAllBytes("x", BufferDone);
                        string ReadedAsCoolBytes = BitConverter.ToString(BufferDone);
                        Console.WriteLine("[Client] Message got readed!\n" + ReadedAsCoolBytes);
                    }
                }
            }
            Console.WriteLine("[Client] Stopping..");
        }

        static void NamedClientPipeStop(string pipename)
        {
            NameClientBool[pipename] = false;
            var client = NameClient[pipename];
            Console.WriteLine("[Client] Disconnect!");
            client.Dispose();
            client.Close();
        }
    }
}