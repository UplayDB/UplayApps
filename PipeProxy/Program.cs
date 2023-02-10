using System.IO.Pipes;

namespace PipeProxxy
{
    internal class Program
    {
        public static Dictionary<string, NamedPipeServerStream> NameServer = new();
        public static Dictionary<string, NamedPipeClientStream> NameClient = new();
        public static Dictionary<string, bool> NameServerBool = new();
        public static Dictionary<string, bool> NameClientBool = new();
        static void Main(string[] args)
        {
            Console.WriteLine("Hello Reverser!");

            string PipeName = "\\terminal_1_uplay_ipc_pipe_";
            if (args.Length > 0)
            {
                // .exe \terminal_1_uplay_ipc_pipe_ =>
                PipeName = args[0];
            }


            var serv = new Thread(NamedServerPipeStart);
            serv.Start(PipeName);

            Console.ReadLine();

            NamedServerPipeStop(PipeName);
            Console.ReadLine();

        }

        private static void NamedClientPipeStart(object? obj)
        {
            NamedClientPipeStart(obj.ToString());
        }

        private static void NamedServerPipeStart(object? obj)
        {
            NamedServerPipeStart(obj.ToString());
        }

        static void NamedServerPipeStart(string pipename)
        {
            Console.WriteLine("[Server] " + pipename);
            NameServerBool.Add(pipename, true);
            var pipeServer = new NamedPipeServerStream(pipename, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.WriteThrough);
            NameServer.Add(pipename, pipeServer);
            bool connectedOrWaiting = false;
            
            Byte[] buffer = new Byte[65535];
            Console.WriteLine("[Server] Starting..");
            while (NameServerBool[pipename])
            {
                if (!connectedOrWaiting)
                {
                    pipeServer.BeginWaitForConnection((a) => { pipeServer.EndWaitForConnection(a); }, null);

                    connectedOrWaiting = true;
                }

                if (pipeServer.IsConnected)
                {
                    Console.WriteLine("[Server] IsConnected!");
                    Int32 count = pipeServer.Read(buffer, 0, 65535);
                    //Console.WriteLine(pipeServer.GetImpersonationUserName());
                    if (count > 0)
                    {
                        MemoryStream ms = new(count);
                        ms.Write(buffer, 0, count);
                        var BufferDone = ms.ToArray();
                        ms.Dispose();
                        ms.Close();

                        File.WriteAllBytes("x", BufferDone);
                        string ReadedAsCoolBytes = BitConverter.ToString(BufferDone);
                        Console.WriteLine("[Server] Message got readed!\n" + ReadedAsCoolBytes);
                    }
                }
            }
            Console.WriteLine("[Server] Stopping..");
        }

        static void NamedServerPipeStop(string pipename)
        {
            NameServerBool[pipename] = false;
            var server = NameServer[pipename];
            Console.WriteLine("[Server] Disconnect!");
            server.Disconnect();
        }

        static void NamedClientPipeStart(string pipename)
        {
            Console.WriteLine("[Client] " + pipename);
            NameClientBool.Add(pipename, true);
            var pipeClient = new NamedPipeClientStream(".",pipename,PipeDirection.InOut,PipeOptions.CurrentUserOnly,System.Security.Principal.TokenImpersonationLevel.Impersonation);
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